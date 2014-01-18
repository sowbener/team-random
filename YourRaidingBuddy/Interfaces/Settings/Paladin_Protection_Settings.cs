using YourBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourBuddy.Interfaces.Settings
{
    internal class YbSettingsPR : Styx.Helpers.Settings
    {

        public YbSettingsPR()
            : base(InternalSettings.SettingsPath + "_Protection.xml")
        {
        }

        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Protection - Ability Options")]
        [DisplayName("Avenging Wrath")]
        [Description("Select the usage of your Avenging Wrath Cooldown to be used at.")]
        public Enum.AbilityTrigger AvengingWrath { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(55)]
        [Category("Protection - Ability Options")]
        [DisplayName("Guardian of Ancient Kings % HP")]
        [Description("Use Guardian of Ancient Kings at % HP")]
        public int GuardianofAncientKingsHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Ability Options")]
        [DisplayName("Guardian of Ancient Kings Enable")]
        [Description("Checked enables Guardian of Anicent Kings usage.")]
        public bool GuardianofAncientKingsEnable { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(55)]
        [Category("Protection - Ability Options")]
        [DisplayName("Ardent Defender % HP")]
        [Description("Use Ardent Defender at % HP")]
        public int ArdentDefenderHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Ability Options")]
        [DisplayName("Ardent Defender Enable")]
        [Description("Checked enables Ardent Defender usage.")]
        public bool ArdentDefenderEnable { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Protection - Ability Options")]
        [DisplayName("Lay on Hands % HP")]
        [Description("Use Lay on Hands at % HP")]
        public int LayonHandsHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Ability Options")]
        [DisplayName("Lay on Hands Enable")]
        [Description("Checked enables Lay on Hands usage.")]
        public bool LayonHandsEnable { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Protection - Ability Options")]
        [DisplayName("Divine Protection % HP")]
        [Description("Use Divine Protection at % HP")]
        public int DivineProtectionHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Ability Options")]
        [DisplayName("Divine Protection Enable")]
        [Description("Checked enables Divine Protection usage.")]
        public bool DivineProtectionEnable { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Protection - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }


        
        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Protection - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Protection - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Protection - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Protection - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

       
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Protection - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
