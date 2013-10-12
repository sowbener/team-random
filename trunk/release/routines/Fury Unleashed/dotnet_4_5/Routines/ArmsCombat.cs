using CommonBehaviors.Actions;
using FuryUnleashed.Shared.Helpers;
using FuryUnleashed.Shared.Managers;
using FuryUnleashed.Shared.Utilities;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = FuryUnleashed.Routines.FuGlobal;
using I = FuryUnleashed.Core.Item;
using SB = FuryUnleashed.Shared.Helpers.SpellBook;
using SG = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SH = FuryUnleashed.Interfaces.Settings.SettingsH;
using Spell = FuryUnleashed.Core.Spell;
using U = FuryUnleashed.Core.Unit;

namespace FuryUnleashed.Routines
{
    class ArmsCombat
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
                        new Decorator(ret => SG.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                        new Decorator(ret => SG.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions())),
                    new Decorator(ret => U.DefaultBuffCheck && ((SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Arms.ShoutSelection,
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
                    new Decorator(ret => SG.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                    new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                    G.InitializeCaching(),
                    G.InitializeOnKeyActions(),
                    new Decorator(ret => SG.Instance.Arms.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                    new Switch<Enum.WoWVersion>(ctx => SG.Instance.General.CrArmsRotVersion,
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Development, DevArmsCombat),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Release, RelArmsCombat)));
            }
        }

