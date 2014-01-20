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
using G = YourBuddy.Rotations.Global;
using SG = YourBuddy.Interfaces.Settings.InternalSettings;
using U = YourBuddy.Core.Unit;

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

            return item.Usable && item.Cooldown == 0;
        }

        public static Composite UseBagItem(int name, CanRunDecoratorDelegate cond, string reason)
        {
            WoWItem item = null;
            return new Decorator(
                delegate(object a)
                {
                    if (!cond(a)) return false;


                    item = Me.BagItems.FirstOrDefault(x => x != null && x.Entry == name && x.Usable && x.Cooldown <= 0 && x.ItemInfo.RequiredLevel <= StyxWoW.Me.Level);

                    return item != null;
                },
                new Sequence(
                    new Action(a => Logger.CombatLog("[Using Item: {0}] [Reason: {1}]", name, reason)),
                    new Action(a => item.Use()),
                    new Action(a => RunStatus.Failure)));
        }

        private static bool CanUseTrinket(Enum.AbilityTrigger usage, WoWItem trinket)
        {
            var canUseTrinket = trinket != null && CanUseEquippedItem(trinket);

            switch (usage)
            {
                case Enum.AbilityTrigger.OnBossDummy:
                    return canUseTrinket && U.IsTargetBoss;
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

        //Monk

        internal static void UseWindwalkerItems()
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

        internal static void UseBrewmasterItems()
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

        //Deathknight
        internal static void UseBloodItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Blood.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Blood.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Blood.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Blood.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Blood.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Blood.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }
        internal static void UseUnholyItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Unholy.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Unholy.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Unholy.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Unholy.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Unholy.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Unholy.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }
        internal static void UseFrostItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Frost.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Frost.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Frost.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Frost.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Frost.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Frost.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }

        //Paladin
        internal static void UseRetributionItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Retribution.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Retribution.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Retribution.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Retribution.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Retribution.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Retribution.UseHands, hands) && Me.HasAura("Inquisition") && (!Me.HasAura("Anicent Power") || Spell.GetAuraStack(Me, "Ancient Power") == 12))//buff.inquisition.up&(buff.ancient_power.down|buff.ancient_power.stack=12))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }
        internal static void UseProtectionItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Protection.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Protection.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Protection.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Protection.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Protection.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Protection.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }

        //Rogue

        internal static void UseAssassinationItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Assassination.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Assassination.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Assassination.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Assassination.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Assassination.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Assassination.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }
        internal static void UseCombatItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Combat.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Combat.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Combat.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Combat.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Combat.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Combat.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }
        internal static void UseSubtletyItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Subtlety.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Subtlety.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Subtlety.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Subtlety.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Subtlety.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Subtlety.UseHands, hands) && Me.HasAura("Shadow Dance"))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }

        //Hunter
        internal static void UseSurvivalItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Survival.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Survival.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Survival.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Survival.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Survival.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Survival.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }
        internal static void UseMarksmanshipItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Marksmanship.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Marksmanship.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Marksmanship.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Marksmanship.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Marksmanship.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Marksmanship.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }
        internal static void UseBeastmasteryItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Beastmastery.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Beastmastery.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Beastmastery.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Beastmastery.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Beastmastery.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Beastmastery.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }

        //Shaman
        internal static void UseEnhancementItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Enhancement.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Enhancement.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Enhancement.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Enhancement.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Enhancement.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Enhancement.UseHands, hands))
            {
                hands.Use();
                Logger.CombatLogFb("Using: Engineering hands are used.");
            }
        }
        internal static void UseElementalItems()
        {
            var firstTrinket = StyxWoW.Me.Inventory.Equipped.Trinket1;
            var secondTrinket = StyxWoW.Me.Inventory.Equipped.Trinket2;
            var hands = StyxWoW.Me.Inventory.Equipped.Hands;

            if (InternalSettings.Instance.Elemental.Trinket1 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Elemental.Trinket2 == Enum.AbilityTrigger.Never &&
                InternalSettings.Instance.Elemental.UseHands == Enum.AbilityTrigger.Never)
                return;

            if (CanUseTrinket(InternalSettings.Instance.Elemental.Trinket1, firstTrinket))
            {
                firstTrinket.Use();
                Logger.CombatLogFb("Using: Slot 1 trinket is used.");
            }

            if (CanUseTrinket(InternalSettings.Instance.Elemental.Trinket2, secondTrinket))
            {
                secondTrinket.Use();
                Logger.CombatLogFb("Using: Slot 2 trinket is used.");
            }

            if (CanUseHands(InternalSettings.Instance.Elemental.UseHands, hands))
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
