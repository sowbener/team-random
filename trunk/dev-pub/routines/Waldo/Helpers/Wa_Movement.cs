#region Revision info

/*
 * $Author: Storm & Wulf$
 * $Date:  $
 * $ID$
 * $Revision:  $
 * $URL: $
 * $LastChangedBy: Wulf$
 * $ChangesMade$
 */

#endregion Revision info

using System;
using System.Linq;
using CommonBehaviors.Actions;
using Waldo.Helpers;
using Logger = Waldo.Helpers.WaLogger;
using Styx.CommonBot.Inventory;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.POI;
using Spell = Waldo.Core.WaSpell;
using Styx.Helpers;
using Styx.Pathing;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;

namespace Waldo.Helpers
{
    internal static class Movement
    {
        /* putting all the Movement/Targeting/Resting logic here */

        public delegate WoWPoint LocationRetriever(object context);

        public delegate float DynamicRangeRetriever(object context);

        private const bool EnableBotTargetingOveride = false;
        internal static bool EnableMovement = true;
        private static bool EnableTargeting = true;

        #region Movement

        // this is used by the moving/facing behavior
        // computed as distance to target's bounding box
        private static float CombatMinDistance
        {
            get { return StyxWoW.Me.IsMelee() ? 1f : 30f; }
        }

        private static float CombatMaxDistance
        {
            get
            {
                return StyxWoW.Me.IsMelee() ? 3.2f : 35f;
                ;
            }
        }

        public static bool IsMelee(this WoWUnit unit)
        {
            {
                if (unit != null)
                {
                    switch (StyxWoW.Me.Class)
                    {
                        case WoWClass.Warrior:
                            return true;
                        case WoWClass.Paladin:
                            return StyxWoW.Me.Specialization != WoWSpec.PaladinHoly;
                        case WoWClass.Hunter:
                            return false;
                        case WoWClass.Rogue:
                            return true;
                        case WoWClass.Priest:
                            return false;
                        case WoWClass.DeathKnight:
                            return true;
                        case WoWClass.Shaman:
                            return StyxWoW.Me.Specialization == WoWSpec.ShamanEnhancement;
                        case WoWClass.Mage:
                            return false;
                        case WoWClass.Warlock:
                            return false;
                        case WoWClass.Druid:
                            return StyxWoW.Me.Specialization != WoWSpec.DruidRestoration &&
                                   StyxWoW.Me.Specialization != WoWSpec.DruidBalance;
                        case WoWClass.Monk:
                            return StyxWoW.Me.Specialization != WoWSpec.MonkMistweaver;
                    }
                }
                return false;
            }
        }

        private static bool IsFlyingUnit
        {
            get
            {
                return StyxWoW.Me.CurrentTarget != null &&
                       (StyxWoW.Me.CurrentTarget.IsFlying ||
                        StyxWoW.Me.CurrentTarget.Distance2DSqr < 5 * 5 &&
                        Math.Abs(StyxWoW.Me.Z - StyxWoW.Me.CurrentTarget.Z) >= 5);
            }
        }

        /// <summary>
        ///     Returns the current Melee range for the player Unit.DistanceToTargetBoundingBox(target)
        /// </summary>
        public static float MeleeRange
        {
            get
            {
                // If we have no target... then give nothing.
                // if (StyxWoW.Me.CurrentTargetGuid == 0)  // chg to GotTarget due to non-zero vals with no target in Guid
                if (!StyxWoW.Me.GotTarget)
                    return 0f;

                if (StyxWoW.Me.CurrentTarget.IsPlayer)
                    return 3.5f;

                return Math.Max(5f, StyxWoW.Me.CombatReach + 1.3333334f + StyxWoW.Me.CurrentTarget.CombatReach);
            }
        }

        /// <summary>
        ///     distance to the targets bounding box
        /// </summary>
        /// <returns>Returns the distance to the targets bounding box</returns>
        public static float DistanceToTargetBoundingBox()
        {
            return
                (float)
                (StyxWoW.Me.CurrentTarget == null
                     ? 999999f
                     : Math.Round(DistanceToTargetBoundingBox(StyxWoW.Me.CurrentTarget), 0));
        }

