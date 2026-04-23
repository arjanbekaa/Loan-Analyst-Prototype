using System.Threading.Tasks;

namespace LoanAnalyst.Client.Services
{
    [System.Serializable]
    public class AuthLoginRequest
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class AuthUserDto
    {
        public string id;
        public string username;
        public string role;
        public string displayName;
    }

    [System.Serializable]
    public class AuthLoginResponse
    {
        public string token;
        public AuthUserDto user;
    }

    public class AuthService
    {
        private readonly ApiClient _apiClient;

        public AuthService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Task<AuthLoginResponse> LoginAsync(string username, string password)
        {
            var payload = new AuthLoginRequest
            {
                username = username,
                password = password
            };

            return _apiClient.PostAsync<AuthLoginRequest, AuthLoginResponse>("/auth/login", payload, authorized: false);
        }
    }
}
