namespace CustomerIOSharp.Test
{
    using System.Threading.Tasks;
    using Xunit;

    public class TriggerBroadcastTests : AuthorizationClass
    {
        [Fact]
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