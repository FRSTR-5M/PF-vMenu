using System;
using System.Collections.Generic;
using System.Linq;

using CitizenFX.Core;

using Newtonsoft.Json;

using static CitizenFX.Core.Native.API;

namespace vMenuClient.data
{
    public static class VehicleData
    {
        public readonly struct VehicleColor
        {
            public readonly int id;
            public readonly string label;

            public VehicleColor(int id, string label)
            {
                if (label == "veh_color_taxi_yellow")
                {
                    if (GetLabelText("veh_color_taxi_yellow") == "NULL")
                    {
                        AddTextEntry("veh_color_taxi_yellow", $"Taxi {GetLabelText("IEC_T20_2")}");
                    }
                }
                else if (label == "veh_color_off_white")
                {
                    if (GetLabelText("veh_color_off_white") == "NULL")
                    {
                        AddTextEntry("veh_color_off_white", "Off White");
                    }
                }
                else if (label == "VERY_DARK_BLUE")
                {
                    if (GetLabelText("VERY_DARK_BLUE") == "NULL")
                    {
                        AddTextEntry("VERY_DARK_BLUE", "Very Dark Blue");
                    }
                }

                this.label = label;
                this.id = id;
            }
        }

        public static readonly List<VehicleColor> ClassicColors = new()
        {
            new VehicleColor(0, "BLACK"),
            new VehicleColor(1, "GRAPHITE"),
            new VehicleColor(2, "BLACK_STEEL"),
            new VehicleColor(3, "DARK_SILVER"),
            new VehicleColor(4, "SILVER"),
            new VehicleColor(5, "BLUE_SILVER"),
            new VehicleColor(6, "ROLLED_STEEL"),
            new VehicleColor(7, "SHADOW_SILVER"),
            new VehicleColor(8, "STONE_SILVER"),
            new VehicleColor(9, "MIDNIGHT_SILVER"),
            new VehicleColor(10, "CAST_IRON_SIL"),
            new VehicleColor(11, "ANTHR_BLACK"),

            new VehicleColor(27, "RED"),
            new VehicleColor(28, "TORINO_RED"),
            new VehicleColor(29, "FORMULA_RED"),
            new VehicleColor(30, "BLAZE_RED"),
            new VehicleColor(31, "GRACE_RED"),
            new VehicleColor(32, "GARNET_RED"),
            new VehicleColor(33, "SUNSET_RED"),
            new VehicleColor(34, "CABERNET_RED"),
            new VehicleColor(35, "CANDY_RED"),
            new VehicleColor(36, "SUNRISE_ORANGE"),
            new VehicleColor(37, "GOLD"),
            new VehicleColor(38, "ORANGE"),

            new VehicleColor(49, "DARK_GREEN"),
            new VehicleColor(50, "RACING_GREEN"),
            new VehicleColor(51, "SEA_GREEN"),
            new VehicleColor(52, "OLIVE_GREEN"),
            new VehicleColor(53, "BRIGHT_GREEN"),
            new VehicleColor(54, "PETROL_GREEN"),

            new VehicleColor(61, "GALAXY_BLUE"),
            new VehicleColor(62, "DARK_BLUE"),
            new VehicleColor(63, "SAXON_BLUE"),
            new VehicleColor(64, "BLUE"),
            new VehicleColor(65, "MARINER_BLUE"),
            new VehicleColor(66, "HARBOR_BLUE"),
            new VehicleColor(67, "DIAMOND_BLUE"),
            new VehicleColor(68, "SURF_BLUE"),
            new VehicleColor(69, "NAUTICAL_BLUE"),
            new VehicleColor(70, "ULTRA_BLUE"),
            new VehicleColor(71, "PURPLE"),
            new VehicleColor(72, "SPIN_PURPLE"),
            new VehicleColor(73, "RACING_BLUE"),
            new VehicleColor(74, "LIGHT_BLUE"),

            new VehicleColor(88, "YELLOW"),
            new VehicleColor(89, "RACE_YELLOW"),
            new VehicleColor(90, "BRONZE"),
            new VehicleColor(91, "FLUR_YELLOW"),
            new VehicleColor(92, "LIME_GREEN"),

            new VehicleColor(94, "UMBER_BROWN"),
            new VehicleColor(95, "CREEK_BROWN"),
            new VehicleColor(96, "CHOCOLATE_BROWN"),
            new VehicleColor(97, "MAPLE_BROWN"),
            new VehicleColor(98, "SADDLE_BROWN"),
            new VehicleColor(99, "STRAW_BROWN"),
            new VehicleColor(100, "MOSS_BROWN"),
            new VehicleColor(101, "BISON_BROWN"),
            new VehicleColor(102, "WOODBEECH_BROWN"),
            new VehicleColor(103, "BEECHWOOD_BROWN"),
            new VehicleColor(104, "SIENNA_BROWN"),
            new VehicleColor(105, "SANDY_BROWN"),
            new VehicleColor(106, "BLEECHED_BROWN"),
            new VehicleColor(107, "CREAM"),

            new VehicleColor(111, "WHITE"),
            new VehicleColor(112, "FROST_WHITE"),

            new VehicleColor(135, "HOT PINK"),
            new VehicleColor(136, "SALMON_PINK"),
            new VehicleColor(137, "PINK"),
            new VehicleColor(138, "BRIGHT_ORANGE"),

            new VehicleColor(141, "MIDNIGHT_BLUE"),
            new VehicleColor(142, "MIGHT_PURPLE"),
            new VehicleColor(143, "WINE_RED"),

            new VehicleColor(145, "BRIGHT_PURPLE"),
            new VehicleColor(146, "VERY_DARK_BLUE"),
            new VehicleColor(147, "BLACK_GRAPHITE"),

            new VehicleColor(150, "LAVA_RED"),
        };

