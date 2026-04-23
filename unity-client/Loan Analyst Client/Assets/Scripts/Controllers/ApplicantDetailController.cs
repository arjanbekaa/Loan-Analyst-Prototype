using LoanAnalyst.Client.Core;
using LoanAnalyst.Client.Models;
using LoanAnalyst.Client.Services;
using LoanAnalyst.UI.Views;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LoanAnalyst.Client.Controllers
{
    public class ApplicantDetailController : MonoBehaviour
    {
        [SerializeField] private ApplicantDetailsView applicantDetailsView;
        [SerializeField] private TextMeshProUGUI headerText;
        [FormerlySerializedAs("applicantSummaryText")]
        [SerializeField] private TextMeshProUGUI applicantNameText;
        [SerializeField] private TextMeshProUGUI analysisText;
        [SerializeField] private TextMeshProUGUI incomeText;
        [SerializeField] private TextMeshProUGUI debtText;
        [SerializeField] private TextMeshProUGUI requestedAmountText;
        
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button backButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button analyzeButton;
        [SerializeField] private Button approveButton;

        private ApplicantService _applicantService;
        private ApplicantDto _currentApplicant;

        private void Awake()
        {
            _applicantService = new ApplicantService(new ApiClient());
            BindViewReferences();
        }

        private async void Start()
        {
            if (!UserSession.IsLoggedIn)
            {
                SceneManager.LoadScene("LoginScene");
                return;
            }

            if (string.IsNullOrWhiteSpace(AppState.SelectedApplicantId))
            {
                SceneManager.LoadScene("ApplicantsScene");
                return;
            }

            headerText.text = $"Operator: {UserSession.DisplayName} ({UserSession.NormalizedRole})";
            ConfigureApproveButton();

            backButton.onClick.AddListener(() => SceneManager.LoadScene("ApplicantsScene"));
            refreshButton.onClick.AddListener(OnRefreshClicked);
            analyzeButton.onClick.AddListener(OnAnalyzeClicked);
            approveButton.onClick.AddListener(OnApproveClicked);

            ConfigureAnalysisText();
            SetError(string.Empty);
            SetStatus("Loading applicant...");
            ClearAnalysis();

            await LoadDetailAsync();
        }

        private void ConfigureApproveButton()
        {
            if (approveButton == null)
            {
                return;
            }

            approveButton.gameObject.SetActive(true);
            approveButton.interactable = UserSession.IsManager;

            if (!UserSession.IsManager)
            {
                SetStatus("Approve is disabled because this operator is not a manager.");
            }
        }

        private async void OnRefreshClicked()
        {
            await LoadDetailAsync();
        }

        private async void OnAnalyzeClicked()
        {
            if (_currentApplicant == null)
            {
                SetError("Applicant detail is not loaded.");
                return;
            }

            SetError(string.Empty);
            SetStatus("Running AI analysis...");
            analyzeButton.interactable = false;

            try
            {
                var request = new AnalyzeRequest
                {
                    monthlyIncome = _currentApplicant.monthlyIncome,
                    monthlyDebtPayments = _currentApplicant.monthlyDebtPayments > 0 ? _currentApplicant.monthlyDebtPayments : 1000f,
                    creditScore = _currentApplicant.creditScore,
                    loanAmount = _currentApplicant.requestedAmount > 0 ? _currentApplicant.requestedAmount : _currentApplicant.loanAmount,
                    loanTermMonths = _currentApplicant.loanTermMonths > 0 ? _currentApplicant.loanTermMonths : 36
                };

                var result = await _applicantService.AnalyzeAsync(_currentApplicant.id, request);

                var reasonsText = result.reasons != null && result.reasons.Length > 0
                    ? string.Join(", ", result.reasons)
                    : "No reasons returned";

                var dtiText = result.metric != null
                    ? result.metric.dti.ToString("0.00")
                    : "n/a";

                var monthlyPaymentText = result.metric != null
                    ? result.metric.estimatedMonthlyPayment.ToString("0.00")
                    : "n/a";

                RenderAnalysis(result, reasonsText, dtiText, monthlyPaymentText);
                SetStatus("Analysis complete.");
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
                analyzeButton.interactable = true;
            }
        }

        private async void OnApproveClicked()
        {
            if (!UserSession.IsManager)
            {
                SetError("Approve action requires manager role.");
                return;
            }

            if (_currentApplicant == null)
            {
                SetError("Applicant detail is not loaded.");
                return;
            }

            SetError(string.Empty);
            SetStatus("Approving applicant...");
            approveButton.interactable = false;

            try
            {
                var result = await _applicantService.ApproveAsync(_currentApplicant.id);
                SetStatus($"{result.message} at {result?.applicant?.approvedAt}");
                await LoadDetailAsync();
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
                approveButton.interactable = true;
            }
        }

        private async System.Threading.Tasks.Task LoadDetailAsync()
        {
            refreshButton.interactable = false;
            SetError(string.Empty);
            SetStatus("Loading applicant...");

            try
            {
                _currentApplicant = await _applicantService.GetApplicantDetailAsync(AppState.SelectedApplicantId);
                if (_currentApplicant == null)
                {
                    ClearApplicantSummary();
                    SetStatus(string.Empty);
                    SetError("Applicant not found.");
                    return;
                }

                RenderApplicantSummary(_currentApplicant);

                SetStatus("Applicant loaded.");
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
                refreshButton.interactable = true;
            }
        }

        private void BindViewReferences()
        {
            if (applicantDetailsView == null)
            {
                applicantDetailsView = FindAnyObjectByType<ApplicantDetailsView>(FindObjectsInactive.Include);
            }

            if (applicantDetailsView == null)
            {
                return;
            }

            applicantNameText = applicantDetailsView.ApplicantNameText != null
                ? applicantDetailsView.ApplicantNameText
                : applicantNameText;
            incomeText = applicantDetailsView.IncomeText != null
                ? applicantDetailsView.IncomeText
                : incomeText;
            debtText = applicantDetailsView.DebtText != null
                ? applicantDetailsView.DebtText
                : debtText;
            requestedAmountText = applicantDetailsView.RequestedAmountText != null
                ? applicantDetailsView.RequestedAmountText
                : requestedAmountText;
            backButton = applicantDetailsView.BackButton != null
                ? applicantDetailsView.BackButton
                : backButton;
            analyzeButton = applicantDetailsView.AnalyzeButton != null
                ? applicantDetailsView.AnalyzeButton
                : analyzeButton;
            approveButton = applicantDetailsView.ApproveButton != null
                ? applicantDetailsView.ApproveButton
                : approveButton;
            statusText = applicantDetailsView.StatusInfoText != null
                ? applicantDetailsView.StatusInfoText
                : statusText;
            errorText = applicantDetailsView.ErrorText != null
                ? applicantDetailsView.ErrorText
                : errorText;
        }

        private void RenderApplicantSummary(ApplicantDto applicant)
        {
            if (applicant == null)
            {
                return;
            }

            if (applicantDetailsView != null)
            {
                if (applicantDetailsView.ApplicantNameText != null)
                {
                    applicantDetailsView.ApplicantNameText.text = applicant.fullName;
                }

                if (applicantDetailsView.StatusText != null)
                {
                    applicantDetailsView.StatusText.text = FormatEnumText(applicant.status);
                }

                if (applicantDetailsView.IncomeText != null)
                {
                    applicantDetailsView.IncomeText.text = FormatCurrency(applicant.monthlyIncome);
                }

                if (applicantDetailsView.DebtText != null)
                {
                    applicantDetailsView.DebtText.text = FormatCurrency(applicant.monthlyDebtPayments);
                }

                if (applicantDetailsView.RequestedAmountText != null)
                {
                    applicantDetailsView.RequestedAmountText.text = FormatCurrency(applicant.requestedAmount);
                }

                return;
            }

            if (applicantNameText != null)
            {
                applicantNameText.text = $"Name: {applicant.fullName}";
            }

            if (incomeText != null)
            {
                incomeText.text = $"Income: {applicant.monthlyIncome}";
            }

            if (debtText != null)
            {
                debtText.text = $"Debt: {applicant.monthlyDebtPayments}";
            }

            if (requestedAmountText != null)
            {
                requestedAmountText.text = $"Requested Amount: {applicant.requestedAmount}";
            }
        }

        private void RenderAnalysis(AnalyzeResponse result, string reasonsText, string dtiText, string monthlyPaymentText)
        {
            if (applicantDetailsView != null)
            {
                if (applicantDetailsView.RiskScoreText != null)
                {
                    applicantDetailsView.RiskScoreText.text = result.riskScore.ToString();
                }

                if (applicantDetailsView.RiskLevelText != null)
                {
                    applicantDetailsView.RiskLevelText.text = FormatEnumText(result.riskLevel);
                }

                if (applicantDetailsView.SummaryText != null)
                {
                    var formattedPaymentText = monthlyPaymentText == "n/a"
                        ? monthlyPaymentText
                        : $"${monthlyPaymentText}";

                    applicantDetailsView.SummaryText.text = $"{reasonsText}\nDTI: {dtiText}\nEstimated payment: {formattedPaymentText}";
                }

                if (applicantDetailsView.RecommendedActionText != null)
                {
                    applicantDetailsView.RecommendedActionText.text = BuildRecommendedAction(result.riskLevel);
                }

                return;
            }

            if (analysisText != null)
            {
                analysisText.text =
                    $"Risk Score: {result.riskScore}\n" +
                    $"Risk Level: {result.riskLevel}\n" +
                    $"Reasons: {reasonsText}\n" +
                    $"DTI: {dtiText}\n" +
                    $"Estimated Monthly Payment: {monthlyPaymentText}";
            }
        }

        private void ConfigureAnalysisText()
        {
            if (analysisText == null)
            {
                return;
            }

            analysisText.enableWordWrapping = true;
            analysisText.overflowMode = TextOverflowModes.Overflow;

            var layoutElement = analysisText.GetComponent<LayoutElement>();
            if (layoutElement != null && layoutElement.preferredHeight < 140f)
            {
                layoutElement.preferredHeight = 140f;
            }
        }

        private void ClearApplicantSummary()
        {
            if (applicantDetailsView != null)
            {
                if (applicantDetailsView.ApplicantNameText != null)
                {
                    applicantDetailsView.ApplicantNameText.text = string.Empty;
                }

                if (applicantDetailsView.StatusText != null)
                {
                    applicantDetailsView.StatusText.text = string.Empty;
                }

                if (applicantDetailsView.IncomeText != null)
                {
                    applicantDetailsView.IncomeText.text = string.Empty;
                }

                if (applicantDetailsView.DebtText != null)
                {
                    applicantDetailsView.DebtText.text = string.Empty;
                }

                if (applicantDetailsView.RequestedAmountText != null)
                {
                    applicantDetailsView.RequestedAmountText.text = string.Empty;
                }

                return;
            }

            if (applicantNameText != null)
            {
                applicantNameText.text = string.Empty;
            }

            if (incomeText != null)
            {
                incomeText.text = string.Empty;
            }

            if (debtText != null)
            {
                debtText.text = string.Empty;
            }

            if (requestedAmountText != null)
            {
                requestedAmountText.text = string.Empty;
            }
        }

        private void ClearAnalysis()
        {
            if (applicantDetailsView != null)
            {
                if (applicantDetailsView.RiskScoreText != null)
                {
                    applicantDetailsView.RiskScoreText.text = string.Empty;
                }

                if (applicantDetailsView.RiskLevelText != null)
                {
                    applicantDetailsView.RiskLevelText.text = string.Empty;
                }

                if (applicantDetailsView.SummaryText != null)
                {
                    applicantDetailsView.SummaryText.text = "Run analysis to populate this area.";
                }

                if (applicantDetailsView.RecommendedActionText != null)
                {
                    applicantDetailsView.RecommendedActionText.text = string.Empty;
                }
            }

            if (analysisText != null)
            {
                analysisText.text = string.Empty;
            }
        }

        private static string FormatCurrency(float value)
        {
            return $"${value:N0}";
        }

        private static string FormatEnumText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var normalized = value.Trim().ToLowerInvariant();
            return char.ToUpperInvariant(normalized[0]) + normalized.Substring(1);
        }

        private static string BuildRecommendedAction(string riskLevel)
        {
            return riskLevel switch
            {
                "LOW" => "Recommend approval.",
                "MEDIUM" => "Recommend manual review.",
                "HIGH" => "Recommend escalation or rejection.",
                _ => "No recommendation available.",
            };
        }

        private void SetError(string value)
        {
            if (errorText != null)
            {
                errorText.text = "Error: " + value;
            }
        }

        private void SetStatus(string value)
        {
            if (statusText != null)
            {
                statusText.text = "Status: " + value;
            }
        }
    }
}
