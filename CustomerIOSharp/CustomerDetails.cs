namespace CustomerIOSharp
{
    public class CustomerDetails : ICustomerDetails
    {
        public CustomerDetails(string email)
        {
            Email = email;
        }

        public string Email { get; private set; }
    }
}