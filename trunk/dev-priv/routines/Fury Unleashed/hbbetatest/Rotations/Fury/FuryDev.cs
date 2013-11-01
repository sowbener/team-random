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
                                        new Decorator(
                                            ret =>
                                                IS.Instance.Fury.CheckAoE &&
                                                U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum,
                                            Dev_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                Dev_FuryNonGcdUtility(),
                                Dev_FuryRacials(),
                                Dev_FuryOffensive(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_FuryRacials(),
                                        Item.CreateItemBehaviour())),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(
                                            ret =>
                                                IS.Instance.Fury.CheckAoE &&
                                                U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum,
                                            Dev_FuryMt()),
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
                                        Item.CreateItemBehaviour())),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(
                                            ret =>
                                                IS.Instance.Fury.CheckAoE && HotKeyManager.IsAoe &&
                                                U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum,
                                            Dev_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_FurySt())
                                        ))))));
            }
        }

        internal static Composite Dev_FurySt()
        {
            return new PrioritySelector(
                new Decorator(ret => Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => Global.DeathSentenceAuraT16), // Added T16 P4.
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.RagingBlow2S || Global.RagingBlow1S && (Global.FadingCs(2500) || Global.CsCd > 18000)),
                        Spell.Cast(SpellBook.Shockwave,
                            ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => Global.SbTalent && Tier6AbilityUsage),
                        Spell.Cast(SpellBook.Bloodthirst,
                            ret =>
                                !Global.RagingBlow2S ||
                                !(Global.RagingBlow1S && (Global.FadingCs(4500) || Global.CsCd > 18000))),
                        Spell.Cast(SpellBook.RagingBlow),
                        Spell.Cast(SpellBook.WildStrike, ret => Global.BloodsurgeAura),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout,
                                Spell.Cast(SpellBook.BattleShout, on => Me, ret => Me.CurrentRage < 60)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout,
                                Spell.Cast(SpellBook.CommandingShout, on => Me, ret => Me.CurrentRage < 60)))
                        //Spell.Cast(SpellBook.WildStrike, ret => UseWildStrike(Global.ColossusSmashAura))
                        )
                    ),
                new Decorator(ret => !Global.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ColossusSmash,
                            ret =>
                                Global.RagingBlow2S || Global.RagingBlow1S || Global.BtCd > 3000),
                        Spell.Cast(SpellBook.Execute, ret => Global.FadingDeathSentence(3000) && Global.CsCd >= 1500),
                        Spell.Cast(SpellBook.RagingBlow,
                            ret =>
                                ((Global.CsCd + 1000) > Spell.AuraTimeLeft(StyxWoW.Me, AuraBook.RagingBlow)) ||
                                Global.RagingBlow2S && Global.BtCd < 2500 && Global.CsCd >= 3000),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityUsage),
                        Spell.Cast(SpellBook.StormBolt,
                            ret =>
                                (Global.DeterminationAura &&
                                 Spell.AuraTimeLeft(StyxWoW.Me, AuraBook.Determination) > Global.CsCd + 2000 ||
                                 Global.OutrageAura &&
                                 Spell.AuraTimeLeft(StyxWoW.Me, AuraBook.Outrage) > Global.CsCd + 2000 ||
                                 Global.CsCd >= 15000) && Tier6AbilityUsage),
                        Spell.Cast(SpellBook.Shockwave,
                            ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityUsage),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.WildStrike, ret => Global.BloodsurgeAura),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.RagingBlow1S && Global.BtCd < 2500 && Global.CsCd >= 5000),
                        //Spell.Cast(SpellBook.Whirlwind, ret => Global.RagingWindAura),
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Fury.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout,
                                Spell.Cast(SpellBook.BattleShout, on => Me,
                                    ret => Me.CurrentRage < 60 && Global.FadingCs(1500))),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout,
                                Spell.Cast(SpellBook.CommandingShout, on => Me,
                                    ret => Me.CurrentRage < 60 && Global.FadingCs(1500)))),
                        Spell.Cast(SpellBook.WildStrike, ret => UseWildStrike(!Global.ColossusSmashAura)),
                        // Also in Dev_FuryHeroicStrike().
                        Spell.Cast(SpellBook.ImpendingVictory,
                            ret => Global.IvTalent && !Global.IvOc && InternalSettings.Instance.Fury.CheckRotImpVic),
                        // Added for the sake of supporting it rotational.
                        Spell.Cast(SpellBook.HeroicThrow, ret => InternalSettings.Instance.Fury.CheckHeroicThrow)
                        // Added for the sake of supporting it rotational.
                        )
                    )
                );
        }

        internal static Composite Dev_FuryExec()
        {
            return new PrioritySelector(
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage),
                        Spell.Cast(SB.Execute),
                        Spell.Cast(SB.HeroicStrike, ret => Lua.PlayerPower == Me.MaxRage))),
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SB.Execute, ret => G.FadingDeathSentence(3000) && G.CsCd >= 1500),
                        // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SB.ColossusSmash),
                        Spell.Cast(SB.Bloodthirst),
                        Spell.Cast(SB.DragonRoar, ret => G.DrTalent && BloodbathSync && Tier4AbilityUsage), // Added
                        Spell.Cast(SB.StormBolt, ret => G.SbTalent && Tier6AbilityUsage), // Added
                        Spell.Cast(SB.RagingBlow),
                        Spell.Cast(SB.Execute, ret => Lua.PlayerPower == Me.MaxRage - 10),
                        Spell.Cast(SB.HeroicStrike, ret => Lua.PlayerPower == Me.MaxRage),
                        Spell.Cast(SB.Shockwave, ret => G.SwTalent && Me.IsFacing(Me.CurrentTarget) && Tier4AbilityUsage),
                        // Added
                        Spell.Cast(SB.Bladestorm,
                            ret => G.BsTalent && G.BtCd >= 2000 && G.CsCd >= 6000 && Tier4AbilityUsage) // Added
                        )));
        }

        internal static Composite Dev_FuryHeroicStrike()
        {
            return Spell.Cast(SpellBook.HeroicStrike, ret => UseHeroicStrike(G.ColossusSmashAura) && G.NormalPhase);
        }

        internal static Composite Dev_FuryMt()
        {
            return new PrioritySelector(
                new Decorator(ret => Unit.NearbyAttackableUnitsCount >= 2,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute,
                            ret => Global.DeathSentenceAuraT16 || Global.ExecutePhase && Global.ColossusSmashAura),
                        Spell.Cast(SpellBook.Cleave, ret => Me.CurrentRage > Me.MaxRage - 40, true),
                        Spell.Cast(SpellBook.Bladestorm, ret => Global.BsTalent && Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => Global.DrTalent && BloodbathSync && Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Shockwave,
                            ret => Global.SwTalent && Me.IsSafelyFacing(Me.CurrentTarget) && Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Bloodthirst),
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.Whirlwind),
                        Spell.Cast(SpellBook.RagingBlow, ret => Global.MeatCleaverAuraS3),
                        new PrioritySelector(
                            new Decorator(ret => Global.ExecutePhase, Dev_FuryExec()),
                            new Decorator(ret => Global.NormalPhase, Dev_FurySt()))
                        )));
        }

        internal static Composite Dev_FuryOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.BerserkerRage,
                    ret =>
                        BerserkerRageUsage && (Global.ColossusSmashAura &&
                        (!Global.RagingBlow2S || (!Global.FadingCs(2500) && Global.RagingBlow1S) ||
                         (!Global.RagingBlow2S && !Global.RagingBlow1S)) || !Global.CsOc && !Global.RagingBlow2S && !Global.RagingBlow1S), true),
                Spell.Cast(SpellBook.Bloodbath,
                    ret =>
                        Global.BbTalent && Tier6AbilityUsage && !Global.CsOc &&
                        (Global.RagingBlow1S || Global.RagingBlow2S), true),
                Spell.Cast(SpellBook.Recklessness,
                    ret => RecklessnessUsage && !Global.CsOc && (Global.RagingBlow1S || Global.RagingBlow2S), true),
                Spell.Cast(SpellBook.Avatar,
                    ret =>
                        Global.AvTalent && RecklessnessSync && Tier6AbilityUsage && !Global.CsOc &&
                        (Global.RagingBlow1S || Global.RagingBlow2S), true),
                Spell.Cast(SpellBook.SkullBanner,
                    ret =>
                        !Global.SkullBannerAura && RecklessnessSync && SkullBannerUsage && !Global.CsOc &&
                        (Global.RagingBlow1S || Global.RagingBlow2S), true)
                );
        }

        internal static Composite Dev_FuryGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ImpendingVictory,
                    ret =>
                        !G.IvOc && G.IvTalent && IS.Instance.Fury.CheckImpVic &&
                        Me.HealthPercent <= IS.Instance.Fury.CheckImpVicNum),
                Spell.Cast(SB.VictoryRush,
                    ret =>
                        !G.VrOc && G.VictoriousAura && IS.Instance.Fury.CheckVicRush &&
                        Me.HealthPercent <= IS.Instance.Fury.CheckVicRushNum),
                Spell.Cast(SB.IntimidatingShout,
                    ret => IS.Instance.Fury.CheckIntimidatingShout && G.IsGlyph && !U.IsTargetBoss),
                Spell.Cast(SB.ShatteringThrow,
                    ret => IS.Instance.Fury.CheckShatteringThrow && U.IsTargetBoss && (G.CsCd <= 3000 || G.SbCd <= 3000))
                );
        }

        internal static Composite Dev_FuryRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => RacialUsage && Global.ColossusSmashAura,
                    Spell.Cast(G.SelectRacialSpell(),
                        ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite Dev_FuryDefensive()
        {
            return new PrioritySelector(
                /*
                new Decorator(ret => Unit.NearbyFriendlyUnits(StyxWoW.Me.Location, 25).Any(),
                            Spell.Cast(SpellBook.Intervene,
                                ret =>
                                    Unit.NearbyFriendlyUnits(StyxWoW.Me.Location, 25)
                                        .Where(x => x.Distance > 12 && x.Distance < 35)
                                        .OrderByDescending(x => x.Distance)
                                        .FirstOrDefault(), ret => StyxWoW.Me.IsFalling)),
                 */
                Spell.Cast(SpellBook.DiebytheSword,
                    ret =>
                        InternalSettings.Instance.Fury.CheckDiebytheSword &&
                        Me.HealthPercent <= InternalSettings.Instance.Fury.CheckDiebytheSwordNum),
                Spell.Cast(SpellBook.EnragedRegeneration,
                    ret =>
                        Global.ErTalent && InternalSettings.Instance.Fury.CheckEnragedRegen &&
                        Me.HealthPercent <= InternalSettings.Instance.Fury.CheckEnragedRegenNum),
                Spell.Cast(SpellBook.ShieldWall,
                    ret =>
                        InternalSettings.Instance.Fury.CheckShieldWall &&
                        Me.HealthPercent <= InternalSettings.Instance.Fury.CheckShieldWallNum),
                Spell.Cast(SpellBook.SpellReflection,
                    ret =>
                        InternalSettings.Instance.Fury.CheckSpellReflect && Me.CurrentTarget != null &&
                        Global.TargettingMe && Me.CurrentTarget.IsCasting),
                Item.FuryUseHealthStone()
                );
        }

        internal static Composite Dev_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SB.DemoralizingBanner, loc => Me.Location,
                    ret =>
                        SettingsH.Instance.DemoBannerChoice == Keys.None && IS.Instance.Fury.CheckDemoBanner &&
                        Me.HealthPercent <= IS.Instance.Fury.CheckDemoBannerNum && U.IsDoNotUseOnTgt),
                Spell.Cast(SB.Hamstring,
                    ret =>
                        !U.IsTargetBoss && !G.HamstringAura &&
                        (IS.Instance.Fury.HamString == Enum.Hamstring.Always ||
                         IS.Instance.Fury.HamString == Enum.Hamstring.AddList && U.IsHamstringTarget)),
                Spell.Cast(SB.MassSpellReflection,
                    ret =>
                        G.MrTalent && Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && MassSpellReflectionUsage),
                Spell.Cast(SB.PiercingHowl,
                    ret =>
                        G.PhTalent && IS.Instance.Fury.CheckStaggeringShout &&
                        U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckPiercingHowlNum),
                Spell.Cast(SB.RallyingCry, ret => U.RaidMembersNeedCryCount > 0),
                Spell.Cast(SB.StaggeringShout,
                    ret =>
                        G.SsTalent && IS.Instance.Fury.CheckPiercingHowl &&
                        U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckPiercingHowlNum)
                );
        }

        #region Booleans

        internal static bool UseWildStrike(bool hasSmashAura)
        {
            if (hasSmashAura)
            {
                if ((Global.BtCd < 1500 || Global.FadingCs(1250)) && !Global.RagingBlow1S && !Global.RagingBlow2S &&
                    Me.CurrentRage >= 70)
                    return true;
            }
            else
            {
                if (Global.CsCd >= 6500 && !Global.RagingBlow1S && !Global.RagingBlow2S && Me.CurrentRage >= 80 &&
                    Global.BtCd < 1500)
                    return true;
            }
            if (Global.UrGlyph && Me.CurrentRage >= Me.MaxRage - 30)
                return true;
            return !Global.UrGlyph && Me.CurrentRage >= Me.MaxRage - 20;
        }

        internal static bool UseHeroicStrike(bool hasSmashAura)
        {
            if (hasSmashAura)
            {
                if ((Global.RagingBlow1S || Global.RagingBlow2S) && Me.CurrentRage >= 40)
                    return true;
                if ((!Global.RagingBlow1S && !Global.RagingBlow2S) && Me.CurrentRage >= 30)
                    return true;
                if ((Global.BtCd < 1500 || Global.FadingCs(2000)) && !Global.RagingBlow1S && !Global.RagingBlow2S &&
                    Me.CurrentRage >= 30)
                    return true;
            }
            else
            {
                if (Global.CsCd >= 4500 && StyxWoW.Me.CurrentRage > 90)
                    return true;
                if (Global.CsCd >= 6500 && Global.BtCd < 1500 && !Global.RagingBlow1S && !Global.RagingBlow2S &&
                    Me.CurrentRage >= 70)
                    return true;
            }
            if (Global.UrGlyph && Me.CurrentRage >= Me.MaxRage - 20)
                return true;
            return !Global.UrGlyph && Me.CurrentRage >= Me.MaxRage - 10;
        }

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
                return ((Global.RecklessnessAura) || (Global.DeterminationAura || Global.OutrageAura));
            }
        }

        internal static bool BloodbathSync
        {
            get
            {
                return (Global.BloodbathAura || !Global.BbTalent || (Global.DeterminationAura || Global.OutrageAura));
            }
        }

        #endregion
    }
}
