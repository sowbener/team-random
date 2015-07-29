using System;
using System.Windows.Media;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;

namespace Enyo.Shared
{
    class Hotkeys
    {
        /// <summary>
        ///     Global bool to see if Enyo's Paused or not.
        /// </summary>
        public static bool IsEnyoPaused { get; set; }

        /// <summary>
        ///     -
        /// </summary>
        private static bool _hkmispaused;

        /// <summary>
        ///     Used for registering hotkeys within Honorbuddy using the HB API.
        /// </summary>
        public static void RegisterKeys()
        {
            HotkeysManager.Initialize(StyxWoW.Memory.Process.MainWindowHandle);
            HotkeysManager.Register("EnyoPause", BotSettings.Instance.PauseKeyChoice, BotSettings.Instance.ModKeyChoice, hk => Paused());
        }

        /// <summary>
        ///     Used to unregister hotkeys using the HB API (Toggle Type).
        /// </summary>
        public static void RemoveKeys()
        {
            HotkeysManager.Unregister("EnyoPause");
        }

        /// <summary>
        ///     Used for reregistering hotkeys within Honorbuddy using the HB API.
        /// </summary>
        public static void ReloadRegisterKeys()
        {
            RemoveKeys();
            RegisterKeys();
        }

        /// <summary>
        ///     Actual Pause functionality.
        /// </summary>
        private static void Paused()
        {
            _hkmispaused = !_hkmispaused;

            if (BotSettings.Instance.UseOverlayOutput)
            {
                if (_hkmispaused)
                {
                    StyxWoW.Overlay.AddToast(new ToastLog(() =>
                        "Enyo: Rotation Paused!", TimeSpan.FromSeconds(BotSettings.Instance.OverlayOutputTime), Colors.Red, Colors.Black, new FontFamily("Century Gothic"), 25));
                }

                if (!_hkmispaused)
                {
                    StyxWoW.Overlay.AddToast(new ToastLog(() =>
                        "Enyo: Rotation Resumed!", TimeSpan.FromSeconds(BotSettings.Instance.OverlayOutputTime), Colors.Green, Colors.Black, new FontFamily("Century Gothic"), 25));
                }
            }

            Logger.PauseLog(_hkmispaused ? "Rotation Paused!" : "Rotation Resumed!");
            Enyo.IsPaused = _hkmispaused;
            TreeRoot.TicksPerSecond = CharacterSettings.Instance.TicksPerSecond;
        }
    }
}
