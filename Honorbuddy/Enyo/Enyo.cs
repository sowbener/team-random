using CommonBehaviors.Actions;
using Enyo.Shared;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Profiles;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.Pathing;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Windows.Forms;

namespace Enyo
{
    class Enyo : BotBase
    {
        /* Public defined */
        /* Version and Settings are like this: X.X.X.X-RX for releases - X.X.X.X-BX for betas */
		public static readonly string Revision = "0.0.0.5-R1";
        public static readonly string Settings = "0.0.0.6-R1";

        public static bool IsPaused;

        /* Private defined */
        private static Composite _root;
        private static PulseFlags _pulseFlags;

        private const string WoWVersion = "6.2 (Warlords of Draenor)";

        /// <summary>
        ///     Returns Me as StyxWoW.Me.
        /// </summary>
        public static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        /// <summary>
        ///     The Navigation Provider.
        /// </summary>
        public static NavigationProvider OldNavigationProvider
        {
            get; set;
        }

        /// <summary>
        ///     The Enyo Navigation Provider.
        /// </summary>
        public static NavigationProvider EnyoNavigationProvider
        {
            get; set;
        }

        #region Overrides
        /// <summary>
        ///     Returns the Name of the BotBase.
        /// </summary>
        public override string Name
        {
            get { return "Enyo"; }
        }

        /// <summary>
        ///     Returns the requires PulseFlags for the BotBase.
        /// </summary>
        public override PulseFlags PulseFlags
        {
            get { return !Hotkeys.IsEnyoPaused ? _pulseFlags : PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel; }
        }

        /// <summary>
        ///     Returns the GUI of the BotBase on button press.
        /// </summary>
        public override Form ConfigurationForm
        {
            get { return new Interface(); }
        }
		
        /// <summary>
        ///     Forcefully disable the profile requirements.
        /// </summary>
		public override bool RequiresProfile
        {
            get { return false; }
        }

        /// <summary>
        ///     Creating the Root Composite - The magic happens within this composite.
        /// </summary>
        public override Composite Root
        {
            get { return _root ?? (_root = CreateRoot()); }
        }

        /// <summary>
        ///     Runs when the BotBase is started - Initializes several basic functions.
        /// </summary>
        public override void Start()
        {
            try
            {
                InitializeComponents();
                InitializeNavigator();
                InitializePlugins();
                
                // ReSharper disable once CSharpWarnings::CS0618
                if (!GlobalSettings.Instance.UseFrameLock && !BotSettings.Instance.UseSoftLock)
                {
                    Logger.PrintLog("------------------------------------------");
                    Logger.PrintLog("HardLock and SoftLock are both disabled - For optimal DPS/HPS I suggest enabling ONE of them.");
                }

                Logger.PrintLog("------------------------------------------");
                Logger.PrintLog("{0} {1} has been started.", Name, Revision);
                Logger.PrintLog("{0} is loaded.", RoutineManager.Current.Name);
                Logger.PrintLog("Supported World of Warcraft version: " + WoWVersion + ".\r\n");
                Logger.PrintLog("Special thanks to the following persons:");
                Logger.PrintLog("Honorbuddy Team.");
                Logger.PrintLog("Awesome Coding Minds.\r\n");
                Logger.PrintLog("Author of Enyo: nomnomnom.");
                Logger.PrintLog("-------------------------------------------\r\n");
            }
            catch (Exception startexception)
            {
                Logger.DiagnosticLog("Start() Exception: {0}", startexception);
            }
        }

        /// <summary>
        ///     Runs when the BotBase is stopped - Stops several basic functions.
        /// </summary>
        public override void Stop()
        {
            try
            {
                Logger.PrintLog("------------------------------------------");
                Logger.PrintLog("Shutdown - Performing required actions.");
                Logger.PrintLog("-------------------------------------------\r\n");

                StopComponents();
            }
            catch (Exception stopexception)
            {
                Logger.DiagnosticLog("Stop() Exception: {0}", stopexception);
            }
        }
        #endregion

        #region Privates & Internals
        /// <summary>
        ///     SanityCheck - Checks if we are actually ingame and able to control the character.
        /// </summary>
        /// <param name="incombat">Return different sanitycheck in and out combat</param>
        private static bool SanityCheckCombat(bool incombat)
        {
            try
            {
                if (!incombat)
                {
                    return Helpers.IsViable(Me) && !Me.Combat && !Me.IsDead;
                }

                return Helpers.IsViable(Me) && (BotSettings.Instance.UseContinuesHealingMode || BotSettings.Instance.UseForcedCombatMode || Me.Combat) && !Me.IsDead;
            }
            catch (Exception sanitycheckexception)
            {
                Logger.DiagnosticLog("SanityCheckCombat() Exception: {0}", sanitycheckexception); return false;
            }
        }

