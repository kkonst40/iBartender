namespace iBartender.Persistence.Entities
{
    public class CommentEntity
    {
        public Guid Id { get; set; }
        public Guid PublicationId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public UserEntity? User { get; set; }
        public PublicationEntity? Publication { get; set; }
    }
}
