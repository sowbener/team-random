using AntiAfk.GUI;
using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.Plugins;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Media;
using Timer = System.Timers.Timer;

namespace AntiAfk
{
    public class AntiAfk : HBPlugin
    {
        public override string Name { get { return "AntiAfk"; } }
        public override string Author { get { return "nomnomnom"; } }
        public override Version Version { get { return new Version(1, 0, 0); } }
        public override bool WantButton { get { return true; } }

        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        public static readonly Stopwatch AntiAfkStopwatch = new Stopwatch();
        private static readonly AntiAfkSettings Settings = new AntiAfkSettings();
        private static Timer _antiafktimer;

        public override string ButtonText
        {
            get { return "AntiAfk Settings"; }
        }

        public override void OnButtonPress()
        {
            new AntiAfkGui().ShowDialog();
        }

        public override void Initialize()
        {
            try
            {
                AntiAfkSettings.Instance.Load();
                GlobalSettings.Instance.LogoutForInactivity = false;

                AfkLogging("[AntiAFK] Loaded - Pulse every {0} miliseconds.", Settings.AntiAfkTime);
                AfkLogging("[AntiAFK] Loaded - Selected key is {0}.", Settings.AntiAfkKey);
                AfkLogging("[AntiAFK] Loaded - Using /GINFO is {0}.", Settings.AntiAfkGinfo);
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
                if (!StyxWoW.IsInWorld || Me == null || !Me.IsValid || Me.IsDead)
                {
                    return;
                }

                AntiAfkStopwatch.Start();
                if (AntiAfkStopwatch.Elapsed.TotalMilliseconds > Settings.AntiAfkTime)
                {
                    if (!AntiAfkSettings.Instance.AntiAfkGinfo)
                    {
                        AfkLogging("[AntiAFK] Time elapsed - Using key!");
                        KeyboardManager.PressKey((Char)Settings.AntiAfkKey);
                        ReleaseTimer(50);
                    }
                    else
                    {
                        AfkLogging("[AntiAFK] Time elapsed - Using /GINFO!");
                        //TODO: Make this.
                        AntiAfkStopwatch.Reset();                   
                    }
                }
            }
            catch (Exception ex)
            {
                AfkLogging("[AntiAFK] Error: {0}", ex);
            }
        }

        public static void AfkLogging(string message, params object[] args)
        {
            Logging.Write(Colors.Magenta, "{0}", String.Format(message, args));
        }

        public static void ReleaseTimer(int tickingtime)
        {
            _antiafktimer = new Timer(tickingtime);
            _antiafktimer.Elapsed += OnTimedEvent;
            _antiafktimer.AutoReset = false;
            _antiafktimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            KeyboardManager.ReleaseKey((Char)Settings.AntiAfkKey);
            AntiAfkStopwatch.Reset();
        }
    }
}