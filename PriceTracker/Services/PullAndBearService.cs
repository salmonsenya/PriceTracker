using PriceTracker.Clients;
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

        private Queue<Item> itemsQueue;
        private Timer timer;

        public PullAndBearService(IBotService botService, ITrackingRepository trackingRepository, IPullAndBearClient asosClient)
        {
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _trackingRepository = trackingRepository ?? throw new ArgumentNullException(nameof(trackingRepository));
            _pullAndBearClient = asosClient ?? throw new ArgumentNullException(nameof(asosClient));
            SetQueueAndTimer();
        }

        private void SetQueueAndTimer()
        {
            itemsQueue = new Queue<Item>();
            timer = new Timer(2000); // 4 sec
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (itemsQueue.Count > 0)
            {
                var item = itemsQueue.Dequeue();
                var newInfo = _pullAndBearClient.GetItemInfoAsync(item.ItemId, item.Url).Result;
                var newItem = _trackingRepository.UpdateInfoOfItemAsync(item.ItemId, newInfo.Status, newInfo.Price, newInfo.PriceCurrency).Result;
                if (item.Status != newInfo.Status || item.Price != newInfo.Price)
                {
                    /*_botService.Client.SendTextMessageAsync(
                        chatId: item.ChatId, 
                        text: "Item has been changed.");*/
                }
                item.Status = newInfo.Status;
                item.Price = newInfo.Price;
                item.PriceCurrency = newInfo.PriceCurrency;
                itemsQueue.Enqueue(item);
            }
        }

        public async Task AddNewItemAsync(Message message)
        {
            var existedItems = _trackingRepository.GetItems();
            itemsQueue = existedItems.Count > 0 ? new Queue<Item>(existedItems) : new Queue<Item>();

            var input = message.Text;
            var url = Regex.Match(input, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?").Value;

            if (await _trackingRepository.IsTracked(url))
            {
                /*await _botService.Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    replyToMessageId: message.MessageId,
                    text: "Item is already added for tracking.");*/
            }
            else
            {
                var newItem = new Item()
                {
                    Url = url,
                    Status = null,
                    Price = null,
                    StartTrackingDate = DateTime.Now,
                    Source = "Pull&Bear",
                    ChatId = message.Chat.Id,
                };

                var itemId = await _trackingRepository.AddNewItemAsync(newItem);
                newItem.ItemId = itemId;
                itemsQueue.Enqueue(newItem);
                

                /*await _botService.Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    replyToMessageId: message.MessageId,
                    text: "Item was added for tracking.");*/
            }
        }
    }
}
