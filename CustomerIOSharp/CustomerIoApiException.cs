using System;
using System.Net;

namespace CustomerIOSharp
{
    public class CustomerIoApiException : Exception
    {
        public CustomerIoApiException(HttpStatusCode code)
            : base(string.Format("Received status code {0} ({1}) from CustomerDetails.io API", (int)code, code.ToString()))
        {
            
        }
    }
}