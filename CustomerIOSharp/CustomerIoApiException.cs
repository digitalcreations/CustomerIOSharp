namespace CustomerIOSharp
{
    using System;
    using System.Net;

    public class CustomerIoApiException : Exception
    {
        public CustomerIoApiException(HttpStatusCode code)
            : base($"Received status code {(int) code} ({code}) from Customer.io API")
        {
        }
    }
}