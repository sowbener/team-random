using FuryUnleashed.Core.Utilities;
using Styx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FuryUnleashed.Core.Helpers
{
    public class ProtTracker
    {

        #region DamageTracker
        private static Dictionary<DateTime, double> _damageTaken;

        private static bool AddingDamageTaken { get; set; }
        private static bool RemovingDamageTaken { get; set; }

        public static void Initialize()
        {
            _damageTaken = new Dictionary<DateTime, double>();
            AttachCombatLogEvent();
            Logger.CombatLogFb("FU: ProtTracker Initialized.");
        }

        public static void Pulse()
        {
            RemoveDamageTaken(DateTime.Now);
        }

        private static void AttachCombatLogEvent()
        {
            CombatLogHandler.Register("SWING_DAMAGE", HandleCombatLog);
            CombatLogHandler.Register("SPELL_DAMAGE", HandleCombatLog);
        }

        private static void HandleCombatLog(CombatLogEventArgs args)
        {
            switch (args.Event)
            {
                case "SWING_DAMAGE":
                    if (args.DestGuid == StyxWoW.Me.Guid)
                    {
                        object damage = args.Amount;
                        if (!AddingDamageTaken)
                            AddDamageTaken(DateTime.Now, (int)(double)damage);
                    }
                    break;

                case "RANGE_DAMAGE":
                case "SPELL_DAMAGE":
                    if (args.DestGuid == StyxWoW.Me.Guid)
                    {
                        object damage = args.Amount;
                        //string school = args.SpellSchool.ToString();
                        //string spellname = args.SpellName;

                        // Do not count damage from no source or maybe this is just particular items like Shannox's Jagged Tear?
                        // Do not count Spirit Link damage since it doesn't affect DS.
                        bool countDamage = args.SourceName != null ||
                                           (args.SpellName == "Spirit Link" && args.SourceName == "Spirit Link Totem");

                        if (countDamage && !AddingDamageTaken)
                        {
                            AddDamageTaken(DateTime.Now, (int)(double)damage);
                        }
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

        internal static double GetDamageTaken(DateTime timestamp = default(DateTime))
        {
            DateTime current = timestamp;
            TimeSpan lastSeconds = TimeSpan.FromSeconds(6);

            return (from entry in _damageTaken
                    let diff = current - entry.Key
                    where diff <= lastSeconds && diff >= TimeSpan.FromSeconds(0)
                    select entry).Aggregate<KeyValuePair<DateTime, double>, double>(0, (current1, entry) => current1 + entry.Value);
        }
        #endregion

        #region VARs
        internal const int ShieldBarrierSpellId = 112048;
        internal const int ShieldBlockSpellId = 132404;
        #endregion

        #region Calculates
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

                    if (2 * (attackpower - 2 * strength) > stamina * 2.5) { sumstam = 2 * (attackpower - 2 * strength); }
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

        public static double CalculateEstimatedBlockValue()
        {
            using (new PerformanceLogger("CalculateEstimatedBlockValue"))
            {
                try
                {
                    var damageoversixseconds = GetDamageTaken(DateTime.Now);
                    var mastery = StyxWoW.Me.Mastery;
                    var criticalBlockChance = (mastery * 2.2) / 100;
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
        
    }
}
