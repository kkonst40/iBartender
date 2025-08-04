using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using ZeroBounceSDK;


namespace iBartender.Application.Utils
{
    public class EmailValidator : IEmailValidator
    {
        private readonly string _zbApiKey;

        public EmailValidator(IConfiguration configuration)
        {
            _zbApiKey = configuration["ZeroBounce:ApiKey"];
        }

        public bool Validate(string email)
        {
            if (!Exists(email))
                return false;
            
            return true;
        }

        private bool Exists(string email)
        {
            ZeroBounce.Instance.Initialize(_zbApiKey);
            string ipAddress = "127.0.0.1";
            ZBValidateStatus result = ZBValidateStatus.Invalid;
            ZeroBounce.Instance.Validate(email, ipAddress,
                response => result = response.Status,
                error => result = ZBValidateStatus.Invalid);

            return result == ZBValidateStatus.Valid;
        }
    }
}
