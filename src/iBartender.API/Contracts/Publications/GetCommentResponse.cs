namespace iBartender.API.Contracts.Publications
{
    public record GetCommentResponse(
        Guid id,
        Guid publicationId,
        Guid userId,
        string text,
        DateTimeOffset createdAt);
}
