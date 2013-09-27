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
    class WaSubtlety
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeSub
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => SG.Instance.General.CheckTreePerformance,
                        WaLogger.TreePerformance("InitializeSub")),
                    new Decorator(ret => (WaHotKeyManager.IsPaused || !WaUnit.DefaultCheck), new ActionAlwaysSucceed()),
                        new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        G.InitializeOnKeyActions(),
                        G.InitializeCaching(),
                    //   new Action(delegate { YBLogger.AdvancedLogP("PoisonNo: = {0}", Poisons.CreateApplyPoisons()); return RunStatus.Failure; }),
                       new Decorator(ret => SG.Instance.General.CheckAWaancedLogging, WaLogger.AWaancedLogging),
                       new Decorator(ret => WaHotKeyManager.IsSpecialKey, new PrioritySelector(Spell.Cast("Feint", ret => SG.Instance.Subtlety.EnableFeintUsage && !Me.HasAura("Feint")))),
                        new Decorator(ret => !Spell.GlobalCooldown() && SH.Instance.ModeSelection == WaEnum.Mode.Auto,
                            new PrioritySelector(
                                new Decorator(ret => SG.Instance.Subtlety.CheckAutoAttack,
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100,
                                    SubDefensive()),
                                new Decorator(ret => SG.Instance.Subtlety.CheckInterrupts && WaUnit.CanInterrupt,
                                    SubInterrupts()),
                                SubUtility(),
                                I.SubUseItems(),
                                SubOffensive(),
                                new Decorator(ret => SG.Instance.Subtlety.CheckAoE && U.NearbyAttackableUnitsCount >= 2, SubMt()),
                                    SubSt())),
                        new Decorator(ret => !Spell.GlobalCooldown() && SH.Instance.ModeSelection == WaEnum.Mode.Hotkey,
                            new PrioritySelector(
                                new Decorator(ret => SG.Instance.Subtlety.CheckAutoAttack,
                                    Lua.StartAutoAttack),
                                new Decorator(ret => Me.HealthPercent < 100,
                                    SubDefensive()),
                                new Decorator(ret => SG.Instance.Subtlety.CheckInterrupts && WaUnit.CanInterrupt,
                                    SubInterrupts()),
                                SubUtility(),
                                new Decorator(ret => WaHotKeyManager.IsCooldown,
                                    new PrioritySelector(
                                        I.SubUseItems(),
                                        SubOffensive())),
                                new Decorator(ret => WaHotKeyManager.IsAoe, SubMt()),
                                new Decorator(ret => !WaHotKeyManager.IsAoe, SubSt()))));
            }
        }
        #endregion

        #region Rotations
        static Composite SubSt()
        {
            return new PrioritySelector(
                Spell.Cast("Premeditation", ret => Lua.PlayerPower < 90 && (Lua.PlayerComboPts < 3 || AntiCipation3Stacks)),
                Spell.Cast("Ambush", ret => Lua.PlayerPower < 90 && (Lua.PlayerComboPts < 5 || AntiCipation3Stacks)),
                Spell.Cast("Shadow Dance", ret => !Me.HasAura("Stealth") && !Me.HasAura("Vanish") && !Me.CurrentTarget.HasMyAura("Find Weakness")),
                //actions+=/vanish,if=energy>=45&energy<=75&combo_points<=3&buff.shadow_dance.down&buff.master_of_subtlety.down&debuff.find_weakness.down
                Spell.Cast("Vanish", ret => Lua.PlayerPower >= 45 && Lua.PlayerPower <= 75 && Lua.PlayerComboPts <= 3 && !Me.HasAura("Shadow Dance") && !Me.HasAura("Master of Subtlety") && !Me.CurrentTarget.HasMyAura("Find Weakness")),
                //actions+=/run_action_list,name=generator,if=talent.anticipation.enabled&anticipation_charges<4&buff.slice_and_dice.up&dot.rupture.remains>2&(buff.slice_and_dice.remains<6|dot.rupture.remains<4)
                new Decorator(ret => AntiCipation4Stacks && (G.SliceAndDiceSubGenerator || G.TargetHaveRupture4), ComboBuilders()),
                new Decorator(ret => Lua.PlayerComboPts == 5, Finishers()),
                new Decorator(ret => Lua.PlayerComboPts < 4 || Lua.PlayerPower > 80 || WaTalentManager.HasTalent(18), ComboBuilders())
                        );

          
        }



        static Composite ComboBuilders()
        {
            return new PrioritySelector(
            new Decorator(ret => !Me.HasAura("Master of Subtlety") && !Me.HasAura("Shadow Dance") && !Me.CurrentTarget.HasMyAura("Find Weakness") && (Lua.PlayerPower < 80 || Lua.PlayerPower < 60), Pooling()),
            Spell.Cast("Hemorrhage", ret => G.HemorrhageDebuffFalling || !StyxWoW.Me.CurrentTarget.MeIsBehind),
                //  Spell.Cast("Shuriken Toss", ret => ShurikenTossEnabled && Lua.PlayerPower < 65),
            Spell.Cast("Backstab", ret => StyxWoW.Me.CurrentTarget.MeIsBehind && !Me.HasAura("Shadow Dance")),
            Pooling());
        }
        
        static Composite Finishers()
        {
            return new PrioritySelector(
            Spell.Cast("Slice and Dice", ret => G.SliceandDiceSub || !Me.HasAura("Slice and Dice")),
            Spell.Cast("Rupture", ret => G.TargetRuptureFalling || G.TargetNoRupture),
            Spell.Cast("Eviscerate"),
            Pooling());
        }

        static Composite Pooling()
        {
            return new PrioritySelector(
                Spell.Cast("Preparation", ret => !Me.HasAura("Vanish") && Spell.GetSpellCooldown("Vanish").Seconds > 60));
        }


        static Composite SubMt()
        {
            return new PrioritySelector(
              new Decorator(ret => WaHotKeyManager.IsSpecialKey, new PrioritySelector(Spell.Cast("Feint", ret => SG.Instance.Subtlety.EnableFeintUsage && !Me.HasAura("Feint")))),
              new Decorator(ret => U.NearbyAttackableUnitsCount > 2,
              new PrioritySelector(
              Spell.Cast("Fan of Knives", ret => Lua.PlayerComboPts < 5),
              Spell.Cast("Rupture", ret => Lua.PlayerComboPts > 1 && G.TargetRuptureFalling),
              Spell.Cast("Envenom", ret => Lua.PlayerComboPts > 4),
              Spell.Cast("Slice and Dice", ret => Me.ComboPoints > 1 && G.FucknoSND)
                )));
        }

        static Composite SubDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Shadowstep", ret => SG.Instance.Subtlety.CheckShadowstep && Me.HealthPercent < SG.Instance.Subtlety.NumShadowstep),
                Spell.Cast("Recuperate", ret => SG.Instance.Subtlety.CheckRecuperate && Me.HealthPercent < SG.Instance.Subtlety.NumRecuperate && WaLua.PlayerComboPts >= SG.Instance.Subtlety.NumRecuperateCombo),
                I.SubUseHealthStone()
                );
        }

        static Composite SubOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Shadow Blades", ret => (G.SpeedBuffsAura || G.ShadowBladesSND) && (
                    (SG.Instance.Subtlety.ShadowBlades == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Subtlety.ShadowBlades == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.ShadowBlades == WaEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Subtlety.ClassRacials == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Subtlety.ClassRacials == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.ClassRacials == WaEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Subtlety.ClassRacials == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Subtlety.ClassRacials == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.ClassRacials == WaEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (G.IloveyouSND || G.ShadowbladesAura || G.SpeedBuffsAura && G.TargetIsClose) && (
                    (SG.Instance.Subtlety.ClassRacials == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Subtlety.ClassRacials == WaEnum.AbilityTrigger.OnBlTwHr && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.ClassRacials == WaEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Marked for Death", ret => G.IloveyouSND && (
                    (SG.Instance.Subtlety.Tier6Abilities == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Subtlety.Tier6Abilities == WaEnum.AbilityTrigger.OnBossDummy && (G.SpeedBuffsAura)) ||
                    (SG.Instance.Subtlety.Tier6Abilities == WaEnum.AbilityTrigger.Always)
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
                new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(500), RunStatus.Failure,
                    Spell.Cast("Kick")
                    ),
                    Spell.Cast("Deadly Throw", ret => G.Kick && TalentManager.HasTalent(4) && Lua.PlayerComboPts > 2
                    ));
        }
        #endregion

        #region BoolsChampion

        private static bool AntiCipation3Stacks { get { return AnticipationStacks < 3 && Me.HasAura(115189); } }
        private static bool AntiCipation4Stacks { get { return AnticipationStacks < 4 && Me.HasAura(115189); } }

        private static uint AnticipationStacks
        {
            get
            {
                return Spell.GetAuraStackCount(115189);
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
