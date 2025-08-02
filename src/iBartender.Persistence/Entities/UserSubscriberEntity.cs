namespace iBartender.Persistence.Entities
{
    public class UserSubscriberEntity
    {
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }

        public Guid SubscriberId { get; set; }
        public UserEntity? Subscriber { get; set; }
    }
}
