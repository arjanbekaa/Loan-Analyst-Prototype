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
        public int riskScore;
        public string riskLevel;
        public string[] reasons;
        public AnalysisMetricDto metric;
    }

    [Serializable]
    public class AnalysisMetricDto
    {
        public float dti;
        public float estimatedMonthlyPayment;
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
        public string decision;
        public string approvedAt;
        public ApproveBy approvedBy;
    }
}
