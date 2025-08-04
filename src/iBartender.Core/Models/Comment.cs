

namespace iBartender.Core.Models
{
    public class Comment
    {
        public Guid Id { get; init; }
        public Guid PublicationId { get; init; }
        public Guid UserId { get; init; }
        public string Text { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }
}
