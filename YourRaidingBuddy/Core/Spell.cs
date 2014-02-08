// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable CompareOfFloatsByEqualityOperator
#define NO_LATENCY_ISSUES_WITH_GLOBAL_COOLDOWN
using CommonBehaviors.Actions;
using YourBuddy.Core.Helpers;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using YourBuddy.Core.Utilities;
using Styx;
using System;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System.Collections.Generic;
using Action = Styx.TreeSharp.Action;
using System.Text;
using Styx.Helpers;
using Styx.Patchables;
using Styx.WoWInternals.World;
using Lua = YourBuddy.Core.Helpers.LuaClass;

namespace YourBuddy.Core
{
    internal static class Spell
    {
        #region Delegates
        // Public delegates
        public delegate WoWUnit UnitSelectionDelegate(object context);
        public delegate WoWPoint LocationRetriever(object context);
        public delegate bool SimpleBooleanDelegate(object context);
        public static LocalPlayer Me { get { return StyxWoW.Me; } }
        public static WoWSpell GcdSpell { get; set; }

        // Internal delegates
        internal delegate T Selection<out T>(object context);
        internal static readonly uint ClientLatency = StyxWoW.WoWClient.Latency;
        #endregion

        #region Casting Methods
        /// <summary>
        /// Casting method for regular casting - string
        /// </summary>
        /// <param name="spellname">String SpellName to Cast</param>
        /// <param name="reqs">-</param>
        /// <param name="failThrough">This at true continues the tree as it returns a runstatus.failure - Required for chain-casting like popping multiple CD's!</param>
        /// <returns></returns>
        public static Composite Cast(string spellname, Selection<bool> reqs = null, bool failThrough = false)
        {
            return Cast(spellname, ret => StyxWoW.Me.CurrentTarget, reqs, failThrough);
        }

        public static Composite Cast(string spellname, UnitSelectionDelegate onUnit, Selection<bool> reqs = null, bool failThrough = false)
        {
            return
                new Decorator(ret => (onUnit != null && onUnit(ret) != null && (reqs == null || reqs(ret)) && SpellManager.CanCast(spellname, onUnit(ret))),
                    new Action(ret =>
                    {
                        if (SpellManager.Cast(spellname, onUnit(ret)))
                        {
                            CooldownTracker.SpellUsed(spellname);
                            Logger.CombatLogOr("Casting: " + spellname + " on " + onUnit(ret).SafeName);
                            if (!failThrough)
                                return RunStatus.Success;
                        }
                        return RunStatus.Failure;
                    }));
        }


        public static Composite CastHack(string spellname, UnitSelectionDelegate onUnit, Selection<bool> reqs = null, bool failThrough = false)
        {
            return
                new Decorator(ret => (onUnit != null && onUnit(ret) != null && (reqs == null || reqs(ret)) && CanCastHack(spellname, onUnit(ret))),
                    new Action(ret =>
                    {
                        if (SpellManager.Cast(spellname, onUnit(ret)))
                        {
                            CooldownTracker.SpellUsed(spellname);
                            Logger.CombatLogOr("Casting: " + spellname + " on " + onUnit(ret).SafeName);
                            if (!failThrough)
                                return RunStatus.Success;
                        }
                        return RunStatus.Failure;
                    }));
        }

        /// <summary>
        /// Casting method for regular casting - int
        /// </summary>
        /// <param name="spellid">Integer Spell to Cast</param>
        /// <param name="reqs">-</param>
        /// <param name="failThrough">This at true continues the tree as it returns a runstatus.failure - Required for chain-casting like popping multiple CD's!</param>
        /// <returns></returns>
        public static Composite Cast(int spellid, Selection<bool> reqs = null, bool failThrough = false)
        {
            return Cast(spellid, ret => StyxWoW.Me.CurrentTarget, reqs, failThrough);
        }

        public static Composite Cast(int spellid, UnitSelectionDelegate onUnit, Selection<bool> reqs = null, bool failThrough = false)
        {
            return
                new Decorator(ret => ((reqs != null && reqs(ret)) || (reqs == null)) && onUnit != null && onUnit(ret) != null && SpellManager.CanCast(spellid, onUnit(ret)),
                    new Action(ret =>
                    {
                        if (SpellManager.Cast(spellid, onUnit(ret)))
                        {
                            CooldownTracker.SpellUsed(spellid);
                            Logger.CombatLogOr("Casting: " + WoWSpell.FromId(spellid).Name + " on " + onUnit(ret).SafeName);
                            if (!failThrough)
                                return RunStatus.Success;
                        }
                        return RunStatus.Failure;
                    }));
        }

        /// <summary>
        /// Casting method used for MultiDotting. Not using any caching, so might decrease performance!
        /// </summary>
        /// <param name="spellid">ID of the spell to be cast - eg SpellBook.Bloodthirst</param>
        /// <param name="debuffid">ID of the debuff to find on a possible multidot target - eg AuraBook.DeepWounds</param>
        /// <param name="auratimeleft">Timeleft on the aura in order to refresh it if no unit without the aura is found - Default 1500ms</param>
        /// <param name="radius">Radius - Default 5 yards</param>
        /// <param name="conedegrees">Degrees for the Facing Cone (Me.IsSafelyFacing() check) - Default 160</param>
        /// <param name="reqs">-</param>
        /// <param name="failThrough">This at true continues the tree as it returns a runstatus.failure - Required for chain-casting like popping multiple CD's!</param>
        /// <returns>-</returns>
        public static Composite MultiDoT(int spellid, int debuffid, int auratimeleft = 1500, double radius = 5, int conedegrees = 160, Selection<bool> reqs = null, bool failThrough = false)
        {
            using (new PerformanceLogger("MultiDoT"))
            {
                WoWUnit target = null;
                try
                {
                    return new Decorator(ret => ((reqs != null && reqs(ret)) || reqs == null) && Unit.NearbyAttackableUnitsCount > 1,
                        new PrioritySelector(dot => target = Unit.MultiDotUnit(debuffid, auratimeleft, radius, conedegrees),
                            new Decorator(ctx => Unit.IsViable(target),
                                Cast(spellid, on => target, ret => Unit.IsViable(target)))));
                }
                catch (Exception ex)
                {
                    return new Action(ret =>
                        {
                            Logger.DiagLogPu("Exception MultiDot {0}", ex);
                            Logger.DiagLogPu("Tried to cast spell (MultiDot)={0}, dotTarget={1},name={3},health={4}", spellid, target != null, target != null ? target.SafeName : "<none>", target != null ? target.HealthPercent : 0);
                        });
                }

            }
        }

