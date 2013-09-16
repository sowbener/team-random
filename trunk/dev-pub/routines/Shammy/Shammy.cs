
#region Bugs & To-Do
/*
 * Current Bugs & To-do's:
 * 
 * 
 */
#endregion

using Shammy.Helpers;
using Shammy.Interfaces.GUI;
using Shammy.Managers;
using JetBrains.Annotations;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using A = Shammy.Routines.SmEnhancement;
using F = Shammy.Routines.SmElemental;
using G = Shammy.Routines.SmGlobal;
using Spell = Shammy.Core.SmSpell;

namespace Shammy
{
    public class SmMain : CombatRoutine
    {
        [UsedImplicitly]
        public static SmMain Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static readonly Version Revision = new Version(1, 0, 6);
        internal static readonly string SmName = "Shammy IR " + Revision;

        public override string Name { get { return SmName; } }
        public override WoWClass Class { get { return WoWClass.Shaman; } }

        #region BotEvents
        public SmMain()
        {
            Instance = this;
            BotEvents.OnBotStopped += SmHotKeyManager.BotEvents_OnBotStopped;
            BotEvents.OnBotStarted += SmHotKeyManager.BotEvents_OnBotStarted;
        }

        private static void StopBot(string reason)
        {
            SmLogger.InitLogW(reason);
            TreeRoot.Stop();
        }
        #endregion

        #region Graphical User Interface
        public override bool WantButton { get { return true; } }
        public override void OnButtonPress()
        {
            new SmInterface().ShowDialog();
        }
        #endregion

        #region Behaviors
        private Composite _combatBehavior, _preCombatBuffBehavior;

        public override Composite CombatBehavior        { get { return _combatBehavior; } }
        public override Composite PreCombatBuffBehavior { get { return _preCombatBuffBehavior; } }
        #endregion

        #region Pulse

       public override void Pulse()
        {
            try
            {
                // Double cast shit
                Spell.PulseDoubleCastEntries();
            }
            catch (Exception e)
            {
                SmLogger.DebugLog("Exception thrown in Pulse(): {0}", e);
            }
        }           
#endregion

        #region Initialize
        public override void Initialize()
        {
            SmLogger.InitLogO("Team Random presents");
            SmLogger.InitLogF(Name + " by nomnomnom & alxaw.");
            SmLogger.InitLogF("Internal revision: " + Revision);
            SmLogger.InitLogO("Our special thanks go out to:");
            SmLogger.InitLogF("Entire PureRotation / CLU Team.");
            SmLogger.InitLogF("Apoc, Highvoltz & Singular Team.");
            SmLogger.InitLogF("All testers & contributors.");
            SmLogger.InitLogF("A Smll list is available on the forums.");
            
            /* Update TalentManager */
            try { SmTalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }

            SmLogger.InitLogO("Your specialization is " + SmTalentManager.CurrentSpec + " and you race is " + Me.Race + ".");

            /* Write information to LogFile */
            SmLogger.WriteToLogFile();

            /* Start Combat */
            PreBuffSelector();
            RotationSelector();
        }
        #endregion

        #region Buff & Rotation Selector
        internal Composite PreBuffSelector()
        {
            if (_preCombatBuffBehavior == null)
            {
                SmLogger.InitLogF("Initializing pre-combat buff behaviours.");
                _preCombatBuffBehavior = new PrioritySelector(G.InitializePreBuff);
            }
            return null;
        }

        internal Composite RotationSelector()
        {
            if (_combatBehavior == null)
            {
                SmLogger.InitLogF("Initializing combat behaviours.");
                _combatBehavior = null;
            }
            switch (Me.Specialization)
            {
                    case WoWSpec.ShamanEnhancement:
                        if (_combatBehavior == null) { _combatBehavior = A.InitializeEnhancement; SmLogger.InitLogO("Enhancement combat behaviour is initialized."); }
                        break;
                    case WoWSpec.ShamanElemental:
                        if (_combatBehavior == null) { _combatBehavior = F.InitializeElemental; SmLogger.InitLogO("Elemental combat behaviour is initialized."); }
                        break;
                    default:
                        StopBot("Current class or specialization is not supported! Stopping HonorBuddy.");
                    break;
            }
            return null;
        }
        #endregion

        #region Forced LockSelector
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
