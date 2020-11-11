using PriceTracker.Models;
using System.Collections.Generic;

namespace PriceTracker.Helpers
{
    public interface ITextConverter
    {
        List<string> ToStrings(List<Item> items);

        string ToString(Item item);
    }
}
