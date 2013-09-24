// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable FieldCanBeMadeReadOnly.Local
#define NO_LATENCY_ISSUES_WITH_GLOBAL_COOLDOWN

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using CommonBehaviors.Actions;
using Styx;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Patchables;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.World;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;
using DeathVader.Helpers;
using System;
using Logger = DeathVader.Helpers.DvLogger;

namespace DeathVader.Core
{
    internal static class DvSpell
    {
        #region Delegates
        // Public delegates
        public delegate WoWUnit UnitSelectionDelegate(object context);
        public delegate WoWPoint LocationRetriever(object context);
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        // Internal delegates
        internal delegate T Selection<out T>(object context);
        #endregion

        #region Casting Methods
        // Casting by Name
        public static Composite Cast(string spell, Selection<bool> reqs = null)
        {
            return Cast(spell, ret => StyxWoW.Me.CurrentTarget, reqs);
        }

        public static Composite Cast(string spell, UnitSelectionDelegate onUnit, Selection<bool> reqs = null)
        {
            return
                new Decorator(ret => (onUnit != null && onUnit(ret) != null && (reqs == null || reqs(ret)) && SpellManager.CanCast(spell, onUnit(ret))),
                    new Action(ret =>
                        {
                            SpellManager.Cast(spell, onUnit(ret));
                            CooldownTracker.SpellUsed(spell);
                            DvLogger.CombatLogO("Casting: " + spell + " on " + onUnit(ret).SafeName);
                        }));
        }

        // Casting by Integer
        public static Composite Cast(int spell, Selection<bool> reqs = null)
        {
            return Cast(spell, ret => StyxWoW.Me.CurrentTarget, reqs);
        }

        public static Composite Cast(int spell, UnitSelectionDelegate onUnit, Selection<bool> reqs = null)
        {
            return
                new Decorator(ret => ((reqs != null && reqs(ret)) || (reqs == null)) && onUnit != null && onUnit(ret) != null && SpellManager.CanCast(spell, onUnit(ret)),
                    new Action(ret =>
                        {
                            SpellManager.Cast(spell, onUnit(ret));
                            CooldownTracker.SpellUsed(spell);
                            DvLogger.CombatLogO("Casting: " + spell + " on " + onUnit(ret).SafeName);
                        }));
        }

        // Casting on Ground by String
        public static Composite CastOnGround(string spell, LocationRetriever onLocation)
        {
            return CastOnGround(spell, onLocation, ret => true);
        }

        public static Composite CastOnGround(string spell, LocationRetriever onLocation, CanRunDecoratorDelegate requirements, bool waitForSpell = false)
        {
            return
                new Decorator(ret => onLocation != null && requirements(ret) && SpellManager.CanCast(spell) && (StyxWoW.Me.Location.Distance(onLocation(ret)) <= SpellManager.Spells[spell].MaxRange || SpellManager.Spells[spell].MaxRange == 0),
                    new Sequence(
                       new Action(ret => SpellManager.Cast(spell)),
                        new DecoratorContinue(ctx => waitForSpell,
                            new WaitContinue(1, ret =>
                                StyxWoW.Me.CurrentPendingCursorSpell != null &&
                                StyxWoW.Me.CurrentPendingCursorSpell.Name == spell, new ActionAlwaysSucceed())),
                        new Action(ret =>
                            {
                                SpellManager.ClickRemoteLocation(onLocation(ret));
                                CooldownTracker.SpellUsed(spell);
                                DvLogger.CombatLogO("Casting: " + spell);
                            }
                        )));
        }
        #endregion

        #region Cooldown Tracking
        // Cached - If no cached result is found, check realtime.
        public static TimeSpan GetSpellCooldown(string spell, bool overide = false)
        {
            if (overide)
                return CooldownTracker.CooldownTimeLeft(spell);

            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.CooldownTimeLeft : results.Original.CooldownTimeLeft;
            }

            return TimeSpan.MaxValue;
        }



        #region Double Cast Shit

        // PRSettings.Instance.ThrottleTime is used throughout the rotationbases to enable a setable expiryTime for the methods below.

        private static readonly Dictionary<string, DoubleCastSpell> DoubleCastEntries = new Dictionary<string, DoubleCastSpell>();

