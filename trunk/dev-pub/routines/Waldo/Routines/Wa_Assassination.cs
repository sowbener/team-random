using CommonBehaviors.Actions;
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
using Styx.CommonBot;
using Styx.WoWInternals;
using TalentManager = Waldo.Managers.WaTalentManager;

namespace Waldo.Routines
{
    class WaAssassination
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeAss
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => SG.Instance.General.CheckTreePerformance,
                        WaLogger.TreePerformance("InitializeAss")),
                    new Decorator(ret => (WaHotKeyManager.IsPaused || !WaUnit.DefaultCheck), new ActionAlwaysSucceed()),
                        new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        G.InitializeOnKeyActions(),
                        G.InitializeCaching(),
                    //   new Action(delegate { YBLogger.AdvancedLogP("PoisonNo: = {0}", Poisons.CreateApplyPoisons()); return RunStatus.Failure; }),
                       new Decorator(ret => SG.Instance.General.CheckAWaancedLogging, WaLogger.AWaancedLogging),
                       new Decorator(ret => WaHotKeyManager.IsSpecialKey, new PrioritySelector(Spell.Cast("Feint", ret => SG.Instance.Assassination.EnableFeintUsage && !Me.HasAura("Feint")))),
                        new Decorator(ret => !Spell.GlobalCooldown() && SH.Instance.ModeSelection == WaEnum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => SG.Instance.Assassination.CheckAutoAttack,
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100,
                                    AssaDefensive()),
                                new Decorator(ret => SG.Instance.Assassination.CheckInterrupts && WaUnit.CanInterrupt,
                                    AssaInterrupts()),
                                AssaUtility(),
                                I.AssaUseItems(),
                                AssaOffensive(),
                                new Decorator(ret => SG.Instance.Assassination.CheckAoE && U.NearbyAttackableUnitsCount >= 2, AssaMt()),
                                    AssaSt())),
                        new Decorator(ret => !Spell.GlobalCooldown() && SH.Instance.ModeSelection == WaEnum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => SG.Instance.Assassination.CheckAutoAttack,
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100,
                                    AssaDefensive()),
                                new Decorator(ret => SG.Instance.Assassination.CheckInterrupts && WaUnit.CanInterrupt,
                                    AssaInterrupts()),
                                AssaUtility(),
                                new Decorator(ret => WaHotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.AssaUseItems(),
                                        AssaOffensive())),
                                new Decorator(ret => WaHotKeyManager.IsAoe, AssaMt()),
                                new Decorator(ret => !WaHotKeyManager.IsAoe, AssaSt()))));
            }
        }
        #endregion

        #region Rotations
        static Composite AssaSt()
        {
            return new PrioritySelector(
                new Decorator(ret => G.FucknoSND,
                    new PrioritySelector(
                        Spell.Cast("Mutilate", ret => Me.ComboPoints < 2),
                        Spell.Cast("Slice and Dice", ret => Me.ComboPoints > 1 && G.FucknoSND)
                        )),
                new Decorator(ret => G.TargetNoRupture && G.IloveyouSND,
                new PrioritySelector(
                    Spell.Cast("Mutilate", ret => Me.ComboPoints < 5),
                    Spell.Cast("Dispatch", ret => Me.ComboPoints < 5 && G.DispatchProc),
                    Spell.Cast("Rupture", ret => Me.ComboPoints > 4)
                    )),
                Spell.Cast("Redirect", ret => Me.RawComboPoints > 0 && Me.ComboPoints < 1),
                Spell.Cast("Expose Armor", ret => G.WeakenedBlowsAura && SG.Instance.Assassination.CheckExposeArmor),
                Spell.Cast("Vanish", ret => (Lua.PlayerPower > 20 && (G.ShadowbladesAura || Me.IsStealthed) && G.IloveyouSND && (
                    (SG.Instance.Assassination.Vanish == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Assassination.Vanish == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.Vanish == WaEnum.AbilityTrigger.Always)
                    ))),
                Spell.Cast("Preparation", ret => G.Vanishisoncooldown && (
                    (SG.Instance.Assassination.PreperationUsage == WaEnum.PreperationUsage.VanishCooldown) ||
                    (SG.Instance.Assassination.PreperationUsage == WaEnum.PreperationUsage.OnBossAndVanishCooldown && WaUnit.IsTargetBoss))),
                Spell.Cast("Dispatch", ret => !G.Anticipate4stacks && (G.YourGoingDownBoss && Lua.PlayerComboPts < 5) || (G.IloveyouSND && G.DispatchProc)),
                Spell.Cast("Mutilate", ret => !G.Anticipate4stacks && (Me.IsStealthed || Me.IsBehind(Me.CurrentTarget) || G.MeHasShadowFocus || (G.NoDispatchLove && Lua.PlayerComboPts < 5 && G.ImDpsingYou))),
                Spell.Cast("Rupture", ret => (Lua.PlayerComboPts > 1 && G.TargetRuptureFalling) || (Lua.PlayerComboPts > 4 && G.TargetRuptureFalling5Cps)),
                new Decorator(ret => Lua.PlayerComboPts > 4 && (G.ShadowbladesAuraActive || G.SpeedBuffsAura),
                    new PrioritySelector(
                        Spell.Cast("Envenom")
                        )),
                new Decorator(ret => G.Anticipate4stacks || (!G.Anticipate4stacks && Lua.PlayerComboPts > 4 && (!Me.HasAura(32645) || G.EnvenomRemains2Seconds)) || (Lua.PlayerComboPts >= 2 && G.MySNDBabyIsFalling),
                        new PrioritySelector(
                        Spell.Cast("Envenom")
                        )));

          
        }

        static Composite AssaMt()
        {
            return new PrioritySelector(
              new Decorator(ret => WaHotKeyManager.IsSpecialKey, new PrioritySelector(Spell.Cast("Feint", ret => SG.Instance.Assassination.EnableFeintUsage && !Me.HasAura("Feint")))),
              new Decorator(ret => U.NearbyAttackableUnitsCount > 2,
              new PrioritySelector(
              Spell.Cast("Fan of Knives", ret => Lua.PlayerComboPts < 5),
              Spell.Cast("Rupture", ret => Lua.PlayerComboPts > 1 && G.TargetRuptureFalling),
              Spell.Cast("Envenom", ret => Lua.PlayerComboPts > 4),
              Spell.Cast("Slice and Dice", ret => Me.ComboPoints > 1 && G.FucknoSND)
                )));
        }

        static Composite AssaDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Shadowstep", ret => SG.Instance.Assassination.CheckShadowstep && Me.HealthPercent < SG.Instance.Assassination.NumShadowstep),
                Spell.Cast("Recuperate", ret => SG.Instance.Assassination.CheckRecuperate && Me.HealthPercent < SG.Instance.Assassination.NumRecuperate && WaLua.PlayerComboPts >= SG.Instance.Assassination.NumRecuperateCombo),
                I.AssaUseHealthStone()
                );
        }

        static Composite AssaOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Shadow Blades", ret => (G.SpeedBuffsAura || G.ShadowBladesSND) && (
                    (SG.Instance.Assassination.ShadowBlades == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Assassination.ShadowBlades == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.ShadowBlades == WaEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Assassination.ClassRacials == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Assassination.ClassRacials == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.ClassRacials == WaEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Assassination.ClassRacials == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Assassination.ClassRacials == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.ClassRacials == WaEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Assassination.ClassRacials == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Assassination.ClassRacials == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.ClassRacials == WaEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Vendetta", ret => (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura) && G.TargetIsClose && (
                    (SG.Instance.Assassination.Vendetta == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Assassination.Vendetta == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.Vendetta == WaEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Marked for Death", ret => G.IloveyouSND && (
                    (SG.Instance.Assassination.Tier6Abilities == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Assassination.Tier6Abilities == WaEnum.AbilityTrigger.OnBossDummy && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.Tier6Abilities == WaEnum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite AssaUtility()
        {
            return new PrioritySelector(

                );
        }

        internal static Composite AssaInterrupts()
        {
            return new PrioritySelector(
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(1000), RunStatus.Failure,
                    Spell.Cast("Kick")
                    ),
              new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(1000), RunStatus.Failure,
                    Spell.Cast("Deadly Throw", ret => G.Kick && TalentManager.HasTalent(4) && Lua.PlayerComboPts > 2)));
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
