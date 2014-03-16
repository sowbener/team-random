using CommonBehaviors.Actions;
using YourRaidingBuddy.Core;
using YourRaidingBuddy.Core.Helpers;
using YourRaidingBuddy.Core.Managers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = YourRaidingBuddy.Rotations.Global;
using I = YourRaidingBuddy.Core.Item;
using Lua = YourRaidingBuddy.Core.Helpers.LuaClass;
using T = YourRaidingBuddy.Core.Managers.TalentManager;
using SG = YourRaidingBuddy.Interfaces.Settings.InternalSettings;
using SH = YourRaidingBuddy.Interfaces.Settings.SettingsH;
using U = YourRaidingBuddy.Core.Unit;
using Styx.CommonBot;
using Styx.WoWInternals;

namespace YourRaidingBuddy.Rotations.Rogue
{
    class Subtlety
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeSub
        {
            get
            {
                return new PrioritySelector(
                 //   new Decorator(ret => SG.Instance.General.CheckTreePerformance,
                //        WaLogger.TreePerformance("InitializeSub")),
                    new Decorator(ret => (HotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        G.InitializeOnKeyActionsR(),
                        G.ManualCastPause(),
                        //new Action(delegate { WaLogger.DumpAuraTables(StyxWoW.Me); return RunStatus.Failure; }),    
                    //   new Action(delegate { YBLogger.AdvancedLogP("PoisonNo: = {0}", Poisons.CreateApplyPoisons()); return RunStatus.Failure; }),
                     Spell.Cast("Expose Armor", ret => G.WeakenedBlowsAura && SG.Instance.Subtlety.CheckExposeArmor),
                       new Decorator(ret => HotKeyManager.IsSpecial, new PrioritySelector(Spell.Cast("Feint", ret => SG.Instance.Subtlety.EnableFeintUsage && !Me.HasAura("Feint")))),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => SG.Instance.Subtlety.CheckAutoAttack,
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100,
                                    SubDefensive()),
                                new Decorator(ret => SG.Instance.Subtlety.CheckInterrupts,
                                    SubInterrupts()),
                                SubUtility(),
                            new Styx.TreeSharp.Action(ret => { Item.UseSubtletyItems(); return RunStatus.Failure; }),
                            new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                SubOffensive(),
                                new Decorator(ret => SG.Instance.Subtlety.CheckAoE && U.NearbyAttackableUnitsCount > 4, SubMt()),
                                SubShadowDance(),
                                new Decorator(ret => Lua.PlayerPower < 75 && G.ShadowDanceOnline && G.SNDSetting > 2 && (G.FindWeaknessOff || G.FindWeakness < 3), new ActionAlwaysSucceed()),
                                SubSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.SemiHotkey,
                            new PrioritySelector(
                                new Decorator(ret => SG.Instance.Subtlety.CheckAutoAttack,
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100,
                                    SubDefensive()),
                                new Decorator(ret => SG.Instance.Subtlety.CheckInterrupts,
                                    SubInterrupts()),
                                SubUtility(),
                           new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        SubShadowDance(),
                                new Styx.TreeSharp.Action(ret => { Item.UseSubtletyItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                         Spell.Cast("Vanish", ret => G.ShadowDanceOffline && G.PremeditationOnline && Lua.PlayerComboPts <= 3 && (G.FindWeaknessOff || G.FindWeakness < 3) && (!Me.HasAura(115191) || !Me.HasAura(115193)) && !Me.HasAura(51713) && Me.IsFacing(Me.CurrentTarget) && (
                                         (SG.Instance.Subtlety.Vanish == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                                         (SG.Instance.Subtlety.Vanish == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                                         (SG.Instance.Subtlety.Vanish == Enum.AbilityTrigger.Always)
                                          )),
                                        SubOffensive())),
                                    SubSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => SG.Instance.Subtlety.CheckAutoAttack,
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100,
                                    SubDefensive()),
                                new Decorator(ret => SG.Instance.Subtlety.CheckInterrupts,
                                    SubInterrupts()),
                                SubUtility(),
                                 new Decorator(ret => HotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        SubShadowDance(),
                                        new Decorator(ret => Lua.PlayerPower < 75 && G.ShadowDanceOnline && (!Me.HasAura(115191) || !Me.HasAura(115193)) && (G.FindWeaknessOff || G.FindWeakness < 3), new ActionAlwaysSucceed()),
                                new Styx.TreeSharp.Action(ret => { Item.UseSubtletyItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                         Spell.Cast("Vanish", ret => G.ShadowDanceOffline && G.PremeditationOnline && Lua.PlayerComboPts <= 3 && (G.FindWeaknessOff || G.FindWeakness < 3) && (!Me.HasAura(115191) || !Me.HasAura(115193)) && !Me.HasAura(51713) && Me.IsFacing(Me.CurrentTarget) && (
                                         (SG.Instance.Subtlety.Vanish == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                                         (SG.Instance.Subtlety.Vanish == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                                         (SG.Instance.Subtlety.Vanish == Enum.AbilityTrigger.Always)
                                          )),
                                        SubOffensive())),

                                new Decorator(ret => HotKeyManager.IsAoe, SubMt()),
                                new Decorator(ret => !HotKeyManager.IsAoe, SubSt()))));
            }
        }
        #endregion

        #region Rotations
        static Composite SubSt()
        {
            return new PrioritySelector(
           //     new Throttle(3, Spell.Cast("Redirect", ret => Me.RawComboPoints > 0 && Me.CurrentTarget != null && Lua.PlayerComboPts < 1 && SG.Instance.General.EnableRedirectRogue)),
                Spell.Cast("Premeditation", ret => Me.CurrentEnergy < 90 && Lua.PlayerComboPts < 3),
                Spell.Cast("Ambush", ret => StyxWoW.Me.CurrentTarget.MeIsBehind && (Lua.PlayerComboPts < 5 || G._anticipationCount < 3)),
                Spell.Cast("Hemorrhage", ret => G.HemorrhageDebuffFalling),
                Spell.Cast("Vanish", ret => SH.Instance.ModeSelection == Enum.Mode.Auto && Lua.PlayerPower <= 75 && Lua.PlayerComboPts <= 3 && !Me.HasAura(51713) && !Me.HasAura("Master of Subtlety") && (G.FindWeaknessOff || G.FindWeakness < 3)),
                new Decorator(ret => Lua.PlayerComboPts > 4, Finishers()),
                new Decorator(ret => !Styx.WoWInternals.WoWSpell.FromId(8676).CanCast && !Me.HasAura(115192) && !Me.HasAura(51713) && (!Me.HasAura(11327) || !Me.HasAura(1784)) && Lua.PlayerComboPts < 5, ComboBuilders())
                        );

          
        }



        static Composite ComboBuilders()
        {
            return new PrioritySelector(
            new Decorator(ret => !Me.HasAura("Master of Subtlety") && !Me.HasAura("Shadow Dance") && (G.FindWeaknessOff || G.FindWeakness < 3) && (Lua.PlayerPower < 80 || Lua.PlayerPower < 60), Pooling()),
            Spell.Cast("Hemorrhage", ret => !StyxWoW.Me.CurrentTarget.MeIsBehind || (Lua.PlayerComboPts == 4 && Lua.PlayerPower > 79) || (!Me.HasAura(115191) && !Me.HasAura(31665) && !Me.HasAura(51713) && !Me.CurrentTarget.HasMyAura("Hemorrhage"))),
                //  Spell.Cast("Shuriken Toss", ret => ShurikenTossEnabled && Lua.PlayerPower < 65),
            Spell.Cast("Backstab", ret => StyxWoW.Me.CurrentTarget.MeIsBehind && !Me.HasAura(115192) && (!Me.HasAura(11327) || !Me.HasAura(1784))),
            Pooling());
        }
        
        static Composite Finishers()
        {
            return new PrioritySelector(
            Spell.Cast("Slice and Dice", ret => G.SliceandDiceSub || !Me.HasAura("Slice and Dice")),
            Spell.Cast("Rupture", ret => Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura(703) && (G.TargetRuptureFalling || G.TargetNoRupture)),
            Spell.Cast("Eviscerate", ret => G.TargetHaveRupture && Me.HasAura("Slice and Dice")));
        }

        static Composite Pooling()
        {
            return new PrioritySelector(
                Spell.Cast("Preparation", ret => !Me.HasAura("Vanish") && G.VanishIsOnCooldown));
        }


        static Composite SubMt()
        {
            return new PrioritySelector(
              Spell.Cast("Redirect", ret => Me.RawComboPoints > 0 && Lua.PlayerComboPts < 1),
         //     new Decorator(ret => HotKeyManager.IsSpecialKey, new PrioritySelector(Spell.Cast("Feint", ret => SG.Instance.Subtlety.EnableFeintUsage && !Me.HasAura("Feint")))),
              new Decorator(ret => U.NearbyAttackableUnitsCount < 4, SubSt()),
              new Decorator(ret => U.NearbyAttackableUnitsCount >= 7, new PrioritySelector(
                Spell.Cast("Fan of Knives", ret => Lua.PlayerComboPts < 4),
                 Spell.Cast("Crimson Tempest", ret => (G.CrimsonTempestNotUp && Lua.PlayerComboPts < 1) || (Lua.PlayerComboPts > 4 && !G.FucknoSND && G.CrimsonTempestNotUp)),
                 Spell.Cast("Slice and Dice", ret => Lua.PlayerComboPts > 4 && G.FucknoSND),
              new Decorator(ret => Lua.PlayerComboPts < 4 && (U.NearbyAttackableUnitsCount >= 4 && U.NearbyAttackableUnitsCount <= 6), new PrioritySelector(
              new Decorator(ret => !Styx.WoWInternals.WoWSpell.FromId(8676).CanCast, ComboBuilders()),
              Spell.Cast("Ambush", ret => StyxWoW.Me.CurrentTarget.MeIsBehind && (Lua.PlayerComboPts < 5 || G._anticipationCount < 3)),
              Spell.Cast("Hemorrhage", ret => !StyxWoW.Me.CurrentTarget.MeIsBehind || (Lua.PlayerComboPts == 4 && Lua.PlayerPower > 79) || (!Me.HasAura(115191) && !Me.HasAura(31665) && !Me.HasAura(51713) && !Me.CurrentTarget.HasMyAura("Hemorrhage"))),
              Spell.Cast("Crimson Tempest", ret => (G.CrimsonTempestNotUp && Lua.PlayerComboPts < 1) || (Lua.PlayerComboPts > 4 && !G.FucknoSND && G.CrimsonTempestNotUp)),
              Spell.Cast("Eviscerate", ret => Lua.PlayerComboPts > 4 && !G.FucknoSND && !G.CrimsonTempestNotUp),
              Spell.Cast("Slice and Dice", ret => Lua.PlayerComboPts > 4 && G.FucknoSND)))
                )));
        }

        static Composite SubDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Shadowstep", ret => SG.Instance.Subtlety.CheckShadowstep && Me.HealthPercent < SG.Instance.Subtlety.NumShadowstep),
                Spell.Cast("Recuperate", ret => SG.Instance.Subtlety.CheckRecuperate && Me.HealthPercent < SG.Instance.Subtlety.NumRecuperate && Lua.PlayerComboPts >= SG.Instance.Subtlety.NumRecuperateCombo),
                Item.SubtletyUseHealthStone());
        }


        static Composite SubShadowDance()
        {
            return new PrioritySelector(
                   Spell.Cast("Shadow Dance", ret => Me.CurrentTarget != null && Lua.PlayerPower > 74 && !Me.HasAura("Stealth") && G.SNDSetting > 2 && !Me.HasAura("Vanish") && (G.FindWeaknessOff || G.FindWeakness < 3) && (
                    (SG.Instance.Subtlety.ShadowDance == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Subtlety.ShadowDance == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.ShadowDance == Enum.AbilityTrigger.Always)
                    )));
        }

        static Composite SubOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Shadow Blades", ret => (G.FindWeaknessOff || G.FindWeakness < 3) || (G.SpeedBuffsAura || Me.HasAura(105697)) && (
                    (SG.Instance.Subtlety.ShadowBlades == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Subtlety.ShadowBlades == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.ShadowBlades == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Subtlety.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Subtlety.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Subtlety.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Subtlety.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Subtlety.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Subtlety.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Marked for Death", ret => G.IloveyouSND && (
                    (SG.Instance.Subtlety.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Subtlety.Tier6Abilities == Enum.AbilityTrigger.OnBossDummy && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.Tier6Abilities == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite SubUtility()
        {
            return new PrioritySelector(

                );
        }

        internal static Composite SubInterrupts()
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

        #region BoolsChampion


        private static uint AnticipationStacks
        {
            get
            {
                return Spell.GetAuraStack(Me, 115189);
            }
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
