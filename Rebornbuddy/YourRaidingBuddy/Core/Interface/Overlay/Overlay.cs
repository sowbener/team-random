using Buddy.Overlay;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Interface.Overlay
{
    internal static class Overlay
    {
        internal static OverlayUIComponent YRBOverlayComponent;

        /// <summary>
        /// Creates the overlay.
        /// </summary>
        internal static void CreateOverlay()
        {
            if (!InternalSettings.Instance.General.Overlay) return;

            YRBOverlayComponent = new FuOverlayControl(); // Create an instance of the class. This is your overlay control now.
            ff14bot.Core.OverlayManager.AddUIComponent(YRBOverlayComponent); // Add this component instance to the overlay window.
        }

        /// <summary>
        /// Rebuilds the overlay.
        /// Uses a variable which triggers the Update() method in FuOverlayControl.
        /// </summary>
        internal static void RebuildOverlay(bool reinitialized)
        {
            /* Required to rebuild this if we are reinitializing the routine */
            if (!reinitialized) return;
            if (!VariableBook.OverlayActive) return;

            VariableBook.OverlayRebuildRequired = true;
        }

        /// <summary>
        /// Disposes the overlay.
        /// </summary>
        internal static void Terminate()
        {
            if (!VariableBook.OverlayActive) return;

            FuOverlayControl.DisposeOverlay();
            VariableBook.OverlayRebuildRequired = false;
            VariableBook.OverlayActive = false;
        }
    }
}