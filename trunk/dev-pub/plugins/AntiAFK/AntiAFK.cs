using AntiAfk.GUI;
using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.Plugins;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Media;
using Action = Styx.TreeSharp.Action;
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
            new AntiAfkGui().ShowDialog();
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
                    if (!AntiAfkSettings.Instance.AntiAfkGinfo)
                    {
                        AfkLogging("[AntiAFK] Time elapsed - Jumping!");
                        KeyboardManager.PressKey((Char)Settings.AntiAfkKey);
                        ReleaseTimer(50);
                    }
                    else
                    {
                        AfkLogging("[AntiAFK] Time elapsed - Using /GINFO!");
                        RunMacroText("/GINFO", ret => true);
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

        public static string RealLuaEscape(string luastring)
        {
            var bytes = Encoding.UTF8.GetBytes(luastring);
            return bytes.Aggregate(String.Empty, (current, b) => current + ("\\" + b));
        }

        public static Composite RunMacroText(string macro, CanRunDecoratorDelegate cond)
        {
            return new Decorator(
                       cond,
                       new PrioritySelector(
                           new Action(a => Lua.DoString("RunMacroText(\"" + RealLuaEscape(macro) + "\")"))));
        }
    }
}