        private static void UpdateDoubleCastEntries(string spellName, double expiryTime)
        {
            if (DoubleCastEntries.ContainsKey(spellName)) DoubleCastEntries[spellName] = new DoubleCastSpell(spellName, expiryTime, DateTime.UtcNow);
            if (!DoubleCastEntries.ContainsKey(spellName)) DoubleCastEntries.Add(spellName, new DoubleCastSpell(spellName, expiryTime, DateTime.UtcNow));
        }

        public static void OutputDoubleCastEntries()
        {
            foreach (var spell in DoubleCastEntries)
            {
                Logger.InfoLog(spell.Key + " time: " + spell.Value.DoubleCastCurrentTime);
            }
        }

        internal static void PulseDoubleCastEntries()
        {
            DoubleCastEntries.RemoveAll(t => DateTime.UtcNow.Subtract(t.DoubleCastCurrentTime).TotalSeconds >= t.DoubleCastExpiryTime);
        }

        public static Composite PreventDoubleCast(string spell, double expiryTime, Selection<bool> reqs = null)
        {
            return PreventDoubleCast(spell, expiryTime, ret => StyxWoW.Me.CurrentTarget, reqs);
        }

        public static Composite PreventDoubleCast(int spell, double expiryTime, Selection<bool> reqs = null)
        {
            return PreventDoubleCast(spell, expiryTime, target => Me.CurrentTarget, reqs);
        }

