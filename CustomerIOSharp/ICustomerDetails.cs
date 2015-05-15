namespace CustomerIOSharp
{
    /// <summary>
    /// Any extra properties added to descendants of this class will be sent along to CustomerDetails.io.
    /// </summary>
    public interface ICustomerDetails
    {
        string Id { get; }
        string Email { get; }
    }
}