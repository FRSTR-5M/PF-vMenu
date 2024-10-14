using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static vMenuShared.PermissionsManager;
using static vMenuShared.ConfigManager;
using vMenuClient;

namespace Freecam2
{
    public class Main : BaseScript
    {
        bool IsInFreecam = false;
        public static string FreecamKey { get; private set; } = "U"; // F2 by default (ReplayStartStopRecordingSecondary)
        public Main()
        {
            TriggerServerEvent("vMenu:RequestPermissions");
            EventHandlers["onClientResourceStart"] += new Action<string>(ResourceStart);
        }

        bool freecamNotAllowedNotified = false;
        private async void ResourceStart(string Name)
        {
            if (!(GetSettingsString(Setting.vmenu_freecam_toggle_key) == null))
            {
                FreecamKey = GetSettingsString(Setting.vmenu_freecam_toggle_key);
            }
            else
            {
                FreecamKey = "U";
            }
            if (GetCurrentResourceName() != Name) return;

            if (IsAllowed(Permission.Freecam))
            {
                RegisterKeyMapping($"{GetSettingsString(Setting.vmenu_individual_server_id)}vMenu:freecam", "vMenu Freecam Toggle Button", "keyboard", FreecamKey);
            }
            RegisterCommand($"{GetSettingsString(Setting.vmenu_individual_server_id)}vMenu:freecam", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (!MainMenu.vMenuEnabled)
                    return;

                if (IsAllowed(Permission.Freecam))
                {
                    IsInFreecam = !IsInFreecam;
                    if (IsInFreecam)
                    {
                        //TriggerEvent("mosh_notify:notify", "INFO", "<span class=\"text-black\">You are now using Freecam²!</span>", "info", "info", 5000);
                        Notify.Success("You are now using Freecam²!");
                        Freecam.Enable();
                    }
                    else
                        Freecam.Disable();
                }
                else if (!freecamNotAllowedNotified)
                {
                    //TriggerEvent("mosh_notify:notify", "Error", "<span class=\"text-black\">You are not allowed to use Freecam²!</span>", "error", "error", 5000);
                    Notify.Error("You are not allowed to use Freecam²! Consider removing this key bind (Settings > Key Bindings > FiveM) if you do not wish to see this message again.");
                    freecamNotAllowedNotified = true;
                }
            }), false);
        }

        public static void SendBasicMessage(string message)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 236, 80, 156 },
                args = new[] { "[Freecam²]", message }
            });
        }
    }
}
