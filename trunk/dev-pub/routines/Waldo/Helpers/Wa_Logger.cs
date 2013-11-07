using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Timers;
using System.Windows.Media;
using Logger = Waldo.Helpers.WaLogger;
using Action = Styx.TreeSharp.Action;
using G = Waldo.Routines.WaGlobal;
using SG = Waldo.Interfaces.Settings.WaSettings;
using SH = Waldo.Interfaces.Settings.WaSettingsH;
using System.Collections.Generic;
using Styx.WoWInternals;

namespace Waldo.Helpers
{
    static class WaLogger
    {

        #region LogTimer
        private static Timer _dvTimer;
        public static void LogTimer(int tickingtime)
        {
            _dvTimer = new Timer(tickingtime);
            _dvTimer.Elapsed += OnTimedEvent;
            _dvTimer.AutoReset = false;
            _dvTimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            WriteToLogFile();
        }
        #endregion

        public static void DumpAuraTables(WoWUnit target)
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                WaLogger.CombatLog("----------- Buffs ------------");
                using (var timer1 = new PerformanceTimer("Buffs"))
                {
                    timer1.Start();
                    foreach (KeyValuePair<string, WoWAura> aura in target.Buffs)
                    {
                        Logger.CombatLog("{0}", aura.Key);
                    }

                    Logger.CombatLog("Buffs: {0}ms", timer1.ElapsedMilliseconds);
                }

                Logger.CombatLog("----------- Debuffs ------------");
                using (var timer2 = new PerformanceTimer("Debuffs"))
                {
                    timer2.Start();
                    foreach (KeyValuePair<string, WoWAura> aura in target.Debuffs)
                    {
                        Logger.CombatLog("{0}", aura.Key);
                    }

                    Logger.CombatLog("Debuffs: {0}ms", timer2.ElapsedMilliseconds);
                }

                Logger.CombatLog("----------- ActiveAuras ------------");
                using (var timer3 = new PerformanceTimer("ActiveAuras"))
                {
                    timer3.Start();
                    foreach (KeyValuePair<string, WoWAura> aura in target.ActiveAuras)
                    {
                        Logger.CombatLog("{0}", aura.Key);
                    }

                    Logger.CombatLog("ActiveAuras: {0}ms", timer3.ElapsedMilliseconds);
                }

                Logger.CombatLog("----------- PassiveAuras ------------");
                using (var timer4 = new PerformanceTimer("PassiveAuras"))
                {
                    timer4.Start();
                    foreach (KeyValuePair<string, WoWAura> aura in target.PassiveAuras)
                    {
                        Logger.CombatLog("{0}", aura.Key);
                    }

                    Logger.CombatLog("PassiveAuras: {0}ms", timer4.ElapsedMilliseconds);
                }

