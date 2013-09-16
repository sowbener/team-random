/*
 * Current Bugs & To-do's:
 * 
 * Implement tierset detection
 * Exception AoE rule for Garalon & Twin Consorts.
 * Investigate HP scan for all party members (Rallying Cry).
 * Look at ragedump with low-enemy HP.
 * Implement all possible talentbuilds.
 * Create de-sync option for trinkets/hands.
 * 
 */

using JetBrains.Annotations;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using YBMoP_BT_Warrior.Helpers;
using YBMoP_BT_Warrior.Interfaces.GUI;
using YBMoP_BT_Warrior.Managers;
using A = YBMoP_BT_Warrior.Routines.YBArms;
using F = YBMoP_BT_Warrior.Routines.YBFury;
using G = YBMoP_BT_Warrior.Routines.YBGlobal;
using P = YBMoP_BT_Warrior.Routines.YBProt;

namespace YBMoP_BT_Warrior
{
    public class YBMain : CombatRoutine
    {
        [UsedImplicitly]
        public static YBMain Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static readonly Version Revision = new Version(2, 1, 3);
        internal static readonly string YBName = "YourBuddy MoP BT IR " + Revision + " - Warrior";

        public override string Name { get { return YBName; } }
        public override WoWClass Class { get { return WoWClass.Warrior; } }

        #region YBMoP BT - BotEvents
        public YBMain()
        {
            Instance = this;
            BotEvents.OnBotStopped += HotKeyManager.BotEvents_OnBotStopped;
            BotEvents.OnBotStarted += HotKeyManager.BotEvents_OnBotStarted;
        }

        private static void StopBot(string reason)
        {
            YBLogger.InitLogW(reason);
            TreeRoot.Stop();
        }
        #endregion

        #region YBMoP BT - Graphical User Interface
        public override bool WantButton { get { return true; } }
        public override void OnButtonPress()
        {
            switch (StyxWoW.Me.Specialization)
            {
                case WoWSpec.WarriorArms:
                    YBLogger.InitLogF(Name + " Graphical User Interface.");
                    YBLogger.InitLogO("Arms configuration GUI isn't ready yet.");
                    break;
                case WoWSpec.WarriorFury:
                    YBLogger.InitLogF(Name + " Graphical User Interface.");
                    YBLogger.InitLogO("Fury configuration GUI opened.");
                    new YBFuryGui().ShowDialog();
                    break;
                case WoWSpec.WarriorProtection:
                    YBLogger.InitLogF(Name + " Graphical User Interface.");
                    YBLogger.InitLogO("Protection configuration GUI opened.");
                    new YBProtGui().ShowDialog();
                    break;
            }
        }
        #endregion

        #region YBMoP BT - Behaviors
        private Composite _combatBehavior, _preCombatBuffBehavior;

        public override Composite CombatBehavior        { get { return _combatBehavior; } }
        public override Composite PreCombatBuffBehavior { get { return _preCombatBuffBehavior; } }
        #endregion

        #region YBMoP BT - Initialize
        public override void Initialize()
        {
            YBLogger.InitLogO("Team Random presents");
            YBLogger.InitLogF(Name + " by nomnomnom & alxaw.");
            YBLogger.InitLogF("Internal revision: " + Revision);
            YBLogger.InitLogO("Our special thanks go out to:");
            YBLogger.InitLogF("Entire PureRotation / CLU Team.");
            YBLogger.InitLogF("Apoc, Highvoltz & Singular Team.");
            YBLogger.InitLogF("All testers & contributors.");
            YBLogger.InitLogF("A full list is available on the forums.");
            
            /* Update TalentManager */
            try { TalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }

            YBLogger.InitLogO("Your specialization is " + TalentManager.CurrentSpec + " and you race is " + Me.Race + ".");

            /* Write information to LogFile */
            YBLogger.WriteToLogFile();

            /* Start Combat */
            PreBuffSelector();
            RotationSelector();
        }
        #endregion

        #region YBMoP BT - Buff & Rotation Selector
        internal Composite PreBuffSelector()
        {
            if (_preCombatBuffBehavior == null)
            {
                YBLogger.InitLogF("Initializing pre-combat buff behaviours.");
                _preCombatBuffBehavior = new PrioritySelector(G.InitializePreBuff);
            }
            return null;
        }

        internal Composite RotationSelector()
        {
            if (_combatBehavior == null)
            {
                YBLogger.InitLogF("Initializing combat behaviours.");
                _combatBehavior = null;
            }
            switch (Me.Specialization)
            {
                    case WoWSpec.WarriorArms:
                        if (_combatBehavior == null) { _combatBehavior = A.InitializeArms; YBLogger.InitLogO("Arms combat behaviour is initialized."); }
                        break;
                    case WoWSpec.WarriorFury:
                        if (_combatBehavior == null) { _combatBehavior = F.InitializeFury; YBLogger.InitLogO("Fury combat behaviour is initialized."); }
                        break;
                    case WoWSpec.WarriorProtection:
                        if (_combatBehavior == null) { _combatBehavior = P.InitializeProt; YBLogger.InitLogO("Protection combat behaviour is initialized."); }
                        break;
                    default:
                        StopBot("Current class or specialization is not supported! Stopping HonorBuddy.");
                    break;
            }
            return null;
        }
        #endregion

        #region YBMoP BT - LockSelector
        [UsedImplicitly]
        private class LockSelector : PrioritySelector
        {
            public LockSelector(params Composite[] children)
                : base(children)
            {
            }
            public override RunStatus Tick(object context)
            {
                using (StyxWoW.Memory.AcquireFrame())
                {
                    return base.Tick(context);
                }
            }
        }
        #endregion
    }
}
