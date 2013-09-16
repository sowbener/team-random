using Styx;
using Styx.Common;
using Styx.Helpers;
using System.IO;
using System.Windows.Forms;
using YBMoP_BT_Warrior.Helpers;

namespace YBMoP_BT_Warrior.Interfaces.Settings
{
    internal class YBSettingsG : Styx.Helpers.Settings
    {
        public static readonly YBSettingsG Instance = new YBSettingsG();

        public YBSettingsG()
            : base(
                Path.Combine(Utilities.AssemblyDirectory,
                             string.Format(@"Settings/YBMoP/General-Settings-{0}-Rev{1}.xml", StyxWoW.Me.Name, YBMain.Revision)))
        {
        }

        [Setting, DefaultValue(false)]
        public bool CheckAdvancedLogging { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckDisableClickToMove { get; set; }
        [Setting, DefaultValue(true)]
        public bool CheckReshapeLifeInterrupts { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckTreePerformance { get; set; }
        [Setting, DefaultValue(true)]
        public bool CheckUpdates { get; set; }
        [Setting, DefaultValue(true)]
        public bool EnableWoWChatOutput { get; set; }

        [Setting, DefaultValue(250)]
        public int NumAdvLogThrottleTime { get; set; }

        [Setting, DefaultValue(Keys.None)]
        public Keys HeroicLeapChoice { get; set; }
        [Setting, DefaultValue(Keys.None)]
        public Keys DemoBannerChoice { get; set; }

        [Setting, DefaultValue(YBEnum.Mode.Auto)]
        public YBEnum.Mode ModeChoice { get; set; }
        [Setting, DefaultValue(ModifierKeys.Alt)]
        public ModifierKeys ModKeyChoice { get; set; }
        [Setting, DefaultValue(Keys.None)]
        public Keys CooldownKeyChoice { get; set; }
        [Setting, DefaultValue(Keys.None)]
        public Keys SwitchKeyChoice { get; set; }
        [Setting, DefaultValue(Keys.None)]
        public Keys SpecialKeyChoice { get; set; }
        [Setting, DefaultValue(Keys.None)]
        public Keys PauseKeyChoice { get; set; }
    }
}
