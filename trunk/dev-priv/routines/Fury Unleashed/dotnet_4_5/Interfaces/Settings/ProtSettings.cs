﻿using System.ComponentModel;
using FuryUnleashed.Shared.Helpers;
using Styx.Helpers;

namespace FuryUnleashed.Interfaces.Settings
{
    internal class SettingsP : Styx.Helpers.Settings
    {

        public SettingsP()
            : base(InternalSettings.SettingsPath + "_Prot.xml")
        {
        }

        // ========================================================================================

        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Protection - Ability Options")]
        [DisplayName("Berserker Rage")]
        [Description("Select the usage of Berserker Rage.")]
        public Enum.AbilityTrigger BerserkerRage { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Protection - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Ability Options")]
        [DisplayName("Enable (Mass) Spell Reflection")]
        [Description("Checked enables (Mass) Spell Reflection.")]
        public bool CheckSpellReflect { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Protection - Ability Options")]
        [DisplayName("Recklessness")]
        [Description("Select the usage of Recklessness.")]
        public Enum.AbilityTrigger Recklessness { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Protection - Ability Options")]
        [DisplayName("Skull Banner")]
        [Description("Select the usage of Skull Banner.")]
        public Enum.AbilityTrigger SkullBanner { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Protection - Ability Options")]
        [DisplayName("Tier 4 Abilities")]
        [Description("Select the usage of Tier 4 - Usage of Bladestorm, Dragon Roar or Shockwave.")]
        public Enum.AbilityTrigger Tier4Abilities { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Protection - Ability Options")]
        [DisplayName("Tier 4 Abilities - AoE")]
        [Description("Select the usage of Tier 4 - Usage of Bladestorm, Dragon Roar or Shockwave during AoE.")]
        public Enum.AbilityTrigger Tier4AoeAbilities { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Protection - Ability Options")]
        [DisplayName("Tier 6 Abilities")]
        [Description("Select the usage of Tier 6 - Usage of Avatar, Bloodbath or Storm Bolt.")]
        public Enum.AbilityTrigger Tier6Abilities { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.Shouts.CommandingShout)]
        [Category("Protection - Ability Options")]
        [DisplayName("Warrior Shout Selector")]
        [Description("Choose preferred Warrior Shout.")]
        public Enum.Shouts ShoutSelection { get; set; }
        #endregion

        // ========================================================================================

        #region Defensive Abilities
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Enable Demoralizing Banner")]
        [Description("Checked enables Demoralizing Banner - Does not work when Demo Banner hotkey is set!")]
        public bool CheckDemoBanner { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Demoralizing Banner %")]
        [Description("Select the use-on HP for Demoralizing Banner usage - Does not work when Demo Banner hotkey is set!")]
        public int CheckDemoBannerNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Never)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Demoralizing Shout")]
        [Description("Select the usage of Demoralizing Shout.")]
        public Enum.AbilityTrigger DemoralizeShout { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Demoralizing Shout %")]
        [Description("Select the use-on HP for Demoralizing Shout usage - 100% is always.")]
        public int DemoShoutNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Enable Last Stand")]
        [Description("Checked enables Last Stand.")]
        public bool CheckLastStand { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Last Stand %")]
        [Description("Will use Last Stand when health % is less than or equal to the set value.")]
        public int CheckLastStandNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Enable Rallying Cry")]
        [Description("Checked enables Rallying Cry - Works for Self, Party and Raidmembers.")]
        public bool CheckRallyingCry { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Rallying Cry %")]
        [Description("Will use Rallying Cry when health % is less than or equal to the set value.")]
        public int CheckRallyingCryNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Shield Block/Barrier")]
        [Description("Checked enables Shield Block or Barrier - Use Hotkeys to switch block/barrier.")]
        public bool CheckShieldBlock { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.BarrierBlock.ShieldBlock)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Select Block/Barrier")]
        [Description("Select which ability should be used - Suggested is to use the Hotkey mode (Will ignore this setting at hotkey mode)!")]
        public Enum.BarrierBlock BarrierBlockSelection { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Enable Shield Wall")]
        [Description("Checked enables Shield Wall.")]
        public bool CheckShieldWall { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Protection - Defensive Options")]
        [DisplayName("Shield Wall %")]
        [Description("Will use Shield Wall when health % is less than or equal to the set value.")]
        public int CheckShieldWallNum { get; set; }
        #endregion

        // ========================================================================================

        #region HP Regeneration Options
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Regeneration Options")]
        [DisplayName("Enable Enraged Regeneration")]
        [Description("Checked enables Enraged Regeneration usage.")]
        public bool CheckEnragedRegen { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Protection - Regeneration Options")]
        [DisplayName("Enraged Regeneration %")]
        [Description("Select the use-on HP for Enraged Regentation usage.")]
        public int CheckEnragedRegenNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Regeneration Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Protection - Regeneration Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.VcTrigger.Always)]
        [Category("Protection - Regeneration Options")]
        [DisplayName("Impending Victory")]
        [Description("Select the usage of Impending Victory.")]
        public Enum.VcTrigger ImpendingVictory { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(75)]
        [Category("Protection - Regeneration Options")]
        [DisplayName("Impending Victory %")]
        [Description("Will use Impending Victory when health % is less than or equal to the set value.")]
        public int ImpendingVictoryNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.VcTrigger.Always)]
        [Category("Protection - Regeneration Options")]
        [DisplayName("Victory Rush")]
        [Description("Select the usage of Victory Rush.")]
        public Enum.VcTrigger VictoryRush { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(75)]
        [Category("Protection - Regeneration Options")]
        [DisplayName("Victory Rush %")]
        [Description("Will use Victory Rush when health % is less than or equal to the set value.")]
        public int VictoryRushNum { get; set; }
        #endregion

        // ========================================================================================

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Protection - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Never)]
        [Category("Protection - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Never)]
        [Category("Protection - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(100)]
        [Category("Protection - Item Options")]
        [DisplayName("Use Trinkets below %")]
        [Description("Will use trinkets when health % is less or equal to this value - 100% ignores this setting!")]
        public int CheckTrinketsNum { get; set; }
        #endregion

        // ========================================================================================

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Selectable Options")]
        [DisplayName("Enable Auto Taunt")]
        [Description("Enables automatic taunting if the mobs target isnt you.")]
        public bool CheckAutoTaunt { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(2)]
        [Category("Protection - Selectable Options")]
        [DisplayName("Multi-Target (AoE) Units")]
        [Description("Select the amount of units within 8 yards before AoE kicks in. Recommended is 2!")]
        public int CheckAoENum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Never)]
        [Category("Protection - Ability Options")]
        [DisplayName("Shattering Throw")]
        [Description("Select the usage of Shattering Throw.")]
        public Enum.AbilityTrigger ShatteringThrow { get; set; }
        #endregion

        // ========================================================================================

        #region Interrupts & Stuns
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.Hamstring.AddList)]
        [Category("Protection - Interrupts & Stuns")]
        [DisplayName("Use Hamstring On")]
        [Description("Select the usage of Hamstring - Does not use on bosses.")]
        public Enum.Hamstring HamString { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Interrupts & Stuns")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Disrupting Shout (Single TGT Interrupts).")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Interrupts & Stuns")]
        [DisplayName("Enable Interrupt AoE")]
        [Description("Enables AoE checking for interrupts (Disrupting Shout).")]
        public bool CheckInterruptsAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(500)]
        [Category("Protection - Interrupts & Stuns")]
        [DisplayName("Interrupt at MS left")]
        [Description("Set the amount of cast-time left in order to interrupt - Milliseconds (1000 is 1 second).")]
        public int NumInterruptTimer { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Protection - Interrupts & Stuns")]
        [DisplayName("Enable Intimidating Shout")]
        [Description("Checked enables Intimidating Shout - Used when target isn't a boss and IS is glyphed.")]
        public bool CheckIntimidatingShout { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Interrupts & Stuns")]
        [DisplayName("Enable Piercing Howl")]
        [Description("Checked enables Piercing Howl (If talented).")]
        public bool CheckPiercingHowl { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(5)]
        [Category("Protection - Interrupts & Stuns")]
        [DisplayName("Piercing Howl Count")]
        [Description("Use Piercing Howl when the amount of ads is equal to or more than the set #.")]
        public int CheckPiercingHowlNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Protection - Interrupts & Stuns")]
        [DisplayName("Enable Staggering Shout")]
        [Description("Checked enables Staggering Shout (If talented).")]
        public bool CheckStaggeringShout { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(5)]
        [Category("Protection - Interrupts & Stuns")]
        [DisplayName("Staggering Shout Count")]
        [Description("Use Staggering Shout when the amount of ads is equal to or more than the set #.")]
        public int CheckStaggeringShoutNum { get; set; }
        #endregion

        // ========================================================================================
    }
}
