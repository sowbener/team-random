using YourBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourBuddy.Interfaces.Settings
{
    internal class YbSettingsRE : Styx.Helpers.Settings
    {

        public YbSettingsRE()
            : base(InternalSettings.SettingsPath + "_Retribution.xml")
        {
        }


        #region Ability Options
       
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Holy Avenger")]
        [Description("Select the usage of Holy Avenger")]
        public Enum.AbilityTrigger HolyAvenger { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Guardian of Ancient Kings")]
        [Description("Select the usage of Guardian of Ancient Kings")]
        public Enum.AbilityTrigger GuardianofAnicentKings { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Avenging Wrath")]
        [Description("Select the usage of Avenging Wrath")]
        public Enum.AbilityTrigger AvengingWrath { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(4)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Seal of Righteousness Count")]
        [Description("Select the use count for Righteousness Seal")]
        public int SealofRighteousnessCount { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Seal Swapping")]
        [Description("Enable / Disable Seal Swapping")]
        public bool UseSealSwapping { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Light's Hammer Hotkey Enable")]
        [Description("Checked enables Light Hammer on Hotkey")]
        public bool UseLightsHammerHotkey { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.TriggerTarget.OnMe)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Light Hammer Location")]
        [Description("Select the usage of Light's Hammer")]
        public Enum.TriggerTarget LightHammerLocation { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Fist of Justice Enable")]
        [Description("Checked enables Fist of Justice Hotkey")]
        public bool EnableFistofJustice { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Retribution - Ability Options")]
        [DisplayName("Word of Glory Enable")]
        [Description("Checked enables Word of Glory")]
        public bool EnableWordofGlory { get; set; }
        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Retribution - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

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
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Retribution - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Retribution - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }
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
