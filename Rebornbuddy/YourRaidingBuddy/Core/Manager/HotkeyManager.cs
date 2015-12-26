﻿using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Managers
{
    internal static class HotkeyManager
    {
        /// <summary>
        ///     Initializing the Hotkeymanager when Hotkeymodes are enabled.
        /// </summary>
        internal static void Initialize(bool reinitialized = false)
        {
            try
            {
                if (InternalSettings.Instance.Hotkeys.SpecialKey != Keys.None || InternalSettings.Instance.Hotkeys.SpecialKey1 != Keys.None || InternalSettings.Instance.Hotkeys.PauseKey != Keys.None)
                    RegisterAutoMode(reinitialized);

                if (InternalSettings.Instance.Hotkeys.HotkeyModeSelection == HotkeyMode.HotkeyMode || InternalSettings.Instance.Hotkeys.HotkeyModeSelection == HotkeyMode.SemiHotkeyMode)
                    RegisterKeys(reinitialized);
            }
            catch (Exception ex)
            {
                Logger.WriteDebug("HotkeyManager.Initialize() Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        ///     Used for registering hotkeys using the HB API (Toggle Type)
        /// </summary>
        private static void RegisterKeys(bool reinitialized)
        {
            if (reinitialized)
                Stop(true);

            if (InternalSettings.Instance.Hotkeys.CooldownKey != Keys.None)
                ff14bot.Managers.HotkeyManager.Register("YrbCooldown", InternalSettings.Instance.Hotkeys.CooldownKey, InternalSettings.Instance.Hotkeys.ModifierKey, hk => Cooldowns());

            if (InternalSettings.Instance.Hotkeys.SpecialKey != Keys.None)
                ff14bot.Managers.HotkeyManager.Register("YrbSpecialKey", InternalSettings.Instance.Hotkeys.SpecialKey, InternalSettings.Instance.Hotkeys.ModifierKey, hk => SpecialKey());

            if (InternalSettings.Instance.Hotkeys.SpecialKey1 != Keys.None)
                ff14bot.Managers.HotkeyManager.Register("YrbSpecialKey1", InternalSettings.Instance.Hotkeys.SpecialKey1, InternalSettings.Instance.Hotkeys.ModifierKey, hk => SpecialKey1());

            if (InternalSettings.Instance.Hotkeys.MultiTargetKey != Keys.None)
                ff14bot.Managers.HotkeyManager.Register("YrbMultiTarget", InternalSettings.Instance.Hotkeys.MultiTargetKey, InternalSettings.Instance.Hotkeys.ModifierKey, hk => MultiTarget());

            if (InternalSettings.Instance.Hotkeys.PauseKey != Keys.None)
                ff14bot.Managers.HotkeyManager.Register("YrbPause", InternalSettings.Instance.Hotkeys.PauseKey, InternalSettings.Instance.Hotkeys.ModifierKey, hk => Paused());


            Logger.WriteDebug(
                reinitialized
                    ? "Hotkeys reregistered: {0} as Pause Key - {1} as Cooldown Key - {2} as MultiTarget Key - {3} as SpecialKey - {4} as SpecialKey1 - {5} as Modifier Key."
                    : "Hotkeys registered: {0} as Pause Key - {1} as Cooldown Key - {2} as MultiTarget Key - {3} as SpecialKey - {4} as SpecialKey1 - {5} as Modifier Key.",
                InternalSettings.Instance.Hotkeys.PauseKey,
                InternalSettings.Instance.Hotkeys.CooldownKey,
                InternalSettings.Instance.Hotkeys.MultiTargetKey,
                InternalSettings.Instance.Hotkeys.SpecialKey,
                InternalSettings.Instance.Hotkeys.SpecialKey1,
                InternalSettings.Instance.Hotkeys.ModifierKey);
        }

        private static void RegisterAutoMode(bool reinitialized)
        {
            if (reinitialized)
                Stop(true);

            if (InternalSettings.Instance.Hotkeys.SpecialKey != Keys.None)
                ff14bot.Managers.HotkeyManager.Register("YrbSpecialKey", InternalSettings.Instance.Hotkeys.SpecialKey, InternalSettings.Instance.Hotkeys.ModifierKey, hk => SpecialKey());

            if (InternalSettings.Instance.Hotkeys.SpecialKey1 != Keys.None)
                ff14bot.Managers.HotkeyManager.Register("YrbSpecialKey1", InternalSettings.Instance.Hotkeys.SpecialKey1, InternalSettings.Instance.Hotkeys.ModifierKey, hk => SpecialKey1());

            if (InternalSettings.Instance.Hotkeys.PauseKey != Keys.None)
                ff14bot.Managers.HotkeyManager.Register("YrbPause", InternalSettings.Instance.Hotkeys.PauseKey, InternalSettings.Instance.Hotkeys.ModifierKey, hk => Paused());


            Logger.WriteDebug(
                reinitialized
                    ? "Hotkeys reregistered: {0} as SpecialKey - {1} as SpecialKey1 - {2} as PauseKey - {3} as ModifierKey"
                    : "Hotkeys registered: {0} as SpecialKey - {1} as SpecialKey1 - {2} as PauseKey - {3} as ModifierKey",
                InternalSettings.Instance.Hotkeys.SpecialKey,
                InternalSettings.Instance.Hotkeys.SpecialKey1,
                InternalSettings.Instance.Hotkeys.PauseKey,
                InternalSettings.Instance.Hotkeys.ModifierKey);
        }

        private static void Cooldowns()
        {
            {
                try
                {
                    VariableBook.HkmCooldowns = !VariableBook.HkmCooldowns;

                    Logger.Write(VariableBook.HkmCooldowns
                        ? @"HotkeyManager: Cooldowns enabled."
                        : @"HotkeyManager: Cooldowns disabled.");

                    VariableBook.OverlayRefreshCooldowns = true;

                    if (InternalSettings.Instance.Hotkeys.CooldownKey != Keys.None &&
                        InternalSettings.Instance.Hotkeys.HotkeyChatOutput && !VariableBook.HkmPaused)
                    {
                        if (VariableBook.HkmCooldowns)
                        {
                            ff14bot.Core.OverlayManager.AddToast(new Logger.ToastLog(() =>
                                "YRB: Cooldowns enabled!", TimeSpan.FromSeconds(3), Colors.Green, Colors.Black,
                                new FontFamily("Century Gothic"), 25));
                        }

                        if (!VariableBook.HkmCooldowns)
                        {
                            ff14bot.Core.OverlayManager.AddToast(new Logger.ToastLog(() =>
                                "YRB: Cooldowns disabled!", TimeSpan.FromSeconds(3), Colors.Red, Colors.Black,
                                new FontFamily("Century Gothic"), 25));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteDebug("HotkeyManager.ApiCooldownsHotkey() Exception: {0}", ex);
                }
            }
        }

        private static void MultiTarget()
        {
            {
                try
                {
                    VariableBook.HkmMultiTarget = !VariableBook.HkmMultiTarget;

                    Logger.Write(VariableBook.HkmMultiTarget
                        ? @"HotkeyManager: AoE enabled."
                        : @"HotkeyManager: AoE disabled.");

                    VariableBook.OverlayRefreshMultiTarget = true;

                    if (InternalSettings.Instance.Hotkeys.MultiTargetKey != Keys.None &&
                        InternalSettings.Instance.Hotkeys.HotkeyChatOutput && !VariableBook.HkmPaused)
                    {
                        if (VariableBook.HkmMultiTarget)
                        {
                            ff14bot.Core.OverlayManager.AddToast(new Logger.ToastLog(() =>
                                "YRB: MultiTarget enabled!", TimeSpan.FromSeconds(3), Colors.Green, Colors.Black,
                                new FontFamily("Century Gothic"), 25));
                        }

                        if (!VariableBook.HkmCooldowns)
                        {
                            ff14bot.Core.OverlayManager.AddToast(new Logger.ToastLog(() =>
                                "YRB: MultiTarget disabled!", TimeSpan.FromSeconds(3), Colors.Red, Colors.Black,
                                new FontFamily("Century Gothic"), 25));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteDebug("HotkeyManager.ApiMultitargetHotkey() Exception: {0}", ex);
                }
            }

        }

        private static void Paused()
        {
            VariableBook.HkmPaused = !VariableBook.HkmPaused;

            Logger.Write(VariableBook.HkmPaused
                ? @"Rotation paused."
                : @"Rotation resumed.");

        }

        private static void SpecialKey()
        {
            VariableBook.HkmSpecialKey = !VariableBook.HkmSpecialKey;

        }

        private static void SpecialKey1()
        {
            VariableBook.HkmSpecialKey1 = !VariableBook.HkmSpecialKey1;
        }


        /// <summary>
        ///     Used to unregister hotkeys using the HB API (Toggle Type).
        /// </summary>
        internal static void Stop(bool reinitialized = false)
        {
            ff14bot.Managers.HotkeyManager.Unregister("YrbCooldown");
            ff14bot.Managers.HotkeyManager.Unregister("YrbMultiTarget");
            ff14bot.Managers.HotkeyManager.Unregister("YrbPause");
            ff14bot.Managers.HotkeyManager.Unregister("YrbSpecialKey");
            ff14bot.Managers.HotkeyManager.Unregister("YrbSpecialKey1");

            Logger.WriteDebug(
                reinitialized
                    ? "HotkeyManager is rebuilding."
                    : "HotkeyManager is stopped. Hotkeys are removed!");
        }


        /// <summary>
        ///     KEYSTATE function.
        /// </summary>
        /// <param name="keyCode">
        /// </param>
        /// <returns>
        ///     The <see cref="short" />.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetAsyncKeyState(int keyCode);

        /// <summary>
        ///     The get key state.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="EnumBook.KeyStates" />.
        /// </returns>
        internal static KeyStates GetAsyncKeyState(Keys key)
        {
            var state = KeyStates.None;

            short retVal = GetAsyncKeyState((int)key);
            if ((retVal & 0x8000) == 0x8000) state |= KeyStates.Down;

            return state;
        }

        /// <summary>
        ///     The is key down.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        internal static bool IsKeyDown(Keys key)
        {
            if (key == Keys.None) return false;
            Keys keyCode = key & Keys.KeyCode;
            int mods = 0;
            var modds = new List<Keys>();

            if ((key & Keys.Shift) != 0)
            {
                mods += 1;
                modds.Add(Keys.ShiftKey);
            }

            if ((key & Keys.Alt) != 0)
            {
                mods += 1;
                modds.Add(Keys.Menu);
            }

            if ((key & Keys.Control) != 0)
            {
                mods += 1;
                modds.Add(Keys.ControlKey);
            }

            return mods != 0
                       ? modds.TrueForAll(k => (KeyStates.Down == (GetAsyncKeyState(k) & KeyStates.Down))) && KeyStates.Down == (GetAsyncKeyState(keyCode) & KeyStates.Down)
                       : KeyStates.Down == (GetAsyncKeyState(keyCode) & KeyStates.Down);
        }

        /// <summary>
        ///     New.. supposed to be faster
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        internal static bool IsAnyKeyDown(IEnumerable<Keys> key)
        {
            return key.Any(IsKeyDown);
        }
   
    }
}