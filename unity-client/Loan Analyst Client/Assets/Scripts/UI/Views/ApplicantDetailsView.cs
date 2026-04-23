using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoanAnalyst.UI.Views
{
    [DisallowMultipleComponent]
    public sealed class ApplicantDetailsView : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Header")]
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI applicantNameText;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Financial Summary")]
        [SerializeField] private TextMeshProUGUI incomeText;
        [SerializeField] private TextMeshProUGUI debtText;
        [SerializeField] private TextMeshProUGUI employmentYearsText;
        [SerializeField] private TextMeshProUGUI missedPaymentsText;
        [SerializeField] private TextMeshProUGUI requestedAmountText;

        [Header("Actions")]
        [SerializeField] private Button analyzeButton;
        [SerializeField] private Button approveButton;
        [SerializeField] private Button rejectOrEscalateButton;

        [Header("AI Results")]
        [SerializeField] private TextMeshProUGUI riskScoreText;
        [SerializeField] private TextMeshProUGUI riskLevelText;
        [SerializeField] private TextMeshProUGUI summaryText;
        [SerializeField] private TextMeshProUGUI recommendedActionText;

        [Header("Feedback")]
        [SerializeField] private TextMeshProUGUI statusInfoText;
        [SerializeField] private TextMeshProUGUI errorText;

        #endregion

        #region Properties

        public Button BackButton => backButton;
        public TextMeshProUGUI ApplicantNameText => applicantNameText;
        public TextMeshProUGUI StatusText => statusText;
        public TextMeshProUGUI IncomeText => incomeText;
        public TextMeshProUGUI DebtText => debtText;
        public TextMeshProUGUI EmploymentYearsText => employmentYearsText;
        public TextMeshProUGUI MissedPaymentsText => missedPaymentsText;
        public TextMeshProUGUI RequestedAmountText => requestedAmountText;
        public Button AnalyzeButton => analyzeButton;
        public Button ApproveButton => approveButton;
        public Button RejectOrEscalateButton => rejectOrEscalateButton;
        public TextMeshProUGUI RiskScoreText => riskScoreText;
        public TextMeshProUGUI RiskLevelText => riskLevelText;
        public TextMeshProUGUI SummaryText => summaryText;
        public TextMeshProUGUI RecommendedActionText => recommendedActionText;
        public TextMeshProUGUI StatusInfoText => statusInfoText;
        public TextMeshProUGUI ErrorText => errorText;

        #endregion
    }
}
