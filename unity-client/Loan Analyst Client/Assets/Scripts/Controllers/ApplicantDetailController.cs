using LoanAnalyst.Client.Core;
using LoanAnalyst.Client.Models;
using LoanAnalyst.Client.Services;
using LoanAnalyst.UI.Views;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LoanAnalyst.Client.Controllers
{
    public class ApplicantDetailController : MonoBehaviour
    {
        [SerializeField] private ApplicantDetailsView applicantDetailsView;
        [SerializeField] private TextMeshProUGUI headerText;

        private ApplicantService _applicantService;
        private ApplicantDto _currentApplicant;

        private void Awake()
        {
            _applicantService = new ApplicantService(new ApiClient());
            if (applicantDetailsView == null)
            {
                applicantDetailsView = FindAnyObjectByType<ApplicantDetailsView>(FindObjectsInactive.Include);
            }
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

            if (applicantDetailsView == null)
            {
                Debug.LogError("ApplicantDetailsView is not assigned.");
                return;
            }

            headerText.text = $"Operator: {UserSession.DisplayName} ({UserSession.NormalizedRole})";
            ConfigureApproveButton();

            applicantDetailsView.BindBackAction(() => SceneManager.LoadScene("ApplicantsScene"));
            applicantDetailsView.BindRefreshAction(OnRefreshClicked);
            applicantDetailsView.BindAnalyzeAction(OnAnalyzeClicked);
            applicantDetailsView.BindApproveAction(OnApproveClicked);
            SetError(string.Empty);
            SetStatus("Loading applicant...");
            applicantDetailsView.ClearAnalysis();

            await LoadDetailAsync();
        }

        private void ConfigureApproveButton()
        {
            if (applicantDetailsView == null)
            {
                return;
            }

            applicantDetailsView.ApproveVisible = UserSession.IsManager;
            applicantDetailsView.ApproveInteractable = UserSession.IsManager && !IsApproved(_currentApplicant);

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
            applicantDetailsView.AnalyzeInteractable = false;

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
                applicantDetailsView.AnalyzeInteractable = true;
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
            applicantDetailsView.ApproveInteractable = false;

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
                applicantDetailsView.ApproveInteractable = UserSession.IsManager && !IsApproved(_currentApplicant);
            }
        }

        private async System.Threading.Tasks.Task LoadDetailAsync()
        {
            applicantDetailsView.RefreshInteractable = false;
            SetError(string.Empty);
            SetStatus("Loading applicant...");

            try
            {
                _currentApplicant = await _applicantService.GetApplicantDetailAsync(AppState.SelectedApplicantId);
                if (_currentApplicant == null)
                {
                    applicantDetailsView.ClearApplicantSummary();
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
                applicantDetailsView.RefreshInteractable = true;
            }
        }

        private void RenderApplicantSummary(ApplicantDto applicant)
        {
            if (applicant == null)
            {
                return;
            }

            applicantDetailsView.ApplicantName = applicant.fullName;
            applicantDetailsView.ApplicantStatus = FormatEnumText(applicant.status);
            applicantDetailsView.Income = FormatCurrency(applicant.monthlyIncome);
            applicantDetailsView.Debt = FormatCurrency(applicant.monthlyDebtPayments);
            applicantDetailsView.RequestedAmount = FormatCurrency(applicant.requestedAmount);
            applicantDetailsView.ApproveInteractable = UserSession.IsManager && !IsApproved(applicant);
        }

        private void RenderAnalysis(AnalyzeResponse result, string reasonsText, string dtiText, string monthlyPaymentText)
        {
            var formattedPaymentText = monthlyPaymentText == "n/a"
                ? monthlyPaymentText
                : $"${monthlyPaymentText}";

            applicantDetailsView.RiskScore = result.riskScore.ToString();
            applicantDetailsView.RiskLevel = FormatEnumText(result.riskLevel);
            applicantDetailsView.AnalysisSummary = $"{reasonsText}\nDTI: {dtiText}\nEstimated payment: {formattedPaymentText}";
            applicantDetailsView.RecommendedAction = BuildRecommendedAction(result.riskLevel);
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
            var normalized = riskLevel?.Trim().ToUpperInvariant();

            return normalized switch
            {
                "LOW" => "Recommend approval.",
                "MEDIUM" => "Recommend manual review.",
                "HIGH" => "Recommend escalation or rejection.",
                _ => "No recommendation available.",
            };
        }

        private static bool IsApproved(ApplicantDto applicant)
        {
            if (applicant == null)
            {
                return false;
            }

            return string.Equals(applicant.status, "approved", System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(applicant.decision, "approved", System.StringComparison.OrdinalIgnoreCase);
        }

        private void SetError(string value)
        {
            applicantDetailsView.ErrorMessage = value;
        }

        private void SetStatus(string value)
        {
            applicantDetailsView.StatusMessage = value;
        }
    }
}
