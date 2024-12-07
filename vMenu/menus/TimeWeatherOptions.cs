using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CitizenFX.Core;

using MenuAPI;

using Newtonsoft.Json;

using vMenuClient.MenuAPIWrapper;

using vMenuShared;

using static vMenuClient.CommonFunctions;

namespace vMenuClient.menus
{
    public class TimeWeatherOptions
    {
        // Variables
        private WMenu menu;

        private WMenuItem overrideLocalTW;

        private WMenuItem timeList;
        private WMenuItem freezeTime;
        private WMenuItem timeSpeed;


        private WMenuItem useDynamicWeather;
        private WMenuItem dynamicWeatherList;
        private WMenuItem dynamicWeatherDay;

        private WMenuItem weatherTypeList;
        private WMenuItem snowEnabled;
        private WMenuItem blackoutList;

        private int DynamicWeatherSelectedDay
        {
            get
            {
                return int.Parse(dynamicWeatherDay.AsDynamicListItem().CurrentItem) - 1;
            }
            set
            {
                var cycle = TimeWeatherCommon.GetSelectedDynamicWeatherCycle();
                value %= cycle.Days;
                if (value < 0)
                {
                    value += cycle.Days;
                }
                dynamicWeatherDay.AsDynamicListItem().CurrentItem = $"{value + 1}";
            }
        }

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            menu = new WMenu(MenuTitle, "Server Time & Weather");

            overrideLocalTW = new MenuCheckboxItem("~y~Override Client-Side~s~", "If enabled, server-side time and weather will override client-side settings.", false).ToWrapped();
            overrideLocalTW.CheckboxChanged += (_s, args) => UpdateOverrideClientTW(args.Checked);
            menu.AddItem(overrideLocalTW);


            var timeSect = new List<WMenuItem>();

            timeList = new MenuListItem(
                "Time",
                TimeWeatherCommon.TimeListOptions,
                TimeWeatherCommon.TimeListOptions.Count / 2,
                "Select the time of day.").ToWrapped();
            timeList.ListSelected += (_s, args) =>
            {
                var serverTime = TimeWeatherCommon.GetServerTime();
                serverTime.Hour = args.ListIndex / 2;
                serverTime.Minute = 30 * (args.ListIndex % 2);
                UpdateServerTime(serverTime);
            };

            freezeTime = new MenuCheckboxItem("Freeze Time", "Keep the clock frozen.", false).ToWrapped();
            freezeTime.CheckboxChanged += (_s, args) =>
            {
                var serverTime = TimeWeatherCommon.GetServerTime();
                serverTime.Frozen = args.Checked;
                UpdateServerTime(serverTime);
            };

            var timeSpeedOptions = TimeWeatherCommon.TimeSpeeds.Select(f => $"{Math.Round(100 * f, 0)}%").ToList();
            timeSpeed = new MenuListItem(
                "Time Speed",
                timeSpeedOptions,
                TimeWeatherCommon.TimeSpeedIndex(1f),
                "Select the rate at which time progresses. Values less than 100% mean slower, values greater than 100% faster progression.").ToWrapped();
            timeSpeed.ListSelected += (_s, args) =>
            {
                var serverTime = TimeWeatherCommon.GetServerTime();
                serverTime.Speed = TimeWeatherCommon.TimeSpeeds[args.ListIndex];
                UpdateServerTime(serverTime);
            };

            timeSect.AddRange([timeList, freezeTime, timeSpeed]);


            var dynamicWeatherSect = new List<WMenuItem>();

            if (TimeWeatherCommon.WeatherCycles.Count > 0)
            {
                useDynamicWeather = new MenuCheckboxItem("Enable Dynamic Weather", "Whether to use dynamic or static weather.", false).ToWrapped();
                useDynamicWeather.CheckboxChanged += (_s, args) =>
                {
                    var serverWeatherSelection = TimeWeatherCommon.GetServerWeatherSelection();
                    serverWeatherSelection.IsDynamic = args.Checked;
                    UpdateServerWeatherSelection(serverWeatherSelection);
                };

                dynamicWeatherList = new MenuListItem(
                    "Dynamic Weather Cycle",
                    TimeWeatherCommon.WeatherCycles.Select(c => c.Name).ToList(),
                    0,
                    "Select the dynamic weather cycle to use.").ToWrapped();
                dynamicWeatherList.ListSelected += (_s, args) =>
                {
                    var serverWeatherSelection = TimeWeatherCommon.GetServerWeatherSelection();
                    serverWeatherSelection.Dynamic.CycleName = args.Item.GetCurrentSelection();
                    UpdateServerWeatherSelection(serverWeatherSelection);
                };

                dynamicWeatherDay = new MenuDynamicListItem(
                    "Dynamic Weather Cycle Day",
                    "1",
                    (_, left) => {
                        DynamicWeatherSelectedDay += left ? -1 : 1;
                        return $"{DynamicWeatherSelectedDay + 1}";
                    },
                    "The current day of the dynamic weather cycle.").ToWrapped();
                dynamicWeatherDay.DynamicListSelected += (_s, args) =>
                {
                    var serverWeatherSelection = TimeWeatherCommon.GetServerWeatherSelection();
                    serverWeatherSelection.Dynamic.Day = DynamicWeatherSelectedDay;
                    UpdateServerWeatherSelection(serverWeatherSelection);
                };

                dynamicWeatherSect.AddRange([useDynamicWeather, dynamicWeatherList, dynamicWeatherDay]);
            }


