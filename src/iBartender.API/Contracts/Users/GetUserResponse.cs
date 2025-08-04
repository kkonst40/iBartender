namespace iBartender.API.Contracts.Users
{
    public record GetUserResponse(
        Guid Id,
        string Login,
        string Location,
        string Photo);
}
