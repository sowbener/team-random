using Xiaolin.Core;
using Xiaolin.Helpers;
using Xiaolin.Managers;
using Styx;
using Styx.CommonBot;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Styx.TreeSharp.Action;
using KP = Xiaolin.Managers.XIHotKeyManager.KeyboardPolling;
using SG = Xiaolin.Interfaces.Settings.XISettings;
using SH = Xiaolin.Interfaces.Settings.XISettingsH;
using Spell = Xiaolin.Core.XISpell;
using T = Xiaolin.Managers.XITalentManager;


namespace Xiaolin.Routines
{
   public class XIGlobal
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Global Used Composites
         internal static Composite InitializePreBuff
        {
            get
            {
                return new PrioritySelector(
                   );
            }
        }

        internal static Composite InitializeCaching()
        {
            return new PrioritySelector(
                new Action(delegate { XISpell.GetCachedAuras(); return RunStatus.Failure; }),
                new Action(delegate { XIUnit.GetNearbyAttackableUnitsCount(); return RunStatus.Failure; })
                );
        }

        internal static bool SpeedBuffsAura { get { return Me.HasAura(2825) || Me.HasAura(80353) || Me.HasAura(32182) || Me.HasAura(90355); } }

        #endregion

    }
}
