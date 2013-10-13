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
                                Spell.Cast(SB.BattleShout, on => Me, ret => !G.BattleShoutAura)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, 
                                Spell.Cast(SB.CommandingShout, on => Me, ret => !G.CommandingShoutAura)))));
            }
        }

        internal static Composite InitializeFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => SG.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                    new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                    G.InitializeCaching(),
                    G.InitializeOnKeyActions(),
                    new Decorator(ret => SG.Instance.Fury.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                    new Switch<Enum.WoWVersion>(ctx => SG.Instance.General.CrFuryRotVersion,
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Development, DevFuryCombat),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Release, RelFuryCombat)));
            }
        }

        internal static Composite DevFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryNonGcdUtility(),
                                Dev_FuryRacials(),
                                Dev_FuryOffensive(),
                                I.CreateItemBehaviour(),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        Dev_FurySt()
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_FuryRacials(),
                                        Dev_FuryOffensive(),
                                        I.CreateItemBehaviour())),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        Dev_FurySt()
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_FuryRacials(),
                                        Dev_FuryOffensive(),
                                        I.CreateItemBehaviour())),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => SG.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        Dev_FurySt()
                                        ))))));
            }
        }

        internal static Composite RelFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_FuryDefensive()),
                                Rel_FuryNonGcdUtility(),
                                Rel_FuryRacials(),
                                Rel_FuryOffensive(),
                                I.CreateItemBehaviour(),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_FuryDefensive()),
                                Rel_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Rel_FuryRacials(),
                                        Rel_FuryOffensive(),
                                        I.CreateItemBehaviour())),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => SG.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_FuryDefensive()),
                                Rel_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Rel_FuryRacials(),
                                        Rel_FuryOffensive(),
                                        I.CreateItemBehaviour())),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => SG.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_FurySt())
                                        ))))));
            }
        }
        #endregion

        #region Development Rotations
        internal static Composite Dev_FurySt()
        {
            return new PrioritySelector(
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.up&debuff.colossus_smash.up
                Spell.Cast(SB.StormBolt, ret => G.SbTalent && G.ReadinessAura && G.ColossusSmashAura && Tier6AbilityUsage),
                //actions.single_target+=/raging_blow,if=buff.raging_blow.stack=2&debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast(SB.RagingBlow, ret => G.RagingBlow2S && G.ColossusSmashAura && G.NormalPhase),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down&debuff.colossus_smash.up
                Spell.Cast(SB.StormBolt, ret => G.SbTalent && !G.ReadinessAura && G.ColossusSmashAura && Tier6AbilityUsage),
                //actions.single_target+=/bloodthirst,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)
                Spell.Cast(SB.Bloodthirst),
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.react&target.health.pct>=20&cooldown.bloodthirst.remains<=1
                Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura && G.NormalPhase && G.BTCD <= 1000),
                //actions.single_target+=/wait,sec=cooldown.bloodthirst.remains,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)&cooldown.bloodthirst.remains<=1&cooldown.bloodthirst.remains
                //new Decorator(ret => , new ActionAlwaysSucceed()),
                //actions.single_target+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                Spell.Cast(SB.DragonRoar, ret => G.DrTalent && (!G.ColossusSmashAura && BloodbathSync) && Tier4AbilityUsage),
                //actions.single_target+=/colossus_smash
                Spell.Cast(SB.ColossusSmash),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down
                Spell.Cast(SB.StormBolt, ret => G.SbTalent && !G.ReadinessAura && Tier6AbilityUsage),
                //actions.single_target+=/execute,if=debuff.colossus_smash.up|rage>70|target.time_to_die<12
                Spell.Cast(SB.Execute, ret => G.ColossusSmashAura || Me.CurrentRage > 70),
                //actions.single_target+=/raging_blow,if=target.health.pct<20|buff.raging_blow.stack=2|(debuff.colossus_smash.up|(cooldown.bloodthirst.remains>=1&buff.raging_blow.remains<=3))
                Spell.Cast(SB.RagingBlow, ret => G.ExecutePhase || G.RagingBlow2S || (G.ColossusSmashAura || G.BTCD >= 1 && G.FadingRb(3000))),
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.up
                Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura),
                //actions.single_target+=/bladestorm,if=enabled&cooldown.bloodthirst.remains>2
                Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.BTCD > 2000 && Tier4AbilityUsage),
                //actions.single_target+=/raging_blow,if=cooldown.colossus_smash.remains>=3
                Spell.Cast(SB.RagingBlow, ret => G.CSCD >= 3000),
                //actions.single_target+=/shockwave,if=enabled
                Spell.Cast(SB.Shockwave, ret => G.SwTalent && Tier4AbilityUsage),
                //actions.single_target+=/heroic_throw,if=debuff.colossus_smash.down&rage<60
                Spell.Cast(SB.HeroicThrow, ret => !G.ColossusSmashAura && Me.CurrentRage < 60 && SG.Instance.Fury.CheckHeroicThrow),
                //actions.single_target+=/battle_shout,if=rage<70&!debuff.colossus_smash.up
                new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me, ret => Me.CurrentRage < 70 && !G.ColossusSmashAura)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me, ret => Me.CurrentRage < 70 && !G.ColossusSmashAura))),                
                //actions.single_target+=/wild_strike,if=debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast(SB.WildStrike, ret => G.ColossusSmashAura && G.NormalPhase),
                //actions.single_target+=/battle_shout,if=rage<70
                new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me, ret => Me.CurrentRage < 70)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me, ret => Me.CurrentRage < 70))),     
                //actions.single_target+=/shattering_throw,if=cooldown.colossus_smash.remains>5
                Spell.Cast(SB.ShatteringThrow, ret => G.CSCD > 5000 && SG.Instance.Fury.CheckShatteringThrow),
                //actions.single_target+=/wild_strike,if=cooldown.colossus_smash.remains>=2&rage>=70&target.health.pct>=20
                Spell.Cast(SB.WildStrike, ret => G.CSCD >= 2000 && Me.CurrentRage >= 70 && G.NormalPhase),
                //actions.single_target+=/impending_victory,if=enabled&target.health.pct>=20&cooldown.colossus_smash.remains>=2
                Spell.Cast(SB.ImpendingVictory, ret => G.IvTalent && !G.IVOC && G.NormalPhase && G.CSCD >= 2000 && SG.Instance.Fury.CheckRotImpVic)
                );
        }

        internal static Composite Dev_FuryHeroicStrike()
        {
            return new PrioritySelector(
                Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage), // Added to prevent ragecapping.
                //actions.single_target+=/heroic_strike,if=((debuff.colossus_smash.up&rage>=40)&target.health.pct>=20)|rage>=100&buff.enrage.up
                Spell.Cast(SB.HeroicStrike, ret => G.NormalPhase && Me.CurrentRage >= 40 && G.ColossusSmashAura || Me.CurrentRage >= Me.MaxRage - 20 && G.EnrageAura));
        }

        internal static Composite Dev_FuryMt()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_FuryOffensive()
        {
            return new PrioritySelector(
                //actions+=/recklessness,if=!talent.bloodbath.enabled&((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20))|buff.bloodbath.up&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20)|target.time_to_die<=12
                Spell.Cast(SB.Recklessness, ret => !G.SkullBannerAura && RecklessnessUsage),
                //actions+=/avatar,if=enabled&(buff.recklessness.up|target.time_to_die<=25)
                Spell.Cast(SB.Avatar, ret => G.AvTalent && G.RecklessnessAura && Tier6AbilityUsage),
                //actions+=/skull_banner,if=buff.skull_banner.down&(((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&target.time_to_die>192&buff.cooldown_reduction.up)|buff.recklessness.up)
                Spell.Cast(SB.SkullBanner, ret => !G.SkullBannerAura && SkullBannerUsage && (G.RecklessnessAura || ((G.CSCD < 2000 || G.ColossusSmashAuraT) && G.ReadinessAura))),
                //actions+=/berserker_rage,if=buff.enrage.remains<1&cooldown.bloodthirst.remains>1
                Spell.Cast(SB.BerserkerRage, ret => (!G.EnrageAura || G.FadingEnrage(1000) && G.BTCD > 1000) && BerserkerRageUsage),
                //actions.single_target=bloodbath,if=enabled&(cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5|target.time_to_die<=20)
                Spell.Cast(SB.Bloodbath, ret => G.BbTalent && (G.CSCD < 2000 || G.ColossusSmashAuraT) && Tier6AbilityUsage)
                );
        }

        internal static Composite Dev_FuryGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ImpendingVictory, ret => !G.IVOC && G.IvTalent && SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SB.VictoryRush, ret => !G.VROC && G.VictoriousAura && SG.Instance.Fury.CheckVicRush && Me.HealthPercent <= SG.Instance.Fury.CheckVicRushNum),
                Spell.Cast(SB.IntimidatingShout, ret => SG.Instance.Fury.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.ShatteringThrow, ret => SG.Instance.Fury.CheckShatteringThrow && U.IsTargetBoss && (G.CSCD <= 3000 || G.SBCD <= 3000))
                );
        }

        internal static Composite Dev_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite Dev_FuryDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.DiebytheSword, ret => SG.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= SG.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SB.EnragedRegeneration, ret => G.ErTalent && SG.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast(SB.ShieldWall, ret => SG.Instance.Fury.CheckShieldWall && Me.HealthPercent <= SG.Instance.Fury.CheckShieldWallNum),
                Spell.Cast(SB.SpellReflection, ret => SG.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && G.TargettingMe && Me.CurrentTarget.IsCasting),
                I.FuryUseHealthStone()
                );
        }

        internal static Composite Dev_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SB.DemoralizingBanner, loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Fury.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast(SB.Hamstring, ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Fury.HamString == Enum.Hamstring.Always || SG.Instance.Fury.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast(SB.MassSpellReflection, ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SB.PiercingHowl, ret => G.PhTalent && SG.Instance.Fury.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SB.StaggeringShout, ret => G.SsTalent && SG.Instance.Fury.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region Release Rotations
        internal static Composite Rel_FurySt()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage >= 30), // Also in Rel_FuryHeroicStrike().

                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16), // Added T16 P4.
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added - Inside CS window.

                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.RagingBlow),
                        Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura))),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(

                        Spell.Cast(SB.Execute, ret => G.FadingDeathSentence(3000) && G.CSCD >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added - Outside CS window.
                        Spell.Cast(SB.StormBolt, ret => G.ReadinessAura && G.CSCD >= 16000 && Tier6AbilityUsage), // Added - When new one is ready in next CS window - With Eye of Galakras.

                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.HeroicStrike, ret => G.CSCD >= 3000 && ((G.UrGlyph && Me.CurrentRage >= Me.MaxRage - 15) || (!G.UrGlyph && Me.CurrentRage >= Me.MaxRage - 5))), // Also in Rel_FuryHeroicStrike().
                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow2S && G.CSCD >= 3000),
                        Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura),
                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow1S && G.CSCD >= 3000),
                        new Switch<Enum.Shouts>(ctx => SG.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me, ret => Me.CurrentRage < 70))),
                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.CSCD >= 6000 && Tier4AbilityUsage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage),
                        Spell.Cast(SB.WildStrike, ret => !G.BloodsurgeAura && G.CSCD >= 3000 && Me.CurrentRage >= 90),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage), // Also in Rel_FuryHeroicStrike().
                        Spell.Cast(SB.ImpendingVictory, ret => G.IvTalent && !G.IVOC && SG.Instance.Fury.CheckRotImpVic), // Added for the sake of supporting it rotational.
                        Spell.Cast(SB.HeroicThrow, ret => SG.Instance.Fury.CheckHeroicThrow) // Added for the sake of supporting it rotational.
                        )));
        }

        internal static Composite Rel_FuryExec()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage),
                        Spell.Cast(SB.Execute),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage))),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.FadingDeathSentence(3000) && G.CSCD >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.RagingBlow),
                        Spell.Cast(SB.Execute, ret => Me.CurrentRage == Me.MaxRage - 10),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added
                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.BTCD >= 2000 && G.CSCD >= 6000 && Tier4AbilityUsage) // Added
                        )));
        }

        internal static Composite Rel_FuryHeroicStrike()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage >= 30))),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => G.CSCD >= 3000 && ((G.UrGlyph && Me.CurrentRage >= Me.MaxRage - 15) || (!G.UrGlyph && Me.CurrentRage >= Me.MaxRage - 5))))),
                Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage));
        }

        internal static Composite Rel_FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 5,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16 || G.ExecutePhase && G.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.Whirlwind),
                        Spell.Cast(SB.RagingBlow, ret => Me.CurrentRage <= 60 && G.MeatCleaverAuraS3)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 4,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16 || G.ExecutePhase && G.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.Whirlwind, ret => !G.MeatCleaverAuraS3),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.RagingBlow, ret => G.MeatCleaverAuraS3),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage > Me.MaxRage - 10)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16 || G.ExecutePhase && G.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.Whirlwind, ret => !G.MeatCleaverAuraS2),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.RagingBlow, ret => G.MeatCleaverAuraS2),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage > Me.MaxRage - 10)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16 || G.ExecutePhase && G.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.Whirlwind, ret => !G.MeatCleaverAuraS1),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.RagingBlow, ret => G.MeatCleaverAuraS1),
                        Spell.Cast(SB.Cleave, ret => Me.CurrentRage > Me.MaxRage - 10),
                        new PrioritySelector(
                            new Decorator(ret => G.ExecutePhase, Rel_FuryExec()),
                            new Decorator(ret => G.NormalPhase, Rel_FurySt()))
                        )));
        }

        internal static Composite Rel_FuryOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.BerserkerRage, ret => (!G.EnrageAura || G.FadingEnrage(500)) && G.BTOC && G.ColossusSmashAura && BerserkerRageUsage),
                Spell.Cast(SB.Bloodbath, ret => G.BbTalent && Tier6AbilityUsage),
                Spell.Cast(SB.Recklessness, ret => RecklessnessUsage),
                Spell.Cast(SB.Avatar, ret => G.AvTalent && RecklessnessSync && Tier6AbilityUsage),
                Spell.Cast(SB.SkullBanner, ret => !G.SkullBannerAura && RecklessnessSync && SkullBannerUsage)
                );
        }

        internal static Composite Rel_FuryGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ImpendingVictory, ret => !G.IVOC && G.IvTalent && SG.Instance.Fury.CheckImpVic && Me.HealthPercent <= SG.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SB.VictoryRush, ret => !G.VROC && G.VictoriousAura && SG.Instance.Fury.CheckVicRush && Me.HealthPercent <= SG.Instance.Fury.CheckVicRushNum),
                Spell.Cast(SB.IntimidatingShout, ret => SG.Instance.Fury.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.ShatteringThrow, ret => SG.Instance.Fury.CheckShatteringThrow && U.IsTargetBoss && (G.CSCD <= 3000 || G.SBCD <= 3000))
                );
        }

        internal static Composite Rel_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite Rel_FuryDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.DiebytheSword, ret => SG.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= SG.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SB.EnragedRegeneration, ret => G.ErTalent && SG.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= SG.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast(SB.ShieldWall, ret => SG.Instance.Fury.CheckShieldWall && Me.HealthPercent <= SG.Instance.Fury.CheckShieldWallNum),
                Spell.Cast(SB.SpellReflection, ret => SG.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && G.TargettingMe && Me.CurrentTarget.IsCasting),
                I.FuryUseHealthStone()
                );
        }

        internal static Composite Rel_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SB.DemoralizingBanner, loc => Me.Location, ret => SH.Instance.DemoBannerChoice == Keys.None && SG.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= SG.Instance.Fury.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast(SB.Hamstring, ret => !U.IsTargetBoss && !G.HamstringAura && (SG.Instance.Fury.HamString == Enum.Hamstring.Always || SG.Instance.Fury.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast(SB.MassSpellReflection, ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SB.PiercingHowl, ret => G.PhTalent && SG.Instance.Fury.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SB.StaggeringShout, ret => G.SsTalent && SG.Instance.Fury.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= SG.Instance.Fury.CheckPiercingHowlNum)
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
                return (G.BloodbathAura || !G.BbTalent || G.ReadinessAura);
            }
        }
        #endregion
    }
}