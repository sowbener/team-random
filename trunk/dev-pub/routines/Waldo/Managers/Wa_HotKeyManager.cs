using Waldo.Helpers;
using Waldo.Interfaces.Settings;
using Styx.Common;
using Styx.CommonBot.Routines;
using Styx.WoWInternals;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Waldo.Managers
{
    public static class WaHotKeyManager
    {
        public static bool IsPaused { get; private set; }
        public static bool IsCooldown { get; private set; }
        public static bool IsTricks { get; private set; }
        public static bool IsAoe { get; private set; }
        public static bool IsSpecialKey { get; private set; }

        public delegate T Selection<out T>(object context);

        public static void BotEvents_OnBotStarted(EventArgs args)
        {
            if (RoutineManager.Current.Name != WaMain.WaName) return;
            if (WaSettingsH.Instance.ModeSelection == WaEnum.Mode.Hotkey)
                RegisterKeys();
            WaLua.DisableClickToMove();
        }

        public static void BotEvents_OnBotStopped(EventArgs args)
        {
            if (RoutineManager.Current.Name != WaMain.WaName) return;
                RemoveAllKeys();
            WaLua.EnableClickToMove();
            WaLogger.DiagLogW("Waldo: stopped!");
        }

        private static void LogKey(string kType, Keys kValue, ModifierKeys kModifier, bool kResult)
        {
            WaLogger.DiagLogW("{0}-key ({1} + {2}) pressed, set to: {3}", kType, kValue, kModifier, kResult);
        }

        internal class KeyboardPolling
        {
            /* Keystates - One press without Spell Queueing */
            [DllImport("user32.dll")]
            private static extern short GetKeyState(int keyCode);

            private static WaEnum.KeyStates GetKeyState(Keys key)
            {
                WaEnum.KeyStates state = WaEnum.KeyStates.None;

                short retVal = GetKeyState((int)key);
                if ((retVal & 0x8000) == 0x8000)
                    state |= WaEnum.KeyStates.Down;

                return state;
            }

            public static bool IsKeyDown(Keys key)
            {
                return WaEnum.KeyStates.Down == (GetKeyState(key) & WaEnum.KeyStates.Down);
            }

            /* Keystates - One press with Spell Queueing */
            [DllImport("user32.dll")]
            internal static extern short GetAsyncKeyState(Keys vKey);

            public static bool IsKeyAsyncDown(Keys vKey)
            {
                return (GetAsyncKeyState(vKey)) != 0;
            }
        }

        public static void RegisterKeys()
        {
            switch (WaSettingsH.Instance.ModeSelection)
            {
                case WaEnum.Mode.Hotkey:
                    switch (WaSettingsH.Instance.ModKeyChoice)
                    {
                        case ModifierKeys.Alt:
                        case ModifierKeys.Control:
                        case ModifierKeys.Shift:
                        case ModifierKeys.Win:
                            HotkeysManager.Register("Pause", WaSettingsH.Instance.PauseKeyChoice, WaSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsPaused = !IsPaused;
                                LogKey("Pause", WaSettingsH.Instance.PauseKeyChoice, WaSettingsH.Instance.ModKeyChoice, IsPaused);
                                if (WaSettings.Instance.General.EnableWoWChatOutput)
                                    Lua.DoString(IsPaused
                                                     ? @"print('Rotation \124cFFE61515 Paused!')"
                                                     : @"print('Rotation \124cFF15E61C Resumed!')");
                            });

                            HotkeysManager.Register("Cooldown", WaSettingsH.Instance.CooldownKeyChoice, WaSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsCooldown = !IsCooldown;
                                LogKey("Cooldown", WaSettingsH.Instance.CooldownKeyChoice, WaSettingsH.Instance.ModKeyChoice, IsCooldown);
                                if (WaSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                                                     : @"print('Cooldowns \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Tricks", WaSettingsH.Instance.Tricks, WaSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsTricks = !IsTricks;
                                LogKey("Tricks", WaSettingsH.Instance.Tricks, WaSettingsH.Instance.ModKeyChoice, IsTricks);
                                if (WaSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsTricks
                                                     ? @"print('Tricks of the Trade \124cFF15E61C Enabled!')"
                                                     : @"print('Tricks of the Trade \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AoE", WaSettingsH.Instance.MultiTgtKeyChoice, WaSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsAoe = !IsAoe;
                                LogKey("AoE", WaSettingsH.Instance.MultiTgtKeyChoice, WaSettingsH.Instance.ModKeyChoice, IsAoe);
                                if (WaSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? @"print('Aoe \124cFF15E61C Enabled!')"
                                                     : @"print('Aoe \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Special", WaSettingsH.Instance.SpecialKeyChoice, WaSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsSpecialKey = !IsSpecialKey;
                                LogKey("Special", WaSettingsH.Instance.SpecialKeyChoice, WaSettingsH.Instance.ModKeyChoice, IsSpecialKey);
                                if (WaSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsSpecialKey
                                                     ? @"print('Special \124cFF15E61C Enabled!')"
                                                     : @"print('Special \124cFFE61515 Disabled!')");
                            });
                            WaLogger.DiagLogW("Waldo: Hotkeys registered with individual values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}, TricksKey {5}",
                                WaSettingsH.Instance.ModKeyChoice,
                                WaSettingsH.Instance.PauseKeyChoice,
                                WaSettingsH.Instance.CooldownKeyChoice,
                                WaSettingsH.Instance.MultiTgtKeyChoice,
                                WaSettingsH.Instance.SpecialKeyChoice,
                                WaSettingsH.Instance.Tricks);
                            break;
                        default:
                            HotkeysManager.Register("Pause", WaSettingsH.Instance.PauseKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsPaused = !IsPaused;
                                LogKey("Pause", WaSettingsH.Instance.PauseKeyChoice, ModifierKeys.Alt, IsPaused);
                                if (WaSettings.Instance.General.EnableWoWChatOutput)
                                    Lua.DoString(IsPaused
                                                     ? @"print('Rotation \124cFFE61515 Paused!')"
                                                     : @"print('Rotation \124cFF15E61C Resumed!')");
                            });

                            HotkeysManager.Register("Cooldown", WaSettingsH.Instance.CooldownKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsCooldown = !IsCooldown;
                                LogKey("Cooldown", WaSettingsH.Instance.CooldownKeyChoice, ModifierKeys.Alt, IsCooldown);
                                if (WaSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                                                     : @"print('Cooldowns \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AoE", WaSettingsH.Instance.MultiTgtKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsAoe = !IsAoe;
                                LogKey("AOE", WaSettingsH.Instance.MultiTgtKeyChoice, ModifierKeys.Alt, IsAoe);
                                if (WaSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? @"print('Aoe \124cFF15E61C Enabled!')"
                                                     : @"print('Aoe \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Tricks", WaSettingsH.Instance.Tricks, WaSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsTricks = !IsTricks;
                                LogKey("Tricks", WaSettingsH.Instance.Tricks, ModifierKeys.Alt, IsTricks);
                                if (WaSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsTricks
                                                     ? @"print('Tricks of the Trade \124cFF15E61C Enabled!')"
                                                     : @"print('Tricks of the Trade \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Special", WaSettingsH.Instance.SpecialKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsSpecialKey = !IsSpecialKey;
                                LogKey("Special", WaSettingsH.Instance.SpecialKeyChoice, ModifierKeys.Alt, IsSpecialKey);
                                if (WaSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsSpecialKey
                                                     ? @"print('Special \124cFF15E61C Enabled!')"
                                                     : @"print('Special \124cFFE61515 Disabled!')");
                            });
                            WaLogger.DiagLogW("Waldo: Hotkeys registered with default values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}, TricksKey: {5}",
                                WaSettingsH.Instance.ModKeyChoice,
                                WaSettingsH.Instance.PauseKeyChoice,
                                WaSettingsH.Instance.CooldownKeyChoice,
                                WaSettingsH.Instance.MultiTgtKeyChoice,
                                WaSettingsH.Instance.SpecialKeyChoice,
                                WaSettingsH.Instance.Tricks);
                            break;
                    }
                    break;
            }
            WaLogger.DiagLogW("Waldo: Hotkeys registered!");
        }

        public static void RemoveAllKeys()
        {
            HotkeysManager.Unregister("Pause");
            HotkeysManager.Unregister("Cooldown");
            HotkeysManager.Unregister("AoE");
            HotkeysManager.Unregister("Special");
            HotkeysManager.Unregister("Tricks");
            WaLogger.DiagLogW("Waldo: Hotkeys removed!");
        }
    }
}