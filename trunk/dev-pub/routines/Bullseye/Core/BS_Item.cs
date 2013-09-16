﻿using Bullseye.Helpers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using G = Bullseye.Routines.BsGlobal;
using SG = Bullseye.Interfaces.Settings.BsSettings;

namespace Bullseye.Core
{
    internal static class BsItem
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } } 

        private static bool CanUseEquippedItem(WoWItem item)
        {
            string itemSpell = Lua.GetReturnVal<string>("return GetItemSpell(" + item.Entry + ")", 0);
            if (string.IsNullOrEmpty(itemSpell))
                return false;

            return item.Usable && item.Cooldown <= 0;
        }

        private static void UseItem(WoWItem item)
        {
            BsLogger.CombatLogP("Using item: " + item.Name);
            item.Use();
        }

        public static WoWItem FindFirstUsableItemBySpell(params string[] spellNames)
        {
            List<WoWItem> carried = StyxWoW.Me.CarriedItems;
            var spellNameHashes = new HashSet<string>(spellNames);

            return (from i in carried
                    let spells = i.ItemSpells
                    where i.ItemInfo != null && spells != null && spells.Count != 0 &&
                          i.Usable &&
                        // ReSharper disable CompareOfFloatsByEqualityOperator
                          i.Cooldown == 0 &&
                        // ReSharper restore CompareOfFloatsByEqualityOperator
                          i.ItemInfo.RequiredLevel <= StyxWoW.Me.Level &&
                          spells.Any(s => s.IsValid && s.ActualSpell != null && spellNameHashes.Contains(s.ActualSpell.Name))
                    orderby i.ItemInfo.Level descending
                    select i).FirstOrDefault();
        }

        private static bool CanUseItem(WoWItem item)
        {
            return item != null && item.Usable && item.CooldownTimeLeft.TotalMilliseconds <= 0;
        }



        public static Composite UseItem(uint id)
        {
            return new PrioritySelector(
                ctx => ObjectManager.GetObjectsOfType<WoWItem>().FirstOrDefault(item => item.Entry == id),
                new Decorator(
                    ctx => ctx != null && CanUseItem((WoWItem)ctx),
                    new Action(ctx => UseItem((WoWItem)ctx))));
        }

        public static Composite UsePotion()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.General.CheckPotion && !Me.HasAura("Potion of Mogu Power") && G.SpeedBuffsAura, UseItem(76089)));
        }

        public static Composite BeastmasteryUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (
                    (SG.Instance.Beastmastery.Trinket1 == BsEnum.AbilityTrigger.OnBossDummy && BsUnit.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.Trinket1 == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.Trinket1 == BsEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Beastmastery.Trinket1 == BsEnum.AbilityTrigger.OnBossDummy && BsUnit.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.Trinket1 == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.Trinket1 == BsEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Beastmastery.UseHands == BsEnum.AbilityTrigger.OnBossDummy && BsUnit.IsTargetBoss) ||
                    (SG.Instance.Beastmastery.UseHands == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Beastmastery.UseHands == BsEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite MarksmanshipUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (
                    (SG.Instance.Marksmanship.Trinket1 == BsEnum.AbilityTrigger.OnBossDummy && BsUnit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.Trinket1 == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.Trinket1 == BsEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Marksmanship.Trinket1 == BsEnum.AbilityTrigger.OnBossDummy && BsUnit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.Trinket1 == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.Trinket1 == BsEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Marksmanship.UseHands == BsEnum.AbilityTrigger.OnBossDummy && BsUnit.IsTargetBoss) ||
                    (SG.Instance.Marksmanship.UseHands == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Marksmanship.UseHands == BsEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite SurvivalUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (
                    (SG.Instance.Survival.Trinket1 == BsEnum.AbilityTrigger.OnBossDummy && BsUnit.IsTargetBoss) ||
                    (SG.Instance.Survival.Trinket1 == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.Trinket1 == BsEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Survival.Trinket1 == BsEnum.AbilityTrigger.OnBossDummy && BsUnit.IsTargetBoss) ||
                    (SG.Instance.Survival.Trinket1 == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.Trinket1 == BsEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Survival.UseHands == BsEnum.AbilityTrigger.OnBossDummy && BsUnit.IsTargetBoss) ||
                    (SG.Instance.Survival.UseHands == BsEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Survival.UseHands == BsEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        
        public static Composite BeastmasteryUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Beastmastery.CheckHealthStone && Me.HealthPercent < SG.Instance.Beastmastery.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                BsLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
                            }
                                )))));
        }


        public static Composite MarksmanshipUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Marksmanship.CheckHealthStone && Me.HealthPercent < SG.Instance.Marksmanship.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                BsLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
                            }
                                )))));
        }

        public static Composite SurvivalUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Survival.CheckHealthStone && Me.HealthPercent < SG.Instance.Survival.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                                {
                                    ((WoWItem)ret).UseContainerItem();
                                    BsLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
                                }
                                )))));
        }


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
                            return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    BsLogger.DiagLogP("Wielding TwoHander: {0}", ex);
                }
                return false;
            }
        }

    }
}
