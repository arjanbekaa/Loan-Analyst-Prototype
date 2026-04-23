using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoanAnalyst.UI.Views
{
    [DisallowMultipleComponent]
    public sealed class ApplicantsListView : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Header")]
        [SerializeField] private TextMeshProUGUI headerTitleText;
        [SerializeField] private TextMeshProUGUI roleText;
        [SerializeField] private Button logoutButton;

        [Header("Filters")]
        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private Button refreshButton;

        [Header("List")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform contentRoot;
        [SerializeField] private TextMeshProUGUI emptyStateText;
        [SerializeField] private ApplicantListItemView itemTemplate;

        #endregion

        #region Properties

        public TextMeshProUGUI HeaderTitleText => headerTitleText;
        public TextMeshProUGUI RoleText => roleText;
        public Button LogoutButton => logoutButton;
        public TMP_InputField SearchInput => searchInput;
        public Button RefreshButton => refreshButton;
        public ScrollRect ScrollRect => scrollRect;
        public RectTransform ContentRoot => contentRoot;
        public TextMeshProUGUI EmptyStateText => emptyStateText;
        public ApplicantListItemView ItemTemplate => itemTemplate;

        #endregion
    }
}
