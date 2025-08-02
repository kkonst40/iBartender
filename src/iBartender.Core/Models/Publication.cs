

namespace iBartender.Core.Models
{
    public class Publication
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public string Text { get; } = string.Empty;
        public List<string> Files { get; } = new List<string>();
        public DateTimeOffset CreatedAt { get; }
        public bool IsEdited { get; }
        private Publication(Guid id, Guid userId, string text, List<string> files, DateTimeOffset createdAt, bool isEdited)
        {
            Id = id;
            UserId = userId;
            Text = text;
            Files = files;
            CreatedAt = createdAt;
            IsEdited = isEdited;
        }

        public static Publication Create(Guid id, Guid userId, string text, List<string> files, DateTimeOffset createdAt, bool isEdited)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id), "New publication id is empty");
            }

            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId), "New publication's owner id is null");
            }

            if (string.IsNullOrEmpty(text) && files.Count == 0)
            {
                throw new ArgumentNullException("Publication content is empty");
            }

            if (createdAt == DateTimeOffset.MinValue)
            {
                throw new Exception("Creation time is incorrect");
            }

            return new Publication(id, userId, text, files, createdAt, isEdited);
        }

    }
}
