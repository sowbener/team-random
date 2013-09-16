using Styx;
using Styx.Common;
using Styx.TreeSharp;
using System;
using System.Diagnostics;
using System.Windows.Media;
using Action = Styx.TreeSharp.Action;
using G = YBMoP_BT_Rogue.Routines.YBGlobal;
using SG = YBMoP_BT_Rogue.Interfaces.Settings.YBSettingsG;

namespace YBMoP_BT_Rogue.Helpers
{
    static class YBLogger
    {
        #region YBMoP BT - Advanced Logging
        // 1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure
        // 1 traverse per xxx timespan ms
        internal static Composite AdvancedLogging
        {
            get
            {
                return new PrioritySelector(
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure,
                            new Action(delegate
                            {
                            //    AdvancedLogP("[Cooldown Time Logging]");
                            //    AdvancedLogW("Bloodbath CDTime: {0}", G.BBCD);
                            //    AdvancedLogW("Bloodthirst CD Time: {0}", G.BTCD);
                            //    AdvancedLogW("ColossusSmash CD Time: {0}", G.CSCD);
                            //    AdvancedLogW("Vanish CD Time: {0}", G.SBCD);
                            //    AdvancedLogW("SpellReflection CD Time: {0}", G.SRCD);
                             //   AdvancedLogW("ShadowBlades CD Time: {0}", G.RCCD);
                            //    AdvancedLogP(" ");
                                return RunStatus.Success;
                            }
                            )),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure,
                            new Action(delegate
                            {
                             //   AdvancedLogP("[On Cooldown Logging]");
                              //  AdvancedLogW("Shadowstep on CD: {0}", G.DBCDOC);
                            //    AdvancedLogW("HeroicLeap on CD: {0}", G.HLCDOC);
                             //   AdvancedLogW("ImpVic on CD: {0}", G.IVCDOC);
                             //   AdvancedLogW("Pummel on CD: {0}", G.PUCDOC);
                            //    AdvancedLogW("VicRush on CD: {0}", G.VRCDOC);
                                AdvancedLogP(" ");
                                return RunStatus.Success;
                            }
                            )),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure,
                            new Action(delegate
                            {
                                AdvancedLogP("[Fading Aura Logging]");
                              //  AdvancedLogW("CS Fading: {0}", G.ColossusSmashAuraT);
                              //  AdvancedLogW("RB Fading: {0}", G.RagingBlowAuraT);
                                AdvancedLogP(" ");
                                return RunStatus.Success;
                            })),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure,
                            new Action(delegate
                            {
                                AdvancedLogP("[Other Logging]");
                          //      AdvancedLogW("Has IS Glyph: {0}", G.ISGlyph);
                                AdvancedLogP(" ");
                                return RunStatus.Success;
                            })));
            }
        }
        #endregion

        #region YBMoP BT - Log Messages
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

        #region YBMoP BT - Log TreePerformanceTimer
        private static readonly Stopwatch TreePerformanceTimer = new Stopwatch();

        internal static Composite TreePerformance(string obj)
        {
            return new Action(ret =>
            {
                if (!SG.Instance.CheckTreePerformance)
                {
                    return RunStatus.Failure;
                }

                if (TreePerformanceTimer.ElapsedMilliseconds > 0)
                {
                    DiagLogW("YBMoP Performance Timer: Elapsed Time to traverse {0}: {1} ms ({2} ms client lag)", obj, TreePerformanceTimer.ElapsedMilliseconds, RealLag.TotalMilliseconds);
                    TreePerformanceTimer.Stop();
                    TreePerformanceTimer.Reset();
                }
                TreePerformanceTimer.Start();
                    return RunStatus.Failure;
            });
        }

        private static TimeSpan RealLag
        {
            get { return TimeSpan.FromMilliseconds(StyxWoW.WoWClient.Latency); }
        }
        #endregion
         private static readonly CapacityQueue<string> LogQueue = new CapacityQueue<string>(5);

        static void Output(LogLevel level, Color color, string format, params object[] args)
        {
            if (LogQueue.Contains(string.Format(format, args))) return;
            LogQueue.Enqueue(string.Format(format, args));

            Logging.Write(level, color, string.Format("[{0}]: {1}", YBMain.YBName, format), args);
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

    }
}
