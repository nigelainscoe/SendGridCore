using Microsoft.Extensions.Configuration;

namespace APITest.Services
{
    public interface IAuthService
    {
        string GetAuthMethod();
        string GetAuthString();
        string GetListToManage();
    }
    public class AuthService : IAuthService
    {
        private readonly string _apikey;
        private readonly string _authorisationMethod;
        private readonly string _listToManage;

        public AuthService(IConfiguration configuration)
        {
            _apikey = configuration["SendGrid:ApiKey"];
            _authorisationMethod = configuration["SendGrid:Authorization"];
            _listToManage = configuration["SendGrid:ListToManage"];

        }

        public string GetAuthString()
        {
            return _apikey;
        }

        public string GetListToManage()
        {
            return _listToManage;
        }

        public string GetAuthMethod()
        {
            return _authorisationMethod;
        }
    }
}
