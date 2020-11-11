using Microsoft.EntityFrameworkCore;
using PriceTracker.Helpers;
using PriceTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceTracker.Repositories
{
    public class TrackingRepository : ITrackingRepository
    {
        private readonly IUpdateInfoHelper _updateInfoHelper;

        private const string REMOVE_EXCEPTION = "Item could not be removed.";
        private const string NOT_FOUND_IN_DB = "Item to remove was not found in DB.";
        private const string REMOVE_FROM_DB_EXCEPTION = "Failed to remove item from DB.";
        private const string USER_NOT_FOUND_EXCEPTION = "Failed to find user in DB";
        private const string ITEM_NOT_FOUND_EXCEPTION = "Failed to find item in DB";
        private const string STATUS_NOT_FOUND_EXCEPTION = "Status for user not defined.";

        public TrackingRepository(IUpdateInfoHelper updateInfoHelper)
        {
            _updateInfoHelper = updateInfoHelper ?? throw new ArgumentNullException(nameof(updateInfoHelper));
        }

        public bool IsUserStatusExists(int userId)
        {
            using var _trackingContext = new TrackingContext();
            return _trackingContext.UserStatuses.Where(x => x.UserId == userId).Count() > 0;
        }

        public async Task<List<UserStatus>> GetUserStatusesAsync()
        {
            using var _trackingContext = new TrackingContext();
            return await _trackingContext.UserStatuses.ToListAsync();
        }

        public async Task<bool> IsWaitingForAddAsync(int userId){
            using var _trackingContext = new TrackingContext();
            var userStatus = await _trackingContext.UserStatuses.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (userStatus == null)
                throw new Exception($"{USER_NOT_FOUND_EXCEPTION}");
            return userStatus.waitingForAdd;
        }

        public async Task<bool> AddUserStatusAsync(int userId)
        {
            using var _trackingContext = new TrackingContext();
            _trackingContext.UserStatuses.Add(new UserStatus { UserId = userId, waitingForAdd = false });
            await _trackingContext.SaveChangesAsync();
            return false;
        }

        public async Task SetWaitingForAddAsync(int userId, bool newValue)
        {
            using var _trackingContext = new TrackingContext();
            var userStatus = await _trackingContext.UserStatuses.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (userStatus == null)
                throw new Exception($"{USER_NOT_FOUND_EXCEPTION}");
            userStatus.waitingForAdd = newValue;
            await _trackingContext.SaveChangesAsync();
        }

        public async Task<int> AddNewItemAsync(Item item)
        {
            using var _trackingContext = new TrackingContext();
            _trackingContext.Items.Add(item);
            await _trackingContext.SaveChangesAsync();
            var result = await _trackingContext.Items.Where(x => x.Url.Equals(item.Url)).FirstOrDefaultAsync();
            return result.ItemId;
        }

        public async Task<List<Item>> GetItemsAsync() {
            using var _trackingContext = new TrackingContext();
            return await _trackingContext.Items.ToListAsync();
        }
          
        public async Task UpdateInfoOfItemAsync(int id, ItemOnline itemOnline)
        {
            using var _trackingContext = new TrackingContext();
            var item = await _trackingContext.Items.Where(i => i.ItemId == id).FirstOrDefaultAsync();
            if (item == null)
                throw new Exception($"{ITEM_NOT_FOUND_EXCEPTION}");
            item = _updateInfoHelper.GetUpdatedItem(item, itemOnline);
            await _trackingContext.SaveChangesAsync();
        }

        public async Task<bool> IsTrackedAsync(string url, int userId)
        {
            using var _trackingContext = new TrackingContext();
            var items = await _trackingContext.Items.Where(i => i.Url.Equals(url) && i.UserId == userId).ToListAsync();
            return items.Count > 0;
        }

        public async Task RemoveItemAsync(string url)
        {
            using var _trackingContext = new TrackingContext();
            var itemToRemove = await _trackingContext.Items.Where(x => x.Name.Equals(url))?.FirstAsync();
            if (itemToRemove == null)
                throw new Exception($"{REMOVE_EXCEPTION} {NOT_FOUND_IN_DB}");

            try
            {
                _trackingContext.Items.Remove(itemToRemove);
            } catch (Exception ex)
            {
                throw new Exception($"{REMOVE_EXCEPTION} {REMOVE_FROM_DB_EXCEPTION} {ex.Message}");
            }
            await _trackingContext.SaveChangesAsync();
        }
    }
}
