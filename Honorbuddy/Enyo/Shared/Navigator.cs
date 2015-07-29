using System;
using Styx;
using Styx.Pathing;

namespace Enyo.Shared
{
    class EnyoNavigator : MeshNavigator
    {
        public override MoveResult MoveTo(WoWPoint location)
        {
            try
            {
                return MoveResult.Failed;
            }
            catch (Exception ex)
            {
                Logger.DiagnosticLog("EnyoNavigator.MoveTo() Exception: {0}", ex); return MoveResult.Failed;
            }
        }
    }
}
