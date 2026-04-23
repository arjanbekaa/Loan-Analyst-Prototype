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
        [SerializeField] private ApplicantsListView applicantsListView;
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
            BindViewReferences();
        }

        private async void Start()
        {
            if (!UserSession.IsLoggedIn)
            {
                SceneManager.LoadScene("LoginScene");
                return;
            }

            if (applicantsListView?.RoleText != null)
            {
                applicantsListView.RoleText.text = $"Signed in as: {UserSession.DisplayName} ({UserSession.NormalizedRole})";
            }

            SetEmptyState(false, string.Empty);
            SetStatus("Loading applicants...");
            SetError(string.Empty);

            if (refreshButton != null)
            {
                refreshButton.onClick.AddListener(OnRefreshClicked);
            }

            if (logoutButton != null)
            {
                logoutButton.onClick.AddListener(OnLogoutClicked);
            }

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
            if (refreshButton != null)
            {
                refreshButton.interactable = false;
            }

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
                    SetError(string.Empty);
                    SetEmptyState(true, "No applicants available. Refresh to load the queue.");
                    return;
                }

                SetEmptyState(false, string.Empty);

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
                if (refreshButton != null)
                {
                    refreshButton.interactable = true;
                }
            }
        }

        private void AddApplicantRow(ApplicantDto applicant)
        {
            var itemTemplate = applicantsListView?.ItemTemplate;
            if (itemTemplate != null && listContent != null)
            {
                var rowView = Instantiate(itemTemplate, listContent);
                rowView.gameObject.SetActive(true);

                if (rowView.ApplicantNameText != null)
                {
                    rowView.ApplicantNameText.text = applicant.fullName;
                }

                if (rowView.RequestedAmountText != null)
                {
                    rowView.RequestedAmountText.text = FormatCurrency(applicant.requestedAmount);
                }

                if (rowView.StatusBadgeText != null)
                {
                    rowView.StatusBadgeText.text = FormatStatus(applicant.status);
                }

                if (rowView.OpenDetailsButton != null)
                {
                    rowView.OpenDetailsButton.onClick.RemoveAllListeners();
                    rowView.OpenDetailsButton.onClick.AddListener(() => OpenApplicant(applicant.id));
                }

                return;
            }

            if (applicantRowTemplate == null || listContent == null)
            {
                return;
            }

            var row = Instantiate(applicantRowTemplate, listContent);
            row.gameObject.SetActive(true);

            var itemView = row.GetComponent<ApplicantListItemView>();
            if (itemView != null)
            {
                itemView.ApplicantNameText.text = applicant.fullName;
                itemView.RequestedAmountText.text = FormatCurrency(applicant.requestedAmount);
                itemView.StatusBadgeText.text = FormatStatus(applicant.status);
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
            if (listContent == null)
            {
                return;
            }

            var itemTemplateTransform = applicantsListView?.ItemTemplate != null
                ? applicantsListView.ItemTemplate.transform
                : null;
            var buttonTemplateTransform = applicantRowTemplate != null
                ? applicantRowTemplate.transform
                : null;

            for (var i = listContent.childCount - 1; i >= 0; i--)
            {
                var child = listContent.GetChild(i);
                if (child == itemTemplateTransform || child == buttonTemplateTransform)
                {
                    continue;
                }
                Destroy(child.gameObject);
            }
        }

        private void BindViewReferences()
        {
            if (applicantsListView == null)
            {
                applicantsListView = FindAnyObjectByType<ApplicantsListView>(FindObjectsInactive.Include);
            }

            if (applicantsListView == null)
            {
                return;
            }

            refreshButton = applicantsListView.RefreshButton != null
                ? applicantsListView.RefreshButton
                : refreshButton;
            logoutButton = applicantsListView.LogoutButton != null
                ? applicantsListView.LogoutButton
                : logoutButton;
            listContent = applicantsListView.ContentRoot != null
                ? applicantsListView.ContentRoot
                : listContent;
        }

        private void SetEmptyState(bool isVisible, string value)
        {
            if (applicantsListView?.EmptyStateText == null)
            {
                return;
            }

            applicantsListView.EmptyStateText.gameObject.SetActive(isVisible);
            applicantsListView.EmptyStateText.text = value;
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