        public static readonly List<VehicleColor> MatteColors = new()
        {
            new VehicleColor(12, "BLACK"),
            new VehicleColor(13, "GREY"),
            new VehicleColor(14, "LIGHT_GREY"),

            new VehicleColor(39, "RED"),
            new VehicleColor(40, "DARK_RED"),
            new VehicleColor(41, "ORANGE"),
            new VehicleColor(42, "YELLOW"),

            new VehicleColor(55, "LIME_GREEN"),

            new VehicleColor(82, "DARK_BLUE"),
            new VehicleColor(83, "BLUE"),
            new VehicleColor(84, "MIDNIGHT_BLUE"),

            new VehicleColor(128, "GREEN"),

            new VehicleColor(148, "Purple"),
            new VehicleColor(149, "MIGHT_PURPLE"),

            new VehicleColor(151, "MATTE_FOR"),
            new VehicleColor(152, "MATTE_OD"),
            new VehicleColor(153, "MATTE_DIRT"),
            new VehicleColor(154, "MATTE_DESERT"),
            new VehicleColor(155, "MATTE_FOIL"),
        };

        public static readonly List<VehicleColor> MetalColors = new()
        {
            new VehicleColor(117, "BR_STEEL"),
            new VehicleColor(118, "BR BLACK_STEEL"),
            new VehicleColor(119, "BR_ALUMINIUM"),
            new VehicleColor(120, "CHROME"),
            new VehicleColor(158, "GOLD_P"),
            new VehicleColor(159, "GOLD_S"),
        };

        public static readonly List<VehicleColor> UtilColors = new()
        {
            new VehicleColor(15, "BLACK"),
            new VehicleColor(16, "FMMC_COL1_1"),
            new VehicleColor(17, "DARK_SILVER"),
            new VehicleColor(18, "SILVER"),
            new VehicleColor(19, "BLACK_STEEL"),
            new VehicleColor(20, "SHADOW_SILVER"),

            new VehicleColor(43, "DARK_RED"),
            new VehicleColor(44, "RED"),
            new VehicleColor(45, "GARNET_RED"),

            new VehicleColor(56, "DARK_GREEN"),
            new VehicleColor(57, "GREEN"),

            new VehicleColor(75, "DARK_BLUE"),
            new VehicleColor(76, "MIDNIGHT_BLUE"),
            new VehicleColor(77, "SAXON_BLUE"),
            new VehicleColor(78, "NAUTICAL_BLUE"),
            new VehicleColor(79, "BLUE"),
            new VehicleColor(80, "FMMC_COL1_13"),
            new VehicleColor(81, "BRIGHT_PURPLE"),

            new VehicleColor(93, "STRAW_BROWN"),

            new VehicleColor(108, "UMBER_BROWN"),
            new VehicleColor(109, "MOSS_BROWN"),
            new VehicleColor(110, "SANDY_BROWN"),

            new VehicleColor(122, "veh_color_off_white"),

            new VehicleColor(125, "BRIGHT_GREEN"),

            new VehicleColor(127, "HARBOR_BLUE"),

            new VehicleColor(134, "FROST_WHITE"),

            new VehicleColor(139, "LIME_GREEN"),
            new VehicleColor(140, "ULTRA_BLUE"),

            new VehicleColor(144, "GREY"),

            new VehicleColor(157, "LIGHT_BLUE"),

            new VehicleColor(160, "YELLOW")
        };