        /// <summary>
        /// Allows waiting for SleepForLagDuration() but ending sooner if condition is met
        /// </summary>
        /// <param name="orUntil">if true will stop waiting sooner than lag maximum</param>
        /// <returns></returns>
        public static Composite CreateWaitForLagDuration(CanRunDecoratorDelegate orUntil = null)
        {
            return new WaitContinue(TimeSpan.FromMilliseconds((StyxWoW.WoWClient.Latency * 2) + 150), orUntil, new ActionAlwaysSucceed());
        }

        /// <summary>
        /// Casting method for casting on Ground location - string
        /// </summary>
        /// <param name="spellname">String Spellname</param>
        /// <param name="onLocation">Location where to cast - Like Me.Location</param>
        /// <returns></returns>
        public static Composite CastOnGround(string spellname, LocationRetriever onLocation)
        {
            return CastOnGround(spellname, onLocation, ret => true);
        }

        public static Composite CastOnGround(string spellname, LocationRetriever onLocation, CanRunDecoratorDelegate requirements, bool waitForSpell = false)
        {
            return
                new Decorator(ret => onLocation != null && requirements(ret) && SpellManager.CanCast(spellname) && (StyxWoW.Me.Location.Distance(onLocation(ret)) <= SpellManager.Spells[spellname].MaxRange || SpellManager.Spells[spellname].MaxRange == 0),
                    new Sequence(
                       new Action(ret => SpellManager.Cast(spellname)),
                        new DecoratorContinue(ctx => waitForSpell,
                            new WaitContinue(1, ret =>
                                StyxWoW.Me.CurrentPendingCursorSpell != null &&
                                StyxWoW.Me.CurrentPendingCursorSpell.Name == spellname, new ActionAlwaysSucceed())),
                        new Action(ret =>
                            {
                                SpellManager.ClickRemoteLocation(onLocation(ret));
                                CooldownTracker.SpellUsed(spellname);
                                Logger.CombatLogOr("Casting: " + spellname);
                            }
                        )));
        }

        /// <summary>
        /// Casting method for casting on Ground location - int
        /// </summary>
        /// <param name="spellid">Integer Spell</param>
        /// <param name="onLocation">Location where to cast - Like Me.Location</param>
        /// <returns></returns>
        public static Composite CastOnGround(int spellid, LocationRetriever onLocation)
        {
            return CastOnGround(spellid, onLocation, ret => true);
        }

        public static Composite CastOnGround(int spellid, LocationRetriever onLocation, CanRunDecoratorDelegate requirements, bool waitForSpell = false)
        {
            return
                new Decorator(ret => onLocation != null && requirements(ret) && SpellManager.CanCast(spellid) && (StyxWoW.Me.Location.Distance(onLocation(ret)) <= WoWSpell.FromId(spellid).MaxRange || WoWSpell.FromId(spellid).MaxRange == 0),
                    new Sequence(
                       new Action(ret => SpellManager.Cast(spellid)),
                        new DecoratorContinue(ctx => waitForSpell,
                            new WaitContinue(1, ret =>
                                StyxWoW.Me.CurrentPendingCursorSpell != null &&
                                StyxWoW.Me.CurrentPendingCursorSpell.Id == spellid, new ActionAlwaysSucceed())),
                        new Action(ret =>
                        {
                            SpellManager.ClickRemoteLocation(onLocation(ret));
                            CooldownTracker.SpellUsed(spellid);
                            Logger.CombatLogOr("Casting: " + spellid);
                        }
                        )));
        }

        /// <summary>
        /// Casting method for casting on Ground location - string
        /// </summary>
        /// <param name="spellname">String Spellname</param>
        /// <param name="onUnit">Casting on unit ground location</param>
        /// <returns></returns>
        public static Composite CastOnGround(string spellname, UnitSelectionDelegate onUnit)
        {
            return CastOnGround(spellname, onUnit, ret => true);
        }

        public static Composite CastOnGround(string spellname, UnitSelectionDelegate onUnit, CanRunDecoratorDelegate requirements, bool waitForSpell = false)
        {
            return
                new Decorator(ret => onUnit != null && requirements(ret) && SpellManager.CanCast(spellname) && (StyxWoW.Me.Location.Distance(onUnit(ret).Location) <= SpellManager.Spells[spellname].MaxRange || SpellManager.Spells[spellname].MaxRange == 0),
                    new Sequence(
                       new Action(ret => SpellManager.Cast(spellname)),
                        new DecoratorContinue(ctx => waitForSpell,
                            new WaitContinue(1, ret =>
                                StyxWoW.Me.CurrentPendingCursorSpell != null &&
                                StyxWoW.Me.CurrentPendingCursorSpell.Name == spellname, new ActionAlwaysSucceed())),
                        new Action(ret =>
                        {
                            SpellManager.ClickRemoteLocation(onUnit(ret).Location);
                            CooldownTracker.SpellUsed(spellname);
                            Logger.CombatLogOr("Casting: " + spellname);
                        }
                        )));
        }

