using CommonBehaviors.Actions;
using Bubbleman.Core;
using Bubbleman.Helpers;
using Bubbleman.Managers;
using System.Linq;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = Bubbleman.Routines.BMGlobal;
using I = Bubbleman.Core.BMItem;
using Lua = Bubbleman.Helpers.BMLua;
using T = Bubbleman.Managers.BMTalentManager;
using SG = Bubbleman.Interfaces.Settings.BMSettings;
using SH = Bubbleman.Interfaces.Settings.BMSettingsH;
using Spell = Bubbleman.Core.BMSpell;
using U = Bubbleman.Core.BMUnit;
using Styx.CommonBot;
using Bubbleman.Interfaces.Settings;

namespace Bubbleman.Routines
{
    class BMRetribution
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeRetribution
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, BMLogger.TreePerformance("InitializeRetribution")),
                        new Decorator(ret => (BMHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.General.CheckABMancedLogging, BMLogger.ABMancedLogging),
                        G.InitializeCaching(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BMEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Retribution.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, RetributionDefensive()),
                                        new Decorator(ret => SG.Instance.Retribution.CheckInterrupts, RetributionInterrupts()),
                                        RetributionUtility(),
                                        I.RetributionUseItems(),
                                        RetributionOffensive(),
                                        new Decorator(ret => SG.Instance.Retribution.CheckAoE && U.NearbyAttackableUnitsCount > 3, RetributionMt()),
                                            RetributionSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BMEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Retribution.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, RetributionDefensive()),
                                        new Decorator(ret => SG.Instance.Retribution.CheckInterrupts, RetributionInterrupts()),
                                        RetributionUtility(),
                                        new Decorator(ret => BMHotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.RetributionUseItems(),
                                                        RetributionOffensive())),
                                        new Decorator(ret => BMHotKeyManager.IsAoe, RetributionMt()),
                                        RetributionSt())));
            }
        }
        #endregion

        #region BoolsTemp

        internal static bool TalentChiBrewEnabled { get { return BMTalentManager.HasTalent(9); } }


        private static BMSettingsRE Retribution { get { return SG.Instance.Retribution; } }

       #endregion

        #region Rotations
        internal static Composite RetributionSt()
        {
            return new PrioritySelector(
                    );

        }

        internal static Composite RetributionMt()
        {
            return new PrioritySelector(
               );
        }



        internal static Composite RetributionDefensive()
        {
            return new PrioritySelector(
                I.RetributionUseHealthStone()
                );
        }


        internal static Composite RetributionOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Retribution.ClassRacials == BMEnum.AbilityTrigger.Always)
                    )),
                    I.UsePotion());
        }

        internal static Composite RetributionUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite RetributionInterrupts()
        {
            {
                 return new PrioritySelector(
                   new Throttle(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("xxx", ret => (SG.Instance.General.InterruptList == BMEnum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == BMEnum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))

                  );
            }
        }
        #endregion


        #region Booleans


        #endregion Booleans

    }
}
