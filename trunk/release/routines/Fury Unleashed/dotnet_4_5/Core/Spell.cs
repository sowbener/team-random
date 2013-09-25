// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable FieldCanBeMadeReadOnly.Local
using CommonBehaviors.Actions;
using FuryUnleashed.Shared.Helpers;
using FuryUnleashed.Shared.Utilities;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;

namespace FuryUnleashed.Core
{
    internal static class Spell
    {
        #region Delegates
        // Public delegates
        public delegate WoWUnit UnitSelectionDelegate(object context);
        public delegate WoWPoint LocationRetriever(object context);
        public static LocalPlayer Me { get { return StyxWoW.Me; } }
        public static WoWSpell GcdSpell { get; set; }

        // Internal delegates
        internal delegate T Selection<out T>(object context);
        internal static readonly uint ClientLatency = StyxWoW.WoWClient.Latency;
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
                            Logger.CombatLogO("Casting: " + spell + " on " + onUnit(ret).SafeName);
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
                            Logger.CombatLogO("Casting: " + WoWSpell.FromId(spell).Name + " on " + onUnit(ret).SafeName);
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
                                Logger.CombatLogO("Casting: " + spell);
                            }
                        )));
        }

        // Casting on Ground by Int
        public static Composite CastOnGround(int spell, LocationRetriever onLocation)
        {
            return CastOnGround(spell, onLocation, ret => true);
        }

        public static Composite CastOnGround(int spell, LocationRetriever onLocation, CanRunDecoratorDelegate requirements, bool waitForSpell = false)
        {
            return
                new Decorator(ret => onLocation != null && requirements(ret) && SpellManager.CanCast(spell) && (StyxWoW.Me.Location.Distance(onLocation(ret)) <= WoWSpell.FromId(spell).MaxRange || WoWSpell.FromId(spell).MaxRange == 0),
                    new Sequence(
                       new Action(ret => SpellManager.Cast(spell)),
                        new DecoratorContinue(ctx => waitForSpell,
                            new WaitContinue(1, ret =>
                                StyxWoW.Me.CurrentPendingCursorSpell != null &&
                                StyxWoW.Me.CurrentPendingCursorSpell.Id == spell, new ActionAlwaysSucceed())),
                        new Action(ret =>
                        {
                            SpellManager.ClickRemoteLocation(onLocation(ret));
                            CooldownTracker.SpellUsed(spell);
                            Logger.CombatLogO("Casting: " + spell);
                        }
                        )));
        }
        #endregion

        #region Sequence Casting Methods
        public class SequenceCast
        {
            private readonly List<Composite> _children;
            private int _current;
            private readonly int _endSequence;
            private bool _sequenceRunning;

            public SequenceCast(List<Composite> l)
            {
                _children = l;
                _current = 0;
                _endSequence = l.Count();
                _sequenceRunning = false;
            }

            public Composite Execute(Selection<bool> reqs = null)
            {
                return new Decorator(ret => _sequenceRunning || ((reqs != null && reqs(ret)) || (reqs == null)), new PrioritySelector(WaitForCast, ExecuteCurrentNode()));
            }

            private static Composite WaitForCast
            {
                get { return new Decorator(ret => StyxWoW.Me.IsCasting || SpellManager.GlobalCooldown, new Action(ret => RunStatus.Success)); }
            }

            private Composite ExecuteCurrentNode()
            {
                return new Action(context =>
                {
                    //Check for end of sequence 
                    if (_current >= _endSequence)
                    {
                        _current = 0;
                        _sequenceRunning = false;
                        return RunStatus.Failure;
                    }

                    //Sequence isnt over, try to run next node 
                    var node = _children.ElementAt(_current);
                    node.Start(context);
                    while (node.Tick(context) == RunStatus.Running)
                    {
                        //Run Node 
                    }
                    node.Stop(context);

                    //Node Failed, so sequence over! 
                    if (node.LastStatus == RunStatus.Failure)
                    {
                        _current = 0;
                        _sequenceRunning = false;
                        return RunStatus.Failure;
                    }

                    //Node Succeded, Increment!! 
                    _current++;
                    _sequenceRunning = true;
                    return RunStatus.Success;
                });
            }
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
        // Cached Aura retrieval - Aura's applied by anyone
        public static bool HasAnyCachedAura(this WoWUnit unit, string aura, int stacks, int msuLeft = 0)
        {
            WoWAura cachedResult = null;

            //Used for debugging
            //if (cachedResult.Name == "Skull Banner")
            //    Logging.Write("Caching Debugger: WoWUnit {0} - Aura {1} with {2} stacks - Msuleft {3}", unit, aura, stacks, msuLeft);

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

        public static bool HasAnyCachedAura(this WoWUnit unit, int aura, int stacks, int msuLeft = 0)
        {
            WoWAura cachedResult = null;

            //Used for debugging
            //if (cachedResult.Name == "Skull Banner")
            //    Logging.Write("Caching Debugger: WoWUnit {0} - Aura {1} with {2} stacks - Msuleft {3}", unit, aura, stacks, msuLeft);

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

            //Used for debugging
            //if (cachedResult.Name == "Skull Banner")
            //    Logging.Write("Caching Debugger: WoWUnit {0} - Aura {1} with {2} stacks - Msuleft {3}", unit, aura, stacks, msuLeft);

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

            //Used for debugging
            //if (cachedResult.Name == "Skull Banner")
            //    Logging.Write("Caching Debugger: WoWUnit {0} - Aura {1} with {2} stacks - Msuleft {3}", unit, aura, stacks, msuLeft);

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

        public static IEnumerable<WoWAura> CachedAuras = new List<WoWAura>();
        public static IEnumerable<WoWAura> CachedTargetAuras = new List<WoWAura>();
        public static void GetCachedAuras()
        {
            if (Me.CurrentTarget != null) 
                CachedTargetAuras = Me.CurrentTarget.GetAllAuras();
                CachedAuras = Me.GetAllAuras();
        }
        #endregion

        #region Non-Cached Aura Functions
        public static bool HasAura(this WoWUnit unit, string aura, int stacks = 0, bool isMyAura = false, int msLeft = 0)
        {
            var result = unit.GetAuraFromName(aura, isMyAura);

            if (result == null)
                return false;

            if (result.TimeLeft.TotalMilliseconds > msLeft)
                return result.StackCount >= stacks;

            return false;
        }

        public static bool HasAura(this WoWUnit unit, int aura, int stacks = 0, bool isMyAura = false, int msLeft = 0)
        {
            var result = unit.GetAuraFromId(aura, isMyAura);

            if (result == null)
                return false;

            if (result.TimeLeft.TotalMilliseconds > msLeft)
                return result.StackCount >= stacks;

            return false;
        }

        /// <summary>
        /// Gets an Aura by Name. Note: this is a fix for the HB API wrapper GetAuraByName
        /// </summary>
        public static WoWAura GetAuraFromName(this WoWUnit unit, string aura, bool isMyAura = false)
        {
            return isMyAura ? unit.GetAllAuras().FirstOrDefault(a => a.Name == aura && a.CreatorGuid == StyxWoW.Me.Guid && a.TimeLeft > TimeSpan.Zero) : unit.GetAllAuras().FirstOrDefault(a => a.Name == aura && a.TimeLeft > TimeSpan.Zero);
        }

        /// <summary>
        /// Gets an Aura by ID. Note: this is a fix for the HB API wrapper GetAuraById
        /// </summary>
        public static WoWAura GetAuraFromId(this WoWUnit unit, int aura, bool isMyAura = false)
        {
            return isMyAura ? unit.GetAllAuras().FirstOrDefault(a => a.SpellId == aura && a.CreatorGuid == StyxWoW.Me.Guid && a.TimeLeft > TimeSpan.Zero) : unit.GetAllAuras().FirstOrDefault(a => a.SpellId == aura && a.TimeLeft > TimeSpan.Zero);
        }

        public static bool HasAuraWithMechanic(this WoWUnit unit, params WoWSpellMechanic[] mechanics)
        {
            var auras = unit.GetAllAuras();
            return auras.Any(a => mechanics.Contains(a.Spell.Mechanic));
        }

        public static bool HasAuraWithMechanic(this WoWUnit unit, params WoWApplyAuraType[] applyType)
        {
            var auras = unit.GetAllAuras();
            return auras.Any(a => a.Spell.SpellEffects.Any(se => applyType.Contains(se.AuraType)));
        }
        #endregion

        #region GCD Detection
        public static bool IsGlobalCooldown()
        {
            using (new PerformanceLogger("IsGlobalCooldown"))
            {
                return (GlobalCooldownLeft.TotalMilliseconds - 10) > StyxWoW.WoWClient.Latency << 1;
            }
        }

        public static void InitGcdSpell()
        {
            if (GetGlobalCooldownSpell != null)
            {
                Logger.DiagLogW("FU: GCD Spell is set to {0}", GetGlobalCooldownSpell);
                GcdSpell = GetGlobalCooldownSpell;
            }
        }

        private static TimeSpan GlobalCooldownLeft
        {
            get { return GcdSpell != null ? GcdSpell.CooldownTimeLeft : SpellManager.GlobalCooldownLeft; }
        }

        public static WoWSpell GetGlobalCooldownSpell
        {
            get { return SpellManager.Spells.FirstOrDefault(s => GcdSpells.Contains(s.Value.Id)).Value; }
        }

        private static readonly HashSet<int> GcdSpells = new HashSet<int>
        {
            7386, // Sunder Armor
            78 // Heroic Strike
        };
        #endregion GCD
    }
}