        /// <summary>get the distance of this point to our point (taking a stab at this description)</summary>
        /// <param name="target">unit to use as the distance check</param>
        /// <returns>The distance to target bounding box.</returns>
        public static float DistanceToTargetBoundingBox(WoWUnit target)
        {
            if (target != null)
            {
                return (float)Math.Max(0f, target.Distance - target.BoundingRadius);
            }
            return 99999;
        }

        /// <summary>
        ///     Returns true if the player is currently channeling a spell
        /// </summary>
        public static bool PlayerIsChanneling
        {
            get { return StyxWoW.Me.ChanneledCastingSpellId != 0; }
        }

        public static Composite CreateFaceTargetBehavior(float viewDegrees = 70f)
        {
            return CreateFaceTargetBehavior(ret => StyxWoW.Me.CurrentTarget);
        }

        public static Composite CreateFaceTargetBehavior(Spell.UnitSelectionDelegate toUnit, float viewDegrees = 70f)
        {
            return new Decorator(
                ret =>
                EnableMovement && toUnit != null && toUnit(ret) != null &&
                !StyxWoW.Me.IsMoving && !toUnit(ret).IsMe &&
                !StyxWoW.Me.IsSafelyFacing(toUnit(ret), viewDegrees),
                new Action(ret =>
                {
                    StyxWoW.Me.CurrentTarget.Face();
                    return RunStatus.Failure;
                }));
        }

        public static Composite CreateMoveToLosBehavior()
        {
            return CreateMoveToLosBehavior(ret => StyxWoW.Me.CurrentTarget);
        }

        public static Composite CreateMoveToLosBehavior(Spell.UnitSelectionDelegate toUnit)
        {
            return
                new Decorator(
                    ret =>
                    EnableMovement && toUnit != null && toUnit(ret) != null && toUnit(ret) != StyxWoW.Me &&
                    !toUnit(ret).InLineOfSpellSight,
                    new Action(ret => Navigator.MoveTo(toUnit(ret).Location)));
        }

        /// <summary>
        ///     Movement Behaviour
        /// </summary>
        public static Composite MovingFacingBehavior()
        {
            return MovingFacingBehavior(ret => StyxWoW.Me.CurrentTarget);
        }

