using CommonBehaviors.Actions;
using FuryUnleashed.Shared.Helpers;
using FuryUnleashed.Shared.Managers;
using FuryUnleashed.Shared.Utilities;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = FuryUnleashed.Routines.FuGlobal;
using I = FuryUnleashed.Core.Item;
using SG = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SH = FuryUnleashed.Interfaces.Settings.SettingsH;
using Spell = FuryUnleashed.Core.Spell;
using U = FuryUnleashed.Core.Unit;

namespace FuryUnleashed.Routines
{
    class ProtCombat
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
                        new Decorator(ret => SG.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                        new Decorator(ret => SG.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions())),
                    new Decorator(ret => U.DefaultBuffCheck && ((SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Protection.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => !G.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => !G.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeProtCombat
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        G.InitializeCaching(),
                        G.InitializeOnKeyActions(),
                        new Decorator(ret => SG.Instance.Protection.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.Auto && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        ProtVictorious(),
                                        ProtUtility(),
                                        I.CreateItemBehaviour(),
                                        ProtRacials(),
                                        ProtOffensive(),
                                        new Decorator(ret => SG.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= 2, ProtMt()),
                                        ProtSt())),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.SemiHotkey && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        ProtVictorious(),
                                        ProtUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.CreateItemBehaviour(),
                                                        ProtRacials(),
                                                        ProtOffensive())),
                                        new Decorator(ret => SG.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= 2, ProtMt()),
                                        ProtSt())),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.Hotkey && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ProtDefensive()),
                                        ProtVictorious(),
                                        ProtUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.CreateItemBehaviour(),
                                                        ProtRacials(),
                                                        ProtOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= 2, ProtMt()),
                                        ProtSt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite ProtSt()
        {
            return new PrioritySelector(
                Spell.Cast("Taunt", ret => SG.Instance.Protection.CheckAutoTaunt && !G.TargettingMe),
                Spell.Cast("Execute", ret => G.ExecuteCheck && Me.CurrentRage >= 100 && !G.TargettingMe),
                Spell.Cast("Heroic Strike", ret => G.UltimatumAura || (Me.CurrentRage >= Me.MaxRage - 10 && !G.TargettingMe && G.NonExecuteCheck)),
                Spell.Cast("Shield Slam", ret => Me.CurrentRage <= 90),
                Spell.Cast("Revenge", ret => Me.CurrentRage <= 100),
                new Switch<Enum.Shouts>(ctx => SG.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage <= 100)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage <= 100))),
                Spell.Cast("Storm Bolt", ret => G.SbTalent&& (
                    (SG.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Dragon Roar", ret => G.DrTalent && (
                    (SG.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.Tier4Abilities == Enum.AbilityTrigger.Always)
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
                    (SG.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && U.NearbyAttackableUnitsCount >= 3 && (
                    (SG.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Bladestorm", ret => G.BsTalent && (
                    (SG.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Revenge", ret => Me.CurrentRage <= 100),
                Spell.Cast("Shield Slam", ret => Me.CurrentRage <= 90),
                new Switch<Enum.Shouts>(ctx => SG.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage <= 100)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage <= 100))),
                Spell.Cast("Devastate"),
                Spell.Cast("Intimidating Shout", ret => SG.Instance.Protection.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss)
                );
        }

        internal static Composite ProtDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Last Stand", ret => SG.Instance.Protection.CheckLastStand && Me.HealthPercent <= SG.Instance.Protection.CheckLastStandNum),
                new Decorator(ret => HotKeyManager.IsSpecial && G.HotkeyMode && SG.Instance.Protection.CheckShieldBlock, 
                    new PrioritySelector(
                        Spell.Cast("Shield Barrier"))),
                new Decorator(ret => !HotKeyManager.IsSpecial && G.HotkeyMode && SG.Instance.Protection.CheckShieldBlock, 
                    new PrioritySelector(
                        Spell.Cast("Shield Block"))),
                Spell.Cast("Shield Block", ret => !G.HotkeyMode && SG.Instance.Protection.CheckShieldBlock && SG.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBlock),
                Spell.Cast("Shield Barrier", ret => !G.HotkeyMode && SG.Instance.Protection.CheckShieldBlock && Me.CurrentRage >= 80 && SG.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBarrier),
                Spell.Cast("Shield Wall", ret => SG.Instance.Protection.CheckShieldWall && Me.HealthPercent <= SG.Instance.Protection.CheckShieldWallNum),
                Spell.Cast("Enraged Regeneration", ret => G.ErTalent && SG.Instance.Protection.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Protection.CheckEnragedRegenNum),
                Spell.Cast("Spell Reflection", ret => SG.Instance.Protection.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Spell.Cast("Mass Spell Reflection", ret => SG.Instance.Protection.CheckSpellReflect && G.SRCD > 0 && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                I.ProtUseHealthStone()
                );
        }

        internal static Composite ProtVictorious()
        {
            return new PrioritySelector(
                //138279 Victorious - T15 Proc ID (Victory Rush & Impending Victory).
                //32216	Victorious - Regular Kill Proc ID (Victory Rush & Impending Victory).
                Spell.Cast("Impending Victory", ret => G.IvTalent && (Me.HealthPercent <= SG.Instance.Protection.ImpendingVictoryNum || G.FadingVc(2000)) && (
                    (SG.Instance.Protection.ImpendingVictory == Enum.VcTrigger.Always && (G.VictoriousAura || G.VictoriousT15P2Aura)) ||
                    (SG.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnVictoriousProc && G.VictoriousAura) ||
                    (SG.Instance.Protection.ImpendingVictory == Enum.VcTrigger.OnT15Proc && G.VictoriousT15P2Aura))),
                Spell.Cast("Victory Rush", ret => !G.IvTalent && (Me.HealthPercent <= SG.Instance.Protection.VictoryRushNum || G.FadingVc(2000)) && (
                    (SG.Instance.Protection.VictoryRush == Enum.VcTrigger.Always && (G.VictoriousAura || G.VictoriousT15P2Aura)) ||
                    (SG.Instance.Protection.VictoryRush == Enum.VcTrigger.OnVictoriousProc && G.VictoriousAura) ||
                    (SG.Instance.Protection.VictoryRush == Enum.VcTrigger.OnT15Proc && G.VictoriousT15P2Aura)))
                    );
        }

        internal static Composite ProtUtility()
        {
            return new PrioritySelector(
                Spell.Cast("Hamstring", ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Protection.HamString == Enum.Hamstring.Always || SG.Instance.Protection.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast("Staggering Shout", ret => G.SsTalent && SG.Instance.Protection.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Protection.CheckPiercingHowlNum),
                Spell.Cast("Piercing Howl", ret => G.PhTalent && SG.Instance.Protection.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Protection.CheckPiercingHowlNum),
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                Spell.Cast("Demoralizing Shout", ret => G.TargettingMe && Me.HealthPercent <= SG.Instance.Protection.DemoShoutNum && (!G.Tier15FourPieceBonusT || (G.Tier15FourPieceBonusT && (!G.SSOC || G.SSCD <= 1000))) && (
                    (SG.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.DemoralizeShout == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rallying Cry", ret => U.RaidMembersNeedCryCount > 0 && !G.LastStandAura),
                Spell.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Protection.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Protection.CheckDemoBannerNum && U.IsDoNotUseOnTgt)
                );
        }

        internal static Composite ProtRacials()
        {
            return new PrioritySelector(
                new Decorator(ret =>
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.ClassRacials == Enum.AbilityTrigger.Always),
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite ProtOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Skull Banner", ret => !G.SkullBannerAura && (
                    (SG.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.SkullBanner == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Avatar", ret => G.AvTalent && (
                    (SG.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserker Rage", ret => !G.EnrageAura && (
                    (SG.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.BerserkerRage == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Bloodbath", ret => G.BbTalent && (
                    (SG.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Recklessness", ret => G.TargetNotNull && (
                    (SG.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.Recklessness == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Shattering Throw", ret => (
                    (SG.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Protection.ShatteringThrow == Enum.AbilityTrigger.Always)
                    )));
        }
        #endregion

    }
}