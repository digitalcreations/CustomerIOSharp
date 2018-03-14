namespace CustomerIOSharp.Test
{
    using System.Threading.Tasks;
    using Xunit;

    public class TriggerBroadcastTests : AuthorizationClass
    {
        //Requires a valid campaign ID 
        [Fact]
        public async Task TriggerBroadcast()
        {
            var customerIo = new CustomerIo(SiteId, ApiKey);
            await customerIo.TriggerBroadcastAsync(18, new
                {
                    Name = "Name 1",
                    TestKey = "Value 2"
                } );
        }

        //Requires a valid campaign ID and a valid Segment ID
        [Fact]
        public async Task TriggerBroadcastWithRecipientFilter()
        {
            var customerIo = new CustomerIo(SiteId, ApiKey);
            await customerIo.TriggerBroadcastAsync(18, new
            {
                Name = "Name 1",
                TestKey = "Value 2"
            },
            new {segment = new { id = 19 } }
            
            );
        }

    }
}