        private static Composite MovingFacingBehavior(Spell.UnitSelectionDelegate onUnit)
        // TODO: Check if we have an obstacle in our way and clear it ALSO need a mounted CHECK!!.
        {
            WoWUnit badStuff =
                ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                             .Where(
                                 q =>
                                 q.CreatedByUnitGuid != StyxWoW.Me.Guid && q.FactionId == 14 && !q.Attackable &&
                                 q.Distance < 8)
                             .OrderBy(u => u.DistanceSqr)
                             .FirstOrDefault();
            return new Sequence(
                // No Target?
                // Targeting Enabled?
                // Aquire Target
                new DecoratorContinue(
                    ret =>
                    (onUnit == null || onUnit(ret) == null || onUnit(ret).IsDead || onUnit(ret).IsFriendly) &&
                    EnableTargeting,
                    new PrioritySelector(
                        ctx =>
                        {
                            // Clear our current target if its Dead or is Friendly.
                            if (ctx != null && (onUnit(ctx).IsDead || onUnit(ctx).IsFriendly))
                            {
                                Logger.DebugLog(
                                    " Target Appears to be dead or a Friendly. Clearing Current Target [" + ctx + "]");
                                StyxWoW.Me.ClearTarget();
                            }

                            // Aquires a target.
                            if (EnsureUnitTargeted != null)
                            {
                                // Clear our current target if its blacklisted.
                                if (Blacklist.Contains(EnsureUnitTargeted.Guid) || EnsureUnitTargeted.IsDead)
                                {
                                    Logger.DebugLog(
                                        " EnsureUnitTargeted Appears to be dead or Blacklisted. Clearing Current Target [" +
                                        EnsureUnitTargeted + "]");
                                    StyxWoW.Me.ClearTarget();
                                }

                                return EnsureUnitTargeted;
                            }

                            return null;
                        },
                        new Decorator(
                            ret => ret != null, //checks that the above ctx returned a valid target.
                            new Sequence(
                //new Action(ret => SysLog.DiagnosticLog(" CLU targeting activated. Targeting " + SysLog.SafeName((WoWUnit)ret))),
                // pending spells like mage blizard cause targeting to fail.
                //new DecoratorContinue(ctx => StyxWoW.Me.CurrentPendingCursorSpell != null,
                //        new Action(ctx => Lua.DoString("SpellStopTargeting()"))),
                                new Action(ret => ((WoWUnit)ret).Target()),
                                new WaitContinue(2, ret => onUnit(ret) != null && onUnit(ret) == (WoWUnit)ret,
                                                 new ActionAlwaysSucceed()))))),
                // Are we Facing the target?
                // Is the Target in line of site?
                // Face Target
                new DecoratorContinue(
                    ret =>
                    onUnit(ret) != null && !StyxWoW.Me.IsSafelyFacing(onUnit(ret), 45f) && onUnit(ret).InLineOfSight,
                    new Sequence(
                        new Action(ret => WoWMovement.Face(onUnit(ret).Guid)))),
                // Target in Line of site?
                // We are not casting?
                // We are not channeling?
                // Move to Location
                new DecoratorContinue(
                    ret =>
                    onUnit(ret) != null && !onUnit(ret).InLineOfSight && !StyxWoW.Me.IsCasting && !PlayerIsChanneling,
                    new Sequence(
                        new Action(ret => Logger.DebugLog(" Target not in LoS. Moving closer.")),
                        new Action(ret => Navigator.MoveTo(onUnit(ret).Location)))),
                // Blacklist targets TODO:  check this.
                new DecoratorContinue(
                    ret =>
                    onUnit(ret) != null && !StyxWoW.Me.IsInInstance &&
                    Navigator.GeneratePath(StyxWoW.Me.Location, onUnit(ret).Location).Length <= 0,
                    new Action(delegate
                    {
                        Blacklist.Add(StyxWoW.Me.CurrentTargetGuid, TimeSpan.FromDays(365));
                        Logger.DebugLog(" Failed to generate path to: {0} blacklisted!",
                                        StyxWoW.Me.CurrentTarget.Name);
                        return RunStatus.Success;
                    })),
                // Move away from bad stuff
                new DecoratorContinue(ret => badStuff != null,
                                      new Sequence(
                                          new Action(ret => Logger.DebugLog(" Movin out of bad Stuff.")),
                                          new Action(ret =>
                                          {
                                              if (badStuff != null)
                                              {
                                                  Logger.DebugLog(" Movin out of {0}.", badStuff);
                                                  Navigator.MoveTo(WoWMovement.CalculatePointFrom(
                                                      badStuff.Location, 10));
                                              }
                                          }))),
                // Target is greater than CombatMinDistance?
                // Target is Moving?
                // We are not moving Forward?
                // Move Forward to   wards target
                new DecoratorContinue(
                    ret => onUnit(ret) != null && DistanceToTargetBoundingBox() >= CombatMinDistance &&
                           onUnit(ret).IsMoving && !StyxWoW.Me.MovementInfo.MovingForward && onUnit(ret).InLineOfSight &&
                           !IsFlyingUnit,
                    new Sequence(
                        new Action(
                            ret =>
                            Logger.DebugLog(" Too far away from moving target (T[{0}] >= P[{1}]). Moving forward.",
                                            DistanceToTargetBoundingBox(), CombatMinDistance)),
                        new Action(ret => WoWMovement.Move(WoWMovement.MovementDirection.Forward)))),
                // Target is less than CombatMinDistance?
                // Target is Moving?
                // We are moving Forward
                // Stop Moving Forward
                new DecoratorContinue(ret => onUnit(ret) != null && DistanceToTargetBoundingBox() < CombatMinDistance &&
                                             onUnit(ret).IsMoving && StyxWoW.Me.MovementInfo.MovingForward &&
                                             onUnit(ret).InLineOfSight,
                                      new Sequence(
                                          new Action(
                                              ret =>
                                              Logger.DebugLog(
                                                  " Too close to target (T[{0}] < P[{1}]). Movement Stopped.",
                                                  DistanceToTargetBoundingBox(), CombatMinDistance)),
                                          new Action(ret => WoWMovement.MoveStop()))),
                // Target is not Moving?
                // Target is greater than CombatMaxDistance?
                // We are not Moving?
                // Move Forward
                new DecoratorContinue(ret => onUnit(ret) != null && !onUnit(ret).IsMoving &&
                                             DistanceToTargetBoundingBox() >= CombatMaxDistance &&
                                             onUnit(ret).InLineOfSight,
                                      new Sequence(
                                          new Action(
                                              ret =>
                                              Logger.DebugLog(
                                                  " Too far away from non moving target (T[{0}] >= P[{1}]). Moving forward.",
                                                  DistanceToTargetBoundingBox(), CombatMaxDistance)),
                                          new Action(
                                              ret =>
                                              WoWMovement.Move(WoWMovement.MovementDirection.Forward,
                                                               new TimeSpan(99, 99, 99))))),
                // Target is less than CombatMaxDistance?
                // We are Moving?
                // We are moving Forward?
                // Stop Moving
                new DecoratorContinue(ret => onUnit(ret) != null && DistanceToTargetBoundingBox() < CombatMaxDistance &&
                                             StyxWoW.Me.IsMoving && StyxWoW.Me.MovementInfo.MovingForward &&
                                             onUnit(ret).InLineOfSight,
                                      new Sequence(
                                          new Action(
                                              ret =>
                                              Logger.DebugLog(
                                                  " Too close to target  (T[{0}] < P[{1}]). Movement Stopped",
                                                  DistanceToTargetBoundingBox(), CombatMaxDistance)),
                                          new Action(ret => WoWMovement.MoveStop()))));
        }

