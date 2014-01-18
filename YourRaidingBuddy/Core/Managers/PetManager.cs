using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommonBehaviors.Actions;
using Styx;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using YourBuddy.Core.Helpers;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;
using Logger = YourBuddy.Core.Utilities.Logger;
using SG = YourBuddy.Interfaces.Settings.InternalSettings;
using Spell = YourBuddy.Core.Spell;

namespace YourBuddy.Core.Helpers
{
    internal static class PetManager
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        private static readonly WaitTimer PetTimer = new WaitTimer(TimeSpan.FromSeconds(2));
        private static readonly WaitTimer CallPetTimer = WaitTimer.OneSecond;
        private static readonly List<WoWPetSpell> PetSpells = new List<WoWPetSpell>();
        private static WoWUnit MyPet { get { return StyxWoW.Me.Pet; } }
        private static bool _wasMounted;
        private static ulong _petGuid;

        #region Delegates
        public delegate T Selection<out T>(object context);
        public delegate WoWPoint LocationRetriever(object context);
        public delegate WoWUnit UnitSelectionDelegate(object context);
        public delegate bool SimpleBooleanDelegate(object context);
        public delegate string SimpleStringDelegate(object context);
        public delegate int SimpleIntDelegate(object context);
        #endregion

        static PetManager()
        {
            // NOTE: This is a bit hackish. This fires VERY OFTEN in major cities. But should prevent us from summoning right after dismounting.
            // Lua.Events.AttachEvent("COMPANION_UPDATE", (s, e) => CallPetTimer.Reset());
            // Note: To be changed to OnDismount with new release
            Mount.OnDismount += (s, e) =>
            {
                if (StyxWoW.Me.Class == WoWClass.Hunter || StyxWoW.Me.PetNumber > 0)
                {
                    PetTimer.Reset();
                }
            };
        }

        internal static void Pulse()
        {
            if (!StyxWoW.Me.GotAlivePet)
            {
                PetSpells.Clear();
                return;
            }

            if (StyxWoW.Me.Mounted)
            {
                _wasMounted = true;
            }

            if (_wasMounted && !StyxWoW.Me.Mounted)
            {
                _wasMounted = false;
                PetTimer.Reset();
            }

            if (StyxWoW.Me.Pet != null && _petGuid != StyxWoW.Me.Pet.Guid)
            {
                _petGuid = StyxWoW.Me.Pet.Guid;
                PetSpells.Clear();
                // Cache the list. yea yea, we should just copy it, but I'd rather have shallow copies of each object, rather than a copy of the list.
                PetSpells.AddRange(StyxWoW.Me.PetSpells);
                PetTimer.Reset();
            }
        }

        /// <summary>Returns the Pet spell cooldown using Timespan (00:00:00.0000000)
        /// gtfo if the Pet dosn't have the spell.</summary>
        /// <param name="name">the name of the spell to check for</param>
        /// <returns>The spell cooldown.</returns>
        public static TimeSpan PetSpellCooldown(string name)
        {
            WoWPetSpell petAction = PetSpells.FirstOrDefault(p => p.ToString() == name);
            if (petAction == null || petAction.Spell == null)
            {
                return TimeSpan.Zero;
            }

            //Logger.DebugLog(" [PetSpellCooldown] {0} : {1}", name, petAction.Spell.CooldownTimeLeft);
            return petAction.Spell.CooldownTimeLeft;
        }

        /// <summary>
        ///   Calls a pet by name, if applicable.
        /// </summary>
        /// <remarks>
        ///   Created 2/7/2011.
        /// </remarks>
        /// <param name = "petNumber">Number of the pet. This parameter is ignored for mages. Warlocks should pass only the name of the pet. Hunters should pass which pet (1, 2, etc)</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool CallPet(string petNumber)
        {
            if (!CallPetTimer.IsFinished)
            {
                return false;
            }

            if (SpellManager.CanCast("Call Pet " + petNumber))
            {
                if (!StyxWoW.Me.GotAlivePet)
                {
                    Logger.InitLog("[Pet] Taking out your badass pet #{0}", petNumber);
                    bool result = SpellManager.Cast("Call Pet " + petNumber);
                    return result;
                }
            }
            return false;
        }

