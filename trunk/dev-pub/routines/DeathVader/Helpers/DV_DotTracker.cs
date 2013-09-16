
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Lua = DeathVader.Helpers.DvLua;
using Logger = DeathVader.Helpers.DvLogger;

namespace DeathVader.Helpers
{
    [UsedImplicitly]
    internal static class DoTTracker
    {
        #region structures

        public struct StatInfo
        {
            public int SpellId;
            public float AttackPower;
            public float CritChance;
            public float Mastery;
            public float Intellect;
            public float SpellPower;
            public float MeleeHaste;
            public float SpellHaste;

            public DateTime TimeTracked { get; set; }

            /// <summary>
            /// Will create an info with empty stats
            /// </summary>
            public StatInfo(int SpellId, float AttackPower, float CritChance, float Mastery, float Intellect, float SpellPower, float MeleeHaste, float SpellHaste, DateTime TimeTracked)
                : this()
            {
                this.SpellId = 0;
                this.AttackPower = 0;
                this.CritChance = 0;
                this.Mastery = 0;
                this.Intellect = 0;
                this.SpellPower = 0;
                this.MeleeHaste = 0;
                this.SpellHaste = 0;
                this.TimeTracked = new DateTime();
            }
        }

        #endregion structures

        # region variables

        public static bool Initialized;

        private static readonly Dictionary<int, StatInfo> StatInfos = new Dictionary<int, StatInfo>();

        private static bool _combatLogAttached;

        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        public static void Initialize()
        {
            AttachCombatLogEvent();
            Initialized = true;
        }

        public static void Pulse()
        {
        }

        public static void Shutdown()
        {
            if (!Initialized) return;
            StatInfos.Clear();
            Initialized = false;
        }

        private static void UpdateStatInfo(ulong destGuid, int spellId)
        {
            // Add the new damage taken data
            try
            {
                Lua._secondaryStats.Refresh();
                var hash = SpellUnitHash(destGuid, spellId);
                if (!StatInfos.ContainsKey(hash))
                {
                    var di = new StatInfo { AttackPower = Lua._secondaryStats.AttackPower, Intellect = Lua._secondaryStats.Intellect, SpellPower = Lua._secondaryStats.SpellPower, TimeTracked = DateTime.Now, CritChance = Lua._secondaryStats.Crit, MeleeHaste = Lua._secondaryStats.MeleeHaste, SpellHaste = Lua._secondaryStats.SpellHaste, Mastery = Lua._secondaryStats.Mastery };
                    Logger.DebugLog("Added {0} with stats AP {1}, Int {2}, Crit {3}, MeleeHaste {4}, SpellHaste {5}, Mastery {6}, SpellPower {7}",
                        spellId,
                        Lua._secondaryStats.AttackPower,
                        Lua._secondaryStats.Intellect,
                        Lua._secondaryStats.Crit,
                        Lua._secondaryStats.MeleeHaste,
                        Lua._secondaryStats.SpellHaste,
                        Lua._secondaryStats.Mastery,
                        Lua._secondaryStats.SpellPower);
                    StatInfos.Add(hash, di);
                }
                else
                {
                    Logger.DebugLog("Updated     {0} with stats AP {1}, Int {2}, Crit {3}, MeleeHaste {4}, SpellHaste {5}, Mastery {6}, SpellPower {7}",
                        spellId,
                        Lua._secondaryStats.AttackPower,
                        Lua._secondaryStats.Intellect,
                        Lua._secondaryStats.Crit,
                        Lua._secondaryStats.MeleeHaste,
                        Lua._secondaryStats.SpellHaste,
                        Lua._secondaryStats.Mastery,
                        Lua._secondaryStats.SpellPower);
                    StatInfo di = StatInfos[hash];
                    di.TimeTracked = DateTime.Now;
                    di.SpellId = spellId;
                    di.AttackPower = Lua._secondaryStats.AttackPower;
                    di.Intellect = Lua._secondaryStats.Intellect;
                    di.TimeTracked = DateTime.Now;
                    di.CritChance = Lua._secondaryStats.Crit;
                    di.MeleeHaste = Lua._secondaryStats.MeleeHaste;
                    di.SpellHaste = Lua._secondaryStats.SpellHaste;
                    di.Mastery = Lua._secondaryStats.Mastery;
                    di.SpellPower = Lua._secondaryStats.SpellPower;
                    StatInfos[hash] = di;
                }
            }
            catch (Exception ex)
            {
                Logger.DebugLog("UpdateStatInfo : {0}", ex);
            }
        }

