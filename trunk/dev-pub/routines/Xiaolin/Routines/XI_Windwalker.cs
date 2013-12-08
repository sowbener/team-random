using CommonBehaviors.Actions;
using Xiaolin.Core;
using Xiaolin.Helpers;
using Xiaolin.Managers;
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

namespace Xiaolin.Routines
{
    class XIWindwalker
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeWindwalker
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, XILogger.TreePerformance("InitializeWindwalker")),
                        new Decorator(ret => (XIHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.General.CheckAXIancedLogging, XILogger.AXIancedLogging),
                        G.InitializeCaching(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == XIEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Windwalker.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, WindwalkerDefensive()),
                                        new Decorator(ret => SG.Instance.Windwalker.CheckInterrupts && U.CanInterrupt, WindwalkerInterrupts()),
                                        WindwalkerUtility(),
                                        I.WindwalkerUseItems(),
                                        WindwalkerOffensive(),
                                        new Decorator(ret => SG.Instance.Windwalker.CheckAoE && U.NearbyAttackableUnitsCount > 3, WindwalkerMt()),
                                            WindwalkerSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == XIEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Windwalker.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, WindwalkerDefensive()),
                                        new Decorator(ret => SG.Instance.Windwalker.CheckInterrupts && U.CanInterrupt, WindwalkerInterrupts()),
                                        WindwalkerUtility(),
                                        new Decorator(ret => XIHotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.WindwalkerUseItems(),
                                                        WindwalkerOffensive())),
                                        new Decorator(ret => XIHotKeyManager.IsAoe, WindwalkerMt()),
                                        WindwalkerSt())));
            }
        }
        #endregion

        #region BoolsTemp

        internal static bool TalentChiBrewEnabled { get { return XITalentManager.HasTalent(9); } }
        internal static bool RisingSunKickReady { get { return !Styx.WoWInternals.WoWSpell.FromId(107428).Cooldown; } }
        internal static bool EnergizingBrewDown { get { return !Me.HasAura(115288); } }
        internal static double TigerPowerRemains { get { return Spell.GetMyAuraTimeLeft(125359, Me); } }
        internal static bool ComboBreakerBoKUp { get { return Me.HasAura(116768); } }
        internal static bool TalentAscensionEnabled { get { return XITalentManager.HasTalent(8); } }
        internal static bool RushingJadeWindTalent { get { return XITalentManager.HasTalent(16); } }
        internal static bool ComboBreakerTpUp { get { return Me.HasAura(118864); } }
        internal static double ComboBreakerTpRemains { get { return Spell.GetMyAuraTimeLeft(118864, Me); } }
        internal static double RisingSunKickDebuffRemains { get { return Spell.GetMyAuraTimeLeft(130320, Me.CurrentTarget); } }
        internal static bool TigerEyeUseDown { get { return !Me.HasAura(116740); } }
        internal static double TigerEyeBrewStacks { get { return Spell.GetAuraStack(Me, 125195); } }

        public static int MaxChi { get { return XITalentManager.HasTalent(8) ? 5 : 4; } } // 
#endregion

        #region Rotations
        internal static Composite WindwalkerSt()
        {
            return new PrioritySelector(
               Spell.Cast("Tiger Palm", ret => TigerPowerRemains <= 3),
               Spell.Cast("Rising Sun Kick"),
              //  new Decorator(ret => RisingSunKickReady, new ActionAlwaysSucceed()),
                Spell.Cast("Tiger Palm", ret => !Me.HasAura(125359) && RisingSunKickDebuffRemains > 1 && Lua.TimeToEnergyCap() > 1),
                Spell.Cast("Fists of Fury", ret => EnergizingBrewDown && Lua.TimeToEnergyCap() > 4 && TigerPowerRemains > 4),
                Spell.Cast("Chi Wave", ret => Lua.TimeToEnergyCap() > 2),
                Spell.Cast("Blackout Kick", ret => ComboBreakerBoKUp),
                Spell.Cast("Tiger Palm", ret => (ComboBreakerTpUp && Lua.TimeToEnergyCap() >= 2) || (ComboBreakerTpRemains <= 2 && ComboBreakerTpUp)),
                Spell.Cast("Jab", ret => MaxChi - Lua.PlayerChi >= 2),
                Spell.Cast("Rushing Jade Wind", ret => RushingJadeWindTalent),
                Spell.Cast("Blackout Kick", ret => !RisingSunKickReady));

        }

        internal static Composite WindwalkerMt()
        {
            return new PrioritySelector(
                Spell.Cast("Rushing Jade Wind", ret => RushingJadeWindTalent),
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
                I.WindwalkerUseHealthStone()
                );
        }


        internal static Composite WindwalkerOffensive()
        {
            return new PrioritySelector(
               Spell.Cast("Tigereye Brew", ret => ((TigerEyeUseDown && TigerEyeBrewStacks == 20) || (Me.GetAllAuras().Any(a => G.AgilityProcList.Contains(a.SpellId)) && TigerEyeUseDown) || (TigerEyeUseDown && Lua.PlayerChi >= 2 && (Me.GetAllAuras().Any(a => G.AgilityProcList.Contains(a.SpellId)) || TigerEyeBrewStacks >= 15 || Me.CurrentTarget.HealthPercent < 20) && RisingSunKickDebuffRemains > 1 && TigerPowerRemains > 1)) && (
                  (SG.Instance.Windwalker.TigereyeBrew == XIEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                   (SG.Instance.Windwalker.TigereyeBrew == XIEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.TigereyeBrew == XIEnum.AbilityTrigger.Always)
                    )),
                //LuaGetSpellCharges()
                //actions+=/chi_brew,if=talent.chi_brew.enabled&chi<=2&(trinket.proc.agility.react|(charges=1&recharge_time<=10)|charges=2|target.time_to_die<charges*10)
               Spell.Cast("Chi Brew", ret => ((TalentChiBrewEnabled && Lua.PlayerChi <= 2) && (Me.GetAllAuras().Any(a => G.AgilityProcList.Contains(a.SpellId)) || (Lua.LuaGetSpellCharges() == 1 && Lua.GetSpellRechargeTime(115399, 45) <= 10) || Lua.LuaGetSpellCharges() == 2 || (Lua.LuaGetSpellCharges() > 1 && Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 10))) && (
                    (SG.Instance.Windwalker.ChiBrew == XIEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Windwalker.ChiBrew == XIEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.ChiBrew == XIEnum.AbilityTrigger.Always)
                    )),
               Spell.Cast("Energizing Brew", ret => Lua.TimeToEnergyCap() > 5 && (
                    (SG.Instance.Windwalker.EnergizingBrew == XIEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Windwalker.EnergizingBrew == XIEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.EnergizingBrew == XIEnum.AbilityTrigger.Always)
                    )),
               Spell.Cast("Invoke Xuen, the White Tiger", ret => (
                    (SG.Instance.Windwalker.Xuen == XIEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Windwalker.Xuen == XIEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.Xuen == XIEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Windwalker.ClassRacials == XIEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Windwalker.ClassRacials == XIEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.ClassRacials == XIEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Windwalker.ClassRacials == XIEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Windwalker.ClassRacials == XIEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.ClassRacials == XIEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Windwalker.ClassRacials == XIEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Windwalker.ClassRacials == XIEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Windwalker.ClassRacials == XIEnum.AbilityTrigger.Always)
                    )),
                    I.UsePotion());
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
                    Spell.Cast("Spear Hand Strike", ret => (SG.Instance.General.InterruptList == XIEnum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == XIEnum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))

                  );
            }
        }
        #endregion


        #region Booleans


        #endregion Booleans

    }
}
