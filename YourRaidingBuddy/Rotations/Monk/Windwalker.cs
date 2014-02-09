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

namespace YourBuddy.Rotations.Monk
{
    class Windwalker
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeWindwalkerCombat
        {
            get
            {
                return new PrioritySelector(
                      //  new Decorator(ret => SG.Instance.General.CheckTreePerformance, Logger.("InitializeWindwalker")),
                        new Decorator(ret => (HotKeyManager.IsPaused || !Unit.DefaultCheck), new ActionAlwaysSucceed()),
                        G.InitializeCaching(),
                        G.ManualCastPause(),
                        G.InitializeOnKeyActionsM(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Windwalker.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, WindwalkerDefensive()),
                                        new Decorator(ret => SG.Instance.Windwalker.CheckInterrupts, WindwalkerInterrupts()),
                                        WindwalkerUtility(),
                                        WindwalkerOffensive(),
                                        new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                        new Action(ret => { Item.UseWindwalkerItems(); return RunStatus.Failure; }),
                                        new Decorator(ret => SG.Instance.Windwalker.CheckAoE && Unit.NearbyAttackableUnitsCount >= 2, WindwalkerMt()),
                                            WindwalkerSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.SemiHotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Windwalker.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, WindwalkerDefensive()),
                                        new Decorator(ret => SG.Instance.Windwalker.CheckInterrupts, WindwalkerInterrupts()),
                                        WindwalkerUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                    new Action(ret => { Item.UseWindwalkerItems(); return RunStatus.Failure; }),
                                                    new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                                        WindwalkerOffensive())),
                                        new Decorator(ret => SG.Instance.Windwalker.CheckAoE && Unit.NearbyAttackableUnitsCount >= 2, WindwalkerMt()),
                                            WindwalkerSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == Enum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Windwalker.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, WindwalkerDefensive()),
                                        new Decorator(ret => SG.Instance.Windwalker.CheckInterrupts, WindwalkerInterrupts()),
                                        WindwalkerUtility(),
                                        new Decorator(ret => HotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                    new Action(ret => { Item.UseWindwalkerItems(); return RunStatus.Failure; }),
                                                    new Decorator(ret => SG.Instance.General.CheckPotionUsage && G.SpeedBuffsAura, Item.UseBagItem(76089, ret => true, "Using Virmen's Bite Potion")),
                                                        WindwalkerOffensive())),
                                        new Decorator(ret => HotKeyManager.IsAoe, WindwalkerMt()),
                                        WindwalkerSt())));
            }
        }
        #endregion

        #region BoolsTemp

        internal static bool TalentChiBrewEnabled { get { return TalentManager.IsSelected(9); } }
        internal static bool FistsofFuryIsCasting { get { return Me.HasAura(113656) && Lua.PlayerPower <= 99; } }
        internal static bool EnergizingBrewDown { get { return !Me.HasAura(115288); } }
        internal static bool RSKCooldown { get { return CooldownTracker.SpellOnCooldown(107428); } }
        internal static double TigerPowerRemains { get { return Spell.GetMyAuraTimeLeft(125359, Me); } }
        internal static bool ComboBreakerBoKUp { get { return Me.HasAura(116768); } }
        internal static bool TalentAscensionEnabled { get { return TalentManager.IsSelected(8); } }
        internal static bool RushingJadeWindTalent { get { return TalentManager.IsSelected(16) && SG.Instance.Windwalker.EnableRJWSingleTarget; } }
        internal static bool ComboBreakerTpUp { get { return Me.HasAura(118864); } }
        internal static double ComboBreakerTpRemains { get { return Spell.GetMyAuraTimeLeft(118864, Me); } }
        internal static double RisingSunKickDebuffRemains { get { return Spell.GetMyAuraTimeLeft(130320, Me.CurrentTarget); } }
        internal static bool TigerEyeUseDown { get { return !Me.HasAura(116740); } }
        internal static double TigerEyeBrewStacks { get { return Spell.GetAuraStack(Me, 125195); } }

        public static int MaxChi { get { return TalentManager.IsSelected(8) ? 5 : 4; } } // 
