namespace iBartender.API.Contracts.Publications
{
    public record CreatePublicationRequest(
        string text,
        IFormFileCollection files);
}
