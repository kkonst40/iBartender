namespace iBartender.API.Contracts.Publications
{
    public record UpdatePublicationRequest(
        string text,
        IFormFileCollection files);
}
