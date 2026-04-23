using LoanAnalyst.Client.Core;
using LoanAnalyst.Client.Models;
using LoanAnalyst.Client.Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LoanAnalyst.Client.Controllers
{
    public class ApplicantDetailController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI applicantSummaryText;
        [SerializeField] private TextMeshProUGUI analysisText;
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

            SetError(string.Empty);
            SetStatus("Loading applicant...");
            analysisText.text = string.Empty;

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
                analysisText.text =
                    $"Risk Score: {result.riskScore}\nRecommendation: {result.recommendation}\nReason: {result.reason}\nModel: {result.modelVersion}";
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
                var approvedAt = !string.IsNullOrWhiteSpace(result?.applicant?.approvedAt)
                    ? result.applicant.approvedAt
                    : result?.applicant?.aprovedAt;

                SetStatus($"{result.message} at {approvedAt}");
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
                    applicantSummaryText.text = string.Empty;
                    SetStatus(string.Empty);
                    SetError("Applicant not found.");
                    return;
                }

                applicantSummaryText.text =
                    $"Id: {_currentApplicant.id}\n" +
                    $"Name: {_currentApplicant.fullName}\n";

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
