using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CitizenFX.Core;

using MenuAPI;

using Microsoft.Win32;

using vMenuClient.data;
using vMenuClient.MenuAPIWrapper;

using static CitizenFX.Core.Native.API;
using static vMenuClient.CommonFunctions;
using static vMenuShared.PermissionsManager;


namespace vMenuClient.menus
{
    public class VehicleOptions
    {
        #region Variables

        private WMenu menu = null;

        // Public variables (getters only), return the private variables.
        public bool VehicleGodMode { get; private set; } = UserDefaults.VehicleGodMode;
        public bool VehicleGodInvincible { get; private set; } = UserDefaults.VehicleGodInvincible;
        public bool VehicleGodEngine { get; private set; } = UserDefaults.VehicleGodEngine;
        public bool VehicleGodVisual { get; private set; } = UserDefaults.VehicleGodVisual;
        public bool VehicleGodStrongWheels { get; private set; } = UserDefaults.VehicleGodStrongWheels;
        public bool VehicleGodRamp { get; private set; } = UserDefaults.VehicleGodRamp;
        public bool VehicleGodAutoRepair { get; private set; } = UserDefaults.VehicleGodAutoRepair;

        public bool VehicleNeverDirty { get; private set; } = UserDefaults.VehicleNeverDirty;
        public bool VehicleEngineAlwaysOn { get; private set; } = UserDefaults.VehicleEngineAlwaysOn;
        public bool VehicleNoSiren { get; private set; } = UserDefaults.VehicleNoSiren;
        public bool VehicleNoBikeHelmet { get; private set; } = UserDefaults.VehicleNoBikeHelmet;
        public bool FlashHighbeamsOnHonk { get; private set; } = UserDefaults.VehicleHighbeamsOnHonk;
        public bool DisablePlaneTurbulence { get; private set; } = UserDefaults.VehicleDisablePlaneTurbulence;
        public bool DisableHelicopterTurbulence { get; private set; } = UserDefaults.VehicleDisableHelicopterTurbulence;
        public bool VehicleBikeSeatbelt { get; private set; } = UserDefaults.VehicleBikeSeatbelt;
        public bool VehicleInfiniteFuel { get; private set; } = false;
        public bool VehicleShowHealth { get; private set; } = false;
        public bool VehicleFrozen { get; private set; } = false;
        public bool VehicleTorqueMultiplier { get; private set; } = false;
        public bool VehiclePowerMultiplier { get; private set; } = false;
        public float VehicleTorqueMultiplierAmount { get; private set; } = 2f;
        public float VehiclePowerMultiplierAmount { get; private set; } = 2f;
        public bool VehicleDeleteRemovedDoors { get; private set; } = false;
        #endregion

        private async Task ConfigureSpeedLimiter(int mode)
        {
            var vehicle = TryGetIntactDriverVehicle("configure the speed limiter");
            if (vehicle == null)
                return;

            bool metric = ShouldUseMetricMeasurements();

            if (mode == 0) // Set
            {
                SetEntityMaxSpeed(vehicle.Handle, 500.01f);
                SetEntityMaxSpeed(vehicle.Handle, vehicle.Speed);

                if (metric)
                {
                    Notify.Info($"Vehicle speed is now limited to ~b~{Math.Round(3.6f * vehicle.Speed)} km/h~s~.");
                }
                else
                {
                    Notify.Info($"Vehicle speed is now limited to ~b~{Math.Round(2.237f * vehicle.Speed)} mph~s~.");
                }

            }
            else if (mode == 1) // Reset
            {
                SetEntityMaxSpeed(vehicle.Handle, 500.01f); // Default max speed seemingly for all vehicles.
                Notify.Info("Vehicle speed is now no longer limited.");
            }
            else if (mode == 2) // custom speed
            {
                var speedStr = await GetUserInput($"Enter a speed ({(metric ? "km/h" : "mph")})", metric ? "100.0" : "60.0", 5);
                if (!string.IsNullOrEmpty(speedStr))
                {
                    if (float.TryParse(speedStr, out var speed))
                    {
                        var speedMs = speed / (metric ? 3.6f : 2.237f);

                        //vehicle.MaxSpeed = outFloat;
                        SetEntityMaxSpeed(vehicle.Handle, 500.01f);
                        await BaseScript.Delay(0);
                        SetEntityMaxSpeed(vehicle.Handle, speedMs);
                        if (ShouldUseMetricMeasurements()) // kph
                        {
                            Notify.Info($"Vehicle speed is now limited to ~b~{Math.Round(speed)} km/h~s~.");
                        }
                        else
                        {
                            Notify.Info($"Vehicle speed is now limited to ~b~{Math.Round(speed)} mph~s~.");
                        }
                    }
                    else
                    {
                        Notify.Error(CommonErrors.InvalidInput, placeholderValue: "speed");
                    }
                }
                else
                {
                    Notify.Error(CommonErrors.InvalidInput, placeholderValue: "speed");
                }
            }
        }

