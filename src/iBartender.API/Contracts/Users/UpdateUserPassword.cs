namespace iBartender.API.Contracts.Users
{
    public record UpdateUserPassword(
        string oldPassword,
        string newPassword,
        string newPasswordConfirm);
}
