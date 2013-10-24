﻿using System.Windows.Forms;
using CommonBehaviors.Actions;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Core.Utilities;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace FuryUnleashed.Rotations
{
    class ArmsRotation
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeArmsPreCombat
        {
            get
            {
                return new PrioritySelector(
                    new PrioritySelector(ret => !Me.Combat,
                        new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        new Decorator(ret => InternalSettings.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                        new Decorator(ret => InternalSettings.Instance.General.CheckPreCombatHk, Global.InitializeOnKeyActions())),
                    new Decorator(ret => Unit.DefaultBuffCheck && ((InternalSettings.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, 
                                Spell.Cast(SpellBook.BattleShout, on => Me, ret => !Global.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, 
                                Spell.Cast(SpellBook.CommandingShout, on => Me, ret => !Global.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => InternalSettings.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                    new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                    Global.InitializeCaching(),
                    Global.InitializeOnKeyActions(),
                    new Decorator(ret => InternalSettings.Instance.Arms.CheckInterrupts && Unit.CanInterrupt, Global.InitializeInterrupts()),
                    new Switch<Enum.WoWVersion>(ctx => InternalSettings.Instance.General.CrArmsRotVersion,
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Development, DevArmsCombat),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Release, RelArmsCombat)));
            }
        }

        internal static Composite DevArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                Dev_ArmsNonGcdUtility(),
                                Dev_ArmsRacials(),
                                Dev_ArmsOffensive(),
                                Item.CreateItemBehaviour(),
                                Dev_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_ArmsSt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                Dev_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_ArmsRacials(),
                                        Dev_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_ArmsSt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                Dev_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_ArmsRacials(),
                                        Dev_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && HotKeyManager.IsAoe && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_ArmsSt())
                                        ))))));
            }
        }

        internal static Composite RelArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_ArmsDefensive()),
                                Rel_ArmsNonGcdUtility(),
                                Rel_ArmsRacials(),
                                Rel_ArmsOffensive(),
                                Item.CreateItemBehaviour(),
                                Rel_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_ArmsGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, Rel_ArmsExec()),
                                        new Decorator(ret => Global.NormalPhase, Rel_ArmsSt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_ArmsDefensive()),
                                Rel_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Rel_ArmsRacials(),
                                        Rel_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                Rel_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_ArmsGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, Rel_ArmsExec()),
                                        new Decorator(ret => Global.NormalPhase, Rel_ArmsSt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_ArmsDefensive()),
                                Rel_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Rel_ArmsRacials(),
                                        Rel_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                Rel_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_ArmsGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && HotKeyManager.IsAoe && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, Rel_ArmsExec()),
                                        new Decorator(ret => Global.NormalPhase, Rel_ArmsSt())
                                        ))))));
            }
        }
        #endregion

        #region Development Rotations
        internal static Composite Dev_ArmsSt()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsExec()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsRageDump()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsMt()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsOffensive()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsGcdUtility()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsRacials()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsDefensive()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsNonGcdUtility()
        {
            return new PrioritySelector();
        }
        #endregion

        #region Release Rotations
        // Based on a mix of (Should perform excellent!):
        // http://www.mmo-champion.com/threads/1340569-5-4-Guide-Arms-PVE
        // http://us.battle.net/wow/en/forum/topic/8087999196?page=1 (Not much, mostly the other link)
        // http://www.icy-veins.com/arms-warrior-wow-pve-dps-rotation-cooldowns-abilities (Not much, mostly the other link)

        // TODO: Thunderclap is fine on MT, would be nice to check Deepwounds Strength on the other Units (to apply stronger DW to the other units on STR procs or something like this).
        internal static Composite Rel_ArmsSt()
        {
            return new PrioritySelector(
                // Inside Colossus Smash window
                new Decorator(ret => Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16), // Added T16 P4
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added.
                        Spell.Cast(SpellBook.MortalStrike), // Trying this for rage.
                        Spell.Cast(SpellBook.Slam),
                        Spell.Cast(SpellBook.Overpower, ret => Global.SLOC && Global.MSOC),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage))),
                // Outside Colossus Smash window
                new Decorator(ret => !Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.FadingDeathSentence(3000) && Global.CSCD >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.MortalStrike),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added.
                        Spell.Cast(SpellBook.StormBolt, ret => Global.ReadinessAura && Global.CSCD >= 16000), // Added - When new one is ready in next CS window - With Eye of Galakras.
                        Spell.Cast(SpellBook.Overpower),
                        Spell.Cast(SpellBook.HeroicThrow, ret => InternalSettings.Instance.Arms.CheckHeroicThrow),
                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Global.CSCD >= 6000 && Tier4AbilityUsage), // Added - For the sake of supporting it.
                        Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added - For the sake of supporting it.
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me))),
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16),
                        Spell.Cast(SpellBook.Slam, ret => Me.CurrentRage >= 90 && !Global.DeathSentenceAuraT16),
                        Spell.Cast(SpellBook.ImpendingVictory, ret => Global.IvTalent && !Global.IVOC && InternalSettings.Instance.Fury.CheckRotImpVic) // Added for the sake of supporting it rotational.                        
                        )));
        }

        internal static Composite Rel_ArmsExec()
        {
            return new PrioritySelector(
                // Inside Colossus Smash window
                new Decorator(ret => Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute),
                        Spell.Cast(SpellBook.MortalStrike),

                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added.

                        Spell.Cast(SpellBook.Overpower),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage))),
                // Outside Colossus Smash window
                new Decorator(ret => !Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Me.CurrentRage >= 80 || (Global.FadingDeathSentence(3000) && Global.CSCD >= 1500)), // Added T16 P4 - Waiting for CS window unless expires.

                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added.
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added.

                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.MortalStrike),
                        Spell.Cast(SpellBook.Overpower),
                        Spell.Cast(SpellBook.HeroicThrow, ret => InternalSettings.Instance.Arms.CheckHeroicThrow),

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Global.CSCD >= 6000 && Tier4AbilityUsage), // Added - For the sake of supporting it.
                        Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added - For the sake of supporting it.

                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me)))))
                );
        }

        internal static Composite Rel_ArmsRageDump()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.HeroicStrike, ret => ((Global.UrGlyph && Me.CurrentRage >= Me.MaxRage - 15) || (!Global.UrGlyph && Me.CurrentRage >= Me.MaxRage - 15)))
                );
        }

        internal static Composite Rel_ArmsMt()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16), // Added.
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ThunderClap, ret => InternalSettings.Instance.Arms.CheckAoEThunderclap && Unit.NeedThunderclapUnitsCount > 0), // Should be MultiDot Mortal Strike ...

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage),

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
                            Rel_ArmsSt()),
                        new Decorator(ret => Global.ExecutePhase,
                            Rel_ArmsExec()))),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount >= 3,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ThunderClap, ret => InternalSettings.Instance.Arms.CheckAoEThunderclap && Unit.NeedThunderclapUnitsCount > 0),

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage),

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
                            Rel_ArmsSt()),
                        new Decorator(ret => Global.ExecutePhase,
                            Rel_ArmsExec()))));
        }

        internal static Composite Rel_ArmsOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.BerserkerRage, ret => (!Global.EnrageAura || Global.FadingEnrage(500)) && Global.ColossusSmashAura && BerserkerRageUsage),
                Spell.Cast(SpellBook.Bloodbath, ret => Tier4AbilityUsage),

                Spell.Cast(SpellBook.Recklessness, ret => RecklessnessUsage),
                Spell.Cast(SpellBook.SkullBanner, ret => !Global.SkullBannerAura && RecklessnessSync && SkullBannerUsage),
                Spell.Cast(SpellBook.Avatar, ret => Global.AvTalent && RecklessnessSync && Tier6AbilityUsage)
                );
        }

        internal static Composite Rel_ArmsGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.ImpendingVictory, ret => !Global.IVOC && Global.IvTalent && InternalSettings.Instance.Arms.CheckImpVic && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckImpVicNum),
                Spell.Cast(SpellBook.VictoryRush, ret => !Global.VROC && Global.VictoriousAura && InternalSettings.Instance.Arms.CheckVicRush && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckVicRushNum),
                Spell.Cast(SpellBook.IntimidatingShout, ret => InternalSettings.Instance.Arms.CheckIntimidatingShout && Global.IsGlyph && !Unit.IsTargetBoss),
                Spell.Cast(SpellBook.ShatteringThrow, ret => InternalSettings.Instance.Arms.CheckShatteringThrow && Unit.IsTargetBoss)
                );
        }

        internal static Composite Rel_ArmsRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(Global.SelectRacialSpell(), ret => Global.SelectRacialSpell() != null && Global.RacialUsageSatisfied(Global.SelectRacialSpell()))
                    ));
        }

        internal static Composite Rel_ArmsDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.DiebytheSword, ret => InternalSettings.Instance.Arms.CheckDiebytheSword && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckDiebytheSwordNum),
                Spell.Cast(SpellBook.EnragedRegeneration, ret => Global.ErTalent && InternalSettings.Instance.Arms.CheckEnragedRegen && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckEnragedRegenNum),
                Spell.Cast(SpellBook.ShieldWall, ret => InternalSettings.Instance.Arms.CheckShieldWall && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckShieldWallNum),
                Spell.Cast(SpellBook.SpellReflection, ret => InternalSettings.Instance.Arms.CheckSpellReflect && Me.CurrentTarget != null && Global.TargettingMe && Me.CurrentTarget.IsCasting),
                Item.ArmsUseHealthStone()
                );
        }

        internal static Composite Rel_ArmsNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SpellBook.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && InternalSettings.Instance.Arms.CheckDemoBanner && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckDemoBannerNum && Unit.IsDoNotUseOnTgt),
                Spell.Cast(SpellBook.Hamstring, ret => !Unit.IsTargetBoss && !Global.HamstringAura && (InternalSettings.Instance.Arms.HamString == Enum.Hamstring.Always || InternalSettings.Instance.Arms.HamString == Enum.Hamstring.AddList && Unit.IsHamstringTarget)),
                Spell.Cast(SpellBook.MassSpellReflection, ret => Global.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SpellBook.PiercingHowl, ret => Global.PhTalent && InternalSettings.Instance.Arms.CheckStaggeringShout && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckPiercingHowlNum),
                Spell.Cast(SpellBook.RallyingCry, ret => Unit.RaidMembersNeedCryCount > 0),
                Spell.Cast(SpellBook.StaggeringShout, ret => Global.SsTalent && InternalSettings.Instance.Arms.CheckPiercingHowl && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region Booleans

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool MassSpellReflectionUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.MassSpellReflection == Enum.MsrTrigger.Always && Global.PUOC && Global.DSOC));
            }
        }

        internal static bool RacialUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.Recklessness == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.Recklessness == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.SkullBanner == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((InternalSettings.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessSync
        {
            get
            {
                return ((Global.RecklessnessAura) || (Global.ReadinessAura));
            }
        }

        internal static bool BloodbathSync
        {
            get
            {
                return ((Global.BloodbathAura || Global.AvTalent || Global.SbTalent) || (Global.ReadinessAura));
            }
        }
        #endregion

        }
}