        /// <summary>
        ///     Calculates the point to move behind the targets location
        /// </summary>
        /// <returns>the WoWpoint location to move to</returns>
        private static WoWPoint CalculatePointBehindTarget()
        {
            return
                StyxWoW.Me.CurrentTarget.Location.RayCast(
                    StyxWoW.Me.CurrentTarget.Rotation + WoWMathHelper.DegreesToRadians(150), MeleeRange - 2f);
        }

        /// <summary>
        ///     Will stop the player if we are moving.
        /// </summary>
        /// <returns>Runstatus.Success if stopped</returns>
        public static Composite EnsureMovementStoppedBehavior()
        {
            return new Decorator(
                ret => EnableMovement && StyxWoW.Me.IsMoving,
                new Action(ret => Navigator.PlayerMover.MoveStop()));
        }

        /// <summary>
        ///     Creates a move to melee range behavior. Will return RunStatus.Success if it has reached the location, or stopped in range. Best used at the end of a rotation.
        /// </summary>
        /// <remarks>
        ///     Created 5/1/2011.
        /// </remarks>
        /// <param name="stopInRange">true to stop in range.</param>
        /// <param name="range">The range.</param>
        /// <returns>.</returns>
        public static Composite CreateMoveToMeleeBehavior(bool stopInRange)
        {
            return CreateMoveToMeleeBehavior(ret => StyxWoW.Me.CurrentTarget.Location, stopInRange);
        }

        public static Composite CreateMoveToMeleeBehavior(LocationRetriever location, bool stopInRange)
        {
            return
                new Decorator(
                    ret => !StyxWoW.Me.IsCasting,
                    CreateMoveToLocationBehavior(location, stopInRange,
                                                 ret => StyxWoW.Me.CurrentTarget.IsPlayer ? 2f : MeleeRange));
        }

        /// <summary>
        ///     Creates a move to location behavior. Will return RunStatus.Success if it has reached the location, or stopped in range. Best used at the end of a rotation.
        /// </summary>
        /// <remarks>
        ///     Created 5/1/2011.
        /// </remarks>
        /// <param name="location">The location.</param>
        /// <param name="stopInRange">true to stop in range.</param>
        /// <param name="range">The range.</param>
        /// <returns>.</returns>
        public static Composite CreateMoveToLocationBehavior(LocationRetriever location, bool stopInRange,
                                                             DynamicRangeRetriever range)
        {
            // Do not fuck with this. It will ensure we stop in range if we're supposed to.
            // Otherwise it'll stick to the targets ass like flies on dog shit.
            // Specifying a range of, 2 or so, will ensure we're constantly running to the target. Specifying 0 will cause us to spin in circles around the target
            // or chase it down like mad. (PVP oriented behavior)
            return
                new Decorator(
                // Don't run if the movement is disabled.
                    ret => EnableMovement,
                    new PrioritySelector(
                        new Decorator(
                // Give it a little more than 1/2 a yard buffer to get it right. CTM is never 'exact' on where we land. So don't expect it to be.
                            ret => stopInRange && StyxWoW.Me.Location.Distance(location(ret)) < range(ret),
                            new PrioritySelector(
                                EnsureMovementStoppedBehavior(),
                // In short; if we're not moving, just 'succeed' here, so we break the tree.
                                new Action(ret => RunStatus.Success)
                                )
                            ),
                        new Action(ret => Navigator.MoveTo(location(ret)))
                        ));
        }

