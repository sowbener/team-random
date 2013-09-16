using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
// Tnx Singular :).
using System;
using System.Collections.Generic;
using System.Linq;
using YBMoP_BT_Warrior.Helpers;
using Action = Styx.TreeSharp.Action;
using G = YBMoP_BT_Warrior.Routines.YBGlobal;
using SF = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsF;
using SP = YBMoP_BT_Warrior.Interfaces.Settings.YBSettingsP;

namespace YBMoP_BT_Warrior.Core
{
    internal static class YBItem
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
            YBLogger.CombatLogP("Using item: " + item.Name);
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

        public static Composite FuryUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => ((G.BloodbathAura) || (G.SBTalent) || (G.AVTalent)) && G.ColossusSmashAura && (
                    (SF.Instance.Trinket1 == YBEnum.AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.Trinket1 == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Trinket1 == YBEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => ((G.BloodbathAura) || (G.SBTalent) || (G.AVTalent)) && G.ColossusSmashAura && (
                    (SF.Instance.Trinket1 == YBEnum.AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.Trinket1 == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.Trinket1 == YBEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => ((G.BloodbathAura) || (G.SBTalent) || (G.AVTalent)) && G.ColossusSmashAura && (
                    (SF.Instance.UseHands == YBEnum.AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.UseHands == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SF.Instance.UseHands == YBEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite ProtUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => G.TargettingMe && (
                    (SP.Instance.Trinket1 == YBEnum.AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SP.Instance.Trinket1 == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.Trinket1 == YBEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => G.TargettingMe && (
                    (SP.Instance.Trinket1 == YBEnum.AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SP.Instance.Trinket1 == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.Trinket1 == YBEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => G.TargettingMe && (
                    (SP.Instance.UseHands == YBEnum.AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SP.Instance.UseHands == YBEnum.AbilityTrigger.OnBlTwHr && G.HasteAbilities) ||
                    (SP.Instance.UseHands == YBEnum.AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite FuryUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SF.Instance.CheckHealthStone && Me.HealthPercent < SF.Instance.NumHealthStone,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                                {
                                    ((WoWItem)ret).UseContainerItem();
                                    YBLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
                                }
                                )))));
        }

        public static Composite ProtUseHealthStone()
        {
            return new PrioritySelector(
                new Decorator(ret => SF.Instance.CheckHealthStone && Me.HealthPercent < SF.Instance.NumHealthStone,
                    new PrioritySelector(ctx => FindFirstUsableItemBySpell("Healthstone"),
                        new Decorator(ret => ret != null,
                            new Action(ret =>
                            {
                                ((WoWItem)ret).UseContainerItem();
                                YBLogger.CombatLogG("Using {0}", ((WoWItem)ret).Name);
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
                    YBLogger.DiagLogP("Wielding TwoHander: {0}", ex);
                }
                return false;
            }
        }

    }
}