        public static readonly List<VehicleColor> WornColors = new()
        {
            new VehicleColor(21, "BLACK"),
            new VehicleColor(22, "GRAPHITE"),
            new VehicleColor(23, "LIGHT_GREY"),
            new VehicleColor(24, "SILVER"),
            new VehicleColor(25, "BLUE_SILVER"),
            new VehicleColor(26, "SHADOW_SILVER"),

            new VehicleColor(46, "RED"),
            new VehicleColor(47, "SALMON_PINK"),
            new VehicleColor(48, "DARK_RED"),

            new VehicleColor(58, "DARK_GREEN"),
            new VehicleColor(59, "GREEN"),
            new VehicleColor(60, "SEA_GREEN"),

            new VehicleColor(85, "DARK_BLUE"),
            new VehicleColor(86, "BLUE"),
            new VehicleColor(87, "LIGHT_BLUE"),

            new VehicleColor(113, "SANDY_BROWN"),
            new VehicleColor(114, "BISON_BROWN"),
            new VehicleColor(115, "CREEK_BROWN"),
            new VehicleColor(116, "BLEECHED_BROWN"),

            new VehicleColor(121, "veh_color_off_white"),

            new VehicleColor(123, "ORANGE"),
            new VehicleColor(124, "SUNRISE_ORANGE"),

            new VehicleColor(126, "veh_color_taxi_yellow"),

            new VehicleColor(129, "RACING_GREEN"),
            new VehicleColor(130, "ORANGE"),
            new VehicleColor(131, "WHITE"),
            new VehicleColor(132, "FROST_WHITE"),
            new VehicleColor(133, "OLIVE_GREEN"),
        };

