using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MenuAPI;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static vMenuClient.CommonFunctions;
using static vMenuShared.PermissionsManager;
using static vMenuShared.ConfigManager;

namespace vMenuClient
{
    public class TeleportOptions
    {
        // Variables
        private Menu menu;

        private MenuItem tpToPrev;
        private Vector3? prevTpCoords = null;
        private float? prevTpHeading = null;
        private bool prevTpSafe = true;

        private MenuItem personalTpLocationsBtn;
        private Menu personalTpLocationsMenu;
        private List<TeleportLocation> personalTpLocations = new List<TeleportLocation>();

        // keybind states
        public bool KbTpToWaypoint { get; private set; } = UserDefaults.KbTpToWaypoint;
        public int KbTpToWaypointKey { get; } = vMenuShared.ConfigManager.GetSettingsInt(vMenuShared.ConfigManager.Setting.vmenu_teleport_to_wp_keybind_key) != -1
            ? vMenuShared.ConfigManager.GetSettingsInt(vMenuShared.ConfigManager.Setting.vmenu_teleport_to_wp_keybind_key)
            : 168; // 168 (F7 by default)

        internal static List<vMenuShared.ConfigManager.TeleportLocation> TpLocations = new List<vMenuShared.ConfigManager.TeleportLocation>();

        public TeleportOptions()
        {
            if (IsAllowed(Permission.TPTeleportToPrev))
            {
                RegisterKeyMapping($"{GetSettingsString(Setting.vmenu_individual_server_id)}vMenu:tpToPrevLocation", "vMenu TP To Prev. Location", "keyboard", "");
                RegisterCommand($"{GetSettingsString(Setting.vmenu_individual_server_id)}vMenu:tpToPrevLocation", new Action<dynamic, List<dynamic>, string>(async (dynamic source, List<dynamic> args, string rawCommand) =>
                {
                    if (!MainMenu.vMenuEnabled)
                        return;

                    await TeleportToPrevTPLocation();
                }), false);
            }

            if(IsAllowed(Permission.TPTeleportToPrev))
            {
                RegisterKeyMapping($"{GetSettingsString(Setting.vmenu_individual_server_id)}vMenu:overridePrevLocation", "vMenu Override Prev. TP Location", "keyboard", "");
                RegisterCommand($"{GetSettingsString(Setting.vmenu_individual_server_id)}vMenu:overridePrevLocation", new Action<dynamic, List<dynamic>, string>((dynamic source, List<dynamic> args, string rawCommand) =>
                {
                    if (!MainMenu.vMenuEnabled)
                        return;

                    OverridePrevLocation(true);
                }), false);
            }
        }

        /// <summary>
        /// Set the previous teleport location
        /// </summary>
        private void SetPrevTpLocation(Vector3? coords, float? heading, bool safe = false)
        {
            if (coords == null)
                return;

            prevTpCoords = coords;
            prevTpHeading = heading;
            prevTpSafe = safe;

            tpToPrev.Enabled = true;
        }

        /// <summary>
        /// Teleport to the previous TP location.
        /// </summary>
        public async Task TeleportToPrevTPLocation()
        {
            if (prevTpCoords is Vector3 coords)
            {
                await TeleportToCoords(coords, prevTpSafe);
                if (prevTpHeading is float heading)
                {
                    SetEntityHeading(Game.PlayerPed.Handle, heading);
                    SetGameplayCamRelativeHeading(0f);
                }
            }
            else
            {
                Notify.Error("There was no previous teleport location!");
            }
        }

        /// <summary>
        /// Set the temporary teleport location to the current location.
        /// </summary>
        public void OverridePrevLocation(bool notify)
        {
            var coords = Game.PlayerPed.Position;
            var heading = Game.PlayerPed.Heading;

            SetPrevTpLocation(coords, heading, true);

            if (notify)
                Notify.Info("Previous TP location overridden.");
        }

