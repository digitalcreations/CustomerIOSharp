namespace CustomerIOSharp;

using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

internal static class HttpClientExtensions
{
    /// <param name="client">HttpClient to use.</param>
    /// <param name="resource"></param>
    /// <param name="httpMethod"></param>
    /// <param name="data"></param>
    /// <exception cref="HttpRequestException" />
    /// <returns>Task on success.</returns>
    internal static async Task CallJsonEndpointAsync(this HttpClient client, string resource, HttpMethod httpMethod, object? data)
    {
        var requestMessage = new HttpRequestMessage(httpMethod, resource)
        {
            Content = new StringContent(
                data == null ? string.Empty : JsonSerializer.Serialize<object>(data, Utilities.JsonSerializerOptions),
                Encoding.UTF8,
                "application/json")
        };
        var result = await client.SendAsync(requestMessage).ConfigureAwait(false);

        result.EnsureSuccessStatusCode();
    }
}