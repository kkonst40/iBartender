namespace iBartender.API.Contracts.Users
{
    public record UpdateUserPasswordRequest(
        string OldPassword,
        string NewPassword,
        string NewPasswordConfirm);
}
