using CommonBehaviors.Actions;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using YBMoP_BT_Rogue.Core;
using YBMoP_BT_Rogue.Helpers;
using YBMoP_BT_Rogue.Managers;
using G = YBMoP_BT_Rogue.Routines.YBGlobal;
using I = YBMoP_BT_Rogue.Core.YBItem;
using Lua = YBMoP_BT_Rogue.Helpers.YBLua;
using SG = YBMoP_BT_Rogue.Interfaces.Settings.YBSettingsG;
using SP = YBMoP_BT_Rogue.Interfaces.Settings.YBSettingsCom;

namespace YBMoP_BT_Rogue.Routines
{
    class YBCom
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeCom
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => SG.Instance.CheckTreePerformance,
                        YBLogger.TreePerformance("InitializeCom")),
                    new Decorator(ret => (HotKeyManager.IsPaused || !YBUnit.DefaultCheck), new ActionAlwaysSucceed()),
                        new Action(delegate { YBSpell.GetCachedAuras(); return RunStatus.Failure; }),
                        new Action(delegate { YBUnit.GetAttackableMeleeUnitsCount(); return RunStatus.Failure; }),
                        new Decorator(ret => SG.Instance.CheckAdvancedLogging,
                            YBLogger.AdvancedLogging),
                        new Decorator(ret => SG.Instance.ModeChoice == Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => SP.Instance.CheckAutoAttack, Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100, ComDefensive()),
                                new Decorator(ret => SP.Instance.CheckInterrupts && YBUnit.CanInterrupt, ComInterrupts()),
                                ComUtility(),
                                I.ComUseItems(),
                                ComOffensive(),
                                new Decorator(ret => SP.Instance.CheckAoE && YBUnit.AttackableMeleeUnitsCount > 3, ComMt()),
                                ComSt())),
                        new Decorator(ret => SG.Instance.ModeChoice == Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => SP.Instance.CheckAutoAttack, Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100, ComDefensive()),
                                new Decorator(ret => SP.Instance.CheckInterrupts && YBUnit.CanInterrupt, ComInterrupts()),
                                ComUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.ComUseItems(),
                                        ComOffensive())),
                                new Decorator(ret => HotKeyManager.IsAoe && SP.Instance.CheckAoE && YBUnit.AttackableMeleeUnitsCount > 3, ComMt()),
                                ComSt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite ComSt()
        {
            return new PrioritySelector(
                new Decorator(ret => HotKeyManager.IsSpecialKey, new PrioritySelector(YBSpell.Cast("Tricks of the Trade", u => TricksTarget, ret => TricksTarget != null))),
                YBSpell.Cast("Expose Armor", ret => G.WeakenedBlowsAura && SP.Instance.CheckExposeArmor),
                YBSpell.Cast("Redirect", ret => Me.RawComboPoints > 0 && YBLua.PlayerComboPts < 1),
                YBSpell.Cast("Ambush", ret => Me.IsStealthed),
                YBSpell.Cast("Revealing Strike", ret => YBLua.PlayerComboPts < 5 && !G.RevealingStrike),
                YBSpell.Cast("Sinister Strike", ret => YBLua.PlayerComboPts < 5),
                YBSpell.Cast("Rupture", ret => SP.Instance.CheckRupture && Me.HasAura(138939) && YBLua.PlayerComboPts == 5),
                YBSpell.Cast("Eviscerate", ret => YBLua.PlayerComboPts > 4 && Me.HasAura("Slice and Dice")),
                YBSpell.Cast("Slice and Dice", ret => !G.SliceAndDiceEnevenom));
        }

        private static WoWUnit TricksTarget
        {
            get
            {
                return G.BestTricksTarget;
            }
        }

        internal static Composite ComMt()
        {
            return new PrioritySelector(
                YBSpell.Cast("Fan of Knives", ret => Me.IsWithinMeleeRange && Me.ComboPoints <= 4),
                YBSpell.Cast("Crimson Tempest", ret => Me.ComboPoints > 4)
                );
        }

        internal static Composite ComDefensive()
        {
            return new PrioritySelector(
                YBSpell.Cast("Recuperate", ret => YBLua.PlayerComboPts > SP.Instance.NumRecuperate && SP.Instance.CheckRecuperate)
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
                YBSpell.Cast("Killing Spree", ret => (Lua.PlayerPower < 35 && YBSpell.HasCachedAura(Me, 5171, 0, 4000) && !Me.HasAura(13750)) && (
                    (SP.Instance.KillingSpree == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SP.Instance.KillingSpree == AbilityTrigger.OnBlTw && (G.SpeedBuffsAura)) ||
                    (SP.Instance.KillingSpree == AbilityTrigger.Always)
                    )),
                YBSpell.Cast("Adrenaline Rush", ret => (Lua.PlayerPower < 35 || Me.HasAura(121471)) && (
                    (SP.Instance.AdrenalineRush == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SP.Instance.AdrenalineRush == AbilityTrigger.OnBlTw && (G.SpeedBuffsAura)) ||
                    (SP.Instance.AdrenalineRush == AbilityTrigger.Always)
                    )),
                YBSpell.Cast("Shadow Blades", ret => YBSpell.HasCachedAura(Me, 5171, 0, 4000) && (
                    (SP.Instance.ShadowBlades == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SP.Instance.ShadowBlades == AbilityTrigger.OnBlTw && (G.SpeedBuffsAura)) ||
                    (SP.Instance.ShadowBlades == AbilityTrigger.Always)
                    )),
                new Decorator(ret => SP.Instance.CheckPotion && G.SpeedBuffsAura, YBItem.UseItem(76089)));
        }

        internal static Composite ComInterrupts()
        {
            return new PrioritySelector(
             );
        }  
        #endregion
    }
}