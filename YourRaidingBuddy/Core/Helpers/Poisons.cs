using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourBuddy.Core;
using YourBuddy.Interfaces.Settings;
using YourBuddy.Core.Helpers;

namespace YourBuddy.Core.Helpers 
{
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
                switch (InternalSettings.Instance.Assassination.PoisonSelectorM)
                {

                    case Enum.PoisonM.Crippling:
                        return 8679;
                    case Enum.PoisonM.Deadly:
                        return 2823;
                    case Enum.PoisonM.Mindnumbing:
                        return 5761;
                    case Enum.PoisonM.Wound:
                        return 108215;                   
                    case Enum.PoisonM.Leeching:
                        return 108211;
                    default:
                        return 0;
                }
            }
        }

        private static int OffHandHandPoisonAss {
            get {
                switch (InternalSettings.Instance.Assassination.PoisonSelectorO) 
                {
                    case Enum.PoisonO.Mindnumbing:
                        return 5761;
                    case Enum.PoisonO.Crippling:
                        return 3408;
                    case Enum.PoisonO.Wound:
                        return 108215;
                    case Enum.PoisonO.Leeching:
                        return 108211;
                    case Enum.PoisonO.Paralytic:
                        return 108215;
                    case Enum.PoisonO.Deadly:
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
                switch (InternalSettings.Instance.Combat.PoisonSelectorM)
                {

                    case Enum.PoisonM.Crippling:
                        return 8679;
                    case Enum.PoisonM.Deadly:
                        return 2823;
                    case Enum.PoisonM.Mindnumbing:
                        return 5761;
                    case Enum.PoisonM.Wound:
                        return 108215;
                    case Enum.PoisonM.Leeching:
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
                switch (InternalSettings.Instance.Combat.PoisonSelectorO)
                {
                    case Enum.PoisonO.Mindnumbing:
                        return 5761;
                    case Enum.PoisonO.Crippling:
                        return 3408;
                    case Enum.PoisonO.Wound:
                        return 108215;
                    case Enum.PoisonO.Leeching:
                        return 108211;
                    case Enum.PoisonO.Paralytic:
                        return 108215;
                    case Enum.PoisonO.Deadly:
                        return 2823;
                    default:
                        return 0;
                }
            }
        }

        private static int MainHandPoisonSub
        {
            get
            {
                switch (InternalSettings.Instance.Subtlety.PoisonSelectorM)
                {

                    case Enum.PoisonM.Crippling:
                        return 8679;
                    case Enum.PoisonM.Deadly:
                        return 2823;
                    case Enum.PoisonM.Mindnumbing:
                        return 5761;
                    case Enum.PoisonM.Wound:
                        return 108215;
                    case Enum.PoisonM.Leeching:
                        return 108211;
                    default:
                        return 0;
                }
            }
        }
        private static int OffHandPoisonSub
        {
            get
            {
                switch (InternalSettings.Instance.Subtlety.PoisonSelectorO)
                {

                    case Enum.PoisonO.Crippling:
                        return 8679;
                    case Enum.PoisonO.Deadly:
                        return 2823;
                    case Enum.PoisonO.Mindnumbing:
                        return 5761;
                    case Enum.PoisonO.Wound:
                        return 108215;
                    case Enum.PoisonO.Leeching:
                        return 108211;
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
                        Spell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(MainHandPoisonAss)),
                        Spell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed()))),

                new Decorator(
                    ret =>
                    !StyxWoW.Me.HasAura(OffHandHandPoisonAss) && SpellManager.HasSpell(OffHandHandPoisonAss),
                    new Sequence(
                        new Action(ret => Navigator.PlayerMover.MoveStop()),
                        Spell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(OffHandHandPoisonAss)),
                        Spell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed())))
                );

        }

        public static Composite CreateApplyPoisonsSub()
        {
            return new PrioritySelector(
                new Decorator(
                    ret => !StyxWoW.Me.HasAura(MainHandPoisonSub) && SpellManager.HasSpell(MainHandPoisonSub),
                    new Sequence(
                        new Action(ret => Navigator.PlayerMover.MoveStop()),
                        Spell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(MainHandPoisonSub)),
                        Spell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed()))),

                new Decorator(
                    ret =>
                    !StyxWoW.Me.HasAura(OffHandPoisonSub) && SpellManager.HasSpell(OffHandPoisonSub),
                    new Sequence(
                        new Action(ret => Navigator.PlayerMover.MoveStop()),
                        Spell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(OffHandPoisonSub)),
                        Spell.CreateWaitForLagDuration(),
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
                        Spell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(MainHandPoisonCom)),
                        Spell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed()))),

                new Decorator(
                    ret =>
                    !StyxWoW.Me.HasAura(OffHandHandPoisonCom) && SpellManager.HasSpell(OffHandHandPoisonCom),
                    new Sequence(
                        new Action(ret => Navigator.PlayerMover.MoveStop()),
                        Spell.CreateWaitForLagDuration(),
                        new Action(ret => SpellManager.CastSpellById(OffHandHandPoisonCom)),
                        Spell.CreateWaitForLagDuration(),
                        new WaitContinue(2, ret => StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(10, ret => !StyxWoW.Me.IsCasting, new ActionAlwaysSucceed()),
                        new WaitContinue(1, ret => false, new ActionAlwaysSucceed())))
                );

        }
    }
}
