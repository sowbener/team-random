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
                        new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        new Decorator(ret => InternalSettings.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                        new Decorator(ret => InternalSettings.Instance.General.CheckPreCombatHk, Global.InitializeOnKeyActions())),
                    new Decorator(ret => Unit.DefaultBuffCheck && ((InternalSettings.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Protection.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, 
                                Spell.Cast(SpellBook.BattleShout, on => Me, ret => !Global.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, 
                                Spell.Cast(SpellBook.CommandingShout, on => Me, ret => !Global.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeProtCombat
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => InternalSettings.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                    new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                    Global.InitializeCaching(),
                    Global.InitializeOnKeyActions(),
                    new Decorator(ret => InternalSettings.Instance.Protection.CheckInterrupts && Unit.CanInterrupt, Global.InitializeInterrupts()),
                    new Switch<Enum.WoWVersion>(ctx => InternalSettings.Instance.General.CrProtRotVersion,
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
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Dev_ProtMt()),
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
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Dev_ProtMt()),
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
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && HotKeyManager.IsAoe && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Dev_ProtMt()),
                                        Dev_ProtSt()
                                        ))))));
            }
        }

        internal static Composite RelProtCombat
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => InternalSettings.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                        new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                        Global.InitializeCaching(),
                        Global.InitializeOnKeyActions(),
                        new Decorator(ret => InternalSettings.Instance.Protection.CheckInterrupts && Unit.CanInterrupt, Global.InitializeInterrupts()),
                        new Decorator(ret => SettingsH.Instance.ModeSelection == Enum.Mode.Auto && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        ProtVictorious(),
                                        ProtUtility(),
                                        Item.CreateItemBehaviour(),
                                        ProtRacials(),
                                        ProtOffensive(),
                                        new Decorator(ret => InternalSettings.Instance.Protection.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckAoENum, ProtMt()),
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
                                        new Decorator(ret => InternalSettings.Instance.Protection.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckAoENum, ProtMt()),
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
                                        new Decorator(ret => HotKeyManager.IsAoe && InternalSettings.Instance.Protection.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckAoENum, ProtMt()),
                                        ProtSt())));
            }
        }
        #endregion

        #region Development Rotations
        internal static Composite Dev_ProtSt()
        {
            return new PrioritySelector(

                Spell.Cast(SpellBook.Execute, ret => Global.ExecutePhase && Me.CurrentRage > 75), // Added
                Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added
                Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && Tier4AbilityUsage), // Added

                Spell.Cast(SpellBook.ShieldSlam),
                Spell.Cast(SpellBook.HeroicStrike, ret => Global.UltimatumAura),
                Spell.Cast(SpellBook.Revenge, ret => Me.CurrentRage != Me.MaxRage),
                Spell.Cast(SpellBook.Devastate, ret => !Global.SunderArmorAura3S || Global.FadingSunder(1500)),
                Spell.Cast(SpellBook.ThunderClap, ret => !Global.WeakenedBlowsAura || Global.FadingWb(1500)),
                new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout))),
                Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage >= Me.MaxRage - 10 && Global.NormalPhase)
                );
        }

        internal static Composite Dev_ProtMt()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.ThunderClap),

                Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage), // Added
                Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && Tier4AbilityAoEUsage), // Added
                Spell.Cast(SpellBook.Shockwave, ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added
                Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Tier4AbilityAoEUsage), // Added

                Spell.Cast(SpellBook.ShieldSlam),
                Spell.Cast(SpellBook.Cleave, ret => Global.UltimatumAura),
                Spell.Cast(SpellBook.Revenge, ret => Me.CurrentRage != Me.MaxRage),
                Spell.Cast(SpellBook.Devastate, ret => !Global.SunderArmorAura3S || Global.FadingSunder(1500)),
                new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout))),
                Spell.Cast(SpellBook.Cleave, ret => Me.CurrentRage >= Me.MaxRage - 10 && Global.NormalPhase)
                );
        }

        internal static Composite Dev_ProtDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.DemoralizingShout, ret => Global.TargettingMe && Me.HealthPercent <= InternalSettings.Instance.Protection.DemoShoutNum && DemoralizingShoutUsage),
                Spell.Cast(SpellBook.EnragedRegeneration, ret => Global.ErTalent && InternalSettings.Instance.Protection.CheckEnragedRegen && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckEnragedRegenNum),
                Spell.Cast(SpellBook.LastStand, ret => InternalSettings.Instance.Protection.CheckLastStand && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckLastStandNum),
                Spell.Cast(SpellBook.MassSpellReflection, ret => Global.MrTalent && InternalSettings.Instance.Protection.CheckSpellReflect && Global.SRCD > 0 && Global.TargetNotNull && Global.TargettingMe && Me.CurrentTarget.IsCasting),
                Spell.Cast(SpellBook.ShieldWall, ret => InternalSettings.Instance.Protection.CheckShieldWall && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckShieldWallNum),
                Spell.Cast(SpellBook.SpellReflection, ret => InternalSettings.Instance.Protection.CheckSpellReflect && Global.TargetNotNull && Global.TargettingMe && Me.CurrentTarget.IsCasting),
                Item.ProtUseHealthStone(),

                // Needs rework!
                new Decorator(ret => HotKeyManager.IsSpecial && Global.HotkeyMode && InternalSettings.Instance.Protection.CheckShieldBlock, 
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ShieldBarrier))),
                new Decorator(ret => !HotKeyManager.IsSpecial && Global.HotkeyMode && InternalSettings.Instance.Protection.CheckShieldBlock, 
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ShieldBlock))),

                Spell.Cast(SpellBook.ShieldBlock, ret => !Global.HotkeyMode && InternalSettings.Instance.Protection.CheckShieldBlock && InternalSettings.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBlock),
                Spell.Cast(SpellBook.ShieldBarrier, ret => !Global.HotkeyMode && InternalSettings.Instance.Protection.CheckShieldBlock && Me.CurrentRage >= 60 && InternalSettings.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBarrier)
                );
        }

        internal static Composite Dev_ProtGcdUtility()
        {
            return new PrioritySelector(
                //138279 Victorious - T15 Proc ID (Victory Rush & Impending Victory).
                //32216	Victorious - Regular Kill Proc ID (Victory Rush & Impending Victory).
                Spell.Cast(SpellBook.IntimidatingShout, ret => InternalSettings.Instance.Protection.CheckIntimidatingShout && Global.IsGlyph && !Unit.IsTargetBoss),
                Spell.Cast(SpellBook.ImpendingVictory, ret => Global.IvTalent && (Me.HealthPercent <= InternalSettings.Instance.Protection.ImpendingVictoryNum || Global.FadingVc(2000)) && ImpendingVictoryUsage),
                Spell.Cast(SpellBook.PiercingHowl, ret => Global.PhTalent && InternalSettings.Instance.Protection.CheckPiercingHowl && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckPiercingHowlNum),
                Spell.Cast(SpellBook.ShatteringThrow, ret => ShatteringThrowUsage),
                Spell.Cast(SpellBook.StaggeringShout, ret => Global.SsTalent && InternalSettings.Instance.Protection.CheckStaggeringShout && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckStaggeringShoutNum),
                Spell.Cast(SpellBook.VictoryRush, ret => !Global.IvTalent && (Me.HealthPercent <= InternalSettings.Instance.Protection.VictoryRushNum || Global.FadingVc(2000)) && VictoryRushUsage)
                );
        }

        internal static Composite Dev_ProtRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(Global.SelectRacialSpell(), ret => Global.SelectRacialSpell() != null && Global.RacialUsageSatisfied(Global.SelectRacialSpell()))
                    ));
        }

        internal static Composite Dev_ProtOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.Avatar, ret => Global.AvTalent && Tier6AbilityUsage),
                Spell.Cast(SpellBook.Bloodbath, ret => Global.BbTalent && Tier6AbilityUsage),
                Spell.Cast(SpellBook.Recklessness, ret => Global.TargetNotNull && RecklessnessUsage),
                Spell.Cast(SpellBook.SkullBanner, ret => !Global.SkullBannerAura && SkullBannerUsage)
                );
        }

        internal static Composite Dev_ProtNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SpellBook.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && InternalSettings.Instance.Protection.CheckDemoBanner && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckDemoBannerNum),
                Spell.Cast(SpellBook.BerserkerRage, ret => !Global.EnrageAura && BerserkerRageUsage),
                Spell.Cast(SpellBook.Taunt, ret => InternalSettings.Instance.Protection.CheckAutoTaunt && !Global.TargettingMe),
                Spell.Cast(SpellBook.RallyingCry, ret => Unit.RaidMembersNeedCryCount > 0 && !Global.LastStandAura && InternalSettings.Instance.Protection.CheckRallyingCry)
                );
        }
        #endregion

        #region Release Rotations
        internal static Composite ProtSt()
        {
            return new PrioritySelector(
                Spell.Cast("Taunt", ret => InternalSettings.Instance.Protection.CheckAutoTaunt && !Global.TargettingMe),
                //Spell.Cast("Execute", ret => G.ExecutePhase && Me.CurrentRage >= 100 && !G.TargettingMe),
                Spell.Cast("Execute", ret => Global.ExecutePhase && Me.CurrentRage > 60),
                Spell.Cast("Heroic Strike", ret => Global.UltimatumAura || (Me.CurrentRage >= Me.MaxRage - 10 && Global.NormalPhase)),
                Spell.Cast("Shield Slam", ret => Me.CurrentRage <= 90),
                Spell.Cast("Revenge", ret => Me.CurrentRage <= 100),
                new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage <= 100)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage <= 100))),
                Spell.Cast("Storm Bolt", ret => Global.SbTalent&& (
                    (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Dragon Roar", ret => Global.DrTalent && (
                    (InternalSettings.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Thunder Clap", ret => !Global.WeakenedBlowsAura),
                Spell.Cast("Devastate")
                );
        }

        internal static Composite ProtMt()
        {
            return new PrioritySelector(
                Spell.Cast("Cleave", ret => Global.UltimatumAura || (Me.CurrentRage >= Me.MaxRage - 10 && !Global.TargettingMe)),
                Spell.Cast("Thunder Clap"),
                Spell.Cast("Dragon Roar", ret => Global.DrTalent && (
                    (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Shockwave", ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Unit.NearbyAttackableUnitsCount >= 3 && (
                    (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Bladestorm", ret => Global.BsTalent && (
                    (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Revenge", ret => Me.CurrentRage <= 100),
                Spell.Cast("Shield Slam", ret => Me.CurrentRage <= 90),
                new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage <= 100)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage <= 100))),
                Spell.Cast("Devastate"),
                Spell.Cast("Intimidating Shout", ret => InternalSettings.Instance.Protection.CheckIntimidatingShout && Global.IsGlyph && !Unit.IsTargetBoss)
                );
        }

        internal static Composite ProtDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Last Stand", ret => InternalSettings.Instance.Protection.CheckLastStand && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckLastStandNum),
                new Decorator(ret => HotKeyManager.IsSpecial && Global.HotkeyMode && InternalSettings.Instance.Protection.CheckShieldBlock, 
                    new PrioritySelector(
                        Spell.Cast("Shield Barrier"))),
                new Decorator(ret => !HotKeyManager.IsSpecial && Global.HotkeyMode && InternalSettings.Instance.Protection.CheckShieldBlock, 
                    new PrioritySelector(
                        Spell.Cast("Shield Block"))),
                Spell.Cast("Shield Block", ret => !Global.HotkeyMode && InternalSettings.Instance.Protection.CheckShieldBlock && InternalSettings.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBlock),
                Spell.Cast("Shield Barrier", ret => !Global.HotkeyMode && InternalSettings.Instance.Protection.CheckShieldBlock && Me.CurrentRage >= 80 && InternalSettings.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBarrier),
                Spell.Cast("Shield Wall", ret => InternalSettings.Instance.Protection.CheckShieldWall && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckShieldWallNum),
                Spell.Cast("Enraged Regeneration", ret => Global.ErTalent && InternalSettings.Instance.Protection.CheckEnragedRegen && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckEnragedRegenNum),
                Spell.Cast("Spell Reflection", ret => InternalSettings.Instance.Protection.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Spell.Cast("Mass Spell Reflection", ret => InternalSettings.Instance.Protection.CheckSpellReflect && Global.SRCD > 0 && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Item.ProtUseHealthStone()
                );
        }

        internal static Composite ProtVictorious()
        {
            return new PrioritySelector(
                //138279 Victorious - T15 Proc ID (Victory Rush & Impending Victory).
                //32216	Victorious - Regular Kill Proc ID (Victory Rush & Impending Victory).
                Spell.Cast("Impending Victory", ret => Global.IvTalent && (Me.HealthPercent <= InternalSettings.Instance.Protection.ImpendingVictoryNum || Global.FadingVc(2000)) && (
                    (InternalSettings.Instance.Protection.ImpendingVictory == Enum.VcTrigger.Always && (Global.VictoriousAura || Global.VictoriousAuraT15)) ||
                    (InternalSettings.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnVictoriousProc && Global.VictoriousAura) ||
                    (InternalSettings.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnT15Proc && Global.VictoriousAuraT15))),
                Spell.Cast("Victory Rush", ret => !Global.IvTalent && (Me.HealthPercent <= InternalSettings.Instance.Protection.VictoryRushNum || Global.FadingVc(2000)) && (
                    (InternalSettings.Instance.Protection.VictoryRush == Enum.VcTrigger.Always && (Global.VictoriousAura || Global.VictoriousAuraT15)) ||
                    (InternalSettings.Instance.Protection.VictoryRush == Enum.VcTrigger.OnVictoriousProc && Global.VictoriousAura) ||
                    (InternalSettings.Instance.Protection.VictoryRush == Enum.VcTrigger.OnT15Proc && Global.VictoriousAuraT15)))
                    );
        }

        internal static Composite ProtUtility()
        {
            return new PrioritySelector(
                Spell.Cast("Hamstring", ret => !Unit.IsTargetBoss && !Global.HamstringAura && (InternalSettings.Instance.Protection.HamString == Enum.Hamstring.Always || InternalSettings.Instance.Protection.HamString == Enum.Hamstring.AddList && Unit.IsHamstringTarget)),
                Spell.Cast("Staggering Shout", ret => Global.SsTalent && InternalSettings.Instance.Protection.CheckPiercingHowl && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckPiercingHowlNum),
                Spell.Cast("Piercing Howl", ret => Global.PhTalent && InternalSettings.Instance.Protection.CheckStaggeringShout && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckPiercingHowlNum),
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                Spell.Cast("Demoralizing Shout", ret => Global.TargettingMe && Me.HealthPercent <= InternalSettings.Instance.Protection.DemoShoutNum && (!Global.Tier15FourPieceBonusT || (Global.Tier15FourPieceBonusT && (!Global.SSOC || Global.SSCD <= 1000))) && (
                    (InternalSettings.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rallying Cry", ret => Unit.RaidMembersNeedCryCount > 0 && !Global.LastStandAura && InternalSettings.Instance.Protection.CheckRallyingCry),
                Spell.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && InternalSettings.Instance.Protection.CheckDemoBanner && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckDemoBannerNum && Unit.IsDoNotUseOnTgt)
                );
        }

        internal static Composite ProtRacials()
        {
            return new PrioritySelector(
                new Decorator(ret =>
                    (InternalSettings.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.ClassRacials == Enum.AbilityTrigger.Always),
                    Spell.Cast(Global.SelectRacialSpell(), ret => Global.SelectRacialSpell() != null && Global.RacialUsageSatisfied(Global.SelectRacialSpell()))
                    ));
        }

        internal static Composite ProtOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Skull Banner", ret => !Global.SkullBannerAura && (
                    (InternalSettings.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.SkullBanner == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Avatar", ret => Global.AvTalent && (
                    (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserker Rage", ret => !Global.EnrageAura && (
                    (InternalSettings.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Bloodbath", ret => Global.BbTalent && (
                    (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Recklessness", ret => Global.TargetNotNull && (
                    (InternalSettings.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.Recklessness == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Shattering Throw", ret => (
                    (InternalSettings.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (InternalSettings.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                    (InternalSettings.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.Always)
                    )));
        }
        #endregion

        #region Booleans

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((InternalSettings.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool DemoralizingShoutUsage
        {
            get
            {
                return ((InternalSettings.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.Always));
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
                return ((InternalSettings.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Protection.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((InternalSettings.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Protection.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool ShatteringThrowUsage
        {
            get
            {
                return ((InternalSettings.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((InternalSettings.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Protection.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((InternalSettings.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                        (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && Global.HasteAbilities) ||
                        (InternalSettings.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool ImpendingVictoryUsage
        {
            get
            {
                return ((InternalSettings.Instance.Protection.ImpendingVictory == Enum.VcTrigger.Always && (Global.VictoriousAura || Global.VictoriousAuraT15)) ||
                        (InternalSettings.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnVictoriousProc && Global.VictoriousAura) ||
                        (InternalSettings.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnT15Proc && Global.VictoriousAuraT15));
            }
        }

        internal static bool VictoryRushUsage
        {
            get
            {
                return ((InternalSettings.Instance.Protection.VictoryRush == Enum.VcTrigger.Always &&(Global.VictoriousAura || Global.VictoriousAuraT15)) ||
                        (InternalSettings.Instance.Protection.VictoryRush == Enum.VcTrigger.OnVictoriousProc && Global.VictoriousAura) ||
                        (InternalSettings.Instance.Protection.VictoryRush == Enum.VcTrigger.OnT15Proc && Global.VictoriousAuraT15));
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