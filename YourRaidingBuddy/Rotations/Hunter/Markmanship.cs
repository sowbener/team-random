using CommonBehaviors.Actions;
using YourBuddy.Core;
using System.Linq;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using SG = YourBuddy.Interfaces.Settings.InternalSettings;
using System.Windows.Forms;
using Styx.CommonBot;
using Logger = YourBuddy.Core.Utilities.Logger;
using LoggerP = YourBuddy.Core.Utilities.PerformanceLogger;
using YourBuddy.Core.Managers;
using G = YourBuddy.Rotations.Global;
using SH = YourBuddy.Interfaces.Settings.SettingsH;
using YourBuddy.Core.Helpers;
using Lua = YourBuddy.Core.Helpers.LuaClass;

namespace YourBuddy.Rotations.Hunter
{
    class Marksmanship
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeMarksmanship
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheckRanged), new ActionAlwaysSucceed()),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        G.InitializeOnKeyActionsH(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Marksmanship.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, MarksmanshipDefensive()),
                                        new Decorator(ret => SG.Instance.Marksmanship.CheckInterrupts && Unit.CanInterrupt, MarksmanshipInterrupts()),
                                        MarksmanshipUtility(),
                                        new Action(ret => { Item.UseMarksmanshipItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        MarksmanshipOffensive(),
                                        new Decorator(ret => SG.Instance.Marksmanship.CheckAoE && (Unit.NearbyAttackableUnitsCount >= 2), MarksmanshipMt()),
                                        MarksmanshipSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Marksmanship.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, MarksmanshipDefensive()),
                                        new Decorator(ret => SG.Instance.Marksmanship.CheckInterrupts && Unit.CanInterrupt, MarksmanshipInterrupts()),
                                        MarksmanshipUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        new Action(ret => { Item.UseMarksmanshipItems(); return RunStatus.Failure; }),
                                                        MarksmanshipOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe && SG.Instance.Marksmanship.CheckAoE && Unit.NearbyAttackableUnitsCount >= 2, MarksmanshipMt()),
                                        MarksmanshipSt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite MarksmanshipSt()
        {
            return new PrioritySelector(
                new Decorator(ret => Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent >= 80,
                 new PrioritySelector(MarksmanshipStOver80())),
                 new Decorator(ret => Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 80,
                 new PrioritySelector(
              //  Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location, ret => SG.Instance.General.EnableTraps),
                Spell.Cast("Powershot", ret => TalentPowershot),
                Spell.Cast("Fervor", ret => FervorReqs),
                Spell.Cast("Dire Beast", ret => Lua.PlayerPower <= 90),
                Spell.Cast("Barrage", ret => TalentBarrage),  
                Spell.Cast("Glaive Toss", ret => TalentGlaiveToss),
                Spell.PreventDoubleCast("Steady Shot", 0.7, ret => PreSteadyBuff && PreSteadyBuffUnder5),
                Spell.PreventDoubleCast("Serpent Sting", 0.7, ret => !SerpentStingRefresh),
                Spell.Cast("Chimera Shot"),
                Spell.PreventDoubleCast("Steady Shot", 0.7, ret => SteadyFocusRemainsSteadyShotCastTime),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.PreventDoubleCast("Aimed Shot", 0.7, ret => MasterMarksManFire),
                Spell.Cast("Aimed Shot", ret => AimedshotEnough),
                Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => EnoughForArcaneShot),
                Spell.PreventDoubleCast("Steady Shot", 1))));

        }


        internal static Composite MarksmanshipStOver80()
        {
            return new PrioritySelector(
                Spell.PreventDoubleCast("Serpent Sting", 0.7, ret => !SerpentStingRefresh),
                Spell.Cast("Chimera Shot"),
                Spell.PreventDoubleCast("Steady Shot", 0.7, ret => PreSteadyBuff && PreSteadyBuffUnder6),
                Spell.Cast("Aimed Shot"),
                Spell.PreventDoubleCast("Steady Shot", 1)
                );

        }

        internal static Composite MarksmanshipMt()
        {
            return new PrioritySelector(
                  );
        }



        internal static Composite MarksmanshipDefensive()
        {
            return new PrioritySelector(
                //I.MarksmanshipUseHealthStone()
                );
        }


        internal static Composite MarksmanshipOffensive()
        {
            return new PrioritySelector(
                   Spell.Cast("A Murder of Crows", ret => MurderofCrows && (
                    (SG.Instance.Marksmanship.MurderofCrows == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.MurderofCrows == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.MurderofCrows == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Lynx Rush", ret => LynxRush && (
                    (SG.Instance.Marksmanship.LynxRush == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.LynxRush == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.LynxRush == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Rapid Fire", ret => RapidFireAura && (
                    (SG.Instance.Marksmanship.RapidFire == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.RapidFire == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.RapidFire == Enum.AbilityTrigger.Always)
                    )),
                //Rabid
                Spell.Cast("Rapid", ret => (
                    (SG.Instance.Marksmanship.Rabid == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.Rabid == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.Rabid == Enum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Stampede", ret => Me.CurrentTarget != null && (
                    (SG.Instance.Marksmanship.Stampede == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.Stampede == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.Stampede == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Marksmanship.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Marksmanship.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Marksmanship.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite MarksmanshipUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite MarksmanshipInterrupts()
        {
            {
                return new PrioritySelector(
                  );
            }
        }
        #endregion


        #region Booleans
        private static uint FocusFireStackCount
        {
            get
            {
                return Spell.GetAuraStack(Me, "Frenzy");
            }
        }

        internal static bool PreSteadyBuff { get { return Me.HasAura(53224); } }
        internal static bool PreSteadyBuffUnder6 { get { return  Spell.GetAuraTimeLeft(53224) < 6; } }
        internal static bool PreSteadyBuffUnder5 { get { return Spell.GetAuraTimeLeft(53224) < 5; } }
        //buff.steady_focus.remains<(action.steady_shot.cast_time+1)
        internal static bool SteadyFocusRemainsSteadyShotCastTime { get { return Me.GetAuraTimeLeft("Steady Focus").TotalSeconds < (Styx.WoWInternals.WoWSpell.FromId(56641).CastTime + 1); } }
        internal static bool AimedshotEnough { get { return Styx.WoWInternals.WoWSpell.FromId(19434).CastTime < 1.7; } }
        internal static bool MasterMarksManFire { get { return Me.HasAura(82926); } }
        internal static bool EnoughForArcaneShot { get { return Lua.PlayerPower >= 60 || (Lua.PlayerPower >= 43 && (Styx.WoWInternals.WoWSpell.FromId(53209).CooldownTimeLeft.TotalSeconds >= Styx.WoWInternals.WoWSpell.FromId(56641).CastTime)) && (!Me.HasAura("Rapid Fire") && !G.SpeedBuffsAura); } }
        internal static bool FervorReqs { get { return TalentManager.IsSelected(10) && (Lua.PlayerPower < 50 || Me.Pet.CurrentEnergy < 50); } }
        internal static bool LockAndLoadProc { get { return Me.HasAura("Lock and Load"); } }
        internal static bool TalentGlaiveToss { get { return TalentManager.IsSelected(16); } }
        internal static bool TalentPowershot { get { return TalentManager.IsSelected(17); } }
        internal static bool TalentBarrage { get { return TalentManager.IsSelected(18); } }
        internal static bool DireBeastEnabled { get { return TalentManager.IsSelected(11); } }
        internal static bool MurderofCrows { get { return TalentManager.IsSelected(13) && Me.CurrentTarget != null && Spell.GetAuraTimeLeft(131894, Me.CurrentTarget) < 2; } }
        internal static bool LynxRush { get { return TalentManager.IsSelected(15) && Me.CurrentTarget != null && Spell.GetAuraTimeLeft(120697, Me.CurrentTarget) < 2; } }
        internal static bool RapidFireAura { get { return !Me.HasAura(3045); } }
        internal static bool SerpentStingRefresh { get { return Me.CurrentTarget != null && (!Me.CurrentTarget.HasMyAura("Serpent Sting") || Spell.GetAuraTimeMilliEnemy("Serpent Sting") < 2000); } }
        internal static bool SerpentStingAoE { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura(1978); } }
        internal static bool ExplosiveShotOffCooldown { get { return !Styx.WoWInternals.WoWSpell.FromId(53301).Cooldown; } }
        internal static bool KillCommandCooldown { get { return Styx.WoWInternals.WoWSpell.FromId(34026).Cooldown; } }
        internal static bool FocusFireFiveStacks { get { return FocusFireStackCount == 5; } }
        internal static bool BestialWrathNotUp { get { return Lua.PlayerPower > 60 && !Me.HasAura(34471); } }
        internal static bool BestialWrathUp { get { return Me.HasAura(34471); } }
        internal static bool TargetSoonDead { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent < 21; } }
        internal static bool MultiShotThrillProc { get { return Me.HasAura("Thrill of the Hunt") && !SerpentStingRefresh; } }
        internal static bool ThrillProc { get { return Me.HasAura("Thrill of the Hunt"); } }
        internal static bool BlackArrowIsOnCooldown { get { return Styx.WoWInternals.WoWSpell.FromId(3674).Cooldown; } }
        internal static bool BestialWrathIsNotOnCooldown { get { return !Styx.WoWInternals.WoWSpell.FromId(19574).Cooldown; } }
        internal static bool BWSpellCoolDown2Seconds { get { return Styx.WoWInternals.WoWSpell.FromId(19574).CooldownTimeLeft.Milliseconds < 2000; } }
        internal static bool Focus60 { get { return Lua.PlayerPower < 64; } }
        internal static bool Focus61 { get { return Lua.PlayerPower >= 61; } }
        internal static bool CarefulAim { get { return Me.HasAura(34483); } }

        #endregion Booleans

    }
}
