namespace iBartender.API.Contracts.Publications
{
    public record GetPublicationResponse(
        Guid Id,
        Guid UserId,
        string Text,
        List<string> Files,
        DateTimeOffset CreationTime,
        bool IsEdited);
}
