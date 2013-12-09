using Bullseye.Helpers;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using SG = Bullseye.Interfaces.Settings.BsSettings;
using System.Collections.Generic;
using System.Linq;
using G = Bullseye.Routines.BsGlobal;
using System;

namespace Bullseye.Core
{
    internal static class BsUnit
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
            if (!DefaultCheck ||
                Blacklist.Contains(target.Guid, BlacklistFlags.All) ||
                BsSpell.SpellDistance(target) > range ||
                !IsEnemy(target) ||
                range > 5 &&
                BsSpell.SpellDistance(target) > 5 &&
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

        private static bool IsDummy(WoWUnit target)
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
        private static readonly List<WoWUnit> NearbyFriendlyUnits = new List<WoWUnit>();
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
                NearbyFriendlyUnits.Clear();
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
                                NearbyFriendlyUnits.Add(unit);
                                NearbyFriendlyPlayers.Add(unit);
                            }
                        }
                        else
                        {
                            FarFriendlyUnits.Add(unit);
                            if (unit.Distance <= 40)
                            {
                                NearbyFriendlyUnits.Add(unit);
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

        // Check if we don't Bsck up while casting shouts ...
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
                NearbyAttackableUnitsCount = NearbyAttackableUnits(StyxWoW.Me.Location, 25).Count();
        }

        internal static bool AoeBPCheck { get { return NearbyAttackableUnits(Me.CurrentTarget.Location, 10).Count(x => !x.HasMyAura("Blood Plague")) >= 3; } }

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
            get { return Me.CurrentTarget.UseAoEList() && Interfaces.Settings.BsSettings.Instance.General.CheckSpecialAoE; }
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