        private bool IsDoorOpen (Vehicle vehicle, int index)
        {
            int handle = vehicle.Handle;

            if (index < 8)
                return GetVehicleDoorAngleRatio(handle, index) > 0.1f;

            if (vehicle.HasBombBay)
                return AreBombBayDoorsOpen(handle);

            return false;
        }

        private void ToggleDoor(Vehicle vehicle, int index, bool open)
        {
            int handle = vehicle.Handle;

            if (index < 8)
            {
                if (open)
                {
                    SetVehicleDoorOpen(handle, index, false, false);
                }
                else
                {
                    SetVehicleDoorShut(handle, index, false);
                }
            }
            else if (vehicle.HasBombBay)
            {
                if (open)
                {
                    OpenBombBayDoors(handle);
                }
                else
                {
                    CloseBombBayDoors(handle);
                }
            }
        }

        private void TryToggleDoor(int index)
        {
            var veh = TryGetIntactDriverVehicle($"open or close {(index == -1 ? "all doors" : "a specific door")}");
            if (veh == null)
                return;

            bool open;
            if (index == -1)
            {
                open = !Enumerable
                    .Range(0, 8)
                    .Where(i => !IsVehicleDoorDamaged(veh.Handle, i))
                    .Any(i => IsDoorOpen(veh, i));
            }
            else
            {
                open = !IsDoorOpen(veh, index);
            }

            if (index != -1)
            {
                ToggleDoor(veh, index, open);
            }
            else
            {
                for (int i = 0; i <= 8; i++)
                {
                    ToggleDoor(veh, i, open);
                }
            }
        }

        private void TryDetachDoor(int index)
        {
            var veh = TryGetIntactDriverVehicle($"detach {(index == -1 ? "all doors" : "a specific door")}");
            if (veh == null)
                return;

            if (index != -1)
            {
                SetVehicleDoorBroken(veh.Handle, index, VehicleDeleteRemovedDoors);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    SetVehicleDoorBroken(veh.Handle, i, VehicleDeleteRemovedDoors);
                }
            }
        }

        private void TryToggleWindow(int index, bool rollUp)
        {
            var veh = TryGetIntactDriverVehicle($"roll vehicle windows {(rollUp ? "up" : "down")}");
            if (veh == null)
                return;

            int handle = veh.Handle;
            if (index == -1 && !rollUp)
            {
                RollDownWindows(handle);
            }
            else if (index == -1)
            {
                for (int i = 0; i < 4; i++)
                {
                    RollUpWindow(handle, i);
                }
            }
            else
            {
                if (rollUp)
                {
                    RollUpWindow(handle, index);
                }
                else
                {
                    RollDownWindow(handle, index);
                }
            }
        }

        private void ToggleTire(Vehicle vehicle, int index, bool inflate)
        {
            int handle = vehicle.Handle;

            if (inflate)
            {
                SetVehicleTyreFixed(handle, index);
            }
            else
            {
                SetVehicleTyreBurst(handle, index, false, 1);
            }
        }

