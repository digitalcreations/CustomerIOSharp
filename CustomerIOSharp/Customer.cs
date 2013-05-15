using System.Collections.Generic;

namespace CustomerIOSharp
{
    public class Customer : ICustomer
    {
        public Customer(string id, string email, IDictionary<string, string> data)
        {
            Id = id;
            Email = email;
            Data = data;
        }

        public Customer(string id, string email)
            : this(id, email, new Dictionary<string, string>())
        {
        }

        public string Id { get; private set; }
        public string Email { get; private set; }
        public IDictionary<string, string> Data { get; private set; }
    }
}