﻿using Xiaolin.Interfaces.Settings;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using System;
using System.Linq;
using System.Text;
using Action = Styx.TreeSharp.Action;
using Logger = Xiaolin.Helpers.XILogger;

// Credits for this code go out to the PureRotation Team!
namespace Xiaolin.Helpers
{
    static class XILua
    {
        #region LuaSecondaryStats Credit: Singular

        internal static SecondaryStats _secondaryStats;          //create within frame (does series of LUA calls)

        internal static void PopulateSecondryStats()
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                _secondaryStats = new SecondaryStats();
            }

            // Haste Rating Required Per 1%
            // Level 60	 Level 70	 Level 80	 Level 85	 Level 90
            //   10	      15.77	      32.79	      128.125	 425.19

            Logger.InfoLog("");
            Logger.InfoLog("Health: {0}", StyxWoW.Me.MaxHealth);
            Logger.InfoLog("Agility: {0}", StyxWoW.Me.Agility);
            Logger.InfoLog("Intellect: {0}", StyxWoW.Me.Intellect);
            Logger.InfoLog("Spirit: {0}", StyxWoW.Me.Spirit);
            Logger.InfoLog("");
            Logger.InfoLog("Attack Power: {0}", StyxWoW.Me.AttackPower);
            Logger.InfoLog("Power: {0:F2}", _secondaryStats.Power);
            Logger.InfoLog("Hit(M/R): {0}/{1}", _secondaryStats.MeleeHit, _secondaryStats.SpellHit);
            Logger.InfoLog("Expertise: {0}", _secondaryStats.Expertise);
            Logger.InfoLog("Mastery: {0:F2}", _secondaryStats.Mastery);
            Logger.InfoLog("Mastery (CR): {0:F2}", _secondaryStats.MasteryCR);
            Logger.InfoLog("Crit: {0:F2}", _secondaryStats.Crit);
            Logger.InfoLog("Haste(M/R): {0} (+{1} % Haste) / {2} (+{3} % Haste)", _secondaryStats.MeleeHaste, Math.Round(_secondaryStats.MeleeHaste / 425.19, 2), _secondaryStats.SpellHaste, Math.Round(_secondaryStats.SpellHaste / 425.19, 2));
            Logger.InfoLog("SpellPen: {0}", _secondaryStats.SpellPen);
            Logger.InfoLog("PvP Resil: {0}", _secondaryStats.Resilience);
            Logger.InfoLog("PvP Power: {0}", _secondaryStats.PvpPower);
            Logger.InfoLog("");
        }

        internal class SecondaryStats
        {
            public float MeleeHit { get; set; }

            public float SpellHit { get; set; }

            public float Expertise { get; set; }

            public float MeleeHaste { get; set; }

            public float SpellHaste { get; set; }

            public float SpellPen { get; set; }

            public float Mastery { get; set; }

            public float MasteryCR { get; set; }

            public float Crit { get; set; }

            public float Resilience { get; set; }

            public float PvpPower { get; set; }

            public float AttackPower { get; set; }

            public float Power { get; set; }

            public float Intellect { get; set; }

            public float SpellPower { get; set; }

            public SecondaryStats()
            {
                Refresh();
            }

            public void Refresh()
            {
                try
                {
                    MeleeHit = Styx.WoWInternals.Lua.GetReturnVal<float>("return GetCombatRating(CR_HIT_MELEE)", 0);
                    SpellHit = Styx.WoWInternals.Lua.GetReturnVal<float>("return GetCombatRating(CR_HIT_SPELL)", 0);
                    Expertise = StyxWoW.Me.Expertise;
                    MeleeHaste = Styx.WoWInternals.Lua.GetReturnVal<float>("return GetCombatRating(CR_HASTE_MELEE)", 0);
                    SpellHaste = Styx.WoWInternals.Lua.GetReturnVal<float>("return GetCombatRating(CR_HASTE_SPELL)", 0);
                    SpellPen = Styx.WoWInternals.Lua.GetReturnVal<float>("return GetSpellPenetration()", 0);
                    Mastery = StyxWoW.Me.Mastery;
                    MasteryCR = Styx.WoWInternals.Lua.GetReturnVal<float>("return GetCombatRating(CR_MASTERY)", 0);
                    Crit = StyxWoW.Me.CritPercent;
                    Resilience = Styx.WoWInternals.Lua.GetReturnVal<float>("return GetCombatRating(COMBAT_RATING_RESILIENCE_CRIT_TAKEN)", 0);
                    PvpPower = Styx.WoWInternals.Lua.GetReturnVal<float>("return GetCombatRating(CR_PVP_POWER)", 0);
                    AttackPower = StyxWoW.Me.AttackPower;
                    Power = Styx.WoWInternals.Lua.GetReturnVal<float>("return select(7,UnitDamage(\"player\"))", 0);
                    Intellect = StyxWoW.Me.Intellect;
                    SpellPower = Styx.WoWInternals.Lua.GetReturnVal<float>("return math.max(GetSpellBonusDamage(1),GetSpellBonusDamage(2),GetSpellBonusDamage(3),GetSpellBonusDamage(4),GetSpellBonusDamage(5),GetSpellBonusDamage(6),GetSpellBonusDamage(7))", 0);
                }
                catch
                {
                    Logger.FailLog(" Lua Failed in SecondaryStats");
                }

            }
        }

        #endregion SecondryStats - Credit: Singular
        
        #region PlayerPower Rage
        public static double PlayerPower
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        return Styx.WoWInternals.Lua.GetReturnVal<int>("return UnitPower(\"player\");", 0);
                    }
                }
                catch { Logger.FailLog(" Lua Failed in PlayerPower"); return StyxWoW.Me.CurrentPower; }
            }
        }

        public static double PlayerPowerMax
        {
            get
            {
                try
                {
                    using (StyxWoW.Memory.AcquireFrame())
                    {
                        return Lua.GetReturnVal<int>("return UnitPowerMax(\"player\",1);", 0);
                    }
                }
                catch
                {
                    XILogger.DiagLogP("XI: Lua Failed in PlayerPowerMax"); return 0;
                }
            }
        }
        #endregion

        #region Start AutoAttack
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

        #region Run Macro text
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

        #region Disable Click-To-Move
        public static void DisableClickToMove()
        {
            if (XISettings.Instance.General.CheckDisableClickToMove)
            {
                Lua.DoString("SetCVar('autoInteract', '0')");
            }
        }

        public static void EnableClickToMove()
        {
            if (XISettings.Instance.General.CheckDisableClickToMove)
            {
                Lua.DoString("SetCVar('autoInteract', '1')");
            }
        }
        #endregion

        #region RuneCoolDown //Thanks wulf <3

        public static double GetRuneCooldown(int runeslot)
        {
            try
            {
                using (StyxWoW.Memory.AcquireFrame())
                {
                    var lua = String.Format("local x=select(1, GetRuneCooldown({0})); if x==nil then return 0 else return x-GetTime() end", runeslot);
                    var t = Double.Parse(Styx.WoWInternals.Lua.GetReturnValues(lua)[0]);
                    return Math.Abs(t);
                }
            }
            catch
            {
            //    Logger.FailLog(" Lua Failed in GetRuneCooldown");
                return 0;
            }
        }
        #endregion

        #region Other LUA
        internal static uint GetFps()
        {
            try
            {
                return (uint)Lua.GetReturnVal<float>("return GetFramerate()", 0);
            }
            catch
            {
            }
            return 0;
        }
        #endregion
    }
}