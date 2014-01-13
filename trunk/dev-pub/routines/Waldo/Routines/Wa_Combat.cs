using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Styx;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.TreeSharp;
using Action = Styx.TreeSharp.Action;
using Styx.Common;
using System.Diagnostics;
using Styx.Common.Helpers;
using System.Drawing;
using Styx.WoWInternals.DBC;
using CommonBehaviors.Actions;
using Waldo.Core;
using Waldo.Helpers;
using Waldo.Managers;
using System.Windows.Forms;
using G = Waldo.Routines.WaGlobal;
using I = Waldo.Core.WaItem;
using Lua = Waldo.Helpers.WaLua;
using T = Waldo.Managers.WaTalentManager;
using SG = Waldo.Interfaces.Settings.WaSettings;
using SH = Waldo.Interfaces.Settings.WaSettingsH;
using Spell = Waldo.Core.WaSpell;
using U = Waldo.Core.WaUnit;

namespace Waldo.Routines
{
   static class WaCombat
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeCom
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, WaLogger.TreePerformance("InitializeCom")),
                        new Decorator(ret => (WaHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => WaHotKeyManager.IsSpecialKey, new PrioritySelector(Spell.Cast("Feint", ret => SG.Instance.Subtlety.EnableFeintUsage && !Me.HasAura("Feint")))),
                        G.InitializeOnKeyActions(),
                        G.ManualCastPause(),
                        new Styx.TreeSharp.Action(delegate { WaUnit.GetNearbyAttackableUnitsCount(); return RunStatus.Failure; }),
                        new Decorator(ret => U.NearbyAttackableUnitsCount > 1, new PrioritySelector(Spell.Cast("Blade Flurry", ret => SG.Instance.Combat.AutoTurnOffBladeFlurry && U.NearbyAttackableUnitsCount < 8))),
                        CreateBladeFlurryBehavior(),
                        new Decorator(ret => SG.Instance.General.CheckAWaancedLogging, WaLogger.AWaancedLogging),
                        new Decorator(ret => !SG.Instance.Combat.PvPRotationCheck && SH.Instance.ModeSelection == WaEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Combat.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, ComDefensive()),
                                        new Decorator(ret => SG.Instance.Combat.CheckInterrupts, ComInterrupts()),
                                        ComUtility(),
                                        I.ComUseItems(),
                                        ComOffensive(),
                                        new Decorator(ret => SG.Instance.Combat.CheckAoE && U.NearbyAttackableUnitsCount > 7, ComMt()),
                                            ComSt())),
                        new Decorator(ret => !SG.Instance.Combat.PvPRotationCheck && SH.Instance.ModeSelection == WaEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Combat.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, ComDefensive()),
                                        new Decorator(ret => SG.Instance.Combat.CheckInterrupts, ComInterrupts()),
                                        ComUtility(),
                                         new Decorator(ret => WaHotKeyManager.IsCooldown,
                                         new PrioritySelector(
                                                        I.ComUseItems(),
                                                        ComOffensive())),
                                        new Decorator(ret => WaHotKeyManager.IsAoe, ComMt()),
                                        ComSt())));
            }
        }
        #endregion

        #region Rotation PvE
       internal static Composite ComSt()
        {
            return new PrioritySelector(
                Spell.Cast("Expose Armor", ret => G.WeakenedBlowsAura && SG.Instance.Combat.CheckExposeArmor),
                Spell.Cast("Redirect", ret => Me.RawComboPoints > 0 && WaLua.PlayerComboPts < 1),
                Spell.Cast("Ambush", ret => Me.IsStealthed),
                Spell.Cast("Revealing Strike", ret => WaLua.PlayerComboPts < 5 && !G.RevealingStrike),
                Spell.Cast("Sinister Strike", ret => WaLua.PlayerComboPts < 5),
                Spell.Cast("Rupture", ret => SG.Instance.Combat.CheckRupture && (Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent >= 10 && !Me.HasAura("Blade Flurry") &&  WaLua.PlayerComboPts == 5 && Me.HasAura(5171) && (G.TargetRuptureFalling || !G.TargetHaveRupture)) && (
                   (SG.Instance.Combat.Rupture == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                   (SG.Instance.Combat.Rupture == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                   (SG.Instance.Combat.Rupture == WaEnum.AbilityTrigger.Always))),
                Spell.Cast("Eviscerate", ret => WaLua.PlayerComboPts > 4 && Me.HasAura("Slice and Dice")),
                Spell.Cast("Slice and Dice", ret => !G.SliceAndDiceEnevenom));
        }


        internal static Composite ComInterrupts()
        {
            return new PrioritySelector(
                   new Throttle(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Kick", ret => (SG.Instance.General.InterruptList == WaEnum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == WaEnum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId()))))),
                    new Throttle(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Deadly Throw", ret => G.Kick && WaTalentManager.HasTalent(4) && Lua.PlayerComboPts > 2 && (
                    (SG.Instance.General.InterruptList == WaEnum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == WaEnum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
                    ));
        }  

        internal static Composite ComMt()
        {
            return new PrioritySelector(
                Spell.Cast("Fan of Knives", ret => Me.IsWithinMeleeRange && WaLua.PlayerComboPts <= 4),
                Spell.Cast("Crimson Tempest", ret => WaLua.PlayerComboPts > 4)
                );
        }

        internal static Composite ComDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Recuperate", ret => Lua.PlayerComboPts >= SG.Instance.Combat.NumRecuperateComboPoints && Me.HealthPercent <= SG.Instance.Combat.NumRecuperateHP && SG.Instance.Combat.CheckRecuperate),
                Spell.Cast("Cloak of Shadows", ret => SG.Instance.Combat.CheckCloakofShadows && Me.HealthPercent <= SG.Instance.Combat.NumCloakofShadowsHP),
                Spell.Cast("Combat Readiness", ret => SG.Instance.Combat.CheckCombatReadiness && Me.HealthPercent <= SG.Instance.Combat.NumCombatReadiness && T.HasTalent(6)),
                Spell.Cast("Shadowstep", ret => SG.Instance.Combat.CheckShadowstep && Me.HealthPercent <= SG.Instance.Combat.NumShadowstep && T.HasTalent(11)),
                Spell.Cast("Evasion", ret => SG.Instance.Combat.CheckEvasion && Me.HealthPercent <= SG.Instance.Combat.NumEvasion),
                I.ComUseHealthStone()
                );
        }

        internal static Composite ComUtility()
        {
            return new PrioritySelector(

                );
        }

        internal static Composite ComOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Killing Spree", ret => (Lua.PlayerPower < 35 && WaSpell.HasCachedAura(Me, 5171, 0, 4000) && !Me.HasAura(13750)) && (
                   (SG.Instance.Combat.KillingSpree == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Combat.KillingSpree == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Combat.KillingSpree == WaEnum.AbilityTrigger.Always)
                   )),
                Spell.Cast("Adrenaline Rush", ret => (Lua.PlayerPower < 35 || Me.HasAura(121471)) && (
                    (SG.Instance.Combat.AdrenalineRush == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Combat.AdrenalineRush == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                   (SG.Instance.Combat.AdrenalineRush == WaEnum.AbilityTrigger.Always)
                   )),
                Spell.Cast("Shadow Blades", ret => Spell.HasCachedAura(Me, 5171, 0, 4000) && (
                    (SG.Instance.Combat.ShadowBlades == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Combat.ShadowBlades == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Combat.ShadowBlades == WaEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Marked for Death", ret => G.IloveyouSND && (
                    (SG.Instance.Combat.Tier6Abilities == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Combat.Tier6Abilities == WaEnum.AbilityTrigger.OnBossDummy && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Combat.Tier6Abilities == WaEnum.AbilityTrigger.Always)
                    )));
        }
        #endregion

        #region Booleans

        internal static Composite CreateBladeFlurryBehavior()
        {
            return new PrioritySelector(
                new Decorator(
                    ret => SG.Instance.Combat.AutoTurnOffBladeFlurry && Me.HasAura("Blade Flurry") && (AoeCount <= 1 || AoeCount > 7),
                    new Sequence(
                        new Styx.TreeSharp.Action(ret => WaLogger.DebugLog("/cancel Blade Flurry")),
                        new Styx.TreeSharp.Action(ret => Me.CancelAura("Blade Flurry")),
                        new Wait(TimeSpan.FromMilliseconds(500), ret => !Me.HasAura("Blade Flurry"), new ActionAlwaysSucceed())
                        )
                    )
                );
        }

        public static int AoeCount { get; set; }

        public static Styx.TreeSharp.Action CreateActionCalcAoeCount()
        {
            return new Styx.TreeSharp.Action(ret =>
            {
                if (Battlegrounds.IsInsideBattleground || NearbyUnfriendlyUnits.Any(u => u.Guid != Me.CurrentTargetGuid))
                    AoeCount = 1;
                else
                    AoeCount = NearbyUnfriendlyUnits.Count(u => u.Distance < (u.MeleeDistance() + 3));
                return RunStatus.Failure;
            });
        }

        public static float MeleeDistance(this WoWUnit unit)
        {
            return MeleeDistance(unit);
        }

        /// <summary>
        /// get melee distance between two units
        /// </summary>
        /// <param name="unit">unit</param>
        /// <param name="other">Me if null, otherwise second unit</param>
        /// <returns></returns>
        public static float MeleeDistance(this WoWUnit unit, WoWUnit other = null)
        {
            // abort if mob null
            if (unit == null)
                return 0;

            if (other == null)
            {
                if (unit.IsMe)
                    return 0;
                other = StyxWoW.Me;
            }

            // pvp, then keep it close
            if (unit.IsPlayer && other.IsPlayer)
                return 3.5f;

            return Math.Max(5f, other.CombatReach + 1.3333334f + unit.CombatReach);
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
                    if (t != null && ValidUnit(t) && SpellDistance(t) < maxSpellDist)
                    {
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        public static float SpellDistance(this WoWUnit unit, WoWUnit other = null)
        {
            // abort if mob null
            if (unit == null)
                return 0;

            // optional arg implying Me, then make sure not Mob also
            if (other == null)
                other = StyxWoW.Me;

            // pvp, then keep it close
            float dist = other.Location.Distance(unit.Location);
            dist -= other.CombatReach + unit.CombatReach;
            return Math.Max(0, dist);
        }


        public static bool ValidUnit(WoWUnit p, bool showReason = false)
        {
            if (p == null || !p.IsValid)
                return false;

            if (StyxWoW.Me.IsInInstance)
            {
        //        if (showReason) Logger.Write(invalidColor, "invalid attack unit {0} is an Instance Ignore Mob", p.SafeName());
                return false;
            }

            // Ignore shit we can't select
            if (!p.CanSelect)
            {
           //     if (showReason) Logger.Write(invalidColor, "invalid attack unit {0} cannot be Selected", p.SafeName());
                return false;
            }

            // Ignore shit we can't attack
            if (!p.Attackable)
            {
          //      if (showReason) Logger.Write(invalidColor, "invalid attack unit {0} cannot be Attacked", p.SafeName());
                return false;
            }

            // Duh
            if (p.IsDead)
            {
           //     if (showReason) Logger.Write(invalidColor, "invalid attack unit {0} is already Dead", p.SafeName());
                return false;
            }

            // check for enemy players here as friendly only seems to work on npc's
            if (p.IsPlayer)
                return p.ToPlayer().IsHorde != StyxWoW.Me.IsHorde;

            // Ignore friendlies!
            if (p.IsFriendly)
            {
            //    if (showReason) Logger.Write(invalidColor, "invalid attack unit {0} is Friendly", p.SafeName());
                return false;
            }

            // Dummies/bosses are valid by default. Period.
          //  if (p.IsTrainingDummy() || p.IsBoss())
           //     return true;

            // If it is a pet/minion/totem, lets find the root of ownership chain
        //    WoWUnit pOwner = GetPlayerParent(p);

            // ignore if owner is player, alive, and not blacklisted then ignore (since killing owner kills it)
         //   if (pOwner != null && pOwner.IsAlive && !Blacklist.Contains(pOwner, BlacklistFlags.Combat))
        //    {
              //  if (showReason) Logger.Write(invalidColor, "invalid attack unit {0} has a Player as Parent", p.SafeName());
       //         return false;
         //   }

            // And ignore critters (except for those ferocious ones) /non-combat pets
            if (p.IsNonCombatPet)
            {
              //  if (showReason) Logger.Write(invalidColor, "{0} is a Noncombat Pet", p.SafeName());
                return false;
            }

            // And ignore critters (except for those ferocious ones) /non-combat pets
            if (p.IsCritter && p.ThreatInfo.ThreatValue == 0 && !p.IsTargetingMyRaidMember)
            {
            //    if (showReason) Logger.Write(invalidColor, "{0} is a Critter", p.SafeName());
                return false;
            }
            /*
                        if (p.CreatedByUnitGuid != 0 || p.SummonedByUnitGuid != 0)
                            return false;
            */
            return true;
        }

        /// <summary>
        ///   Gets the nearby unfriendly units within 40 yards.
        /// </summary>
        /// <value>The nearby unfriendly units.</value>
        public static IEnumerable<WoWUnit> NearbyUnfriendlyUnits
        {
            get
            {
                return UnfriendlyUnits(40);
            }
        }



        #endregion Booleans

    }
}
