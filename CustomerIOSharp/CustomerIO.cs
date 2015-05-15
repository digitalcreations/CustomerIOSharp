namespace CustomerIOSharp
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using RestSharp.Portable;

    public class CustomerIo
    {
        private readonly ICustomerFactory _customerFactory;

        private const string Endpoint = "https://track.customer.io/api/v1/";

        private const string MethodCustomer = "customers/{customer_id}";
        private const string MethodTrack = "customers/{customer_id}/events";

        private readonly RestClient _client;

        public CustomerIo(string siteId, string apiKey, ICustomerFactory customerFactory)
        {
            this._customerFactory = customerFactory;

            this._client = new RestClient(Endpoint)
                {
                    Authenticator = new FixedHttpBasicAuthenticator(siteId, apiKey)
                };
        }

        private async Task CallMethodAsync(string method, string customerId, HttpMethod httpMethod, object data)
        {
            // do not transmit events if we do not have a customer id
            if (customerId == null) return;

            var request = new RestRequest(method)
            {
                Method = httpMethod,
                Serializer = new JsonSerializer()
            };
            request.AddUrlSegment(@"customer_id", customerId);
            request.AddBody(data);
            
            var response = await this._client.Execute(request).ConfigureAwait (false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new CustomerIoApiException(response.StatusCode);
            }
        }

        public async Task IdentifyAsync()
        {
            var id = this._customerFactory.GetCustomerId();
            await this.CallMethodAsync(MethodCustomer, id, HttpMethod.Put, this._customerFactory.GetCustomerDetails());
        }

        public async Task DeleteCustomerAsync()
        {
            var id = this._customerFactory.GetCustomerId();
            await this.CallMethodAsync(MethodCustomer, id, HttpMethod.Delete, null);
        }

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="eventName">The name of the event you want to track</param>
        /// <param name="data">Any related information you’d like to attach to this event. These attributes can be used in your triggers to control who should receive the triggered email. You can set any number of data key and values.</param>
        /// <param name="timestamp">Allows you to back-date the event, pass null to use current time</param>
        /// <returns>Nothing if successful, throws if failed</returns>
        /// <exception cref="CustomerIoApiException">If any code besides 200 OK is returned from the server.</exception>
        public async Task TrackEventAsync(string eventName, object data = null, DateTime? timestamp = null)
        {
            var id = this._customerFactory.GetCustomerId();

            var wrappedData = new TrackedEvent
                {
                    Name = eventName,
                    Data = data,
                    Timestamp = timestamp
                };

            await this.CallMethodAsync(MethodTrack, id, HttpMethod.Post, wrappedData);
        }
    }
}
