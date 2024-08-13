using System.Text.Json.Serialization;

namespace Netatmo.Models.Client.Energy
{
    public class CreateHomeScheduleResponse : DataResponse
    {
        [JsonPropertyName("schedule_id")]
        public string ScheduleId { get; set; }
    }
}