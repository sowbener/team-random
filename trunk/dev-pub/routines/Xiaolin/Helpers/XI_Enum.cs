
using System;

namespace Xiaolin.Helpers
{
    internal static class XIEnum
    {
        #region CBOItem
        public class XICboItem
        {
            public readonly int E;
            private readonly string _s;

            public XICboItem(int pe, string ps)
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
