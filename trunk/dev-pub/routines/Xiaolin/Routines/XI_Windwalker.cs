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

        #region Rotations
        internal static Composite WindwalkerSt()
        {
            return new PrioritySelector(
                );

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
