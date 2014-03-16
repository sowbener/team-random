using YourRaidingBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Interfaces.Settings
{
    internal class YbSettingsBMM : Styx.Helpers.Settings
    {

        public YbSettingsBMM()
            : base(InternalSettings.SettingsPath + "_Brewmaster.xml")
        {
        }

        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Set Black Ox Statue")]
        public bool SummonBlackOxStatue { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Guard HP Percent")]
        [Description("Changes the HP Percent to use Guard")]
        public int GuardHPPercent { get; set; }

         [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("HP AP Guard")]
        [Description("% of your total HP to be used as AP. Default 10")]
        public int HPAPScale { get; set; }


         [Setting]
         [Styx.Helpers.DefaultValue(10)]
         [Category("Brewmaster - Ability Options")]
         [DisplayName("HP Moderate Scale")]
         [Description("Moderate needs to be higher than % of your total HP before using. Default 10")]
         public int HPModerateScale { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Use RJW on Single Target")]
        [Description("Uses Rushing Jade Wind on Single Target")]
        public bool UseRJWSingleTarget { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Use Purifying Brew on Moderate Stagger")]
        [Description("Uses Purifying Brew on Moderate Stagger. Default is Enabled")]
        public bool PurifyingModerate{ get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Use Purifying Brew on Light Stagger")]
        [Description("Uses Purifying Brew on Light Stagger. Default is Disabled (Enable if you're farming)")]
        public bool PurifyingLight { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Auto-Use Dizzying Haze")]
        [Description("Uses Dizzying Haze")]
        public bool UseDizzyingHaze { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Use Rushing Jade Wind / Spinning Crane Kick")]
        [Description("Enables usage of Rushing Jade Wind or Spinning Crane Kick on AoE")]
        public bool CheckRJW { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(6)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Rushing Jade Wind / Spinning Crane Kick Count")]
        [Description("Changes the amount of units to use RJW or SCK")]
        public int RJWCount { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Use Breath of Fire")]
        public bool CheckBreathofFire { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(80)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Dampen Harm HP Percent")]
        [Description("Changes the HP Percent to use Dampen Harm")]
        public int DampenHarmPercent { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(40)]
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
        [Styx.Helpers.DefaultValue(90)]
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
        [Styx.Helpers.DefaultValue(0)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Healing Sphere HP %")]
        [Description("Changes the HP % to use Healing Sphere")]
        public int HealingSpherePercent { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Brewmaster - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }


        
        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Brewmaster - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Brewmaster - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Brewmaster - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Brewmaster - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

       
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Brewmaster - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Brewmaster - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Brewmaster - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Brewmaster - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
