﻿using CommonBehaviors.Actions;
using Waldo.Core;
using Waldo.Helpers;
using Waldo.Managers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = Waldo.Routines.WaGlobal;
using I = Waldo.Core.WaItem;
using Lua = Waldo.Helpers.WaLua;
using T = Waldo.Managers.WaTalentManager;
using SG = Waldo.Interfaces.Settings.WaSettings;
using SH = Waldo.Interfaces.Settings.WaSettingsH;
using Spell = Waldo.Core.WaSpell;
using U = Waldo.Core.WaUnit;
using Styx.WoWInternals;
using Styx.CommonBot;

namespace Waldo.Routines
{
    class WaCombat
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
                        new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        G.InitializeOnKeyActions(),
                        new Action(delegate { WaUnit.GetNearbyAttackableUnitsCount(); return RunStatus.Failure; }),
                        Spell.Cast("Blade Flurry", ret => U.NearbyAttackableUnitsCount > 1 && U.NearbyAttackableUnitsCount <= 8 && SG.Instance.Combat.AutoTurnOffBladeFlurry),
                        new Decorator(a => U.NearbyAttackableUnitsCount > 7 && SG.Instance.Combat.AutoTurnOffBladeFlurry, new Action(delegate { Me.CancelAura("Blade Flurry"); return RunStatus.Failure; })),
                        new Decorator(ret => SG.Instance.General.CheckAWaancedLogging, WaLogger.AWaancedLogging),
                        new Decorator(ret => !SG.Instance.Combat.PvPRotationCheck && SH.Instance.ModeSelection == WaEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Combat.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, ComDefensive()),
                                        new Decorator(ret => SG.Instance.Combat.CheckInterrupts, ComInterrupts()),
                                        ComUtility(),
                                        I.ComUseItems(),
                                        ComOffensive(),
                                        new Decorator(ret => SG.Instance.Combat.CheckAoE && U.AttackableMeleeUnitsCount > 1, ComMt()),
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
                                        new Decorator(ret => WaHotKeyManager.IsAoe && U.AttackableMeleeUnitsCount > 1, ComMt()),
                                        ComSt())));
            }
        }
        #endregion

        #region Rotation PvE
       internal static Composite ComSt()
        {
            return new PrioritySelector(
                Spell.Cast("Expose Armor", ret => G.WeakenedBlowsAura && SG.Instance.Combat.CheckExposeArmor),
                Spell.Cast("Redirect", ret => Me.RawComboPoints > 0 && WaLua.PlayerComboPts > 1),
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


        #endregion Booleans

    }
}
