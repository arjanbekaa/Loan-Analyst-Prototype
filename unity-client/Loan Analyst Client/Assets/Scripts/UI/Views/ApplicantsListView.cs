using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LoanAnalyst.UI.Views
{
    [DisallowMultipleComponent]
    public sealed class ApplicantsListView : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Header")]
        [SerializeField] private TextMeshProUGUI headerTitleText;
        [SerializeField] private TextMeshProUGUI roleText;
        [SerializeField] private Button logoutButton;

        [Header("Filters")]
        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private Button refreshButton;

        [Header("List")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform contentRoot;
        [SerializeField] private TextMeshProUGUI emptyStateText;
        [SerializeField] private ApplicantListItemView itemTemplate;

        [Header("Feedback")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI errorText;

        #endregion

        public string HeaderTitle
        {
            get => GetText(headerTitleText);
            set => SetText(headerTitleText, value);
        }

        public string SignedInRole
        {
            get => GetText(roleText);
            set => SetText(roleText, value);
        }

        public string StatusMessage
        {
            get => GetText(statusText);
            set => SetFeedbackText(statusText, "Status: ", value);
        }

        public string ErrorMessage
        {
            get => GetText(errorText);
            set => SetFeedbackText(errorText, "Error: ", value);
        }

        public string EmptyStateMessage
        {
            get => GetText(emptyStateText);
            set => SetText(emptyStateText, value);
        }

        public bool EmptyStateVisible
        {
            get => emptyStateText != null && emptyStateText.gameObject.activeSelf;
            set
            {
                if (emptyStateText != null)
                {
                    emptyStateText.gameObject.SetActive(value);
                }
            }
        }

        public bool RefreshInteractable
        {
            get => refreshButton != null && refreshButton.interactable;
            set
            {
                if (refreshButton != null)
                {
                    refreshButton.interactable = value;
                }
            }
        }

        public RectTransform ContentRoot => contentRoot;
        public ApplicantListItemView ItemTemplate => itemTemplate;

        public void BindRefreshAction(UnityAction action)
        {
            ReplaceButtonHandler(refreshButton, action);
        }

        public void BindLogoutAction(UnityAction action)
        {
            ReplaceButtonHandler(logoutButton, action);
        }

        public ApplicantListItemView CreateItem()
        {
            if (itemTemplate == null || contentRoot == null)
            {
                return null;
            }

            var item = Instantiate(itemTemplate, contentRoot);
            item.gameObject.SetActive(true);
            return item;
        }

        public void ClearItems()
        {
            if (contentRoot == null)
            {
                return;
            }

            for (var i = contentRoot.childCount - 1; i >= 0; i--)
            {
                var child = contentRoot.GetChild(i);
                if (itemTemplate != null && child == itemTemplate.transform)
                {
                    continue;
                }

                Destroy(child.gameObject);
            }
        }

        private static void ReplaceButtonHandler(Button button, UnityAction action)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            if (action != null)
            {
                button.onClick.AddListener(action);
            }
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

        private static void SetFeedbackText(TMP_Text text, string prefix, string value)
        {
            if (text == null)
            {
                return;
            }

            text.text = string.IsNullOrEmpty(value)
                ? string.Empty
                : $"{prefix}{value}";
        }
    }
}
