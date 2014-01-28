using FuryUnleashed.Core.Utilities;
using Styx;
using System;
using System.Collections.Generic;
using System.Linq;
using Styx.Common;

namespace FuryUnleashed.Core.Helpers
{
    public class DamageTracker
    {
        #region DamageTracker
        private static Dictionary<DateTime, double> _damageTaken;

        private static bool AddingDamageTaken { get; set; }
        private static bool RemovingDamageTaken { get; set; }

        public static void Initialize()
        {
            _damageTaken = new Dictionary<DateTime, double>();
            CombatLogHandler.Initialize();
            AttachCombatLogEvent();
            Logger.CombatLogFb("Damage Tracker Initialized.");
        }

        public static void Pulse()
        {
            RemoveDamageTaken(DateTime.Now);
        }

        private static void AttachCombatLogEvent()
        {
            CombatLogHandler.Register("SWING_DAMAGE", HandleCombatLog);
            CombatLogHandler.Register("SPELL_DAMAGE", HandleCombatLog);
            CombatLogHandler.Register("RANGE_DAMAGE", HandleCombatLog);
        }

        private static void HandleCombatLog(CombatLogEventArgs args)
        {
            switch (args.Event)
            {
                case "SWING_DAMAGE":
                    if (args.DestGuid == Root.MyGuid)
                    {
                        object damage = args.Amount;
                        if (!AddingDamageTaken)
                            AddDamageTaken(DateTime.Now, (int)damage);
                    }
                    break;

                case "RANGE_DAMAGE":
                    if (args.DestGuid == Root.MyGuid && StyxWoW.Me.Specialization != WoWSpec.WarriorProtection)
                    {
                        object damage = args.Amount;
                        if (!AddingDamageTaken)
                            AddDamageTaken(DateTime.Now, (int)damage);
                    }
                    break;

                case "SPELL_DAMAGE":
                    if (args.DestGuid == Root.MyGuid)
                    {
                        object damage = args.Amount;

                        bool countDamage = args.SourceName != null || (args.SpellName == "Spirit Link" && args.SourceName == "Spirit Link Totem");

                        if (countDamage && !AddingDamageTaken)
                            AddDamageTaken(DateTime.Now, (int)damage);
                    }
                    break;
            }
        }

        private static void AddDamageTaken(DateTime timestamp, int damage)
        {
            try
            {
                if (RemovingDamageTaken) return;

                AddingDamageTaken = true;
                _damageTaken[timestamp] = damage;
                AddingDamageTaken = false;
                Logging.WriteDiagnostic(" Adding Damage amount {0}", damage);
            }
            catch (Exception ex)
            {
                _damageTaken[timestamp] = _damageTaken.ContainsKey(timestamp) ? damage : 0;
                AddingDamageTaken = false;

                Logger.DiagLogWh("AddDamageTaken : {0}", ex);
            }
        }

        private static void RemoveDamageTaken(DateTime timestamp)
        {
            TimeSpan lastSeconds = TimeSpan.FromSeconds(6);

            try
            {
                if (AddingDamageTaken) return;

                RemovingDamageTaken = true;
                Dictionary<DateTime, double> removeList = new Dictionary<DateTime, double>();

                // Remove any data older than lastSeconds + 3
                foreach (var entry in _damageTaken.Where(entry => timestamp - entry.Key > lastSeconds + TimeSpan.FromSeconds(3)))
                {
                    removeList.Add(entry.Key, entry.Value);
                }

                foreach (var entry in removeList)
                {
                    _damageTaken.Remove(entry.Key);
                }
                RemovingDamageTaken = false;
            }
            catch (Exception ex)
            {
                Logger.DiagLogWh("RemovingDamageTaken : {0}", ex);
            }
        }

