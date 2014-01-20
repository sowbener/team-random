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

namespace YourBuddy.Rotations.Rogue
{
    class Assassination
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeAss
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),           
                        G.ManualCastPause(),
                    //   new Action(delegate { YBLogger.AdvancedLogP("PoisonNo: = {0}", Poisons.CreateApplyPoisons()); return RunStatus.Failure; }),
                   //    new Decorator(ret => SG.Instance.General.CheckAWaancedLogging, WaLogger.AWaancedLogging),
                       new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Feint", ret => SG.Instance.Assassination.EnableFeintUsage && !Me.HasAura("Feint")))),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => SG.Instance.Assassination.CheckAutoAttack,
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100,
                                    AssaDefensive()),
                                new Decorator(ret => SG.Instance.Assassination.CheckInterrupts,
                                    AssaInterrupts()),
                                AssaUtility(),
                          new Action(ret => { Item.UseAssassinationItems(); return RunStatus.Failure; }),
                          new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                AssaOffensive(),
                                new Decorator(ret => SG.Instance.Assassination.CheckAoE && U.NearbyAttackableUnitsCount >= 2, AssaMt()),
                                    AssaSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => SG.Instance.Assassination.CheckAutoAttack,
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100,
                                    AssaDefensive()),
                                new Decorator(ret => SG.Instance.Assassination.CheckInterrupts,
                                    AssaInterrupts()),
                                AssaUtility(),
                                new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                               new Action(ret => { Item.UseAssassinationItems(); return RunStatus.Failure; }),
                               new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        AssaOffensive())),
                                new Decorator(ret => HotKeyManager.IsAoe, AssaMt()),
                                new Decorator(ret => !HotKeyManager.IsAoe, AssaSt()))));
            }
        }
        #endregion

        #region Rotations
        internal static Composite AssaSt()
        {
            return new PrioritySelector(
                new Decorator(ret => G.FucknoSND,
                    new PrioritySelector(
                        Spell.Cast("Mutilate", ret => Lua.PlayerComboPts < 2),
                        Spell.Cast("Slice and Dice", ret => Lua.PlayerComboPts > 1 && G.FucknoSND)
                        )),
                new Decorator(ret => G.TargetNoRupture && G.IloveyouSND,
                new PrioritySelector(
                    Spell.Cast("Mutilate", ret => Lua.PlayerComboPts < 5),
                    Spell.Cast("Dispatch", ret => Lua.PlayerComboPts < 5 && G.DispatchProc),
                    Spell.Cast("Rupture", ret => Lua.PlayerComboPts > 4)
                    )),
                Spell.Cast("Redirect", ret => Me.RawComboPoints > 0 && Lua.PlayerComboPts < 1),
                Spell.Cast("Expose Armor", ret => G.WeakenedBlowsAura && SG.Instance.Assassination.CheckExposeArmor),
                Spell.Cast("Vanish", ret => (Lua.PlayerPower > 20 && (G.ShadowbladesAura || Me.IsStealthed) && G.IloveyouSND && (
                    (SG.Instance.Assassination.Vanish == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Assassination.Vanish == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.Vanish == Enum.AbilityTrigger.Always)
                    ))),
                Spell.Cast("Preparation", ret => G.Vanishisoncooldown && (
                    (SG.Instance.Assassination.PreperationUsage == Enum.PreperationUsage.VanishCooldown) ||
                    (SG.Instance.Assassination.PreperationUsage == Enum.PreperationUsage.OnBossAndVanishCooldown && U.IsTargetBoss))),
                Spell.Cast("Dispatch", ret => G._anticipationCount < 4 && (G.YourGoingDownBoss && Lua.PlayerComboPts < 5) || (G.IloveyouSND && G.DispatchProc)),
                Spell.Cast("Mutilate", ret => G._anticipationCount < 4 && (Me.IsStealthed || Me.IsBehind(Me.CurrentTarget) || G.MeHasShadowFocus || (G.NoDispatchLove && Lua.PlayerComboPts < 5 && G.ImDpsingYou))),
                Spell.Cast("Rupture", ret => (Lua.PlayerComboPts > 1 && G.TargetRuptureFalling) || (Lua.PlayerComboPts > 4 && G.TargetRuptureFalling5Cps)),
                new Decorator(ret => Lua.PlayerComboPts > 4 && (G.ShadowbladesAuraActive || G.SpeedBuffsAura),
                    new PrioritySelector(
                        Spell.Cast("Envenom")
                        )),
                new Decorator(ret => G._anticipationCount > 4 || (G._anticipationCount < 4 && Lua.PlayerComboPts > 4 && (!Me.HasAura(32645) || G.EnvenomRemains2Seconds)) || (Lua.PlayerComboPts >= 2 && G.MySNDBabyIsFalling),
                        new PrioritySelector(
                        Spell.Cast("Envenom")
                        )));

          
        }

        static Composite AssaMt()
        {
            return new PrioritySelector(
              new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Feint", ret => SG.Instance.Assassination.EnableFeintUsage && !Me.HasAura("Feint")))),
              new Decorator(ret => U.NearbyAttackableUnitsCount > 2,
              new PrioritySelector(
              Spell.Cast("Fan of Knives", ret => Lua.PlayerComboPts < 5),
              Spell.Cast("Rupture", ret => Lua.PlayerComboPts > 1 && G.TargetRuptureFalling),
              Spell.Cast("Envenom", ret => Lua.PlayerComboPts > 4),
              Spell.Cast("Slice and Dice", ret => Lua.PlayerComboPts > 1 && G.FucknoSND)
                )));
        }

        static Composite AssaDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Shadowstep", ret => SG.Instance.Assassination.CheckShadowstep && Me.HealthPercent < SG.Instance.Assassination.NumShadowstep),
                Spell.Cast("Recuperate", ret => SG.Instance.Assassination.CheckRecuperate && Me.HealthPercent < SG.Instance.Assassination.NumRecuperate && Lua.PlayerComboPts >= SG.Instance.Assassination.NumRecuperateCombo)
             //   I.AssaUseHealthStone()
                );
        }

        static Composite AssaOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Shadow Blades", ret => (G.SpeedBuffsAura || G.ShadowBladesSND) && (
                    (SG.Instance.Assassination.ShadowBlades == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Assassination.ShadowBlades == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.ShadowBlades == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Assassination.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Assassination.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Assassination.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Assassination.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Assassination.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Assassination.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Vendetta", ret => (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura) && G.TargetIsClose && (
                    (SG.Instance.Assassination.Vendetta == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Assassination.Vendetta == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.Vendetta == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Marked for Death", ret => G.IloveyouSND && (
                    (SG.Instance.Assassination.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Assassination.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Assassination.Tier6Abilities == Enum.AbilityTrigger.Always)
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
                   new Throttle(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Kick", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId()))))),
                    new Throttle(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Deadly Throw", ret => G.Kick && TalentManager.IsSelected(4) && Lua.PlayerComboPts > 2 && (
                    (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
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
