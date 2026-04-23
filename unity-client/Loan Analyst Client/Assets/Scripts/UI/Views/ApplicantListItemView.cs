using TMPro;
using UnityEngine;
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

        #region Properties

        public TextMeshProUGUI ApplicantNameText => applicantNameText;
        public TextMeshProUGUI RequestedAmountText => requestedAmountText;
        public TextMeshProUGUI StatusBadgeText => statusBadgeText;
        public Image StatusBadgeBackground => statusBadgeBackground;
        public Button OpenDetailsButton => openDetailsButton;

        #endregion
    }
}