        internal static double GetDamageTaken(DateTime timestamp = default(DateTime), double lastseconds = 6)
        {
            DateTime current = timestamp;
            TimeSpan lastSeconds = TimeSpan.FromSeconds(lastseconds);

            return (from entry in _damageTaken
                    let diff = current - entry.Key
                    where diff <= lastSeconds && diff >= TimeSpan.FromSeconds(0)
                    select entry).Aggregate<KeyValuePair<DateTime, double>, double>(0, (current1, entry) => current1 + entry.Value);
        }
        #endregion

        #region Protection-Spec Functions
        internal const int ShieldBarrierSpellId = 112048;
        internal const int ShieldBlockSpellId = 132404;

        public static double CalculateEstimatedAbsorbValue()
        {
            using (new PerformanceLogger("CalculateEstimatedAbsorbValue"))
            {
                try
                {
                    double sumstam;

                    var attackpower = StyxWoW.Me.AttackPower;
                    var strength = StyxWoW.Me.Strength;
                    var stamina = StyxWoW.Me.Stamina;

                    var sumrage = StyxWoW.Me.CurrentRage > 60 ? 60 : StyxWoW.Me.CurrentRage;

                    if (sumrage < 20) sumrage = 20;

                    var ragemultiplier = sumrage / 60.0;

                    if (1.8 * (attackpower - 2 * strength) > stamina * 2.5) { sumstam = 2 * (attackpower - 2 * strength); }
                    else sumstam = stamina * 2.5;

                    var barrierresult = sumstam * ragemultiplier;

                    return barrierresult;
                }
                catch (Exception exabsorbcalc)
                {
                    Logger.DiagLogFb("FU: Failed CalculateEstimatedAbsorbValue - {0}", exabsorbcalc);
                }
            }
            return 0;
        }

        // Crit Block Formula: (http://www.wowwiki.com/Critical_Block)
        // Mastery Ratio is 272.7 (http://wowpedia.org/Mastery)
        //
        // var criticalBlockChance = (mastery * 2.2) / 100; // Shield Maid version ...
        public static double CalculateEstimatedBlockValue()
        {
            using (new PerformanceLogger("CalculateEstimatedBlockValue"))
            {
                try
                {
                    var damageoversixseconds = GetDamageTaken(DateTime.Now); // Verified and Correct
                    var mastery = StyxWoW.Me.Mastery; // Verified and Correct
                    var criticalBlockChance = (8 * 1.5) + (mastery / 272.7 * 1.5); // Verified and Correct
                    var blockresult = damageoversixseconds * criticalBlockChance * 0.6 + damageoversixseconds * (1 - criticalBlockChance) * 0.3;

                    return blockresult;
                }
                catch (Exception exblockcalc)
                {
                    Logger.DiagLogFb("FU: Failed CalculateEstimatedBlockValue - {0}", exblockcalc);
                }
            }
            return 1;
        }
        #endregion

        #region Fury-Spec Functions
        //public static bool StanceTracker()
        //{
        //    using (new PerformanceLogger("StanceTracker"))
        //    {
        //        try
        //        {
        //            // Battle: You gain rage from auto-attacking: 3.5 Rage for a 1.00 Weapon Speed for each hit. Offhand weapons generate half the rage of the main-hand.
        //            // Berserker: You gain rage from damage taken. The formula is 1 rage for 1% of health lost, and 50% of the rage generation of auto-attacks when compared to Battle Stance.

        //            var weaponspeed = 3.6;
        //            var damageoverthreeseconds = GetDamageTaken(DateTime.Now, 3); // Datetime 3 seconds - retrieves damage received over 3 seconds.
        //            var healthperpercent = StyxWoW.Me.MaxHealth / 100; // Calculates amount of health per percent.
        //            var battlestanceregen = (weaponspeed * 3.5) + (weaponspeed * 1.75);
        //            //var berserkerstanceregen = ;
        //        }
        //        catch (Exception exstancetrack)
        //        {
        //            Logger.DiagLogFb("FU: Failed CalculateEstimatedBlockValue - {0}", exstancetrack);
        //        }
        //    }
        //    return false;
        //}
        #endregion
    }
}