        /// <summary> This Method can enable / disable the Moving Check from CanCast and should fix Pauls issues </summary>
        public static Composite PreventDoubleCast(string spell, double expiryTime, UnitSelectionDelegate onUnit, Selection<bool> reqs = null, bool ignoreMoving = false)
        {
            return
                new Decorator(
                    ret =>
                        ((reqs != null && reqs(ret)) || (reqs == null))
                        && onUnit != null
                        && onUnit(ret) != null
                        && SpellManager.CanCast(spell, onUnit(ret), true, !ignoreMoving)
                        && !DoubleCastEntries.ContainsKey(spell + onUnit(ret).GetHashCode()),
                    new Sequence(
                        new Action(ret => SpellManager.Cast(spell, onUnit(ret))),
                        new Action(ret => Logger.InfoLog(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent))),
                //new Action(ret => Logger.InfoLog(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}% PH: {4:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent, HealManager.PredictedHealthPercent(onUnit(ret), PRSettings.Instance.includeMyHeals)))),
                        new Action(ret => UpdateDoubleCastEntries(spell.ToString(CultureInfo.InvariantCulture) + onUnit(ret).GetHashCode(), expiryTime))));
        }

        public static bool ShowPlayerNames { get; set; }

        public static string SafeName(this WoWObject obj)
        {
            if (obj.IsMe)
            {
                return "Me";
            }

            string name;
            if (obj is WoWPlayer)
            {
                if (RaFHelper.Leader == obj)
                    return "Tank";

                name = ShowPlayerNames ? ((WoWPlayer)obj).Name : ((WoWPlayer)obj).Class.ToString();
            }
            else if (obj is WoWUnit && obj.ToUnit().IsPet)
            {
                name = obj.ToUnit().OwnedByRoot.SafeName() + ":Pet";
            }
            else
            {
                name = obj.Name;
            }
            return name;
        }


        public static Composite PreventDoubleCast(int spell, double expiryTime, UnitSelectionDelegate onUnit, Selection<bool> reqs = null)
        {
            return
                new Decorator(
                    ret =>
                        ((reqs != null && reqs(ret)) || (reqs == null))
                        && onUnit != null
                        && onUnit(ret) != null
                        && SpellManager.CanCast(spell, onUnit(ret))
                        && !DoubleCastEntries.ContainsKey(spell.ToString(CultureInfo.InvariantCulture) + onUnit(ret).GetHashCode()),
                    new Sequence(
                        new Action(ret => SpellManager.Cast(spell, onUnit(ret))),
                        new Action(ret => Logger.InfoLog(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent))),
                        new Action(ret => UpdateDoubleCastEntries(spell.ToString(CultureInfo.InvariantCulture) + onUnit(ret).GetHashCode(), expiryTime))));
        }


        public static Composite PreventDoubleCastNoCanCast(string spell, double expiryTime, UnitSelectionDelegate onUnit, Selection<bool> reqs = null)
        {
            return
                new Decorator(
                    ret =>
                        ((reqs != null && reqs(ret)) || (reqs == null))
                        && onUnit != null
                        && onUnit(ret) != null
                        && SpellManager.CanCast(spell, onUnit(ret))
                        && !DoubleCastEntries.ContainsKey(spell + onUnit(ret).GetHashCode()),
                    new Sequence(
                        new Action(ret => SpellManager.Cast(spell, onUnit(ret))),
                        new Action(ret => Logger.InfoLog(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent))),
                        new Action(ret => UpdateDoubleCastEntries(spell.ToString(CultureInfo.InvariantCulture) + onUnit(ret).GetHashCode(), expiryTime))));
        }


        public static Composite PreventDoubleCastOnGround(string spell, double expiryTime, LocationRetriever onLocation)
        {
            return PreventDoubleCastOnGround(spell, expiryTime, onLocation, ret => true);
        }

        public static Composite PreventDoubleCastOnGround(string spell, double expiryTime, LocationRetriever onLocation, CanRunDecoratorDelegate requirements, bool waitForSpell = false)
        {
            return new Decorator(
                    ret =>
                    onLocation != null && requirements(ret) && SpellManager.CanCast(spell) &&
                        //StyxWoW.Me.CurrentTarget != null && !BossList.IgnoreAoE.Contains(StyxWoW.Me.CurrentTarget.Entry) 
                    (StyxWoW.Me.Location.Distance(onLocation(ret)) <= SpellManager.Spells[spell].MaxRange ||
                     SpellManager.Spells[spell].MaxRange == 0) && !DoubleCastEntries.ContainsKey(spell.ToString(CultureInfo.InvariantCulture) + onLocation(ret)
                    ),
                    new Sequence(
                        new Action(ret => SpellManager.Cast(spell)),

                        new DecoratorContinue(ctx => waitForSpell,
                            new WaitContinue(1,
                                ret =>
                                StyxWoW.Me.CurrentPendingCursorSpell != null &&
                                StyxWoW.Me.CurrentPendingCursorSpell.Name == spell, new ActionAlwaysSucceed())),

                        new Action(ret => SpellManager.ClickRemoteLocation(onLocation(ret))),
                        new Action(ret => Logger.InfoLog(String.Format("*{0} at {1:F1}", spell, onLocation(ret)))),
                        new Action(ret => UpdateDoubleCastEntries(spell.ToString(CultureInfo.InvariantCulture) + onLocation(ret), expiryTime))));
        }


        public static Composite PreventDoubleHeal(string spell, double expiryTime, UnitSelectionDelegate onUnit, int HP = 100, Selection<bool> reqs = null)
        {
            return new Decorator(
                ret => (onUnit != null && onUnit(ret) != null && ((reqs != null && reqs(ret)) || (reqs == null)) && onUnit(ret).HealthPercent <= HP)
                    && SpellManager.CanCast(spell, onUnit(ret))
                    && !DoubleCastEntries.ContainsKey(spell.ToString(CultureInfo.InvariantCulture) + onUnit(ret).GetHashCode()),
                new Sequence(
                    new Action(ret => SpellManager.Cast(spell, onUnit(ret))),
                    new Action(ret => Logger.InfoLog(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent))),
                //new Action(ret => Logger.InfoLog(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}% PH: {4:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent, HealManager.PredictedHealthPercent(onUnit(ret), PRSettings.Instance.includeMyHeals)))),
                    new Action(ret => UpdateDoubleCastEntries(spell.ToString(CultureInfo.InvariantCulture) + onUnit(ret).GetHashCode(), expiryTime))));
        }


        public static Composite PreventDoubleHeal(int spell, double expiryTime, UnitSelectionDelegate onUnit, int HP = 100, Selection<bool> reqs = null)
        {
            return new Decorator(
                ret => (onUnit != null && onUnit(ret) != null && ((reqs != null && reqs(ret)) || (reqs == null)) && onUnit(ret).HealthPercent <= HP)
                    && SpellManager.CanCast(spell, onUnit(ret))
                    && !DoubleCastEntries.ContainsKey(spell.ToString(CultureInfo.InvariantCulture) + onUnit(ret).GetHashCode()),
                new Sequence(
                    new Action(ret => SpellManager.Cast(spell, onUnit(ret))),
                    new Action(ret => Logger.InfoLog(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent))),
                    new Action(ret => UpdateDoubleCastEntries(spell.ToString(CultureInfo.InvariantCulture) + onUnit(ret).GetHashCode(), expiryTime))));
        }

        private struct DoubleCastSpell
        {
            public DoubleCastSpell(string spellName, double expiryTime, DateTime currentTime)
                : this()
            {
                DoubleCastSpellName = spellName;
                DoubleCastExpiryTime = expiryTime;
                DoubleCastCurrentTime = currentTime;
            }

            private string DoubleCastSpellName { get; set; }

            public double DoubleCastExpiryTime { get; set; }

            public DateTime DoubleCastCurrentTime { get; set; }
        }

        #endregion Double Cast Shit

        public static TimeSpan GetSpellCooldown(int spell, bool overide = false)
        {
            if (overide)
                return CooldownTracker.CooldownTimeLeft(spell);

            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.CooldownTimeLeft : results.Original.CooldownTimeLeft;
            }

            return TimeSpan.MaxValue;
        }

        public static bool SpellOnCooldown(string spell, bool overide = false)
        {
            if (overide)
                return !CooldownTracker.IsOnCooldown(spell);

            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.Cooldown : results.Original.Cooldown;
            }

            return false;
        }

        public static bool SpellOnCooldown(int spell, bool overide = false)
        {
            if (overide)
                return !CooldownTracker.IsOnCooldown(spell);

            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.Cooldown : results.Original.Cooldown;
            }

            return false;
        }
        #endregion

        #region Cooldown Tracker
        // Actual tracker for the cooldowns.
        public static class CooldownTracker
        {
            public static bool IsOnCooldown(int spell)
            {
                SpellFindResults results;
                if (SpellManager.FindSpell(spell, out results))
                {
                    WoWSpell result = results.Override ?? results.Original;
                    long lastUsed;
                    if (_cooldowns.TryGetValue(result, out lastUsed))
                    {
                        if (DateTime.Now.Ticks < lastUsed)
                        {
                            return result.Cooldown;
                        }
                        return false;
                    }
                }
                return false;
            }

            public static bool IsOnCooldown(string spell)
            {
                SpellFindResults results;
                if (SpellManager.FindSpell(spell, out results))
                {
                    WoWSpell result = results.Override ?? results.Original;
                    long lastUsed;
                    if (_cooldowns.TryGetValue(result, out lastUsed))
                    {
                        if (DateTime.Now.Ticks < lastUsed)
                        {
                            return result.Cooldown;
                        }
                        return false;
                    }
                }
                return false;
            }

            public static TimeSpan CooldownTimeLeft(int spell)
            {
                SpellFindResults results;
                if (SpellManager.FindSpell(spell, out results))
                {
                    WoWSpell result = results.Override ?? results.Original;
                    long lastUsed;
                    if (_cooldowns.TryGetValue(result, out lastUsed))
                    {
                        if (DateTime.Now.Ticks < lastUsed)
                        {
                            return result.CooldownTimeLeft;
                        }
                        return TimeSpan.MaxValue;
                    }
                }
                return TimeSpan.MaxValue;
            }

            public static TimeSpan CooldownTimeLeft(string spell)
            {
                SpellFindResults results;
                if (SpellManager.FindSpell(spell, out results))
                {
                    WoWSpell result = results.Override ?? results.Original;
                    long lastUsed;
                    if (_cooldowns.TryGetValue(result, out lastUsed))
                    {
                        if (DateTime.Now.Ticks < lastUsed)
                        {
                            return result.CooldownTimeLeft;
                        }
                        return TimeSpan.MaxValue;
                    }
                }
                return TimeSpan.MaxValue;
            }

            public static void SpellUsed(string spell)
            {
                SpellFindResults results;
                if (SpellManager.FindSpell(spell, out results))
                {
                    WoWSpell result = results.Override ?? results.Original;
                    _cooldowns[result] = result.CooldownTimeLeft.Ticks + DateTime.Now.Ticks;
                }
            }

            public static void SpellUsed(int spell)
            {
                SpellFindResults results;
                if (SpellManager.FindSpell(spell, out results))
                {
                    WoWSpell result = results.Override ?? results.Original;
                    _cooldowns[result] = result.CooldownTimeLeft.Ticks + DateTime.Now.Ticks;
                }
            }

            private static Dictionary<WoWSpell, long> _cooldowns = new Dictionary<WoWSpell, long>();
        }
        #endregion

        #region Aura Tracking & Caching

        public static WoWAura GetAuraFromName(this WoWUnit unit, string aura, bool isMyAura = false)
        {
            return isMyAura ? unit.GetAllAuras().FirstOrDefault(a => a.Name == aura && a.CreatorGuid == StyxWoW.Me.Guid && a.TimeLeft > TimeSpan.Zero) : unit.GetAllAuras().FirstOrDefault(a => a.Name == aura && a.TimeLeft > TimeSpan.Zero);
        }

        /// <summary>
        /// Gets an Aura by ID. Note: this is a fix for the HB API wraper GetAuraById
        /// </summary>
        public static WoWAura GetAuraFromID(this WoWUnit unit, int aura, bool isMyAura = false)
        {
            return isMyAura ? unit.GetAllAuras().FirstOrDefault(a => a.SpellId == aura && a.CreatorGuid == StyxWoW.Me.Guid && a.TimeLeft > TimeSpan.Zero) : unit.GetAllAuras().FirstOrDefault(a => a.SpellId == aura && a.TimeLeft > TimeSpan.Zero);
        }

        public static uint GetAuraStackCount(string aura)
        {
            var result = StyxWoW.Me.GetAuraFromName(aura);

            if (result != null)
            {
                if (result.StackCount > 0)
                    return result.StackCount;
            }

            return 0;
        }

        public static uint GetAuraStackCount(int spellId)
        {
            var result = StyxWoW.Me.GetAuraFromID(spellId);

            if (result != null)
            {
                if (result.StackCount > 0)
                    return result.StackCount;
            }

            return 0;
        }
        
        // Cached Aura retrieval - Aura's applied by anyone
        public static bool HasAnyCachedAura(this WoWUnit unit, string aura, int stacks, int msuLeft = 0)
        {
            WoWAura cachedResult = null;

            if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                cachedResult = CachedTargetAuras.FirstOrDefault(a => a.Name == aura);

            if (CachedAuras != null && unit == Me)
                cachedResult = CachedAuras.FirstOrDefault(a => a.Name == aura);

            if (cachedResult == null)
                return false;

            if (cachedResult.TimeLeft.TotalMilliseconds > msuLeft)
                return cachedResult.StackCount >= stacks;

            return false;
        }



        public static bool IsGlobalCooldown(DvEnum.LagTolerance allow = DvEnum.LagTolerance.Yes)
        {
#if NO_LATENCY_ISSUES_WITH_GLOBAL_COOLDOWN
            uint latency = allow == DvEnum.LagTolerance.Yes ? StyxWoW.WoWClient.Latency : 0;
            TimeSpan gcdTimeLeft = GcdTimeLeft;
            return gcdTimeLeft.TotalMilliseconds > latency;
#else
            return Spell.FixGlobalCooldown;
#endif
        }

        #region Fix HonorBuddys GCD Handling