#endregion

        #region Rotations
        internal static Composite WindwalkerSt()
        {
            return new PrioritySelector(
               Spell.Cast("Tiger Palm", ret => TigerPowerRemains <= 3),
               Spell.Cast("Rising Sun Kick"),
                Spell.Cast("Tiger Palm", ret => !Me.HasAura(125359) && RisingSunKickDebuffRemains > 1 && Lua.TimeToEnergyCap() > 1),
                Spell.Cast("Touch of Death", ret => SG.Instance.Windwalker.TouchOfDeath && ((Me.HasAura(124490) && Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 10) || (Lua.PlayerChi >= 3 && Me.HasAura("Death Note")))),
                Spell.Cast("Fists of Fury", ret => !Me.IsMoving && EnergizingBrewDown && Lua.TimeToEnergyCap() > 4 && TigerPowerRemains > 4),
                Spell.Cast("Chi Wave"),
                Spell.Cast("Blackout Kick", ret => ComboBreakerBoKUp),
                Spell.Cast("Tiger Palm", ret => (ComboBreakerTpUp && Lua.TimeToEnergyCap() >= 2) || (ComboBreakerTpRemains <= 2 && ComboBreakerTpUp)),
                Spell.Cast("Jab", ret => MaxChi - Lua.PlayerChi >= 2),
                Spell.Cast("Rushing Jade Wind", ret => RushingJadeWindTalent && Lua.RJWOK() >= 20),
                Spell.Cast("Blackout Kick", ret => Lua.BlackoutKickOK() >= 40)
                );

        }

        internal static Composite WindwalkerMt()
        {
            return new PrioritySelector(
                Spell.Cast("Rushing Jade Wind", ret => TalentManager.IsSelected(16) && !CooldownTracker.SpellOnCooldown("Rising Sun Kick")),
                new Decorator(ret => CooldownTracker.GetSpellCooldown("Rising Sun Kick").TotalMilliseconds < 1000, new Action(delegate { Me.CancelAura("Spinning Crane Kick"); return RunStatus.Failure; })), // If Rising Sun Kick is coming off cooldown, STOP
                Spell.Cast("Spinning Crane Kick", ret => CooldownTracker.GetSpellCooldown("Rising Sun Kick").TotalMilliseconds > 1000 && Unit.NearbyAttackableUnitsCount > 3),
             //   Spell.Cast("Storm, Earth, and Fire"),
                WindwalkerSt());
        }



        internal static Composite WindwalkerDefensive()
        {
            return new PrioritySelector(
                Spell.Cast("Expel Harm", ret => Me.HealthPercent <= SG.Instance.Windwalker.ExpelHarmHP),
                Spell.Cast("Fortifying Brew", ret => Me.HealthPercent <= SG.Instance.Windwalker.FortifyingBrewHP),
           //     Spell.Cast("Healing Sphere", ret => Me, ret => Me.HealthPercent <= 90),
                Spell.Cast("Touch of Karma", ret => Me.HealthPercent <= SG.Instance.Windwalker.TouchofKarmaHP),
                Item.WindwalkerUseHealthStone()
                );
        }


        internal static Composite WindwalkerOffensive()
        {
            return new PrioritySelector(
               Spell.Cast("Tigereye Brew", ret => ((TigerEyeUseDown && TigerEyeBrewStacks == 20) || (Me.GetAllAuras().Any(a => G.AgilityProcList.Contains(a.SpellId)) && TigerEyeUseDown) || (TigerEyeUseDown && Lua.PlayerChi >= 2 && (Me.GetAllAuras().Any(a => G.AgilityProcList.Contains(a.SpellId)) || TigerEyeBrewStacks >= 15 || Me.CurrentTarget.HealthPercent < 20) && RisingSunKickDebuffRemains > 1 && TigerPowerRemains > 1)) && (
                  (SG.Instance.Windwalker.TigereyeBrew == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                   (SG.Instance.Windwalker.TigereyeBrew == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.TigereyeBrew == Enum.AbilityTrigger.Always)
                    )),
                
               Spell.Cast("Chi Brew", ret => ((TalentChiBrewEnabled && Lua.PlayerChi <= 2) && (Me.GetAllAuras().Any(a => G.AgilityProcList.Contains(a.SpellId)) || (Lua.LuaGetSpellCharges() == 1 && Lua.GetSpellRechargeTime(115399, 45) <= 10) || Lua.LuaGetSpellCharges() == 2 || (Lua.LuaGetSpellCharges() > 1 && Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 10))) && (
                    (SG.Instance.Windwalker.ChiBrew == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Windwalker.ChiBrew == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.ChiBrew == Enum.AbilityTrigger.Always)
                    )),
               Spell.Cast("Energizing Brew", ret => Lua.TimeToEnergyCap() > 5 && (
                    (SG.Instance.Windwalker.EnergizingBrew == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Windwalker.EnergizingBrew == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.EnergizingBrew == Enum.AbilityTrigger.Always)
                    )),
               Spell.Cast("Invoke Xuen, the White Tiger", ret => (TalentManager.IsSelected(17) || !TalentManager.IsSelected(16)) && (
                    (SG.Instance.Windwalker.Xuen == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Windwalker.Xuen == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.Xuen == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Windwalker.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Windwalker.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Windwalker.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Windwalker.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.ClassRacials == Enum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Windwalker.ClassRacials == Enum.AbilityTrigger.OnBossDummy && Unit.IsTargetBoss) ||
                    (SG.Instance.Windwalker.ClassRacials == Enum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.ClassRacials == Enum.AbilityTrigger.Always)
                    )));
        }

        internal static Composite WindwalkerUtility()
        {
            return new PrioritySelector(
                  );
        }

        internal static Composite WindwalkerInterrupts()
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


        #endregion Booleans

    }
}
