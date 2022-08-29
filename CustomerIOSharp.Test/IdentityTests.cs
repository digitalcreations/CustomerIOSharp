namespace CustomerIOSharp.Test;

using System;
using System.Threading.Tasks;
using Xunit;

public class IdentityTests : AuthorizationClass
{
    [Fact]
    public async Task IdentifyAsyncFailsIfNotGivenIdentity()
    {
        var customerIo = new TrackApi(SiteId, ApiKey);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await customerIo.IdentifyAsync());
    }

    [Fact]
    public async Task IdentifyAsyncSucceedsWithStaticIdentity()
    {
        var customerIo = new TrackApi(SiteId, ApiKey);
        await customerIo.IdentifyAsync(new CustomerDetails("from_static_identity", "static@example.com"));
    }

    [Fact]
    public async Task IdentifyAsyncSucceedsWithIdentityFactory()
    {
        var customerIo = new TrackApi(SiteId, ApiKey, new IdentityFactory());
        await customerIo.IdentifyAsync();
    }

    [Fact]
    public async Task IdentifyAsyncSucceedsWithIdentityFactoryAndCustomCustomerDetails()
    {
        var customerIo = new TrackApi(SiteId, ApiKey, new IdentityFactoryWithExtraCustomerDetails());
        await customerIo.IdentifyAsync();
    }

    private class IdentityFactoryWithExtraCustomerDetails : ICustomerFactory
    {
        private const string CustomerId = "from_customer_factory_with_extra_field";
        private const string CustomerEmail = "extra@example.com";
        private const string CustomerName = "Man With Extra Fields";

        public string GetCustomerId() => CustomerId;

        public Task<ICustomerDetails> GetCustomerDetailsAsync() => Task.FromResult<ICustomerDetails>(new Customer(CustomerId, CustomerEmail,CustomerName));

        private record Customer(string Id, string Email, string Name) : ICustomerDetails;
    }

    private class IdentityFactory : ICustomerFactory
    {
        private const string CustomerId = "from_customer_factory";
        private const string CustomerEmail = "factory@example.com";

        public string GetCustomerId() => CustomerId;

        public Task<ICustomerDetails> GetCustomerDetailsAsync() => Task.FromResult<ICustomerDetails>(new CustomerDetails(CustomerId, CustomerEmail));
    }
}