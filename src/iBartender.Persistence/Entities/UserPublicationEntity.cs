namespace iBartender.Persistence.Entities
{
    public class UserPublicationEntity
    {
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }

        public Guid PublicationId { get; set; }
        public PublicationEntity? Publication { get; set; }
    }
}
