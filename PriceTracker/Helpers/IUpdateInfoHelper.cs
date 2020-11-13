using PriceTracker.Models;

namespace PriceTracker.Helpers
{
    public interface IUpdateInfoHelper
    {
        void GetUpdatedItem(ref Item item, ItemOnline newInfo);

        void GetUpdatedItem(ref Item item, Item newInfo);
    }
}
