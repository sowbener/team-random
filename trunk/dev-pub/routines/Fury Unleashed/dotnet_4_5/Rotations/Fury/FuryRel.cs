using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using Lua = FuryUnleashed.Core.Helpers.LuaClass;

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
                                Rel_FuryNonGcdUtility(),
                                Rel_FuryRacials(),
                                Rel_FuryOffensive(),
                                Item.CreateItemBehaviour(),
                                Rel_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Rel_FurySt())
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
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Rel_FurySt())
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
                                        new Decorator(ret => InternalSettings.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => Global.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => Global.NormalPhase, Rel_FurySt())
                                        ))))));
            }
        }

        internal static Composite Rel_FurySt()
        {
            return new PrioritySelector(
                new Decorator(ret => Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16), // Added T16 P4.
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && FuryGlobal.Tier6AbilityUsage),
                        Spell.Cast(SpellBook.RagingBlow),
                        Spell.Cast(SpellBook.Bloodthirst, ret => !Global.RagingBlow2S || !(Global.RagingBlow1S && (Global.FadingCs(4500) || Global.ColossusSmashSpellCooldown > 18000))),
                        Spell.Cast(SpellBook.WildStrike, ret => Global.BloodsurgeAura),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && FuryGlobal.Tier4AbilityUsage),
                        Spell.Cast(SpellBook.Shockwave, ret => Global.ShockwaveTalent && Global.ShockwaveFacing && FuryGlobal.Tier4AbilityUsage),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 60)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 60)))
                        )),
                new Decorator(ret => !Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ColossusSmash, ret => Global.RagingBlow2S || Global.RagingBlow1S || Global.BloodthirstSpellCooldown > 3000),
                        Spell.Cast(SpellBook.Execute, ret => Global.FadingDeathSentence(3000) && Global.ColossusSmashSpellCooldown >= 1500),
                        Spell.Cast(SpellBook.RagingBlow, ret => ((Global.ColossusSmashSpellCooldown + 1000) > Spell.AuraTimeLeft(StyxWoW.Me, AuraBook.RagingBlow) || (Global.RagingBlow2S && Global.BloodthirstSpellCooldown < 2500)) && Global.ColossusSmashSpellCooldown >= 3000),
                        new Decorator(ret => (Global.DeterminationAura || Global.OutrageAura) && Global.ColossusSmashSpellCooldown >= 10000 && FuryGlobal.Tier6AbilityUsage,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && FuryGlobal.Tier6AbilityUsage),
                                Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && FuryGlobal.Tier4AbilityUsage),
                                Spell.Cast(SpellBook.Shockwave, ret => Global.ShockwaveTalent && Global.ShockwaveFacing && FuryGlobal.Tier4AbilityUsage)
                                )),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.WildStrike, ret => Global.BloodsurgeAura),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.RagingBlow1S && Global.BloodthirstSpellCooldown < 2500 && Global.ColossusSmashSpellCooldown >= 3500),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me, ret => Lua.PlayerPower < 60 && Global.FadingCs(1500))),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Lua.PlayerPower < 60 && Global.FadingCs(1500)))),
                        Spell.Cast(SpellBook.WildStrike, ret => FuryGlobal.UseWildStrike(!Global.ColossusSmashAura)),
                        Spell.Cast(SpellBook.ImpendingVictory, ret => Global.ImpendingVictoryTalent && !Global.ImpendingVictoryOnCooldown && InternalSettings.Instance.Fury.CheckRotImpVic),
                        Spell.Cast(SpellBook.HeroicThrow, ret => InternalSettings.Instance.Fury.CheckHeroicThrow)
                        ))
                );
        }

        internal static Composite Rel_FuryExec()
        {
            return new PrioritySelector(
                new Decorator(ret => Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && FuryGlobal.Tier6AbilityUsage),
                        Spell.Cast(SpellBook.Execute),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Lua.PlayerPower == Me.MaxRage))),
               new Decorator(ret => !Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.FadingDeathSentence(3000) && Global.ColossusSmashSpellCooldown >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && BloodbathSync && FuryGlobal.Tier4AbilityUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && FuryGlobal.Tier6AbilityUsage),
                        Spell.Cast(SpellBook.RagingBlow),
                        Spell.Cast(SpellBook.Execute, ret => Lua.PlayerPower == Me.MaxRage - 10),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Lua.PlayerPower == Me.MaxRage),
                        Spell.Cast(SpellBook.Shockwave, ret => Global.ShockwaveTalent && Global.ShockwaveFacing && FuryGlobal.Tier4AbilityUsage),
                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BladestormTalent && Global.BloodthirstSpellCooldown >= 2000 && Global.ColossusSmashSpellCooldown >= 6000 && FuryGlobal.Tier4AbilityUsage) // Added
                        )));
        }

        internal static Composite Rel_FuryHeroicStrike()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.HeroicStrike, ret => Lua.PlayerPower == Me.MaxRage, true),
                Spell.Cast(SpellBook.HeroicStrike, ret => FuryGlobal.UseHeroicStrike(Global.ColossusSmashAura) && Global.NormalPhase, true)
                );
        }

        internal static Composite Rel_FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => Unit.NearbyAttackableUnitsCount >= 2,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16 || Global.ExecutePhase && Global.ColossusSmashAura),
                        Spell.Cast(SpellBook.Cleave, ret => Lua.PlayerPower > Me.MaxRage - 40, true),
                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BladestormTalent && FuryGlobal.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && BloodbathSync && FuryGlobal.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Shockwave, ret => Global.ShockwaveTalent && Global.ShockwaveFacing && FuryGlobal.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.Whirlwind),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.MeatCleaverAuraS3),
                        new PrioritySelector(
                            new Decorator(ret => Global.ExecutePhase, Rel_FuryExec()),
                            new Decorator(ret => Global.NormalPhase, Rel_FurySt()))
                        )));
        }

        internal static Composite Rel_FuryOffensive()
        {
            return new PrioritySelector(
                // Spells
                Spell.Cast(SpellBook.BerserkerRage, ret => FuryGlobal.BerserkerRageUsage && (Global.ColossusSmashAura && (!Global.RagingBlow2S || (!Global.FadingCs(2500) && Global.RagingBlow1S) || (!Global.RagingBlowAura)) || !Global.ColossusSmashOnCooldown && !Global.RagingBlow2S && !Global.RagingBlow1S), true),
                Spell.Cast(SpellBook.Recklessness, ret => FuryGlobal.RecklessnessUsage && Global.ColossusSmashTracker && (!Global.FadingOffensiveCooldowns() || Global.RunningOffensiveCoolDowns), true),
                Spell.Cast(SpellBook.SkullBanner, ret => !Global.SkullBannerAura && FuryGlobal.SkullBannerUsage && Global.ColossusSmashTracker && (!Global.FadingOffensiveCooldowns() || RecklessnessSync), true),

                // Talents
                Spell.Cast(SpellBook.Bloodbath, ret => Global.BloodbathTalent && FuryGlobal.Tier6AbilityUsage && (Global.ColossusSmashTracker || RecklessnessSync), true),
                Spell.Cast(SpellBook.Avatar, ret => Global.AvatarTalent && FuryGlobal.Tier6AbilityUsage && Global.ColossusSmashTracker && (!Global.FadingOffensiveCooldowns() || RecklessnessSync), true)
                );
        }

        internal static Composite Rel_FuryGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.ImpendingVictory, ret => !Global.ImpendingVictoryOnCooldown && Global.ImpendingVictoryTalent && InternalSettings.Instance.Fury.CheckImpVic && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SpellBook.VictoryRush, ret => !Global.VictoryRushOnCooldown && Global.VictoriousAura && InternalSettings.Instance.Fury.CheckVicRush && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckVicRushNum),
                Spell.Cast(SpellBook.IntimidatingShout, ret => InternalSettings.Instance.Fury.CheckIntimidatingShout && Global.IntimidatingShoutGlyph && !Unit.IsTargetBoss),
                Spell.Cast(SpellBook.ShatteringThrow, ret => InternalSettings.Instance.Fury.CheckShatteringThrow && Unit.IsTargetBoss && (Global.ColossusSmashSpellCooldown <= 3000 || Global.SkullBannerSpellCooldown <= 3000))
                );
        }

        internal static Composite Rel_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => FuryGlobal.RacialUsage && Global.ColossusSmashAura,
                    Spell.Cast(Global.SelectRacialSpell(), ret => Global.SelectRacialSpell() != null && Global.RacialUsageSatisfied(Global.SelectRacialSpell()))
                    ));
        }

        internal static Composite Rel_FuryDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.DiebytheSword, ret => InternalSettings.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SpellBook.EnragedRegeneration, ret => Global.EnragedRegenerationTalent && InternalSettings.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast(SpellBook.ShieldWall, ret => InternalSettings.Instance.Fury.CheckShieldWall && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckShieldWallNum),
                Spell.Cast(SpellBook.SpellReflection, ret => InternalSettings.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null && Global.TargettingMe && Me.CurrentTarget.IsCasting),
                Item.FuryUseHealthStone()
                );
        }

        internal static Composite Rel_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SpellBook.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && InternalSettings.Instance.Fury.CheckDemoBanner && Me.HealthPercent <= InternalSettings.Instance.Fury.CheckDemoBannerNum && Unit.IsDoNotUseOnTgt),
                Spell.Cast(SpellBook.Hamstring, ret => !Unit.IsTargetBoss && !Global.HamstringAura && FuryGlobal.HamstringUsage),
                Spell.Cast(SpellBook.MassSpellReflection, ret => Global.MassSpellReflectionTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && FuryGlobal.MassSpellReflectionUsage),
                Spell.Cast(SpellBook.PiercingHowl, ret => Global.PiercingHowlTalent && InternalSettings.Instance.Fury.CheckStaggeringShout && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SpellBook.RallyingCry, ret => Unit.RaidMembersNeedCryCount > 0),
                Spell.Cast(SpellBook.StaggeringShout, ret => Global.StaggeringShoutTalent && InternalSettings.Instance.Fury.CheckPiercingHowl && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Fury.CheckPiercingHowlNum),
                new Decorator(ret => Unit.VigilanceTarget != null,
                    Spell.Cast(SpellBook.Vigilance, on => Unit.VigilanceTarget))
                );
        }

        #region Booleans
        internal static bool RecklessnessSync
        {
            get
            {
                return (Global.RecklessnessAura);
            }
        }

        internal static bool BloodbathSync
        {
            get
            {
                return (Global.BloodbathAura || !Global.BloodbathTalent);
            }
        }
        #endregion
    }
}
