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

        #region Rotations
        internal static Composite BrewmasterSt()
        {
            return new PrioritySelector(
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
                I.BrewmasterUseHealthStone()
                );
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


        #endregion Booleans

    }
}
