using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CitizenFX.Core;

using MenuAPI;

using Newtonsoft.Json;


using vMenuShared;

using static vMenuClient.CommonFunctions;

namespace vMenuClient.menus
{
    public class TimeWeatherOptions
    {
        // Variables
        private Menu menu;

        private MenuCheckboxItem overrideTime;
        private MenuListItem customTimeList;
        private MenuCheckboxItem freezeTime;

        private MenuCheckboxItem overrideWeather;
        private MenuListItem customWeatherList;
        private MenuCheckboxItem snowEnabled;
        private MenuListItem blackoutList;

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            menu = new Menu("Time & Weather", "Server Time & Weather");

            overrideTime = new MenuCheckboxItem("Override Time", "Whether to override the GTA Online time.", false);
            menu.AddMenuItem(overrideTime);

            var customtimeListOptions =
                Enumerable.Range(0,48).Select(i => $"{i / 2:D2}:{30 * (i % 2):D2}").ToList();
            customTimeList = new MenuListItem(
                "Custom Time",
                customtimeListOptions,
                customtimeListOptions.Count / 2,
                "Select custom time of day.");
            menu.AddMenuItem(customTimeList);

            freezeTime = new MenuCheckboxItem("Freeze Time", "Whether to keep the clock frozen.", false);
            menu.AddMenuItem(freezeTime);

            overrideWeather = new MenuCheckboxItem("Override Weather Type", "Whether to override the GTA Online weather type.", false);
            menu.AddMenuItem(overrideWeather);

            var customWeatherListOptions = new List<string>()
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

            menu.OnListItemSelect += (_menu, item, _ix, _a) => {
                var serverTime = TimeWeatherCommon.GetServerTime();
                var serverWeather = TimeWeatherCommon.GetServerWeather();

                if (item == customTimeList)
                {
                    serverTime.Hour = customTimeList.ListIndex / 2;
                    serverTime.Minute = 30 * (customTimeList.ListIndex % 2);
                    UpdateServerTime(serverTime);
                }
                else if (item == customWeatherList)
                {
                    serverWeather.WeatherType =
                        TimeWeatherCommon.WeatherNameToType[
                            customWeatherList.GetCurrentSelection().ToLower().Replace(" ", "")];

                    switch(serverWeather.WeatherType)
                    {
                        case TimeWeatherCommon.WeatherType.Blizzard:
                        case TimeWeatherCommon.WeatherType.Snow:
                        case TimeWeatherCommon.WeatherType.SnowLight:
                        case TimeWeatherCommon.WeatherType.Xmas:
                            snowEnabled.Checked = true;
                            serverWeather.Snow = true;
                            break;
                        default:
                            snowEnabled.Checked = false;
                            serverWeather.Snow = false;
                            break;
                    }
                    UpdateServerWeather(serverWeather);
                }
                else if (item == blackoutList)
                {
                    serverWeather.Blackout = (TimeWeatherCommon.BlackoutState)blackoutList.ListIndex;
                    UpdateServerWeather(serverWeather);
                }
            };

            menu.OnCheckboxChange += (_menu, item, _ix, check) => {
                var serverTime = TimeWeatherCommon.GetServerTime();
                var serverWeather = TimeWeatherCommon.GetServerWeather();

                if (item == overrideTime)
                {
                    serverTime.Override = check;
                    serverTime.Hour = customTimeList.ListIndex / 2;
                    serverTime.Minute = 30 * (customTimeList.ListIndex % 2);
                    UpdateServerTime(serverTime);
                }
                else if (item == freezeTime)
                {
                    serverTime.Frozen = check;
                    UpdateServerTime(serverTime);
                }
                else if (item == overrideWeather)
                {
                    serverWeather.Override = check;
                    UpdateServerWeather(serverWeather);
                }
                else if (item == snowEnabled)
                {
                    serverWeather.Snow = snowEnabled.Checked;
                    UpdateServerWeather(serverWeather);
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

        private void UpdateServerTime(TimeWeatherCommon.ServerTimeState ts)
        {
            var json = JsonConvert.SerializeObject(ts);
            BaseScript.TriggerServerEvent("vMenu:UpdateServerTime", json);
        }

        private void UpdateServerWeather(TimeWeatherCommon.ServerWeatherState ws)
        {
            var json = JsonConvert.SerializeObject(ws);
            BaseScript.TriggerServerEvent("vMenu:UpdateServerWeather", json);
        }

        public async Task Sync()
        {
            if (!await MainMenu.CheckVMenuEnabled())
                return;

            var serverTime = TimeWeatherCommon.GetServerTime();
            var serverWeather = TimeWeatherCommon.GetServerWeather();

            if (!overrideTime.Selected || !menu.Visible)
                overrideTime.Checked = serverTime.Override;

            if (!customTimeList.Selected || !menu.Visible)
                customTimeList.ListIndex = 2 * serverTime.Hour + serverTime.Minute / 30;

            if (!freezeTime.Selected || !menu.Visible)
                freezeTime.Checked = serverTime.Frozen;

            if (!overrideWeather.Selected || !menu.Visible)
                overrideWeather.Checked = serverWeather.Override;

            if (!customWeatherList.Selected || !menu.Visible)
            {
                customWeatherList.ListIndex =
                    customWeatherList
                        .ListItems
                        .FindIndex(s =>
                            s.ToLower().Replace(" ", "") ==
                                TimeWeatherCommon.WeatherTypeToName[serverWeather.WeatherType]);
            }

            if (!snowEnabled.Selected || !menu.Visible)
                snowEnabled.Checked = serverWeather.Snow;

            if (!blackoutList.Selected || !menu.Visible)
                blackoutList.ListIndex = (int)serverWeather.Blackout;

            await Delay(100);
        }
    }
}
