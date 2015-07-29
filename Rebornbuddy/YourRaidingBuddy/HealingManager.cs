using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using YourRaidingBuddy.Settings;
using YourRaidingBuddy.Interfaces.Settings;
using YourRaidingBuddy.Helpers;

namespace YourRaidingBuddy
{
    public class HealTargeting : Targeting
    {
        private static HealTargeting _instance;
        /// <summary> Default constructor. </summary>
        public HealTargeting()
        {
            Provider = new HealTargetingProvider();
        }

        public static int D15, D25, PeopleDying;
        /// <summary> Gets the instance. </summary>
        /// <value> The instance. </value>
        public static HealTargeting Instance
        {
            get { return _instance ?? (_instance = new HealTargeting()); }
        }
    }

    public class HealTargetingProvider : ITargetingProvider
    {
        internal static int HealingUnits;

        public sealed class TargetPriority
        {
            public BattleCharacter Object;
            public double Score;
        }

        public List<BattleCharacter> GetObjectsByWeight()
        {

            var wecareabout = Unit.FriendlyPriorities;
            List<TargetPriority> wrapped = wecareabout.Select(n => new TargetPriority {Object = n,}).ToList();
            DefaultTargetWeight(wrapped);
            return new List<BattleCharacter>(wrapped.OrderByDescending(s => s.Score).Select(s => s.Object).ToList());
        }
        private void DefaultTargetWeight(List<TargetPriority> units)
        {
            HealTargeting.D15 = 0;
            HealingUnits = Unit.NearbyUnitsNeedHealing(15).Count();
            HealTargeting.D25 = 0;
            HealTargeting.PeopleDying = 0;
            var myLoc = Core.Player.Location;
            foreach (var prio in units)
            {
                var u = prio.Object;
                if (u == null || !u.IsValid)
                {
                    prio.Score = -9999f;
                    continue;
                }
                // The more health they have, the lower the score.
                // This should give -500 for units at 100%
                // And -50 for units at 10%
                try
                {
                    prio.Score = u.IsAlive ? 500f : -500f;
                    prio.Score -= u.CurrentHealthPercent*5;
                    var distance = u.Location.Distance2D(myLoc);
                        if (distance < 25 && u.CurrentHealthPercent < InternalSettings.Instance.General.HealthPercent) HealTargeting.D25++;
                    if (distance < 15 && u.CurrentHealthPercent < InternalSettings.Instance.General.HealthPercent) HealTargeting.D15++;
                    if (u.CurrentHealthPercent < InternalSettings.Instance.General.CriticalHealthPercent)
                        HealTargeting.PeopleDying++;

                    // If they're out of range, give them a bit lower score.
                    if (distance > 40)
                    {
                        prio.Score -= 50f;
                    }

                    // If they're out of LOS, again, lower score!
                    if (!u.InLineOfSight())
                    {
                        prio.Score -= 100f;
                    }

                    //Give tanks more weight. If the tank dies, we all die. KEEP HIM UP.
                    if (u.InCombat && (u.CurrentJob == ff14bot.Enums.ClassJobType.Warrior || u.CurrentJob == ff14bot.Enums.ClassJobType.Paladin || 
                       u.CurrentJob == ff14bot.Enums.ClassJobType.Gladiator || u.CurrentJob == ff14bot.Enums.ClassJobType.Marauder) && u.CurrentHealthPercent < 30)

                    {
                        prio.Score += 200f;
                    }

                    if(HealingUnits > 2)
                    {
                        prio.Score += 30f;
                    }

                    if (HealingUnits > 3)
                    {
                        prio.Score += 40f;
                    }

                    if (HealingUnits > 4)
                    {
                        prio.Score += 50f;
                    }

                    if (HealingUnits > 5)
                    {
                        prio.Score += 60f;
                    }

                    /* Give flag carriers more weight in battlegrounds. We need to keep them alive!
                    if (inBg && u.IsPlayer && u.Auras.Keys.Any(a => a.ToLowerInvariant().Contains("flag")))
                    {
                        prio.Score += 100f;
                    }*/
                }
                catch (System.AccessViolationException)
                {
                    prio.Score = -9999f;
                }
            }
        }
    }
}