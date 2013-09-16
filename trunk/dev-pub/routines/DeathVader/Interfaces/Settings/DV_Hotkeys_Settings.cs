using System.Windows.Forms;
using DeathVader.Helpers;
using Styx.Common;
using Styx.Helpers;
using System.ComponentModel;

namespace DeathVader.Interfaces.Settings
{
    internal class DvSettingsH : Styx.Helpers.Settings
    {
        public static DvSettingsH Instance = new DvSettingsH();

        public DvSettingsH()
            : base(DvSettings.SettingsPath + "_Hotkey.xml")
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
        [DisplayName("Raise Ally Key")]
        [Description("Choose preferred Raise Ally key.")]
        public Keys RaiseAlly { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Tier6 Ability Key")]
        [Description("Choose preferred Tier 6 ability key.")]
        public Keys Tier6 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Keys.None)]
        [Category("Hotkeys")]
        [DisplayName("Army of the Dead Ability Key")]
        [Description("Choose preferred Army of the Dead during combat ability key.")]
        public Keys ArmyofTheDeadKey { get; set; }

        
        [Setting]
        [Styx.Helpers.DefaultValue(DvEnum.Mode.Auto)]
        [Category("Hotkeys")]
        [DisplayName("Mode Selector")]
        [Description("Choose preferred Hotkey Mode.")]
        public DvEnum.Mode ModeSelection { get; set; }

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
        [Styx.Helpers.DefaultValue(Keys.F1)]
        [Category("Hotkeys")]
        [DisplayName("Special Key")]
        [Description("Choose preferred special key.")]
        public Keys SpecialKeyChoice { get; set; }
        #endregion
    }
}
