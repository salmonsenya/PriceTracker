using Microsoft.AspNetCore.Mvc;
using PriceTracker.Models;
using System.Threading.Tasks;

namespace PriceTracker.Clients
{
    public interface IASOSClient
    {
        Task<TrackingStatus> GetItemInfoAsync(string paramUrl);
    }
}
