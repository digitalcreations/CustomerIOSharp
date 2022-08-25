﻿namespace CustomerIOSharp;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <seealso cref="https://stackoverflow.com/a/70225152/1606">Adapted from this answer on Stack Overflow.</seealso>
public class UnixToNullableDateTimeConverter : JsonConverter<DateTime?>
{
    public override bool HandleNull => true;
    public bool? IsFormatInSeconds { get; set; } = null;

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetInt64(out var time))
        {
            // if 'IsFormatInSeconds' is unspecified, then deduce the correct type based on whether it can be represented in the allowed .net DateTime range
            if (IsFormatInSeconds == true || IsFormatInSeconds == null && time > _unixMinSeconds && time < _unixMaxSeconds)
                return DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
            return DateTimeOffset.FromUnixTimeMilliseconds(time).LocalDateTime;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value.ToUnixTimestamp());
        }
        else
        {
            writer.WriteNullValue();
        }
    }

    private static readonly long _unixMinSeconds = DateTimeOffset.MinValue.ToUnixTimeSeconds() - DateTimeOffset.UnixEpoch.ToUnixTimeSeconds(); // -62_135_596_800
    private static readonly long _unixMaxSeconds = DateTimeOffset.MaxValue.ToUnixTimeSeconds() - DateTimeOffset.UnixEpoch.ToUnixTimeSeconds(); // 253_402_300_799
}