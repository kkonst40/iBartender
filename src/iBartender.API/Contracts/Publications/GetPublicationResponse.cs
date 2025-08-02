namespace iBartender.API.Contracts.Publications
{
    public record GetPublicationResponse(
        Guid id,
        Guid userId,
        string text,
        List<string> files,
        DateTimeOffset creationTime,
        bool isEdited);
}
