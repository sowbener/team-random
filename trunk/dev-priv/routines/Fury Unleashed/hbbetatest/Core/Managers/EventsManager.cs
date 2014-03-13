using FuryUnleashed.Core.Utilities;
using Styx.WoWInternals;

namespace FuryUnleashed.Core.Managers
{
    internal static class EventsManager
    {
        static EventsManager()
        {
            Logger.DiagLogWh("[FU] Initialized Events (Handler UpdateRaidLists)");
            Lua.Events.AttachEvent("GROUP_ROSTER_UPDATE", UpdateRaidLists);
            Lua.Events.AttachEvent("PARTY_CONVERTED_TO_RAID", UpdateRaidLists);
            Lua.Events.AttachEvent("RAID_ROSTER_UPDATE", UpdateRaidLists);
            Lua.Events.AttachEvent("ROLE_CHANGED_INFORM", UpdateRaidLists);

            Logger.DiagLogWh("[FU] Initialized Events (Handler UpdateTalentManager)");
            Lua.Events.AttachEvent("ACTIVE_TALENT_GROUP_CHANGED", TalentManager.UpdateTalentManager);
            Lua.Events.AttachEvent("CHARACTER_POINTS_CHANGED", TalentManager.UpdateTalentManager);
            Lua.Events.AttachEvent("GLYPH_UPDATED", TalentManager.UpdateTalentManager);
            Lua.Events.AttachEvent("LEARNED_SPELL_IN_TAB", TalentManager.UpdateTalentManager);
            Lua.Events.AttachEvent("PLAYER_LEVEL_UP", TalentManager.UpdateTalentManager);
            Lua.Events.AttachEvent("PLAYER_SPECIALIZATION_CHANGED", TalentManager.UpdateTalentManager);
        }

        private static void UpdateRaidLists(object sender, LuaEventArgs args)
        {
            Unit.UpdateRaidLists();
        }

        public static void Initialize() 
        { }
    }
}
