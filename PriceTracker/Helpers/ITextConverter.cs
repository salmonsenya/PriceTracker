using PriceTracker.Models;
using System.Collections.Generic;

namespace PriceTracker.Helpers
{
    public interface ITextConverter
    {
        IEnumerable<string> ToStrings(List<Item> items);

        string ToString(Item item);

        string ToString(Item item, ItemOnline newInfo);
    }
}
