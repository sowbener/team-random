using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using Lua = FuryUnleashed.Core.Helpers.LuaClass;

namespace FuryUnleashed.Rotations.Protection
{
    class ProtDev
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        // TODO: Add Execute Support
        internal static Composite DevProtCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                Dev_ProtDefensive(),
                                Dev_ProtOffensive(),
                                Dev_ProtRacials(),
                                Dev_ProtNonGcdUtility(),
                                Dev_ProtRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ProtGcdUtility(),
                                        new Decorator(ret => InternalSettings.Instance.Protection.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckAoENum, Dev_ProtMt()),
                                        new Decorator(ret => InternalSettings.Instance.Protection.CheckAoE && Unit.NearbyAttackableUnitsCount < InternalSettings.Instance.Protection.CheckAoENum, Dev_ProtSt())
                                )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                )),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                ))));
            }
        }

        internal static Composite Dev_ProtRageDump()
        {
            return new PrioritySelector(
                new Decorator(ret => Unit.NearbyAttackableUnitsCount < InternalSettings.Instance.Protection.CheckAoENum,
                    Spell.Cast(SpellBook.HeroicStrike, ret => (Lua.PlayerPower >= Lua.PlayerPowerMax - 10 && Global.NormalPhase) || Global.UltimatumAura, true)),
                new Decorator(ret => InternalSettings.Instance.Protection.CheckAoE && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckAoENum,
                    Spell.Cast(SpellBook.Cleave, ret => (Lua.PlayerPower >= Lua.PlayerPowerMax - 10 && Global.NormalPhase) || Global.UltimatumAura, true))
                );
        }

        internal static Composite Dev_ProtSt()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.Execute, ret => Global.ExecutePhase && Lua.PlayerPower >= Lua.PlayerPowerMax - 10),

                Spell.Cast(SpellBook.ShieldSlam),
                Spell.Cast(SpellBook.Revenge, ret => Lua.PlayerPower <= Lua.PlayerPowerMax - 10),

                // Added to support and DPS increase.
                Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && ProtGlobal.Tier4AbilityUsage),
                Spell.Cast(SpellBook.Devastate, ret => !Global.WeakenedArmor3S), // Builds up 3 sunder stacks.
                Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && ProtGlobal.Tier6AbilityUsage),

                Spell.Cast(SpellBook.ThunderClap, ret => !Global.WeakenedBlowsAura || Global.FadingWb(1500)),
                new Switch<Enum.Shouts>(ctx => InternalSettings.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SpellBook.BattleShout, ret => Lua.PlayerPower <= Lua.PlayerPowerMax - 25)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SpellBook.CommandingShout, ret => Lua.PlayerPower <= Lua.PlayerPowerMax - 25))),
                Spell.Cast(SpellBook.Devastate)
                );
        }

        internal static Composite Dev_ProtMt()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.Bladestorm, ret => Global.BladestormTalent && ProtGlobal.Tier4AbilityAoEUsage),
                Spell.Cast(SpellBook.DragonRoar, ret => Global.DragonRoarTalent && ProtGlobal.Tier4AbilityAoEUsage),
                Spell.Cast(SpellBook.Shockwave, ret => Global.ShockwaveTalent && ProtGlobal.Tier4AbilityAoEUsage),
                Spell.Cast(SpellBook.StormBolt, ret => Global.StormBoltTalent && ProtGlobal.Tier6AbilityAoEUsage),

                Spell.Cast(SpellBook.ThunderClap),
                Dev_ProtSt()
                );
        }

        internal static Composite Dev_ProtDefensive()
        {
            return new PrioritySelector(
                // HP Regeneration
                Spell.Cast(SpellBook.EnragedRegeneration, on => Me, ret => Global.EnragedRegenerationTalent && InternalSettings.Instance.Protection.CheckEnragedRegen && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckEnragedRegenNum),
                Item.ProtUseHealthStone(),

                // Defensive
                Spell.CastOnGround(SpellBook.DemoralizingBanner, loc => Me.Location, ret => SettingsH.Instance.DemoBannerChoice == Keys.None && InternalSettings.Instance.Protection.CheckDemoBanner && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckDemoBannerNum),
                Spell.Cast(SpellBook.DemoralizingShout, on => Me, ret => ProtGlobal.DemoralizingShoutUsage && Me.HealthPercent <= InternalSettings.Instance.Protection.DemoShoutNum),
                Spell.Cast(SpellBook.LastStand, on => Me, ret => InternalSettings.Instance.Protection.CheckLastStand && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckLastStandNum),
                Spell.Cast(SpellBook.ShieldWall, on => Me, ret => InternalSettings.Instance.Protection.CheckShieldWall && Me.HealthPercent <= InternalSettings.Instance.Protection.CheckShieldWallNum),

                Spell.Cast(SpellBook.SpellReflection, ret => ProtGlobal.SpellReflectionUsage && Unit.IsViable(Me.CurrentTarget) && Global.TargettingMe && Me.CurrentTarget.IsCasting),
                Spell.Cast(SpellBook.MassSpellReflection, ret => Global.MassSpellReflectionTalent && ProtGlobal.MassSpellReflectionUsage && Global.SpellReflectionSpellCooldown > 0 && Unit.IsViable(Me.CurrentTarget) && Global.TargettingMe && Me.CurrentTarget.IsCasting),

                new Decorator(ret => InternalSettings.Instance.Protection.CheckShieldBarrierBlock && !InternalSettings.Instance.Protection.CheckShieldBbAdvancedLogics && Lua.PlayerPower >= InternalSettings.Instance.Protection.ShieldBarrierBlockNum,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ShieldBarrier, on => Me, ret => InternalSettings.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBarrier, true),
                        Spell.Cast(SpellBook.ShieldBlock, on => Me, ret => InternalSettings.Instance.Protection.BarrierBlockSelection == Enum.BarrierBlock.ShieldBlock, true))),

                new Decorator(ret => InternalSettings.Instance.Protection.CheckShieldBarrierBlock && InternalSettings.Instance.Protection.CheckShieldBbAdvancedLogics && Lua.PlayerPower >= InternalSettings.Instance.Protection.ShieldBarrierBlockNum,
                    new PrioritySelector(
                        Spell.Cast(SpellBook.ShieldBarrier, on => Me, ret => (ProtTracker.CalculateEstimatedAbsorbValue() > ProtTracker.CalculateEstimatedBlockValue()) || (Lua.PlayerPower > 90 && Me.HealthPercent < 100), true),
                        Spell.Cast(SpellBook.ShieldBlock, on => Me, ret => ProtTracker.CalculateEstimatedAbsorbValue() <= ProtTracker.CalculateEstimatedBlockValue(), true))));
        }

        internal static Composite Dev_ProtGcdUtility()
        {
            return new PrioritySelector(
                //138279 Victorious - T15 Proc ID (Victory Rush & Impending Victory).
                //32216	Victorious - Regular Kill Proc ID (Victory Rush & Impending Victory).
                Spell.Cast(SpellBook.ImpendingVictory, ret => Global.ImpendingVictoryTalent && Me.HealthPercent <= InternalSettings.Instance.Protection.ImpendingVictoryNum),
                Spell.Cast(SpellBook.VictoryRush, ret => Me.HealthPercent <= InternalSettings.Instance.Protection.VictoryRushNum)
                );
        }

        internal static Composite Dev_ProtNonGcdUtility()
        {
            return new PrioritySelector(
                // Need to improve hamstring method
                Spell.Cast(SpellBook.Hamstring, ret => !Unit.IsTargetBoss && !Global.HamstringAura && (InternalSettings.Instance.Protection.HamString == Enum.Hamstring.Always || InternalSettings.Instance.Protection.HamString == Enum.Hamstring.AddList && Unit.IsHamstringTarget)),
                Spell.Cast(SpellBook.IntimidatingShout, ret => InternalSettings.Instance.Protection.CheckIntimidatingShout && Global.IntimidatingShoutGlyph && !Unit.IsTargetBoss),
                Spell.Cast(SpellBook.Taunt, ret => InternalSettings.Instance.Protection.CheckAutoTaunt && !Global.TargettingMe),
                Spell.Cast(SpellBook.RallyingCry, ret => Unit.RaidMembersNeedCryCount > 0 && !Global.LastStandAura && InternalSettings.Instance.Protection.CheckRallyingCry),
                Spell.Cast(SpellBook.StaggeringShout, ret => Global.StaggeringShoutTalent && InternalSettings.Instance.Protection.CheckStaggeringShout && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckStaggeringShoutNum),
                Spell.Cast(SpellBook.PiercingHowl, ret => Global.PiercingHowlTalent && InternalSettings.Instance.Protection.CheckPiercingHowl && Unit.NearbyAttackableUnitsCount >= InternalSettings.Instance.Protection.CheckPiercingHowlNum),
                new Decorator(ret => Unit.VigilanceTarget != null,
                    Spell.Cast(SpellBook.Vigilance, on => Unit.VigilanceTarget))
                );
        }

        internal static Composite Dev_ProtRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => ProtGlobal.RacialUsage,
                    Spell.Cast(Global.SelectRacialSpell(), ret => Global.SelectRacialSpell() != null && Global.RacialUsageSatisfied(Global.SelectRacialSpell()))
                    ));
        }

        internal static Composite Dev_ProtOffensive()
        {
            return new PrioritySelector(
                Spell.Cast(SpellBook.Avatar, on => Me, ret => Global.AvatarTalent && ProtGlobal.Tier6AbilityUsage, true),
                Spell.Cast(SpellBook.BerserkerRage, on => Me, ret => (!Global.EnrageAura || Global.FadingEnrage(1500)) && ProtGlobal.BerserkerRageUsage, true),
                Spell.Cast(SpellBook.Bloodbath, on => Me, ret => Global.BloodbathTalent && ProtGlobal.Tier6AbilityUsage, true),
                Spell.Cast(SpellBook.Recklessness, on => Me, ret => ProtGlobal.RecklessnessUsage, true),
                Spell.Cast(SpellBook.SkullBanner, ret => !Global.SkullBannerAura && ProtGlobal.SkullBannerUsage, true)
                );
        }
    }
}
