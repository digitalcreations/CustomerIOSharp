namespace CustomerIOSharp;

using System;
using System.Net;

public class CustomerIoApiException : Exception
{
    public CustomerIoApiException(HttpStatusCode code, string reason)
        : base($"Received status code {(int) code} ({reason}) from Customer.io API")
    {
    }
}