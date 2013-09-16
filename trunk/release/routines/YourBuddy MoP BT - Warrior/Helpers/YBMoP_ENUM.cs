
namespace YBMoP_BT_Warrior.Helpers
{
    internal static class YBEnum
    {
        #region Hotkey ENUMS
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
            Battle,
            Commanding
        }
        #endregion
    }
}
