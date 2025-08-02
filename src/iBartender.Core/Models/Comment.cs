

namespace iBartender.Core.Models
{
    public class Comment
    {
        public Guid Id { get; }
        public Guid PublicationId { get; }
        public Guid UserId { get; }
        public string Text { get; } = string.Empty;
        public DateTimeOffset CreatedAt { get; }
        private Comment(Guid id, Guid publicationId, Guid userId, string text, DateTimeOffset createdAt) 
        {
            Id = id;
            PublicationId = publicationId;
            UserId = userId;
            Text = text;
            CreatedAt = createdAt;
        }

        public static Comment Create(Guid id, Guid publicationId, Guid userId, string text, DateTimeOffset createdAt)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id), "New publication id is empty");
            }
            if (publicationId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId), "New comment's publication id is null");
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId), "New comment's owner id is null");
            }
            if (string.IsNullOrEmpty(text))
            {
                throw new Exception("Comment content is empty");
            }

            return new Comment(id, publicationId, userId, text, createdAt);
        }
    }
}
