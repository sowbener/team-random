
using Waldo.Helpers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using G = Waldo.Routines.WaGlobal;
using SG = Waldo.Interfaces.Settings.WaSettings;

namespace Waldo.Core
{
    internal static class WaItem
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
            WaLogger.CombatLogP("Using item: " + item.Name);
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
            return item != null && item.Usable && item.Cooldown == 0;
        }


        public static Composite UseItem(uint id)
        {
            return new PrioritySelector(
                ctx => ObjectManager.GetObjectsOfType<WoWItem>().FirstOrDefault(item => item.Entry == id),
                new Decorator(
                    ctx => ctx != null && CanUseItem((WoWItem)ctx),
                    new Action(ctx => UseItem((WoWItem)ctx))));
        }


        public static Composite ComUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (
                    (SG.Instance.Combat.Trinket1 == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Combat.Trinket1 == WaEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Combat.Trinket1 == WaEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),

                new Decorator(ret => SG.Instance.General.CheckPotion && G.SpeedBuffsAura, UseItem(76089)),
                new Decorator(ret => (
                    (SG.Instance.Combat.Trinket2 == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Combat.Trinket2 == WaEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Combat.Trinket2 == WaEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Combat.UseHands == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Combat.UseHands == WaEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Combat.UseHands == WaEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite AssaUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (
                    (SG.Instance.Assassination.Trinket1 == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Assassination.Trinket1 == WaEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Assassination.Trinket1 == WaEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),

                new Decorator(ret => SG.Instance.General.CheckPotion && G.SpeedBuffsAura, UseItem(76089)),
                new Decorator(ret => (
                    (SG.Instance.Assassination.Trinket2 == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Assassination.Trinket2 == WaEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Assassination.Trinket2 == WaEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Assassination.UseHands == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Assassination.UseHands == WaEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Assassination.UseHands == WaEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite SubUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (
                    (SG.Instance.Subtlety.Trinket1 == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Subtlety.Trinket1 == WaEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Subtlety.Trinket1 == WaEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),

                new Decorator(ret => SG.Instance.General.CheckPotion && G.SpeedBuffsAura, UseItem(76089)),
                new Decorator(ret => (
                    (SG.Instance.Subtlety.Trinket2 == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Subtlety.Trinket2 == WaEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Subtlety.Trinket2 == WaEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => Me.HasAura("Shadow Dance") && (
                    (SG.Instance.Subtlety.UseHands == WaEnum.AbilityTrigger.OnBossDummy && WaUnit.IsTargetBoss) ||
                    (SG.Instance.Subtlety.UseHands == WaEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Subtlety.UseHands == WaEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }


        public static Composite ComUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Combat.CheckHealthStone && !Me.GetAllAuras().Any(a => G.DoNotUseHealing.Contains(a.SpellId)) && Me.HealthPercent < SG.Instance.Combat.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                WaLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
                            }
                                )))));
        }

        public static Composite AssaUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Assassination.CheckHealthStone && !Me.GetAllAuras().Any(a => G.DoNotUseHealing.Contains(a.SpellId)) && Me.HealthPercent < SG.Instance.Assassination.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                WaLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
                            }
                                )))));
        }


        public static Composite SubUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Subtlety.CheckHealthStone && !Me.GetAllAuras().Any(a => G.DoNotUseHealing.Contains(a.SpellId)) && Me.HealthPercent < SG.Instance.Subtlety.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                WaLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
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
                    WaLogger.DiagLogP("Wielding TwoHander: {0}", ex);
                }
                return false;
            }
        }

    }
}