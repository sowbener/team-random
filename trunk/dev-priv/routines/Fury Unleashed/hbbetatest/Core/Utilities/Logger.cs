﻿using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Interfaces.Settings;
using FuryUnleashed.Rotations;
using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Action = Styx.TreeSharp.Action;

namespace FuryUnleashed.Core.Utilities
{
    static class Logger
    {
        private static string _lastCombatmsg;

        // Combatlogging.
        public static void CombatLogFb(string message, params object[] args)
        {
            if (message == _lastCombatmsg && !message.Contains("Execute")) return; 
            Logging.Write(Colors.Firebrick, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogLg(string message, params object[] args)
        {
            if (message == _lastCombatmsg && !message.Contains("Execute")) return; 
            Logging.Write(Colors.LimeGreen, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogOr(string message, params object[] args)
        {
            if (message == _lastCombatmsg && !message.Contains("Execute")) return; 
            Logging.Write(Colors.Orange, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogPu(string message, params object[] args)
        {
            if (message == _lastCombatmsg && !message.Contains("Execute")) return; 
            Logging.Write(Colors.MediumPurple, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogWh(string message, params object[] args)
        {
            if (message == _lastCombatmsg && !message.Contains("Execute")) return; 
            Logging.Write(Colors.White, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        // Debuglogging.
        public static void DiagLogFb(string message, params object[] args)
        {
            if (message == null) return;
            Logging.WriteDiagnostic(Colors.Firebrick, "{0}", String.Format(message, args));
        }

        public static void DiagLogLg(string message, params object[] args)
        {
            if (message == null) return;
            Logging.WriteDiagnostic(Colors.LimeGreen, "{0}", String.Format(message, args));
        }

        public static void DiagLogOr(string message, params object[] args)
        {
            if (message == null) return;
            Logging.WriteDiagnostic(Colors.Orange, "{0}", String.Format(message, args));
        }

        public static void DiagLogPu(string message, params object[] args)
        {
            if (message == null) return;
            Logging.WriteDiagnostic(Colors.MediumPurple, "{0}", String.Format(message, args));
        }

        public static void DiagLogWh(string message, params object[] args)
        {
            if (message == null) return;
            Logging.WriteDiagnostic(Colors.White, "{0}", String.Format(message, args));
        }

        // Specific Loggers
        public static void CooldownTrackerLog(string message, params object[] args)
        {
            if (message == null || InternalSettings.Instance.General.CheckCooldownTrackerLogging == false) return;
            Logging.WriteDiagnostic(Colors.Crimson, "{0}", String.Format(message, args));
        }

        //public static void AdvancedLogP(string message, params object[] args)
        //{
        //    if (message == null) return;
        //    Logging.Write(Colors.MediumPurple, "{0}", String.Format(message, args));
        //}

        //public static void AdvancedLogW(string message, params object[] args)
        //{
        //    if (message == null) return;
        //    Logging.Write(Colors.White, "{0}", String.Format(message, args));
        //}

        //public static void InitLogF(string message, params object[] args)
        //{
        //    if (message == null) return;
        //    Logging.Write(Colors.Firebrick, "{0}", String.Format(message, args));
        //}

        //public static void InitLogO(string message, params object[] args)
        //{
        //    if (message == null) return;
        //    Logging.Write(Colors.Orange, "{0}", String.Format(message, args));
        //}

        //public static void InitLogW(string message, params object[] args)
        //{
        //    if (message == null) return;
        //    Logging.Write(Colors.White, "{0}", String.Format(message, args));
        //}

        //public static void DiagLogW(string message, params object[] args)
        //{
        //    if (message == null) return;
        //    Logging.WriteDiagnostic(Colors.White, "{0}", String.Format(message, args));
        //}

        //public static void DiagLogP(string message, params object[] args)
        //{
        //    if (message == null) return;
        //    Logging.WriteDiagnostic(Colors.MediumPurple, "{0}", String.Format(message, args));
        //}

        //private static string _lastCombatmsg;

        //public static void CombatLogO(string message, params object[] args)
        //{
        //    if (message == _lastCombatmsg && !message.Contains("Execute")) return;
        //    Logging.Write(Colors.Orange, "{0}", String.Format(message, args));
        //    _lastCombatmsg = message;
        //}

        //public static void CombatLogF(string message, params object[] args)
        //{
        //    if (message == _lastCombatmsg && !message.Contains("Execute")) return;
        //    Logging.Write(Colors.Firebrick, "{0}", String.Format(message, args));
        //    _lastCombatmsg = message;
        //}

        //public static void CombatLogG(string message, params object[] args)
        //{
        //    if (message == _lastCombatmsg && !message.Contains("Execute")) return;
        //    Logging.Write(Colors.LimeGreen, "{0}", String.Format(message, args));
        //    _lastCombatmsg = message;
        //}

        //public static void CombatLogP(string message, params object[] args)
        //{
        //    if (message == _lastCombatmsg && !message.Contains("Execute")) return;
        //    Logging.Write(Colors.MediumPurple, "{0}", String.Format(message, args));
        //    _lastCombatmsg = message;
        //}

        public static void WriteFileLog(string message, params object[] args)
        {
            if (message == null) return;
            WriteFile("{0}", String.Format(message, args));
        }

        public static void WriteInfoToLogFile()
        {
            WriteFile("Fury Unleashed: Diagnostic Logging");
            WriteFile("");
            WriteFile("{0} is the used revision.", Root.Revision);
            WriteFile("Current race {0} with {1} as spec and level {2}.", StyxWoW.Me.Race, StyxWoW.Me.Specialization.ToString().CamelToSpaced(), StyxWoW.Me.Level);
            WriteFile("{0} is your faction", StyxWoW.Me.IsAlliance ? "Alliance" : "Horde");
            WriteFile("");
            WriteFile("{0:F1} days since Windows was started.", TimeSpan.FromMilliseconds(Environment.TickCount).TotalHours / 24.0);
            WriteFile("{0} FPS currently in WoW.", LuaClass.GetFps());
            WriteFile("{0} ms of Latency in WoW.", StyxWoW.WoWClient.Latency);
            WriteFile("");
            WriteFile("{0} is the HB path.", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            WriteFile("");
            LogSettings("General Settings (SettingsG)", InternalSettings.Instance.General);
            LogSettings("Hotkey Settings (SettingsH)", SettingsH.Instance);
            if (Global.IsArmsSpec)
                LogSettings("Arms Settings (SettingsA)", InternalSettings.Instance.Arms);
            if (Global.IsFurySpec)
                LogSettings("Fury Settings (SettingsF)", InternalSettings.Instance.Fury);
            if (Global.IsProtSpec)
                LogSettings("Protection Settings (SettingsP)", InternalSettings.Instance.Protection);
            WriteFile("");
            WriteFile("====== Talents ======");
            WriteFile("Juggernaut Talent: {0}", Global.JnTalent);
            WriteFile("Double Time Talent: {0}", Global.DtTalent);
            WriteFile("Warbringer Talent: {0}", Global.WbTalent);
            WriteFile("Enraged Regeneration Talent: {0}", Global.ErTalent);
            WriteFile("Second Wind Talent: {0}", Global.ScTalent);
            WriteFile("Impending Victory Talent: {0}", Global.IvTalent);
            WriteFile("Staggering Shout Talent: {0}", Global.SsTalent);
            WriteFile("Piercing Howl Talent: {0}", Global.PhTalent);
            WriteFile("Disrupting Shout Talent: {0}", Global.DsTalent);
            WriteFile("Bladestorm Talent: {0}", Global.BsTalent);
            WriteFile("Shockwave Talent: {0}", Global.SwTalent);
            WriteFile("Dragon Roar Talent: {0}", Global.DrTalent);
            WriteFile("Mass Spell Reflection Talent: {0}", Global.MrTalent);
            WriteFile("Safeguard Talent: {0}", Global.SgTalent);
            WriteFile("Vigilance Talent: {0}", Global.VgTalent);
            WriteFile("Avatar Talent: {0}", Global.AvTalent);
            WriteFile("Bloodbath Talent: {0}", Global.BbTalent);
            WriteFile("Storm Bolt Talent: {0}", Global.SbTalent);
            WriteFile("");
            WriteFile("====== Tier Bonuses ======");
            WriteFile("Tier 15 DPS 2P: {0}", Global.Tier15TwoPieceBonus);
            WriteFile("Tier 15 DPS 4P: {0}", Global.SkullBannerAuraT15);
            WriteFile("Tier 15 Prot 2P: {0}", Global.Tier15TwoPieceBonusT);
            WriteFile("Tier 15 Prot 4P: {0}", Global.Tier15FourPieceBonusT);
            WriteFile("Tier 16 DPS 2P: {0}", Global.Tier16TwoPieceBonus);
            WriteFile("Tier 16 DPS 4P: {0}", Global.DeathSentenceAuraT16);
            WriteFile("Tier 16 Prot 2P: {0}", Global.Tier16TwoPieceBonusT);
            WriteFile("Tier 16 Prot 4P: {0}", Global.Tier16FourPieceBonusT);
            WriteFile("");
            WriteFile("======= Other Info =======");
            WriteFile("2H Weapons: {0}", Global.WieldsTwoHandedWeapons);
        }

        private static Timer _fuTimer;

        public static void LogTimer(int tickingtime)
        {
            _fuTimer = new Timer(tickingtime);
            _fuTimer.Elapsed += OnTimedEvent;
            _fuTimer.AutoReset = false;
            _fuTimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            WriteInfoToLogFile();
        }

        internal static void StatCounter()
        {
            try
            {
                var statcounterDate = DateTime.Now.DayOfYear.ToString(CultureInfo.InvariantCulture);
                if (!statcounterDate.Equals(InternalSettings.Instance.General.LastStatCounted))
                {
                    Parallel.Invoke(
                        () => new WebClient().DownloadData("http://c.statcounter.com/9163286/0/396c7d29/1/"),
                        () => DiagLogWh("FU: StatCounter has been updated!"));
                    InternalSettings.Instance.General.LastStatCounted = statcounterDate;
                    InternalSettings.Instance.Save();
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { /* Catch all errors */ }
        }


        public static void WriteFile(string message)
        {
            WriteFile(LogLevel.Verbose, message);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static void WriteFile(string message, params object[] args)
        {
            WriteFile(LogLevel.Verbose, message, args);
        }

        public static void WriteFile(LogLevel ll, string message, params object[] args)
        {
            if (GlobalSettings.Instance.LogLevel >= LogLevel.Quiet)
                Logging.WriteToFileSync(ll, "Fury Unleashed: " + message, args);
        }

        public static void LogSettings(string desc, Settings set)
        {
            if (set == null)
                return;

            WriteFile("====== {0} ======", desc);
            foreach (var kvp in set.GetSettings())
            {
                WriteFile("  {0}: {1}", kvp.Key, kvp.Value.ToString());
            }
            WriteFile("");
        }

        public static string CamelToSpaced(this string str)
        {
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsUpper(c))
                {
                    sb.Append(' ');
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        #region Advanced Logging
        // 1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure
        // 1 traverse per xxx timespan ms
        //internal static Composite AdvancedLogging
        //{
        //    get
        //    {
        //        return new PrioritySelector(
        //            //Cached Units Logging
        //            new Decorator(ret => InternalSettings.Instance.General.CheckUnitLogging,
        //                new PrioritySelector(
        //                    new ThrottlePasses(1,
        //                        TimeSpan.FromMilliseconds(InternalSettings.Instance.General.LoggingThrottleNum),
        //                        RunStatus.Failure,
        //                        new Action(delegate
        //                        {
        //                            CombatLogPu("Cached Unit Counts:");
        //                            CombatLogWh("Units - In Range (2Y - SlamCleave): {0}",Unit.NearbySlamCleaveUnitsCount);
        //                            CombatLogWh("Units - In Range (5Y - Melee): {0}", Unit.AttackableMeleeUnitsCount);
        //                            CombatLogWh("Units - In Range (8Y - AoE): {0}", Unit.NearbyAttackableUnitsCount);
        //                            CombatLogWh("Units - Interrupts (10Y): {0}", Unit.InterruptableUnitsCount);
        //                            CombatLogWh("Units - Rallying Cry (30Y): {0}", Unit.RaidMembersNeedCryCount);
        //                            CombatLogWh("Units - Deep Wounds (8Y): {0}", Unit.NeedThunderclapUnitsCount);
        //                            CombatLogPu("Units - Slam Viable: {0}", Global.SlamViable);
        //                            CombatLogPu("Units - Whirlwind viable: {0}", Global.WhirlwindViable);
        //                        }
        //                        )))),
        //            // Aura & CD logging.
        //            new Decorator(ret => InternalSettings.Instance.General.CheckCacheLogging,
        //                new PrioritySelector(
        //                    new ThrottlePasses(1,
        //                        TimeSpan.FromMilliseconds(InternalSettings.Instance.General.LoggingThrottleNum),
        //                        RunStatus.Failure,
        //                        new Action(delegate
        //                        {
        //                            // ReSharper disable InconsistentNaming
        //                            CombatLogPu("Cooldowns:");
        //                            foreach (var WoWAura in CooldownTracker.cooldowns)
        //                            {
        //                                CombatLogWh("{0}", WoWAura);
        //                            }
        //                            //CombatLogPu("Self Aura's:");
        //                            //foreach (var WoWAura in Spell.CachedAuras)
        //                            //{
        //                            //    CombatLogWh("{0}", WoWAura);
        //                            //}
        //                        }
        //                        )))),
        //            //Temporary Functions Logging
        //            new Decorator(ret => InternalSettings.Instance.General.CheckTestLogging,
        //                new PrioritySelector(
        //                    new ThrottlePasses(1,
        //                        TimeSpan.FromMilliseconds(InternalSettings.Instance.General.LoggingThrottleNum),
        //                        RunStatus.Failure,
        //                        new Action(delegate
        //                        {
        //                            CombatLogPu("Test Logging:");
        //                            CombatLogWh("BT OC {0}", Global.BtOc);
        //                            CombatLogWh("Slam Cost: {0}", WoWSpell.FromId(SpellBook.Slam).PowerCost);
        //                            CombatLogWh("Whirlwind Cost: {0}", WoWSpell.FromId(SpellBook.Whirlwind).PowerCost);
        //                        }
        //                        )))));
        //    }
        //}
        #endregion

    }
}