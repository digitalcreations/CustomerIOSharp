namespace CustomerIOSharp.Test
{
    using System.Threading.Tasks;
    using Xunit;

    public class EventTests : AuthorizationClass
    {
        [Fact]
        public async Task TrackEvent()
        {
            var customerIo = new CustomerIo(SiteId, ApiKey);
            await customerIo.TrackEventAsync("signup", new
                {
                    Group = "trial",
                    Referrer = "email campaign"
                },
                null,
                "from_static_identity");
        }

        [Fact]
        public async Task TrackEventFailsWithoutIdentity()
        {
            var customerIo = new CustomerIo(SiteId, ApiKey);
            await Assert.ThrowsAsync<CustomerIoApiException>(async () =>
                await customerIo.TrackEventAsync("signup", new
                {
                    Group = "trial",
                    Referrer = "email campaign"
                }));
        }
    }
}