        public static StatInfo SpellStats(WoWUnit u, int spellId)
        {
            if (StatInfos.ContainsKey(SpellUnitHash(u, spellId)))
            {
                return StatInfos[SpellUnitHash(u, spellId)];
            }
            return new StatInfo();
        }

        private static void AttachCombatLogEvent()
        {
            if (_combatLogAttached)
                return;
            Styx.WoWInternals.Lua.Events.AttachEvent("COMBAT_LOG_EVENT_UNFILTERED", HandleCombatLog);
            if (
                !Styx.WoWInternals.Lua.Events.AddFilter(
                    "COMBAT_LOG_EVENT_UNFILTERED",
                    "return args[2] == 'SPELL_AURA_APPLIED' or args[2] == 'SPELL_AURA_REFRESH' or args[2] == 'SPELL_AURA_REMOVED'"))
            {
                Logger.InfoLog(
                    "ERROR: Could not add combat log event filter! - Performance may be horrible, and things may not work properly!");
            }

            Logger.InfoLog("Attached combat log");
            _combatLogAttached = true;
        }

        private static void HandleCombatLog(object sender, LuaEventArgs args)
        {
            var e = new CombatLogEventArgs(args.EventName, args.FireTimeStamp, args.Args);

            //string TrackEvent = "";
            //for (int i = 0; i < e.Args.Length; i++)
            //{
            //    if (e.Args[i] != null)
            //    {
            //        TrackEvent += string.Format(" Index: {0} - Value: {1}", i, e.Args[i]);
            //    }
            //}
            //if (TrackEvent.Length > 0) Logger.DebugLog(TrackEvent);
            switch (e.Event)
            {
                case "SPELL_AURA_APPLIED":
                    try
                    {
                        //Add too List
                        if (e.SourceGuid != 0 && e.SourceGuid == StyxWoW.Me.Guid)
                        {
                            UpdateStatInfo(e.DestGuid, e.SpellId);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    break;

                case "SPELL_AURA_REFRESH":
                    try
                    {
                        //Add too List
                        //Logger.DebugLog("{0} - {1} - {2} - {3} - {4} - {5}", e.SourceGuid, e.DestGuid, e.SourceUnit, e.DestUnit, e.SpellId, e.SpellName);
                        if (e.SourceGuid != 0 && e.SourceGuid == StyxWoW.Me.Guid)
                        {
                            UpdateStatInfo(e.DestGuid, e.SpellId);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    break;

                case "SPELL_AURA_REMOVED":
                    try
                    {
                        //remove from list
                        if (e.SourceGuid != 0 && e.SourceGuid == StyxWoW.Me.Guid)
                        {
                            RemoveDotDamage(e.DestGuid, e.SpellId);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    break;
            }
        }

        private static void RemoveDotDamage(ulong destGuid, int spellId)
        {
            TimeSpan lastSeconds = TimeSpan.FromSeconds(600); // our window

            try
            {
                //directly removing throws errors, so we have to do a workaround
                Dictionary<int, StatInfo> RemoveList = new Dictionary<int, StatInfo>();

                // Remove any data older than lastSeconds
                foreach (var entry in StatInfos.Where(entry => entry.Value.TimeTracked < DateTime.Now - lastSeconds))
                {
                    RemoveList.Add(entry.Key, entry.Value);

                    //DotInfos.Remove(entry.Key);
                }
                foreach (var entry in RemoveList)
                {
                    StatInfos.Remove(entry.Key);
                }

                //Remove aura from tracking if faded from target
                var hash = SpellUnitHash(destGuid, spellId);
                if (StatInfos.ContainsKey(hash)) StatInfos.Remove(hash);
            }
            catch (Exception ex)
            {
                Logger.DebugLog("RemoveDotDamage : {0}", ex);
            }
        }

        private static int SpellUnitHash(WoWUnit u, int spellId)
        {
            return SpellUnitHash(u.Guid, spellId);
        }

        private static int SpellUnitHash(ulong guid, int spellId)
        {
            string toHash = guid.ToString(CultureInfo.InvariantCulture) + '_' + spellId.ToString(CultureInfo.InvariantCulture);
            return toHash.GetHashCode();
        }

        #endregion
    }
}