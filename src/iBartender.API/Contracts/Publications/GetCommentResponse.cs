namespace iBartender.API.Contracts.Publications
{
    public record GetCommentResponse(
        Guid Id,
        Guid PublicationId,
        Guid UserId,
        string Text,
        DateTimeOffset CreatedAt);
}
