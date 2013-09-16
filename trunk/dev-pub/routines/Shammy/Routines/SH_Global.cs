using Shammy.Core;
using Shammy.Helpers;
using Shammy.Managers;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using KP = Shammy.Managers.SmHotKeyManager.KeyboardPolling;
using SG = Shammy.Interfaces.Settings.SmSettings;
using SH = Shammy.Interfaces.Settings.SmSettingsH;
using Spell = Shammy.Core.SmSpell;
using T = Shammy.Managers.SmTalentManager;
using W = Shammy.Helpers.WeaponImbue;

namespace Shammy.Routines
{
    class SmGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
        internal static Composite InitializePreBuff
        {
            get
            {
                return new PrioritySelector(
                   new Decorator(ret => SG.Instance.General.CheckPreCombatHk, InitializeOnKeyActions()),
                    new Decorator(ret => Me.Specialization == WoWSpec.ShamanEnhancement && SmUnit.DefaultBuffCheck && ((SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new PrioritySelector(
                        Spell.Cast("Lightning Shield", ret => Me, ret => SG.Instance.Elemental.PrebuffPet && !Me.HasAura("Lightning Shield")),
                     W.CreateWeaponImbueEnhancement())),
                       new Decorator(ret => Me.Specialization == WoWSpec.ShamanElemental && SmUnit.DefaultBuffCheck && ((SG.Instance.General.CheckPreCombatBuff && !Me.Combat) || Me.Combat),
                        new PrioritySelector(
                     Spell.Cast("Lightning Shield", ret => Me, ret => SG.Instance.Elemental.PrebuffPet && !Me.HasAura("Lightning Shield")),
                     W.CreateWeaponImbueElemental())),
                    new Action(delegate { SmSpell.GetCachedAuras(); return RunStatus.Failure; }));
            }
        }

        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
                new Action(delegate { SmSpell.GetCachedAuras(); return RunStatus.Failure; }),
                new Action(delegate { SmUnit.GetAttackableMeleeUnitsCount(); return RunStatus.Failure; })
                );
        }

        internal static bool SpeedBuffsAura { get { return Me.HasAura(2825) || Me.HasAura(80353) || Me.HasAura(32182) || Me.HasAura(90355); } }

        #endregion

        #region  StuffDK
        internal static bool PillarofEnhancement { get { return Me.HasCachedAura(51271, 0); } }

        #endregion

        #region Runes

        internal static int DeathRuneSlotsActive
        {
            get { return StyxWoW.Me.DeathRuneCount; }
        }

        internal static int BloodRuneSlotsActive
        {
            get
            {
                //Logger.InfoLog("B {0}, b {1}", StyxWoW.Me.GetRuneCount(0) != 0 ? "Ready" : "depleted", StyxWoW.Me.GetRuneCount(1) != 0 ? "Ready" : "depleted");
                return StyxWoW.Me.BloodRuneCount;
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
                WoWAura EnhancementFever =
                    Me.CurrentTarget.GetAllAuras().FirstOrDefault(
                        u => u.CreatorGuid == Me.Guid && u.SpellId == 55095);
                WoWAura bloodPlague =
                    Me.CurrentTarget.GetAllAuras().FirstOrDefault(
                        u => u.CreatorGuid == Me.Guid && u.SpellId == 55078);
                return EnhancementFever != null && EnhancementFever.TimeLeft >= TimeSpan.FromSeconds(2) ||
                       (bloodPlague != null && bloodPlague.TimeLeft >= TimeSpan.FromSeconds(2));
            }
        }

        internal static bool HasffDis
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura EnhancementFever = Me.CurrentTarget.GetAllAuras().FirstOrDefault(u => u.CreatorGuid == Me.Guid && u.SpellId == 55095);
                return EnhancementFever != null && EnhancementFever.TimeLeft >= TimeSpan.FromSeconds(2);
            }
        }

        public static WoWUnit BestElementalFrenzyTarget
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

        internal static bool HasbpDisPL1
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura bloodPlague = Me.CurrentTarget.GetAllAuras().FirstOrDefault(u => u.CreatorGuid == Me.Guid && u.SpellId == 55078);
                return bloodPlague != null && bloodPlague.TimeLeft >= TimeSpan.FromSeconds(1);
            }
        }

        internal static bool HasbpDisPL2
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura bloodPlague = Me.CurrentTarget.GetAllAuras().FirstOrDefault(u => u.CreatorGuid == Me.Guid && u.SpellId == 55078);
                return bloodPlague != null && bloodPlague.TimeLeft >= TimeSpan.FromSeconds(3);
            }
        }

        public static bool CanCastPlagueLeech
        {
            get
            {
                if (!Me.GotTarget)
                    return false;

                WoWAura EnhancementFever =
                    Me.CurrentTarget.GetAllAuras().FirstOrDefault(
                        u => u.CreatorGuid == Me.Guid && u.SpellId == 55095);
                WoWAura bloodPlague =
                    Me.CurrentTarget.GetAllAuras().FirstOrDefault(
                        u => u.CreatorGuid == Me.Guid && u.SpellId == 55078);

                // if there is 3 or less seconds left on the diseases and we have a fully depleted rune then return true.
                return EnhancementFever != null && EnhancementFever.TimeLeft < TimeSpan.FromSeconds(2) ||
                       bloodPlague != null && bloodPlague.TimeLeft < TimeSpan.FromSeconds(2);
            }
        }

        #endregion Diseases

        #region YOUWANNALOOKHERE stuff
        internal static Composite InitializeOnKeyActions()
        {
            return new PrioritySelector(
                new Decorator(ret => SmHotKeyManager.KeyboardPolling.IsKeyDown(SH.Instance.AMZ) && SpellManager.CanCast("Healing Rain"),
                    new Action(ret =>
                    {
                        SpellManager.Cast("Healing Rain");
                        Styx.WoWInternals.Lua.DoString("if SpellIsTargeting() then CameraOrSelectOrMoveStart() CameraOrSelectOrMoveStop() end");
                        SmLogger.CombatLogP("Casting: Healing Rain - On Mouse Location");
                    })),
                new Decorator(ret => SmHotKeyManager.KeyboardPolling.IsKeyAsyncDown(SH.Instance.Tier6),
                    new PrioritySelector(
                        Spell.PreventDoubleCast("Healing Tide Totem", 1, ret => T.HasTalent(13)),
                        Spell.Cast("Ancestral Guidance", ret => T.HasTalent(14)),
                        Spell.Cast("Conductivity", ret => T.HasTalent(15)))));
        }
        #endregion


        }
    }
