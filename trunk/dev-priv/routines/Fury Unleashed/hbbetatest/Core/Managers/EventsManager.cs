using Styx.WoWInternals;

namespace FuryUnleashed.Core.Managers
{
    class EventsManager
    {
        static EventsManager()
        {
            Lua.Events.AttachEvent("GROUP_ROSTER_UPDATE", UpdateRaidLists);
        }

        private static void UpdateRaidLists(object sender, LuaEventArgs args)
        {
            Unit.UpdateRaidLists();
        }
    }
}
