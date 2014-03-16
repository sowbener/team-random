using System;
using YourRaidingBuddy.Core.Managers;
using YourRaidingBuddy.Core.Utilities;
using Styx.CommonBot.Routines;

namespace YourRaidingBuddy.Core.Helpers
{
    class BotEvents
    {
        public static void OnBotStarted(EventArgs args)
        {
            if (RoutineManager.Current.Name != Root.YbName) 
                return;
            HotKeyManager.RegisterKeys();
            LuaClass.DisableClickToMove();
            Logger.DiagLogPu("YourRaidingBuddy: Started! (OnBotStarted)");
        }

        public static void OnBotStopped(EventArgs args)
        {
            if (RoutineManager.Current.Name != Root.YbName) 
                return;
            HotKeyManager.RemoveAllKeys();
            LuaClass.EnableClickToMove();
            Logger.DiagLogPu("YourRaidingBuddy: Stopped! (OnBotStopped)");
        }

        public static void OnBotChanged(EventArgs args)
        {
            if (RoutineManager.Current.Name != Root.YbName)
                return;
            HotKeyManager.RemoveAllKeys();
            HotKeyManager.RegisterKeys();
            Logger.DiagLogPu("YourRaidingBuddy: Started! (OnBotChanged)");
        }
    }
}
