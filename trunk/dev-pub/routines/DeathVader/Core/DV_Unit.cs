using DeathVader.Helpers;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System.Collections.Generic;
using System.Linq;

namespace DeathVader.Core
{
    internal static class DvUnit
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        // Check if we can attack
        internal static bool DefaultCheck
        {
            get
            {
                return !Me.Mounted && Me.CurrentTarget != null && Me.CurrentTarget.Attackable &&
                       !Me.CurrentTarget.IsDead;
            }
        }

        public static HashSet<uint> IgnoreMobs = new HashSet<uint>
            {
                52288, // Venomous Effusion (NPC near the snake boss in ZG. Its the green lines on the ground. We want to ignore them.)
                52302, // Venomous Effusion Stalker (Same as above. A dummy unit)
                52320, // Pool of Acid
                52525, // Bloodvenom

                52387, // Cave in stalker - Kilnara
            };

        // Check if we don't Dvck up while casting shouts ...
        internal static bool DefaultBuffCheck
        {
            get
            {
                return !Me.Mounted && !Me.IsDead && !Me.IsFlying && !Me.IsOnTransport &&
                       !Me.HasAura("Food") && !Me.HasAura("Drink");
            }
        }

        // Check if we can interrupt
        internal static bool CanInterrupt
        {
            get { return Me.CurrentTarget != null && Me.CurrentTarget.IsCasting && Me.CurrentTarget.CanInterruptCurrentSpellCast; }
        }

        private static bool IsStandingInGroundEffect()
        {
            {
                return
                    ObjectManager.GetObjectsOfType<WoWDynamicObject>().Where(obj => obj.Distance <= obj.Radius).Any(obj => obj.Caster.IsHostile);
            }
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
                NearbyAttackableUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 10).Count();
        }

        internal static bool AoeBPCheck { get { return NearbyAttackableUnits(Me.CurrentTarget.Location, 10).Count(x => !x.HasMyAura("Blood Plague")) > 2; } }

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

        public static bool IsAoETarget
        {
            get { return Me.CurrentTarget.UseAoEList() && Interfaces.Settings.DvSettings.Instance.General.CheckSpecialAoE; }
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
            get { return !Me.CurrentTarget.DoNotUseOnTgtList(); }
        }
    }
}
