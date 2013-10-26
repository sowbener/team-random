using CommonBehaviors.Actions;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Core.Utilities;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = FuryUnleashed.Rotations.Global;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SB = FuryUnleashed.Core.Helpers.SpellBook;
using U = FuryUnleashed.Core.Unit;

namespace FuryUnleashed.Rotations
{
    class FuryRotation
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
                        //new Decorator(ret => IS.Instance.General.CheckDebugLogging, Logger.AdvancedLogging),
                        new Decorator(ret => IS.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions())),
                    new Decorator(ret => U.DefaultBuffCheck && ((IS.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
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
                    new Decorator(ret => IS.Instance.General.CheckTreePerformance, TreeSharp.Tree(true)),
                    new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                    //new Action(delegate { ObjectManager.Update(); return RunStatus.Failure; }),
                    G.InitializeCaching(),
                    G.InitializeOnKeyActions(),
                    new Decorator(ret => IS.Instance.Fury.CheckInterrupts && U.CanInterrupt, G.InitializeInterrupts()),
                    new Switch<Enum.WoWVersion>(ctx => IS.Instance.General.CrFuryRotVersion,
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Development, DevFuryCombat),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.SimCraft, SimcFuryCombat),
                        new SwitchArgument<Enum.WoWVersion>(Enum.WoWVersion.Release, RelFuryCombat)));
            }
        }

        internal static Composite DevFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryNonGcdUtility(),
                                Dev_FuryRacials(),
                                Dev_FuryOffensive(),
                                Item.CreateItemBehaviour(),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_FuryRacials(),
                                        Dev_FuryOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_FuryRacials(),
                                        Dev_FuryOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_FurySt())
                                        ))))));
            }
        }

        internal static Composite SimcFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Sim_FuryDefensive()),
                                Sim_FuryNonGcdUtility(),
                                Sim_FuryRacials(),
                                Sim_FuryOffensive(),
                                Item.CreateItemBehaviour(),
                                Sim_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Sim_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Sim_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Sim_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Sim_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Sim_FuryDefensive()),
                                Sim_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Sim_FuryRacials(),
                                        Sim_FuryOffensive(),
                                        Item.CreateItemBehaviour())),
                                Sim_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Sim_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Sim_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Sim_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Sim_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Sim_FuryDefensive()),
                                Sim_FuryNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Sim_FuryRacials(),
                                        Sim_FuryOffensive(),
                                        Item.CreateItemBehaviour())),
                                Sim_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Sim_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Sim_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Sim_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Sim_FurySt())
                                        ))))));
            }
        }

        internal static Composite RelFuryCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_FuryDefensive()),
                                Rel_FuryNonGcdUtility(),
                                Rel_FuryRacials(),
                                Rel_FuryOffensive(),
                                Item.CreateItemBehaviour(),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Rel_FuryMt()),
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
                                        Item.CreateItemBehaviour())),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Rel_FuryMt()),
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
                                        Item.CreateItemBehaviour())),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_FurySt())
                                        ))))));
            }
        }
        #endregion

        #region Development Rotations
        // TODO: Make this (Advanced Rota): http://www.icy-veins.com/fury-warrior-wow-pve-dps-rotation-cooldowns-abilities#advanced-rotation
        internal static Composite Dev_FurySt()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryExec()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryHeroicStrike()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryMt()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryOffensive()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryGcdUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryRacials()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryDefensive()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                );
        }
        #endregion

        #region SimCraft Rotations
        internal static Composite Sim_FuryHeroicStrike()
        {
            return new PrioritySelector(
                //Added to prevent ragecapping
                Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage),
                //actions.single_target+=/heroic_strike,if=((debuff.colossus_smash.up&rage>=40)&target.health.pct>=20)|rage>=100&buff.enrage.up
                Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage >= 100 && G.EnrageAura || G.NormalPhase && Me.CurrentRage >= 40 && G.ColossusSmashAura)
                );
        }

        internal static Composite Sim_FurySt()
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
                Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura && G.NormalPhase && G.BtCd <= 1000),
                //actions.single_target+=/wait,sec=cooldown.bloodthirst.remains,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)&cooldown.bloodthirst.remains<=1&cooldown.bloodthirst.remains
                new Decorator(ret => (G.NormalPhase || !G.ColossusSmashAura || (Me.CurrentRage < 30 && !G.EnrageAura)) && G.BtCd <= 1000, new ActionAlwaysSucceed()),
                //or alxaw's version:
                //actions.single_target+=/wait,sec=cooldown.bloodthirst.remains,if=(debuff.colossus_smash.up&buff.enrage.up&rage>=30)|(!debuff.colossus_smash.up&buff.enrage.down&rage>=30&cooldown.berserker_rage.remains>1)&cooldown.bloodthirst.remains<=1&cooldown.bloodthirst.remains
                //actions.single_target+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                Spell.Cast(SB.DragonRoar, ret => G.DrTalent && !G.ColossusSmashAura && BloodbathSync && Tier4AbilityUsage),
                //actions.single_target+=/colossus_smash
                Spell.Cast(SB.ColossusSmash),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down
                Spell.Cast(SB.StormBolt, ret => G.SbTalent && !G.ReadinessAura && Tier6AbilityUsage),
                //actions.single_target+=/execute,if=debuff.colossus_smash.up|rage>70|target.time_to_die<12
                Spell.Cast(SB.Execute, ret => G.ColossusSmashAura || Me.CurrentRage > 70),
                //actions.single_target+=/raging_blow,if=target.health.pct<20|buff.raging_blow.stack=2|(debuff.colossus_smash.up|(cooldown.bloodthirst.remains>=1&buff.raging_blow.remains<=3))
                Spell.Cast(SB.RagingBlow, ret => G.ExecutePhase || G.RagingBlow2S || G.ColossusSmashAura || (G.BtCd >= 1000 && G.FadingRb(3000))),
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.up
                Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura),
                //actions.single_target+=/bladestorm,if=enabled&cooldown.bloodthirst.remains>2
                Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.BtCd > 2000 && Tier4AbilityUsage),
                //actions.single_target+=/raging_blow,if=cooldown.colossus_smash.remains>=3
                Spell.Cast(SB.RagingBlow, ret => G.RemainingCs(3000)),
                //actions.single_target+=/shockwave,if=enabled
                Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage),
                //actions.single_target+=/heroic_throw,if=debuff.colossus_smash.down&rage<60
                Spell.Cast(SB.HeroicThrow, ret => !G.ColossusSmashAura && Me.CurrentRage < 60 && IS.Instance.Fury.CheckHeroicThrow),
                //actions.single_target+=/battle_shout,if=rage<70&!debuff.colossus_smash.up
                new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me, ret => Me.CurrentRage < 70 && !G.ColossusSmashAura)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me, ret => Me.CurrentRage < 70 && !G.ColossusSmashAura))),
                //actions.single_target+=/wild_strike,if=debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast(SB.WildStrike, ret => G.ColossusSmashAura && G.NormalPhase),
                //actions.single_target+=/battle_shout,if=rage<70
                new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me, ret => Me.CurrentRage < 70)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me, ret => Me.CurrentRage < 70))),
                //actions.single_target+=/shattering_throw,if=cooldown.colossus_smash.remains>5
                Spell.Cast(SB.ShatteringThrow, ret => G.CsCd > 5000 && IS.Instance.Fury.CheckShatteringThrow),
                //actions.single_target+=/wild_strike,if=cooldown.colossus_smash.remains>=2&rage>=70&target.health.pct>=20
                Spell.Cast(SB.WildStrike, ret => G.CsCd >= 2000 && Me.CurrentRage >= 70 && G.NormalPhase),
                //actions.single_target+=/impending_victory,if=enabled&target.health.pct>=20&cooldown.colossus_smash.remains>=2
                Spell.Cast(SB.ImpendingVictory, ret => G.IvTalent && G.NormalPhase && G.CsCd >= 2000 && IS.Instance.Fury.CheckRotImpVic)
                );
        }

        // TODO: Make execute from http://www.icy-veins.com/fury-warrior-wow-pve-dps-rotation-cooldowns-abilities#advanced-rotation
        internal static Composite Sim_FuryExec()
        {
            return new PrioritySelector(
                new Decorator(ret => true,
                    new PrioritySelector()),
                new Decorator(ret => true,
                    new PrioritySelector()));
        }

        internal static Composite Sim_FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 4,
                    new PrioritySelector(
                        //actions.aoe=bloodbath,if=enabled&buff.enrage.up
                        //actions.aoe+=/cleave,if=rage>110
                        //actions.aoe+=/heroic_leap,if=buff.enrage.up
                        //actions.aoe+=/dragon_roar,if=enabled&!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        //actions.aoe+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        //actions.aoe+=/shockwave,if=enabled
                        //actions.aoe+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking&buff.enrage.down
                        //actions.aoe+=/raging_blow,if=buff.meat_cleaver.stack=3
                        //actions.aoe+=/whirlwind
                        //actions.aoe+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking
                        //actions.aoe+=/colossus_smash
                        //actions.aoe+=/battle_shout,if=rage<70
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(
                        //actions.three_targets=bloodbath,if=enabled&buff.enrage.up
                        //actions.three_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                        //actions.three_targets+=/heroic_leap,if=buff.enrage.up
                        //actions.three_targets+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                        //actions.three_targets+=/shockwave,if=enabled
                        //actions.three_targets+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        //actions.three_targets+=/colossus_smash
                        //actions.three_targets+=/storm_bolt,if=enabled
                        //actions.three_targets+=/raging_blow,if=buff.meat_cleaver.stack=2
                        //actions.three_targets+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking
                        //actions.three_targets+=/whirlwind
                        //actions.three_targets+=/raging_blow
                        //actions.three_targets+=/battle_shout,if=rage<70
                        //actions.three_targets+=/heroic_throw
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        //actions.two_targets=bloodbath,if=enabled&buff.enrage.up
                        //actions.two_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                        //actions.two_targets+=/heroic_leap,if=buff.enrage.up
                        //actions.two_targets+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                        //actions.two_targets+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        //actions.two_targets+=/shockwave,if=enabled
                        //actions.two_targets+=/colossus_smash
                        //actions.two_targets+=/bloodthirst,cycle_targets=1,if=dot.deep_wounds.remains<5
                        //actions.two_targets+=/bloodthirst,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)
                        //actions.two_targets+=/storm_bolt,if=enabled
                        //actions.two_targets+=/wait,sec=cooldown.bloodthirst.remains,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)&cooldown.bloodthirst.remains<=1&cooldown.bloodthirst.remains
                        //actions.two_targets+=/execute,if=debuff.colossus_smash.up
                        //actions.two_targets+=/raging_blow,if=buff.meat_cleaver.up|target.health.pct<20
                        //actions.two_targets+=/whirlwind,if=!buff.meat_cleaver.up
                        //actions.two_targets+=/battle_shout
                        //actions.two_targets+=/heroic_throw
                        )));
        }

        internal static Composite Sim_FuryOffensive()
        {
            return new PrioritySelector(
                //actions.single_target=bloodbath,if=enabled&(cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5|target.time_to_die<=20)
                Spell.Cast(SB.Bloodbath, ret => G.BbTalent && (G.CsCd < 2000 || G.RemainingCs(5000)) && Tier6AbilityUsage),
                //actions+=/recklessness,if=!talent.bloodbath.enabled&((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20))|buff.bloodbath.up&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20)|target.time_to_die<=12
                Spell.Cast(SB.Recklessness, ret => ((!G.BbTalent && (G.ExecutePhase || G.CsCd < 2000 || G.RemainingCs(5000))) || (G.BloodbathAura)) && RecklessnessUsage),
                //actions+=/avatar,if=enabled&(buff.recklessness.up|target.time_to_die<=25)
                Spell.Cast(SB.Avatar, ret => G.AvTalent && G.RecklessnessAura && Tier6AbilityUsage),
                //actions+=/skull_banner,if=buff.skull_banner.down&(((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&target.time_to_die>192&buff.cooldown_reduction.up)|buff.recklessness.up)
                Spell.Cast(SB.SkullBanner, ret => !G.SkullBannerAura && SkullBannerUsage && (G.RecklessnessAura || G.CsCd < 2000 || G.RemainingCs(5000))),
                //actions+=/berserker_rage,if=buff.enrage.remains<1&cooldown.bloodthirst.remains>1
                Spell.Cast(SB.BerserkerRage, ret => (G.FadingEnrage(1000) && !G.EnrageAura) && G.BtCd > 1000 && BerserkerRageUsage)
                );
        }

        internal static Composite Sim_FuryGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ImpendingVictory, ret => !G.IvOc && G.IvTalent && IS.Instance.Fury.CheckImpVic && Me.HealthPercent <= IS.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SB.VictoryRush, ret => !G.VrOc && G.VictoriousAura && IS.Instance.Fury.CheckVicRush && Me.HealthPercent <= IS.Instance.Fury.CheckVicRushNum),
                Spell.Cast(SB.IntimidatingShout, ret => IS.Instance.Fury.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.ShatteringThrow, ret => IS.Instance.Fury.CheckShatteringThrow && U.IsTargetBoss && (G.CsCd <= 3000 || G.SbCd <= 3000))
                );
        }

        internal static Composite Sim_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite Sim_FuryDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.DiebytheSword, ret => IS.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= IS.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SB.EnragedRegeneration, ret => G.ErTalent && IS.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= IS.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast(SB.ShieldWall, ret => IS.Instance.Fury.CheckShieldWall && Me.HealthPercent <= IS.Instance.Fury.CheckShieldWallNum),
                Spell.Cast(SB.SpellReflection, ret => IS.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && G.TargettingMe && Me.CurrentTarget.IsCasting),
                Item.FuryUseHealthStone()
                );
        }

        internal static Composite Sim_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SB.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && IS.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= IS.Instance.Fury.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast(SB.Hamstring, ret => !U.IsTargetBoss && !G.HamstringAura && (IS.Instance.Fury.HamString == Enum.Hamstring.Always || IS.Instance.Fury.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast(SB.MassSpellReflection, ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SB.PiercingHowl, ret => G.PhTalent && IS.Instance.Fury.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SB.StaggeringShout, ret => G.SsTalent && IS.Instance.Fury.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckPiercingHowlNum)
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

                        Spell.Cast(SB.Execute, ret => G.FadingDeathSentence(3000) && G.CsCd >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added - Outside CS window.
                        Spell.Cast(SB.StormBolt, ret => G.ReadinessAura && G.CsCd >= 16000 && Tier6AbilityUsage), // Added - When new one is ready in next CS window - With Eye of Galakras.

                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.HeroicStrike, ret => G.CsCd >= 3000 && ((G.UrGlyph && Me.CurrentRage >= Me.MaxRage - 15) || (!G.UrGlyph && Me.CurrentRage >= Me.MaxRage - 5))), // Also in Rel_FuryHeroicStrike().
                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow2S && G.CsCd >= 3000),
                        Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura),
                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow1S && G.CsCd >= 3000),
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me, ret => Me.CurrentRage < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me, ret => Me.CurrentRage < 70))),
                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.CsCd >= 6000 && Tier4AbilityUsage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage),
                        Spell.Cast(SB.WildStrike, ret => !G.BloodsurgeAura && G.CsCd >= 3000 && Me.CurrentRage >= 90),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage), // Also in Rel_FuryHeroicStrike().
                        Spell.Cast(SB.ImpendingVictory, ret => G.IvTalent && !G.IvOc && IS.Instance.Fury.CheckRotImpVic), // Added for the sake of supporting it rotational.
                        Spell.Cast(SB.HeroicThrow, ret => IS.Instance.Fury.CheckHeroicThrow) // Added for the sake of supporting it rotational.
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
                        Spell.Cast(SB.Execute, ret => G.FadingDeathSentence(3000) && G.CsCd >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.RagingBlow),
                        Spell.Cast(SB.Execute, ret => Me.CurrentRage == Me.MaxRage - 10),
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage), // Added
                        Spell.Cast(SB.Bladestorm, ret => G.BsTalent && G.BtCd >= 2000 && G.CsCd >= 6000 && Tier4AbilityUsage) // Added
                        )));
        }

        internal static Composite Rel_FuryHeroicStrike()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura && G.NormalPhase,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => Me.CurrentRage >= 30))),
                new Decorator(ret => !G.ColossusSmashAura && G.NormalPhase,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => G.CsCd >= 3000 && ((G.UrGlyph && Me.CurrentRage >= Me.MaxRage - 15) || (!G.UrGlyph && Me.CurrentRage >= Me.MaxRage - 5))))),
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
                Spell.Cast(SB.BerserkerRage, ret => (!G.EnrageAura || G.FadingEnrage(500)) && G.BtOc && G.ColossusSmashAura && BerserkerRageUsage),
                Spell.Cast(SB.Bloodbath, ret => G.BbTalent && Tier6AbilityUsage),
                Spell.Cast(SB.Recklessness, ret => RecklessnessUsage),
                Spell.Cast(SB.Avatar, ret => G.AvTalent && RecklessnessSync && Tier6AbilityUsage),
                Spell.Cast(SB.SkullBanner, ret => !G.SkullBannerAura && RecklessnessSync && SkullBannerUsage)
                );
        }

        internal static Composite Rel_FuryGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ImpendingVictory, ret => !G.IvOc && G.IvTalent && IS.Instance.Fury.CheckImpVic && Me.HealthPercent <= IS.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SB.VictoryRush, ret => !G.VrOc && G.VictoriousAura && IS.Instance.Fury.CheckVicRush && Me.HealthPercent <= IS.Instance.Fury.CheckVicRushNum),
                Spell.Cast(SB.IntimidatingShout, ret => IS.Instance.Fury.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.ShatteringThrow, ret => IS.Instance.Fury.CheckShatteringThrow && U.IsTargetBoss && (G.CsCd <= 3000 || G.SbCd <= 3000))
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
                Spell.Cast(SB.DiebytheSword, ret => IS.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= IS.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SB.EnragedRegeneration, ret => G.ErTalent && IS.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= IS.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast(SB.ShieldWall, ret => IS.Instance.Fury.CheckShieldWall && Me.HealthPercent <= IS.Instance.Fury.CheckShieldWallNum),
                Spell.Cast(SB.SpellReflection, ret => IS.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && G.TargettingMe && Me.CurrentTarget.IsCasting),
                Item.FuryUseHealthStone()
                );
        }

        internal static Composite Rel_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SB.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && IS.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= IS.Instance.Fury.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast(SB.Hamstring, ret => !U.IsTargetBoss && !G.HamstringAura && (IS.Instance.Fury.HamString == Enum.Hamstring.Always || IS.Instance.Fury.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast(SB.MassSpellReflection, ret => G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SB.PiercingHowl, ret => G.PhTalent && IS.Instance.Fury.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SB.StaggeringShout, ret => G.SsTalent && IS.Instance.Fury.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckPiercingHowlNum)
                );
        }
        #endregion

        #region Booleans

        internal static bool BerserkerRageUsage
        {
            get
            {
                return ((IS.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.BerserkerRage == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool MassSpellReflectionUsage
        {
            get
            {
                return ((IS.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.MassSpellReflection == Enum.MsrTrigger.Always && G.PuOc && G.DsOc));
            }
        }

        internal static bool RacialUsage
        {
            get
            {
                return ((IS.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.ClassRacials == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool RecklessnessUsage
        {
            get
            {
                return ((IS.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.Recklessness == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.Recklessness == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool SkullBannerUsage
        {
            get
            {
                return ((IS.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.SkullBanner == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.SkullBanner == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityUsage
        {
            get
            {
                return ((IS.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.Tier4Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityUsage
        {
            get
            {
                return ((IS.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.Tier6Abilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier4AbilityAoEUsage
        {
            get
            {
                return ((IS.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.Tier4AoeAbilities == Enum.AbilityTrigger.Always));
            }
        }

        internal static bool Tier6AbilityAoEUsage
        {
            get
            {
                return ((IS.Instance.Fury.Tier6AoeAbilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                        (IS.Instance.Fury.Tier6AoeAbilities == Enum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                        (IS.Instance.Fury.Tier6AoeAbilities == Enum.AbilityTrigger.Always));
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