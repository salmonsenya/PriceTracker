using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PriceTracker.Consts;
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
        private readonly DbContextOptions<TrackingContext> _options;
        private const string CONNECTION_STRING = "TrackingItemsDb";

        public TrackingRepository(IUpdateInfoHelper updateInfoHelper, IConfiguration configuration)
        {
            _updateInfoHelper = updateInfoHelper;
            var _optionsBuilder = new DbContextOptionsBuilder<TrackingContext>()
                .UseMySql(configuration.GetConnectionString(CONNECTION_STRING));
            _options = _optionsBuilder.Options;
        }

        public async Task<bool> IsUserStatusExistsAsync(int userId)
        {
            using var _trackingContext = new TrackingContext(_options);
            return await _trackingContext.UserStatuses.Where(x => x.UserId == userId).AnyAsync();
        }

        public async Task<List<UserStatus>> GetUserStatusesAsync()
        {
            using var _trackingContext = new TrackingContext(_options);
            return await _trackingContext.UserStatuses.ToListAsync();
        }

        public async Task<bool> IsWaitingForAddAsync(int userId){
            using var _trackingContext = new TrackingContext(_options);
            var userStatus = await _trackingContext.UserStatuses.Where(x => x.UserId == userId).FirstOrDefaultAsync() ??
                throw new Exception($"{Exceptions.USER_NOT_FOUND_EXCEPTION}");
            return userStatus.waitingForAdd;
        }

        public async Task<bool> AddUserStatusAsync(int userId)
        {
            using var _trackingContext = new TrackingContext(_options);
            await _trackingContext.UserStatuses.AddAsync(new UserStatus { UserId = userId, waitingForAdd = false });
            await _trackingContext.SaveChangesAsync();
            return false;
        }

        public async Task SetWaitingForAddAsync(int userId, bool newValue)
        {
            using var _trackingContext = new TrackingContext(_options);
            var userStatus = await _trackingContext.UserStatuses.Where(x => x.UserId == userId).FirstOrDefaultAsync() ??
                throw new Exception($"{Exceptions.USER_NOT_FOUND_EXCEPTION}");
            userStatus.waitingForAdd = newValue;
            await _trackingContext.SaveChangesAsync();
        }

        public async Task<int> AddNewItemAsync(Item item)
        {
            using var _trackingContext = new TrackingContext(_options);
            await _trackingContext.Items.AddAsync(item);
            await _trackingContext.SaveChangesAsync();
            var result = await _trackingContext.Items.Where(x => x.Url.Equals(item.Url)).FirstOrDefaultAsync();
            return result.ItemId;
        }

        public async Task<List<Item>> GetItemsAsync() {
            using var _trackingContext = new TrackingContext(_options);
            return await _trackingContext.Items.ToListAsync();
        }
          
        public async Task UpdateInfoOfItemAsync(Item itemUpdated)
        {
            using var _trackingContext = new TrackingContext(_options);
            var item = await _trackingContext.Items.Where(i => i.ItemId == itemUpdated.ItemId).FirstOrDefaultAsync() ??
                throw new Exception($"{Exceptions.ITEM_NOT_FOUND_EXCEPTION}");
            _updateInfoHelper.GetUpdatedItem(ref item, itemUpdated);
            await _trackingContext.SaveChangesAsync();
        }

        public async Task<bool> IsTrackedAsync(string url, int userId)
        {
            using var _trackingContext = new TrackingContext(_options);
            var items = _trackingContext.Items.Where(i => i.Url.Equals(url) && i.UserId == userId);
            return await items.AnyAsync();
        }

        public async Task RemoveItemAsync(string url)
        {
            using var _trackingContext = new TrackingContext(_options);
            var itemToRemove = await _trackingContext.Items.Where(x => x.Name.Equals(url)).FirstOrDefaultAsync() ?? 
                throw new Exception($"{Exceptions.REMOVE_EXCEPTION} {Exceptions.NOT_FOUND_IN_DB_EXCEPTION}");

            try
            {
                _trackingContext.Items.Remove(itemToRemove);
            } catch (Exception ex)
            {
                throw new Exception($"{Exceptions.REMOVE_EXCEPTION} {Exceptions.REMOVE_FROM_DB_EXCEPTION} {ex.Message}");
            }
            await _trackingContext.SaveChangesAsync();
        }
    }
}
