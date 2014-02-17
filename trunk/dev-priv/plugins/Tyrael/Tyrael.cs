using CommonBehaviors.Actions;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Profiles;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Windows.Forms;
using System.Windows.Media;
using Tyrael.Shared;

namespace Tyrael
{
    public class Tyrael : BotBase
    {
        public static readonly Version Revision = new Version(5, 6, 1);
        public static LocalPlayer Me { get { return StyxWoW.Me; } }
        public static bool IsPaused;

        private static Composite _root;
        private static PulseFlags _pulseFlags;

        #region Overrides
        public override string Name
        {
            get { return "Tyrael"; }
        }

        public override PulseFlags PulseFlags
        {
            get { return !TyraelUtilities.IsTyraelPaused ? _pulseFlags : PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel; }
        }

        public override Form ConfigurationForm
        {
            get { return new TyraelInterface(); }
        }
		
		public override bool RequiresProfile
        {
            get { return false; }
        }

        public override Composite Root
        {
            get { return _root ?? (_root = CreateRoot()); }
        }

        /// <summary>
        /// Runs when the bot starts. Loads several Tyrael functions and sets some basic settings.
        /// </summary>
        public override void Start()
        {
            try
            {
                InitializeComponents();
                InitializePlugins();

                Logging.Write(Colors.White, "------------------------------------------");
                Logging.Write(Colors.DodgerBlue, "[Tyrael] {0} is loaded.", RoutineManager.Current.Name);
                Logging.Write(Colors.DodgerBlue, "[Tyrael] {0} {1} has been started.", Name, Revision);
                Logging.Write(Colors.DodgerBlue, "\r\n");
                Logging.Write(Colors.DodgerBlue, "[Tyrael] Special thanks to the following persons:");
                Logging.Write(Colors.DodgerBlue, "[Tyrael] PureRotation Team");
                Logging.Write(Colors.White, "-------------------------------------------\r\n");

                if (!GlobalSettings.Instance.UseFrameLock && !TyraelSettings.Instance.UseSoftLock)
                {
                    Logging.Write(Colors.Red, "[Tyrael] HardLock and SoftLock are both disabled - For optimal DPS/HPS I suggest enabling ONE of them.");
                }
            }
            catch (Exception startexception)
            {
                Logging.WriteDiagnostic("[Tyrael] Error: {0}", startexception);
            }
        }

        /// <summary>
        /// Runs when the bot stops. Unloads several Tyrael functions and resets basic settings.
        /// </summary>
        public override void Stop()
        {
            try
            {
                Logging.Write(Colors.White, "------------------------------------------");
                Logging.Write(Colors.DodgerBlue, "[Tyrael] Shutdown - Performing required actions.");
                StopComponents();
                Logging.Write(Colors.DodgerBlue, "[Tyrael] Shutdown - Complete.");
                Logging.Write(Colors.White, "-------------------------------------------\r\n");
            }
            catch (Exception stopexception)
            {
                Logging.WriteDiagnostic("[Tyrael] Error: {0}", stopexception);
            }
        }
        #endregion

        #region Privates & Internals
        /// <summary>
        /// SanityCheck - Checks if we are actually ingame and able to control the character.
        /// </summary>
        /// <returns>true / false</returns>
        private static bool SanityCheckCombat()
        {
            return TyraelUtilities.IsViable(Me) && (StyxWoW.Me.Combat || TyraelSettings.Instance.CheckHealingMode) && !Me.IsDead;
        }

        /// <summary>
        /// Actual Root Composite - Within this the RoutineManager runs the routines behaviors.
        /// </summary>
        /// <returns>Routines Behaviors</returns>
        private static Composite CreateRoot()
        {
            return new PrioritySelector(
                new Decorator(ret => IsPaused, new ActionAlwaysSucceed()),
                new Decorator(ret => SanityCheckCombat(),
                    SelectLockMethod(
                        RoutineManager.Current.HealBehavior,
                        RoutineManager.Current.CombatBuffBehavior ?? new ActionAlwaysFail(),
                        RoutineManager.Current.CombatBehavior)),
                    RoutineManager.Current.PreCombatBuffBehavior,
                    RoutineManager.Current.RestBehavior);
        }

        /// <summary>
        /// Initializes all the required components for Tyrael to run.
        /// </summary>
        private static void InitializeComponents()
        {
            try
            {
                TyraelUpdater.CheckForUpdate();

                TyraelSettings.Instance.Load();

                TyraelUtilities.ClickToMove();
                TyraelUtilities.StatCounter();
                TyraelUtilities.RegisterHotkeys();

                ProfileManager.LoadEmpty();

                GlobalSettings.Instance.LogoutForInactivity = false;
                TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;
            }
            catch (Exception initializecomponentsexception)
            {
                Logging.WriteDiagnostic("[Tyrael] Error: {0}", initializecomponentsexception);
            }
        }

        /// <summary>
        /// Stops components for Tyrael.
        /// </summary>
        private static void StopComponents()
        {
            try
            {
                TyraelUtilities.EnableClickToMove();
                TyraelUtilities.RemoveHotkeys();

                GlobalSettings.Instance.LogoutForInactivity = true;
                TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;
            }
            catch (Exception stopcomponentsexception)
            {
                Logging.WriteDiagnostic("[Tyrael] Error: {0}", stopcomponentsexception);
            }
        }

        /// <summary>
        /// Initializes the proper plugins based on the Setting CheckPluginPulsing.
        /// </summary>
        internal static void InitializePlugins()
        {
            try
            {
                if (TyraelSettings.Instance.CheckPluginPulsing)
                {
                    _pulseFlags = PulseFlags.Plugins | PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel;
                }
                else
                {
                    _pulseFlags = PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel;
                }
            }
            catch (Exception initializepluginsexception)
            {
                Logging.WriteDiagnostic("[Tyrael] Error: {0}", initializepluginsexception);
            }
        }
        #endregion

        #region Softlock
        private static Composite SelectLockMethod(params Composite[] children)
        {
            return TyraelSettings.Instance.UseSoftLock ? new FrameLockSelector(children) : new PrioritySelector(children);
        }

        public class FrameLockSelector : PrioritySelector
        {
            public FrameLockSelector(params Composite[] children)
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
