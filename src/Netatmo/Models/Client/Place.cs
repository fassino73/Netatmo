using System.Text.Json.Serialization;
using NodaTime;

namespace Netatmo.Models.Client
{
    public class Place
    {
        [JsonPropertyName("altitude")]
        public double Altitude { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("timezone")]
        public DateTimeZone Timezone { get; set; }

        [JsonPropertyName("location")]
        public double[] Location { get; set; }
    }
}