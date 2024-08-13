using NodaTime;
using System.Text.Json.Serialization;

namespace Netatmo.Models.Client.Weather.StationsData.DashboardData
{
    public interface IDashBoardData
    {
        [JsonPropertyName("time_utc")]
        Instant TimeUtc { get; set; }
    }
}