        internal static Composite DevArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                Dev_ArmsNonGcdUtility(),
                                Dev_ArmsRacials(),
                                Dev_ArmsOffensive(),
                                I.CreateItemBehaviour(),
                                Dev_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
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
                                        I.CreateItemBehaviour())),
                                Dev_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
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
                                        I.CreateItemBehaviour())),
                                Dev_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(ret => SG.Instance.Arms.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
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
                    new Switch<Enum.Mode>(ctx => SH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_ArmsDefensive()),
                                Rel_ArmsNonGcdUtility(),
                                Rel_ArmsRacials(),
                                Rel_ArmsOffensive(),
                                I.CreateItemBehaviour(),
                                Rel_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_ArmsGcdUtility(),
                                        new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
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
                                        I.CreateItemBehaviour())),
                                Rel_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_ArmsGcdUtility(),
                                        new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
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
                                        I.CreateItemBehaviour())),
                                Rel_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_ArmsGcdUtility(),
                                        new Decorator(ret => SG.Instance.Arms.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
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
                        Spell.Cast(SB.MortalStrike), // Trying this for rage.
                        Spell.Cast(SB.Slam),
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added.
                        Spell.Cast(SB.Overpower, ret => G.SLOC && G.MSOC),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage))),
                // Outside Colossus Smash window
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.FadingDeathSentence(3000) && G.CSCD >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.MortalStrike),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added.
                        Spell.Cast(SB.StormBolt, ret => G.ReadinessAura && G.CSCD >= 16000), // Added - When new one is ready in next CS window - With Eye of Galakras.
                        Spell.Cast(SB.Overpower),
                        Spell.Cast(SB.HeroicThrow, ret => SG.Instance.Arms.CheckHeroicThrow),
                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.CSCD >= 6000 && Tier4AbilityUsage), // Added - For the sake of supporting it.
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added - For the sake of supporting it.
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me))),
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16),
                        Spell.Cast(SB.Slam, ret => Me.CurrentRage >= 90 && !G.DeathSentenceAuraT16),
                        Spell.Cast(SB.ImpendingVictory, ret => G.IvTalent && !G.IVOC && SG.Instance.Fury.CheckRotImpVic) // Added for the sake of supporting it rotational.                        
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
                        Spell.Cast(SB.Execute, ret => Me.CurrentRage >= 80 || (G.FadingDeathSentence(3000) && G.CSCD >= 1500)), // Added T16 P4 - Waiting for CS window unless expires.

                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added.
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added.

                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.MortalStrike),
                        Spell.Cast(SB.Overpower),
                        Spell.Cast(SB.HeroicThrow, ret => SG.Instance.Arms.CheckHeroicThrow),

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.CSCD >= 6000 && Tier4AbilityUsage), // Added - For the sake of supporting it.
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added - For the sake of supporting it.

                        new Switch<Enum.Shouts>(ctx => SG.Instance.Arms.ShoutSelection,
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
                        Spell.Cast(SB.ThunderClap, ret => SG.Instance.Arms.CheckAoEThunderclap && U.NeedThunderclapUnitsCount > 0), // Should be MultiDot Mortal Strike ...

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage),

                        Spell.Cast(SB.SweepingStrikes),
                        Spell.Cast(SB.ColossusSmash), // Added.
                        Spell.Cast(SB.MortalStrike), // Added - Generate rage.
                        new Decorator(ret => SG.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SB.Slam, ret => G.SlamViable),
                                Spell.Cast(SB.Whirlwind, ret => G.WhirlwindViable))),
                        new Decorator(ret => !SG.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SB.Slam))),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage == Me.MaxRage),
                        new Decorator(ret => G.NormalPhase,
                            Rel_ArmsSt()),
                        new Decorator(ret => G.ExecutePhase,
                            Rel_ArmsExec()))),
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 3,
                    new PrioritySelector(
                        Spell.Cast(SB.ThunderClap, ret => SG.Instance.Arms.CheckAoEThunderclap && U.NeedThunderclapUnitsCount > 0),

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage),

                        Spell.Cast(SB.SweepingStrikes),
                        Spell.Cast(SB.ColossusSmash), // Added.
                        Spell.Cast(SB.MortalStrike), // Added - Generate rage.
                        new Decorator(ret => SG.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SB.Slam, ret => G.SlamViable),
                                Spell.Cast(SB.Whirlwind, ret => G.WhirlwindViable))),
                        new Decorator(ret => !SG.Instance.Arms.CheckExperimentalAoE,
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
                Spell.Cast(SB.ImpendingVictory, ret => !G.IVOC && G.IvTalent && SG.Instance.Arms.CheckImpVic && Me.HealthPercent <= SG.Instance.Arms.CheckImpVicNum),
                Spell.Cast(SB.VictoryRush, ret => !G.VROC && G.VictoriousAura && SG.Instance.Arms.CheckVicRush && Me.HealthPercent <= SG.Instance.Arms.CheckVicRushNum),
                Spell.Cast(SB.IntimidatingShout, ret => SG.Instance.Arms.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.ShatteringThrow, ret => SG.Instance.Arms.CheckShatteringThrow && U.IsTargetBoss)
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
                Spell.Cast(SB.DiebytheSword, ret => SG.Instance.Arms.CheckDiebytheSword && Me.HealthPercent <= SG.Instance.Arms.CheckDiebytheSwordNum),
                Spell.Cast(SB.EnragedRegeneration, ret => G.ErTalent && SG.Instance.Arms.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Arms.CheckEnragedRegenNum),
                Spell.Cast(SB.ShieldWall, ret => SG.Instance.Arms.CheckShieldWall && Me.HealthPercent <= SG.Instance.Arms.CheckShieldWallNum),
                Spell.Cast(SB.SpellReflection, ret => SG.Instance.Arms.CheckSpellReflect && Me.CurrentTarget != null && G.TargettingMe && Me.CurrentTarget.IsCasting),
                I.ArmsUseHealthStone()
                );
        }

        internal static Composite Rel_ArmsNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SB.DemoralizingBanner, loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Arms.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Arms.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast(SB.Hamstring, ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Arms.HamString == Enum.Hamstring.Always || SG.Instance.Arms.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast(SB.MassSpellReflection, ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SB.PiercingHowl, ret => G.PhTalent && SG.Instance.Arms.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckPiercingHowlNum),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SB.StaggeringShout, ret => G.SsTalent && SG.Instance.Arms.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region Booleans

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((SG.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool MassSpellReflectionUsage
        {
            get
            {
                return ((SG.Instance.Arms.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Arms.MassSpellReflection == Enum.MsrTrigger.Always && G.PUOC && G.DSOC));
            }
        }

        internal static bool RacialUsage
        {
            get
            {
                return ((SG.Instance.Arms.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Arms.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Arms.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((SG.Instance.Arms.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Arms.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Arms.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((SG.Instance.Arms.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Arms.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Arms.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
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
