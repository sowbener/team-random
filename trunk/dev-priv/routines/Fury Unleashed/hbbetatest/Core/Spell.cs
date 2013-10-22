// ReSharper disable ImplicitlyCapturedClosure
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

        #region Aura Functions
        public static bool FadingAura(WoWUnit onUnit, int auraId, int fadingtime, bool isFromMe = true)
        {
            using (new PerformanceLogger("FadingAura"))
            {
                try
                {
                    if (onUnit == null) return false;
                    WoWAura aura = isFromMe ? onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                    return aura != null && aura.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
                }
                catch (Exception) { return false; }
            }
        }

        public static double GetAuraTimeLeft(WoWUnit onUnit, int auraId, bool isFromMe = true)
        {
            using (new PerformanceLogger("GetAuraTimeLeft"))
            {
                try
                {
                    if (onUnit == null) return 0;
                    WoWAura aura = isFromMe ? onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                    return aura != null ? aura.TimeLeft.TotalMilliseconds : 0;
                }
                catch { return 0; }
            }
        }

        public static bool HasAura(WoWUnit onUnit, int auraId, bool isFromMe = false)
        {
            using (new PerformanceLogger("HasAura"))
            {
                try
                {
                    if (onUnit == null) return false;
                    WoWAura aura = isFromMe ? onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                    return aura != null;
                }
                catch (Exception) { return false; }
            }
        }

        public static bool HasAnyAura(WoWUnit onUnit, bool isFromMe = true, params int[] auraIDs)
        {
            using (new PerformanceLogger("HasAnyAura"))
            {
                try
                {
                    if (onUnit == null) return false;
                    WoWAura aura = isFromMe ? onUnit.GetAllAuras().FirstOrDefault(a => auraIDs.Contains(a.SpellId) && a.CreatorGuid == Me.Guid) : onUnit.GetAllAuras().FirstOrDefault(a => auraIDs.Contains(a.SpellId));
                    return aura != null;
                }
                catch (Exception) { return false; }
            }
        }

        public static bool RemainingAura(WoWUnit onUnit, int auraId, int remainingtime, bool isFromMe = true)
        {
            using (new PerformanceLogger("RemainingAura"))
            {
                try
                {
                    if (onUnit == null) return false;
                    WoWAura aura = isFromMe ? onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                    return aura != null && aura.TimeLeft >= TimeSpan.FromMilliseconds(remainingtime);
                }
                catch (Exception) { return false; }
            }
        }

        public static uint StackCount(WoWUnit onUnit, int auraId, bool isFromMe = true)
        {
            using (new PerformanceLogger("StackCount"))
            {
                try
                {
                    if (onUnit == null) return 0;
                    WoWAura aura = isFromMe ? onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Me.Guid) : onUnit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                    return aura != null ? aura.StackCount : 0;
                }
                catch { return 0; }
            }
        }
        #endregion
    }
}
