using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using static vMenuShared.ConfigManager;

namespace vMenuShared
{
    public static class TimeWeatherCommon
    {
        private static Dictionary<T, string> EnumToNameDict<T>()
        {
            return Enum
                .GetValues(typeof(T))
                .Cast<T>()
                .ToDictionary(i => i, i => i.ToString().ToLower());
        }

        private static Dictionary<T2, T1> DictSwapKv<T1, T2>(Dictionary<T1, T2> dict) =>
            dict.ToDictionary(kv => kv.Value, kv => kv.Key);

        public static List<string> TimeListOptions =
            Enumerable.Range(0,48).Select(i => $"{i / 2:D2}:{30 * (i % 2):D2}").ToList();

        public enum WeatherType
        {
            Clear,
            ExtraSunny,
            Clouds,
            Overcast,
            Rain,
            Clearing,
            Thunder,
            Smog,
            Foggy,
            Xmas,
            Snow,
            SnowLight,
            Blizzard,
            Halloween,
            Neutral,
        }

        public static Dictionary<WeatherType, string> WeatherTypeToName = EnumToNameDict<WeatherType>();
        public static Dictionary<string, WeatherType> WeatherNameToType = DictSwapKv(WeatherTypeToName);

        public static List<string> WeatherTypeOptionsList = new List<string>()
        {
            "Clear",
            "Extra Sunny",
            "Clouds",
            "Overcast",
            "Rain",
            "Clearing",
            "Thunder",
            "Smog",
            "Foggy",
            "Halloween",
            "Xmas",
            "Snow",
            "Snow Light",
            "Blizzard",
            "Neutral",
        };

        public enum BlackoutState
        {
            Off,
            Buildings,
            Everything
        }

        public static Dictionary<BlackoutState, string> BlackoutTypeToName = EnumToNameDict<BlackoutState>();
        public static Dictionary<string, BlackoutState> BlackoutNameToType = DictSwapKv(BlackoutTypeToName);

        public static List<string> BlackoutStateOptionsList = new List<string>()
        {
            "Off",
            "Buildings",
            "Everything",
        };

        public class TimeState
        {
            public int Hour { get; set; } = 0;
            public int Minute { get; set; } = 0;
            public bool Frozen { get; set; } = false;

            public int DayMinutes
            {
                get => 60 * Hour + Minute;
            }

            public TimeState Clone()
            {
                return new TimeState
                {
                    Hour = Hour,
                    Minute = Minute,
                    Frozen = Frozen,
                };
            }
        }

        public class WeatherState
        {
            public WeatherType WeatherType { get; set; } = WeatherType.Clear;
            public bool Snow { get; set; } = false;
            public BlackoutState Blackout { get; set; } = BlackoutState.Off;

            public WeatherState Clone()
            {
                return new WeatherState
                {
                    WeatherType = WeatherType,
                    Snow = Snow,
                    Blackout = Blackout,
                };
            }
        }

        public static TimeState GetServerTime()
        {
            if (!GetSettingsBool(Setting.vmenu_enable_time_weather_sync))
                return new TimeState();

            var json = GetSettingsString(Setting.vmenu_server_time, "{}");
            if (string.IsNullOrEmpty(json))
                return new TimeState();

            try
            {
                return JsonConvert.DeserializeObject<TimeState>(json);
            }
            catch
            {
                return new TimeState();
            }
        }

        public static WeatherState GetServerWeather()
        {
            if (!GetSettingsBool(Setting.vmenu_enable_time_weather_sync))
                return new WeatherState();

            var json = GetSettingsString(Setting.vmenu_server_weather, "{}");
            if (string.IsNullOrEmpty(json))
                return new WeatherState();

            try
            {
                return JsonConvert.DeserializeObject<WeatherState>(json);
            }
            catch
            {
                return new WeatherState();
            }
        }

        public static bool GetOverrideClientTW()
        {
            return GetSettingsBool(Setting.vmenu_override_client_time_weather);
        }
    }
}