        /// <summary>
        ///     Actual Root Composite - Within this the RoutineManager runs the routines behaviors.
        /// </summary>
        private static Composite CreateRoot()
        {
            return new PrioritySelector(
                new Decorator(ret => IsPaused, 
                    new ActionAlwaysSucceed()),
                new Decorator(ret => SanityCheckCombat(false), 
                    RoutineManager.Current.PreCombatBuffBehavior),
                new Decorator(ret => SanityCheckCombat(true),
                    SelectLockMethod(
                        RoutineManager.Current.HealBehavior,
                        RoutineManager.Current.CombatBuffBehavior ?? new ActionAlwaysFail(),
                        RoutineManager.Current.CombatBehavior)));
        }

        /// <summary>
        ///     Initializes all the required components for Enyo to run.
        /// </summary>
        private static void InitializeComponents()
        {
            try
            {
                BotSettings.Instance.Load();
                Hotkeys.RegisterKeys();
                Logger.PrintInformation();
                ClicktoMove.ClickToMove(3000);

                ProfileManager.LoadEmpty();

                GlobalSettings.Instance.LogoutForInactivity = false;
                TreeRoot.TicksPerSecond = CharacterSettings.Instance.TicksPerSecond;
            }
            catch (Exception initializecomponentsexception)
            {
                Logger.DiagnosticLog("InitializeComponents() Exception: {0}", initializecomponentsexception);
            }
        }

        /// <summary>
        ///     Stops components for Enyo.
        /// </summary>
        private static void StopComponents()
        {
            try
            {
                /* Restoring the NavigationProvider */
                Navigator.NavigationProvider = OldNavigationProvider;

                Hotkeys.RemoveKeys();
                GlobalSettings.Instance.LogoutForInactivity = true;
                TreeRoot.TicksPerSecond = CharacterSettings.Instance.TicksPerSecond;
            }
            catch (Exception stopcomponentsexception)
            {
                Logger.DiagnosticLog("StopComponents() Exception: {0}", stopcomponentsexception);
            }
        }

        /// <summary>
        ///     Initializes the proper plugins based on the Setting CheckPluginPulsing.
        /// </summary>
        public static void InitializePlugins()
        {
            try
            {
                if (BotSettings.Instance.UsePluginPulsing)
                {
                    _pulseFlags = PulseFlags.CharacterManager | PulseFlags.InfoPanel | PulseFlags.Lua | PulseFlags.Objects | PulseFlags.Plugins;
                }
                else
                {
                    _pulseFlags = PulseFlags.InfoPanel | PulseFlags.Lua | PulseFlags.Objects;
                }
            }
            catch (Exception initializepluginsexception)
            {
                Logger.DiagnosticLog("InitializePlugins() Exception: {0}", initializepluginsexception);
            }
        }

        public static void InitializeNavigator()
        {
            try
            {
                /* Saving current NavigationProvider */
                OldNavigationProvider = Navigator.NavigationProvider;

                if (BotSettings.Instance.UseEnyoNavigator)
                {
                    /* Initializing Enyo's NavigationProvider*/
                    MeshNavigator enyonav = new EnyoNavigator();
                    Navigator.NavigationProvider = enyonav;

                    EnyoNavigationProvider = enyonav;
                }

                else if (!BotSettings.Instance.UseEnyoNavigator)
                {
                    /* Restoring the NavigationProvider */
                    Navigator.NavigationProvider = OldNavigationProvider;
                }
            }
            catch (Exception initializepluginsexception)
            {
                Logger.DiagnosticLog("InitializeNavigator() Exception: {0}", initializepluginsexception);
            }
        }
        #endregion

        #region Softlock
        private static Composite SelectLockMethod(params Composite[] children)
        {
            // ReSharper disable once CSharpWarnings::CS0618
            return BotSettings.Instance.UseSoftLock && !GlobalSettings.Instance.UseFrameLock ? new FrameLockSelector(children) : new PrioritySelector(children);
        }

        private class FrameLockSelector : PrioritySelector
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
