using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.UI.Screen;
using static CitizenFX.Core.Native.API;
using static vMenuClient.CommonFunctions;
using static vMenuShared.PermissionsManager;
using vMenuShared;

namespace vMenuClient
{
    public class PlayerTimeWeatherOptions
    {
        public enum BlackoutState
        {
            Off = 0,
            Buildings = 1,
            All = 2
        }

        // Variables
        private Menu menu;

        public MenuCheckboxItem clientSidedEnabled;
        public MenuCheckboxItem overrideTime;
        public MenuCheckboxItem freezeTime;
        public MenuListItem timeDataList;
        public MenuListItem blackoutList;
        public MenuCheckboxItem snowEnabled;
        public List<string> weatherListData = new List<string>() { "Dynamic", "Clear", "Extra Sunny", "Clouds", "Overcast", "Rain", "Clearing", "Thunder", "Smog", "Foggy", "Halloween", "Xmas", "Snow", "Snow Light", "Blizzard", "Neutral" };
        public MenuListItem weatherList;

        public int SelectedTimeHour
        {
            get => timeDataList.ListIndex / 2;
        }

        public int SelectedTimeMinute
        {
            get => 30 * (timeDataList.ListIndex % 2);
        }

        public BlackoutState SelectedBlackoutState
        {
            get => (BlackoutState)blackoutList.ListIndex;
        }

        public string SelectedWeatherId
        {
            get => weatherList.GetCurrentSelection().Replace(" ", "").ToLower();
        }

        private static void ResetTime()
        {
            NetworkClearClockTimeOverride();
        }

        private static void ResetWeather()
        {
            ClearOverrideWeather();
            ClearWeatherTypePersist();
        }

        public static void ResetAll()
        {
            ResetTime();
            ResetWeather();

            SetArtificialLightsState(false);

            ForceSnowPass(false);
            SetForceVehicleTrails(false);
            SetForcePedFootstepsTracks(false);
        }

        private void ApplySelectedTime()
        {
            if (overrideTime.Checked)
            {
                NetworkOverrideClockTime(SelectedTimeHour, SelectedTimeMinute, 0);
            }
            else
            {
                ResetTime();
            }
        }

        private void ApplySelectedWeather()
        {
            if (SelectedWeatherId != "dynamic")
            {
                SetWeatherTypeNowPersist(SelectedWeatherId);
            }
            else
            {
                ResetWeather();
            }

            ForceSnowPass(snowEnabled.Checked);
            SetForceVehicleTrails(snowEnabled.Checked);
            SetForcePedFootstepsTracks(snowEnabled.Checked);

            SetArtificialLightsState(SelectedBlackoutState != BlackoutState.Off);
            SetArtificialLightsStateAffectsVehicles(SelectedBlackoutState == BlackoutState.All);
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

                    clientSidedEnabled.Enabled = true;
                }
                else if (!value && enabled)
                {
                    enabled = false;

                    clientSidedEnabled.Enabled = false;
                    clientSidedEnabled.Checked = false;

                    ResetAll();
                }
            }
        }

        private bool clientSidedEnabledOld = false;
        private bool overrideTimeOld = false;
        private int timeSelectionOld = -1;

        private bool CheckNeedsTimeUpdate()
        {
            bool ret = (clientSidedEnabled.Checked && !clientSidedEnabledOld)
                || (overrideTime.Checked && !overrideTimeOld)
                || (timeSelectionOld != timeDataList.ListIndex);

            return ret; 
        }

        private void ResetNeedsTimeUpdate()
        {
            clientSidedEnabledOld = clientSidedEnabled.Checked;
            overrideTimeOld = overrideTime.Checked;
            timeSelectionOld = timeDataList.ListIndex;
        }

        public void ApplyTimeTick()
        {
            if (!clientSidedEnabled.Checked)
                return;

            if (overrideTime.Checked)
            {
                if (freezeTime.Checked || CheckNeedsTimeUpdate())
                {
                    ApplySelectedTime();
                }
            }
            else
            {
                ResetTime();
            }

            ResetNeedsTimeUpdate();
        }

        public void ApplyWeatherTick()
        {
            if (!clientSidedEnabled.Checked)
                return;

            ApplySelectedWeather();
        }

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {
            menu = new Menu("Time & Weather", "Time & Weather Options");

            clientSidedEnabled = new MenuCheckboxItem("Client-Sided Time & Weather", "Enable or disable client-sided time and weather.", false);
            menu.AddMenuItem(clientSidedEnabled);

            overrideTime = new MenuCheckboxItem("Override Time", "Whether to use the selected or the global GTA time.", true);
            menu.AddMenuItem(overrideTime);

            freezeTime = new MenuCheckboxItem("Freeze Time", "Keep the clock frozen at the selected time.");
            menu.AddMenuItem(freezeTime);

            List<string> timeData = new List<string>();
            for (var i = 0; i < 48; i++)
            {
                timeData.Add($"{i / 2:D2}:{30 * (i % 2):D2}");
            }
            timeDataList = new MenuListItem("Change Time", timeData, 24, "Select time of day.");
            menu.AddMenuItem(timeDataList);

            blackoutList = new MenuListItem("Blackout", new List<string>{"Off", "Buildings", "All"}, 0, "Toggle blackout mode.");
            menu.AddMenuItem(blackoutList);

            snowEnabled = new MenuCheckboxItem("Snow", "Enable or disable snow effects (cannot disable snow with Xmas weather).");
            menu.AddMenuItem(snowEnabled);

            weatherList = new MenuListItem("Change Weather", weatherListData, 0, "Select weather.");
            menu.AddMenuItem(weatherList);

            menu.OnCheckboxChange += (_menu, item, _index, checked_) => {
                if (item == clientSidedEnabled && !checked_)
                {
                    ResetAll();
                }
            };

            menu.OnListIndexChange += (_menu, item, _oldIx, _newIx, _itemIx) => {
                if (item == weatherList)
                {
                    snowEnabled.Checked = SelectedWeatherId is "snowlight" or "blizzard" or "snow";
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