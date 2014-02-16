using YourBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourBuddy.Interfaces.Settings
{
    internal class YbSettingsSV : Styx.Helpers.Settings
    {

        public YbSettingsSV()
            : base(InternalSettings.SettingsPath + "_Survival.xml")
        {
        }


        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Survival - Ability Options")]
        [DisplayName("A Murder of Crows")]
        [Description("Select the usage of A Murder of Crows.")]
        public Enum.AbilityTrigger MurderofCrows { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.Traps.ExplosiveTrap)]
        [Category("Survival - Item Options")]
        [DisplayName("Select Trap to Use")]
        [Description("Select which trap to use auto.")]
        public Enum.Traps TrapSwitch { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Survival - Item Options")]
        [DisplayName("Enable / Disable Trap usage")]
        [Description("Enable / Disable Trap usage Auto")]
        public bool EnableTraps { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Survival - Item Options")]
        [DisplayName("Enable / Disable Facing Auto")]
        [Description("Enable / Disable Facing Auto")]
        public bool EnableFacing { get; set; }

       
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Survival - Ability Options")]
        [DisplayName("Lynx Rush")]
        [Description("Select the usage of Lynx Rush.")]
        public Enum.AbilityTrigger LynxRush  { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Survival - Ability Options")]
        [DisplayName("Rabid")]
        [Description("Select the usage of Rabid")]
        public Enum.AbilityTrigger Rabid { get; set; }
       
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Survival - Ability Options")]
        [DisplayName("Rapid Fire")]
        [Description("Select the usage of Rapid Fire")]
        public Enum.AbilityTrigger RapidFire { get; set; }
       
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Survival - Ability Options")]
        [DisplayName("Stampede")]
        [Description("Select the usage of Stampede.")]
        public Enum.AbilityTrigger Stampede { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Survival - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Survival - Ability Options")]
        [DisplayName("Use Black Arrow on Focus Unit Only")]
        [Description("Select the usage of Focus Target only for Black Arrow (adds)")]
        public bool UseBlackArrowFocusTarget { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Survival - Selectable Options")]
        [DisplayName("Enable Pet Stuff")]
        [Description("Enables Pet Stuff (Auto Mend Pet)")]
        public bool EnablePetStuff { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(2)]
        [Category("Survival - Ability Options")]
        [DisplayName("Cleave Number")]
        [Description("Select amount of units to use the cleave rotation for 3-4 targets")]
        public int AoEMultiShotCount { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(2)]
        [Category("Survival - Ability Options")]
        [DisplayName("AoE Number")]
        [Description("Select amount of units to use the AoE Rotation")]
        public int AoECount { get; set; }


        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Survival - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Survival - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Survival - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Survival - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Survival - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Survival - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Survival - Selectable Options")]
        [DisplayName("NOT IN USE")]
        [Description("DO NOT ENABLE YET")]
        public bool CheckAMZ { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Survival - Selectable Options")]
        [DisplayName("Enable Call Pet")]
        [Description("Enables Call Pet")]
        public bool EnableCallPet { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(Enum.CallPet.Pet1)]
        [Category("Survival - Selectable Options")]
        [DisplayName("Which Pet To Call?")]
        [Description("Select the Pet you want to cast.")]
        public Enum.CallPet CallPet { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Survival - Selectable Options")]
        [DisplayName("Enable Revive Pet")]
        [Description("Enables Auto Revive Pet")]
        public bool EnableRevivePet { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Survival - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Survival - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Survival - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Survival - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Survival - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
