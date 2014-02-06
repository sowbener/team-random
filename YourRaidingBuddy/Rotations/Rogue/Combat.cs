using CommonBehaviors.Actions;
using YourBuddy.Core;
using YourBuddy.Core.Helpers;
using YourBuddy.Core.Managers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = YourBuddy.Rotations.Global;
using I = YourBuddy.Core.Item;
using Lua = YourBuddy.Core.Helpers.LuaClass;
using T = YourBuddy.Core.Managers.TalentManager;
using SG = YourBuddy.Interfaces.Settings.InternalSettings;
using SH = YourBuddy.Interfaces.Settings.SettingsH;
using U = YourBuddy.Core.Unit;
using Styx.CommonBot;
using Styx.WoWInternals;
using System.Collections.Generic;
using System;
using Enum = YourBuddy.Core.Helpers.Enum;
using YourBuddy.Core.Utilities;

namespace YourBuddy.Rotations.Rogue
{
   static class Combat
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeCom
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                       new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Feint", ret => SG.Instance.Subtlety.EnableFeintUsage && !Me.HasAura("Feint")))),
                        G.InitializeOnKeyActionsR(),
                        G.ManualCastPause(),
                        new Decorator(ret => U.NearbyAttackableUnitsCount > 1 || U.NearbyAttackableUnitsCount < 8, new PrioritySelector(Spell.Cast("Blade Flurry", ret => SG.Instance.Combat.AutoTurnOffBladeFlurry))),
                       CreateBladeFlurryBehavior(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Combat.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, ComDefensive()),
                                        new Decorator(ret => SG.Instance.Combat.CheckInterrupts, ComInterrupts()),
                                        ComUtility(),
                                       new Styx.TreeSharp.Action(ret => { Item.UseCombatItems(); return RunStatus.Failure; }),
                                       new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        ComOffensive(),
                                        new Decorator(ret => SG.Instance.Combat.CheckAoE && U.NearbyAttackableUnitsCount > 7, ComMt()),
                                            ComSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Combat.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, ComDefensive()),
                                        new Decorator(ret => SG.Instance.Combat.CheckInterrupts, ComInterrupts()),
                                        ComUtility(),
                                         new Decorator(ret => HotKeyManager.IsCooldown,
                                         new PrioritySelector(
                                          new Styx.TreeSharp.Action(ret => { Item.UseCombatItems(); return RunStatus.Failure; }),
                                          new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                                        ComOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, ComMt()),
                                        ComSt())));
            }
        }
        #endregion

        #region Rotation PvE
       internal static Composite ComSt()
        {
            return new PrioritySelector(
                Spell.Cast("Expose Armor", ret => G.WeakenedBlowsAura && SG.Instance.Combat.CheckExposeArmor),
                Spell.Cast("Redirect", ret => Me.RawComboPoints > 0 && Lua.PlayerComboPts < 1),
                Spell.Cast("Ambush", ret => Me.IsStealthed),
                Spell.Cast("Slice and Dice", ret => !Me.HasAura("Slice and Dice") || Spell.GetAuraTimeLeft("Slice and Dice") < 3),
                Spell.Cast("Revealing Strike", ret => Lua.PlayerComboPts < 5 && (Me.CurrentTarget.HasAura("Revealing Strike") || !G.RevealingStrike)),
                Spell.Cast("Sinister Strike", ret => Lua.PlayerComboPts < 5 && G.RevealingStrike),
                Spell.Cast("Rupture", ret => SG.Instance.Combat.CheckRupture && (Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent >= 10 && !Me.HasAura("Blade Flurry") &&  Lua.PlayerComboPts == 5 && Me.HasAura(5171) && (G.TargetRuptureFalling || !G.TargetHaveRupture)) && (
                   (SG.Instance.Combat.Rupture == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                   (SG.Instance.Combat.Rupture == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                   (SG.Instance.Combat.Rupture == Enum.AbilityTrigger.Always))),
                Spell.Cast("Eviscerate", ret => Lua.PlayerComboPts > 4 && Me.HasAura("Slice and Dice")));
        }


        internal static Composite ComInterrupts()
        {
            return new PrioritySelector(
                   new Throttle(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Kick", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId()))))),
                    new Throttle(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Deadly Throw", ret => G.Kick && TalentManager.IsSelected(4) && Lua.PlayerComboPts > 2 && (
                    (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
                    ));
        }  

        internal static Composite ComMt()
        {
            return new PrioritySelector(
                Spell.Cast("Fan of Knives", ret => Me.IsWithinMeleeRange && Lua.PlayerComboPts <= 4),
                Spell.Cast("Crimson Tempest", ret => Lua.PlayerComboPts > 4)
                );
        }

        internal static Composite ComDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Recuperate", ret => Lua.PlayerComboPts >= SG.Instance.Combat.NumRecuperateComboPoints && Me.HealthPercent <= SG.Instance.Combat.NumRecuperateHP && SG.Instance.Combat.CheckRecuperate),
                Spell.Cast("Cloak of Shadows", ret => SG.Instance.Combat.CheckCloakofShadows && Me.HealthPercent <= SG.Instance.Combat.NumCloakofShadowsHP),
                Spell.Cast("Combat Readiness", ret => SG.Instance.Combat.CheckCombatReadiness && Me.HealthPercent <= SG.Instance.Combat.NumCombatReadiness && T.IsSelected(6)),
                Spell.Cast("Shadowstep", ret => SG.Instance.Combat.CheckShadowstep && Me.HealthPercent <= SG.Instance.Combat.NumShadowstep && T.IsSelected(11)),
                Spell.Cast("Evasion", ret => SG.Instance.Combat.CheckEvasion && Me.HealthPercent <= SG.Instance.Combat.NumEvasion),
                Item.CombatUseHealthStone()
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
                Spell.Cast("Killing Spree", ret => (Lua.PlayerPower < 35 && G.SNDSetting > 4 && !Me.HasAura(13750)) && (
                   (SG.Instance.Combat.KillingSpree == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Combat.KillingSpree == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Combat.KillingSpree == Enum.AbilityTrigger.Always)
                   )),
                Spell.Cast("Adrenaline Rush", ret => (Lua.PlayerPower < 35 || Me.HasAura(121471)) && (
                    (SG.Instance.Combat.AdrenalineRush == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Combat.AdrenalineRush == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                   (SG.Instance.Combat.AdrenalineRush == Enum.AbilityTrigger.Always)
                   )),
                Spell.Cast("Shadow Blades", ret => G.SNDSetting > 4 && (
                    (SG.Instance.Combat.ShadowBlades == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Combat.ShadowBlades == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Combat.ShadowBlades == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Marked for Death", ret => G.IloveyouSND && (
                    (SG.Instance.Combat.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Combat.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Combat.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )));
        }
        #endregion

        #region Booleans

        internal static Composite CreateBladeFlurryBehavior()
        {
            return new PrioritySelector(
                new Decorator(
                    ret => SG.Instance.Combat.AutoTurnOffBladeFlurry && Me.HasAura("Blade Flurry") && (U.NearbyAttackableUnitsCount > 7 || U.NearbyAttackableUnitsCount < 2),
                    new Sequence(
                        new Styx.TreeSharp.Action(ret => Logger.DiagLogFb("/cancel Blade Flurry")),
                        new Styx.TreeSharp.Action(ret => Me.CancelAura("Blade Flurry")),
                        new Wait(TimeSpan.FromMilliseconds(500), ret => !Me.HasAura("Blade Flurry"), new ActionAlwaysSucceed())
                        )
                    )
                );
        }


        #endregion Booleans

    }
}
