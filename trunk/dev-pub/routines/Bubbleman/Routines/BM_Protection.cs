using CommonBehaviors.Actions;
using Bubbleman.Core;
using Bubbleman.Helpers;
using Bubbleman.Managers;
using Bubbleman.Interfaces;
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
    class BMProtection
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeProtection
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, BMLogger.TreePerformance("InitializeProtection")),
                        new Decorator(ret => (BMHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => SG.Instance.General.CheckABMancedLogging, BMLogger.ABMancedLogging),
                        G.InitializeCaching(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BMEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Protection.CheckAutoAttack, Lua.StartAutoAttack), 
                                            ProtectionDefensive(),
                                        new Decorator(ret => SG.Instance.Protection.CheckInterrupts && U.CanInterrupt, ProtectionInterrupts()),
                                        ProtectionUtility(),
                                        I.ProtectionUseItems(),
                                        ProtectionOffensive(),
                                        new Decorator(ret => SG.Instance.Protection.CheckAoE && U.NearbyAttackableUnitsCount > 2, ProtectionMt()),
                                            ProtectionSt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BMEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Protection.CheckAutoAttack, Lua.StartAutoAttack),
                                        ProtectionDefensive(),
                                        new Decorator(ret => SG.Instance.Protection.CheckInterrupts && U.CanInterrupt, ProtectionInterrupts()),
                                        ProtectionUtility(),
                                        new Decorator(ret => BMHotKeyManager.ElusiveBrew, new PrioritySelector(Spell.Cast("Elusive Brew"))),
                                        new Decorator(ret => BMHotKeyManager.IsCooldown,
                                                new PrioritySelector(
                                                        I.ProtectionUseItems(),
                                                        ProtectionOffensive())),
                                        new Decorator(ret => BMHotKeyManager.IsAoe, ProtectionMt()),
                                        ProtectionSt())));
            }
        }
        #endregion

        #region BoolsTemp

        internal static bool TalentJadeWindEnabled { get { return BMTalentManager.HasTalent(16); } }

        private static BMSettingsPR ProtectionSettings { get { return SG.Instance.Protection; } }




        #endregion

        #region Rotations

        internal static Composite ChiBuilder()
        {
            return new PrioritySelector(
                );

        }

       

        internal static Composite ProtectionSt()
        {
            return new PrioritySelector(
                );

        }

        internal static Composite ProtectionMt()
        {
            return new PrioritySelector(
                  );
        }


        private static Composite HandleHealingCooldowns()
        {
            return new PrioritySelector(
                   // Spell.CastOnGround("Healing Sphere", ret => Me.Location, ret => NeedHealingSphere),
                    I.ProtectionUseHealthStone());
        }


        internal static Composite ProtectionDefensive()
        {
                        return new PrioritySelector(
                );
        }


        internal static Composite ProtectionOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Protection.ClassRacials == BMEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.ClassRacials == BMEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Protection.ClassRacials == BMEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Protection.ClassRacials == BMEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.ClassRacials == BMEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Protection.ClassRacials == BMEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Protection.ClassRacials == BMEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Protection.ClassRacials == BMEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Protection.ClassRacials == BMEnum.AbilityTrigger.Always)
                    )),
                    I.UsePotion());
        }

        internal static Composite ProtectionUtility()
        {
            return new PrioritySelector(
                );
        }

        internal static Composite ProtectionInterrupts()
        {
            {
                  return new PrioritySelector(
                   new Throttle(1, System.TimeSpan.FromMilliseconds(G._random.Next(SG.Instance.General.InterruptStart, SG.Instance.General.InterruptEnd)), RunStatus.Failure,
                    Spell.Cast("Spear Hand Strike", ret => (SG.Instance.General.InterruptList == BMEnum.InterruptList.MoP && (G.InterruptListMoP.Contains(Me.CurrentTarget.CurrentCastorChannelId()))) ||
                    (SG.Instance.General.InterruptList == BMEnum.InterruptList.NextExpensionPack && (G.InterruptListTBA.Contains(Me.CurrentTarget.CurrentCastorChannelId())))))
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
