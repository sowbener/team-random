using System.Windows.Forms;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using G = FuryUnleashed.Rotations.Global;
using AG = FuryUnleashed.Rotations.Arms.ArmsGlobal;
using Lua = FuryUnleashed.Core.Helpers.LuaClass;

namespace FuryUnleashed.Rotations.Arms
{
    class ArmsRel
    {
        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        internal static Composite RelArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_ArmsDefensive()),
                                Rel_ArmsNonGcdUtility(),
                                Rel_ArmsRacials(),
                                Rel_ArmsOffensive(),
                                Item.CreateItemBehaviour(),
                                Rel_ArmsHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_ArmsGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_ArmsSt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_ArmsDefensive()),
                                Rel_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Rel_ArmsRacials(),
                                        Rel_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                Rel_ArmsHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_ArmsGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_ArmsSt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Rel_ArmsDefensive()),
                                Rel_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Rel_ArmsRacials(),
                                        Rel_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                Rel_ArmsHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Rel_ArmsGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && HotKeyManager.IsAoe && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum, Rel_ArmsMt()),
                                        new Decorator(ret => G.ExecutePhase, Rel_ArmsExec()),
                                        new Decorator(ret => G.NormalPhase, Rel_ArmsSt())
                                        ))))));
            }
        }

        internal static Composite Rel_ArmsSt()
        {
            return new PrioritySelector(
                // Inside Colossus Smash window
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => G.DeathSentenceAuraT16), // Added T16 P4
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && AG.Tier6AbilityUsage), // Added.
                        Spell.Cast(SpellBook.MortalStrike), // Trying this for rage.
                        Spell.Cast(SpellBook.Slam),
                        Spell.Cast(SpellBook.Overpower, ret => G.SlamOnCooldown && G.MortalStrikeOnCooldown),
                        Spell.Cast(SpellBook.HeroicStrike, ret => Me.CurrentRage == Me.MaxRage))),
                // Outside Colossus Smash window
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Execute, ret => G.FadingDeathSentence(3000) && G.ColossusSmashSpellCooldown >= 1500), // Added T16 P4 - Waiting for CS window unless expires.
                        Spell.Cast(SpellBook.ColossusSmash),
                        Spell.Cast(SpellBook.MortalStrike),
                        Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && AG.BloodbathSync && AG.Tier4AbilityUsage), // Added.
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && G.ReadinessAura && G.ColossusSmashSpellCooldown >= 16000 && AG.Tier6AbilityUsage), // Added - When new one is ready in next CS window - With Eye of Galakras.
                        Spell.Cast(SpellBook.Overpower),
                        Spell.Cast(SpellBook.HeroicThrow, ret => AG.HeroicThrowUsage),
                        Spell.Cast(SpellBook.Bladestorm, ret => G.BladestormTalent && G.ColossusSmashSpellCooldown >= 6000 && AG.Tier4AbilityUsage), // Added - For the sake of supporting it.
                        Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && AG.Tier4AbilityUsage), // Added - For the sake of supporting it.
                        new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Arms.ShoutSelection,
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, on => Me)),
                            new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, on => Me))),
                        Spell.Cast(SpellBook.Execute, ret => G.DeathSentenceAuraT16),
                        Spell.Cast(SpellBook.Slam, ret => Me.CurrentRage >= 90 && !G.DeathSentenceAuraT16),
                        Spell.Cast(SpellBook.ImpendingVictory, ret => G.ImpendingVictoryTalent && !G.ImpendingVictoryOnCooldown && AG.ImpendingVictoryUsage) // Added for the sake of supporting it rotational.                        
                        )));
        }

        internal static Composite Rel_ArmsHeroicStrike()
        {
            return new PrioritySelector(
                new Decorator(ret => (!InternalSettings.Instance.Arms.CheckAoE || Unit.NearbyAttackableUnitsCount < InternalSettings.Instance.Arms.CheckAoENum),
                    new PrioritySelector(
                        Spell.Cast(SpellBook.HeroicStrike, ret => ((G.UnendingRageGlyph && Me.CurrentRage >= Me.MaxRage - 15) || (!G.UnendingRageGlyph && Me.CurrentRage >= Me.MaxRage - 15)), true))),
                new Decorator(ret => InternalSettings.Instance.Arms.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckAoENum,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.Cleave, ret => Me.CurrentRage == Me.MaxRage))));
        }

        internal static Composite Rel_ArmsExec()
        {
            return new PrioritySelector(
                new Decorator(ret => !G.ColossusSmashAura,
                    new PrioritySelector()),
                new Decorator(ret => G.ColossusSmashAura,
                    new PrioritySelector()));
        }

        internal static Composite Rel_ArmsMt()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.Execute, ret => G.DeathSentenceAuraT16), // Added.
                new Decorator(ret => Unit.NearbyAttackableUnitsCount == 2,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ThunderClap, ret => InternalSettings.Instance.Arms.CheckAoEThunderclap && Unit.NeedThunderclapUnitsCount > 0), // Should be MultiDot Mortal Strike ...

                        Spell.Cast(SpellBook.Bladestorm, ret => G.BladestormTalent && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && AG.BloodbathSync && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && AG.Tier6AbilityAoEUsage),

                        Spell.Cast(SpellBook.SweepingStrikes),
                        Spell.Cast(SpellBook.ColossusSmash), // Added.
                        Spell.Cast(SpellBook.MortalStrike), // Added - Generate rage.
                        new Decorator(ret => InternalSettings.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.Slam, ret => G.SlamViable),
                                Spell.Cast(SpellBook.Whirlwind, ret => G.WhirlwindViable))),
                        new Decorator(ret => !InternalSettings.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.Slam))),
                        new Decorator(ret => G.NormalPhase, Rel_ArmsSt()),
                        new Decorator(ret => G.ExecutePhase, Rel_ArmsExec()))),
                new Decorator(ret => Unit.NearbyAttackableUnitsCount >= 3,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ThunderClap, ret => InternalSettings.Instance.Arms.CheckAoEThunderclap && Unit.NeedThunderclapUnitsCount > 0),

                        Spell.Cast(SpellBook.Bladestorm, ret => G.BladestormTalent && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.DragonRoar, ret => G.DragonRoarTalent && AG.BloodbathSync && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.Shockwave, ret => G.ShockwaveTalent && G.ShockwaveFacing && AG.Tier4AbilityAoEUsage),
                        Spell.Cast(SpellBook.StormBolt, ret => G.StormBoltTalent && AG.Tier6AbilityAoEUsage),

                        Spell.Cast(SpellBook.SweepingStrikes),
                        Spell.Cast(SpellBook.ColossusSmash), // Added.
                        Spell.Cast(SpellBook.MortalStrike), // Added - Generate rage.
                        new Decorator(ret => InternalSettings.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.Slam, ret => G.SlamViable),
                                Spell.Cast(SpellBook.Whirlwind, ret => G.WhirlwindViable))),
                        new Decorator(ret => !InternalSettings.Instance.Arms.CheckExperimentalAoE,
                            new PrioritySelector(
                                Spell.Cast(SpellBook.Slam))),
                        new Decorator(ret => G.NormalPhase, Rel_ArmsSt()),
                        new Decorator(ret => G.ExecutePhase, Rel_ArmsExec()))));
        }

        internal static Composite Rel_ArmsOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.BerserkerRage, ret => (!G.EnrageAura || G.FadingEnrage(500)) && G.ColossusSmashAura && AG.BerserkerRageUsage),
                Spell.Cast(SpellBook.Bloodbath, ret => G.BloodbathTalent && AG.Tier4AbilityUsage),

                Spell.Cast(SpellBook.Recklessness, ret => AG.RecklessnessUsage),
                Spell.Cast(SpellBook.SkullBanner, ret => !G.SkullBannerAura && AG.RecklessnessSync && AG.SkullBannerUsage),
                Spell.Cast(SpellBook.Avatar, ret => G.AvatarTalent && AG.RecklessnessSync && AG.Tier6AbilityUsage));
        }

        internal static Composite Rel_ArmsGcdUtility()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.ImpendingVictory, ret => !G.ImpendingVictoryOnCooldown && G.ImpendingVictoryTalent && InternalSettings.Instance.Arms.CheckImpVic && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckImpVicNum),
                Spell.Cast(SpellBook.VictoryRush, ret => !G.VictoryRushOnCooldown && G.VictoriousAura && InternalSettings.Instance.Arms.CheckVicRush && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckVicRushNum),
                Spell.Cast(SpellBook.IntimidatingShout, ret => InternalSettings.Instance.Arms.CheckIntimidatingShout && G.IntimidatingShoutGlyph && !Unit.IsTargetBoss),
                Spell.Cast(SpellBook.ShatteringThrow, ret => InternalSettings.Instance.Arms.CheckShatteringThrow && Unit.IsTargetBoss && (G.ColossusSmashSpellCooldown <= 3000 || G.SkullBannerSpellCooldown <= 3000)));
        }

        internal static Composite Rel_ArmsRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => AG.RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))));
        }

        internal static Composite Rel_ArmsDefensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.DiebytheSword, ret => InternalSettings.Instance.Arms.CheckDiebytheSword && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckDiebytheSwordNum),
                Spell.Cast(SpellBook.EnragedRegeneration, ret => G.EnragedRegenerationTalent && InternalSettings.Instance.Arms.CheckEnragedRegen && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckEnragedRegenNum),
                Spell.Cast(SpellBook.ShieldWall, ret => InternalSettings.Instance.Arms.CheckShieldWall && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckShieldWallNum),
                Spell.Cast(SpellBook.SpellReflection, ret => InternalSettings.Instance.Arms.CheckSpellReflect && Unit.IsViable(Me.CurrentTarget) && Unit.IsTargettingMe && Me.CurrentTarget.IsCasting),
                Item.ArmsUseHealthStone());
        }

        internal static Composite Rel_ArmsNonGcdUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround(SpellBook.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && InternalSettings.Instance.Arms.CheckDemoBanner && Me.HealthPercent <= InternalSettings.Instance.Arms.CheckDemoBannerNum && Unit.IsDoNotUseOnTgt),
                Spell.Cast(SpellBook.Hamstring, ret => !Unit.IsTargetBoss && !G.HamstringAura && (InternalSettings.Instance.Arms.HamString == Enum.Hamstring.Always || InternalSettings.Instance.Arms.HamString == Enum.Hamstring.AddList && Unit.IsHamstringTarget)),
                Spell.Cast(SpellBook.MassSpellReflection, ret => G.MassSpellReflectionTalent && Unit.IsViable(Me.CurrentTarget) && Me.CurrentTarget.IsCasting && AG.MassSpellReflectionUsage),
                Spell.Cast(SpellBook.PiercingHowl, ret => G.PiercingHowlTalent && InternalSettings.Instance.Arms.CheckStaggeringShout && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckPiercingHowlNum),
                Spell.Cast(SpellBook.RallyingCry, ret => Unit.RaidMembersNeedCryCount > 0),
                Spell.Cast(SpellBook.StaggeringShout, ret => G.StaggeringShoutTalent && InternalSettings.Instance.Arms.CheckPiercingHowl && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Arms.CheckPiercingHowlNum),
                new Decorator(ret => Unit.VigilanceTarget != null,
                    Spell.Cast(SpellBook.Vigilance, on => Unit.VigilanceTarget)));
        }
    }
}
