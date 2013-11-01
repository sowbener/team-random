using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using FG = FuryUnleashed.Rotations.Fury.FuryGlobal;
using G = FuryUnleashed.Rotations.Global;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using Lua = FuryUnleashed.Core.Helpers.LuaClass;
using SB = FuryUnleashed.Core.Helpers.SpellBook;
using U = FuryUnleashed.Core.Unit;

namespace FuryUnleashed.Rotations.Fury
{
    class FuryRel
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

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
                                Rel_FuryRacials(),
                                Rel_FuryOffensive(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Rel_FuryRacials(),
                                        Item.CreateItemBehaviour())),
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
                                        Item.CreateItemBehaviour())),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Rel_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_FurySt())
                                        ))))));
            }
        }

        internal static Composite Rel_FurySt()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => Lua.PlayerPower >= 30), // Also in Rel_FuryHeroicStrike().

                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16), // Added T16 P4.
                        Spell.Cast(SB.StormBolt, ret => G.StormBoltTalent && FG.Tier6AbilityUsage), // Added - Inside CS window.

                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.RagingBlow),
                        Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura))),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(

                        Spell.Cast(SB.Execute, ret => G.FadingDeathSentence(3000) && G.ColossusSmashSpellCooldown >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SB.DragonRoar, ret => G.DragonRoarTalent && FG.BloodbathSync && FG.Tier4AbilityUsage), // Added - Outside CS window.
                        Spell.Cast(SB.StormBolt, ret => G.ReadinessAura && G.ColossusSmashSpellCooldown >= 16000 && FG.Tier6AbilityUsage), // Added - When new one is ready in next CS window - With Eye of Galakras.

                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.HeroicStrike, ret => G.ColossusSmashSpellCooldown >= 3000 && ((G.UnendingRageGlyph && Lua.PlayerPower >= Me.MaxRage - 15) || (!G.UnendingRageGlyph && Lua.PlayerPower >= Me.MaxRage - 5))), // Also in Rel_FuryHeroicStrike().
                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow2S && G.ColossusSmashSpellCooldown >= 3000),
                        Spell.Cast(SB.WildStrike, ret => G.BloodsurgeAura),
                        Spell.Cast(SB.RagingBlow, ret => G.RagingBlow1S && G.ColossusSmashSpellCooldown >= 3000),
                        new Switch<Enum.Shouts>(ctx => IS.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout, on => Me, ret => Lua.PlayerPower < 70)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout, on => Me, ret => Lua.PlayerPower < 70))),
                        Spell.Cast(SB.Bladestorm, ret => G.BladestormTalent && G.ColossusSmashSpellCooldown >= 6000 && FG.Tier4AbilityUsage),
                        Spell.Cast(SB.Shockwave, ret => G.ShockwaveTalent && Me.IsFacing(Me.CurrentTarget) && FG.Tier4AbilityUsage),
                        Spell.Cast(SB.WildStrike, ret => !G.BloodsurgeAura && G.ColossusSmashSpellCooldown >= 3000 && Lua.PlayerPower >= 90),
                        Spell.Cast(SB.HeroicStrike, ret => Lua.PlayerPower == Me.MaxRage), // Also in Rel_FuryHeroicStrike().
                        Spell.Cast(SB.ImpendingVictory, ret => G.ImpendingVictoryTalent && !G.ImpendingVictoryOnCooldown && IS.Instance.Fury.CheckRotImpVic), // Added for the sake of supporting it rotational.
                        Spell.Cast(SB.HeroicThrow, ret => IS.Instance.Fury.CheckHeroicThrow) // Added for the sake of supporting it rotational.
                        )));
        }

        internal static Composite Rel_FuryExec()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.StormBolt, ret => G.StormBoltTalent && FuryGlobal.Tier6AbilityUsage),
                        Spell.Cast(SB.Execute),
                        Spell.Cast(SB.HeroicStrike, ret => Lua.PlayerPower == Me.MaxRage))),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.FadingDeathSentence(3000) && G.ColossusSmashSpellCooldown >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.DragonRoar, ret => G.DragonRoarTalent && FG.BloodbathSync && FG.Tier4AbilityUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.StormBoltTalent && FG.Tier6AbilityUsage), // Added
                        Spell.Cast(SB.RagingBlow),
                        Spell.Cast(SB.Execute, ret => Lua.PlayerPower == Me.MaxRage - 10),
                        Spell.Cast(SB.HeroicStrike, ret => Lua.PlayerPower == Me.MaxRage),
                        Spell.Cast(SB.Shockwave, ret => G.ShockwaveTalent && Me.IsFacing(Me.CurrentTarget) && FG.Tier4AbilityUsage), // Added
                        Spell.Cast(SB.Bladestorm, ret => G.BladestormTalent && G.BloodthirstSpellCooldown >= 2000 && G.ColossusSmashSpellCooldown >= 6000 && FG.Tier4AbilityUsage) // Added
                        )));
        }

        internal static Composite Rel_FuryHeroicStrike()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura && G.NormalPhase,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => Lua.PlayerPower >= 30))),
                new Decorator(ret => !G.ColossusSmashAura && G.NormalPhase,
                    new PrioritySelector(
                        Spell.Cast(SB.HeroicStrike, ret => G.ColossusSmashSpellCooldown >= 3000 && ((G.UnendingRageGlyph && Lua.PlayerPower >= Me.MaxRage - 15) || (!G.UnendingRageGlyph && Lua.PlayerPower >= Me.MaxRage - 5))))),
                Spell.Cast(SB.HeroicStrike, ret => Lua.PlayerPower == Me.MaxRage));
        }

        internal static Composite Rel_FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => U.NearbyAttackableUnitsCount >= 5,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16 || G.ExecutePhase && G.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SB.Bladestorm, ret => G.BladestormTalent && FG.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DragonRoarTalent && FG.BloodbathSync && FG.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.StormBoltTalent && FG.Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.ShockwaveTalent && Me.IsFacing(Me.CurrentTarget) && FG.Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.Whirlwind),
                        Spell.Cast(SB.RagingBlow, ret => Lua.PlayerPower <= 60 && G.MeatCleaverAuraS3)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 4,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16 || G.ExecutePhase && G.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SB.Bladestorm, ret => G.BladestormTalent && FG.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DragonRoarTalent && FG.BloodbathSync && FG.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.StormBoltTalent && FG.Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.ShockwaveTalent && Me.IsFacing(Me.CurrentTarget) && FG.Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.Whirlwind, ret => !G.MeatCleaverAuraS3),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.RagingBlow, ret => G.MeatCleaverAuraS3),
                        Spell.Cast(SB.Cleave, ret => Lua.PlayerPower > Me.MaxRage - 10)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 3,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16 || G.ExecutePhase && G.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SB.Bladestorm, ret => G.BladestormTalent && FG.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DragonRoarTalent && FG.BloodbathSync && FG.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.StormBoltTalent && FG.Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.ShockwaveTalent && Me.IsFacing(Me.CurrentTarget) && FG.Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.Whirlwind, ret => !G.MeatCleaverAuraS2),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.RagingBlow, ret => G.MeatCleaverAuraS2),
                        Spell.Cast(SB.Cleave, ret => Lua.PlayerPower > Me.MaxRage - 10)
                        )),
                new Decorator(ret => U.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.DeathSentenceAuraT16 || G.ExecutePhase && G.ColossusSmashAura), // Added - Only in CS window or Death Sentence.

                        Spell.Cast(SB.Bladestorm, ret => G.BladestormTalent && FG.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.DragonRoar, ret => G.DragonRoarTalent && FG.BloodbathSync && FG.Tier4AbilityAoEUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.StormBoltTalent && FG.Tier6AbilityUsage), // Added
                        Spell.Cast(SB.Shockwave, ret => G.ShockwaveTalent && Me.IsFacing(Me.CurrentTarget) && FG.Tier4AbilityAoEUsage), // Added

                        Spell.Cast(SB.Whirlwind, ret => !G.MeatCleaverAuraS1),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.RagingBlow, ret => G.MeatCleaverAuraS1),
                        Spell.Cast(SB.Cleave, ret => Lua.PlayerPower > Me.MaxRage - 10),
                        new PrioritySelector(
                            new Decorator(ret => G.ExecutePhase, Rel_FuryExec()),
                            new Decorator(ret => G.NormalPhase, Rel_FurySt()))
                        )));
        }

        internal static Composite Rel_FuryOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SB.BerserkerRage, ret => (!G.EnrageAura || G.FadingEnrage(500)) && G.BloodThirstOnCooldown && G.ColossusSmashAura && FG.BerserkerRageUsage),
                Spell.Cast(SB.Bloodbath, ret => G.BloodbathTalent && FG.Tier6AbilityUsage),
                Spell.Cast(SB.Recklessness, ret => FG.RecklessnessUsage),
                Spell.Cast(SB.Avatar, ret => G.AvatarTalent && FG.RecklessnessSync && FG.Tier6AbilityUsage),
                Spell.Cast(SB.SkullBanner, ret => !G.SkullBannerAura && FG.RecklessnessSync && FG.SkullBannerUsage)
                );
        }

        internal static Composite Rel_FuryGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ImpendingVictory, ret => !G.ImpendingVictoryOnCooldown && G.ImpendingVictoryTalent && IS.Instance.Fury.CheckImpVic && Me.HealthPercent <= IS.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SB.VictoryRush, ret => !G.VictoryRushOnCooldown && G.VictoriousAura && IS.Instance.Fury.CheckVicRush && Me.HealthPercent <= IS.Instance.Fury.CheckVicRushNum),
                Spell.Cast(SB.IntimidatingShout, ret => IS.Instance.Fury.CheckIntimidatingShout && G.IntimidatingShoutGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.ShatteringThrow, ret => IS.Instance.Fury.CheckShatteringThrow && U.IsTargetBoss && (G.ColossusSmashSpellCooldown <= 3000 || G.SkullBannerSpellCooldown <= 3000))
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
                Spell.Cast(SB.DiebytheSword, ret => IS.Instance.Fury.CheckDiebytheSword && Me.HealthPercent <= IS.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SB.EnragedRegeneration, ret => G.EnragedRegenerationTalent && IS.Instance.Fury.CheckEnragedRegen && Me.HealthPercent <= IS.Instance.Fury.CheckEnragedRegenNum),
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
                Spell.Cast(SB.MassSpellReflection, ret => G.MassSpellReflectionTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && FG.MassSpellReflectionUsage),
                Spell.Cast(SB.PiercingHowl, ret => G.PiercingHowlTalent && IS.Instance.Fury.CheckStaggeringShout && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SB.StaggeringShout, ret => G.StaggeringShoutTalent && IS.Instance.Fury.CheckPiercingHowl && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckPiercingHowlNum)
                );
        }
    }
}