        public static readonly List<VehicleColor> ChameleonColors = new()
        {
            new VehicleColor(161, "ANOD_RED"),
            new VehicleColor(162, "ANOD_WINE"),
            new VehicleColor(163, "ANOD_PURPLE"),
            new VehicleColor(164, "ANOD_BLUE"),
            new VehicleColor(165, "ANOD_GREEN"),
            new VehicleColor(166, "ANOD_LIME"),
            new VehicleColor(167, "ANOD_COPPER"),
            new VehicleColor(168, "ANOD_BRONZE"),
            new VehicleColor(169, "ANOD_CHAMPAGNE"),
            new VehicleColor(170, "ANOD_GOLD"),
            new VehicleColor(171, "GREEN_BLUE_FLIP"),
            new VehicleColor(172, "GREEN_RED_FLIP"),
            new VehicleColor(173, "GREEN_BROW_FLIP"),
            new VehicleColor(174, "GREEN_TURQ_FLIP"),
            new VehicleColor(175, "GREEN_PURP_FLIP"),
            new VehicleColor(176, "TEAL_PURP_FLIP"),
            new VehicleColor(177, "TURQ_RED_FLIP"),
            new VehicleColor(178, "TURQ_PURP_FLIP"),
            new VehicleColor(179, "CYAN_PURP_FLIP"),
            new VehicleColor(180, "BLUE_PINK_FLIP"),
            new VehicleColor(181, "BLUE_GREEN_FLIP"),
            new VehicleColor(182, "PURP_RED_FLIP"),
            new VehicleColor(183, "PURP_GREEN_FLIP"),
            new VehicleColor(184, "MAGEN_GREE_FLIP"),
            new VehicleColor(185, "MAGEN_YELL_FLIP"),
            new VehicleColor(186, "BURG_GREEN_FLIP"),
            new VehicleColor(187, "MAGEN_CYAN_FLIP"),
            new VehicleColor(188, "COPPE_PURP_FLIP"),
            new VehicleColor(189, "MAGEN_ORAN_FLIP"),
            new VehicleColor(190, "RED_ORANGE_FLIP"),
            new VehicleColor(191, "ORANG_PURP_FLIP"),
            new VehicleColor(192, "ORANG_BLUE_FLIP"),
            new VehicleColor(193, "WHITE_PURP_FLIP"),
            new VehicleColor(194, "RED_RAINBO_FLIP"),
            new VehicleColor(195, "BLU_RAINBO_FLIP"),
            new VehicleColor(196, "DARKGREENPEARL"),
            new VehicleColor(197, "DARKTEALPEARL"),
            new VehicleColor(198, "DARKBLUEPEARL"),
            new VehicleColor(199, "DARKPURPLEPEARL"),
            new VehicleColor(200, "OIL_SLICK_PEARL"),
            new VehicleColor(201, "LIT_GREEN_PEARL"),
            new VehicleColor(202, "LIT_BLUE_PEARL"),
            new VehicleColor(203, "LIT_PURP_PEARL"),
            new VehicleColor(204, "LIT_PINK_PEARL"),
            new VehicleColor(205, "OFFWHITE_PRISMA"),
            new VehicleColor(206, "PINK_PEARL"),
            new VehicleColor(207, "YELLOW_PEARL"),
            new VehicleColor(208, "GREEN_PEARL"),
            new VehicleColor(209, "BLUE_PEARL"),
            new VehicleColor(210, "CREAM_PEARL"),
            new VehicleColor(211, "WHITE_PRISMA"),
            new VehicleColor(212, "GRAPHITE_PRISMA"),
            new VehicleColor(213, "DARKBLUEPRISMA"),
            new VehicleColor(214, "DARKPURPPRISMA"),
            new VehicleColor(215, "HOT_PINK_PRISMA"),
            new VehicleColor(216, "RED_PRISMA"),
            new VehicleColor(217, "GREEN_PRISMA"),
            new VehicleColor(218, "BLACK_PRISMA"),
            new VehicleColor(219, "OIL_SLIC_PRISMA"),
            new VehicleColor(220, "RAINBOW_PRISMA"),
            new VehicleColor(221, "BLACK_HOLO"),
            new VehicleColor(222, "WHITE_HOLO"),
            new VehicleColor(223, "YKTA_MONOCHROME"),
            new VehicleColor(224, "YKTA_NITE_DAY"),
            new VehicleColor(225, "YKTA_VERLIERER2"),
            new VehicleColor(226, "YKTA_SPRUNK_EX"),
            new VehicleColor(227, "YKTA_VICE_CITY"),
            new VehicleColor(228, "YKTA_SYNTHWAVE"),
            new VehicleColor(229, "YKTA_FOUR_SEASO"),
            new VehicleColor(230, "YKTA_M9_THROWBA"),
            new VehicleColor(231, "YKTA_BUBBLEGUM"),
            new VehicleColor(232, "YKTA_FULL_RBOW"),
            new VehicleColor(233, "YKTA_SUNSETS"),
            new VehicleColor(234, "YKTA_THE_SEVEN"),
            new VehicleColor(235, "YKTA_KAMENRIDER"),
            new VehicleColor(236, "YKTA_CHROMABERA"),
            new VehicleColor(237, "YKTA_CHRISTMAS"),
            new VehicleColor(238, "YKTA_TEMPERATUR"),
            new VehicleColor(239, "YKTA_HSW"),
            new VehicleColor(240, "YKTA_ELECTRO"),
            new VehicleColor(241, "YKTA_MONIKA"),
            new VehicleColor(242, "YKTA_FUBUKI"),

        };

        public static Dictionary<int, string> ClassIdToName { get; } =
            Enumerable
                .Range(0, 23)
                .ToDictionary(c => c, c => GetLabelText($"VEH_CLASS_{c}"));

        public static Dictionary<string, int> ClassNameToId { get; } =
            ClassIdToName.ToDictionary(kv => kv.Value, kv => kv.Key);

        private static HashSet<string> addonVehicles = null;

        private static HashSet<string> vehicleDisablelist = null;
        private static HashSet<string> vehicleBlacklist = null;

