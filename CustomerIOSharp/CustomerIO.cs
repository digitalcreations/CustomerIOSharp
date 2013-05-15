using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RestSharp;

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

        private async Task CallMethodAsync(string method, Method httpMethod, IDictionary<string, string> data)
        {
            var customer = _customerFactory.GetCustomer();
            // do not transmit events when we do not have a customer id
            if (customer == null) return;

            var request = new RestRequest(method)
            {
                Method = httpMethod
            };
            request.AddUrlSegment(@"customer_id", customer.Id);
            if (data != null)
            {
                foreach (var kvp in data)
                {
                    request.AddParameter(kvp.Key, kvp.Value);
                }
            }

            var response = await _client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new CustomerIoApiException(response.StatusCode);
            }
        }

        public async Task IdentifyAsync()
        {
            var customer = _customerFactory.GetCustomer();
            // do not transmit events when we do not have a customer id
            if (customer == null) return;

            var data = customer.Data;
            data["email"] = customer.Email;
            await CallMethodAsync(MethodCustomer, Method.PUT, data);
        }

        public async Task DeleteCustomerAsync()
        {
            await CallMethodAsync(MethodCustomer, Method.DELETE, new Dictionary<string, string>());
        }

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="eventName">The name of the event you want to track</param>
        /// <param name="data">Any related information you’d like to attach to this event. These attributes can be used in your triggers to control who should receive the triggered email. You can set any number of data key and values.</param>
        /// <returns>Nothing if successful, throws if failed</returns>
        /// <exception cref="CustomerIoApiException">If any code besides 200 OK is returned from the server.</exception>
        public async Task TrackEventAsync(string eventName, IDictionary<string, string> data = null)
        {
            if (data == null)
            {
                data = new Dictionary<string, string>();
            }
            data = data.Select(kvp => new KeyValuePair<string, string>(string.Format("data[{0}]", kvp.Key), kvp.Value))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            data["name"] = eventName;

            await CallMethodAsync(MethodTrack, Method.POST, data);
        }
    }
}
