namespace CustomerIOSharp
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using RestSharp.Portable;

    public class CustomerIo
    {
        private readonly ICustomerFactory _customerFactory;

        private readonly JsonSerializer _jsonSerializer;

        private const string Endpoint = "https://track.customer.io/api/v1/";

        private const string MethodCustomer = "customers/{customer_id}";
        private const string MethodTrack = "customers/{customer_id}/events";

        private readonly RestClient _client;

        public CustomerIo(string siteId, string apiKey, ICustomerFactory customerFactory = null, JsonSerializer jsonSerializer = null)
        {
            this._customerFactory = customerFactory;
            this._jsonSerializer = jsonSerializer;

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
                Serializer = new SerializerWrapper(this._jsonSerializer)
            };
            request.AddUrlSegment(@"customer_id", customerId);
            request.AddBody(data);
            
            var response = await this._client.Execute(request).ConfigureAwait (false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new CustomerIoApiException(response.StatusCode);
            }
        }

        public async Task IdentifyAsync(ICustomerDetails customer = null)
        {
            if (customer == null && this._customerFactory == null)
            {
                throw new ArgumentNullException(
                    "customer",
                    "Missing both customer and customer factory, so can not determine who to track");
            }

            customer = customer ?? this._customerFactory.GetCustomerDetails();
            await this.CallMethodAsync(MethodCustomer, customer.Id, HttpMethod.Put, customer);
        }

        public async Task DeleteCustomerAsync(string customerId = null)
        {
            if (String.IsNullOrEmpty(customerId) && this._customerFactory == null)
            {
                throw new ArgumentNullException(
                    "customerId",
                    "Missing both customerId and customer factory, so can not determine who to track");
            }

            customerId = customerId ?? this._customerFactory.GetCustomerId();
            await this.CallMethodAsync(MethodCustomer, customerId, HttpMethod.Delete, null);
        }

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="eventName">The name of the event you want to track</param>
        /// <param name="data">Any related information you’d like to attach to this event. These attributes can be used in your triggers to control who should receive the triggered email. You can set any number of data key and values.</param>
        /// <param name="timestamp">Allows you to back-date the event, pass null to use current time</param>
        /// <param name="customerId"></param>
        /// <returns>Nothing if successful, throws if failed</returns>
        /// <exception cref="CustomerIoApiException">If any code besides 200 OK is returned from the server.</exception>
        public async Task TrackEventAsync(string eventName, object data = null, DateTime? timestamp = null, string customerId = null)
        {
            if (String.IsNullOrEmpty(customerId) && this._customerFactory == null)
            {
                throw new ArgumentNullException(
                    "customerId",
                    "Missing both customerId and customer factory, so can not determine who to track");
            }
            customerId = customerId ?? this._customerFactory.GetCustomerId();

            var wrappedData = new TrackedEvent
                {
                    Name = eventName,
                    Data = data,
                    Timestamp = timestamp
                };

            await this.CallMethodAsync(MethodTrack, customerId, HttpMethod.Post, wrappedData);
        }
    }
}
