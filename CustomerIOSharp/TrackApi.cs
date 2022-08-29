namespace CustomerIOSharp;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

public class TrackApi
{
    private const string TrackEndpoint = "https://track.customer.io/api/v1";

    private readonly ICustomerFactory? _customerFactory;

    private readonly HttpClient _httpClient;

    public TrackApi(string siteId, string apiKey, ICustomerFactory? customerFactory = null)
    {
        _customerFactory = customerFactory;

        _httpClient = new HttpClient();

        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{siteId}:{apiKey}"));
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
    }

    public async Task IdentifyAsync(ICustomerDetails? customer = null)
    {
        customer = EnsureCustomerOrThrow(customer);

        // do not transmit events if we do not have a customer id
        if (customer?.Id == null) return;

        var resource = $"{TrackEndpoint}/customers/{customer.Id}";

        await _httpClient.CallJsonEndpointAsync(resource, HttpMethod.Put, customer).ConfigureAwait(false);
    }

    public async Task DeleteCustomerAsync(string? customerId = null)
    {
        customerId = EnsureCustomerIdOrThrow(customerId);

        // do not transmit events if we do not have a customer id
        if (customerId == null) return;

        var resource = $"{TrackEndpoint}/customers/{customerId}";

        await _httpClient.CallJsonEndpointAsync(resource, HttpMethod.Delete, null).ConfigureAwait(false);
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
    public async Task TrackEventAsync(string eventName, object? data = null, DateTime? timestamp = null, string? customerId = null)
    {
        customerId = EnsureCustomerIdOrThrow(customerId);

        var wrappedData = new TrackedEvent(eventName, data, timestamp);

        var resource = $"{TrackEndpoint}/customers/{customerId}/events";

        await _httpClient.CallJsonEndpointAsync(
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
    public async Task TrackNonCustomerEventAsync(string eventName, object? data = null, DateTime? timestamp = null)
    {
        var wrappedData = new TrackedEvent(eventName, data, timestamp);

        var resource = $"{TrackEndpoint}/events";

        await _httpClient.CallJsonEndpointAsync(
            resource,
            HttpMethod.Post,
            wrappedData).ConfigureAwait(false);
    }

    private string EnsureCustomerIdOrThrow(string? customerId)
    {
        if (string.IsNullOrEmpty(customerId) && _customerFactory == null)
        {
            throw new ArgumentNullException(
                nameof(customerId),
                "Missing both customerId and customer factory, so can not determine who to track");
        }

        customerId = customerId ?? _customerFactory?.GetCustomerId();
        if (string.IsNullOrEmpty(customerId))
        {
            throw new ArgumentNullException(
                nameof(customerId), 
                "Could not determine customer ID from either provided parameter or customer factory.");
        }

        return customerId;
    }

    private ICustomerDetails EnsureCustomerOrThrow(ICustomerDetails? customer)
    {
        if (customer == null && _customerFactory == null)
        {
            throw new ArgumentNullException(
                nameof(customer),
                "Missing both customer and customer factory, so can not determine who to track");
        }

        customer = customer ?? _customerFactory?.GetCustomerDetails();
        if (customer == null)
        {
            throw new ArgumentNullException(
                nameof(customer),
                "Could not determine customer ID from either provided parameter or customer factory.");
        }

        return customer;
    }
}