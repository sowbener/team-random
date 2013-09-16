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
    class BsBeastMastery
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeBeastMastery
        {
            get
            {
                return new PrioritySelector(
                        new Decorator(ret => SG.Instance.General.CheckTreePerformance, BsLogger.TreePerformance("InitializeBeastMastery")),
                        new Decorator(ret => (BsHotKeyManager.IsPaused || !U.DefaultCheck), new ActionAlwaysSucceed()),
                        new Decorator(ret => !SG.Instance.General.CheckPreCombatHk, G.InitializeOnKeyActions()),
                        new Decorator(ret => SG.Instance.General.CheckABsancedLogging, BsLogger.ABsancedLogging),
                        G.InitializeCaching(),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BsEnum.Mode.Auto,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BeastmasteryDefensive()),
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckInterrupts && U.CanInterrupt, BeastmasteryInterrupts()),
                                        BeastmasteryUtility(),
                                        I.BeastmasteryUseItems(),
                                        BeastmasteryOffensive(),
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckAoE && (U.NearbyAttackableUnitsCount >= 2 || U.IsAoETarget), BeastmasteryMt()),
                                        BeastmasterySt())),
                        new Decorator(ret => !Spell.IsGlobalCooldown() && SH.Instance.ModeSelection == BsEnum.Mode.Hotkey,
                                new PrioritySelector(
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckAutoAttack, Lua.StartAutoAttack),
                                        new Decorator(ret => Me.HealthPercent < 100, BeastmasteryDefensive()),
                                        new Decorator(ret => SG.Instance.Beastmastery.CheckInterrupts && U.CanInterrupt, BeastmasteryInterrupts()),
                                        BeastmasteryUtility(),
                                        new Decorator(ret => BsHotKeyManager.IsCooldown, 
                                        new PrioritySelector(
                                        I.BeastmasteryUseItems(),
                                        BeastmasteryOffensive())),
                                        new Decorator(ret => BsHotKeyManager.IsAoe && SG.Instance.Beastmastery.CheckAoE && (U.NearbyAttackableUnitsCount >= 3 || U.IsAoETarget), BeastmasteryMt()),
                                        BeastmasterySt())));
            }
        }
        #endregion

        #region Rotations
        internal static Composite BeastmasterySt()
        {
            return new PrioritySelector(
                 );
        }


        internal static Composite BeastmasteryMt()
        {
            return new PrioritySelector(
                       );
        }

        internal static Composite BeastmasteryDefensive()
        {
            return new PrioritySelector(     
                I.BeastmasteryUseHealthStone()
                );
        }


        internal static Composite BeastmasteryOffensive()
        {
            return new PrioritySelector(
                Spell.Cast("Berserking", ret => Me.Race == WoWRace.Troll && (
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Blood Fury", ret => Me.Race == WoWRace.Orc && (
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                Spell.Cast("Rocket Barrage", ret => Me.Race == WoWRace.Goblin && (
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBossDummy && U.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.ClassRacials == BsEnum.AbilityTrigger.Always)
                    )),
                    I.UsePotion());
        }

        internal static Composite BeastmasteryUtility()
        {
            return new PrioritySelector(
                 
                );
        }


  

        internal static Composite BeastmasteryInterrupts()
        {
            return new PrioritySelector(
                       );
        }
        #endregion

        #region Booleans

  
        #endregion Booleans

        }
}