        /// <summary>
        /// Load all saved personal teleport locations.
        /// </summary>
        public void LoadPersonalPlayerLocations()
        {
            personalTpLocations.Clear();

            var tpLocStrs = new List<string>();
            var findHandle = StartFindKvp("vmenu_tp_");
            while (true)
            {
                var tpLocStr = FindKvp(findHandle);

                if (tpLocStr is not "" and not null and not "NULL")
                {
                    tpLocStrs.Add(tpLocStr);
                }
                else
                {
                    EndFindKvp(findHandle);
                    break;
                }
            }

            foreach (var tpLocStr in tpLocStrs)
            {
                var tpLocJson = StorageManager.GetJsonData(tpLocStr);
                var tpLoc = JsonConvert.DeserializeObject<TeleportLocation>(tpLocJson);
                AddPersonalPlayerLocationToMenu(tpLoc);
            }
        }

        /// <summary>
        /// Save a new personal teleport location.
        /// </summary>
        public async void SavePersonalPlayerLocation()
        {
            var pos = Game.PlayerPed.Position;
            var heading = Game.PlayerPed.Heading;
            var locationName = await GetUserInput("Enter location save name", 30);
            if (string.IsNullOrEmpty(locationName))
            {
                Notify.Error(CommonErrors.InvalidInput);
                return;
            }
            if (personalTpLocations.Any(loc => loc.name == locationName))
            {
                Notify.Error("This location name is already used, please use a different name.");
                return;
            }

            var tpLoc = new TeleportLocation(locationName, pos, heading);
            var tpLocJson = JsonConvert.SerializeObject(tpLoc);

            StorageManager.SaveJsonData($"vmenu_tp_{locationName}", tpLocJson, true);
            AddPersonalPlayerLocationToMenu(tpLoc);
        }

        public void RemovePersonalPlayerLocation(TeleportLocation tpLoc)
        {
            StorageManager.DeleteSavedStorageItem($"vmenu_tp_{tpLoc.name}");
            personalTpLocations.Remove(tpLoc);
        }

        public void AddPersonalPlayerLocationToMenu(TeleportLocation tpLoc)
        {
            personalTpLocations.Add(tpLoc);

            float x = tpLoc.coordinates.X, y = tpLoc.coordinates.Y, z = tpLoc.coordinates.Z;

            var tpLocBtn = new MenuItem(tpLoc.name, $"Teleport to ~b~{tpLoc.name}~w~") { Label = "→→→" };
            var tpLocMenu = new Menu(MenuTitle, tpLoc.name);

            var tpBtn = new MenuItem("Teleport", $"Teleport to~n~x: ~y~{x}~n~~s~y: ~y~{y}~n~~s~z: ~y~{z}~n~~s~heading: ~y~{tpLoc.heading}");
            var delBtn = new MenuItem("~r~Delete~s~", "~r~Delete this teleport location.~n~Warning: this can NOT be undone!~s~");

            tpLocMenu.AddMenuItem(tpBtn);
            tpLocMenu.AddMenuItem(delBtn);

            tpLocMenu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == tpBtn)
                {
                    SetPrevTpLocation(tpLoc.coordinates, tpLoc.heading, true);
                    await TeleportToCoords(tpLoc.coordinates, true);
                    SetEntityHeading(Game.PlayerPed.Handle, tpLoc.heading);
                    SetGameplayCamRelativeHeading(0f);
                }
                else if (item == delBtn)
                {
                    tpLocMenu.GoBack();
                    personalTpLocationsMenu.RemoveMenuItem(tpLocBtn);
                    RemovePersonalPlayerLocation(tpLoc);
                    if (personalTpLocations.Count == 0)
                    {
                        personalTpLocationsMenu.GoBack();
                        personalTpLocationsBtn.Enabled = false;
                    }
                }
            };

            tpLocMenu.OnMenuClose += (sender) =>
            {
                // So delete is not selected when the menu is reopened
                tpLocMenu.RefreshIndex();
            };

            MenuController.AddSubmenu(personalTpLocationsMenu, tpLocMenu);
            MenuController.BindMenuItem(personalTpLocationsMenu, tpLocMenu, tpLocBtn);

            personalTpLocationsMenu.AddMenuItem(tpLocBtn);
            personalTpLocationsMenu.SortMenuItems((a, b) => string.Compare(a.Text, b.Text));

