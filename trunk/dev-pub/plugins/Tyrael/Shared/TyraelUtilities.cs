using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Media;
using Action = Styx.TreeSharp.Action;
using ScaleTimer = System.Timers.Timer;

namespace Tyrael.Shared
{
    public class TyraelUtilities
    {
        #region Performance Timer
        public static int CachedTreeRootTime = 0;
        public static bool TimerReady = true;

        private static readonly Stopwatch TreePerformanceTimer = new Stopwatch();
        private static ScaleTimer _timer;

        /* Usage: TreeSharp.Tree(true) within a composite. */
        internal static Composite Tree(bool enable)
        {
            return new Action(ret =>
            {
                if (TreePerformanceTimer.ElapsedMilliseconds > 0)
                {
                    var elapsed = (int)TreePerformanceTimer.ElapsedMilliseconds;
                    CachedTreeRootTime = elapsed;

                    if (IsKeyAsyncDown(TyraelSettings.Instance.TreePerformanceChoice))
                    {
                        Logging.Write(Colors.GreenYellow, "[TreePerformance] Elapsed Time to traverse: {0} ms ({1} ms client lag)", elapsed, Lag.TotalMilliseconds);
                    }

                    TreePerformanceTimer.Stop();
                    TreePerformanceTimer.Reset();
                }

                TreePerformanceTimer.Start();
                return RunStatus.Failure;
            });
        }

        private static TimeSpan Lag
        {
            get { return TimeSpan.FromMilliseconds((StyxWoW.WoWClient.Latency)); }
        }

        public static void Scaling()
        {
            if (!TyraelSettings.Instance.ScaleTps || TimerReady == false) 
                return;

            if (CachedTreeRootTime < 1)
            {
                Logging.WriteDiagnostic(Colors.DodgerBlue, "[Tyrael] CachedTreeRootTicks returns null ({0})", CachedTreeRootTime);
                return;
            }

            if (CachedTreeRootTime > 199)
            {
                TreeRoot.TicksPerSecond = 5;
            }

            TimerReady = false;
            var scaledTps = 1000/CachedTreeRootTime;
            TreeRoot.TicksPerSecond = (byte)scaledTps;

            if (IsKeyAsyncDown(TyraelSettings.Instance.TreePerformanceChoice))
            {
                Logging.WriteDiagnostic(Colors.GreenYellow, "TPS set to: {0} ({1} is CachedTreeRootTicks) - ({2} is TimerState)", TreeRoot.TicksPerSecond, CachedTreeRootTime, TimerReady);
            }

            ScaleTimer(TyraelSettings.Instance.ScaleRefresh);
        }

        public static void ScaleTimer(int tickingtime)
        {
            _timer = new ScaleTimer(tickingtime);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = false;
            _timer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            TimerReady = true;
        }
        #endregion

        #region Hotkeys
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);
        public static bool IsKeyAsyncDown(Keys key)
        {
            return (GetAsyncKeyState(key)) != 0;
        }

        public static bool IsTyraelPaused { get; set; }

        public static void RegisterHotkeys()
        {
            HotkeysManager.Register("Tyrael Pause", TyraelSettings.Instance.PauseKeyChoice, TyraelSettings.Instance.ModKeyChoice, hk =>
            {
                IsTyraelPaused = !IsTyraelPaused;
                if (IsTyraelPaused)
                {
                    Lua.DoString("print('[Tyrael] Rotation Paused!')");
                    TreeRoot.TicksPerSecond = 5;
                }
                else
                {
                    Lua.DoString("print('[Tyrael] Rotation Resumed!')");
                    TreeRoot.TicksPerSecond = (byte)TyraelSettings.Instance.HonorbuddyTps;
                }
            });
        }

        public static void RemoveHotkeys()
        {
            HotkeysManager.Unregister("Tyrael Pause");
        }

        public static void ReRegisterHotkeys()
        {
            RemoveHotkeys();
            RegisterHotkeys();
        }
        #endregion

        #region Click-To-Move (CTM)
        public static void ClickToMove()
        {
            Lua.DoString(TyraelSettings.Instance.ClickToMove
                ? "SetCVar('autoInteract', '1')"
                : "SetCVar('autoInteract', '0')");
        }
        #endregion

        #region Others
        public enum LockState
        {
            True,
            False
        }

        internal static void StatCounter()
        {
            try
            {
                var statcounterDate = DateTime.Now.DayOfYear.ToString(CultureInfo.InvariantCulture);
                if (!statcounterDate.Equals(TyraelSettings.Instance.LastStatCounted))
                {
                    Parallel.Invoke(
                        () => new WebClient().DownloadData("http://c.statcounter.com/9219924/0/e3fed179/1/"),
                        () => Logging.WriteDiagnostic("FU: StatCounter has been updated!"));
                    TyraelSettings.Instance.LastStatCounted = statcounterDate;
                    TyraelSettings.Instance.Save();
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { /* Catch all errors */ }
        }
        #endregion
    }
}
