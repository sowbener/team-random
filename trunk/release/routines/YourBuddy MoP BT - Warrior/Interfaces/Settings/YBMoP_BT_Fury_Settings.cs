using Styx;
using Styx.Common;
using Styx.Helpers;
using System.IO;
using YBMoP_BT_Warrior.Helpers;

namespace YBMoP_BT_Warrior.Interfaces.Settings
{
    internal class YBSettingsF : Styx.Helpers.Settings
    {
        public static readonly YBSettingsF Instance = new YBSettingsF();

        public YBSettingsF()
            : base(
                Path.Combine(Utilities.AssemblyDirectory,
                             string.Format(@"Settings/YBMoP/Fury-Settings-{0}-Rev{1}.xml", StyxWoW.Me.Name, YBMain.Revision)))
        {
        }

        [Setting, DefaultValue(YBEnum.AbilityTrigger.Always)]
        public YBEnum.AbilityTrigger BerserkerRage { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.Always)]
        public YBEnum.AbilityTrigger ClassRacials { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.OnBossDummy)]
        public YBEnum.AbilityTrigger Recklessness { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.OnBossDummy)]
        public YBEnum.AbilityTrigger SkullBanner { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.Always)]
        public YBEnum.AbilityTrigger Tier4Abilities { get; set; }
        [Setting, DefaultValue(YBEnum.AbilityTrigger.OnBossDummy)]
        public YBEnum.AbilityTrigger Tier6Abilities { get; set; }
        [Setting, DefaultValue(YBEnum.Shouts.Battle)]
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
        public int NumDiebytheSword { get; set; }
        [Setting, DefaultValue(10)]
        public int NumEnragedRegen { get; set; }
        [Setting, DefaultValue(10)]
        public int NumHealthStone { get; set; }
        [Setting, DefaultValue(10)]
        public int NumImpVic { get; set; }
        [Setting, DefaultValue(10)]
        public int NumRallyingCry { get; set; }
        [Setting, DefaultValue(10)]
        public int NumLifeblood { get; set; }

        [Setting, DefaultValue(true)]
        public bool CheckAutoAttack { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckInterrupts { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckDemoBanner { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckDiebytheSword { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckEnragedRegen { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckHealthStone { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckImpVic { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckRallyingCry { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckLifeblood { get; set; }
        [Setting, DefaultValue(true)]
        public bool CheckHeroicThrow { get; set; }
        [Setting, DefaultValue(true)]
        public bool CheckAoE { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckShatteringThrow { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckRotImpVic { get; set; }
    }
}
