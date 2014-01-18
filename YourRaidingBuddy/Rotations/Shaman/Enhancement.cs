using CommonBehaviors.Actions;
using YourBuddy.Core;
using System.Linq;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using SG = YourBuddy.Interfaces.Settings.InternalSettings;
using System.Windows.Forms;
using Styx.CommonBot;
using Logger = YourBuddy.Core.Utilities.Logger;
using LoggerP = YourBuddy.Core.Utilities.PerformanceLogger;
using YourBuddy.Core.Managers;
using G = YourBuddy.Rotations.Global;
using SH = YourBuddy.Interfaces.Settings.SettingsH;
using YourBuddy.Core.Helpers;
using Lua = YourBuddy.Core.Helpers.LuaClass;
using U = YourBuddy.Core.Unit;
using Styx.WoWInternals;

namespace YourBuddy.Rotations.Shaman
{
    class Enhancement
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeEnhancement
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                 //       new Decorator(ret => !SG.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions()),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Enhancement.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => SG.Instance.Enhancement.EnableSelfHealing && Me.HealthPercent < 100, EnhancementDefensive()),
                                        new Decorator(ret => SG.Instance.Enhancement.CheckInterrupts && Unit.CanInterrupt, EnhancementInterrupts()),
                                        EnhancementUtility(),
                                        //I.EnhancementUseItems(),
                                        EnhancementOffensive(),
                                        new Decorator(ret => SG.Instance.Enhancement.RecallTotemsEnable, Totems.CreateRecallTotems()),
                                        new Decorator(ret => SG.Instance.Enhancement.CheckAoE && (Unit.NearbyAttackableUnitsCount >= 2), EnhancementMt()),
                                        EnhancementSingleTarget())),
                    //FirstPvPEnhancement
                   new Decorator(ret => SG.Instance.Enhancement.PvPRotationCheck && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Elemental.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, EnhancementDefensivePvP()),
                                        new Decorator(ret => SG.Instance.Elemental.CheckInterrupts && Unit.CanInterrupt, EnhancementInterruptsPvP()),
                                        EnhancementUtilityPvP(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                        new PrioritySelector(
                                        //I.EnhancementUseItems(),
                                        EnhancementOffensive())),
                                        EnhancementSingleTargetPvP())),
                    //EndPvPEnhancement
                        new Decorator(ret => !SpellManager.GlobalCooldown &&  SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Enhancement.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => SG.Instance.Enhancement.EnableSelfHealing && Me.HealthPercent < 100, EnhancementDefensive()),
                                        new Decorator(ret => SG.Instance.Enhancement.CheckInterrupts && Unit.CanInterrupt, EnhancementInterrupts()),
                                        EnhancementUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                        new PrioritySelector(
                                    //    I.EnhancementUseItems(),
                                        EnhancementOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe && (Unit.NearbyAttackableUnitsCount > 1), EnhancementMt()),
                                        EnhancementSingleTarget())));
            }
        }
        #endregion

        #region RotationPvE

        internal static Composite EnhancementSingleTarget()
        {
            return new PrioritySelector(
                // Add Stormlash totem during Heroism. Once it's on CD, go back to Searing totem. (not sure how to code)
                Spell.PreventDoubleCast("Stormlash Totem", 1, ret => SG.Instance.Enhancement.EnableStormLashTotem && G.SpeedBuffsAura && !Totems.Exist(WoWTotemType.Air)),
                Spell.PreventDoubleCast("Searing Totem", 1, ret => NeedSearingTotem),
                Spell.Cast("Unleash Elements", ret => TalentManager.IsSelected(16)),
                Spell.PreventDoubleCast("Elemental Blast", 1, ret => TalentManager.IsSelected(18) && Me.HasAura(53817, 1)),
                Spell.PreventDoubleCast("Lightning Bolt", 1, ret => MaelstormStacks5),
                Spell.PreventDoubleCast("Lightning Bolt", 1, ret => Me.HasAura(138136) && MaelstormStacks4 && !Me.HasAura(128201)),
                Spell.Cast("Primal Strike"),
                Spell.Cast("Flame Shock", ret => Me.HasAura("Unleash Flame") && !Me.CurrentTarget.HasAura("Flame Shock")),
                Spell.Cast("Lava Lash"),
                Spell.Cast("Flame Shock", ret => Me.HasAura("Unleash Flame") || (!Me.HasAura("Unleash Flame") && !Me.CurrentTarget.HasAura("Flame Shock") && CooldownTracker.GetSpellCooldown("Unleash Elements").TotalMilliseconds > 5000)),
                Spell.Cast("Unleash Elements"),
                Spell.PreventDoubleCast("Chain Lightning", 1, ret => MaelstormStacks3 && !Me.HasAura(128201)),
                Spell.PreventDoubleCast("Chain Lightning", 1, ret => Me.HasAura("Ancestral Swiftness")),
                Spell.Cast("Earth Shock", ret => !FlameShock5),
                Spell.PreventDoubleCast("Chain Lightning", 1, ret => EverythingOnCooldown && MaelstormStacks1 && !Me.HasAura(128201)));
        }

        // SpellID = "Fire Nova" (Fire Nova)
        // SpellID = "Flame Shock" (Flame Shock)
        // SpellID = "Flame Shock" (Fire Nova)
        // SpellID = "Lava Lash" (Lava Lash)
        // SpellID = "Magma Totem" (Magma Totem)
        // SpellID = "Searing Totem" (Searing Totem)
        // SpellID = "Chain Lightning" (Chain lightning)
        // SpellID = "Unleash Elements" (Unleash elements)
        // SpellID = "Earth Shock" (Earth Shock)
        internal static Composite EnhancementMt()
        {
            return new PrioritySelector(
             Spell.PreventDoubleCast("Magma Totem", 1, ret => U.NearbyAttackableUnitsCount > 5 && NeedMagmaTotem),
             Spell.PreventDoubleCast("Searing Totem", 1, ret => U.NearbyAttackableUnitsCount <= 5 && NeedSearingTotem),
             new Decorator(ret => Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura("Flame Shock"),
                 new PrioritySelector(
                     Spell.Cast("Flame Shock"))),
                     Spell.Cast("Lava Lash"),
            new Decorator(ret => Me.CurrentTarget != null && G.TargetsHaveFlameShock4,
                new PrioritySelector(
                    Spell.Cast("Fire Nova"))),
           new Decorator(ret => Me.CurrentTarget != null && Me.CurrentTarget.HasMyAura("Flame Shock"),
               new PrioritySelector(
               Spell.Cast("Fire Nova"),
               Spell.PreventDoubleCast("Chain Lightning", 1, ret => MaelstormStacks4),
               Spell.Cast("Unleash Elements"),
               Spell.PreventDoubleCast("Chain Lightning", 1, ret => MaelstormStacks2),
               Spell.Cast("Stormstrike"),
               Spell.Cast("Earth Shock", ret => U.NearbyAttackableUnitsCount < 4))));
        }

        internal static Composite EnhancementDefensive()
        {
            return new PrioritySelector(
                //I.EnhancementUseHealthStone(),
                Spell.Cast("Shamanistic Rage", ret => PVEShamanisticRage),
                Spell.Cast("Ancestral Guidance", ret => PVEAncestralGuidance),
                Spell.PreventDoubleCast("Healing Tide Totem", 1, ret => PVEHealingTideTotem),
                Spell.PreventDoubleCast("Healing Stream Totem", 1, ret => PVEHealingStreamTotem),
                Spell.Cast("Stone Bulwark Totem", ret => PVEStoneBulwarkTotem),
                Spell.Cast("Healing Surge", ret => PVEHealingSurge),
                Spell.Cast("Gift of the Naaru", ret => PVEGiftOfTheNaaru)
                );
        }

        internal static Composite EnhancementOffensive()
        {
            return new PrioritySelector(
              Spell.Cast(114049, ret => CooldownTracker.GetSpellCooldown("Primal Strike").TotalMilliseconds >= 3000 && !Me.HasAura(128201) && !CooldownTracker.SpellOnCooldown(114049) && (
                    (SG.Instance.Enhancement.Ascendance == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.Ascendance == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.Ascendance == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast(2894, ret => (
                 (SG.Instance.Enhancement.FireElemental == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.FireElemental == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.FireElemental == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Lifeblood", ret => SpellManager.HasSpell(121279) && (
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast(16166, ret => Me.CurrentTarget != null && (Totems.Exist(WoWTotem.FireElemental) || CooldownTracker.GetSpellCooldown("Fire Elemental Totem").TotalSeconds > 70) &&
                        (
                        (SG.Instance.Enhancement.ElementalMastery == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Enhancement.ElementalMastery == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                        (SG.Instance.Enhancement.ElementalMastery == Enum.AbilityTrigger.Always)
                        )
                    ),
                 Spell.Cast(51533, ret => (
                 (SG.Instance.Enhancement.FeralSpirit == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.FeralSpirit == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.FeralSpirit == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite EnhancementUtility()
        {
            return new PrioritySelector(
                    Spell.Cast("Spirit Walk", ret => PVPSpiritWalk)
                    );
        }

        internal static Composite EnhancementInterrupts()
        {
            {
                return new PrioritySelector(
                   new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Wind Shear", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId()))))));
            }
        }
        #endregion


        #region RotationPvP

        internal static Composite EnhancementSingleTargetPvP()
        {
            return new PrioritySelector(
                // Add Stormlash totem during Heroism. Once it's on CD, go back to Searing totem. (not sure how to code)
                Spell.PreventDoubleCast("Stormlash Totem", 1, ret => G.SpeedBuffsAura && !Totems.Exist(WoWTotemType.Air)),
                Spell.PreventDoubleCast("Searing Totem", 1, ret => Me, ret => NeedSearingTotem),
                Spell.Cast("Unleash Elements", ret => TalentManager.IsSelected(16)),
                Spell.PreventDoubleCast("Elemental Blast", 1, ret => TalentManager.IsSelected(18) && Me.HasAura(53817, 1)),
                Spell.PreventDoubleCast("Lightning Bolt", 1, ret => MaelstormStacks5),
                Spell.PreventDoubleCast("Lightning Bolt", 1, ret => Me.HasAura(138136) && MaelstormStacks4 && !Me.HasAura(128201)),
                Spell.Cast("Primal Strike", ret => !MaelstormStacks4 || !MaelstormStacks5),
                Spell.Cast("Flame Shock", ret => Me.HasAura("Unleash Flame") && !Me.CurrentTarget.HasAura("Flame Shock")),
                Spell.Cast("Lava Lash"),
                Spell.Cast("Flame Shock", ret => Me.HasAura("Unleash Flame") || (!Me.HasAura("Unleash Flame") && !Me.CurrentTarget.HasAura("Flame Shock") && CooldownTracker.GetSpellCooldown("Unleash Elements").TotalMilliseconds > 5000)),
                Spell.Cast("Unleash Elements"),
                Spell.PreventDoubleCast("Lightning Bolt", 1, ret => MaelstormStacks3 && !Me.HasAura(128201)),
                Spell.PreventDoubleCast("Lightning Bolt", 1, ret => Me.HasAura("Ancestral Swiftness")),
                Spell.Cast("Earth Shock"),
                Spell.PreventDoubleCast("Lightning Bolt", 1, ret => EverythingOnCooldown && MaelstormStacks1 && !Me.HasAura(128201))
                 );
        }


        internal static Composite EnhancementDefensivePvP()
        {
            return new PrioritySelector(
                //I.EnhancementUseHealthStone(),
                Spell.Cast("Shamanistic Rage", ret => PVPShamanisticRage),
                Spell.Cast("Ancestral Guidance", ret => PVPAncestralGuidance),
                Spell.Cast("Stone Bulwark Totem", ret => PVPStoneBulwarkTotem),
                Spell.Cast("Healing Surge", ret => PVPHealingSurge),
                Spell.Cast("Gift of the Naaru", ret => PVPGiftOfTheNaaru),
                new Decorator(ret => Me.FocusedUnit != null && Me.FocusedUnit.IsFriendly,
                    new PrioritySelector(
                        Spell.Cast("Healing Surge", on => Me.FocusedUnit, ret => PVPFocusHealingSurge),
                        Spell.Cast("Gift of the Naaru", on => Me.FocusedUnit, ret => PVPFocusGiftOfTheNaaru)
                        )
                    )
                );
        }

        internal static Composite EnhancementOffensivePvP()
        {
            return new PrioritySelector(
                Spell.Cast("Elemental Mastery", ret => Me.CurrentTarget != null && (Totems.Exist(WoWTotem.FireElemental) || CooldownTracker.GetSpellCooldown("Fire Elemental Totem").TotalSeconds > 70) &&
                        (
                        (SG.Instance.Enhancement.ElementalMastery == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Enhancement.ElementalMastery == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                        (SG.Instance.Enhancement.ElementalMastery == Enum.AbilityTrigger.Always)
                        )
                    ),
                //actions+=/ascendance,if=cooldown.strike.remains>=3
                Spell.Cast("Ascendance", ret => CooldownTracker.GetSpellCooldown("Primal Strike").TotalMilliseconds >= 3000 && !Me.HasAura(128201) && !CooldownTracker.SpellOnCooldown(114049) && (
                    (SG.Instance.Enhancement.Ascendance == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.Ascendance == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.Ascendance == Enum.AbilityTrigger.Always)
                    )),
                 Spell.Cast("Fire Elemental Totem", ret => (
                 (SG.Instance.Enhancement.FireElemental == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.FireElemental == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.FireElemental == Enum.AbilityTrigger.Always)
                    )),
                 Spell.Cast("Feral Spirit", ret => (
                 (SG.Instance.Enhancement.FeralSpirit == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.FeralSpirit == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.FeralSpirit == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite EnhancementUtilityPvP()
        {
            return new PrioritySelector(
                    Spell.Cast("Spirit Walk", ret => PVPSpiritWalk),
                    Spell.Cast("Frost Shock", ret => PVPFrostShock),
                    Spell.Cast("Unlesh Elements", ret => PVPUnleashElements),
                    new Decorator(ret => Me.FocusedUnit != null && Me.FocusedUnit.IsHostile,
                        new PrioritySelector(
                            Spell.Cast("Hex", on => Me.FocusedUnit, ret => Me.FocusedUnit.Distance < 30 && Me.FocusedUnit.InLineOfSpellSight && !PVPHasGrounding)
                            )
                        )
                    );
        }

        internal static Composite EnhancementInterruptsPvP()
        {
            return new PrioritySelector(
                        Spell.Cast("Wind Shear", on => Me.CurrentTarget, ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.InLineOfSpellSight),
                        Spell.Cast("Grounding Totem", ret => Me.CurrentTarget.IsCasting && !Me.CurrentTarget.IsCastingHealingSpell && Me.CurrentTarget.InLineOfSpellSight && CooldownTracker.GetSpellCooldown("Wind Shear").TotalMilliseconds >= 500),
                        new Decorator(ret => Me.FocusedUnit != null && Me.FocusedUnit.IsHostile,
                            new PrioritySelector(
                                Spell.Cast("Wind Shear", on => Me.FocusedUnit, ret => Me.FocusedUnit.IsCasting && Me.FocusedUnit.InLineOfSpellSight),
                                Spell.Cast("Grounding Totem", ret => Me.FocusedUnit.IsCasting && !Me.FocusedUnit.IsCastingHealingSpell && Me.FocusedUnit.InLineOfSpellSight && CooldownTracker.GetSpellCooldown("Wind Shear").TotalMilliseconds >= 500)
                                )
                            )
                      );
        }
        #endregion


        #region Booleans PvP

        private static bool PVPHasGrounding { get { return Me.CurrentTarget.HasAura("Grounding Totem") || Me.FocusedUnit.HasAura("Grounding Totem"); } }
        private static bool PVPFrostShock { get { return !Me.CurrentTarget.IsWithinMeleeRange || !Me.CurrentTarget.HasAura("Frost Shock"); } }
        private static bool PVPUnleashElements { get { return !Me.CurrentTarget.IsWithinMeleeRange; } }
        private static bool PVPSpiritWalk { get { return Me.Rooted && !Me.Stunned && !Me.CurrentTarget.IsWithinMeleeRange; } }
        private static bool PVPShamanisticRage { get { return Me.HealthPercent <= 80 || Me.CurrentMana < 10000; } }
        private static bool PVPAncestralGuidance { get { return Me.HealthPercent <= 80 && !Me.Stunned; } }
        private static bool PVPStoneBulwarkTotem { get { return Me.HealthPercent <= 70; } }
        private static bool PVPGiftOfTheNaaru { get { return Me.HealthPercent <= 68; } }
        private static bool PVPFocusGiftOfTheNaaru { get { return Me.FocusedUnit.HealthPercent <= 67 && Me.FocusedUnit.InLineOfSpellSight; } }
        private static bool PVPHealingSurge
        {
            get
            {
                return (Me.HealthPercent <= 75 && MaelstormStacks5) ||
                        (Me.HealthPercent <= 70 && MaelstormStacks3 && !Me.IsMoving) ||
                        (Me.HealthPercent <= 40 && !Me.IsMoving);

            }
        }
        private static bool PVPFocusHealingSurge
        {
            get
            {
                return ((Me.FocusedUnit.HealthPercent <= 74 && MaelstormStacks5 && Me.FocusedUnit.InLineOfSpellSight) ||
                        (Me.FocusedUnit.HealthPercent <= 69 && MaelstormStacks3 && !Me.IsMoving && Me.FocusedUnit.InLineOfSpellSight) ||
                        (Me.FocusedUnit.HealthPercent <= 39 && !Me.IsMoving)) && Me.FocusedUnit.InLineOfSpellSight;

            }
        }

        #endregion


        #region Booleans

        private static bool MaelstormStacks4 { get { return Me.HasAura(53817) && Spell.GetAuraStack(Me, 53817) >= 4; } }
        private static bool MaelstormStacks5 { get { return Me.HasAura(53817) && Spell.GetAuraStack(Me, 53817) >= 5; } }
        private static bool MaelstormStacks2 { get { return Me.HasAura(53817) && Spell.GetAuraStack(Me, 53817) >= 2; } }
        private static bool MaelstormStacks3 { get { return Me.HasAura(53817) && Spell.GetAuraStack(Me, 53817) >= 3; } }
        private static bool MaelstormStacks1 { get { return Me.HasAura(53817) && Spell.GetAuraStack(Me, 53817) > 1; } }
        private static bool NeedFlameShockRefresh { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasAura("Flame Shock", 0, 3000); } }
        private static bool FlameShock5 { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasAura("Flame Shock", 0, 4000); } }



        private static bool PVESpiritWalk { get { return SG.Instance.Enhancement.UseSpiritWalkAuto && Me.Rooted && !Me.Stunned && !Me.CurrentTarget.IsWithinMeleeRange; } }
        private static bool PVEShamanisticRage { get { return SG.Instance.Enhancement.EnableShamanisticRage && Me.HealthPercent <= SG.Instance.Enhancement.ShamanisticRageHP; } }
        private static bool PVEHealingStreamTotem { get { return SG.Instance.Enhancement.EnableHealingStreamTotem && Me.CurrentTarget.SpellDistance() < Totems.GetTotemRange(WoWTotem.HealingStream) - 2f 
                            && !Totems.Exist(WoWTotemType.Water) && Me.HealthPercent <= SG.Instance.Enhancement.HealingStreamHP; } }
        private static bool PVEAncestralGuidance { get { return TalentManager.IsSelected(14) && SG.Instance.Enhancement.EnableAncestralGuidance && Me.HealthPercent <= SG.Instance.Enhancement.AncestralGuidanceHP && !Me.Stunned; } }
        private static bool PVEStoneBulwarkTotem { get { return SG.Instance.Enhancement.EnableStoneBulwarkTotem && Me.HealthPercent <= SG.Instance.Enhancement.StoneBulwarkTotemHP; } }
        private static bool PVEGiftOfTheNaaru { get { return SG.Instance.Enhancement.EnableGiftOfTheNaaru && Me.HealthPercent <= SG.Instance.Enhancement.GiftOfTheNaaruHP; } }
        private static bool PVEHealingTideTotem
        {
            get
            {
                return SG.Instance.Enhancement.EnableHealingTideTotem && Me.CurrentTarget.SpellDistance() < Totems.GetTotemRange(WoWTotem.HealingTide) - 2f
                    && !Totems.Exist(WoWTotemType.Water) && Me.HealthPercent <= SG.Instance.Enhancement.HealingTideHP;
            }
        }
        private static bool PVEHealingSurge
        {
            get
            {
                return (Me.HealthPercent <= SG.Instance.Enhancement.HealingSurgeHP && MaelstormStacks5);

            }
        }

        private static bool EverythingOnCooldown
        {
            get { return CooldownTracker.GetSpellCooldown(17364).TotalMilliseconds > 1500 && CooldownTracker.GetSpellCooldown(73899).TotalMilliseconds > 1500 && CooldownTracker.GetSpellCooldown(8056).TotalMilliseconds > 1000 && CooldownTracker.GetSpellCooldown("Lava Lash").TotalMilliseconds > 1500 && CooldownTracker.GetSpellCooldown("Unleash Elements").TotalMilliseconds > 1500; }
        }


        private static bool NeedSearingTotem
        {
            get
            {
                return Me.GotTarget 
                            && Me.CurrentTarget.SpellDistance() < Totems.GetTotemRange(WoWTotem.Searing) - 2f 
                            && !Totems.Exist(WoWTotemType.Fire);
            }
        }

        private static bool NeedMagmaTotem
        {
            get
            {
                return Me.GotTarget
                            && Me.CurrentTarget.SpellDistance() < Totems.GetTotemRange(WoWTotem.Magma) - 2f
                            && !Totems.Exist(WoWTotemType.Fire);
            }
        }

        #endregion Booleans

    }
}