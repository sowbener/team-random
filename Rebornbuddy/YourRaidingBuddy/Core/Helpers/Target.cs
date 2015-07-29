using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using System.Linq;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Helpers
{
    public class Target
    {
        public static uint[] SpellsFrontalNarrow = { 317, 325, 331, 334, 345, 346, 399, 411, 446, 448, 490, 495, 
            505, 511, 520, 545, 552, 566, 568, 571, 580, 588, 589, 604, 618, 692, 720, 865, 866, 909, 914, 924, 937, 974, 975, 1135, 3158 };
        public static Dictionary<uint, AoeType> Aoes = new Dictionary<uint, AoeType>
        {
            {490,AoeType.StripRegularFrontal},
            {940,AoeType.StripRegularFrontal},
            {974,AoeType.StripRegularFrontal},
            {975,AoeType.StripRegularFrontal},
            {569,AoeType.ConeNarrowBehind},
            {351,AoeType.ConeNarrowFrontal},
            {352,AoeType.ConeNarrowFrontal},
            {444,AoeType.ConeNarrowFrontal},
            {498,AoeType.ConeNarrowFrontal},
            {517,AoeType.ConeNarrowFrontal},
            {528,AoeType.ConeNarrowFrontal},
            {720,AoeType.ConeNarrowFrontal},
            {446,AoeType.ConeWideFrontal},
            {506,AoeType.ConeWideFrontal},
            {518,AoeType.ConeWideFrontal},
            {580,AoeType.ConeWideFrontal},
            {604,AoeType.ConeWideFrontal},
            {606,AoeType.ConeWideFrontal},
            {618,AoeType.ConeWideFrontal},
            {356,AoeType.ConeWideFrontal},
            {937,AoeType.ConeWideFrontal},
            {562,AoeType.CircleTargetLarge},
            {738,AoeType.CircleTargetLarge},
            {978,AoeType.CircleTargetLarge},
            {1311,AoeType.CircleTargetLarge},
            {563,AoeType.CircleTargetLargePersistent},
            {954,AoeType.CircleTargetSmall},
            {417,AoeType.CircleMobLarge},
            {1003,AoeType.CircleMobLarge},
            {1006,AoeType.CircleMobLarge},
            {377,AoeType.CircleMobLarge},
            {497,AoeType.CircleMobLarge},
            {336,AoeType.CircleMobSmall},
            {561,AoeType.CircleMobSmall},
            {596,AoeType.CircleMobSmall},
            {331,AoeType.ConeVeryNarrowFrontal},
            {547,AoeType.CircleMobLarge},
            {495,AoeType.ConeNarrowFrontal},
            {566,AoeType.ConeNarrowFrontal},
            {545,AoeType.ConeNarrowFrontal},
            //{,AoeType},
        };
        public static bool ShouldDodgeBehind(GameObject o)
        {
            return InternalSettings.Instance.General.Dodge && (Core.Player.CurrentTarget as Character) != null && (Core.Player.CurrentTarget as Character).IsCasting 
                && (ShouldDodgeBehind((Core.Player.CurrentTarget as Character).SpellCastInfo.ActionId)
                || GetAoeType((Core.Player.CurrentTarget as Character).SpellCastInfo.ActionId) == (AoeType.FlankingSafe | AoeType.ConeWideFrontal | AoeType.StripWideFrontal));
        }
        public static bool ShouldDodgeBehind(uint spell)
        {
            return SpellsFrontalNarrow.Contains(spell);
        }
        public static bool ShouldDodgeAway(GameObject o)
        {
            return InternalSettings.Instance.General.Dodge && (Core.Player.CurrentTarget as Character) != null && (Core.Player.CurrentTarget as Character).IsCasting
                && GetAoeType((Core.Player.CurrentTarget as Character).SpellCastInfo.ActionId) == AoeType.Circle;
        }
        public static AoeType GetAoeType(uint spell)
        {
            AoeType t;
            if (!Aoes.TryGetValue(spell, out t)) t = AoeType.None;
            return t;
        }
        static public IEnumerable<BattleCharacter> mNearbyEnemyUnits()
        {
                if (!InternalSettings.Instance.General.Targeting && !InternalSettings.Instance.General.Aoe) return null;
                return GameObjectManager.GetObjectsOfType<BattleCharacter>(true)
                .Where(unit => !IsAboveTheGround(unit) //&& unit.IsVisible 
                     && unit.Distance2D(Core.Player) <= 40 && unit.CanAttack && unit.IsTargetable
                    && !unit.IsDead)
                        .OrderBy(unit => unit.Distance2D()).ToList();
        }

        static public IEnumerable<BattleCharacter> mHostileUnits()
        {
            
                if (!InternalSettings.Instance.General.Targeting && !InternalSettings.Instance.General.Aoe) return null;
                return GameObjectManager.GetObjectsOfType<BattleCharacter>(true)
                    .Where(unit => !IsAboveTheGround(unit)
                        && (unit.CurrentTargetId == Core.Player.ObjectId
                        //|| unit.IsTargetingMyPartyMember || unit.IsPlayer
                       //|| unit.ObjectId == botBasePoi || unit.ObjectId == botBaseUnit || unit.ObjectId == botBaseEntity 
                        || unit.TaggerObjectId == Core.Player.ObjectId)
                        && unit.CanAttack && unit.IsTargetable && !unit.IsDead && unit.Distance2D() <= 40 )
                       
                            .OrderBy(unit => unit.Distance2D()).ToList();
            
        }
        // static public List<uint> mBlacklist { get; private set; }
        /*
        static public uint botBaseEntity { get; private set; }
        static public uint botBaseUnit { get; private set; }
        static public uint botBasePoi { get; private set; }
        static public void Pulse()
        {
            var tar = CombatTargeting.Instance;
            if (tar != null)
            {
                var poi = Poi.Current;
                BattleCharacter entity = tar.FirstEntity;
                BattleCharacter unit = tar.FirstUnit;
                BattleCharacter poiCharacter = poi.BattleCharacter;
                if (entity != null)
                    botBaseEntity= entity.ObjectId;
                if(tar.FirstUnit !=null)
                    botBaseUnit= unit.ObjectId;
                if (poiCharacter != null)
                    botBasePoi = poiCharacter.ObjectId;
            }
        }        */

        public static bool IsPoiTarget(GameObject u)
        {
            if (u == null) return false;
            return Poi.Current.Unit!=null && u.ObjectId == Poi.Current.Unit.ObjectId || Poi.Current.BattleCharacter!=null && u.ObjectId == Poi.Current.BattleCharacter.ObjectId;
        }
        static public bool AoeSafe { get; private set; } //to implement
        static public int EnemiesNearTarget(float range, GameObject target = null)
        {
            target = target ?? Core.Player.CurrentTarget;
            if (target == null)
                return 0;
            var tarLoc = target.Location;
            if (mNearbyEnemyUnits() == null) return 0;
            return mNearbyEnemyUnits().Count(u => u.ObjectId != target.ObjectId && u.Location.Distance3D(tarLoc) <= range);
        }
        public static bool IsAboveTheGround(GameObject u)
        {
            var height = u.Z - Core.Player.Z;
            return height > System.Math.Max(Core.Player.CombatReach - 0.1f + u.CombatReach, 2.5f);
        }
        static public bool IsCurTargetSpecial()
        {
            // will implement
            return false;
        }

#pragma warning disable 1998
        public static async Task<bool> EnsureValidTarget()
#pragma warning restore 1998
        {
            if (Core.Player.SpellCastInfo != null && Core.Player.CastingSpellId != 125 && Core.Player.SpellCastInfo.TargetId != GameObjectManager.EmptyGameObject)
            {
                var ct = GameObjectManager.GetObjectByObjectId(Core.Player.SpellCastInfo.TargetId);
                if (ct as Character != null && (ct as Character).IsDead)
                {
                    Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Cast Target Dead, Clearing Target");
                    Movement.StopMove();
                    if(Core.Player.IsCasting)
                        Actionmanager.StopCasting();
                    if (InternalSettings.Instance.General.Targeting)
                     return GetNewTarget();
                }
            }

            if (!InternalSettings.Instance.General.Targeting) return false;
                 //Core.Player.CurrentTarget != null && ( Core.Player.CurrentTarget.ObjectId == botBaseEntity
                 //|| Core.Player.CurrentTarget.ObjectId == botBaseUnit || Core.Player.CurrentTarget.ObjectId == botBasePoi) && 
            if ((Core.Player.CurrentTarget as Character) == null
                 || Core.Player.CurrentHealthPercent < 60 && mNearbyEnemyUnits() != null
                 && mHostileUnits().Count(unit => unit.Distance2D() <= 10) > 0 && Core.Player.CurrentTarget.Distance2D() > 25
                 || !Core.Player.CurrentTarget.CanAttack || (Core.Player.CurrentTarget as Character).IsDead)
                    GetNewTarget();
            return false;
        }
        static private bool GetNewTarget()
        {
            Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Looking for NewTarget");
            if (mHostileUnits() == null) return false;
            var nextUnit = mHostileUnits().FirstOrDefault();
            if (nextUnit != null)
            {
                Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Changing target to " + nextUnit.Name);
                Poi.Current = new Poi(nextUnit, PoiType.Kill);
                //nextUnit.Target();
            }
            else
            {
                if (mNearbyEnemyUnits() == null) return false;
                nextUnit = mNearbyEnemyUnits().FirstOrDefault();
                if (nextUnit != null)
                {
                    Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Changing target to " + nextUnit.Name);
                    Poi.Current = new Poi(nextUnit, PoiType.Kill);
                }
                else  if (Core.Player.HasTarget)
                {
                    //Core.Player.ClearTarget();
                    Poi.Clear("[Reborn] Invaild");
                    Logging.Write(Colors.OrangeRed, "[YourRaidingBuddy] Clearing Target");
                } 
            }
            return false;
        }

        public static uint LastTargetId;
        public static bool TargetChanged;

        public static void TargetChange()
        {
            if (Core.Player.CurrentTarget != null
                && Core.Player.CurrentTarget.Type == ff14bot.Enums.GameObjectType.BattleNpc
                && Core.Player.CurrentTarget.ObjectId != LastTargetId
                && Core.Player.CurrentTarget.ObjectId != 0)
            {
                LastTargetId = Core.Player.CurrentTarget.ObjectId;
                TargetChanged = true;
            }
        }
    }
}