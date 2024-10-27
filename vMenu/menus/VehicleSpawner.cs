using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

using MenuAPI;

using vMenuClient.data;

using static vMenuShared.ConfigManager;
using static vMenuClient.CommonFunctions;
using static vMenuShared.PermissionsManager;
using vMenuClient.MenuAPIWrapper;

namespace vMenuClient.menus
{
    public class VehicleSpawner
    {
        // Variables
        private WMenu menu;


        private WMenu allVehiclesMenu;


        public bool SpawnInVehicle { get; private set; } = UserDefaults.VehicleSpawnerSpawnInside;
        public bool ReplaceVehicle { get; private set; } = UserDefaults.VehicleSpawnerReplacePrevious;
        public bool SpawnNpcLike { get; private set; } = UserDefaults.VehicleSpawnerSpawnNpcLike;


        private List<VehicleData.VehicleInfo> allowedVehicles;
        private HashSet<string> displayVehicles;


        public static List<bool> allowedCategories;

        public static bool IsVehicleAllowedClass(VehicleData.VehicleInfo vi) => allowedCategories[vi.Class];
        public static bool IsVehicleAllowedAddon(VehicleData.VehicleInfo vi) =>
            !vi.IsAddon || IsAllowed(Permission.VSAddon);
        public static bool IsVehicleAllowedBlacklist(VehicleData.VehicleInfo vi) =>
            !VehicleData.VehicleBlacklist.Contains(vi.Shortname) || IsAllowed(Permission.VOVehiclesBlacklist);
        public static bool IsVehicleAllowedDisablelist(VehicleData.VehicleInfo vi) =>
            !VehicleData.VehicleDisablelist.Contains(vi.Shortname) || IsAllowed(Permission.VODisableFromDefaultList);

        public static bool IsVehicleAllowed(VehicleData.VehicleInfo vi) =>
            IsVehicleAllowedClass(vi) &&
            IsVehicleAllowedAddon(vi) &&
            IsVehicleAllowedBlacklist(vi) &&
            IsVehicleAllowedDisablelist(vi);

        public static bool ShowVehicle(VehicleData.VehicleInfo vi) =>
            IsVehicleAllowed(vi) && !VehicleData.VehicleDisablelist.Contains(vi.Shortname);


        public static Tuple<string,string,string> GetDigitsNondigitsRest(string s)
        {
            int i = 0;
            for (; i < s.Length && char.IsDigit(s[i]); ++i) ;

            string digits = s.Substring(0, i);

            int k = i;
            for(; i < s.Length && !char.IsDigit(s[i]); ++i) ;

            string nondigits = s.Substring(k, i - k);
            string rest = s.Substring(i);

            return new Tuple<string, string, string>(digits, nondigits, rest);
        }

        public static int StringCompareWithNumbers(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return s1.CompareTo(s2);

            var dndr1 = GetDigitsNondigitsRest(s1);
            var dndr2 = GetDigitsNondigitsRest(s2);

            var digits1 = dndr1.Item1;
            var digits2 = dndr2.Item1;

            int ret;
            if (!string.IsNullOrEmpty(digits1) && !string.IsNullOrEmpty(digits2))
            {
                int num1 = int.Parse(digits1);
                int num2 = int.Parse(digits2);

                ret = num1.CompareTo(num2);
                if (ret != 0)
                    return ret;

                // If both digit strings parse to the same number, order the one with (more) leading zeros before
                ret = digits2.Length.CompareTo(digits1.Length);
                if (ret != 0)
                    return ret;
            }
            else if (!string.IsNullOrEmpty(digits1) && string.IsNullOrEmpty(digits2))
            {
                return -1;
            }
            else if (string.IsNullOrEmpty(digits1) && !string.IsNullOrEmpty(digits2))
            {
                return 1;
            }

            var nondigits1 = dndr1.Item2;
            var nondigits2 = dndr2.Item2;

            ret = nondigits1.CompareTo(nondigits2);
            if (ret != 0)
                return ret;

            var rest1 = dndr1.Item3;
            var rest2 = dndr2.Item3;

            return StringCompareWithNumbers(rest1, rest2);
        }