        private void TryToggleTire(int index)
        {
            var veh = TryGetIntactDriverVehicle($"inflate or burst {(index == -1 ? "all tires" : "a specific tire")}");
            if (veh == null)
                return;

            bool inflate;
            if (index == -1)
            {
                inflate = Enumerable.Range(0, 8).Any(i => IsVehicleTyreBurst(veh.Handle, i, false));
            }
            else
            {
                inflate = IsVehicleTyreBurst(veh.Handle, index, false);
            }

            if (index != -1)
            {
                ToggleTire(veh, index, inflate);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    ToggleTire(veh, i, inflate);
                }
            }
        }

        private void TryDetachWheel(int index)
        {
            var veh = TryGetIntactDriverVehicle($"detach {(index == -1 ? "all wheels" : "a specific wheel")}");
            if (veh == null)
                return;

            int handle = veh.Handle;

            if (index != -1)
            {
                BreakOffVehicleWheel(handle, index, false, false, true, false);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    BreakOffVehicleWheel(handle, i, false, false, true, false);
                }
            }
        }

        private void TryToggleVehicleVisibility()
        {
            var vehicle = TryGetIntactDriverVehicle("change its visibility");

            if (vehicle.IsVisible)
            {
                // Check the visibility of all peds inside before setting the vehicle as invisible.
                var visiblePeds = new Dictionary<Ped, bool>();
                foreach (var p in vehicle.Occupants)
                {
                    visiblePeds.Add(p, p.IsVisible);
                }

                vehicle.IsVisible = false;

                // Restore visibility for each ped.
                foreach (var pe in visiblePeds)
                {
                    pe.Key.IsVisible = pe.Value;
                }
            }
            else
            {
                vehicle.IsVisible = true;
            }
        }

        private void SetVehicleIndicators(Vehicle veh, bool left, bool right)
        {
            SetVehicleIndicatorLights(veh.Handle, 1, left);
            SetVehicleIndicatorLights(veh.Handle, 0, right);
        }

        private void ToggleVehicleLights(int index)
        {
            var veh = TryGetIntactDriverVehicle("toggle the lights");
            if (veh == null)
                return;

            var state = GetVehicleIndicatorLights(veh.Handle); // 0 = none, 1 = left, 2 = right, 3 = both

            if (index == 0) // Hazard lights
            {
                bool on = state != 3;
                SetVehicleIndicators(veh, on, on);
            }
            else if (index == 1) // left indicator
            {
                SetVehicleIndicators(veh, state != 1, false);
            }
            else if (index == 2) // right indicator
            {
                SetVehicleIndicators(veh, false, state != 2);
            }
            else if (index == 3) // Interior lights
            {
                SetVehicleInteriorlight(veh.Handle, !IsVehicleInteriorLightOn(veh.Handle));
            }
            else if (index == 4) // helicopter spotlight
            {
                SetVehicleSearchlight(veh.Handle, !IsVehicleSearchlightOn(veh.Handle), true);
            }
        }

        private void ApplyVehiclePowerMultiplier()
        {
            var veh = TryGetIntactDriverVehicle(null);
            if (veh == null)
                return;

            float mult = VehiclePowerMultiplier ? VehiclePowerMultiplierAmount : 1f;
            SetVehicleEnginePowerMultiplier(veh.Handle, mult);
        }

        #region CreateMenu()
        /// <summary>
        /// Create menu creates the vehicle options menu.
        /// </summary>
        private void CreateMenu()
        {
            // Create the menu.
            menu = new WMenu(MenuTitle, "Vehicle Options");

            if (IsAllowed(Permission.VORepair))
            {
                var fixVehicle = new MenuItem("Repair Vehicle", "Repair you vehicle's visual and physical damage.").ToWrapped();
                fixVehicle.Selected += (_s, _args) =>
                {
                    var veh = TryGetDriverVehicle("repair");
                    veh?.Repair();
                };

                menu.AddItem(fixVehicle);
            }

            #region God Mode
            if (IsAllowed(Permission.VOGod)) // GOD MODE
            {
                var vehGodMenu = new WMenu(MenuTitle, "God Mode");


                var vehicleGod = new MenuCheckboxItem("Enable", "Enable vehicle god mode.", VehicleGodMode).ToWrapped();
                vehicleGod.CheckboxChanged += (_s, args) => VehicleGodMode = args.Checked;


                var godInvincible = new MenuCheckboxItem("Invincible", "Makes the car invincible. Includes fire damage, explosion damage, collision damage and more.", VehicleGodInvincible).ToWrapped();
                godInvincible.CheckboxChanged += (_s, args) => VehicleGodInvincible = args.Checked;

                var godEngine = new MenuCheckboxItem("Engine Damage", "Disables your engine from taking any damage.", VehicleGodEngine).ToWrapped();
                godEngine.CheckboxChanged += (_s, args) => VehicleGodEngine = args.Checked;

                var godVisual = new MenuCheckboxItem("Visual Damage", "This prevents scratches and other damage decals from being applied to your vehicle. ~y~It does not prevent (body) deformation damage.~s~", VehicleGodVisual).ToWrapped();
                godVisual.CheckboxChanged += (_s, args) => VehicleGodVisual = args.Checked;

                var godStrongWheels = new MenuCheckboxItem("Strong Wheels", "Disables your wheels from being deformed and causing reduced handling. ~y~This does not make tires bulletproof.~s~", VehicleGodStrongWheels).ToWrapped();
                godStrongWheels.CheckboxChanged += (_s, args) => VehicleGodStrongWheels = args.Checked;

                var godRamp = new MenuCheckboxItem("Ramp Damage", "Disables vehicles such as the Ramp Buggy from taking damage when using the ramp.", VehicleGodRamp).ToWrapped();
                godRamp.CheckboxChanged += (_s, args) => VehicleGodRamp = args.Checked;

                var godAutoRepair = new MenuCheckboxItem("~y~Auto Repair~s~", "Automatically repairs your vehicle when it has ~italic~any~italic~ damage. ~y~It is recommended to keep this turned off to prevent glitches.~s~", VehicleGodAutoRepair).ToWrapped();
                godAutoRepair.CheckboxChanged += (_s, args) => VehicleGodAutoRepair = args.Checked;

                vehGodMenu
                    .AddItem(vehicleGod)
                    .AddSection("Disabled Damage Types", [godInvincible, godEngine, godVisual, godStrongWheels, godRamp, godAutoRepair]);
                menu.AddSubmenu(vehGodMenu, "Enable vehicle god mode of various damage types.");
            }
            #endregion

            #region Engine
            {
                var engineMenu = new WMenu(MenuTitle, "Engine");

                var multipliersSect = new List<WMenuItem>();
                var engineSect = new List<WMenuItem>();

                // {2,4,8,...,1024}
                var multiplierList = Enumerable.Range(1, 11).Select(x => $"{Math.Pow(2,x)}x").ToList();

                var indexToMult = (int index) => (float)Math.Pow(2, index + 1);

                if (IsAllowed(Permission.VOTorqueMultiplier))
                {
                    var text = "Torque Multiplier";
                    var torqueMultiplier = new MenuListItem(text, multiplierList, 0, "Set the engine torque multiplier. Click to enable or disable the multiplier.").ToWrapped();
                    torqueMultiplier.ListSelected += (_s, args) =>
                    {
                        VehicleTorqueMultiplier = !VehicleTorqueMultiplier;
                        args.Item.Text = VehicleTorqueMultiplier ? $"~g~{text}~s~" : text;
                    };
                    torqueMultiplier.ListChanged += (_s, args) => VehicleTorqueMultiplierAmount = indexToMult(args.ListIndexNew);

                    multipliersSect.Add(torqueMultiplier);
                }
                if (IsAllowed(Permission.VOPowerMultiplier))
                {
                    var text = "Power Multiplier";
                    var powerMultiplier = new MenuListItem(text, multiplierList, 0, "Set the engine power multiplier. Click to enable or disable the multiplier.").ToWrapped();
                    powerMultiplier.ListSelected += (_s, args) =>
                    {
                        VehiclePowerMultiplier = !VehiclePowerMultiplier;
                        args.Item.Text = VehiclePowerMultiplier ? $"~g~{text}~s~" : text;
                        ApplyVehiclePowerMultiplier();
                    };
                    powerMultiplier.ListChanged += (_s, args) =>
                    {
                        VehiclePowerMultiplierAmount = indexToMult(args.ListIndexNew);
                        ApplyVehiclePowerMultiplier();
                    };

                    multipliersSect.Add(powerMultiplier);
                }


                if (IsAllowed(Permission.VOEngine))
                {
                    var toggleEngine = new MenuItem("Toggle Engine", "Turn your engine on or off.").ToWrapped();
                    toggleEngine.Selected += (_s, _args) =>
                    {
                        var veh = TryGetIntactDriverVehicle("toggle the engine");
                        if (veh == null)
                            return;
                        SetVehicleEngineOn(veh.Handle, !veh.IsEngineRunning, false, true);
                    };

                    engineSect.Add(toggleEngine);
                }

                if (IsAllowed(Permission.VOEngineAlwaysOn))
                {
                    var vehicleEngineAO = new MenuCheckboxItem("Engine Always On", "Keeps your vehicle engine running when you exit your vehicle.", VehicleEngineAlwaysOn).ToWrapped();
                    vehicleEngineAO.CheckboxChanged += (_s, args) => VehicleEngineAlwaysOn = args.Checked;

                    engineSect.Add(vehicleEngineAO);
                }

                if (IsAllowed(Permission.VODestroyEngine))
                {
                    var destroyEngine = new MenuItem("Destroy Engine", "Destroy your vehicle's engine.").ToWrapped();
                    destroyEngine.Selected += (_s, _args) =>
                    {
                        var veh = TryGetIntactDriverVehicle("destroy the engine");
                        if (veh == null)
                            return;
                        SetVehicleEngineHealth(veh.Handle, -4000);
                    };

                    engineSect.Add(destroyEngine);
                }


                engineMenu.AddSections([
                    new Section("Multipliers", multipliersSect),
                    new Section("Engine", engineSect),
                ]);

                menu.AddSubmenu(engineMenu, "Modify vehicle engine performance.");
            }
            #endregion

            #region Doors & Windows
            {
                var wheelsAndWindowsMenu = new WMenu(MenuTitle, "Doors & Windows");

                var doorsSect = new List<WMenuItem>();
                var windowsSect = new List<WMenuItem>();

                if (IsAllowed(Permission.VODoors))
                {
                    var doors = new List<string>{ "Front Left", "Front Right", "Rear Left", "Rear Right", "Hood", "Trunk", "Extra 1", "Extra 2", "Bomb Bay" };

                    var toggleAll = new MenuItem("Toggle All Doors", "Open or close all vehicle doors.").ToWrapped();
                    toggleAll.Selected += (_s, _args) => TryToggleDoor(-1);

                    var toggleDoor = new MenuListItem("Toggle Door", doors, 0, "Open or close a specific vehicle door (if it exists).").ToWrapped();
                    toggleDoor.ListSelected += (_s, args) => TryToggleDoor(args.ListIndex);

                    var removeAll = new MenuItem("Detach All Doors", "Detach all vehicle doors.").ToWrapped();
                    removeAll.Selected += (_s, _args) => TryDetachDoor(-1);

                    var removableDoors = new List<string>{ "Front Left", "Front Right", "Rear Left", "Rear Right", "Hood", "Trunk" };
                    var removeDoorList = new MenuListItem("Detach Door", removableDoors, 0, "Detach a specific vehicle door.").ToWrapped();
                    removeDoorList.ListSelected += (_s, args) => TryDetachDoor(args.ListIndex);

                    var deleteDoors = new MenuCheckboxItem("Delete Removed Doors", "When enabled, doors that you remove using the list above will be deleted from the world instead of falling to the ground.", VehicleDeleteRemovedDoors).ToWrapped();
                    deleteDoors.CheckboxChanged += (_s, args) => VehicleDeleteRemovedDoors = args.Checked;

                    doorsSect.AddRange([toggleAll, toggleDoor, removeAll, removeDoorList, deleteDoors]);
                }

                if (IsAllowed(Permission.VOWindows))
                {
                    var windows = new List<string> { "Front Left", "Front Right", "Rear Left", "Rear Right" };

                    var toggleAllWindows = new MenuListItem("All Windows", new List<string>{ "Up", "Down" }, 0, "Roll all vehicle windows up or down.").ToWrapped();
                    toggleAllWindows.ListSelected += (_s, args) => TryToggleWindow(-1, args.ListIndex == 0);

                    var rollUpWindow = new MenuListItem("Roll Window Up", windows, 0, "Roll a specific vehicle window up (if it exists).").ToWrapped();
                    rollUpWindow.ListSelected += (_s, args) => TryToggleWindow(args.ListIndex, true);

                    var rollDownWindow = new MenuListItem("Roll Window Down", windows, 0, "Roll a specific vehicle window down (if it exists).").ToWrapped();
                    rollDownWindow.ListSelected += (_s, args) => TryToggleWindow(args.ListIndex, false);

                    windowsSect.AddRange([toggleAllWindows, rollUpWindow, rollDownWindow]);
                }

                wheelsAndWindowsMenu.AddSections([
                    new Section("Doors", doorsSect),
                    new Section("Windows", windowsSect)
                ]);

                menu.AddSubmenu(wheelsAndWindowsMenu, "Toggle your vehicle's doors and windows");
            }
            #endregion

            #region Wheels & Tires
            if (IsAllowed(Permission.VOFixOrDestroyTires))
            {
                var tiresAndWheelsMenu = new WMenu(MenuTitle, "Wheels & Tires");

                var wheels = Enumerable.Range(0, 8).Select(x => $"Wheel #{x}").ToList();
                var tires = Enumerable.Range(0, 8).Select(x => $"Tire #{x}").ToList();

                var detachAllWheels = new MenuItem("Detach All Wheels", "Detach all vehicle wheels.").ToWrapped();
                detachAllWheels.Selected += (_s, args) => TryDetachWheel(-1);

                var detachWheel = new MenuListItem("Detach Wheel", wheels, 0, "Detach all vehicle wheels.").ToWrapped();
                detachWheel.ListSelected += (_s, args) => TryDetachWheel(args.ListIndex);

                var toggleAllTires = new MenuItem("Toggle All Tires", "Inflate or burst all vehicle tires.").ToWrapped();
                toggleAllTires.Selected += (_s, args) => TryToggleTire(-1);

                var toggleTire = new MenuListItem("Toggle Tire", tires, 0, "Inflate or burst a specific vehicle tire.").ToWrapped();
                toggleTire.ListSelected += (_s, args) => TryToggleTire(args.ListIndex);


                tiresAndWheelsMenu.AddItems([detachAllWheels, detachWheel, toggleAllTires, toggleTire]);

                menu.AddSubmenu(tiresAndWheelsMenu, "Remove your vehicle's wheels, or de- and inflate its tires.");
            }
            #endregion

            {
                var radioStationOptions = new List<Tuple<RadioStation, string>>
                {
                    new Tuple<RadioStation, string>(RadioStation.LosSantosRockRadio, "Los Santos Rock Radio"),
                    new Tuple<RadioStation, string>(RadioStation.NonStopPopFM, "Non Stop Pop FM"),
                    new Tuple<RadioStation, string>(RadioStation.RadioLosSantos, "Radio Los Santos"),
                    new Tuple<RadioStation, string>(RadioStation.ChannelX, "Channel X"),
                    new Tuple<RadioStation, string>(RadioStation.WestCoastTalkRadio, "West Coast Talk Radio"),
                    new Tuple<RadioStation, string>(RadioStation.RebelRadio, "Rebel Radio"),
                    new Tuple<RadioStation, string>(RadioStation.SoulwaxFM, "Soulwax FM"),
                    new Tuple<RadioStation, string>(RadioStation.EastLosFM, "East Los FM"),
                    new Tuple<RadioStation, string>(RadioStation.WestCoastClassics, "West Coast Classics"),
                    new Tuple<RadioStation, string>(RadioStation.TheBlueArk, "The Blue Ark"),
                    new Tuple<RadioStation, string>(RadioStation.WorldWideFM, "World Wide FM"),
                    new Tuple<RadioStation, string>(RadioStation.FlyloFM, "Flylo FM"),
                    new Tuple<RadioStation, string>(RadioStation.TheLowdown, "The Lowdown 91.1"),
                    new Tuple<RadioStation, string>(RadioStation.TheLab, "The Lab"),
                    new Tuple<RadioStation, string>(RadioStation.RadioMirrorPark, "Radio Mirror Park"),
                    new Tuple<RadioStation, string>(RadioStation.Space, "Space 103.2"),
                    new Tuple<RadioStation, string>(RadioStation.VinewoodBoulevardRadio, "Vinewood Boulevard Radio"),
                    new Tuple<RadioStation, string>(RadioStation.BlondedLosSantos, "Blonded Los Santos 97.8 FM"),
                    new Tuple<RadioStation, string>(RadioStation.LosSantosUndergroundRadio, "Los Santos Underground Radio"),
                    new Tuple<RadioStation, string>(RadioStation.RadioOff, "~italic~Off~italic~"),
                    // We disable these, because they are not reliable to set and depend on where on the map you are
                    // new Tuple<RadioStation, string>(RadioStation.BlaineCountyRadio, "Blaine County Radio"),
                    // new Tuple<RadioStation, string>(RadioStation.SelfRadio , "~italic~Media Player~italic~"),
                };

                var radioIndex = radioStationOptions
                    .Select((t, i) => new Tuple<RadioStation,int>(t.Item1,i))
                    .FirstOrDefault(t => t.Item1 == (RadioStation)UserDefaults.VehicleDefaultRadio)
                    .Item2;

                var radioStations = new MenuListItem(
                    "Radio Station",
                    radioStationOptions.Select(t => t.Item2).ToList(),
                    radioIndex,
                    "Select a radio station for your current and also new vehicles.").ToWrapped();
                radioStations.ListSelected += (_s, args) =>
                {
                    var newStation = radioStationOptions[args.ListIndex].Item1;
                    UserDefaults.VehicleDefaultRadio = (int)newStation;

                    var veh = TryGetIntactDriverVehicle(null);
                    if (veh == null)
                        return;

                    SetVehRadioStation(veh.Handle, RadioData.RadioStationToGameName[newStation]);
                };

                menu.AddItem(radioStations);
            }

            if (IsAllowed(Permission.VOLights))
            {
                var lights = new List<string>()
                {
                    "Hazard Lights",
                    "Left Indicator",
                    "Right Indicator",
                    "Interior Lights",
                    "Helicopter Spotlight",
                };
                var vehicleLights = new MenuListItem("Toggle Vehicle Lights", lights, 0, "Turn vehicle lights on or off.").ToWrapped();
                vehicleLights.ListSelected += (_s, args) => ToggleVehicleLights(args.ListIndex);

                menu.AddItem(vehicleLights);
            }

            #region Dust & Dirt
            {
                var dustAndDirt = new WMenu(MenuTitle, "Dust & Dirt");

                if (IsAllowed(Permission.VOWash))
                {
                    var cleanVehicle = new MenuItem("Wash Vehicle", "Clean your vehicle.").ToWrapped();
                    cleanVehicle.Selected += (_s, _args) => TryGetIntactDriverVehicle("wash")?.Wash();

                    var dirtLevels = new List<string> { "No Dirt", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
                    var setDirtLevel = new MenuListItem("Set Dirt Level", dirtLevels, 0, "Select how much dirt should be visible on your vehicle.").ToWrapped();
                    setDirtLevel.ListSelected += (_s, args) =>
                    {
                        var veh = TryGetIntactDriverVehicle("change the dirt level");
                        if (veh == null)
                            return;

                        veh.DirtLevel = args.ListIndex;
                    };

                    dustAndDirt.AddItems([cleanVehicle, setDirtLevel]);
                }

                if (IsAllowed(Permission.VOKeepClean))
                {
                    var vehicleNeverDirty = new MenuCheckboxItem("Keep Vehicle Clean", "This will automatically wash your vehicle whenever it is dirty. ~y~This only cleans dust or dirt, but ~italic~not~italic~ mud, snow or damage decals.~s~ Repair your vehicle to remove them.", VehicleNeverDirty).ToWrapped();
                    vehicleNeverDirty.CheckboxChanged += (_s, args) => VehicleNeverDirty = args.Checked;

                    dustAndDirt.AddItem(vehicleNeverDirty);
                }

                menu.AddSubmenu(dustAndDirt, "Change dust and dirt on your vehicle.");
            }
            #endregion

            #region Misc
            {
                var miscMenu = new WMenu(MenuTitle, "Miscellaneous");

                if (IsAllowed(Permission.VOFlip))
                {
                    var flipVehicle = new MenuItem("Flip Vehicle", "Sets your vehicle on all wheels.").ToWrapped();
                    flipVehicle.Selected += (_s, _args) =>
                    {
                        var veh = TryGetIntactDriverVehicle("flip the vehicle");
                        if (veh == null)
                            return;

                        SetVehicleOnGroundProperly(veh.Handle);
                    };

                    miscMenu.AddItem(flipVehicle);
                }

                if (IsAllowed(Permission.VOFlashHighbeamsOnHonk))
                {
                    var highbeamsOnHonk = new MenuCheckboxItem("Flash Highbeams On Honk", "Flash your highbeams when honking. ~y~Does not work during the day when you have your lights turned off.~s~", FlashHighbeamsOnHonk).ToWrapped();
                    highbeamsOnHonk.CheckboxChanged += (_s, args) => FlashHighbeamsOnHonk = args.Checked;

                    miscMenu.AddItem(highbeamsOnHonk);
                }

                if (IsAllowed(Permission.VOCycleSeats))
                {
                    var cycleSeats = new MenuItem("Cycle Through Vehicle Seats", "Cycle through the available vehicle seats.").ToWrapped();
                    cycleSeats.Selected += (_s, _args) =>
                    {
                        var veh = TryGetIntactVehicle("switch seats");
                        if (veh == null)
                            return;

                        CycleThroughSeats();
                    };

                    miscMenu.AddItem(cycleSeats);
                }

                if (IsAllowed(Permission.VOBikeSeatbelt))
                {
                    var vehicleBikeSeatbelt = new MenuCheckboxItem("Bike Seatbelt", "Prevents you from being knocked off your bike, bicycle, ATV or similar.", VehicleBikeSeatbelt).ToWrapped();
                    vehicleBikeSeatbelt.CheckboxChanged += (_s, args) => VehicleBikeSeatbelt = args.Checked;

                    miscMenu.AddItem(vehicleBikeSeatbelt);
                }
                if (IsAllowed(Permission.VONoHelmet))
                {
                    var vehicleNoBikeHelmet = new MenuCheckboxItem("No Bike Helmet", "Prevent auto-equipping a helmet when getting on a bike or quad.", VehicleNoBikeHelmet).ToWrapped();
                    vehicleNoBikeHelmet.CheckboxChanged += (_s, args) => VehicleNoBikeHelmet = args.Checked;

                    miscMenu.AddItem(vehicleNoBikeHelmet);
                }

                if (IsAllowed(Permission.VODisableTurbulence))
                {
                    var noTurbulencePlane = new MenuCheckboxItem("Disable Plane Turbulence", "Disables the turbulence for all planes.", DisablePlaneTurbulence).ToWrapped();
                    noTurbulencePlane.CheckboxChanged += (_s, args) => DisablePlaneTurbulence = args.Checked;

                    var noTurbulenceHeli = new MenuCheckboxItem("Disable Helicopter Turbulence", "Disables the turbulence for all helicopters.", DisableHelicopterTurbulence).ToWrapped();
                    noTurbulenceHeli.CheckboxChanged += (_s, args) => DisableHelicopterTurbulence = args.Checked;

                    miscMenu.AddItems([noTurbulencePlane, noTurbulenceHeli]);
                }


                if (IsAllowed(Permission.VOSpeedLimiter))
                {
                    var speedLimiterOptions = new List<string>() { "Set", "Reset", "Custom" };
                    var speedLimiter = new MenuListItem("Speed Limiter", speedLimiterOptions, 0, "Configure the speed limiter for your current vehicle.").ToWrapped();
                    speedLimiter.ListSelected += async (_o, args) => await ConfigureSpeedLimiter(args.ListIndex);

                    miscMenu.AddItem(speedLimiter);
                }

                if (IsAllowed(Permission.VOAlarm))
                {
                    var vehicleAlarm = new MenuItem("Toggle Vehicle Alarm", "Starts or stops your vehicle's alarm. ~y~Some vehicles might not have an alarm.~s~").ToWrapped();
                    vehicleAlarm.Selected += (_s, _args) =>
                    {
                        var veh = TryGetIntactDriverVehicle("toggle the alarm");
                        if (veh == null)
                            return;

                        ToggleVehicleAlarm(veh);
                    };

                    miscMenu.AddItem(vehicleAlarm);
                }
                if (IsAllowed(Permission.VONoSiren) && !vMenuShared.ConfigManager.GetSettingsBool(vMenuShared.ConfigManager.Setting.vmenu_use_els_compatibility_mode))
                {
                    var vehicleNoSiren = new MenuCheckboxItem("Disable Siren", "Disables your vehicle's siren. ~y~Only works if your vehicle actually has a siren.~s~", VehicleNoSiren).ToWrapped();
                    vehicleNoSiren.CheckboxChanged += (_s, args) => VehicleNoSiren = args.Checked;

                    miscMenu.AddItem(vehicleNoSiren);
                }


                {
                    var showHealth = new MenuCheckboxItem("Show Vehicle Health", "Displays the vehicle's health on the screen.", VehicleShowHealth).ToWrapped();
                    showHealth.CheckboxChanged += (_s, args) => VehicleShowHealth = args.Checked;

                    miscMenu.AddItem(showHealth);
                }

                if (IsAllowed(Permission.VOReduceDriftSuspension))
                {
                    var reduceDriftSuspension = new MenuItem("Toggle Drift Suspension", "Reduce the suspension of the vehicle to make it even lower to drift. Use the option again to revert back to your original suspension. ~y~This modification overrides the original advanced handling flags of the vehicle!~s~").ToWrapped();
                    reduceDriftSuspension.Selected += (_s, _args) => SetVehicleDriftSuspension();

                    miscMenu.AddItem(reduceDriftSuspension);
                }

                if (IsAllowed(Permission.VOInfiniteFuel))
                {
                    var infiniteFuel = new MenuCheckboxItem("Infinite Fuel", "Enables or disables infinite fuel for this vehicle. ~y~Only works if ~o~FRFuel~y~ is installed.~s~", VehicleInfiniteFuel).ToWrapped();
                    infiniteFuel.CheckboxChanged += (_s, args) => VehicleInfiniteFuel = args.Checked;

                    miscMenu.AddItem(infiniteFuel);
                }


                if (IsAllowed(Permission.VOFreeze))
                {
                    var vehicleFreeze = new MenuCheckboxItem("Freeze Vehicle", "Freeze your vehicle's position.", VehicleFrozen).ToWrapped();
                    vehicleFreeze.CheckboxChanged += (_s, args) =>
                    {
                        VehicleFrozen = args.Checked;

                        var veh = TryGetDriverVehicle(null);
                        if (veh != null)
                        {
                            FreezeEntityPosition(veh.Handle, args.Checked);
                        }
                    };

                    miscMenu.AddItem(vehicleFreeze);
                }

                if (IsAllowed(Permission.VOInvisible))
                {
                    var vehicleInvisible = new MenuItem("Vehicle Visible", "Toggle the visibility of your vehicle. ~y~Your vehicle will be made visible again as soon as you leave the vehicle.~s~").ToWrapped();
                    vehicleInvisible.Selected += (_s, _args) => TryToggleVehicleVisibility();

                    miscMenu.AddItem(vehicleInvisible);
                }

                menu.AddSubmenu(miscMenu, "Miscellaneous vehicle settings.");
            }
            #endregion

            if (IsAllowed(Permission.VODelete))
            {
                var delete = WMenuItem.CreateConfirmationButton("~r~Delete~s~", "Deletes your vehicle. ~y~This cannot be undone~s~.");
                delete.Confirmed += (_s, _args) =>
                {
                    var veh = TryGetDriverVehicle("delete the vehicle");
                    if (veh == null)
                        return;

                    SetVehicleHasBeenOwnedByPlayer(veh.Handle, false);
                    SetEntityAsMissionEntity(veh.Handle, false, false);
                    veh.Delete();
                };

                menu.AddItem(delete);
            }
        }
        #endregion

        /// <summary>
        /// Public get method for the menu. Checks if the menu exists, if not create the menu first.
        /// </summary>
        /// <returns>Returns the Vehicle Options menu.</returns>
        public Menu GetMenu()
        {
            // If menu doesn't exist. Create one.
            if (menu == null)
            {
                CreateMenu();
            }
            // Return the menu.
            return menu.Menu;
        }
    }
}
