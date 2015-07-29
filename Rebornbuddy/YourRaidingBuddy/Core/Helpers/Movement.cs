using System;
using Clio.Utilities;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Interfaces;
using ff14bot.Managers;
using ff14bot.Objects;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot.Navigation;
using Clio.Common;
using YourRaidingBuddy.Settings;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Helpers
{
    public static class Movement
    {
        //npc interact 7f

        //Core.Player.CombatReach 0.5f 
        //Melee Spell 3f
        //actual range is Mine + Theirs + Spell
        public static Vector3 OldTargetLocation, MyOldLocation, AvoidTo;
        public static float OldHeading;
        public static int RunningCounter;
        public static bool Running, RangeToggle, RangeToggleFilter, Avoiding;
        public static TimeSpan AvoidTill;
        public static IPlayerMover MyMover = new SlideMover();
        #region Range
        public static float MeleeRange
        {
            get
            {
                return Core.Player.CombatReach + 3f + Core.Player.CurrentTarget.CombatReach;
            }
        }
        public static float ObjectMeleeRange(GameObject o)
        {
            return Core.Player.CombatReach + 3f + o.CombatReach;
            
        }
        public static float ObjectSpellRange(GameObject o)
        {
            return Core.Player.CombatReach + 25f + o.CombatReach;
        }
        public static float CombatMovementRange(GameObject o)
        {
            return o == null ? 0 : Math.Max(ObjectMeleeRange(o) - 0.3f, 3.5f);
        }
        public static bool IsInSafeMeleeRange(GameObject o)
        {
            if (o == null) return false;
            return o.Distance2D() < CombatMovementRange(o);
        }
        public static bool IsInSafeRange(GameObject o)
        {
            if (o == null) return false;
            switch (Core.Player.CurrentJob)
            {
                case ClassJobType.Ninja:
                case ClassJobType.Rogue:
                case ClassJobType.Dragoon:
                case ClassJobType.Lancer:
                case ClassJobType.Monk:
                case ClassJobType.Pugilist:
                case ClassJobType.Marauder:
                case ClassJobType.Warrior:
                case ClassJobType.Gladiator:
                case ClassJobType.Paladin:
                    return o.Distance2D() < CombatMovementRange(o);
                default:
                    return o.Distance2D() < ObjectSpellRange(o);
            }
        }
        public static bool TooClose(GameObject o)
        {
            if (o == null) return false;
            return o.Distance2D() < 1f;
        }
        public static void CheckRangeToggle(GameObject o)
        {
            //Core.Player.Distance2D(Core.Player.CurrentTarget.Location) > Core.Player.Distance2D(OldTargetLocation)) 
            if (!InternalSettings.Instance.General.Movement || !InternalSettings.Instance.General.StopInRange || o == null) return;
            if (!RangeToggle && IsInSafeRange(o) && o.InLineOfSight()) 
            {
                StopMove();
                RangeToggle = true;
            }//use last targObjId after i make universal
            if (RangeToggle && !IsInSafeRange(o)) RangeToggle = false;

        }
#endregion
        #region Face

#pragma warning disable 1998
        public static async Task CheckFace(GameObject o)
#pragma warning restore 1998
        {//&& !Helpers.Rogue.me.MovementInfo.IsStrafing
            if (InternalSettings.Instance.General.Movement && o != null && MovementManager.IsMoving 
                && !TooClose(o) && o.InLineOfSight() && !IsFacingMovement(Core.Player, o)
                ) Face(o);
        }
        public static void Face(GameObject o)
        {
            //StopMove();
            o.Face();
        }
        public static bool IsFacingMovement(GameObject face, GameObject location)
        {
            if (face == null || location == null) return false;
            var d = MathEx.NormalizeRadian(face.Heading - MathHelper.CalculateHeading(location.Location,face.Location));
            if (d > Math.PI) d = Math.Abs(d - 2 * (float)Math.PI);
            //Logging.Write("Differance: " + d);
            return d < 0.12;
        }
        public static bool IsFacingMovement(GameObject face, Vector3 location)
        {
            if (face == null) return false;
            var d = MathEx.NormalizeRadian(face.Heading - MathHelper.CalculateHeading(location, face.Location));
            if (d > Math.PI) d = Math.Abs(d - 2 * (float)Math.PI);
            //Logging.Write("Differance: " + d);
            return d < 0.12;
        }
        public static bool IsBehind(GameObject face, GameObject location)
        {
            if (face == null || location == null) return false;
            var d = MathEx.NormalizeRadian((face.Heading+(float)Math.PI) - MathHelper.CalculateHeading(location.Location, face.Location));
            if (d > Math.PI) d = Math.Abs(d - 2 * (float)Math.PI);
            //if (YourRaidingBuddy.WindowSettings as RogueSetting != null) Logging.Write("IsBehind: " + face.IsBehind + " Differance: " + d + " " + (0.87f * (YourRaidingBuddy.WindowSettings as RogueSetting).TrickIsBehindAdjustment / 100));
                return d < (0.87f * (InternalSettings.Instance.Ninja.TrickIsBehindAdjustment / 100));
            return d < 0.87f;
        }
        public static bool IsFacing(GameObject face,GameObject location)
        {
            if (face == null||location ==null) return false;
            var d = MathEx.NormalizeRadian(face.Heading - MathHelper.CalculateHeading(location.Location, face.Location));
            if (d > Math.PI) d = Math.Abs(d - 2 * (float)Math.PI);
            //Logging.Write("Differance: " + d);
            return d < 0.785f;
        }
        #endregion
        #region Move


        public static void CheckStuckRunning()
        {
            if (Core.Player.CurrentTarget != null)
            {
                if (!Avoiding && Core.Player.CurrentTarget.Distance2D() < 2f)
                {
                    var d = MathEx.NormalizeRadian(Core.Player.Heading - OldHeading);
                    if (d > Math.PI) d = Math.Abs(d - 2 * (float)Math.PI);
                    if (d > 1.5f) StopMove();
                }
                OldHeading = Core.Player.Heading;
            }
            if (InternalSettings.Instance.General.Movement && InternalSettings.Instance.General.MeshFreeMovement) //none mesh setting
            {
                if (Running && Core.Player.CurrentTarget != null && !IsInSafeRange(Core.Player.CurrentTarget) && RunningCounter > 400000)
                {
                    //write anti stuck by heading and running into oblivion in different directions.
                }

                if (Running && Core.Player.CurrentTarget != null && Spell.Gcd && IsInSafeRange(Core.Player.CurrentTarget)
                    && Core.Player.CurrentTarget.InLineOfSight())
                {
                    Running = false;
                    RunningCounter = 0;
                }
                else if (!Running && RunningCounter > 45)
                {
                    Logging.Write(Colors.SkyBlue, "[YourRaidingBuddy] Movement problem detected, switching to mesh.");
                    Running = true;
                    StopMove();
                }
                else if (Core.Player.Distance2D(MyOldLocation) > 3)
                {
                    MyOldLocation = Core.Player.Location;
                    if(!Running) RunningCounter = 0;
                }
                else if (Core.Player.CurrentTarget != null && !Avoiding
                    && RunningCounter < 500000 && MovementManager.IsMoving
                    && (Spell.GcdTime < 10
                    || !IsInSafeRange(Core.Player.CurrentTarget) || !Core.Player.CurrentTarget.InLineOfSight()))
                    RunningCounter++;
            }
        }
        public static bool StopMove()
        {

            if (MovementManager.IsMoving && InternalSettings.Instance.General.Movement)
            {
                Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Stop Moving");
                MyMover.MoveStop();
            }
            return false;
        }
        public static async Task<bool> MoveToTarget()
        {
            if (Core.Player.CurrentTarget == null || !InternalSettings.Instance.General.Movement || Core.Player.IsMounted
                || Core.Player.IsCasting && GameObjectManager.GetObjectByObjectId(Core.Player.SpellCastInfo.TargetId).InLineOfSight() 
                && (Core.Player.SpellCastInfo.SpellData == null
                    || GameObjectManager.GetObjectByObjectId(Core.Player.SpellCastInfo.TargetId).Distance2D() < Core.Player.SpellCastInfo.SpellData.Range)) return false;
            if (InternalSettings.Instance.General.Dodge && Avoiding) return true;
            await CheckFace(Core.Player.CurrentTarget);
            CheckRangeToggle(Core.Player.CurrentTarget);
            if (InternalSettings.Instance.General.DontMoveIfMovinToYou // && !(InternalSettings.Instance.General.Dodge && Target.ShouldDodgeBehind(Core.Player.CurrentTarget))
                && Core.Player.Distance2D(Core.Player.CurrentTarget.Location) < Core.Player.Distance2D(OldTargetLocation) && !MovementManager.IsMoving)
                return true;
            if (Core.Player.CurrentTarget == null) return false;
            if (!Core.Player.CurrentTarget.InLineOfSight() || !IsInSafeRange(Core.Player.CurrentTarget)
                //|| MovementManager.IsMoving && !IsFacingMovement(Core.Player, Core.Player.CurrentTarget)
                )
            {
                MoveTo(Core.Player.CurrentTarget);
                OldTargetLocation = Core.Player.CurrentTarget.Location;
                //Navigator.PlayerMover.MoveTowards(MathEx.GetPointAt(o.Location, CombatMovementRange(o), MathEx.NormalizeRadian(MathHelper.CalculateHeading(Core.Player.Location, o.Location))));
                return false;
            }
            OldTargetLocation = Core.Player.CurrentTarget.Location;
            return false;
        }

        public static MoveResult MoveBehind(GameObject c)
        {
            if (Core.Player.IsCasting || c == null || !InternalSettings.Instance.General.Movement || Core.Player.IsMounted) return MoveResult.Failed;
            //if (IsTargetMoving) return MoveToTarget(GameObject c);
           return Navigator.NavigationProvider.MoveToRandomSpotWithin(
                MathEx.GetPointAt(c.Location, ObjectMeleeRange(c)/ 2, MathEx.NormalizeRadian(c.Heading + (float)Math.PI)), 1);
        }
        public static MoveResult MoveTo(GameObject o)
        {//Math.Abs(o.Z - Core.Player.Z) > 1 ?
            //MovementManager.IsMoving && Movement.IsFacingMovement(Core.Player, o)
            if (o == null) return MoveResult.Failed;
            if (MovementManager.IsMoving && IsFacingMovement(Core.Player, o)) return MoveResult.Moving;
            if (!o.InLineOfSight() || Running || !InternalSettings.Instance.General.MeshFreeMovement
                || (Core.Player.Location.Z + 2) < o.Location.Z 
                || InternalSettings.Instance.General.MeshOnStillTargets && Core.Player.CurrentTarget.Distance2D(OldTargetLocation) < 0.05)
            {
                MoveResult mr = Navigator.MoveTo(o.Location);
                if (mr == MoveResult.ReachedDestination)
                {
                    StopMove();
                    Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Location Reached?");
                    return mr;
                }//add timer for generating/ed
                if (mr != MoveResult.Moving)
                    Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Movement Result: " + mr);
                if (mr == MoveResult.Done || mr == MoveResult.Moved || mr == MoveResult.Moving 
                    || mr == MoveResult.GeneratingPath || mr == MoveResult.PathGenerated) 
                    return mr;

            }
            if (InternalSettings.Instance.General.MeshFreeMovement)
            {
                Navigator.Clear();
                Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Moving Mesh Free");
                MyMover.MoveTowards(o.Location); 
            }
            //MathEx.GetPointAt(o.Location, CombatMovementRange(o), MathEx.NormalizeRadian(MathHelper.CalculateHeading(Core.Player.Location, o.Location))));
            return MoveResult.Moving;
            //return !o.InLineOfSight() ? Navigator.MoveTo(o.Location) : Navigator.NavigationProvider.MoveToRandomSpotWithin(MathEx.GetPointAt(o.Location, CombatMovementRange(o), MathEx.NormalizeRadian(MathHelper.CalculateHeading(Core.Player.Location, Core.Player.CurrentTarget.Location))), 1f);
        }
#pragma warning disable 1998
        public static async Task<bool> PullMove(GameObject o)
#pragma warning restore 1998
        {
            if (!InternalSettings.Instance.General.Movement || o == null) return false;
            CheckRangeToggle(o);
            OldTargetLocation = Core.Player.CurrentTarget.Location;
            if (o.InLineOfSight() && IsInSafeRange(o))
            {
                StopMove();
                return false;
            }
            if (!(MovementManager.IsMoving && IsFacingMovement(Core.Player, o))) MoveTo(o);
            return true;
        }
        //Navigator.NavigationProvider.CanFullyNavigateTo()
        //Strafing
#pragma warning disable 1998
        public static async Task<MoveResult> RunBehindTarget(GameObject c)
#pragma warning restore 1998
        {
            if (c == null) return MoveResult.Failed;
            var avoidPoint = MathEx.GetPointAt(c.Location, ObjectMeleeRange(c),
                MathEx.NormalizeRadian(c.Heading + (float) Math.PI));
            if (!(Core.Player.Distance2D(avoidPoint) > 1f)) return MoveResult.ReachedDestination;
            Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Running Behind Target");
            AvoidTo = avoidPoint;
            AvoidTill = DateTime.UtcNow.TimeOfDay + TimeSpan.FromSeconds(1.3);
            Avoiding = true;
            return MoveResult.Moving;
        }
#pragma warning disable 1998
        public static async Task<bool> Avoid()
#pragma warning restore 1998
        {
            if (!Avoiding) return false;
            if (AvoidTill < DateTime.UtcNow.TimeOfDay || Core.Player.Distance2D(AvoidTo) < 1f)
            {
                Avoiding = false;
                StopMove();
                return false;
            }
            if (!(IsFacingMovement(Core.Player, AvoidTo) && MovementManager.IsMoving))
                MyMover.MoveTowards(AvoidTo);
            return true;
        }
        public static async Task Avoidance()
        {
            if (!InternalSettings.Instance.General.Dodge || InternalSettings.Instance.General.Movement)
            if (Core.Player.CurrentTarget != null && !MovementManager.IsMoving && Target.ShouldDodgeBehind(Core.Player.CurrentTarget) 
                && (!Core.Player.CurrentTarget.IsBehind || TooClose(Core.Player.CurrentTarget))
                && IsInSafeMeleeRange(Core.Player.CurrentTarget))
                    await RunBehindTarget(Core.Player.CurrentTarget);
            if (Avoiding) await Avoid();
        }
        #endregion
    }
}