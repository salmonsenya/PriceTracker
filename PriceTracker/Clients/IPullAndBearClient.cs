﻿using PriceTracker.Models;
using System.Threading.Tasks;

namespace PriceTracker.Clients
{
    public interface IPullAndBearClient
    {
        Task<TrackingStatus> GetItemInfoAsync(int itemId, string paramUrl);
    }
}