using Styx;
using Styx.TreeSharp;

namespace Waldo.Helpers
{
    internal class WaEncounterSpecifics
    {
        // Heart of Fear - Amber-Shaper Un'sok
        public static Composite HandleActionBarInterrupts()
        {
            return new PrioritySelector(
                new Decorator(ret => StyxWoW.Me.HasAura(122370),
                              new PrioritySelector(
                                  WaLua.RunMacroText("/click OverrideActionBarButton1", ret => StyxWoW.Me.CurrentTarget != null && StyxWoW.Me.CurrentTarget.CastingSpellId == 122402),
                                  WaLua.RunMacroText("/click OverrideActionBarButton2", ret => StyxWoW.Me.IsCasting),
                                  WaLua.RunMacroText("/click OverrideActionBarButton4", ret => StyxWoW.Me.HealthPercent < 17)
                                  )));
        }
    }
}