        public static Composite CreateHunterCallPetBehavior()
        {
            return new Decorator(
                ret => !Me.GotAlivePet
                    && PetTimer.IsFinished
                    && !Me.Mounted && !Me.OnTaxi,
                new PrioritySelector(
                    new Decorator(ret => (MyPet == null || MyPet.IsDead) && ((Me.Specialization == WoWSpec.HunterBeastMastery && SG.Instance.Beastmastery.EnableRevivePet) || (Me.Specialization == WoWSpec.HunterSurvival && SG.Instance.Survival.EnableRevivePet)),
                        new Sequence(
                            new Action(ret => Logger.DebugLog("CallPet: attempting Revive Pet - cancast={0}", SpellManager.CanCast("Revive Pet"))),
                            Spell.Cast("Revive Pet"),
                            Spell.CreateWaitForLagDuration(or => Me.IsCasting),
                            new Wait(TimeSpan.FromMilliseconds(750), ret => Me.IsCasting, new ActionAlwaysSucceed())
                            )
                        ),
                    new Decorator(ret => MyPet == null && ((Me.Specialization == WoWSpec.HunterBeastMastery && SG.Instance.Beastmastery.CallPet != Enum.CallPet.None) || (Me.Specialization == WoWSpec.HunterSurvival && SG.Instance.Survival.CallPet != Enum.CallPet.None)),
                        new Sequence(
                            new Action(ret => Logger.DebugLog("CallPet: attempting Call Pet {0} - canbuff={1}", SG.Instance.Beastmastery.CallPet, SpellManager.CanCast("Call Pet " + CallPetNumber.ToString(CultureInfo.InvariantCulture)))),
                            new Action(ret => CallPet(CallPetNumber.ToString(CultureInfo.InvariantCulture))),
                            Spell.CreateWaitForLagDuration(or => Me.GotAlivePet),
                            new WaitContinue(1, ret => Me.GotAlivePet, new ActionAlwaysSucceed())
                        )
                    )
                ));
        }

        public static bool CanCastPetAction(string action)
        {
            WoWPetSpell petAction = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (petAction == null || petAction.Spell == null)
            {
                return false;
            }

            return !petAction.Spell.Cooldown;
        }

        public static void CastPetAction(string action)
        {
            WoWPetSpell spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            Logger.InitLog("[Pet] Casting {0}", action);
            Lua.DoString("CastPetAction({0})", spell.ActionBarIndex + 1);
        }

        private static WoWUnit cached;
        public static void CastPetAction(string action, WoWUnit on)
        {
            WoWPetSpell spell = PetSpells.FirstOrDefault(p => p.ToString() == action);
            if (spell == null)
                return;

            Logger.InitLog("[Pet] Casting {0} on {1}", action, @on.SafeName);
            if (Me.FocusedUnit != null)
            {
                cached = Me.FocusedUnit;
            }
            StyxWoW.Me.SetFocus(on);
            Lua.DoString("CastPetAction({0}, 'focus')", spell.ActionBarIndex + 1);
            if (cached != null)
            {
                StyxWoW.Me.SetFocus(cached);
            }
            else
                StyxWoW.Me.SetFocus(0);
        }

        /// <summary>
        ///  Creates a behavior to cast a pet action by name of the pet spell on current target.
        /// </summary>
        /// <param name="action"> The name of the pet spell that will be casted. </param>
        /// <returns></returns>
        public static Composite CreateCastPetAction(string action)
        {
            return CreateCastPetAction(action, ret => true);
        }

        /// <summary>
        ///  Creates a behavior to cast a pet action by name of the pet spell on current target, if the extra conditions are met.
        /// </summary>
        /// <param name="action"> The name of the pet spell that will be casted. </param>
        /// <param name="extra"> Extra conditions that will be checked. </param>
        /// <returns></returns>
        public static Composite CreateCastPetAction(string action, SimpleBooleanDelegate extra)
        {
            return CreateCastPetActionOn(action, ret => StyxWoW.Me.CurrentTarget, extra);
        }

        /// <summary>
        /// Creates a behavior to cast a pet action by name of the pet spell on the specified U.
        /// </summary>
        /// <param name="action"> The name of the pet spell that will be casted. </param>
        /// <param name="onUnit"> The unit to cast the spell on. </param>
        /// <returns></returns>
        public static Composite CreateCastPetActionOn(string action, UnitSelectionDelegate onUnit)
        {
            return CreateCastPetActionOn(action, onUnit, ret => true);
        }

