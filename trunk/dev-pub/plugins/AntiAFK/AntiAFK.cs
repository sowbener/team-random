﻿using AntiAfk.GUI;
using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.Plugins;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
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
        private static readonly Stopwatch AntiAfkStopwatch = new Stopwatch();
        private static readonly AntiAfkSettings Settings = new AntiAfkSettings();
        private static Timer _antiafktimer;

        public override string ButtonText
        {
            get { return "AntiAfk Settings"; }
        }

        public override void OnButtonPress()
        {
            new AntiAFKGui().ShowDialog();
        }

        public override void Initialize()
        {
            AfkLogging("[AntiAFK] Loaded - Pulse every {0} miliseconds.", Settings.AntiAfkTime);
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
                    KeyboardManager.PressKey((Char)Keys.Space);
                    ReleaseTimer(50);
                }
            }
            catch (Exception ex)
            {
                AfkLogging("[AntiAFK] Error: {0}", ex);
            }
        }

        public static void AfkLogging(string message, params object[] args)
        {
            if (message == null) 
                return;
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
            KeyboardManager.ReleaseKey((Char)Keys.Space);
            AfkLogging("[AntiAFK] Time elapsed - I jumped!");
            AntiAfkStopwatch.Reset();
        }
    }
}