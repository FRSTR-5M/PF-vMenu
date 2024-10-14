using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using MenuAPI;

using vMenuShared;

namespace vMenuClient
{
    public class PlayerTimeWeatherOptions
    {
        public enum VariableSource
        {
            Server,
            GtaOnline,
            Custom
        }

        public class ClientTimeState
        {
            public VariableSource TimeSource { get; set; } = VariableSource.Server;

            public bool Frozen { get; set; } = false;
            public int CustomHour { get; set; } = 12;
            public int CustomMinute { get; set; } = 0;
        }

        public class ClientWeatherState
        {
            public bool Override { get; set; } = false;

            public VariableSource WeatherTypeSource { get; set; } = VariableSource.Server;

            public TimeWeatherCommon.WeatherType CustomWeatherType { get; set; } = TimeWeatherCommon.WeatherType.Clear;

            public bool Snow { get; set; } = false;
            public TimeWeatherCommon.BlackoutState Blackout { get; set; } = TimeWeatherCommon.BlackoutState.Off;
        }

        // Variables
        private Menu menu;

        private MenuListItem timeList;
        private MenuListItem customTimeList;
        private MenuCheckboxItem freezeTime;
        private MenuCheckboxItem overrideWeather;
        private MenuListItem customWeatherList;
        private MenuCheckboxItem snowEnabled;
        private MenuListItem blackoutList;

        public ClientTimeState ClientTime { get; private set; } = new ClientTimeState();
        public ClientWeatherState ClientWeather { get; private set; } = new ClientWeatherState();

        void ToggleCustomTimeOptions(bool enabled)
        {
            customTimeList.Enabled = enabled;
            freezeTime.Enabled = enabled;
        }

        void ToggleCustomWeatherOptions(bool enabled)
        {
            customWeatherList.Enabled = enabled;
            snowEnabled.Enabled = enabled;
            blackoutList.Enabled = enabled;
        }

        bool enabled = true;
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (value && !enabled)
                {
                    enabled = true;

                    ToggleCustomTimeOptions(true);
                    ToggleCustomWeatherOptions(true);
                }
                else if (!value && enabled)
                {
                    enabled = false;

                    ToggleCustomTimeOptions(false);
                    ToggleCustomWeatherOptions(false);

                    timeList.ListIndex = 0;
                    overrideWeather.Checked = false;
                }
            }
        }

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            menu = new Menu("Time & Weather", "Local Time & Weather");

            timeList = new MenuListItem("Time", new List<string>{"Server", "GTA Online", "Custom"}, 0, "Select the time to use.");
            menu.AddMenuItem(timeList);

            var customTimeListOptions =
                Enumerable.Range(0,48).Select(i => $"{i / 2:D2}:{30 * (i % 2):D2}").ToList();
            customTimeList = new MenuListItem(
                "Custom Time",
                customTimeListOptions,
                customTimeListOptions.Count / 2,
                "Select custom time of day.");
            menu.AddMenuItem(customTimeList);

            freezeTime = new MenuCheckboxItem("Freeze Time", "Keep the clock frozen at the selected custom time.", false);
            menu.AddMenuItem(freezeTime);

            ToggleCustomTimeOptions(false);

            overrideWeather = new MenuCheckboxItem("Override Weather", "Whether to override the server weather.", false);
            menu.AddMenuItem(overrideWeather);

            var customWeatherListOptions = new List<string>()
            {
                "~italic~Server~italic~",
                "~italic~GTA Online~italic~",
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
            customWeatherList = new MenuListItem("Weather Type", customWeatherListOptions, 0, "Select the weather type.");
            menu.AddMenuItem(customWeatherList);

            snowEnabled = new MenuCheckboxItem("Snow", "Enable or disable snow textures (snow is always enabled with Xmas weather).", false);
            menu.AddMenuItem(snowEnabled);

            blackoutList = new MenuListItem(
                "Blackout",
                new List<string>{"Off", "Buildings", "Everything"},
                0,
                "Select the blackout state.");
            menu.AddMenuItem(blackoutList);

            ToggleCustomWeatherOptions(false);

            menu.OnListIndexChange += (_menu, item, _oldIx, _newIx, _itemIx) => {
                if (item == timeList)
                {
                    string timeSelection = item.GetCurrentSelection().ToLower().Replace(" ", "");
                    switch (timeSelection)
                    {
                        case "server":
                            ClientTime.TimeSource = VariableSource.Server;
                            break;
                        case "gtaonline":
                            ClientTime.TimeSource = VariableSource.GtaOnline;
                            break;
                        case "custom":
                            ClientTime.TimeSource = VariableSource.Custom;
                            break;
                    }

                    ToggleCustomTimeOptions(ClientTime.TimeSource == VariableSource.Custom);
                }
                else if (item == customTimeList)
                {
                    ClientTime.CustomHour = customTimeList.ListIndex / 2;
                    ClientTime.CustomMinute = 30 * (customTimeList.ListIndex % 2);
                }
                else if (item == customWeatherList)
                {
                    switch (customWeatherList.ListIndex)
                    {
                        case 0:
                            ClientWeather.WeatherTypeSource = VariableSource.Server;
                            break;
                        case 1:
                            ClientWeather.WeatherTypeSource = VariableSource.GtaOnline;
                            break;
                        default:
                            string weather = customWeatherList.GetCurrentSelection().ToLower().Replace(" ", "");
                            ClientWeather.WeatherTypeSource = VariableSource.Custom;
                            ClientWeather.CustomWeatherType = TimeWeatherCommon.WeatherNameToType[weather];
                            ClientWeather.Snow = snowEnabled.Checked = weather is "snowlight" or "blizzard" or "snow" or "xmas";
                            break;
                    }
                }
                else if (item == blackoutList)
                {
                    ClientWeather.Blackout = (TimeWeatherCommon.BlackoutState)blackoutList.ListIndex;
                }
            };

            menu.OnCheckboxChange += (_menu, item, _ix, check) => {
                if (item == overrideWeather)
                {
                    ClientWeather.Override = check;

                    ToggleCustomWeatherOptions(check);
                }
                else if (item == freezeTime)
                {
                    ClientTime.Frozen = check;
                }
                else if (item == snowEnabled)
                {
                    ClientWeather.Snow = check;
                }
            };
        }

        /// <summary>
        /// Create the menu if it doesn't exist, and then returns it.
        /// </summary>
        /// <returns>The Menu</returns>
        public Menu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }
    }
}
