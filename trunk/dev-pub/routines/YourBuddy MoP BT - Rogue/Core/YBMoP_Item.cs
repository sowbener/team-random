// Tnx Singular :).
using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using YBMoP_BT_Rogue.Helpers;
using YBMoP_BT_Rogue.Routines;
using Action = Styx.TreeSharp.Action;
using G = YBMoP_BT_Rogue.Routines.YBGlobal;
using SF = YBMoP_BT_Rogue.Interfaces.Settings.YBSettingsAss;
using SP = YBMoP_BT_Rogue.Interfaces.Settings.YBSettingsCom;

namespace YBMoP_BT_Rogue.Core
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

        public static void UseItem(WoWItem item)
        {
            YBLogger.CombatLogP("Using item: " + item.Name);
            item.Use();
        }

        private static bool CanUseItem(WoWItem item)
        {
            return item != null && item.Usable && item.Cooldown <= 0;
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

        public static Composite UseItem(uint id)
        {
            return new PrioritySelector(
                ctx => ObjectManager.GetObjectsOfType<WoWItem>().FirstOrDefault(item => item.Entry == id),
                new Decorator(
                    ctx => ctx != null && CanUseItem((WoWItem)ctx),
                    new Action(ctx => UseItem((WoWItem)ctx))));
        }


        public static Composite AssaUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (G.VendettaAura || G.ShadowbladesAura || G.IloveyouSND) && (
                    (SF.Instance.Trinket1 == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.Trinket1 == AbilityTrigger.OnBlTw && (YBGlobal.SpeedBuffsAura)) ||
                    (SF.Instance.Trinket1 == AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (G.VendettaAura || G.ShadowbladesAura || G.IloveyouSND) && (
                    (SF.Instance.Trinket1 == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.Trinket1 == AbilityTrigger.OnBlTw && (YBGlobal.SpeedBuffsAura)) ||
                    (SF.Instance.Trinket1 == AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (G.VendettaAura || G.ShadowbladesAura || G.IloveyouSND) && (
                    (SF.Instance.UseHands == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.UseHands == AbilityTrigger.OnBlTw && (YBGlobal.SpeedBuffsAura)) ||
                    (SF.Instance.UseHands == AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite ComUseItems()
        {
            return new PrioritySelector(
                new Decorator(ret => (G.ShadowbladesAura || G.IloveyouSND) && (
                    (SF.Instance.Trinket1 == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.Trinket1 == AbilityTrigger.OnBlTw && (YBGlobal.SpeedBuffsAura)) ||
                    (SF.Instance.Trinket1 == AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket1),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (G.ShadowbladesAura || G.IloveyouSND) && (
                    (SF.Instance.Trinket1 == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.Trinket1 == AbilityTrigger.OnBlTw && (YBGlobal.SpeedBuffsAura)) ||
                    (SF.Instance.Trinket1 == AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Trinket2),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))),
                new Decorator(ret => (G.ShadowbladesAura || G.IloveyouSND) && (
                    (SF.Instance.UseHands == AbilityTrigger.OnBossDummy && YBUnit.IsTargetBoss) ||
                    (SF.Instance.UseHands == AbilityTrigger.OnBlTw && (YBGlobal.SpeedBuffsAura)) ||
                    (SF.Instance.UseHands == AbilityTrigger.Always)),
                    new PrioritySelector(ctx => StyxWoW.Me.Inventory.GetItemBySlot((uint)WoWInventorySlot.Hands),
                        new Decorator(ctx => ctx != null && CanUseEquippedItem((WoWItem)ctx),
                            new Action(ctx => UseItem((WoWItem)ctx))))));
        }

        public static Composite AssaUseHealthStone()
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
    }
}
