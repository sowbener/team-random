using System.Windows.Forms;
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
    class ProtRotation
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeProtPreCombat
        {
            get
            {
                return new PrioritySelector(
                    new PrioritySelector(ret => !Me.Combat,
                        //new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        //new Decorator(ret => IS.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                        new Decorator(ret => IS.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions())),
                    new Decorator(ret => U.DefaultBuffCheck && ((IS.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Protection.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, 
                                Spell.Cast(SB.BattleShout, on => Me, ret => !G.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, 
                                Spell.Cast(SB.CommandingShout, on => Me, ret => !G.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeProtCombat
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => IS.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                    new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                    G.InitializeCaching(),
                    G.InitializeOnKeyActions(),
                    new Decorator(ret => IS.Instance.Protection.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                    new Switch<Enum.WoWVersion>(ctx => IS.Instance.General.CrProtRotVersion,
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Development, DevProtCombat),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Release, RelProtCombat)));
            }
        }

        internal static Composite DevProtCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ProtDefensive()),
                                Dev_ProtNonGcdUtility(),
                                Dev_ProtRacials(),
                                Dev_ProtOffensive(),
                                Item.CreateItemBehaviour(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ProtGcdUtility(),
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ProtMt()),
                                        Dev_ProtSt()
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ProtDefensive()),
                                Dev_ProtNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_ProtRacials(),
                                        Dev_ProtOffensive(),
                                        Item.CreateItemBehaviour())),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ProtGcdUtility(),
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ProtMt()),
                                        Dev_ProtSt()
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ProtDefensive()),
                                Dev_ProtNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_ProtRacials(),
                                        Dev_ProtOffensive(),
                                        Item.CreateItemBehaviour())),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ProtGcdUtility(),
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ProtMt()),
                                        Dev_ProtSt()
                                        ))))));
            }
        }

        internal static Composite RelProtCombat
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => IS.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                        G.InitializeCaching(),
                        G.InitializeOnKeyActions(),
                        new Decorator(ret => IS.Instance.Protection.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                        new Decorator(ret => SettingsH.Instance.ModeSelection == Enum.Mode.Auto && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        ProtVictorious(),
                                        ProtUtility(),
                                        Item.CreateItemBehaviour(),
                                        ProtRacials(),
                                        ProtOffensive(),
                                        new Decorator(ret => IS.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckAoENum, ProtMt()),
                                        ProtSt())),
                        new Decorator(ret => SettingsH.Instance.ModeSelection == Enum.Mode.SemiHotkey && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        ProtVictorious(),
                                        ProtUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        Item.CreateItemBehaviour(),
                                                        ProtRacials(),
                                                        ProtOffensive())),
                                        new Decorator(ret => IS.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckAoENum, ProtMt()),
                                        ProtSt())),
                        new Decorator(ret => SettingsH.Instance.ModeSelection == Enum.Mode.Hotkey && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        ProtVictorious(),
                                        ProtUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        Item.CreateItemBehaviour(),
                                                        ProtRacials(),
                                                        ProtOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe && IS.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckAoENum, ProtMt()),
                                        ProtSt())));
            }
        }
        #endregion

        #region Development Rotations
        internal static Composite Dev_ProtSt()
        {
            return new PrioritySelector(

                Spell.Cast(SB.Execute, ret => G.ExecutePhase && Me.CurrentRage > 75), // Added
                Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                Spell.Cast(SB.DragonRoar, ret => G.DrTalent && Tier4AbilityUsage), // Added

                Spell.Cast(SB.ShieldSlam),
                Spell.Cast(SB.HeroicStrike, ret => G.UltimatumAura),
                Spell.Cast(SB.Revenge, ret => Me.CurrentRage != Me.MaxRage),
                Spell.Cast(SB.Devastate, ret => !G.WeakenedArmor3S || G.FadingSunder(1500)),
                Spell.Cast(SB.ThunderClap, ret => !G.WeakenedBlowsAura || G.FadingWb(1500)),
                new Switch<Enum.Shouts>(ctx => IS.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout))),
                Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage >= Me.MaxRage - 10 && G.NormalPhase)
                );
        }

        internal static Composite Dev_ProtMt()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ThunderClap),

                Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                Spell.Cast(SB.DragonRoar, ret => G.DrTalent && Tier4AbilityAoEUsage), // Added
                Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added
                Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage), // Added

                Spell.Cast(SB.ShieldSlam),
                Spell.Cast(SB.Cleave, ret => G.UltimatumAura),
                Spell.Cast(SB.Revenge, ret => Me.CurrentRage != Me.MaxRage),
                Spell.Cast(SB.Devastate, ret => !G.WeakenedArmor3S || G.FadingSunder(1500)),
                new Switch<Enum.Shouts>(ctx => IS.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout))),
                Spell.Cast(SB.Cleave, ret => Me.CurrentRage >= Me.MaxRage - 10 && G.NormalPhase)
                );
        }

        internal static Composite Dev_ProtDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.DemoralizingShout, ret => G.TargettingMe && Me.HealthPercent <= IS.Instance.Protection.DemoShoutNum && DemoralizingShoutUsage),
                Spell.Cast(SB.EnragedRegeneration, ret => G.ErTalent && IS.Instance.Protection.CheckEnragedRegen && Me.HealthPercent <= IS.Instance.Protection.CheckEnragedRegenNum),
                Spell.Cast(SB.LastStand, ret => IS.Instance.Protection.CheckLastStand && Me.HealthPercent <= IS.Instance.Protection.CheckLastStandNum),
                Spell.Cast(SB.MassSpellReflection, ret => G.MrTalent && IS.Instance.Protection.CheckSpellReflect && G.SrCd > 0 && G.TargetNotNull && G.TargettingMe && Me.CurrentTarget.IsCasting),
                Spell.Cast(SB.ShieldWall, ret => IS.Instance.Protection.CheckShieldWall && Me.HealthPercent <= IS.Instance.Protection.CheckShieldWallNum),
                Spell.Cast(SB.SpellReflection, ret => IS.Instance.Protection.CheckSpellReflect && G.TargetNotNull && G.TargettingMe && Me.CurrentTarget.IsCasting),
                Item.ProtUseHealthStone(),

                // Needs rework!
                new Decorator(ret => HotKeyManager.IsSpecial && G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock, 
                    new PrioritySelector(
                        Spell.Cast(SB.ShieldBarrier))),
                new Decorator(ret => !HotKeyManager.IsSpecial && G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock, 
                    new PrioritySelector(
                        Spell.Cast(SB.ShieldBlock))),

                Spell.Cast(SB.ShieldBlock, ret => !G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock && IS.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBlock),
                Spell.Cast(SB.ShieldBarrier, ret => !G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock && Me.CurrentRage >= 60 && IS.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBarrier)
                );
        }

        internal static Composite Dev_ProtGcdUtility()
        {
            return new PrioritySelector(
                //138279 Victorious - T15 Proc ID (Victory Rush & Impending Victory).
                //32216	Victorious - Regular Kill Proc ID (Victory Rush & Impending Victory).
                Spell.Cast(SB.IntimidatingShout, ret => IS.Instance.Protection.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.ImpendingVictory, ret => G.IvTalent && (Me.HealthPercent <= IS.Instance.Protection.ImpendingVictoryNum || G.FadingVc(2000)) && ImpendingVictoryUsage),
                Spell.Cast(SB.PiercingHowl, ret => G.PhTalent && IS.Instance.Protection.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckPiercingHowlNum),
                Spell.Cast(SB.ShatteringThrow, ret => ShatteringThrowUsage),
                Spell.Cast(SB.StaggeringShout, ret => G.SsTalent && IS.Instance.Protection.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckStaggeringShoutNum),
                Spell.Cast(SB.VictoryRush, ret => !G.IvTalent && (Me.HealthPercent <= IS.Instance.Protection.VictoryRushNum || G.FadingVc(2000)) && VictoryRushUsage)
                );
        }

        internal static Composite Dev_ProtRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite Dev_ProtOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.Avatar, ret => G.AvTalent && Tier6AbilityUsage),
                Spell.Cast(SB.Bloodbath, ret => G.BbTalent && Tier6AbilityUsage),
                Spell.Cast(SB.Recklessness, ret => G.TargetNotNull && RecklessnessUsage),
                Spell.Cast(SB.SkullBanner, ret => !G.SkullBannerAura && SkullBannerUsage)
                );
        }

        internal static Composite Dev_ProtNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SB.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && IS.Instance.Protection.CheckDemoBanner && Me.HealthPercent <= IS.Instance.Protection.CheckDemoBannerNum),
                Spell.Cast(SB.BerserkerRage, ret => !G.EnrageAura && BerserkerRageUsage),
                Spell.Cast(SB.Taunt, ret => IS.Instance.Protection.CheckAutoTaunt && !G.TargettingMe),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0 && !G.LastStandAura && IS.Instance.Protection.CheckRallyingCry)
                );
        }
        #endregion

        #region Release Rotations
        internal static Composite ProtSt()
        {
            return new PrioritySelector(
                Spell.Cast("Taunt", ret => IS.Instance.Protection.CheckAutoTaunt && !G.TargettingMe),
                //Spell.Cast("Execute", ret => G.ExecutePhase && Me.CurrentRage >= 100 && !G.TargettingMe),
                Spell.Cast("Execute", ret => G.ExecutePhase && Me.CurrentRage > 60),
                Spell.Cast("Heroic Strike", ret => G.UltimatumAura || (Me.CurrentRage >= Me.MaxRage - 10 && G.NormalPhase)),
                Spell.Cast("Shield Slam", ret => Me.CurrentRage <= 90),
                Spell.Cast("Revenge", ret => Me.CurrentRage <= 100),
                new Switch<Enum.Shouts>(ctx => IS.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage <= 100)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage <= 100))),
                Spell.Cast("Storm Bolt", ret => G.SbTalent&& (
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Dragon Roar", ret => G.DrTalent && (
                    (IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Thunder Clap", ret => !G.WeakenedBlowsAura),
                Spell.Cast("Devastate")
                );
        }

        internal static Composite ProtMt()
        {
            return new PrioritySelector(
                Spell.Cast("Cleave", ret => G.UltimatumAura || (Me.CurrentRage >= Me.MaxRage - 10 && !G.TargettingMe)),
                Spell.Cast("Thunder Clap"),
                Spell.Cast("Dragon Roar", ret => G.DrTalent && (
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && U.NearbyAttackableUnitsCount >= 3 && (
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Bladestorm", ret => G.BsTalent && (
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Revenge", ret => Me.CurrentRage <= 100),
                Spell.Cast("Shield Slam", ret => Me.CurrentRage <= 90),
                new Switch<Enum.Shouts>(ctx => IS.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage <= 100)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage <= 100))),
                Spell.Cast("Devastate"),
                Spell.Cast("Intimidating Shout", ret => IS.Instance.Protection.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss)
                );
        }

        internal static Composite ProtDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Last Stand", ret => IS.Instance.Protection.CheckLastStand && Me.HealthPercent <= IS.Instance.Protection.CheckLastStandNum),
                new Decorator(ret => HotKeyManager.IsSpecial && G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock, 
                    new PrioritySelector(
                        Spell.Cast("Shield Barrier"))),
                new Decorator(ret => !HotKeyManager.IsSpecial && G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock, 
                    new PrioritySelector(
                        Spell.Cast("Shield Block"))),
                Spell.Cast("Shield Block", ret => !G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock && IS.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBlock),
                Spell.Cast("Shield Barrier", ret => !G.HotkeyMode && IS.Instance.Protection.CheckShieldBlock && Me.CurrentRage >= 80 && IS.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBarrier),
                Spell.Cast("Shield Wall", ret => IS.Instance.Protection.CheckShieldWall && Me.HealthPercent <= IS.Instance.Protection.CheckShieldWallNum),
                Spell.Cast("Enraged Regeneration", ret => G.ErTalent && IS.Instance.Protection.CheckEnragedRegen && Me.HealthPercent <= IS.Instance.Protection.CheckEnragedRegenNum),
                Spell.Cast("Spell Reflection", ret => IS.Instance.Protection.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Spell.Cast("Mass Spell Reflection", ret => IS.Instance.Protection.CheckSpellReflect && G.SrCd > 0 && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Item.ProtUseHealthStone()
                );
        }

        internal static Composite ProtVictorious()
        {
            return new PrioritySelector(
                //138279 Victorious - T15 Proc ID (Victory Rush & Impending Victory).
                //32216	Victorious - Regular Kill Proc ID (Victory Rush & Impending Victory).
                Spell.Cast("Impending Victory", ret => G.IvTalent && (Me.HealthPercent <= IS.Instance.Protection.ImpendingVictoryNum || G.FadingVc(2000)) && (
                    (IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.Always && (G.VictoriousAura || G.VictoriousAuraT15)) ||
                    (IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnVictoriousProc && G.VictoriousAura) ||
                    (IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnT15Proc && G.VictoriousAuraT15))),
                Spell.Cast("Victory Rush", ret => !G.IvTalent && (Me.HealthPercent <= IS.Instance.Protection.VictoryRushNum || G.FadingVc(2000)) && (
                    (IS.Instance.Protection.VictoryRush == Enum.VcTrigger.Always && (G.VictoriousAura || G.VictoriousAuraT15)) ||
                    (IS.Instance.Protection.VictoryRush == Enum.VcTrigger.OnVictoriousProc && G.VictoriousAura) ||
                    (IS.Instance.Protection.VictoryRush == Enum.VcTrigger.OnT15Proc && G.VictoriousAuraT15)))
                    );
        }

        internal static Composite ProtUtility()
        {
            return new PrioritySelector(
                Spell.Cast("Hamstring", ret => !U.IsTargetBoss && !G.HamstringAura && (IS.Instance.Protection.HamString == Enum.Hamstring.Always || IS.Instance.Protection.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast("Staggering Shout", ret => G.SsTalent && IS.Instance.Protection.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckPiercingHowlNum),
                Spell.Cast("Piercing Howl", ret => G.PhTalent && IS.Instance.Protection.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckPiercingHowlNum),
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                Spell.Cast("Demoralizing Shout", ret => G.TargettingMe && Me.HealthPercent <= IS.Instance.Protection.DemoShoutNum && (!G.Tier15FourPieceBonusT || (G.Tier15FourPieceBonusT && (!G.SsOc || G.SsCd <= 1000))) && (
                    (IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rallying Cry", ret => U.RaidMembersNeedCryCount > 0 && !G.LastStandAura && IS.Instance.Protection.CheckRallyingCry),
                Spell.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && IS.Instance.Protection.CheckDemoBanner && Me.HealthPercent <= IS.Instance.Protection.CheckDemoBannerNum && U.IsDoNotUseOnTgt)
                );
        }

        internal static Composite ProtRacials()
        {
            return new PrioritySelector(
                new Decorator(ret =>
                    (IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.Always),
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite ProtOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Skull Banner", ret => !G.SkullBannerAura && (
                    (IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Avatar", ret => G.AvTalent && (
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserker Rage", ret => !G.EnrageAura && (
                    (IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Bloodbath", ret => G.BbTalent && (
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Recklessness", ret => G.TargetNotNull && (
                    (IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Shattering Throw", ret => (
                    (IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.Always)
                    )));
        }
        #endregion

        #region Booleans

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool DemoralizingShoutUsage
        {
            get
            {
                return ((IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.Always));
            }
        }

        //internal static bool MassSpellReflectionUsage
        //{
        //    get
        //    {
        //        return ((SG.Instance.Protection.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
        //                (SG.Instance.Protection.MassSpellReflection == Enum.MsrTrigger.Always && G.PUOC && G.DSOC));
        //    }
        //}

        internal static bool RacialUsage
        {
            get
            {
                return ((IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool ShatteringThrowUsage
        {
            get
            {
                return ((IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool ImpendingVictoryUsage
        {
            get
            {
                return ((IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.Always && (G.VictoriousAura || G.VictoriousAuraT15)) ||
                        (IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnVictoriousProc && G.VictoriousAura) ||
                        (IS.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnT15Proc && G.VictoriousAuraT15));
            }
        }

        internal static bool VictoryRushUsage
        {
            get
            {
                return ((IS.Instance.Protection.VictoryRush == Enum.VcTrigger.Always &&(G.VictoriousAura || G.VictoriousAuraT15)) ||
                        (IS.Instance.Protection.VictoryRush == Enum.VcTrigger.OnVictoriousProc && G.VictoriousAura) ||
                        (IS.Instance.Protection.VictoryRush == Enum.VcTrigger.OnT15Proc && G.VictoriousAuraT15));
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