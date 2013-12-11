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
using Action = Styx.TreeSharp.Action;

namespace Tyrael
{
    public class Tyrael : BotBase
    {
        public static readonly Version Revision = new Version(5, 5, 1);
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

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

        public override Composite Root
        {
            get { return _root ?? (_root = CreateRoot()); }
        }

        public override void Start()
        {
            try
            {
                Updater.CheckForUpdate();
                TyraelSettings.Instance.Load();
                ProfileManager.LoadEmpty();
                GlobalSettings.Instance.LogoutForInactivity = false;

                PluginPulsing();

                TyraelUtilities.ClickToMove();
                TyraelUtilities.StatCounter();
                TyraelUtilities.RegisterHotkeys();
                TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;

                Logging.Write(Colors.White, "------------------------------------------");
                Logging.Write(Colors.DodgerBlue, "[Tyrael] {0} is loaded.", RoutineManager.Current.Name);
                Logging.Write(Colors.DodgerBlue, "[Tyrael] {0} {1} has been started.", Name, Revision);
                Logging.Write(Colors.DodgerBlue, "\r\n");
                Logging.Write(Colors.DodgerBlue, "[Tyrael] Special thanks to the following persons:");
                Logging.Write(Colors.DodgerBlue, "[Tyrael] PureRotation Team");
                Logging.Write(Colors.DodgerBlue, "[Tyrael] Minify Author (Mirabis)");
                Logging.Write(Colors.White, "-------------------------------------------\r\n");
            }
            catch (Exception initExept)
            {
                Logging.WriteDiagnostic(initExept.ToString());
            }
        }

        public override void Stop()
        {
            Logging.Write(Colors.DodgerBlue, "[Tyrael] Shutdown - Removing hotkeys.");
            TyraelUtilities.RemoveHotkeys();
            Logging.Write(Colors.DodgerBlue, "[Tyrael] Shutdown - Reconfiguring Honorbuddy.");
            GlobalSettings.Instance.LogoutForInactivity = true;
            Logging.Write(Colors.DodgerBlue, "[Tyrael] Shutdown - Resetting TreeRoot to default value.");
            TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;
            Logging.Write(Colors.DodgerBlue, "[Tyrael] Shutdown - Complete.");
        }

        public static void PluginPulsing()
        {
            if (TyraelSettings.Instance.PluginPulsing)
            {
                _pulseFlags = PulseFlags.Plugins | PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel;
                //Logging.Write(Colors.DodgerBlue, "[Tyrael] Plugins are enabled!");
            }
            else
            {
                _pulseFlags = PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel;
                //Logging.Write(Colors.DodgerBlue, "[Tyrael] Plugins are disabled!");
            }
        }
        #endregion

        #region Selectors
        private static PrioritySelector CreateRoot()
        {
            return new PrioritySelector(
                new Decorator(ret => TyraelUtilities.IsTyraelPaused, new ActionAlwaysSucceed()),
                new Switch<TyraelUtilities.Minify>(ctx => TyraelSettings.Instance.Minify,
                    new SwitchArgument<TyraelUtilities.Minify>(TyraelUtilities.Minify.True, Minified()),
                    new SwitchArgument<TyraelUtilities.Minify>(TyraelUtilities.Minify.False, NonMinified())));
        }

        private static Composite NonMinified()
        {
            return new PrioritySelector(
                new Decorator(ret => SanityCheckCombat(),
                    new PrioritySelector(
                        RoutineManager.Current.HealBehavior,
                        RoutineManager.Current.CombatBuffBehavior ?? new Action(ret => RunStatus.Failure),
                        RoutineManager.Current.CombatBehavior)),
                    RoutineManager.Current.PreCombatBuffBehavior,
                    RoutineManager.Current.RestBehavior);
        }

        private static Composite Minified()
        {
            return new PrioritySelector(
                new Decorator(ret => SanityCheckCombat(),
                    new PrioritySelector(
                        new Action(delegate { StyxWoW.Memory.ReleaseFrame(false); return RunStatus.Failure; }), 
                        RoutineManager.Current.HealBehavior,
                        RoutineManager.Current.CombatBuffBehavior ?? new Action(ret => RunStatus.Failure),
                        RoutineManager.Current.CombatBehavior)),
                    RoutineManager.Current.PreCombatBuffBehavior,
                    RoutineManager.Current.RestBehavior);
        }

        private static bool SanityCheckCombat()
        {
            return TyraelUtilities.IsViable(Me) && (TyraelSettings.Instance.HealingMode || StyxWoW.Me.Combat) && !Me.IsDead;
        }
        #endregion
    }
}
