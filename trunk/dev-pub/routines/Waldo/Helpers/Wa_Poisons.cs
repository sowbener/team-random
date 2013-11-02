using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waldo.Core;
using Waldo.Interfaces.Settings;
using Waldo.Helpers;

namespace Waldo.Helpers {
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
                switch (WaSettings.Instance.Assassination.PoisonSelectorM)
                {

                    case WaEnum.PoisonM.Crippling:
                        return 8679;
                    case WaEnum.PoisonM.Deadly:
                        return 2823;
                    case WaEnum.PoisonM.Mindnumbing:
                        return 5761;
                    case WaEnum.PoisonM.Wound:
                        return 108215;                   
                    case WaEnum.PoisonM.Leeching:
                        return 108211;
                    default:
                        return 0;
                }
            }
        }

        private static int OffHandHandPoisonAss {
            get {
                switch (WaSettings.Instance.Assassination.PoisonSelectorO) 
                {
                    case WaEnum.PoisonO.Mindnumbing:
                        return 5761;
                    case WaEnum.PoisonO.Crippling:
                        return 3408;
                    case WaEnum.PoisonO.Wound:
                        return 108215;
                    case WaEnum.PoisonO.Leeching:
                        return 108211;
                    case WaEnum.PoisonO.Paralytic:
                        return 108215;
                    case WaEnum.PoisonO.Deadly:
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
                switch (WaSettings.Instance.Combat.PoisonSelectorM)
                {

                    case WaEnum.PoisonM.Crippling:
                        return 8679;
                    case WaEnum.PoisonM.Deadly:
                        return 2823;
                    case WaEnum.PoisonM.Mindnumbing:
                        return 5761;
                    case WaEnum.PoisonM.Wound:
                        return 108215;
                    case WaEnum.PoisonM.Leeching:
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
                switch (WaSettings.Instance.Combat.PoisonSelectorO)
                {
                    case WaEnum.PoisonO.Mindnumbing:
                        return 5761;
                    case WaEnum.PoisonO.Crippling:
                        return 3408;
                    case WaEnum.PoisonO.Wound:
                        return 108215;
                    case WaEnum.PoisonO.Leeching:
                        return 108211;
                    case WaEnum.PoisonO.Paralytic:
                        return 108215;
                    case WaEnum.PoisonO.Deadly:
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
                        WaSpell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(MainHandPoisonAss)),
                        WaSpell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed()))),

                new Decorator(
                    ret =>
                    !StyxWoW.Me.HasAura(OffHandHandPoisonAss) && SpellManager.HasSpell(OffHandHandPoisonAss),
                    new Sequence(
                        new Action(ret => Navigator.PlayerMover.MoveStop()),
                        WaSpell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(OffHandHandPoisonAss)),
                        WaSpell.CreateWaitForLagDuration(),
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
                        WaSpell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(MainHandPoisonCom)),
                        WaSpell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed()))),

                new Decorator(
                    ret =>
                    !StyxWoW.Me.HasAura(OffHandHandPoisonCom) && SpellManager.HasSpell(OffHandHandPoisonCom),
                    new Sequence(
                        new Action(ret => Navigator.PlayerMover.MoveStop()),
                        WaSpell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(OffHandHandPoisonCom)),
                        WaSpell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed())))
                );

        }
    }
}
