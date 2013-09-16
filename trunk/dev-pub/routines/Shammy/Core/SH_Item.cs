
using Shammy.Helpers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using G = Shammy.Routines.SmGlobal;
using SG = Shammy.Interfaces.Settings.SmSettings;

namespace Shammy.Core
{
    internal static class SmItem
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
            SmLogger.CombatLogP("Using item: " + item.Name);
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


        public static Composite ElementalUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (
                    (SG.Instance.Elemental.Trinket1 == SmEnum.AbilityTrigger.OnBossDummy && SmUnit.IsTargetBoss) ||
                    (SG.Instance.Elemental.Trinket1 == SmEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Elemental.Trinket1 == SmEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),

                new Decorator(ret => SG.Instance.General.CheckPotion && !Me.HasAura("Potion of the Jade Serpent") && G.SpeedBuffsAura, UseItem(76093)),
                new Decorator(ret => (
                    (SG.Instance.Elemental.Trinket1 == SmEnum.AbilityTrigger.OnBossDummy && SmUnit.IsTargetBoss) ||
                    (SG.Instance.Elemental.Trinket1 == SmEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Elemental.Trinket1 == SmEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Elemental.UseHands == SmEnum.AbilityTrigger.OnBossDummy && SmUnit.IsTargetBoss) ||
                    (SG.Instance.Elemental.UseHands == SmEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Elemental.UseHands == SmEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite EnhancementUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (
                    (SG.Instance.Enhancement.Trinket1 == SmEnum.AbilityTrigger.OnBossDummy && SmUnit.IsTargetBoss) ||
                    (SG.Instance.Enhancement.Trinket1 == SmEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.Trinket1 == SmEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),

                new Decorator(ret => SG.Instance.General.CheckPotion && !Me.HasAura("Virmen's Bite") && G.SpeedBuffsAura, UseItem(76089)),
                new Decorator(ret => (
                    (SG.Instance.Enhancement.Trinket2 == SmEnum.AbilityTrigger.OnBossDummy && SmUnit.IsTargetBoss) ||
                    (SG.Instance.Enhancement.Trinket2 == SmEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.Trinket2 == SmEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (
                    (SG.Instance.Enhancement.UseHands == SmEnum.AbilityTrigger.OnBossDummy && SmUnit.IsTargetBoss) ||
                    (SG.Instance.Enhancement.UseHands == SmEnum.AbilityTrigger.OnBlTwHr && G.SpeedBuffsAura) ||
                    (SG.Instance.Enhancement.UseHands == SmEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }


        public static Composite ElementalUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Elemental.CheckHealthStone && Me.HealthPercent < SG.Instance.Elemental.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                SmLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
                            }
                                )))));
        }

        public static Composite EnhancementUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SG.Instance.Enhancement.CheckHealthStone && Me.HealthPercent < SG.Instance.Enhancement.CheckHealthStoneNum,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                SmLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
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
                    SmLogger.DiagLogP("Wielding TwoHander: {0}", ex);
                }
                return false;
            }
        }

    }
}