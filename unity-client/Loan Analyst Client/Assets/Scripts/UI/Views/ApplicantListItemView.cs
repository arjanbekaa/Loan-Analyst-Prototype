using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LoanAnalyst.UI.Views
{
    [DisallowMultipleComponent]
    public sealed class ApplicantListItemView : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private TextMeshProUGUI applicantNameText;
        [SerializeField] private TextMeshProUGUI requestedAmountText;
        [SerializeField] private TextMeshProUGUI statusBadgeText;
        [SerializeField] private Image statusBadgeBackground;
        [SerializeField] private Button openDetailsButton;

        #endregion

        private Color _defaultStatusBadgeColor;
        private Color _defaultStatusTextColor;

        public string ApplicantName
        {
            get => GetText(applicantNameText);
            set => SetText(applicantNameText, value);
        }

        public string RequestedAmount
        {
            get => GetText(requestedAmountText);
            set => SetText(requestedAmountText, value);
        }

        public string StatusBadge
        {
            get => GetText(statusBadgeText);
            set
            {
                SetText(statusBadgeText, value);
                ApplyStatusTheme(value);
            }
        }

        public Image StatusBadgeBackground => statusBadgeBackground;

        public void BindOpenDetailsAction(UnityAction action)
        {
            if (openDetailsButton == null)
            {
                return;
            }

            openDetailsButton.onClick.RemoveAllListeners();
            if (action != null)
            {
                openDetailsButton.onClick.AddListener(action);
            }
        }

        private void Awake()
        {
            _defaultStatusBadgeColor = statusBadgeBackground != null
                ? statusBadgeBackground.color
                : Color.white;
            _defaultStatusTextColor = statusBadgeText != null
                ? statusBadgeText.color
                : Color.white;
        }

        private void ApplyStatusTheme(string value)
        {
            var normalized = value?.Trim().ToLowerInvariant();

            if (statusBadgeBackground != null)
            {
                statusBadgeBackground.color = normalized switch
                {
                    "approved" => new Color(0.18f, 0.62f, 0.31f, 1f),
                    "rejected" => new Color(0.74f, 0.25f, 0.25f, 1f),
                    _ => _defaultStatusBadgeColor,
                };
            }

            if (statusBadgeText != null)
            {
                statusBadgeText.color = normalized == "approved"
                    ? Color.white
                    : _defaultStatusTextColor;
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
    }
}
