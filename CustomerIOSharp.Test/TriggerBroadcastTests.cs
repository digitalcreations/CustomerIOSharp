namespace CustomerIOSharp.Test
{
    using System.Threading.Tasks;
    using Xunit;

    public class TriggerBroadcastTests : AuthorizationClass
    {
        [Fact(Skip = "Test is skipped because of rate limiting from customer.io, and the test with a segment id below is more useful.")]
        public async Task TriggerBroadcast()
        {
            var customerIo = new AppApi(AppApiKey);
            await customerIo.TriggerBroadcastAsync(
                BroadcastCampaignId,
                new
                {
                    Name = "Name 1",
                    TestKey = "Value 2"
                });
        }

        [Fact]
        public async Task TriggerBroadcastWithRecipientFilter()
        {
            var customerIo = new AppApi(AppApiKey);
            await customerIo.TriggerBroadcastAsync(
                BroadcastCampaignId, 
                new
                {
                    Name = "Name 1",
                    TestKey = "Value 2"
                },
                new { segment = new { id = BroadcastSegmentId } });
        }
    }
}