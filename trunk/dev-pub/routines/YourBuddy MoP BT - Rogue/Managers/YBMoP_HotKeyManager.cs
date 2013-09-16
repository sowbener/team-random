using System.Runtime.InteropServices;
using Styx.Common;
using Styx.CommonBot.Routines;
using Styx.WoWInternals;
using System;
using System.Windows.Forms;
using YBMoP_BT_Rogue.Helpers;
using YBMoP_BT_Rogue.Interfaces.Settings;

namespace YBMoP_BT_Rogue.Managers
{
    public static class HotKeyManager
    {
        public static bool IsPaused { get; private set; }
        public static bool IsCooldown { get; private set; }
        public static bool IsAoe { get; private set; }
        public static bool IsSpecialKey { get; private set; }

        public static void BotEvents_OnBotStarted(EventArgs args)
        {
            if (RoutineManager.Current.Name != YBMain.YBName) return;
            if (YBSettingsG.Instance.ModeChoice == Mode.Hotkey)
                RegisterKeys();
            YBLua.DisableClickToMove();
        }

        private static void LogKey(string kType, Keys kValue, ModifierKeys kModifier, bool kResult)
        {
            YBLogger.DiagLogW("{0}-key ({1} + {2}) pressed, set to: {3}", kType, kValue, kModifier, kResult);
        }

        public static void BotEvents_OnBotStopped(EventArgs args)
        {
            if (RoutineManager.Current.Name != YBMain.YBName) return;
                RemoveAllKeys();
            YBLua.EnableClickToMove();
            YBLogger.DiagLogW("YBMoP BT: stopped!");
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);
        public static bool IsKeyDown(Keys key)
        {
            return (GetAsyncKeyState(key)) != 0;
        }

        public static void RegisterKeys()
        {
            switch (YBSettingsG.Instance.ModeChoice)
            {
                case Mode.Hotkey:
                    switch (YBSettingsG.Instance.ModKeyChoice)
                    {
                        case ModifierKeys.Alt:
                        case ModifierKeys.Control:
                        case ModifierKeys.Shift:
                        case ModifierKeys.Win:
                            HotkeysManager.Register("Pause", YBSettingsG.Instance.PauseKeyChoice, YBSettingsG.Instance.ModKeyChoice, hk =>
                            {
                                IsPaused = !IsPaused;
                                LogKey("Pause", YBSettingsG.Instance.PauseKeyChoice, YBSettingsG.Instance.ModKeyChoice, IsPaused);
                                if (YBSettingsG.Instance.EnableWoWChatOutput)
                                    Lua.DoString(IsPaused
                                                     ? @"print('Rotation \124cFFE61515 Paused!')"
                                                     : @"print('Rotation \124cFF15E61C Resumed!')");
                            });

                            HotkeysManager.Register("Cooldown", YBSettingsG.Instance.CooldownKeyChoice, YBSettingsG.Instance.ModKeyChoice, hk =>
                            {
                                IsCooldown = !IsCooldown;
                                LogKey("Cooldown", YBSettingsG.Instance.CooldownKeyChoice, YBSettingsG.Instance.ModKeyChoice, IsCooldown);
                                if (YBSettingsG.Instance.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                                                     : @"print('Cooldowns \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AoE", YBSettingsG.Instance.SwitchKeyChoice, YBSettingsG.Instance.ModKeyChoice, hk =>
                            {
                                IsAoe = !IsAoe;
                                LogKey("AoE", YBSettingsG.Instance.SwitchKeyChoice, YBSettingsG.Instance.ModKeyChoice, IsAoe);
                                if (YBSettingsG.Instance.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? @"print('Aoe \124cFF15E61C Enabled!')"
                                                     : @"print('Aoe \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Special", YBSettingsG.Instance.SpecialKeyChoice, YBSettingsG.Instance.ModKeyChoice, hk =>
                            {
                                IsSpecialKey = !IsSpecialKey;
                                LogKey("Special", YBSettingsG.Instance.SpecialKeyChoice, YBSettingsG.Instance.ModKeyChoice, IsSpecialKey);
                                if (YBSettingsG.Instance.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsSpecialKey
                                                     ? @"print('Special \124cFF15E61C Enabled!')"
                                                     : @"print('Special \124cFFE61515 Disabled!')");
                            });
                            YBLogger.DiagLogW("YBMoP BT: Hotkeys registered with individual values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}",
                                YBSettingsG.Instance.ModKeyChoice,
                                YBSettingsG.Instance.PauseKeyChoice,
                                YBSettingsG.Instance.CooldownKeyChoice,
                                YBSettingsG.Instance.SwitchKeyChoice,
                                YBSettingsG.Instance.SpecialKeyChoice);
                            break;
                        default:
                            HotkeysManager.Register("Pause", YBSettingsG.Instance.PauseKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsPaused = !IsPaused;
                                LogKey("Pause", YBSettingsG.Instance.PauseKeyChoice, ModifierKeys.Alt, IsPaused);
                                if (YBSettingsG.Instance.EnableWoWChatOutput)
                                    Lua.DoString(IsPaused
                                                     ? @"print('Rotation \124cFFE61515 Paused!')"
                                                     : @"print('Rotation \124cFF15E61C Resumed!')");
                            });

                            HotkeysManager.Register("Cooldown", YBSettingsG.Instance.CooldownKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsCooldown = !IsCooldown;
                                LogKey("Cooldown", YBSettingsG.Instance.CooldownKeyChoice, ModifierKeys.Alt, IsCooldown);
                                if (YBSettingsG.Instance.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Cooldowns \124cFF15E61C Enabled!')"
                                                     : @"print('Cooldowns \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AoE", YBSettingsG.Instance.SwitchKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsAoe = !IsAoe;
                                LogKey("AOE", YBSettingsG.Instance.SwitchKeyChoice, ModifierKeys.Alt, IsAoe);
                                if (YBSettingsG.Instance.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsAoe
                                                     ? @"print('Aoe \124cFF15E61C Enabled!')"
                                                     : @"print('Aoe \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("Special", YBSettingsG.Instance.SpecialKeyChoice, ModifierKeys.Alt, hk =>
                            {
                                IsSpecialKey = !IsSpecialKey;
                                LogKey("Special", YBSettingsG.Instance.SpecialKeyChoice, ModifierKeys.Alt, IsSpecialKey);
                                if (YBSettingsG.Instance.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsSpecialKey
                                                     ? @"print('Special \124cFF15E61C Enabled!')"
                                                     : @"print('Special \124cFFE61515 Disabled!')");
                            });
                            YBLogger.DiagLogW("YBMoP BT: Hotkeys registered with default values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}",
                                YBSettingsG.Instance.ModKeyChoice,
                                YBSettingsG.Instance.PauseKeyChoice,
                                YBSettingsG.Instance.CooldownKeyChoice,
                                YBSettingsG.Instance.SwitchKeyChoice,
                                YBSettingsG.Instance.SpecialKeyChoice);
                            break;
                    }
                    break;
            }
            YBLogger.DiagLogW("YBMoP BT: Hotkeys registered!");
        }

        public static void RemoveAllKeys()
        {
            HotkeysManager.Unregister("Pause");
            HotkeysManager.Unregister("Cooldown");
            HotkeysManager.Unregister("AoE");
            HotkeysManager.Unregister("Special");
            YBLogger.DiagLogW("YBMoP BT: Hotkeys removed!");
        }
    }
}