using Xiaolin.Core;
using Xiaolin.Helpers;
using Xiaolin.Managers;
using Styx;
using System;
using System.Collections.Generic;
using System.Linq;
using Styx.Common;
using Logger = Xiaolin.Helpers.XILogger;
using SG = Xiaolin.Interfaces.Settings;

namespace Xiaolin.Helpers
{
    public class GuardTracker
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
            Logger.CombatLog("XI: GuardTracker Initialized.");
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
                            AddDamageTaken(DateTime.Now, (int)damage);
                    }
                    break;

                case "RANGE_DAMAGE":
                case "SPELL_DAMAGE":
                    if (args.DestGuid == StyxWoW.Me.Guid)
                    {
                        object damage = args.Amount;

                        // Do not count damage from no source or maybe this is just particular items like Shannox's Jagged Tear?
                        // Do not count Spirit Link damage since it doesn't affect DS.
                        bool countDamage = args.SourceName != null ||
                                           (args.SpellName == "Spirit Link" && args.SourceName == "Spirit Link Totem");

                        if (countDamage && !AddingDamageTaken)
                        {
                            AddDamageTaken(DateTime.Now, (int)damage);
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
                Logging.WriteDiagnostic(" Adding Damage amount {0}", damage);
            }
            catch (Exception ex)
            {
                _damageTaken[timestamp] = _damageTaken.ContainsKey(timestamp) ? damage : 0;
                AddingDamageTaken = false;

                Logger.DiagLogW("AddDamageTaken : {0}", ex);
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
                Logger.DiagLogW("RemovingDamageTaken : {0}", ex);
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

        #region Shield
        public static bool GuardOK
        {
            get
            {
                double predictedGuardValue = 0.0;
                double fakeGuardValue = 0.0;

                double attackpower = StyxWoW.Me.AttackPower;


                var healthDeficit = (StyxWoW.Me.MaxHealth * SG.XISettings.Instance.Brewmaster.HPAPScale/100);

                // This should return the predicted shield over the last 5 seconds
                //[(Attack power * 1.971) + 14232]  
                predictedGuardValue = Math.Truncate(attackpower * 2);
                fakeGuardValue = Math.Truncate(XIMain._initap * 2);

                
             //    Logger.DebugLog("======================================================================================");
              //   Logger.DebugLog("DSTracker: PredictedGuardValue[{0}] >= attackpowerfake [{1}]", fakeGuardValue, predictedGuardValue);
            //    Logger.DebugLog("DSTracker: attackpowerlua[{0}] >= attackpowerapi [{1}]", attackpower, XIMain._initap);
             //    Logger.DebugLog("======================================================================================");
             

                // If our attackpower beats the estimate then we do it.
                var GuardShieldUp = predictedGuardValue + healthDeficit > fakeGuardValue && StyxWoW.Me.HealthPercent < 99;

                return GuardShieldUp;
            }
        }
        #endregion

    }
}
      