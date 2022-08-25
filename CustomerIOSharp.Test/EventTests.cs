namespace CustomerIOSharp.Test;

using System;
using System.Threading.Tasks;
using Xunit;

public class EventTests : AuthorizationClass
{
    [Fact]
    public async Task TrackEvent()
    {
        var customerIo = new TrackApi(SiteId, ApiKey);
        await customerIo.TrackEventAsync("signup", new
            {
                Group = "trial",
                Referrer = "email campaign"
            },
            new DateTime(2022, 8, 25),
            "from_static_identity");
    }

    [Fact]
    public async Task TrackEventFailsWithoutIdentity()
    {
        var customerIo = new TrackApi(SiteId, ApiKey);
        await Assert.ThrowsAsync<CustomerIoApiException>(async () =>
            await customerIo.TrackEventAsync("signup", new
            {
                Group = "trial",
                Referrer = "email campaign"
            }));
    }
}