namespace CustomerIOSharp
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class CustomerIo
    {
        private const string TrackEndpoint = "https://track.customer.io/api/v1/";
        private const string ApiEndpoint = "https://api.customer.io/v1/api/";
        private const string MethodCustomer = "customers/{customer_id}";
        private const string MethodCustomerEvent = "customers/{customer_id}/events";
        private const string MethodTriggerBroadcast = "campaigns/{campaign_id}/triggers";                
        private const string MethodEvent = "events";

        private readonly ICustomerFactory _customerFactory;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        private readonly HttpClient _httpClient;

        public CustomerIo(string siteId, string apiKey, ICustomerFactory customerFactory = null)
        {
            _customerFactory = customerFactory;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(TrackEndpoint)
            };
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{siteId}:{apiKey}"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");

            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            foreach (var converter in _jsonSerializerSettings.Converters.OfType<DateTimeConverterBase>().ToList())
            {
                _jsonSerializerSettings.Converters.Remove(converter);
            }

            _jsonSerializerSettings.Converters.Add(new UnixTimestampConverter());
        }

        public async Task IdentifyAsync(ICustomerDetails customer = null)
        {
            if (customer == null && _customerFactory == null)
            {
                throw new ArgumentNullException(
                    nameof(customer),
                    "Missing both customer and customer factory, so can not determine who to track");
            }

            customer = customer ?? _customerFactory.GetCustomerDetails();

            // do not transmit events if we do not have a customer id
            if (customer?.Id == null) return;

            var resource = MethodCustomer.Replace("{customer_id}", customer.Id);

            await CallMethodAsync(resource, HttpMethod.Put, customer).ConfigureAwait(false);
        }

        public async Task DeleteCustomerAsync(string customerId = null)
        {
            if (string.IsNullOrEmpty(customerId) && _customerFactory == null)
            {
                throw new ArgumentNullException(
                    nameof(customerId),
                    "Missing both customerId and customer factory, so can not determine who to track");
            }

            customerId = customerId ?? _customerFactory.GetCustomerId();

            // do not transmit events if we do not have a customer id
            if (customerId == null) return;

            var resource = MethodCustomer.Replace("{customer_id}", customerId);

            await CallMethodAsync(resource, HttpMethod.Delete, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="eventName">The name of the event you want to track</param>
        /// <param name="data">Any related information you’d like to attach to this event. These attributes can be used in your triggers to control who should receive the triggered email. You can set any number of data key and values.</param>
        /// <param name="timestamp">Allows you to back-date the event, pass null to use current time</param>
        /// <param name="customerId">Specify customer id this is valid for, or null to look it up using the customer factory.</param>
        /// <returns>Nothing if successful, throws if failed</returns>
        /// <exception cref="CustomerIoApiException">If any code besides 200 OK is returned from the server.</exception>
        public async Task TrackEventAsync(string eventName, object data = null, DateTime? timestamp = null,
            string customerId = null)
        {
            if (string.IsNullOrEmpty(customerId) && _customerFactory != null)
            {
                customerId = customerId ?? _customerFactory.GetCustomerId();
            }

            var wrappedData = new TrackedEvent
            {
                Name = eventName,
                Data = data,
                Timestamp = timestamp
            };

            var resource = MethodCustomerEvent.Replace("{customer_id}", customerId);

            await CallMethodAsync(
                resource,
                HttpMethod.Post,
                wrappedData).ConfigureAwait(false);
        }

        /// <summary>
        /// Track a custom event for a non-customer.
        /// </summary>
        /// <see cref="http://customer.io/docs/invitation-emails.html" />
        /// <param name="eventName">The name of the event you want to track</param>
        /// <param name="data">Any related information you’d like to attach to this event. These attributes can be used in your triggers to control who should receive the triggered email. You can set any number of data key and values.</param>
        /// <param name="timestamp">Allows you to back-date the event, pass null to use current time</param>
        /// <returns>Nothing if successful, throws if failed</returns>
        /// <exception cref="CustomerIoApiException">If any code besides 200 OK is returned from the server.</exception>
        /// 
        public async Task TrackNonCustomerEventAsync(string eventName, object data = null, DateTime? timestamp = null)
        {
            var wrappedData = new TrackedEvent
            {
                Name = eventName,
                Data = data,
                Timestamp = timestamp
            };

            await CallMethodAsync(
                MethodEvent,
                HttpMethod.Post,
                wrappedData).ConfigureAwait(false);
        }

        /// <summary>
        /// Track a custom event for a non-customer.
        /// </summary>
        /// <see cref="https://learn.customer.io/api/#apibroadcast_trigger" />
        /// <param name="campaignId">The Campaign Id you wish to trigger</param>
        /// <param name="data">Any related information you’d like to attach to this broadcast. These attributes can be used in the email/ action body of the triggered email. You can set any number of data key and values.</param>
        /// <param name="recipientFilter">Allows you to pass in filters that will override any preset segment or recipient criteria. </param>
        /// <see cref="https://learn.customer.io/documentation/api-triggered-broadcast-setup.html#step-1-define-recipients" />        
        /// <returns>Nothing if successful, throws if failed</returns>
        /// <exception cref="CustomerIoApiException">If any code besides 200 OK is returned from the server.</exception>
        public async Task TriggerBroadcastAsync(string campaignId, object data = null, object recipientFilter = null)
        {
            var wrappedData = new TriggerBroadcast
            {                
                Data = data,
                Recipients = recipientFilter
            };

            //This line is important as we need to use the alternative endpoint.
            this._httpClient.BaseAddress = new Uri(ApiEndpoint);
                 
            var resource = MethodTriggerBroadcast.Replace("{campaign_id}", campaignId);  

            await CallMethodAsync(
                resource,
                HttpMethod.Post,
                wrappedData).ConfigureAwait(false);
        }
        

        private async Task CallMethodAsync(string resource, HttpMethod httpMethod, object data
         )
        {

            var requestMessage = new HttpRequestMessage(httpMethod, resource)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(data, _jsonSerializerSettings),
                    Encoding.UTF8,
                    "application/json")
            };
            var result = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new CustomerIoApiException(result.StatusCode, result.ReasonPhrase);
            }
        }
    }
}