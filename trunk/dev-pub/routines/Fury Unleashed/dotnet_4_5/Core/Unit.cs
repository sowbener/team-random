
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Utilities;
using FuryUnleashed.Interfaces.Settings;
using FuryUnleashed.Rotations;
using Styx;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Enum = FuryUnleashed.Core.Helpers.Enum;

namespace FuryUnleashed.Core
{
    internal static class Unit
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static readonly Random Random = new Random();

        public static WoWUnit VigilanceTarget;

        #region Caching RaidMembers Functions
        // RaidMembers IEnumerable
        internal static IEnumerable<WoWPlayer> RaidMembers
        {
            get { return ObjectManager.GetObjectsOfType<WoWPlayer>(true, true).Where(u => IsViable(u) && u.CanSelect && !u.IsDead && u.IsInMyPartyOrRaid); }
        }

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

        #region Caching AttackableUnits Functions

        // AttackableUnits IEnumerable
        internal static IEnumerable<WoWUnit> AttackableUnits
        {
            get { return ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(u => u.Attackable && u.CanSelect && !u.IsFriendly && !u.IsDead && !u.IsNonCombatPet && !u.IsCritter); }
        }

        // NearbyAttackableUnits IEnumerable
        internal static IEnumerable<WoWUnit> NearbyAttackableUnits(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableUnits;
            var maxDistance = radius * radius;
            return hostile.Where(x => x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        // NearbyAttackableUnits IEnumerable
        internal static IEnumerable<WoWUnit> NearbyCastingUnits(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableUnits;
            var maxDistance = radius * radius;
            return
                hostile.Where(
                    x =>
                        !x.IsFriendly && (x.IsCasting || x.IsChanneling) && (x.IsTargetingPet || x.IsTargetingMyPartyMember || x.IsTargetingMyRaidMember) &&
                        x.CanInterruptCurrentSpellCast && x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        // NearbyAttackableUnits IEnumerable
        internal static IEnumerable<WoWUnit> NearbyCastingUnitsTargetingMe(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableUnits;
            var maxDistance = radius * radius;
            return
                hostile.Where(
                    x =>
                        !x.IsFriendly && (x.IsCasting || x.IsChanneling) && x.IsTargetingPet &&
                        x.CanInterruptCurrentSpellCast && x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        // Counts
        public static int AttackableMeleeUnitsCount;
        public static void GetAttackableMeleeUnitsCount()
        {
            using (new PerformanceLogger("GetAttackableMeleeUnitsCount"))
                if (IsViable(Me.CurrentTarget))
                    AttackableMeleeUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 5).Count();
        }

        public static int InterruptableUnitsCount;
        public static void GetInterruptableUnitsCount()
        {
            using (new PerformanceLogger("GetInterruptableUnitsCount"))
                if (IsViable(Me.CurrentTarget))
                    InterruptableUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 10).Count(u => u.IsCasting && u.CanInterruptCurrentSpellCast);
        }

        public static int NearbyAttackableUnitsCount;
        public static float NearbyAttackableUnitsFloat;
        public static void GetNearbyAttackableUnitsCount()
        {
            using (new PerformanceLogger("GetNearbyAttackableUnitsCount"))
                if (IsViable(Me.CurrentTarget))
                    NearbyAttackableUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 8).Count();
                    NearbyAttackableUnitsFloat = NearbyAttackableUnits(StyxWoW.Me.Location, 8).Count();
        }

        public static int NearbySlamCleaveUnitsCount;
        public static float NearbySlamCleaveUnitsFloat;
        public static void GetNearbySlamCleaveUnitsCount()
        {
            using (new PerformanceLogger("GetNearbySlamCleaveUnitsCount"))
                if (IsViable(Me.CurrentTarget))
                    NearbySlamCleaveUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 2).Count();
                    NearbySlamCleaveUnitsFloat = NearbyAttackableUnits(StyxWoW.Me.Location, 2).Count();
        }

        public static int NeedThunderclapUnitsCount;
        public static void GetNeedThunderclapUnitsCount()
        {
            using (new PerformanceLogger("GetNeedThunderclapUnitsCount"))
                if (IsViable(Me.CurrentTarget))
                    NeedThunderclapUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 8).Count(u => !u.HasAura(AuraBook.DeepWounds));
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
    }
}
