using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CitizenFX.Core;

using MenuAPI;

using vMenuClient.data;

using static vMenuShared.ConfigManager;
using static vMenuClient.CommonFunctions;
using static vMenuShared.PermissionsManager;
using CitizenFX.Core.NaturalMotion;

namespace vMenuClient.menus
{
    public class VehicleSpawner
    {
        // Variables
        private Menu menu;
        public static Dictionary<string, uint> AddonVehicles;
        public bool SpawnInVehicle { get; private set; } = UserDefaults.VehicleSpawnerSpawnInside;
        public bool ReplaceVehicle { get; private set; } = UserDefaults.VehicleSpawnerReplacePrevious;
        public bool SpawnNpcLike { get; private set; } = UserDefaults.VehicleSpawnerSpawnNpcLike;
        public bool loadcarnames { get; private set; }

        public static List<bool> allowedCategories;

        private static readonly LanguageManager Lm = new LanguageManager();

        private void CreateMenu()
        {
            // Create the menu.
            menu = new Menu(MenuTitle, "Vehicle Spawner");

            var spawnByName = new MenuItem("Spawn Vehicle By Model Name", "Enter the name of a vehicle to spawn.");
            var searchVehicles = new MenuItem("Search Vehicle By Name", "Search all vehicles by (model) name.");

            // Add the items to the menu.
            if (IsAllowed(Permission.VSSpawnByName))
            {
                menu.AddMenuItem(spawnByName);
            }
            menu.AddMenuItem(searchVehicles);

            var classPermissionDict =
                Enumerable.Range(0,23)
                    .ToDictionary(classId => classId, classId => (Permission)((int)Permission.VSCompacts + classId));

            int StringCompareNullLast(string s1, string s2)
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

            var compareVehicleClassDict =
                new int[]{0,3,4,9,22,1,6,5,7,2,12,8,15,16,14,20,18,10,19,17,11,13,21}
                    .Select((num, ix) => new KeyValuePair<int,int>(num,ix))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            int CompareVehicleClass(int i1, int i2)
            {
                return compareVehicleClassDict[i1].CompareTo(compareVehicleClassDict[i2]);
            }

            Tuple<string,string,string> GetDigitsNondigitsRest(string s)
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

            int StringCompareWithNumbers(string s1, string s2)
            {
                if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                    return s1.CompareTo(s2);

                var dndr1 = GetDigitsNondigitsRest(s1);
                var dndr2 = GetDigitsNondigitsRest(s2);

                var digits1 = dndr1.Item1;
                var digits2 = dndr2.Item1;

                int ret = 0;
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

            Tuple<string, string> GetYearModel(string name)
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

            int CompareVehicleNames(VehicleData.VehicleInfo vi1, VehicleData.VehicleInfo vi2)
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

            async Task<bool> SearchVehiclesMenu(Menu menu, string input = null)
            {
                if (input == null)
                    input = await GetUserInput("Search vehicles. Leave empty to reset");

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
                        return false;
                    }
                    else
                    {
                        Subtitle.Custom("Search completed.");
                        return true;
                    }
                }
                else
                {
                    menu.ResetFilter();
                    Subtitle.Custom("Search reset.");
                    return true;
                }
            }

            bool IsClassAllowed(int classId) => IsAllowed(classPermissionDict[classId]);

            bool IsVehicleAllowed(VehicleData.VehicleInfo vi)
            {
                if (!IsClassAllowed(vi.Class))
                    return false;

                if (vi.IsAddon && !IsAllowed(Permission.VSAddon))
                    return false;

                if (VehicleData.VehicleBlacklist.Contains(vi.Shortname) &&
                    !IsAllowed(Permission.VOVehiclesBlacklist))
                    return false;

                if (VehicleData.VehicleDisablelist.Contains(vi.Shortname) &&
                    !IsAllowed(Permission.VODisableFromDefaultList))
                    return false;

                return true;
            }

            List<VehicleData.VehicleInfo> SortAndFilterVehicles(IEnumerable<VehicleData.VehicleInfo> vehicles)
            {
                return vehicles
                    .Where(IsVehicleAllowed)
                    .OrderBy(
                        v => v,
                        Comparer<VehicleData.VehicleInfo>.Create(CompareVehicleNames))
                    .ToList();
            }

