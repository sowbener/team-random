using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.TreeSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using Action = Styx.TreeSharp.Action;
using G = YBMoP_BT_Warrior.Routines.YBGlobal;
using SG = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsG;

namespace YBMoP_BT_Warrior.Helpers
{
    static class YBLogger
    {
        #region YBMoP BT - Advanced Logging
        // 1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure
        // 1 traverse per xxx timespan ms
        internal static Composite AdvancedLogging
        {
            get
            {
                return new PrioritySelector(
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure,
                            new Action(delegate
                            {
                                AdvancedLogP("[Cooldown Time Logging]");
                                AdvancedLogW("Avatar CD Time: {0}", G.AVCD);
                                AdvancedLogW("Bloodbath CD Time: {0}", G.BBCD);
                                AdvancedLogW("Bloodthirst CD Time: {0}", G.BTCD);
                                AdvancedLogW("ColossusSmash CD Time: {0}", G.CSCD);
                                AdvancedLogW("Pummel CD Time: {0}", G.PUCD);
                                AdvancedLogW("SkullBanner CD Time: {0}", G.SBCD);
                                AdvancedLogW("SpellReflection CD Time: {0}", G.SRCD);
                                AdvancedLogW("Recklessness CD Time: {0}", G.RCCD);
                                AdvancedLogP(" ");
                                return RunStatus.Success;
                            }
                            )),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure,
                            new Action(delegate
                            {
                                AdvancedLogP("[On Cooldown Logging]");
                                AdvancedLogW("Bloodthirst on CD: {0}", G.BTCDOC);
                                AdvancedLogW("DemoBanner on CD: {0}", G.DBCDOC);
                                AdvancedLogW("HeroicLeap on CD: {0}", G.HLCDOC);
                                AdvancedLogW("ImpVic on CD: {0}", G.IVCDOC);
                                AdvancedLogW("Pummel on CD: {0}", G.PUCDOC);
                                AdvancedLogW("VicRush on CD: {0}", G.VRCDOC);
                                AdvancedLogP(" ");
                                return RunStatus.Success;
                            }
                            )),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure,
                            new Action(delegate
                            {
                                AdvancedLogP("[Fading Aura Logging]");
                                AdvancedLogW("CS Fading 1S: {0}", G.FadingCS1S);
                                AdvancedLogW("CS Fading 2S: {0}", G.FadingCS2S);
                                AdvancedLogW("RB Fading 3S: {0}", G.FadingRB3S);
                                AdvancedLogP(" ");
                                return RunStatus.Success;
                            })),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure,
                            new Action(delegate
                            {
                                AdvancedLogP("[Talentmanager Logging]");
                                AdvancedLogW("Has IS Glyph: {0}", G.ISGlyph);
                                AdvancedLogW("Has UR Glyph: {0}", G.URGlyph);
                                AdvancedLogP(" ");
                                AdvancedLogW("Talent Avatar: {0}", G.AVTalent);
                                AdvancedLogW("Talent Bloodbath: {0}", G.BBTalent);
                                AdvancedLogW("Talent Bladestorm: {0}", G.BSTalent);
                                AdvancedLogW("Talent Dragon Roar: {0}", G.DRTalent);
                                AdvancedLogW("Talent Enraged Regen: {0}", G.ERTalent);
                                AdvancedLogW("Talent Storm Bolt: {0}", G.SBTalent);
                                AdvancedLogW("Talent Shockwave: {0}", G.SWTalent);
                                AdvancedLogW("Talent Impending Vic: {0}", G.IVTalent);
                                AdvancedLogP(" ");
                                return RunStatus.Success;
                            })),
                        new ThrottlePasses(1, TimeSpan.FromMilliseconds(SG.Instance.NumAdvLogThrottleTime), RunStatus.Failure,
                            new Action(delegate
                            {
                                AdvancedLogP("[Other Logging]");
                                AdvancedLogP(" ");
                                return RunStatus.Success;
                            })));
            }
        }

        internal static void WriteToLogFile()
        {
            WriteFile("YBMoP BT: Diagnostic Logging");
            WriteFile("");
            WriteFile("{0} is the used revision.", YBMain.Revision);
            WriteFile("{0} - {1} - {2} is the current race, specialization & level.", StyxWoW.Me.Race, StyxWoW.Me.Specialization, StyxWoW.Me.Level);
            WriteFile("{0} is your faction", StyxWoW.Me.IsAlliance ? "Alliance" : "Horde");
            WriteFile("");
            WriteFile("{0:F1} days since Windows was started.", TimeSpan.FromMilliseconds(Environment.TickCount).TotalHours / 24.0);
            WriteFile("{0} FPS currently in WoW.", YBLua.GetFps());
            WriteFile("{0} ms of Latency in WoW.", StyxWoW.WoWClient.Latency);
            WriteFile("");
            WriteFile("{0} is the HB path.", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            WriteFile("");
            LogSettings("YBSettingsG - (Global)", Interfaces.Settings.YBSettingsG.Instance);
            if (StyxWoW.Me.Specialization == WoWSpec.WarriorArms)
                LogSettings("YBSettingsA - (Arms)", Interfaces.Settings.YBSettingsA.Instance);
            if (StyxWoW.Me.Specialization == WoWSpec.WarriorFury)
                LogSettings("YBSettingsF - (Fury)", Interfaces.Settings.YBSettingsF.Instance);
            if (StyxWoW.Me.Specialization == WoWSpec.WarriorProtection)
                LogSettings("YBSettingsP - (Prot)", Interfaces.Settings.YBSettingsP.Instance);
            WriteFile("");
            WriteFile("Juggernaut Talent: {0}", G.JNTalent);
            WriteFile("Double Time Talent: {0}", G.DTTalent);
            WriteFile("Warbringer Talent: {0}", G.WBTalent);
            WriteFile("Enraged Regeneration Talent: {0}", G.ERTalent);
            WriteFile("Second Wind Talent: {0}", G.SCTalent);
            WriteFile("Impending Victory Talent: {0}", G.IVTalent);
            WriteFile("Staggering Shout Talent: {0}", G.SSTalent);
            WriteFile("Piercing Howl Talent: {0}", G.PHTalent);
            WriteFile("Disrupting Shout Talent: {0}", G.DSTalent);
            WriteFile("Bladestorm Talent: {0}", G.BSTalent);
            WriteFile("Shockwave Talent: {0}", G.SWTalent);
            WriteFile("Dragon Roar Talent: {0}", G.DRTalent);
            WriteFile("Mass Spell Reflection Talent: {0}", G.MRTalent);
            WriteFile("Safeguard Talent: {0}", G.SGTalent);
            WriteFile("Vigilance Talent: {0}", G.VGTalent);
            WriteFile("Avatar Talent: {0}", G.AVTalent);
            WriteFile("Bloodbath Talent: {0}", G.BBTalent);
            WriteFile("Storm Bolt Talent: {0}", G.SBTalent);
        }
        #endregion

        #region YBMoP BT - Log Messages
        public static void WriteFileLog(string message, params object[] args)
        {
            if (message == null) return;
            WriteFile("{0}", String.Format(message, args));
        }

        public static void AdvancedLogP(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.MediumPurple, "{0}", String.Format(message, args));
        }

        public static void AdvancedLogW(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.White, "{0}", String.Format(message, args));
        }

        public static void InitLogF(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.Firebrick, "{0}", String.Format(message, args));
        }

        public static void InitLogO(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.Orange, "{0}", String.Format(message, args));
        }

        public static void InitLogW(string message, params object[] args)
        {
            if (message == null) return;
            Logging.Write(Colors.White, "{0}", String.Format(message, args));
        }

        public static void DiagLogW(string message, params object[] args)
        {
            if (message == null) return;
            Logging.WriteDiagnostic(Colors.White, "{0}", String.Format(message, args));
        }

        public static void DiagLogP(string message, params object[] args)
        {
            if (message == null) return;
            Logging.WriteDiagnostic(Colors.MediumPurple, "{0}", String.Format(message, args));
        }

        public static void CombatLogO(string message, params object[] args)
        {
            if (message == _lastCombatmsg) return;
            Logging.Write(Colors.Orange, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogG(string message, params object[] args)
        {
            if (message == _lastCombatmsg) return;
            Logging.Write(Colors.LimeGreen, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        public static void CombatLogP(string message, params object[] args)
        {
            if (message == _lastCombatmsg) return;
            Logging.Write(Colors.MediumPurple, "{0}", String.Format(message, args));
            _lastCombatmsg = message;
        }

        private static string _lastCombatmsg;
        #endregion

        #region YBMoP BT - Log TreePerformanceTimer
        private static readonly Stopwatch TreePerformanceTimer = new Stopwatch();

        internal static Composite TreePerformance(string obj)
        {
            return new Action(ret =>
            {
                if (!SG.Instance.CheckTreePerformance)
                {
                    return RunStatus.Failure;
                }

                if (TreePerformanceTimer.ElapsedMilliseconds > 0)
                {
                    DiagLogW("YBMoP Performance Timer: Elapsed Time to traverse {0}: {1} ms ({2} ms client lag)", obj, TreePerformanceTimer.ElapsedMilliseconds, RealLag.TotalMilliseconds);
                    TreePerformanceTimer.Stop();
                    TreePerformanceTimer.Reset();
                }
                TreePerformanceTimer.Start();
                    return RunStatus.Failure;
            });
        }

        private static TimeSpan RealLag
        {
            get { return TimeSpan.FromMilliseconds(StyxWoW.WoWClient.Latency); }
        }
        #endregion

        #region Other Logging
        // Tnx Singular!
        public static void WriteFile(string message)
        {
            WriteFile(LogLevel.Verbose, message);
        }

        public static void WriteFile(string message, params object[] args)
        {
            WriteFile(LogLevel.Verbose, message, args);
        }

        public static void WriteFile(LogLevel ll, string message, params object[] args)
        {
            if (GlobalSettings.Instance.LogLevel >= LogLevel.Quiet)
                Logging.WriteToFileSync(ll, "YBMoP: " + message, args);
        }

        public static void LogSettings(string desc, Settings set)
        {
            if (set == null)
                return;

            WriteFile("====== {0} ======", desc);
            foreach (var kvp in set.GetSettings())
            {
                WriteFile("  {0}: {1}", kvp.Key, kvp.Value.ToString());
            }
            WriteFile("");
        }
        #endregion
    }
}
