namespace iBartender.API.Contracts.Users
{
    public record LoginUserRequest(
        string email,
        string password);
}
