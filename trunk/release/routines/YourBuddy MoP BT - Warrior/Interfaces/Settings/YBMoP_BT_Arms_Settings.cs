using Styx;
using Styx.Common;
using Styx.Helpers;
using System.IO;
using YBMoP_BT_Warrior.Helpers;

namespace YBMoP_BT_Warrior.Interfaces.Settings
{
    internal class YBSettingsA : Styx.Helpers.Settings
    {
        public static readonly YBSettingsA Instance = new YBSettingsA();

        public YBSettingsA()
            : base(
                Path.Combine(Utilities.AssemblyDirectory,
                             string.Format(@"Settings/YBMoP/Arms-Settings-{0}-Rev{1}.xml", StyxWoW.Me.Name, YBMain.Revision)))
        {
        }

        [Setting, DefaultValue(true)]
        public bool CheckAutoAttack { get; set; }
        [Setting, DefaultValue(true)]
        public bool CheckAoE { get; set; }
        [Setting, DefaultValue(true)]
        public bool CheckInterrupts { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckShatteringThrow { get; set; }

    }
}
