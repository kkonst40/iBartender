namespace iBartender.API.Contracts.Users
{
    public record CreateUserRequest(
        string login,
        string email,
        string password);
}
