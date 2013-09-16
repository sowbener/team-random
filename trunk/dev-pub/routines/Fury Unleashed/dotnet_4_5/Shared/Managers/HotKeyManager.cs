using System.Runtime.InteropServices;
using System.Windows.Forms;
using FuryUnleashed.Interfaces.Settings;
using FuryUnleashed.Routines;
using FuryUnleashed.Shared.Helpers;
using Styx.Common;
using Styx.WoWInternals;

namespace FuryUnleashed.Shared.Managers
{
    internal static class HotKeyManager
    {
        public static bool IsPaused { get; private set; }
        public static bool IsCooldown { get; private set; }
        public static bool IsAoe { get; private set; }
        public static bool IsSpecial { get; private set; }

        public delegate T Selection<out T>(object context);

        private static void LogKey(string kType, Keys kValue, ModifierKeys kModifier, bool kResult)
        {
            Logger.DiagLogW("{0}-key ({1} + {2}) pressed, set to: {3}", kType, kValue, kModifier, kResult);
        }

        /* Keystates - One press with Spell Queueing */
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);
        public static bool IsKeyAsyncDown(Keys key)
        {
            return (GetAsyncKeyState(key)) != 0;
        }

        /* Keystates - One press without Spell Queueing */
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int keyCode);
        private static Enum.KeyStates GetKeyState(Keys key)
        {
            Enum.KeyStates state = Enum.KeyStates.None;

            short retVal = GetKeyState((int)key);
            if ((retVal & 0x8000) == 0x8000)
                state |= Enum.KeyStates.Down;

            return state;
        }

        public static bool IsKeyDown(Keys key)
        {
            return Enum.KeyStates.Down == (GetKeyState(key) & Enum.KeyStates.Down);
        }

        public static void RegisterKeys()
        {
            if (SettingsH.Instance.ModeSelection == Enum.Mode.Hotkey || SettingsH.Instance.ModeSelection == Enum.Mode.SemiHotkey)
            {
                HotkeysManager.Register("Pause", SettingsH.Instance.PauseKeyChoice, SettingsH.Instance.ModKeyChoice, hk =>
                {
                    IsPaused = !IsPaused;
                    LogKey("Pause", SettingsH.Instance.PauseKeyChoice, SettingsH.Instance.ModKeyChoice, IsPaused);
                    if (InternalSettings.Instance.General.CheckHotkeyChatOutput)
                        Lua.DoString(IsPaused
                            ? @"print('Rotation \124cFFE61515 Paused!')"
                            : @"print('Rotation \124cFF15E61C Resumed!')");
                });

                HotkeysManager.Register("Cooldown", SettingsH.Instance.CooldownKeyChoice, SettingsH.Instance.ModKeyChoice, hk =>
                {
                    IsCooldown = !IsCooldown;
                    LogKey("Cooldown", SettingsH.Instance.CooldownKeyChoice, SettingsH.Instance.ModKeyChoice, IsCooldown);
                    if (InternalSettings.Instance.General.CheckHotkeyChatOutput && !IsPaused)
                        Lua.DoString(IsCooldown
                            ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                            : @"print('Cooldowns \124cFFE61515 Disabled!')");
                });

                HotkeysManager.Register("AoE", SettingsH.Instance.MultiTgtKeyChoice, SettingsH.Instance.ModKeyChoice, hk =>
                {
                    IsAoe = !IsAoe;
                    LogKey("AoE", SettingsH.Instance.MultiTgtKeyChoice, SettingsH.Instance.ModKeyChoice, IsAoe);
                    if (InternalSettings.Instance.General.CheckHotkeyChatOutput && !IsPaused)
                        Lua.DoString(IsAoe
                            ? @"print('Aoe \124cFF15E61C Enabled!')"
                            : @"print('Aoe \124cFFE61515 Disabled!')");
                });

                HotkeysManager.Register("Special", SettingsH.Instance.SpecialKeyChoice, SettingsH.Instance.ModKeyChoice, hk =>
                {
                    IsSpecial = !IsSpecial;
                    LogKey("Special", SettingsH.Instance.SpecialKeyChoice, SettingsH.Instance.ModKeyChoice, IsSpecial);
                    if (InternalSettings.Instance.General.CheckHotkeyChatOutput && !IsPaused && (FuGlobal.IsArmsSpec || FuGlobal.IsFurySpec))
                        Lua.DoString(IsSpecial
                            ? @"print('Special \124cFF15E61C Enabled!')"
                            : @"print('Special \124cFFE61515 Disabled!')");
                    if (InternalSettings.Instance.General.CheckHotkeyChatOutput && !IsPaused && FuGlobal.IsProtSpec)
                        Lua.DoString(IsSpecial
                            ? @"print('Shield Barrier \124cFF15E61C Enabled!')"
                            : @"print('Shield Block \124cFF15E61C Enabled!')");
                });

                Logger.DiagLogP("Fury Unleashed: Hotkeys registered with the following values: {0} as Pause Key, {1} as Cooldown Key, {2} as AoE Key, {3} as the Special key and {4} as Modifier Key.",
                    SettingsH.Instance.PauseKeyChoice,
                    SettingsH.Instance.CooldownKeyChoice,
                    SettingsH.Instance.MultiTgtKeyChoice,
                    SettingsH.Instance.SpecialKeyChoice,
                    SettingsH.Instance.ModKeyChoice);
            }
        }

        public static void RemoveAllKeys()
        {
            HotkeysManager.Unregister("Pause");
            HotkeysManager.Unregister("Cooldown");
            HotkeysManager.Unregister("AoE");
            HotkeysManager.Unregister("Special");
            Logger.DiagLogW("Fury Unleashed: Hotkeys removed!");
        }
    }
}