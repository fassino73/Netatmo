using NodaTime;
using System.Text.Json.Serialization;

namespace Netatmo.Models.Client
{
    public class DataResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("time_exec")]
        public double? TimeExec { get; set; }

        [JsonPropertyName("time_server")]
        public Instant? TimeServer { get; set; }
    }

    public class DataResponse<T> : DataResponse
    {
        [JsonPropertyName("body")]
        public T Body { get; set; }
    }
}