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
using G = FuryUnleashed.Rotations.Global;
using U = FuryUnleashed.Core.Unit;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SB = FuryUnleashed.Core.Helpers.SpellBook;

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
                        //new Decorator(ret => IS.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                        new Decorator(ret => IS.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions())),
                    new Decorator(ret => U.DefaultBuffCheck && ((IS.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, 
                                Spell.Cast(SB.BattleShout, on => Me, ret => !G.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, 
                                Spell.Cast(SB.CommandingShout, on => Me, ret => !G.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => IS.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                    //new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                    G.InitializeCaching(),
                    G.InitializeOnKeyActions(),
                    new Decorator(ret => IS.Instance.Arms.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                    new Switch<Enum.WoWVersion>(ctx => IS.Instance.General.CrArmsRotVersion,
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
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_ArmsSt())
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
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_ArmsSt())
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
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_ArmsSt())
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
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_ArmsSt())
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
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_ArmsSt())
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
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_ArmsSt())
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
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16), // Added T16 P4
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added.
                        Spell.Cast(SB.MortalStrike), // Trying this for rage.
                        Spell.Cast(SB.Slam),
                        Spell.Cast(SB.Overpower, ret => G.SlOc && G.MsOc),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage))),
                // Outside Colossus Smash window
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.FadingDeathSentence(3000) && G.CsCd >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.MortalStrike),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added.
                        Spell.Cast(SB.StormBolt, ret => G.ReadinessAura && G.CsCd >= 16000), // Added - When new one is ready in next CS window - With Eye of Galakras.
                        Spell.Cast(SB.Overpower),
                        Spell.Cast(SB.HeroicThrow, ret => IS.Instance.Arms.CheckHeroicThrow),
                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.CsCd >= 6000 && Tier4AbilityUsage), // Added - For the sake of supporting it.
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added - For the sake of supporting it.
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me))),
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16),
                        Spell.Cast(SB.Slam, ret => Me.CurrentRage >= 90 && !G.DeathSentenceAuraT16),
                        Spell.Cast(SB.ImpendingVictory, ret => G.IvTalent && !G.IvOc && IS.Instance.Fury.CheckRotImpVic) // Added for the sake of supporting it rotational.                        
                        )));
        }

        internal static Composite Rel_ArmsExec()
        {
            return new PrioritySelector(
                // Inside Colossus Smash window
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute),
                        Spell.Cast(SB.MortalStrike),

                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added.

                        Spell.Cast(SB.Overpower),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage))),
                // Outside Colossus Smash window
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => Me.CurrentRage >= 80 || (G.FadingDeathSentence(3000) && G.CsCd >= 1500)), // Added T16 P4 - Waiting for CS window unless expires.

                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added.
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added.

                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.MortalStrike),
                        Spell.Cast(SB.Overpower),
                        Spell.Cast(SB.HeroicThrow, ret => IS.Instance.Arms.CheckHeroicThrow),

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.CsCd >= 6000 && Tier4AbilityUsage), // Added - For the sake of supporting it.
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added - For the sake of supporting it.

                        new Switch<Enum.Shouts>(ctx => IS.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me)))))
                );
        }

        internal static Composite Rel_ArmsRageDump()
        {
            return new PrioritySelector(
                Spell.Cast(SB.HeroicStrike, ret => ((G.UrGlyph && Me.CurrentRage >= Me.MaxRage - 15) || (!G.UrGlyph && Me.CurrentRage >= Me.MaxRage - 15)))
                );
        }

        internal static Composite Rel_ArmsMt()
        {
            return new PrioritySelector(
                Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16), // Added.
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast(SB.ThunderClap, ret => IS.Instance.Arms.CheckAoEThunderclap && U.NeedThunderclapUnitsCount > 0), // Should be MultiDot Mortal Strike ...

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage),

                        Spell.Cast(SB.SweepingStrikes),
                        Spell.Cast(SB.ColossusSmash), // Added.
                        Spell.Cast(SB.MortalStrike), // Added - Generate rage.
                        new Decorator(ret => IS.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SB.Slam, ret => G.SlamViable),
                                Spell.Cast(SB.Whirlwind, ret => G.WhirlwindViable))),
                        new Decorator(ret => !IS.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SB.Slam))),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage == Me.MaxRage),
                        new Decorator(ret => G.NormalPhase,
                            Rel_ArmsSt()),
                        new Decorator(ret => G.ExecutePhase,
                            Rel_ArmsExec()))),
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 3,
                    new PrioritySelector(
                        Spell.Cast(SB.ThunderClap, ret => IS.Instance.Arms.CheckAoEThunderclap && U.NeedThunderclapUnitsCount > 0),

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage),

                        Spell.Cast(SB.SweepingStrikes),
                        Spell.Cast(SB.ColossusSmash), // Added.
                        Spell.Cast(SB.MortalStrike), // Added - Generate rage.
                        new Decorator(ret => IS.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SB.Slam, ret => G.SlamViable),
                                Spell.Cast(SB.Whirlwind, ret => G.WhirlwindViable))),
                        new Decorator(ret => !IS.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SB.Slam))),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage == Me.MaxRage),
                        new Decorator(ret => G.NormalPhase,
                            Rel_ArmsSt()),
                        new Decorator(ret => G.ExecutePhase,
                            Rel_ArmsExec()))));
        }

        internal static Composite Rel_ArmsOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.BerserkerRage, ret => (!G.EnrageAura || G.FadingEnrage(500)) && G.ColossusSmashAura && BerserkerRageUsage),
                Spell.Cast(SB.Bloodbath, ret => Tier4AbilityUsage),

                Spell.Cast(SB.Recklessness, ret => RecklessnessUsage),
                Spell.Cast(SB.SkullBanner, ret => !G.SkullBannerAura && RecklessnessSync && SkullBannerUsage),
                Spell.Cast(SB.Avatar, ret => G.AvTalent && RecklessnessSync && Tier6AbilityUsage)
                );
        }

        internal static Composite Rel_ArmsGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ImpendingVictory, ret => !G.IvOc && G.IvTalent && IS.Instance.Arms.CheckImpVic && Me.HealthPercent <= IS.Instance.Arms.CheckImpVicNum),
                Spell.Cast(SB.VictoryRush, ret => !G.VrOc && G.VictoriousAura && IS.Instance.Arms.CheckVicRush && Me.HealthPercent <= IS.Instance.Arms.CheckVicRushNum),
                Spell.Cast(SB.IntimidatingShout, ret => IS.Instance.Arms.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.ShatteringThrow, ret => IS.Instance.Arms.CheckShatteringThrow && U.IsTargetBoss)
                );
        }

        internal static Composite Rel_ArmsRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite Rel_ArmsDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.DiebytheSword, ret => IS.Instance.Arms.CheckDiebytheSword && Me.HealthPercent <= IS.Instance.Arms.CheckDiebytheSwordNum),
                Spell.Cast(SB.EnragedRegeneration, ret => G.ErTalent && IS.Instance.Arms.CheckEnragedRegen && Me.HealthPercent <= IS.Instance.Arms.CheckEnragedRegenNum),
                Spell.Cast(SB.ShieldWall, ret => IS.Instance.Arms.CheckShieldWall && Me.HealthPercent <= IS.Instance.Arms.CheckShieldWallNum),
                Spell.Cast(SB.SpellReflection, ret => IS.Instance.Arms.CheckSpellReflect && Me.CurrentTarget != null && G.TargettingMe && Me.CurrentTarget.IsCasting),
                Item.ArmsUseHealthStone()
                );
        }

        internal static Composite Rel_ArmsNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SB.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && IS.Instance.Arms.CheckDemoBanner && Me.HealthPercent <= IS.Instance.Arms.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast(SB.Hamstring, ret => !U.IsTargetBoss && !G.HamstringAura && (IS.Instance.Arms.HamString == Enum.Hamstring.Always || IS.Instance.Arms.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast(SB.MassSpellReflection, ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SB.PiercingHowl, ret => G.PhTalent && IS.Instance.Arms.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckPiercingHowlNum),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SB.StaggeringShout, ret => G.SsTalent && IS.Instance.Arms.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region Booleans

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((IS.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool MassSpellReflectionUsage
        {
            get
            {
                return ((IS.Instance.Arms.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Arms.MassSpellReflection == Enum.MsrTrigger.Always && G.PuOc && G.DsOc));
            }
        }

        internal static bool RacialUsage
        {
            get
            {
                return ((IS.Instance.Arms.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Arms.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Arms.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((IS.Instance.Arms.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Arms.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Arms.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((IS.Instance.Arms.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Arms.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Arms.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((IS.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((IS.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((IS.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessSync
        {
            get
            {
                return ((G.RecklessnessAura) || (G.ReadinessAura));
            }
        }

        internal static bool BloodbathSync
        {
            get
            {
                return ((G.BloodbathAura || G.AvTalent || G.SbTalent) || (G.ReadinessAura));
            }
        }
        #endregion

        }
}