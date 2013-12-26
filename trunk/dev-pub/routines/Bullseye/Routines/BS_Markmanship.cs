using CommonBehaviors.Actions;
using Bullseye.Core;
using Bullseye.Helpers;
using Bullseye.Managers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = Bullseye.Routines.BsGlobal;
using I = Bullseye.Core.BsItem;
using Lua = Bullseye.Helpers.BsLua;
using T = Bullseye.Managers.BsTalentManager;
using SG = Bullseye.Interfaces.Settings.BsSettings;
using SH = Bullseye.Interfaces.Settings.BsSettingsH;
using Spell = Bullseye.Core.BsSpell;
using U = Bullseye.Core.BsUnit;
using Styx.CommonBot;

namespace Bullseye.Routines
{
    class BsMarksmanship
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeMarksmanship
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, BsLogger.TreePerformance("InitializeMarksmanship")),
                        new Decorator(ret => (BsHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.General.CheckABsancedLogging, BsLogger.ABsancedLogging),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BsEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Marksmanship.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, MarksmanshipDefensive()),
                                        new Decorator(ret => SG.Instance.Marksmanship.CheckInterrupts && U.CanInterrupt, MarksmanshipInterrupts()),
                                        MarksmanshipUtility(),
                                        I.MarksmanshipUseItems(),
                                        MarksmanshipOffensive(),
                                        new Decorator(ret => SG.Instance.Marksmanship.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), MarksmanshipMt()),
                                        MarksmanshipSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BsEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Marksmanship.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, MarksmanshipDefensive()),
                                        new Decorator(ret => SG.Instance.Marksmanship.CheckInterrupts && U.CanInterrupt, MarksmanshipInterrupts()),
                                        MarksmanshipUtility(),
                                        new Decorator(ret => BsHotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.MarksmanshipUseItems(),
                                                        MarksmanshipOffensive())),
                                        new Decorator(ret => BsHotKeyManager.IsAoe && SG.Instance.Marksmanship.CheckAoE && U.AttackableMeleeUnitsCount >= 2, MarksmanshipMt()),
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
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location, ret => SG.Instance.General.EnableTraps),
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
                I.MarksmanshipUseHealthStone()
                );
        }


        internal static Composite MarksmanshipOffensive()
        {
            return new PrioritySelector(
                   Spell.Cast("A Murder of Crows", ret => MurderofCrows && (
                    (SG.Instance.Marksmanship.MurderofCrows == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.MurderofCrows == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.MurderofCrows == BsEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Lynx Rush", ret => LynxRush && (
                    (SG.Instance.Marksmanship.LynxRush == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.LynxRush == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.LynxRush == BsEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Rapid Fire", ret => RapidFireAura && (
                    (SG.Instance.Marksmanship.RapidFire == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.RapidFire == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.RapidFire == BsEnum.AbilityTrigger.Always)
                    )),
                //Rabid
                Spell.Cast("Rapid", ret => (
                    (SG.Instance.Marksmanship.Rabid == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.Rabid == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.Rabid == BsEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Stampede", ret => Me.CurrentTarget != null && (
                    (SG.Instance.Marksmanship.Stampede == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.Stampede == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.Stampede == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Marksmanship.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Marksmanship.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Marksmanship.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                    I.UsePotion());
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
                return Spell.GetAuraStackCount("Frenzy");
            }
        }

        internal static bool PreSteadyBuff { get { return Me.HasAura(53224); } }
        internal static bool PreSteadyBuffUnder6 { get { return !Me.HasCachedAura(53224, 0, 6000); } }
        internal static bool PreSteadyBuffUnder5 { get { return !Me.HasCachedAura(53224, 0, 5000); } }
        //buff.steady_focus.remains<(action.steady_shot.cast_time+1)
        internal static bool SteadyFocusRemainsSteadyShotCastTime { get { return Me.GetAuraTimeLeft("Steady Focus").TotalSeconds < (Styx.WoWInternals.WoWSpell.FromId(56641).CastTime + 1); } }
        internal static bool AimedshotEnough { get { return Styx.WoWInternals.WoWSpell.FromId(19434).CastTime < 1.7; } }
        internal static bool MasterMarksManFire { get { return Me.HasAura(82926); } }
        internal static bool EnoughForArcaneShot { get { return Lua.PlayerPower >= 60 || (Lua.PlayerPower >= 43 && (Styx.WoWInternals.WoWSpell.FromId(53209).CooldownTimeLeft.TotalSeconds >= Styx.WoWInternals.WoWSpell.FromId(56641).CastTime)) && (!Me.HasAura("Rapid Fire") && !G.SpeedBuffsAura); } }
        internal static bool FervorReqs { get { return BsTalentManager.HasTalent(10) && (Lua.PlayerPower < 50 || Me.Pet.CurrentEnergy < 50); } }
        internal static bool LockAndLoadProc { get { return Me.HasAura("Lock and Load"); } }
        internal static bool TalentGlaiveToss { get { return BsTalentManager.HasTalent(16); } }
        internal static bool TalentPowershot { get { return BsTalentManager.HasTalent(17); } }
        internal static bool TalentBarrage { get { return BsTalentManager.HasTalent(18); } }
        internal static bool DireBeastEnabled { get { return BsTalentManager.HasTalent(11); } }
        internal static bool MurderofCrows { get { return BsTalentManager.HasTalent(13) && Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(131894, 0, 2000); } }
        internal static bool LynxRush { get { return BsTalentManager.HasTalent(15) && Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(120697, 0, 2000); } }
        internal static bool RapidFireAura { get { return !Me.HasAura(3045); } }
        internal static bool SerpentStingRefresh { get { return Me.CurrentTarget != null && Me.CurrentTarget.HasCachedAura("Serpent Sting", 0, 2000); } }
        internal static bool SerpentStingAoE { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasMyAura(1978); } }
        internal static bool SerpentStingRefresh6Seconds { get { return Me.CurrentTarget != null && Me.CurrentTarget.HasCachedAura("Serpent Sting", 0, 6000); } }
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
