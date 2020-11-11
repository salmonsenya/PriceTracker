using AutoMapper;
using PriceTracker.Clients;
using PriceTracker.Helpers;
using PriceTracker.Models;
using PriceTracker.Repositories;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Telegram.Bot.Types;
using PriceTracker.Consts;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Services
{
    public class ShopService : IShopService
    {
        private readonly ITrackingRepository _trackingRepository;
        private readonly IPullAndBearClient _pullAndBearClient;
        private readonly IBershkaClient _bershkaClient;
        private readonly IMapper _mapper;
        private readonly IShopDefiner _shopDefiner;
        private readonly ITimerService _timerService;

        private const string UNKNOWN_SHOP_EXCEPTION = "Item could not be added for tracking. Unknown shop.";
        private readonly string NEED_INLINE_LINK_EXCEPTION = $"Insert a link of item you want to add for tracking after command /{Commands.ADD} + space.";
        private const string ALREADY_TRACKED_EXCEPTION = "Item is already added for tracking.";
        private const string PARSE_NAME_EXCEPTION = "Failed to parse name of item to delete.";

        public ShopService(
            ITrackingRepository trackingRepository,
            IPullAndBearClient pullAndBearClient,
            IBershkaClient bershkaClient,
            IMapper mapper,
            IShopDefiner shopDefiner,
            ITimerService timerService)
        {
            _trackingRepository = trackingRepository ?? throw new ArgumentNullException(nameof(trackingRepository));
            _pullAndBearClient = pullAndBearClient ?? throw new ArgumentNullException(nameof(pullAndBearClient));
            _bershkaClient = bershkaClient ?? throw new ArgumentNullException(nameof(bershkaClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _shopDefiner = shopDefiner ?? throw new ArgumentNullException(nameof(shopDefiner));
            _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
        }

        public async Task<List<Item>> GetTrackedItemsAsync(int userId)
        {
            var items = await _trackingRepository.GetItemsAsync();
            var userItems = items.Where(x => x.UserId == userId).ToList();
            return userItems;
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
            _timerService.Enqueue(newItem);
            return newItem;
        }

        public async Task RemoveItemAsync(Message itemMessage)
        {
            var itemText = itemMessage.Text;
            var firstLine = itemText.Substring(0, itemText.IndexOf(Environment.NewLine));

            if (string.IsNullOrEmpty(firstLine))
                throw new Exception(PARSE_NAME_EXCEPTION);

            _timerService.RemoveFromQueue(firstLine);

            await _trackingRepository.RemoveItemAsync(firstLine);
        }
    }
}