#if HONORBUDDY_GCD_IS_WORKING
#else

        private static WoWSpell _gcdCheck = null;

        public static string FixGlobalCooldownCheckSpell
        {
            get
            {
                return _gcdCheck == null ? null : _gcdCheck.Name;
            }
            set
            {
                SpellFindResults sfr;
                if (!SpellManager.FindSpell(value, out sfr))
                {
                    _gcdCheck = null;
                    Logger.DebugLog("GCD check fix spell {0} not known", value);
                }
                else
                {
                    _gcdCheck = sfr.Original;
                    Logger.DebugLog("GCD check fix spell set to: {0}", value);
                }
            }
        }

#endif

        public static bool GcdActive
        {
            get
            {
#if HONORBUDDY_GCD_IS_WORKING
                return SpellManager.GlobalCooldown;
#else
                if (_gcdCheck == null)
                    return SpellManager.GlobalCooldown;

                return _gcdCheck.Cooldown;
#endif
            }
        }

        public static TimeSpan GcdTimeLeft
        {
            get
            {
#if HONORBUDDY_GCD_IS_WORKING
                return SpellManager.GlobalCooldownLeft;
#else
                try
                {
                    if (_gcdCheck != null)
                        return _gcdCheck.CooldownTimeLeft;
                }
                catch (System.AccessViolationException)
                {
                    Logger.WriteFile("GcdTimeLeft: handled access exception, reinitializing gcd spell");
                    GcdInitialize();
                }
                catch (Styx.InvalidObjectPointerException)
                {
                    Logger.WriteFile("GcdTimeLeft: handled invobj exception, reinitializing gcd spell");
                    GcdInitialize();
                }

                // use default value here (reinit should fix _gcdCheck for next call)
                return SpellManager.GlobalCooldownLeft;
#endif
            }
        }

        public static void GcdInitialize()
        {
#if HONORBUDDY_GCD_IS_WORKING
            Logger.WriteDebug("GcdInitialize: using HonorBuddy GCD");
#else
            Logger.DebugLog("FixGlobalCooldownInitialize: using Singular GCD");
            switch (StyxWoW.Me.Class)
            {
                case WoWClass.DeathKnight:
                    FixGlobalCooldownCheckSpell = "Frost Presence";
                    break;
            }

            if (FixGlobalCooldownCheckSpell != null)
                return;

            switch (StyxWoW.Me.Class)
            {
                case WoWClass.DeathKnight:
                    // FixGlobalCooldownCheckSpell = "";
                    break;
            }
#endif
        }

        #endregion

        public static TimeSpan GetAuraTimeLeft(this WoWUnit onUnit, string auraName, bool fromMyAura = true)
        {
            WoWAura wantedAura =
                onUnit.GetAllAuras().Where(a => a != null && a.Name == auraName && a.TimeLeft > TimeSpan.Zero && (!fromMyAura || a.CreatorGuid == StyxWoW.Me.Guid)).FirstOrDefault();

            return wantedAura != null ? wantedAura.TimeLeft : TimeSpan.Zero;
        }

        public static TimeSpan GetAuraTimeLeft(this WoWUnit onUnit, int auraID, bool fromMyAura = true)
        {
            WoWAura wantedAura = onUnit.GetAllAuras()
                .Where(a => a.SpellId == auraID && a.TimeLeft > TimeSpan.Zero && (!fromMyAura || a.CreatorGuid == StyxWoW.Me.Guid)).FirstOrDefault();

            return wantedAura != null ? wantedAura.TimeLeft : TimeSpan.Zero;
        }

        public static bool HasAnyCachedAura(this WoWUnit unit, int aura, int stacks, int msuLeft = 0)
        {
            WoWAura cachedResult = null;

            if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                cachedResult = CachedTargetAuras.FirstOrDefault(a => a.SpellId == aura);

            if (CachedAuras != null && unit == Me)
                cachedResult = CachedAuras.FirstOrDefault(a => a.SpellId == aura);

            if (cachedResult == null)
                return false;

            if (cachedResult.TimeLeft.TotalMilliseconds > msuLeft)
                return cachedResult.StackCount >= stacks;

            return false;
        }

        // Cached Aura retrieval - Aura's applied by StyxWoW.Me.Guid
        public static bool HasCachedAura(this WoWUnit unit, string aura, int stacks, int msuLeft = 0)
        {
            WoWAura cachedResult = null;

            if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                cachedResult = CachedTargetAuras.FirstOrDefault(a => a.Name == aura && a.CreatorGuid == StyxWoW.Me.Guid);

            if (CachedAuras != null && unit == Me)
                cachedResult = CachedAuras.FirstOrDefault(a => a.Name == aura && a.CreatorGuid == StyxWoW.Me.Guid);

            if (cachedResult == null)
                return false;

            if (cachedResult.TimeLeft.TotalMilliseconds > msuLeft)
                return cachedResult.StackCount >= stacks;

            return false;
        }

        public static uint HasCachedStack(this WoWUnit unit, string aura, bool fromMyAura = false)
        {
            {
                if (unit != null)
                {

                    var cachedResult = fromMyAura ? CachedTargetAuras.FirstOrDefault(a => a.Name == aura && a.CreatorGuid == StyxWoW.Me.Guid && a.TimeLeft > TimeSpan.Zero) : CachedTargetAuras.FirstOrDefault(a => a.Name == aura && a.TimeLeft > TimeSpan.Zero);

                    if (cachedResult == null)
                        return 0;

                    return cachedResult.StackCount;
                }
                return 0;
            }
        }
        
        //this WoWUnit unit, string aura, bool fromMyAura = false, int expiry = 1000

        public static bool HasCachedAura(this WoWUnit unit, int aura, int stacks, int msuLeft = 0)
        {
            WoWAura cachedResult = null;

            if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                cachedResult = CachedTargetAuras.FirstOrDefault(a => a.SpellId == aura && a.CreatorGuid == StyxWoW.Me.Guid);

            if (CachedAuras != null && unit == Me)
                cachedResult = CachedAuras.FirstOrDefault(a => a.SpellId == aura && a.CreatorGuid == StyxWoW.Me.Guid);

            if (cachedResult == null)
                return false;

            if (cachedResult.TimeLeft.TotalMilliseconds > msuLeft)
                return cachedResult.StackCount >= stacks;

            return false;
        }

        public static double GetAuraTimeLeft(string aura)
        {
            return GetAuraTimeLeft(aura, StyxWoW.Me);
        }

        public static double GetAuraTimeLeft(string aura, WoWUnit onUnit)
        {
            if (onUnit != null)
            {
                var result = onUnit.GetAuraByName(aura);
                if (result != null)
                {
                    if (result.TimeLeft.TotalSeconds > 0)
                        return result.TimeLeft.TotalSeconds;
                }
            }
            return 0;
        }

        public static double GetAuraTimeLeft(int aura)
        {
            return GetAuraTimeLeft(aura, StyxWoW.Me);
        }

        public static double GetAuraTimeLeft(int aura, WoWUnit onUnit)
        {
            if (onUnit != null)
            {
                var result = onUnit.GetAuraById(aura);
                if (result != null)
                {
                    if (result.TimeLeft.TotalSeconds > 0)
                        return result.TimeLeft.TotalSeconds;
                }
            }
            return 0;
        }

        public static double GetMyAuraTimeLeft(string aura, WoWUnit onUnit)
        {
            if (onUnit != null)
            {
                var result = onUnit.GetAllAuras().FirstOrDefault(a => a.Name == aura && a.CreatorGuid == Me.Guid);
                if (result != null && result.TimeLeft.TotalSeconds > 0)
                    return result.TimeLeft.TotalSeconds;
            }
            return 0;
        }

        public static double GetMyAuraTimeLeft(int aura, WoWUnit onUnit)
        {
            if (onUnit != null)
            {
                var result = onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == aura && a.CreatorGuid == Me.Guid);
                if (result != null && result.TimeLeft.TotalSeconds > 0)
                    return result.TimeLeft.TotalSeconds;
            }
            return 0;
        }

        public static double GetMyAuraTimeLeft(string[] aura, WoWUnit onUnit)
        {
            if (onUnit != null)
            {
                var auras = onUnit.GetAllAuras();
                var hashes = new HashSet<string>(aura);
                var result = auras.FirstOrDefault(a => hashes.Contains(a.Name) && a.CreatorGuid == Me.Guid);
                if (result != null && result.TimeLeft.TotalSeconds > 0)
                    return result.TimeLeft.TotalSeconds;
            }
            return 0;
        }

        public static bool HasMyAura(this WoWUnit unit, string aura)
        {
            return unit.GetAllAuras().Any(a => a.Name == aura && a.CreatorGuid == Me.Guid);
        }

        public static bool HasMyAura(this WoWUnit unit, int spellId)
        {
            return unit.GetAllAuras().Any(a => a.SpellId == spellId && a.CreatorGuid == Me.Guid);
        }

        public static double GetMyAuraTimeLeft(HashSet<int> aura, WoWUnit onUnit)
        {
            if (onUnit != null)
            {
                var auras = onUnit.GetAllAuras();
                var result = auras.FirstOrDefault(a => aura.Contains(a.SpellId) && a.CreatorGuid == Me.Guid);
                if (result != null && result.TimeLeft.TotalSeconds > 0)
                    return result.TimeLeft.TotalSeconds;
            }
            return 0;
        }

        public static IEnumerable<WoWAura> CachedTargetAuras = new List<WoWAura>();
        public static IEnumerable<WoWAura> CachedAuras = new List<WoWAura>();
        public static void GetCachedAuras()
        {
            if (Me.CurrentTarget != null) CachedTargetAuras = Me.CurrentTarget.GetAllAuras();
            CachedAuras = Me.GetAllAuras();
        }
        #endregion
    }
}
