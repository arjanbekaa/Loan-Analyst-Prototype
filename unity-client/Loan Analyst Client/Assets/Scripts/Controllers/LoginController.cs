using LoanAnalyst.Client.Core;
using LoanAnalyst.Client.Services;
using LoanAnalyst.UI.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace LoanAnalyst.Client.Controllers
{
    public class LoginController : MonoBehaviour
    {
        [SerializeField] private LoginView loginView;

        private AuthService _authService;
        private bool _isLoggingIn;

        private void Awake()
        {
            _authService = new AuthService(new ApiClient());
            if (loginView == null)
            {
                loginView = FindAnyObjectByType<LoginView>(FindObjectsInactive.Include);
            }
        }

        private void Start()
        {
            if (loginView == null)
            {
                Debug.LogError("LoginView is not assigned.");
                return;
            }

            UserSession.Clear();
            SetError(string.Empty);
            SetStatus($"API: {ApiConfig.BaseUrl}");
            loginView.BindLoginAction(OnLoginClicked);
            loginView.FocusUsername();
        }

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.tabKey.wasPressedThisFrame)
            {
                if (loginView.IsUsernameFocused)
                {
                    loginView.FocusPassword();
                }
                else
                {
                    loginView.FocusUsername();
                }
            }

            if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
            {
                OnLoginClicked();
            }
#endif
        }

        private async void OnLoginClicked()
        {
            if (_isLoggingIn)
            {
                return;
            }

            SetError(string.Empty);
            SetStatus("Logging in...");

            var username = loginView.Username;
            var password = loginView.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                SetStatus(string.Empty);
                SetError("Username and password are required.");
                return;
            }

            _isLoggingIn = true;
            loginView.LoginInteractable = false;
            try
            {
                var response = await _authService.LoginAsync(username, password);
                UserSession.JwtToken = response.token;
                UserSession.UserId = response.user?.id;
                UserSession.Username = response.user?.username;
                UserSession.Role = response.user?.role?.Trim().ToLowerInvariant();
                UserSession.DisplayName = response.user?.displayName;

                SceneManager.LoadScene("ApplicantsScene");
            }
            catch (ApiException ex)
            {
                SetStatus(string.Empty);
                SetError($"{ex.Message} (HTTP {ex.StatusCode})");
            }
            catch (System.Exception ex)
            {
                SetStatus(string.Empty);
                SetError($"Unexpected error: {ex.Message}");
            }
            finally
            {
                _isLoggingIn = false;
                loginView.LoginInteractable = true;
            }
        }

        private void SetError(string value)
        {
            loginView.ErrorMessage = value;
        }

        private void SetStatus(string value)
        {
            loginView.StatusMessage = value;
        }
    }
}
