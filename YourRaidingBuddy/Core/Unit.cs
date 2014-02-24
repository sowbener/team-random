
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
using Styx.TreeSharp;
using Styx.CommonBot;
using G = YourBuddy.Rotations.Global;

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
                return ObjectManager.GetObjectsOfType<WoWUnit>(true, false).Where(u => IsViable(u) && u.Attackable && u.CanSelect && !u.IsFriendly && !u.IsDead && !u.IsNonCombatPet && !u.IsCritter && u.Distance <= 45);
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

            return hostile.Where(x => x.Location.DistanceSqr(fromLocation) < maxDistance && !x.ExcludeFromAoE());
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

        /// <summary>
        /// Retrieves the count (Integer) of nearby units of my target (8y) - Used in YourBuddy.Rotations.Global.InitializeCaching()
        /// </summary>
        public static int NearbyTargetAttackableUnitsCount;
        public static void GetNearbyTargetAttackableUnitsCount()
        {
            using (new PerformanceLogger("GetNearbyTargetAttackableUnitsCount"))
            {
                if (IsViable(StyxWoW.Me.CurrentTarget))
                {
                    NearbyTargetAttackableUnitsCount = NearbyAttackableUnits(StyxWoW.Me.CurrentTarget.Location, 8).Count();
                }
            }
        }

        public static void AquireRangedTarget()
        {
            WoWUnit bestTarget = null;
            int bestClusterSize = 0;
            IEnumerable<WoWUnit> units = AttackableUnits.OrderByDescending(u => u.Distance);

            foreach (WoWUnit unit in units)
            {
                if ((unit.IsTargetingMyPartyMember || unit.IsTargetingMyRaidMember || unit.IsTargetingMeOrPet) && (bestTarget == null || NearbyAttackableUnits(unit.Location, 8).Count() > bestClusterSize))
                {
                    bestTarget = unit;
                    bestClusterSize = NearbyAttackableUnits(unit.Location, 8).Count();
                }
            }

            if (bestTarget != null)
            {
                bestTarget.Target();
                return;
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
                return !Me.Mounted && !Me.IsDead && DefaultTargetCheck;
            }
        }

        internal static bool DefaultTargetCheck
        {
            get
            {
                return IsViable(Me.CurrentTarget) && Me.CurrentTarget.Attackable && !Me.CurrentTarget.IsDead && Me.CurrentTarget.SpellDistance(Me.CurrentTarget) < 45f;
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

        #region Hunter Things

        #region GetUnits

        private static bool IsMyPartyRaidMember(WoWUnit target)
        {


            if (target.IsPlayer)
            {
                var player = target as WoWPlayer;
                if (player != null && (player == Me || player.IsInMyPartyOrRaid))
                {
                    return true;
                }
            }
            else
            {
                var player = target.CreatedByUnit as WoWPlayer;
                if (player != null && (player == Me || player.IsInMyPartyOrRaid))
                {
                    return true;
                }
            }

            return false;
        }

        private static readonly HashSet<uint> NeedHealUnit = new HashSet<uint>
            {
                62442, //Tsulong
                71404, //Wrathion <The Black Prince> 
                71166, //Wrathion <The Black Prince> 
                71165, //Wrathion <The Black Prince>
            };


        #region ClearEnemyandFriendListCache

        private static void ClearEnemyandFriendListCacheEvent(object sender, LuaEventArgs args)
        {


            GlobalCheckTime = DateTime.Now - TimeSpan.FromSeconds(30);

            EnemyListCache.Clear();
            FriendListCache.Clear();
        }

        #endregion

        #region GlobalCheck

        private static bool HasAuraPreparation;
        private static bool HasAuraArenaPreparation;
        private static bool InArena;
        private static bool InBattleground;
        private static bool InInstance;
        private static bool InDungeon;
        private static bool InRaid;
        private static DateTime GlobalCheckTime;

        private static void GlobalCheck()
        {
            HasAuraPreparation = Me.HasAura("Preparation");
            HasAuraArenaPreparation = Me.HasAura("Arena Preparation");

            if (GlobalCheckTime < DateTime.Now || Me.IsResting)
            {
                GlobalCheckTime = DateTime.Now + TimeSpan.FromSeconds(30);

                InArena = Me.CurrentMap.IsArena;

                InBattleground = Me.CurrentMap.IsBattleground;

                InInstance = Me.CurrentMap.IsInstance;

                InDungeon = Me.CurrentMap.IsDungeon;

                InRaid = Me.CurrentMap.IsRaid;
            }
        }
        #endregion

        #region EnemyListCache

        private static readonly Dictionary<ulong, DateTime> EnemyListCache = new Dictionary<ulong, DateTime>();

        private static void EnemyListCacheClear()
        {


            var indexToRemove =
                EnemyListCache.Where(
                    unit =>
                    unit.Value < DateTime.Now)
                              .Select(unit => unit.Key)
                              .ToList();



            foreach (var index in indexToRemove)
            {
                //Logging.Write("Remove {0} from EnemyList", EnemyList[index]);
                EnemyListCache.Remove(index);
            }


        }

        private static void EnemyListCacheAdd(WoWUnit unit, int expireSeconds = 60)
        {
            if (EnemyListCache.ContainsKey(unit.Guid)) return;
            //Logging.Write("Add {0} ({1}) to EnemyListCache", unit.Guid, unit.Name);
            EnemyListCache.Add(unit.Guid, DateTime.Now + TimeSpan.FromSeconds(expireSeconds));
        }

        #endregion

        #region FriendListCache

        private static readonly Dictionary<ulong, DateTime> FriendListCache = new Dictionary<ulong, DateTime>();

        private static void FriendListCacheClear()
        {


            var indexToRemove =
                FriendListCache.Where(
                    unit =>
                    unit.Value < DateTime.Now)
                               .Select(unit => unit.Key)
                               .ToList();


            foreach (var index in indexToRemove)
            {
                //Logging.Write("Remove {0} from FriendList", FriendList[index]);
                FriendListCache.Remove(index);
            }


        }

        private static void FriendListCacheAdd(WoWUnit unit, int expireSeconds = 60)
        {
            if (FriendListCache.ContainsKey(unit.Guid)) return;
            //Logging.Write("Add {0} ({1}) to FriendListCache", unit.Guid, unit.Name);
            FriendListCache.Add(unit.Guid, DateTime.Now + TimeSpan.FromSeconds(expireSeconds));
        }

        #endregion

        #region InLineOfSpellSightCache

        private static readonly Dictionary<ulong, DateTime> InLineOfSpellSightCache = new Dictionary<ulong, DateTime>();

        private static void InLineOfSpellSightCacheClear()
        {
            var indexToRemove =
                InLineOfSpellSightCache.Where(
                    unit =>
                    unit.Value < DateTime.Now)
                                       .Select(unit => unit.Key)
                                       .ToList();


            foreach (var index in indexToRemove)
            {
                InLineOfSpellSightCache.Remove(index);
            }
        }

        private static void InLineOfSpellSightCacheAdd(WoWUnit unit, int expireMilliseconds = 100)
        {
            if (InLineOfSpellSightCache.ContainsKey(unit.Guid)) return;
            //Logging.Write("Add {0} ({1}) to InLineOfSpellSightCache", unit.Guid, unit.Name);
            InLineOfSpellSightCache.Add(unit.Guid, DateTime.Now + TimeSpan.FromMilliseconds(expireMilliseconds));
        }

        private static bool InLineOfSpellSightCheck(WoWUnit target)
        {
            if (!DefaultCheck)
            {
                return false;
            }

            InLineOfSpellSightCacheClear();

            if (target == Me || InLineOfSpellSightCache.ContainsKey(target.Guid))
            {
                return true;
            }

            if (target.InLineOfSpellSight)
            {
                if (InRaid || InDungeon)
                {
                    InLineOfSpellSightCacheAdd(target, 300);
                    return true;
                }
                InLineOfSpellSightCacheAdd(target);
                return true;
            }
            return false;
        }

        #endregion

        internal static IEnumerable<WoWUnit> GetAllUnits()
        {
            return ObjectManager.GetObjectsOfType<WoWUnit>(true, true);
        }

        internal static bool Attackable(WoWUnit target, int range)
        {
            if (
                Blacklist.Contains(target.Guid, BlacklistFlags.All) ||
                Spell.SpellDistance(target) > range ||
                !IsEnemy(target) ||
                range > 5 &&
                !InLineOfSpellSightCheck(target))
            {
                return false;
            }
            return true;
        }

        private static bool DebuffCCBreakonDamage(WoWUnit target)
        {
            throw new NotImplementedException();
        }

        private static bool IsEnemy(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {


                if (EnemyListCache.ContainsKey(target.Guid))
                {
                    //Logging.Write("{0} in EnemyListCache, Skip Check!", target.Name);
                    return true;
                }

                if (FriendListCache.ContainsKey(target.Guid))
                {
                    //Logging.Write("{0} in EnemyListCache, Skip Check!", target.Name);
                    return false;
                }


                if (IsMyPartyRaidMember(target))
                {
                    //Logging.Write("{0} in IsMyPartyRaidMember, Skip Check!", target.Name);
                    return false;
                }

                if (InArena || InBattleground)
                {
                    return true;
                }

                if (!target.IsFriendly || IsDummy(target) && Me.Combat && Me.IsFacing(target))
                {
                    return true;
                }

                return false;
            }
        }

        public static IEnumerable<WoWUnit> NearbyUnfriendlyUnits
        {
            get
            {
                return UnfriendlyUnits(40);
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

        public static WoWUnit GetPlayerParent(WoWUnit unit)
        {
            // If it is a pet/minion/totem, lets find the root of ownership chain
            WoWUnit pOwner = unit;
            while (true)
            {
                if (pOwner.OwnedByUnit != null)
                    pOwner = pOwner.OwnedByRoot;
                else if (pOwner.CreatedByUnit != null)
                    pOwner = pOwner.CreatedByUnit;
                else if (pOwner.SummonedByUnit != null)
                    pOwner = pOwner.SummonedByUnit;
                else
                    break;
            }

            if (unit != pOwner && pOwner.IsPlayer)
                return pOwner;

            return null;
        }

        public static bool ValidUnit(WoWUnit p, bool showReason = false)
        {
            if (p == null || !p.IsValid)
                return false;

            if (StyxWoW.Me.IsInInstance && IgnoreMobs.Contains(p.Entry))
            {
                if (showReason) Logger.InfoLog("invalid attack unit {0} is an Instance Ignore Mob", p.SafeName);
                return false;
            }

            // Ignore shit we can't select
            if (!p.CanSelect)
            {
                if (showReason) Logger.InfoLog("invalid attack unit {0} cannot be Selected", p.SafeName);
                return false;
            }

            // Ignore shit we can't attack
            if (!p.Attackable)
            {
                if (showReason) Logger.InfoLog("invalid attack unit {0} cannot be Attacked", p.SafeName);
                return false;
            }

            // Duh
            if (p.IsDead)
            {
                if (showReason) Logger.InfoLog("invalid attack unit {0} is already Dead", p.SafeName);
                return false;
            }

            // check for enemy players here as friendly only seems to work on npc's
            if (p.IsPlayer)
                return p.ToPlayer().IsHorde != StyxWoW.Me.IsHorde;

            // Ignore friendlies!
            if (p.IsFriendly)
            {
                if (showReason) Logger.InfoLog("invalid attack unit {0} is Friendly", p.SafeName);
                return false;
            }

            // Dummies/bosses are valid by default. Period.
            if (Unit.IsDummy(Me.CurrentTarget) || p.TargetIsBoss())
                return true;

            // If it is a pet/minion/totem, lets find the root of ownership chain
            WoWUnit pOwner = GetPlayerParent(p);

            // ignore if owner is player, alive, and not blacklisted then ignore (since killing owner kills it)
            if (pOwner != null && pOwner.IsAlive && !Blacklist.Contains(pOwner, BlacklistFlags.Combat))
            {
                if (showReason) Logger.InfoLog("invalid attack unit {0} has a Player as Parent", p.SafeName);
                return false;
            }

            // And ignore critters (except for those ferocious ones) /non-combat pets
            if (p.IsNonCombatPet)
            {
                if (showReason) Logger.InfoLog("{0} is a Noncombat Pet", p.SafeName);
                return false;
            }

            // And ignore critters (except for those ferocious ones) /non-combat pets
            if (p.IsCritter && p.ThreatInfo.ThreatValue == 0 && !p.IsTargetingMyRaidMember)
            {
                if (showReason) Logger.InfoLog("{0} is a Critter", p.SafeName);
                return false;
            }
            if (p.IsCrowdControlled())
            {
                if (showReason) Logger.InfoLog("{0} is a crowd controlled", p.SafeName);
                return false;
            }
            /*
                        if (p.CreatedByUnitGuid != 0 || p.SummonedByUnitGuid != 0)
                            return false;
            */
            return true;
        }

        public static IEnumerable<WoWUnit> UnfriendlyUnits(int maxSpellDist)
        {
            Type typeWoWUnit = typeof(WoWUnit);
            Type typeWoWPlayer = typeof(WoWPlayer);
            List<WoWUnit> list = new List<WoWUnit>();
            List<WoWObject> objectList = ObjectManager.ObjectList;
            for (int i = 0; i < objectList.Count; i++)
            {
                Type type = objectList[i].GetType();
                if (type == typeWoWUnit || type == typeWoWPlayer)
                {
                    WoWUnit t = objectList[i] as WoWUnit;
                    if (t != null && ValidUnit(t) && t.SpellDistance() < maxSpellDist)
                    {
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// aura considered expired if spell of same name as aura is known and aura not present or has less than specified time remaining
        /// </summary>
        /// <param name="u">unit</param>
        /// <param name="aura">name of aura with spell of same name that applies</param>
        /// <returns>true if spell known and aura missing or less than 'secs' time left, otherwise false</returns>
        public static bool HasAuraExpired(this WoWUnit u, string aura, int secs = 3, bool myAura = true)
        {
            return u.HasAuraExpired(aura, aura, secs, myAura);
        }

        private static IEnumerable<WoWAura> AllAuras(this WoWUnit u)
        {
            return u.GetAllAuras();
        }

        internal static bool IsCrowdControlled(this WoWUnit unit)
        {
            if (unit != null)
            {
                return unit.AllAuras().Any(a =>
                                                    a.IsHarmful && (
                                                    a.Spell.Mechanic == WoWSpellMechanic.Banished ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Disoriented ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Charmed ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Horrified ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Incapacitated ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Polymorphed ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Sapped ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Shackled ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Asleep ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Frozen ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Invulnerable ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Invulnerable2 ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Turned ||
                                                    a.Spell.Mechanic == WoWSpellMechanic.Fleeing ||

                                                    // Really want to ignore hexed mobs.
                                                    a.Spell.Name == "Hex"));
            }
            return false;
        }

        /// <summary>
        /// aura considered expired if spell is known and aura not present or has less than specified time remaining
        /// </summary>
        /// <param name="u">unit</param>
        /// <param name="spell">spell that applies aura</param>
        /// <param name="aura">aura</param>
        /// <returns>true if spell known and aura missing or less than 'secs' time left, otherwise false</returns>
        public static bool HasAuraExpired(this WoWUnit u, string spell, string aura, int secs = 3, bool myAura = true)
        {
            // need to compare millisecs even though seconds are provided.  otherwise see it as expired 999 ms early because
            // .. of loss of precision
            return SpellManager.HasSpell(spell) && u.GetAuraTimeLeft(aura, myAura).TotalSeconds <= (double)secs;
        }

        internal static bool IsDummy(WoWUnit target)
        {
            if (!DefaultCheck)
            {
                return false;
            }
            return target.Entry == 31146 || // Raider's
                   target.Entry == 67127 || // Shine Training Dummy
                   target.Entry == 46647 || // 81-85
                   target.Entry == 32546 || // Ebon Knight's (DK)
                   target.Entry == 31144 || // 79-80
                   target.Entry == 32543 || // Veteran's (Eastern Plaguelands)
                   target.Entry == 32667 || // 70
                   target.Entry == 32542 || // 65 EPL
                   target.Entry == 32666 || // 60
                   target.Entry == 30527; // ?? Boss one (no idea?)
        }

        private static readonly List<WoWUnit> NearbyFriendlyPlayers = new List<WoWUnit>();
        private static readonly List<WoWUnit> NearbyFriendlyUnitss = new List<WoWUnit>();
        private static readonly List<WoWUnit> FarFriendlyPlayers = new List<WoWUnit>();
        private static readonly List<WoWUnit> FarFriendlyUnits = new List<WoWUnit>();
        private static readonly List<WoWUnit> NearbyUnFriendlyPlayers = new List<WoWUnit>();
        internal static readonly List<WoWUnit> NearbyUnFriendlyUnits = new List<WoWUnit>();
        private static readonly List<WoWUnit> FarUnFriendlyPlayers = new List<WoWUnit>(); //Don't use in this CC
        private static readonly List<WoWUnit> FarUnFriendlyUnits = new List<WoWUnit>();

        internal static Composite GetUnits()
        {
            return new Styx.TreeSharp.Action(delegate
            {

                NearbyFriendlyPlayers.Clear();
                NearbyFriendlyUnitss.Clear();
                FarFriendlyPlayers.Clear();
                FarFriendlyUnits.Clear();
                NearbyUnFriendlyPlayers.Clear();
                NearbyUnFriendlyUnits.Clear();
                FarUnFriendlyPlayers.Clear();
                FarUnFriendlyUnits.Clear();

                foreach (
                    var unit in
                        GetAllUnits()
                            .Where(
                                unit =>
                                    unit.CanSelect &&
                                    unit.IsAlive &&
                                    //  BsSpell.SpellDistance(unit) <=  &&
                                    !Blacklist.Contains(unit.Guid, BlacklistFlags.All)))
                {
                    if (IsMyPartyRaidMember(unit))
                    {
                        var player = unit as WoWPlayer;
                        if (player != null)
                        {
                            FarFriendlyPlayers.Add(unit);
                            FarFriendlyUnits.Add(unit);
                            if (unit.Distance <= 40)
                            {
                                NearbyFriendlyUnitss.Add(unit);
                                NearbyFriendlyPlayers.Add(unit);
                            }
                        }
                        else
                        {
                            FarFriendlyUnits.Add(unit);
                            if (unit.Distance <= 40)
                            {
                                NearbyFriendlyUnitss.Add(unit);
                            }
                        }
                    }
                    else if (IsEnemy(unit))
                    {
                        var player = unit as WoWPlayer;
                        if (player != null)
                        {
                            FarUnFriendlyPlayers.Add(unit);
                            FarUnFriendlyUnits.Add(unit);
                            if (unit.Distance <= 40)
                            {
                                NearbyUnFriendlyUnits.Add(unit);
                                NearbyUnFriendlyPlayers.Add(unit);
                            }
                        }
                        else
                        {
                            FarUnFriendlyUnits.Add(unit);
                            if (unit.Distance <= 40)
                            {
                                NearbyUnFriendlyUnits.Add(unit);
                            }
                        }
                    }
                }
                return RunStatus.Failure;
            });
        }

        #endregion


        #endregion

        #region DeathKnight Things

        internal static bool AoeBPCheck { get { return NearbyAttackableUnits(Me.CurrentTarget.Location, 10).Count(x => !x.HasAura("Blood Plague")) > 2; } }
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