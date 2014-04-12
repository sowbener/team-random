using YourRaidingBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourRaidingBuddy.Interfaces.Settings
{
    internal class YbSettingsUD : Styx.Helpers.Settings
    {

        public YbSettingsUD()
            : base(InternalSettings.SettingsPath + "_Unholy.xml")
        {
        }

        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Unholy - Ability Options")]
        [DisplayName("Summon Gargoyle")]
        [Description("Select the usage of Gargoyle.")]
        public Enum.AbilityTrigger SummonGargoyle { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Unholy - Ability Options")]
        [DisplayName("Summon Gargoyle AoE")]
        [Description("Select the usage of Gargoyle in AoE.")]
        public bool UseGargoyleInAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Unholy - Ability Options")]
        [DisplayName("Override Dots with More Powerful ones")]
        [Description("Select the usage of Override Dots With More Powerful Ones")]
        public bool EnablePowerDots { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Unholy - Ability Options")]
        [DisplayName("Raise Dead Pet Enable")]
        [Description("Select the usage of Auto-Raise Dead")]
        public bool PrebuffPet { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Unholy - Ability Options")]
        [DisplayName("Empower Rune Weapon")]
        [Description("Select the usage of Empower Rune Weapon.")]
        public Enum.AbilityTrigger EmpowerRuneWeapon { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Unholy - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Unholy - Ability Options")]
        [DisplayName("Unholy Frenzy")]
        [Description("Select the usage of Unholy Frenzy.")]
        public Enum.AbilityTrigger UnholyFrenzy { get; set; }
        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Unholy - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

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
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Unholy - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Unholy - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }
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
        [Styx.Helpers.DefaultValue(35)]
        [Category("Unholy - Selectable Options")]
        [DisplayName("Soul Reaper %")]
        [Description("Select the use-on HP for Soul Reaper usage.")]
        public int SoulReaperHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(3000)]
        [Category("Unholy - Selectable Options")]
        [DisplayName("Attack Power Required")]
        [Description("Attack Power Required to Override CurrentDot.")]
        public int AttackPowerDot { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(80)]
        [Category("Unholy - Selectable Options")]
        [DisplayName("Death Siphon %")]
        [Description("Uses Death Siphon if specced at % of HP")]
        public int DeathSiphonHP { get; set; }
       


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
