using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using G = FuryUnleashed.Rotations.Global;
using U = FuryUnleashed.Core.Unit;
using L = FuryUnleashed.Core.Helpers.LuaClass;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
using SB = FuryUnleashed.Core.Helpers.SpellBook;

namespace FuryUnleashed.Rotations.Protection
{
    class ProtDev
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static Composite DevProtCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ProtDefensive()),
                                Dev_ProtNonGcdUtility(),
                                Dev_ProtRacials(),
                                Dev_ProtOffensive(),
                                Item.CreateItemBehaviour(),
                                Dev_ProtRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ProtGcdUtility(),
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ProtMt()),
                                        Dev_ProtSt()
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ProtDefensive()),
                                Dev_ProtNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_ProtRacials(),
                                        Dev_ProtOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_ProtRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ProtGcdUtility(),
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ProtMt()),
                                        Dev_ProtSt()
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ProtDefensive()),
                                Dev_ProtNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_ProtRacials(),
                                        Dev_ProtOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_ProtRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ProtGcdUtility(),
                                        new Decorator(ret => IS.Instance.Arms.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Arms.CheckAoENum, Dev_ProtMt()),
                                        Dev_ProtSt()
                                        ))))));
            }
        }

        internal static Composite Dev_ProtRageDump()
        {
            return new PrioritySelector(
                new Decorator(ret => U.NearbyAttackableUnitsCount < IS.Instance.Protection.CheckAoENum,
                    Spell.Cast(SB.HeroicStrike, ret => L.PlayerPower >= L.PlayerPowerMax - 10 && G.NormalPhase)),
                new Decorator(ret => IS.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Protection.CheckAoENum,
                    Spell.Cast(SB.Cleave))
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
                );
        }

        internal static Composite Dev_ProtGcdUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_ProtRacials()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_ProtOffensive()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_ProtNonGcdUtility()
        {
            return new PrioritySelector(
                );
        }
    }
}
