using TMPro;
using UnityEngine;
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

        #region Properties

        public TextMeshProUGUI TitleText => titleText;
        public TextMeshProUGUI SubtitleText => subtitleText;
        public TextMeshProUGUI ErrorText => errorText;
        public TextMeshProUGUI StatusText => statusText;
        public TextMeshProUGUI FooterText => footerText;
        public TMP_InputField UsernameInput => usernameInput;
        public TMP_InputField PasswordInput => passwordInput;
        public Button LoginButton => loginButton;

        #endregion
    }
}
