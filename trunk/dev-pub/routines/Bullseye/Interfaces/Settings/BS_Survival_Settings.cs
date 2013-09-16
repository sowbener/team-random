using Bullseye.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace Bullseye.Interfaces.Settings
{
    internal class BsSettingsSV : Styx.Helpers.Settings
    {

        public BsSettingsSV()
            : base(BsSettings.SettingsPath + "_Survival.xml")
        {
        }


        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(BsEnum.AbilityTrigger.Always)]
        [Category("Survival - Ability Options")]
        [DisplayName("A Murder of Crows")]
        [Description("Select the usage of A Murder of Crows.")]
        public BsEnum.AbilityTrigger MurderofCrows { get; set; }
       
        [Setting]
        [Styx.Helpers.DefaultValue(BsEnum.AbilityTrigger.Always)]
        [Category("Survival - Ability Options")]
        [DisplayName("Lynx Rush")]
        [Description("Select the usage of Lynx Rush.")]
        public BsEnum.AbilityTrigger LynxRush  { get; set; }
       
        [Setting]
        [Styx.Helpers.DefaultValue(BsEnum.AbilityTrigger.Always)]
        [Category("Survival - Ability Options")]
        [DisplayName("Rapid Fire")]
        [Description("Select the usage of Rapid Fire")]
        public BsEnum.AbilityTrigger RapidFire { get; set; }
       
        [Setting]
        [Styx.Helpers.DefaultValue(BsEnum.AbilityTrigger.Always)]
        [Category("Survival - Ability Options")]
        [DisplayName("Stampede")]
        [Description("Select the usage of Stampede.")]
        public BsEnum.AbilityTrigger Stampede { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(BsEnum.AbilityTrigger.OnBossDummy)]
        [Category("Frost - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public BsEnum.AbilityTrigger ClassRacials { get; set; }


        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(BsEnum.AbilityTrigger.OnBossDummy)]
        [Category("Frost - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public BsEnum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Frost - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Frost - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(BsEnum.AbilityTrigger.OnBossDummy)]
        [Category("Frost - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public BsEnum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(BsEnum.AbilityTrigger.OnBossDummy)]
        [Category("Frost - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public BsEnum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable AMZ")]
        [Description("Enables AMZ usage")]
        public bool CheckAMZ { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
