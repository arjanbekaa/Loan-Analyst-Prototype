using System;

namespace LoanAnalyst.Client.Models
{
    [Serializable]
    public class ApplicantListResponse
    {
        public ApplicantDto[] applicants;
    }

    [Serializable]
    public class ApplicantDto
    {
        public string id;
        public string fullName;
        public float requestedAmount;
        public float monthlyIncome;
        public float monthlyDebtPayments;
        public int creditScore;
        public float loanAmount;
        public int loanTermMonths;
        public string status;
        public string decision;
    }

    [Serializable]
    public class AnalyzeRequest
    {
        public float monthlyIncome;
        public float monthlyDebtPayments;
        public int creditScore;
        public float loanAmount;
        public int loanTermMonths;
    }

    [Serializable]
    public class AnalyzeResponse
    {
        public string applicantId;
        public float riskScore;
        public string recommendation;
        public string reason;
        public string modelVersion;
    }

    [Serializable]
    public class ApproveBy
    {
        public string id;
        public string username;
        public string role;
    }

    [Serializable]
    public class ApproveResponse
    {
        public string message;
        public ApplicantApproved applicant;
        public string error;
    }

    [Serializable]
    public class ApplicantApproved
    {
        public string id;
        public string fullName;
        public float requestedAmount;
        public float monthlyIncome;
        public int creditScore;
        public string status;
        public string approvedAt;
        public ApproveBy approvedBy;
        // Backend currently has a typo in key name: aprovedAt/aprovedBy.
        public string aprovedAt;
        public ApproveBy aprovedBy;
    }
}
