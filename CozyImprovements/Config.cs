using BepInEx.Configuration;

namespace SpyciBot.LC.CozyImprovements
{
    public class Config
    {
        public ConfigEntry<bool> configBigDoorButtons;
        public ConfigEntry<bool> configEasyLaunchLever;
        public ConfigEntry<bool> configBigTeleporterButtons;
        public ConfigEntry<bool> configBigMonitorButtons;
        public Config(ConfigFile cfg)
        {
            configBigDoorButtons = cfg.Bind(
                    "Accessibility",                  // Config subsection
                    "BigDoorButtons",                  // Key of this config
                    false,                               // Default value
                    "Enlarges the door buttons so they're easier to press"         // Description
            );

            configEasyLaunchLever = cfg.Bind(
                    "Accessibility",                  // Config subsection
                    "EasyLaunchLever",                  // Key of this config
                    true,                               // Default value
                    "Enlarges the hitbox for the Launch Lever to cover more of the table so it's easier to pull"         // Description
            );
            configBigTeleporterButtons = cfg.Bind(
                    "Accessibility",                  // Config subsection
                    "BigTeleporterButtons",                  // Key of this config
                    false,                               // Default value
                    "Enlarges the teleporter buttons so they're easier to press"         // Description
            );
            configBigMonitorButtons = cfg.Bind(
                    "Accessibility",                  // Config subsection
                    "BigMonitorButtons",                  // Key of this config
                    false,                               // Default value
                    "Enlarges the Monitor buttons so they're easier to press"         // Description
            );
        }

    }
}
