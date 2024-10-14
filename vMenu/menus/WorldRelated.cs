using System;
using System.Collections.Generic;
using System.Linq;
using MenuAPI;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static vMenuClient.CommonFunctions;
using static vMenuShared.PermissionsManager;
using static vMenuShared.ConfigManager;
using System.Collections;

namespace vMenuClient
{
    public class NPCDensityMenu : BaseScript
    {
        private Menu menu;

        private static readonly LanguageManager Lm = new LanguageManager();
        IDictionary<string, string> WorldDensity = new Dictionary<string, string>();
        private void CreateMenu()
        {
            // Create the menu
            menu = new Menu(MenuTitle, "NPC Density Options (Experimental)");
            //TriggerServerEvent("vMenu:GetPrimaryMaterial_Sync", VehToNet(vehicle))
            var valsvdm = GetSettingsFloat(Setting.vmenu_set_vehicle_density_multiplier);   
            var valspdm = GetSettingsFloat(Setting.vmenu_set_ped_density_multiplier);       
            var valsrvdm = GetSettingsFloat(Setting.vmenu_set_random_vehicle_density_multiplier);  
            var valspvdm = GetSettingsFloat(Setting.vmenu_set_parked_vehicle_density_multiplier);
            var valsdpdm = GetSettingsFloat(Setting.vmenu_set_scenario_ped_density_multiplier);     
            var valsgt = GetSettingsBool(Setting.vmenu_set_garbage_trucks);
            var valsrb = GetSettingsBool(Setting.vmenu_set_random_boats);
            var valscrc = GetSettingsBool(Setting.vmenu_set_create_random_cops);
            var valscrcno = GetSettingsBool(Setting.vmenu_set_create_random_cops_not_onscenarios);
            var valscrcos = GetSettingsBool(Setting.vmenu_set_create_random_cops_on_scenarios);
            MenuSliderItem svdm = new MenuSliderItem($"Vehicle Density Multiplier", 0, 20, (int)(valsvdm*10), false);
            MenuSliderItem spdm = new MenuSliderItem($"Ped Density Multiplier", 0, 20, (int)(valspdm*10), false);
            MenuSliderItem srvdm = new MenuSliderItem($"Random Vehicle Density Multiplier", 0, 20, (int)(valsrvdm*10), false);
            MenuSliderItem spvdm = new MenuSliderItem($"Parked Vehicle Density Multiplier", 0, 20, (int)(valspvdm*10), false);
            MenuSliderItem sdpdm = new MenuSliderItem($"Scenario Ped Density Multiplier", 0, 20, (int)(valsdpdm*10), false);

            MenuCheckboxItem sgt = new MenuCheckboxItem("Garbage Trucks", "", valsgt);
            MenuCheckboxItem srb = new MenuCheckboxItem("Random Boats", "", valsrb);
            MenuCheckboxItem scrc = new MenuCheckboxItem("Create Random Cops", "", valscrc);
            MenuCheckboxItem scrcno = new MenuCheckboxItem("Create Random Cops Not On Scenarios", "", valscrcno);
            MenuCheckboxItem scrcos = new MenuCheckboxItem("Create Random Cops On Scenarios", "", valscrcos);

            MenuItem updatetoserver = new MenuItem("Update Server", "Update Values To Server.");


            menu.AddMenuItem(svdm);
            menu.AddMenuItem(spdm);
            menu.AddMenuItem(srvdm);
            menu.AddMenuItem(spvdm);
            menu.AddMenuItem(sdpdm);
            menu.AddMenuItem(sgt);
            menu.AddMenuItem(srb);
            menu.AddMenuItem(scrc);
            menu.AddMenuItem(scrcno);
            menu.AddMenuItem(scrcos);

            menu.AddMenuItem(updatetoserver);
            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == updatetoserver)
                {
                    WorldDensity["vmenu_set_vehicle_density_multiplier"] = ((float)svdm.Position/10).ToString();
                    WorldDensity["vmenu_set_ped_density_multiplier"] = ((float)spdm.Position/10).ToString();
                    WorldDensity["vmenu_set_random_vehicle_density_multiplier"] = ((float)srvdm.Position/10).ToString();
                    WorldDensity["vmenu_set_parked_vehicle_density_multiplier"] = ((float)spvdm.Position/10).ToString();
                    WorldDensity["vmenu_set_scenario_ped_density_multiplier"] = ((float)sdpdm.Position/10).ToString();
                    WorldDensity["vmenu_set_garbage_trucks"] = sgt.Checked.ToString().ToLower();
                    WorldDensity["vmenu_set_random_boats"] = srb.Checked.ToString().ToLower();
                    WorldDensity["vmenu_set_create_random_cops"] = scrc.Checked.ToString().ToLower();
                    WorldDensity["vmenu_set_create_random_cops_not_onscenarios"] = scrcno.Checked.ToString().ToLower();
                    WorldDensity["vmenu_set_create_random_cops_on_scenarios"] = scrcos.Checked.ToString().ToLower();
                    TriggerServerEvent("vMenu:WorldRelated_Server", WorldDensity);
                }
            };

        }
        public void RefreshMenu()
        {
            menu.ClearMenuItems();
            var valsvdm = GetSettingsFloat(Setting.vmenu_set_vehicle_density_multiplier);
            var valspdm = GetSettingsFloat(Setting.vmenu_set_ped_density_multiplier);
            var valsrvdm = GetSettingsFloat(Setting.vmenu_set_random_vehicle_density_multiplier);
            var valspvdm = GetSettingsFloat(Setting.vmenu_set_parked_vehicle_density_multiplier);
            var valsdpdm = GetSettingsFloat(Setting.vmenu_set_scenario_ped_density_multiplier);
            var valsgt = GetSettingsBool(Setting.vmenu_set_garbage_trucks);
            var valsrb = GetSettingsBool(Setting.vmenu_set_random_boats);
            var valscrc = GetSettingsBool(Setting.vmenu_set_create_random_cops);
            var valscrcno = GetSettingsBool(Setting.vmenu_set_create_random_cops_not_onscenarios);
            var valscrcos = GetSettingsBool(Setting.vmenu_set_create_random_cops_on_scenarios);
            MenuSliderItem svdm = new MenuSliderItem($"Vehicle Density Multiplier", 0, 20, (int)(valsvdm*10), false);
            MenuSliderItem spdm = new MenuSliderItem($"Ped Density Multiplier", 0, 20, (int)(valspdm*10), false);
            MenuSliderItem srvdm = new MenuSliderItem($"Random Vehicle Density Multiplier", 0, 20, (int)(valsrvdm*10), false);
            MenuSliderItem spvdm = new MenuSliderItem($"Parked Vehicle Density Multiplier", 0, 20, (int)(valspvdm*10), false);
            MenuSliderItem sdpdm = new MenuSliderItem($"Scenario Ped Density Multiplier", 0, 20, (int)(valsdpdm*10), false);

            MenuCheckboxItem sgt = new MenuCheckboxItem("Garbage Trucks", "", valsgt);
            MenuCheckboxItem srb = new MenuCheckboxItem("Random Boats", "", valsrb);
            MenuCheckboxItem scrc = new MenuCheckboxItem("Create Random Cops", "", valscrc);
            MenuCheckboxItem scrcno = new MenuCheckboxItem("Create Random Cops Not On Scenarios", "", valscrcno);
            MenuCheckboxItem scrcos = new MenuCheckboxItem("Create Random Cops On Scenarios", "", valscrcos);
            MenuItem updatetoserver = new MenuItem("Update Server", "Update Values to server.");

            menu.AddMenuItem(svdm);
            menu.AddMenuItem(spdm);
            menu.AddMenuItem(srvdm);
            menu.AddMenuItem(spvdm);
            menu.AddMenuItem(sdpdm);
            menu.AddMenuItem(sgt);
            menu.AddMenuItem(srb);
            menu.AddMenuItem(scrc);
            menu.AddMenuItem(scrcno);
            menu.AddMenuItem(scrcos);
            
            menu.AddMenuItem(updatetoserver);

            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == updatetoserver)
                {
                    WorldDensity["vmenu_set_vehicle_density_multiplier"] = ((float)svdm.Position/10).ToString();
                    WorldDensity["vmenu_set_ped_density_multiplier"] = ((float)spdm.Position/10).ToString();
                    WorldDensity["vmenu_set_random_vehicle_density_multiplier"] = ((float)srvdm.Position/10).ToString();
                    WorldDensity["vmenu_set_parked_vehicle_density_multiplier"] = ((float)spvdm.Position/10).ToString();
                    WorldDensity["vmenu_set_scenario_ped_density_multiplier"] = ((float)sdpdm.Position/10).ToString();
                    WorldDensity["vmenu_set_garbage_trucks"] = sgt.Checked.ToString().ToLower();
                    WorldDensity["vmenu_set_random_boats"] = srb.Checked.ToString().ToLower();
                    WorldDensity["vmenu_set_create_random_cops"] = scrc.Checked.ToString().ToLower();
                    WorldDensity["vmenu_set_create_random_cops_not_onscenarios"] = scrcno.Checked.ToString().ToLower();
                    WorldDensity["vmenu_set_create_random_cops_on_scenarios"] = scrcos.Checked.ToString().ToLower();
                    TriggerServerEvent("vMenu:WorldRelated_Server", WorldDensity);
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






