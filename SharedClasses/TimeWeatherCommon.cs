using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using static CitizenFX.Core.Native.API;

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

        public enum BlackoutState
        {
            Off,
            Buildings,
            Everything
        }

        public static Dictionary<BlackoutState, string> BlackoutTypeToName = EnumToNameDict<BlackoutState>();
        public static Dictionary<string, BlackoutState> BlackoutNameToType = DictSwapKv(BlackoutTypeToName);

        public class ServerTimeState
        {
            public bool Override { get; set; } = false;
            public int Hour { get; set; } = 0;
            public int Minute { get; set; } = 0;
            public bool Frozen { get; set; } = false;
        }

        public class ServerWeatherState
        {
            public bool Override { get; set; } = false;
            public WeatherType WeatherType { get; set; } = WeatherType.Clear;
            public bool Snow { get; set; } = false;
            public BlackoutState Blackout { get; set; } = BlackoutState.Off;
        }

        public static ServerTimeState GetServerTime()
        {
            if (!GetSettingsBool(Setting.vmenu_enable_time_weather_sync))
                return new ServerTimeState();

            var json = GetSettingsString(Setting.vmenu_server_time, "{}");
            if (string.IsNullOrEmpty(json))
                return new ServerTimeState();

            try
            {
                return JsonConvert.DeserializeObject<ServerTimeState>(json);
            }
            catch
            {
                return new ServerTimeState();
            }
        }

        public static ServerWeatherState GetServerWeather()
        {
            if (!GetSettingsBool(Setting.vmenu_enable_time_weather_sync))
                return new ServerWeatherState();

            var json = GetSettingsString(Setting.vmenu_server_weather, "{}");
            if (string.IsNullOrEmpty(json))
                return new ServerWeatherState();

            try
            {
                return JsonConvert.DeserializeObject<ServerWeatherState>(json);
            }
            catch
            {
                return new ServerWeatherState();
            }
        }
    }
}
