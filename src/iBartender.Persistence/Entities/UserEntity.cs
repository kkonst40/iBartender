namespace iBartender.Persistence.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Bio {  get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public Guid TokenId { get; set; }

        public ICollection<PublicationEntity> Publications { get; set; } = [];
        public ICollection<UserSubscriberEntity> Subscribers { get; set; } = [];
        public ICollection<UserSubscriberEntity> Subscriptions { get; set; } = [];
        public ICollection<UserPublicationEntity> LikedPublications { get; set; } = [];
        public ICollection<CommentEntity> Comments { get; set; } = [];
    }
}
