using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using G = FuryUnleashed.Rotations.Global;
using U = FuryUnleashed.Core.Unit;
using L = FuryUnleashed.Core.Helpers.LuaClass;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SB = FuryUnleashed.Core.Helpers.SpellBook;
using PG = FuryUnleashed.Rotations.Protection.ProtGlobal;

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
                                )),
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
                new Decorator(ret => U.NearbyAttackableUnitsCount < IS.Instance.Protection.CheckAoENum,
                    Spell.Cast(SB.HeroicStrike, ret => L.PlayerPower >= L.PlayerPowerMax - 10 && G.NormalPhase)),
                new Decorator(ret => IS.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckAoENum,
                    Spell.Cast(SB.Cleave, ret => L.PlayerPower >= L.PlayerPowerMax - 10 && G.NormalPhase))
                );
        }

        internal static Composite Dev_ProtSt()
        {
            return new PrioritySelector(
                Spell.Cast(SB.Devastate, ret => !G.WeakenedArmor3S),

                Spell.Cast(SB.ShieldSlam),
                Spell.Cast(SB.Revenge, ret => L.PlayerPower != L.PlayerPowerMax),
                Spell.Cast(SB.Devastate),
                Spell.Cast(SB.ThunderClap, ret => !G.WeakenedBlowsAura || G.FadingWb(1500)),
                new Switch<Enum.Shouts>(ctx => IS.Instance.Protection.ShoutSelection,
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.BattleShout, Spell.Cast(SB.BattleShout)),
                    new SwitchArgument<Enum.Shouts>(Enum.Shouts.CommandingShout, Spell.Cast(SB.CommandingShout)))
                );
        }

        internal static Composite Dev_ProtMt()
        {
            return new PrioritySelector(
                Spell.Cast(SB.ThunderClap),
                Dev_ProtSt()
                );
        }

        internal static Composite Dev_ProtDefensive()
        {
            return new PrioritySelector(
                new Decorator(ret => IS.Instance.Protection.CheckShieldBbAdvancedLogics,
                    new PrioritySelector(
                        Spell.Cast(SB.ShieldBarrier, ret => 
                            (ProtTracker.CalculateEstimatedAbsorbValue() > ProtTracker.CalculateEstimatedBlockValue()) ||
                            (Me.CurrentRage > 90 && Me.HealthPercent < 100)),
                        Spell.Cast(SB.ShieldBlock, ret => 
                            ProtTracker.CalculateEstimatedAbsorbValue() <= ProtTracker.CalculateEstimatedBlockValue()))));
        }

        internal static Composite Dev_ProtGcdUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_ProtNonGcdUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_ProtRacials()
        {
            return new PrioritySelector(
                new Decorator(ret => PG.RacialUsage,
                    Spell.Cast(G.SelectRacialSpell(), ret => G.SelectRacialSpell() != null && G.RacialUsageSatisfied(G.SelectRacialSpell()))
                    ));
        }

        internal static Composite Dev_ProtOffensive()
        {
            return new PrioritySelector(
                );
        }
    }
}
