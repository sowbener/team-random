using CommonBehaviors.Actions;
using Xiaolin.Core;
using Xiaolin.Helpers;
using Xiaolin.Managers;
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
                                        new Decorator(ret => SG.Instance.Windwalker.CheckAoE && (U.NearbyAttackableUnitsCount >= 2), WindwalkerMt()),
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
                                        new Decorator(ret => XIHotKeyManager.IsAoe && SG.Instance.Windwalker.CheckAoE && U.AttackableMeleeUnitsCount >= 2, WindwalkerMt()),
                                        WindwalkerSt())));
            }
        }
        #endregion

        #region BoolsTemp

        internal static bool RisingSunKickReady { get { return !Styx.WoWInternals.WoWSpell.FromId(107428).Cooldown; } }
        internal static bool EnergizingBrewDown { get { return !Me.HasAura(115288); } }
        internal static double TigerPowerRemains { get { return Spell.GetMyAuraTimeLeft(125359, Me); } }
        internal static bool ComboBreakerBoKUp { get { return Me.HasAura(116768); } }
        internal static bool TalentAscensionEnabled { get { return XITalentManager.HasTalent(8); } }
        internal static bool ComboBreakerTpUp { get { return Me.HasAura(118864); } }
        internal static double ComboBreakerTpRemains { get { return Spell.GetMyAuraTimeLeft(118864, Me); } }
        internal static double RisingSunKickDebuffRemains { get { return Spell.GetMyAuraTimeLeft(130320, Me.CurrentTarget); } }
        internal static bool TigerEyeUseDown { get { return !Me.HasAura(116740); } }
#endregion

        #region Rotations
        internal static Composite WindwalkerSt()
        {
            return new PrioritySelector(
               Spell.Cast("Tigereye Brew", ret => TigerEyeUseDown && Spell.GetSpellCooldown(107428).TotalSeconds < 1 && Lua.PlayerChi >= 2 && RisingSunKickDebuffRemains > 1 && TigerPowerRemains > 1),
              // actions+=/energizing_brew,if=energy.time_to_max>5
              Spell.Cast("Energizing Brew", ret => Lua.TimeToEnergyCap() > 5),
              Spell.Cast("Invoke Xuen, the White Tiger"),
               Spell.Cast("Tiger Palm", ret => TigerPowerRemains <= 3),
               Spell.Cast("Rising Sun Kick"),
              //  new Decorator(ret => RisingSunKickReady, new ActionAlwaysSucceed()),
                Spell.Cast("Tiger Palm", ret => !Me.HasAura(125359) && RisingSunKickDebuffRemains > 1 && Lua.TimeToEnergyCap() > 1),
                Spell.Cast("Fists of Fury", ret => EnergizingBrewDown && Lua.TimeToEnergyCap() > 4 && TigerPowerRemains > 4),
                Spell.Cast("Chi Wave", ret => Lua.TimeToEnergyCap() > 2),
                Spell.Cast("Blackout Kick", ret => ComboBreakerBoKUp),
                Spell.Cast("Tiger Palm", ret => (ComboBreakerTpUp && Lua.TimeToEnergyCap() >= 2) || (ComboBreakerTpRemains <= 2 && ComboBreakerTpUp)),
                Spell.Cast("Jab", ret => Lua.PlayerChi < 3),
         //       Spell.PreventDoubleCast("Jab", 0.5, ret => !TalentAscensionEnabled && Lua.PlayerChi <= 2),
                Spell.Cast("Blackout Kick", ret => !RisingSunKickReady));

        }

        internal static Composite WindwalkerMt()
        {
            return new PrioritySelector(
                  );
        }



        internal static Composite WindwalkerDefensive()
        {
            return new PrioritySelector(
                I.WindwalkerUseHealthStone()
                );
        }


        internal static Composite WindwalkerOffensive()
        {
            return new PrioritySelector(
                //actions+=/tigereye_brew,if=buff.tigereye_brew_use.down&cooldown.rising_sun_kick.remains=0&chi>=2&target.debuff.rising_sun_kick.remains&buff.tiger_power.remains
                
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
                  );
            }
        }
        #endregion


        #region Booleans


        #endregion Booleans

    }
}
