using Shammy.Helpers;
using Shammy.Interfaces.Settings;
using Styx.Common;
using Styx.CommonBot.Routines;
using Styx.WoWInternals;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Shammy.Managers
{
    public static class SmHotKeyManager
    {
        public static bool IsPaused { get; private set; }
        public static bool IsCooldown { get; private set; }
        public static bool IsAoe { get; private set; }
        public static bool IsSpecialKey { get; private set; }

        public delegate T Selection<out T>(object context);

        public static void BotEvents_OnBotStarted(EventArgs args)
        {
            if (RoutineManager.Current.Name != SmMain.SmName) return;
            if (SmSettingsH.Instance.ModeSelection == SmEnum.Mode.Hotkey)
                RegisterKeys();
            SmLua.DisableClickToMove();
        }

        public static void BotEvents_OnBotStopped(EventArgs args)
        {
            if (RoutineManager.Current.Name != SmMain.SmName) return;
                RemoveAllKeys();
            SmLua.EnableClickToMove();
            SmLogger.DiagLogW("Shammy: stopped!");
        }

        private static void LogKey(string kType, Keys kValue, ModifierKeys kModifier, bool kResult)
        {
            SmLogger.DiagLogW("{0}-key ({1} + {2}) pressed, set to: {3}", kType, kValue, kModifier, kResult);
        }

        internal class KeyboardPolling
        {
            /* Keystates - One press without Spell Queueing */
            [DllImport("user32.dll")]
            private static extern short GetKeyState(int keyCode);

            private static SmEnum.KeyStates GetKeyState(Keys key)
            {
                SmEnum.KeyStates state = SmEnum.KeyStates.None;

                short retVal = GetKeyState((int)key);
                if ((retVal & 0x8000) == 0x8000)
                    state |= SmEnum.KeyStates.Down;

                return state;
            }

            public static bool IsKeyDown(Keys key)
            {
                return SmEnum.KeyStates.Down == (GetKeyState(key) & SmEnum.KeyStates.Down);
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
            switch (SmSettingsH.Instance.ModeSelection)
            {
                case SmEnum.Mode.Hotkey:
                    switch (SmSettingsH.Instance.ModKeyChoice)
                    {
                        case ModifierKeys.Alt:
                        case ModifierKeys.Control:
                        case ModifierKeys.Shift:
                        case ModifierKeys.Win:
                            HotkeysManager.Register("Pause", SmSettingsH.Instance.PauseKeyChoice, SmSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsPaused = !IsPaused;
                                LogKey("Pause", SmSettingsH.Instance.PauseKeyChoice, SmSettingsH.Instance.ModKeyChoice, IsPaused);
                                if (SmSettings.Instance.General.EnableWoWChatOutput)
                                    Lua.DoString(IsPaused
                                                     ? @"print('Rotation \124cFFE61515 Paused!')"
                                                     : @"print('Rotation \124cFF15E61C Resumed!')");
                            });

                            HotkeysManager.Register("Cooldown", SmSettingsH.Instance.CooldownKeyChoice, SmSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsCooldown = !IsCooldown;
                                LogKey("Cooldown", SmSettingsH.Instance.CooldownKeyChoice, SmSettingsH.Instance.ModKeyChoice, IsCooldown);
                                if (SmSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                                                     : @"print('Cooldowns \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AoE", SmSettingsH.Instance.MultiTgtKeyChoice, SmSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsAoe = !IsAoe;
                                LogKey("AoE", SmSettingsH.Instance.MultiTgtKeyChoice, SmSettingsH.Instance.ModKeyChoice, IsAoe);
                                if (SmSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? @"print('Aoe \124cFF15E61C Enabled!')"
                                                     : @"print('Aoe \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Special", SmSettingsH.Instance.SpecialKeyChoice, SmSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsSpecialKey = !IsSpecialKey;
                                LogKey("Special", SmSettingsH.Instance.SpecialKeyChoice, SmSettingsH.Instance.ModKeyChoice, IsSpecialKey);
                                if (SmSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsSpecialKey
                                                     ? @"print('Special \124cFF15E61C Enabled!')"
                                                     : @"print('Special \124cFFE61515 Disabled!')");
                            });
                            SmLogger.DiagLogW("Shammy: Hotkeys registered with individual values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}",
                                SmSettingsH.Instance.ModKeyChoice,
                                SmSettingsH.Instance.PauseKeyChoice,
                                SmSettingsH.Instance.CooldownKeyChoice,
                                SmSettingsH.Instance.MultiTgtKeyChoice,
                                SmSettingsH.Instance.SpecialKeyChoice);
                            break;
                        default:
                            HotkeysManager.Register("Pause", SmSettingsH.Instance.PauseKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsPaused = !IsPaused;
                                LogKey("Pause", SmSettingsH.Instance.PauseKeyChoice, ModifierKeys.Alt, IsPaused);
                                if (SmSettings.Instance.General.EnableWoWChatOutput)
                                    Lua.DoString(IsPaused
                                                     ? @"print('Rotation \124cFFE61515 Paused!')"
                                                     : @"print('Rotation \124cFF15E61C Resumed!')");
                            });

                            HotkeysManager.Register("Cooldown", SmSettingsH.Instance.CooldownKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsCooldown = !IsCooldown;
                                LogKey("Cooldown", SmSettingsH.Instance.CooldownKeyChoice, ModifierKeys.Alt, IsCooldown);
                                if (SmSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                                                     : @"print('Cooldowns \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AoE", SmSettingsH.Instance.MultiTgtKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsAoe = !IsAoe;
                                LogKey("AOE", SmSettingsH.Instance.MultiTgtKeyChoice, ModifierKeys.Alt, IsAoe);
                                if (SmSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? @"print('Aoe \124cFF15E61C Enabled!')"
                                                     : @"print('Aoe \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Special", SmSettingsH.Instance.SpecialKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsSpecialKey = !IsSpecialKey;
                                LogKey("Special", SmSettingsH.Instance.SpecialKeyChoice, ModifierKeys.Alt, IsSpecialKey);
                                if (SmSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsSpecialKey
                                                     ? @"print('Special \124cFF15E61C Enabled!')"
                                                     : @"print('Special \124cFFE61515 Disabled!')");
                            });
                            SmLogger.DiagLogW("Shammy: Hotkeys registered with default values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}",
                                SmSettingsH.Instance.ModKeyChoice,
                                SmSettingsH.Instance.PauseKeyChoice,
                                SmSettingsH.Instance.CooldownKeyChoice,
                                SmSettingsH.Instance.MultiTgtKeyChoice,
                                SmSettingsH.Instance.SpecialKeyChoice);
                            break;
                    }
                    break;
            }
            SmLogger.DiagLogW("Shammy: Hotkeys registered!");
        }

        public static void RemoveAllKeys()
        {
            HotkeysManager.Unregister("Pause");
            HotkeysManager.Unregister("Cooldown");
            HotkeysManager.Unregister("AoE");
            HotkeysManager.Unregister("Special");
            SmLogger.DiagLogW("Shammy: Hotkeys removed!");
        }
    }
}