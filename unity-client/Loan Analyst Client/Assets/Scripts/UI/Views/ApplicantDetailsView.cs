using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LoanAnalyst.UI.Views
{
    [DisallowMultipleComponent]
    public sealed class ApplicantDetailsView : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI applicantNameText;
        [SerializeField] private Image statusBadgeBackground;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Financial Summary")]
        [SerializeField] private TextMeshProUGUI incomeText;
        [SerializeField] private TextMeshProUGUI debtText;
        [SerializeField] private TextMeshProUGUI requestedAmountText;

        [Header("Actions")]
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button analyzeButton;
        [SerializeField] private Button approveButton;

        [Header("AI Results")]
        [SerializeField] private TextMeshProUGUI riskScoreText;
        [SerializeField] private TextMeshProUGUI riskLevelText;
        [SerializeField] private TextMeshProUGUI summaryText;
        [SerializeField] private TextMeshProUGUI recommendedActionText;

        [Header("Feedback")]
        [SerializeField] private TextMeshProUGUI statusInfoText;
        [SerializeField] private TextMeshProUGUI errorText;

        private Color _defaultStatusBadgeColor;
        private Color _defaultStatusTextColor;

        public string ApplicantName
        {
            get => GetText(applicantNameText);
            set => SetText(applicantNameText, value);
        }

        public string ApplicantStatus
        {
            get => GetText(statusText);
            set
            {
                SetText(statusText, value);
                ApplyStatusTheme(value);
            }
        }

        public string Income
        {
            get => GetText(incomeText);
            set => SetText(incomeText, value);
        }

        public string Debt
        {
            get => GetText(debtText);
            set => SetText(debtText, value);
        }

        public string RequestedAmount
        {
            get => GetText(requestedAmountText);
            set => SetText(requestedAmountText, value);
        }

        public string RiskScore
        {
            get => GetText(riskScoreText);
            set => SetText(riskScoreText, value);
        }

        public string RiskLevel
        {
            get => GetText(riskLevelText);
            set => SetText(riskLevelText, value);
        }

        public string AnalysisSummary
        {
            get => GetText(summaryText);
            set => SetText(summaryText, value);
        }

        public string RecommendedAction
        {
            get => GetText(recommendedActionText);
            set => SetText(recommendedActionText, value);
        }

        public string StatusMessage
        {
            get => GetText(statusInfoText);
            set => SetFeedbackText(statusInfoText, "Status: ", value);
        }

        public string ErrorMessage
        {
            get => GetText(errorText);
            set => SetFeedbackText(errorText, "Error: ", value);
        }

        public bool RefreshInteractable
        {
            get => GetInteractable(refreshButton);
            set => SetInteractable(refreshButton, value);
        }

        public bool AnalyzeInteractable
        {
            get => GetInteractable(analyzeButton);
            set => SetInteractable(analyzeButton, value);
        }

        public bool ApproveInteractable
        {
            get => GetInteractable(approveButton);
            set => SetInteractable(approveButton, value);
        }

        public bool ApproveVisible
        {
            get => GetVisible(approveButton);
            set => SetVisible(approveButton, value);
        }

        private void Awake()
        {
            _defaultStatusBadgeColor = statusBadgeBackground != null
                ? statusBadgeBackground.color
                : Color.white;
            _defaultStatusTextColor = statusText != null
                ? statusText.color
                : Color.white;

            ConfigureSummaryText();
        }

        public void BindBackAction(UnityAction action)
        {
            ReplaceButtonHandler(backButton, action);
        }

        public void BindRefreshAction(UnityAction action)
        {
            ReplaceButtonHandler(refreshButton, action);
        }

        public void BindAnalyzeAction(UnityAction action)
        {
            ReplaceButtonHandler(analyzeButton, action);
        }

        public void BindApproveAction(UnityAction action)
        {
            ReplaceButtonHandler(approveButton, action);
        }

        public void ClearApplicantSummary()
        {
            ApplicantName = string.Empty;
            ApplicantStatus = string.Empty;
            Income = string.Empty;
            Debt = string.Empty;
            RequestedAmount = string.Empty;
        }

        public void ClearAnalysis()
        {
            RiskScore = string.Empty;
            RiskLevel = string.Empty;
            AnalysisSummary = "Run analysis to populate this area.";
            RecommendedAction = string.Empty;
        }

        private void ConfigureSummaryText()
        {
            if (summaryText == null)
            {
                return;
            }

            summaryText.enableWordWrapping = true;
            summaryText.overflowMode = TextOverflowModes.Overflow;

            var layoutElement = summaryText.GetComponent<LayoutElement>();
            if (layoutElement != null && layoutElement.preferredHeight < 140f)
            {
                layoutElement.preferredHeight = 140f;
            }
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

            if (statusText != null)
            {
                statusText.color = normalized == "approved"
                    ? Color.white
                    : _defaultStatusTextColor;
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

        private static bool GetInteractable(Selectable selectable)
        {
            return selectable != null && selectable.interactable;
        }

        private static void SetInteractable(Selectable selectable, bool value)
        {
            if (selectable != null)
            {
                selectable.interactable = value;
            }
        }

        private static bool GetVisible(Component component)
        {
            return component != null && component.gameObject.activeSelf;
        }

        private static void SetVisible(Component component, bool value)
        {
            if (component != null)
            {
                component.gameObject.SetActive(value);
            }
        }
    }
}
