namespace CustomerIOSharp
{
    internal class CustomerDetails : ICustomerDetails
    {
        public CustomerDetails(string id, string email)
        {
            this.Id = id;
            this.Email = email;
        }

        public string Id { get; private set; }

        public string Email { get; private set; }
    }
}