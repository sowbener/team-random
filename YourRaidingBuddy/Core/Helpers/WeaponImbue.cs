using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourRaidingBuddy.Core;
using YourRaidingBuddy.Interfaces.Settings;
using YourRaidingBuddy.Core.Helpers;
using SG = YourRaidingBuddy.Interfaces.Settings.InternalSettings;
using SH = YourRaidingBuddy.Interfaces.Settings.SettingsH;
using CommonBehaviors.Actions;
using Styx;
using Styx.CommonBot;
using Styx.Pathing;
using Styx.TreeSharp;

namespace YourRaidingBuddy.Core.Helpers
{

    public static class WeaponImbue
    {
        private static bool NeedsWeaponImbue
        {
            get
            {
                return StyxWoW.Me.Inventory.Equipped.MainHand != null &&
                       StyxWoW.Me.Inventory.Equipped.MainHand.TemporaryEnchantment.Id == 0 &&
                       StyxWoW.Me.Inventory.Equipped.MainHand.ItemInfo.WeaponClass != WoWItemWeaponClass.FishingPole;
            }
        }

        private static bool NeedsWeaponImbueOffhand
        {
            get
            {
                return StyxWoW.Me.Inventory.Equipped.OffHand != null &&
                       StyxWoW.Me.Inventory.Equipped.OffHand.TemporaryEnchantment.Id == 0 &&
                       StyxWoW.Me.Inventory.Equipped.OffHand.ItemInfo.WeaponClass != WoWItemWeaponClass.FishingPole;
            }
        }

        private static int MainHandWeaponImbueEnhancement
        {
            get
            {
                switch (SG.Instance.Enhancement.WeaponImbueSelectorM)
                {

                    case Enum.WeaponImbueM.Windfury:
                        return 8232;
                    case Enum.WeaponImbueM.Flametongue:
                        return 8024;
                    case Enum.WeaponImbueM.Frostbrand:
                        return 8033;
                    case Enum.WeaponImbueM.Rockbiter:
                        return 8017;
                    default:
                        return 0;
                }
            }
        }

        private static int OffHandWeaponImbueEnhancement
        {
            get
            {
                switch (SG.Instance.Enhancement.WeaponImbueSelectorO)
                {
                    case Enum.WeaponImbueO.Windfury:
                        return 8232;
                    case Enum.WeaponImbueO.Flametongue:
                        return 8024;
                    case Enum.WeaponImbueO.Frostbrand:
                        return 8033;
                    case Enum.WeaponImbueO.Rockbiter:
                        return 8017;
                    default:
                        return 0;
                }
            }
        }

        private static int MainHandWeaponImbueElemental
        {
            get
            {
                switch (SG.Instance.Elemental.WeaponImbueSelectorM)
                {

                    case Enum.WeaponImbueM.Windfury:
                        return 8232;
                    case Enum.WeaponImbueM.Flametongue:
                        return 8024;
                    case Enum.WeaponImbueM.Frostbrand:
                        return 8033;
                    case Enum.WeaponImbueM.Rockbiter:
                        return 8017;
                    default:
                        return 0;
                }
            }
        }

        public static Composite CreateWeaponImbueEnhancement()
        {
            return new PrioritySelector(
                new Decorator(
                    ret => NeedsWeaponImbue && SpellManager.HasSpell(MainHandWeaponImbueEnhancement),
                    new Sequence(
                        new Styx.TreeSharp.Action(ret => SpellManager.CastSpellById(MainHandWeaponImbueEnhancement)),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed()))),

                new Decorator(
                    ret =>
                    NeedsWeaponImbueOffhand && !StyxWoW.Me.HasAura(OffHandWeaponImbueEnhancement) && SpellManager.HasSpell(OffHandWeaponImbueEnhancement),
                    new Sequence(
                        new Styx.TreeSharp.Action(ret => SpellManager.CastSpellById(OffHandWeaponImbueEnhancement)),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed())))
                );

        }

        public static Composite CreateWeaponImbueElemental()
        {
            return new PrioritySelector(
                new Decorator(
                    ret => NeedsWeaponImbue && !StyxWoW.Me.HasAura(MainHandWeaponImbueElemental) && SpellManager.HasSpell(MainHandWeaponImbueElemental),
                    new Sequence(
                        new Styx.TreeSharp.Action(ret => SpellManager.CastSpellById(MainHandWeaponImbueElemental)),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed())))
                );

        }
    }
}
