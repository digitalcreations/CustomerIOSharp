namespace CustomerIOSharp
{
    public class CustomerDetails : ICustomerDetails
    {
        public CustomerDetails(string id, string email)
        {
            Id = id;
            Email = email;
        }

        public string Id { get; }

        public string Email { get; }
    }
}