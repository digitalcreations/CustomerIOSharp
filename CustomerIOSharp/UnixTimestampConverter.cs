namespace CustomerIOSharp
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class UnixTimestampConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            long val;
            if (value is DateTime)
            {
                val = ((DateTime)value).ToUnixTimestamp();
            }
            else
            {
                throw new Exception("Expected date object value.");
            }
            writer.WriteValue(val);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Integer)
                throw new Exception("Wrong Token Type");

            var seconds = (int) reader.Value;
            return seconds.ToDate();
        }
    }
}