        public static Tuple<string, string> GetYearModel(string name)
        {
            if (name.Length < 6)
                return new Tuple<string, string>("", name);

            bool hasYear = true;
            for (int i = 0; i < 4; i++)
            {
                if (!char.IsDigit(name[i]))
                {
                    hasYear = false;
                    break;
                }
            }
            if (name[4] != ' ')
                hasYear = false;

            if (!hasYear)
                return new Tuple<string, string>("", name);

            return new Tuple<string, string>(name.Substring(0,4), name.Substring(4));
        }

        public static int CompareVehicleNames(VehicleData.VehicleInfo vi1, VehicleData.VehicleInfo vi2)
        {
            var name1 = vi1.Name;
            var name2 = vi2.Name;

            var yearModel1 = GetYearModel(name1);
            var yearModel2 = GetYearModel(name2);

            var year1Str = yearModel1.Item1;
            year1Str = string.IsNullOrEmpty(year1Str) ? "0000" : year1Str;
            var year2Str = yearModel2.Item1;
            year2Str = string.IsNullOrEmpty(year2Str) ? "0000" : year2Str;

            var year1 = int.Parse(year1Str);
            var year2 = int.Parse(year2Str);

            var model1 = yearModel1.Item2;
            var model2 = yearModel2.Item2;

            int ret = StringCompareWithNumbers(model1, model2);
            if (ret != 0)
                return ret;

            ret = year1.CompareTo(year2);
            if (ret != 0)
                return ret;

            return StringCompareWithNumbers(vi1.Shortname, vi2.Shortname);
        }


        public static int StringCompareNullLast(string s1, string s2)
        {
            if (s1 == "NULL" && s2 != "NULL")
            {
                return 1;
            }
            else if (s1 != "NULL" && s2 == "NULL")
            {
                return -1;
            }
            else
            {
                return string.Compare(s1, s2);
            }
        }


        private static Dictionary<int,int> compareVehicleClassDict =
            new int[]{0,3,4,9,22,1,6,5,7,2,12,8,15,16,14,20,18,10,19,17,11,13,21}
                .Select((num, ix) => new KeyValuePair<int,int>(num,ix))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        private static int CompareVehicleClass(int i1, int i2)
        {
            return compareVehicleClassDict[i1].CompareTo(compareVehicleClassDict[i2]);
        }


        private WMenuItem CreateSpawnVehicleButton(VehicleData.VehicleInfo vi)
        {
            var textColor = !vi.HasProperName ? "~y~" : vi.IsAddon ? "~b~" : "~s~";
            var text = $"{textColor}{vi.Name}~s~";

            var manufacturerDescr = vi.Manufacturer != "NULL" ? $"~b~{vi.Manufacturer}~s~ " : "";
            var description = $"Spawn the {manufacturerDescr}~b~{vi.Name}~s~.";

            var btn = new MenuItem(text, description)
            {
                Label = $"~c~({vi.Shortname})~s~",
            }.ToWrapped();
            btn.Selected += async (_s, _args) => await SpawnVehicle(vi.Shortname, SpawnInVehicle, ReplaceVehicle, SpawnNpcLike);

            return btn;
        }

