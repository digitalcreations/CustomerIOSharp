namespace CustomerIOSharp
{
    public interface ICustomerFactory
    {
        string GetCustomerId();

        ICustomerDetails GetCustomerDetails();
    }
}