using Clio.Utilities;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Helpers
{
    internal static class Unit
    {
        internal static readonly Stopwatch CombatTime = new Stopwatch();

        internal static bool ExceptionCheck()
        {
            try
            {
                //using (new PerformanceLogger("RunComposites", Enums.PerformanceCategory.RunComposites))
                {
                    var localunit = Core.Player;
                    var targetunit = Core.Player.CurrentTarget;


                    return
                        !targetunit.IsViable() ||
                        !targetunit.CanAttack ||
                        !targetunit.IsTargetable;
                       // targetunit.CurrentHealth > 0;

                        //!localunit.IsAlive;
                }
            }
            catch (Exception ex)
            {
                if(InternalSettings.Instance.General.Debug) Logger.WriteDebug("RunComposites Exception: {0}", ex.ToString()); return false;
            }
        }

        internal static bool IsViable(this GameObject gameObject)
        {
            return (gameObject != null) && gameObject.IsValid;
        }

        internal static bool IsViableTarget(this GameObject unit)
        {
            if (!unit.IsViable())
                return false;
            if (!unit.CanAttack)
                return false;
            if (!unit.IsTargetable)
                return false;

            return unit.CurrentHealth > 0;

        }



        internal static bool IsViableHealing(this GameObject unit)
        {
            if (unit == null || !unit.IsValid)
                return false;
            if (!unit.IsTargetable)
                return false;

            return unit.CurrentHealth > 0;

        }

        /// <summary>
        ///     HostilePriorities list
        /// </summary>
        internal static List<BattleCharacter> HostilePriorities
        {
            get;
            private set;
        }

        internal static List<BattleCharacter> FriendlyPriorities
        {
            get;
            private set;
        }


        internal static IEnumerable<BattleCharacter> NearbyUnitsNeedHealing(double radius)
        {
            var friendly = FriendlyPriorities;
            var maxDistance = radius * radius;

            return friendly.Where(x => x.Location.Distance2DSqr(HealTargeting.Instance.FirstUnit.Location) < maxDistance);
        }

        /// <summary>
        ///     Filling the lists of wowunits on pulse.
        /// </summary>
        /// <param name="friendlyradius">Radius for DistanceSqr (25f default).</param>
        /// <param name="hostileradius">Radius for DistanceSqr (25f default).</param>
        internal static void UpdatePriorities(double friendlyradius = 25f, double hostileradius = 40f)
        {
            try
            {
                var unitlist = GameObjectManager.GetObjectsOfType<BattleCharacter>().ToList();

                /* Filling Friendly unit list */
                FriendlyPriorities = unitlist.Where(u =>
                u.IsViableHealing() && u.Type == GameObjectType.Pc &&
                u.IsAlive && u.CurrentHealthPercent < 99 &&
                u.Distance(Core.Me) <= friendlyradius).ToList();

                /* Perform Friendly Unit funcions */
                PulseFriendlyUnitFunctions();

                /* Filling Hostile unit list */
                HostilePriorities = unitlist.Where(u =>
                    u.IsViableTarget() &&
                    u.Distance(Core.Me) <= hostileradius).ToList();

                /* Perform Hostile Unit funcions */
                PulseHostileUnitFunctions();

                /* Diagnostic Logging for this void */
                if (InternalSettings.Instance.General.Debug)
                {
                    foreach (var u in HostilePriorities)
                    {
                        //      Logger.WriteDebug("{0} is Hostile", u.SafeName());
                    }

                    //     Logger.WriteDebug("PulseHostileWoWUnits Count {0}", HostilePriorities.Count);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebug("UnitBase.UpdatePriorities() Exception: {0}", ex);
            }
        }

        internal static void PulseHostileUnitFunctions()
        {
            try
            {
                //  using (new PerformanceLogger("PulseHostileUnitFunctions", Enums.PerformanceCategory.PulseHostileUnitFunctions))
                {
                    var hostile = HostilePriorities;

                    if(Core.Player.CurrentJob == ClassJobType.Bard)
                    {
                        VariableBook.HostileUnitsCount = hostile.Count(u => u.IsViable() && Core.Player.CurrentTarget != null && u.Location.DistanceSqr(Core.Player.CurrentTarget.Location) <= 8);
                    }

                    if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Pugilist)
                    {
                        VariableBook.HostileUnitsCount = hostile.Count(u => u.IsViable() && u.Distance(Core.Player) <= 10);
                    }

                    if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Ninja)
                    {
                        VariableBook.HostileUnitsCount = hostile.Count(u => u.IsViable() && u.Distance(Core.Player) <= 10);
                    }

                    if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Dragoon)
                    {
                        VariableBook.HostileUnitsCount = hostile.Count(u => u.IsViable() && u.Distance(Core.Player) <= 10);
                    }

                    if (Core.Player.CurrentJob == ClassJobType.Machinist)
                    {
                        VariableBook.HostileUnitsCount = hostile.Count(u => u.IsViable() && u.Distance(Core.Player) <= 10);

                    }

                    if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.DarkKnight)
                    {
                        VariableBook.HostileUnitsCount = hostile.Count(u => u.IsViable() && u.Distance(Core.Player) <= 10);

                        VariableBook.HostileUnitsTargettingMeCount = hostile.Count(u => u.IsViable() && u.TargetCharacter == Core.Player);

                        VariableBook.UnleashRangeNonAggroUnitsCount = hostile.Count(u => u.IsViable() && u.Distance(Core.Player) <= 5 && u.TargetCharacter != Core.Player);

                        VariableBook.ProvokeRangeNonAggroAndTappedUnitsCount = hostile.Count(u => u.IsViable() && u.Distance(Core.Player) <= 25 && u.TargetCharacter != Core.Player && u.Tapped);

                        VariableBook.NearestHostileUnitNotTargettingMe = hostile.Where(
                          u => u.IsViable() &&
                          u.Distance2D(Core.Player) <= 25 &&
                          u.TargetCharacter != Core.Player &&
                          u.Tapped
                        ).OrderBy(u => u.Distance2D()).FirstOrDefault();
                    }

                    if (InternalSettings.Instance.General.Debug)
                    {
                        //          Logger.WriteDebug("HostileUnitsCountPugilist: {0}", VariableBook.HostileUnitsCount);
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.WriteDebug("PulseHostileUnitFunctions Exception: {0}", ex.ToString());
            }
        }

        internal static void PulseFriendlyUnitFunctions()
        {
            try
            {
                //  using (new PerformanceLogger("PulseHostileUnitFunctions", Enums.PerformanceCategory.PulseHostileUnitFunctions))
                {
                    var friendly = FriendlyPriorities;

                    if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Conjurer)
                    {
                        VariableBook.FriendlyUnitsCount = friendly.Count(u => u.IsViableHealing() && u.Distance(Core.Player) <= 30);
                    }

                    if (InternalSettings.Instance.General.Debug)
                    {
                        Logger.WriteDebug("FriendlyHealingTesting: {0}", VariableBook.FriendlyUnitsCount);
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.WriteDebug("PulseHostileUnitFunctions Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        ///     Retrieving all WoWUnits
        /// </summary>
        private static IEnumerable<BattleCharacter> SearchAreaUnits()
        {
            try
            {
                //     using (new PerformanceLogger("SearchAreaUnits", Enums.PerformanceCategory.SearchAreaUnits))
                {
                    return GameObjectManager.GetObjectsOfType<BattleCharacter>();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteDebug("SearchAreaUnits Exception: {0}", ex.ToString()); return null;
            }
        }
    }
}
