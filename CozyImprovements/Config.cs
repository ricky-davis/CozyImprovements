using BepInEx.Configuration;

namespace SpyciBot.LC.CozyImprovements
{
    public class Config
    {

        // General
        public ConfigEntry<bool> configStorageLights;
        public ConfigEntry<bool> configLightSwitchGlow;
        public ConfigEntry<bool> configTerminalGlow;
        public ConfigEntry<bool> configTerminalMonitorAlwaysOn;
        public ConfigEntry<bool> configChargeStationGlow;
        // Accessibility
        public ConfigEntry<bool> configBigDoorButtons;
        public ConfigEntry<bool> configEasyLaunchLever;
        public ConfigEntry<bool> configBigTeleporterButtons;
        public ConfigEntry<bool> configBigMonitorButtons;
        public Config(ConfigFile cfg)
        {
            // General
            // General
            // General
            configStorageLights = cfg.Bind(
                    "General",                  // Config subsection
                    "StorageLightsEnabled",                  // Key of this config
                    true,                               // Default value
                    "Makes the LightSwitch glow in the dark"         // Description
            );
            configLightSwitchGlow = cfg.Bind(
                    "General",                  // Config subsection
                    "LightSwitchGlowEnabled",                  // Key of this config
                    true,                               // Default value
                    "Makes the LightSwitch glow in the dark"         // Description
            );
            configTerminalGlow = cfg.Bind(
                    "General",                  // Config subsection
                    "TerminalGlowEnabled",                  // Key of this config
                    true,                               // Default value
                    "Makes the Terminal glow active all the time"         // Description
            );
            configTerminalMonitorAlwaysOn = cfg.Bind(
                    "General",                  // Config subsection
                    "TerminalMonitorAlwaysOn",                  // Key of this config
                    true,                               // Default value
                    "Makes the Terminal screen active all the time; Will show the screen you left it on"         // Description
            );
            configChargeStationGlow = cfg.Bind(
                    "General",                  // Config subsection
                    "ChargeStationGlowEnabled",                  // Key of this config
                    true,                               // Default value
                    "Makes the Charging Station glow with a yellow light"         // Description
            );
            // Accessibility
            // Accessibility
            // Accessibility
            configBigDoorButtons = cfg.Bind(
                    "General.Accessibility",                  // Config subsection
                    "BigDoorButtonsEnabled",                  // Key of this config
                    false,                               // Default value
                    "Enlarges the door buttons so they're easier to press"         // Description
            );
            configEasyLaunchLever = cfg.Bind(
                    "General.Accessibility",                  // Config subsection
                    "EasyLaunchLeverEnabled",                  // Key of this config
                    true,                               // Default value
                    "Enlarges the hitbox for the Launch Lever to cover more of the table so it's easier to pull"         // Description
            );
            configBigTeleporterButtons = cfg.Bind(
                    "General.Accessibility",                  // Config subsection
                    "BigTeleporterButtonsEnabled",                  // Key of this config
                    false,                               // Default value
                    "Enlarges the teleporter buttons so they're easier to press"         // Description
            );
            configBigMonitorButtons = cfg.Bind(
                    "General.Accessibility",                  // Config subsection
                    "BigMonitorButtonsEnabled",                  // Key of this config
                    false,                               // Default value
                    "Enlarges the Monitor buttons so they're easier to press"         // Description
            );
        }

    }
}
