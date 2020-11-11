using PriceTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace PriceTracker.Helpers
{
    public class TextConverter: ITextConverter
    {
        public List<string> ToStrings(List<Item> items)
        {
            var textItems = items.Select(x => $@"
*{x.Name}*
Current: {x.Price} {x.PriceCurrency}
[View on site]({x.Url})
");
            return textItems.ToList();
        }

        public string ToString(Item item) =>
            $@"
*{item.Name}*
Current: {item.Price} {item.PriceCurrency}
[View on site]({item.Url})";

        public string ToString(Item item, ItemOnline newInfo) =>
            $@"
Item price has been changed.
*{item.Name}*
Previous: {item.Price} {item.PriceCurrency}
Current: {newInfo.Price} {newInfo.PriceCurrency}
[View on site]({item.Url})
";
    }
}
