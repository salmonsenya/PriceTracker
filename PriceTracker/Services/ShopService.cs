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
using PriceTracker.Consts;
using System.Linq;

namespace PriceTracker.Services
{
    public class ShopService : IShopService
    {
        private readonly IBotService _botService;
        private readonly ITrackingRepository _trackingRepository;
        private readonly IPullAndBearClient _pullAndBearClient;
        private readonly IBershkaClient _bershkaClient;
        private readonly IMapper _mapper;
        private readonly IUpdateInfoHelper _updateInfoHelper;
        private readonly IShopDefiner _shopDefiner;

        private Queue<Item> itemsQueue;
        private Timer timer;

        private const string UNKNOWN_SHOP_EXCEPTION = "Item could not be added for tracking. Unknown shop.";
        private readonly string NEED_INLINE_LINK_EXCEPTION = $"Insert a link of item you want to add for tracking after command /{Commands.ADD} + space.";
        private const string ALREADY_TRACKED_EXCEPTION = "Item is already added for tracking.";
        private const string PARSE_NAME_EXCEPTION = "Failed to parse name of item to delete.";
        private const string REMOVE_FROM_QUEUE_EXCEPTION = "Item could not be removed from queue.";

        public ShopService(
            IBotService botService,
            ITrackingRepository trackingRepository,
            IPullAndBearClient pullAndBearClient,
            IBershkaClient bershkaClient,
            IMapper mapper,
            IUpdateInfoHelper updateInfoHelper,
            IShopDefiner shopDefiner)
        {
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _trackingRepository = trackingRepository ?? throw new ArgumentNullException(nameof(trackingRepository));
            _pullAndBearClient = pullAndBearClient ?? throw new ArgumentNullException(nameof(pullAndBearClient));
            _bershkaClient = bershkaClient ?? throw new ArgumentNullException(nameof(bershkaClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _updateInfoHelper = updateInfoHelper ?? throw new ArgumentNullException(nameof(updateInfoHelper));
            _shopDefiner = shopDefiner ?? throw new ArgumentNullException(nameof(shopDefiner));

            List<Item> existedItems = _trackingRepository.GetItemsAsync().Result;
            itemsQueue = existedItems?.Count > 0 ? new Queue<Item>(existedItems) : new Queue<Item>();

            timer = new Timer(10000); // 10 sec
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public async Task<List<Item>> GetTrackedItemsAsync(int userId)
        {
            var items = await _trackingRepository.GetItemsAsync();
            var userItems = items.Where(x => x.UserId == userId).ToList();
            return userItems;
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (itemsQueue.Count > 0)
            {
                var item = itemsQueue.Dequeue();
                try
                {
                    ItemOnline newInfo = null;
                    try
                    {
                        var shopName = item.Source;
                        newInfo = shopName switch
                        {
                            PullAndBear.SHOP_NAME => await _pullAndBearClient.GetItemInfoAsync(item.Url),
                            Bershka.SHOP_NAME => await _bershkaClient.GetItemInfoAsync(item.Url),
                            _ => throw new Exception(UNKNOWN_SHOP_EXCEPTION),
                        };
                    } catch (Exception ex)
                    {
                        // it's ok; lets update item information next time;
                    }
                    if (newInfo != null)
                    {
                        await _trackingRepository.UpdateInfoOfItemAsync(item.ItemId, newInfo);
                        if (item.Status != newInfo.Status || item.Price != newInfo.Price)
                        {
                            await _botService.SendMessageMarkdownV2Async(
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

        public async Task<Item> AddNewItemAsync(Message message)
        {
            var url = Regex.Match(message.Text, Common.UrlTemplate).Value;
            if (string.IsNullOrEmpty(url))
                throw new Exception(NEED_INLINE_LINK_EXCEPTION);

            if (await _trackingRepository.IsTrackedAsync(url, message.From.Id))
                throw new Exception(ALREADY_TRACKED_EXCEPTION);

            var shopName = _shopDefiner.GetShopName(url);
            var newInfo = shopName switch
            {
                PullAndBear.SHOP_NAME => await _pullAndBearClient.GetItemInfoAsync(url),
                Bershka.SHOP_NAME => await _bershkaClient.GetItemInfoAsync(url),
                _ => throw new Exception(UNKNOWN_SHOP_EXCEPTION),
            };

            var newItem = _mapper.Map<ItemOnline, Item>(newInfo);
            newItem.Url = url;
            newItem.StartTrackingDate = DateTime.Now;
            newItem.Source = shopName;
            newItem.ChatId = message.Chat.Id;
            newItem.UserId = message.From.Id;

            var itemId = await _trackingRepository.AddNewItemAsync(newItem);
            newItem.ItemId = itemId;
            itemsQueue.Enqueue(newItem);
            return newItem;
        }

        public async Task RemoveItemAsync(Message itemMessage)
        {
            var itemText = itemMessage.Text;
            var firstLine = itemText.Substring(0, itemText.IndexOf(Environment.NewLine));

            if (string.IsNullOrEmpty(firstLine))
                throw new Exception(PARSE_NAME_EXCEPTION);

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
                throw new Exception(REMOVE_FROM_QUEUE_EXCEPTION);

            await _trackingRepository.RemoveItemAsync(firstLine);
        }
    }
}
