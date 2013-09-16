using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using System.Windows.Forms;
using System.Windows.Media;

namespace Tyrael.Shared
{
    public class TyraelHotkeyManager
    {
        public static bool IsPaused { get; set; }

        private static void LogKey(string kType, Keys kValue, ModifierKeys kModifier, bool kResult)
        {
            Logging.WriteDiagnostic(Colors.DodgerBlue, "{0}-key ({1} + {2}) pressed, set to: {3}", kType, kValue, kModifier, kResult);
        }

        public static void RegisterHotKeys()
        {
            HotkeysManager.Register("TR-Pause", TyraelSettings.Instance.PauseKeyChoice, TyraelSettings.Instance.ModKeyChoice, hk =>
            {
                IsPaused = !IsPaused;
                LogKey("TR-Pause", TyraelSettings.Instance.PauseKeyChoice, TyraelSettings.Instance.ModKeyChoice, IsPaused);
                if (TyraelSettings.Instance.ChatOutput)
                {
                    if (IsPaused)
                    {
                        TreeRoot.TicksPerSecond = 5;
                        Lua.DoString(@"print('[Tyrael] Rotation \124cFFE61515 Paused!')");
                    }
                    else
                    {
                        TreeRoot.TicksPerSecond = (byte)TyraelSettings.Instance.HonorbuddyTps;
                        Lua.DoString(@"print('[Tyrael] Rotation \124cFF15E61C Resumed!')");
                    }
                }
            });

            Logging.Write(Colors.DodgerBlue, "[Tyrael] Hotkeys registered with {0} as PauseKey and {1} as ModifierKey.", TyraelSettings.Instance.PauseKeyChoice, TyraelSettings.Instance.ModKeyChoice);
        }

        public static void RemoveAllKeys()
        {
            HotkeysManager.Unregister("TR-Pause");
            Logging.Write(Colors.DodgerBlue, "[Tyrael] Hotkeys unregistered!");
        }
    }
}
