using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using System;
using System.Linq;
using System.Text;
using YBMoP_BT_Rogue.Interfaces.Settings;
using Action = Styx.TreeSharp.Action;

// Credits for this code go out to the PureRotation Team!
namespace YBMoP_BT_Rogue.Helpers
{
    static class YBLua
    {
        #region YBMoP BT - PlayerPower Energy
        public static int PlayerPower
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        return Lua.GetReturnVal<int>("return UnitPower(\"player\");", 0);
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion

        #region YBMoP BT - ComboPoints
        public static double PlayerComboPts
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        return Styx.WoWInternals.Lua.GetReturnVal<int>("return GetComboPoints(\"player\");", 0);
                    }
                }
                catch
                {
                  //  Logger.FailLog(" Lua Failed in PlayerComboPts");
                    return 0;
                }
            }
        }
        #endregion

        #region YBMoP BT - Start AutoAttack
        internal static Composite StartAutoAttack
        {
            get
            {
                return new Action(ret =>
                    {
                        if (!StyxWoW.Me.IsAutoAttacking)
                            Lua.DoString("StartAttack()");
                        return RunStatus.Failure;
                    });
            }
        }
        #endregion

        #region YBMoP BT - Run Macro text
        public static string RealLuaEscape(string luastring)
        {
            var bytes = Encoding.UTF8.GetBytes(luastring);
            return bytes.Aggregate(String.Empty, (current, b) => current + ("\\" + b));
        }

        public static Composite RunMacroText(string macro, CanRunDecoratorDelegate cond)
        {
            return new Decorator(
                       cond,
                       new PrioritySelector(
                           new Action(a => Lua.DoString("RunMacroText(\"" + RealLuaEscape(macro) + "\")"))));
        }
        #endregion

        #region YBMoP BT - Disable Click-To-Move
        public static void DisableClickToMove()
        {
            if (YBSettingsG.Instance.CheckDisableClickToMove)
            {
                Lua.DoString("SetCVar('autoInteract', '0')");
            }
        }

        public static void EnableClickToMove()
        {
            if (!YBSettingsG.Instance.CheckDisableClickToMove)
            {
                Lua.DoString("SetCVar('autoInteract', '1')");
            }
        }
        #endregion
    }
}
