namespace CustomerIOSharp;

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
}
