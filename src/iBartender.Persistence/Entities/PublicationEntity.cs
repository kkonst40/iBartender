namespace iBartender.Persistence.Entities
{
    public class PublicationEntity
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<string> Files { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }

        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }
        public ICollection<UserPublicationEntity> LikedPublications { get; set; } = [];
        public ICollection<CommentEntity> Comments { get; set; } = [];
    }
}
