using Styx;
using Styx.Common;
using Styx.Helpers;
using System.IO;
using YBMoP_BT_Rogue.Helpers;

namespace YBMoP_BT_Rogue.Interfaces.Settings
{
    internal class YBSettingsAss : Styx.Helpers.Settings
    {
        public static readonly YBSettingsAss Instance = new YBSettingsAss();

        public YBSettingsAss()
            : base(
                Path.Combine(Utilities.AssemblyDirectory,
                             string.Format(@"Settings/YBMoP/Ass-Settings-{0}-Rev{1}.xml", StyxWoW.Me.Name, YBMain.Revision)))
        {
        }

        [Setting, DefaultValue(AbilityTrigger.Always)]
        public AbilityTrigger Vendetta { get; set; }
        [Setting, DefaultValue(AbilityTrigger.Always)]
        public AbilityTrigger ClassRacials { get; set; }
        [Setting, DefaultValue(AbilityTrigger.OnBossDummy)]
        public AbilityTrigger ShadowBlades { get; set; }
        [Setting, DefaultValue(AbilityTrigger.OnBossDummy)]
        public AbilityTrigger Vanish { get; set; }
        [Setting, DefaultValue(AbilityTrigger.Always)]
        public AbilityTrigger Tier4Abilities { get; set; }
        [Setting, DefaultValue(AbilityTrigger.OnBossDummy)]
        public AbilityTrigger Tier6Abilities { get; set; }
        [Setting, DefaultValue(PoisonM.Deadly)]
        public PoisonM PoisonSelectorM { get; set; }
        [Setting, DefaultValue(PoisonO.Crippling)]
        public PoisonO PoisonSelectorO { get; set; }
        [Setting, DefaultValue(AbilityTrigger.OnBossDummy)]
        public AbilityTrigger UseHands { get; set; }
        [Setting, DefaultValue(AbilityTrigger.OnBossDummy)]
        public AbilityTrigger Trinket1 { get; set; }
        [Setting, DefaultValue(AbilityTrigger.OnBossDummy)]
        public AbilityTrigger Trinket2 { get; set; }

        [Setting, DefaultValue(10)]
        public int NumShadowstep { get; set; }
        [Setting, DefaultValue(5)]
        public int NumRecuperateCombo { get; set; }
        [Setting, DefaultValue(5)]
        public int NumAoE { get; set; }
        [Setting, DefaultValue(5)]
        public int NumRecuperate { get; set; }
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
        [Setting, DefaultValue(true)]
        public bool CheckPreparation { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckInterrupts { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckShadowstep { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckRecuperate { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckRecuperateCombo { get; set; }
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
        public bool CheckPoison { get; set; }
        [Setting, DefaultValue(true)]
        public bool CheckAoE { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckExposeArmor { get; set; }
        [Setting, DefaultValue(false)]
        public bool CheckPotion { get; set; }
    }
}
