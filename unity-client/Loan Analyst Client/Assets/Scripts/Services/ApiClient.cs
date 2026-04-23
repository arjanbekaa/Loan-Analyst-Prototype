using System.Text;
using System.Threading.Tasks;
using LoanAnalyst.Client.Core;
using LoanAnalyst.Client.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace LoanAnalyst.Client.Services
{
    public class ApiClient
    {
        public async Task<TResponse> GetAsync<TResponse>(string path, bool authorized = true) where TResponse : class
        {
            return await SendAsync<object, TResponse>(UnityWebRequest.kHttpVerbGET, path, null, authorized);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest payload, bool authorized = true) where TResponse : class
        {
            return await SendAsync<TRequest, TResponse>(UnityWebRequest.kHttpVerbPOST, path, payload, authorized);
        }

        private async Task<TResponse> SendAsync<TRequest, TResponse>(string method, string path, TRequest payload, bool authorized) where TResponse : class
        {
            var url = $"{ApiConfig.BaseUrl}{path}";
            var request = new UnityWebRequest(url, method);

            if (payload != null)
            {
                var json = JsonUtility.ToJson(payload);
                var bytes = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bytes);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            if (authorized)
            {
                if (string.IsNullOrWhiteSpace(UserSession.JwtToken))
                {
                    throw new ApiException(401, "Missing JWT token in Unity session.");
                }
                request.SetRequestHeader("Authorization", $"Bearer {UserSession.JwtToken}");
            }

            await request.SendAsync();

            var body = request.ReadBodySafe();
            if (request.HasNetworkError())
            {
                throw new ApiException(0, $"Network error: {request.error}");
            }

            if (request.responseCode < 200 || request.responseCode >= 300)
            {
                var parsedError = SafeParse<ApiErrorResponse>(body);
                var errorMessage = parsedError != null && !string.IsNullOrWhiteSpace(parsedError.message)
                    ? parsedError.message
                    : $"Request failed ({request.responseCode})";

                if (parsedError != null && !string.IsNullOrWhiteSpace(parsedError.error))
                {
                    errorMessage = $"{errorMessage} ({parsedError.error})";
                }

                throw new ApiException(request.responseCode, errorMessage, body);
            }

            if (typeof(TResponse) == typeof(string))
            {
                return body as TResponse;
            }

            var parsed = SafeParse<TResponse>(body);
            if (parsed == null)
            {
                throw new ApiException(request.responseCode, "Response JSON could not be parsed.", body);
            }
            return parsed;
        }

        private static T SafeParse<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
