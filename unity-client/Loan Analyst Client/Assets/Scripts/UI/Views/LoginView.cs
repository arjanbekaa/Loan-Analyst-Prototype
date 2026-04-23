using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LoanAnalyst.UI.Views
{
    [DisallowMultipleComponent]
    public sealed class LoginView : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI footerText;

        [Header("Inputs")]
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;

        [Header("Actions")]
        [SerializeField] private Button loginButton;

        #endregion

        public string ErrorMessage
        {
            get => GetText(errorText);
            set => SetText(errorText, value);
        }

        public string StatusMessage
        {
            get => GetText(statusText);
            set => SetText(statusText, value);
        }

        public string Username
        {
            get => usernameInput != null ? usernameInput.text?.Trim() ?? string.Empty : string.Empty;
            set
            {
                if (usernameInput != null)
                {
                    usernameInput.text = value ?? string.Empty;
                }
            }
        }

        public string Password
        {
            get => passwordInput != null ? passwordInput.text ?? string.Empty : string.Empty;
            set
            {
                if (passwordInput != null)
                {
                    passwordInput.text = value ?? string.Empty;
                }
            }
        }

        public bool IsUsernameFocused => usernameInput != null && usernameInput.isFocused;

        public bool LoginInteractable
        {
            get => loginButton != null && loginButton.interactable;
            set
            {
                if (loginButton != null)
                {
                    loginButton.interactable = value;
                }
            }
        }

        public void BindLoginAction(UnityAction action)
        {
            if (loginButton == null)
            {
                return;
            }

            loginButton.onClick.RemoveAllListeners();
            if (action != null)
            {
                loginButton.onClick.AddListener(action);
            }
        }

        public void FocusUsername()
        {
            FocusInput(usernameInput);
        }

        public void FocusPassword()
        {
            FocusInput(passwordInput);
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

        private static string GetText(TMP_Text text)
        {
            return text != null ? text.text : string.Empty;
        }

        private static void SetText(TMP_Text text, string value)
        {
            if (text != null)
            {
                text.text = value ?? string.Empty;
            }
        }
    }
}
