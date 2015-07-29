using Styx.WoWInternals.WoWObjects;

namespace Enyo.Shared
{
    class Helpers
    {
        /// <summary>
        ///     Is the Object viable?
        /// </summary>
        public static bool IsViable(WoWObject wowObject)
        {
            return (wowObject != null) && wowObject.IsValid;
        }
    }
}
