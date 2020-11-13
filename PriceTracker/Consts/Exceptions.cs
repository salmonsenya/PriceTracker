namespace PriceTracker.Consts
{
    public static class Exceptions
    {
        public const string HTTP_EXCEPTION = "The HTTP status code of the response was not expected";

        public const string PARSING_EXCEPTION = "Page could not be parsed.";
        public const string XPATH_EXCEPTION = "XML path could not be found on page.";

        public const string NOT_DEFINED_EXCEPTION = "Item could not be added for tracking. Shop could not be defined from url.";

        public const string REMOVE_EXCEPTION = "Item could not be removed from cart.";
        public const string NOT_FOUND_IN_DB_EXCEPTION = "Item to remove was not found in DB.";
        public const string REMOVE_FROM_DB_EXCEPTION = "Failed to remove item from DB.";
        public const string USER_NOT_FOUND_EXCEPTION = "Failed to find user in DB";
        public const string ITEM_NOT_FOUND_EXCEPTION = "Failed to find item in DB";

        public const string UNKNOWN_SHOP_EXCEPTION = "Item could not be added for tracking. Unknown shop.";
        public const string ALREADY_TRACKED_EXCEPTION = "Item is already added for tracking.";
        public const string PARSE_NAME_EXCEPTION = "Failed to parse name of item to delete.";

        public const string REMOVE_FROM_QUEUE_EXCEPTION = "Item could not be removed from queue.";

        public const string EMPTY_CART = "Your cart is empty";
        public const string NEED_CORRECT_LINK_EXCEPTION = "Insert a correct link.";
        public const string NEED_LINK_EXCEPTION = "Insert a link of item you want to add.";
        public const string REMOVED = "Item was removed from cart.";
    }
}
