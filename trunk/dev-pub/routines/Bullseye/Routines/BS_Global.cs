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

        #region Diseases


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

        #region AMZ stuff
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
