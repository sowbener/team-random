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
    class FuryCombat
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeFuryPreCombat
        {
            get
            {
                return new PrioritySelector(
                    new PrioritySelector(ret => !Me.Combat,
                        new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        new Decorator(ret => SG.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                        new Decorator(ret => SG.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions())),
                    new Decorator(ret => U.DefaultBuffCheck && ((SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => !G.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => !G.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeFuryCombat
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        G.InitializeCaching(),
                        G.InitializeOnKeyActions(),
                        new Decorator(ret => SG.Instance.Fury.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.Auto && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                        FuryUtility(),
                                        I.CreateItemBehaviour(),
                                        FuryRacials(),
                                        FuryOffensive(),
                                        new Decorator(ret => SG.Instance.Fury.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), FuryMt()),
                                        FurySt())),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.SemiHotkey && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                        FuryUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.CreateItemBehaviour(),
                                                        FuryRacials(),
                                                        FuryOffensive())),
                                        new Decorator(ret => SG.Instance.Fury.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), FuryMt()),
                                        FurySt())),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.Hotkey && !Spell.IsGlobalCooldown(),
                                new PrioritySelector(
                                        new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                        FuryUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.CreateItemBehaviour(),
                                                        FuryRacials(),
                                                        FuryOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Fury.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), FuryMt()),
                                        FurySt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite FurySt()
        {
            return new PrioritySelector(
                // SimulationCraft 530-3 (r16559) - 1H/2H Fury - Slightly Optimized.
                new Decorator(ret => G.AlmostDead,
                        new PrioritySelector(
                                Spell.Cast("Colossus Smash", ret => G.ColossusSmashAura),
                                Spell.Cast("Execute"),
                                Spell.Cast("Heroic Strike", ret => G.DumpAllRage && Me.CurrentRage > 60)
                                )),
                Spell.Cast("Heroic Strike", ret => Me.CurrentRage >= 110 || (G.NonExecuteCheck && G.ColossusSmashAura && Me.CurrentRage >= 40)),
                Spell.Cast("Raging Blow", ret => G.RagingBlowStacks && G.ColossusSmashAura && G.NonExecuteCheck),
                Spell.Cast("Bloodthirst"),
                Spell.Cast("Wild Strike", ret => G.BloodsurgeAura && G.NonExecuteCheck && G.BTCD <= 1000),
                Spell.Cast("Dragon Roar", ret => G.DrTalent && (G.BloodbathAura || G.AvTalent || G.SbTalent) && !G.ColossusSmashAura && (
                    (SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Colossus Smash"),
                Spell.Cast("Execute", ret => G.ExecuteCheck),
                Spell.Cast("Storm Bolt", ret => G.SbTalent && (
                    (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Raging Blow", ret => G.RagingBlowStacks || (G.RagingBlowAura && (G.ColossusSmashAura || (G.BTCD >= 1000 && G.FadingRb(3000))))),
                Spell.Cast("Wild Strike", ret => G.BloodsurgeAura),
                Spell.Cast("Raging Blow", ret => G.RagingBlowAura && G.CSCD >= 3000),
                Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && (
                    (SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Heroic Throw", ret => SG.Instance.Fury.CheckHeroicThrow && !G.ColossusSmashAura),
                new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70 && !G.ColossusSmashAura)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70 && !G.ColossusSmashAura))),
                Spell.Cast("Wild Strike", ret => !G.WieldsTwoHandedWeapons && G.ColossusSmashAura && G.NonExecuteCheck),
                Spell.Cast("Whirlwind", ret => G.WieldsTwoHandedWeapons && G.ColossusSmashAura && G.NonExecuteCheck),
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && G.NonExecuteCheck && ((SG.Instance.Fury.CheckRotImpVic) || (SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum))),
                Spell.Cast("Wild Strike", ret => !G.WieldsTwoHandedWeapons && G.CSCD >= 2000 && Me.CurrentRage >= 80 && G.NonExecuteCheck),
                Spell.Cast("Whirlwind", ret => G.WieldsTwoHandedWeapons && G.CSCD >= 2000 && Me.CurrentRage >= 80 && G.NonExecuteCheck),
                new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70)))
                );
        }

        // SimulationCraft 530-6 (r16981) - 1H/2H Fury
        internal static Composite FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast("Cleave", ret => Me.CurrentRage >= 110),
                        Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && (G.BloodbathAura || G.AvTalent || G.SbTalent) && (
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always))),
                        Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && (
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always))),
                        Spell.Cast("Bladestorm", ret => G.BsTalent && (
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always))),
                        Spell.Cast("Execute", ret => G.ColossusSmashAura),
                        Spell.Cast("Bloodthirst"),
                        Spell.Cast("Colossus Smash"),
                        Spell.Cast("Raging Blow", ret => G.MeatCleaverAuraS1),
                        Spell.Cast("Whirlwind", ret => !G.MeatCleaverAuraS1),
                        Spell.Cast("Raging Blow", ret => G.RagingBlowStacks || (G.RagingBlowAura && (G.ColossusSmashAura || G.CSCD >= 3000 || G.FadingRb(3000)))),
                        Spell.Cast("Storm Bolt", ret => G.SbTalent && (
                            (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always)
                            )),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70))),
                        Spell.Cast("Heroic Throw", ret => SG.Instance.Fury.CheckHeroicThrow))),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(
                        Spell.Cast("Cleave", ret => Me.CurrentRage >= 110),
                        Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && (G.BloodbathAura || G.AvTalent || G.SbTalent) && (
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always))),
                        Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && (
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always))),
                        Spell.Cast("Bladestorm", ret => G.BsTalent && G.EnrageAura && (
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always))),
                        Spell.Cast("Raging Blow", ret => G.FadingRb(2000) || G.MeatCleaverAuraS2),
                        Spell.Cast("Bloodthirst"),
                        Spell.Cast("Whirlwind"),
                        Spell.Cast("Colossus Smash"),
                        Spell.Cast("Storm Bolt", ret => G.SbTalent && (
                            (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always)
                            )),
                        Spell.Cast("Raging Blow", ret => G.RagingBlowStacks || (G.RagingBlowAura && (G.ColossusSmashAura || G.CSCD >= 3000 || G.FadingRb(3000)))),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70))),
                        Spell.Cast("Heroic Throw", ret => SG.Instance.Fury.CheckHeroicThrow))),
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 4,
                    new PrioritySelector(
                        Spell.Cast("Cleave", ret => Me.CurrentRage >= 110),
                        Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && (G.BloodbathAura || G.AvTalent || G.SbTalent) && (
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                            )),
                        Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && (
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                            )),
                        Spell.Cast("Bladestorm", ret => G.BsTalent && G.EnrageAura && (
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always)
                            )),
                        Spell.Cast("Bloodthirst", ret => !G.EnrageAura),
                        Spell.Cast("Colossus Smash"),
                        Spell.Cast("Raging Blow", ret => G.MeatCleaverAuraS3),
                        Spell.Cast("Whirlwind"),
                        Spell.Cast("Storm Bolt", ret => G.SbTalent && (
                            (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                            (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                            (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always)
                            )),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70))),
                        Spell.Cast("Bloodthirst"))));
        }

        internal static Composite FuryOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Bloodbath", ret => G.BbTalent && (G.CSCD < 2000 || G.ColossusSmashAuraT) && (
                    (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Recklessness", ret => (G.ColossusSmashAuraT || G.CSCD <= 4000) && ((G.SbTalent) || (G.AvTalent && (G.AvatarAura || G.AVCD <= 3000)) || (G.BbTalent && (G.BloodbathAura || G.BBCD <= 3000))) && (
                    (SG.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Fury.Recklessness == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Avatar", ret => G.AvTalent && (G.RecklessnessAura || G.RCCD >= 180000) && (
                    (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Skull Banner", ret => (!G.SkullBannerAura && (G.RCCD >= 180000 || G.RecklessnessAura)) && (
                    (SG.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Fury.SkullBanner == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserker Rage", ret => (!G.EnrageAura || (G.RagingBlowStacks && G.NonExecuteCheck) || (G.RecklessnessAuraT && !G.RagingBlowAura)) && (
                    (SG.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret =>
                    (SG.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Fury.ClassRacials == Enum.AbilityTrigger.Always),
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite FuryDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Die by the Sword", ret => SG.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= SG.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast("Enraged Regeneration", ret => G.ErTalent && SG.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum),
                Spell.Cast("Lifeblood", ret => SG.Instance.Fury.CheckLifeblood && Me.HealthPercent <= SG.Instance.Fury.CheckLifebloodNum),
                Spell.Cast("Shield Wall", ret => SG.Instance.Fury.CheckShieldWall && Me.HealthPercent <= SG.Instance.Fury.CheckShieldWallNum),
                Spell.Cast("Victory Rush", ret => !G.VROC && !G.IvTalent && G.VictoriousAura && SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum),
                I.FuryUseHealthStone()
                );
        }

        internal static Composite FuryUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Fury.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast("Hamstring", ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Fury.HamString == Enum.Hamstring.Always || SG.Instance.Fury.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast("Intimidating Shout", ret => SG.Instance.Fury.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast("Mass Spell Reflection", ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && (
                    (SG.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.Always && G.PUOC && G.DSOC)
                    )),
                Spell.Cast("Piercing Howl", ret => G.PhTalent && SG.Instance.Fury.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast("Rallying Cry", ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast("Shattering Throw", ret => SG.Instance.Fury.CheckShatteringThrow && U.IsTargetBoss && (G.CSCD <= 3000 || G.SBCD <= 3000)),
                Spell.Cast("Spell Reflection", ret => SG.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Spell.Cast("Staggering Shout", ret => G.SsTalent && SG.Instance.Fury.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum)
                );
        }
        #endregion

    }
}