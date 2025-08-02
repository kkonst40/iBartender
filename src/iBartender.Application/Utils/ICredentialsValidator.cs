namespace iBartender.Application.Utils
{
    public interface ICredentialsValidator
    {
        bool ValidateEmail(string email);
        bool ValidateLogin(string email);
        bool ValidatePassword(string email);
    }
}