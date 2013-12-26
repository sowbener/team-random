
#region Bugs & To-Do
/*
 * Current Bugs & To-do's:
 * 

 * 
 */
#endregion

using Bullseye.Helpers;
using Bullseye.Interfaces.GUI;
using Bullseye.Managers;
using JetBrains.Annotations;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using BM = Bullseye.Routines.BsBeastMastery;
using SV = Bullseye.Routines.BsSurvival;
using MM = Bullseye.Routines.BsMarksmanship;
using SG = Bullseye.Interfaces.Settings.BsSettings;
using G = Bullseye.Routines.BsGlobal;
using Spell = Bullseye.Core.BsSpell;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace Bullseye
{
    public class BsMain : CombatRoutine
    {
        [UsedImplicitly]
        public static BsMain Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static readonly Version Revision = new Version(1, 1, 1);
        internal static readonly string BsName = "Bullseye - IR " + Revision;


        public override string Name { get { return BsName; } }
        public override WoWClass Class { get { return WoWClass.Hunter; } }

        #region BotEvents
        public BsMain()
        {
            Instance = this;
            BotEvents.OnBotStopped += BsHotKeyManager.BotEvents_OnBotStopped;
            BotEvents.OnBotStarted += BsHotKeyManager.BotEvents_OnBotStarted;
        }

        private static void StopBot(string reason)
        {
            BsLogger.InitLogW(reason);
            TreeRoot.Stop();
        }
        #endregion

        #region Graphical User Interface
        public override bool WantButton { get { return true; } }
        public override void OnButtonPress()
        {
            new BsInterface().ShowDialog();
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
                BsLogger.DebugLog("Exception thrown in Pulse(): {0}", e);
            }
        }


#endregion

        #region Initialize
        public override void Initialize()
        {
            BsLogger.InitLogO("Team Random presents");
            BsLogger.InitLogF(Name + " by nomnomnom & alxaw.");
            BsLogger.InitLogF("Internal revision: " + Revision);
            BsLogger.InitLogO("Our special thanks go out to:");
            BsLogger.InitLogF("Entire PureRotation / CLU Team.");
            BsLogger.InitLogF("Apoc, Highvoltz & Singular Team.");
            BsLogger.InitLogF("All testers & contributors.");
            BsLogger.InitLogF("A Bsll list is available on the forums.");
            
            /* Update TalentManager */
            try { BsTalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }

            BsLogger.InitLogO("Your specialization is " + BsTalentManager.CurrentSpec + " and your race is " + Me.Race + ".");

            /* Gather required information */
            BsLogger.StatCounter();
            BsLogger.LogTimer(500);
            G.GetBinding();

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
                BsLogger.InitLogF("Initializing pre-combat buff behaviours.");
                _preCombatBuffBehavior = new PrioritySelector(G.InitializePreBuff);
            }
            return null;
        }

        internal Composite RotationSelector()
        {
            if (_combatBehavior == null)
            {
                BsLogger.InitLogF("Initializing combat behaviours.");
                _combatBehavior = null;
            }
            switch (Me.Specialization)
            {
                    case WoWSpec.HunterBeastMastery:
                        if (_combatBehavior == null) { _combatBehavior = BM.InitializeBeastMastery; BsLogger.InitLogO("Beastmastery combat behaviour is initialized."); }
                        break;
                    case WoWSpec.HunterMarksmanship:
                        if (_combatBehavior == null) { _combatBehavior = MM.InitializeMarksmanship ; BsLogger.InitLogO("Marksmanship combat behaviour is initialized."); }
                        break;
                case WoWSpec.HunterSurvival:
                        if (_combatBehavior == null) { _combatBehavior = SV.InitializeSurvival; BsLogger.InitLogO("Survival combat behaviour is initialized."); }
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
