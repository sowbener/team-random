
using YourBuddy.Core.Helpers;
using YourBuddy.Core.Utilities;
using YourBuddy.Interfaces.Settings;
using YourBuddy.Rotations;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Enum = YourBuddy.Core.Helpers.Enum;
using U = YourBuddy.Core.Unit;

namespace YourBuddy.Core
{
    internal static class Unit
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static readonly Random Random = new Random();

        public static WoWUnit VigilanceTarget;

        #region Unit Caching Functions
        /// <summary>
        /// Core check on Attackable Units (60 yards) - Further filtering should be applied.
        /// </summary>
        internal static IEnumerable<WoWUnit> AttackableUnits
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(u => IsViable(u) && u.Attackable && u.CanSelect && !u.IsFriendly && !u.IsDead && !u.IsNonCombatPet && !u.IsCritter && u.Distance <= 60);
            }
        }

        internal static IEnumerable<WoWUnit> AttackableBosses
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(u => IsViable(u) && u.Attackable && u.CanSelect && !u.IsFriendly && !u.IsDead && U.IsTargetBoss && u.Distance <= 60);
            }
        }

        /// <summary>
        /// Core check on Friendly Units (60 yards) - Further filtering should be applied.
        /// </summary>
        internal static IEnumerable<WoWUnit> FriendlyUnits
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(u => IsViable(u) && u.IsFriendly && !u.IsDead && !u.IsNonCombatPet && !u.IsCritter && u.Distance <= 60);
            }
        }

        internal static IEnumerable<WoWUnit> AggroUnits
        {
            get { return ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(u => u.Aggro && u.Attackable && !u.IsDead && !u.IsFriendly && u.CanSelect); }
        }

        internal static IEnumerable<WoWUnit> NearbyAggroUnits(WoWPoint fromLocation, double radius)
        {
            var hostile = AggroUnits;
            var maxDistance = radius * radius;
            return hostile.Where(x => x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        public static int NearbyAggroUnitsCount;

        public static void GetNearbyAggroUnitsCount()
        {
            if (Me.CurrentTarget != null)
                NearbyAggroUnitsCount = NearbyAggroUnits(StyxWoW.Me.Location, 20).Count();
        }

        public static void GetNearbyBossesUnitsCount()
        {
            if (Me.CurrentTarget != null)
                NearbyBossesUnitsCount = NearbyBossesUnits(StyxWoW.Me.Location, 20).Count();
        }
        public static int NearbyBossesUnitsCount;

        internal static IEnumerable<WoWUnit> NearbyBossesUnits(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableBosses;
            var maxDistance = radius * radius;
            return hostile.Where(x => x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        /// <summary>
        /// Core check on Raid Members - Further filtering should be applied.
        /// </summary>
        internal static IEnumerable<WoWPlayer> RaidMembers
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWPlayer>(true, true).Where(u => IsViable(u) && u.CanSelect && !u.IsDead && u.IsInMyPartyOrRaid);
            }
        }

        /// <summary>
        /// Core check on Attackable Units nearby (Specific Range) - Further filtering should be applied.
        /// </summary>
        /// <param name="fromLocation">Location - EG StyxWoW.Me.Location</param>
        /// <param name="radius">Radius for DistanceSqr (Yards)</param>
        /// <returns>Unit (Enemyunit) which fits the arguments</returns>
        internal static IEnumerable<WoWUnit> NearbyAttackableUnits(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableUnits;
            var maxDistance = radius * radius;

            return hostile.Where(x => x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        /// <summary>
        /// Core check on Nearby Casting Units which can be interrupted and not casted on StyxWoW.Me - Further filtering should be applied.
        /// </summary>
        /// <param name="fromLocation">Location - EG StyxWoW.Me.Location</param>
        /// <param name="radius">Radius for DistanceSqr (Yards)</param>
        /// <returns>Unit which fits the arguments</returns>
        internal static IEnumerable<WoWUnit> NearbyCastingUnits(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableUnits;
            var maxDistance = radius * radius;

            return hostile.Where(x => IsViable(x) && (x.IsCasting || x.IsChanneling) && (x.IsTargetingPet || x.IsTargetingMyPartyMember || x.IsTargetingMyRaidMember) && x.CanInterruptCurrentSpellCast && x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        /// <summary>
        /// Core check on Nearby Casting Units which can be interrupted and casted on StyxWoW.Me - Further filtering should be applied.
        /// </summary>
        /// <param name="fromLocation">Location - EG StyxWoW.Me.Location</param>
        /// <param name="radius">Radius for DistanceSqr (Yards)</param>
        /// <returns>Unit which fits the arguments</returns>
        internal static IEnumerable<WoWUnit> NearbyCastingUnitsTargetingMe(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableUnits;
            var maxDistance = radius * radius;

            return hostile.Where(x => IsViable(x) && (x.IsCasting || x.IsChanneling) && x.IsTargetingMeOrPet && x.CanInterruptCurrentSpellCast && x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        /// <summary>
        /// Core check on Friendly Units nearby (Specific Range) - Further filtering should be applied.
        /// </summary>
        /// <param name="fromLocation">Location - EG StyxWoW.Me.Location</param>
        /// <param name="radius">Radius for DistanceSqr (Yards)</param>
        /// <returns>Unit which fits the arguments</returns>
        internal static IEnumerable<WoWUnit> NearbyFriendlyUnits(WoWPoint fromLocation, double radius)
        {
            var friendly = FriendlyUnits;
            var maxDistance = radius * radius;

            return friendly.Where(x => IsViable(x) && x.IsFriendly && x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        /// <summary>
        /// Core check on Raid Members nearby (Specific Range) - Further filtering should be applied.
        /// </summary>
        /// <param name="fromLocation">Location - EG StyxWoW.Me.Location</param>
        /// <param name="radius">Radius for DistanceSqr (Yards)</param>
        /// <returns>Unit (raidmember - IsInMyPartyOrRaid) which fits the arguments</returns>
        internal static IEnumerable<WoWUnit> NearbyRaidMembers(WoWPoint fromLocation, double radius)
        {
            var units = RaidMembers;
            var maxDistance = radius * radius;
            return units.Where(u => IsViable(u) && u.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        /// <summary>
        /// Selects nearby unit within range without a specific aura to cast upon (Integer Used)
        /// </summary>
        /// <param name="debuffid">ID of the required debuff - For bloodthirst it's deep wounds.</param>
        /// <param name="auratimeleft">If all units have deep wounds, refresh at how much MS left on aura</param>
        /// <param name="radius">Range - eg 5 yards for Bloodthirst</param>
        /// <param name="conedegrees">Degrees for the Facing Cone (Me.IsSafelyFacing() check) - Default 160</param>
        /// <returns></returns>
        internal static WoWUnit MultiDotUnit(int debuffid, int auratimeleft, double radius, int conedegrees)
        {
            var selectableunits = NearbyAttackableUnits(StyxWoW.Me.Location, radius).Where(x => IsViable(x) && !x.IsPlayer && Me.IsSafelyFacing(x, conedegrees)).OrderByDescending(x => x.HealthPercent);
            var dotunit = selectableunits.FirstOrDefault(x => IsViable(x) && Me.IsSafelyFacing(x, conedegrees) && !Spell.HasAura(x, debuffid)) ??
                          selectableunits.FirstOrDefault(x => IsViable(x) && Me.IsSafelyFacing(x, conedegrees) && Spell.HasAura(x, debuffid, 0, auratimeleft));

            return dotunit;
        }

        /// <summary>
        /// Retrieves the target which requires Vigilance - Not StyxWoW.Me
        /// </summary>
        /// <returns>Viable unit to cast Vigilance on - Based on Settings</returns>

        /// <summary>
        /// Retrieves the count (Integer) of melee units (5y) - Used in YourBuddy.Rotations.Global.InitializeCaching()
        /// </summary>
        public static int AttackableMeleeUnitsCount;
        public static void GetAttackableMeleeUnitsCount()
        {
            using (new PerformanceLogger("GetAttackableMeleeUnitsCount"))
            {
                if (IsViable(Me.CurrentTarget))
                {
                    AttackableMeleeUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 5).Count();
                }
            }
        }


        /// <summary>
        /// Retrieves the count (Integer & Float) of nearby units (8y) - Used in YourBuddy.Rotations.Global.InitializeCaching()
        /// </summary>
        public static int NearbyAttackableUnitsCount;
        public static float NearbyAttackableUnitsFloat;
        public static void GetNearbyAttackableUnitsCount()
        {
            using (new PerformanceLogger("GetNearbyAttackableUnitsCount"))
            {
                if (IsViable(Me.CurrentTarget))
                {
                    NearbyAttackableUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 8).Count();
                    NearbyAttackableUnitsFloat = NearbyAttackableUnits(StyxWoW.Me.Location, 8).Count();
                }
            }
        }



        #endregion

        #region Global Functions
        /// <summary>
        /// Default check to see if we can interrupt the spell cast or channel.
        /// </summary>
        internal static bool CanInterrupt
        {
            get
            {
                using (new PerformanceLogger("CanInterrupt"))
                {
                    return IsViable(Me.CurrentTarget) && (Me.CurrentTarget.IsCasting || Me.CurrentTarget.IsChanneling) && Me.CurrentTarget.CanInterruptCurrentSpellCast &&
                            ((InternalSettings.Instance.General.InterruptMode == Enum.Interrupts.RandomTimed && Me.CurrentTarget.CurrentCastTimeLeft.TotalMilliseconds <= Random.Next(250, 1500)) ||
                            (InternalSettings.Instance.General.InterruptMode == Enum.Interrupts.Constant && Me.CurrentTarget.CurrentCastTimeLeft.TotalMilliseconds <= InternalSettings.Instance.General.InterruptNum)
                            );
                }
            }
        }

        /// <summary>
        /// Default check before casting - Checks various things like if StyxWoW.Me.CurrentTarget is viable, self not mounted and more.
        /// </summary>
        internal static bool DefaultCheck
        {
            get
            {
                return IsViable(Me.CurrentTarget) && !Me.Mounted && Me.CurrentTarget.Attackable && !Me.CurrentTarget.IsDead && Me.CurrentTarget.IsWithinMeleeRange;
            }
        }

        internal static bool DefaultCheckRanged
        {
            get
            {
                return IsViable(Me.CurrentTarget) && !Me.Mounted && Me.CurrentTarget.Attackable && !Me.CurrentTarget.IsDead;
            }
        }

        /// <summary>
        /// Default check before buffing - Checks various things like if StyxWoW.Me is viable, self not mounted and more.
        /// </summary>
        internal static bool DefaultBuffCheck
        {
            get
            {
                return IsViable(Me) && !Me.Mounted && !Me.IsDead && !Me.IsFlying && !Me.IsOnTransport && !Me.IsChanneling && !Me.HasAura("Food") && !Me.HasAura("Drink");
            }
        }

        /// <summary>
        /// Check to see if CurrentTarget has you as target.
        /// </summary>
        internal static bool IsTargettingMe
        {
            get { return Me.CurrentTarget.CurrentTargetGuid == Root.MyGuid; }
        }

        /// <summary>
        /// Check to be used in any function which retrieves objects from the objectmanager.
        /// </summary>
        /// <param name="wowObject">wowObject to check if object does not equal null and isvalid.</param>
        /// <returns>Isviable or not.</returns>
        public static bool IsViable(WoWObject wowObject)
        {
            return (wowObject != null) && wowObject.IsValid;
        }

        /// <summary>
        /// Can be used to force a ObjectManager update - Can be used in Pulse()
        /// </summary>
        public static void UpdateObjectManager()
        {
            using (new PerformanceLogger("ObjManager Update"))
            {
                ObjectManager.Update();
            }
        }
        #endregion

        #region DeathKnight Things

        internal static bool AoeBPCheck { get { return NearbyAttackableUnits(Me.CurrentTarget.Location, 10).Count(x => !x.HasMyAura("Blood Plague")) > 2; } }
        #endregion

        #region HashSet Checks
        /// <summary>
        /// Checks if Unit is viable and if Unit is on Hashlist for this function.
        /// </summary>
        public static bool IsHamstringTarget
        {
            get
            {
                return IsViable(Me.CurrentTarget) && Me.CurrentTarget.HamstringUnitsList();
            }
        }

        /// <summary>
        /// Checks if Unit is viable and if Unit is on Hashlist for this function.
        /// </summary>
        public static bool IsTargetRare
        {
            get
            {
                return IsViable(Me.CurrentTarget) && Me.CurrentTarget.RareUnitsList();
            }
        }

        /// <summary>
        /// Checks if Unit is viable and if Unit is on Hashlist for this function.
        /// </summary>
        public static bool IsTargetBoss
        {
            get
            {
                return IsViable(Me.CurrentTarget) && Me.CurrentTarget.TargetIsBoss();
            }
        }

        /// <summary>
        /// Checks if Unit is viable and if Unit is on Hashlist for this function.
        /// </summary>
        public static bool IsDoNotUseOnTgt
        {
            get
            {
                return IsViable(Me.CurrentTarget) && !Me.CurrentTarget.DoNotUseOnTgtList();
            }
        }
        #endregion

        #region Range Checks
        /// <summary>
        /// Using Math.Abs calculating the actual maximum range for spell cast on U.
        /// </summary>
        public static float ActualMaxRange(WoWSpell spell, WoWUnit unit)
        {
            return Math.Abs(spell.MaxRange) < 1 ? 0 : (IsViable(unit) ? spell.MaxRange + unit.CombatReach + Me.CombatReach - 0.1f : spell.MaxRange);
        }

        /// <summary>
        /// Using Math.Abs calculating the actual minimum range for spell cast on U.
        /// </summary>
        public static float ActualMinRange(WoWSpell spell, WoWUnit unit)
        {
            return Math.Abs(spell.MinRange) < 1 ? 0 : (IsViable(unit) ? spell.MinRange + unit.CombatReach + Me.CombatReach + 0.1f : spell.MinRange);
        }
        #endregion
    }
}