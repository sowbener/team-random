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
    class FuryDev
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

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
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_FurySt())
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
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Dev_FuryMt()),
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
                                        Dev_FuryOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => IS.Instance.Fury.CheckAoE && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_FurySt())
                                        ))))));
            }
        }

        internal static Composite Dev_FurySt()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryExec()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryHeroicStrike()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryMt()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryOffensive()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryGcdUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryRacials()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryDefensive()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite Dev_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                );
        }
    }
}
