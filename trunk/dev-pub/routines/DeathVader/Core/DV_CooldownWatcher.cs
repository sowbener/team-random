using CommonBehaviors.Actions;
using JetBrains.Annotations;
using DeathVader.Core;
using DeathVader.Helpers;
using Styx;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.TreeSharp;
using System.Linq;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Logger = DeathVader.Helpers.DvLogger;
using System;
using System.Collections.Generic;
using Action = Styx.TreeSharp.Action;
namespace DeathVader.Core
{
    // OK heres how it works. You cast the spell you want to track using
    //
    //  CooldownTracker.Cast("Outbreak")
    //
    // then to check if the outbreak is on cooldown
    //
    //  CooldownTracker.SpellOnCooldown("Outbreak")
    //
    // it is important to use the CooldownTrackers cast methods to track the cooldown of your spell.
    // GetSpellCooldown is the same (use the CooldownTracker.Cast("Outbreak") method) and then check it with
    //
    // CooldownTracker.GetSpellCooldown("Outbreak").TotalMilliseconds > 4
    //
    // you can also supply a buffer for the GetSpellCooldown which will begin to check honorbuddy for the Cooldowntimeleft at the set buffer time.
    // if the buffer was like this: CooldownTracker.GetSpellCooldown("Outbreak", 4) <-- 4 Seconds
    // it would query Honorbuddy for the cooldowntimeleft 4 seconds before the spell expired.
    // the default it 2 seconds. so if a spell has a cooldown of 8 seconds, for the first 6 seconds honorbuddy will not be checked
    // when there is 2 seconds remaining the honorbuddy cooldowntimeleft will be returned.
    // excample.
    // 00.00.08.000 <-- spell used
    // not checked
    // 00.00.02.000 <-- Honorbuddy Cooldowntimeleft is checked from here on.
    // 00.00.01.999
    // 00.00.01.750
    // 00.00.00.500
    // 00.00.00.000

    [UsedImplicitly]

    internal static class CooldownWatcher
    {
        static CooldownWatcher()
        {
            // Cache dat shit..
            MeGuid = StyxWoW.Me.Guid;
            SpellCooldownEntries = new Dictionary<ulong, SpellCooldown>();
            LocalizedFailTypeConstants = new Dictionary<string, string>();
            LocalizedInvalidTargetsConstants = new Dictionary<string, string>();
            PopulatelocalizedConstants();
        }

        private static ulong MeGuid { get; set; }

        #region WoW Constants

        private static readonly Dictionary<string, string> LocalizedFailTypeConstants = new Dictionary<string, string>();
        public static readonly Dictionary<string, string> LocalizedInvalidTargetsConstants = new Dictionary<string, string>();

        private static void PopulatelocalizedConstants()
        {
            foreach (String ft in DvHashSets.FailType)
            {
                var result = GetSymbolicLocalizeValue(ft);

                //Logger.Output("{1} :: {0}", result, ft);

                if (!string.IsNullOrEmpty(result)) LocalizedFailTypeConstants.Add(result, ft);
            }

            foreach (String ft in DvHashSets.InvalidTargets)
            {
                var result = GetSymbolicLocalizeValue(ft);

                // Logger.Output("{1} :: {0}", result, ft);

                if (!string.IsNullOrEmpty(result)) LocalizedInvalidTargetsConstants.Add(result, ft);
            }
        }

        private static string GetSymbolicLocalizeValue(string symbolicName)
        {
            var localString = Lua.GetReturnVal<string>("return " + symbolicName, 0);
            return localString;
        }

        #endregion

        public static void Initialize()
        {
            CombatLogHandler.Register("SPELL_CAST_START", HandleCombatLogEvent);
            CombatLogHandler.Register("SPELL_CAST_FAILED", HandleCombatLogEvent);
            CombatLogHandler.Register("SPELL_CAST_SUCCESS", HandleCombatLogEvent);
            CombatLogHandler.Register("SPELL_CAST_MISS", HandleCombatLogEvent);
        }

        public static void Shutdown()
        {
            CombatLogHandler.Remove("SPELL_CAST_START");
            CombatLogHandler.Remove("SPELL_CAST_FAILED");
            CombatLogHandler.Remove("SPELL_CAST_SUCCESS");
            CombatLogHandler.Remove("SPELL_CAST_MISS");

        }

        #region Cooldowns

        private static double BaseCooldown(this string spell)
        {

            SpellFindResults results;
            if (!SpellManager.FindSpell(spell, out results)) return 0;

            return results.Override != null
                ? results.Override.CooldownTimeLeft.TotalMilliseconds
                : results.Original.CooldownTimeLeft.TotalMilliseconds;
        }


