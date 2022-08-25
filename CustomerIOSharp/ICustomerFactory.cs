namespace CustomerIOSharp;

public interface ICustomerFactory
{
    /// <summary>
    /// Lets us get the customer id without looking up all the details of the customer.
    /// </summary>
    /// <returns>Unique identifier for this customer.</returns>
    string GetCustomerId();

    /// <summary>
    /// Get all the details about the customer.
    /// </summary>
    /// <returns></returns>
    ICustomerDetails GetCustomerDetails();
}