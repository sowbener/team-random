﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;
using Styx;
using Styx.Common;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Action = Styx.TreeSharp.Action;

namespace Tyrael.Shared
{
    public class TyraelUtilities
    {
        #region Performance Timer

        public static int CachedTreeRootTicks;
        private static readonly Stopwatch TreePerformanceTimer = new Stopwatch();

        /* Usage: TreeSharp.Tree(true) within a composite. */
        internal static Composite Tree(bool enable)
        {
            return new Action(ret =>
            {
                if (TreePerformanceTimer.ElapsedMilliseconds > 0)
                {
                    var elapsed = (int)TreePerformanceTimer.ElapsedMilliseconds;
                    CachedTreeRootTicks = elapsed;

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

        //private static readonly Stopwatch TreePerformanceTimer = new Stopwatch();
        //private static int _cachedTreeRoot;

        //internal static Composite Tree(bool enable)
        //{
        //    return new Action(ret =>
        //    {
        //        if (TreePerformanceTimer.ElapsedMilliseconds > 0)
        //        {
        //            var elap = (int)TreePerformanceTimer.ElapsedMilliseconds;
        //            _cachedTreeRoot = elap;
        //            //if (IsKeyAsyncDown(TyraelSettings.Instance.TreePerformanceChoice))
        //            Logging.Write(Colors.GreenYellow, @"[TreePerformance] Elapsed Time to traverse: {0} ms ({1} ms client lag)", elap, Lag.TotalMilliseconds);

        //            TreePerformanceTimer.Stop();
        //            TreePerformanceTimer.Reset();
        //        }

        //        TreePerformanceTimer.Start();

        //        return RunStatus.Failure;
        //    });
        //}
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