        private WMenu CreateVehicleMenu(string subtitle, List<VehicleData.VehicleInfo> vehicles, bool showDisabled = false)
        {
            var vehiclesMenu = new WMenu(MenuTitle, subtitle);

            if (!showDisabled)
            {
                vehicles = vehicles.Where(vi => displayVehicles.Contains(vi.Shortname)).ToList();
            }

            if (vehicles.Count > 10)
            {
                int noMenuItems = vehicles.Count + 1;

                var filterBtn = new MenuItem("~g~Search Vehicles~s~", "Search vehicles or reset a search.").ToWrapped();
                filterBtn.Selected += async (_s, _args) => await SearchVehiclesMenu(vehiclesMenu.Menu);

                vehiclesMenu.AddItem(filterBtn);

                int increment = 1;
                void SetIncrement(int newIncrement)
                {
                    increment = newIncrement;
                    vehiclesMenu.Menu.InstructionalButtons.Remove(Control.NextCamera);
                    vehiclesMenu.Menu.InstructionalButtons.Add(Control.NextCamera, $"Increment: {increment}");
                }

                SetIncrement(1);

                vehiclesMenu.Menu.ButtonPressHandlers.Add(new Menu.ButtonPressHandler(
                    Control.NextCamera,
                    Menu.ControlPressCheckType.JUST_RELEASED,
                    (m, _c) => SetIncrement(increment == 1 ? 10: 1),
                    true));

                vehiclesMenu.Menu.InstructionalButtons.Add(Control.SelectWeapon, "Search Vehicles");
                vehiclesMenu.Menu.ButtonPressHandlers.Add(new Menu.ButtonPressHandler(
                    Control.SelectWeapon,
                    Menu.ControlPressCheckType.JUST_RELEASED,
                    async (m, _c) => await SearchVehiclesMenu(m),
                    true));

                vehiclesMenu.Closed += (_s, _args) => SetIncrement(1);

                bool incrementing = false;
                vehiclesMenu.IndexChanged += (_s, args) =>
                {
                    if (increment == 1 || incrementing)
                        return;

                    if (Math.Abs(args.IndexNew - args.IndexOld) > 1)
                    {
                        SetIncrement(1);
                        return;
                    }

                    if ((args.IndexOld < args.IndexNew && args.IndexOld + increment >= noMenuItems) ||
                        (args.IndexNew < args.IndexOld) && args.IndexOld - increment < 0)
                    {
                        SetIncrement(1);
                        return;
                    }

                    incrementing = true;
                    int indexNew = args.IndexNew;
                    for (int i = 0; i < increment - 1; i++)
                    {
                        if (args.IndexOld < args.IndexNew)
                        {
                            vehiclesMenu.Menu.GoDown();
                            indexNew++;
                        }
                        else
                        {
                            vehiclesMenu.Menu.GoUp();
                            indexNew--;
                        }
                    }
                    incrementing = false;

                    if (indexNew + increment >= noMenuItems || indexNew - increment < 0)
                    {
                        SetIncrement(1);
                    }
                };
            }

            foreach(var vehicle in vehicles)
            {
                var btn = CreateSpawnVehicleButton(vehicle);
                vehiclesMenu.AddItem(btn);
            }

            return vehiclesMenu;
        }

        private WMenu CreateVehicleGroupsMenu(string subtitle, Func<string, string> description, List<Tuple<string, List<VehicleData.VehicleInfo>>> vehicleGroups)
        {
            WMenu vehicleGroupsMenu = new WMenu(MenuTitle, subtitle);

            foreach (var group in vehicleGroups)
            {
                var name = group.Item1;
                var nameOrUnknown = name != "NULL" ? name : "~italic~Unknown~italic~";
                var vehiclesMenu = CreateVehicleMenu(nameOrUnknown, group.Item2);
                vehicleGroupsMenu.AddSubmenu(vehiclesMenu, $"Spawn vehicles {description(name)}");
            }

            return vehicleGroupsMenu;
        }


        private Menu currentSearchedMenu = null;

        private async Task<bool> SearchVehiclesMenu(Menu menu, string input = null)
        {
            if (input == null)
                input = await GetUserInput("Search vehicles. Leave empty to reset");

            if (input == null)
                return false;


            if (!string.IsNullOrEmpty(input))
            {
                menu.FilterMenuItems(mi =>
                    string.IsNullOrEmpty(mi.Label) ||
                    mi.Label.ToLower().Contains(input.ToLower()) ||
                    mi.Text.ToLower().Contains(input.ToLower()));

                if (menu.Size == 0 || (string.IsNullOrEmpty(menu.GetMenuItems()[0].Label) && menu.Size == 1))
                {
                    Subtitle.Custom("There are no vehicles matching this search.");
                    menu.ResetFilter();
                    if (menu == currentSearchedMenu)
                    {
                        currentSearchedMenu = null;
                    }
                    return false;
                }
                else
                {
                    if (menu != currentSearchedMenu)
                    {
                        currentSearchedMenu?.ResetFilter();
                    }
                    currentSearchedMenu = menu;
                    Subtitle.Custom("Search completed.");
                    return true;
                }
            }
            else
            {
                if (menu == currentSearchedMenu)
                {
                    currentSearchedMenu?.ResetFilter();
                    currentSearchedMenu = null;
                    Subtitle.Custom("Search reset.");
                }
                return false;
            }
        }