        private static void LoadAddonData()
        {
            addonVehicles = new HashSet<string>();
            vehicleDisablelist= new HashSet<string>();
            vehicleBlacklist = new HashSet<string>();

            void FillAddonSet(Dictionary<string, List<string>> addonData, string listName, HashSet<string> set)
            {
                List<string> list;
                if (!addonData.TryGetValue(listName, out list))
                    return;

                foreach(var addon in list)
                {
                    if (!IsModelInCdimage((uint)GetHashKey(addon)))
                    {
                        Debug.WriteLine($"^3[vMenu] [WARNING]^7 Vehicle \"{addon}\" specified in \"addons.json > {listName}\" not available");
                    }
                    else
                    {
                        set.Add(addon.ToLower());
                    }
                }
            }

            string jsonData = LoadResourceFile(GetCurrentResourceName(), "config/addons.json") ?? "{}";
            try
            {
                var addons = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData);

                FillAddonSet(addons, "vehicles", addonVehicles);
                FillAddonSet(addons, "disablefromdefaultlist", vehicleDisablelist);
                FillAddonSet(addons, "vehicleblacklist", vehicleBlacklist);
            }
            catch
            {
                Debug.WriteLine($"^1[vMenu] [ERROR]^7 Could not parse \"addons.json\"");
            }
        }

        public static HashSet<string> AddonVehicles
        { 
            get
            {
                if (addonVehicles != null)
                    return addonVehicles;

                LoadAddonData();
                return addonVehicles;
            }
        }

        public static HashSet<string> VehicleBlacklist
        {
            get
            {
                if (vehicleBlacklist != null)
                    return vehicleBlacklist;

                LoadAddonData();
                return vehicleBlacklist;
            }
        }
        public static HashSet<string> VehicleDisablelist
        {
            get
            {
                if (vehicleDisablelist != null)
                    return vehicleDisablelist;

                LoadAddonData();
                return vehicleDisablelist;
            }
        }

        public class VehicleInfo
        {
            public static string GetName(uint hash, string shortname, out bool hasProperName)
            {
                string displayname = GetDisplayNameFromVehicleModel(hash);
                if (string.IsNullOrWhiteSpace(displayname) || displayname == "CARNOTFOUND")
                {
                    hasProperName = false;
                    return shortname.ToUpper();
                }

                string labelname = GetLabelText(displayname);
                if (labelname == "NULL")
                {
                    hasProperName = false;
                    return displayname;
                }

                hasProperName = true;
                return labelname;
            }

            public static string GetManufacturer(uint hash)
            {
                string makename = GetMakeNameFromVehicleModel(hash);
                if (string.IsNullOrWhiteSpace(makename) || makename == "CARNOTFOUND")
                    return "NULL";

                string labelname = GetLabelText(makename);
                if (labelname == "NULL")
                    return makename;

                return labelname;
            }

            public VehicleInfo(string shortname)
            { 
                Shortname = shortname;
                Hash = (uint)GetHashKey(shortname);

                bool hasProperName;
                Name = GetName(Hash, shortname, out hasProperName);
                HasProperName = hasProperName;

                Manufacturer = GetManufacturer(Hash);
                Class = GetVehicleClassFromName(Hash);
            }

            public uint Hash { get; }
            public string Shortname { get; }
            public string Name { get; } = null;
            public bool HasProperName { get; } = false;
            public string Manufacturer { get; } = null;
            public int Class { get; }
            public bool IsAddon
            {
                get => AddonVehicles.Contains(Shortname);
            }
        }

        private static Dictionary<string, VehicleInfo> allVehicles = null;

        public static Dictionary<string, VehicleInfo> AllVehicles
        {
            get
            {
                if (allVehicles != null)
                    return allVehicles;

                allVehicles = ((List<object>)GetAllVehicleModels())
                    .Select(shortname => new VehicleInfo((string)shortname))
                    .ToDictionary(vi => vi.Shortname, vi => vi);
                return allVehicles;
            }
        }

        public class CustomVehicleClass
        {
            public CustomVehicleClass(string name, List<VehicleInfo> vehicles)
            {
                Name = name;
                Vehicles = vehicles;
            }

