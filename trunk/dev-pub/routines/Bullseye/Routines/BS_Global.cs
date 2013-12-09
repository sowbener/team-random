using Bullseye.Core;
using Bullseye.Helpers;
using Bullseye.Managers;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using KP = Bullseye.Managers.BsHotKeyManager.KeyboardPolling;
using SG = Bullseye.Interfaces.Settings.BsSettings;
using SH = Bullseye.Interfaces.Settings.BsSettingsH;
using Spell = Bullseye.Core.BsSpell;
using T = Bullseye.Managers.BsTalentManager;



namespace Bullseye.Routines
{
   public class BsGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
         internal static Composite InitializePreBuff
        {
            get
            {
                return new PrioritySelector(
                   new Decorator(ret => SG.Instance.General.CheckPreCombatHk, InitializeOnKeyActions()),
                  new Decorator(ret => SG.Instance.Beastmastery.EnableCallPet || SG.Instance.Survival.EnableCallPet, PetManager.CreateHunterCallPetBehavior()));
            }
        }

        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
                new Action(delegate { BsSpell.GetCachedAuras(); return RunStatus.Failure; }),
                new Action(delegate { BsUnit.GetNearbyAttackableUnitsCount(); return RunStatus.Failure; })
                );
        }

        internal static bool SpeedBuffsAura { get { return Me.HasAura(2825) || Me.HasAura(80353) || Me.HasAura(32182) || Me.HasAura(90355); } }

        #endregion           

        #region InterruptList

        internal static Random _random = new Random();

        internal static readonly HashSet<int> InterruptListMoP = new HashSet<int>
       {
           136797, // Dino-Mending (ToT)
           136587, // Venom Bolt Volley (ToT)
           61909, // Fireball (ToT)
           136189, // Sandbolt (ToT)
           144583, // Wulf Healing (SoO)
           144018, // Corruption (SoO)
           144649, // Hurl Corruption (SoO)
           143473, // Empowered Chain Heal (SoO)
           145555, // Empowered Chain Heal (SoO)
           143432, // Arcane Shock (SoO)
           143431, // Magistrike (SoO)
           145230, // Forbidden Magic (SoO)
           144922, // Harden Flesh
           143958, // Corruption Shock
           145631, // Corruption Chain
           144923, // Earthen Shard
           144379, // Mocking Blast (Manifestation of Pride)
           144468, // Inspiring Song (Chi Ji Timeless Isle)
           146728, // Chain-Heal (SoO)
           146757, // Chain-Heal (SoO)
           143423, // Sha Sear
           123654, // Ming (5man HC instance)
           121284, // Wing Leader (5man HC instance)
           117833, // Crazy Thought (MSV)
           117628, // Shadow Blast (MSV)
           125877, // Dispatch (HoF)
           124077, // Dispatch (HoF)


       };


        internal static readonly HashSet<int> InterruptListTBA = new HashSet<int>
        {

        };

        #endregion

        #region Serpentsting


        internal static bool HasSerpentSting
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                //return Me.CurrentTarget.HasCachedAura(Defines.SerpentStingAura, 0, 2000);	// ID doesn't work - have to use string...why?
                //return Me.CurrentTarget.CachedHasAura("Serpent Sting", 0, true, 2000);
                return Me.CurrentTarget != null && Me.CurrentTarget.HasAura("Serpent Sting", 0, true, 2000);
            }
        }


        #endregion Diseases      

        #region AutoTarget

        private static DateTime LastAutoTarget;

       internal static Composite AutoTarget()
        {
            return new Styx.TreeSharp.Action(delegate
            {
                if (!SG.Instance.General.AutoTarget ||
                    LastAutoTarget > DateTime.Now)
                {
                    return RunStatus.Failure;
                }

                if (Me.CurrentTarget != null && Me.CurrentTarget.IsValid &&
                    !AttackableNoLoS(Me.CurrentTarget, 50) &&
                    GetBestTarget() &&
                    UnitBestTarget != null &&
                    UnitBestTarget.IsValid &&
                    Me.CurrentTarget != UnitBestTarget)
                {
                    UnitBestTarget.Target();
                    BsLogger.DebugLog("Switch to Best Unit");
                    LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(100);
                    return RunStatus.Failure;
                }

                if (Me.CurrentTarget == null &&
                    GetBestTarget() &&
                    UnitBestTarget != null &&
                    UnitBestTarget.IsValid)
                {
                    UnitBestTarget.Target();
                    BsLogger.DebugLog("Target  Best Unit");
                    LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(100);
                    return RunStatus.Failure;
                }
                LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(100);
                return RunStatus.Failure;
            });
        }

        #endregion

        #region GetBestTarget

        private static bool AttackableNoLoS(WoWUnit target, int range)
        {
            if (target == null ||
                !target.IsValid ||
                !target.Attackable ||
                !target.CanSelect ||
                !target.IsAlive ||
                //target.IsCritter ||
                //Blacklist.Contains(target.Guid, BlacklistFlags.All) ||
                Spell.SpellDistance(target) > range)
            {
                return false;
            }
            return true;
        }

        private static WoWUnit UnitBestTarget;

        private static bool GetBestTarget()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                UnitBestTarget = null;
                if (Me.CurrentMap.IsBattleground || Me.CurrentMap.IsArena)
                {
                    UnitBestTarget = BsUnit.NearbyUnFriendlyUnits.OrderBy(unit => unit.CurrentHealth).FirstOrDefault(
                        unit => unit != null && unit.IsValid &&
                                BsUnit.Attackable(unit, 40));

                    if (UnitBestTarget == null)
                    {
                        UnitBestTarget = BsUnit.NearbyUnFriendlyUnits.OrderBy(unit => unit.Distance).FirstOrDefault(
                            unit => unit != null && unit.IsValid &&
                                    BsUnit.Attackable(unit, 80));
                    }
                }

                if (UnitBestTarget == null && !Me.CurrentMap.IsBattleground && !Me.CurrentMap.IsArena)
                {
                    UnitBestTarget = BsUnit.NearbyUnFriendlyUnits.OrderBy(unit => unit.ThreatInfo.RawPercent).FirstOrDefault(
                        unit => unit.IsValid &&
                                Me.IsFacing(unit) &&
                                (unit.IsTargetingMyPartyMember || unit.IsTargetingMyRaidMember ||
                                 unit.IsTargetingMeOrPet) &&
                                BsUnit.Attackable(unit, 40));
                }

                return UnitBestTarget != null && !UnitBestTarget.IsDisabled;
            }
        }

        #endregion

        #region GlobalHotKeys
        internal static Composite InitializeOnKeyActions()
        {
            return new PrioritySelector(
                new Decorator(ret => BsHotKeyManager.KeyboardPolling.IsKeyDown(SH.Instance.AMZ) && SpellManager.CanCast("Anti-Magic Zone"),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Anti-Magic Zone");
                        Styx.WoWInternals.Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        BsLogger.CombatLogP("Casting: Anti-Magic Zone - On Mouse Location");
                    })),
                new Decorator(ret => BsHotKeyManager.KeyboardPolling.IsKeyAsyncDown(SH.Instance.Deterrence),
                    new PrioritySelector(
                        Spell.Cast("Deterrence"))),
                new Decorator(ret => BsHotKeyManager.KeyboardPolling.IsKeyAsyncDown(SH.Instance.Disengage),
                    new PrioritySelector(
                        Spell.Cast("Gorefiend's Grasp", ret => T.HasTalent(16)),
                        Spell.Cast("Remorseless Winter", ret => T.HasTalent(17)),
                        Spell.Cast("Desecrated Ground", ret => T.HasTalent(18)))));
        }
        #endregion


        }
    }
