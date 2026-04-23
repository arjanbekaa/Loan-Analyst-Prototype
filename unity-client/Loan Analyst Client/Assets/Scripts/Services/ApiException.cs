using System;

namespace LoanAnalyst.Client.Services
{
    public class ApiException : Exception
    {
        public long StatusCode { get; }
        public string RawBody { get; }

        public ApiException(long statusCode, string message, string rawBody = null) : base(message)
        {
            StatusCode = statusCode;
            RawBody = rawBody;
        }
    }
}
