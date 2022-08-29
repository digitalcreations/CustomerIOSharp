namespace CustomerIOSharp;

using System.Net.Http;
using System.Threading.Tasks;

public class AppApi
{
    private const string ApiEndpoint = "https://api.customer.io/v1/api";

    private readonly HttpClient _httpClient;

    public AppApi(string appApiKey)
    {
        _httpClient = new HttpClient();

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {appApiKey}");
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
    public async Task TriggerBroadcastAsync(int campaignId, object? data = null, object? recipientFilter = null)
    {
        var wrappedData = new TriggerBroadcast(data, recipientFilter);

        var resource = $"{ApiEndpoint}/campaigns/{campaignId}/triggers";

        await _httpClient.CallJsonEndpointAsync(
            resource,
            HttpMethod.Post,
            wrappedData).ConfigureAwait(false);

    }
}