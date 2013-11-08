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
        public static bool RaiseAllyKey { get; private set; }
        public static bool Tier6AbilitiesKey { get; private set; }
        public static bool ArmyofTheDeadKey { get; private set; }
        public static bool AMZKey { get; private set; }


        public delegate T Selection<out T>(object context);

        public static void BotEvents_OnBotStarted(EventArgs args)
        {
            if (RoutineManager.Current.Name != DvMain.DvName) return;
            if (DvSettingsH.Instance.ModeSelection == DvEnum.Mode.Abilities || DvSettingsH.Instance.ModeSelection == DvEnum.Mode.Hotkey)
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

        public static void OnBotChanged(EventArgs args)
        {
            if (RoutineManager.Current.Name != DvMain.DvName)
                return;
            DvHotKeyManager.RemoveAllKeys();
            DvHotKeyManager.RegisterKeys();
            DvLogger.DiagLogW("Death Vader: Started! (OnBotChanged)");
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

            if (DvSettingsH.Instance.ModeSelection == DvEnum.Mode.Abilities || DvSettingsH.Instance.ModeSelection == DvEnum.Mode.Hotkey)
            {           
                            
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

                            HotkeysManager.Register("Tier6AbilitiesKey", DvSettingsH.Instance.Tier6, DvSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                Tier6AbilitiesKey = !Tier6AbilitiesKey;
                                LogKey("Tier6", DvSettingsH.Instance.Tier6, DvSettingsH.Instance.ModKeyChoice, Tier6AbilitiesKey);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Tier 6 Ability \124cFF15E61C Enabled!')"
                                                     : @"print('Tier 6 Ability \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("ArmyofTheDeadKey", DvSettingsH.Instance.ArmyofTheDeadKey, DvSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                ArmyofTheDeadKey = !ArmyofTheDeadKey;
                                LogKey("ArmyofTheDeadKey", DvSettingsH.Instance.ArmyofTheDeadKey, DvSettingsH.Instance.ModKeyChoice, ArmyofTheDeadKey);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Army of the Dead \124cFF15E61C Enabled!')"
                                                     : @"print('Army of the Dead \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("RaiseAllyKey", DvSettingsH.Instance.RaiseAlly, DvSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                RaiseAllyKey = !RaiseAllyKey;
                                LogKey("RaiseAllyKey", DvSettingsH.Instance.RaiseAlly, DvSettingsH.Instance.ModKeyChoice, RaiseAllyKey);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Raise Ally \124cFF15E61C Enabled!')"
                                                     : @"print('Raise Ally \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AMZKey", DvSettingsH.Instance.AMZ, DvSettingsH.Instance.ModKeyChoice, hk =>
                            {
                                AMZKey = !AMZKey;
                                LogKey("AMZKey", DvSettingsH.Instance.AMZ, DvSettingsH.Instance.ModKeyChoice, AMZKey);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('AMZ \124cFF15E61C Enabled!')"
                                                     : @"print('AMZ \124cFFE61515 Disabled!')");
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
                            DvLogger.DiagLogW("Death Vader: Hotkeys registered with individual values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}, AMZ {5}, ArmyofTheDead {6}, Raise Ally {7}, Tier6 {8}",
                                DvSettingsH.Instance.ModKeyChoice,
                                DvSettingsH.Instance.PauseKeyChoice,
                                DvSettingsH.Instance.CooldownKeyChoice,
                                DvSettingsH.Instance.MultiTgtKeyChoice,
                                DvSettingsH.Instance.SpecialKeyChoice,
                                DvSettingsH.Instance.AMZ,
                                DvSettingsH.Instance.ArmyofTheDeadKey,
                                DvSettingsH.Instance.RaiseAlly,
                                DvSettingsH.Instance.Tier6);
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


                            HotkeysManager.Register("Tier6AbilitiesKey", DvSettingsH.Instance.Tier6, ModifierKeys.Alt, hk =>
                            {
                                Tier6AbilitiesKey = !Tier6AbilitiesKey;
                                LogKey("Tier6", DvSettingsH.Instance.Tier6, ModifierKeys.Alt, Tier6AbilitiesKey);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Tier 6 Ability \124cFF15E61C Enabled!')"
                                                     : @"print('Tier 6 Ability \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("ArmyofTheDeadKey", DvSettingsH.Instance.ArmyofTheDeadKey, ModifierKeys.Alt, hk =>
                            {
                                ArmyofTheDeadKey = !ArmyofTheDeadKey;
                                LogKey("ArmyofTheDeadKey", DvSettingsH.Instance.ArmyofTheDeadKey, ModifierKeys.Alt, ArmyofTheDeadKey);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Army of the Dead \124cFF15E61C Enabled!')"
                                                     : @"print('Army of the Dead \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("RaiseAllyKey", DvSettingsH.Instance.RaiseAlly, ModifierKeys.Alt, hk =>
                            {
                                RaiseAllyKey = !RaiseAllyKey;
                                LogKey("RaiseAllyKey", DvSettingsH.Instance.RaiseAlly, ModifierKeys.Alt, RaiseAllyKey);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('Raise Ally \124cFF15E61C Enabled!')"
                                                     : @"print('Raise Ally \124cFFE61515 Disabled!')");
                            });

                            HotkeysManager.Register("AMZKey", DvSettingsH.Instance.AMZ, ModifierKeys.Alt, hk =>
                            {
                                AMZKey = !AMZKey;
                                LogKey("AMZKey", DvSettingsH.Instance.AMZ, ModifierKeys.Alt, AMZKey);
                                if (DvSettings.Instance.General.EnableWoWChatOutput && !IsPaused)
                                    Lua.DoString(IsCooldown
                                                     ? @"print('AMZ \124cFF15E61C Enabled!')"
                                                     : @"print('AMZ \124cFFE61515 Disabled!')");
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
                            DvLogger.DiagLogW("Death Vader: Hotkeys registered with default values. Configured ModifierKey: {0}, PauseKey: {1}, CooldownKey: {2}, AoEKey: {3}, SpecialKey: {4}, AMZ {5}, ArmyofTheDead {6}, Raise Ally {7}, Tier6 {8}",
                                DvSettingsH.Instance.ModKeyChoice,
                                DvSettingsH.Instance.PauseKeyChoice,
                                DvSettingsH.Instance.CooldownKeyChoice,
                                DvSettingsH.Instance.MultiTgtKeyChoice,
                                DvSettingsH.Instance.SpecialKeyChoice,
                                DvSettingsH.Instance.AMZ,
                                DvSettingsH.Instance.ArmyofTheDeadKey,
                                DvSettingsH.Instance.RaiseAlly,
                                DvSettingsH.Instance.Tier6);
                                  
                    }
            }
            //DvLogger.DiagLogW("Death Vader: Hotkeys registered!");      

        public static void RemoveAllKeys()
        {
            HotkeysManager.Unregister("Pause");
            HotkeysManager.Unregister("Cooldown");
            HotkeysManager.Unregister("AoE");
            HotkeysManager.Unregister("Special");
            HotkeysManager.Unregister("AMZKey");
            HotkeysManager.Unregister("RaiseAllyKey");
            HotkeysManager.Unregister("ArmyofTheDeadKey");
            HotkeysManager.Unregister("Tier6");
            DvLogger.DiagLogW("Death Vader: Hotkeys removed!");
        }
    }
}