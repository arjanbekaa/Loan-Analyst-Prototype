using System.Linq;
using System.Threading.Tasks;
using LoanAnalyst.Client.Models;

namespace LoanAnalyst.Client.Services
{
    [System.Serializable]
    public class EmptyBody
    {
    }

    public class ApplicantService
    {
        private readonly ApiClient _apiClient;

        public ApplicantService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Task<ApplicantListResponse> GetApplicantsAsync()
        {
            return _apiClient.GetAsync<ApplicantListResponse>("/applicants", authorized: true);
        }

        // Backend currently does not expose GET /applicants/:id, so we derive details from list response.
        public async Task<ApplicantDto> GetApplicantDetailAsync(string applicantId)
        {
            var list = await GetApplicantsAsync();
            return list?.applicants?.FirstOrDefault(a => a.id == applicantId);
        }

        public Task<AnalyzeResponse> AnalyzeAsync(string applicantId, AnalyzeRequest payload)
        {
            return _apiClient.PostAsync<AnalyzeRequest, AnalyzeResponse>($"/applicants/{applicantId}/analyze", payload, authorized: true);
        }

        public Task<ApproveResponse> ApproveAsync(string applicantId)
        {
            return _apiClient.PostAsync<EmptyBody, ApproveResponse>($"/applicants/{applicantId}/approve", new EmptyBody(), authorized: true);
        }
    }
}
