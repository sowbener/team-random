using System.IO;
using System.Windows.Forms;
using Styx;
using Styx.Common;
using Styx.Helpers;

namespace Enyo.Shared
{
    class BotSettings : Settings
    {
        private static BotSettings _instance;

        public static BotSettings Instance
        {
            get { return _instance ?? (_instance = new BotSettings()); }
        }

        public BotSettings()
            : base(
                Path.Combine(Utilities.AssemblyDirectory,
                             string.Format(@"Settings/Enyo/Enyo-Store-Settings-{0}-SettingsRev{1}.xml", StyxWoW.Me.Name, Enyo.Settings)))
        {
        }

        #region UI Settings
        [Setting, DefaultValue(true)]
        public bool UseOverlayOutput { get; set; }

        [Setting, DefaultValue(true)]
        public bool ClickToMove { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseContinuesHealingMode { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseForcedCombatMode { get; set; }

        [Setting, DefaultValue(false)]
        public bool UsePluginPulsing { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseSoftLock { get; set; }

        [Setting, DefaultValue(false)]
        public bool UseEnyoNavigator { get; set; }

        [Setting, DefaultValue(Keys.X)]
        public Keys PauseKeyChoice { get; set; }

        [Setting, DefaultValue(ModifierKeys.Alt)]
        public ModifierKeys ModKeyChoice { get; set; }
        #endregion

        #region Manual Settings
        [Setting, DefaultValue("")]
        public string LastStatCounted { get; set; }

        [Setting, DefaultValue(4)]
        public int OverlayOutputTime { get; set; }
        #endregion
    }
}
