namespace iBartender.API.Contracts.Users
{
    public record CreateUserRequest(
        string Login,
        string Email,
        string Password);
}
