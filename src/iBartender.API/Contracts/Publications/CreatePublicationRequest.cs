namespace iBartender.API.Contracts.Publications
{
    public record CreatePublicationRequest(
        string Text,
        IFormFileCollection Files);
}
