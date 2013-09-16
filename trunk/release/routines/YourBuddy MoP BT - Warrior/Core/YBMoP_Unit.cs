﻿using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System.Collections.Generic;
using System.Linq;
using YBMoP_BT_Warrior.Helpers;

namespace YBMoP_BT_Warrior.Core
{
    internal static class YBUnit
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        // Check if we can attack
        internal static bool DefaultCheck
        {
            get
            {
                return !Me.Mounted && Me.CurrentTarget != null && Me.CurrentTarget.Attackable &&
                       !Me.CurrentTarget.IsDead && Me.CurrentTarget.IsWithinMeleeRange;
            }
        }

        // Check if we don't fuck up while casting shouts ...
        internal static bool DefaultBuffCheck
        {
            get
            {
                return !Me.Mounted && !Me.IsDead && !Me.Combat && !Me.IsFlying && !Me.IsOnTransport &&
                       !Me.HasAura("Food") && !Me.HasAura("Drink");
            }
        }

        // Check if we can interrupt
        internal static bool CanInterrupt
        {
            get { return Me.CurrentTarget.IsCasting && Me.CurrentTarget.CanInterruptCurrentSpellCast; }
        }
        // Get UnitCount for Attackble Mobs.
        internal static IEnumerable<WoWUnit> AttackableUnits
        {
            get { return ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(u => u.Attackable && u.CanSelect && !u.IsFriendly && !u.IsDead && !u.IsNonCombatPet && !u.IsCritter); }
        }

        internal static IEnumerable<WoWUnit> AttackableMeleeUnits
        {
            get { return AttackableUnits.Where(u => u.IsWithinMeleeRange); }
        }

        internal static IEnumerable<WoWUnit> NearbyAttackableUnits(WoWPoint fromLocation, double radius)
        {
            var hostile = AttackableUnits;
            var maxDistance = radius * radius;
            return hostile.Where(x => x.Location.DistanceSqr(fromLocation) < maxDistance);
        }

        // Cache UnitCount for Attackable Mobs.
        public static int AttackableMeleeUnitsCount;
        public static void GetAttackableMeleeUnitsCount()
        {
            if (Me.CurrentTarget != null)
                AttackableMeleeUnitsCount = AttackableMeleeUnits.Count();
        }

        public static int NearbyAttackableUnitsCount;
        public static void GetNearbyAttackableUnitsCount()
        {
            if (Me.CurrentTarget != null)
                NearbyAttackableUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 8).Count();
        }

        // Get GroupCount for friendly players in raid.
        internal static IEnumerable<WoWPlayer> RaidMembers
        {
            get { return ObjectManager.GetObjectsOfType<WoWPlayer>(true, true).Where(u => u.CanSelect && !u.IsDead && u.IsInMyPartyOrRaid); }
        }

        // Is Targer a Boss or Dummy
        public static bool IsTargetBoss
        {
            get { return Me.CurrentTarget.TargetIsBoss(); }
        }

        public static bool IsShieldBarrierTarget
        {
            get { return Me.CurrentTarget.TargetShieldBarrierList(); }
        }

        public static bool IsShieldBlockTarget
        {
            get { return !Me.CurrentTarget.TargetShieldBarrierList(); }
        }

        public static bool IsDoNotUseOnTgt
        {
            get { return !Me.CurrentTarget.DoNotUseOnTgt(); }
        }
    }
}
