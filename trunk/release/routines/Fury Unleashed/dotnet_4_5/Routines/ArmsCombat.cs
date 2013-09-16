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
                    new Switch<Enum.WoWVersion>(ctx => SG.Instance.General.WowRotVersion,
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.SiegeOfOrgrimmar,
                            new Decorator(ret => !Spell.IsGlobalCooldown(), SoOArmsCombat)),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.ThroneOfThunder,
                            new Decorator(ret => !Spell.IsGlobalCooldown(), ToTArmsCombat))));
            }
        }

        internal static Composite SoOArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, SoO_ArmsDefensive()),
                                SoO_ArmsUtility(),
                                SoO_ArmsVictorious(),
                                I.CreateItemBehaviour(),
                                SoO_ArmsRacials(),
                                SoO_ArmsOffensive(),
                                new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= 2, SoO_ArmsMt()),
                                SoO_ArmsSt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, SoO_ArmsDefensive()),
                                SoO_ArmsUtility(),
                                SoO_ArmsVictorious(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.CreateItemBehaviour(),
                                        SoO_ArmsRacials(),
                                        SoO_ArmsOffensive())),
                                new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= 2, SoO_ArmsMt()),
                                SoO_ArmsSt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, SoO_ArmsDefensive()),
                                SoO_ArmsUtility(),
                                SoO_ArmsVictorious(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.CreateItemBehaviour(),
                                        SoO_ArmsRacials(),
                                        SoO_ArmsOffensive())),
                                new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= 2, SoO_ArmsMt()),
                                SoO_ArmsSt()))));
            }
        }

        internal static Composite ToTArmsCombat
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
                                new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= 2, ArmsMt()),
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
                                new Decorator(ret => SG.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= 2, ArmsMt()),
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
                                new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Arms.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), ArmsMt()),
                                ArmsSt()))));
            }
        }
        #endregion

        #region 5.4 Rotations

        // SimulationCraft 540-1 (r17624) - 2H Arms

        internal static Composite SoO_ArmsSt()
        {
            return new PrioritySelector(
                //actions.single_target=heroic_strike,if=rage>115|(debuff.colossus_smash.up&rage>40&set_bonus.tier16_2pc_melee)
                Spell.Cast("Heroic Strike", ret => Me.CurrentRage > 115 || (G.ColossusSmashAura && Me.CurrentRage > 40 && G.Tier16TwoPieceBonus)),
                //actions.single_target+=/colossus_smash,if=debuff.colossus_smash.remains<1.0
                Spell.Cast("Colossus Smash", ret => !G.ColossusSmashAura || G.FadingCs(1000)),
                //actions.single_target+=/mortal_strike
                Spell.Cast("Mortal Strike"),
                //actions.single_target+=/storm_bolt,if=enabled&debuff.colossus_smash.up
                Spell.Cast("Storm Bolt", ret => G.SbTalent && G.ColossusSmashAura && Tier6AbilityUsage),
                //actions.single_target+=/dragon_roar,if=enabled&!debuff.colossus_smash.up
                Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && Tier4AbilityUsage),
                //actions.single_target+=/execute,if=buff.sudden_execute.down|buff.taste_for_blood.down|rage>70
                Spell.Cast("Execute", ret => !G.SuddenExecAura || !G.TasteforBloodAura || Me.CurrentRage > 70),
                //actions.single_target+=/slam,if=(debuff.colossus_smash.up&buff.taste_for_blood.stack<3)&target.health.pct>=20
                Spell.Cast("Slam", ret => G.NonExecuteCheck && !G.TasteForBloodS3 && G.ColossusSmashAura),
                //actions.single_target+=/overpower,if=target.health.pct>=20|buff.sudden_execute.up
                Spell.Cast("Overpower", ret => G.NonExecuteCheck || G.SuddenExecAura),
                //actions.single_target+=/battle_shout
                new Switch<Enum.Shouts>(ctx => SG.Instance.Arms.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me))),
                //actions.single_target+=/heroic_throw 
                Spell.Cast("Heroic Throw", ret => SG.Instance.Arms.CheckHeroicThrow)
                );
        }

        internal static Composite SoO_ArmsMt()
        {
            return new PrioritySelector(
                //actions.aoe=sweeping_strikes
                Spell.Cast("Sweeping Strikes"),
                //actions.aoe+=/cleave,if=rage>110&active_enemies<=4
                Spell.Cast("Cleave", ret => Me.CurrentRage > 110 && U.NearbyAttackableUnitsCount <= 4),
                //actions.aoe+=/bladestorm,if=enabled&(buff.bloodbath.up|!talent.bloodbath.enabled)
                Spell.Cast("Bladestorm", ret => G.BsTalent && (G.BloodbathAura || !G.BbTalent) && Tier4AbilityAoEUsage),
                //actions.aoe+=/dragon_roar,if=enabled&(buff.bloodbath.up|!talent.bloodbath.enabled)
                Spell.Cast("Dragon Roar", ret => G.DrTalent && (G.BloodbathAura || !G.BbTalent) && Tier4AbilityAoEUsage),
                //Added
                Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                //actions.aoe+=/thunder_clap,target=2,if=dot.deep_wounds.attack_power*1.1<stat.attack_power
                Spell.Cast("Thunderclap", ret => U.NeedThunderclapUnitsCount > 1),
                //actions.aoe+=/colossus_smash,if=debuff.colossus_smash.remains<1
                Spell.Cast("Colossus Smash", ret => !G.ColossusSmashAura || G.FadingCs(1000)),
                //actions.aoe+=/slam,if=buff.sweeping_strikes.up&debuff.colossus_smash.up
                Spell.Cast("Slam", ret => G.SweepingStrikesAura && G.ColossusSmashAura),
                //actions.aoe+=/mortal_strike
                Spell.Cast("Mortal Strike"),
                //Added
                Spell.Cast("Storm Bolt", ret => G.SwTalent && Tier6AbilityUsage),
                //actions.aoe+=/slam,if=buff.sweeping_strikes.up
                Spell.Cast("Slam", ret => G.SweepingStrikesAura),
                //actions.aoe+=/battle_shout,if=rage<70 
                new Switch<Enum.Shouts>(ctx => SG.Instance.Arms.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me)))
                );
        }

        internal static Composite SoO_ArmsOffensive()
        {
            return new PrioritySelector(
                //actions+=/recklessness,if=!talent.bloodbath.enabled&((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20))|buff.bloodbath.up&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20)|target.time_to_die<=12
                Spell.Cast("Recklessness", ret => ((G.AvatarAura || G.AVCD <= 1500) || (G.BloodbathAura || G.BBCD <= 1500) || (G.SbTalent)) && RecklessnessUsage),
                //actions+=/bloodbath,if=enabled&(debuff.colossus_smash.up|cooldown.colossus_smash.remains<4)
                Spell.Cast("Bloodbath", ret => G.BbTalent && (G.ColossusSmashAura || G.CSCD < 4000) && Tier6AbilityUsage),
                //actions+=/avatar,if=enabled&(buff.recklessness.up|target.time_to_die<=25)
                Spell.Cast("Avatar", ret => G.AvTalent && G.RecklessnessAura && Tier6AbilityUsage),
                //actions+=/skull_banner,if=buff.skull_banner.down&(((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&target.time_to_die>192&buff.cooldown_reduction.up)|buff.recklessness.up)
                Spell.Cast("Skull Banner", ret => !G.SkullBannerAura && G.RecklessnessAura && SkullBannerUsage),
                //actions+=/berserker_rage,if=buff.enrage.remains<0.5
                Spell.Cast("Berserker Rage", ret => (!G.EnrageAura || G.FadingEnrage(500)) && BerserkerRageUsage)
                );
        }

        internal static Composite SoO_ArmsVictorious()
        {
            return new PrioritySelector(
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && SG.Instance.Arms.CheckImpVic && Me.HealthPercent <= SG.Instance.Arms.CheckImpVicNum),
                Spell.Cast("Victory Rush", ret => !G.VROC && !G.IvTalent && G.VictoriousAura && SG.Instance.Arms.CheckImpVic && Me.HealthPercent <= SG.Instance.Arms.CheckImpVicNum)
                );
        }

        internal static Composite SoO_ArmsRacials()
        {
            return new PrioritySelector(
                new Decorator(ret =>
                    (SG.Instance.Arms.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Arms.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SG.Instance.Arms.ClassRacials == Enum.AbilityTrigger.Always),
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite SoO_ArmsDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Die by the Sword", ret => SG.Instance.Arms.CheckDiebytheSword && Me.HealthPercent <= SG.Instance.Arms.CheckDiebytheSwordNum),
                Spell.Cast("Enraged Regeneration", ret => G.ErTalent && SG.Instance.Arms.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Arms.CheckEnragedRegenNum),
                Spell.Cast("Lifeblood", ret => SG.Instance.Arms.CheckLifeblood && Me.HealthPercent <= SG.Instance.Arms.CheckLifebloodNum),
                Spell.Cast("Shield Wall", ret => SG.Instance.Arms.CheckShieldWall && Me.HealthPercent <= SG.Instance.Arms.CheckShieldWallNum),
                I.ArmsUseHealthStone()
                );
        }

        internal static Composite SoO_ArmsUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Arms.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Arms.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast("Hamstring", ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Arms.HamString == Enum.Hamstring.Always || SG.Instance.Arms.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast("Intimidating Shout", ret => SG.Instance.Arms.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast("Mass Spell Reflection", ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast("Piercing Howl", ret => G.PhTalent && SG.Instance.Arms.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckPiercingHowlNum),
                Spell.Cast("Rallying Cry", ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast("Shattering Throw", ret => SG.Instance.Arms.CheckShatteringThrow && U.IsTargetBoss && (G.CSCD <= 3000 || G.SBCD <= 3000)),
                Spell.Cast("Spell Reflection", ret => SG.Instance.Arms.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Spell.Cast("Staggering Shout", ret => G.SsTalent && SG.Instance.Arms.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Arms.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region 5.3 Rotations
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
                Spell.Cast("Heroic Throw")
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

        internal static Composite ArmsVictorious()
        {
            return new PrioritySelector(
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && SG.Instance.Arms.CheckImpVic && Me.HealthPercent <= SG.Instance.Arms.CheckImpVicNum),
                Spell.Cast("Victory Rush", ret => !G.VROC && !G.IvTalent && G.VictoriousAura && SG.Instance.Arms.CheckImpVic && Me.HealthPercent <= SG.Instance.Arms.CheckImpVicNum)
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
        #endregion

        }
}
