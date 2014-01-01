using Xiaolin.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace Xiaolin.Interfaces.Settings
{
    internal class XISettingsWW : Styx.Helpers.Settings
    {

        public XISettingsWW()
            : base(XISettings.SettingsPath + "_Windwalker.xml")
        {
        }


        #region Ability Options
       
        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.Always)]
        [Category("Windwalker - Ability Options")]
        [DisplayName("Energizing Brew")]
        [Description("Select the usage of Energizing Brew.")]
        public XIEnum.AbilityTrigger EnergizingBrew { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.Always)]
        [Category("Windwalker - Ability Options")]
        [DisplayName("Tigereye Brew")]
        [Description("Select the usage of Tigereye Brew.")]
        public XIEnum.AbilityTrigger TigereyeBrew { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.Always)]
        [Category("Windwalker - Ability Options")]
        [DisplayName("Chi Brew")]
        [Description("Select the usage of Chi Brew")]
        public XIEnum.AbilityTrigger ChiBrew { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.Always)]
        [Category("Windwalker - Ability Options")]
        [DisplayName("Xuen")]
        [Description("Select the usage of Xuen")]
        public XIEnum.AbilityTrigger Xuen { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Windwalker - Ability Options")]
        [DisplayName("Touch of Death")]
        [Description("Select the usage of Touch of Death. Default is Off")]
        public bool TouchOfDeath { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Windwalker - Ability Options")]
        [DisplayName("Rushing Jade Wind Single Target")]
        [Description("Select the usage of Rushing Jade Wind Single Target")]
        public bool EnableRJWSingleTarget { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.OnBossDummy)]
        [Category("Windwalker - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public XIEnum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Windwalker - Item Options")]
        [DisplayName("Expel Harm %")]
        [Description("Select the use-on HP for Expel Harm usage.")]
        public int ExpelHarmHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Windwalker - Item Options")]
        [DisplayName("Fortifying Brew  %")]
        [Description("Select the use-on HP for Fortifying Brew usage.")]
        public int FortifyingBrewHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Windwalker - Item Options")]
        [DisplayName("Touch of Karma %")]
        [Description("Select the use-on HP for Touch of Karma usage.")]
        public int TouchofKarmaHP { get; set; }



        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.OnBossDummy)]
        [Category("Windwalker - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public XIEnum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Windwalker - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Windwalker - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.OnBossDummy)]
        [Category("Windwalker - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public XIEnum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.OnBossDummy)]
        [Category("Windwalker - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public XIEnum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Windwalker - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Windwalker - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Windwalker - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Windwalker - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Windwalker - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Windwalker - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
