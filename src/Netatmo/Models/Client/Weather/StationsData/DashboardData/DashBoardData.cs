using NodaTime;
using System.Text.Json.Serialization;

namespace Netatmo.Models.Client.Weather.StationsData.DashboardData
{
    public class DashBoardData : IDashBoardData
    {
        [JsonPropertyName("time_utc")]
        public Instant TimeUtc { get; set; }
    }
}