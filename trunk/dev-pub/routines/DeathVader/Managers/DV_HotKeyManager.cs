using DeathVader.Helpers;
using DeathVader.Interfaces.Settings;
using Styx.Common;
using Styx.CommonBot.Routines;
using Styx.WoWInternals;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DeathVader.Managers
{
    public static class DvHotKeyManager
    {
        public static bool IsPaused { get; private set; }
        public static bool IsCooldown { get; private set; }
        public static bool IsAoe { get; private set; }
        public static bool IsSpecialKey { get; private set; }

        public delegate T Selection<out T>(object context);

        public static void BotEvents_OnBotStarted(EventArgs args)
        {
            if (RoutineManager.Current.Name != DvMain.DvName) return;
            if (DvSettingsH.Instance.ModeSelection == DvEnum.Mode.Hotkey)
                RegisterKeys();
            DvLua.DisableClickToMove();
        }

        public static void BotEvents_OnBotStopped(EventArgs args)
        {
            if (RoutineManager.Current.Name != DvMain.DvName) return;
                RemoveAllKeys();
            DvLua.EnableClickToMove();
            DvLogger.DiagLogW("Death Vader: stopped!");
        }

        private static void LogKey(string kType, Keys kValue, ModifierKeys kModifier, bool kResult)
        {
            DvLogger.DiagLogW("{0}-key ({1} + {2}) pressed, set to: {3}", kType, kValue, kModifier, kResult);
        }

        internal class KeyboardPolling
        {
            /* Keystates - One press without Spell Queueing */
            [DllImport("user32.dll")]
            private static extern short GetKeyState(int keyCode);

            private static DvEnum.KeyStates GetKeyState(Keys key)
            {
                DvEnum.KeyStates state = DvEnum.KeyStates.None;

                short retVal = GetKeyState((int)key);
                if ((retVal & 0x8000) == 0x8000)
                    state |= DvEnum.KeyStates.Down;

                return state;
            }

            public static bool IsKeyDown(Keys key)
            {
                return DvEnum.KeyStates.Down == (GetKeyState(key) & DvEnum.KeyStates.Down);
            }

            /* Keystates - One press with Spell Queueing */
            [DllImport("user32.dll")]
            private static extern short GetAsyncKeyState(Keys vKey);

            public static bool IsKeyAsyncDown(Keys vKey)
            {
                return (GetAsyncKeyState(vKey)) != 0;
            }
        }

        public static void RegisterKeys()
        {
            switch (DvSettingsH.Instance.ModeSelection)
            {
                case DvEnum.Mode.Hotkey:
                    switch (DvSettingsH.Instance.ModKeyChoice)
                    {
                        case ModifierKeys.Alt:
                        case ModifierKeys.Control:
                        case ModifierKeys.Shift:
                        case ModifierKeys.Win:
                            HotkeysManager.Register("Pause", DvSettingsH.Instance.PauseKeyChoice, DvSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsPaused = !IsPaused;
                                LogKey("Pause", DvSettingsH.Instance.PauseKeyChoice, DvSettingsH.Instance.ModKeyChoice, IsPaused);
                                if (DvSettings.Instance.General.EnableWoWChatOutput)
                                    Lua.DoString(IsPaused
                                                     ? @"print('Rotation \124cFFE61515 Paused!')"
                                                     : @"print('Rotation \124cFF15E61C Resumed!')");
                            });

                            HotkeysManager.Register("Cooldown", DvSettingsH.Instance.CooldownKeyChoice, DvSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsCooldown = !IsCooldown;
                                LogKey("Cooldown", DvSettingsH.Instance.CooldownKeyChoice, DvSettingsH.Instance.ModKeyChoice, IsCooldown);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                                                     : @"print('Cooldowns \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AoE", DvSettingsH.Instance.MultiTgtKeyChoice, DvSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsAoe = !IsAoe;
                                LogKey("AoE", DvSettingsH.Instance.MultiTgtKeyChoice, DvSettingsH.Instance.ModKeyChoice, IsAoe);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? @"print('Aoe \124cFF15E61C Enabled!')"
                                                     : @"print('Aoe \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Special", DvSettingsH.Instance.SpecialKeyChoice, DvSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsSpecialKey = !IsSpecialKey;
                                LogKey("Special", DvSettingsH.Instance.SpecialKeyChoice, DvSettingsH.Instance.ModKeyChoice, IsSpecialKey);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsSpecialKey
                                                     ? @"print('Special \124cFF15E61C Enabled!')"
                                                     : @"print('Special \124cFFE61515 Disabled!')");
                            });
                            DvLogger.DiagLogW("Death Vader: Hotkeys registered with individual values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}",
                                DvSettingsH.Instance.ModKeyChoice,
                                DvSettingsH.Instance.PauseKeyChoice,
                                DvSettingsH.Instance.CooldownKeyChoice,
                                DvSettingsH.Instance.MultiTgtKeyChoice,
                                DvSettingsH.Instance.SpecialKeyChoice);
                            break;
                        default:
                            HotkeysManager.Register("Pause", DvSettingsH.Instance.PauseKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsPaused = !IsPaused;
                                LogKey("Pause", DvSettingsH.Instance.PauseKeyChoice, ModifierKeys.Alt, IsPaused);
                                if (DvSettings.Instance.General.EnableWoWChatOutput)
                                    Lua.DoString(IsPaused
                                                     ? @"print('Rotation \124cFFE61515 Paused!')"
                                                     : @"print('Rotation \124cFF15E61C Resumed!')");
                            });

                            HotkeysManager.Register("Cooldown", DvSettingsH.Instance.CooldownKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsCooldown = !IsCooldown;
                                LogKey("Cooldown", DvSettingsH.Instance.CooldownKeyChoice, ModifierKeys.Alt, IsCooldown);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                                                     : @"print('Cooldowns \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AoE", DvSettingsH.Instance.MultiTgtKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsAoe = !IsAoe;
                                LogKey("AOE", DvSettingsH.Instance.MultiTgtKeyChoice, ModifierKeys.Alt, IsAoe);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? @"print('Aoe \124cFF15E61C Enabled!')"
                                                     : @"print('Aoe \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Special", DvSettingsH.Instance.SpecialKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsSpecialKey = !IsSpecialKey;
                                LogKey("Special", DvSettingsH.Instance.SpecialKeyChoice, ModifierKeys.Alt, IsSpecialKey);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsSpecialKey
                                                     ? @"print('Special \124cFF15E61C Enabled!')"
                                                     : @"print('Special \124cFFE61515 Disabled!')");
                            });
                            DvLogger.DiagLogW("Death Vader: Hotkeys registered with default values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}",
                                DvSettingsH.Instance.ModKeyChoice,
                                DvSettingsH.Instance.PauseKeyChoice,
                                DvSettingsH.Instance.CooldownKeyChoice,
                                DvSettingsH.Instance.MultiTgtKeyChoice,
                                DvSettingsH.Instance.SpecialKeyChoice);
                            break;
                    }
                    break;
            }
            DvLogger.DiagLogW("Death Vader: Hotkeys registered!");
        }

        public static void RemoveAllKeys()
        {
            HotkeysManager.Unregister("Pause");
            HotkeysManager.Unregister("Cooldown");
            HotkeysManager.Unregister("AoE");
            HotkeysManager.Unregister("Special");
            DvLogger.DiagLogW("Death Vader: Hotkeys removed!");
        }
    }
}