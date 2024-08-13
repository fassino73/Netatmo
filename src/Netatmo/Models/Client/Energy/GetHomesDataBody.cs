using Netatmo.Models.Client.Energy.HomesData;
using System.Text.Json.Serialization;

namespace Netatmo.Models.Client.Energy
{
    public class GetHomesDataBody
    {
        [JsonPropertyName("homes")]
        public Home[] Homes { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }
    }
}