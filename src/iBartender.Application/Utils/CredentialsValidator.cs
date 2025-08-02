using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using ZeroBounceSDK;


namespace iBartender.Application.Utils
{
    public class CredentialsValidator : ICredentialsValidator
    {
        private readonly string _zbApiKey;

        private readonly int _passwordMinLength;
        private readonly int _passwordMaxLength;
        private readonly string _passwordAllowedChars;

        private readonly int _loginMinLength;
        private readonly int _loginMaxLength;
        private readonly string _loginAllowedChars;

        public CredentialsValidator(IConfiguration configuration)
        {
            _zbApiKey = configuration["ZeroBounce:ApiKey"];

            _passwordMinLength = Convert.ToInt32(configuration["PasswordRules:MinLength"]);
            _passwordMaxLength = Convert.ToInt32(configuration["PasswordRules:MaxLength"]);
            _passwordAllowedChars = configuration["PasswordRules:AllowedChars"];

            _loginMinLength = Convert.ToInt32(configuration["LoginRules:MinLength"]);
            _loginMaxLength = Convert.ToInt32(configuration["LoginRules:MaxLength"]);
            _loginAllowedChars = configuration["LoginRules:AllowedChars"];
        }

        public bool ValidateEmail(string email)
        {
            if (!IsCorrect(email))
                return false;

            //if (!ExistsZB(email))
            //    return false;
            
            return true;
        }

        public bool ValidateLogin(string login)
        {
            if (string.IsNullOrEmpty(login))
                return false;

            if (login.Length > _loginMaxLength || login.Length < _loginMinLength)
                return false;

            foreach (var c in login)
                if (!_loginAllowedChars.Contains(c))
                    return false;

            return true;
        }

        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (password.Length > _passwordMaxLength || password.Length < _passwordMinLength)
                return false;

            foreach (var c in password)
                if (!_passwordAllowedChars.Contains(c))
                    return false;

            return true;
        }

        private bool IsCorrect(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email
                       && mailAddress.Host.Contains('.')
                       && !mailAddress.Host.StartsWith('.')
                       && !mailAddress.Host.EndsWith('.');
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ExistsZB(string email)
        {
            ZeroBounce.Instance.Initialize(_zbApiKey);
            string ipAddress = "127.0.0.1";
            ZBValidateStatus result = ZBValidateStatus.Invalid;
            ZeroBounce.Instance.Validate(email, ipAddress,
                response => result = response.Status,
                error => result = ZBValidateStatus.Invalid);

            return (result == ZBValidateStatus.Valid);
        }
    }
}
