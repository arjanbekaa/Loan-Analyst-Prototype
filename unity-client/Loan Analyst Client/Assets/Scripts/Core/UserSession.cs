namespace LoanAnalyst.Client.Core
{
    public static class UserSession
    {
        public static string JwtToken;
        public static string UserId;
        public static string Username;
        public static string DisplayName;
        public static string Role;

        public static bool IsLoggedIn => !string.IsNullOrWhiteSpace(JwtToken);
        public static string NormalizedRole => string.IsNullOrWhiteSpace(Role) ? string.Empty : Role.Trim().ToLowerInvariant();
        public static bool IsManager => NormalizedRole == "manager";
        public static bool IsReviewer => NormalizedRole == "reviewer";

        public static void Clear()
        {
            JwtToken = null;
            UserId = null;
            Username = null;
            DisplayName = null;
            Role = null;
        }
    }
}
