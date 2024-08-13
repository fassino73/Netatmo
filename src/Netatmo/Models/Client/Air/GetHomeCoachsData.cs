using Netatmo.Models.Client.Air.HomesCoachs;
using System.Text.Json.Serialization;

namespace Netatmo.Models.Client.Air
{
    public class GetHomeCoachsData
    {
        [JsonPropertyName("devices")]
        public Devices[] Devices { get; set; }
        
        [JsonPropertyName("user")]
        public User User { get; set; }
    }
}