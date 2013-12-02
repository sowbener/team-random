// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable FieldCanBeMadeReadOnly.Local

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
using Shammy.Helpers;
using System;
using Logger = Shammy.Helpers.SmLogger;
using U = Shammy.Core.SmUnit;

namespace Shammy.Core
{
    internal static class SmSpell
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
                            SmLogger.CombatLogO("Casting: " + spell);
                        }));
        }



        // Casting by Integer
        public static Composite Cast(int spell, Selection<bool> reqs = null)
        {
            return Cast(spell, ret => StyxWoW.Me.CurrentTarget, reqs);
        }

        public static bool SpellOnCooldowns(int id)
        {
            return GetSpellCooldowns(id) > 0;
        }

        public static double GetSpellCooldowns(int spell)
        {
            SpellFindResults results;
            if (!SpellManager.FindSpell(spell, out results)) return TimeSpan.MaxValue.TotalSeconds;
            return results.Override != null
                ? results.Override.CooldownTimeLeft.TotalSeconds - SpellManager.GlobalCooldownLeft.TotalSeconds - 0.1
                : results.Original.CooldownTimeLeft.TotalSeconds - SpellManager.GlobalCooldownLeft.TotalSeconds - 0.1;
        }

        public static Composite Cast(int spell, UnitSelectionDelegate onUnit, Selection<bool> reqs = null)
        {
            return
                new Decorator(ret => ((reqs != null && !SpellOnCooldowns(spell) && reqs(ret)) || (reqs == null)) && onUnit != null && onUnit(ret) != null && !SpellOnCooldowns(spell) && SpellManager.CanCast(spell, onUnit(ret)),
                    new Action(ret =>
                        {
                            SpellManager.Cast(spell, onUnit(ret));
                            CooldownTracker.SpellUsed(spell);
                            SmLogger.CombatLogO("Casting: " + spell);
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
                                SmLogger.CombatLogO("Casting: " + spell);
                            }
                        )));
        }
        #endregion

        #region InterruptShitet
        internal static WoWSpell CastOrChanneledSpell(this WoWUnit u)
        {
            if (u == null) return WoWSpell.FromId(0);
            return u.CastingSpell ?? (u.ChanneledSpell);
        }

        internal static int CurrentCastorChannelId(this WoWUnit u)
        {
            if (u == null) return 0;

            return u.IsCasting ? u.CastingSpellId : (u.IsChanneling ? u.ChanneledCastingSpellId : 0);
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


        public static uint GetAuraStack(WoWUnit unit, int spellId)
        {
            if (unit != null)
            {
                var wantedAura = Me.GetAllAuras().FirstOrDefault(a => a.SpellId == spellId && a.StackCount > 0 && a.CreatorGuid == Me.Guid);
                return wantedAura != null ? wantedAura.StackCount : 0;
            }
            return 0;
        }
        #endregion

        #region Cast - Multi DoT

        /// <summary> Multi-DoT targets within range of target</summary>
        public static Composite MultiDoT(string spellName, WoWUnit unit, Selection<bool> reqs = null)
        {
            return MultiDoT(spellName, unit, 15, 1, reqs);
        }

        /// <summary> Multi-DoT targets within range of target</summary>
        public static Composite MultiDoT(string spellName, WoWUnit unit, double radius, double refreshDurationRemaining, Selection<bool> reqs = null)
        {
            WoWUnit dotTarget = null;
            return new PrioritySelector(
                        new Decorator(ret => unit != null && ((reqs != null && reqs(ret)) || reqs == null),
                              new PrioritySelector(ctx => dotTarget = GetMultiDoTTarget(unit, spellName, radius, refreshDurationRemaining),
                                  PreventDoubleCast(spellName, GetSpellCastTime(spellName) + 0.5, on => dotTarget, ret => dotTarget != null))));
        }

        /// <summary> Multi-DoT targets within range of target</summary>
        public static Composite PreventDoubleMultiDoT(string spellName, double expiryTime, WoWUnit unit, double radius, double refreshDurationRemaining, Selection<bool> reqs = null)
        {
            WoWUnit dotTarget = null;
            return new PrioritySelector(
                        new Decorator(ret => unit != null && ((reqs != null && reqs(ret)) || reqs == null),
                              new PrioritySelector(ctx => dotTarget = GetMultiDoTTarget(unit, spellName, radius, refreshDurationRemaining),
                                  PreventDoubleCast(spellName, expiryTime, on => dotTarget, ret => dotTarget != null))));
        }

        public static double GetSpellCastTime(string spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.CastTime / 1000.0 : results.Original.CastTime / 1000.0;
            }

            return 99999.9;
        }

        public static double GetSpellCastTime(int spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.CastTime / 1000.0 : results.Original.CastTime / 1000.0;
            }
            return 99999.9;
        }

        #endregion

        #region MultiDotting

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

        internal static WoWUnit GetMultiDoTTarget(WoWUnit unit, string debuff, double radius, double refreshDurationRemaining)
        {
            // find unit without our debuff
            var dotTarget = U.NearbyAttackableUnits(unit.Location, radius)
                .Where(x => x != null)
                .OrderByDescending(x => x.HealthPercent)
                .FirstOrDefault(x => !x.HasMyAura(debuff) && x.InLineOfSpellSight);

            if (dotTarget == null)
            {
                // If we couldn't find one without our debuff, then find ones where debuff is about to expire.
                dotTarget = U.NearbyAttackableUnits(unit.Location, radius)
                            .Where(x => x != null)
                            .OrderByDescending(x => x.HealthPercent)
                            .FirstOrDefault(x => (x.HasMyAura(debuff) && GetMyAuraTimeLeft(debuff, x) < refreshDurationRemaining) && x.InLineOfSpellSight);
            }
            return dotTarget;
        }

        #endregion MultiDotting


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

        public static bool HasAnyAura(this WoWUnit unit, params string[] auraNames)
        {
            var auras = unit.GetAllAuras();
            var hashes = new HashSet<string>(auraNames);
            return auras.Any(a => hashes.Contains(a.Name));
        }

        public static bool HasAnyAura(this WoWUnit unit, params int[] auraIDs)
        {
            return auraIDs.Any(unit.HasAura);
        }

        public static bool HasAnyAura(this WoWUnit unit, HashSet<int> auraIDs)
        {
            var auras = unit.GetAllAuras();
            return auras.Any(a => auraIDs.Contains(a.SpellId));
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

        public static float SpellDistance(this WoWUnit unit, WoWUnit other = null)
        {
            // abort if mob null
            if (unit == null)
                return 0;

            // optional arg implying Me, then make sure not Mob also
            if (other == null)
                other = StyxWoW.Me;

            // pvp, then keep it close
            float dist = other.Location.Distance(unit.Location);
            dist -= other.CombatReach + unit.CombatReach;
            return Math.Max(0, dist);
        }

        /// <summary>
        /// get the  combined base spell and hitbox range of <c>unit</c>.
        /// </summary>
        /// <param name="unit">unit</param>
        /// <param name="baseSpellRange"></param>
        /// <param name="other">Me if null, otherwise second unit</param>
        /// <returns></returns>
        public static float SpellRange(this WoWUnit unit, float baseSpellRange, WoWUnit other = null)
        {
            // abort if mob null
            if (unit == null)
                return 0;

            // optional arg implying Me, then make sure not Mob also
            if (other == null)
                other = StyxWoW.Me;
            return baseSpellRange + other.CombatReach + unit.CombatReach;
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
