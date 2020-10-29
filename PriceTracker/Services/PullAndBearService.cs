using PriceTracker.Clients;
using PriceTracker.Models;
using PriceTracker.Repositories;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PriceTracker.Services
{
    public class PullAndBearService : IPullAndBearService
    {
        private readonly IBotService _botService;
        private readonly ITrackingRepository _trackingRepository;
        private readonly IPullAndBearClient _pullAndBearClient;

        private Queue<Item> itemsQueue;
        private Timer timer;

        public PullAndBearService(IBotService botService, ITrackingRepository trackingRepository, IPullAndBearClient pullAndBearClient)
        {
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _trackingRepository = trackingRepository ?? throw new ArgumentNullException(nameof(trackingRepository));
            _pullAndBearClient = pullAndBearClient ?? throw new ArgumentNullException(nameof(pullAndBearClient));

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
                var newInfo = await _pullAndBearClient.GetItemInfoAsync(item.ItemId, item.Url);
                await _trackingRepository.UpdateInfoOfItemAsync(item.ItemId, newInfo.Status, newInfo.Price, newInfo.PriceCurrency, newInfo.Name, newInfo.Image);
                if (item.Status != newInfo.Status || item.Price != newInfo.Price)
                {
                    try
                    {
                        await _botService.Client.SendTextMessageAsync(
                            chatId: item.ChatId,                         
                            parseMode: ParseMode.MarkdownV2,
                            disableWebPagePreview: false,
                            text: $@"
Item price has been changed.
*{item.Name}*
Previous: {item.Price} {item.PriceCurrency}
Current: {newInfo.Price} {newInfo.PriceCurrency}
[View on site]({item.Url})
");
                    } catch (Exception ex){}
                }
                item.Status = newInfo.Status;
                item.Price = newInfo.Price;
                item.PriceCurrency = newInfo.PriceCurrency;
                itemsQueue.Enqueue(item);
            }
        }

        public async Task AddNewItemAsync(Message message)
        {
            var input = message.Text;
            var url = Regex.Match(input, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?").Value;

            if (await _trackingRepository.IsTracked(url, message.From.Id))
            {
                try
                {
                    await _botService.Client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        replyToMessageId: message.MessageId,
                        text: "Item is already added for tracking.");
                }
                catch (Exception ex){}
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
                    UserId = message.From.Id
                };

                var itemId = await _trackingRepository.AddNewItemAsync(newItem);
                newItem.ItemId = itemId;
                itemsQueue.Enqueue(newItem);

                try
                {
                    await _botService.Client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        replyToMessageId: message.MessageId,
                        text: "Item was added for tracking.");
                } catch (Exception ex){}
            }
        }
    }
}