            MenuItem CreateSpawnVehicleButton(VehicleData.VehicleInfo vi)
            {
                var textColor = !vi.HasProperName ? "~y~" : vi.IsAddon ? "~b~" : "~s~";
                var text = $"{textColor}{vi.Name}~s~";

                var manufacturerDescr = vi.Manufacturer != "NULL" ? $"~b~{vi.Manufacturer}~s~ " : "";
                var description = $"Spawn the {manufacturerDescr}~b~{vi.Name}~s~.";

                var btn = new MenuItem(text, description)
                {
                    Label = $"~c~({vi.Shortname})~s~",
                    ItemData = vi.Shortname
                };

                return btn;
            }

            void SetupSpawnVehiclesMenu(Menu menu, List<VehicleData.VehicleInfo> vehicles)
            {
                MenuItem filterBtn = null;
                if (vehicles.Count > 10)
                {
                    filterBtn = new MenuItem("~g~Search Vehicles~s~", "Search vehicles.");
                    menu.AddMenuItem(filterBtn);
                }

                foreach(var vehicle in vehicles)
                {
                    var btn = CreateSpawnVehicleButton(vehicle);
                    menu.AddMenuItem(btn);
                }

                menu.OnItemSelect += async (_sender, item, _index) => {
                    if (item == filterBtn)
                    {
                        await SearchVehiclesMenu(menu);
                    }
                    await SpawnVehicle(item.ItemData.ToString(), SpawnInVehicle, ReplaceVehicle, SpawnNpcLike);
                };
                menu.OnMenuClose += m => m.ResetFilter();

                menu.InstructionalButtons.Add(Control.Jump, "Search Vehicles");
                menu.ButtonPressHandlers.Add(new Menu.ButtonPressHandler(
                    Control.Jump,
                    Menu.ControlPressCheckType.JUST_RELEASED,
                    async (m, _c) => await SearchVehiclesMenu(m),
                    true));
            }

            void SetupSpawnVehiclesMenus(Menu menu, Func<string, string> description, List<Tuple<string, List<VehicleData.VehicleInfo>>> vehicleGroups)
            {
                foreach (var group in vehicleGroups)
                {
                    var name = group.Item1;
                    var nonNullName = name != "NULL" ? name : "Unknown";

                    var btn = new MenuItem(name != "NULL" ? name : "~italic~Unknown~italic~", $"Spawn vehicles {description(name)}.")
                    {
                        Label = "→→→"
                    };
                    var childMenu = new Menu(MenuTitle, nonNullName);

                    var vehicleList = SortAndFilterVehicles(group.Item2);
                    if (vehicleList.Count > 0)
                    {
                        SetupSpawnVehiclesMenu(childMenu, vehicleList);

                        MenuController.AddSubmenu(menu, childMenu);
                        MenuController.BindMenuItem(menu, childMenu, btn);
                        menu.AddMenuItem(btn);
                    }
                }
            }

            Menu allVehiclesMenu;
            {
                var allVehiclesBtn = new MenuItem("All Vehicles", "All vehicles.")
                {
                    Label = "→→→"
                };
                allVehiclesMenu = new Menu(MenuTitle, "All Vehicles");

                var vehicleList = SortAndFilterVehicles(VehicleData.AllVehicles.Values);
                SetupSpawnVehiclesMenu(allVehiclesMenu, vehicleList);

                MenuController.AddSubmenu(menu, allVehiclesMenu);
                MenuController.BindMenuItem(menu, allVehiclesMenu, allVehiclesBtn);
                menu.AddMenuItem(allVehiclesBtn);
            }

