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
using TU = Tyrael.Shared.TyraelUtilities;
using TA = Styx.TreeSharp.Action;

namespace Tyrael
{
    public class Tyrael : BotBase
    {
        public static readonly Version Revision = new Version(5, 3, 0);
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        private static PulseFlags _pulseFlags;
        private static Composite _root;

        #region Overrides
        public override string Name
        {
            get { return "Tyrael"; }
        }

        public override PulseFlags PulseFlags
        {
            get { return !TU.IsTyraelPaused ? _pulseFlags : PulseFlags.Objects | PulseFlags.Lua; }
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
                TyraelSettings.Instance.Load();
                ProfileManager.LoadEmpty();
                GlobalSettings.Instance.LogoutForInactivity = false;

                PluginPulsing();

                TU.ClickToMove();
                TU.StatCounter();
                TU.RegisterHotkeys();
                TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;

                Logging.Write(Colors.DodgerBlue, "[Tyrael] {0} is loaded.", RoutineManager.Current.Name);
                Logging.Write(Colors.DodgerBlue, "[Tyrael] {0} {1} has been started.", Name, Revision);
            }
            catch (Exception initExept)
            {
                Logging.WriteDiagnostic(initExept.ToString());
            }
        }

        public override void Stop()
        {
            Logging.Write(Colors.DodgerBlue, "[Tyrael] Shutdown - Removing hotkeys.");
            TU.RemoveHotkeys();
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
                _pulseFlags = PulseFlags.Plugins | PulseFlags.Objects | PulseFlags.Lua;
                Logging.Write(Colors.DodgerBlue, "[Tyrael] Plugins are enabled!");
            }
            else
            {
                _pulseFlags = PulseFlags.Objects | PulseFlags.Lua;
                Logging.Write(Colors.DodgerBlue, "[Tyrael] Plugins are disabled!");
            }
        }
        #endregion

        #region Selectors
        private static PrioritySelector CreateRoot()
        {
            return new PrioritySelector(
                new Decorator(ret => TU.IsTyraelPaused, new ActionAlwaysSucceed()),
                new Decorator(ret => SanityCheckCombat(),
                    new PrioritySelector(
                        RoutineManager.Current.HealBehavior,
                        RoutineManager.Current.CombatBuffBehavior ?? new TA(ret => RunStatus.Failure),
                        RoutineManager.Current.CombatBehavior
                        )),
                RoutineManager.Current.PreCombatBuffBehavior,
                RoutineManager.Current.RestBehavior);
        }

        private static bool SanityCheckCombat()
        {
            return Me != null && Me.IsValid && (TyraelSettings.Instance.HealingMode || StyxWoW.Me.Combat) && !Me.IsDead;
        }
        #endregion
    }
}
