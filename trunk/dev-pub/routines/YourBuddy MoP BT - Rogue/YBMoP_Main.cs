/*
 * Current Bugs & To-do's
 * Skull Banner gets popped even when there already is one.
 * Exception AoE rule for Garalon & Twin Consorts. 
 */
using JetBrains.Annotations;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using YBMoP_BT_Rogue.Helpers;
using YBMoP_BT_Rogue.Interfaces.GUI;
using YBMoP_BT_Rogue.Managers;
using F = YBMoP_BT_Rogue.Routines.YBAss;
using G = YBMoP_BT_Rogue.Routines.YBGlobal;
using P = YBMoP_BT_Rogue.Routines.YBCom;

namespace YBMoP_BT_Rogue
{
    public class YBMain : CombatRoutine
    {
        [UsedImplicitly]
        public static YBMain Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static readonly Version Revision = new Version(0, 0, 1);
        internal static readonly string YBName = "YourBuddy MoP BT IR " + Revision + " - Rogue";

        public override string Name { get { return YBName; } }
        public override WoWClass Class { get { return WoWClass.Rogue; } }

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
                case WoWSpec.RogueSubtlety:
                    YBLogger.InitLogF(Name + " Graphical User Interface.");
                    YBLogger.InitLogO("Subtlety is not supported yet!");
                    break;
                case WoWSpec.RogueAssassination:
                    YBLogger.InitLogF(Name + " Graphical User Interface.");
                    YBLogger.InitLogO("Assassination configuration GUI opened.");
                    new YBAssGui().ShowDialog();
                    break;
                case WoWSpec.RogueCombat:
                    YBLogger.InitLogF(Name + " Graphical User Interface.");
                    YBLogger.InitLogO("Combat configuration GUI opened.");
                    new YBComGui().ShowDialog();
                    break;
            }
        }
        #endregion

        #region YBMoP BT - Behaviors
        private Composite _combatBehavior, _combatBuffBehavior, _preCombatBuffBehavior;

        public override Composite CombatBehavior        { get { return _combatBehavior; } }
        public override Composite PreCombatBuffBehavior { get { return _preCombatBuffBehavior; } }
        public override Composite CombatBuffBehavior    { get { return _combatBuffBehavior; } }
        #endregion

        #region YBMoP BT - Initialize
        public override void Initialize()
        {
            YBLogger.InitLogO("Team Random presents");
            YBLogger.InitLogF(Name + " by nomnomnom & alxaw");
            YBLogger.InitLogF("Internal revision: " + Revision);
            YBLogger.InitLogW(" ");
            YBLogger.InitLogF("Thanks to the following developers:");
            YBLogger.InitLogO("Apoc, Highvoltz, Stormchasing, Weischbier & Wulf!");
            YBLogger.InitLogF("Thanks to the following testers:");
            YBLogger.InitLogO("Schm0, Redhood, uzi2k4, panYama, Pipedreams");
            YBLogger.InitLogW(" ");

            /* Update TalentManager */
            try { TalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }

            YBLogger.InitLogF("Your current specialization is " + TalentManager.CurrentSpec);
            YBLogger.InitLogF("Your current race is " + Me.Race);

            /* Start Combat */
            if (_preCombatBuffBehavior == null)
                _preCombatBuffBehavior = new PrioritySelector(G.InitializePreBuff);

            if (_combatBuffBehavior == null)
                _combatBuffBehavior = new PrioritySelector();

            RotationSelector();
        }
        #endregion

        #region YBMoP BT - Rotation Selector
        internal Composite RotationSelector()
        {
            if (_combatBehavior == null)
            {
                YBLogger.InitLogF("Rebuilding your specialization");
                _combatBehavior = null;
            }
            switch (Me.Specialization)
            {
                    case WoWSpec.RogueSubtlety:
                        StopBot("Current specialization is Arms. Not supported!");
                        break;
                    case WoWSpec.RogueAssassination:
                        if (_combatBehavior == null) { _combatBehavior = F.InitializeAss; YBLogger.InitLogW("Current specialization is Assassination."); }
                        break;
                    case WoWSpec.RogueCombat:
                        if (_combatBehavior == null) { _combatBehavior = P.InitializeCom; YBLogger.InitLogW("Current specialization is Combat"); }
                        break;
                    default:
                        StopBot("Current class or specialization is not supported!");
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
