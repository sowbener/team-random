
using System;

namespace Waldo.Helpers
{
    internal static class WaEnum
    {
        #region CBOItem
        public class WaCboItem
        {
            public readonly int E;
            private readonly string _s;

            public WaCboItem(int pe, string ps)
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
        
        public enum PreperationUsage
        {
            Never,
            VanishCooldown,
            OnBossAndVanishCooldown
        }

        public enum InterruptList
        {
            MoP,
            NextExpensionPack
        }

        public enum PoisonM
        {
            Deadly,
            Wound,
            Leeching,
            Crippling,
            Mindnumbing

        }

        public enum PoisonO
        {
            Deadly,
            Wound,
            Leeching,
            Crippling,
            Mindnumbing

        }

        #endregion
    }
}
