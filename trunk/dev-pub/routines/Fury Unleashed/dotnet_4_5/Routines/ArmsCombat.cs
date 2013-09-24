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
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Development,
                            new Decorator(ret => true, DevArmsCombat)),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Release,
                            new Decorator(ret => true, RelArmsCombat))));
                            //!Spell.IsGlobalCooldown()
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
                                Dev_ArmsUtility(),
                                Dev_ArmsVictorious(),
                                I.CreateItemBehaviour(),
                                Dev_ArmsRacials(),
                                Dev_ArmsOffensive(),
                                new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                new Decorator(ret => G.ExecuteCheck, Dev_ArmsExec()),
                                Dev_ArmsSt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                Dev_ArmsUtility(),
                                Dev_ArmsVictorious(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.CreateItemBehaviour(),
                                        Dev_ArmsRacials(),
                                        Dev_ArmsOffensive())),
                                new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                new Decorator(ret => G.ExecuteCheck, Dev_ArmsExec()),
                                Dev_ArmsSt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                Dev_ArmsUtility(),
                                Dev_ArmsVictorious(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.CreateItemBehaviour(),
                                        Dev_ArmsRacials(),
                                        Dev_ArmsOffensive())),
                                new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                new Decorator(ret => G.ExecuteCheck, Dev_ArmsExec()),
                                Dev_ArmsSt()))));
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
                                new Decorator(ret => Me.HealthPercent < 100, ArmsDefensive()),
                                ArmsUtility(),
                                ArmsVictorious(),
                                I.CreateItemBehaviour(),
                                ArmsRacials(),
                                ArmsOffensive(),
                                new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, ArmsMt()),
                                ArmsSt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, ArmsDefensive()),
                                ArmsUtility(),
                                ArmsVictorious(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.CreateItemBehaviour(),
                                        ArmsRacials(),
                                        ArmsOffensive())),
                                new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, ArmsMt()),
                                ArmsSt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, ArmsDefensive()),
                                ArmsUtility(),
                                ArmsVictorious(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.CreateItemBehaviour(),
                                        ArmsRacials(),
                                        ArmsOffensive())),
                                new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckAoENum, ArmsMt()),
                                ArmsSt()))));
            }
        }
        #endregion

        #region Development Rotations
        // Based on a mix of (Should perform excellent!):
        // No T16P2 rotational support, as it's experimental.
        // http://www.mmo-champion.com/threads/1340569-5-4-Guide-Arms-PVE
        // http://www.icy-veins.com/arms-warrior-wow-pve-dps-rotation-cooldowns-abilities (Not much, mostly the other link)
        internal static Composite Dev_ArmsSt()
        {
            return new PrioritySelector(
                // Inside Colossus Smash window
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathScentenceAuraT16),
                        Spell.Cast(SB.Slam, ret => !G.DeathScentenceAuraT16),
                        Spell.Cast(SB.MortalStrike, ret => G.SLOC),
                        Spell.Cast(SB.Overpower, ret => G.SLOC && G.MSOC),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage - 10))),
                // Outside Colossus Smash window
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.MortalStrike),

                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added.
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added.

                        Spell.Cast(SB.Overpower),
                        Spell.Cast(SB.HeroicThrow, ret => SG.Instance.Arms.CheckHeroicThrow),

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.CSCD >= 6000 && Tier4AbilityUsage), // Added - For the sake of supporting it.
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added - For the sake of supporting it.

                        new Switch<Enum.Shouts>(ctx => SG.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me))),
                        Spell.Cast(SB.Execute, ret => G.DeathScentenceAuraT16),
                        Spell.Cast(SB.Slam, ret => Me.CurrentRage >= 90 && !G.DeathScentenceAuraT16),
                        Spell.Cast(SB.ImpendingVictory, ret => G.IvTalent && !G.IVOC && SG.Instance.Fury.CheckRotImpVic) // Added for the sake of supporting it rotational.                        
                        )));
        }

        internal static Composite Dev_ArmsExec()
        {
            return new PrioritySelector(
                // Inside Colossus Smash window
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute),
                        Spell.Cast(SB.MortalStrike),

                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && Me.CurrentRage < 30 && BloodbathSync && Tier4AbilityUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Me.CurrentRage < 30 && Tier6AbilityUsage), // Added.

                        Spell.Cast(SB.Overpower),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage - 10))),
                // Outside Colossus Smash window
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => Me.CurrentRage >= 80 || G.DeathScentenceAuraT16),

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

        //Check from here
        internal static Composite Dev_ArmsMt()
        {
            return new PrioritySelector(
                Spell.Cast(SB.SweepingStrikes),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast(SB.ThunderClap, ret => U.NeedThunderclapUnitsCount > 0), // Should be MultiDot Mortal Strike ...

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage),

                        Spell.Cast(SB.Slam, ret => U.NearbySlamCleaveUnitsCount >= 2),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage == Me.MaxRage),
                        new Decorator(ret => G.NonExecuteCheck,
                            Dev_ArmsSt()),
                        new Decorator(ret => G.ExecuteCheck,
                            Dev_ArmsExec()))),
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 3,
                    new PrioritySelector(
                        Spell.Cast(SB.ThunderClap, ret => U.NeedThunderclapUnitsCount > 0),

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage),

                        Spell.Cast(SB.Slam, ret => U.NearbySlamCleaveUnitsCount >= 3),
                        Spell.Cast(SB.Whirlwind, ret => U.NearbySlamCleaveUnitsCount < U.NearbyAttackableUnitsCount),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage == Me.MaxRage),
                        new Decorator(ret => G.NonExecuteCheck,
                            Dev_ArmsSt()),
                        new Decorator(ret => G.ExecuteCheck,
                            Dev_ArmsExec()))));
        }

        internal static Composite Dev_ArmsOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.BerserkerRage, ret => (!G.EnrageAura || G.FadingEnrage(500)) && G.ColossusSmashAura && BerserkerRageUsage),
                Spell.Cast(SB.Bloodbath, ret => Tier4AbilityUsage),

                Spell.Cast(SB.Recklessness, ret => RecklessnessUsage),
                Spell.Cast(SB.SkullBanner, ret => !G.SkullBannerAura && RecklessnessSync && SkullBannerUsage),
                Spell.Cast(SB.Avatar, ret => G.AvTalent && RecklessnessSync && Tier6AbilityUsage)
                );
        }

        internal static Composite Dev_ArmsVictorious()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ImpendingVictory, ret => !G.IVOC && G.IvTalent && SG.Instance.Arms.CheckImpVic && Me.HealthPercent <= SG.Instance.Arms.CheckImpVicNum),
                Spell.Cast(SB.VictoryRush, ret => !G.VROC && G.VictoriousAura && SG.Instance.Arms.CheckVicRush && Me.HealthPercent <= SG.Instance.Arms.CheckVicRushNum)
                );
        }

        internal static Composite Dev_ArmsRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite Dev_ArmsDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.DiebytheSword, ret => SG.Instance.Arms.CheckDiebytheSword && Me.HealthPercent <= SG.Instance.Arms.CheckDiebytheSwordNum),
                Spell.Cast(SB.EnragedRegeneration, ret => G.ErTalent && SG.Instance.Arms.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Arms.CheckEnragedRegenNum),
                Spell.Cast(SB.ShieldWall, ret => SG.Instance.Arms.CheckShieldWall && Me.HealthPercent <= SG.Instance.Arms.CheckShieldWallNum),
                Spell.Cast(SB.SpellReflection, ret => SG.Instance.Arms.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                I.ArmsUseHealthStone()
                );
        }

        internal static Composite Dev_ArmsUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SB.DemoralizingBanner, loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Arms.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Arms.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast(SB.Hamstring, ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Arms.HamString == Enum.Hamstring.Always || SG.Instance.Arms.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast(SB.IntimidatingShout, ret => SG.Instance.Arms.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.MassSpellReflection, ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SB.PiercingHowl, ret => G.PhTalent && SG.Instance.Arms.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckPiercingHowlNum),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SB.ShatteringThrow, ret => SG.Instance.Arms.CheckShatteringThrow && U.IsTargetBoss),
                Spell.Cast(SB.StaggeringShout, ret => G.SsTalent && SG.Instance.Arms.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region Release Rotations
        internal static Composite ArmsSt()
        {
            return new PrioritySelector(
                // SimulationCraft 530-6 (r16981) - 2H Arms - Slightly Optimized
                Spell.Cast("Execute", ret => G.DeathScentenceAuraT16),
                Spell.Cast("Heroic Strike", ret => Me.CurrentRage >= Me.MaxRage - 15 || (G.ColossusSmashAura && Me.CurrentRage >= Me.MaxRage - 40 && G.NonExecuteCheck)),
                Spell.Cast("Mortal Strike"),
                Spell.Cast("Dragon Roar", ret => G.DrTalent && (G.BloodbathAura || G.AvTalent || G.SbTalent) && !G.ColossusSmashAura && G.NonExecuteCheck && (
                    (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Storm Bolt", ret => G.SbTalent && G.ColossusSmashAura && (
                    (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Colossus Smash", ret => !G.ColossusSmashAura || G.FadingCs(1000)),
                Spell.Cast("Execute", ret => G.ColossusSmashAura || G.RecklessnessAura || Me.CurrentRage >= Me.MaxRage - 25 || G.DeathScentenceAuraT16),
                Spell.Cast("Dragon Roar", ret => G.DrTalent && (((G.BloodbathAura || G.AvTalent || G.SbTalent) && G.NonExecuteCheck) || (!G.ColossusSmashAura && G.ExecuteCheck)) && (
                    (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Slam", ret => G.ColossusSmashAura && (G.FadingCs(1000) || G.RecklessnessAura) && G.NonExecuteCheck),
                Spell.Cast("Overpower", ret => G.TasteForBloodS3 && G.NonExecuteCheck),
                Spell.Cast("Slam", ret => G.ColossusSmashAura && G.FadingCs(2500) && G.NonExecuteCheck),
                Spell.Cast("Execute", ret => !G.SuddenExecAura || G.DeathScentenceAuraT16),
                Spell.Cast("Overpower", ret => G.NonExecuteCheck || G.SuddenExecAura),
                Spell.Cast("Slam", ret => Me.CurrentRage >= 40 && G.NonExecuteCheck),
                new Switch<Enum.Shouts>(ctx => SG.Instance.Arms.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me))),
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && SG.Instance.Arms.CheckRotImpVic),
                Spell.Cast("Heroic Throw")
                );
        }

        internal static Composite ArmsMt()
        {
            return new PrioritySelector(
                Spell.Cast("Execute", ret => G.DeathScentenceAuraT16),
                Spell.Cast("Sweeping Strikes"),
                Spell.Cast("Cleave", ret => Me.CurrentRage > 110),
                Spell.Cast("Mortal Strike"),
                Spell.Cast("Dragon Roar", ret => G.DrTalent && G.BloodbathAura && (
                    (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Bladestorm", ret => G.BsTalent && (
                    (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && (
                    (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Thunder Clap", ret => U.NearbyAttackableUnitsCount >= 4),
                Spell.Cast("Colossus Smash", ret => G.FadingCs(1000)),
                Spell.Cast("Slam", ret => U.NearbyAttackableUnitsCount <= 3),
                Spell.Cast("Overpower"),
                Spell.Cast("Whirlwind", ret => U.NearbyAttackableUnitsCount >= 4),
                new Switch<Enum.Shouts>(ctx => SG.Instance.Arms.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70)))
                );
        }

        internal static Composite ArmsOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Berserker Rage", ret => !G.EnrageAura && Me.CurrentRage <= Me.MaxRage - 10 && (
                    (SG.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.BerserkerRage == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Bloodbath", ret => G.BbTalent && (
                    (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Recklessness", ret => (
                    (SG.Instance.Arms.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.Recklessness == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Skull Banner", ret => !G.SkullBannerAura && G.RecklessnessAura && (
                    (SG.Instance.Arms.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.SkullBanner == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Avatar", ret => G.AvTalent && G.RecklessnessAura && (
                    (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite ArmsVictorious()
        {
            return new PrioritySelector(
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && SG.Instance.Arms.CheckImpVic && Me.HealthPercent <= SG.Instance.Arms.CheckImpVicNum),
                Spell.Cast("Victory Rush", ret => !G.VROC && !G.IvTalent && G.VictoriousAura && SG.Instance.Arms.CheckVicRush && Me.HealthPercent <= SG.Instance.Arms.CheckVicRushNum)
                );
        }

        internal static Composite ArmsRacials()
        {
            return new PrioritySelector(
                new Decorator(ret =>
                    (SG.Instance.Arms.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.ClassRacials == Enum.AbilityTrigger.Always),
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite ArmsDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Die by the Sword", ret => SG.Instance.Arms.CheckDiebytheSword && Me.HealthPercent <= SG.Instance.Arms.CheckDiebytheSwordNum),
                Spell.Cast("Enraged Regeneration", ret => G.ErTalent && SG.Instance.Arms.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Arms.CheckEnragedRegenNum),
                Spell.Cast("Lifeblood", ret => SG.Instance.Arms.CheckLifeblood && Me.HealthPercent <= SG.Instance.Arms.CheckLifebloodNum),
                Spell.Cast("Shield Wall", ret => SG.Instance.Arms.CheckShieldWall && Me.HealthPercent <= SG.Instance.Arms.CheckShieldWallNum),
                I.ArmsUseHealthStone()
                );
        }

        internal static Composite ArmsUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Arms.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Arms.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast("Hamstring", ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Arms.HamString == Enum.Hamstring.Always || SG.Instance.Arms.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast("Intimidating Shout", ret => SG.Instance.Arms.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast("Mass Spell Reflection", ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && (
                    (SG.Instance.Arms.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.MassSpellReflection == Enum.MsrTrigger.Always && G.PUOC && G.DSOC)
                    )),
                Spell.Cast("Piercing Howl", ret => G.PhTalent && SG.Instance.Arms.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckPiercingHowlNum),
                Spell.Cast("Rallying Cry", ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast("Shattering Throw", ret => SG.Instance.Arms.CheckShatteringThrow && U.IsTargetBoss && (G.CSCD <= 3000 || G.SBCD <= 3000)),
                Spell.Cast("Spell Reflection", ret => SG.Instance.Arms.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Spell.Cast("Staggering Shout", ret => G.SsTalent && SG.Instance.Arms.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckPiercingHowlNum)
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
