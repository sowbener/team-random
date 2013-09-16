using CommonBehaviors.Actions;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using YBMoP_BT_Rogue.Core;
using YBMoP_BT_Rogue.Helpers;
using YBMoP_BT_Rogue.Managers;
using G = YBMoP_BT_Rogue.Routines.YBGlobal;
using I = YBMoP_BT_Rogue.Core.YBItem;
using Lua = YBMoP_BT_Rogue.Helpers.YBLua;
using SF = YBMoP_BT_Rogue.Interfaces.Settings.YBSettingsAss;
using SG = YBMoP_BT_Rogue.Interfaces.Settings.YBSettingsG;

namespace YBMoP_BT_Rogue.Routines
{
    class YBAss
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeAss
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => SG.Instance.CheckTreePerformance,
                        YBLogger.TreePerformance("InitializeAss")),
                    new Decorator(ret => (HotKeyManager.IsPaused || !YBUnit.DefaultCheck), new ActionAlwaysSucceed()),
                        new Action(delegate { YBSpell.GetCachedAuras(); return RunStatus.Failure; }),
                        new Action(delegate { YBUnit.GetAttackableMeleeUnitsCount(); return RunStatus.Failure; }),
                     //   new Action(delegate { YBLogger.AdvancedLogP("PoisonNo: = {0}", Poisons.CreateApplyPoisons()); return RunStatus.Failure; }),
                        new Decorator(ret => SG.Instance.CheckAdvancedLogging,
                            YBLogger.AdvancedLogging),
                        new Decorator(ret => SG.Instance.ModeChoice == Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => SF.Instance.CheckAutoAttack, 
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100, 
                                    AssaDefensive()),
                                new Decorator(ret => SF.Instance.CheckInterrupts && YBUnit.CanInterrupt,
                                    AssaInterrupts()),
                                AssaUtility(),
                                I.AssaUseItems(),
                                AssaOffensive(),
                                new Decorator(ret => SF.Instance.CheckAoE && YBUnit.AttackableMeleeUnitsCount >= SF.Instance.NumAoE, AssaMt()),
                                    AssaSt())),
                        new Decorator(ret => SG.Instance.ModeChoice == Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => SF.Instance.CheckAutoAttack, 
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100, 
                                    AssaDefensive()),
                                new Decorator(ret => SF.Instance.CheckInterrupts && YBUnit.CanInterrupt,
                                    AssaInterrupts()),
                                AssaUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.AssaUseItems(),
                                        AssaOffensive())),
                                new Decorator(ret => HotKeyManager.IsAoe, AssaMt()),
                                new Decorator(ret => !HotKeyManager.IsAoe, AssaSt()))));
            }
        }
        #endregion

        #region Rotations
       static Composite AssaSt()
        {
            return new PrioritySelector(
                new Decorator(ret => HotKeyManager.IsSpecialKey, new PrioritySelector(YBSpell.Cast("Tricks of the Trade", u => TricksTarget, ret => TricksTarget != null))),
                new Decorator(ret => G.FucknoSND,
                    new PrioritySelector(
                        YBSpell.Cast("Mutilate", ret => Lua.PlayerComboPts < 2),
                        YBSpell.Cast("Slice and Dice", ret => Lua.PlayerComboPts > 1 && G.FucknoSND)
                        )),
                new Decorator(ret => G.TargetNoRupture && G.IloveyouSND,
                new PrioritySelector(
                    YBSpell.Cast("Mutilate", ret => Lua.PlayerComboPts < 5),
                    YBSpell.Cast("Dispatch", ret => Lua.PlayerComboPts < 5 && G.DispatchProc),
                    YBSpell.Cast("Rupture", ret => Lua.PlayerComboPts > 4)
                    )),
                YBSpell.Cast("Redirect", ret => Me.RawComboPoints > 0 && Lua.PlayerComboPts < 1),
                YBSpell.Cast("Expose Armor", ret => G.WeakenedBlowsAura && SF.Instance.CheckExposeArmor),
                YBSpell.Cast("Vanish", ret => (Lua.PlayerPower > 20 && (G.ShadowbladesAura || Me.IsStealthed) && G.IloveyouSND && (
                    (SF.Instance.Vanish == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.Vanish == AbilityTrigger.OnBlTw && (G.SpeedBuffsAura)) ||
                    (SF.Instance.Vanish == AbilityTrigger.Always)
                    ))),
                YBSpell.Cast("Preparation",  ret => G.Vanishisoncooldown && SF.Instance.CheckPreparation),
                YBSpell.Cast("Dispatch", ret => !G.Anticipate4stacks && (G.YourGoingDownBoss && Lua.PlayerComboPts < 5) || (G.IloveyouSND && G.DispatchProc)),
                YBSpell.Cast("Mutilate", ret => !G.Anticipate4stacks && (Me.IsStealthed || Me.IsBehind(Me.CurrentTarget) || G.MeHasShadowFocus || (G.NoDispatchLove && Lua.PlayerComboPts < 5 && G.ImDpsingYou))),
                YBSpell.Cast("Rupture", ret => Lua.PlayerComboPts > 1 && G.TargetRuptureFalling),
                new Decorator(ret => Lua.PlayerComboPts > 4 && (G.ShadowbladesAuraActive || G.SpeedBuffsAura),
                    new PrioritySelector(
                        YBSpell.Cast("Envenom")
                        )),
                new Decorator(ret => G.Anticipate4stacks || (!G.Anticipate4stacks && Lua.PlayerComboPts > 4 && (!Me.HasAura(32645) || !G.EnvenomRemains2Seconds)) || (Lua.PlayerComboPts >= 2 && G.MySNDBabyIsFalling),
                        new PrioritySelector(
                        YBSpell.Cast("Envenom")
                        )));
        }

        static Composite AssaMt()
        {
            return new PrioritySelector(
              YBSpell.Cast("Fan of Knives", ret => Lua.PlayerComboPts < 5), 
              YBSpell.Cast("Envenom", ret => (Lua.PlayerComboPts == 3 && G.TargetNoRupture) || (Lua.PlayerComboPts > 4 && G.TargetHaveRupture)),
              YBSpell.Cast("Rupture", ret => Lua.PlayerComboPts > 4 && G.TargetRuptureFalling)
                );
        }

        static Composite AssaDefensive()
        {
            return new PrioritySelector(
                YBSpell.Cast("Shadowstep", ret => SF.Instance.CheckShadowstep && Me.HealthPercent < SF.Instance.NumShadowstep),
                YBSpell.Cast("Recuperate", ret => SF.Instance.CheckRecuperate && Me.HealthPercent < SF.Instance.NumRecuperate && YBLua.PlayerComboPts == SF.Instance.NumRecuperateCombo),
                I.AssaUseHealthStone()
                );
        }

        static Composite AssaOffensive()
        {
            return new PrioritySelector(
                YBSpell.Cast("Shadow Blades", ret => (G.SpeedBuffsAura || G.ShadowBladesSND) && (
                    (SF.Instance.ShadowBlades == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.ShadowBlades == AbilityTrigger.OnBlTw && (G.SpeedBuffsAura)) ||
                    (SF.Instance.ShadowBlades == AbilityTrigger.Always)
                    )),
                YBSpell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SF.Instance.ClassRacials == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.ClassRacials == AbilityTrigger.OnBlTw && (G.SpeedBuffsAura)) ||
                    (SF.Instance.ClassRacials == AbilityTrigger.Always)
                    )),
                YBSpell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SF.Instance.ClassRacials == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.ClassRacials == AbilityTrigger.OnBlTw && (G.SpeedBuffsAura)) ||
                    (SF.Instance.ClassRacials == AbilityTrigger.Always)
                    )),
                YBSpell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SF.Instance.ClassRacials == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.ClassRacials == AbilityTrigger.OnBlTw && (G.SpeedBuffsAura)) ||
                    (SF.Instance.ClassRacials == AbilityTrigger.Always)
                    )),
                    YBSpell.Cast("Vendetta", ret => (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura) && G.TargetIsClose && (
                    (SF.Instance.Vendetta == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.Vendetta == AbilityTrigger.OnBlTw && (G.SpeedBuffsAura)) ||
                    (SF.Instance.Vendetta == AbilityTrigger.Always)
                    )),
                    YBSpell.Cast("Marked for Death", ret => G.IloveyouSND && (
                    (SF.Instance.Tier6Abilities == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.Tier6Abilities == AbilityTrigger.OnBlTw && (G.SpeedBuffsAura)) ||
                    (SF.Instance.Tier6Abilities == AbilityTrigger.Always)
                    )),
                    new Decorator(ret => SF.Instance.CheckPotion && G.SpeedBuffsAura, YBItem.UseItem(76089)));
        }

        internal static Composite AssaUtility()
        {   
            return new PrioritySelector(
              
                );
        }

        internal static Composite AssaInterrupts()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(500), RunStatus.Failure,
                    YBSpell.Cast("Kick")
                    ),
                    YBSpell.Cast("Deadly Throw", ret => G.Kick && TalentManager.HasTalent(4) && YBLua.PlayerComboPts > 2
                    ));
        } 
        #endregion

        #region Tricks of the trade
        private static WoWUnit TricksTarget
        {
            get
            {
                return G.BestTricksTarget;
            }
        }
        #endregion
    }
}
