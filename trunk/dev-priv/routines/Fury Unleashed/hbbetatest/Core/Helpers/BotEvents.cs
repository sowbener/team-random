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

            /* Disable Ingame Settings */
            LuaClass.DisableScriptErrors();
            LuaClass.DisableClickToMove();

            /* Initialize Hotkey Functions */
            HotKeyManager.RegisterKeys();
            HotKeyManager.InitializeBindings();

            /* Initialize Routine Functions */
            DamageTracker.Initialize();
            Unit.InitializeSmartTaunt();

            Logger.DiagLogPu("Fury Unleashed: Started! (OnBotStarted)");
        }

        public static void OnBotStopped(EventArgs args)
        {
            if (RoutineManager.Current.Name != Root.FuName)
            {
                return;
            }

            /* Enable Ingame Settings */
            LuaClass.EnableScriptErrors();
            LuaClass.EnableClickToMove();

            /* Removing Hotkey Functions */
            HotKeyManager.RemoveAllKeys();

            /* Stopping Routine Functions */
            DamageTracker.Stop();

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
