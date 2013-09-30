using DeathVader.Helpers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using G = DeathVader.Routines.DvGlobal;
using SG = DeathVader.Interfaces.Settings.DvSettings;

namespace DeathVader.Core
{
    internal static class DvItem
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
            DvLogger.CombatLogP("Using item: " + item.Name);
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
                ctx => ObjectManager.GetObjectsOfTypeFast<WoWItem>().FirstOrDefault(item => item.Entry == id),
                new Decorator(
                    ctx => ctx != null && CanUseItem((WoWItem)ctx),
                    new Action(ctx => UseItem((WoWItem)ctx))));
        }

        public static Composite UsePotion()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.General.CheckPotion && !Me.HasAura("Potion of Mogu Power") && G.SpeedBuffsAura, UseItem(76089)));
        }

        public static Composite UnholyUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => Me.CurrentTarget.IsWithinMeleeRange && (
                    (SG.Instance.Unholy.Trinket1 == DvEnum.AbilityTrigger.OnBossDummy && DvUnit.IsTargetBoss) ||
                    (SG.Instance.Unholy.Trinket1 == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Unholy.Trinket1 == DvEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => Me.CurrentTarget.IsWithinMeleeRange && (
                    (SG.Instance.Unholy.Trinket1 == DvEnum.AbilityTrigger.OnBossDummy && DvUnit.IsTargetBoss) ||
                    (SG.Instance.Unholy.Trinket1 == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Unholy.Trinket1 == DvEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => Me.CurrentTarget.IsWithinMeleeRange && (
                    (SG.Instance.Unholy.UseHands == DvEnum.AbilityTrigger.OnBossDummy && DvUnit.IsTargetBoss) ||
                    (SG.Instance.Unholy.UseHands == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Unholy.UseHands == DvEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite BloodUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (
                    (SG.Instance.Blood.Trinket1 == DvEnum.AbilityTrigger.OnBossDummy && DvUnit.IsTargetBoss) ||
                    (SG.Instance.Blood.Trinket1 == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Blood.Trinket1 == DvEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Blood.Trinket1 == DvEnum.AbilityTrigger.OnBossDummy && DvUnit.IsTargetBoss) ||
                    (SG.Instance.Blood.Trinket1 == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Blood.Trinket1 == DvEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Blood.UseHands == DvEnum.AbilityTrigger.OnBossDummy && DvUnit.IsTargetBoss) ||
                    (SG.Instance.Blood.UseHands == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Blood.UseHands == DvEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite FrostUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => Me.CurrentTarget.IsWithinMeleeRange && G.PillarofFrost && (
                    (SG.Instance.Frost.Trinket1 == DvEnum.AbilityTrigger.OnBossDummy && DvUnit.IsTargetBoss) ||
                    (SG.Instance.Frost.Trinket1 == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.Trinket1 == DvEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => Me.CurrentTarget.IsWithinMeleeRange && G.PillarofFrost && (
                    (SG.Instance.Frost.Trinket1 == DvEnum.AbilityTrigger.OnBossDummy && DvUnit.IsTargetBoss) ||
                    (SG.Instance.Frost.Trinket1 == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.Trinket1 == DvEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => Me.CurrentTarget.IsWithinMeleeRange && G.PillarofFrost && (
                    (SG.Instance.Frost.UseHands == DvEnum.AbilityTrigger.OnBossDummy && DvUnit.IsTargetBoss) ||
                    (SG.Instance.Frost.UseHands == DvEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Frost.UseHands == DvEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        
        public static Composite UnholyUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Unholy.CheckHealthStone && Me.HealthPercent < SG.Instance.Unholy.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                DvLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
                            }
                                )))));
        }

        public static Composite BloodUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Blood.CheckHealthStone && Me.HealthPercent < SG.Instance.Blood.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                DvLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
                            }
                                )))));
        }

        public static Composite FrostUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Frost.CheckHealthStone && Me.HealthPercent < SG.Instance.Frost.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                                {
                                    ((WoWItem)ret).UseContainerItem();
                                    DvLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
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
                    DvLogger.DiagLogP("Wielding TwoHander: {0}", ex);
                }
                return false;
            }
        }

    }
}
