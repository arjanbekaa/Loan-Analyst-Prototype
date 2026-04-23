using System;

namespace LoanAnalyst.Client.Models
{
    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class LoginResponse
    {
        public string token;
        public UserDto user;
    }

    [Serializable]
    public class UserDto
    {
        public string id;
        public string username;
        public string role;
        public string displayName;
    }
}
