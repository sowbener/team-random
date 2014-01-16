using YourBuddy.Core.Utilities;
using YourBuddy.Interfaces.Settings;
using YourBuddy.Rotations;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using Enum = YourBuddy.Core.Helpers.Enum;

namespace YourBuddy.Core
{
    internal static class Item
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } } 

        #region Equipped Item Usage Wrappers
        private static bool CanUseEquippedItem(WoWItem item)
        {
            var itemSpell = Lua.GetReturnVal<string>("return GetItemSpell(" + item.Entry + ")", 0);
            if (string.IsNullOrEmpty(itemSpell))
                return false;

            return item.Usable && item.Cooldown <= 0;
        }

        private static bool CanUseTrinket(Enum.AbilityTrigger usage, WoWItem trinket)
        {
            var canUseTrinket = trinket != null && CanUseEquippedItem(trinket);

            switch (usage)
            {
                case Enum.AbilityTrigger.OnBossDummy:
                    return canUseTrinket && Unit.IsTargetBoss;
                case Enum.AbilityTrigger.OnBlTwHr:
                    return canUseTrinket;
                case Enum.AbilityTrigger.Always:
                    return canUseTrinket;
            }
            return false;
        }

        private static bool CanUseHands(Enum.AbilityTrigger usage, WoWItem hands)
        {
            var canUseHands = hands != null && CanUseEquippedItem(hands);

            switch (usage)
            {
                case Enum.AbilityTrigger.OnBossDummy:
                    return canUseHands && Unit.IsTargetBoss;
                case Enum.AbilityTrigger.OnBlTwHr:
                    return canUseHands;
                case Enum.AbilityTrigger.Always:
                    return canUseHands;

            }
            return false;
        }

        private static void UseWindwalkerItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Windwalker.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Windwalker.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Windwalker.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Windwalker.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Windwalker.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Windwalker.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }

        private static void UseBrewmasterItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Brewmaster.Trinket1 == Enum.AbilityTrigger.Never && 
                InternalSettings.Instance.Brewmaster.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Brewmaster.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Brewmaster.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Brewmaster.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Brewmaster.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }


        public static Composite CreateItemBehaviour()
        {
            return new Switch<WoWSpec>(ret => Me.Specialization,
                new SwitchArgument<WoWSpec>(WoWSpec.MonkWindwalker,
            //        new Decorator(ret => 
                        new Action(ret => { UseWindwalkerItems(); return RunStatus.Failure; } )),
                new SwitchArgument<WoWSpec>(WoWSpec.MonkBrewmaster,
              //      new Decorator(ret => 
                        new Action(ret => { UseBrewmasterItems(); return RunStatus.Failure; })));
        }
        #endregion

        #region Useable Item Wrappers
        public static WoWItem FindFirstUsableItemBySpell(params string[] spellNames)
        {
            List<WoWItem> carried = StyxWoW.Me.CarriedItems;
            var spellNameHashes = new HashSet<string>(spellNames);

            return (from i in carried
                    let spells = i.ItemSpells
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    where i.ItemInfo != null && spells != null && spells.Count != 0 && i.Usable && i.Cooldown == 0 && i.ItemInfo.RequiredLevel <= StyxWoW.Me.Level && spells.Any(s => s.IsValid && s.ActualSpell != null && spellNameHashes.Contains(s.ActualSpell.Name))
                    orderby i.ItemInfo.Level descending
                    select i).FirstOrDefault();
        }

        public static Composite WindwalkerUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => InternalSettings.Instance.Windwalker.CheckHealthStone && Me.HealthPercent < InternalSettings.Instance.Windwalker.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                Logger.CombatLogLg("Using {0}", ((WoWItem)ret).Name);
                            })))));
        }

        public static Composite BrewmasterUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => InternalSettings.Instance.Brewmaster.CheckHealthStone && Me.HealthPercent < InternalSettings.Instance.Brewmaster.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                Logger.CombatLogLg("Using {0}", ((WoWItem)ret).Name);
                            })))));
        }

        #endregion

        #region Other Item Functions
        internal static bool WieldsTwoHandedWeapons
        {
            get
            {
                try
                {
                    switch (Me.Inventory.Equipped.MainHand.ItemInfo.WeaponClass)
                    {
                        case WoWItemWeaponClass.ExoticTwoHand:
                        case WoWItemWeaponClass.MaceTwoHand:
                        case WoWItemWeaponClass.AxeTwoHand:
                        case WoWItemWeaponClass.SwordTwoHand:
                        case WoWItemWeaponClass.Polearm:
                            return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.DiagLogPu("Wielding TwoHander: {0}", ex);
                }
                return false;
            }
        }
        #endregion
    }
}
