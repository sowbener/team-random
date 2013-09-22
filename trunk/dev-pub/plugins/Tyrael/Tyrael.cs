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
        public static readonly Version Revision = new Version(5, 2, 6);
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        private const byte DefaultTps = 30;

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

                TU.StatCounter();
                TU.RegisterHotkeys();
                TreeRoot.TicksPerSecond = (byte)TyraelSettings.Instance.HonorbuddyTps;

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
            TreeRoot.TicksPerSecond = DefaultTps;
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
                new Switch<TU.LockState>(ctx => TyraelSettings.Instance.FrameLock,
                    new SwitchArgument<TU.LockState>(TU.LockState.True, ExecuteFrameLocked()),
                    new SwitchArgument<TU.LockState>(TU.LockState.False, ExecuteNormal())),
                RoutineManager.Current.PreCombatBuffBehavior,
                RoutineManager.Current.RestBehavior);
        }

        private static Composite ExecuteFrameLocked()
        {
            return new Decorator(ret => SanityCheckCombat(),
                new PrioritySelector(
                    new FrameLockSelector(RoutineManager.Current.HealBehavior),
                    new FrameLockSelector(RoutineManager.Current.CombatBuffBehavior ?? new TA(ret => RunStatus.Failure)),
                    new FrameLockSelector(RoutineManager.Current.CombatBehavior)));
        }

        private static Composite ExecuteNormal()
        {
            return new Decorator(ret => SanityCheckCombat(),
                new PrioritySelector(
                    RoutineManager.Current.HealBehavior,
                    RoutineManager.Current.CombatBuffBehavior ?? new TA(ret => RunStatus.Failure),
                    RoutineManager.Current.CombatBehavior));
        }

        private static bool SanityCheckCombat()
        {
            return Me != null && Me.IsValid && (TyraelSettings.Instance.HealingMode || StyxWoW.Me.Combat) && !Me.IsDead;
        }
        #endregion

        #region Framelock
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