        /// <summary>
        ///  Creates a behavior to cast a pet action by name of the pet spell on the specified unit, if the extra conditions are met.
        /// </summary>
        /// <param name="action"> The name of the pet spell that will be casted. </param>
        /// <param name="onUnit"> The unit to cast the spell on. </param>
        /// <param name="extra"> Extra conditions that will be checked. </param>
        /// <returns></returns>
        public static Composite CreateCastPetActionOn(string action, UnitSelectionDelegate onUnit, SimpleBooleanDelegate extra)
        {
            return new Decorator(
                ret => extra(ret) && CanCastPetAction(action),
                new Action(ret => CastPetAction(action, onUnit(ret))));
        }

        /// <summary>
        ///  Creates a behavior to cast a pet action by name of the pet spell on current target's location. (like Freeze of Water Elemental)
        /// </summary>
        /// <param name="action"> The name of the pet spell that will be casted. </param>
        /// <returns></returns>
        public static Composite CreateCastPetActionOnLocation(string action)
        {
            return CreateCastPetActionOnLocation(action, ret => true);
        }

        /// <summary>
        ///  Creates a behavior to cast a pet action by name of the pet spell on current target's location, if extra conditions are met
        ///  (like Freeze of Water Elemental)
        /// </summary>
        /// <param name="action"> The name of the pet spell that will be casted. </param>
        /// <param name="extra"> Extra conditions that will be checked. </param>
        /// <returns></returns>
        private static Composite CreateCastPetActionOnLocation(string action, SimpleBooleanDelegate extra)
        {
            return CreateCastPetActionOnLocation(action, extra);
        }

        /// <summary>
        ///  Creates a behavior to cast a pet action by name of the pet spell on specified location.  (like Freeze of Water Elemental)
        /// </summary>
        /// <param name="action"> The name of the pet spell that will be casted. </param>
        /// <param name="location"> The point to click. </param>
        /// <returns></returns>
        public static Composite CreateCastPetActionOnLocation(string action, Func<object, WoWPoint> location)
        {
            return CreateCastPetActionOnLocation(action, location, ret => true);
        }

        /// <summary>
        ///  Creates a behavior to cast a pet action by name of the pet spell on specified location, if extra conditions are met
        ///  (like Freeze of Water Elemental)
        /// </summary>
        /// <param name="action"> The name of the pet spell that will be casted. </param>
        /// <param name="location"> The point to click. </param>
        /// <param name="extra"> Extra conditions that will be checked. </param>
        /// <returns></returns>
        public static Composite CreateCastPetActionOnLocation(string action, Func<object, WoWPoint> location, SimpleBooleanDelegate extra)
        {
            return new Decorator(
                ret => extra(ret) && CanCastPetAction(action),
                new Sequence(
                    new Action(ret => CastPetAction(action)),
                    new WaitContinue(TimeSpan.FromMilliseconds(250), ret => false, new ActionAlwaysSucceed()),
                    new Action(ret => SpellManager.ClickRemoteLocation(location(ret)))));
        }

        
        #region SomePetCallStuff

        /// <summary>
        /// Convert Settings enum to int, to call it in cc.
        /// </summary>
        public static int CallPetNumber
        {
            get
            {
                if (Me.Specialization == WoWSpec.HunterBeastMastery)
                {
                    switch (SG.Instance.Beastmastery.CallPet)
                    {
                        case Enum.CallPet.Pet1:
                            return 1;
                        case Enum.CallPet.Pet2:
                            return 2;
                        case Enum.CallPet.Pet3:
                            return 3;
                        case Enum.CallPet.Pet4:
                            return 4;
                        case Enum.CallPet.Pet5:
                            return 5;
                    }
                    return 0;
                }
                else if (Me.Specialization == WoWSpec.HunterSurvival)
                {
                    switch (SG.Instance.Survival.CallPet)
                    {
                        case Enum.CallPet.Pet1:
                            return 1;
                        case Enum.CallPet.Pet2:
                            return 2;
                        case Enum.CallPet.Pet3:
                            return 3;
                        case Enum.CallPet.Pet4:
                            return 4;
                        case Enum.CallPet.Pet5:
                            return 5;
                    }
                    return 0;
                }
                return 0;
            }
        }

        #endregion
    }
}
