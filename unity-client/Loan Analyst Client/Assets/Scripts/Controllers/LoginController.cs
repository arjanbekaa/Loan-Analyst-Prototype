using LoanAnalyst.Client.Core;
using LoanAnalyst.Client.Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace LoanAnalyst.Client.Controllers
{
    public class LoginController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private TextMeshProUGUI statusText;

        private AuthService _authService;
        private bool _isLoggingIn;

        private void Awake()
        {
            _authService = new AuthService(new ApiClient());
        }

        private void Start()
        {
            UserSession.Clear();
            SetError(string.Empty);
            SetStatus($"API: {ApiConfig.BaseUrl}");
            loginButton.onClick.AddListener(OnLoginClicked);
            FocusInput(usernameInput);
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
                FocusInput(usernameInput != null && usernameInput.isFocused ? passwordInput : usernameInput);
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

            var username = usernameInput.text?.Trim();
            var password = passwordInput.text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                SetStatus(string.Empty);
                SetError("Username and password are required.");
                return;
            }

            _isLoggingIn = true;
            loginButton.interactable = false;
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
                loginButton.interactable = true;
            }
        }

        private static void FocusInput(TMP_InputField input)
        {
            if (input == null)
            {
                return;
            }

            input.Select();
            input.ActivateInputField();
        }

        private void SetError(string value)
        {
            if (errorText != null)
            {
                errorText.text = value;
            }
        }

        private void SetStatus(string value)
        {
            if (statusText != null)
            {
                statusText.text = value;
            }
        }
    }
}