        /// <summary>
        ///     Creates a move to target behavior. Will return RunStatus.Success if it has reached the location, or stopped in range. Best used at the end of a rotation.
        /// </summary>
        /// <remarks>
        ///     Created 5/1/2011.
        /// </remarks>
        /// <param name="stopInRange">true to stop in range.</param>
        /// <param name="range">The range.</param>
        /// <returns>.</returns>
        public static Composite CreateMoveToTargetBehavior(bool stopInRange, float range)
        {
            return CreateMoveToTargetBehavior(stopInRange, range, ret => StyxWoW.Me.CurrentTarget);
        }

        /// <summary>
        ///     Creates a move to target behavior. Will return RunStatus.Success if it has reached the location, or stopped in range. Best used at the end of a rotation.
        /// </summary>
        /// <remarks>
        ///     Created 5/1/2011.
        /// </remarks>
        /// <param name="stopInRange">true to stop in range.</param>
        /// <param name="range">The range.</param>
        /// <param name="onUnit">The unit to move to.</param>
        /// <returns>.</returns>
        public static Composite CreateMoveToTargetBehavior(bool stopInRange, float range,
                                                           Spell.UnitSelectionDelegate onUnit)
        {
            return
                new Decorator(
                    ret => onUnit != null && onUnit(ret) != null && onUnit(ret) != StyxWoW.Me && !StyxWoW.Me.IsCasting,
                    CreateMoveToLocationBehavior(ret => onUnit(ret).Location, stopInRange, ret => range));
        }

        #endregion

        #region Targeting

        /// <summary>
        ///     Returns a valid target
        /// </summary>
        public static WoWUnit EnsureUnitTargeted
        {
            get
            {
                // If we have a RaF leader, then use its target.
                WoWPlayer rafLeader = RaFHelper.Leader;
                if (rafLeader != null && rafLeader.IsValid && !rafLeader.IsMe && rafLeader.Combat &&
                    rafLeader.CurrentTarget != null && rafLeader.CurrentTarget.IsAlive &&
                    !Blacklist.Contains(rafLeader.CurrentTarget))
                {
                    Logger.DebugLog(" targeting activated. *Engaging [{0}] Reason: RaFHelper*", rafLeader);
                    return rafLeader.CurrentTarget;
                }

                // Healers first


                // Enemys Attacking Us - battlegrounds only


                // Flag Carrier units  - battlegrounds only


                // Low Health units  - battlegrounds only


                // Check botpoi first and make sure our target is set to POI's object.
                if (BotPoi.Current.Type == PoiType.Kill)
                {
                    WoWObject obj = BotPoi.Current.AsObject;

                    if (obj != null)
                    {
                        if (StyxWoW.Me.CurrentTarget != obj)
                        {
                            Logger.DebugLog(" targeting activated. *Engaging [{0}] Reason: BotPoi*", (WoWUnit)obj);
                            return (WoWUnit)obj;
                        }
                    }
                }

                // Does the target list have anything in it? And is the unit in combat?
                // Make sure we only check target combat, if we're NOT in a BG. (Inside BGs, all targets are valid!!)
                WoWUnit firstUnit = Targeting.Instance.FirstUnit;
                if (firstUnit != null && firstUnit.IsAlive && !firstUnit.IsMe &&
                    (firstUnit.Combat) && !Blacklist.Contains(firstUnit))
                {
                    Logger.DebugLog(" Waldo targeting activated. *Engaging [{0}] Reason: Target list*", firstUnit);
                    return firstUnit;
                }

                // Check for Instancebuddy and Disable targeting


                // Target the unit everyone else is belting on.


                Logger.DebugLog(" targeting FAILED. *Reason: I cannot find a good target.*");
                return null;
            }
        }

        #endregion

        #region Resting

