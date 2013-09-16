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


        #endregion Booleans

    }
}
