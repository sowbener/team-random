﻿using Bubbleman.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace Bubbleman.Interfaces.Settings
{
    internal class BMSettingsRE : Styx.Helpers.Settings
    {

        public BMSettingsRE()
            : base(BMSettings.SettingsPath + "_Retribution.xml")
        {
        }


        #region Ability Options
       
        [Styx.Helpers.DefaultValue(BMEnum.AbilityTrigger.Always)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Energizing Brew")]
        [Description("Select the usage of Energizing Brew.")]
        public BMEnum.AbilityTrigger EnergizingBrew { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(BMEnum.AbilityTrigger.Always)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Tigereye Brew")]
        [Description("Select the usage of Tigereye Brew.")]
        public BMEnum.AbilityTrigger TigereyeBrew { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(BMEnum.AbilityTrigger.Always)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Chi Brew")]
        [Description("Select the usage of Chi Brew")]
        public BMEnum.AbilityTrigger ChiBrew { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(BMEnum.AbilityTrigger.Always)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Xuen")]
        [Description("Select the usage of Xuen")]
        public BMEnum.AbilityTrigger Xuen { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Rushing Jade Wind Single Target")]
        [Description("Select the usage of Rushing Jade Wind Single Target")]
        public bool EnableRJWSingleTarget { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(BMEnum.AbilityTrigger.OnBossDummy)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public BMEnum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Retribution - Item Options")]
        [DisplayName("Expel Harm %")]
        [Description("Select the use-on HP for Expel Harm usage.")]
        public int ExpelHarmHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Retribution - Item Options")]
        [DisplayName("Fortifying Brew  %")]
        [Description("Select the use-on HP for Fortifying Brew usage.")]
        public int FortifyingBrewHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Retribution - Item Options")]
        [DisplayName("Touch of Karma %")]
        [Description("Select the use-on HP for Touch of Karma usage.")]
        public int TouchofKarmaHP { get; set; }



        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(BMEnum.AbilityTrigger.OnBossDummy)]
        [Category("Retribution - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public BMEnum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Retribution - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Retribution - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(BMEnum.AbilityTrigger.OnBossDummy)]
        [Category("Retribution - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public BMEnum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(BMEnum.AbilityTrigger.OnBossDummy)]
        [Category("Retribution - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public BMEnum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Retribution - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Retribution - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Retribution - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Retribution - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Retribution - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Retribution - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}