using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using CitizenFX.Core;
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


        public static List<string> TimeListOptions =
            Enumerable.Range(0, 48).Select(i => $"{i / 2:D2}:{30 * (i % 2):D2}").ToList();


        public static List<float> TimeSpeeds = new List<float>
            { 0.1f, 0.15f, 0.2f, 0.3f, 0.5f, 0.75f, 1f, 1.5f, 2f, 3f, 4f, 6f, 8f };

        public static int TimeSpeedIndex(float speed)
        {
            return TimeSpeeds
                .Select((f, i) => new Tuple<float, int>(f, i))
                .OrderBy(t => Math.Abs(t.Item1 - speed))
                .First()
                .Item2;
        }


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
            public float Minute { get; set; } = 0;
            public bool Frozen { get; set; } = false;
            public float Speed { get; set; } = 1.0f;

            public float DayMinutes
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
                    Speed = Speed,
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


        public class DynamicWeatherSelection
        {
            public int Day { get; set; } = 0;
            public string CycleName { get; set; } = null;
        }

        public class ServerWeatherSelection
        {
            public bool IsDynamic { get; set; } = false;

            public WeatherState Static { get; set; } = new WeatherState();
            public DynamicWeatherSelection Dynamic { get; set; } = new DynamicWeatherSelection();
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

        public static ServerWeatherSelection GetServerWeatherSelection()
        {
            if (!GetSettingsBool(Setting.vmenu_enable_time_weather_sync))
                return new ServerWeatherSelection();

            var json = GetSettingsString(Setting.vmenu_server_weather, "{}");
            if (string.IsNullOrEmpty(json))
                return new ServerWeatherSelection();

            try
            {
                return JsonConvert.DeserializeObject<ServerWeatherSelection>(json);
            }
            catch
            {
                return new ServerWeatherSelection();
            }
        }

        public static WeatherState GetServerWeather()
        {
            if (!GetSettingsBool(Setting.vmenu_enable_time_weather_sync))
                return new WeatherState();

            var json = GetSettingsString(Setting.vmenu_server_weather_state, "{}");
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

        public class WeatherCycleException : Exception
        {
            public WeatherCycleException(string message) : base(message)
            {
            }
        }

        public class WeatherCycleStateJson
        {
            public string Weather { get; set; }
            public bool Snow { get; set; }
            public bool Blackout { get; set; }
            public int Time { get; set; }

            public WeatherCycleStateJson Clone()
            {
                return new WeatherCycleStateJson()
                {
                    Weather = Weather,
                    Snow = Snow,
                    Blackout = Blackout,
                    Time = Time
                };
            }

            public WeatherState ToWeatherState(int index)
            {
                var name = Weather.ToLower().Replace(" ", "");
                if (!WeatherNameToType.ContainsKey(name))
                    throw new WeatherCycleException($"state {index} invalid weather \"{name}\"");

                return new WeatherState
                {
                    WeatherType = WeatherNameToType[name],
                    Snow = Snow,
                    Blackout = Blackout ? BlackoutState.Buildings : BlackoutState.Off,
                };
            }
        }

        public class WeatherCycleJson
        {
            public string Name { get; set; }
            public List<WeatherCycleStateJson> States { get; set; }
        }

        public class WeatherCycle
        {
            private readonly SortedDictionary<int, WeatherState> cycle;

            public string Name { get; private set; }

            public int Days { get; private set; }
            public int Hours => 24 * Days;

            private WeatherCycle(string name, SortedDictionary<int, WeatherState> cycle, int days)
            {
                Name = name;
                this.cycle = cycle;
                Days = days;
            }

            public WeatherState GetStateAt(int day, int hour)
            {
                int offset = (24 * day + hour) % Hours;
                return cycle.Last(kv => kv.Key <= offset).Value;
            }

            // If the hour of the first state is not 0, this function will transform the states such that it is, without
            // changing the cycle they generate
            private static List<WeatherCycleStateJson> TransformFirstHour0(List<WeatherCycleStateJson> states)
            {
                if (states.First().Time == 0)
                    return states;

                // Get all states from the end of the list that could also be moved in front of the first state without
                // changing the cycle
                var movableToFront = states
                    .Select((s, i) => new KeyValuePair<int, WeatherCycleStateJson>(i, s))
                    .Reverse()
                    .TakeWhile(kv => kv.Value.Time < states[0].Time)
                    .Reverse()
                    .ToList();

                // If such states exists, we move them to the front of the list
                if (movableToFront.Count != 0)
                {
                    states.RemoveRange(movableToFront.First().Key, states.Count - movableToFront.Count);
                    states = Enumerable.Concat(movableToFront.Select(kv => kv.Value), states).ToList();
                }

                if (states[0].Time == 0)
                    return states;

                // We now definitely know that the hour of the first state is less than that of the last. So if the
                // first state's hour is still not 0, we simply reuse the last state for that
                var hour0State = states.Last().Clone();
                hour0State.Time = 0;
                states.Insert(0, hour0State);

                return states;
            }

            public static WeatherCycle FromJson(WeatherCycleJson json)
            {
                var name = json.Name;
                var states = json.States;

                if (states.Count == 0)
                    throw new WeatherCycleException("List of states must not be empty");

                if (states.Count == 1)
                    return new WeatherCycle(
                        name,
                        new SortedDictionary<int, WeatherState> { [0] = states.First().ToWeatherState(0) }, 1);

                states = TransformFirstHour0(states);

                var cycle = new SortedDictionary<int, WeatherState>
                {
                    [0] = states.First().ToWeatherState(0),
                };

                int days = 1;
                int totalHours = 0;

                for (int i = 1; i < states.Count; i++)
                {
                    int prevHour = states[i - 1].Time;
                    int hour = states[i].Time;

                    if (hour < 0 || hour >= 24)
                        throw new WeatherCycleException($"state {i}'s hour is not in [0,24)");

                    int diff;
                    if (hour > prevHour)
                    {
                        diff = hour - prevHour;
                    }
                    else
                    {
                        days += 1;
                        diff = 24 - (prevHour - hour);
                    }

                    totalHours += diff;

                    cycle[totalHours] = states[i].ToWeatherState(i);
                }

                return new WeatherCycle(name, cycle, days);
            }
        }

        private static List<WeatherCycle> WeatherCyclesFromJson()
        {
            var errorPrefix = "^1[vMenu] [ERROR]^7 ";

            var json = LoadResourceFile(GetCurrentResourceName(), "config/weathercycles.json");
            if (string.IsNullOrEmpty(json))
                return new List<WeatherCycle>();

            List<WeatherCycleJson> userCycles;
            try
            {
                userCycles = JsonConvert.DeserializeObject<List<WeatherCycleJson>>(json);
            }
            catch
            {
                Debug.WriteLine($"{errorPrefix}Could not parse \"config/weathercycles.json\"");
                return new List<WeatherCycle>();
            }

            var cycles = new List<WeatherCycle>();
            foreach (var userCycle in userCycles)
            {
                var name = userCycle.Name;
                try
                {
                    cycles.Add(WeatherCycle.FromJson(userCycle));
                }
                catch (WeatherCycleException e)
                {
                    Debug.WriteLine($"{errorPrefix}Error parsing cycle {name}: {e.Message}");
                }
                catch
                {
                    Debug.WriteLine($"{errorPrefix}Error parsing cycle {name}");
                }
            }

            return cycles;
        }

        private static List<WeatherCycle> weatherCycles = null;
        public static List<WeatherCycle> WeatherCycles
        {
            get
            {
                if (weatherCycles == null)
                {
                    weatherCycles = WeatherCyclesFromJson();
                }

                return weatherCycles;
            }
        }

        public static Dictionary<string, WeatherCycle> WeatherCyclesDict =
            WeatherCycles.ToDictionary(c => c.Name, c => c);

        public static WeatherCycle GetSelectedDynamicWeatherCycle()
        {
            var selection = GetServerWeatherSelection();
            if (string.IsNullOrEmpty(selection.Dynamic.CycleName))
                return null;

            WeatherCycle cycle;
            WeatherCyclesDict.TryGetValue(selection.Dynamic.CycleName, out cycle);

            return cycle;
        }
    }
}
