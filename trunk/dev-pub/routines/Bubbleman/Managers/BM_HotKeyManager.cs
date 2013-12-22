using Bubbleman.Helpers;
using Bubbleman.Interfaces.Settings;
using Styx.Common;
using Styx.CommonBot.Routines;
using Styx.WoWInternals;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bubbleman.Managers
{
    public static class BMHotKeyManager
    {
        public static bool IsPaused { get; private set; }
        public static bool IsCooldown { get; private set; }
        public static bool IsAoe { get; private set; }
        public static bool IsSpecialKey { get; private set; }
        public static bool ElusiveBrew { get; private set; }
        public static bool AutoDizzyingHaze { get; private set; }

        public delegate T Selection<out T>(object context);

        public static void BotEvents_OnBotStarted(EventArgs args)
        {
            if (RoutineManager.Current.Name != BMMain.BMName) return;
            if (BMSettingsH.Instance.ModeSelection == BMEnum.Mode.Hotkey)
                RegisterKeys();
              BMLua.DisableClickToMove();
        }

        public static void BotEvents_OnBotStopped(EventArgs args)
        {
            if (RoutineManager.Current.Name != BMMain.BMName) return;
                RemoveAllKeys();
        //    BMLua.EnableClickToMove();
            BMLogger.DiagLogW("Bubbleman: stopped!");
        }

        private static void LogKey(string kType, Keys kValue, ModifierKeys kModifier, bool kResult)
        {
            BMLogger.DiagLogW("{0}-key ({1} + {2}) pressed, set to: {3}", kType, kValue, kModifier, kResult);
        }

        internal class KeyboardPolling
        {
            /* Keystates - One press without Spell Queueing */
            [DllImport("user32.dll")]
            private static extern short GetKeyState(int keyCode);

            private static BMEnum.KeyStates GetKeyState(Keys key)
            {
                BMEnum.KeyStates state = BMEnum.KeyStates.None;

                short retVal = GetKeyState((int)key);
                if ((retVal & 0x8000) == 0x8000)
                    state |= BMEnum.KeyStates.Down;

                return state;
            }

            public static bool IsKeyDown(Keys key)
            {
                return BMEnum.KeyStates.Down == (GetKeyState(key) & BMEnum.KeyStates.Down);
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
            switch (BMSettingsH.Instance.ModeSelection)
            {
                case BMEnum.Mode.Hotkey:
                    switch (BMSettingsH.Instance.ModKeyChoice)
                    {
                        case ModifierKeys.Alt:
                        case ModifierKeys.Control:
                        case ModifierKeys.Shift:
                        case ModifierKeys.Win:
                            HotkeysManager.Register("Pause", BMSettingsH.Instance.PauseKeyChoice, BMSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsPaused = !IsPaused;
                                LogKey("Pause", BMSettingsH.Instance.PauseKeyChoice, BMSettingsH.Instance.ModKeyChoice, IsPaused);
                                if (BMSettings.Instance.General.EnableMacroOutput)
                                    Lua.DoString(IsPaused
                                                    ? "RunMacroText('/Def true')"
                                                    : "RunMacroText('/Def false')");
                                if (BMSettings.Instance.General.EnableWoWChatOutput)
                                    Lua.DoString(IsPaused
                                                     ? @"print('Rotation \124cFFE61515 Paused!')"
                                                     : @"print('Rotation \124cFF15E61C Resumed!')");
                            });

                            HotkeysManager.Register("Cooldown", BMSettingsH.Instance.CooldownKeyChoice, BMSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsCooldown = !IsCooldown;
                                LogKey("Cooldown", BMSettingsH.Instance.CooldownKeyChoice, BMSettingsH.Instance.ModKeyChoice, IsCooldown);
                                if (BMSettings.Instance.General.EnableMacroOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? "RunMacroText('/MyCooldowns true')"
                                                     : "RunMacroText('/MyCooldowns false')");
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                                                     : @"print('Cooldowns \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Elusivebrew", BMSettingsH.Instance.ElusiveBrew, BMSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                ElusiveBrew = !ElusiveBrew;
                                LogKey("Elusivebrew", BMSettingsH.Instance.ElusiveBrew, BMSettingsH.Instance.ModKeyChoice, ElusiveBrew);
                                if (BMSettings.Instance.General.EnableMacroOutput && !IsPaused)
                                    Lua.DoString(ElusiveBrew
                                                     ? "RunMacroText('/elusivebrew true')"
                                                     : "RunMacroText('/elusivebrew false')");
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(ElusiveBrew
                                                     ? @"print('ElusiveBrew \124cFF15E61C Enabled!')"
                                                     : @"print('ElusiveBrew \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AutoDizzying", BMSettingsH.Instance.DizzyingHaze, BMSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                AutoDizzyingHaze = !AutoDizzyingHaze;
                                LogKey("AutoDizzying", BMSettingsH.Instance.DizzyingHaze, BMSettingsH.Instance.ModKeyChoice, AutoDizzyingHaze);
                                if (BMSettings.Instance.General.EnableMacroOutput && !IsPaused)
                                    Lua.DoString(AutoDizzyingHaze
                                                     ? "RunMacroText('/dizzling true')"
                                                     : "RunMacroText('/dizzling false')");
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(AutoDizzyingHaze
                                                     ? @"print('Dizzling \124cFF15E61C Enabled!')"
                                                     : @"print('Dizzling \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AoE", BMSettingsH.Instance.MultiTgtKeyChoice, BMSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsAoe = !IsAoe;
                                LogKey("AoE", BMSettingsH.Instance.MultiTgtKeyChoice, BMSettingsH.Instance.ModKeyChoice, IsAoe);
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? "RunMacroText('/AoEderp true')"
                                                     : "RunMacroText('/AoEderp false')");
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe                                                
                                                     ? @"print('Aoe \124cFF15E61C Enabled!')"
                                                     : @"print('Aoe \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Special", BMSettingsH.Instance.SpecialKeyChoice, BMSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                IsSpecialKey = !IsSpecialKey;
                                LogKey("Special", BMSettingsH.Instance.SpecialKeyChoice, BMSettingsH.Instance.ModKeyChoice, IsSpecialKey);
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsSpecialKey
                                                     ? @"print('Special \124cFF15E61C Enabled!')"
                                                     : @"print('Special \124cFFE61515 Disabled!')");
                            });
                            BMLogger.DiagLogW("Bubbleman: Hotkeys registered with individual values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}, DizzyingHaze{5}, ElusiveBrew{6}",
                                BMSettingsH.Instance.ModKeyChoice,
                                BMSettingsH.Instance.PauseKeyChoice,
                                BMSettingsH.Instance.CooldownKeyChoice,
                                BMSettingsH.Instance.MultiTgtKeyChoice,
                                BMSettingsH.Instance.SpecialKeyChoice,
                                BMSettingsH.Instance.DizzyingHaze,
                                BMSettingsH.Instance.ElusiveBrew);
                            break;
                        default:
                            HotkeysManager.Register("Pause", BMSettingsH.Instance.PauseKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsPaused = !IsPaused;
                                LogKey("Pause", BMSettingsH.Instance.PauseKeyChoice, ModifierKeys.Alt, IsPaused);
                                if (BMSettings.Instance.General.EnableMacroOutput)
                                    Lua.DoString(IsPaused
                                                    ? "RunMacroText('/Def true')"
                                                    : "RunMacroText('/Def false')");
                                if (BMSettings.Instance.General.EnableWoWChatOutput)
                                    Lua.DoString(IsPaused
                                                     ? @"print('Rotation \124cFFE61515 Paused!')"
                                                     : @"print('Rotation \124cFF15E61C Resumed!')");
                            });

                            HotkeysManager.Register("Cooldown", BMSettingsH.Instance.CooldownKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsCooldown = !IsCooldown;
                                LogKey("Cooldown", BMSettingsH.Instance.CooldownKeyChoice, ModifierKeys.Alt, IsCooldown);
                                if (BMSettings.Instance.General.EnableMacroOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? "RunMacroText('/MyCooldowns true')"
                                                     : "RunMacroText('/MyCooldowns false')");
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                                                     : @"print('Cooldowns \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AoE", BMSettingsH.Instance.MultiTgtKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsAoe = !IsAoe;
                                LogKey("AOE", BMSettingsH.Instance.MultiTgtKeyChoice, ModifierKeys.Alt, IsAoe);
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? "RunMacroText('/AoEderp true')"
                                                     : "RunMacroText('/AoEderp false')");
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? @"print('Aoe \124cFF15E61C Enabled!')"
                                                     : @"print('Aoe \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Elusivebrew", BMSettingsH.Instance.ElusiveBrew, ModifierKeys.Alt, hk =>
                            {
                                ElusiveBrew = !ElusiveBrew;
                                LogKey("Elusivebrew", BMSettingsH.Instance.ElusiveBrew, ModifierKeys.Alt, ElusiveBrew);
                                if (BMSettings.Instance.General.EnableMacroOutput && !IsPaused)
                                    Lua.DoString(ElusiveBrew
                                                     ? "RunMacroText('/elusivebrew true')"
                                                     : "RunMacroText('/elusivebrew false')");
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(ElusiveBrew
                                                     ? @"print('ElusiveBrew \124cFF15E61C Enabled!')"
                                                     : @"print('ElusiveBrew \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AutoDizzying", BMSettingsH.Instance.DizzyingHaze, ModifierKeys.Alt, hk =>
                            {
                                AutoDizzyingHaze = !AutoDizzyingHaze;
                                LogKey("AutoDizzying", BMSettingsH.Instance.DizzyingHaze, ModifierKeys.Alt, AutoDizzyingHaze);
                                if (BMSettings.Instance.General.EnableMacroOutput && !IsPaused)
                                    Lua.DoString(AutoDizzyingHaze
                                                     ? "RunMacroText('/dizzling true')"
                                                     : "RunMacroText('/dizzling false')");
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(AutoDizzyingHaze
                                                     ? @"print('Dizzling \124cFF15E61C Enabled!')"
                                                     : @"print('Dizzling \124cFFE61515 Disabled!')");
                            });


                            HotkeysManager.Register("Special", BMSettingsH.Instance.SpecialKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsSpecialKey = !IsSpecialKey;
                                LogKey("Special", BMSettingsH.Instance.SpecialKeyChoice, ModifierKeys.Alt, IsSpecialKey);
                                if (BMSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsSpecialKey
                                                     ? @"print('Special \124cFF15E61C Enabled!')"
                                                     : @"print('Special \124cFFE61515 Disabled!')");
                            });
                            BMLogger.DiagLogW("Bubbleman: Hotkeys registered with default values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}, DizzyingHaze{5}, ElusiveBrew{6}",
                                BMSettingsH.Instance.ModKeyChoice,
                                BMSettingsH.Instance.PauseKeyChoice,
                                BMSettingsH.Instance.CooldownKeyChoice,
                                BMSettingsH.Instance.MultiTgtKeyChoice,
                                BMSettingsH.Instance.SpecialKeyChoice,
                                BMSettingsH.Instance.DizzyingHaze,
                                BMSettingsH.Instance.ElusiveBrew);
                            break;
                    }
                    break;
            }
            BMLogger.DiagLogW("Bubbleman: Hotkeys registered!");
        }

        public static void RemoveAllKeys()
        {
            HotkeysManager.Unregister("Pause");
            HotkeysManager.Unregister("Cooldown");
            HotkeysManager.Unregister("AoE");
            HotkeysManager.Unregister("AutoDizzying");
            HotkeysManager.Unregister("Elusivebrew");
            HotkeysManager.Unregister("Special");
            BMLogger.DiagLogW("Bubbleman: Hotkeys removed!");
        }
    }
}