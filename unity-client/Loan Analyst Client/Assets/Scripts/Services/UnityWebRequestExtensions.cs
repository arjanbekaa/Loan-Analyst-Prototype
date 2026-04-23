using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace LoanAnalyst.Client.Services
{
    public static class UnityWebRequestExtensions
    {
        public static Task<UnityWebRequest> SendAsync(this UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            var op = request.SendWebRequest();
            op.completed += _ => { tcs.TrySetResult(request); };
            return tcs.Task;
        }

        public static string ReadBodySafe(this UnityWebRequest request)
        {
            return request.downloadHandler != null ? request.downloadHandler.text : string.Empty;
        }

        public static bool HasNetworkError(this UnityWebRequest request)
        {
            return request.result == UnityWebRequest.Result.ConnectionError ||
                   request.result == UnityWebRequest.Result.DataProcessingError;
        }
    }
}
