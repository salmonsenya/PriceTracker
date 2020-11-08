using AutoMapper;
using PriceTracker.Clients;
using PriceTracker.Helpers;
using PriceTracker.Models;
using PriceTracker.Repositories;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot.Types;

namespace PriceTracker.Services
{
    public class PullAndBearService : IPullAndBearService
    {
        private readonly IBotService _botService;
        private readonly ITrackingRepository _trackingRepository;
        private readonly IPullAndBearClient _pullAndBearClient;
        private readonly IMapper _mapper;
        private readonly IUpdateInfoHelper _updateInfoHelper;

        private Queue<Item> itemsQueue;
        private Timer timer;

        public PullAndBearService(IBotService botService, ITrackingRepository trackingRepository, IPullAndBearClient pullAndBearClient, IMapper mapper, IUpdateInfoHelper updateInfoHelper)
        {
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _trackingRepository = trackingRepository ?? throw new ArgumentNullException(nameof(trackingRepository));
            _pullAndBearClient = pullAndBearClient ?? throw new ArgumentNullException(nameof(pullAndBearClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _updateInfoHelper = updateInfoHelper ?? throw new ArgumentNullException(nameof(updateInfoHelper));

            List<Item> existedItems = _trackingRepository.GetItemsAsync().Result;
            itemsQueue = existedItems?.Count > 0 ? new Queue<Item>(existedItems) : new Queue<Item>();

            timer = new Timer(5000); // 5 sec
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public async Task<List<Item>> GetTrackedItemsAsync() =>
            await _trackingRepository.GetItemsAsync();

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (itemsQueue.Count > 0)
            {
                var item = itemsQueue.Dequeue();
                try
                {
                    var newInfo = await _pullAndBearClient.GetItemInfoAsync(item.Url);
                    if (newInfo != null)
                    {
                        await _trackingRepository.UpdateInfoOfItemAsync(item.ItemId, newInfo);
                        if (item.Status != newInfo.Status || item.Price != newInfo.Price)
                        {
                            await _botService.SendMessageMarkdownV2(
                                item.ChatId,
                                $@"
Item price has been changed.
*{item.Name}*
Previous: {item.Price} {item.PriceCurrency}
Current: {newInfo.Price} {newInfo.PriceCurrency}
[View on site]({item.Url})
");
                        }
                        item = _updateInfoHelper.GetUpdatedItem(item, newInfo);
                    }
                }
                catch (Exception ex) { }
                itemsQueue.Enqueue(item);
            }
        }

        public async Task AddNewItemAsync(Message message)
        {
            var input = message.Text;
            var url = Regex.Match(input, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?").Value;
            if (string.IsNullOrEmpty(url))
            {
                await _botService.SendMessage(
                    message.Chat.Id,
                    message.MessageId,
                    "Insert a link of item you want to add for tracking after command /add + space.");
                return;
            }

            if (await _trackingRepository.IsTracked(url, message.From.Id))
            {
                await _botService.SendMessage(
                    message.Chat.Id,
                    message.MessageId,
                    "Item is already added for tracking.");
            }
            else
            {
                var newInfo = await _pullAndBearClient.GetItemInfoAsync(url);
                var newItem = _mapper.Map<ItemOnline, Item>(newInfo);
                newItem.Url = url;
                newItem.StartTrackingDate = DateTime.Now;
                newItem.Source = "Pull&Bear";
                newItem.ChatId = message.Chat.Id;
                newItem.UserId = message.From.Id;



                var itemId = await _trackingRepository.AddNewItemAsync(newItem);
                newItem.ItemId = itemId;
                itemsQueue.Enqueue(newItem);
                await _botService.SendMessageButtonMarkdownV2(
                    message.Chat.Id,
                    message.MessageId,
                    $@"
*{newItem.Name}*
Current: {newItem.Price} {newItem.PriceCurrency}
[View on site]({newItem.Url})
");
            }
        }

        public async Task RemoveItemAsync(Message itemMessage)
        {
            var itemText = itemMessage.Text;
            var firstLine = itemText.Substring(0, itemText.IndexOf(Environment.NewLine));

            if (string.IsNullOrEmpty(firstLine))
            {
                await _botService.SendMessage(
                    itemMessage.Chat.Id,
                    itemMessage.MessageId,
                    "Name of item u want to delete wasn't found.");
                return;
            }
            // remove from queue
            var removedFromQueue = false;
            for (int i = 0; i < itemsQueue.Count; i++)
            {
                var item = itemsQueue.Dequeue();
                if (item.Name.Equals(firstLine))
                {
                    removedFromQueue = true;
                    break;
                }
                itemsQueue.Enqueue(item);
            }
            if (!removedFromQueue)
            {
                await _botService.SendMessage(
                    itemMessage.Chat.Id,
                    itemMessage.MessageId,
                    "Item could not be removed from queue.");
                return;
            }
            await _botService.SendMessage(
                itemMessage.Chat.Id,
                itemMessage.MessageId,
                "Item was removed from queue.");

            // remove from db
            try
            {
                await _trackingRepository.RemoveItem(firstLine);
            } catch (Exception ex)
            {
                await _botService.SendMessage(
                    itemMessage.Chat.Id,
                    itemMessage.MessageId,
                    $@"{ex.Message}");
            }
            await _botService.SendMessage(
                itemMessage.Chat.Id,
                itemMessage.MessageId,
                "Item was removed from DB.");
        }
    }
}