        /// <summary>
        /// Casting method for casting on Ground location - int
        /// </summary>
        /// <param name="spellid">Integer Spell ID</param>
        /// <param name="onUnit">Casting on unit ground location</param>
        /// <returns></returns>
        public static Composite CastOnGround(int spellid, UnitSelectionDelegate onUnit)
        {
            return CastOnGround(spellid, onUnit, ret => true);
        }

        public static Composite CastOnGround(int spellid, UnitSelectionDelegate onUnit, CanRunDecoratorDelegate requirements, bool waitForSpell = false)
        {
            return
                new Decorator(ret => onUnit != null && requirements(ret) && SpellManager.CanCast(spellid) && (StyxWoW.Me.Location.Distance(onUnit(ret).Location) <= WoWSpell.FromId(spellid).MaxRange || WoWSpell.FromId(spellid).MaxRange == 0),
                    new Sequence(
                       new Action(ret => SpellManager.Cast(spellid)),
                        new DecoratorContinue(ctx => waitForSpell,
                            new WaitContinue(1, ret =>
                                StyxWoW.Me.CurrentPendingCursorSpell != null &&
                                StyxWoW.Me.CurrentPendingCursorSpell.Id == spellid, new ActionAlwaysSucceed())),
                        new Action(ret =>
                        {
                            SpellManager.ClickRemoteLocation(onUnit(ret).Location);
                            CooldownTracker.SpellUsed(spellid);
                            Logger.CombatLogOr("Casting: " + spellid);
                        }
                        )));
        }
        #endregion

        #region GCD Detection
        public static bool IsGlobalCooldown(YourBuddy.Core.Helpers.Enum.LagTolerance allow = YourBuddy.Core.Helpers.Enum.LagTolerance.Yes)
        {
#if NO_LATENCY_ISSUES_WITH_GLOBAL_COOLDOWN
            uint latency = allow == YourBuddy.Core.Helpers.Enum.LagTolerance.Yes ? StyxWoW.WoWClient.Latency : 0;
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
                    Logger.DiagLogFb("GCD check fix spell {0} not known", value);
                }
                else
                {
                    _gcdCheck = sfr.Original;
                    Logger.DiagLogFb("GCD check fix spell set to: {0}", value);
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
            Logger.DiagLogFb("FixGlobalCooldownInitialize: using YourBuddy GCD");
            switch (StyxWoW.Me.Class)
            {
                case WoWClass.DeathKnight:
                    FixGlobalCooldownCheckSpell = "Frost Presence";
                    break;
                case WoWClass.Hunter:
                    FixGlobalCooldownCheckSpell = "Hunter's Mark";
                    break;
                case WoWClass.Monk:
                    FixGlobalCooldownCheckSpell = "Stance of the Fierce Tiger";
                    break;
                case WoWClass.Paladin:
                    FixGlobalCooldownCheckSpell = "Righteous Fury";
                    break;
                case WoWClass.Rogue:
                    FixGlobalCooldownCheckSpell = "Sap";
                    break;
                case WoWClass.Shaman:
                    FixGlobalCooldownCheckSpell = "Lightning Shield";
                    break;
            }

            if (FixGlobalCooldownCheckSpell != null)
                return;

            switch (StyxWoW.Me.Class)
            {
                case WoWClass.DeathKnight:
                    // FixGlobalCooldownCheckSpell = "";
                    break;
                case WoWClass.Hunter:
                    FixGlobalCooldownCheckSpell = "Arcane Shot";
                    break;
                case WoWClass.Monk:
                    //FixGlobalCooldownCheckSpell = "";
                    break;
                case WoWClass.Paladin:
                    FixGlobalCooldownCheckSpell = "Seal of Command";
                    break;
                case WoWClass.Rogue:
                    FixGlobalCooldownCheckSpell = "Sinister Strike";
                    break;
                case WoWClass.Shaman:
                    FixGlobalCooldownCheckSpell = "Lightning Bolt";
                    break;
            }
#endif
        }
        #endregion GCD
        #endregion

        #region CancelAura

        public static void CancelAura(this WoWUnit unit, string aura)
        {
            WoWAura a = unit.GetAuraFromName(aura);
            if (a != null && a.Cancellable)
                a.TryCancelAura();
        }

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
        #endregion
        
        #region AuraStuffMore

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


        public static TimeSpan GetAuraTimeLeft(this WoWUnit onUnit, string auraName, bool fromMyAura = true)
        {
            WoWAura wantedAura =
                onUnit.GetAllAuras().Where(a => a != null && a.Name == auraName && a.TimeLeft > TimeSpan.Zero && (!fromMyAura || a.CreatorGuid == StyxWoW.Me.Guid)).FirstOrDefault();

            return wantedAura != null ? wantedAura.TimeLeft : TimeSpan.Zero;
        }

