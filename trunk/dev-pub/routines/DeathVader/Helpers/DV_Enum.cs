
using System;

namespace DeathVader.Helpers
{
    internal static class DvEnum
    {
        #region CBOItem
        public class DvCboItem
        {
            public readonly int E;
            private readonly string _s;

            public DvCboItem(int pe, string ps)
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
            Hotkey,
            Abilities
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
