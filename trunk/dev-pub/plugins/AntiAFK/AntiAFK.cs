using AntiAfk.GUI;
using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.Plugins;
using Styx.WoWInternals;
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
        internal static readonly Stopwatch AntiAfkStopwatch = new Stopwatch();
        internal static readonly AntiAfkSettings Settings = new AntiAfkSettings();
        internal static Timer Antiafktimer;

        public override string Name
        {
            get { return "AntiAfk"; }
        }

        public override string Author
        {
            get { return "nomnomnom"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }

        public override bool WantButton
        {
            get { return true; }
        }

        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        public override string ButtonText
        {
            get { return "AntiAFK Settings"; }
        }

        public override void OnButtonPress()
        {
            new AntiAfkGui().ShowDialog();
            AfkLogging("[AntiAFK] GUI has been opened.");
        }

        public override void Initialize()
        {
            try
            {
                AntiAfkSettings.Instance.Load();
                GlobalSettings.Instance.LogoutForInactivity = false;

                AfkLogging("\r\n-------------------------------------------");
                AfkLogging("This plugin is written by nomnomnom \r\n");
                AfkLogging("[AntiAFK] Loaded - Pulse every {0} miliseconds.", Settings.AntiAfkTime);
                AfkLogging("[AntiAFK] Loaded - Selected key is {0}.", Settings.AntiAfkKey);
                AfkLogging("[AntiAFK] Loaded - Using /GINFO is {0}.", Settings.AntiAfkGinfo);
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
                if (!StyxWoW.IsInWorld || Me == null || !Me.IsValid || Me.IsDead)
                {
                    return;
                }

                AntiAfkStopwatch.Start();

                if (AntiAfkStopwatch.Elapsed.TotalMilliseconds >= Settings.AntiAfkTime)
                {
                    if (!AntiAfkSettings.Instance.AntiAfkGinfo)
                    {
                        AfkLogging("[AntiAFK] Time elapsed - Using key!");
                        KeyboardManager.PressKey((Char)Settings.AntiAfkKey);
                        ReleaseTimer(25);
                    }
                    else
                    {
                        AfkLogging("[AntiAFK] Time elapsed - Using /GINFO!");
                        Lua.DoString("RunMacroText(\"/ginfo\");");
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
            Antiafktimer = new Timer(tickingtime);
            Antiafktimer.Elapsed += OnTimedEvent;
            Antiafktimer.AutoReset = false;
            Antiafktimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            KeyboardManager.ReleaseKey((Char)Settings.AntiAfkKey);
            AntiAfkStopwatch.Reset();
        }
    }
}