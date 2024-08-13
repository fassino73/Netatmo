using Netatmo.Models.Client.Weather.StationsData;
using System.Text.Json.Serialization;

namespace Netatmo.Models.Client
{
    public class User
    {
        [JsonPropertyName("administrative")]
        public Administrative Administrative { get; set; }

        [JsonPropertyName("mail")]
        public string Mail { get; set; }
    }
}