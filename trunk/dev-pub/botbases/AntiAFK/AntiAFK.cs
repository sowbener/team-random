using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AntiAFK.GUI;
using CommonBehaviors.Actions;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Media;
using Timer = System.Timers.Timer;

namespace AntiAFK
{
    public class AntiAFK : BotBase
    {
        private static readonly Stopwatch AntiAfkStopwatch = new Stopwatch();
        private static readonly Random Random = new Random();

        private const double Version = 1.03;

        private static Composite _root;
        private static PulseFlags _pulseFlags;
        private static Timer _antiafktimer;
        private static int _elapsedtime;

        internal static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        #region Overrides
        public override string Name
        {
            get { return "AntiAFK Bot"; }
        }

        public override Form ConfigurationForm
        {
            get { return new AntiAfkGui(); }
        }

        public override bool RequiresProfile
        {
            get { return false; }
        }

        public override PulseFlags PulseFlags
        {
            get { return _pulseFlags; }
        }

        public override void Start()
        {
            try
            {
                StatCounter();
                AntiAFKSettings.Instance.Load();

                if (GlobalSettings.Instance.LogoutForInactivity)
                {
                    GlobalSettings.Instance.LogoutForInactivity = false;
                }

                AFKLogging("\r\n-------------------------------------------");
                AFKLogging("[AntiAFK Bot] AntiAFK Bot - Version {0}", Version);
                AFKLogging("[AntiAFK Bot] This BotBase is written by nomnomnom");
                AFKLogging("-------------------------------------------\r\n");
            }
            catch (Exception exinfo)
            {
                AFKLoggingDiag("Error - {0}", exinfo);
            }
        }

        public override void Pulse()
        {
            try
            {
                if (!StyxWoW.IsInWorld || Me == null || !Me.IsValid)
                {
                    return;
                }

                if (_elapsedtime == 0)
                {
                    _elapsedtime = Random.Next(AntiAFKSettings.Instance.AntiAfkTimeValue, AntiAFKSettings.Instance.AntiAfkTimeValue + AntiAFKSettings.Instance.AntiAfkRandomValue);
                }
                var keytopress = AntiAFKSettings.Instance.AntiAfkKey;

                if (StyxWoW.Me.IsAFKFlagged)
                {
                    if (!AntiAfkStopwatch.IsRunning)
                    {
                        AntiAfkStopwatch.Start();
                    }

                    if (AntiAfkStopwatch.Elapsed.TotalSeconds >= _elapsedtime)
                    {
                        AFKLogging("[AntiAFK Bot] {0} seconds elapsed - Using key!", _elapsedtime);
                        KeyboardManager.PressKey((Char)keytopress);
                        ReleaseTimer(25);
                    }
                }
            }
            catch (Exception ex)
            {
                AFKLoggingDiag("[AntiAFK Bot] Error: {0}", ex);
            }
        }

        public override void Stop()
        {
            if (AntiAfkStopwatch.IsRunning)
            {
                AntiAfkStopwatch.Stop();
            }
        }
        #endregion

        #region Others
        public static void ReleaseTimer(int tickingtime)
        {
            _antiafktimer = new Timer(tickingtime);
            _antiafktimer.Elapsed += OnTimedEvent;
            _antiafktimer.AutoReset = false;
            _antiafktimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var keytopress = AntiAFKSettings.Instance.AntiAfkKey;

            KeyboardManager.ReleaseKey((Char)keytopress);
            AntiAfkStopwatch.Reset();
            AntiAfkStopwatch.Stop();
            _elapsedtime = 0;
        }

        public static void AFKLogging(string message, params object[] args)
        {
            Logging.Write(Colors.Magenta, "{0}", String.Format(message, args));
        }

        public static void AFKLoggingDiag(string message, params object[] args)
        {
            Logging.WriteDiagnostic(Colors.Magenta, "{0}", String.Format(message, args));
        }

        public static void PluginPulsing()
        {
            if (AntiAFKSettings.Instance.AntiAfkPlugins)
            {
                _pulseFlags = PulseFlags.Plugins | PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel;
                AFKLogging("Plugins are enabled!");
            }
            else
            {
                _pulseFlags = PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel;
                AFKLogging("Plugins are disabled!");
            }
        }

        internal static void StatCounter()
        {
            try
            {
                var statcounterDate = DateTime.Now.DayOfYear.ToString(CultureInfo.InvariantCulture);
                if (!statcounterDate.Equals(AntiAFKSettings.Instance.LastStatCounted))
                {
                    Parallel.Invoke(
                        () => new WebClient().DownloadData("http://c.statcounter.com/9363381/0/e4308450/1/"),
                        () => AFKLoggingDiag("[AntiAFK Bot] StatCounter has been updated!"));
                    AntiAFKSettings.Instance.LastStatCounted = statcounterDate;
                    AntiAFKSettings.Instance.Save();
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { /* Catch all errors */ }
        }
        #endregion

        #region Obsolete
        public override Composite Root
        {
            get { return _root ?? (_root = CreateRoot()); }
        }

        private static PrioritySelector CreateRoot()
        {
            return new PrioritySelector(
                new Decorator(new ActionAlwaysSucceed()));
        }
        #endregion
    }
}
