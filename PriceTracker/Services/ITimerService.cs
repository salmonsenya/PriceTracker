using PriceTracker.Models;

namespace PriceTracker.Services
{
    public interface ITimerService
    {
        void Enqueue(Item newItem);

        void RemoveFromQueue(string name);
    }
}