        public static Composite CreateHunterTrapBehavior(string trapName, bool useLauncher, UnitSelectionDelegate onUnit, SimpleBooleanDelegate require = null)
        {
            return new PrioritySelector(
                new Decorator(
                    ret => onUnit != null && onUnit(ret) != null
                        && (require == null || require(ret))
                        && onUnit(ret).DistanceSqr < (40 * 40)
                        && SpellManager.HasSpell(trapName) && CooldownTracker.GetSpellCooldown(trapName) == TimeSpan.Zero,
                    new Sequence(
                        new Action(ret => Logger.DebugLog("Trap: use trap launcher requested: {0}", useLauncher)),

                        // add or remove trap launcher based upon parameter 
                        new PrioritySelector(
                            new Decorator(ret => useLauncher && Me.HasAura("Trap Launcher"), new ActionAlwaysSucceed()),
                            Spell.Cast("Trap Launcher", req => useLauncher),
                            new Decorator(ret => !useLauncher, new Action(ret => Me.CancelAura("Trap Launcher")))
                            ),

                        // wait for launcher to appear (or dissappear) as required
                        new PrioritySelector(
                            new Wait(TimeSpan.FromMilliseconds(500),
                                until => (!useLauncher && !Me.HasAura("Trap Launcher")) || (useLauncher && Me.HasAura("Trap Launcher")),
                                new ActionAlwaysSucceed()),
                            new Action(ret =>
                            {
                                Logger.DebugLog("Trap: FAILURE! unable to {0} the Trap Launcher aura", useLauncher ? "Buff" : "Cancel");
                                return RunStatus.Failure;
                            })
                            ),

                        new Action(ret => Logger.DebugLog("Trap: launcher aura present = {0}", Me.HasAura("Trap Launcher"))),
                        new Action(ret => Logger.DebugLog("Trap: cancast = {0}", Spell.CanCastHack(trapName, onUnit(ret)))),

                        new Action(ret => Logger.CombatLog("^{0} trap: {1} on {2}", useLauncher ? "Launch" : "Set", trapName, onUnit(ret).SafeName())),
                // Spell.Cast( trapName, ctx => onUnit(ctx)),
                        new Action(ret => SpellManager.Cast(trapName, onUnit(ret))),

                        Spell.CreateWaitForLagDuration(),
                        new Action(ctx => SpellManager.ClickRemoteLocation(onUnit(ctx).Location)),
                        new Action(ret => Logger.DebugLog("Trap: Complete!"))
                        )
                    )
                );
        }

        #endregion

        #region Buff
        public static Composite CastRaidBuff(string name, CanRunDecoratorDelegate cond)
        {
            return new Decorator(
                delegate(object a)
                {
                    if (!YourBuddy.Interfaces.Settings.InternalSettings.Instance.General.EnableRaidPartyBuffing)
                        return false;

                    if (!cond(a))
                        return false;

                    if (!SpellManager.CanCast(name, Me))
                    {
                        return false;
                    }

                    var players = new List<WoWPlayer> { Me };
                    if (Me.GroupInfo.IsInRaid)
                    {
                        players.Remove(Me);
                        players.AddRange(Me.RaidMembers);
                    }
                    else if (Me.GroupInfo.IsInParty)
                    {
                        players.AddRange(Me.PartyMembers);
                    }

                    var ProvidablePlayerBuffs = new HashSet<int>();

                    return players.Any(x => x.Distance2DSqr < 40 * 40 && ((!x.HasAnyAura(YourBuddy.Rotations.Global.LegacyoftheWhiteTiger) || !Me.HasAnyAura(YourBuddy.Rotations.Global.LegacyoftheWhiteTiger) && Me.Specialization == WoWSpec.MonkWindwalker) || !x.HasAnyAura(YourBuddy.Rotations.Global.LegacyoftheEmperor) || Me.HasAnyAura(YourBuddy.Rotations.Global.LegacyoftheEmperor) && !x.IsDead && !x.IsGhost && x.IsAlive));
                },
                new Sequence(
                    new Action(a => SpellManager.Cast(name))));
        }


        #endregion

        #region Cast Hack - allows casting spells that CanCast returns False

        public static bool CanCastHack(string castName)
        {
            return CanCastHack(castName, Me.CurrentTarget, skipWowCheck: false);
        }
        public static bool CanCastHack(int castId)
        {
            return CanCastHack(castId, Me.CurrentTarget, skipWowCheck: false);
        }
        /// <summary>
        /// CastHack following done because CanCast() wants spell as "Metamorphosis: Doom" while Cast() and aura name are "Doom"
        /// </summary>
        /// <param name="castName"></param>
        /// <param name="onUnit"></param>
        /// <param name="requirements"></param>
        /// <returns></returns>
        public static bool CanCastHack(string castName, WoWUnit unit, bool skipWowCheck = false)
        {
            SpellFindResults sfr;
            if (!SpellManager.FindSpell(castName, out sfr))
            {
                Logger.DiagLogFb("CanCast: spell [{0}] not known", castName);
                return false;
            }

            return CanCastHack(sfr, unit, skipWowCheck);
        }
        /// <summary>
        /// CastHack following done because CanCast() wants spell as "Metamorphosis: Doom" while Cast() and aura name are "Doom"
        /// </summary>
        /// <param name="castName"></param>
        /// <param name="onUnit"></param>
        /// <param name="requirements"></param>
        /// <returns></returns>
        public static bool CanCastHack(int castId, WoWUnit unit, bool skipWowCheck = false)
        {
            SpellFindResults sfr;
            if (!SpellManager.FindSpell(castId, out sfr))
            {
                return false;
            }

            return CanCastHack(sfr, unit, skipWowCheck);
        }
        /// <summary>
        /// CastHack following done because CanCast() wants spell as "Metamorphosis: Doom" while Cast() and aura name are "Doom"
        /// </summary>
        public static bool CanCastHack(SpellFindResults sfr, WoWUnit unit, bool skipWowCheck = false)
        {
            WoWSpell spell = sfr.Override ?? sfr.Original;
            // check range
            if (unit != null && !spell.IsSelfOnlySpell && !unit.IsMe)
            {
                if (spell.IsMeleeSpell && !unit.IsWithinMeleeRange)
                {
                    Logger.DiagLogFb("CanCastHack[{0}]: not in melee range", spell.Name);
                    return false;
                }
                if (spell.HasRange)
                {
                    if (unit.Distance > spell.ActualMaxRange(unit))
                    {
                        //  Logger.DebugLog("CanCastHack[{0}]: out of range - further than {1:F1}", spell.Name, ActualMaxRange(unit));
                        return false;
                    }
                    if (unit.Distance < spell.ActualMinRange(unit))
                    {
                        //   Logger.DebugLog("CanCastHack[{0}]: out of range - closer than {1:F1}", spell.Name, ActualMinRange(unit));
                        return false;
                    }
                }

                if (!unit.InLineOfSpellSight)
                {
                    Logger.DiagLogFb("CanCastHack[{0}]: not in spell line of {1}", spell.Name, unit.SafeName);
                    return false;
                }
            }



            if (Me.ChanneledCastingSpellId == 0)
            {
                uint num = StyxWoW.WoWClient.Latency * 2u;
                if (StyxWoW.Me.IsCasting && Me.CurrentCastTimeLeft.TotalMilliseconds > num)
                {
                    Logger.DiagLogFb("CanCastHack[{0}]: current cast of [1] has {2:F0} ms left", spell.Name, Me.CurrentCastId, Me.CurrentCastTimeLeft.TotalMilliseconds - num);
                    return false;
                }

                if (spell.CooldownTimeLeft.TotalMilliseconds > num)
                {
                    return false;
                }
            }
            bool formSwitch = false;
            double currentPower = Lua.PlayerPower;
            if (Me.Class == WoWClass.Druid)
            {
                if (Me.Shapeshift == ShapeshiftForm.Cat || Me.Shapeshift == ShapeshiftForm.Bear || Me.Shapeshift == ShapeshiftForm.DireBear)
                {
                    if (Me.HealingSpellIds.Contains(spell.Id))
                    {
                        formSwitch = true;
                        currentPower = Me.CurrentMana;
                    }
                    else if (spell.PowerCost >= 100)
                    {
                        formSwitch = true;
                        currentPower = Me.CurrentMana;
                    }
                }
            }

            if (currentPower < (uint)spell.PowerCost)
            {
                Logger.DiagLogFb("CanCast[{0}]: insufficient power (need {1:F0}, have {2:F0} {3})", spell.Name, spell.PowerCost, currentPower, formSwitch ? "Mana in Form" : Me.PowerType.ToString());
                return false;
            }

            // override spell will sometimes always have cancast=false, so check original also
            if (!skipWowCheck && !spell.CanCast && (sfr.Override == null || !sfr.Original.CanCast))
            {
                Logger.DiagLogFb("CanCast[{0}]: spell specific CanCast failed (#{1})", spell.Name, spell.Id);

                return false;
            }
            return true;
        }

