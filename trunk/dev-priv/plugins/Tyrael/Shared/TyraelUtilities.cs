using System.Timers;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tyrael.Shared
{
    public class TyraelUtilities
    {
        #region Hotkeys
        public static bool IsTyraelPaused { get; set; }

        public static void RegisterHotkeys()
        {
            HotkeysManager.Register("Tyrael Pause", TyraelSettings.Instance.PauseKeyChoice, TyraelSettings.Instance.ModKeyChoice, hk =>
            {
                IsTyraelPaused = !IsTyraelPaused;
                if (IsTyraelPaused)
                {
                    if (TyraelSettings.Instance.CheckChatOutput)
                    {
                        Lua.DoString(@"print('[Tyrael] Rotation \124cFFE61515 Paused!')");
                    }

                    if (TyraelSettings.Instance.CheckRaidWarningOutput)
                    {
                        Lua.DoString("RaidNotice_AddMessage(RaidWarningFrame, \"[Tyrael] Rotation Paused!\", ChatTypeInfo[\"RAID_WARNING\"]);");
                    }

                    Logging.Write(Colors.Red, "[Tyrael] Rotation Paused!");
                    TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond; Tyrael.IsPaused = true;
                }
                else
                {
                    if (TyraelSettings.Instance.CheckChatOutput)
                    {
                        Lua.DoString(@"print('[Tyrael] Rotation \124cFF15E61C Resumed!')");
                    }

                    if (TyraelSettings.Instance.CheckRaidWarningOutput)
                    {
                        Lua.DoString("RaidNotice_AddMessage(RaidWarningFrame, \"[Tyrael] Rotation Resumed!\", ChatTypeInfo[\"GUILD\"]);");
                    }

                    Logging.Write(Colors.LimeGreen, "[Tyrael] Rotation Resumed!");
                    TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond; Tyrael.IsPaused = false;
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
            if (!TyraelSettings.Instance.CheckClickToMove)
            {
                Lua.DoString("SetCVar('autoInteract', '0')");
            }

            if (TyraelSettings.Instance.CheckClickToMove)
            {
                Lua.DoString("SetCVar('autoInteract', '1')");
            }
        }

        public static void DisableClickToMove()
        {
            Lua.DoString("SetCVar('autoInteract', '0')");
        }

        public static void EnableClickToMove()
        {
            Lua.DoString("SetCVar('autoInteract', '1')");
        }
        #endregion

        #region Logging
        public static void WriteInfoToLogFile()
        {
            WriteFile("[Tyrael] Diagnostic Logging");
            LogSettings("[Tyrael] Settings", TyraelSettings.Instance);
        }

        public static void WriteFile(string message)
        {
            WriteFile(LogLevel.Verbose, message);
        }

        public static void WriteFile(string message, params object[] args)
        {
            WriteFile(LogLevel.Verbose, message, args);
        }

        public static void WriteFile(LogLevel ll, string message, params object[] args)
        {
            if (GlobalSettings.Instance.LogLevel >= LogLevel.Quiet)
                Logging.WriteToFileSync(ll, "[Tyrael] " + message, args);
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

        private static Timer _tyraelTimer;

        internal static void LogTimer(int tickingtime)
        {
            _tyraelTimer = new Timer(tickingtime);
            _tyraelTimer.Elapsed += OnTimedEvent;
            _tyraelTimer.AutoReset = false;
            _tyraelTimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            WriteInfoToLogFile();
        }
        #endregion

        #region Others
        public enum SvnUrl
        {
            Release,
            Development
        }

        public static bool IsViable(WoWObject wowObject)
        {
            return (wowObject != null) && wowObject.IsValid;
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
                        () => Logging.WriteDiagnostic("Tyrael: StatCounter has been updated!"));
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
