

namespace iBartender.Core.Models
{
    public class Publication
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Text { get; init; } = string.Empty;
        public List<string> Files { get; init; } = new List<string>();
        public DateTime CreatedAt { get; init; }
        public bool IsEdited { get; init; }
    }
}
