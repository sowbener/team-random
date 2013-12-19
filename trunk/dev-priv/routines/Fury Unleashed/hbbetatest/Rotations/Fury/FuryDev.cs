using System.Windows.Forms;
using CommonBehaviors.Actions;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
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
                //Added for Supporting it.
                Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16 && Global.ColossusSmashAura || Global.FadingDeathSentence(3000) && Global.ColossusSmashSpellCooldown >= 1500), // Added T16 P4.
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.up&debuff.colossus_smash.up
                Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && Global.ReadinessAura && Global.ColossusSmashAura && FuryGlobal.Tier6AbilityUsage),
                //actions.single_target+=/raging_blow,if=buff.raging_blow.stack=2&debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast(SpellBook.RagingBlow, ret => Global.RagingBlow2S && Global.ColossusSmashAura && Global.NormalPhase),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down&debuff.colossus_smash.up
                Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && !Global.ReadinessAura && Global.ColossusSmashAura && FuryGlobal.Tier6AbilityUsage),
                //actions.single_target+=/bloodthirst,if=!(target.health.pct<20&debuff.colossus_smash.up&rage>=30&buff.enrage.up)
                Spell.Cast(SpellBook.Bloodthirst),
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.react&target.health.pct>=20&cooldown.bloodthirst.remains<=1
                Spell.Cast(SpellBook.WildStrike, ret => Global.BloodsurgeAura && Global.NormalPhase && Global.BloodthirstSpellCooldown <= 1000),
                //actions.single_target+=/dragon_roar,if=enabled&(!debuff.colossus_smash.up&(buff.bloodbath.up|!talent.bloodbath.enabled))
                Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && (!Global.ColossusSmashAura && FuryGlobal.BloodbathSync) && FuryGlobal.Tier4AbilityUsage),
                //actions.single_target+=/colossus_smash
                Spell.Cast(SpellBook.ColossusSmash),
                //actions.single_target+=/storm_bolt,if=enabled&buff.cooldown_reduction.down
                Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && !Global.ReadinessAura && FuryGlobal.Tier6AbilityUsage),
                //actions.single_target+=/execute,if=debuff.colossus_smash.up|rage>70|target.time_to_die<12
                Spell.Cast(SpellBook.Execute, ret => Global.ColossusSmashAura || Lua.PlayerPower > 70),
                //actions.single_target+=/raging_blow,if=target.health.pct<20|buff.raging_blow.stack=2|(debuff.colossus_smash.up|(cooldown.bloodthirst.remains>=1&buff.raging_blow.remains<=3))
                Spell.Cast(SpellBook.RagingBlow, ret => Global.ExecutePhase || Global.RagingBlow2S || (Global.ColossusSmashAura || Global.BloodthirstSpellCooldown >= 1 && Global.FadingRb(3000))),
                //actions.single_target+=/wild_strike,if=buff.bloodsurge.up
                Spell.Cast(SpellBook.WildStrike, ret => Global.BloodsurgeAura),
                //actions.single_target+=/bladestorm,if=enabled&cooldown.bloodthirst.remains>2
                Spell.Cast(SpellBook.Bladestorm, ret => Global.BladestormTalent && Global.BloodthirstSpellCooldown > 2000 && FuryGlobal.Tier4AbilityUsage),
                //actions.single_target+=/raging_blow,if=cooldown.colossus_smash.remains>=3
                Spell.Cast(SpellBook.RagingBlow, ret => Global.ColossusSmashSpellCooldown >= 3000),
                //actions.single_target+=/shockwave,if=enabled
                Spell.Cast(SpellBook.Shockwave, ret => Global.ShockwaveTalent && Global.ShockwaveFacing && FuryGlobal.Tier4AbilityUsage),
                //actions.single_target+=/heroic_throw,if=debuff.colossus_smash.down&rage<60
                Spell.Cast(SpellBook.HeroicThrow, ret => !Global.ColossusSmashAura && Lua.PlayerPower < 60 && FuryGlobal.HeroicThrowUsage),
                //actions.single_target+=/battle_shout,if=rage<70&!debuff.colossus_smash.up
                new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 70 && !Global.ColossusSmashAura)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 70 && !Global.ColossusSmashAura))),
                //actions.single_target+=/wild_strike,if=debuff.colossus_smash.up&target.health.pct>=20
                Spell.Cast(SpellBook.WildStrike, ret => Global.ColossusSmashAura && Global.NormalPhase),
                //actions.single_target+=/battle_shout,if=rage<70
                new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 70)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 70))),
                //actions.single_target+=/wild_strike,if=cooldown.colossus_smash.remains>=2&rage>=70&target.health.pct>=20
                Spell.Cast(SpellBook.WildStrike, ret => Global.ColossusSmashSpellCooldown >= 2000 && Lua.PlayerPower >= 70 && Global.NormalPhase),
                //actions.single_target+=/impending_victory,if=enabled&target.health.pct>=20&cooldown.colossus_smash.remains>=2
                Spell.Cast(SpellBook.ImpendingVictory, ret => Global.ImpendingVictoryTalent && !Global.ImpendingVictoryOnCooldown && Global.NormalPhase && Global.ColossusSmashSpellCooldown >= 2000 && FuryGlobal.ImpendingVictoryUsage)
                );
        }

        internal static Composite Dev_FuryHeroicStrike()
        {
            return new PrioritySelector(
                new Decorator(ret => !InternalSettings.Instance.Fury.CheckAoE || Unit.NearbyAttackableUnitsCount < InternalSettings.Instance.Fury.CheckAoENum,
                    new PrioritySelector(
                //actions.single_target+=/heroic_strike,if=(debuff.colossus_smash.up&rage>=40&target.health.pct>=20|rage>=100)&buff.enrage.up - Added Lua.PlayerPower == Lua.PlayerPowerMax
                        Spell.Cast(SpellBook.HeroicStrike, ret => Lua.PlayerPower == Lua.PlayerPowerMax || ((Global.ColossusSmashAura && Lua.PlayerPower >= 40 && Global.NormalPhase || Lua.PlayerPower >= Lua.PlayerPowerMax - 20) && Global.EnrageAura), true))),
                new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum,
                    new PrioritySelector(
                //actions.three_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90 //actions.two_targets+=/cleave,if=(rage>=60&debuff.colossus_smash.up)|rage>90
                        Spell.Cast(SpellBook.Cleave, ret => (Lua.PlayerPower >= 60 && Global.ColossusSmashAura || Lua.PlayerPower > 90) && Unit.NearbyAttackableUnitsCount < 4, true),
                //actions.aoe+=/cleave,if=rage>110 - Added Lua.PlayerPower == Lua.PlayerPowerMax
                        Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower == Lua.PlayerPowerMax || Lua.PlayerPower > Lua.PlayerPowerMax - 10 && Unit.NearbyAttackableUnitsCount >= 4, true))));
        }

        // Using this for Execute Phase - http://www.icy-veins.com/fury-warrior-wow-pve-dps-rotation-cooldowns-abilities
        internal static Composite Dev_FuryExec()
        {
            return new PrioritySelector(
                new Decorator(ret => Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && FuryGlobal.Tier6AbilityUsage),
                        Spell.Cast(SpellBook.Bloodthirst, ret => !Global.EnrageAura),
                        Spell.Cast(SpellBook.Execute),
                        Spell.Cast(SpellBook.RagingBlow))),
                new Decorator(ret => !Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && !Global.ReadinessAura && FuryGlobal.Tier6AbilityUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && FuryGlobal.Tier4AbilityUsage),
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.Execute, ret => Lua.PlayerPower >= 100),
                        Spell.Cast(SpellBook.Bloodthirst, ret => Lua.PlayerPower < 100),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.RagingBlow2S),
                        Spell.Cast(SpellBook.Execute, ret => Global.ColossusSmashOnCooldown && Global.BloodThirstOnCooldown && Global.RagingBlow1S && Lua.PlayerPower > 80 && Global.ColossusSmashSpellCooldown >= 1500),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.RagingBlow1S && Lua.PlayerPower < 60),
                        Spell.Cast(SpellBook.WildStrike, ret => Global.BloodsurgeAura),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 50)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 50))),
                        Spell.Cast(SpellBook.HeroicThrow, ret => FuryGlobal.HeroicThrowUsage)
                        )));
        }

        // Need to rework AoE
        internal static Composite Dev_FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => Unit.NearbyAttackableUnitsCount >= 5,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16 || Global.ExecutePhase && Global.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BladestormTalent && FuryGlobal.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && FuryGlobal.BloodbathSync && FuryGlobal.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && FuryGlobal.Tier6AbilityUsage), // Added
                        Spell.Cast(SpellBook.Shockwave, ret => Global.ShockwaveTalent && Global.ShockwaveFacing && FuryGlobal.Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.Whirlwind),
                        Spell.Cast(SpellBook.RagingBlow, ret => Lua.PlayerPower <= 60 && Global.MeatCleaverAuraS3)
                        )),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 4,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16 || Global.ExecutePhase && Global.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BladestormTalent && FuryGlobal.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && FuryGlobal.BloodbathSync && FuryGlobal.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && FuryGlobal.Tier6AbilityUsage), // Added
                        Spell.Cast(SpellBook.Shockwave, ret => Global.ShockwaveTalent && Global.ShockwaveFacing && FuryGlobal.Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SpellBook.Whirlwind, ret => !Global.MeatCleaverAuraS3),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.MeatCleaverAuraS3),
                        Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower > Me.MaxRage - 10)
                        )),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16 || Global.ExecutePhase && Global.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BladestormTalent && FuryGlobal.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && FuryGlobal.BloodbathSync && FuryGlobal.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && FuryGlobal.Tier6AbilityUsage), // Added
                        Spell.Cast(SpellBook.Shockwave, ret => Global.ShockwaveTalent && Global.ShockwaveFacing && FuryGlobal.Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SpellBook.Whirlwind, ret => !Global.MeatCleaverAuraS2),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.MeatCleaverAuraS2),
                        Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower > Me.MaxRage - 10)
                        )),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16 || Global.ExecutePhase && Global.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BladestormTalent && FuryGlobal.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && FuryGlobal.BloodbathSync && FuryGlobal.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && FuryGlobal.Tier6AbilityUsage), // Added
                        Spell.Cast(SpellBook.Shockwave, ret => Global.ShockwaveTalent && Global.ShockwaveFacing && FuryGlobal.Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SpellBook.Whirlwind, ret => !Global.MeatCleaverAuraS1),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.MeatCleaverAuraS1),
                        Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower > Me.MaxRage - 10),
                        new PrioritySelector(
                            new Decorator(ret => Global.ExecutePhase, Dev_FuryExec()),
                            new Decorator(ret => Global.NormalPhase, Dev_FurySt()))
                        )));
        }

        internal static Composite Dev_FuryOffensive()
        {
            return new PrioritySelector(
                //# This incredibly long line can be translated to 'Use recklessness on cooldown with colossus smash; unless the boss will die before the ability is usable again, and then combine with execute instead.'
                //actions+=/recklessness,if=!talent.bloodbath.enabled&((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20))|buff.bloodbath.up&(target.time_to_die>(192*buff.cooldown_reduction.value)|target.health.pct<20)|target.time_to_die<=12
                Spell.Cast(SpellBook.Recklessness, ret => (Global.RemainingCs(5000) || Global.ColossusSmashSpellCooldown < 2000) && FuryGlobal.RecklessnessUsage, true),
                //actions+=/avatar,if=enabled&(buff.recklessness.up|target.time_to_die<=25)
                Spell.Cast(SpellBook.Avatar, ret => Global.AvatarTalent && Global.RecklessnessAura && FuryGlobal.Tier6AbilityUsage, true),
                //actions+=/skull_banner,if=buff.skull_banner.down&(((cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5)&target.time_to_die>192&buff.cooldown_reduction.up)|buff.recklessness.up)
                Spell.Cast(SpellBook.SkullBanner, ret => !Global.SkullBannerAura && Global.RecklessnessAura && FuryGlobal.SkullBannerUsage, true),
                //# There is a 0.25~ second delay in enrage application, this delay allows enrage to cover 4 GCDs of ability usage.
                //actions+=/berserker_rage,if=buff.enrage.remains<1&cooldown.bloodthirst.remains>1
                Spell.Cast(SpellBook.BerserkerRage, ret => (Global.FadingEnrage(1000) || Global.BloodthirstSpellCooldown > 1000) && FuryGlobal.BerserkerRageUsage, true),
                //actions.single_target=bloodbath,if=enabled&(cooldown.colossus_smash.remains<2|debuff.colossus_smash.remains>=5|target.time_to_die<=20)
                Spell.Cast(SpellBook.Bloodbath, ret => Global.BloodbathTalent && (Global.RemainingCs(5000) || Global.ColossusSmashSpellCooldown < 2000) && FuryGlobal.Tier6AbilityUsage, true)
                );
        }

        internal static Composite Dev_FuryGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.ImpendingVictory, ret => !Global.ImpendingVictoryOnCooldown && Global.ImpendingVictoryTalent && InternalSettings.Instance.Fury.CheckImpVic && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SpellBook.VictoryRush, ret => !Global.VictoryRushOnCooldown && Global.VictoriousAura && InternalSettings.Instance.Fury.CheckVicRush && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckVicRushNum),
                Spell.Cast(SpellBook.IntimidatingShout, ret => InternalSettings.Instance.Fury.CheckIntimidatingShout && Global.IntimidatingShoutGlyph && !Unit.IsTargetBoss),
                Spell.Cast(SpellBook.ShatteringThrow, ret => InternalSettings.Instance.Fury.CheckShatteringThrow && Unit.IsTargetBoss && (Global.ColossusSmashSpellCooldown <= 3000 || Global.SkullBannerSpellCooldown <= 3000))
                );
        }

        internal static Composite Dev_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => FuryGlobal.RacialUsage,
                    Spell.Cast(Global.SelectRacialSpell(), ret => Global.SelectRacialSpell() != null && Global.RacialUsageSatisfied(Global.SelectRacialSpell()))
                    ));
        }

        internal static Composite Dev_FuryDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.DiebytheSword, ret => InternalSettings.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SpellBook.EnragedRegeneration, ret => Global.EnragedRegenerationTalent && InternalSettings.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast(SpellBook.ShieldWall, ret => InternalSettings.Instance.Fury.CheckShieldWall && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckShieldWallNum),
                Spell.Cast(SpellBook.SpellReflection, ret => InternalSettings.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && Global.TargettingMe && Me.CurrentTarget.IsCasting),
                Item.FuryUseHealthStone()
                );
        }

        internal static Composite Dev_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SpellBook.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && InternalSettings.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckDemoBannerNum && Unit.IsDoNotUseOnTgt),
                Spell.Cast(SpellBook.Hamstring, ret => !Unit.IsTargetBoss && !Global.HamstringAura && (InternalSettings.Instance.Fury.HamString == Enum.Hamstring.Always || InternalSettings.Instance.Fury.HamString == Enum.Hamstring.AddList && Unit.IsHamstringTarget)),
                Spell.Cast(SpellBook.MassSpellReflection, ret => Global.MassSpellReflectionTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && FuryGlobal.MassSpellReflectionUsage),
                Spell.Cast(SpellBook.PiercingHowl, ret => Global.PiercingHowlTalent && InternalSettings.Instance.Fury.CheckStaggeringShout && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SpellBook.RallyingCry, ret => Unit.RaidMembersNeedCryCount > 0),
                Spell.Cast(SpellBook.StaggeringShout, ret => Global.StaggeringShoutTalent && InternalSettings.Instance.Fury.CheckPiercingHowl && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckPiercingHowlNum),
                new Decorator(ret => Unit.VigilanceTarget != null,
                    Spell.Cast(SpellBook.Vigilance, on => Unit.VigilanceTarget))
                );
        }
    }
}
