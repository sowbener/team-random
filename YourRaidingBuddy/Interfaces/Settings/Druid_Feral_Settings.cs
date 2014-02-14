using YourBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourBuddy.Interfaces.Settings
{
    internal class YbSettingsDF : Styx.Helpers.Settings
    {

        public YbSettingsDF()
            : base(InternalSettings.SettingsPath + "_Feral.xml")
        {
        }


        #region Ability Options
       
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Feral - Ability Options")]
        [DisplayName("xxxxx")]
        [Description("xxxxx")]
        public Enum.AbilityTrigger xxx { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Feral - Ability Options")]
        [DisplayName("Enable Advance Rotation (Rune or Orgination REQUIRED)")]
        [Description("Enables the Rune of Orgination Rotation")]
        public bool RuneofOrginationRotation { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Feral - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Feral - Ability Options")]
        [DisplayName("Enable Tier4 Abilities (AoE)")]
        [Description("Enables Tier4 Abilities to be used on AoE. Default is Enabled")]
        public bool UseTier4AoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Feral - Ability Options")]
        [DisplayName("Enable Auto-Target")]
        [Description("Enables Auto-Target, default is False")]
        public bool AutoTarget { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(2)]
        [Category("Feral - Ability Options")]
        [DisplayName("Cleave Number")]
        [Description("Select amount of units to use the cleave rotation for 3-4 targets")]
        public int AoEMultiShotCount { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(2)]
        [Category("Feral - Ability Options")]
        [DisplayName("AoE Number")]
        [Description("Select amount of units to use the AoE Rotation")]
        public int AoECount { get; set; }


        #endregion

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Feral - Ability Options")]
        [DisplayName("Enable or Disable Trap Usage")]
        [Description("This enables Trap Usage")]
        public bool EnableTraps { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.Traps.ExplosiveTrap)]
        [Category("Feral - Ability Options")]
        [DisplayName("Select Trap to Use")]
        [Description("Select which trap to use auto.")]
        public Enum.Traps TrapSwitch { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.CallPet.Pet1)]
        [Category("Feral Pet")]
        [DisplayName("Which Pet To Call?")]
        [Description("Select the Pet you want to cast.")]
        public Enum.CallPet CallPet { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Feral Pet")]
        [DisplayName("Mend Pet HP %")]
        [Description("Chooose which HP in % to use Mend Pet. Default is 40%")]
        public int MendPetHP { get; set; }

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Feral - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Feral - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Feral - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Feral - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Feral - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Feral - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Feral - Selectable Options")]
        [DisplayName("NOT IN USE")]
        [Description("NOT IN USE")]
        public bool CheckAMZ { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Feral - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Feral - Selectable Options")]
        [DisplayName("Enable Pet Stuff")]
        [Description("Enables Pet Stuff (Auto Mend Pet)")]
        public bool EnablePetStuff { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Feral - Selectable Options")]
        [DisplayName("Enable Call Pet")]
        [Description("Enables Call Pet")]
        public bool EnableCallPet { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Feral - Selectable Options")]
        [DisplayName("Enable Revive Pet")]
        [Description("Enables Auto Revive Pet")]
        public bool EnableRevivePet { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Feral - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Feral - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Feral - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Feral - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
