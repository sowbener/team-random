
using System;

namespace Bullseye.Helpers
{
    internal static class BsEnum
    {
        #region CBOItem
        public class BsCboItem
        {
            public readonly int E;
            private readonly string _s;

            public BsCboItem(int pe, string ps)
            {
                E = pe;
                _s = ps;
            }

            public override string ToString()
            {
                return _s;
            }
        }
        #endregion

        #region PetStuff
        public enum CallPet
        {
            None,
            Pet1 = 1,
            Pet2 = 2,
            Pet3 = 3,
            Pet4 = 4,
            Pet5 = 5
        }
        #endregion

        #region Hotkey ENUMS
        [Flags]
        public enum KeyStates
        {
            None = 0,
            Down = 1,
        }

        public enum Mode
        {
            Auto,
            Hotkey
        }
        #endregion

        #region General ENUMS
        public enum AbilityTrigger
        {
            Never,
            Always,
            OnBossDummy,
            OnBlTwHr
        }

        public enum VcTrigger
        {
            Never,
            Always,
            OnT15Proc
        }

        public enum InterruptList
        {
            MoP,
            NextExpensionPack
        }

        public enum Shouts
        {
            BattleShout,
            CommandingShout
        }

        public enum LagTolerance
        {
            No = 0,
            Yes
        }
        #endregion
    }
}
