using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using Enum = FuryUnleashed.Core.Helpers.Enum;

namespace FuryUnleashed.Rotations.Arms
{
    class ArmsDev
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        internal static Composite DevArmsCombat
        {
            get
            {
                return new PrioritySelector(
                    new Switch<Core.Helpers.Enum.Mode>(ctx => SettingsH.Instance.ModeSelection,
                        new SwitchArgument<Core.Helpers.Enum.Mode>(Core.Helpers.Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                Dev_ArmsNonGcdUtility(),
                                Dev_ArmsRacials(),
                                Dev_ArmsOffensive(),
                                Item.CreateItemBehaviour(),
                                Dev_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(
                                            ret =>
                                                InternalSettings.Instance.Arms.CheckAoE &&
                                                Unit.NearbyAttackableUnitsCount >=
                                                InternalSettings.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_ArmsSt())
                                        )))),
                        new SwitchArgument<Core.Helpers.Enum.Mode>(Core.Helpers.Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                Dev_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_ArmsRacials(),
                                        Dev_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(
                                            ret =>
                                                InternalSettings.Instance.Arms.CheckAoE &&
                                                Unit.NearbyAttackableUnitsCount >=
                                                InternalSettings.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_ArmsSt())
                                        )))),
                        new SwitchArgument<Core.Helpers.Enum.Mode>(Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => Me.HealthPercent < 100, Dev_ArmsDefensive()),
                                Dev_ArmsNonGcdUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        Dev_ArmsRacials(),
                                        Dev_ArmsOffensive(),
                                        Item.CreateItemBehaviour())),
                                Dev_ArmsRageDump(),
                                new Decorator(ret => !Spell.IsGlobalCooldown(),
                                    new PrioritySelector(
                                        Dev_ArmsGcdUtility(),
                                        new Decorator(
                                            ret =>
                                                InternalSettings.Instance.Arms.CheckAoE && HotKeyManager.IsAoe &&
                                                Unit.NearbyAttackableUnitsCount >=
                                                InternalSettings.Instance.Arms.CheckAoENum, Dev_ArmsMt()),
                                        new Decorator(ret => Global.ExecutePhase, Dev_ArmsExec()),
                                        new Decorator(ret => Global.NormalPhase, Dev_ArmsSt())
                                        ))))));
            }
        }
        #region Development Rotations

        internal static Composite Dev_ArmsSt()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsExec()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsRageDump()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsMt()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsOffensive()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsGcdUtility()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsRacials()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsDefensive()
        {
            return new PrioritySelector();
        }

        internal static Composite Dev_ArmsNonGcdUtility()
        {
            return new PrioritySelector();
        }

        #endregion
    }
}
