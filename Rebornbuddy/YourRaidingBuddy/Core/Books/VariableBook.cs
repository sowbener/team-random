using ff14bot.Enums;
using ff14bot.Objects;

namespace YourRaidingBuddy.Books
{
    internal static class VariableBook
    {
        #region Initialize
        internal static void Initialize()
        {
            Tier4AbilityToggled = false;
            Tier6AbilityToggled = false;
            Tier7AbilityToggled = false;

            HkmPaused = false;
            HkmCooldowns = false;
            HkmMultiTarget = false;
            HkmSpecialKey = false;
            HkmSpecialKey1 = false;

            HostileUnitsCount = 0;
            FriendlyUnitsCount = 1;

            UnleashRangeNonAggroUnitsCount = 0;
            ProvokeRangeNonAggroAndTappedUnitsCount = 0;
            HostileUnitsTargettingMeCount = 0;

            RebuildRequired = false;
        }


        #endregion

        #region Extensions

        public static bool Between(this double distance, double min, double max)
        {
            return distance >= min && distance <= max;
        }

        public static bool Between(this float distance, float min, float max)
        {
            return distance >= min && distance <= max;
        }

        public static bool Between(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }
        public static bool Between(this uint value, uint min, uint max)
        {
            return value >= min && value <= max;
        }

        #endregion

        #region Class Stuff

        public static ClassJobType OldJob { get; set; }

        #endregion

        #region EventsTracker
        internal static bool RaidEncounterInProgress
        {
            get;
            set;
        }

        internal static bool Tier4AbilityToggled
        {
            get;
            set;
        }

        internal static bool Tier6AbilityToggled
        {
            get;
            set;
        }

        internal static bool Tier7AbilityToggled
        {
            get;
            set;
        }


        #endregion

        #region Hotkeys
        internal static bool HkmPaused
        {
            get;
            set;
        }

        internal static bool HkmCooldowns
        {
            get;
            set;
        }

        internal static bool HkmMultiTarget
        {
            get;
            set;
        }

        internal static bool HkmSpecialKey
        {
            get;
            set;
        }

        internal static bool HkmSpecialKey1
        {
            get;
            set;
        }

        internal static bool Tier4Logged
        {
            get;
            set;
        }

        internal static bool Tier6Logged
        {
            get;
            set;
        }

        internal static bool Tier7Logged
        {
            get;
            set;
        }

        #endregion

        #region Units
        /// <summary>
        ///     Required integers for the counts
        /// </summary>

        internal static int HostileUnitsCount
        {
            get;
            set;
        }

        internal static int FriendlyUnitsCount
        {
            get;
            set;
        }

        #endregion

        #region Dark Knight
        internal static int UnleashRangeNonAggroUnitsCount
        {
            get;
            set;
        }
        internal static int ProvokeRangeNonAggroAndTappedUnitsCount
        {
            get;
            set;
        }
        internal static int HostileUnitsTargettingMeCount
        {
            get;
            set;
        }
        internal static BattleCharacter NearestHostileUnitNotTargettingMe
        {
            get;
            set;
        }
        #endregion

        #region Other
        internal static bool RebuildRequired
        {
            get;
            set;
        }

        #endregion

        #region Overlay
        internal static bool OverlayActive
        {
            get;
            set;
        }

        internal static bool OverlayPaused
        {
            get;
            set;
        }

        internal static bool OverlayRefreshPaused
        {
            get;
            set;
        }

        internal static bool OverlayRefreshMultiTarget
        {
            get;
            set;
        }

        internal static bool OverlayRefreshCooldowns
        {
            get;
            set;
        }

        internal static bool OverlayRefreshCharges
        {
            get;
            set;
        }

        internal static bool OverlayRebuildRequired
        {
            get;
            set;
        }
        #endregion
    }
}
