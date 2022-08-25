namespace CustomerIOSharp;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

internal static class Utilities
{
    static Utilities()
    {
        var settings = new JsonSerializerSettings()
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        foreach (var converter in settings.Converters.OfType<DateTimeConverterBase>().ToList())
        {
            settings.Converters.Remove(converter);
        }

        settings.Converters.Add(new UnixTimestampConverter());

        JsonSerializerSettings = settings;
    }

    internal static JsonSerializerSettings JsonSerializerSettings { get; }

    internal static async Task CallMethodAsync(HttpClient client, string resource, HttpMethod httpMethod, object data)
    {

        var requestMessage = new HttpRequestMessage(httpMethod, resource)
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(data, Utilities.JsonSerializerSettings),
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