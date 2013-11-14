using CommonBehaviors.Actions;
using Xiaolin.Core;
using Xiaolin.Helpers;
using Xiaolin.Managers;
using Xiaolin.Interfaces;
using System.Linq;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = Xiaolin.Routines.XIGlobal;
using I = Xiaolin.Core.XIItem;
using Lua = Xiaolin.Helpers.XILua;
using T = Xiaolin.Managers.XITalentManager;
using SG = Xiaolin.Interfaces.Settings.XISettings;
using SH = Xiaolin.Interfaces.Settings.XISettingsH;
using Spell = Xiaolin.Core.XISpell;
using U = Xiaolin.Core.XIUnit;
using Styx.CommonBot;
using Xiaolin.Interfaces.Settings;

namespace Xiaolin.Routines
{
    class XIBrewmaster
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeBrewmaster
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, XILogger.TreePerformance("InitializeBrewmaster")),
                        new Decorator(ret => (XIHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.General.CheckAXIancedLogging, XILogger.AXIancedLogging),
                        G.InitializeCaching(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == XIEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Brewmaster.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BrewmasterDefensive()),
                                        new Decorator(ret => SG.Instance.Brewmaster.CheckInterrupts && U.CanInterrupt, BrewmasterInterrupts()),
                                        BrewmasterUtility(),
                                        I.BrewmasterUseItems(),
                                        BrewmasterOffensive(),
                                        new Decorator(ret => SG.Instance.Brewmaster.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), BrewmasterMt()),
                                            BrewmasterSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == XIEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Brewmaster.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BrewmasterDefensive()),
                                        new Decorator(ret => SG.Instance.Brewmaster.CheckInterrupts && U.CanInterrupt, BrewmasterInterrupts()),
                                        BrewmasterUtility(),
                                        new Decorator(ret => XIHotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.BrewmasterUseItems(),
                                                        BrewmasterOffensive())),
                                        new Decorator(ret => XIHotKeyManager.IsAoe && SG.Instance.Brewmaster.CheckAoE && U.AttackableMeleeUnitsCount >= 2, BrewmasterMt()),
                                        BrewmasterSt())));
            }
        }
        #endregion

        #region BoolsTemp

        private static XISettingsBM MonkSettings { get { return SG.Instance.Brewmaster; } }

        internal static double ShuffleSetting { get { return Spell.GetAuraTimeLeft(115307, Me); } }

        internal static bool CanUsePurifyingBrew { get { return ((Lua.PlayerChi >= 3 || Me.HasAura(138237)) && Me.HasAura(124273)) || (Me.HasAura(124274) && (ShuffleSetting > 4 || Lua.PlayerChi > 3)) || (Me.HasAura(124275) && (ShuffleSetting > 10 || Lua.PlayerChi > 3)); } }

        internal static bool CanApplyDizzyingHaze { get { return XIUnit.NearbyAttackableUnits(Me.CurrentTarget.Location, 8).Any(x => !x.HasAura("Dizzying Haze") && !x.IsBoss && !x.IsFlying) && Me.HasAura("Shuffle"); } }

        internal static bool CanApplyBreathofFire { get { return XIUnit.NearbyAttackableUnits(Me.CurrentTarget.Location, 12).Any(x => !x.HasAura("Breath of Fire") && Me.IsSafelyFacing(x)); } }

        internal static bool CanPlaceBlackOxStatue { get { return SG.Instance.Brewmaster.SummonBlackOxStatue && Me.CurrentTarget != null && !Me.HasAura("Sanctuary of the Ox") && !Me.CurrentTarget.IsFlying && !Me.IsOnTransport; } }


        public static int MaxChi { get { return XITalentManager.HasTalent(8) ? 5 : 4; } } // 
        
        
        private static bool NeedGuard { get { return Me.HasAura("Power Guard") && Me.HasAura("Shuffle") && Me.HealthPercent <= MonkSettings.GuardHPPercent; } }

        private static bool NeedDampenHarm { get { return XITalentManager.HasTalent(14) && Me.HealthPercent <= MonkSettings.DampenHarmPercent && !Me.HasAura("Fortifying Brew"); } }

        private static bool NeedFortifyingBrew { get { return Me.HealthPercent <= MonkSettings.FortifyingBrewPercent && !Me.HasAura("Fortifying Brew") && !Me.HasAura("Dampen Harm"); } }

        private static bool NeedZenMeditation { get { return Me.HealthPercent <= MonkSettings.ZenMeditationPercent; } }

        private static bool NeedElusiveBrew { get { return Spell.GetAuraStack(Me, 128939) > MonkSettings.ElusiveBrew && Me.HealthPercent <= MonkSettings.ElusiveBrewHP; } }

        private static bool NeedBuildStacksForGaurd { get { return !Me.HasAura(118636) || !Me.HasAura(125359) || Lua.PlayerPower <= 30; } }

        private static bool NeedRushingJadeWind { get { return Lua.PlayerChi >= 2 && XITalentManager.HasTalent(16); } }

        private static bool NeedBlackoutKick { get { return (!Me.HasAura(115307) && Lua.PlayerChi >= 2) || ShuffleSetting <= 3 || Lua.PlayerChi >= 4; } }

        private static bool NeedTouchofDeath { get { return Me.HasAura("Death Note") && (Me.HealthPercent > 60 || XITalentManager.HasGlyph("Touch of Death")); } }

        private static bool NeedDizzyingHaze { get { return Lua.PlayerPower >= 40 && CanApplyDizzyingHaze; } }

        private static bool NeedSpinningCraneKick { get { return XIUnit.NearbyAttackableUnitsCount >= MonkSettings.SpinningCraneKickCount && ShuffleSetting > 4; } }

        private static bool CanJab { get { return Styx.WoWInternals.WoWSpell.FromId(121253).Cooldown; } }

        private static bool NeedBreathofFire { get { return CanApplyBreathofFire && Me.HasAura("Shuffle"); } }

        private static bool NeedChiWave { get { return XITalentManager.HasTalent(4) && Me.HealthPercent <= MonkSettings.ChiWavePercent; } }

        private static bool NeedHealingSphere { get { return Lua.PlayerPower >= 60 && Me.HealthPercent <= MonkSettings.HealingSpherePercent; } }

        private static bool NeedZenSphere { get { return XITalentManager.HasTalent(5) && Me.HealthPercent <= MonkSettings.BrewWithZenSphere; } }



        #endregion

        #region Rotations

        internal static Composite ChiBuilder()
        {
            return new PrioritySelector(
            Spell.Cast("Keg Smash", ret => Lua.PlayerPower >= 40),
            Spell.Cast("Expel Harm", ret => (Lua.PlayerPower >= 40 && Me.HealthPercent <= 90) || (Me.HealthPercent <= 35)),
            Spell.Cast("Jab", ret => Lua.PlayerPower >= 40 && Lua.JabOK() >= 30),
            Spell.Cast("Tiger Palm", ret => Lua.JabOK() < 30)            
                );

        }
        internal static Composite BrewmasterSt()
        {
            return new PrioritySelector(
            Spell.CastOnGround("Summon Black Ox Statue", ret => Me.CurrentTarget.Location, ret => CanPlaceBlackOxStatue, true), // Checks target is not flying and we are not fighting elegon.
            Spell.Cast("Blackout Kick", ret => NeedBlackoutKick), // Apply fhuffle if not active or MaxChi
            new Decorator(ret => Lua.PlayerChi > 2 && ShuffleSetting < 5, new ActionAlwaysSucceed()),
            new Decorator(ret =>  Lua.PlayerChi < MaxChi, ChiBuilder()),
            Spell.Cast("Tiger Palm", ret => NeedBuildStacksForGaurd) // Build PG and TP for Guard
                );

        }

        internal static Composite BrewmasterMt()
        {
            return new PrioritySelector(
                  );
        }



        internal static Composite BrewmasterDefensive()
        {
                        return new PrioritySelector(
                     Spell.Cast("Purifying Brew", ret => CanUsePurifyingBrew), // Top Priority
                     Spell.PreventDoubleCast("Guard", 0.5, ret => NeedGuard), // Debating..if we dont check for shuffle it steals Chi from Blackout Kick
                     Spell.PreventDoubleCast("Dampen Harm", 0.5, ret => NeedDampenHarm),
                     Spell.Cast("Fortifying Brew", ret => NeedFortifyingBrew),
                     Spell.PreventDoubleCast("Chi Wave", 0.5, ret => NeedChiWave),
                     Spell.PreventDoubleCast("Zen Sphere", 0.5, ret => NeedZenSphere),
                     I.BrewmasterUseHealthStone(),
                     Spell.Cast("Zen Meditation", ret => NeedZenMeditation), // yeah yeah..its channeled..but it’ll reduce that one melee hit by 90%, which might save our life.
                     Spell.Cast("Elusive Brew", ret => NeedElusiveBrew));
        }


        internal static Composite BrewmasterOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Brewmaster.ClassRacials == XIEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Brewmaster.ClassRacials == XIEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Brewmaster.ClassRacials == XIEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Brewmaster.ClassRacials == XIEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Brewmaster.ClassRacials == XIEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Brewmaster.ClassRacials == XIEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Brewmaster.ClassRacials == XIEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Brewmaster.ClassRacials == XIEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Brewmaster.ClassRacials == XIEnum.AbilityTrigger.Always)
                    )),
                    I.UsePotion());
        }

        internal static Composite BrewmasterUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround("Summon Black Ox Statue", ret => Me.CurrentTarget.Location, ret => CanPlaceBlackOxStatue, true),
                Spell.Cast("Disable", ret => UnitIsFleeing)
                );
        }

        internal static Composite BrewmasterInterrupts()
        {
            {
                return new PrioritySelector(
                  );
            }
        }
        #endregion


        #region Booleans
        //thanks PR
        internal static bool UnitIsFleeing { get { return Me.CurrentTarget != null && ((Me.CurrentTarget.IsPlayer || Me.CurrentTarget.Fleeing) && Me.CurrentTarget.MovementInfo.RunSpeed > 3.5); } }
        #endregion Booleans

    }
}
