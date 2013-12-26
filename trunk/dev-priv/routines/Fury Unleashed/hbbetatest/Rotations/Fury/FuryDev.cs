using CommonBehaviors.Actions;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using G = FuryUnleashed.Rotations.Global;
using FG = FuryUnleashed.Rotations.Fury.FuryGlobal;
using Lua = FuryUnleashed.Core.Helpers.LuaClass;

namespace FuryUnleashed.Rotations.Fury
{
    internal class FuryDev
    {
        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
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
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_FurySt())
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
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_FurySt())
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
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_FurySt())
                                        ))))));
            }
        }

        internal static Composite Dev_FurySt()
        {
            return new PrioritySelector(
                //# A very brief overview of the fury single target rotation, keep in mind that this does not include various subtle aspects in the action list, and is only intended for newer players.
                //# Fury is a priority-based rotation with moderate amounts of rage management. It is  played in a 20-21 second cycle, based around usage of Colossus Smash.
                //# Bloodthirst is used on cooldown, raging blow is a higher priority than bloodsurge-buffed wild strikes, and the player should attempt to save up rage by foregoing usage of 'rage dumps' such as
                //# Heroic strike and non-bloodsurge buffed wild strikes when colossus smash is not applied to the target. The goal is to go into using colossus smash with 100-115~ rage
                //# and then expend all of this rage by using heroic strike 3-4 times during colossus smash. It's also a good idea to save 1 charge of raging blow to use inside of this 6.5 second window.
                //# Cooldowns are stacked whenever possible, and only delayed for the very last use of them.

                //actions.single_target+=/heroic_leap,if=debuff.colossus_smash.up
                //actions.single_target+=/bloodthirst,if=!buff.enrage.up
                Spell.Cast(SpellBook.Bloodthirst, ret => !G.EnrageAura),
                //# Galakras cooldown reduction trinket allows Storm Bolt to be synced with Colossus Smash. 'buff.cooldown_reduction.up' is a hackish way of seeing if the trinket is equipped.
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.up&debuff.colossus_smash.up
                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && G.ReadinessAura && G.ColossusSmashAura && FG.Tier6AbilityUsage),
                //# Delay Bloodthirst if 2 stacks of raging blow are available inside Colossus Smash.
                //actions.single_target+=/raging_blow,if=buff.raging_blow.stack=2&debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast(SpellBook.RagingBlow, ret => G.RagingBlow2S && G.ColossusSmashAura && G.NormalPhase),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down&debuff.colossus_smash.up
                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && !G.ReadinessAura && G.ColossusSmashAura && FG.Tier6AbilityUsage),
                //actions.single_target+=/bloodthirst
                Spell.Cast(SpellBook.Bloodthirst),
                //# The GCD reduction of the Bloodsurge buff allows 3 Wild Strikes in-between Bloodthirst.
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.react&target.health.pct>=20&cooldown.bloodthirst.remains<=1
                Spell.Cast(SpellBook.WildStrike, ret => G.BloodsurgeAura && G.NormalPhase && G.BloodthirstSpellCooldown <= 1000),
                //actions.single_target+=/wait,sec=cooldown.bloodthirst.remains,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)&cooldown.bloodthirst.remains<=1&cooldown.bloodthirst.remains
                new Decorator(ret => G.NormalPhase && !G.ColossusSmashAura && Lua.PlayerPower < 30 && !G.EnrageAura && G.BloodthirstSpellCooldown <= 1000, new ActionAlwaysSucceed()),
                //actions.single_target+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && !G.ColossusSmashAura && FG.BloodbathSync && FG.Tier4AbilityUsage),
                //# The debuff from Colossus Smash lasts 6.5 seconds and also has 0.25~ seconds of travel time. This allows 4 1.5 second globals to be used inside of it every time now.
                //actions.single_target+=/colossus_smash
                Spell.Cast(SpellBook.ColossusSmash),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down
                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && G.ReadinessAura && FG.Tier6AbilityUsage),
                //actions.single_target+=/execute,if=debuff.colossus_smash.up|rage>70|target.time_to_die<12
                Spell.Cast(SpellBook.Execute, ret => G.ColossusSmashAura || Lua.PlayerPower > 70 || G.DumpAllRage),
                //actions.single_target+=/raging_blow,if=target.health.pct<20|buff.raging_blow.stack=2|debuff.colossus_smash.up|buff.raging_blow.remains<=3
                Spell.Cast(SpellBook.RagingBlow, ret => G.ExecutePhase || G.RagingBlow2S || G.ColossusSmashAura || G.FadingRagingBlow(3000)),
                //# Titan's Grip Bladestorm is competitive with Dragon Roar (Not quite better) with a single target.
                //actions.single_target+=/bladestorm,if=enabled
                Spell.Cast(SpellBook.Bladestorm, ret => G.BladestormTalent && FG.Tier4AbilityUsage),
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.up
                Spell.Cast(SpellBook.WildStrike, ret => G.BloodsurgeAura),
                //# If Colossus Smash is coming up soon, it's a good idea to save 1 charge of raging blow for it.
                //actions.single_target+=/raging_blow,if=cooldown.colossus_smash.remains>=3
                Spell.Cast(SpellBook.RagingBlow, ret => G.ColossusSmashSpellCooldown >= 3000),
                //actions.single_target+=/shockwave,if=enabled
                Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && FG.Tier4AbilityUsage),
                //actions.single_target+=/heroic_throw,if=debuff.colossus_smash.down&rage<60
                Spell.Cast(SpellBook.HeroicThrow, ret => !G.ColossusSmashAura && Lua.PlayerPower < 60 && FG.HeroicThrowUsage),
                //# Shattering Throw is a very small personal dps increase, but a fairly significant raid dps increase. Only use to fill empty globals.
                //actions.single_target+=/shattering_throw,if=cooldown.colossus_smash.remains>5
                Spell.Cast(SpellBook.ShatteringThrow, ret => G.ColossusSmashSpellCooldown >= 5000 && FG.ShatteringThrowUsage),
                //actions.single_target+=/wild_strike,if=debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast(SpellBook.WildStrike, ret => G.ColossusSmashAura && G.NormalPhase),
                //actions.single_target+=/battle_shout,if=rage<70
                new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 70)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 70))),
                //actions.single_target+=/wild_strike,if=cooldown.colossus_smash.remains>=2&rage>=70&target.health.pct>=20
                Spell.Cast(SpellBook.WildStrike, ret => G.ColossusSmashSpellCooldown >= 2000 && Lua.PlayerPower >= 70 && G.NormalPhase),
                //actions.single_target+=/impending_victory,if=enabled&target.health.pct>=20&cooldown.colossus_smash.remains>=2
                Spell.Cast(SpellBook.ImpendingVictory, ret => G.ImpendingVictoryTalent && G.NormalPhase && G.ColossusSmashSpellCooldown >= 2000 && FG.ImpendingVictoryUsage)
                );
        }

        internal static Composite Dev_FuryHeroicStrike()
        {
            return new PrioritySelector(
                new Decorator(ret => !InternalSettings.Instance.Fury.CheckAoE || Unit.NearbyAttackableUnitsCount < InternalSettings.Instance.Fury.CheckAoENum,
                    new PrioritySelector(
                        //actions.single_target+=/heroic_strike,if=(debuff.colossus_smash.up&rage>=40&target.health.pct>=20|rage>=100)&buff.enrage.up
                        Spell.Cast(SpellBook.HeroicStrike, ret => (G.ColossusSmashAura && Lua.PlayerPower >= 40 && G.NormalPhase || Lua.PlayerPower >= 100) && G.EnrageAura, true))),
                new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Cleave, ret => true, true))));
        }

        // Using this for Execute Phase - http://www.icy-veins.com/fury-warrior-wow-pve-dps-rotation-cooldowns-abilities
        internal static Composite Dev_FuryExec()
        {
            return new PrioritySelector(
                );
        }

        // Using this for AoE Phase - http://www.icy-veins.com/fury-warrior-wow-pve-dps-rotation-cooldowns-abilities
        internal static Composite Dev_FuryMt()
        {
            return new PrioritySelector(
                //actions.two_targets=bloodbath,if=enabled&buff.enrage.up
                //actions.two_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                //actions.two_targets+=/heroic_leap,if=buff.enrage.up
                //# Generally, if an encounter has any type of AoE, Bladestorm will be the better choice.
                //actions.two_targets+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                //actions.two_targets+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                //actions.two_targets+=/shockwave,if=enabled
                //actions.two_targets+=/colossus_smash
                //# Keep deep wounds on as many targets as possible.
                //actions.two_targets+=/bloodthirst,cycle_targets=1,if=dot.deep_wounds.remains<5
                //actions.two_targets+=/bloodthirst,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)
                //actions.two_targets+=/storm_bolt,if=enabled&debuff.colossus_smash.up
                //actions.two_targets+=/wait,sec=cooldown.bloodthirst.remains,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)&cooldown.bloodthirst.remains<=1&cooldown.bloodthirst.remains
                //actions.two_targets+=/execute,if=debuff.colossus_smash.up
                //actions.two_targets+=/raging_blow,if=buff.meat_cleaver.up|target.health.pct<20
                //actions.two_targets+=/whirlwind,if=!buff.meat_cleaver.up
                //actions.two_targets+=/battle_shout,if=rage<70
                //actions.two_targets+=/heroic_throw

                //actions.three_targets=bloodbath,if=enabled&buff.enrage.up
                //actions.three_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                //actions.three_targets+=/heroic_leap,if=buff.enrage.up
                //actions.three_targets+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                //actions.three_targets+=/shockwave,if=enabled
                //actions.three_targets+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                //actions.three_targets+=/colossus_smash
                //actions.three_targets+=/storm_bolt,if=enabled&debuff.colossus_smash.up
                //actions.three_targets+=/raging_blow,if=buff.meat_cleaver.stack=2
                //actions.three_targets+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking
                //actions.three_targets+=/whirlwind
                //actions.three_targets+=/raging_blow
                //actions.three_targets+=/battle_shout,if=rage<70
                //actions.three_targets+=/heroic_throw

                //actions.aoe=bloodbath,if=enabled&buff.enrage.up
                //actions.aoe+=/cleave,if=rage>110
                //actions.aoe+=/heroic_leap,if=buff.enrage.up
                //# Dragon roar is a poor choice on large-scale AoE as the damage it does is reduced with additional targets. The damage it does per target is reduced by the following amounts:
                //# 1/2/3/4/5+ targets ---> 0%/25%/35%/45%/50%
                //actions.aoe+=/dragon_roar,if=enabled&debuff.colossus_smash.down&(buff.bloodbath.up|!talent.bloodbath.enabled)
                //actions.aoe+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                //actions.aoe+=/storm_bolt,if=enabled&debuff.colossus_smash.up
                //# Note: The 20 second reduction when hitting 3+ targets is not modeled.
                //actions.aoe+=/shockwave,if=enabled
                //# Enrage overlaps 4 GCDs, which allows bloodthirst to be used mostly to keep enrage up, as rage income is typically not an issue with the aoe rotation.
                //actions.aoe+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking&buff.enrage.down
                //actions.aoe+=/raging_blow,if=buff.meat_cleaver.stack=3
                //actions.aoe+=/whirlwind
                //actions.aoe+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking
                //actions.aoe+=/colossus_smash
                //actions.aoe+=/battle_shout,if=rage<70
                );
        }

        internal static Composite Dev_FuryOffensive()
        {
            return new PrioritySelector(
                //actions=auto_attack
                //actions+=/mogu_power_potion,if=(target.health.pct<20&buff.recklessness.up)|target.time_to_die<=25
                //# This incredibly long line can be translated to 'Use recklessness on cooldown with colossus smash; unless the boss will die before the ability is usable again, and then combine with execute instead.'
                //actions+=/recklessness,if=!talent.bloodbath.enabled&((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20))|buff.bloodbath.up&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20)|target.time_to_die<=12
                //actions+=/avatar,if=enabled&(buff.recklessness.up|target.time_to_die<=25)
                //actions+=/skull_banner,if=buff.skull_banner.down&(((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&target.time_to_die>192&buff.cooldown_reduction.up)|buff.recklessness.up)
                //actions+=/use_item,slot=hands,if=!talent.bloodbath.enabled&debuff.colossus_smash.up|buff.bloodbath.up
                //actions+=/blood_fury,if=buff.cooldown_reduction.down&(buff.bloodbath.up|(!talent.bloodbath.enabled&debuff.colossus_smash.up))|buff.cooldown_reduction.up&buff.recklessness.up
                //actions+=/berserking,if=buff.cooldown_reduction.down&(buff.bloodbath.up|(!talent.bloodbath.enabled&debuff.colossus_smash.up))|buff.cooldown_reduction.up&buff.recklessness.up
                //actions+=/arcane_torrent,if=buff.cooldown_reduction.down&(buff.bloodbath.up|(!talent.bloodbath.enabled&debuff.colossus_smash.up))|buff.cooldown_reduction.up&buff.recklessness.up
                //# There is a 0.25~ second delay in enrage application, this delay allows enrage to cover 4 GCDs of ability usage.
                //actions+=/berserker_rage,if=buff.enrage.remains<1&cooldown.bloodthirst.remains>1
                //actions+=/run_action_list,name=single_target,if=active_enemies=1
                //actions+=/run_action_list,name=two_targets,if=active_enemies=2
                //actions+=/run_action_list,name=three_targets,if=active_enemies=3
                //actions+=/run_action_list,name=aoe,if=active_enemies>3

                //actions.single_target=bloodbath,if=enabled&(cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5|target.time_to_die<=20)
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
    }
}
