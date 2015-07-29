using System.Timers;
using Styx.WoWInternals;

namespace Enyo.Shared
{
    class ClicktoMove
    {
        /// <summary>
        ///     This runs the RunClickToMove() void after a set time.
        /// </summary>
        /// <param name="tickingtime">Ticking time in MS.</param>
        public static void ClickToMove(int tickingtime)
        {
            _enyotimer = new Timer(tickingtime);
            _enyotimer.Elapsed += OnTimedEvent;
            _enyotimer.AutoReset = false;
            _enyotimer.Enabled = true;
        }

        private static Timer _enyotimer;

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            RunClickToMove();
        }

        /// <summary>
        ///     Enables or Disables CTM when this void is executed.
        /// </summary>
        public static void RunClickToMove()
        {
            Lua.DoString(!BotSettings.Instance.ClickToMove
                ? "SetCVar('autoInteract', '0')"
                : "SetCVar('autoInteract', '1')");
        }
    }
}
