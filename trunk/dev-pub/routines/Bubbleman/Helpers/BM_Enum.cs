
using System;

namespace Bubbleman.Helpers
{
    internal static class BMEnum
    {
        #region CBOItem
        public class BMCboItem
        {
            public readonly int E;
            private readonly string _s;

            public BMCboItem(int pe, string ps)
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

        public enum InterruptList
        {
            MoP,
            NextExpensionPack
        }

        public enum VcTrigger
        {
            Never,
            Always,
            OnT15Proc
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
