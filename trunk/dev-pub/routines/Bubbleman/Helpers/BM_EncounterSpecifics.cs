using Styx;
using Styx.TreeSharp;

namespace Bubbleman.Helpers
{
    internal class BMEncounterSpecifics
    {
        // Heart of Fear - Amber-Shaper Un'sok
        public static Composite HandleActionBarInterrupts()
        {
            return new PrioritySelector(
                new Decorator(ret => StyxWoW.Me.HasAura(122370),
                              new PrioritySelector(
                                  BMLua.RunMacroText("/click OverrideActionBarButton1", ret => StyxWoW.Me.CurrentTarget != null && StyxWoW.Me.CurrentTarget.CastingSpellId == 122402),
                                  BMLua.RunMacroText("/click OverrideActionBarButton2", ret => StyxWoW.Me.IsCasting),
                                  BMLua.RunMacroText("/click OverrideActionBarButton4", ret => StyxWoW.Me.HealthPercent < 17)
                                  )));
        }
    }
}
