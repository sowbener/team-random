using System;
using YourBuddy.Core.Managers;
using YourBuddy.Core.Utilities;
using Styx.CommonBot.Routines;

namespace YourBuddy.Core.Helpers
{
    class BotEvents
    {
        public static void OnBotStarted(EventArgs args)
        {
            if (RoutineManager.Current.Name != Root.YbName) 
                return;
            HotKeyManager.RegisterKeys();
            LuaClass.DisableClickToMove();
            Logger.DiagLogPu("YourBuddy: Started! (OnBotStarted)");
        }

        public static void OnBotStopped(EventArgs args)
        {
            if (RoutineManager.Current.Name != Root.YbName) 
                return;
            HotKeyManager.RemoveAllKeys();
            LuaClass.EnableClickToMove();
            Logger.DiagLogPu("YourBuddy: Stopped! (OnBotStopped)");
        }

        public static void OnBotChanged(EventArgs args)
        {
            if (RoutineManager.Current.Name != Root.YbName)
                return;
            HotKeyManager.RemoveAllKeys();
            HotKeyManager.RegisterKeys();
            Logger.DiagLogPu("YourBuddy: Started! (OnBotChanged)");
        }
    }
}
