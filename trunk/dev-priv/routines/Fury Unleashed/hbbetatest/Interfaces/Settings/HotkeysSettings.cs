using System.Windows.Forms;
using FuryUnleashed.Core.Helpers;
using Styx.Common;
using Styx.Helpers;
using System.ComponentModel;

namespace FuryUnleashed.Interfaces.Settings
{
    internal class SettingsH : Styx.Helpers.Settings
    {
        public static SettingsH Instance = new SettingsH();

        public SettingsH()
            : base(InternalSettings.SettingsPath + "_Hotkey.xml")
        {
        }

        // ========================================================================================

        #region Hotkeys
        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Demoralizing Banner Key")]
        [Description("Choose preferred Demoralizing Banner key.")]
        public Keys DemoBannerChoice { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Heroic Leap Key")]
        [Description("Choose preferred Heroic Leap key.")]
        public Keys HeroicLeapChoice { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Mocking Banner Key")]
        [Description("Choose preferred Mocking Banner key.")]
        public Keys MockingBannerChoice { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Shattering Throw Key")]
        [Description("Choose preferred Shattering Throw key.")]
        public Keys ShatteringThrowChoice { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Tier 4 Ability Key")]
        [Description("Choose preferred Tier 4 ability key.")]
        public Keys Tier4Choice { get; set; }

        // ========================================================================================
        
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.Mode.Auto)]
        [Category("Hotkeys")]
        [DisplayName("Mode Selector")]
        [Description("Choose preferred Hotkey Mode.")]
        public Enum.Mode ModeSelection { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(ModifierKeys.Alt)]
        [Category("Hotkeys")]
        [DisplayName("Modifier Key")]
        [Description("Choose preferred modifier key.")]
        public ModifierKeys ModKeyChoice { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Pause Key")]
        [Description("Choose preferred pause key.")]
        public Keys PauseKeyChoice { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Cooldown Key")]
        [Description("Choose preferred cooldown key.")]
        public Keys CooldownKeyChoice { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Multi-Target Key (AoE)")]
        [Description("Choose preferred AoE key.")]
        public Keys MultiTgtKeyChoice { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Special Key")]
        [Description("Arms: None - Fury: None - Prot: Switch Shield Block or Barrier.")]
        public Keys SpecialKeyChoice { get; set; }
        #endregion

        // ========================================================================================
    }
}
