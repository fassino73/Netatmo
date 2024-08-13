using System.Collections.Generic;
using System.Text.Json;
using Flurl.Http.Configuration;
using Netatmo.Converters;

namespace Netatmo
{
    public static class Configuration
    {
        public static JsonSerializerOptions JsonSerializerSettings()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyProperties = true,
                Converters =
                {
                    new TimestampToInstantConverter(),
                    new StringToDateTimeZoneConverter()
                }
            };
        }

        public static void ConfigureRequest(FlurlHttpSettings settings)
        {
            settings.JsonSerializer = new DefaultJsonSerializer(JsonSerializerSettings());
        }
    }
}