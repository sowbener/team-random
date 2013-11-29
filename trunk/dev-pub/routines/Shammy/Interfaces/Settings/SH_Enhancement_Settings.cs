using Shammy.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace Shammy.Interfaces.Settings
{
    internal class SmSettingsF : Styx.Helpers.Settings
    {

        public SmSettingsF()
            : base(SmSettings.SettingsPath + "_Enhancement.xml")
        {
        }


        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.AbilityTrigger.Always)]
        [Category("Enhancement - Ability Options")]
        [DisplayName("Ascendance")]
        [Description("Select the usage of Ascendance.")]
        public SmEnum.AbilityTrigger Ascendance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.AbilityTrigger.Always)]
        [Category("Enhancement - Ability Options")]
        [DisplayName("Fire Elemental")]
        [Description("Select the usage of Fire Elemental Totem.")]
        public SmEnum.AbilityTrigger FireElemental { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.AbilityTrigger.Always)]
        [Category("Enhancement - Ability Options")]
        [DisplayName("Elemental Mastery")]
        [Description("Select the usage of Elemental Mastery")]
        public SmEnum.AbilityTrigger ElementalMastery { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.WeaponImbueM.Windfury)]
        [Category("Enhancement - Ability Options")]
        [DisplayName("Buff Main-Hand")]
        [Description("Select which buff you want on your main-hand.")]
        public SmEnum.WeaponImbueM WeaponImbueSelectorM { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.WeaponImbueO.Flametongue)]
        [Category("Enhancement - Ability Options")]
        [DisplayName("Buff Off-Hand")]
        [Description("Select which buff you want on your Off-hand.")]
        public SmEnum.WeaponImbueO WeaponImbueSelectorO { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.AbilityTrigger.Always)]
        [Category("Enhancement - Ability Options")]
        [DisplayName("Feral Spirit")]
        [Description("Select the usage of Feral Spirit.")]
        public SmEnum.AbilityTrigger FeralSpirit { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.AbilityTrigger.OnBossDummy)]
        [Category("Enhancement - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public SmEnum.AbilityTrigger ClassRacials { get; set; }

        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.AbilityTrigger.OnBossDummy)]
        [Category("Enhancement - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public SmEnum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Enhancement - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Enhancement - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.AbilityTrigger.OnBossDummy)]
        [Category("Enhancement - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public SmEnum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(SmEnum.AbilityTrigger.OnBossDummy)]
        [Category("Enhancement - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public SmEnum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Self-Healing")]
        [Description("Enables SelfHealing and Defensive Cooldowns")]
        public bool EnableSelfHealing { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Stormlash Totem")]
        [Description("Enable Stormlash Totem during Bloodlust or similar.")]
        public bool EnableStormLashTotem { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Auto SpiritWalk")]
        [Description("Enable Auto use of Spirit Walking.")]
        public bool UseSpiritWalkAuto { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Shamanistic Rage")]
        [Description("Enable Shamanistic Rage during Self-healing Enabled.")]
        public bool EnableShamanisticRage { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable AncestralGuidance")]
        [Description("Enable Ancestral Guidance during Self-healing Enabled.")]
        public bool EnableAncestralGuidance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Stone-Bulwark Totem")]
        [Description("Enable Stone-Bulwark Totem during Self-healing Enabled.")]
        public bool EnableStoneBulwarkTotem { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Gift of The Naaru.")]
        [Description("Enable Gift of The Naaru during Self-healing Enabled.")]
        public bool EnableGiftOfTheNaaru { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Healing-Stream Totem")]
        [Description("Enable Healing-Stream Totem during Self-healing Enabled.")]
        public bool EnableHealingStreamTotem { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Healing-Tide Totem")]
        [Description("Enable Healing-Tide Totem during Self-healing Enabled.")]
        public bool EnableHealingTideTotem { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(70)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Shamanistic Rage %")]
        [Description("Will use Shamanistic Rage when health % is less than or equal to the set value.")]
        public int ShamanisticRageHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(50)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Healing-Stream Totem %")]
        [Description("Will use Healing-Stream Totem when health % is less than or equal to the set value.")]
        public int HealingStreamHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(50)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Healing-Tide Totem %")]
        [Description("Will use Healing-Tide Totem when health % is less than or equal to the set value.")]
        public int HealingTideHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Ascestral Guidance %")]
        [Description("Will use Ancestral Guidance when health % is less than or equal to the set value.")]
        public int AncestralGuidanceHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Stone Bulwark Totem %")]
        [Description("Will use Stone Bulwark Totem when health % is less than or equal to the set value.")]
        public int StoneBulwarkTotemHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Gift of The Naaru %")]
        [Description("Will use Gift of The Naaru when health % is less than or equal to the set value.")]
        public int GiftOfTheNaaruHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(80)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Healing Surge %")]
        [Description("Will use Healing Surge when health % is less than or equal to the set value.")]
        public int HealingSurgeHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable PvP Rotation")]
        [Description("Enables the Enhancement PvP Rotation")]
        public bool PvPRotationCheck { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Totem Recall")]
        [Description("Enables Totem Recall if no totems are detected within the range.")]
        public bool RecallTotemsEnable { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Enhancement - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
