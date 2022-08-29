using System.Threading.Tasks;

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
    /// <remarks>This method is meant to be async so you can potentially make requests to an identity store to get the details needed.</remarks>
    /// <returns></returns>
    Task<ICustomerDetails> GetCustomerDetailsAsync();
}