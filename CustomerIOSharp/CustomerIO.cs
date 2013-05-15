using System.Net;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers;

namespace CustomerIOSharp
{
    public class CustomerIo
    {
        private readonly string _siteId;
        private readonly string _apiKey;
        private readonly ICustomerFactory _customerFactory;

        private const string Endpoint = "https://track.customer.io/api/v1/";

        private const string MethodCustomer = "customers/{customer_id}";
        private const string MethodTrack = "customers/{customer_id}/events";

        private readonly RestClient _client;

        public CustomerIo(string siteId, string apiKey, ICustomerFactory customerFactory)
        {
            _siteId = siteId;
            _apiKey = apiKey;
            _customerFactory = customerFactory;

            _client = new RestClient(Endpoint)
                {
                    Authenticator = new HttpBasicAuthenticator(_siteId, _apiKey)
                };
        }

        private async Task CallMethodAsync(string method, string customerId, Method httpMethod, object data)
        {
            // do not transmit events if we do not have a customer id
            if (customerId == null) return;

            var request = new RestRequest(method)
            {
                Method = httpMethod,
                RequestFormat = DataFormat.Json,
                JsonSerializer = new JsonSerializer()
            };
            request.AddUrlSegment(@"customer_id", customerId);
            request.AddBody(data);
            
            var response = await _client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new CustomerIoApiException(response.StatusCode);
            }
        }

        public async Task IdentifyAsync()
        {
            var id = _customerFactory.GetCustomerId();

            await CallMethodAsync(MethodCustomer, id, Method.PUT, _customerFactory.GetCustomerDetails());
        }

        public async Task DeleteCustomerAsync()
        {
            var id = _customerFactory.GetCustomerId();
            await CallMethodAsync(MethodCustomer, id, Method.DELETE, null);
        }

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="eventName">The name of the event you want to track</param>
        /// <param name="data">Any related information you’d like to attach to this event. These attributes can be used in your triggers to control who should receive the triggered email. You can set any number of data key and values.</param>
        /// <returns>Nothing if successful, throws if failed</returns>
        /// <exception cref="CustomerIoApiException">If any code besides 200 OK is returned from the server.</exception>
        public async Task TrackEventAsync(string eventName, object data = null)
        {
            var id = _customerFactory.GetCustomerId();

            var wrappedData = new TrackedEvent
                {
                    Name = eventName,
                    Data = data
                };

            await CallMethodAsync(MethodTrack, id, Method.POST, wrappedData);
        }
    }

    [SerializeAs(NameStyle = NameStyle.CamelCase)]
    class TrackedEvent
    {
        public string Name { get; set; }
        public object Data { get; set; }
    }
}
