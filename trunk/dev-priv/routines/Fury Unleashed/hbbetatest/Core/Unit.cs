using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Managers;
using FuryUnleashed.Core.Utilities;
using FuryUnleashed.Interfaces.Settings;
using FuryUnleashed.Rotations;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using Enum = FuryUnleashed.Core.Helpers.Enum;

namespace FuryUnleashed.Core
{
    internal static class Unit
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static readonly Random Random = new Random();
        private const int AttackableunitsExpiry = 100;
        private const int RaidmembersExpiry = 500;

        public static WoWUnit VigilanceTarget;
        public static List<WoWUnit> CachedAttackableUnitsList;
        public static List<WoWPlayer> CachedRaidMembersList;

        #region Caching RaidMembers Functions - Needs to be migrated to v2
        internal static IEnumerable<WoWUnit> NearbyRaidMembers(WoWPoint fromLocation, double radius)
        {
            var units = RaidMembers;
            var maxDistance = radius * radius;
            return units.Where(u => IsViable(u) && u.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        public static int RaidMembersNeedCryCount;
        public static void GetRaidMembersNeedCryCount()
        {
            using (new PerformanceLogger("GetRaidMembersNeedCryCount"))
                if (IsViable(Me.CurrentTarget))
                    RaidMembersNeedCryCount = NearbyRaidMembers(StyxWoW.Me.Location, 30).Count(u => u.Combat && !u.HasAura(97462) && (
                    (Global.IsArmsSpec && u.HealthPercent <= InternalSettings.Instance.Arms.CheckRallyingCryNum) ||
                    (Global.IsFurySpec && u.HealthPercent <= InternalSettings.Instance.Fury.CheckRallyingCryNum) ||
                    (Global.IsProtSpec && u.HealthPercent <= InternalSettings.Instance.Protection.CheckRallyingCryNum)
                    ));
        }

        internal static IEnumerable<WoWUnit> FriendlyUnits
        {
            get { return ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(u => IsViable(u) && u.IsFriendly && !u.IsDead && !u.IsNonCombatPet && !u.IsCritter); }
        }

        // NearbyAttackableUnits IEnumerable
        internal static IEnumerable<WoWUnit> NearbyFriendlyUnits(WoWPoint fromLocation, double radius)
        {
            var friendly = FriendlyUnits;
            var maxDistance = radius * radius;

            return friendly.Where(x => IsViable(x) && x.IsFriendly && x.Location.DistanceSqr(fromLocation) < maxDistance);
        }
        #endregion

        #region Cached Units v1 - Needs to be migrated to v2
        // NearbyAttackableUnits IEnumerable
        internal static IEnumerable<WoWUnit> NearbyAttackableUnits(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableUnits;
            var maxDistance = radius * radius;
            return hostile.Where(x => x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        // NearbyCastingUnits IEnumerable
        internal static IEnumerable<WoWUnit> NearbyCastingUnits(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableUnits;
            var maxDistance = radius * radius;
            return hostile.Where(x =>
                        !x.IsFriendly && (x.IsCasting || x.IsChanneling) && (x.IsTargetingPet || x.IsTargetingMyPartyMember || x.IsTargetingMyRaidMember) &&
                        x.CanInterruptCurrentSpellCast && x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        // NearbyCastingUnitsTargetingMe IEnumerable
        internal static IEnumerable<WoWUnit> NearbyCastingUnitsTargetingMe(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableUnits;
            var maxDistance = radius * radius;
            return hostile.Where(x =>
                        !x.IsFriendly && (x.IsCasting || x.IsChanneling) && x.IsTargetingPet &&
                        x.CanInterruptCurrentSpellCast && x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        public static int InterruptableUnitsCount;
        public static void GetInterruptableUnitsCount()
        {
            using (new PerformanceLogger("GetInterruptableUnitsCount"))
                if (IsViable(Me.CurrentTarget))
                    InterruptableUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 10).Count(u => u.IsCasting && u.CanInterruptCurrentSpellCast);
        }
        #endregion

        #region Unit Booleans
        public static bool GetVigilanceTarget()
        {
            using (new PerformanceLogger("VigilanceTarget"))
            {
                VigilanceTarget = null;

                var tankOnly = InternalSettings.Instance.General.Vigilance == Enum.VigilanceTrigger.OnTank;

                switch (InternalSettings.Instance.General.Vigilance)
                {
                    case Enum.VigilanceTrigger.Never:
                        return false;
                    default:
                        VigilanceTarget = (from u in NearbyRaidMembers(StyxWoW.Me.Location, 30)
                            where u.IsValid
                            where u.Guid != Me.Guid
                            where !tankOnly || u.HasAura(AuraBook.Vengeance)
                            where u.HealthPercent <= InternalSettings.Instance.General.VigilanceNum
                            select u).FirstOrDefault();
                            
                    return VigilanceTarget != null;
                }
            }
        }

        public static bool IsInRange(this WoWUnit unit, string spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                var spellId = results.Override != null ? results.Override.Id : results.Original.Id;
                return IsInRange(unit, WoWSpell.FromId(spellId));
            }
            return false;
        }

        public static bool IsInRange(this WoWUnit unit, int spell)
        {
            return IsInRange(unit, WoWSpell.FromId(spell));
        }

        public static bool IsInRange(this WoWUnit unit, WoWSpell spell)
        {
            return unit.Distance <= (spell.MaxRange + unit.CombatReach);
        }

        public static bool IsHamstringTarget
        {
            get { return IsViable(Me.CurrentTarget) && Me.CurrentTarget.HamstringUnitsList(); }
        }

        public static bool IsTargetRare
        {
            get { return IsViable(Me.CurrentTarget) && Me.CurrentTarget.RareUnitsList(); }
        }

        public static bool IsTargetBoss
        {
            get { return IsViable(Me.CurrentTarget) && Me.CurrentTarget.TargetIsBoss(); }
        }

        public static bool IsDoNotUseOnTgt
        {
            get { return IsViable(Me.CurrentTarget) && !Me.CurrentTarget.DoNotUseOnTgtList(); }
        }
        #endregion

        #region Self Functions
        // Using CombatReach - Range test
        public static float CalculatedMeleeRange
        {
            get
            {
                if (!StyxWoW.Me.GotTarget) { return 5f; }
                if (StyxWoW.Me.CurrentTarget.IsPlayer) { return 5f; }
                return Math.Max(5f, StyxWoW.Me.CombatReach + 1.3333334f + StyxWoW.Me.CurrentTarget.CombatReach);
            }
        }
		
		public static float ActualMaxRange(WoWSpell spell, WoWUnit unit)
        {
            return Math.Abs(spell.MaxRange) < 1 ? 0 : (IsViable(unit) ? spell.MaxRange + unit.CombatReach + Me.CombatReach - 0.1f : spell.MaxRange);
        }

        public static float ActualMinRange(WoWSpell spell, WoWUnit unit)
        {
            return Math.Abs(spell.MinRange) < 1 ? 0 : (IsViable(unit) ? spell.MinRange + unit.CombatReach + Me.CombatReach + 0.1f : spell.MinRange);
        }
        #endregion

        #region Simple Booleans
        internal static bool DefaultCheck
        {
            get
            {
                return IsViable(Me.CurrentTarget) && !Me.Mounted && Me.CurrentTarget.Attackable && !Me.CurrentTarget.IsDead && Me.CurrentTarget.IsWithinMeleeRange;
            }
        }

        internal static bool DefaultBuffCheck
        {
            get
            {
                return IsViable(Me) && !Me.Mounted && !Me.IsDead && !Me.IsFlying && !Me.IsOnTransport && !Me.IsChanneling && !Me.HasAura("Food") && !Me.HasAura("Drink");
            }
        }

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

        // Can be used in Pulse.
        public static void UpdateObjectManager()
        {
            using (new PerformanceLogger("ObjManager Update"))
                ObjectManager.Update();
        }

        public static bool IsViable(WoWObject wowObject)
        {
            return (wowObject != null) && wowObject.IsValid;
        }
        #endregion

        #region Cached Units v2
        /// <summary>
        /// Initialize the required lists for Cached Units.
        /// </summary>
        public static void InitializeCacheLists()
        {
            CachedAttackableUnitsList = new List<WoWUnit>();
            CachedRaidMembersList = new List<WoWPlayer>();

            Logger.DiagLogPu("FU: Caching Lists are created!");
        }

        /// <summary>
        /// Pulsed in FuryUnleashed.Rotations.Global.InitializeCaching() - Starts the filling of the CachedAttackableUnitsList
        /// </summary>
        public static Composite FillCacheLists
        {
            get 
            { return new Action(delegate
                {
                    CachedAttackableUnitsList = CacheAttackableUnits;
                    CachedRaidMembersList = CacheRaidMembers; 
                        return RunStatus.Failure;
                }); 
            }
        }

        /// <summary>
        /// Core check on Attackable Units - Further filtering happens with the cached list.
        /// </summary>
        internal static IEnumerable<WoWUnit> AttackableUnits
        {
            get { return ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(u => IsViable(u) && u.Attackable && u.CanSelect && !u.IsFriendly && !u.IsDead && !u.IsNonCombatPet && !u.IsCritter && u.Distance <= 60); }
        }

        // RaidMembers IEnumerable
        internal static IEnumerable<WoWPlayer> RaidMembers
        {
            get { return ObjectManager.GetObjectsOfType<WoWPlayer>(true, true).Where(u => IsViable(u) && u.CanSelect && !u.IsDead && u.IsInMyPartyOrRaid); }
        }

        /// <summary>
        /// Populates the CachedAttackableUnitsList on request of PulseCache Composite - Used in FuryUnleashed.Rotations.Global.InitializeCaching()
        /// </summary>
        public static List<WoWUnit> CacheAttackableUnits
        {
            get
            {
                const string cachekey = "AttackableUnits";
                var attackableUnits = CacheManager.Get<List<WoWUnit>>(cachekey);

                if (attackableUnits == null)
                {
                    attackableUnits = AttackableUnits.ToList();
                    CacheManager.Add(attackableUnits, cachekey, AttackableunitsExpiry);
                }
                return attackableUnits;
            }
        }

        /// <summary>
        /// Populates the CachedRaidMembersList on request of PulseCache Composite - Used in FuryUnleashed.Rotations.Global.InitializeCaching()
        /// </summary>
        public static List<WoWPlayer> CacheRaidMembers
        {
            get
            {
                const string cachekey = "RaidMembers";
                var raidMembers = CacheManager.Get<List<WoWPlayer>>(cachekey);

                if (raidMembers == null)
                {
                    raidMembers = RaidMembers.ToList();
                    CacheManager.Add(raidMembers, cachekey, RaidmembersExpiry);
                }
                return raidMembers;
            }
        }

        /// <summary>
        /// IEnumerable for retrieving WowUnits within a certain range from the cache (CachedAttackableUnitsList)
        /// </summary>
        /// <param name="fromLocation">DistanceSqr - 2y for SlamCleave - 5y for melee - 8y for AoE</param>
        /// <param name="radius">-</param>
        /// <returns></returns>
        internal static IEnumerable<WoWUnit> CachedAttackableUnits(WoWPoint fromLocation, double radius)
        {
            using (new PerformanceLogger("CachedNearbyAttackableUnits"))
            {
                var hostile = CachedAttackableUnitsList;
                var maxDistance = radius * radius;
                return hostile.Where(x => x.Location.DistanceSqr(fromLocation) < maxDistance);   
            }
        }

        /// <summary>
        /// Retrieves the count (Integer & Float) of melee units (5y) - Used in FuryUnleashed.Rotations.Global.InitializeCaching()
        /// </summary>
        public static int AttackableMeleeUnitsCount;
        public static void GetAttackableMeleeUnitsCount()
        {
            using (new PerformanceLogger("GetAttackableMeleeUnitsCount"))
            {
                if (IsViable(Me.CurrentTarget))
                {
                    AttackableMeleeUnitsCount = CachedAttackableUnits(StyxWoW.Me.Location, 5).Count();
                }
            }
        }

        /// <summary>
        /// Retrieves the count (Integer & Float) of nearby units (8y) - Used in FuryUnleashed.Rotations.Global.InitializeCaching()
        /// </summary>
        public static int NearbyAttackableUnitsCount;
        public static float NearbyAttackableUnitsFloat;
        public static void GetNearbyAttackableUnitsCount()
        {
            using (new PerformanceLogger("GetNearbyAttackableUnitsCount"))
            {
                if (IsViable(Me.CurrentTarget))
                {
                    NearbyAttackableUnitsCount = CachedAttackableUnits(StyxWoW.Me.Location, 8).Count();
                    NearbyAttackableUnitsFloat = CachedAttackableUnits(StyxWoW.Me.Location, 8).Count();
                }
            }
        }

        /// <summary>
        /// Retrieves the count (Integer & Float) of nearby units for the Slam Cleave logic (2y) - Used in FuryUnleashed.Rotations.Global.InitializeCaching()
        /// </summary>
        public static int NearbySlamCleaveUnitsCount;
        public static float NearbySlamCleaveUnitsFloat;
        public static void GetNearbySlamCleaveUnitsCount()
        {
            using (new PerformanceLogger("GetNearbySlamCleaveUnitsCount"))
            {
                if (IsViable(Me.CurrentTarget))
                {
                    NearbySlamCleaveUnitsCount = CachedAttackableUnits(StyxWoW.Me.Location, 2).Count();
                    NearbySlamCleaveUnitsFloat = CachedAttackableUnits(StyxWoW.Me.Location, 2).Count();
                }
            }
        }

        /// <summary>
        /// Retrieves the count (Integer) of nearby units which need Thunderclap (8y) - Used in FuryUnleashed.Rotations.Global.InitializeCaching()
        /// </summary>
        public static int NeedThunderclapUnitsCount;
        public static void GetNeedThunderclapUnitsCount()
        {
            using (new PerformanceLogger("GetNeedThunderclapUnitsCount"))
            {
                if (IsViable(Me.CurrentTarget))
                {
                    NeedThunderclapUnitsCount = CachedAttackableUnits(StyxWoW.Me.Location, 8).Count(u => !u.HasAura(AuraBook.DeepWounds));
                }
            }
        }

        /// <summary>
        /// Used for Multidotting Bloodthirst - Deep Wounds - STRING
        /// </summary>
        /// <param name="unit">Selected unit for Multidot - Retrieved from CachedAttackableUnitsList</param>
        /// <param name="debuff">Aura which unit has or do not has.</param>
        /// <param name="radius">0 by default - Only the unit no unit around it.</param>
        /// <param name="refreshDurationRemaining">If no unit without aura is found, it reapplies to the user on which the bufftimer is below refreshDurationRemaining</param>
        /// <returns></returns>
        internal static WoWUnit MultiDotUnits(WoWUnit unit, string debuff, double radius, int refreshDurationRemaining)
        {
            IEnumerable<WoWUnit> attackable = 
                CachedAttackableUnits(unit.Location, radius).Where(x => IsViable(x) && !x.IsPlayer).OrderByDescending(x => x.HealthPercent);

            var dotTarget = attackable.FirstOrDefault(x => !Spell.HasAura(x, debuff));

            if (dotTarget == null)
            {
                dotTarget = attackable.FirstOrDefault(x => Spell.HasAura(x, debuff, 0, refreshDurationRemaining));
            }
            return dotTarget;
        }

        /// <summary>
        /// Used for Multidotting Bloodthirst - Deep Wounds - INTEGER
        /// </summary>
        /// <param name="unit">Selected unit for Multidot - Retrieved from CachedAttackableUnitsList</param>
        /// <param name="debuff">Aura which unit has or do not has.</param>
        /// <param name="radius">0 by default - Only the unit no unit around it.</param>
        /// <param name="refreshDurationRemaining">If no unit without aura is found, it reapplies to the user on which the bufftimer is below refreshDurationRemaining</param>
        /// <returns></returns>
        internal static WoWUnit MultiDotUnits(WoWUnit unit, int debuff, double radius, int refreshDurationRemaining)
        {
            IEnumerable<WoWUnit> attackable = CachedAttackableUnits(unit.Location, radius).Where(x => IsViable(x) && !x.IsPlayer).OrderByDescending(x => x.HealthPercent);

            var dotTarget = attackable.FirstOrDefault(x => !Spell.HasAura(x, debuff));

            if (dotTarget == null)
            {
                dotTarget = attackable.FirstOrDefault(x => Spell.HasAura(x, debuff, 0, refreshDurationRemaining));
            }
            return dotTarget;
        }
        #endregion
    }
}
