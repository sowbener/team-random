using FuryUnleashed.Core;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using FG = FuryUnleashed.Rotations.Fury.FuryGlobal;
using G = FuryUnleashed.Rotations.Global;
using IS = FuryUnleashed.Interfaces.Settings.InternalSettings;
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
                                new Decorator(ret => FG.StanceDanceUsage, Dev_FuryStanceDance()),
                                Dev_FuryNonGcdUtility(),
                                Dev_FuryRacials(),
                                Dev_FuryOffensive(),
                                Item.CreateItemBehaviour(),
                                Dev_FuryHeroicStrike(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_FuryGcdUtility(),
                                        new Decorator(ret => FG.MultiTargetUsage && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                new Decorator(ret => FG.StanceDanceUsage, Dev_FuryStanceDance()),
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
                                        new Decorator(ret => FG.MultiTargetUsage && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_FurySt())
                                        )))),
                        new SwitchArgument<Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_FuryDefensive()),
                                new Decorator(ret => FG.StanceDanceUsage, Dev_FuryStanceDance()),
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
                                        new Decorator(ret => FG.MultiTargetUsage && HotKeyManager.IsAoe && U.NearbyAttackableUnitsCount >= IS.Instance.Fury.CheckAoENum, Dev_FuryMt()),
                                        new Decorator(ret => G.ExecutePhase, Dev_FuryExec()),
                                        new Decorator(ret => G.NormalPhase, Dev_FurySt())
                                        ))))));
            }
        }

        internal static Composite Dev_FurySt()
        {
            return new PrioritySelector(
                FuryRel.Rel_FurySt()
                );
        }

        internal static Composite Dev_FuryStanceDance()
        {
            return new PrioritySelector(
                FuryRel.Rel_FuryStanceDance()
                );
        }

        internal static Composite Dev_FuryHeroicStrike()
        {
            return new PrioritySelector(
                FuryRel.Rel_FuryHeroicStrike()
                );
        }

        internal static Composite Dev_FuryExec()
        {
            return new PrioritySelector(
                FuryRel.Rel_FuryExec()
                );
        }

        internal static Composite Dev_FuryMt()
        {
            return new PrioritySelector(
                FuryRel.Rel_FuryMt()
                );
        }

        internal static Composite Dev_FuryOffensive()
        {
            return new PrioritySelector(
                FuryRel.Rel_FuryOffensive()
                );
        }

        internal static Composite Dev_FuryGcdUtility()
        {
            return new PrioritySelector(
                FuryRel.Rel_FuryGcdUtility()
                );
        }

        internal static Composite Dev_FuryRacials()
        {
            return new PrioritySelector(
                FuryRel.Rel_FuryRacials()
                );
        }

        internal static Composite Dev_FuryDefensive()
        {
            return new PrioritySelector(
                FuryRel.Rel_FuryDefensive()
                );
        }

        internal static Composite Dev_FuryNonGcdUtility()
        {
            return new PrioritySelector(
                FuryRel.Rel_FuryNonGcdUtility()
                );
        }
    }
}
