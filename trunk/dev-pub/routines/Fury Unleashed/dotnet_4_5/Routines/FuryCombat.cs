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
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, 
                                Spell.Cast("Battle Shout", on => Me, ret => !G.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, 
                                Spell.Cast("Commanding Shout", on => Me, ret => !G.CommandingShoutAura)))));
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
                    new Switch<Enum.WoWVersion>(ctx => SG.Instance.General.WowRotVersion,
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.SiegeOfOrgrimmar,
                            new Decorator(ret => !Spell.IsGlobalCooldown(), SoOFuryCombat)),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.ThroneOfThunder,
                            new Decorator(ret => !Spell.IsGlobalCooldown(), ToTFuryCombat))));
            }
        }

        internal static Composite SoOFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, SoO_FuryDefensive()),
                                SoO_FuryUtility(),
                                SoO_FuryVictorious(),
                                I.CreateItemBehaviour(),
                                SoO_FuryRacials(),
                                SoO_FuryOffensive(),
                                new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= 2, SoO_FuryMt()),
                                SoO_FurySt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, SoO_FuryDefensive()),
                                SoO_FuryUtility(),
                                SoO_FuryVictorious(),
                                I.CreateItemBehaviour(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        SoO_FuryRacials(),
                                        SoO_FuryOffensive())),
                                new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= 2, SoO_FuryMt()),
                                SoO_FurySt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, SoO_FuryDefensive()),
                                SoO_FuryUtility(),
                                SoO_FuryVictorious(),
                                I.CreateItemBehaviour(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        SoO_FuryRacials(),
                                        SoO_FuryOffensive())),
                                new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= 2, SoO_FuryMt()),
                                SoO_FurySt()))));
            }
        }

        internal static Composite ToTFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                FuryUtility(),
                                FuryVictorious(),
                                I.CreateItemBehaviour(),
                                FuryRacials(),
                                FuryOffensive(),
                                new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= 2, FuryMt()),
                                FurySt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                FuryUtility(),
                                FuryVictorious(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.CreateItemBehaviour(),
                                        FuryRacials(),
                                        FuryOffensive())),
                                new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= 2, FuryMt()),
                                FurySt())),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, FuryDefensive()),
                                FuryUtility(),
                                FuryVictorious(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.CreateItemBehaviour(),
                                        FuryRacials(),
                                        FuryOffensive())),
                                new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Fury.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), FuryMt()),
                                FurySt()))));
            }
        }
        #endregion

        #region 5.4 Rotations
        // SimulationCraft 540-1 (r17675) - 1H/2H Fury - Slightly Modified.
        internal static Composite SoO_FurySt()
        {
            return new PrioritySelector(
                new Decorator(ret => G.AlmostDead,
                    new PrioritySelector(
                        Spell.Cast("Colossus Smash", ret => G.ColossusSmashAura),
                        Spell.Cast("Bloodthirst", ret => !G.EnrageAura),
                        Spell.Cast("Execute"),
                        Spell.Cast("Heroic Strike", ret => G.DumpAllRage && Me.CurrentRage > 60)
                        )),
                //actions.single_target+=/heroic_strike,if=((debuff.colossus_smash.up&rage>=40)&target.health.pct>=20)|rage>=100&buff.enrage.up
                Spell.Cast("Heroic Strike", ret => (Me.CurrentRage >= 100 && G.EnrageAura) || (G.NonExecuteCheck && Me.CurrentRage >= 40 && G.ColossusSmashAura)),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.up&debuff.colossus_smash.up
                Spell.Cast("Storm Bolt", ret => G.SbTalent && G.ReadinessAura && G.ColossusSmashAura && Tier6AbilityUsage),
                //actions.single_target+=/raging_blow,if=buff.raging_blow.stack=2&debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast("Raging Blow", ret => G.RagingBlow2S && G.ColossusSmashAura && G.NonExecuteCheck),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down&debuff.colossus_smash.up
                Spell.Cast("Storm Bolt", ret => G.SbTalent && !G.ReadinessAura && G.ColossusSmashAura && Tier6AbilityUsage),
                //actions.single_target+=/bloodthirst,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up) //uuh?
                Spell.Cast("Bloodthirst"),
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.react&target.health.pct>=20&cooldown.bloodthirst.remains<=1
                Spell.Cast("Wild Strike", ret => G.BloodsurgeAura && G.NonExecuteCheck && G.BTCD <= 1000),
                //actions.single_target+=/wait,sec=cooldown.bloodthirst.remains,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)&cooldown.bloodthirst.remains<=1&cooldown.bloodthirst.remains
                //actions.single_target+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && (G.BloodbathAura || !G.BbTalent) && Tier4AbilityUsage),
                //actions.single_target+=/colossus_smash
                Spell.Cast("Colossus Smash"),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down
                Spell.Cast("Storm Bolt", ret => G.SbTalent && !G.ReadinessAura && Tier6AbilityUsage),
                //actions.single_target+=/execute,if=buff.enrage.up|debuff.colossus_smash.up|rage>70|target.time_to_die<12
                Spell.Cast("Execute", ret => G.ExecuteCheck && (G.EnrageAura || G.ColossusSmashAura || Me.CurrentRage > 70 || G.AlmostDead)),
                //actions.single_target+=/raging_blow,if=buff.raging_blow.stack=2|(debuff.colossus_smash.up|(cooldown.bloodthirst.remains>=1&buff.raging_blow.remains<=3))
                Spell.Cast("Raging Blow", ret => G.RagingBlow2S || G.ColossusSmashAura || (G.BTCD >= 1000 && G.FadingRb(3000))),
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.up
                Spell.Cast("Wild Strike", ret => G.BloodsurgeAura),
                //actions.single_target+=/bladestorm,if=enabled&cooldown.bloodthirst.remains>2
                Spell.Cast("Bladestorm", ret => G.BsTalent && G.BTCD > 2 && Tier4AbilityUsage),
                //actions.single_target+=/raging_blow,if=cooldown.colossus_smash.remains>=3
                Spell.Cast("Raging Blow", ret => G.CSCD >= 3000),
                //actions.single_target+=/shockwave,if=enabled
                Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage),
                //actions.single_target+=/heroic_throw,if=debuff.colossus_smash.down&rage<60
                Spell.Cast("Heroic Throw", ret => !G.ColossusSmashAura && Me.CurrentRage < 60 && SG.Instance.Fury.CheckHeroicThrow),
                //actions.single_target+=/battle_shout,if=rage<70&!debuff.colossus_smash.up
                new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70 && !G.ColossusSmashAura)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70 && !G.ColossusSmashAura))),
                //actions.single_target+=/wild_strike,if=debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast("Wild Strike", ret => G.ColossusSmashAura && G.NonExecuteCheck),
                //actions.single_target+=/battle_shout,if=rage<70
                new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70))),
                //actions.single_target+=/shattering_throw,if=cooldown.colossus_smash.remains>5
                Spell.Cast("Shattering Throw", ret => G.CSCD > 5000 && SG.Instance.Fury.CheckShatteringThrow),
                //actions.single_target+=/wild_strike,if=cooldown.colossus_smash.remains>=2&rage>=70&target.health.pct>=20
                Spell.Cast("Wild Strike", ret => G.CSCD >= 2000 && Me.CurrentRage >= 70 && G.NonExecuteCheck),
                //actions.single_target+=/impending_victory,if=enabled&target.health.pct>=20&cooldown.colossus_smash.remains>=2
                Spell.Cast("Impending Victory", ret => G.IvTalent && G.NonExecuteCheck && G.CSCD >= 2000 && SG.Instance.Fury.CheckRotImpVic)
                );
        }

        internal static Composite SoO_FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        //actions.two_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                        Spell.Cast("Cleave", ret => Me.CurrentRage > 90 || (Me.CurrentRage >= 60 && G.ColossusSmashAura)),
                        //actions.two_targets+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                        Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && (G.BloodbathAura || !G.BbTalent) && Tier4AbilityAoEUsage),
                        //actions.two_targets+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        Spell.Cast("Bladestorm", ret => G.BsTalent && G.EnrageAura && (G.BloodbathAura || !G.BbTalent) && Tier4AbilityAoEUsage),
                        //actions.two_targets+=/shockwave,if=enabled
                        Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        //actions.two_targets+=/colossus_smash
                        Spell.Cast("Colossus Smash"),
                        //actions.two_targets+=/bloodthirst,cycle_targets=1,if=dot.deep_wounds.remains<5
                        //actions.two_targets+=/bloodthirst,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)
                        Spell.Cast("Bloodthirst", ret => !G.DeepWoundsAura || G.FadingDw(5000) || (G.NonExecuteCheck && G.ColossusSmashAura && Me.CurrentRage >= 30 && G.EnrageAura)),
                        //actions.two_targets+=/storm_bolt,if=enabled
                        Spell.Cast("Storm Bolt", ret => G.SbTalent && Tier6AbilityUsage),
                        //actions.two_targets+=/execute,if=debuff.colossus_smash.up
                        Spell.Cast("Execute", ret => G.ExecuteCheck && G.ColossusSmashAura),
                        //actions.two_targets+=/raging_blow,if=buff.meat_cleaver.up|target.health.pct<20
                        Spell.Cast("Raging Blow", ret => G.MeatCleaverAuraS1 || G.ExecuteCheck),
                        //actions.two_targets+=/whirlwind,if=!buff.meat_cleaver.up
                        Spell.Cast("Whirlwind", ret => !G.MeatCleaverAuraS1),
                        //actions.two_targets+=/battle_shout
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me))),
                        //actions.two_targets+=/heroic_throw
                        Spell.Cast("Heroic Throw", ret => SG.Instance.Fury.CheckHeroicThrow)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(
                        //actions.three_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                        Spell.Cast("Cleave", ret => Me.CurrentRage > 90 || (Me.CurrentRage >= 60 && G.ColossusSmashAura)),
                        //actions.three_targets+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                        Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && (G.BloodbathAura || !G.BbTalent) && Tier4AbilityAoEUsage),
                        //actions.three_targets+=/shockwave,if=enabled
                        Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        //actions.three_targets+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        Spell.Cast("Bladestorm", ret => G.BsTalent && G.EnrageAura && (G.BloodbathAura || !G.BbTalent) && Tier4AbilityAoEUsage),
                        //actions.three_targets+=/colossus_smash
                        Spell.Cast("Colossus Smash"),
                        //actions.three_targets+=/storm_bolt,if=enabled
                        Spell.Cast("Storm Bolt", ret => G.SbTalent && Tier6AbilityUsage),
                        //actions.three_targets+=/raging_blow,if=buff.meat_cleaver.stack=2
                        Spell.Cast("Raging Blow", ret => G.MeatCleaverAuraS2),
                        //actions.three_targets+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking
                        Spell.Cast("Bloodthirst", ret => !G.DeepWoundsAura),
                        //Own Addition - Spread Deep Wounds since Buff.
                        //Spell.Cast("Thunder Clap", ret => U.NeedThunderclapUnitsCount > 0),
                        //actions.three_targets+=/whirlwind
                        Spell.Cast("Whirlwind"),
                        //actions.three_targets+=/raging_blow
                        Spell.Cast("Raging Blow"),
                        //actions.three_targets+=/battle_shout,if=rage<70
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70))),
                        //actions.three_targets+=/heroic_throw
                        Spell.Cast("Heroic Throw", ret => SG.Instance.Fury.CheckHeroicThrow)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 4,
                    new PrioritySelector(
                        //actions.aoe+=/cleave,if=rage>110
                        Spell.Cast("Cleave", ret => Me.CurrentRage > 110),
                        //actions.aoe+=/dragon_roar,if=enabled&!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        Spell.Cast("Dragon Roar", ret => G.DrTalent && !G.ColossusSmashAura && (G.BloodbathAura || !G.BbTalent) && Tier4AbilityAoEUsage),
                        //actions.aoe+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        Spell.Cast("Bladestorm", ret => G.BsTalent && G.EnrageAura && (G.BloodbathAura || !G.BbTalent) && Tier4AbilityAoEUsage),
                        //actions.aoe+=/shockwave,if=enabled
                        Spell.Cast("Shockwave", ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        //actions.aoe+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking&buff.enrage.down
                        Spell.Cast("Bloodthirst", ret => !G.DeepWoundsAura && !G.EnrageAura),
                        //actions.aoe+=/raging_blow,if=buff.meat_cleaver.stack=3
                        Spell.Cast("Raging Blow", ret => G.MeatCleaverAuraS3),
                        //Own Addition - Spread Deep Wounds since Buff.
                        //Spell.Cast("Thunder Clap", ret => U.NeedThunderclapUnitsCount > 0),
                        //actions.aoe+=/whirlwind
                        Spell.Cast("Whirlwind"),
                        //actions.aoe+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking
                        Spell.Cast("Bloodthirst", ret => !G.DeepWoundsAura),
                        //actions.aoe+=/colossus_smash
                        Spell.Cast("Colossus Smash"),
                        //actions.aoe+=/battle_shout,if=rage<70
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast("Battle Shout", on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast("Commanding Shout", on => Me, ret => Me.CurrentRage < 70)))
                        )));
        }

        internal static Composite SoO_FuryOffensive()
        {
            return new PrioritySelector(
                //actions.single_target=bloodbath,if=enabled&(cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5|target.time_to_die<=20)
                Spell.Cast("Bloodbath", ret => G.BbTalent && (G.CSCD < 2000 || G.ColossusSmashAuraT) && Tier6AbilityUsage),
                //actions+=/recklessness,if=!talent.bloodbath.enabled&((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20))|buff.bloodbath.up&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20)|target.time_to_die<=12
                Spell.Cast("Recklessness", ret => ((G.AvatarAura || G.AVCD <= 1500) || (G.BloodbathAura || G.BBCD <= 1500) || (G.SbTalent)) && RecklessnessUsage),
                //actions+=/avatar,if=enabled&(buff.recklessness.up|target.time_to_die<=25)
                Spell.Cast("Avatar", ret => G.AvTalent && G.RecklessnessAura && Tier6AbilityUsage),
                //actions+=/skull_banner,if=buff.skull_banner.down&(((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&target.time_to_die>192&buff.cooldown_reduction.up)|buff.recklessness.up)
                Spell.Cast("Skull Banner", ret => !G.SkullBannerAura && G.RecklessnessAura && SkullBannerUsage),
                //actions+=/berserker_rage,if=buff.enrage.remains<1&cooldown.bloodthirst.remains>1
                Spell.Cast("Berserker Rage", ret => (!G.EnrageAura || (G.FadingEnrage(1000) && G.BTCD > 1000)) && BerserkerRageUsage));
        }

        internal static Composite SoO_FuryVictorious()
        {
            return new PrioritySelector(
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum),
                Spell.Cast("Victory Rush", ret => !G.VROC && !G.IvTalent && G.VictoriousAura && SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum)
                );
        }

        internal static Composite SoO_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite SoO_FuryDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Die by the Sword", ret => SG.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= SG.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast("Enraged Regeneration", ret => G.ErTalent && SG.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast("Lifeblood", ret => SG.Instance.Fury.CheckLifeblood && Me.HealthPercent <= SG.Instance.Fury.CheckLifebloodNum),
                Spell.Cast("Shield Wall", ret => SG.Instance.Fury.CheckShieldWall && Me.HealthPercent <= SG.Instance.Fury.CheckShieldWallNum),
                I.FuryUseHealthStone()
                );
        }

        internal static Composite SoO_FuryUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround("Demoralizing Banner", loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Fury.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast("Hamstring", ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Fury.HamString == Enum.Hamstring.Always || SG.Instance.Fury.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast("Intimidating Shout", ret => SG.Instance.Fury.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast("Mass Spell Reflection", ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast("Piercing Howl", ret => G.PhTalent && SG.Instance.Fury.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast("Rallying Cry", ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast("Shattering Throw", ret => SG.Instance.Fury.CheckShatteringThrow && U.IsTargetBoss && (G.CSCD <= 3000 || G.SBCD <= 3000)),
                Spell.Cast("Spell Reflection", ret => SG.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && Me.CurrentTarget.CurrentTargetGuid == Me.Guid && Me.CurrentTarget.IsCasting),
                Spell.Cast("Staggering Shout", ret => G.SsTalent && SG.Instance.Fury.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region 5.3 Rotations
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
                Spell.Cast("Raging Blow", ret => G.RagingBlow2S && G.ColossusSmashAura && G.NonExecuteCheck),
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
                Spell.Cast("Raging Blow", ret => G.RagingBlow2S || (G.RagingBlowAura && (G.ColossusSmashAura || (G.BTCD >= 1000 && G.FadingRb(3000))))),
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
                        Spell.Cast("Raging Blow", ret => G.RagingBlow2S || (G.RagingBlowAura && (G.ColossusSmashAura || G.CSCD >= 3000 || G.FadingRb(3000)))),
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
                        Spell.Cast("Raging Blow", ret => G.RagingBlow2S || (G.RagingBlowAura && (G.ColossusSmashAura || G.CSCD >= 3000 || G.FadingRb(3000)))),
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

        internal static Composite FuryVictorious()
        {
            return new PrioritySelector(
                Spell.Cast("Impending Victory", ret => !G.IVOC && G.IvTalent && SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum),
                Spell.Cast("Victory Rush", ret => !G.VROC && !G.IvTalent && G.VictoriousAura && SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum)
                );
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
                Spell.Cast("Berserker Rage", ret => (!G.EnrageAura || (G.RagingBlow2S && G.NonExecuteCheck) || (G.RecklessnessAuraT && !G.RagingBlowAura)) && (
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
                Spell.Cast("Lifeblood", ret => SG.Instance.Fury.CheckLifeblood && Me.HealthPercent <= SG.Instance.Fury.CheckLifebloodNum),
                Spell.Cast("Shield Wall", ret => SG.Instance.Fury.CheckShieldWall && Me.HealthPercent <= SG.Instance.Fury.CheckShieldWallNum),
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

        #region Booleans

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((SG.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool MassSpellReflectionUsage
        {
            get
            {
                return ((SG.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.Always && G.PUOC && G.DSOC));
            }
        }

        internal static bool RacialUsage
        {
            get
            {
                return ((SG.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((SG.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((SG.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (SG.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }
        #endregion
    }
}