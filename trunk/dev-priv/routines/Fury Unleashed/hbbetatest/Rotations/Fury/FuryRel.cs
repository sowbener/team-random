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
using FG = FuryUnleashed.Rotations.Fury.FuryGlobal;
using G = FuryUnleashed.Rotations.Global;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using Lua = FuryUnleashed.Core.Helpers.LuaClass;
using U = FuryUnleashed.Core.Unit;

namespace FuryUnleashed.Rotations.Fury
{
    internal class FuryRel
    {
        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
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
                                new Decorator(ret => FG.StanceDanceUsage, Rel_FuryStanceDance()),
                                Rel_FuryNonGcdUtility(),
                                Rel_FuryRacials(),
                                Rel_FuryOffensive(),
                                Item.CreateItemBehaviour(),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => FG.MultiTargetUsage && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_FuryDefensive()),
                                new Decorator(ret => FG.StanceDanceUsage, Rel_FuryStanceDance()),
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
                                        new Decorator(ret => FG.MultiTargetUsage && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_FuryDefensive()),
                                new Decorator(ret => FG.StanceDanceUsage, Rel_FuryStanceDance()),
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
                                        new Decorator(ret => FG.MultiTargetUsage && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_FurySt())
                                        ))))));
            }
        }

        internal static Composite Rel_FurySt()
        {
            return new PrioritySelector(
                //Added for Supporting it.
                Spell.Cast(SpellBook.Execute, ret => G.DeathSentenceAuraT16 && G.ColossusSmashAura || G.FadingDeathSentence(3000) && G.ColossusSmashSpellCooldown >= 1500), // Added T16 P4.
                //actions.single_target+=/heroic_leap,if=debuff.colossus_smash.up
                new Decorator(ret => FG.HeroicLeapUsage && U.IsViable(Me.CurrentTarget) && G.ColossusSmashAura && Me.CurrentTarget.Distance >= 8 && Me.CurrentTarget.Distance <= 40,
                    Spell.CastOnGround(SpellBook.HeroicLeap, on => Me.CurrentTarget.Location)),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.up&debuff.colossus_smash.up
                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && G.ReadinessAura && G.ColossusSmashAura && FG.Tier6AbilityUsage),
                //actions.single_target+=/raging_blow,if=buff.raging_blow.stack=2&debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast(SpellBook.RagingBlow, ret => G.RagingBlow2S && G.ColossusSmashAura && G.NormalPhase),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down&debuff.colossus_smash.up
                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && !G.ReadinessAura && G.ColossusSmashAura && FG.Tier6AbilityUsage),
                //actions.single_target+=/bloodthirst,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)
                Spell.MultiDoT(SpellBook.Bloodbath, AuraBook.DeepWounds, 1500, 5, 160, ret => FG.MultiTargetUsage),
                Spell.Cast(SpellBook.Bloodthirst),
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.react&target.health.pct>=20&cooldown.bloodthirst.remains<=1
                Spell.Cast(SpellBook.WildStrike, ret => G.BloodsurgeAura && G.NormalPhase && G.BloodthirstSpellCooldown <= 1000),
                //actions.single_target+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && (!G.ColossusSmashAura && FG.BloodbathSync) && FG.Tier4AbilityUsage),
                //actions.single_target+=/colossus_smash
                Spell.Cast(SpellBook.ColossusSmash),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down
                Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && !G.ReadinessAura && FG.Tier6AbilityUsage),
                //actions.single_target+=/execute,if=debuff.colossus_smash.up|rage>70|target.time_to_die<12
                Spell.Cast(SpellBook.Execute, ret => G.ColossusSmashAura || Lua.PlayerPower > 70),
                //actions.single_target+=/raging_blow,if=target.health.pct<20|buff.raging_blow.stack=2|(debuff.colossus_smash.up|(cooldown.bloodthirst.remains>=1&buff.raging_blow.remains<=3))
                Spell.Cast(SpellBook.RagingBlow, ret => G.ExecutePhase || G.RagingBlow2S || (G.ColossusSmashAura || G.BloodthirstSpellCooldown >= 1 && G.FadingRagingBlow(3000))),
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.up
                Spell.Cast(SpellBook.WildStrike, ret => G.BloodsurgeAura),
                //actions.single_target+=/bladestorm,if=enabled&cooldown.bloodthirst.remains>2
                Spell.Cast(SpellBook.Bladestorm, ret => G.BladestormTalent && G.BloodthirstSpellCooldown > 2000 && FG.Tier4AbilityUsage),
                //actions.single_target+=/raging_blow,if=cooldown.colossus_smash.remains>=3
                Spell.Cast(SpellBook.RagingBlow, ret => G.ColossusSmashSpellCooldown >= 3000),
                //actions.single_target+=/shockwave,if=enabled
                Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && FG.Tier4AbilityUsage),
                //actions.single_target+=/heroic_throw,if=debuff.colossus_smash.down&rage<60
                Spell.Cast(SpellBook.HeroicThrow, ret => !G.ColossusSmashAura && Lua.PlayerPower < 60 && FG.HeroicThrowUsage),
                //actions.single_target+=/battle_shout,if=rage<70&!debuff.colossus_smash.up
                new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 70 && !G.ColossusSmashAura)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 70 && !G.ColossusSmashAura))),
                //actions.single_target+=/wild_strike,if=debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast(SpellBook.WildStrike, ret => G.ColossusSmashAura && G.NormalPhase),
                //actions.single_target+=/battle_shout,if=rage<70
                new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 70)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 70))),
                //actions.single_target+=/wild_strike,if=cooldown.colossus_smash.remains>=2&rage>=70&target.health.pct>=20
                Spell.Cast(SpellBook.WildStrike, ret => G.ColossusSmashSpellCooldown >= 2000 && Lua.PlayerPower >= 70 && G.NormalPhase),
                //actions.single_target+=/impending_victory,if=enabled&target.health.pct>=20&cooldown.colossus_smash.remains>=2
                Spell.Cast(SpellBook.ImpendingVictory, ret => G.ImpendingVictoryTalent && !G.ImpendingVictoryOnCooldown && G.NormalPhase && G.ColossusSmashSpellCooldown >= 2000 && FG.RotationalImpendingVictoryUsage)
                );
        }

        internal static Composite Rel_FuryStanceDance()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.BattleStance, ret => !G.BattleStanceAura && !DamageTracker.CalculateBerserkerStance(), true),
                Spell.Cast(SpellBook.BerserkerStance, ret => !G.BerserkerStanceAura && DamageTracker.CalculateBerserkerStance(), true));
        }

        internal static Composite Rel_FuryHeroicStrike()
        {
            return new PrioritySelector(
                new Decorator(ret => (!FG.MultiTargetUsage || U.NearbyAttackableUnitsCount < IS.Instance.Fury.CheckAoENum) && G.NormalPhase,
                    new PrioritySelector(
                        //actions.single_target+=/heroic_strike,if=(debuff.colossus_smash.up&rage>=40&target.health.pct>=20|rage>=100)&buff.enrage.up
                        Spell.Cast(SpellBook.HeroicStrike, ret => (G.ColossusSmashAura && Lua.PlayerPower >= 40 || Lua.PlayerPower >= 100) && G.EnrageAura, true))),
                new Decorator(ret => FG.MultiTargetUsage && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum,
                    new PrioritySelector(
                        new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                            //actions.two_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                            Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower >= 60 && G.ColossusSmashAura || Lua.PlayerPower > 90, true)),
                        new Decorator(ret => U.NearbyAttackableUnitsCount == 3,
                            //actions.three_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                            Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower >= 60 && G.ColossusSmashAura || Lua.PlayerPower > 90, true)),
                        new Decorator(ret => U.NearbyAttackableUnitsCount >= 3,
                            //actions.aoe+=/cleave,if=rage>110
                            Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower > Lua.PlayerPowerMax - 10, true)))));
        }

        internal static Composite Rel_FuryExec()
        {
            return new PrioritySelector(
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        //Prevent Rage Capping - With passthrough to the next item in the same tree.
                        Spell.Cast(SpellBook.HeroicStrike, ret => Lua.PlayerPower >= Lua.PlayerPowerMax - 5, true),
                        //Only if Bloodthirst and Colossus Smash are not available, you have 1 charge of Raging Blow, your rage is over 80, and the global cooldown after it would not be Colossus Smash.
                        Spell.Cast(SpellBook.Execute, ret => G.BloodThirstOnCooldown && G.ColossusSmashSpellCooldown > 3000 && G.RagingBlow1S && Lua.PlayerPower > 80),
                        //Use Colossus Smash.
                        Spell.Cast(SpellBook.ColossusSmash),
                        //Use regular rotation.
                        Rel_FurySt())),
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.HeroicStrike, ret => Lua.PlayerPower >= Lua.PlayerPowerMax - 5, true),
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && FG.Tier6AbilityUsage),
                        new Decorator(ret => FG.HeroicLeapUsage && U.IsViable(Me.CurrentTarget) && G.ColossusSmashAura && Me.CurrentTarget.Distance >= 8 && Me.CurrentTarget.Distance <= 40,
                            Spell.CastOnGround(SpellBook.HeroicLeap, on => Me.CurrentTarget.Location)),
                        Spell.Cast(SpellBook.Bloodthirst, ret => !G.EnrageAura && G.BerserkerRageOnCooldown),
                        Spell.Cast(SpellBook.Execute),
                        Spell.Cast(SpellBook.RagingBlow))));
        }

        internal static Composite Rel_FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        //actions.two_targets=bloodbath,if=enabled&buff.enrage.up
                        Spell.Cast(SpellBook.Bloodbath, ret => G.BloodbathTalent && G.EnrageAura && FG.Tier6AbilityAoEUsage, true),
                        //actions.two_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                        Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower >= 60 && G.ColossusSmashAura || Lua.PlayerPower > 90, true),
                        //actions.two_targets+=/heroic_leap,if=buff.enrage.up
                        new Decorator(ret => FG.HeroicLeapUsage && U.IsViable(Me.CurrentTarget) && G.EnrageAura && Me.CurrentTarget.Distance >= 8 && Me.CurrentTarget.Distance <= 40,
                            Spell.CastOnGround(SpellBook.HeroicLeap, on => Me.CurrentTarget.Location)),
                        //# Generally, if an encounter has any type of AoE, Bladestorm will be the better choice.
                        //actions.two_targets+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                        Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && !G.ColossusSmashAura && FG.BloodbathSync && FG.Tier4AbilityAoEUsage),
                        //actions.two_targets+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        Spell.Cast(SpellBook.Bladestorm, ret => G.BladestormTalent && G.EnrageAura && FG.BloodbathSync && FG.Tier4AbilityAoEUsage),
                        //actions.two_targets+=/shockwave,if=enabled
                        Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && FG.Tier4AbilityAoEUsage),
                        //actions.two_targets+=/colossus_smash
                        Spell.Cast(SpellBook.ColossusSmash),
                        //# Keep deep wounds on as many targets as possible.
                        //actions.two_targets+=/bloodthirst,cycle_targets=1,if=dot.deep_wounds.remains<5
                        Spell.MultiDoT(SpellBook.Bloodthirst, AuraBook.DeepWounds, 5000),
                        //actions.two_targets+=/bloodthirst,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)
                        Spell.Cast(SpellBook.Bloodthirst, ret => G.NormalPhase && !G.ColossusSmashAura && Lua.PlayerPower < 30 && !G.EnrageAura),
                        //actions.two_targets+=/storm_bolt,if=enabled&debuff.colossus_smash.up
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && G.ColossusSmashAura && FG.Tier6AbilityAoEUsage),
                        //actions.two_targets+=/wait,sec=cooldown.bloodthirst.remains,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)&cooldown.bloodthirst.remains<=1&cooldown.bloodthirst.remains
                        new Decorator(ret => G.NormalPhase && !G.ColossusSmashAura && Lua.PlayerPower < 30 && !G.EnrageAura && G.BloodthirstSpellCooldown <= 1000, new ActionAlwaysSucceed()),
                        //actions.two_targets+=/execute,if=debuff.colossus_smash.up
                        Spell.Cast(SpellBook.Execute, ret => G.ColossusSmashAura),
                        //actions.two_targets+=/raging_blow,if=buff.meat_cleaver.up|target.health.pct<20
                        Spell.Cast(SpellBook.RagingBlow, ret => G.MeatCleaverAura || G.ExecutePhase),
                        //actions.two_targets+=/whirlwind,if=!buff.meat_cleaver.up
                        Spell.Cast(SpellBook.Whirlwind, ret => !G.MeatCleaverAura),
                        //actions.two_targets+=/battle_shout,if=rage<70
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 70))),
                        //actions.two_targets+=/heroic_throw
                        Spell.Cast(SpellBook.HeroicThrow, ret => FG.HeroicThrowUsage)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(
                        //actions.three_targets=bloodbath,if=enabled&buff.enrage.up
                        Spell.Cast(SpellBook.Bloodbath, ret => G.BloodbathTalent && G.EnrageAura && FG.Tier6AbilityAoEUsage, true),
                        //actions.three_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                        Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower >= 60 && G.ColossusSmashAura || Lua.PlayerPower > 90, true),
                        //actions.three_targets+=/heroic_leap,if=buff.enrage.up
                        new Decorator(ret => FG.HeroicLeapUsage && U.IsViable(Me.CurrentTarget) && G.EnrageAura && Me.CurrentTarget.Distance >= 8 && Me.CurrentTarget.Distance <= 40,
                            Spell.CastOnGround(SpellBook.HeroicLeap, on => Me.CurrentTarget.Location)),
                        //actions.three_targets+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                        Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && !G.ColossusSmashAura && FG.BloodbathSync && FG.Tier4AbilityAoEUsage),
                        //actions.three_targets+=/shockwave,if=enabled
                        Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && FG.Tier4AbilityAoEUsage),
                        //actions.three_targets+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        Spell.Cast(SpellBook.Bladestorm, ret => G.BladestormTalent && G.EnrageAura && FG.BloodbathSync && FG.Tier4AbilityAoEUsage),
                        //actions.three_targets+=/colossus_smash
                        Spell.Cast(SpellBook.ColossusSmash),
                        //actions.three_targets+=/storm_bolt,if=enabled&debuff.colossus_smash.up
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && G.ColossusSmashAura && FG.Tier6AbilityAoEUsage),
                        //actions.three_targets+=/raging_blow,if=buff.meat_cleaver.stack=2
                        Spell.Cast(SpellBook.RagingBlow, ret => G.MeatCleaverAuraS2),
                        //actions.three_targets+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking
                        Spell.MultiDoT(SpellBook.Bloodthirst, AuraBook.DeepWounds),
                        //actions.three_targets+=/whirlwind
                        Spell.Cast(SpellBook.Whirlwind),
                        //actions.three_targets+=/raging_blow
                        Spell.Cast(SpellBook.RagingBlow),
                        //actions.three_targets+=/battle_shout,if=rage<70
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 70))),
                        //actions.three_targets+=/heroic_throw
                        Spell.Cast(SpellBook.HeroicThrow, ret => FG.HeroicThrowUsage)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 3,
                    new PrioritySelector(
                        //actions.aoe=bloodbath,if=enabled&buff.enrage.up
                        Spell.Cast(SpellBook.Bloodbath, ret => G.BloodbathTalent && G.EnrageAura && FG.Tier6AbilityAoEUsage, true),
                        //actions.aoe+=/cleave,if=rage>110
                        Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower > Lua.PlayerPowerMax - 10, true),
                        //actions.aoe+=/heroic_leap,if=buff.enrage.up
                        new Decorator(ret => FG.HeroicLeapUsage && U.IsViable(Me.CurrentTarget) && G.EnrageAura && Me.CurrentTarget.Distance >= 8 && Me.CurrentTarget.Distance <= 40,
                            Spell.CastOnGround(SpellBook.HeroicLeap, on => Me.CurrentTarget.Location)),
                        //# Dragon roar is a poor choice on large-scale AoE as the damage it does is reduced with additional targets. The damage it does per target is reduced by the following amounts:
                        //# 1/2/3/4/5+ targets ---> 0%/25%/35%/45%/50%
                        //actions.aoe+=/dragon_roar,if=enabled&debuff.colossus_smash.down&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && !G.ColossusSmashAura && FG.BloodbathSync && FG.Tier4AbilityAoEUsage),
                        //actions.aoe+=/bladestorm,if=enabled&buff.enrage.up&(buff.bloodbath.up|!talent.bloodbath.enabled)
                        Spell.Cast(SpellBook.Bladestorm, ret => G.BladestormTalent && G.EnrageAura && FG.BloodbathSync && FG.Tier4AbilityAoEUsage),
                        //actions.aoe+=/storm_bolt,if=enabled&debuff.colossus_smash.up
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && G.ColossusSmashAura && FG.Tier6AbilityAoEUsage),
                        //# Note: The 20 second reduction when hitting 3+ targets is not modeled.
                        //actions.aoe+=/shockwave,if=enabled
                        Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && FG.Tier4AbilityAoEUsage),
                        //# Enrage overlaps 4 GCDs, which allows bloodthirst to be used mostly to keep enrage up, as rage income is typically not an issue with the aoe rotation.
                        //actions.aoe+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking&buff.enrage.down
                        Spell.MultiDoT(SpellBook.Bloodthirst, AuraBook.DeepWounds, 1500, 5, 160, ret => !G.EnrageAura),
                        //actions.aoe+=/raging_blow,if=buff.meat_cleaver.stack=3
                        Spell.Cast(SpellBook.RagingBlow, ret => G.MeatCleaverAuraS3),
                        //actions.aoe+=/whirlwind
                        Spell.Cast(SpellBook.Whirlwind),
                        //actions.aoe+=/bloodthirst,cycle_targets=1,if=!dot.deep_wounds.ticking
                        Spell.MultiDoT(SpellBook.Bloodthirst, AuraBook.DeepWounds),
                        //actions.aoe+=/colossus_smash
                        Spell.Cast(SpellBook.ColossusSmash),
                        //actions.aoe+=/battle_shout,if=rage<70
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 70)))
                        )));
        }

        internal static Composite Rel_FuryOffensive()
        {
            return new PrioritySelector(
                //# There is a 0.25~ second delay in enrage application, this delay allows enrage to cover 4 GCDs of ability usage.
                //actions+=/berserker_rage,if=buff.enrage.remains<1&cooldown.bloodthirst.remains>1
                Spell.Cast(SpellBook.BerserkerRage, ret => (!G.EnrageAura && G.BloodThirstOnCooldown || G.FadingEnrage(1000) && G.BloodbathSpellCooldown > 1000) && FG.BerserkerRageUsage, true),
                //actions.single_target=bloodbath,if=enabled&(cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5|target.time_to_die<=20)
                Spell.Cast(SpellBook.Bloodbath, ret => G.BloodbathTalent && (G.ColossusSmashSpellCooldown < 2000 || G.RemainingColossusSmash(5000) || G.AlmostDead) && FG.Tier6AbilityUsage, true),
                //# This incredibly long line can be translated to 'Use recklessness on cooldown with colossus smash; unless the boss will die before the ability is usable again, and then combine with execute instead.'
                //actions+=/recklessness,if=!talent.bloodbath.enabled&((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20))|buff.bloodbath.up&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20)|target.time_to_die<=12
                Spell.Cast(SpellBook.Recklessness, ret => ((!G.BloodbathTalent && (G.RemainingColossusSmash(5000) || G.ColossusSmashSpellCooldown < 2000)) || (!G.BloodbathAura && (G.RemainingColossusSmash(5000) || G.ColossusSmashSpellCooldown < 2000))) && FG.RecklessnessUsage, true),
                //actions+=/avatar,if=enabled&(buff.recklessness.up|target.time_to_die<=25)
                Spell.Cast(SpellBook.Avatar, ret => G.AvatarTalent && (G.RecklessnessAura || G.AlmostDead) && FG.Tier6AbilityUsage, true),
                //actions+=/skull_banner,if=buff.skull_banner.down&(((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&target.time_to_die>192&buff.cooldown_reduction.up)|buff.recklessness.up)
                Spell.Cast(SpellBook.SkullBanner, ret => !Global.SkullBannerAura && (G.RecklessnessAura || G.ColossusSmashSpellCooldown < 2000 || G.RemainingColossusSmash(5000)) && FG.SkullBannerUsage, true)
                );
        }

        internal static Composite Rel_FuryGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.ImpendingVictory, ret => !G.ImpendingVictoryOnCooldown && G.ImpendingVictoryTalent && FG.ImpendingVictoryUsage && Me.HealthPercent <= IS.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SpellBook.VictoryRush, ret => !G.VictoryRushOnCooldown && G.VictoriousAura && FG.VictoryRushUsage && Me.HealthPercent <= IS.Instance.Fury.CheckVicRushNum),
                Spell.Cast(SpellBook.IntimidatingShout, ret => FG.IntimidatingShoutUsage && G.IntimidatingShoutGlyph && !U.IsTargetBoss),
                //actions.single_target+=/shattering_throw,if=cooldown.colossus_smash.remains>5
                Spell.Cast(SpellBook.ShatteringThrow, ret => FG.ShatteringThrowUsage && G.ColossusSmashSpellCooldown > 5)
                );
        }

        internal static Composite Rel_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => FG.RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite Rel_FuryDefensive()
        {
            return new PrioritySelector(
                // HP Regeneration
                new Decorator(ret => G.EnragedRegenerationTalent && IS.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= IS.Instance.Fury.CheckEnragedRegenNum,
                    new PrioritySelector(
                        new Decorator(ret => G.EnrageAura,
                            Spell.Cast(SpellBook.EnragedRegeneration, on => Me)),
                        new Decorator(ret => !G.EnrageAura && !G.BerserkerRageOnCooldown,
                            new Sequence(
                                new Action(ctx => Logger.CombatLogWh("Using Berserker Rage to Enrage - Required for Emergency Enraged Regeneration")),
                                new Action(ctx => Spell.Cast(SpellBook.BerserkerRage, on => Me, ret => true, true)),
                                new Action(ctx => Spell.Cast(SpellBook.EnragedRegeneration, on => Me)))),
                        new Decorator(ret => !G.EnrageAura && G.BerserkerRageOnCooldown,
                            Spell.Cast(SpellBook.EnragedRegeneration, on => Me)))),

                Spell.Cast(SpellBook.DiebytheSword, ret => FG.DiebytheSwordUsage && Me.HealthPercent <= IS.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SpellBook.ShieldWall, ret => FG.ShieldWallUsage && Me.HealthPercent <= IS.Instance.Fury.CheckShieldWallNum),
                Spell.Cast(SpellBook.SpellReflection, ret => FG.SpellReflectUsage && U.IsViable(Me.CurrentTarget) && U.IsTargettingMe && Me.CurrentTarget.IsCasting),
                Item.FuryUseHealthStone()
                );
        }

        internal static Composite Rel_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SpellBook.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && FG.DemoralizingBannerUsage && Me.HealthPercent <= IS.Instance.Fury.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast(SpellBook.Hamstring, ret => !U.IsTargetBoss && !G.HamstringAura && (IS.Instance.Fury.HamString == Enum.Hamstring.Always || IS.Instance.Fury.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast(SpellBook.MassSpellReflection, ret => G.MassSpellReflectionTalent && U.IsViable(Me.CurrentTarget) && Me.CurrentTarget.IsCasting && FG.MassSpellReflectionUsage),
                Spell.Cast(SpellBook.PiercingHowl, ret => G.PiercingHowlTalent && FG.PiercingHowlUsage && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SpellBook.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SpellBook.StaggeringShout, ret => G.StaggeringShoutTalent && FG.StaggeringShoutUsage && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckStaggeringShoutNum),
                new Decorator(ret => U.VigilanceTarget != null,
                    Spell.Cast(SpellBook.Vigilance, on => U.VigilanceTarget))
                );
        }
    }
}