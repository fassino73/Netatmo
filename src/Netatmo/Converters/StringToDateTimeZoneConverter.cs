using System;
using Flurl.Util;
using System.Text.Json.Serialization;
using NodaTime;
using System.Text.Json;

namespace Netatmo.Converters
{
    public class StringToDateTimeZoneConverter : JsonConverter<DateTimeZone>
    {
        public override void Write(Utf8JsonWriter writer, DateTimeZone value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToInvariantString());
        }

        public override DateTimeZone Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
        {
            return DateTimeZoneProviders.Tzdb[reader.GetString()];
        }
    }
}