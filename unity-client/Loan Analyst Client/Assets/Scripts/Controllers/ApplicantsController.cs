using LoanAnalyst.Client.Core;
using LoanAnalyst.Client.Models;
using LoanAnalyst.Client.Services;
using LoanAnalyst.UI.Views;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LoanAnalyst.Client.Controllers
{
    public class ApplicantsController : MonoBehaviour
    {
        [SerializeField] private ApplicantsListView applicantsListView;

        private ApplicantService _applicantService;

        private void Awake()
        {
            _applicantService = new ApplicantService(new ApiClient());
            if (applicantsListView == null)
            {
                applicantsListView = FindAnyObjectByType<ApplicantsListView>(FindObjectsInactive.Include);
            }
        }

        private async void Start()
        {
            if (!UserSession.IsLoggedIn)
            {
                SceneManager.LoadScene("LoginScene");
                return;
            }

            if (applicantsListView == null)
            {
                Debug.LogError("ApplicantsListView is not assigned.");
                return;
            }

            applicantsListView.SignedInRole = $"Signed in as: {UserSession.DisplayName} ({UserSession.NormalizedRole})";
            applicantsListView.EmptyStateVisible = false;
            applicantsListView.EmptyStateMessage = string.Empty;
            SetStatus("Loading applicants...");
            SetError(string.Empty);

            applicantsListView.BindRefreshAction(OnRefreshClicked);
            applicantsListView.BindLogoutAction(OnLogoutClicked);

            await LoadApplicantsAsync();
        }

        private async void OnRefreshClicked()
        {
            await LoadApplicantsAsync();
        }

        private void OnLogoutClicked()
        {
            UserSession.Clear();
            AppState.SelectedApplicantId = null;
            SceneManager.LoadScene("LoginScene");
        }

        private async System.Threading.Tasks.Task LoadApplicantsAsync()
        {
            applicantsListView.RefreshInteractable = false;
            SetError(string.Empty);
            SetStatus("Loading applicants...");

            try
            {
                applicantsListView.ClearItems();
                var response = await _applicantService.GetApplicantsAsync();
                var applicants = response?.applicants;

                if (applicants == null || applicants.Length == 0)
                {
                    SetStatus(string.Empty);
                    SetError(string.Empty);
                    applicantsListView.EmptyStateVisible = true;
                    applicantsListView.EmptyStateMessage = "No applicants available. Refresh to load the queue.";
                    return;
                }

                applicantsListView.EmptyStateVisible = false;
                applicantsListView.EmptyStateMessage = string.Empty;

                foreach (var applicant in applicants)
                {
                    AddApplicantRow(applicant);
                }

                SetStatus($"Loaded {applicants.Length} applicants.");
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
                applicantsListView.RefreshInteractable = true;
            }
        }

        private void AddApplicantRow(ApplicantDto applicant)
        {
            var rowView = applicantsListView.CreateItem();
            if (rowView == null)
            {
                return;
            }

            rowView.ApplicantName = applicant.fullName;
            rowView.RequestedAmount = FormatCurrency(applicant.requestedAmount);
            rowView.StatusBadge = FormatStatus(applicant.status);
            rowView.BindOpenDetailsAction(() => OpenApplicant(applicant.id));
        }

        private void OpenApplicant(string applicantId)
        {
            AppState.SelectedApplicantId = applicantId;
            SceneManager.LoadScene("ApplicantDetailScene");
        }

        private void SetError(string value)
        {
            applicantsListView.ErrorMessage = value;
        }

        private void SetStatus(string value)
        {
            applicantsListView.StatusMessage = value;
        }

        private static string FormatStatus(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "UNKNOWN"
                : value.Trim().ToUpperInvariant();
        }

        private static string FormatCurrency(float value)
        {
            return $"Requested: ${value:N0}";
        }
    }
}