            personalTpLocationsBtn.Enabled = true;
        }

        /// <summary>
        /// Creates the menu.
        /// </summary>
        private void CreateMenu()
        {

            menu = new Menu(MenuTitle, "Teleport");
            // menu items
            var teleportMenu = new Menu(MenuTitle, "Server Teleport Locations");
            var teleportMenuBtn = new MenuItem("Server Teleport Locations", "Teleport to pre-configured locations, added by the server owner.");
            MenuController.AddSubmenu(menu, teleportMenu);
            MenuController.BindMenuItem(menu, teleportMenu, teleportMenuBtn);

            personalTpLocationsMenu = new Menu(MenuTitle, "Personal Teleport Locations");
            personalTpLocationsBtn = new MenuItem("Personal Teleport Locations", "Teleport to your personal teleport locations.") { Label = "→→→" };
            MenuController.AddSubmenu(menu, personalTpLocationsMenu);
            MenuController.BindMenuItem(menu, personalTpLocationsMenu, personalTpLocationsBtn);

            // Keybind settings menu items
            var kbTpToWaypoint = new MenuCheckboxItem("Teleport To Waypoint", "Teleport to your waypoint when pressing the keybind. By default, this keybind is set to ~r~F7~s~, server owners are able to change this however so ask them if you don't know what it is.", KbTpToWaypoint);
            var backBtn = new MenuItem("Back");

            // Teleportation options
            if (IsAllowed(Permission.TPTeleportToWp) || IsAllowed(Permission.TPTeleportLocations) || IsAllowed(Permission.TPTeleportToCoord) || IsAllowed(Permission.TPTeleportPersonalLocations) || IsAllowed(Permission.TPTeleportToPrev))
            {
                var tptowp = new MenuItem("Teleport To Waypoint", "Teleport to the waypoint on your map.");
                var tpToCoord = new MenuItem("Teleport To Coordinates", "Enter the X, Y, Z coordinates and you will be teleported to that location.");
                tpToPrev = new MenuItem("Teleport To Previous Location", "Teleport to the location you last teleported to.");
                var overridePrevBtn = new MenuItem("Override Previous Location", "Overrides the previous teleport location with your current position.");
                var savePersonalLocationBtn = new MenuItem("Save Personal Teleport Location", "Adds your current location to the personal teleport locations menu saved locally.");
                var saveLocationBtn = new MenuItem("Save Server Teleport Location", "Adds your current location to the teleport locations menu and saves it on the server ~r~~h~(script restart required after adding new location(s)).");
                menu.OnItemSelect += async (sender, item, index) =>
                {
                    // Teleport to waypoint.
                    if (item == tptowp)
                    {
                        var coords = await TeleportToWp();
                        SetPrevTpLocation(coords, null, false);
                    }
                    else if (item == tpToCoord)
                    {
                        var x = await GetUserInput("Enter X coordinate.");
                        if (string.IsNullOrEmpty(x))
                        {
                            Notify.Error(CommonErrors.InvalidInput);
                            return;
                        }
                        var y = await GetUserInput("Enter Y coordinate.");
                        if (string.IsNullOrEmpty(y))
                        {
                            Notify.Error(CommonErrors.InvalidInput);
                            return;
                        }
                        var z = await GetUserInput("Enter Z coordinate.");
                        if (string.IsNullOrEmpty(z))
                        {
                            Notify.Error(CommonErrors.InvalidInput);
                            return;
                        }

                        if (!float.TryParse(x, out var posX))
                        {
                            if (int.TryParse(x, out var intX))
                            {
                                posX = intX;
                            }
                            else
                            {
                                Notify.Error("You did not enter a valid X coordinate.");
                                return;
                            }
                        }
                        if (!float.TryParse(y, out var posY))
                        {
                            if (int.TryParse(y, out var intY))
                            {
                                posY = intY;
                            }
                            else
                            {
                                Notify.Error("You did not enter a valid Y coordinate.");
                                return;
                            }
                        }
                        if (!float.TryParse(z, out var posZ))
                        {
                            if (int.TryParse(z, out var intZ))
                            {
                                posZ = intZ;
                            }
                            else
                            {
                                Notify.Error("You did not enter a valid Z coordinate.");
                                return;
                            }
                        }

                        var coords = new Vector3(posX, posY, posZ);
                        SetPrevTpLocation(coords, null, true);
                        await TeleportToCoords(coords, true);
                    }
                    else if (item == tpToPrev)
                    {
                        await TeleportToPrevTPLocation();
                    }
                    else if (item == overridePrevBtn)
                    {
                        OverridePrevLocation(false);
                    }
                    else if (item == savePersonalLocationBtn)
                    {
                        SavePersonalPlayerLocation();
                    }
                    else if (item == saveLocationBtn)
                    {
                        SavePlayerLocationToLocationsFile();
                    }
                };

                if (IsAllowed(Permission.TPTeleportToWp))
                {
                    menu.AddMenuItem(tptowp);
                }
                if (IsAllowed(Permission.TPTeleportToCoord))
                {
                    menu.AddMenuItem(tpToCoord);
                }
                if (IsAllowed(Permission.TPTeleportToPrev))
                {
                    tpToPrev.Enabled = false;
                    menu.AddMenuItem(tpToPrev);
                }
                if (IsAllowed(Permission.TPTeleportPersonalLocations))
                {
                    personalTpLocationsBtn.Enabled = false;
                    menu.AddMenuItem(personalTpLocationsBtn);
                    LoadPersonalPlayerLocations();
                }
                if (IsAllowed(Permission.TPTeleportLocations))
                {
                    menu.AddMenuItem(teleportMenuBtn);

                    MenuController.AddSubmenu(menu, teleportMenu);
                    MenuController.BindMenuItem(menu, teleportMenu, teleportMenuBtn);
                    teleportMenuBtn.Label = "→→→";

                    teleportMenu.OnMenuOpen += (sender) =>
                    {
                        var jsonFile2 = LoadResourceFile(GetCurrentResourceName(), "config/TeleportCategories.json");
                        var data2 = JsonConvert.DeserializeObject<vMenuShared.ConfigManager.LocationsSubMenu>(jsonFile2);

                        if (teleportMenu.Size != data2.teleports.Count())
                        {
                            teleportMenu.ClearMenuItems();
                            foreach (var location in data2.teleports)
                            {
                                Debug.WriteLine(location.JsonName);

                                var jsonFile = LoadResourceFile(GetCurrentResourceName(), "config/locations/" + location.JsonName);
                                var data = JsonConvert.DeserializeObject<vMenuShared.ConfigManager.Locationsteleport>(jsonFile);
                                Menu teleportSubMenu = new Menu(MenuTitle, location.name);
                                MenuItem teleportSubMenuBtn = new MenuItem(location.name, $"Teleport to ~b~{location.name}~w~, added by the server owner.") { Label = "→→→" };
                                teleportMenu.AddMenuItem(teleportSubMenuBtn);


                                foreach (var tplocations in data.teleports)
                                {
                                    var x = Math.Round(tplocations.coordinates.X, 2);
                                    var y = Math.Round(tplocations.coordinates.Y, 2);
                                    var z = Math.Round(tplocations.coordinates.Z, 2);
                                    var heading = Math.Round(tplocations.heading, 2);
                                    var tpBtn = new MenuItem(tplocations.name, $"Teleport to ~y~{tplocations.name}~n~~s~x: ~y~{x}~n~~s~y: ~y~{y}~n~~s~z: ~y~{z}~n~~s~heading: ~y~{heading}") { ItemData = tplocations };
                                    teleportSubMenu.AddMenuItem(tpBtn);
                                }

                                if (teleportSubMenu.Size > 0)
                                {
                                    MenuController.AddSubmenu(teleportMenu, teleportSubMenu);
                                    MenuController.BindMenuItem(teleportMenu, teleportSubMenu, teleportSubMenuBtn);
                                }
                                teleportSubMenu.OnItemSelect += async (sender, item, index) =>
                                {
                                    if (item.ItemData is vMenuShared.ConfigManager.TeleportLocation tl)
                                    {
                                        SetPrevTpLocation(tl.coordinates, tl.heading, true);
                                        await TeleportToCoords(tl.coordinates, true);
                                        SetEntityHeading(Game.PlayerPed.Handle, tl.heading);
                                        SetGameplayCamRelativeHeading(0f);
                                    }
                                };
                            }

                        }
                    };
                }

                if (IsAllowed(Permission.TPTeleportToPrev))
                {
                    menu.AddMenuItem(overridePrevBtn);
                }
                if (IsAllowed(Permission.TPTeleportPersonalLocations))
                {
                    menu.AddMenuItem(savePersonalLocationBtn);
                }
                if (IsAllowed(Permission.TPTeleportSaveLocation))
                {
                    menu.AddMenuItem(saveLocationBtn);
                };
            }
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
