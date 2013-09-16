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
                   new Decorator(ret => SG.Instance.General.CheckPreCombatHk, InitializeOnKeyActions()));
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

        #region  StuffDK
        internal static bool PillarofFrost { get { return Me.HasCachedAura(51271, 0); } }

        #endregion

        #region Runes

        internal static int DeathRuneSlotsActive
        {
            get {  return Me.GetRuneCount(0) + Me.GetRuneCount(1); }
        }

        internal static int BloodRuneSlotsActive { get { return Me.GetRuneCount(0) + Me.GetRuneCount(1); } }

        internal static int FrostRuneSlotsActive { get { return Me.GetRuneCount(2) + Me.GetRuneCount(3); } }


        internal static bool DeathRuneSlotsActiveFesterReal { get { return StyxWoW.Me.BloodRuneCount > 1 || StyxWoW.Me.DeathRuneCount > 1; } }
        
        internal static int UnholyRuneSlotsActive { get { return Me.GetRuneCount(4) + Me.GetRuneCount(5); } }

        /// <summary>
        /// check that we are in the last tick of Frost Fever or Blood Plague on current target and have a fully depleted rune
        /// </summary>
        internal static bool CanCastPlagueLeechDW
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                return BsTalentManager.HasTalent(2) &&  (BloodRuneSlotsActive == 0 || FrostRuneSlotsActive == 0 || UnholyRuneSlotsActive == 0);
            }
        }




        internal static int ActiveRuneCount
        {
            get
            {
                return StyxWoW.Me.BloodRuneCount + StyxWoW.Me.FrostRuneCount + StyxWoW.Me.UnholyRuneCount +
                       StyxWoW.Me.DeathRuneCount;
            }
        }

        #region Tier Piece Armor Detection

        /// <summary>Checks the number of Tier pieces a player is wearing </summary>
        private static int NumTierPieces(int txxItemSetId)
        {
            {
                var
                count = Me.Inventory.Equipped.Hands != null && Me.Inventory.Equipped.Hands.ItemInfo.ItemSetId != 0 && Me.Inventory.Equipped.Hands.ItemInfo.ItemSetId == txxItemSetId ? 1 : 0;
                count += Me.Inventory.Equipped.Legs != null && Me.Inventory.Equipped.Legs.ItemInfo.ItemSetId != 0 && Me.Inventory.Equipped.Legs.ItemInfo.ItemSetId == txxItemSetId ? 1 : 0;
                count += Me.Inventory.Equipped.Chest != null && Me.Inventory.Equipped.Chest.ItemInfo.ItemSetId != 0 && Me.Inventory.Equipped.Chest.ItemInfo.ItemSetId == txxItemSetId ? 1 : 0;
                count += Me.Inventory.Equipped.Shoulder != null && Me.Inventory.Equipped.Shoulder.ItemInfo.ItemSetId != 0 && Me.Inventory.Equipped.Shoulder.ItemInfo.ItemSetId == txxItemSetId ? 1 : 0;
                count += Me.Inventory.Equipped.Head != null && Me.Inventory.Equipped.Head.ItemInfo.ItemSetId != 0 && Me.Inventory.Equipped.Head.ItemInfo.ItemSetId == txxItemSetId ? 1 : 0;
                return count;
            }
        }

        /// <summary>Returns true if player has 2pc Tier set Bonus</summary>
        public static bool Has2PcTeirBonus(int txxItemSetId)
        {
            {
                return NumTierPieces(txxItemSetId) >= 2;
            }
        }

        /// <summary>Returns true if player has 4pc Tier set Bonus</summary>
        public static bool Has4PcTeirBonus(int txxItemSetId)
        {
            {
                return NumTierPieces(txxItemSetId) == 4;
            }
        }

        #endregion Tier Piece Armor Detection

        private static readonly HashSet<int> Tier15Ids = new HashSet<int>
        {
            -724, // 535 ilvl
            1152, // 522 ilvl
            -701,  // 502 ilvl
        };

        internal static bool Has4PcTier15 { get { return Tier15Ids.Any(Has4PcTeirBonus); } }

        internal static bool Has2PcTier15 { get { return Tier15Ids.Any(Has2PcTeirBonus); } }

        #endregion Runes

        #region Diseases

        internal static bool HasBothDis
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura frostFever =
                    Me.CurrentTarget.GetAllAuras().FirstOrDefault(
                        u => u.CreatorGuid == Me.Guid && u.SpellId == 55095);
                WoWAura bloodPlague =
                    Me.CurrentTarget.GetAllAuras().FirstOrDefault(
                        u => u.CreatorGuid == Me.Guid && u.SpellId == 55078);
                return frostFever != null && frostFever.TimeLeft >= TimeSpan.FromSeconds(2) ||
                       (bloodPlague != null && bloodPlague.TimeLeft >= TimeSpan.FromSeconds(2));
            }
        }

        internal static bool HasffDis
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura frostFever = Me.CurrentTarget.GetAllAuras().FirstOrDefault(u => u.CreatorGuid == Me.Guid && u.SpellId == 55095);
                return frostFever != null && frostFever.TimeLeft >= TimeSpan.FromSeconds(2);
            }
        }

        public static WoWUnit BestUnholyFrenzyTarget
        {
            get
            {
                // If the player has a focus target set, use it instead.
                if (StyxWoW.Me.FocusedUnitGuid != 0 && StyxWoW.Me.FocusedUnit.IsAlive)
                    return StyxWoW.Me.FocusedUnit;

                return Me;
            }
        }

        internal static bool HasbpDis
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura bloodPlague = Me.CurrentTarget.GetAllAuras().FirstOrDefault(u => u.CreatorGuid == Me.Guid && u.SpellId == 55078);
                return bloodPlague != null && bloodPlague.TimeLeft >= TimeSpan.FromSeconds(2);
            }
        }



        #endregion Diseases

        #region Misc

        internal static bool CanEmpowerRuneWeapon
        {
            get
            {
                return StyxWoW.Me.CurrentRunicPower < 26 && DeathRuneSlotsActive + FrostRuneSlotsActive == 0 && UnholyRuneSlotsActive < 2;
            }
        }

        internal static bool CanEmpowerRuneWeaponDW
        {
            get
            {
                return BsLua.PlayerPower < 20 && BsSpell.GetSpellCooldown(49020).TotalMilliseconds > 2000 && BsSpell.GetSpellCooldown(49184).TotalMilliseconds > 2000 && Me.IsWithinMeleeRange;
            }
        }

        #endregion Misc

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

        #region BloodTapCommonStuff

        private static bool NeedBloodTap
        {
            get { return Me.HasCachedAura("Blood Charge", 5); }

        #endregion

        }
    }
}
