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

        private MenuCheckboxItem overrideLocalTW;

        private MenuListItem timeList;
        private MenuCheckboxItem freezeTime;

        private MenuListItem weatherTypeList;
        private MenuCheckboxItem snowEnabled;
        private MenuListItem blackoutList;

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            menu = new Menu(MenuTitle, "Server Time & Weather");
            AddSpacerAction(menu);

            overrideLocalTW = new MenuCheckboxItem("~y~Override Client-Side~s~", "If enabled, server-side time and weather will override client-side settings.", false);
            menu.AddMenuItem(overrideLocalTW);

            var timeSpacer = GetSpacerMenuItem("Time");
            menu.AddMenuItem(timeSpacer);

            timeList = new MenuListItem(
                "Time",
                TimeWeatherCommon.TimeListOptions,
                TimeWeatherCommon.TimeListOptions.Count / 2,
                "Select the time of day.");
            menu.AddMenuItem(timeList);

            freezeTime = new MenuCheckboxItem("Freeze Time", "Keep the clock frozen.", false);
            menu.AddMenuItem(freezeTime);

            var weatherSpacer = GetSpacerMenuItem("Weather");
            menu.AddMenuItem(weatherSpacer);

            weatherTypeList = new MenuListItem(
                "Weather Type",
                TimeWeatherCommon.WeatherTypeOptionsList,
                0,
                "Select the weather type.");
            menu.AddMenuItem(weatherTypeList);

            snowEnabled = new MenuCheckboxItem(
                "Snow",
                "Enable or disable snow textures (snow is always enabled with ~b~Xmas~s~ weather).",
                false);
            menu.AddMenuItem(snowEnabled);

            blackoutList = new MenuListItem(
                "Blackout",
                TimeWeatherCommon.BlackoutStateOptionsList,
                0,
                "Select the blackout state.");
            menu.AddMenuItem(blackoutList);

            menu.OnListItemSelect += (_menu, item, _ix, _a) => {
                var serverTime = TimeWeatherCommon.GetServerTime();
                var serverWeather = TimeWeatherCommon.GetServerWeather();

                if (item == timeList)
                {
                    serverTime.Hour = timeList.ListIndex / 2;
                    serverTime.Minute = 30 * (timeList.ListIndex % 2);
                    UpdateServerTime(serverTime);
                }
                else if (item == weatherTypeList)
                {
                    string weatherType = weatherTypeList.GetCurrentSelection().ToLower().Replace(" ", "");
                    serverWeather.WeatherType = TimeWeatherCommon.WeatherNameToType[weatherType];

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

                if (item == overrideLocalTW)
                {
                    UpdateOverrideClientTW(check);
                }
                else if (item == freezeTime)
                {
                    serverTime.Frozen = check;
                    UpdateServerTime(serverTime);
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

        private void UpdateServerTime(TimeWeatherCommon.TimeState ts)
        {
            var json = JsonConvert.SerializeObject(ts);
            BaseScript.TriggerServerEvent("vMenu:UpdateServerTime", json);
        }

        private void UpdateServerWeather(TimeWeatherCommon.WeatherState ws)
        {
            var json = JsonConvert.SerializeObject(ws);
            BaseScript.TriggerServerEvent("vMenu:UpdateServerWeather", json);
        }

        private void UpdateOverrideClientTW(bool override_)
        {
            BaseScript.TriggerServerEvent("vMenu:UpdateOverrideClientTW", override_);
        }

        public async Task Sync()
        {
            if (!await MainMenu.CheckVMenuEnabled())
                return;

            var serverTime = TimeWeatherCommon.GetServerTime();
            var serverWeather = TimeWeatherCommon.GetServerWeather();

            if (!timeList.Selected || !menu.Visible)
                timeList.ListIndex = 2 * serverTime.Hour + serverTime.Minute / 30;

            if (!freezeTime.Selected || !menu.Visible)
                freezeTime.Checked = serverTime.Frozen;

            if (!weatherTypeList.Selected || !menu.Visible)
            {
                weatherTypeList.ListIndex =
                    weatherTypeList
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
