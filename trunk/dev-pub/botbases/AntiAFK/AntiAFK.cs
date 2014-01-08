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
    public class AntiAfk : BotBase
    {
        internal static readonly Stopwatch AntiAfkStopwatch = new Stopwatch();
        internal static readonly AntiAFKSettings Settings = new AntiAFKSettings();
        internal static double Version = 1.02;
        internal static Timer Antiafktimer;

        private static PulseFlags _pulseFlags;
        private static Composite _root;

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

                AfkLogging("\r\n-------------------------------------------");
                AfkLogging("[AntiAFK] BotBase - Version {0}", Version);
                AfkLogging("[AntiAFK] This botbase is written by nomnomnom \r\n");
                AfkLogging("[AntiAFK] Loaded - Pulse every {0} miliseconds.", AntiAFKSettings.Instance.AntiAfkTime);
                AfkLogging("[AntiAFK] Loaded - Selected key is {0}.", AntiAFKSettings.Instance.AntiAfkKey);
                AfkLogging("-------------------------------------------\r\n");
            }
            catch (Exception exinfo)
            {
                AfkLogging("[AntiAFK] Error - {0}", exinfo);
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

                Settings.AntiAfkKey = AntiAFKSettings.Instance.AntiAfkKey;
                Settings.AntiAfkTime = AntiAFKSettings.Instance.AntiAfkTime;

                if (StyxWoW.Me.IsAFKFlagged)
                {
                    if (!AntiAfkStopwatch.IsRunning)
                    {
                        AntiAfkStopwatch.Start();
                    }

                    if (AntiAfkStopwatch.Elapsed.TotalSeconds >= Settings.AntiAfkTime)
                    {
                        AfkLogging("[AntiAFK] Time elapsed - Using key!");
                        KeyboardManager.PressKey((Char)Settings.AntiAfkKey);
                        ReleaseTimer(25);
                    }
                }
            }
            catch (Exception ex)
            {
                AfkLogging("[AntiAFK] Error: {0}", ex);
            }
        }

        public override void Stop()
        {
            if (AntiAfkStopwatch.IsRunning)
            {
                AntiAfkStopwatch.Stop();
            }
        }

        public override PulseFlags PulseFlags
        {
            get { return _pulseFlags; }
        }

        public override Composite Root
        {
            get { return _root ?? (_root = CreateRoot()); }
        }
        #endregion

        #region Others
        public static void ReleaseTimer(int tickingtime)
        {
            Antiafktimer = new Timer(tickingtime);
            Antiafktimer.Elapsed += OnTimedEvent;
            Antiafktimer.AutoReset = false;
            Antiafktimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            AfkLogging("[AntiAFK] Releasing key!");
            KeyboardManager.ReleaseKey((Char)Settings.AntiAfkKey);
            AntiAfkStopwatch.Reset();
            AntiAfkStopwatch.Stop();
        }

        public static void AfkLogging(string message, params object[] args)
        {
            Logging.Write(Colors.Magenta, "{0}", String.Format(message, args));
        }

        public static void PluginPulsing()
        {
            if (AntiAFKSettings.Instance.AntiAfkPlugins)
            {
                _pulseFlags = PulseFlags.Plugins | PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel;
                AfkLogging("[AntiAFK] Plugins are enabled!");
            }
            else
            {
                _pulseFlags = PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel;
                AfkLogging("[AntiAFK] Plugins are disabled!");
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
                        () => Logging.WriteDiagnostic(Colors.Magenta, "[AntiAFK] StatCounter has been updated!"));
                    AntiAFKSettings.Instance.LastStatCounted = statcounterDate;
                    AntiAFKSettings.Instance.Save();
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { /* Catch all errors */ }
        }
        #endregion

        #region Obsolete
        private static PrioritySelector CreateRoot()
        {
            return new PrioritySelector(
                new Decorator(new ActionAlwaysSucceed()));
        }
        #endregion
    }
}
