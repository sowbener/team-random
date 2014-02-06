using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using Enum = FuryUnleashed.Core.Helpers.Enum;
using AG = FuryUnleashed.Rotations.Arms.ArmsGlobal;
using G = FuryUnleashed.Rotations.Global;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using Lua = FuryUnleashed.Core.Helpers.LuaClass;
using U = FuryUnleashed.Core.Unit;

namespace FuryUnleashed.Rotations.Arms
{
    class ArmsDev
    {
        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        internal static Composite DevArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Action(delegate { Item.RefreshSecondaryStats(); return RunStatus.Failure; }),
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                ArmsRel.RelArmsCombat
                                )),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                ArmsRel.RelArmsCombat
                                )),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                ArmsRel.RelArmsCombat
                                ))));
            }
        }

        internal static Composite Rel_ArmsSt()
        {
            return new PrioritySelector(
                //actions.single_target+=/mortal_strike,if=dot.deep_wounds.remains<1.0|buff.enrage.down|rage<10
                Spell.Cast(SpellBook.MortalStrike, ret => G.FadingDeepWounds(1000) || !G.EnrageAura || Lua.PlayerPower < 10),
                //actions.single_target+=/colossus_smash,if=debuff.colossus_smash.remains<1.0
                Spell.Cast(SpellBook.ColossusSmash, ret => !G.ColossusSmashAura || G.FadingColossusSmash(1000)),
                //actions.single_target+=/mortal_strike
                Spell.Cast(SpellBook.MortalStrike),
                //actions.single_target+=/storm_bolt,if=enabled&debuff.colossus_smash.up
                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && G.ColossusSmashAura),
                //actions.single_target+=/dragon_roar,if=enabled&debuff.colossus_smash.down
                Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && !G.ColossusSmashAura),
                //actions.single_target+=/execute,if=buff.sudden_execute.down|buff.taste_for_blood.down|rage>90|target.time_to_die<12
                Spell.Cast(SpellBook.Execute, ret => G.ExecutePhase && (!G.SuddenExecAura || !G.TasteforBloodAura || Lua.PlayerPower > 90 || G.AlmostDead)),
                //# Slam is preferable to overpower with crit procs/recklessness.
                //actions.single_target+=/slam,if=target.health.pct>=20&(stat.crit>25000|buff.recklessness.up)
                Spell.Cast(SpellBook.Slam, ret => G.NormalPhase && (Item.CritRating > 25000 || G.RecklessnessAura)),
                //actions.single_target+=/overpower,if=target.health.pct>=20&rage<100|buff.sudden_execute.up
                Spell.Cast(SpellBook.Overpower, ret => G.NormalPhase && Lua.PlayerPower < 100 || G.SuddenExecAura),
                //actions.single_target+=/slam,if=target.health.pct>=20
                Spell.Cast(SpellBook.Slam, ret => G.NormalPhase),
                //actions.single_target+=/battle_shout
                new Switch<Enum.Shouts>(ctx => IS.Instance.Arms.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me))),
                //actions.single_target+=/heroic_throw
                Spell.Cast(SpellBook.HeroicThrow, ret => AG.HeroicThrowUsage)
                );
        }

        internal static Composite Rel_ArmsHeroicStrike()
        {
            return new PrioritySelector(
                //actions.single_target=heroic_strike,if=rage>115|(debuff.colossus_smash.up&rage>60&set_bonus.tier16_2pc_melee)
                Spell.Cast(SpellBook.HeroicStrike, ret => Lua.PlayerPower > Lua.PlayerPowerMax - 5 || G.ColossusSmashAura && Lua.PlayerPower > 60 && G.Tier16TwoPieceBonus, true)
                );
        }

        internal static Composite Rel_ArmsExec()
        {
            return new PrioritySelector(
                Rel_ArmsSt()
                );
        }

        internal static Composite Rel_ArmsMt()
        {
            return new PrioritySelector(
                //actions.aoe=sweeping_strikes
                Spell.Cast(SpellBook.SweepingStrikes),
                //actions.aoe+=/cleave,if=rage>110&active_enemies<=4
                //actions.aoe+=/bladestorm,if=enabled&(buff.bloodbath.up|!talent.bloodbath.enabled)
                //actions.aoe+=/dragon_roar,if=enabled&debuff.colossus_smash.down
                //actions.aoe+=/colossus_smash,if=debuff.colossus_smash.remains<1
                //actions.aoe+=/thunder_clap,target=2,if=dot.deep_wounds.attack_power*1.1<stat.attack_power
                //actions.aoe+=/mortal_strike,if=active_enemies=2|rage<50
                //actions.aoe+=/execute,if=buff.sudden_execute.down&active_enemies=2
                Spell.Cast(SpellBook.Execute, ret => !G.SuddenExecAura && U.NearbyAttackableUnitsCount == 2),
                //actions.aoe+=/slam,if=buff.sweeping_strikes.up&debuff.colossus_smash.up
                Spell.Cast(SpellBook.Slam, ret => G.SweepingStrikesAura && G.ColossusSmashAura),
                //actions.aoe+=/overpower,if=active_enemies=2
                Spell.Cast(SpellBook.Overpower, ret => U.NearbyAttackableUnitsCount == 2),
                //actions.aoe+=/slam,if=buff.sweeping_strikes.up
                Spell.Cast(SpellBook.Slam, ret => G.SweepingStrikesAura),
                //actions.aoe+=/battle_shout
                new Switch<Enum.Shouts>(ctx => IS.Instance.Arms.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me)))
                );
        }

        internal static Composite Rel_ArmsOffensive()
        {
            return new PrioritySelector(
                //actions=auto_attack
                //actions+=/mogu_power_potion,if=(target.health.pct<20&buff.recklessness.up)|buff.bloodlust.react|target.time_to_die<=25
                //# This incredibly long line (Due to differing talent choices) says 'Use recklessness on cooldown with colossus smash, unless the boss will die before the ability is usable again, and then use it with execute.'
                //actions+=/recklessness,if=!talent.bloodbath.enabled&((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20))|buff.bloodbath.up&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20)|target.time_to_die<=12
                //actions+=/avatar,if=enabled&(buff.recklessness.up|target.time_to_die<=25)
                //actions+=/skull_banner,if=buff.skull_banner.down&(((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&target.time_to_die>192&buff.cooldown_reduction.up)|buff.recklessness.up)
                //actions+=/use_item,slot=hands,if=!talent.bloodbath.enabled&debuff.colossus_smash.up|buff.bloodbath.up
                //actions+=/blood_fury,if=buff.cooldown_reduction.down&(buff.bloodbath.up|(!talent.bloodbath.enabled&debuff.colossus_smash.up))|buff.cooldown_reduction.up&buff.recklessness.up
                //actions+=/berserking,if=buff.cooldown_reduction.down&(buff.bloodbath.up|(!talent.bloodbath.enabled&debuff.colossus_smash.up))|buff.cooldown_reduction.up&buff.recklessness.up
                //actions+=/arcane_torrent,if=buff.cooldown_reduction.down&(buff.bloodbath.up|(!talent.bloodbath.enabled&debuff.colossus_smash.up))|buff.cooldown_reduction.up&buff.recklessness.up
                //actions+=/bloodbath,if=enabled&(debuff.colossus_smash.up|cooldown.colossus_smash.remains<4|target.time_to_die<=20)
                //actions+=/berserker_rage,if=buff.enrage.remains<0.5
                //actions+=/heroic_leap,if=debuff.colossus_smash.up
                //actions+=/run_action_list,name=aoe,if=active_enemies>=2
                //actions+=/run_action_list,name=single_target,if=active_enemies<2
                );
        }

        internal static Composite Rel_ArmsGcdUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Rel_ArmsRacials()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Rel_ArmsDefensive()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Rel_ArmsNonGcdUtility()
        {
            return new PrioritySelector(
                );
        }
    }
}
