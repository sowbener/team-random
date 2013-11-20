﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace FuryUnleashed.Rotations.Arms
{
    internal class ArmsPvP
    {
        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        internal static Composite PvPArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, PvP_ArmsDefensive()),
                                PvP_ArmsNonGcdUtility(),
                                PvP_ArmsRacials(),
                                PvP_ArmsOffensive(),
                                Item.CreateItemBehaviour(),
                                PvP_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        PvP_ArmsGcdUtility(),
                                        new Decorator(
                                            ret =>
                                                InternalSettings.Instance.Arms.CheckAoE &&
                                                Unit.NearbyAttackableUnitsCount >=
                                                InternalSettings.Instance.Arms.CheckAoENum, PvP_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, PvP_ArmsExecute()),
                                        new Decorator(ret => Global.NormalPhase, PvP_ArmsSingleTarget())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, PvP_ArmsDefensive()),
                                PvP_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        PvP_ArmsRacials(),
                                        PvP_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                PvP_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        PvP_ArmsGcdUtility(),
                                        new Decorator(
                                            ret =>
                                                InternalSettings.Instance.Arms.CheckAoE &&
                                                Unit.NearbyAttackableUnitsCount >=
                                                InternalSettings.Instance.Arms.CheckAoENum, PvP_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, PvP_ArmsExecute()),
                                        new Decorator(ret => Global.NormalPhase, PvP_ArmsSingleTarget())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, PvP_ArmsDefensive()),
                                PvP_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        PvP_ArmsRacials(),
                                        PvP_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                PvP_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        PvP_ArmsGcdUtility(),
                                        new Decorator(
                                            ret =>
                                                InternalSettings.Instance.Arms.CheckAoE && HotKeyManager.IsAoe &&
                                                Unit.NearbyAttackableUnitsCount >=
                                                InternalSettings.Instance.Arms.CheckAoENum, PvP_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, PvP_ArmsExecute()),
                                        new Decorator(ret => Global.NormalPhase, PvP_ArmsSingleTarget())
                                        ))))));
            }
        }

        #region Release Rotations

        // Based on a mix of (Should perform excellent!):
        // http://www.mmo-champion.com/threads/1340569-5-4-Guide-Arms-PVE
        // http://us.battle.net/wow/en/forum/topic/8087999196?page=1 (Not much, mostly the other link)
        // http://www.icy-veins.com/arms-warrior-wow-pve-dps-rotation-cooldowns-abilities (Not much, mostly the other link)
        // http://www.noxxic.com/wow/pve/warrior/fury/dps-rotation-and-cooldowns

        // TODO: Thunderclap is fine on MT, would be nice to check Deepwounds Strength on the other Units (to apply stronger DW to the other units on STR procs or something like this).
        internal static Composite PvP_ArmsSingleTarget()
        {
            return new PrioritySelector(
                // Inside Colossus Smash window
                new Decorator(ret => Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16), // Added T16 P4
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && ArmsGlobal.Tier6AbilityUsage), // Added.
                        Spell.Cast(SpellBook.MortalStrike), // Trying this for rage.
                        Spell.Cast(SpellBook.Slam),
                        Spell.Cast(SpellBook.Overpower, ret => Global.SlamOnCooldown && Global.MortalStrikeOnCooldown),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage))),
                // Outside Colossus Smash window
                new Decorator(ret => !Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute,
                            ret => Global.FadingDeathSentence(3000) && Global.ColossusSmashSpellCooldown >= 1500),
                // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.MortalStrike),
                        Spell.Cast(SpellBook.DragonRoar,
                            ret => Global.DragonRoarTalent && ArmsGlobal.BloodbathSync && ArmsGlobal.Tier4AbilityUsage), // Added.
                // Also in PvP_FuryHeroicStrike().
                        Spell.Cast(SpellBook.StormBolt,
                            ret => Global.DeterminationAura || Global.OutrageAura || Global.ColossusSmashSpellCooldown >= 14000 && ArmsGlobal.Tier6AbilityUsage),
                // Added - When new one is ready in next CS window - With Eye of Galakras.
                        Spell.Cast(SpellBook.Overpower),
                        Spell.Cast(SpellBook.HeroicThrow, ret => InternalSettings.Instance.Arms.CheckHeroicThrow),
                        Spell.Cast(SpellBook.Bladestorm,
                            ret => Global.BladestormTalent && Global.ColossusSmashSpellCooldown >= 6000 && ArmsGlobal.Tier4AbilityUsage),
                // Added - For the sake of supporting it.
                        Spell.Cast(SpellBook.Shockwave,
                            ret => Global.ShockwaveTalent && Global.ShockwaveFacing && ArmsGlobal.Tier4AbilityUsage),
                // Added - For the sake of supporting it.
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout,
                                Spell.Cast(SpellBook.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout,
                                Spell.Cast(SpellBook.CommandingShout, on => Me))),
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16),
                        Spell.Cast(SpellBook.Slam, ret => Me.CurrentRage >= 90 && !Global.DeathSentenceAuraT16),
                        Spell.Cast(SpellBook.ImpendingVictory,
                            ret =>
                                Global.ImpendingVictoryTalent && !Global.ImpendingVictoryOnCooldown && InternalSettings.Instance.Fury.CheckRotImpVic)
                // Added for the sake of supporting it rotational.                        
                        )));
        }

        internal static Composite PvP_ArmsExecute()
        {
            return new PrioritySelector(
                // Inside Colossus Smash window
                new Decorator(ret => Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute),
                        Spell.Cast(SpellBook.MortalStrike),

                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && ArmsGlobal.Tier6AbilityUsage), // Added.

                        Spell.Cast(SpellBook.Overpower),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage))),
                // Outside Colossus Smash window
                new Decorator(ret => !Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute,
                            ret => Me.CurrentRage >= 80 || (Global.FadingDeathSentence(3000) && Global.ColossusSmashSpellCooldown >= 1500)),
                // Added T16 P4 - Waiting for CS window unless expires.

                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && ArmsGlobal.BloodbathSync && ArmsGlobal.Tier4AbilityUsage),
                // Added.
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && ArmsGlobal.Tier6AbilityUsage), // Added.

                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.MortalStrike),
                        Spell.Cast(SpellBook.Overpower),
                        Spell.Cast(SpellBook.HeroicThrow, ret => InternalSettings.Instance.Arms.CheckHeroicThrow),

                        Spell.Cast(SpellBook.Bladestorm,
                            ret => Global.BladestormTalent && Global.ColossusSmashSpellCooldown >= 6000 && ArmsGlobal.Tier4AbilityUsage),
                // Added - For the sake of supporting it.
                        Spell.Cast(SpellBook.Shockwave,
                            ret => Global.ShockwaveTalent && Global.ShockwaveFacing && ArmsGlobal.Tier4AbilityUsage),
                // Added - For the sake of supporting it.

                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout,
                                Spell.Cast(SpellBook.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout,
                                Spell.Cast(SpellBook.CommandingShout, on => Me)))))
                );
        }

        internal static Composite PvP_ArmsRageDump()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.HeroicStrike,
                    ret =>
                        ((Global.UnendingRageGlyph && Me.CurrentRage >= Me.MaxRage - 15) ||
                         (!Global.UnendingRageGlyph && Me.CurrentRage >= Me.MaxRage - 15)))
                );
        }

        internal static Composite PvP_ArmsMt()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16), // Added.
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ThunderClap,
                            ret =>
                                InternalSettings.Instance.Arms.CheckAoEThunderclap && Unit.NeedThunderclapUnitsCount > 0),
                // Should be MultiDot Mortal Strike ...

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BladestormTalent && ArmsGlobal.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && ArmsGlobal.BloodbathSync && ArmsGlobal.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Shockwave,
                            ret => Global.ShockwaveTalent && Global.ShockwaveFacing && ArmsGlobal.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && ArmsGlobal.Tier6AbilityUsage),

                        Spell.Cast(SpellBook.SweepingStrikes),
                        Spell.Cast(SpellBook.ColossusSmash), // Added.
                        Spell.Cast(SpellBook.MortalStrike), // Added - Generate rage.
                        new Decorator(ret => InternalSettings.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.Slam, ret => Global.SlamViable),
                                Spell.Cast(SpellBook.Whirlwind, ret => Global.WhirlwindViable))),
                        new Decorator(ret => !InternalSettings.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.Slam))),
                        Spell.Cast(SpellBook.Cleave, ret => Me.CurrentRage == Me.MaxRage),
                        new Decorator(ret => Global.NormalPhase,
                            PvP_ArmsSingleTarget()),
                        new Decorator(ret => Global.ExecutePhase,
                            PvP_ArmsExecute()))),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount >= 3,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ThunderClap,
                            ret =>
                                InternalSettings.Instance.Arms.CheckAoEThunderclap && Unit.NeedThunderclapUnitsCount > 0),

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BladestormTalent && ArmsGlobal.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && ArmsGlobal.BloodbathSync && ArmsGlobal.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Shockwave,
                            ret => Global.ShockwaveTalent && Global.ShockwaveFacing && ArmsGlobal.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && ArmsGlobal.Tier6AbilityUsage),

                        Spell.Cast(SpellBook.SweepingStrikes),
                        Spell.Cast(SpellBook.ColossusSmash), // Added.
                        Spell.Cast(SpellBook.MortalStrike), // Added - Generate rage.
                        new Decorator(ret => InternalSettings.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.Slam, ret => Global.SlamViable),
                                Spell.Cast(SpellBook.Whirlwind, ret => Global.WhirlwindViable))),
                        new Decorator(ret => !InternalSettings.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.Slam))),
                        Spell.Cast(SpellBook.Cleave, ret => Me.CurrentRage == Me.MaxRage),
                        new Decorator(ret => Global.NormalPhase,
                            PvP_ArmsSingleTarget()),
                        new Decorator(ret => Global.ExecutePhase,
                            PvP_ArmsExecute()))));
        }

        internal static Composite PvP_ArmsOffensive()
        {
            return new PrioritySelector(

                new Decorator(ret => !StyxWoW.Me.IsInInstance && StyxWoW.Me.CurrentTarget != null &&
                                     StyxWoW.Me.CurrentTarget.IsPlayer && !StyxWoW.Me.CurrentTarget.IsFriendly &&
                                     StyxWoW.Me.CurrentTarget.Distance > 12 &&
                                     StyxWoW.Me.CurrentTarget.Distance < 30,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Charge),
                        Spell.CastOnGround(SpellBook.HeroicLeap, ret => StyxWoW.Me.CurrentTarget),
                        Spell.Cast(SpellBook.BerserkerRage,
                            ret =>
                                (!Global.EnrageAura || Global.FadingEnrage(500)) && Global.ColossusSmashAura &&
                                ArmsGlobal.BerserkerRageUsage),
                        Spell.Cast(SpellBook.Bloodbath, ret => ArmsGlobal.Tier4AbilityUsage),

                        Spell.Cast(SpellBook.Recklessness, ret => ArmsGlobal.RecklessnessUsage),
                        Spell.Cast(SpellBook.SkullBanner,
                            ret => !Global.SkullBannerAura && ArmsGlobal.RecklessnessSync && ArmsGlobal.SkullBannerUsage),
                        Spell.Cast(SpellBook.Avatar, ret => Global.AvatarTalent && ArmsGlobal.RecklessnessSync && ArmsGlobal.Tier6AbilityUsage)
                        )
                    )
                );
        }

        internal static Composite PvP_ArmsGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.ImpendingVictory,
                    ret =>
                        !Global.ImpendingVictoryOnCooldown && Global.ImpendingVictoryTalent && InternalSettings.Instance.Arms.CheckImpVic &&
                        Me.HealthPercent <= InternalSettings.Instance.Arms.CheckImpVicNum),
                Spell.Cast(SpellBook.VictoryRush,
                    ret =>
                        !Global.VictoryRushOnCooldown && Global.VictoriousAura && InternalSettings.Instance.Arms.CheckVicRush &&
                        Me.HealthPercent <= InternalSettings.Instance.Arms.CheckVicRushNum),
                Spell.Cast(SpellBook.IntimidatingShout,
                    ret => InternalSettings.Instance.Arms.CheckIntimidatingShout && Global.IntimidatingShoutGlyph && !Unit.IsTargetBoss),
                Spell.Cast(SpellBook.ShatteringThrow,
                    ret => InternalSettings.Instance.Arms.CheckShatteringThrow && Unit.IsTargetBoss)
                );
        }

        internal static Composite PvP_ArmsRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => ArmsGlobal.RacialUsage,
                    Spell.Cast(Global.SelectRacialSpell(),
                        ret =>
                            Global.SelectRacialSpell() != null &&
                            Global.RacialUsageSatisfied(Global.SelectRacialSpell()))
                    ));
        }

        internal static Composite PvP_ArmsDefensive()
        {
            return new PrioritySelector(
                // Interrupts
                new Decorator(
                    ret =>
                        !StyxWoW.Me.IsInInstance && StyxWoW.Me.CurrentTarget.IsPlayer &&
                        !StyxWoW.Me.CurrentTarget.IsFriendly && StyxWoW.Me.IsCasting ||
                        StyxWoW.Me.CurrentTarget.IsChanneling,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Pummel, ret => StyxWoW.Me.CurrentTarget.Distance < 6),
                        Spell.Cast(SpellBook.DisruptingShout, ret => StyxWoW.Me.CurrentTarget.Distance < 6),
                        Spell.Cast(SpellBook.IntimidatingShout, ret => StyxWoW.Me.CurrentTarget.Distance < 6),
                        Spell.Cast(SpellBook.Charge,
                            ret => StyxWoW.Me.CurrentTarget.Distance < 25 && StyxWoW.Me.CurrentTarget.Distance > 12))),
                Spell.Cast(SpellBook.Avatar, ret => Global.AvatarTalent && IsSnaredOrRootedOrDazed(StyxWoW.Me)),
                Spell.Cast(SpellBook.BerserkerRage, ret => NeedBerserkerRage()),
                new Decorator(ret => Me.HealthPercent < 25 && StyxWoW.Me.CurrentTarget.IsPlayer && StyxWoW.Me.CurrentTarget.IsHostile,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.IntimidatingShout,
                            ret =>
                                Me.HealthPercent < 65 && !StyxWoW.Me.IsInInstance && StyxWoW.Me.CurrentTarget.IsPlayer &&
                                !StyxWoW.Me.CurrentTarget.IsFriendly && Unit.NearbyAttackableUnitsCount >= 1),
                        new Decorator(ret => Unit.NearbyFriendlyUnits(StyxWoW.Me.Location, 25).Any(),
                            Spell.Cast(SpellBook.Intervene,
                                ret =>
                                    Unit.NearbyFriendlyUnits(StyxWoW.Me.Location, 25)
                                        .Where(x => x.Distance > 12 && x.Distance < 35 && x.InLineOfSpellSight)
                                        .OrderByDescending(x => x.Distance)
                                        .FirstOrDefault(),
                                ret => NeedBerserkerRage())
                            ),
                        new PrioritySelector(
                            ctx => Unit.NearbyFriendlyUnits(StyxWoW.Me.Location, 35)
                                .OrderByDescending(x => x.Distance)
                                .FirstOrDefault(x => x.Distance > 12 && x.InLineOfSpellSight),
                            new Decorator(ctx => ctx != null,
                                Spell.CastOnGround(SpellBook.HeroicLeap, ctx => (WoWUnit)ctx))
                            ),
                        new PrioritySelector(
                            ctx => Unit.NearbyAttackableUnits(StyxWoW.Me.Location, 35)
                                .OrderByDescending(x => x.Distance)
                                .FirstOrDefault(x => x.Distance > 12 && x.InLineOfSpellSight),
                            new Decorator(ctx => ctx != null,
                                Spell.CastOnGround(SpellBook.HeroicLeap, ctx => (WoWUnit)ctx))
                            )
                        )
                    ),
                Spell.Cast(SpellBook.DiebytheSword,
                    ret =>
                        InternalSettings.Instance.Arms.CheckDiebytheSword &&
                        Me.HealthPercent <= InternalSettings.Instance.Arms.CheckDiebytheSwordNum),
                Spell.Cast(SpellBook.EnragedRegeneration,
                    ret =>
                        Global.EnragedRegenerationTalent && InternalSettings.Instance.Arms.CheckEnragedRegen &&
                        Me.HealthPercent <= InternalSettings.Instance.Arms.CheckEnragedRegenNum),
                Spell.Cast(SpellBook.ShieldWall,
                    ret =>
                        InternalSettings.Instance.Arms.CheckShieldWall &&
                        Me.HealthPercent <= InternalSettings.Instance.Arms.CheckShieldWallNum),
                Spell.Cast(SpellBook.SpellReflection,
                    ret =>
                        ((InternalSettings.Instance.Arms.CheckSpellReflect && Me.CurrentTarget != null &&
                          Global.TargettingMe && Me.CurrentTarget.IsCasting) ||
                         Unit.NearbyCastingUnitsTargetingMe(StyxWoW.Me.Location, 45).FirstOrDefault() != null)),
                Item.ArmsUseHealthStone()
                );
        }

        internal static Composite PvP_ArmsNonGcdUtility()
        {
            return new PrioritySelector(
                EquipTwoHander(),
                EquipSwordAndBoard(),
                Spell.CastOnGround(SpellBook.DemoralizingBanner, loc => Me.Location,
                    ret =>
                        SettingsH.Instance.DemoBannerChoice == Keys.None &&
                        InternalSettings.Instance.Arms.CheckDemoBanner &&
                        Me.HealthPercent <= InternalSettings.Instance.Arms.CheckDemoBannerNum && Unit.IsDoNotUseOnTgt),
                Spell.Cast(SpellBook.Hamstring,
                    ret =>
                        !IsCrowdControlled(StyxWoW.Me.CurrentTarget) && !Unit.IsTargetBoss && !Global.HamstringAura &&
                        (InternalSettings.Instance.Arms.HamString == Enum.Hamstring.Always ||
                         InternalSettings.Instance.Arms.HamString == Enum.Hamstring.AddList && Unit.IsHamstringTarget)),
                Spell.Cast(SpellBook.MassSpellReflection,
                    ret =>
                        Global.MassSpellReflectionTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting &&
                        ArmsGlobal.MassSpellReflectionUsage),
                Spell.Cast(SpellBook.PiercingHowl,
                    ret =>
                        Global.PiercingHowlTalent && InternalSettings.Instance.Arms.CheckStaggeringShout &&
                        Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckPiercingHowlNum),
                Spell.Cast(SpellBook.RallyingCry, ret => Unit.RaidMembersNeedCryCount > 0),
                Spell.Cast(SpellBook.StaggeringShout,
                    ret =>
                        Global.StaggeringShoutTalent && InternalSettings.Instance.Arms.CheckPiercingHowl &&
                        Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckPiercingHowlNum),
                new Decorator(ret => Unit.VigilanceTarget != null,
                    Spell.Cast(SpellBook.Vigilance, on => Unit.VigilanceTarget))
                );
        }

        private static Composite EquipTwoHander()
        {
            return
                new Decorator(
                    ret => (Me.HealthPercent > 40 || Me.CurrentTarget.HealthPercent < 20) && !TwoHanderEquipped(),
                    new Action(ret =>
                    {
                        Lua.DoString("RunMacroText(\"/equipset PVP\")");
                        return RunStatus.Success;
                    }));

        }

        private static Composite EquipSwordAndBoard()
        {
            return
                new Decorator(
                    ret => (Me.HealthPercent < 25 && Me.CurrentTarget.HealthPercent > 40) && TwoHanderEquipped(),
                    new Action(ret =>
                    {
                        Lua.DoString("RunMacroText(\"/equipset Shield\")");
                        return RunStatus.Success;
                    }));

        }

        #endregion

        #region Booleans

        private static bool ShieldEquipped()
        {
            if (StyxWoW.Me.Inventory.Equipped.OffHand.ItemInfo.ItemClass == WoWItemClass.Weapon)
                return false;
            return (StyxWoW.Me.Inventory.Equipped.OffHand.ItemInfo.InventoryType == InventoryType.Shield);
        }

        private static bool TwoHanderEquipped()
        {
            if (StyxWoW.Me.Inventory.Equipped.MainHand == null)
                return false;
            return (StyxWoW.Me.Inventory.Equipped.MainHand.ItemInfo.InventoryType == InventoryType.TwoHandWeapon);
        }

        internal static bool IsSnaredOrRootedOrDazed(WoWUnit unit)
        {
            if (unit != null)
            {
                Dictionary<string, WoWAura>.ValueCollection auras = unit.Auras.Values;

                return auras.Any(a =>
                    a.IsHarmful && (
                        a.Spell.Mechanic == WoWSpellMechanic.Rooted ||
                        a.Spell.Mechanic == WoWSpellMechanic.Snared ||
                        a.Spell.Mechanic == WoWSpellMechanic.Dazed));
            }
            return false;
        }

        internal static bool NeedBerserkerRage()
        {
            Dictionary<string, WoWAura>.ValueCollection auras = StyxWoW.Me.Auras.Values;

            return auras.Any(a =>
                a.IsHarmful && (
                    a.Spell.Mechanic == WoWSpellMechanic.Fleeing ||
                    a.Spell.Mechanic == WoWSpellMechanic.Sapped ||
                    a.Spell.Mechanic == WoWSpellMechanic.Incapacitated ||
                    a.Spell.Mechanic == WoWSpellMechanic.Turned));
        }

        internal static bool IsCrowdControlled(WoWUnit unit)
        {
            if (unit != null)
            {
                Dictionary<string, WoWAura>.ValueCollection auras = unit.Auras.Values;

                return auras.Any(a =>
                    a.IsHarmful && (
                        a.Spell.Mechanic == WoWSpellMechanic.Banished ||
                        a.Spell.Mechanic == WoWSpellMechanic.Disoriented ||
                        a.Spell.Mechanic == WoWSpellMechanic.Charmed ||
                        a.Spell.Mechanic == WoWSpellMechanic.Horrified ||
                        a.Spell.Mechanic == WoWSpellMechanic.Incapacitated ||
                        a.Spell.Mechanic == WoWSpellMechanic.Polymorphed ||
                        a.Spell.Mechanic == WoWSpellMechanic.Sapped ||
                        a.Spell.Mechanic == WoWSpellMechanic.Shackled ||
                        a.Spell.Mechanic == WoWSpellMechanic.Asleep ||
                        a.Spell.Mechanic == WoWSpellMechanic.Frozen ||
                        a.Spell.Mechanic == WoWSpellMechanic.Invulnerable ||
                        a.Spell.Mechanic == WoWSpellMechanic.Invulnerable2 ||
                        a.Spell.Mechanic == WoWSpellMechanic.Turned ||
                        a.Spell.Mechanic == WoWSpellMechanic.Fleeing ||
                        a.Spell.Mechanic == WoWSpellMechanic.Snared ||
                        a.Spell.Name == "Hex"));
            }
            return false;
        }

        #endregion

    }
}
