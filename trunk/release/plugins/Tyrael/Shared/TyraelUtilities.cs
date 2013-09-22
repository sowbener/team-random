using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using System;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tyrael.Shared
{
    public class TyraelUtilities
    {
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
                    Lua.DoString(@"print('[Tyrael] Rotation \124cFFE61515 Paused!')");
                    TreeRoot.TicksPerSecond = 5;
                }
                else
                {
                    Lua.DoString(@"print('[Tyrael] Rotation \124cFF15E61C Resumed!')");
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
