using System.Collections.Generic;

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

        public static class Vehicles
        {
            #region Vehicle List Per Class

            #region Compacts
            public static List<string> Compacts { get; } = new List<string>()
            {
                "ISSI4",
                "ASBO", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "BLISTA",
                "KANJO", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "BRIOSO2", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "BRIOSO3", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "BRIOSO",
                "CLUB", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "DILETTANTE",
                "DILETTANTE2",
                "ISSI5",
                "ISSI2",
                "ISSI3",
                "ISSI6",
                "PANTO",
                "PRAIRIE",
                "RHAPSODY",
                "WEEVIL", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
            };
            #endregion
            #region Sedans
            public static List<string> Sedans { get; } = new List<string>()
            {
                "ASEA",
                "ASEA2",
                "ASTEROPE",
				"ASTEROPE2", // CHOP SHOP DLC - Requires b3095
                "CINQUEMILA", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "COGNOSCENTI",
                "COGNOSCENTI2",
                "COG55",
                "COG552",
                "DEITY", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "EMPEROR",
                "EMPEROR2",
                "EMPEROR3",
                "FUGITIVE",
                "GLENDALE",
                "GLENDALE2", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "INGOT",
                "INTRUDER",
                "PREMIER",
                "PRIMO",
                "PRIMO2",
                "REGINA",
                "RHINEHART", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "ROMERO",
                "SCHAFTER2",
                "SCHAFTER6",
                "SCHAFTER5",
                "STAFFORD",
                "STANIER",
                "STRATUM",
                "STRETCH",
                "SUPERD",
                "SURGE",
                "TAILGATER",
                "TAILGATER2", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "LIMO2",
				"VORSCHLAGHAMMER", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
				"DRIFTVORSCHLAG", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "WARRENER",
                "WARRENER2", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "WASHINGTON",
            };
            #endregion
            #region SUVs
            public static List<string> SUVs { get; } = new List<string>()
            {
                "ALEUTIAN", // CHOP SHOP DLC - Requires b3095
                "ASTRON", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "BALLER",
                "BALLER2",
                "BALLER3",
                "BALLER5",
                "BALLER4",
                "BALLER6",
                "BALLER7", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
				"BALLER8", // CHOP SHOP DLC - Requires b3095
                "BJXL",
				"CASTIGATOR", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "CAVALCADE",
                "CAVALCADE2",
                "CAVALCADE3", // CHOP SHOP DLC - Requires b3095
                "CONTENDER",
				"DORADO", // CHOP SHOP DLC - Requires b3095
                "DUBSTA",
                "DUBSTA2",
                "FQ2",
                "GRANGER",
                "GRANGER2", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "GRESLEY",
                "HABANERO",
                "HUNTLEY",
                "IWAGEN", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "ISSI8", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "JUBILEE", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "LANDSTALKER",
                "LANDSTALKER2", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "MESA",
                "MESA2",
                "NOVAK", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "PATRIOT",
                "PATRIOT2",
                "RADI",
                "REBLA", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "ROCOTO",
                "SEMINOLE",
                "SEMINOLE2", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "SERRANO",
                "SQUADDIE", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "TOROS",
				"VIVANITE", // CHOP SHOP DLC - Requires b3095
                "XLS",
                "XLS2",
            };
            #endregion
            #region Coupes
            public static List<string> Coupes { get; } = new List<string>()
            {
                "COGCABRIO",
				"EUROSX32", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "EXEMPLAR",
                "F620",
                "FELON",
                "FELON2",
				"FR36", // CHOP SHOP DLC - Requires b3095
				"DRIFTFR36", // CHOP SHOP DLC - Requires b3095
                "JACKAL",
                "KANJOSJ", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "ORACLE2",
                "ORACLE",
                "POSTLUDE", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "PREVION", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "SENTINEL2",
                "SENTINEL",
                "WINDSOR",
                "WINDSOR2",
                "ZION",
                "ZION2",
            };
            #endregion
            #region Muscle
            public static List<string> Muscle { get; } = new List<string>()
            {
                "DOMINATOR4",
                "IMPALER2",
                "IMPERATOR",
                "SLAMVAN4",
                "DUKES3", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "BLADE",
                "BRIGHAM",  // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "BROADWAY", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "BUCCANEER",
                "BUCCANEER2",
                "BUFFALO5", // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "BUFFALO4", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "STALION2",
                "CHINO",
                "CHINO2",
                "CLIQUE",
                "CLIQUE2", // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "COQUETTE3",
                "DEVIANT",
                "DOMINATOR",
                "DOMINATOR7", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "DOMINATOR10", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "DOMINATOR9", // CHOP SHOP DLC - Requires b3095
                "DOMINATOR8", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "DOMINATOR3",
                "YOSEMITE2", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
				"DRIFTYOSEMITE", // CHOP SHOP DLC - Requires b3095
                "DUKES2",
                "DUKES",
                "ELLIE",
                "EUDORA", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "FACTION",
                "FACTION2",
                "FACTION3",
                "DOMINATOR5",
                "IMPALER3",
                "IMPERATOR2",
                "SLAMVAN5",
                "GAUNTLET",
                "GAUNTLET3", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "GAUNTLET5", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "GAUNTLET4", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "GREENWOOD", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "HERMES",
                "HOTKNIFE",
                "HUSTLER",
                "IMPALER",
                "IMPALER6", // CHOP SHOP DLC - Requires b3095
                "IMPALER5", // CHOP SHOP DLC - Requires b3095
                "SLAMVAN2",
                "LURCHER",
                "MANANA2", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "MOONBEAM",
                "MOONBEAM2",
                "DOMINATOR6",
                "IMPALER4",
                "IMPERATOR3",
                "SLAMVAN6",
                "NIGHTSHADE",
                "PEYOTE2", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "PHOENIX",
                "PICADOR",
                "DOMINATOR2",
                "RATLOADER",
                "RATLOADER2",
                "GAUNTLET2",
                "RUINER",
                "RUINER3",
                "RUINER2",
                "RUINER4", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "SABREGT",
                "SABREGT2",
                "SLAMVAN",
                "SLAMVAN3",
                "STALION",
                "TAHOMA", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "TAMPA",
                "TULIP",
                "TULIP2", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "VAMOS",
                "VIGERO",
                "VIGERO2", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "VIGERO3", // CHOP SHOP DLC - Requires b3095
                "VIRGO",
                "VIRGO3",
                "VIRGO2",
                "VOODOO2",
                "VOODOO",
                "TAMPA3",
                "WEEVIL2", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "YOSEMITE",
            };
            #endregion
            #region SportsClassics
            public static List<string> SportsClassics { get; } = new List<string>()
            {
                "Z190",
                "ARDENT",
                "CASCO",
                "CHEBUREK",
                "CHEETAH2",
                "COQUETTE2",
                "COQUETTE5", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "DELUXO",
                "DYNASTY", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "FAGALOA",
                "BTYPE2",
                "GT500",
                "INFERNUS2",
                "JB700",
                "JB7002", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "MAMBA",
                "MANANA",
                "MICHELLI",
                "MONROE",
                "NEBULA", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
				"DRIFTNEBULA", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "PEYOTE",
                "PEYOTE3", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "PIGALLE",
                "RAPIDGT3",
                "RETINUE",
                "RETINUE2", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "BTYPE",
                "BTYPE3",
                "SAVESTRA",
                "STINGER",
                "STINGERGT",
                "FELTZER3", // Stirling GT
                "STROMBERG",
                "SWINGER",
                "TOREADOR", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "TORERO",
                "TORNADO3",
                "TORNADO2",
                "TORNADO4",
                "TORNADO",
                "TORNADO5",
                "TORNADO6",
                "TURISMO2",
                "VISERIS",
                "ZION3", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "ZTYPE",
            };
            #endregion
            #region Sports
            public static List<string> Sports { get; } = new List<string>()
            {
                "TENF", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "TENF2", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "R300", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "DRAFTER", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "NINEF",
                "NINEF2",
                "ALPHA",
                "ZR380",
                "BANSHEE",
                "BESTIAGTS",
                "BLISTA2",
                "BUFFALO",
                "BUFFALO2",
                "CALICO", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "CARBONIZZARE",
                "COMET2",
                "COMET3",
                "COMET6", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "COMET7", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "COMET4",
                "COMET5",
                "COQUETTE",
                "COQUETTE4", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "CORSITA", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "CYPHER", // LS TUNERS (MPTUNER) DLC - Requires b2372
				"DRIFTCYPHER", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "TAMPA2",
				"DRIFTTAMPA", // CHOP SHOP DLC - Requires b3095
                "ELEGY",
                "ELEGY2",
				"ENVISAGE", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "EUROS", // LS TUNERS (MPTUNER) DLC - Requires b2372
				"DRIFTEUROS", // CHOP SHOP DLC - Requires b3095
                "FELTZER2",
                "FLASHGT",
                "FUROREGT",
                "FUSILADE",
                "FUTO",
				"DRIFTFUTO", // CHOP SHOP DLC - Requires b3095
                "FUTO2", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "ZR3802",
                "GB200",
                "BLISTA3",
                "GROWLER", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "EVERON2", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "GAUNTLET6",  // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "HOTRING",
                "IMORGON", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "ISSI7", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "ITALIGTO",
                "STINGERTT",  // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "ITALIRSX", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "JESTER",
                "JESTER3",
                "JESTER2",
                "JESTER4", // LS TUNERS (MPTUNER) DLC - Requires b2372
				"DRIFTJESTER", // CHOP SHOP DLC - Requires b3095
                "JUGULAR", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "KHAMELION",
                "KOMODA", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "KURUMA",
                "KURUMA2",
                "COUREUR",  // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "LOCUST", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "LYNX",
                "MASSACRO",
                "MASSACRO2",
                "NEO", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "NEON",
                "ZR3803",
				"NIOBE", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "OMNIS",
                "OMNISEGT", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "PANTHERE", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "PARAGON", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "PARAGON2", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
				"PARAGON3", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "PARIAH",
                "PENUMBRA",
                "PENUMBRA2", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "RAIDEN",
                "RAPIDGT",
                "RAPIDGT2",
                "RAPTOR",
                "REMUS", // LS TUNERS (MPTUNER) DLC - Requires b2372
				"DRIFTREMUS", // CHOP SHOP DLC - Requires b3095
                "REVOLTER",
                "RT3000", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "RUSTON",
                "SCHAFTER4",
                "SCHAFTER3",
                "SCHLAGEN",
                "SCHWARZER",
                "SENTINEL3",
                "SENTINEL4", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
				"DRIFTSENTINEL", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "SEVEN70",
                "SM722", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699 
                "SPECTER",
                "SPECTER2",
                "BUFFALO3",
                "STREITER",
                "SUGOI", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "SULTAN",
                "SULTAN2", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "SULTAN3", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "SURANO",
                "TROPOS",
                "VECTRE", // LS TUNERS (MPTUNER) DLC - Requires b2372
                "VERLIERER2",
                "VETO", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "VETO2", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "VSTR", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "ZR350", // LS TUNERS (MPTUNER) DLC - Requires b2372
				"DRIFTZR350", // CHOP SHOP DLC - Requires b3095
            };
            #endregion
            #region Super
            public static List<string> Super { get; } = new List<string>()
            {
                "PFISTER811",
                "ADDER",
                "AUTARCH",
                "BANSHEE2",
                "BULLET",
                "CHAMPION", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "CHEETAH",
                "CYCLONE",
                "DEVESTE",
                "EMERUS", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "ENTITY3", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "ENTITYXF",
                "ENTITY2",
                "SHEAVA", // ETR1
                "FMJ",
                "FURIA", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "GP1",
                "IGNUS", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "INFERNUS",
                "ITALIGTB",
                "ITALIGTB2",
                "KRIEGER", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "LM87", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "NERO",
                "NERO2",
                "OSIRIS",
                "PENETRATOR",
				"PIPISTRELLO", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "LE7B",
                "REAPER",
                "VOLTIC2",
                "S80", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "SC1",
                "SCRAMJET",
                "SULTANRS",
                "T20",
                "TAIPAN",
                "TEMPESTA",
                "TEZERACT",
                "THRAX", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "TIGON", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "TORERO2", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "TURISMO3", // CHOP SHOP DLC - Requires b3095
                "TURISMOR",
                "TYRANT",
                "TYRUS",
                "VACCA",
                "VAGNER",
                "VIGILANTE",
                "VIRTUE", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "VISIONE",
                "VOLTIC",
                "PROTOTIPO",
                "XA21",
                "ZENO", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "ZENTORNO",
                "ZORRUSSO", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
            };
            #endregion
            #region Motorcycles
            public static List<string> Motorcycles { get; } = new List<string>()
            {
                "AKUMA",
                "DEATHBIKE",
                "AVARUS",
                "BAGGER",
                "BATI",
                "BATI2",
                "BF400",
                "CARBONRS",
                "CHIMERA",
                "CLIFFHANGER",
                "DAEMON",
                "DAEMON2",
                "DEFILER",
                "DIABLOUS",
                "DIABLOUS2",
                "DOUBLE",
                "ENDURO",
                "ESSKEY",
                "FAGGIO2",
                "FAGGIO3",
                "FAGGIO",
                "FCR",
                "FCR2",
                "DEATHBIKE2",
                "GARGOYLE",
                "HAKUCHOU",
                "HAKUCHOU2",
                "HEXER",
                "INNOVATION",
                "LECTRO",
                "MANCHEZ",
                "MANCHEZ2", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "MANCHEZ3", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "NEMESIS",
                "NIGHTBLADE",
                "DEATHBIKE3",
                "OPPRESSOR",
                "OPPRESSOR2",
                "PCJ",
				"PIZZABOY", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "POWERSURGE", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "RROCKET", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "RATBIKE",
                "REEVER", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "RUFFIAN",
                "SANCHEZ2",
                "SANCHEZ",
                "SANCTUS",
                "SHINOBI", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "SHOTARO",
                "SOVEREIGN",
                "STRYDER", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "THRUST",
                "VADER",
                "VINDICATOR",
                "VORTEX",
                "WOLFSBANE",
                "ZOMBIEA",
                "ZOMBIEB",
            };
            #endregion
            #region OffRoad
            public static List<string> OffRoad { get; } = new List<string>()
            {
                "BRUISER",
                "BRUTUS",
                "MONSTER3",
                "BIFTA",
                "BLAZER",
                "BLAZER5",
                "BLAZER2",
                "BODHI2",
                "BOOR", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "BRAWLER",
                "CARACARA",
                "CARACARA2", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "TROPHYTRUCK2",
                "DRAUGUR", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "DUBSTA3",
                "DUNE",
                "DUNE3",
                "DLOADER",
                "EVERON", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "FREECRAWLER",
                "BRUISER2",
                "BRUTUS2",
                "MONSTER4",
                "HELLION", // CASINO AND RESORT (MPVINEWOOD) DLC - Requires b2060
                "BLAZER3",
                "BFINJECTION",
                "INSURGENT2",
                "INSURGENT",
                "INSURGENT3",
                "KALAHARI",
                "KAMACHO",
                "MONSTER",
                "MARSHALL",
                "MENACER",
                "MESA3",
                "MONSTROCITI",  // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "BRUISER3",
                "BRUTUS3",
                "MONSTER5",
                "NIGHTSHARK",
                "OUTLAW", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "PATRIOT3", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "DUNE4",
                "DUNE5",
                "RANCHERXL",
                "RANCHERXL2",
                "RATEL",  // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "RCBANDITO",
                "REBEL2",
                "RIATA",
                "REBEL",
                "SANDKING2",
                "SANDKING",
                "DUNE2",
                "BLAZER4",
                "TECHNICAL",
                "TECHNICAL2",
                "TECHNICAL3",
				"TERMINUS", // CHOP SHOP DLC - Requires b3095
                "TROPHYTRUCK",
                "VAGRANT", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "VERUS", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "L35", // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "WINKY", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "YOSEMITE1500", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "YOSEMITE3", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "ZHABA", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
            };
            #endregion
            #region Industrial
            public static List<string> Industrial { get; } = new List<string>()
            {
                "CUTTER",
                "HANDLER",
                "BULLDOZER",
                "DUMP",
                "FLATBED",
                "GUARDIAN",
                "MIXER",
                "MIXER2",
                "RUBBLE",
                "TIPTRUCK",
                "TIPTRUCK2",
            };
            #endregion
            #region Utility
            public static List<string> Utility { get; } = new List<string>()
            {
                "AIRTUG",
                "CADDY3",
                "CADDY2",
                "CADDY",
                "DOCKTUG",
                "TRACTOR2", // Fieldmaster
                "TRACTOR3", // Fieldmaster
                "FORKLIFT",
                "MOWER", // Lawnmower
                "RIPLEY",
                "SADLER",
                "SADLER2",
                "SCRAP",
                "SLAMTRUCK", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "TOWTRUCK4", // CHOP SHOP DLC - Requires b3095
                "TOWTRUCK3", // CHOP SHOP DLC - Requires b3095
                "TOWTRUCK",
                "TOWTRUCK2",
                "TRACTOR", // Tractor (rusted/old)
                "UTILLITRUCK2",
                "UTILLITRUCK",
                "UTILLITRUCK3",

                /// Trailers

                /// Army Trailers
                "ARMYTRAILER2", // Civillian
                "ARMYTRAILER", // Military
                "ARMYTANKER", // Army Tanker
                "FREIGHTTRAILER", // Extended
                "TRAILERLARGE", // Mobile Operations Center

                /// Large Trailers
                "DOCKTRAILER", // Shipping Container Trailer
                "TR3", // Large Boat Trailer (Sailboat)
                "TR2", // Large Vehicle Trailer
                "TR4", // Large Vehicle Trailer (Mission Cars)
                "TRFLAT", // Large Flatbed Empty Trailer
                "TRAILERS", // Container/Curtain Trailer
                "TRAILERS2", // Box Trailer
                "TRAILERS3", // Ramp Box Trailer
                "TRAILERS4", // White Container Trailer
                "TRAILERS5", // Christmas Trailer CHOP SHOP DLC - Requires b3095
                "TVTRAILER", // Fame or Shame Trailer
                "TVTRAILER2", // LS Panic CHOP SHOP DLC - Requires b3095
                "TRAILERLOGS", // Logs Trailer
                "TANKER", // Ron Oil Tanker Trailer
                "TANKER2", // Ron Oil Tanker Trailer (Heist Version)

                /// Medium Trailers
                "BALETRAILER", // (Tractor Hay Bale Trailer)
                "GRAINTRAILER", // (Tractor Grain Trailer)

                // Ortega's trailer, we don't want this one because you can't drive them.
                //"PROPTRAILER",

                /// Small Trailers
                "BOATTRAILER", // Small Boat Trailer
				"BOATTRAILER2", // CHOP SHOP DLC - Requires b3095
				"BOATTRAILER3", // CHOP SHOP DLC - Requires b3095
                "RAKETRAILER", // Tractor Tow Plow/Rake
                "TRAILERSMALL", // Small Utility Trailer
            };
            #endregion
            #region Vans
            public static List<string> Vans { get; } = new List<string>()
            {
                "BOXVILLE5",
                "BISON2",
                "BISON3",
                "BISON",
                "BOBCATXL",
                "BOXVILLE2",
                "BOXVILLE3",
                "BOXVILLE",
                "BOXVILLE4",
                "BOXVILLE6", // CHOP SHOP DLC - Requires b3095
                "BURRITO2",
                "BURRITO3",
                "BURRITO",
                "BURRITO4",
                "BURRITO5",
                "CAMPER",
                "SPEEDO2",
                "GBURRITO",
                "GBURRITO2",
                "JOURNEY",
                "JOURNEY2", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "MINIVAN",
                "MINIVAN2",
                "PARADISE",
                "PONY",
                "PONY2",
                "RUMPO",
                "RUMPO2",
                "RUMPO3",
                "SPEEDO",
                "SPEEDO4",
                "SPEEDO5", // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "SURFER",
                "SURFER2",
                "SURFER3", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "TACO",
                "YOUGA",
                "YOUGA2",
                "YOUGA3", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "YOUGA4", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
            };
            #endregion
            #region Cycles
            public static List<string> Cycles { get; } = new List<string>()
            {
                "BMX",
                "CRUISER",
                "TRIBIKE2",
                "FIXTER",
                "INDUCTOR", // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "INDUCTOR2", // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "SCORCHER",
                "TRIBIKE3",
                "TRIBIKE",
            };
            #endregion
            #region Boats
            public static List<string> Boats { get; } = new List<string>()
            {
                "AVISA", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "DINGHY2",
                "DINGHY",
                "DINGHY3",
                "DINGHY4",
                "JETMAX",
                "KOSATKA", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "SUBMERSIBLE2",
                "PATROLBOAT", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "LONGFIN", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "MARQUIS",
                "PREDATOR",
                "SEASHARK",
                "SEASHARK2",
                "SEASHARK3",
                "SPEEDER2",
                "SPEEDER",
                "SQUALO",
                "SUBMERSIBLE",
                "SUNTRAP",
                "TORO",
                "TORO2",
                "TROPIC",
                "TROPIC2",
                "TUG",
                "DINGHY5", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
            };
            #endregion
            #region Helicopters
            public static List<string> Helicopters { get; } = new List<string>()
            {
                "AKULA",
                "ANNIHILATOR",
                "ANNIHILATOR2", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "BUZZARD2",
                "BUZZARD",
                "CARGOBOB",
                "CARGOBOB4",
                "CARGOBOB2",
                "CARGOBOB3",
                "CONADA", // CRIMINAL ENTERPRISES (MPSUM2) DLC - Requires b2699
                "HUNTER",
                "FROGGER",
                "FROGGER2",
                "HAVOK",
                "MAVERICK",
                "POLMAV",
                "SAVAGE",
                "SEASPARROW",
                "SKYLIFT",
                "SEASPARROW2", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "SEASPARROW3", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "SUPERVOLITO",
                "SUPERVOLITO2",
                "SWIFT",
                "SWIFT2",
                "VALKYRIE",
                "VALKYRIE2",
                "VOLATUS",
                "CONADA2", // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
            };
            #endregion
            #region Planes
            public static List<string> Planes { get; } = new List<string>()
            {
                "ALPHAZ1",
                "BLIMP",
                "AVENGER",
                "AVENGER2",
                "AVENGER3",  // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "AVENGER4",
                "STRIKEFORCE",
                "BESRA",
                "BLIMP3",
                "CARGOPLANE",
                "CARGOPLANE2",
                "CUBAN800",
                "DODO",
                "DUSTER",
                "RAIJU",  // SAN ANDREAS MERCENARIES (MP2023_01) DLC - Requires b2944
                "HOWARD",
                "HYDRA",
                "JET",
                "STARLING",
                "LUXOR",
                "LUXOR2",
                "STUNT",
                "MAMMATUS",
                "MILJET",
                "MOGUL",
                "NIMBUS",
                "NOKOTA",
                "LAZER",
                "PYRO",
                "BOMBUSHKA",
                "ALKONOST", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "ROGUE",
                "SEABREEZE",
                "SHAMAL",
                "STREAMER216",
                "TITAN",
                "TULA",
                "MICROLIGHT",
                "MOLOTOK",
                "VELUM",
                "VELUM2",
                "VESTRA",
                "VOLATOL",
                "BLIMP2",
            };
            #endregion
            #region Service
            public static List<string> Service { get; } = new List<string>()
            {
                "AIRBUS",
                "BRICKADE",
                "BRICKADE2", // LOS SANTOS DRUG WARS (mpchristmas3) DLC - Requires b2802
                "BUS",
                "COACH",
                "RALLYTRUCK",
                "PBUS2",
                "RENTALBUS",
                "TAXI",
                "TOURBUS",
                "TRASH",
                "TRASH2",
                "WASTELANDER",
            };
            #endregion
            #region Emergency
            public static List<string> Emergency { get; } = new List<string>()
            {
                "AMBULANCE",
                "FBI",
                "FBI2",
                "FIRETRUK",
                "LGUARD",
                "PBUS",
                "POLICE",
                "POLICE2",
                "POLICE3",
                "POLICE4",
                "POLICE5", // CHOP SHOP DLC - Requires b3095
                "POLICEB",
                "POLICEOLD1",
                "POLICEOLD2",
                "POLICET",
                "POLICET3", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
				"POLDOMINATOR10", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
				"POLDORADO", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
				"POLGAUNTLET", // CHOP SHOP DLC - Requires b3095
				"POLGREENWOOD", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
				"POLIMPALER5", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
				"POLIMPALER6", // BOTTOM DOLLAR BOUNTIES DLC - Requires b3258
                "PRANGER",
                "RIOT",
                "RIOT2",
                "SHERIFF",
                "SHERIFF2",
            };
            #endregion
            #region Military
            public static List<string> Military { get; } = new List<string>()
            {
                "APC",
                "SCARAB",
                "BARRACKS",
                "BARRACKS3",
                "BARRACKS2",
                "BARRAGE",
                "CHERNOBOG",
                "CRUSADER",
                "SCARAB2",
                "HALFTRACK",
                "MINITANK", // CASINO HEIST (MPHEIST3) DLC - Requires b2060
                "SCARAB3",
                "RHINO",
                "THRUSTER", // Jetpack
                "KHANJALI",
                "VETIR", // CAYO PERICO (MPHEIST4) DLC - Requires b2189
                "TRAILERSMALL2", // Anti Aircraft Trailer
            };
            #endregion
            #region Commercial
            public static List<string> Commercial { get; } = new List<string>()
            {
                "CERBERUS",
                "BENSON",
				"BENSON2", // CHOP SHOP DLC - Requires b3095
                "BIFF",
                "CERBERUS2",
                "HAULER",
                "HAULER2",
                "MULE",
                "MULE3",
                "MULE2",
                "MULE5", // THE CONTRACT (MPSECURITY) DLC - Requires b2545
                "MULE4",
                "CERBERUS3",
                "PACKER",
                "PHANTOM",
                "PHANTOM4", // CHOP SHOP DLC - Requires b3095
                "PHANTOM3",
                "PHANTOM2",
                "POUNDER",
                "POUNDER2",
                "STOCKADE",
                "STOCKADE3",
                "TERBYTE",
            };
            #endregion
            #region Trains
            public static List<string> Trains { get; } = new List<string>()
            {
                "CABLECAR",
                "FREIGHT",
                "FREIGHT2", // CHOP SHOP DLC - Requires b3095
                "FREIGHTCAR",
                "FREIGHTCONT1",
                "FREIGHTCONT2",
                "FREIGHTGRAIN",
                "METROTRAIN",
                "TANKERCAR",
            };
            #endregion
            #region OpenWheel
            public static List<string> OpenWheel { get; } = new List<string>()
            {
                "OPENWHEEL1", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "OPENWHEEL2", // SUMMER SPECIAL (MPSUM) DLC - Requires b2060
                "FORMULA",
                "FORMULA2",
            };
            #endregion


            /*
            Compacts = 0,
            Sedans = 1,
            SUVs = 2,
            Coupes = 3,
            Muscle = 4,
            SportsClassics = 5,
            Sports = 6,
            Super = 7,
            Motorcycles = 8,
            OffRoad = 9,
            Industrial = 10,
            Utility = 11,
            Vans = 12,
            Cycles = 13,
            Boats = 14,
            Helicopters = 15,
            Planes = 16,
            Service = 17,
            Emergency = 18,
            Military = 19,
            Commercial = 20,
            Trains = 21
            OpenWheel = 22
            */

            public static Dictionary<string, List<string>> VehicleClasses { get; } = new Dictionary<string, List<string>>()
            {
                [GetLabelText("VEH_CLASS_0")] = Compacts,
                [GetLabelText("VEH_CLASS_1")] = Sedans,
                [GetLabelText("VEH_CLASS_2")] = SUVs,
                [GetLabelText("VEH_CLASS_3")] = Coupes,
                [GetLabelText("VEH_CLASS_4")] = Muscle,
                [GetLabelText("VEH_CLASS_5")] = SportsClassics,
                [GetLabelText("VEH_CLASS_6")] = Sports,
                [GetLabelText("VEH_CLASS_7")] = Super,
                [GetLabelText("VEH_CLASS_8")] = Motorcycles,
                [GetLabelText("VEH_CLASS_9")] = OffRoad,
                [GetLabelText("VEH_CLASS_10")] = Industrial,
                [GetLabelText("VEH_CLASS_11")] = Utility,
                [GetLabelText("VEH_CLASS_12")] = Vans,
                [GetLabelText("VEH_CLASS_13")] = Cycles,
                [GetLabelText("VEH_CLASS_14")] = Boats,
                [GetLabelText("VEH_CLASS_15")] = Helicopters,
                [GetLabelText("VEH_CLASS_16")] = Planes,
                [GetLabelText("VEH_CLASS_17")] = Service,
                [GetLabelText("VEH_CLASS_18")] = Emergency,
                [GetLabelText("VEH_CLASS_19")] = Military,
                [GetLabelText("VEH_CLASS_20")] = Commercial,
                [GetLabelText("VEH_CLASS_21")] = Trains,
                [GetLabelText("VEH_CLASS_22")] = OpenWheel,
            };
            #endregion

            public static string[] GetAllVehicles()
            {
                var vehs = new List<string>();
                foreach (var vc in VehicleClasses)
                {
                    foreach (var c in vc.Value)
                    {
                        vehs.Add(c);
                    }
                }
                return vehs.ToArray();
            }
        }
    }
}
