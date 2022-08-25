namespace CustomerIOSharp;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

internal static class Utilities
{
    static Utilities()
    {
        var options = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        options.Converters.Clear();
        options.Converters.Add(new UnixToNullableDateTimeConverter());

        JsonSerializerOptions = options;
    }

    internal static JsonSerializerOptions JsonSerializerOptions { get; }

    internal static async Task CallMethodAsync(HttpClient client, string resource, HttpMethod httpMethod, object data)
    {

        var requestMessage = new HttpRequestMessage(httpMethod, resource)
        {
            Content = new StringContent(
                JsonSerializer.Serialize<object>(data, Utilities.JsonSerializerOptions),
                Encoding.UTF8,
                "application/json")
        };
        var result = await client.SendAsync(requestMessage).ConfigureAwait(false);
        if (result.StatusCode != HttpStatusCode.OK)
        {
            throw new CustomerIoApiException(result.StatusCode, result.ReasonPhrase);
        }
    }
}