using FuryUnleashed.Core;
using FuryUnleashed.Interfaces.Settings;
using FuryUnleashed.Routines;
using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.TreeSharp;
using Styx.WoWInternals;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using Action = Styx.TreeSharp.Action;

namespace FuryUnleashed.Shared.Helpers
{
    static class Logger
    {
        public static void WriteFileLog(string message, params object[] args)
        {
            if (message == null) return;
            WriteFile("{0}", String.Format(message, args));
        }

        public static void AdvancedLogP(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.MediumPurple, "{0}", String.Format(message, args));
        }

        public static void AdvancedLogW(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.White, "{0}", String.Format(message, args));
        }

        public static void InitLogF(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.Firebrick, "{0}", String.Format(message, args));
        }

        public static void InitLogO(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.Orange, "{0}", String.Format(message, args));
        }

        public static void InitLogW(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.White, "{0}", String.Format(message, args));
        }

        public static void DiagLogW(string message, params object[] args)
        {
            if (message == null) return;
            Logging.WriteDiagnostic(Colors.White, "{0}", String.Format(message, args));
        }

        public static void DiagLogP(string message, params object[] args)
        {
            if (message == null) return;
            Logging.WriteDiagnostic(Colors.MediumPurple, "{0}", String.Format(message, args));
        }

        private static string _lastCombatmsg;

        public static void CombatLogO(string message, params object[] args)
        {
            if (message == _lastCombatmsg && !message.Contains("Execute")) return;
            Logging.Write(Colors.Orange, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogF(string message, params object[] args)
        {
            if (message == _lastCombatmsg && !message.Contains("Execute")) return;
            Logging.Write(Colors.Firebrick, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogG(string message, params object[] args)
        {
            if (message == _lastCombatmsg && !message.Contains("Execute")) return;
            Logging.Write(Colors.LimeGreen, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogP(string message, params object[] args)
        {
            if (message == _lastCombatmsg && !message.Contains("Execute")) return;
            Logging.Write(Colors.MediumPurple, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
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
            if (FuGlobal.IsArmsSpec)
                LogSettings("Arms Settings (SettingsA)", InternalSettings.Instance.Arms);
            if (FuGlobal.IsFurySpec)
                LogSettings("Fury Settings (SettingsF)", InternalSettings.Instance.Fury);
            if (FuGlobal.IsProtSpec)
                LogSettings("Protection Settings (SettingsP)", InternalSettings.Instance.Protection);
            WriteFile("");
            WriteFile("====== Talents ======");
            WriteFile("Juggernaut Talent: {0}", FuGlobal.JnTalent);
            WriteFile("Double Time Talent: {0}", FuGlobal.DtTalent);
            WriteFile("Warbringer Talent: {0}", FuGlobal.WbTalent);
            WriteFile("Enraged Regeneration Talent: {0}", FuGlobal.ErTalent);
            WriteFile("Second Wind Talent: {0}", FuGlobal.ScTalent);
            WriteFile("Impending Victory Talent: {0}", FuGlobal.IvTalent);
            WriteFile("Staggering Shout Talent: {0}", FuGlobal.SsTalent);
            WriteFile("Piercing Howl Talent: {0}", FuGlobal.PhTalent);
            WriteFile("Disrupting Shout Talent: {0}", FuGlobal.DsTalent);
            WriteFile("Bladestorm Talent: {0}", FuGlobal.BsTalent);
            WriteFile("Shockwave Talent: {0}", FuGlobal.SwTalent);
            WriteFile("Dragon Roar Talent: {0}", FuGlobal.DrTalent);
            WriteFile("Mass Spell Reflection Talent: {0}", FuGlobal.MrTalent);
            WriteFile("Safeguard Talent: {0}", FuGlobal.SgTalent);
            WriteFile("Vigilance Talent: {0}", FuGlobal.VgTalent);
            WriteFile("Avatar Talent: {0}", FuGlobal.AvTalent);
            WriteFile("Bloodbath Talent: {0}", FuGlobal.BbTalent);
            WriteFile("Storm Bolt Talent: {0}", FuGlobal.SbTalent);
            WriteFile("");
            WriteFile("====== Tier Bonuses ======");
            WriteFile("Tier 15 DPS 2P: {0}", FuGlobal.Tier15TwoPieceBonus);
            WriteFile("Tier 15 DPS 4P: {0}", FuGlobal.Tier15FourPieceBonus);
            WriteFile("Tier 15 Prot 2P: {0}", FuGlobal.Tier15TwoPieceBonusT);
            WriteFile("Tier 15 Prot 4P: {0}", FuGlobal.Tier15FourPieceBonusT);
            WriteFile("Tier 16 DPS 2P: {0}", FuGlobal.Tier16TwoPieceBonus);
            WriteFile("Tier 16 DPS 4P: {0}", FuGlobal.Tier16FourPieceBonus);
            WriteFile("Tier 16 Prot 2P: {0}", FuGlobal.Tier16TwoPieceBonusT);
            WriteFile("Tier 16 Prot 4P: {0}", FuGlobal.Tier16FourPieceBonusT);
            WriteFile("");
            WriteFile("======= Other Info =======");
            WriteFile("2H Weapons: {0}", FuGlobal.WieldsTwoHandedWeapons);
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
                        () => DiagLogW("FU: StatCounter has been updated!"));
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
        internal static Composite AdvancedLogging
        {
            get
            {
                return new PrioritySelector(
                    //Cached Units Logging
                    new Decorator(ret => InternalSettings.Instance.General.CheckUnitLogging,
                        new PrioritySelector(
                            new ThrottlePasses(1,
                                TimeSpan.FromMilliseconds(InternalSettings.Instance.General.LoggingThrottleNum),
                                RunStatus.Failure,
                                new Action(delegate
                                {
                                    AdvancedLogP("Cached Unit Counts:");
                                    AdvancedLogW("Units - In Range (2Y - SlamCleave): {0}",
                                        Unit.NearbySlamCleaveUnitsCount);
                                    AdvancedLogW("Units - In Range (5Y - Melee): {0}", Unit.AttackableMeleeUnitsCount);
                                    AdvancedLogW("Units - In Range (8Y - AoE): {0}", Unit.NearbyAttackableUnitsCount);
                                    AdvancedLogW("Units - Interrupts (10Y): {0}", Unit.InterruptableUnitsCount);
                                    AdvancedLogW("Units - Rallying Cry (30Y): {0}", Unit.RaidMembersNeedCryCount);
                                    AdvancedLogW("Units - Deep Wounds (8Y): {0}", Unit.NeedThunderclapUnitsCount);
                                    AdvancedLogP("Units - Slam Viable: {0}", FuGlobal.SlamViable);
                                    AdvancedLogP("Units - Whirlwind viable: {0}", FuGlobal.WhirlwindViable);
                                }
                                    )))),
                    // Cached Aura's Logging
                    new Decorator(ret => InternalSettings.Instance.General.CheckCacheLogging,
                        new PrioritySelector(
                            new ThrottlePasses(1,
                                TimeSpan.FromMilliseconds(InternalSettings.Instance.General.LoggingThrottleNum),
                                RunStatus.Failure,
                                new Action(delegate
                                {
                                    // ReSharper disable InconsistentNaming
                                    AdvancedLogP("Cached Target Aura's:");
                                    foreach (var WoWAura in Spell.CachedTargetAuras)
                                    {
                                        AdvancedLogW("{0}", WoWAura);
                                    }
                                    AdvancedLogP("Cached Self Aura's:");
                                    foreach (var WoWAura in Spell.CachedAuras)
                                    {
                                        AdvancedLogW("{0}", WoWAura);
                                    }
                                }
                                    )))),
                    //Temporary Functions Logging
                    new Decorator(ret => InternalSettings.Instance.General.CheckTestLogging,
                        new PrioritySelector(
                            new ThrottlePasses(1,
                                TimeSpan.FromMilliseconds(InternalSettings.Instance.General.LoggingThrottleNum),
                                RunStatus.Failure,
                                new Action(delegate
                                {
                                    AdvancedLogP("Test Logging:");
                                    AdvancedLogW("Slam Cost: {0}", WoWSpell.FromId(SpellBook.Slam).PowerCost);
                                    AdvancedLogW("Whirlwind Cost: {0}", WoWSpell.FromId(SpellBook.Whirlwind).PowerCost);
                                }
                                    )))));
            }
        }
        #endregion

    }
}