        private void CreateMenu()
        {
            allowedVehicles = VehicleData.AllVehicles.Values
                .Where(IsVehicleAllowed)
                .OrderBy(vi => vi, Comparer<VehicleData.VehicleInfo>.Create(CompareVehicleNames))
                .ToList();
            displayVehicles = new HashSet<string>(allowedVehicles.Where(ShowVehicle).Select(vi => vi.Shortname));

            // Create the menu.
            menu = new WMenu(MenuTitle, "Spawn Vehicles");


            {
                var searchVehicles = new MenuItem("Search Vehicle By Name", "Search all vehicles by (model) name.").ToWrapped();
                searchVehicles.Selected += async (_s, _args) =>
                {
                    var input = await GetUserInput("Search vehicle");
                    if (string.IsNullOrEmpty(input))
                        return;

                    var success = await SearchVehiclesMenu(allVehiclesMenu.Menu, input);
                    if (success)
                    {
                        MenuController.CloseAllMenus();
                        allVehiclesMenu.Menu.OpenMenu();
                    }
                };

                menu.AddItem(searchVehicles);
            }

            {
                var random = new Random();
                var randomVehiclesList = displayVehicles.Where(veh => 
                {
                    var hash = (uint)GetHashKey(veh);
                    return
                        IsThisModelABicycle(hash) ||
                        IsThisModelABike(hash) ||
                        IsThisModelACar(hash) ||
                        IsThisModelAnAmphibiousCar(hash) ||
                        IsThisModelAnAmphibiousQuadbike((int)hash) ||
                        IsThisModelAQuadbike(hash);
                }).ToList();

                var spawnRandom = new MenuItem("Spawn Random Vehicle", "Spawn a random land-based vehicle.").ToWrapped();
                spawnRandom.Selected += async (_s, _args) =>
                {
                    if (randomVehiclesList.Count == 0)
                    {
                        Notify.Error("You are not able to spawn any random vehicles, sorry");
                        return;
                    }
                    var veh = randomVehiclesList[random.Next(0, randomVehiclesList.Count)];
                    await SpawnVehicle(veh, SpawnInVehicle, ReplaceVehicle, SpawnNpcLike);
                };

                menu.AddItem(spawnRandom);
            }

            {
                allVehiclesMenu = CreateVehicleMenu("All Vehicles", allowedVehicles);
                menu.AddSubmenu(allVehiclesMenu);
            }

            {
                var vehiclesByManufacturer = allowedVehicles
                    .ToLookup(vehicle => vehicle.Manufacturer)
                    .OrderBy(g => g.Key, Comparer<string>.Create(StringCompareNullLast))
                    .Select(g => new Tuple<string, List<VehicleData.VehicleInfo>>(g.Key, g.ToList()))
                    .ToList();

                var vehiclesByManufacturerMenu = CreateVehicleGroupsMenu(
                    "Manufacturers",
                    s => s == "NULL" ? "without a known manufacturer" : $"from ~b~{s}~s~",
                    vehiclesByManufacturer);

                menu.AddSubmenu(vehiclesByManufacturerMenu);
            }

            bool hasCustomClasses = false;
            bool onlyCustomClasses = GetSettingsBool(Setting.vmenu_only_custom_classes);

            if (VehicleData.CustomVehiclesClasses.Count > 0)
            {
                hasCustomClasses = true;

                var textPrefix = !onlyCustomClasses ? "Custom " : "";
                var descriptionInfinx = !onlyCustomClasses ? "custom " : "";

                var vehiclesByCustomClass = VehicleData.CustomVehiclesClasses
                    .Select(c => new Tuple<string, List<VehicleData.VehicleInfo>>(c.Name, c.Vehicles))
                    .ToList();

                var vehiclesByCustomClassMenu = CreateVehicleGroupsMenu(
                    $"{textPrefix}Classes",
                    s => $"from the {descriptionInfinx}~b~{s}~s~ class",
                    vehiclesByCustomClass);

                menu.AddSubmenu(vehiclesByCustomClassMenu, $"Vehicles grouped by {descriptionInfinx}class.");
            }

            if (!onlyCustomClasses || !hasCustomClasses)
            {
                var textPrefix = hasCustomClasses ? "Default " : "";
                var descriptionInfinx = hasCustomClasses ? "default " : "";

                var vehiclesByDefaultClass = allowedVehicles
                    .ToLookup(vehicle => vehicle.Class)
                    .OrderBy(g => g.Key, Comparer<int>.Create(CompareVehicleClass))
                    .Select(g => new Tuple<string, List<VehicleData.VehicleInfo>>(VehicleData.ClassIdToName[g.Key], g.ToList()))
                    .ToList();

                var vehiclesByDefaultClassMenu = CreateVehicleGroupsMenu(
                    $"{textPrefix}Classes",
                    s => $"from the {descriptionInfinx}~b~{s}~s~ class",
                    vehiclesByDefaultClass);

                menu.AddSubmenu(vehiclesByDefaultClassMenu, $"Vehicles grouped by {descriptionInfinx}class.");
            }

            {
                var spawnOptionsMenu = new Menu(MenuTitle, "Spawn Options");
                var spawnOptionsBtn = new MenuItem("Spawn Options", "Change vehicle spawn options.");

                var spawnInVeh = new MenuCheckboxItem("Spawn Inside Vehicle", "This will teleport you into the vehicle when you spawn it.", SpawnInVehicle);
                var replacePrev = new MenuCheckboxItem("Replace Previous Vehicle", "This will automatically delete your previously spawned vehicle when you spawn a new vehicle.", ReplaceVehicle);
                var spawnNpcLike = new MenuCheckboxItem("Spawn NPC-Like Vehicle", "This will make the spawned vehicle behave more like an NPC vehicle. It will explode on heavy impact and despawn when too far away.", SpawnNpcLike);      

                spawnOptionsMenu.AddMenuItem(spawnInVeh);
                if (IsAllowed(Permission.VSDisableReplacePrevious))
                {
                    spawnOptionsMenu.AddMenuItem(replacePrev);
                }
                else
                {
                    replacePrev = null;
                    ReplaceVehicle = true;
                }
                spawnOptionsMenu.AddMenuItem(spawnNpcLike);

                menu.AddSubmenu(spawnOptionsMenu);

                spawnOptionsMenu.OnCheckboxChange += (sender, item, index, _checked) =>
                {
                    if (item == spawnInVeh)
                    {
                        SpawnInVehicle = _checked;
                    }
                    else if (item == replacePrev)
                    {
                        ReplaceVehicle = _checked;
                    }
                    else if (item == spawnNpcLike)
                    {
                        SpawnNpcLike = _checked;
                    }
                };
            }


            var restrictedSect = new List<WMenuItem>();

            if (VehicleData.VehicleDisablelist.Count > 0 && IsAllowed(Permission.VODisableFromDefaultList))
            {
                var allowedDisabledVehicles = allowedVehicles
                    .Where(vi => VehicleData.VehicleDisablelist.Contains(vi.Shortname))
                    .ToList();

                if (allowedDisabledVehicles.Count > 0)
                {
                    var disabledVehiclesMenu = CreateVehicleMenu("Hidden Vehicles", allowedDisabledVehicles, true);

                    WMenuItem button = new MenuItem(
                        "~y~Hidden Vehicles~s~",
                        "Vehicles in ~b~addons.json > disablefromdefaultlist~s~. ~y~These vehicles will not show in other vehicle lists and can only be spawned by players with the ~o~VODisableFromDefaultList~y~ permission.~s~")
                        .ToWrapped();
                    menu.BindSubmenu(disabledVehiclesMenu, button);

                    restrictedSect.Add(button);
                }
            }

            if (VehicleData.VehicleBlacklist.Count > 0 && IsAllowed(Permission.VOVehiclesBlacklist))
            {
                var allowedBlacklistedVehicles = allowedVehicles
                    .Where(vi => VehicleData.VehicleBlacklist.Contains(vi.Shortname))
                    .ToList();

                if (allowedBlacklistedVehicles.Count > 0)
                {
                    var disabledVehiclesMenu = CreateVehicleMenu("Blacklisted Vehicles", allowedBlacklistedVehicles, true);

                    WMenuItem button = new MenuItem(
                        "~y~Blacklisted Vehicles~s~",
                        "Vehicles in ~b~addons.json > vehicleblacklist~s~. ~y~These vehicles ~italic~will~italic~ show in other vehicle lists, but can only be spawned by players with the ~o~VOVehiclesBlacklist~y~ permission.~s~")
                        .ToWrapped();
                    menu.BindSubmenu(disabledVehiclesMenu, button);

                    restrictedSect.Add(button);
                }
            }

            menu.AddSection("Restricted Vehicles", restrictedSect);
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
    }
}