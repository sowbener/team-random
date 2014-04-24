using YourRaidingBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourRaidingBuddy.Interfaces.Settings
{
    internal class YbSettingsDB : Styx.Helpers.Settings
    {

        public YbSettingsDB()
            : base(InternalSettings.SettingsPath + "_Boomkin.xml")
        {
        }


        #region Ability Options
       
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Boomkin - Ability Options")]
        [DisplayName("Celestial Alignment")]
        [Description("Select the usage of Celestial Alignment.")]
        public Enum.AbilityTrigger Celestial { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Boomkin - Ability Options")]
        [DisplayName("Incarnation: Chosen of Elune")]
        [Description("Select the usage of Incarnation: Chosen of Elune.")]
        public Enum.AbilityTrigger Incarnation { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Boomkin - Ability Options")]
        [DisplayName("Force of Nature")]
        [Description("Select the usage of Force of Nature")]
        public Enum.AbilityTrigger ForceofNature { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Boomkin - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(2)]
        [Category("Boomkin - Ability Options")]
        [DisplayName("AoE Number")]
        [Description("Select amount of units to use the AoE Rotation")]
        public int AoECount { get; set; }


        #endregion

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Boomkin - Ability Options")]
        [DisplayName("Enable or Disable Facing")]
        [Description("This enables Facing")]
        public bool EnableFacing { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Healing")]
        [DisplayName("Rejuvenation")]
        [Description("Enable / Disable Rejuvenation")]
        public bool EnableRejuvenation { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Healing")]
        [DisplayName("Healing Touch")]
        [Description("Enable / Disable Healing Touch")]
        public bool EnableHealingTouch{ get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Healing")]
        [DisplayName("Nature's Swiftness")]
        [Description("Enable / Disable Nature's Swiftness")]
        public bool EnableNatureSwiftness { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Healing")]
        [DisplayName("Innervate")]
        [Description("Enable / Disable Innervate")]
        public bool EnableInnervate { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Healing")]
        [DisplayName("Barkskin")]
        [Description("Enable / Disable Innervate")]
        public bool EnableBarkskin { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Healing")]
        [DisplayName("Barkskin HP %")]
        [Description("Chooose which HP in % to use Barkskin. Default is 40%")]
        public int BarkskinHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Healing")]
        [DisplayName("Innervate MP %")]
        [Description("Chooose which MP in % to use Innervate. Default is 20%")]
        public int InnervateMP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Healing")]
        [DisplayName("Healing Touch HP %")]
        [Description("Chooose which HP in % to use Healing Touch. Default is 30%")]
        public int HealingTouchHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Healing")]
        [DisplayName("Rejuvenation HP %")]
        [Description("Chooose which HP in % to use Rejuvenation. Default is 60%")]
        public int RejuvenationHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Healing")]
        [DisplayName("Nature's Swiftness HP %")]
        [Description("Chooose which HP in % to use Nature's Swiftness. Default is 20%")]
        public int NatureSwiftnessHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.TriggerTarget.FocusTarget)]
        [Category("Boomkin - Ability Options")]
        [DisplayName("Rebirth")]
        [Description("Select the usage of Rebirth")]
        public Enum.TriggerTarget RebrithLogic { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.EclipseType.Solar)]
        [Category("Boomkin - Ability Options")]
        [DisplayName("Eclipse Type PreCombat")]
        [Description("Select which Eclipse Type you want PreBuff")]
        public Enum.EclipseType WhichEclipse { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Boomkin - Ability Options")]
        [DisplayName("Only change Eclipse Precombat if none")]
        [Description("Only change Eclipse Precombat if none")]
        public bool OnlyChangeIfNoEclipse { get; set; }

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Boomkin - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Boomkin - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Boomkin - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Boomkin - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Boomkin - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Boomkin - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Boomkin - Selectable Options")]
        [DisplayName("NOT IN USE")]
        [Description("NOT IN USE")]
        public bool CheckAMZ { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Boomkin - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }



        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Boomkin - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Boomkin - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Boomkin - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Boomkin - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
