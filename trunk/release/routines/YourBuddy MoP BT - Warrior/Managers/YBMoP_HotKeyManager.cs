using System.Runtime.InteropServices;
using Styx.Common;
using Styx.CommonBot.Routines;
using Styx.WoWInternals;
using System;
using System.Windows.Forms;
using YBMoP_BT_Warrior.Helpers;
using YBMoP_BT_Warrior.Interfaces.Settings;

namespace YBMoP_BT_Warrior.Managers
{
    public static class HotKeyManager
    {
        public static bool IsPaused { get; private set; }
        public static bool IsCooldown { get; private set; }
        public static bool IsAoe { get; private set; }
        public static bool IsSpecialKey { get; private set; }

        public static bool IsDemoBanner { get; private set; }
        public static bool IsHeroicLeap { get; private set; }

        public static void BotEvents_OnBotStarted(EventArgs args)
        {
            if (RoutineManager.Current.Name != YBMain.YBName) return;
            if (YBSettingsG.Instance.ModeChoice == YBEnum.Mode.Hotkey)
                RegisterKeys();
            //RegisterActionKeys();
            YBLua.DisableClickToMove();
        }

        public static void BotEvents_OnBotStopped(EventArgs args)
        {
            if (RoutineManager.Current.Name != YBMain.YBName) return;
                RemoveAllKeys();
            //RemoveActionKeys();
            YBLua.EnableClickToMove();
            YBLogger.DiagLogW("YBMoP BT: stopped!");
        }

        private static void LogKey(string kType, Keys kValue, ModifierKeys kModifier, bool kResult)
        {
            YBLogger.DiagLogW("{0}-key ({1} + {2}) pressed, set to: {3}", kType, kValue, kModifier, kResult);
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);
        public static bool IsKeyDown(Keys vKey)
        {
            return (GetAsyncKeyState(vKey)) != 0;
        }

        public static void RegisterActionKeys()
        {
            HotkeysManager.Register("DemoBanner", YBSettingsG.Instance.DemoBannerChoice, YBSettingsG.Instance.ModKeyChoice, hk =>
            {
                IsDemoBanner = !IsDemoBanner;
                LogKey("DemoBanner", YBSettingsG.Instance.HeroicLeapChoice, YBSettingsG.Instance.ModKeyChoice, IsDemoBanner);
            });

            HotkeysManager.Register("HeroicLeap", YBSettingsG.Instance.HeroicLeapChoice, YBSettingsG.Instance.ModKeyChoice, hk =>
            {
                IsHeroicLeap = !IsHeroicLeap;
                LogKey("HeroicLeap", YBSettingsG.Instance.HeroicLeapChoice, YBSettingsG.Instance.ModKeyChoice, IsHeroicLeap);
            });
            YBLogger.DiagLogW("YBMoP BT: Actionkeys registered. Configured Demoralizing Banner Key: {0} & Heroic Leap Key: {1}",
                YBSettingsG.Instance.DemoBannerChoice,
                YBSettingsG.Instance.HeroicLeapChoice);
        }

        public static void RegisterKeys()
        {
            switch (YBSettingsG.Instance.ModeChoice)
            {
                case YBEnum.Mode.Hotkey:
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

        public static void RemoveActionKeys()
        {
            HotkeysManager.Unregister("DemoBanner");
            HotkeysManager.Unregister("HeroicLeap");
            YBLogger.DiagLogW("YBMoP BT: Actionkeys removed!");            
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