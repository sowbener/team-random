using System.Linq;
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
    class BsSurvival
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeSurvival
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, BsLogger.TreePerformance("InitializeSurvival")),
                        new Decorator(ret => (BsHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.General.CheckABsancedLogging, BsLogger.ABsancedLogging),
                        new Decorator(ret => SG.Instance.Survival.EnableCallPet, PetManager.CreateHunterCallPetBehavior()),
                        new Decorator(ret => BsHotKeyManager.IsSpecialKey, new PrioritySelector(Spell.Cast("Binding Shot", ret => BsTalentManager.HasTalent(4)))),
                        G.InitializeCaching(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BsEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Survival.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, SurvivalDefensive()),
                                        new Decorator(ret => SG.Instance.Survival.CheckInterrupts && U.CanInterrupt, SurvivalInterrupts()),
                                        SurvivalUtility(),
                                        I.SurvivalUseItems(),
                                        SurvivalOffensive(),
                                        new Decorator(ret => SG.Instance.Survival.CheckAoE && (BsUnit.NearbyAttackableUnitsCount > 2), SurvivalMt()),
                                            SurvivalSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BsEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Survival.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, SurvivalDefensive()),
                                        new Decorator(ret => SG.Instance.Survival.CheckInterrupts && U.CanInterrupt, SurvivalInterrupts()),
                                        SurvivalUtility(),
                                        new Decorator(ret => BsHotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.SurvivalUseItems(),
                                                        SurvivalOffensive())),
                                        new Decorator(ret => BsHotKeyManager.IsAoe, SurvivalMt()),
                                        SurvivalSt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite SurvivalSt()
        {
            return new PrioritySelector
            (
            Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location, ret => SG.Instance.General.EnableTraps),
            Spell.Cast("Fervor", ret => FervorReqs),
            Spell.Cast("Explosive Shot", ret => LockAndLoadProc),
            Spell.Cast("Glaive Toss", ret => TalentGlaiveToss),
            Spell.PreventDoubleCast("Serpent Sting", 0.7, ret => !SerpentStingRefresh),
            Spell.Cast("Explosive Shot"),
            Spell.Cast("Kill Shot", ret => TargetSoonDead),
            Spell.Cast("Black Arrow"),
            Spell.Cast("Arcane Shot", ret => ThrillProc && BlackArrowIsOnCooldown),
            Spell.Cast("Dire Beast"),
            Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => !SerpentStingRefresh6Seconds, true),
            Spell.PreventDoubleCast("Arcane Shot", 0.7, ret => Focus67),
            Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus66, true),
            Spell.PreventDoubleCast("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }

        internal static Composite SurvivalMt()
        {
            return new PrioritySelector(
                Spell.Cast("Serpent Sting", ret => !G.HasSerpentSting),
                Spell.Cast("Glaive Toss"),
                Spell.Cast("Powershot"),
                Spell.Cast("Barrage"),
                Spell.Cast("Multi-Shot", ret => Lua.PlayerPower > 79),
                Spell.Cast("Black Arrow"),
                Spell.Cast("Explosive Shot"),
                Spell.Cast("Kill Shot", ret => TargetSoonDead),
                Spell.Cast("Multi-Shot"),
                Spell.CastHunterTrap("Explosive Trap", loc => Me.CurrentTarget.Location),
                Spell.PreventDoubleCast("Cobra Shot", Spell.GetSpellCastTime(77767), target => Me.CurrentTarget, ret => Focus66, true),
                Spell.PreventDoubleCast("Steady Shot", Spell.GetSpellCastTime(56641), target => Me.CurrentTarget, ret => Lua.PlayerPower < 30 && Me.Level < 81, true));
        }


        internal static Composite HandleCommon()
        {
            return new PrioritySelector(
                Spell.Cast("Mend Pet", ret => Me.Pet.HealthPercent <= 40 && !Me.Pet.HasAura("Mend Pet")));
        }

        internal static Composite SurvivalDefensive()
        {
            return new PrioritySelector(
                PetManager.CreateCastPetAction("Heart of the Phoenix", ret => SG.Instance.Survival.EnableRevivePet && (Me.Pet == null || (Me.Pet != null && !Me.Pet.IsAlive))), 
                I.SurvivalUseHealthStone()
                );
        }


        internal static Composite SurvivalOffensive()
        {
            return new PrioritySelector(
                //actions+=/a_murder_of_crows,if=enabled&!ticking
                //actions+=/lynx_rush,if=enabled&!dot.lynx_rush.ticking
                //actions+=/rapid_fire,if=!buff.rapid_fire.up
                //actions+=/stampede,if=buff.rapid_fire.up|buff.bloodlust.react|target.time_to_die<=25
                //PetManager.CreateCastPetActionOn("Rabid", ret => Me, ret => PetSettings.Rabid && Lists.TargetIsBoss(Target)), 
                Spell.Cast("A Murder of Crows", ret => MurderofCrows && (
                    (SG.Instance.Survival.MurderofCrows == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.MurderofCrows == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.MurderofCrows == BsEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Lynx Rush", ret => LynxRush && (
                    (SG.Instance.Survival.LynxRush == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.LynxRush == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.LynxRush == BsEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Rapid Fire", ret => RapidFireAura && (
                    (SG.Instance.Survival.RapidFire == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.RapidFire == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.RapidFire == BsEnum.AbilityTrigger.Always)
                    )),
                    Spell.Cast("Stampede", ret => Me.CurrentTarget != null && (RapidFireAura || G.SpeedBuffsAura || Me.CurrentTarget.HealthPercent <= 25) && (
                    (SG.Instance.Beastmastery.Stampede == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.Stampede == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.Stampede == BsEnum.AbilityTrigger.Always)
                    )),
                    PetManager.CreateCastPetActionOn("Rabid", ret => Me, ret => Me.CurrentTarget != null && (RapidFireAura || G.SpeedBuffsAura || Me.CurrentTarget.HealthPercent <= 25) && (
                    (SG.Instance.Survival.Rabid == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.Rabid == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.Rabid == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Survival.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Survival.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Survival.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Survival.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                    I.UsePotion());
        }

        internal static Composite SurvivalUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite SurvivalInterrupts()
        {
            {
                return new PrioritySelector(
                    new ThrottlePasses(1, System.TimeSpan.FromMilliseconds(1000), RunStatus.Failure,
                        Spell.Cast("Counter Shot")
                          ));
            }
        }

        #endregion


        #region Booleans

        internal static bool FervorReqs { get { return BsTalentManager.HasTalent(10) && Lua.PlayerPower <= 50; } }
        internal static bool LockAndLoadProc { get { return Me.HasAura("Lock and Load"); } }
        internal static bool TalentGlaiveToss { get { return BsTalentManager.HasTalent(16); } }
        internal static bool TalentPowershot { get { return BsTalentManager.HasTalent(17); } }
        internal static bool TalentBarrage { get { return BsTalentManager.HasTalent(18); } }
        internal static bool DireBeastEnabled { get { return BsTalentManager.HasTalent(11); } }
        internal static bool MurderofCrows { get { return BsTalentManager.HasTalent(13) && Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(131894, 0, 2000); } }
        internal static bool LynxRush { get { return BsTalentManager.HasTalent(15) && Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(120697, 0, 2000); } }
        internal static bool RapidFireAura { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(3045, 0, 2000); } }
        internal static bool SerpentStingRefresh { get { return Me.CurrentTarget !=null && Me.CurrentTarget.HasCachedAura("Serpent Sting", 0, 2000); } }
        internal static bool SerpentStingRefresh6Seconds { get { return Me.CurrentTarget != null && Me.CurrentTarget.HasCachedAura("Serpent Sting", 0, 6000); } }
        internal static bool ExplosiveShotOffCooldown { get { return !Styx.WoWInternals.WoWSpell.FromId(53301).Cooldown; } }
        internal static bool TargetSoonDead { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent < 21; } }
        internal static bool MultiShotThrillProc { get { return Me.HasAura(34720) && !SerpentStingRefresh; } }
        internal static bool ThrillProc { get { return Me.HasAura(34720); } }
        internal static bool BlackArrowIsOnCooldown { get { return Styx.WoWInternals.WoWSpell.FromId(3674).Cooldown; } }
        internal static bool Focus66 { get { return Lua.PlayerPower < 66; } }
        internal static bool Focus67 { get { return Lua.PlayerPower >= 67; } }



        #endregion Booleans

    }
}
