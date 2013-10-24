﻿// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable FieldCanBeMadeReadOnly.Local
using CommonBehaviors.Actions;
using FuryUnleashed.Core.Helpers;
using FuryUnleashed.Core.Utilities;
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
            return new Decorator(ret => (onUnit != null && onUnit(ret) != null && (reqs == null || reqs(ret)) && SpellManager.CanCast(spell, onUnit(ret))),
                    new Action(ret =>
                        {
                            SpellManager.Cast(spell, onUnit(ret));
                            CooldownTracker.SpellUsed(spell);
                            Logger.CombatLogOr("Casting: " + spell + " on " + onUnit(ret).SafeName);
                        }));
        }

        // Casting by Integer
        public static Composite Cast(int spell, Selection<bool> reqs = null)
        {
            return Cast(spell, ret => StyxWoW.Me.CurrentTarget, reqs);
        }

        public static Composite Cast(int spell, UnitSelectionDelegate onUnit, Selection<bool> reqs = null)
        {
            return new Decorator(ret => (onUnit != null && onUnit(ret) != null && (reqs == null || reqs(ret)) && SpellManager.CanCast(spell, onUnit(ret))),
                    new Action(ret =>
                    {
                        SpellManager.Cast(spell, onUnit(ret));
                        CooldownTracker.SpellUsed(spell);
                        Logger.CombatLogOr("Casting: " + WoWSpell.FromId(spell).Name + " on " + onUnit(ret).SafeName);
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
                                Logger.CombatLogOr("Casting: " + spell);
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
                            Logger.CombatLogOr("Casting: " + spell);
                        }
                        )));
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
                Logger.DiagLogWh("FU: GCD Spell is set to {0}", GetGlobalCooldownSpell);
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

        #region Cached & Non-Cached Aura Functions
        public static IEnumerable<WoWAura> CachedAuras = new List<WoWAura>();
        public static IEnumerable<WoWAura> CachedTargetAuras = new List<WoWAura>();

        /// <summary>
        /// Gets aura's on StyxWoW.Me and StyxWoW.Me.CurrentTarget and adds them to CachedAuras & CachedTargetAuras lists for one traverse.
        /// </summary>
        public static void GetCachedAuras()
        {
            using (new PerformanceLogger("GetCachedAuras"))
            {
                try
                {
                    if (Unit.IsViable(Me)) { CachedAuras = Me.GetAllAuras(); }
                    if (Unit.IsViable(Me.CurrentTarget)) { CachedTargetAuras = Me.CurrentTarget.GetAllAuras(); }
                }
                catch (Exception getcachedauraException)
                {
                    Logger.DiagLogFb("FU: Failed to retrieve aura's - {0}", getcachedauraException);
                }
            }
        }

        /// <summary>
        /// HasAura extention - String
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, etc)</param>
        /// <param name="auraname">Full Auraname</param>
        /// <param name="stacks">Amount of Stacks on the Aura</param>
        /// <param name="msuLeft">Timeleft on the Aura</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <returns></returns>
        public static bool HasAura(WoWUnit unit, string auraname, int stacks = 0, int msuLeft = 0, bool isFromMe = true, bool cached = true)
        {
            using (new PerformanceLogger("HasAura"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return false;

                    WoWAura auraresult = null;

                    if (!cached)
                    {
                        auraresult = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Me.Guid) : unit.GetAllAuras().FirstOrDefault(a => a.Name == auraname);
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        auraresult = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Me.Guid) : CachedTargetAuras.FirstOrDefault(a => a.Name == auraname);
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        auraresult = isFromMe ? CachedAuras.FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Me.Guid) : CachedAuras.FirstOrDefault(a => a.Name == auraname);
                    }

                    if (auraresult == null) return false;

                    if (auraresult.TimeLeft.TotalMilliseconds > msuLeft)
                        return auraresult.StackCount >= stacks;
                }
                catch (Exception) { return false; }
            }
            return false;
        }

        /// <summary>
        /// HasAura extention - ID
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, etc)</param>
        /// <param name="auraId">AuraBook.Auraname or Aura ID</param>
        /// <param name="stacks">Amount of Stacks on the Aura</param>
        /// <param name="msuLeft">Timeleft on the Aura</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <returns></returns>
        public static bool HasAura(WoWUnit unit, int auraId, int stacks = 0, int msuLeft = 0, bool isFromMe = true, bool cached = true)
        {
            using (new PerformanceLogger("HasAura"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return false;

                    WoWAura auraresult = null;

                    if (!cached)
                    {
                        auraresult = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        auraresult = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId);
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        auraresult = isFromMe ? CachedAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : CachedAuras.FirstOrDefault(a => a.SpellId == auraId);
                    }

                    if (auraresult == null) return false;

                    if (auraresult.TimeLeft.TotalMilliseconds > msuLeft)
                        return auraresult.StackCount >= stacks;
                }
                catch (Exception) { return false; }
            }
            return false;
        }

        /// <summary>
        /// Gets fading aura timeleft  - ID
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, Etc)</param>
        /// <param name="auraId">AuraBook.Auraname or Aura ID</param>
        /// <param name="fadingtime">Timeleft on the Aura</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <returns></returns>
        public static bool FadingAura(WoWUnit unit, int auraId, int fadingtime, bool isFromMe = true, bool cached = true)
        {
            using (new PerformanceLogger("FadingAura"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return false;

                    if (!cached)
                    {
                        WoWAura aura = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        WoWAura aura = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        WoWAura aura = isFromMe ? CachedAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : CachedAuras.FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);                        
                    }
                }
                catch (Exception) { return false; }
            }
            return false;
        }

        /// <summary>
        /// Gets remaining aura timeleft  - ID
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, Etc)</param>
        /// <param name="auraId">AuraBook.Auraname or Aura ID</param>
        /// <param name="remainingtime">Timeleft on the Aura</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <returns></returns>
        public static bool RemainingAura(WoWUnit unit, int auraId, int remainingtime, bool isFromMe = true, bool cached = true)
        {
            using (new PerformanceLogger("RemainingAura"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return false;

                    if (!cached)
                    {
                        WoWAura aura = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft >= TimeSpan.FromMilliseconds(remainingtime);
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        WoWAura aura = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft >= TimeSpan.FromMilliseconds(remainingtime);
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        WoWAura aura = isFromMe ? CachedAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : CachedAuras.FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft >= TimeSpan.FromMilliseconds(remainingtime);
                    }
                }
                catch (Exception) { return false; }
            }
            return false;
        }
        #endregion
    }
}