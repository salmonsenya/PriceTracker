namespace PriceTracker.Models
{
    public class UserStatus
    {
        public int UserStatusId { get; set; }

        public int UserId { get; set; }

        public bool waitingForAdd { get; set; }
    }
}
