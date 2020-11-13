using PriceTracker.Models;
using System;
using System.Collections.Generic;
using System.Timers;
using PriceTracker.Repositories;
using PriceTracker.Consts;
using PriceTracker.Clients;
using PriceTracker.Helpers;

namespace PriceTracker.Services
{
    public class TimerService : ITimerService
    {
        private readonly static int maxUpdateTimeIntervalHours = 24;
        private const string UNKNOWN_SHOP_EXCEPTION = "Item could not be added for tracking. Unknown shop.";
        private const string REMOVE_FROM_QUEUE_EXCEPTION = "Item could not be removed from queue.";
        private readonly string CANT_UPDATE_EXCEPTION = $"Item info was not updated for {maxUpdateTimeIntervalHours} hours.";

        private Queue<Item> itemsQueue;
        private Timer timer;

        private readonly ITrackingRepository _trackingRepository;
        private readonly IPullAndBearClient _pullAndBearClient;
        private readonly IBershkaClient _bershkaClient;
        private readonly IBotService _botService;
        private readonly ITextConverter _textConverter;
        private readonly IUpdateInfoHelper _updateInfoHelper;

        public TimerService(
            ITrackingRepository trackingRepository,
            IPullAndBearClient pullAndBearClient,
            IBershkaClient bershkaClient,
            IBotService botService,
            ITextConverter textConverter,
            IUpdateInfoHelper updateInfoHelper)
        {
            _trackingRepository = trackingRepository ?? throw new ArgumentNullException(nameof(trackingRepository));
            _pullAndBearClient = pullAndBearClient ?? throw new ArgumentNullException(nameof(pullAndBearClient));
            _bershkaClient = bershkaClient ?? throw new ArgumentNullException(nameof(bershkaClient));
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _textConverter = textConverter ?? throw new ArgumentNullException(nameof(textConverter));
            _updateInfoHelper = updateInfoHelper ?? throw new ArgumentNullException(nameof(updateInfoHelper));

            List<Item> existedItems = _trackingRepository.GetItemsAsync().Result;
            itemsQueue = existedItems?.Count > 0 ? new Queue<Item>(existedItems) : new Queue<Item>();

            timer = new Timer(10000); // 10 sec
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public void Enqueue(Item newItem) =>
            itemsQueue.Enqueue(newItem);

        public void RemoveFromQueue(string name)
        {
            var removedFromQueue = false;
            for (int i = 0; i < itemsQueue.Count; i++)
            {
                var item = itemsQueue.Dequeue();
                if (item.Name.Equals(name))
                {
                    removedFromQueue = true;
                    break;
                }
                itemsQueue.Enqueue(item);
            }
            if (!removedFromQueue)
                throw new Exception(REMOVE_FROM_QUEUE_EXCEPTION);
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (itemsQueue.Count == 0)
                return;

            var item = itemsQueue.Dequeue();
            try
            {
                var newInfo = item.Source switch
                {
                    PullAndBear.SHOP_NAME => await _pullAndBearClient.GetItemInfoAsync(item.Url),
                    Bershka.SHOP_NAME => await _bershkaClient.GetItemInfoAsync(item.Url),
                    _ => throw new Exception(UNKNOWN_SHOP_EXCEPTION),
                };
                if (item.Status != newInfo.Status || item.Price != newInfo.Price)
                    await _botService.SendMessageMarkdownV2Async(
                        item.ChatId,
                        _textConverter.ToString(item, newInfo));
                _updateInfoHelper.GetUpdatedItem(ref item, newInfo);
                await _trackingRepository.UpdateInfoOfItemAsync(item);
                
            }
            catch (Exception ex)
            {
                // it's ok; lets update item information next time;
            }
            itemsQueue.Enqueue(item);
            if ((DateTime.Now - item.LastUpdateDate).TotalHours > maxUpdateTimeIntervalHours)
                await _botService.SendMessageAsync(
                    item.ChatId,
                    CANT_UPDATE_EXCEPTION);
        }
    }
}
