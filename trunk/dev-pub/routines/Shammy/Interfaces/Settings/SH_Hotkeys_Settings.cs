using System.Windows.Forms;
using Shammy.Helpers;
using Styx.Common;
using Styx.Helpers;
using System.ComponentModel;

namespace Shammy.Interfaces.Settings
{
    internal class SmSettingsH : Styx.Helpers.Settings
    {
        public static SmSettingsH Instance = new SmSettingsH();

        public SmSettingsH()
            : base(SmSettings.SettingsPath + "_Hotkey.xml")
        {
        }


        #region Hotkeys
        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("AMZ Key")]
        [Description("Choose preferred AMZ key.")]
        public Keys AMZ { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Tier6 Ability Key")]
        [Description("Choose preferred Tier 6 ability key.")]
        public Keys Tier6 { get; set; }

        
        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.Mode.Auto)]
        [Category("Hotkeys")]
        [DisplayName("Mode Selector")]
        [Description("Choose preferred Hotkey Mode.")]
        public SmEnum.Mode ModeSelection { get; set; }

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
        [Description("Choose preferred special key.")]
        public Keys SpecialKeyChoice { get; set; }
        #endregion
    }
}
