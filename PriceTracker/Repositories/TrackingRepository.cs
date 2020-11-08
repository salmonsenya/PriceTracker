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
        public TrackingRepository(IUpdateInfoHelper updateInfoHelper)
        {
            _updateInfoHelper = updateInfoHelper ?? throw new ArgumentNullException(nameof(updateInfoHelper));
        }

        public async Task<bool> IsWaitingForAdd(int userId){
            using var _trackingContext = new TrackingContext();
            var userStatus = await _trackingContext.UserStatuses.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (userStatus == null)
            {
                await AddUserStatus(userId);
                userStatus = await _trackingContext.UserStatuses.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            }
            return userStatus.waitingForAdd;
        }

        public async Task AddUserStatus(int userId)
        {
            using var _trackingContext = new TrackingContext();
            _trackingContext.UserStatuses.Add(new UserStatus { UserId = userId, waitingForAdd = false });
            await _trackingContext.SaveChangesAsync();
        }

        public async Task SetWaitingForAdd(int userId, bool newValue)
        {
            using var _trackingContext = new TrackingContext();
            var userStatus = await _trackingContext.UserStatuses.Where(x => x.UserId == userId).FirstOrDefaultAsync();
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
            item = _updateInfoHelper.GetUpdatedItem(item, itemOnline);
            await _trackingContext.SaveChangesAsync();
        }

        public async Task<bool> IsTracked(string url, int userId)
        {
            using var _trackingContext = new TrackingContext();
            var items = await _trackingContext.Items.Where(i => i.Url.Equals(url) && i.UserId == userId).ToListAsync();
            return items.Count > 0;
        }

        public async Task RemoveItem(string url)
        {
            using var _trackingContext = new TrackingContext();
            var itemToRemove = await _trackingContext.Items.SingleOrDefaultAsync(x => x.Name.Equals(url));
            if (itemToRemove == null)
            {
                throw new System.Exception(message: "Item to remove could not be found in DB.");
            }

            try
            {
                _trackingContext.Items.Remove(itemToRemove);
            } catch (Exception ex)
            {
                throw new System.Exception(message: "Item to remove could not be removed from DB.");
            }
            await _trackingContext.SaveChangesAsync();
        }
    }
}
