using System;
using Netatmo.Enums;
using Netatmo.Models.Client.Weather.StationsData.DashboardData;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;
using System.Text.Json.Nodes;

namespace Netatmo.Models.Client.Weather.StationsData
{
    public class Module
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        // NAMain: Base station, NAModule1: Outdoor Module, NAModule2: Wind Gauge, NAModule3: Rain Gauge, NAModule4: Optional indoor module
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("module_name")]
        public string ModuleName { get; set; }

        // Current radio status per module. (90=low, 60=highest)
        [JsonPropertyName("rf_status")]
        public int RfStatus { get; set; }

        public RfStrengthEnum RfStrength
        {
            get
            {
                if (RfStatus <= 60) return RfStrengthEnum.FullSignal;

                if (RfStatus <= 70) return RfStrengthEnum.High;

                if (RfStatus <= 80) return RfStrengthEnum.Medium;

                return RfStrengthEnum.Low;
            }
        }

        // Percentage of battery remaining (10=low)
        [JsonPropertyName("battery_percent")]
        public int BatteryPercent { get; set; }

        [JsonPropertyName("battery_vp")]
        public int BatteryVp { get; set; }

        public BatteryLevelEnum BatteryStatus
        {
            get
            {
                switch (Type)
                {
                    case "NAModule2" when BatteryVp >= 6000:
                        return BatteryLevelEnum.Max;
                    case "NAModule2" when BatteryVp >= 5590:
                        return BatteryLevelEnum.Full;
                    case "NAModule2" when BatteryVp >= 5180:
                        return BatteryLevelEnum.High;
                    case "NAModule2" when BatteryVp >= 4770:
                        return BatteryLevelEnum.Medium;
                    case "NAModule2" when BatteryVp >= 4360:
                        return BatteryLevelEnum.Low;
                    case "NAModule2":
                        return BatteryLevelEnum.VeryLow;
                    case "NAModule4" when BatteryVp >= 6000:
                        return BatteryLevelEnum.Max;
                    case "NAModule4" when BatteryVp >= 5640:
                        return BatteryLevelEnum.Full;
                    case "NAModule4" when BatteryVp >= 5280:
                        return BatteryLevelEnum.High;
                    case "NAModule4" when BatteryVp >= 4920:
                        return BatteryLevelEnum.Medium;
                    case "NAModule4" when BatteryVp >= 4560:
                        return BatteryLevelEnum.Low;
                    case "NAModule4":
                        return BatteryLevelEnum.VeryLow;
                    case "NAModule1" when BatteryVp >= 6000:
                    case "NAModule3" when BatteryVp >= 6000:
                        return BatteryLevelEnum.Max;
                    case "NAModule1" when BatteryVp >= 5500:
                    case "NAModule3" when BatteryVp >= 5500:
                        return BatteryLevelEnum.Full;
                    case "NAModule1" when BatteryVp >= 5000:
                    case "NAModule3" when BatteryVp >= 5000:
                        return BatteryLevelEnum.High;
                    case "NAModule1" when BatteryVp >= 4500:
                    case "NAModule3" when BatteryVp >= 4500:
                        return BatteryLevelEnum.Medium;
                    case "NAModule1" when BatteryVp >= 4000:
                    case "NAModule3" when BatteryVp >= 4000:
                        return BatteryLevelEnum.Low;
                    case "NAModule1":
                    case "NAModule3":
                        return BatteryLevelEnum.VeryLow;
                    default:
                        return BatteryLevelEnum.Undefined;
                }
            }
        }

        [JsonPropertyName("firmware")]
        public int Firmware { get; set; }

        [JsonPropertyName("last_message")]
        public Instant LastMessageAt { get; set; }

        [JsonPropertyName("last_seen")]
        public Instant LastSeenAt { get; set; }

        [JsonPropertyName("last_setup")]
        public Instant LastSetupAt { get; set; }

        [JsonPropertyName("data_type")]
        public string[] DataType { get; set; }

        [JsonPropertyName("dashboard_data")]
        public JsonObject DashboardData { get; set; }

        public T GetDashboardData<T>()where T : IDashBoardData
        {
            Type expectedType;
            switch(Type)
            {
                case "NAMain":
                    expectedType = typeof(BaseStationDashBoardData);
                    break;
                case "NAModule1":
                    expectedType = typeof(OutdoorDashBoardData);
                    break;
                case "NAModule2":
                    expectedType = typeof(WindGaugeDashBoardData);
                    break;
                case "NAModule3":
                    expectedType = typeof(RainGaugeDashBoardData);
                    break;
                case "NAModule4":
                    expectedType = typeof(IndoorDashBoardData);
                    break;
                default:
                    expectedType = typeof(DashBoardData);
                    break;
            }

            if (expectedType != typeof(T))
            {
                throw new ArgumentException($"{expectedType.Name} should be expected");
            }
            
            return JsonSerializer.Deserialize<T>(DashboardData, Configuration.JsonSerializerSettings());
        }
    }
}