            public CustomVehicleClass(string name, List<string> shortnames)
            {
                Name = name;
                Vehicles = new List<VehicleInfo>();
                foreach(var shortname in shortnames)
                {
                    VehicleInfo vi;
                    if(AllVehicles.TryGetValue(shortname, out vi))
                    {
                        Vehicles.Add(vi);
                    }
                    else
                    {
                        Debug.WriteLine($"^1[vMenu] [ERROR]^7 Vehicle class \"{name}\" contains invalid model \"{shortname}\"");
                    }
                }
            }

            public string Name { get; }
            public List<VehicleInfo> Vehicles { get; }
        }

        public class CustomVehicleClassJson
        {
            public string Name { get; set; }
            public List<string> Vehicles { get; set; }

            public string Builtin { get; set; }
        }

        private static readonly Dictionary<string, int> permissiveClassNameToId =
            ClassNameToId
                .Select(kv => {
                    var className = kv.Key.ToLower();
                    var classId = kv.Value;

                    return new Dictionary<string, int>()
                    {
                        [className] = classId,
                        [className.Replace("-", "")] = classId,
                        [className.Replace("-", " ")] = classId,
                        [className.Replace(" ", "")] = classId,
                        [className.Replace(" ", "-")] = classId,
                    };
                })
                .SelectMany(x => x)
                .Select(kv => {
                    var className = kv.Key;

                    string altClassName;
                    if (className.EndsWith("s"))
                    {
                        altClassName = className.Substring(0, className.Length - 1);
                    }
                    else
                    {
                        altClassName = className + "s";
                    }

                    return new Dictionary<string, int>()
                    {
                        [className] = kv.Value,
                        [altClassName] = kv.Value
                    };
                })
                .SelectMany(x => x)
                .ToDictionary(x => x.Key, x => x.Value);

        private static Dictionary<int, List<VehicleInfo>> vehiclesByClass = AllVehicles.Values
            .ToLookup(vi => vi.Class)
            .ToDictionary(g => g.Key, g => g.ToList());

        private static CustomVehicleClass CustomVehicleClassFromJson(CustomVehicleClassJson customVehicleClassJson)
        {
            if (!string.IsNullOrEmpty(customVehicleClassJson.Builtin))
            {
                string builtinClassName = customVehicleClassJson.Builtin;
                int builtinClassId;
                if (permissiveClassNameToId.TryGetValue(builtinClassName.ToLower(), out builtinClassId))
                {
                    var name = ClassIdToName[builtinClassId];
                    var vehicles = vehiclesByClass[builtinClassId];

                    return new CustomVehicleClass(name, vehicles);
                }
                else
                {
                    Debug.WriteLine($"^1[vMenu] [ERROR]^7 Invalid builtin class \"{builtinClassName.ToLower()}\"");
                    return null;
                }
            }
            else
            {
                var name = customVehicleClassJson.Name ?? "";
                if (string.IsNullOrEmpty(name))
                {
                    Debug.WriteLine($"^3[vMenu] [WARNING]^7 Empty custom class name");
                }

                var vehicles = customVehicleClassJson.Vehicles ?? new List<string>();
                if (vehicles.Count == 0)
                {
                    Debug.WriteLine($"^3[vMenu] [WARNING]^7 Empty custom class \"{name}\"");
                }

                return new CustomVehicleClass(name, vehicles);
            }
        }

        private static List<CustomVehicleClass> customVehicleClasses = null;
        public static List<CustomVehicleClass> CustomVehiclesClasses
        {
            get
            {
                if (customVehicleClasses != null)
                    return customVehicleClasses;

                customVehicleClasses = new List<CustomVehicleClass>();

                var json = LoadResourceFile(GetCurrentResourceName(), "config/vehicle-classes.json");
                if (string.IsNullOrEmpty(json))
                {
                    return customVehicleClasses;
                }

                List<CustomVehicleClassJson> customVehicleClassesJson;
                try
                {
                    customVehicleClassesJson = JsonConvert.DeserializeObject<List<CustomVehicleClassJson>>(json);
                }
                catch
                {
                    Debug.WriteLine($"^1[vMenu] [ERROR]^7 Could not parse \"vehicle-classes.json\"");
                    return customVehicleClasses;
                }

                foreach (var customVehicleClassJson in customVehicleClassesJson)
                {
                    var customClass = CustomVehicleClassFromJson(customVehicleClassJson);
                    if (customClass != null)
                        customVehicleClasses.Add(customClass);
                }

                return customVehicleClasses;
            }
        }
    }
}