        public static Composite DefaultRestBehaviour()
        {
            return

                // Don't fucking run the rest behavior (or any other) if we're dead or a ghost. Thats all.
                new Decorator(
                    ret => !StyxWoW.Me.IsDead && !StyxWoW.Me.IsGhost && !StyxWoW.Me.IsCasting && EnableMovement && BotPoi.Current.Type != PoiType.Loot,
                    new PrioritySelector(

                // Make sure we wait out res sickness. Fuck the classes that can deal with it. :O
                        new Decorator(
                            ret => StyxWoW.Me.HasAura("Resurrection Sickness"),
                            new Action(ret => { })),

                // Check if we're allowed to eat (and make sure we have some food. Don't bother going further if we have none.
                        new Decorator(
                            ret =>
                            !StyxWoW.Me.IsSwimming && StyxWoW.Me.HealthPercent <= Waldo.Interfaces.Settings.WaSettings.Instance.General.HPMovementRest && !StyxWoW.Me.HasAura("Food") &&
                            Consumable.GetBestFood(false) != null && !StyxWoW.Me.IsCasting,
                            new PrioritySelector(
                                new Decorator(
                                    ret => StyxWoW.Me.IsMoving,
                                    new Action(ret => Navigator.PlayerMover.MoveStop())),
                                new Sequence(
                                    new Action(
                                        ret =>
                                        {
                                            Styx.CommonBot.Rest.FeedImmediate();
                                        }),
                                    Spell.CreateWaitForLagDuration()))),

                // Make sure we're a class with mana, if not, just ignore drinking all together! Other than that... same for food.
                        new Decorator(
                            ret =>

                                // TODO: Does this Druid check need to be still in here?
                            !StyxWoW.Me.IsSwimming && (StyxWoW.Me.PowerType == WoWPowerType.Mana || StyxWoW.Me.Class == WoWClass.Druid) &&
                            StyxWoW.Me.ManaPercent <= 60 &&
                            !StyxWoW.Me.HasAura("Drink") && Consumable.GetBestDrink(false) != null && !StyxWoW.Me.IsCasting,
                            new PrioritySelector(
                                new Decorator(
                                    ret => StyxWoW.Me.IsMoving,
                                    new Action(ret => Navigator.PlayerMover.MoveStop())),
                                new Sequence(
                                    new Action(ret =>
                                    {
                                        Styx.CommonBot.Rest.DrinkImmediate();
                                    }),
                                    Spell.CreateWaitForLagDuration()))),

                // This is to ensure we STAY SEATED while eating/drinking. No reason for us to get up before we have to.
                        new Decorator(
                            ret =>
                            (StyxWoW.Me.HasAura("Food") && StyxWoW.Me.HealthPercent < 95) ||
                            (StyxWoW.Me.HasAura("Drink") && StyxWoW.Me.PowerType == WoWPowerType.Mana && StyxWoW.Me.ManaPercent < 95),
                            new ActionAlwaysSucceed()),
                        new Decorator(
                            ret =>
                            ((StyxWoW.Me.PowerType == WoWPowerType.Mana && StyxWoW.Me.ManaPercent <= 60) ||
                             StyxWoW.Me.HealthPercent <= Waldo.Interfaces.Settings.WaSettings.Instance.General.HPMovementRest) && !StyxWoW.Me.CurrentMap.IsBattleground,
                            new Action(ret => Logger.DebugLog("We have no food/drink. Waiting to recover our health/mana back")))
                    ));
        }

        #endregion

        #region Pull

        public static Composite DefaultPullBehaviour() // I just through some deathknight moves in here but just put whatever your class can do I cbf!! -- wulf
        {
            return new PrioritySelector(
                   CreateMoveToLosBehavior(),
                   CreateFaceTargetBehavior(),
                   new Sequence(
                       new DecoratorContinue(ret => StyxWoW.Me.IsMoving,
                           new Action(ret => EnsureMovementStoppedBehavior())),
                       new WaitContinue(1, new ActionAlwaysSucceed())),
                    Spell.Cast("Sinister Strike", ret => StyxWoW.Me.IsWithinMeleeRange),
                    Spell.Cast("Throw", ret => StyxWoW.Me.CurrentTarget.Distance <= 30),
                   CreateMoveToMeleeBehavior(true));
        }

        #endregion
    }
}