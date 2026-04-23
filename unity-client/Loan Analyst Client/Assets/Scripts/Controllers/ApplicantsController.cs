using LoanAnalyst.Client.Core;
using LoanAnalyst.Client.Models;
using LoanAnalyst.Client.Services;
using LoanAnalyst.UI.Views;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LoanAnalyst.Client.Controllers
{
    public class ApplicantsController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private RectTransform listContent;
        [SerializeField] private Button applicantRowTemplate;

        private ApplicantService _applicantService;

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

            headerText.text = $"Operator: {UserSession.DisplayName} ({UserSession.NormalizedRole})";
            SetStatus("Loading applicants...");
            SetError(string.Empty);

            refreshButton.onClick.AddListener(OnRefreshClicked);
            logoutButton.onClick.AddListener(OnLogoutClicked);

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
            refreshButton.interactable = false;
            SetError(string.Empty);
            SetStatus("Loading applicants...");

            try
            {
                ClearList();
                var response = await _applicantService.GetApplicantsAsync();
                var applicants = response?.applicants;

                if (applicants == null || applicants.Length == 0)
                {
                    SetStatus(string.Empty);
                    SetError("No applicants available.");
                    return;
                }

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
                refreshButton.interactable = true;
            }
        }

        private void AddApplicantRow(ApplicantDto applicant)
        {
            var row = Instantiate(applicantRowTemplate, listContent);
            row.gameObject.SetActive(true);

            var itemView = row.GetComponent<ApplicantListItemView>();
            if (itemView != null)
            {
                itemView.ApplicantNameText.text = applicant.fullName;
                itemView.RequestedAmountText.text = $"Requested: {applicant.requestedAmount}";
                itemView.StatusBadgeText.text = string.IsNullOrWhiteSpace(applicant.status) ? "UNKNOWN" : applicant.status.ToUpperInvariant();
                itemView.OpenDetailsButton.onClick.RemoveAllListeners();
                itemView.OpenDetailsButton.onClick.AddListener(() => OpenApplicant(applicant.id));
            }
            else
            {
                var text = row.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = $"{applicant.id} | {applicant.fullName} | {applicant.status} | {applicant.requestedAmount}";
                }
            }

            row.onClick.RemoveAllListeners();
            row.onClick.AddListener(() => OpenApplicant(applicant.id));
        }

        private void OpenApplicant(string applicantId)
        {
            AppState.SelectedApplicantId = applicantId;
            SceneManager.LoadScene("ApplicantDetailScene");
        }

        private void ClearList()
        {
            for (var i = listContent.childCount - 1; i >= 0; i--)
            {
                var child = listContent.GetChild(i);
                if (child == applicantRowTemplate.transform)
                {
                    continue;
                }
                Destroy(child.gameObject);
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
