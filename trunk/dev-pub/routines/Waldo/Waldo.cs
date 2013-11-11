
#region Bugs & To-Do
/*
 * Current Bugs & To-do's:
 * 
 * 
 */
#endregion

using Waldo.Helpers;
using Waldo.Interfaces.GUI;
using Waldo.Managers;
using Waldo.Core;
using JetBrains.Annotations;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using A = Waldo.Routines.WaAssassination;
using F = Waldo.Routines.WaCombat;
using S = Waldo.Routines.WaSubtlety;
using G = Waldo.Routines.WaGlobal;
using Movement = Waldo.Helpers.Movement;
using Spell = Waldo.Core.WaSpell;
using Waldo.Core;
using System.Reflection;
using System.Text;
using System.IO;

namespace Waldo
{
    public class WaMain : CombatRoutine
    {
        [UsedImplicitly]
        public static WaMain Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static readonly Version Revision = new Version(1, 0, 6);
        internal static readonly string WaName = "Waldo - IR " + Revision;

        public override string Name { get { return WaName; } }
        public override WoWClass Class { get { return WoWClass.Rogue; } }

        #region BotEvents
        public WaMain()
        {
            Instance = this;
            BotEvents.OnBotStopped += WaHotKeyManager.BotEvents_OnBotStopped;
            BotEvents.OnBotStarted += WaHotKeyManager.BotEvents_OnBotStarted;
        }

        private static void StopBot(string reason)
        {
            WaLogger.InitLogW(reason);
            TreeRoot.Stop();
        }
        #endregion

        #region Graphical User Interface
        public override bool WantButton { get { return true; } }
        public override void OnButtonPress()
        {
            new WaInterface().ShowDialog();
        }
        #endregion

        #region Behaviors
        private Composite _combat, _preCombatBuffBehavior, _restBehavior, _pullBehavior;

        public override Composite CombatBehavior { get { return _combat ?? (_combat = CreateCombat()); } }
        public override Composite PreCombatBuffBehavior { get { return _preCombatBuffBehavior; } }
        public override Composite RestBehavior { get { return _restBehavior; } }
        public override Composite PullBehavior { get { return _pullBehavior; } }
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
                WaLogger.DebugLog("Exception thrown in Pulse(): {0}", e);
            }
        }           
#endregion

        #region Initialize
        public override void Initialize()
        {
            WaLogger.InitLogO("Team Random presents");
            WaLogger.InitLogF(Name + " by nomnomnom & alxaw.");
            WaLogger.InitLogF("Internal revision: " + Revision);
            WaLogger.InitLogO("Our special thanks go out to:");
            WaLogger.InitLogF("Entire PureRotation / CLU Team.");
            WaLogger.InitLogF("Apoc, Highvoltz & Singular Team.");
            WaLogger.InitLogF("All testers & contributors.");
            WaLogger.InitLogF("A Wall list is available on the forums.");
            
            /* Update TalentManager */
            try { WaTalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }

            WaLogger.InitLogO("Your specialization is " + WaTalentManager.CurrentSpec + " and you race is " + Me.Race + ".");


            /* Movement Test Shit */
            //     PullnShit();

            //Cooldownwatcher
            CooldownWatcher.Initialize();
           

            //if (_combatBehavior != null) _combatBehavior = new Sequence(new DecoratorContinue(x => Movement.EnableMovement && (!Me.IsCasting || !Movement.PlayerIsChanneling), Movement.MovingFacingBehavior());
            /* Gather required information */
            WaLogger.StatCounter();
            WaLogger.LogTimer(500);

            /* Start Combat */
            PreBuffSelector();
            CreateCombat();
        }
        #endregion

        #region Buff & Rotation Selector
        internal Composite PreBuffSelector()
        {
            if (_preCombatBuffBehavior == null)
            {
                WaLogger.InitLogF("Initializing pre-combat buff behaviours.");
                _preCombatBuffBehavior = new PrioritySelector(G.InitializePreBuff);
            }
            return null;
        }

        internal static Composite CreateCombat()
        {
            return new Switch<WoWSpec>(ret => Me.Specialization,
                new SwitchArgument<WoWSpec>(WoWSpec.RogueAssassination, A.InitializeAss),
                new SwitchArgument<WoWSpec>(WoWSpec.RogueCombat, F.InitializeCom),
                new SwitchArgument<WoWSpec>(WoWSpec.RogueSubtlety, S.InitializeSub)
                );
        }

 

        public bool PullnShit()
        {
            try
            {
                WaLogger.DebugLog("PullnShit Enabled.");

                if (_pullBehavior != null) _pullBehavior = new Decorator(ret => true, Movement.DefaultPullBehaviour());
                if (_restBehavior != null) _restBehavior = new Decorator(ret => AllowPulse && !Me.IsFlying, Movement.DefaultRestBehaviour());

                return true;
            }
            catch (Exception ex)
            {
                WaLogger.DebugLog("[PullnShit] Exception was thrown: {0}", ex);
                return false;
            }
        }


        private static bool AllowPulse { get { return !StyxWoW.Me.Mounted; } }

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
