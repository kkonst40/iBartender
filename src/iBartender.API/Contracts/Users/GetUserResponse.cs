namespace iBartender.API.Contracts.Users
{
    public record GetUserResponse(
        Guid id,
        string login,
        string location,
        string photo);
}
