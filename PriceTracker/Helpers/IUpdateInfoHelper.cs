using PriceTracker.Models;

namespace PriceTracker.Helpers
{
    public interface IUpdateInfoHelper
    {
        Item GetUpdatedItem(Item item, ItemOnline newInfo);
    }
}
