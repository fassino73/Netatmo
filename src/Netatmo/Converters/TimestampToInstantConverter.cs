using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;

namespace Netatmo.Converters
{
    public class TimestampToInstantConverter : JsonConverter<Instant>
    {
        public override void Write(Utf8JsonWriter writer, Instant value, JsonSerializerOptions serializer)
        {
            writer.WriteNumberValue(value.ToUnixTimeSeconds());
        }

        public override Instant Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
        {
            return Instant.FromUnixTimeSeconds(reader.GetInt64());
        }
    }
}