            var staticWeatherSect = new List<WMenuItem>();

            weatherTypeList = new MenuListItem(
                "Static Weather Type",
                TimeWeatherCommon.WeatherTypeOptions,
                0,
                "Select the static weather type.").ToWrapped();
            weatherTypeList.ListSelected += (_s, args) =>
            {
                var serverWeatherSelection = TimeWeatherCommon.GetServerWeatherSelection();
                serverWeatherSelection.Static.WeatherType =
                    TimeWeatherCommon.WeatherTypeOptionsIndexToType(args.ListIndex);
                switch(serverWeatherSelection.Static.WeatherType)
                {
                    case TimeWeatherCommon.WeatherType.Blizzard:
                    case TimeWeatherCommon.WeatherType.Snow:
                    case TimeWeatherCommon.WeatherType.SnowLight:
                    case TimeWeatherCommon.WeatherType.Xmas:
                        snowEnabled.AsCheckboxItem().Checked = true;
                        serverWeatherSelection.Static.Snow = true;
                        break;
                    default:
                        snowEnabled.AsCheckboxItem().Checked = false;
                        serverWeatherSelection.Static.Snow = false;
                        break;
                }
                UpdateServerWeatherSelection(serverWeatherSelection);
            };

            snowEnabled = new MenuCheckboxItem(
                "Static Weather Snow",
                "Enable or disable snow textures (snow is always enabled with ~b~Xmas~s~ weather).",
                false).ToWrapped();
            snowEnabled.CheckboxChanged += (_s, args) =>
            {
                var serverWeatherSelection = TimeWeatherCommon.GetServerWeatherSelection();
                serverWeatherSelection.Static.Snow = args.Checked;
                UpdateServerWeatherSelection(serverWeatherSelection);
            };

            blackoutList = new MenuListItem(
                "Static Weather Blackout",
                TimeWeatherCommon.BlackoutStateOptionsList,
                0,
                "Select the blackout state.").ToWrapped();
            blackoutList.ListSelected += (_s, args) =>
            {
                var serverWeatherSelection = TimeWeatherCommon.GetServerWeatherSelection();
                serverWeatherSelection.Static.Blackout = (TimeWeatherCommon.BlackoutState)args.ListIndex;
                UpdateServerWeatherSelection(serverWeatherSelection);
            };

            staticWeatherSect.AddRange([weatherTypeList, snowEnabled, blackoutList]);


            menu.AddSection("Time", timeSect);
            menu.AddSection("Dynamic Weather", dynamicWeatherSect);
            menu.AddSection(dynamicWeatherSect.Count > 0 ? "Static Weather" : "Weather", staticWeatherSect);
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
            return menu.Menu;
        }

        private void UpdateServerTime(TimeWeatherCommon.TimeState ts)
        {
            var json = JsonConvert.SerializeObject(ts);
            BaseScript.TriggerServerEvent("vMenu:UpdateServerTime", json);
        }

        private void UpdateServerWeatherSelection(TimeWeatherCommon.ServerWeatherSelection ws)
        {
            var json = JsonConvert.SerializeObject(ws);
            BaseScript.TriggerServerEvent("vMenu:UpdateServerWeatherSelection", json);
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
            var serverWeatherSelection = TimeWeatherCommon.GetServerWeatherSelection();

            var menuInvisible = !menu.Menu.Visible;


            if (!timeList.MenuItem.Selected || menuInvisible)
                timeList.AsListItem().ListIndex = (int)(2 * serverTime.Hour + serverTime.Minute / 30);

            if (!freezeTime.MenuItem.Selected || menuInvisible)
                freezeTime.AsCheckboxItem().Checked = serverTime.Frozen;

            if (!timeSpeed.MenuItem.Selected || menuInvisible)
                timeSpeed.AsListItem().ListIndex = TimeWeatherCommon.TimeSpeedIndex(serverTime.Speed);


            if (useDynamicWeather != null)
            {
                var dynamicWeather = serverWeatherSelection.Dynamic;

                if (!useDynamicWeather.MenuItem.Selected || menuInvisible)
                    useDynamicWeather.AsCheckboxItem().Checked = serverWeatherSelection.IsDynamic;

                if (!dynamicWeatherList.MenuItem.Selected || menuInvisible)
                {
                    dynamicWeatherList.AsListItem().ListIndex =
                        TimeWeatherCommon.WeatherCycles.FindIndex(s => s.Name == dynamicWeather.CycleName);
                }

                if (!dynamicWeatherDay.MenuItem.Selected || menuInvisible)
                    dynamicWeatherDay.AsDynamicListItem().CurrentItem = $"{dynamicWeather.Day + 1}";
            }


            var staticWeather = serverWeatherSelection.Static;

            if (!weatherTypeList.MenuItem.Selected || menuInvisible)
            {
                weatherTypeList.AsListItem().ListIndex =
                    TimeWeatherCommon.WeatherTypeToOptionsIndex(staticWeather.WeatherType);
            }
            if (!snowEnabled.MenuItem.Selected || menuInvisible)
                snowEnabled.AsCheckboxItem().Checked = staticWeather.Snow;

            if (!blackoutList.MenuItem.Selected || menuInvisible)
                blackoutList.AsListItem().ListIndex = (int)staticWeather.Blackout;

            await Delay(100);
        }
    }
}
