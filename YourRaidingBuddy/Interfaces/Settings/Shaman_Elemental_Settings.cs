using YourRaidingBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourRaidingBuddy.Interfaces.Settings
{
    internal class YbSettingsES: Styx.Helpers.Settings
    {

        public YbSettingsES()
            : base(InternalSettings.SettingsPath + "_Elemental.xml")
        {
        }

        #region Ability Options

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.WeaponImbueM.Flametongue)]
        [Category("Elemental - Ability Options")]
        [DisplayName("Buff Main-Hand")]
        [Description("Select which buff you want on your main-hand.")]
        public Enum.WeaponImbueM WeaponImbueSelectorM { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Elemental - Ability Options")]
        [DisplayName("Prebuff Lightning-Shield")]
        [Description("Select the usage of Lightning-Shield")]
        public bool PrebuffPet { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Elemental - Ability Options")]
        [DisplayName("Storm Lash Totem")]
        [Description("Enable to use Storm Lash Totem on Bloodlust/Heroism similar effects.")]
        public bool EnableStormLashTotem { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Elemental - Ability Options")]
        [DisplayName("Flame Shock Refresh Before Ascendance")]
        [Description("Enable/Disable Refresh Flame Shock Before Ascendance.")]
        public bool UseFlameShockRefreshAscendance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Elemental - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Elemental - Ability Options")]
        [DisplayName("Ascendance")]
        [Description("Select the usage of Ascendance.")]
        public Enum.AbilityTrigger Ascendance { get; set; }
      
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Elemental - Ability Options")]
        [DisplayName("Ancestral Swiftness")]
        [Description("Select the usage of Ancestral Swiftness")]
        public Enum.AbilityTrigger AncestralSwiftness { get; set; }
       
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Elemental - Ability Options")]
        [DisplayName("Elemental Mastery")]
        [Description("Select the usage of Elemental Mastery")]
        public Enum.AbilityTrigger ElementalMastery { get; set; }
       
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Elemental - Ability Options")]
        [DisplayName("Fire Elemental Totem")]
        [Description("Select the usage of Fire Elemental.")]
        public Enum.AbilityTrigger FireElemental { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Elemental - Ability Options")]
        [DisplayName("Earth Elemental Totem")]
        [Description("Select the usage of Earth Elemental.")]
        public Enum.AbilityTrigger EarthElemental { get; set; }

        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Elemental - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Elemental - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Elemental - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Elemental - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Elemental - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region DefensiveCooldownsElemental

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Enable Selfhealing")]
        [Description("Enable Auto use of Selfhealing.")]
        public bool EnableSelfHealing { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Enable Auto SpiritWalk")]
        [Description("Enable Auto use of Spirit Walking.")]
        public bool UseSpiritWalkAuto { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Enable Shamanistic Rage")]
        [Description("Enable Shamanistic Rage during Self-healing Enabled.")]
        public bool EnableShamanisticRage { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Enable AncestralGuidance")]
        [Description("Enable Ancestral Guidance during Self-healing Enabled.")]
        public bool EnableAncestralGuidance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Enable Stone-Bulwark Totem")]
        [Description("Enable Stone-Bulwark Totem during Self-healing Enabled.")]
        public bool EnableStoneBulwarkTotem { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Enable Gift of The NaarU.")]
        [Description("Enable Gift of The Naaru during Self-healing Enabled.")]
        public bool EnableGiftOfTheNaaru { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Enable Healing-Stream Totem")]
        [Description("Enable Healing-Stream Totem during Self-healing Enabled.")]
        public bool EnableHealingStreamTotem { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(70)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Shamanistic Rage %")]
        [Description("Will use Shamanistic Rage when health % is less than or equal to the set value.")]
        public int ShamanisticRageHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(50)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Healing-Stream Totem %")]
        [Description("Will use Healing-Stream Totem when health % is less than or equal to the set value.")]
        public int HealingStreamHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Ascestral Guidance %")]
        [Description("Will use Ancestral Guidance when health % is less than or equal to the set value.")]
        public int AncestralGuidanceHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Stone Bulwark Totem %")]
        [Description("Will use Stone Bulwark Totem when health % is less than or equal to the set value.")]
        public int StoneBulwarkTotemHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Gift of The Naaru %")]
        [Description("Will use Gift of The Naaru when health % is less than or equal to the set value.")]
        public int GiftOfTheNaaruHP { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(80)]
        [Category("Elemental - Defensive Options")]
        [DisplayName("Healing Surge %")]
        [Description("Will use Healing Surge when health % is less than or equal to the set value.")]
        public int HealingSurgeHP { get; set; }

        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Elemental - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Elemental - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Elemental - Selectable Options")]
        [DisplayName("Enable PvP Rotation")]
        [Description("Enables the Elemental PvP Rotation")]
        public bool PvPRotationCheck { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Elemental - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Elemental - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Elemental - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Elemental - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
