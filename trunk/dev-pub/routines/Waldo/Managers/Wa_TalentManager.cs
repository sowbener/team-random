using Waldo.Helpers;
using Styx;
using Styx.WoWInternals;
using System;
using System.Collections.Generic;
using System.Linq;

// Credits for this code go out to the PureRotation Team!
namespace Waldo.Managers
{
    internal static class WaTalentManager
    {
        static WaTalentManager()
        {
            Talents = new List<Talent>();
            Glyphs = new HashSet<string>();
            Lua.Events.AttachEvent("CHARACTER_POINTS_CHANGED", UpdateTalentManager);
            Lua.Events.AttachEvent("GLYPH_UPDATED", UpdateTalentManager);
            Lua.Events.AttachEvent("ACTIVE_TALENT_GROUP_CHANGED", UpdateTalentManager);
            Lua.Events.AttachEvent("LEARNED_SPELL_IN_TAB", UpdateTalentManager);

        }

        public static WoWSpec CurrentSpec { get; private set; }
        private static List<Talent> Talents { get; set; }
        private static HashSet<string> Glyphs { get; set; }

        public static bool HasGlyph(string glyphName)
        {
            return Glyphs.Count > 0 && Glyphs.Contains(glyphName);
        }

        public static bool HasTalent(int index)
        {
            return Talents.FirstOrDefault(t => t.Index == index).Count != 0;
        }

        private static void UpdateTalentManager(object sender, LuaEventArgs args)
        {
            Update();
        }

        public static void Update()
        {
            // Don't bother if we're < 10
            if (StyxWoW.Me.Level < 10)
            {
                CurrentSpec = WoWSpec.None;
                return;
            }

            // Keep the frame stuck so we can do a bunch of injecting at once.
            using (StyxWoW.Memory.AcquireFrame())
            {
                CurrentSpec = StyxWoW.Me.Specialization;
                Talents.Clear();

                // Always 18 talents. 6 rows of 3 talents.
                for (int index = 0; index <= 6 * 3; index++)
                {
                    var selected = Lua.GetReturnVal<int>(string.Format("return GetTalentInfo({0})", index), 4);
                    switch (selected)
                    {
                        case 1:
                            {
                                var t = new Talent { Index = index, Count = 1 }; //Name = talentName
                                Talents.Add(t);
                            }
                            break;
                    }
                }
            }

            Glyphs.Clear();
            var glyphCount = Lua.GetReturnVal<int>("return GetNumGlyphSockets()", 0);

            WaLogger.DiagLogW("Glyphdetection - GetNumGlyphSockets {0}", glyphCount);

            if (glyphCount != 0)
            {

                for (int i = 1; i <= glyphCount; i++)
                {
                    var lua = String.Format("local enabled, glyphType, glyphTooltipIndex, glyphSpellID, icon = GetGlyphSocketInfo({0});if (enabled) then return glyphSpellID else return 0 end", i);
                    var glyphSpellId = Lua.GetReturnVal<int>(lua, 0);
                    try
                    {
                        if (glyphSpellId > 0)
                        {
                            WaLogger.DiagLogW("Glyphdetection - SpellId: {0},Name:{1} ,WoWSpell: {2}", glyphSpellId, WoWSpell.FromId(glyphSpellId).Name, WoWSpell.FromId(glyphSpellId));
                            Glyphs.Add(WoWSpell.FromId(glyphSpellId).Name.Replace("Glyph of ", ""));
                        }
                        else
                        {
                            WaLogger.DiagLogW("Glyphdetection - Couldn't find all values to detect the Glyph in slot {0}", i);
                        }
                    }
                    catch (Exception ex)
                    {
                        WaLogger.DiagLogW("We couldn't detect your Glyphs");
                        WaLogger.DiagLogW("Report this message to us: " + ex);
                    }
                }
            }
        }

        public static void Dumpglyphs()
        {
            foreach (var glyph in Glyphs)
            {
                WaLogger.DiagLogW("{0}", glyph);
            }
        }

        #region Nested type: Talent

        private struct Talent
        {
            public int Count;
            public int Index;
        }
        #endregion
    }
}