                Logger.CombatLog("----------- Auras ------------");
                using (var timer5 = new PerformanceTimer("PassiveAuras"))
                {
                    timer5.Start();
                    foreach (KeyValuePair<string, WoWAura> aura in target.Auras)
                    {
                        Logger.CombatLog("{0}", aura.Key);
                    }

                    Logger.CombatLog("Auras: {0}ms", timer5.ElapsedMilliseconds);
                }
                Logger.CombatLog("----------- GetAllAuras ------------");
                using (var timer6 = new PerformanceTimer("GetAllAuras"))
                {
                    timer6.Start();
                    foreach (WoWAura aura in target.GetAllAuras())
                    {
                        Logger.CombatLog("{0}", aura.Name);
                    }

                    Logger.CombatLog("GetAllAuras: {0}ms", timer6.ElapsedMilliseconds);
                }
            }
        }

        #region AWaanced Logging
        // 1, TimeSpan.FromMilliseconds(SG.Instance.NumAWaLogThrottleTime), RunStatus.Failure
        // 1 traverse per xxx timespan ms
        internal static Composite AWaancedLogging
        {
            get
            {
                return new PrioritySelector(
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.General.LoggingThrottleNum), RunStatus.Failure,
                            new Action(delegate
                            {
                                AWaancedLogP("Cooldown Time Logging:");
                                AWaancedLogP(" ");
                                return RunStatus.Success;
                            }
                            )),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.General.LoggingThrottleNum), RunStatus.Failure,
                            new Action(delegate
                            {
                                AWaancedLogP("On Cooldown Logging:");
                                AWaancedLogP(" ");
                                return RunStatus.Success;
                            }
                            )),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.General.LoggingThrottleNum), RunStatus.Failure,
                            new Action(delegate
                            {
                                AWaancedLogP("[Fading Aura Logging]");
                                AWaancedLogP(" ");
                                return RunStatus.Success;
                            })),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.General.LoggingThrottleNum), RunStatus.Failure,
                            new Action(delegate
                            {
                                AWaancedLogP("[Other Logging]");
                             //   AWaancedLogW("Tier 14 2P: {0}", G.Tier14TwoPieceBonus);
                            //    AWaancedLogW("Tier 14 4P: {0}", G.Tier14FourPieceBonus);
                             //   AWaancedLogW("Tier 15 2P: {0}", G.Tier15TwoPieceBonus);
                            //    AWaancedLogW("Tier 15 4P: {0}", G.Tier15FourPieceBonus);
                            //    AWaancedLogW("AlmostDead {0} or DumpAllRage {1}", G.AlmostDead, G.DumpAllRage);
                                AWaancedLogP(" ");
                                return RunStatus.Success;
                            })));
            }
        }

        internal static void WriteToLogFile()
        {
            WriteFile("Waldo: Diagnostic Logging");
            WriteFile("");
            WriteFile("{0} is the used revision.", WaMain.Revision);
            WriteFile("Current race {0} with {1} as spec and level {2}.", StyxWoW.Me.Race, StyxWoW.Me.Specialization, StyxWoW.Me.Level);
            WriteFile("{0} is your faction", StyxWoW.Me.IsAlliance ? "Alliance" : "Horde");
            WriteFile("");
            WriteFile("{0:F1} days since Windows was started.", TimeSpan.FromMilliseconds(Environment.TickCount).TotalHours / 24.0);
            WriteFile("{0} FPS currently in WoW.", WaLua.GetFps());
            WriteFile("{0} ms of Latency in WoW.", StyxWoW.WoWClient.Latency);
            WriteFile("");
            WriteFile("{0} is the HB path.", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            WriteFile("");
            LogSettings("WaSettingsG - (General)", SG.Instance.General);
            LogSettings("WaSettingsG - (Hotkeys)", SH.Instance);
            if (StyxWoW.Me.Specialization == WoWSpec.ShamanEnhancement)
                LogSettings("WaSettingsA - (Assassination)", SG.Instance.Assassination);
            if (StyxWoW.Me.Specialization == WoWSpec.ShamanElemental)
                LogSettings("WaSettingsU - (Combat)", SG.Instance.Combat);
            WriteFile("");
            WriteFile("====== Talents ======");
          //  WriteFile("Juggernaut Talent: {0}", G.JNTalent);
         //   WriteFile("Double Time Talent: {0}", G.DTTalent);
         //   WriteFile("Warbringer Talent: {0}", G.WBTalent);
         //   WriteFile("Enraged Regeneration Talent: {0}", G.ERTalent);
        //    WriteFile("Second Wind Talent: {0}", G.SCTalent);
        //    WriteFile("Impending Victory Talent: {0}", G.IVTalent);
        //    WriteFile("Staggering Shout Talent: {0}", G.SSTalent);
        //    WriteFile("Piercing Howl Talent: {0}", G.PHTalent);
        //    WriteFile("Disrupting Shout Talent: {0}", G.DSTalent);
        //    WriteFile("Bladestorm Talent: {0}", G.BSTalent);
        //    WriteFile("Shockwave Talent: {0}", G.SWTalent);
        //    WriteFile("Dragon Roar Talent: {0}", G.DRTalent);
        //    WriteFile("Mass Spell Reflection Talent: {0}", G.MRTalent);
         //   WriteFile("Safeguard Talent: {0}", G.SGTalent);
         //   WriteFile("Vigilance Talent: {0}", G.VGTalent);
         //   WriteFile("Avatar Talent: {0}", G.AVTalent);
        //    WriteFile("Bloodbath Talent: {0}", G.BBTalent);
         //   WriteFile("Storm Bolt Talent: {0}", G.SBTalent);
            WriteFile("");
         //   WriteFile("====== Tier Bonuses ======");
        //    WriteFile("Tier 14 2P: {0}", G.Tier14TwoPieceBonus);
        //    WriteFile("Tier 14 4P: {0}", G.Tier14FourPieceBonus);
        //    WriteFile("Tier 15 2P: {0}", G.Tier15TwoPieceBonus);
        //    WriteFile("Tier 15 4P: {0}", G.Tier15FourPieceBonus);
        }
        #endregion

        #region Log Messages
        public static void WriteFileLog(string message, params object[] args)
        {
            if (message == null) return;
            WriteFile("{0}", String.Format(message, args));
        }

        public static void AWaancedLogP(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.MediumPurple, "{0}", String.Format(message, args));
        }

        public static void AWaancedLogW(string message, params object[] args)
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

        public static void CombatLogO(string message, params object[] args)
        {
            if (message == _lastCombatmsg) return;
            Logging.Write(Colors.Orange, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogG(string message, params object[] args)
        {
            if (message == _lastCombatmsg) return;
            Logging.Write(Colors.LimeGreen, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogP(string message, params object[] args)
        {
            if (message == _lastCombatmsg) return;
            Logging.Write(Colors.MediumPurple, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        private static string _lastCombatmsg;
        #endregion

        #region Tracking
        // Tracking usage - No personal info is stored publically!
        // ReSharper disable ObjectCreationAsStatement
        // ReSharper disable EmptyGeneralCatchClause

        internal static void StatCounter()
        {
            if (SG.Instance.General.CheckAllowUsageTracking && WaMain.WaName == "Waldo - IR " + WaMain.Revision)
            {
                try
                {
                    var statcounterDate = DateTime.Now.DayOfYear.ToString(CultureInfo.InvariantCulture);
                    var statcounterPath = string.Format("{0}\\Settings\\Waldo\\StatCounter\\", Utilities.AssemblyDirectory);

                    if (!File.Exists(Path.Combine(statcounterPath, statcounterDate)))
                    {
                        //src="http://c.statcounter.com/9163315/0/1452d0b5/1/"
                        new MemoryStream(new WebClient().DownloadData("http://c.statcounter.com/9163315/0/1452d0b5/1/"));
                        if (!Directory.Exists(statcounterPath)) { Directory.CreateDirectory(statcounterPath); }
                        File.Create(Path.Combine(statcounterPath, statcounterDate));
                        DiagLogW("Wa: StatCounter has been updated!");

                        var statcounterCleanup = Directory.GetFiles(statcounterPath);
                        foreach (string counterfile in statcounterCleanup)
                        {
                            var name = new FileInfo(counterfile).Name;
                            name = name.ToLower();
                            if (name != statcounterDate)
                            {
                                File.Delete(counterfile);
                            }
                        }
                    }
                }
                catch { } // Catch all, like 404's.
            }
        }
        // ReSharper restore ObjectCreationAsStatement
        // ReSharper restore EmptyGeneralCatchClause
        #endregion

        #region Log TreePerformanceTimer
        private static readonly Stopwatch TreePerformanceTimer = new Stopwatch();
        private static readonly Stopwatch CompositePerformanceTimer = new Stopwatch();

        /// <summary>  Usage: Spell.TreePerformance(true) within a composite. </summary>
        /// Example: new Decorator(ret => WaLogger.TreePerformance("InitializeElemental")),
        internal static Composite TreePerformance(string obj)
        {
            return new Action(ret =>
            {
                if (!SG.Instance.General.CheckTreePerformance)
                {
                    return RunStatus.Failure;
                }

                if (TreePerformanceTimer.ElapsedMilliseconds > 0)
                {
                    DiagLogW("Combat Unleased Performance Timer: Elapsed Time to traverse {0}: {1} ms ({2} ms client lag)", obj, TreePerformanceTimer.ElapsedMilliseconds, RealLag.TotalMilliseconds);
                    TreePerformanceTimer.Stop();
                    TreePerformanceTimer.Reset();
                }
                TreePerformanceTimer.Start();
                    return RunStatus.Failure;
            });
        }

        /// <summary>  Usage: Spell.CompositePerformance(Composite, "SomeComposite") within a composite. </summary>
        /// Example: WaLogger.CompositePerformance(RageDump(), "RageDump()"),
        /// private static Composite RageDump()
        /// {
        ///     return new PrioritySelector(
        ///         S.Cast("Heroic Strike", ret => G.DumpAllRage)
        /// );
        /// }
        internal static Composite CompositePerformance(Composite child, string name = "SomeComposite")
        {
            return new Sequence(
                new Action(delegate
                {
                    CompositePerformanceTimer.Reset();
                    CompositePerformanceTimer.Start();
                    return RunStatus.Success;
                }),
                child,
                new Action(delegate
                {
                    CompositePerformanceTimer.Stop();
                    DiagLogW("Combat Unleased Composite Timer: {0} took {1} ms", name,
                                   CompositePerformanceTimer.ElapsedMilliseconds);
                    return RunStatus.Success;
                })
                );
        }

        private static TimeSpan RealLag
        {
            get { return TimeSpan.FromMilliseconds(StyxWoW.WoWClient.Latency); }
        }
        #endregion

        #region Other Logging
        // Tnx Singular!
        public static void WriteFile(string message)
        {
            WriteFile(LogLevel.Verbose, message);
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        public static void WriteFile(string message, params object[] args)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            WriteFile(LogLevel.Verbose, message, args);
        }

        public static void WriteFile(LogLevel ll, string message, params object[] args)
        {
            if (GlobalSettings.Instance.LogLevel >= LogLevel.Quiet)
                Logging.WriteToFileSync(ll, "Waldo: " + message, args);
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
         #endregion

       #region Logging
        private static readonly CapacityQueue<string> LogQueue = new CapacityQueue<string>(5);

        static void Output(LogLevel level, Color color, string format, params object[] args)
        {
            if (LogQueue.Contains(string.Format(format, args))) return;
            LogQueue.Enqueue(string.Format(format, args));

            Logging.Write(level, color, string.Format("[{0}]: {1}", Waldo.WaMain.WaName, format), args);
        }

        public static void InitLog(string format, params object[] args)
        { Output(LogLevel.Normal, Colors.Gainsboro, format, args); }

        public static void ItemLog(string format, params object[] args)
        { Output(LogLevel.Diagnostic, Colors.LawnGreen, format, args); }

        public static void FailLog(string format, params object[] args)
        { Output(LogLevel.Normal, Colors.GreenYellow, format, args); }

        public static void InfoLog(string format, params object[] args)
        { Output(LogLevel.Normal, Colors.CornflowerBlue, format, args); }

        public static void CombatLog(string format, params object[] args)
        { Output(LogLevel.Diagnostic, Colors.Pink, format, args); }

        public static void DebugLog(string format, params object[] args)
        { Output(LogLevel.Diagnostic, Colors.DarkGoldenrod, format, args); }

#endregion

    }
}
