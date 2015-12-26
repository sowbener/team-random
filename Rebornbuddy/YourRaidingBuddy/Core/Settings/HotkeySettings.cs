
using ff14bot.Helpers;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    internal class SettingsH : JsonSettings
    {
        public SettingsH()
            : base(InternalSettings.RoutineSettingsPath + "_Hotkey.json")
        {
        }
        /* ======================================== */

        #region Toggle Type
        [Setting]
        [DefaultValue(HotkeyMode.Automatic)]
        [Category("Toggle Type Hotkeys")]
        [DisplayName("Hotkey Mode Selector")]
        [Description("Select the hotkey mode: Auto, Semi Hotkey (Only Pause & Cooldowns) or Hotkey (All hotkeys).")]
        public HotkeyMode HotkeyModeSelection { get; set; }


        [Setting]
        [DefaultValue(true)]
        [Category("Hotkey Settings")]
        [DisplayName(@"Hotkeys - Enable Overlay Output")]
        [Description("Enables Overlay output for hotkeys.")]
        public bool HotkeyChatOutput { get; set; }


        [Setting]
        [DefaultValue(ModifierKeys.Shift)]
        [Category("Toggle Type Hotkeys")]
        [DisplayName("Modifier Key")]
        [Description("Select the preferred modifier key for the hotkeys.")]
        public ModifierKeys ModifierKey { get; set; }

        [Setting]
        [DefaultValue(Keys.None)]
        [Category("Toggle Type Hotkeys")]
        [DisplayName("Pause Key")]
        [Description("Select the preferred key to toggle pause the routine.")]
        public Keys PauseKey { get; set; }

        [Setting]
        [DefaultValue(Keys.None)]
        [Category("Toggle Type Hotkeys")]
        [DisplayName("Cooldown Key")]
        [Description("Select the preferred key to toggle cooldowns.")]
        public Keys CooldownKey { get; set; }

        [Setting]
        [DefaultValue(Keys.None)]
        [Category("Toggle Type Hotkeys")]
        [DisplayName("SpecialKey")]
        [Description("Select the preferred key to toggle SpecialKey")]
        public Keys SpecialKey { get; set; }

        [Setting]
        [DefaultValue(Keys.None)]
        [Category("Toggle Type Hotkeys")]
        [DisplayName("SpecialKey1")]
        [Description("Select the preferred key to toggle SpecialKey1")]
        public Keys SpecialKey1 { get; set; }

        [Setting]
        [DefaultValue(Keys.None)]
        [Category("Toggle Type Hotkeys")]
        [DisplayName("Multi-Target Key (AoE)")]
        [Description("Select the preferred key to toggle AoE (Multi-Target).")]
        public Keys MultiTargetKey { get; set; }
        #endregion

    }
}
