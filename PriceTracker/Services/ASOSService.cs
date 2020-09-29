using PriceTracker.Clients;
using PriceTracker.Models;
using PriceTracker.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot.Types;

namespace PriceTracker.Services
{
    public class ASOSService : IASOSService
    {
        private readonly IBotService _botService;
        private readonly ITrackingRepository _trackingRepository;
        private readonly IASOSClient _asosClient;

        private Queue<Item> itemsQueue;
        private Timer timer;

        public ASOSService(IBotService botService, ITrackingRepository trackingRepository, IASOSClient asosClient)
        {
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _trackingRepository = trackingRepository ?? throw new ArgumentNullException(nameof(trackingRepository));
            _asosClient = asosClient ?? throw new ArgumentNullException(nameof(asosClient));

            itemsQueue = new Queue<Item>();
            timer = new Timer(2000); // 2 sec
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var item = itemsQueue.Dequeue();
            var result =  _asosClient.GetItemInfoAsync(item.Url).Result;
            _trackingRepository.UpdateInfoOfItemAsync(item.ItemId, result.Status, result.Price);
            itemsQueue.Enqueue(item);
        }

        public async Task AddNewItemAsync(Message message)
        {
            var input = message.Text;
            var url = "";

            if (await _trackingRepository.IsTracked(url))
            {
                await _botService.Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    replyToMessageId: message.MessageId,
                    text: "Item is already added for tracking.");
            }
            else
            {
                var newItem = new Item()
                {
                    Url = url,
                    Status = null,
                    Price = null,
                    SatrtTrackingDate = DateTime.Now
                };
                itemsQueue.Enqueue(newItem);
                await _trackingRepository.AddNewItemAsync(newItem);
                await _botService.Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    replyToMessageId: message.MessageId,
                    text: "Item was added for tracking.");
            }
        }
    }
}