            {
                var vehiclesByManufacturerBtn = new MenuItem("Manufacturers", "Vehicles grouped by manufacturer.")
                {
                    Label = "→→→"
                };
                var vehiclesByManufacturerMenu = new Menu(MenuTitle, "Manufacturers");

                var groups = VehicleData.VehiclesByManufacturer
                    .OrderBy(kv => kv.Key, Comparer<string>.Create(StringCompareNullLast))
                    .Select(kv => new Tuple<string, List<VehicleData.VehicleInfo>>(kv.Key, kv.Value))
                    .ToList();

                SetupSpawnVehiclesMenus(vehiclesByManufacturerMenu, s => s == "NULL" ? "without a known manufacturer" : $"from ~b~{s}~s~", groups);

                MenuController.AddSubmenu(menu, vehiclesByManufacturerMenu);
                MenuController.BindMenuItem(menu, vehiclesByManufacturerMenu, vehiclesByManufacturerBtn);
                menu.AddMenuItem(vehiclesByManufacturerBtn);
            }

            bool hasCustomClasses = false;
            bool onlyCustomClasses = GetSettingsBool(Setting.vmenu_only_custom_classes);

            if (VehicleData.CustomVehiclesClasses.Count > 0)
            {
                hasCustomClasses = true;

                var textPrefix = !onlyCustomClasses ? "Custom " : "";
                var descriptionInfinx = !onlyCustomClasses ? "custom " : "";

                var vehiclesByCustomClassBtn = new MenuItem($"{textPrefix}Classes", $"Vehicles grouped by {descriptionInfinx}class.")
                {
                    Label = "→→→"
                };
                var vehiclesByCustomClassMenu = new Menu(MenuTitle, $"{textPrefix}Classes");

                var groups = VehicleData.CustomVehiclesClasses
                    .Select(c => new Tuple<string, List<VehicleData.VehicleInfo>>(c.Name, c.Vehicles))
                    .ToList();

                SetupSpawnVehiclesMenus(vehiclesByCustomClassMenu, s => $"from the {descriptionInfinx}~b~{s}~s~ class", groups);

                MenuController.AddSubmenu(menu, vehiclesByCustomClassMenu);
                MenuController.BindMenuItem(menu, vehiclesByCustomClassMenu, vehiclesByCustomClassBtn);
                menu.AddMenuItem(vehiclesByCustomClassBtn);
            }

            if (!onlyCustomClasses || !hasCustomClasses)
            {
                var textPrefix = hasCustomClasses ? "Default " : "";
                var descriptionInfinx = hasCustomClasses ? "default " : "";

                var vehiclesByDefaultClassBtn = new MenuItem($"{textPrefix}Classes", $"Vehicles grouped by {descriptionInfinx}class.")
                {
                    Label = "→→→"
                };
                var vehiclesByDefaultClassMenu = new Menu(MenuTitle, $"{textPrefix}Classes");

                var groups = VehicleData.VehiclesByClass
                    .OrderBy(kv => kv.Key, Comparer<int>.Create(CompareVehicleClass))
                    .Select(kv => new Tuple<string, List<VehicleData.VehicleInfo>>(VehicleData.ClassIdToName[kv.Key], kv.Value))
                    .ToList();

                SetupSpawnVehiclesMenus(vehiclesByDefaultClassMenu, s => $"from the {descriptionInfinx}~b~{s}~s~ class", groups);

                MenuController.AddSubmenu(menu, vehiclesByDefaultClassMenu);
                MenuController.BindMenuItem(menu, vehiclesByDefaultClassMenu, vehiclesByDefaultClassBtn);
                menu.AddMenuItem(vehiclesByDefaultClassBtn);
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

                MenuController.AddSubmenu(menu, spawnOptionsMenu);
                MenuController.BindMenuItem(menu, spawnOptionsMenu, spawnOptionsBtn);
                menu.AddMenuItem(spawnOptionsBtn);

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

            // Handle button presses.
            menu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == spawnByName)
                {
                    // Passing "custom" as the vehicle name, will ask the user for input.
                    await SpawnVehicle("custom", SpawnInVehicle, ReplaceVehicle, SpawnNpcLike);
                }
                else if (item == searchVehicles)
                {
                    var input = await GetUserInput("Search vehicle");
                    if (string.IsNullOrEmpty(input))
                        return;

                    var success = await SearchVehiclesMenu(allVehiclesMenu, input);
                    if (success)
                    {
                        MenuController.CloseAllMenus();
                        allVehiclesMenu.OpenMenu();
                    }
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