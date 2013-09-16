
using System;

namespace Shammy.Helpers
{
    internal static class SmEnum
    {
        #region CBOItem
        public class SmCboItem
        {
            public readonly int E;
            private readonly string _s;

            public SmCboItem(int pe, string ps)
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

        public enum WeaponImbueM
        {
          Windfury, 
          Flametongue, 
          Frostbrand, 
          Rockbiter

        }

        public enum WeaponImbueO
        {
            Windfury,
            Flametongue,
            Frostbrand,
            Rockbiter

        }
        #endregion
    }
}
