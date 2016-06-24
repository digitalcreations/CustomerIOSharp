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
        private const string Endpoint = "https://track.customer.io/api/v1/";

        private const string MethodCustomer = "customers/{customer_id}";
        private const string MethodCustomerEvent = "customers/{customer_id}/events";
        private const string MethodEvent = "events";

        private readonly ICustomerFactory _customerFactory;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        private readonly HttpClient _httpClient;

        public CustomerIo(string siteId, string apiKey, ICustomerFactory customerFactory = null)
        {
            this._customerFactory = customerFactory;

            this._httpClient = new HttpClient
            {
                BaseAddress = new Uri(Endpoint),
            };
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{siteId}:{apiKey}"));
            this._httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");

            this._jsonSerializerSettings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            foreach (var converter in this._jsonSerializerSettings.Converters.OfType<DateTimeConverterBase>().ToList())
            {
                this._jsonSerializerSettings.Converters.Remove(converter);
            }

            this._jsonSerializerSettings.Converters.Add(new UnixTimestampConverter());

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

            // do not transmit events if we do not have a customer id
            if (customer?.Id == null) return;

            await this.CallMethodAsync(MethodCustomer, HttpMethod.Put, customer, customer.Id);
        }

        public async Task DeleteCustomerAsync(string customerId = null)
        {
            if (string.IsNullOrEmpty(customerId) && this._customerFactory == null)
            {
                throw new ArgumentNullException(
                    "customerId",
                    "Missing both customerId and customer factory, so can not determine who to track");
            }

            customerId = customerId ?? this._customerFactory.GetCustomerId();

            // do not transmit events if we do not have a customer id
            if (customerId == null) return;

            await this.CallMethodAsync(MethodCustomer, HttpMethod.Delete, null, customerId);
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
        public async Task TrackEventAsync(string eventName, object data = null, DateTime? timestamp = null, string customerId = null)
        {
            if (string.IsNullOrEmpty(customerId) && this._customerFactory != null)
            {
                customerId = customerId ?? this._customerFactory.GetCustomerId();
            }

            var wrappedData = new TrackedEvent
            {
                Name = eventName,
                Data = data,
                Timestamp = timestamp
            };

            await this.CallMethodAsync(
                MethodCustomerEvent,
                HttpMethod.Post,
                wrappedData,
                customerId);
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
        public async Task TrackNonCustomerEventAsync(string eventName, object data = null, DateTime? timestamp = null)
        {
            var wrappedData = new TrackedEvent
            {
                Name = eventName,
                Data = data,
                Timestamp = timestamp
            };

            await this.CallMethodAsync(
                MethodEvent,
                HttpMethod.Post,
                wrappedData);
        }

        private async Task CallMethodAsync(string resource, HttpMethod httpMethod, object data, string customerId = null)
        {
            resource = resource.Replace("{customer_id}", customerId);
            var requestMessage = new HttpRequestMessage(httpMethod, resource)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(data, this._jsonSerializerSettings),
                    Encoding.UTF8,
                    "application/json")
            };
            var result = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new CustomerIoApiException(result.StatusCode);
            }
        }
    }
}
