using System;
using System.Collections.Generic;
using YourRaidingBuddy.Core.Utilities;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.TreeSharp;
using Styx;
using CommonBehaviors.Actions;
using Styx.WoWInternals.WoWObjects;
using JetBrains.Annotations;

namespace YourRaidingBuddy.Core.Helpers
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

    internal static class CooldownTracker
    {
        // ReSharper disable once InconsistentNaming
        internal static Dictionary<WoWSpell, DateTime> cooldowns = new Dictionary<WoWSpell, DateTime>();

        // Adding casted spells to dictionary with datetime
        internal static void SpellUsed(string spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                WoWSpell result = results.Override ?? results.Original;
                cooldowns[result] = DateTime.Now.Add(result.CooldownTimeLeft);
            }
        }

        internal static void SpellUsed(int spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                WoWSpell result = results.Override ?? results.Original;
                cooldowns[result] = DateTime.Now.Add(result.CooldownTimeLeft);
            }
        }

        // Get the CooldownTimeLeft on a spell, by default it will only return the true result at less than a second.
        public static TimeSpan GetSpellCooldown(string spell, int buffer = 1, bool cdoverride = true)
        {
            using (new PerformanceLogger("GetSpellCooldown"))
            {
                if (cdoverride) return CooldownTimeLeft(spell);

                SpellFindResults results;
                if (SpellManager.FindSpell(spell, out results))
                {
                    return results.Override != null ? results.Override.CooldownTimeLeft : results.Original.CooldownTimeLeft;
                }

                return TimeSpan.MaxValue;
            }
        }

        public static TimeSpan GetSpellCooldown(int spell, int buffer = 1, bool cdoverride = true)
        {
            using (new PerformanceLogger("GetSpellCooldown"))
            {
                if (cdoverride) return CooldownTimeLeft(spell);

                SpellFindResults results;
                if (SpellManager.FindSpell(spell, out results))
                {
                    return results.Override != null ? results.Override.CooldownTimeLeft : results.Original.CooldownTimeLeft;
                }

                return TimeSpan.MaxValue;
            }
        }

        // Checks if spell is on cooldown.
        public static bool SpellOnCooldown(string spell, bool cdoverride = true)
        {
            using (new PerformanceLogger("SpellOnCooldown"))
            {
                if (cdoverride) return IsOnCooldown(spell);

                SpellFindResults results;
                if (SpellManager.FindSpell(spell, out results))
                {
                    return results.Override != null ? results.Override.Cooldown : results.Original.Cooldown;
                }

                return false;
            }

        }

        public static bool SpellOnCooldown(int spell, bool cdoverride = true)
        {
            using (new PerformanceLogger("SpellOnCooldown"))
            {
                if (cdoverride) return IsOnCooldown(spell);

                SpellFindResults results;
                if (SpellManager.FindSpell(spell, out results))
                {
                    return results.Override != null ? results.Override.Cooldown : results.Original.Cooldown;
                }

                return false;
            }
        }

        // Magic behind it ...
        private static bool IsOnCooldown(int spell, int buffer = 1)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                WoWSpell result = results.Override ?? results.Original;
                try
                {
                    DateTime lastUsed;
                    if (cooldowns.TryGetValue(result, out lastUsed))
                    {
                        var lastUsedminusBuffer = lastUsed - TimeSpan.FromSeconds(buffer);
                        if (DateTime.Compare(DateTime.Now, lastUsedminusBuffer) > 0)
                        {
                            Logger.CooldownTrackerLog("FU: Checked cooldown for {0} at {1}", spell, DateTime.Now);
                            return result.Cooldown;
                        }
                        Logger.CooldownTrackerLog("FU: Found {0} but not ready to check at {1}", spell, DateTime.Now);
                        return true;
                    }
                }
                catch
                {
                    Logger.CooldownTrackerLog("FU: FindSpell Found {0} but currently not in our cooldownlist, first usage?", spell);
                    return false;
                }
            }
            else
            {
                Logger.CooldownTrackerLog("FU: FindSpell {0} failed at {1}", spell, DateTime.Now);
            }
            return false;
        }

        private static bool IsOnCooldown(string spell, int buffer = 1)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                WoWSpell result = results.Override ?? results.Original;
                try
                {
                    DateTime lastUsed;
                    if (cooldowns.TryGetValue(result, out lastUsed))
                    {
                        var lastUsedminusBuffer = lastUsed - TimeSpan.FromSeconds(buffer);
                        if (DateTime.Compare(DateTime.Now, lastUsedminusBuffer) > 0)
                        {
                            Logger.CooldownTrackerLog("FU: Checked cooldown for {0} at {1}", spell, DateTime.Now);
                            return result.Cooldown;
                        }

                        Logger.CooldownTrackerLog("FU: Found {0} but not ready to check at {1}", spell, DateTime.Now);
                        return true;
                    }
                }
                catch
                {
                    Logger.CooldownTrackerLog("FU: FindSpell Found {0} but currently not in our cooldownlist, first usage?", spell);
                    return false;
                }
            }
            else
            {
                Logger.CooldownTrackerLog("FU: FindSpell {0} failed at {1}", spell, DateTime.Now);
            }
            return false;
        }

        private static TimeSpan CooldownTimeLeft(int spell, int buffer = 1)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                WoWSpell result = results.Override ?? results.Original;
                try
                {
                    DateTime lastUsed;
                    if (cooldowns.TryGetValue(result, out lastUsed))
                    {
                        var lastUsedminusBuffer = lastUsed - TimeSpan.FromSeconds(buffer);
                        if (DateTime.Compare(DateTime.Now, lastUsedminusBuffer) > 0)
                        {
                            Logger.CooldownTrackerLog("FU: Checked CooldownTimeLeft for {0} at {1}", spell, DateTime.Now);
                            return result.CooldownTimeLeft;
                        }

                        Logger.CooldownTrackerLog("FU: Found {0} but not ready to check CooldownTimeLeft {1}", spell, DateTime.Now);
                        return TimeSpan.FromSeconds(10);
                    }
                }
                catch
                {
                    Logger.CooldownTrackerLog("FU: FindSpell Found {0} but currently not in our cooldownlist, first usage?", spell);
                    return TimeSpan.MaxValue;
                }
            }
            else
            {
                Logger.CooldownTrackerLog("FU: FindSpell {0} failed at {1}", spell, DateTime.Now);
            }
            return TimeSpan.MaxValue;
        }

        private static TimeSpan CooldownTimeLeft(string spell, int buffer = 1)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                WoWSpell result = results.Override ?? results.Original;
                try
                {
                    DateTime lastUsed;
                    if (cooldowns.TryGetValue(result, out lastUsed))
                    {
                        var lastUsedminusBuffer = lastUsed - TimeSpan.FromSeconds(buffer);
                        if (DateTime.Compare(DateTime.Now, lastUsedminusBuffer) > 0)
                        {
                            Logger.CooldownTrackerLog("FU: Checked CooldownTimeLeft for {0} at {1}", spell, DateTime.Now);
                            return result.CooldownTimeLeft;
                        }

                        Logger.CooldownTrackerLog("FU: Found {0} but not ready to check CooldownTimeLeft {1}", spell, DateTime.Now);
                        return TimeSpan.FromSeconds(10);
                    }
                }
                catch
                {
                    Logger.CooldownTrackerLog("FU: FindSpell Found {0} but currently not in our cooldownlist, first usage?", spell);
                    return TimeSpan.MaxValue;
                }
            }
            else
            {
                Logger.CooldownTrackerLog("FU: FindSpell {0} failed at {1}", spell, DateTime.Now);
            }
            return TimeSpan.MaxValue;
        }
    }
}