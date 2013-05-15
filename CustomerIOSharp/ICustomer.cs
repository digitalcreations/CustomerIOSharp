using System.Collections.Generic;

namespace CustomerIOSharp
{
    public interface ICustomer
    {
        string Id { get; }
        string Email { get; }
        IDictionary<string, string> Data { get; }
    }
}