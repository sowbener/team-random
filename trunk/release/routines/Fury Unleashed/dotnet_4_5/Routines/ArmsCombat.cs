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
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => !G.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => !G.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeArmsCombat
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        G.InitializeCaching(),
                        G.InitializeOnKeyActions(),
                        new Decorator(ret => SG.Instance.Arms.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.Auto && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ArmsDefensive()),
                                        ArmsUtility(),
                                        I.CreateItemBehaviour(),
                                        ArmsRacials(),
                                        ArmsOffensive(),
                                        new Decorator(ret => SG.Instance.Arms.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), ArmsMt()),
                                        ArmsSt())),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.SemiHotkey && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ArmsDefensive()),
                                        ArmsUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                    I.CreateItemBehaviour(),
                                                    ArmsRacials(),
                                                    ArmsOffensive())),
                                        new Decorator(ret => SG.Instance.Arms.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), ArmsMt()),
                                        ArmsSt())),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.Hotkey && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, ArmsDefensive()),
                                        ArmsUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                    I.CreateItemBehaviour(),
                                                    ArmsRacials(),
                                                    ArmsOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Arms.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), ArmsMt()),
                                        ArmsSt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite ArmsSt()
        {
            return new PrioritySelector(
                // SimulationCraft 530-6 (r16981) - 2H Arms
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
                Spell.Cast("Execute", ret => G.ColossusSmashAura || G.RecklessnessAura || Me.CurrentRage >= Me.MaxRage - 25),
                Spell.Cast("Dragon Roar", ret => G.DrTalent && (((G.BloodbathAura || G.AvTalent || G.SbTalent) && G.NonExecuteCheck) || (!G.ColossusSmashAura && G.ExecuteCheck)) && (
                    (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.Tier4Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Slam", ret => G.ColossusSmashAura && (G.FadingCs(1000) || G.RecklessnessAura) && G.NonExecuteCheck),
                Spell.Cast("Overpower", ret => G.TasteForBloodS3 && G.NonExecuteCheck),
                Spell.Cast("Slam", ret => G.ColossusSmashAura && G.FadingCs(2500) && G.NonExecuteCheck),
                Spell.Cast("Execute", ret => !G.SuddenExecAura),
                Spell.Cast("Overpower", ret => G.NonExecuteCheck || G.SuddenExecAura),
                Spell.Cast("Slam", ret => Me.CurrentRage >= 40 && G.NonExecuteCheck),
                new Switch<Enum.Shouts>(ctx => SG.Instance.Arms.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me))),
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && SG.Instance.Arms.CheckRotImpVic),
                Spell.Cast("Heroic Throw", ret => SG.Instance.Arms.CheckHeroicThrow)
                );
        }

        // SimulationCraft 530-6 (r16981) - 2H Arms
        internal static Composite ArmsMt()
        {
            return new PrioritySelector(
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
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && SG.Instance.Arms.CheckImpVic && Me.HealthPercent <= SG.Instance.Arms.CheckImpVicNum),
                Spell.Cast("Lifeblood", ret => SG.Instance.Arms.CheckLifeblood && Me.HealthPercent <= SG.Instance.Arms.CheckLifebloodNum),
                Spell.Cast("Shield Wall", ret => SG.Instance.Arms.CheckShieldWall && Me.HealthPercent <= SG.Instance.Arms.CheckShieldWallNum),
                Spell.Cast("Victory Rush", ret => !G.VROC && !G.IvTalent && G.VictoriousAura && SG.Instance.Arms.CheckImpVic && Me.HealthPercent <= SG.Instance.Arms.CheckImpVicNum),
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

        }
}
