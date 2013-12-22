using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.TreeSharp;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Timers;
using System.Windows.Media;
using Action = Styx.TreeSharp.Action;
using G = Bubbleman.Routines.BMGlobal;
using SG = Bubbleman.Interfaces.Settings.BMSettings;
using SH = Bubbleman.Interfaces.Settings.BMSettingsH;

namespace Bubbleman.Helpers
{
    static class BMLogger
    {
        #region LogTimer
        private static Timer _BMTimer;
        public static void LogTimer(int tickingtime)
        {
            _BMTimer = new Timer(tickingtime);
            _BMTimer.Elapsed += OnTimedEvent;
            _BMTimer.AutoReset = false;
            _BMTimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            WriteToLogFile();
        }

        public static void Performance(string format, params object[] args)
        {
            if (SG.Instance.General.PerformanceLogging == LogCategory.Performance)
                Write(LogLevel.Normal, Colors.HotPink, format, args);
        }

        private static void Write(LogLevel level, Color color, string format, params object[] args)
        {
            if (LogQueue.Contains(string.Format(format, args))) return;
            LogQueue.Enqueue(string.Format(format, args));

            Styx.Common.Logging.Write(level, color, string.Format("[{0}]: {1}", Bubbleman.BMMain.BMName, format), args);
        }
        #endregion

        #region ABManced Logging
        // 1, TimeSpan.FromMilliseconds(SG.Instance.NumABMLogThrottleTime), RunStatus.Failure
        // 1 traverse per xxx timespan ms
        internal static Composite ABMancedLogging
        {
            get
            {
                return new PrioritySelector(
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.General.LoggingThrottleNum), RunStatus.Failure,
                            new Action(delegate
                            {
                                ABMancedLogP("Cooldown Time Logging:");
                                ABMancedLogP(" ");
                                return RunStatus.Success;
                            }
                            )),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.General.LoggingThrottleNum), RunStatus.Failure,
                            new Action(delegate
                            {
                                ABMancedLogP("On Cooldown Logging:");
                                ABMancedLogP(" ");
                                return RunStatus.Success;
                            }
                            )),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.General.LoggingThrottleNum), RunStatus.Failure,
                            new Action(delegate
                            {
                                ABMancedLogP("[Fading Aura Logging]");
                                ABMancedLogP(" ");
                                return RunStatus.Success;
                            })),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.General.LoggingThrottleNum), RunStatus.Failure,
                            new Action(delegate
                            {
                                ABMancedLogP("[Other Logging]");
                             //   ABMancedLogW("Tier 14 2P: {0}", G.Tier14TwoPieceBonus);
                            //    ABMancedLogW("Tier 14 4P: {0}", G.Tier14FourPieceBonus);
                             //   ABMancedLogW("Tier 15 2P: {0}", G.Tier15TwoPieceBonus);
                            //    ABMancedLogW("Tier 15 4P: {0}", G.Tier15FourPieceBonus);
                            //    ABMancedLogW("AlmostDead {0} or DumpAllRage {1}", G.AlmostDead, G.DumpAllRage);
                                ABMancedLogP(" ");
                                return RunStatus.Success;
                            })));
            }
        }

        internal static void WriteToLogFile()
        {
            WriteFile("Bubbleman: Diagnostic Logging");
            WriteFile("");
            WriteFile("{0} is the used revision.", BMMain.Revision);
            WriteFile("Current race {0} with {1} as spec and level {2}.", StyxWoW.Me.Race, StyxWoW.Me.Specialization, StyxWoW.Me.Level);
            WriteFile("{0} is your faction", StyxWoW.Me.IsAlliance ? "Alliance" : "Horde");
            WriteFile("");
            WriteFile("{0:F1} days since Windows was started.", TimeSpan.FromMilliseconds(Environment.TickCount).TotalHours / 24.0);
            WriteFile("{0} FPS currently in WoW.", BMLua.GetFps());
            WriteFile("{0} ms of Latency in WoW.", StyxWoW.WoWClient.Latency);
            WriteFile("");
            WriteFile("{0} is the HB path.", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            WriteFile("");
            LogSettings("BMSettingsG - (General)", SG.Instance.General);
            LogSettings("BMSettingsG - (Hotkeys)", SH.Instance);
            if (StyxWoW.Me.Specialization == WoWSpec.PaladinProtection)
                LogSettings("BMSettingsA - (Protection)", SG.Instance.Protection);
            if (StyxWoW.Me.Specialization == WoWSpec.PaladinRetribution)
                LogSettings("BMSettingsU - (Retribution)", SG.Instance.Retribution);
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
        //    WriteFile("Bladestorm Talent: {0}", G.BMTalent);
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

        public static void ABMancedLogP(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.MediumPurple, "{0}", String.Format(message, args));
        }

        public static void ABMancedLogW(string message, params object[] args)
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
            if (SG.Instance.General.CheckAllowUsageTracking && BMMain.BMName == "Bubbleman - IR " + BMMain.Revision)
            {
                try
                {
                    var statcounterDate = DateTime.Now.DayOfYear.ToString(CultureInfo.InvariantCulture);
                    var statcounterPath = string.Format("{0}\\Settings\\Bubbleman\\StatCounter\\", Utilities.AssemblyDirectory);

                    if (!File.Exists(Path.Combine(statcounterPath, statcounterDate)))
                    {
                        //src="http://c.statcounter.com/9194670/0/acfba480/1/"
                        new MemoryStream(new WebClient().DownloadData("http://c.statcounter.com/9194670/0/acfba480/1/"));
                        if (!Directory.Exists(statcounterPath)) { Directory.CreateDirectory(statcounterPath); }
                        File.Create(Path.Combine(statcounterPath, statcounterDate));
                        DiagLogW("BM: StatCounter has been updated!");

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
        /// Example: new Decorator(ret => BMLogger.TreePerformance("InitializeUnholy")),
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
                    DiagLogW("Unholy Unleased Performance Timer: Elapsed Time to traverse {0}: {1} ms ({2} ms client lag)", obj, TreePerformanceTimer.ElapsedMilliseconds, RealLag.TotalMilliseconds);
                    TreePerformanceTimer.Stop();
                    TreePerformanceTimer.Reset();
                }
                TreePerformanceTimer.Start();
                    return RunStatus.Failure;
            });
        }

        /// <summary>  Usage: Spell.CompositePerformance(Composite, "SomeComposite") within a composite. </summary>
        /// Example: BMLogger.CompositePerformance(RageDump(), "RageDump()"),
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
                    DiagLogW("Unholy Unleased Composite Timer: {0} took {1} ms", name,
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
                Logging.WriteToFileSync(ll, "Bubbleman: " + message, args);
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

            Logging.Write(level, color, string.Format("[{0}]: {1}", Bubbleman.BMMain.BMName, format), args);
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
