﻿using CommonBehaviors.Actions;
using YourBuddy.Core;
using YourBuddy.Interfaces;
using System.Linq;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using Styx.CommonBot;
using SG = YourBuddy.Interfaces.Settings.InternalSettings;
using TalentManager = YourBuddy.Core.Managers.TalentManager;
using YourBuddy.Core.Helpers;
using YourBuddy.Core.Managers;
using SH = YourBuddy.Interfaces.Settings.SettingsH;
using Lua = YourBuddy.Core.Helpers.LuaClass;
using G = YourBuddy.Rotations.Global;
using YourBuddy.Interfaces.Settings;
using U = YourBuddy.Core.Unit;

namespace YourBuddy.Rotations.Monk
{
    class Brewmaster
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeBrewmasterCombat
        {
            get
            {
                return new PrioritySelector(
                       // new Decorator(ret => SG.Instance.General.CheckTreePerformance, XILogger.TreePerformance("InitializeBrewmaster")),
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Brewmaster.CheckAutoAttack, Lua.StartAutoAttack),
                                            Spell.Cast("Elusive Brew", ret => U.NearbyAggroUnitsCount >= 1 && Spell.GetAuraStack(Me, 128939) >= MonkSettings.ElusiveBrew),    
                                            BrewmasterDefensive(),
                                        new Decorator(ret => SG.Instance.Brewmaster.CheckInterrupts, BrewmasterInterrupts()),
                                        BrewmasterUtility(),
                                      //  I.BrewmasterUseItems(),
                                        BrewmasterOffensive(),
                                        new Decorator(ret => SG.Instance.Brewmaster.CheckAoE && Unit.NearbyAttackableUnitsCount > 2, BrewmasterMt()),
                                            BrewmasterSt())),
                        new Decorator(ret => SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Brewmaster.CheckAutoAttack, Lua.StartAutoAttack),
                                        BrewmasterDefensive(),
                                        new Decorator(ret => SG.Instance.Brewmaster.CheckInterrupts, BrewmasterInterrupts()),
                                        BrewmasterUtility(),
                                        Spell.Cast("Elusive Brew", ret => U.NearbyAggroUnitsCount >= 1 && Spell.GetAuraStack(Me, 128939) >= MonkSettings.ElusiveBrew))),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                   //     I.BrewmasterUseItems(),
                                                        BrewmasterOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, BrewmasterMt()),
                                        BrewmasterSt());
            }
        }
        #endregion

        #region BoolsTemp

        internal static bool TalentJadeWindEnabled { get { return TalentManager.IsSelected(16); } }

        private static YbSettingsBMM MonkSettings { get { return SG.Instance.Brewmaster; } }

        internal static double ShuffleSetting { get { return Spell.GetMyAuraTimeLeft(115307, Me); } }

        internal static bool CanUsePurifyingBrew { get { return ((Lua.PlayerChi >= 3 || Me.HasAura(138237)) && Me.HasAura(124273)) || (MonkSettings.PurifyingModerate && Lua.PurifyingBrew(124274) > (StyxWoW.Me.MaxHealth * MonkSettings.HPModerateScale / 100) && Me.HasAura(124274) && (ShuffleSetting > 4 || Lua.PlayerChi > 3)) || (MonkSettings.PurifyingLight && Me.HasAura(124275) && (ShuffleSetting > 10 || Lua.PlayerChi > 3)); } }

        internal static bool CanApplyDizzyingHaze { get { return U.NearbyAttackableUnits(Me.CurrentTarget.Location, 8).Any(x => !x.HasAura("Dizzying Haze") && !x.IsBoss && !x.IsFlying) && Me.HasAura("Shuffle"); } }

        internal static bool CanApplyBreathofFire { get { return U.NearbyAttackableUnits(Me.CurrentTarget.Location, 12).Any(x => !x.HasAura("Breath of Fire") && Me.IsSafelyFacing(x)); } }

        internal static bool CanPlaceBlackOxStatue { get { return SG.Instance.Brewmaster.SummonBlackOxStatue && Me.CurrentTarget != null && !Me.HasAura("Sanctuary of the Ox") && !Me.CurrentTarget.IsFlying && !Me.IsOnTransport; } }


        public static int MaxChi { get { return TalentManager.IsSelected(8) ? 5 : 4; } } // 


        private static bool NeedGuard { get { return Me.HasAura(118636) && ShuffleSetting > 2; } }

        private static bool NeedDampenHarm { get { return TalentManager.IsSelected(14) && Me.HealthPercent <= MonkSettings.DampenHarmPercent && !Me.HasAura("Fortifying Brew"); } }

        private static bool NeedFortifyingBrew { get { return Me.HealthPercent <= MonkSettings.FortifyingBrewPercent && !Me.HasAura("Fortifying Brew") && !Me.HasAura("Dampen Harm"); } }

        private static bool NeedZenMeditation { get { return Me.HealthPercent <= MonkSettings.ZenMeditationPercent; } }

        private static bool NeedBuildStacksForGaurd { get { return Lua.PlayerChi >= 1 && !Me.HasAura(118636); } }

        private static bool NeedRushingJadeWind { get { return Lua.PlayerChi >= 2 && TalentManager.IsSelected(16); } }

        private static bool GuardOK { get { return Lua.PlayerChi >= 2 && (Root._initap + Root._NewAP) > Root._initap + (StyxWoW.Me.MaxHealth * MonkSettings.HPAPScale / 100) && Me.HasAura(118636) && ShuffleSetting >= 1; } }

        private static bool NeedBlackoutKick { get { return (!Me.HasAura(115307) && Lua.PlayerChi >= 2) || Lua.PlayerChi > 3 || (ShuffleSetting <= 3 && Lua.PlayerChi >= 2); } }

        private static bool NeedTouchofDeath { get { return Me.HasAura("Death Note") && (Me.HealthPercent > 60 || TalentManager.HasGlyph("Touch of Death")); } }

        private static bool NeedDizzyingHaze { get { return Lua.PlayerPower >= 40 && CanApplyDizzyingHaze && MonkSettings.UseDizzyingHaze; } }

        private static bool CanJab { get { return Styx.WoWInternals.WoWSpell.FromId(121253).Cooldown; } }

        private static bool NeedBreathofFire { get { return CanApplyBreathofFire && ShuffleSetting >= 4; } }

        private static bool NeedChiWave { get { return TalentManager.IsSelected(4); } }

        internal static Composite ClearDizzyingHaze()
        {
            return new Decorator(ret => StyxWoW.Me.HasPendingSpell("Dizzying Haze") && CanApplyDizzyingHaze, new Action(ret => Styx.WoWInternals.Lua.DoString("SpellStopTargeting()")));
        }

        private static bool NeedHealingSphere { get { return Lua.PlayerPower >= 60 && Me.HealthPercent <= MonkSettings.HealingSpherePercent; } }

        private static bool NeedZenSphere { get { return TalentManager.IsSelected(5) && Me.HealthPercent <= MonkSettings.BrewWithZenSphere; } }



        #endregion

        #region Rotations

        internal static Composite ChiBuilder()
        {
            return new PrioritySelector(
            Spell.Cast("Keg Smash", ret => Lua.PlayerPower >= 40),
            Spell.Cast("Expel Harm", ret => (Me.HealthPercent <= 90  && Lua.JabOK() <= 30) || Me.HealthPercent <= 35),
            Spell.PreventDoubleCast("Jab", 0.5, ret => Lua.PlayerPower >= 40 && Lua.JabOK() >= 30 && Me.HealthPercent > 35),
            Spell.PreventDoubleCast("Tiger Palm", 1, ret => Lua.JabOK() <= 30 && ShuffleSetting > 3 && CooldownTracker.GetSpellCooldown(115295).TotalSeconds > 1)
                );

        }

       

        internal static Composite BrewmasterSt()
        {
            return new PrioritySelector(
            Spell.Cast("Blackout Kick", ret => NeedBlackoutKick), // Apply shuffle if not active or MaxChi
            Spell.PreventDoubleCast("Tiger Palm", 1, ret => NeedBuildStacksForGaurd), // Build PG and
            Spell.Cast("Rushing Jade Wind", ret =>  ShuffleSetting >= 5 && MonkSettings.UseRJWSingleTarget),
            Spell.Cast("Touch of Death", ret => NeedTouchofDeath), // Touch of Death fosho
            new Decorator(ret => Lua.PlayerChi < MaxChi, ChiBuilder())
                );

        }

        internal static Composite BrewmasterMt()
        {
            return new PrioritySelector(
            Spell.Cast("Blackout Kick", ret => NeedBlackoutKick), // Apply fhuffle if not active or MaxChi
            Spell.PreventDoubleCast("Tiger Palm", 1, ret => NeedBuildStacksForGaurd), // Build PG and TP for Guard
            new Decorator(ret => !Me.HasAura("Shuffle"), new Action(delegate { Me.CancelAura("Spinning Crane Kick"); return RunStatus.Failure; })), // If we loose shuffle, STOP
            Spell.CastOnGround("Dizzying Haze", ret => Me.CurrentTarget.Location, ret => NeedDizzyingHaze),
            ClearDizzyingHaze(), // hackish but that fucking circle shit pisses me off.. -- wulf.
            Spell.Cast("Spinning Crane Kick", ret => U.NearbyAttackableUnitsCount >= MonkSettings.RJWCount && (ShuffleSetting >= 1 && Me.HasAura(118636) && Me.HasAura(125359) || (ShuffleSetting >= 1 && Me.HealthPercent < 70 && CooldownTracker.GetSpellCooldown(115295).TotalSeconds > 1))),
            Spell.Cast("Breath of Fire", ret => NeedBreathofFire && MonkSettings.CheckBreathofFire),
            new Decorator(ret => Me.CurrentChi < Me.MaxChi, ChiBuilder())
                  );
        }


        private static Composite HandleHealingCooldowns()
        {
            return new PrioritySelector(
                   // Spell.CastOnGround("Healing Sphere", ret => Me.Location, ret => NeedHealingSphere),
                    Item.BrewmasterUseHealthStone());
        }


        internal static Composite BrewmasterDefensive()
        {
                        return new PrioritySelector(
                     Spell.Cast("Purifying Brew", ret => CanUsePurifyingBrew), // Top Priority
                     Spell.Cast("Dampen Harm", ret => NeedDampenHarm),
                     Spell.Cast("Guard", ret => GuardOK),
                     Spell.Cast("Chi Wave"),
                     new Decorator(ret => Me.HealthPercent < 100, HandleHealingCooldowns()),
                     Spell.Cast("Fortifying Brew", ret => NeedFortifyingBrew),
                     Spell.PreventDoubleCast("Zen Sphere", 0.5, ret => NeedZenSphere),
                     Spell.Cast("Zen Meditation", ret => NeedZenMeditation));
        }


        internal static Composite BrewmasterOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Brewmaster.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Brewmaster.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Brewmaster.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Brewmaster.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Brewmaster.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Brewmaster.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Brewmaster.ClassRacials == Enum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Brewmaster.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Brewmaster.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite BrewmasterUtility()
        {
            return new PrioritySelector(
                Spell.CastOnGround("Summon Black Ox Statue", ret => Me.CurrentTarget.Location, ret => CanPlaceBlackOxStatue, true), // Checks target is not flying and we are not fighting elegon.
                Spell.Cast("Disable", ret => UnitIsFleeing)
                );
        }

        internal static Composite BrewmasterInterrupts()
        {
            {
                  return new PrioritySelector(
                   new Throttle(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Spear Hand Strike", ret => (SG.Instance.General.InterruptList == Enum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == Enum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
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