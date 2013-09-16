using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YBMoP_BT_Rogue.Core;
using YBMoP_BT_Rogue.Interfaces.Settings;
using YBMoP_BT_Rogue.Helpers;

namespace YBMoP_BT_Rogue.Helpers {
    using CommonBehaviors.Actions;
    using Styx;
    using Styx.CommonBot;
    using Styx.Pathing;
    using Styx.TreeSharp;

    public static class Poisons {
        private static bool NeedsPoison {
            get {
                return StyxWoW.Me.Inventory.Equipped.MainHand != null &&
                       StyxWoW.Me.Inventory.Equipped.MainHand.TemporaryEnchantment.Id == 0 &&
                       StyxWoW.Me.Inventory.Equipped.MainHand.ItemInfo.WeaponClass != WoWItemWeaponClass.FishingPole;
            }
        }

        private static int MainHandPoisonAss {
            get {
                switch (YBSettingsAss.Instance.PoisonSelectorM)
                {

                    case PoisonM.Crippling:
                        return 8679;
                    case PoisonM.Deadly:
                        return 2823;
                    case PoisonM.Mindnumbing:
                        return 5761;
                    case PoisonM.Wound:
                        return 108215;
                    case PoisonM.Leeching:
                        return 108211;
                    default:
                        return 0;
                }
            }
        }

        private static int OffHandHandPoisonAss {
            get {
                switch (YBSettingsAss.Instance.PoisonSelectorO) 
                {
                    case PoisonO.Mindnumbing:
                        return 5761;
                    case PoisonO.Crippling:
                        return 3408;
                    case PoisonO.Wound:
                        return 108215;
                    case PoisonO.Leeching:
                        return 108211;
                    case PoisonO.Deadly:
                        return 2823;
                    default:
                        return 0;
                }
            }
        }

        private static int MainHandPoisonCom
        {
            get
            {
                switch (YBSettingsCom.Instance.PoisonSelectorM)
                {

                    case PoisonM.Crippling:
                        return 8679;
                    case PoisonM.Deadly:
                        return 2823;
                    case PoisonM.Mindnumbing:
                        return 5761;
                    case PoisonM.Wound:
                        return 108215;
                    case PoisonM.Leeching:
                        return 108211;
                    default:
                        return 0;
                }
            }
        }

        private static int OffHandHandPoisonCom
        {
            get
            {
                switch (YBSettingsCom.Instance.PoisonSelectorO)
                {
                    case PoisonO.Mindnumbing:
                        return 5761;
                    case PoisonO.Crippling:
                        return 3408;
                    case PoisonO.Wound:
                        return 108215;
                    case PoisonO.Leeching:
                        return 108211;
                    case PoisonO.Deadly:
                        return 2823;
                    default:
                        return 0;
                }
            }
        }

        public static Composite CreateApplyPoisonsAss() 
        {
            return new PrioritySelector(
                new Decorator(
                    ret => !StyxWoW.Me.HasAura(MainHandPoisonAss) && SpellManager.HasSpell(MainHandPoisonAss),
                    new Sequence(
                        new Action(ret => Navigator.PlayerMover.MoveStop()),
                        YBSpell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(MainHandPoisonAss)),
                        YBSpell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed()))),

                new Decorator(
                    ret =>
                    !StyxWoW.Me.HasAura(OffHandHandPoisonAss) && SpellManager.HasSpell(OffHandHandPoisonAss),
                    new Sequence(
                        new Action(ret => Navigator.PlayerMover.MoveStop()),
                        YBSpell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(OffHandHandPoisonAss)),
                        YBSpell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed())))
                );

        }

        public static Composite CreateApplyPoisonsCom()
        {
            return new PrioritySelector(
                new Decorator(
                    ret => !StyxWoW.Me.HasAura(MainHandPoisonCom) && SpellManager.HasSpell(MainHandPoisonCom),
                    new Sequence(
                        new Action(ret => Navigator.PlayerMover.MoveStop()),
                        YBSpell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(MainHandPoisonCom)),
                        YBSpell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed()))),

                new Decorator(
                    ret =>
                    !StyxWoW.Me.HasAura(OffHandHandPoisonCom) && SpellManager.HasSpell(OffHandHandPoisonCom),
                    new Sequence(
                        new Action(ret => Navigator.PlayerMover.MoveStop()),
                        YBSpell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(OffHandHandPoisonCom)),
                        YBSpell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed())))
                );

        }
    }
}