                public static bool HaveAllowMovingWhileCastingAura(WoWSpell spell = null)
        {
            return Me.GetAllAuras().Any(a => a.ApplyAuraType == (WoWApplyAuraType)330 && (spell == null || spell.CastTime < (uint)a.TimeLeft.TotalMilliseconds));
        }

        public static bool IsFunnel(string name)
        {
            SpellFindResults sfr;
            SpellManager.FindSpell(name, out sfr);
            WoWSpell spell = sfr.Override ?? sfr.Original;
            if (spell == null)
                return false;
            return IsFunnel(spell);
        }

  
        public static bool IsFunnel(WoWSpell spell)
        {
            // HV has the answer... ty m8
            bool IsChanneled = false;
            var row = StyxWoW.Db[Styx.Patchables.ClientDb.Spell].GetRow((uint)spell.Id);
            if (row.IsValid)
            {
                var spellMiscIdx = row.GetField<uint>(24);
                row = StyxWoW.Db[Styx.Patchables.ClientDb.SpellMisc].GetRow(spellMiscIdx);
                var flags = row.GetField<uint>(4);
                IsChanneled = (flags & 68) != 0;
            }

            return IsChanneled;
        }

        public static float ActualMinRange(this WoWSpell spell, WoWUnit unit)
        {
            if (spell.MinRange == 0)
                return 0;

            // some code was using 1.66666675f instead of Me.CombatReach ?
            return unit != null ? spell.MinRange + unit.CombatReach + StyxWoW.Me.CombatReach : spell.MinRange;
        }

        public static float ActualMaxRange(this WoWSpell spell, WoWUnit unit)
        {
            if (spell.MaxRange == 0)
                return 0;
            // 0.1 margin for error
            return unit != null ? spell.MaxRange + unit.CombatReach + StyxWoW.Me.CombatReach : spell.MaxRange;
        }

        public static float ActualMaxRange(string name, WoWUnit unit)
        {
            SpellFindResults sfr;
            if (!SpellManager.FindSpell(name, out sfr))
                return 0f;

            WoWSpell spell = sfr.Override ?? sfr.Original;
            return spell.ActualMaxRange(unit);
        }
        #endregion

        #region More AuraShit

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

        public static double GetAuraTimeLeftMilli(string aura, WoWUnit onUnit)
        {
            if (onUnit != null)
            {
                var result = onUnit.GetAuraByName(aura);
                if (result != null)
                {
                    //  if (result.TimeLeft.Milliseconds > 0)
                    return result.TimeLeft.Milliseconds;
                }
            }
            return 0;
        }


        public static double GetAuraTimeMilliEnemy(string aura)
        {
            return GetAuraTimeLeftMilli(aura, Me.CurrentTarget);
        }

