using System;
using System.Linq;
using System.Text;
using FuryUnleashed.Core.Utilities;
using FuryUnleashed.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;

// Credits for this code go out to the PureRotation Team!
namespace FuryUnleashed.Core.Helpers
{
    internal static class LuaClass
    {
        /// <summary>
        /// Lua to start AutoAttacking
        /// </summary>
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

        /// <summary>
        /// Lua used for running Macro's ingame.
        /// </summary>
        /// <param name="luastring">Lua/Macro to Execute</param>
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

        /// <summary>
        /// Returns Me.CurrentRage via LUA instead of HB API - HB API as Backup.
        /// </summary>
        public static uint PlayerPower
        {
            get
            {
                try { return Lua.GetReturnVal<uint>("return UnitPower(\"player\");", 0); }
                catch { Logger.DiagLogFb("[FU] Lua Failed in PlayerPower"); return StyxWoW.Me.CurrentPower; }
            }
        }

        /// <summary>
        /// Returns Me.CurrentRage via LUA instead of HB API - Using a new frame  - HB API as Backup.
        /// </summary>
        public static uint PlayerPowerFlocked
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        return Lua.GetReturnVal<uint>("return UnitPower(\"player\");", 0);
                    }
                }
                catch { Logger.DiagLogFb("[FU] Lua Failed in PlayerPower"); return StyxWoW.Me.CurrentPower; }
            }
        }

        /// <summary>
        /// Returns Me.MaxRage via LUA instead of HB API - HB API as Backup.
        /// </summary>
        public static uint PlayerPowerMax
        {
            get
            {
                try { return Lua.GetReturnVal<uint>("return UnitPowerMax(\"player\",1);", 0); }
                catch { Logger.DiagLogFb("[FU] Lua Failed in PlayerPowerMax"); return StyxWoW.Me.MaxRage; }
            }
        }

        /// <summary>
        /// Returns Me.MaxRage via LUA instead of HB API - Using a new frame  - HB API as Backup.
        /// </summary>
        public static uint PlayerPowerMaxFlocked
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        return Lua.GetReturnVal<uint>("return UnitPowerMax(\"player\",1);", 0);
                    }
                }
                catch
                {
                    Logger.DiagLogFb("[FU] Lua Failed in PlayerPowerMax"); return StyxWoW.Me.MaxRage;
                }
            }
        }

        /// <summary>
        /// Updates the Ingame Focus Frame after setting a Focus via HB.
        /// </summary>
        /// <param name="unit">Me.FocusedUnit</param>
        public static void UpdateFocusFrame(WoWUnit unit)
        {
            Lua.DoString("TargetFrame_Update(FocusFrame)");
        }  


        /// <summary>
        /// Disabled the Click to Move function
        /// </summary>
        public static void DisableClickToMove()
        {
            if (InternalSettings.Instance.General.CheckDisableClickToMove)
            {
                Lua.DoString("SetCVar('autoInteract', '0')");
            }
        }

        /// <summary>
        /// Enables the Click to Move function
        /// </summary>
        public static void EnableClickToMove()
        {
            if (InternalSettings.Instance.General.CheckDisableClickToMove)
            {
                Lua.DoString("SetCVar('autoInteract', '1')");
            }
        }

        /// <summary>
        /// Disables Script Errors
        /// </summary>
        public static void DisableScriptErrors()
        {
            Lua.DoString("SetCVar('scriptErrors', '0')");
        }

        /// <summary>
        /// Enables Script Errors
        /// </summary>
        public static void EnableScriptErrors()
        {
            Lua.DoString("SetCVar('scriptErrors', '1')");
        }

        /// <summary>
        /// Retrieves FPS ingame (WoW)
        /// </summary>
        /// <returns>Frames per Second</returns>
        internal static uint GetFps()
        {
            try
            {
                return (uint)Lua.GetReturnVal<float>("return GetFramerate()", 0);
            }
            catch (Exception ex)
            {
                Logger.DiagLogFb("[FU] {0} - GetFps()", ex);
            }
            return 0;
        }

        /// <summary>
        /// Used to check if unit is a Tank
        /// </summary>
        /// <param name="player">WoWPlayer</param>
        /// <returns>The tank</returns>
        public static bool IsTank(WoWPlayer player)
        {
            return Lua.GetReturnValues("return UnitGroupRolesAssigned('" + DeUnicodify(player.Name) + "')").First() == "TANK";
        }

        public static string DeUnicodify(string spell)
        {
            StringBuilder sb = new StringBuilder();

            byte[] bytes = Encoding.UTF8.GetBytes(spell);

            foreach (byte b in bytes)
            {
                if (b != 0)
                    sb.Append("\\" + b);
            }

            return sb.ToString();
        }
    }
}
