﻿// ReSharper disable InconsistentNaming

using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using YBMoP_BT_Rogue.Core;
using YBMoP_BT_Rogue.Helpers;
using YBMoP_BT_Rogue.Managers;
using Action = Styx.TreeSharp.Action;
using SF = YBMoP_BT_Rogue.Interfaces.Settings.YBSettingsAss;
using SG = YBMoP_BT_Rogue.Interfaces.Settings.YBSettingsG;
using SP = YBMoP_BT_Rogue.Interfaces.Settings.YBSettingsCom;

namespace YBMoP_BT_Rogue.Routines
{
    class YBGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
        internal static Composite InitializePreBuff
        {
            get
            {
                return new PrioritySelector(
                    new Action(delegate { YBSpell.GetCachedAuras(); return RunStatus.Failure; }),
                    new Decorator(ret => Me.Specialization == WoWSpec.RogueAssassination && YBUnit.DefaultBuffCheck,
                        new PrioritySelector(
                    new Decorator(ret => SF.Instance.CheckPoison, Poisons.CreateApplyPoisonsAss()))),
                    new Decorator(ret => Me.Specialization == WoWSpec.RogueCombat && YBUnit.DefaultBuffCheck,
                        new PrioritySelector(
                   new Decorator(ret => SP.Instance.CheckPoison, Poisons.CreateApplyPoisonsCom()))));
            }
        }

        #endregion

        #region Booleans & Doubles
        // Booleans for multiple use.
        internal static bool YourGoingDownBoss   { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent <= 35; } }
        internal static bool ImDpsingYou { get { return Me.CurrentTarget != null && Me.CurrentTarget.HealthPercent >= 35; } }
        internal static bool TargettingMe       { get { return Me.CurrentTarget.IsTargetingMeOrPet; } }
        internal static bool TargetIsClose { get { return Me.CurrentTarget != null && Me.CurrentTarget.IsWithinMeleeRange; } }

        // Cached Aura's - Can only be used with MY aura's (HasCachedAura).
        internal static bool VendettaAura { get { return Me.HasCachedAura(79140, 0); } }
        internal static bool ShadowbladesAura { get { return !Me.HasCachedAura(121471, 0); } }
        internal static bool ShadowbladesAuraActive { get { return Me.HasAura("Shadow Blades"); } }
        internal static bool DispatchProc { get { return Me.HasCachedAura(121153, 0); } }
        internal static bool NoDispatchLove { get { return !Me.HasCachedAura(121153, 0); } }
        internal static bool TargetNoRupture { get { return Me.CurrentTarget != null && !Me.CurrentTarget.HasCachedAura(1943, 0); } }
        internal static bool TargetHaveRupture { get { return Me.CurrentTarget != null & Me.CurrentTarget.HasCachedAura(1943, 0); } }
        internal static bool TargetRuptureFalling { get { return Me.CurrentTarget != null & !Me.CurrentTarget.HasCachedAura(1943, 0, 2000); } }
        internal static bool Anticipate4stacks { get { return Me.HasCachedAura(115189, 4); } }
        internal static bool EnvenomRemains2Seconds { get { return Me.HasCachedAura(32645, 0, 2000); } }
        internal static bool ShadowBladesSND { get { return Me.HasCachedAura(5171, 0, 12000); } }
        internal static bool FucknoSND { get { return !Me.HasCachedAura(5171, 0);  } }
        internal static bool IloveyouSND { get { return Me.HasCachedAura(5171, 0);  } }
        internal static bool MySNDBabyIsFalling { get { return !Me.HasCachedAura(5171, 0, 2000); } }
        internal static bool MeHasShadowFocus { get { return Me.HasCachedAura(108209, 0); } }
        internal static bool Vanishisoncooldown { get { return YBSpell.SpellOnCooldown(1856); } }
        internal static bool Kick { get { return YBSpell.SpellOnCooldown("Kick"); } }

        private static readonly HashSet<int> Tier14Ids = new HashSet<int>
        {
            -526, // 509 ilvl
            1124, // 496 ilvl
            -503,  // 483 ilvl
        };

        private static readonly HashSet<int> Tier15Ids = new HashSet<int>
        {
            1167, // 522 ilvl
            -740, // 535 ilvl
            -717,  // 502 ilvl
        };

        public static bool Has4PcTier15 { get { return Tier15Ids.Any(Has4PcTeirBonus); } }

        public static bool Has2PcTier15 { get { return Tier15Ids.Any(Has2PcTeirBonus); } }

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

        // Cached Stacked Aura's - Can only be used with MY aura's (HasCachedAura).

        // Cached Timed Aura's - Can only be used with MY aura's (HasCachedAura).

        //Rogue Shit
        public static WoWUnit BestTricksTarget
        {
            get
            {
                if (!StyxWoW.Me.GroupInfo.IsInParty &&
                     !StyxWoW.Me.GroupInfo.IsInRaid)
                {
                    return null;
                }

                // If the player has a focus target set, use it instead.
                if (StyxWoW.Me.FocusedUnitGuid != 0)
                {
                    return StyxWoW.Me.FocusedUnit;
                }

                if (RaFHelper.Leader != null &&
                     !RaFHelper.Leader.IsMe)
                {
                    // Leader first, always. Otherwise, pick a rogue/DK/War pref. Fall back to others just in case.
                    return RaFHelper.Leader;
                }

                WoWPlayer bestTank =
                    StyxWoW.Me.GroupInfo.RaidMembers.Where
                        (member => member.HasRole(WoWPartyMember.GroupRole.Tank) && member.Health > 0).Select
                        (member => member.ToPlayer()).Where(player => player != null).OrderBy
                        (player => player.DistanceSqr).FirstOrDefault();

                if (bestTank != null)
                {
                    return bestTank;
                }

                WoWPlayer bestPlayer =
                    StyxWoW.Me.GroupInfo.RaidMembers.Where
                        (member => member.HasRole(WoWPartyMember.GroupRole.Damage) && member.Health > 0).Select
                        (member => member.ToPlayer()).Where(player => player != null).OrderBy
                        (player => player.DistanceSqr).FirstOrDefault();

                return bestPlayer;
            }
        }

        internal static bool SliceAndDiceEnevenom
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura SilceAndDiceEne = Me.GetAllAuras().FirstOrDefault(u =>  u.SpellId == 5171);
                return SilceAndDiceEne != null && SilceAndDiceEne.TimeLeft >= TimeSpan.FromMilliseconds(3000);
            }
        }

        internal static bool RevealingStrike
        {
            get
            {
                if (!Me.GotTarget)
                    return false;
                WoWAura Reveal = Me.CurrentTarget.GetAllAuras().FirstOrDefault(u => u.CreatorGuid == Me.Guid && u.SpellId == 84617);
                return Reveal != null && Reveal.TimeLeft >= TimeSpan.FromMilliseconds(3000);
            }
        }


        // Cached Aura's - Can be used with ANY aura's (HasAnyCachedAura).
        internal static bool SpeedBuffsAura { get { return Me.HasAura(2825) || Me.HasAura(80353) || Me.HasAura(32182) || Me.HasAura(90355); } }

        internal static bool WeakenedBlowsAura { get { return !Me.CurrentTarget.HasAnyCachedAura(113746, 0); } }
        
        // Talentmanager - HasTalents

        // Talentmanager - HaSGlyphs


        // Cooldown Tracker ( Translate: Impending Victory CoolDown On Cooldown)


        // Cooldown Tracker ( Translate: Bloodbath Cooldown)

        #endregion
    }
}
