namespace iBartender.API.Contracts.Users
{
    public record LoginUserRequest(
        string Email,
        string Password);
}