        public static double GetAuraTimeMilliMe(string aura)
        {
            return GetAuraTimeLeftMilli(aura, StyxWoW.Me);
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

        #region MeHasAnyAura (Credits millz)

        public static bool HasAnyAura(this WoWUnit unit, params string[] auraNames)
        {
            if (unit == null) return false;

            var auras = unit.GetAllAuras();
            var hashes = new HashSet<string>(auraNames);
            return auras.Any(a => hashes.Contains(a.Name));
        }

        public static bool HasAnyAura(this WoWUnit unit, HashSet<int> auraIDs)
        {
            if (unit == null) return false;

            var auras = unit.GetAllAuras();
            return auras.Any(a => auraIDs.Contains(a.SpellId));
        }
        #endregion

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
                Logger.CombatLogLg(spell.Key + " time: " + spell.Value.DoubleCastCurrentTime);
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
                        new Action(ret => Logger.CombatLogLg(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent))),
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
                        new Action(ret => Logger.CombatLogLg(String.Format("*{0} at {1:F1}", spell, onLocation(ret)))),
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
                    new Action(ret => Logger.CombatLogLg(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent))),
                //new Action(ret => Logger.InfoLog(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}% PH: {4:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent, HealManager.PredictedHealthPercent(onUnit(ret), PRSettings.Instance.includeMyHeals)))),
                    new Action(ret => UpdateDoubleCastEntries(spell.ToString(CultureInfo.InvariantCulture) + onUnit(ret).GetHashCode(), expiryTime))));
        }

        public static Composite PreventDoubleCast(string spell, double expiryTime, UnitSelectionDelegate onUnit, Selection<bool> reqs = null, bool ignoreMoving = false)
        {
            return
                new Decorator(
                    ret =>
                        ((reqs != null && reqs(ret)) || (reqs == null))
                        && onUnit != null
                        && onUnit(ret) != null
                        && SpellManager.CanCast(spell, onUnit(ret), !ignoreMoving)//CanCastHack(spell, onUnit(ret), !ignoreMoving)
                        && !DoubleCastEntries.ContainsKey(spell + onUnit(ret).GetHashCode()),
                    new Sequence(
                        new Action(ret => SpellManager.Cast(spell, onUnit(ret))),
                        new Action(ret => Logger.DiagLogFb(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent))),
                //new Action(ret => Logger.InfoLog(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}% PH: {4:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent, HealManager.PredictedHealthPercent(onUnit(ret), PRSettings.Instance.includeMyHeals)))),
                        new Action(ret => UpdateDoubleCastEntries(spell.ToString(CultureInfo.InvariantCulture) + onUnit(ret).GetHashCode(), expiryTime))));
        }

        public static Composite PreventDoubleCastHack(string spell, double expiryTime, UnitSelectionDelegate onUnit, Selection<bool> reqs = null, bool ignoreMoving = false)
        {
            return
                new Decorator(
                    ret =>
                        ((reqs != null && reqs(ret)) || (reqs == null))
                        && onUnit != null
                        && onUnit(ret) != null
                        && CanCastHack(spell, onUnit(ret), !ignoreMoving)//CanCastHack(spell, onUnit(ret), !ignoreMoving)
                        && !DoubleCastEntries.ContainsKey(spell + onUnit(ret).GetHashCode()),
                    new Sequence(
                        new Action(ret => SpellManager.Cast(spell, onUnit(ret))),
                        new Action(ret => Logger.DiagLogFb(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent))),
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
                    new Action(ret => Logger.CombatLogLg(String.Format("*{0} on {1} at {2:F1} yds at {3:F1}%", spell, onUnit(ret).SafeName(), onUnit(ret).Distance, onUnit(ret).HealthPercent))),
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
            var dotTarget = Unit.NearbyAttackableUnits(unit.Location, radius)
                .Where(x => x != null)
                .OrderByDescending(x => x.HealthPercent)
                .FirstOrDefault(x => !x.HasMyAura(debuff) && x.InLineOfSpellSight);

            if (dotTarget == null)
            {
                // If we couldn't find one without our debuff, then find ones where debuff is about to expire.
                dotTarget = Unit.NearbyAttackableUnits(unit.Location, radius)
                            .Where(x => x != null)
                            .OrderByDescending(x => x.HealthPercent)
                            .FirstOrDefault(x => (x.HasMyAura(debuff) && GetMyAuraTimeLeft(debuff, x) < refreshDurationRemaining) && x.InLineOfSpellSight);
            }
            return dotTarget;
        }

        #endregion


        #region StackCount and other various of things --Alex
        public static uint GetAuraStack(WoWUnit unit, int spellId)
        {
            if (unit != null)
            {
                var wantedAura = Me.GetAllAuras().FirstOrDefault(a => a.SpellId == spellId && a.StackCount > 0 && a.CreatorGuid == Me.Guid);
                return wantedAura != null ? wantedAura.StackCount : 0;
            }
            return 0;
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

        public static Composite CastHunterTrap(string trapName, LocationRetriever onLocation, CanRunDecoratorDelegate req = null)
        {
            const bool useLauncher = true;
            return new PrioritySelector(
                new Decorator(
                    ret => onLocation != null
                        && (req == null || req(ret))
                        && StyxWoW.Me.Location.Distance(onLocation(ret)) < (40 * 40)
                        && SpellManager.HasSpell(trapName) && CooldownTracker.GetSpellCooldown(trapName) == TimeSpan.Zero,
                    new Sequence(
                        new Action(ret => Logger.DebugLog("Trap: use trap launcher requested: {0}", useLauncher)),
                        new PrioritySelector(
                            new Decorator(ret => useLauncher && Me.HasAura("Trap Launcher"), new ActionAlwaysSucceed()),
                            CastHack("Trap Launcher", ret => Me.CurrentTarget, ret => useLauncher && !Me.HasAura("Trap Launcher")),
                            new Decorator(ret => !useLauncher, new Action(ret => Me.CancelAura("Trap Launcher")))
                            ),
                        new Wait(TimeSpan.FromMilliseconds(500),
                            ret => (useLauncher && Me.HasAura("Trap Launcher")),
                            new ActionAlwaysSucceed()),
                        new Action(ret => Logger.DebugLog("Trap: launcher aura present = {0}", Me.HasAura("Trap Launcher"))),
                        new Action(ret => Logger.DebugLog("^{0} trap: {1}", useLauncher ? "Launch" : "Set", trapName)),
                        new Action(ret => SpellManager.Cast(trapName)),
                        new Action(ret => { SpellManager.ClickRemoteLocation(onLocation(ret)); }),
                        new Action(ret => Logger.DebugLog("Trap: Complete!"))
                        )
                    )
                );
        }

        public static Composite CastHunterTrap(int trapName, LocationRetriever onLocation, CanRunDecoratorDelegate req = null)
        {
            const bool useLauncher = true;
            return new PrioritySelector(
                new Decorator(
                    ret => onLocation != null
                        && (req == null || req(ret))
                        && StyxWoW.Me.Location.DistanceSqr(onLocation(ret)) < (40 * 40)
                        && SpellManager.HasSpell(trapName) && CooldownTracker.GetSpellCooldown(trapName) == TimeSpan.Zero,
                    new Sequence(
                        new Action(ret => Logger.DebugLog("Trap: use trap launcher requested: {0}", useLauncher)),
                        new PrioritySelector(
                            new Decorator(ret => useLauncher && Me.HasAura("Trap Launcher"), new ActionAlwaysSucceed()),
                            Cast("Trap Launcher", ret => useLauncher && !Me.HasAura("Trap Launcher")),
                            new Decorator(ret => !useLauncher, new Action(ret => Me.CancelAura("Trap Launcher")))
                            ),
                        new Wait(TimeSpan.FromMilliseconds(500),
                            ret => (useLauncher && Me.HasAura("Trap Launcher")),
                            new ActionAlwaysSucceed()),
                        new Action(ret => Logger.DebugLog("Trap: launcher aura present = {0}", Me.HasAura("Trap Launcher"))),
                        new Action(ret => Logger.DebugLog("^{0} trap: {1}", useLauncher ? "Launch" : "Set", trapName)),
                        new Action(ret => SpellManager.Cast(trapName)),
                        new Action(ret => { SpellManager.ClickRemoteLocation(onLocation(ret)); }),
                        new Action(ret => Logger.DebugLog("Trap: Complete!"))
                        )
                    )
                );
        }

        public static uint GetAuraStack(WoWUnit unit, string spellId)
        {
            if (unit != null)
            {
                var wantedAura = Me.GetAllAuras().FirstOrDefault(a => a.Name == spellId && a.StackCount > 0 && a.CreatorGuid == Me.Guid);
                return wantedAura != null ? wantedAura.StackCount : 0;
            }
            return 0;
        }

        public static Composite CreateWaitForLagDuration()
        {
            return new WaitContinue(TimeSpan.FromMilliseconds((StyxWoW.WoWClient.Latency * 2) + 150), ret => false, new ActionAlwaysSucceed());
        }
        #endregion

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
                    Logger.DiagLogFb("Yb: Failed to retrieve aura's - {0}", getcachedauraException);
                }
            }
        }

        /// <summary>
        /// HasAura extension - String
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, etc)</param>
        /// <param name="auraname">Full Auraname</param>
        /// <param name="stacks">Amount of Stacks on the Aura</param>
        /// <param name="msuLeft">Timeleft on the Aura</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <param name="nonlinq">Avoids using LINQ query to retrieve aura, use this is aura retrieval fails for whatever reason - !!!Not possible to check anything but aura!!!</param>
        /// <returns></returns>
        public static bool HasAura(this WoWUnit unit, string auraname, int stacks = 0, int msuLeft = 0, bool isFromMe = true, bool cached = false, bool nonlinq = false)
        {
            using (new PerformanceLogger("HasAura-String"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return false;

                    WoWAura auraresult = null;

                    if (nonlinq)
                    {
                        if (unit.HasAura(auraname)) return true;
                    }

                    if (!cached)
                    {
                        auraresult = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : unit.GetAllAuras().FirstOrDefault(a => a.Name == auraname);
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        auraresult = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : CachedTargetAuras.FirstOrDefault(a => a.Name == auraname);
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        auraresult = isFromMe ? CachedAuras.FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : CachedAuras.FirstOrDefault(a => a.Name == auraname);
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
        /// HasAura extension - ID
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, etc)</param>
        /// <param name="auraId">AuraBook.Auraname or Aura ID</param>
        /// <param name="stacks">Amount of Stacks on the Aura</param>
        /// <param name="msuLeft">Timeleft on the Aura</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <param name="nonlinq">Avoids using LINQ query to retrieve aura, use this is aura retrieval fails for whatever reason - !!!Not possible to check anything but aura!!!</param>
        /// <returns></returns>
        public static bool HasAura(this WoWUnit unit, int auraId, int stacks = 0, int msuLeft = 0, bool isFromMe = true, bool cached = false, bool nonlinq = false)
        {
            using (new PerformanceLogger("HasAura-Int"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return false;

                    WoWAura auraresult = null;

                    if (nonlinq)
                    {
                        if (unit.HasAura(auraId)) return true;
                    }

                    if (!cached)
                    {
                        auraresult = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        auraresult = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId);
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        auraresult = isFromMe ? CachedAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : CachedAuras.FirstOrDefault(a => a.SpellId == auraId);
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
        /// Gets fading aura timeleft  - String
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, Etc)</param>
        /// <param name="auraname">Full Auraname</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <returns></returns>
        public static double AuraTimeLeft(this WoWUnit unit, string auraname, bool isFromMe = true, bool cached = false)
        {
            using (new PerformanceLogger("AuraTimeLeft-String"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return 0;

                    if (!cached)
                    {
                        WoWAura aura = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : unit.GetAllAuras().FirstOrDefault(a => a.Name == auraname);
                        return aura != null ? aura.TimeLeft.TotalMilliseconds : 0;
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        WoWAura aura = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : CachedTargetAuras.FirstOrDefault(a => a.Name == auraname);
                        return aura != null ? aura.TimeLeft.TotalMilliseconds : 0;
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        WoWAura aura = isFromMe ? CachedAuras.FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : CachedAuras.FirstOrDefault(a => a.Name == auraname);
                        return aura != null ? aura.TimeLeft.TotalMilliseconds : 0;
                    }
                    return 0;
                }
                catch (Exception) { return 0; }
            }
        }

        /// <summary>
        /// Gets fading aura timeleft  - ID
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, Etc)</param>
        /// <param name="auraId">AuraBook.Auraname or Aura ID</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <returns></returns>
        public static double AuraTimeLeft(this WoWUnit unit, int auraId, bool isFromMe = true, bool cached = false)
        {
            using (new PerformanceLogger("AuraTimeLeft-Int"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return 0;

                    if (!cached)
                    {
                        WoWAura aura = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null ? aura.TimeLeft.TotalMilliseconds : 0;
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        WoWAura aura = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null ? aura.TimeLeft.TotalMilliseconds : 0;
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        WoWAura aura = isFromMe ? CachedAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : CachedAuras.FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null ? aura.TimeLeft.TotalMilliseconds : 0;
                    }
                    return 0;
                }
                catch (Exception) { return 0; }
            }
        }

        /// <summary>
        /// Gets fading aura timeleft - String
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, Etc)</param>
        /// <param name="auraname">Full Auraname</param>
        /// <param name="fadingtime">Timeleft on the Aura</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <returns></returns>
        public static bool FadingAura(this WoWUnit unit, string auraname, int fadingtime, bool isFromMe = true, bool cached = false)
        {
            using (new PerformanceLogger("FadingAura-String"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return false;

                    if (!cached)
                    {
                        WoWAura aura = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : unit.GetAllAuras().FirstOrDefault(a => a.Name == auraname);
                        return aura != null && aura.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        WoWAura aura = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : CachedTargetAuras.FirstOrDefault(a => a.Name == auraname);
                        return aura != null && aura.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        WoWAura aura = isFromMe ? CachedAuras.FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : CachedAuras.FirstOrDefault(a => a.Name == auraname);
                        return aura != null && aura.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
                    }
                }
                catch (Exception) { return false; }
            }
            return false;
        }

        /// <summary>
        /// Gets fading aura timeleft - ID
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, Etc)</param>
        /// <param name="auraId">AuraBook.Auraname or Aura ID</param>
        /// <param name="fadingtime">Timeleft on the Aura</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <returns></returns>
        public static bool FadingAura(this WoWUnit unit, int auraId, int fadingtime, bool isFromMe = true, bool cached = false)
        {
            using (new PerformanceLogger("FadingAura-Int"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return false;

                    if (!cached)
                    {
                        WoWAura aura = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        WoWAura aura = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        WoWAura aura = isFromMe ? CachedAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : CachedAuras.FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft <= TimeSpan.FromMilliseconds(fadingtime);                        
                    }
                }
                catch (Exception) { return false; }
            }
            return false;
        }

        /// <summary>
        /// Gets remaining aura timeleft - String
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, Etc)</param>
        /// <param name="auraname">Full Auraname</param>
        /// <param name="remainingtime">Timeleft on the Aura</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <returns></returns>
        public static bool RemainingAura(this WoWUnit unit, string auraname, int remainingtime, bool isFromMe = true, bool cached = false)
        {
            using (new PerformanceLogger("RemainingAura-String"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return false;

                    if (!cached)
                    {
                        WoWAura aura = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : unit.GetAllAuras().FirstOrDefault(a => a.Name == auraname);
                        return aura != null && aura.TimeLeft >= TimeSpan.FromMilliseconds(remainingtime);
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        WoWAura aura = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : CachedTargetAuras.FirstOrDefault(a => a.Name == auraname);
                        return aura != null && aura.TimeLeft >= TimeSpan.FromMilliseconds(remainingtime);
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        WoWAura aura = isFromMe ? CachedAuras.FirstOrDefault(a => a.Name == auraname && a.CreatorGuid == Root.MyGuid) : CachedAuras.FirstOrDefault(a => a.Name == auraname);
                        return aura != null && aura.TimeLeft >= TimeSpan.FromMilliseconds(remainingtime);
                    }
                }
                catch (Exception) { return false; }
            }
            return false;
        }

        /// <summary>
        /// Gets remaining aura timeleft - ID
        /// </summary>
        /// <param name="unit">Unit (Me, Me.CurrentTarget, Etc)</param>
        /// <param name="auraId">AuraBook.Auraname or Aura ID</param>
        /// <param name="remainingtime">Timeleft on the Aura</param>
        /// <param name="isFromMe">True, False - Is aura made by Me</param>
        /// <param name="cached">True, False - Use cached aura's</param>
        /// <returns></returns>
        public static bool RemainingAura(this WoWUnit unit, int auraId, int remainingtime, bool isFromMe = true, bool cached = false)
        {
            using (new PerformanceLogger("RemainingAura-Int"))
            {
                try
                {
                    if (!Unit.IsViable(unit)) return false;

                    if (!cached)
                    {
                        WoWAura aura = isFromMe ? unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : unit.GetAllAuras().FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft >= TimeSpan.FromMilliseconds(remainingtime);
                    }

                    if (CachedTargetAuras != null && unit == Me.CurrentTarget)
                    {
                        WoWAura aura = isFromMe ? CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : CachedTargetAuras.FirstOrDefault(a => a.SpellId == auraId);
                        return aura != null && aura.TimeLeft >= TimeSpan.FromMilliseconds(remainingtime);
                    }

                    if (CachedAuras != null && unit == Me)
                    {
                        WoWAura aura = isFromMe ? CachedAuras.FirstOrDefault(a => a.SpellId == auraId && a.CreatorGuid == Root.MyGuid) : CachedAuras.FirstOrDefault(a => a.SpellId == auraId);
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
