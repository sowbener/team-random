using System;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Core.Utilities;
using Styx.CommonBot.Routines;

namespace FuryUnleashed.Core.Helpers
{
    class BotEvents
    {
        public static void OnBotStarted(EventArgs args)
        {
            if (RoutineManager.Current.Name != Root.FuName)
            {
                return;
            }

            /* Initialize Hotkey Functions */
            HotKeyManager.InitializeBindings();
            HotKeyManager.RegisterKeys();

            /* Initialize Routine Functions */
            DamageTracker.Initialize();
            Unit.InitializeSmartTaunt();

            /* Disable Ingame Settings */
            LuaClass.DisableScriptErrors();
            LuaClass.DisableClickToMove();

            Logger.DiagLogPu("Fury Unleashed: Started! (OnBotStarted)");
        }

        public static void OnBotStopped(EventArgs args)
        {
            if (RoutineManager.Current.Name != Root.FuName)
            {
                return;
            }

            /* Removing Hotkey Functions */
            HotKeyManager.RemoveAllKeys();

            /* Stopping Routine Functions */
            DamageTracker.Stop();

            /* Enable Ingame Settings */
            LuaClass.EnableScriptErrors();
            LuaClass.EnableClickToMove();

            Logger.DiagLogPu("Fury Unleashed: Stopped! (OnBotStopped)");
        }

        public static void OnBotChanged(EventArgs args)
        {
            if (RoutineManager.Current.Name != Root.FuName)
            {
                return;
            }

            /* Restarting Hotkey Functions */
            HotKeyManager.RemoveAllKeys();
            HotKeyManager.RegisterKeys();

            Logger.DiagLogPu("Fury Unleashed: Started! (OnBotChanged)");
        }
    }
}
