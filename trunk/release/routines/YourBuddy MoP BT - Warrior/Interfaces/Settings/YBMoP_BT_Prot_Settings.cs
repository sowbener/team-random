using Styx;
using Styx.Common;
using Styx.Helpers;
using System.IO;
using YBMoP_BT_Warrior.Helpers;

namespace YBMoP_BT_Warrior.Interfaces.Settings
{
    internal class YBSettingsP : Styx.Helpers.Settings
    {
        public static readonly YBSettingsP Instance = new YBSettingsP();

        public YBSettingsP()
            : base(
                Path.Combine(Utilities.AssemblyDirectory,
                             string.Format(@"Settings/YBMoP/Prot-Settings-{0}-Rev{1}.xml", StyxWoW.Me.Name, YBMain.Revision)))
        {
        }

        [Setting, DefaultValue(YBEnum.AbilityTrigger.Always)]
        public YBEnum.AbilityTrigger BerserkerRage { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.Always)]
        public YBEnum.AbilityTrigger ClassRacials { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.Never)]
        public YBEnum.AbilityTrigger DemoralizeShout { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.OnBossDummy)]
        public YBEnum.AbilityTrigger Recklessness { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.OnBossDummy)]
        public YBEnum.AbilityTrigger SkullBanner { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.Never)]
        public YBEnum.AbilityTrigger ShatteringThrow { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.Always)]
        public YBEnum.AbilityTrigger Tier4Abilities { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.OnBossDummy)]
        public YBEnum.AbilityTrigger Tier6Abilities { get; set; }
        [Setting, DefaultValue(YBEnum.VcTrigger.Always)]
        public YBEnum.VcTrigger VictoryRush { get; set; }
        [Setting, DefaultValue(YBEnum.Shouts.Commanding)]
        public YBEnum.Shouts ShoutSelector { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.OnBossDummy)]
        public YBEnum.AbilityTrigger UseHands { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.OnBossDummy)]
        public YBEnum.AbilityTrigger Trinket1 { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.OnBossDummy)]
        public YBEnum.AbilityTrigger Trinket2 { get; set; }

        [Setting, DefaultValue(10)]
        public int NumDemoBanner { get; set; }
        [Setting, DefaultValue(10)]
        public int NumEnragedRegen { get; set; }
        [Setting, DefaultValue(10)]
        public int NumHealthStone { get; set; }
        [Setting, DefaultValue(10)]
        public int NumLastStand { get; set; }
        [Setting, DefaultValue(10)]
        public int NumRallyingCry { get; set; }
        [Setting, DefaultValue(10)]
        public int NumShieldWall { get; set; }

        [Setting, DefaultValue(false)]
        public bool CheckDemoBanner { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckEnragedRegen { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckHealthStone { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckLastStand { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckRallyingCry { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckShieldWall { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckAutoAttack { get; set; }
        [Setting, DefaultValue(true)]
        public bool CheckInterrupts { get; set; }
        [Setting, DefaultValue(true)]
        public bool CheckAoE { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckShieldBlock { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckSpellReflect { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckTaunting { get; set; } 
    }
}
