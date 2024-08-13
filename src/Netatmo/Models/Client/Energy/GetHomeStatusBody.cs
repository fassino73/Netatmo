using Netatmo.Models.Client.Energy.HomeStatus;
using System.Text.Json.Serialization;

namespace Netatmo.Models.Client.Energy
{
    public class GetHomeStatusBody
    {
        [JsonPropertyName("home")]
        public Home Home { get; set; }
    }
}