        public static bool OnCooldown(this int spell)
        {
            using (new PerformanceLogger("Cooldown"))
            {
                var result = WoWSpell.FromId(spell);
                return result != null && result.IsValid && GetSpellCooldownTimeLeft(result.Id) > 100;
            }
        }

        #endregion

        private static void HandleCombatLogEvent(CombatLogEventArgs args)
        {
            switch (args.Event)
            {
                case "SPELL_CAST_SUCCESS":
                case "SPELL_CAST_START":
                case "SPELL_CAST_MISS":
                    if (args.SourceGuid == MeGuid)
                        SPELL_CAST_START(args);
                    break;

                case "SPELL_CAST_FAILED":
                    if (args.SourceGuid == MeGuid)
                        SPELL_CAST_FAILED(args);
                    break;
            }
        }

        private static void SPELL_CAST_FAILED(CombatLogEventArgs args)
        {
            var spellId = args.SpellId;
            //get the english spell name, not the localized one!
            var spellName = WoWSpell.FromId(spellId).Name;
            var guid = MeGuid + (ulong)spellId;
            var eName = args.Event;
            var baseCooldown = spellName.BaseCooldown();
            var reason = args.FailedType;

            Logger.DebugLog(" [Cooldown Watcher] : {2} because [{3}] for spell [{0} : {4}] cooldown left is {1}ms", spellName, baseCooldown, eName, reason, spellId);

            string result2;
            if (LocalizedFailTypeConstants.TryGetValue(reason, out result2))
            {
                SpellCooldownEntries.RemoveAll(t => t.SpellCooldownKey == guid);
                UpdateSpellCooldownEntries(guid, spellName, baseCooldown);
                return;
            }

            SpellCooldownEntries.RemoveAll(t => t.SpellCooldownKey == guid);

        }

        private static void SPELL_CAST_START(CombatLogEventArgs args)
        {
            var spellId = args.SpellId;
            //get the english spell name, not the localized one!
            var spellName = WoWSpell.FromId(spellId).Name;
            var guid = MeGuid + (ulong)spellId;
            var eName = args.Event;
            var baseCooldown = spellName.BaseCooldown();

            //Logger.Output(" [Cooldowns] : {2} {0} base cooldown is {1}", spellName, baseCooldown, eName);

            if (baseCooldown > 0 && spellId != 0) UpdateSpellCooldownEntries(guid, spellName, baseCooldown);
        }

        #region Spell Cooldown Entries

        private static readonly Dictionary<ulong, SpellCooldown> SpellCooldownEntries = new Dictionary<ulong, SpellCooldown>();

        public static void OutputSpellCooldownEntries()
        {
            if (SpellCooldownEntries.Values.Count < 1) return;
            Logger.DebugLog(" --> We have {0} spells on cooldown for {1}", SpellCooldownEntries.Count, SpellCooldownEntries.Values.FirstOrDefault().SpellCooldownName);
        }

        internal static double GetSpellCooldownTimeLeft(int spell)
        {
            var guid = MeGuid + (ulong)spell;

            SpellCooldown results;
            var msleft = !SpellCooldownEntries.TryGetValue(guid, out results) ? 0 : DateTime.UtcNow.Subtract(results.SpellCooldownCurrentTime).TotalMilliseconds >= results.SpellCooldownExpiryTime ? 0 : DateTime.UtcNow.Subtract(results.SpellCooldownCurrentTime).TotalMilliseconds;
            //Logger.Output(" --> GetSpellCooldownTimeLeft for {0} is {1}", WoWSpell.FromId(spell).Name, msleft);
            return msleft;
        }

        internal static void PulseSpellCooldownEntries()
        {
            SpellCooldownEntries.RemoveAll(t => DateTime.UtcNow.Subtract(t.SpellCooldownCurrentTime).TotalSeconds >= t.SpellCooldownExpiryTime);
        }

        private static void UpdateSpellCooldownEntries(ulong key, string name, double expiryTime)
        {
            if (!SpellCooldownEntries.ContainsKey(key)) SpellCooldownEntries.Add(key, new SpellCooldown(key, name, expiryTime, DateTime.UtcNow));
        }

        private struct SpellCooldown
        {
            public SpellCooldown(ulong key, string name, double expiryTime, DateTime currentTime)
                : this()
            {
                SpellCooldownKey = key;
                SpellCooldownName = name;
                SpellCooldownExpiryTime = expiryTime;
                SpellCooldownCurrentTime = currentTime;
            }

            public DateTime SpellCooldownCurrentTime { get; private set; }

            public double SpellCooldownExpiryTime { get; set; }

            public ulong SpellCooldownKey { get; set; }

            public string SpellCooldownName { get; set; }
        }

        #endregion Spell Cooldown Entries
    }
}