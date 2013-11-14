using Xiaolin.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace Xiaolin.Interfaces.Settings
{
    internal class XISettingsBM : Styx.Helpers.Settings
    {

        public XISettingsBM()
            : base(XISettings.SettingsPath + "_Brewmaster.xml")
        {
        }

        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Set Black Ox Statue")]
        public bool SummonBlackOxStatue { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Use Elusive Brew")]
        public bool UseElusiveBrew { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(6)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Shuffle Timer")]
        [Description("Changes the timer to use Shuffle")]
        public int ShuffleSetting { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(90)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Guard HP Percent")]
        [Description("Changes the HP Percent to use Guard")]
        public int GuardHPPercent { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(50)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Dampen Harm HP Percent")]
        [Description("Changes the HP Percent to use Dampen Harm")]
        public int DampenHarmPercent { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Fortifying Brew HP %")]
        [Description("Changes the HP % to use Fortifying Brew")]
        public int FortifyingBrewPercent { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(30)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Zen Meditation HP %")]
        [Description("Changes the HP % to use Zen Meditation")]
        public int ZenMeditationPercent { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Elusive Brew Count")]
        [Description("Uses Elusive Brew at this Count")]
        public int ElusiveBrew { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(50)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Elusive Brew HP %")]
        [Description("Changes the HP % to use Elusive Brew")]
        public int ElusiveBrewHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Zen Sphere")]
        [Description("Changes the Zen Sphere")]
        public int BrewWithZenSphere { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(99)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Chi Wave HP %")]
        [Description("Changes the HP % to use Chi Wave")]
        public int ChiWavePercent { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(4)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Spinning Crane AoE Count")]
        [Description("Changes the Count to use Spinning Crane Kick")]
        public int SpinningCraneKickCount { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(6)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Healing Sphere HP %")]
        [Description("Changes the HP % to use Healing Sphere")]
        public int HealingSpherePercent { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.OnBossDummy)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public XIEnum.AbilityTrigger ClassRacials { get; set; }


        
        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.OnBossDummy)]
        [Category("Unholy - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public XIEnum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Unholy - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Unholy - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.OnBossDummy)]
        [Category("Unholy - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public XIEnum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(XIEnum.AbilityTrigger.OnBossDummy)]
        [Category("Unholy - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public XIEnum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Unholy - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Unholy - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

       


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Unholy - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Unholy - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Unholy - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Unholy - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
