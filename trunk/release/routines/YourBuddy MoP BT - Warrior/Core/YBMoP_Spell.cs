// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable FieldCanBeMadeReadOnly.Local

using CommonBehaviors.Actions;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using YBMoP_BT_Warrior.Helpers;
using Action = Styx.TreeSharp.Action;

namespace YBMoP_BT_Warrior.Core
{
    internal static class YBSpell
    {
        #region YBMoP BT - Delegates
        // Public delegates
        public delegate WoWUnit UnitSelectionDelegate(object context);
        public delegate WoWPoint LocationRetriever(object context);
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        // Internal delegates
        internal delegate T Selection<out T>(object context);
        #endregion

        #region YBMoP BT - Casting Methods
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
                            YBLogger.CombatLogO("Casting: " + spell);
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
                            YBLogger.CombatLogO("Casting: " + spell);
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
                            }
                        )));
        }
        #endregion

        #region Cooldown Tracking
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

        #region YBMoP BT - Aura Tracking & Caching
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
