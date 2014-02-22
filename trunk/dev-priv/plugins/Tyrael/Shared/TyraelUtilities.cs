﻿using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
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
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetAsyncKeyState(int vkey);

        public static bool IsKeyAsyncDown(Keys key)
        {
            if (GetActiveWindow() != StyxWoW.Memory.Process.MainWindowHandle)
                return false;

            return key != Keys.None && (GetAsyncKeyState((int)key) & 0x8000) != 0;
        }

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
                        Lua.DoString("RaidNotice_AddMessage(RaidWarningFrame, \"[Tyrael] Rotation Paused!\", ChatTypeInfo[\"SKILL\"]);");
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

        internal static void DiagnosticLogging()
        {
            Logging.WriteDiagnostic(Colors.White, "------------------------------------------");
            Logging.WriteDiagnostic("[Tyrael] {0}", TreeRoot.Current);
        }
        #endregion
    }
}
