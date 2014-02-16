using System;
using System.Collections.Generic;
using System.Linq;
using YourBuddy.Core;
using Styx;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Lua = Styx.WoWInternals.Lua;
using YourBuddy.Core.Helpers;
using DF = YourBuddy.Rotations.Druid.Feral;


namespace YourBuddy.Rotations.Druid
{
    internal class CommonDruid
    {
        private static bool _combatLogAttached;
        public static DateTime RipAppliedDateTime;
        public static double RealRipTimeLeft;
        public static double Ripextends;
        public static double ClipTime;

        /// <summary>
        ///     erm..me
        /// </summary>
        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        public static bool IsBehind
        {
            get { return Me.CurrentTarget != null && StyxWoW.Me.IsBehind(Me.CurrentTarget); }
        }


        public static void Initialize()
        {
            AttachCombatLogEvent();
        }

        private static void AttachCombatLogEvent()
        {
            CombatLogHandler.Register("SPELL_AURA_APPLIED", HandleCombatLog);
            CombatLogHandler.Register("SPELL_AURA_REFRESH", HandleCombatLog);
            CombatLogHandler.Register("SPELL_DAMAGE", HandleCombatLog);
            CombatLogHandler.Register("SPELL_AURA_REMOVED", HandleCombatLog);
        }

        private static void HandleCombatLog(CombatLogEventArgs args)
        {
            //var e = new CombatLogEventArgs(args.EventName, args.FireTimeStamp, args.Args);
            switch (args.Event)
            {
                case "SPELL_AURA_APPLIED":
                    if (args.SourceGuid != 1 && args.SourceGuid == StyxWoW.Me.Guid)
                    {
                        if (args.SpellId == 1822)
                        {
                            Spell.UpdateRakeTargets(args.DestGuid, DF.Rake_sDamage);
                        }
                        if (args.SpellId == 1079)
                        {
                            RipAppliedDateTime = DateTime.Now;
                            DF._dot_rip_multiplier = DF.Rip_sDamage;
                            Ripextends = 0;
                        }
                    }
                    break;

                case "SPELL_AURA_REFRESH":
                    if (args.SourceGuid != 1 && args.SourceGuid == StyxWoW.Me.Guid)
                    {
                        if (args.SpellId == 1822)
                        {
                            Spell.UpdateRakeTargets(args.DestGuid, DF.Rake_sDamage);
                        }
                        if (args.SpellId == 1079)
                        {
                            ClipTime =
                                CalcClipTime((16 + CalcExtensionsTime() + ClipTime) -
                                             (-1 * (RipAppliedDateTime - DateTime.Now).TotalSeconds));
                            RipAppliedDateTime = DateTime.Now;
                            DF._dot_rip_multiplier = DF.Rip_sDamage;
                            Ripextends = 0;
                        }
                    }
                    break;

                case "SPELL_DAMAGE":
                    if (args.SourceGuid != 1 && args.SourceGuid == StyxWoW.Me.Guid)
                    {
                        if (DF.RipUp)
                        {
                            if (args.SpellId == 5221 || args.SpellId == 114236 || args.SpellId == 102545 || args.SpellId == 6785 ||
                                args.SpellId == 33876)
                                //Normal Shred, Glyphed Shred, Ravage, other Ravage, Mangle 		
                                if (Ripextends < 3)
                                    Ripextends++;
                            if (args.SpellId == 22568 && StyxWoW.Me.CurrentTarget != null &&
                                StyxWoW.Me.CurrentTarget.HealthPercent < 25)
                            {
                                ClipTime =
                                    CalcClipTime((16 + CalcExtensionsTime() + ClipTime) -
                                                 (-1 * (RipAppliedDateTime - DateTime.Now).TotalSeconds));
                                RipAppliedDateTime = DateTime.Now;
                                Ripextends = 0;
                            }
                        }
                    }
                    break;

                case "SPELL_AURA_REMOVED":
                    if (args.SourceGuid != 1 && args.SourceGuid == StyxWoW.Me.Guid)
                    {
                        if (args.SpellId == 1822)
                        {
                            Spell.UpdateRakeTargets(args.DestGuid, 0);
                        }
                        if (args.SpellId == 1079)
                        {
                            DF._dot_rip_multiplier = 0;
                            Ripextends = 0;
                            ClipTime = 0;
                        }
                    }
                    break;
            }
        }
        private static double CalcClipTime(double time)
        {
            if (time < 2)
                return time;
            if (time < 4)
                return time - 2;
            if (time < 6)
                return time - 4;
            if (time < 8)
                return time - 6;
            if (time < 10)
                return time - 8;
            if (time < 12)
                return time - 10;
            if (time < 14)
                return time - 12;
            if (time < 16)
                return time - 14;
            if (time < 18)
                return time - 16;
            if (time < 20)
                return time - 18;
            if (time < 22)
                return time - 20;
            if (time < 24)
                return time - 22;
            if (time < 26)
                return time - 24;
            if (time < 28)
                return time - 26;
            return 0;
        }

        public static double CalcExtensionsTime()
        {
            if (Ripextends == 3)
                return 8;
            if (Ripextends == 2)
                return 6;
            if (Ripextends == 1)
                return 2;
            return 0;
        }

        public static void CalcRealTimeOfRip()
        {
            RealRipTimeLeft = !DF.RipUp
                ? 0
                : (16 + CalcExtensionsTime() + ClipTime) - (-1 * (RipAppliedDateTime - DateTime.Now).TotalSeconds);
        }



    }
}