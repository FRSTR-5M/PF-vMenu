using MenuAPI;

using static vMenuClient.CommonFunctions;

namespace vMenuClient.menus
{
    public class About
    {
        // Variables
        private Menu menu;

        private void CreateMenu()
        {
            // Create the menu.
            menu = new Menu(MenuTitle, "About");

            {
                var items = MenuItemsFromJsonTuples("config/about.json");
                if (items.Count > 0)
                {
                    items.ForEach(menu.AddMenuItem);
                }
            }

            var credits = new MenuItem("vMenu Credits", $"vMenu is made by ~b~Vespura~s~. Extra modifications are done by members of ~b~Project Fairness Labs~s~ and ~b~BenniCubed~s~.\n\nDownload version ~b~{MainMenu.Version}~s~ of vMenu: ~b~github.com/BenniCubed/PF-vMenu~s~");

            var serverInfoMessage = vMenuShared.ConfigManager.GetSettingsString(vMenuShared.ConfigManager.Setting.vmenu_server_info_message);
            if (!string.IsNullOrEmpty(serverInfoMessage))
            {
                var serverInfo = new MenuItem("Server Info", serverInfoMessage);
                var siteUrl = vMenuShared.ConfigManager.GetSettingsString(vMenuShared.ConfigManager.Setting.vmenu_server_info_website_url);
                if (!string.IsNullOrEmpty(siteUrl))
                {
                    serverInfo.Label = $"{siteUrl}";
                }
                menu.AddMenuItem(serverInfo);
            }
            menu.AddMenuItem(credits);
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
