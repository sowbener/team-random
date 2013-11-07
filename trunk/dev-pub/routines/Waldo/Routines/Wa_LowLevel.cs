using CommonBehaviors.Actions;
using Waldo.Core;
using Waldo.Helpers;
using Waldo.Managers;
using Styx;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Forms;
using G = Waldo.Routines.WaGlobal;
using I = Waldo.Core.WaItem;
using Lua = Waldo.Helpers.WaLua;
using T = Waldo.Managers.WaTalentManager;
using SG = Waldo.Interfaces.Settings.WaSettings;
using SH = Waldo.Interfaces.Settings.WaSettingsH;
using Spell = Waldo.Core.WaSpell;
using U = Waldo.Core.WaUnit;
using Styx.CommonBot;
using Styx.WoWInternals;
using TalentManager = Waldo.Managers.WaTalentManager;

namespace Waldo.Routines
{
    class WaLowLevel
    {
        private static LocalPlayer Me { get { return StyxWoW.Me; } }

        #region Initialize Rotations
        internal static Composite InitializeLowLevel
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(ret => SG.Instance.General.CheckTreePerformance,
                        WaLogger.TreePerformance("InitializeLowLevel")),
                    new Decorator(ret => (WaHotKeyManager.IsPaused || !WaUnit.DefaultCheck), new ActionAlwaysSucceed()),
                        new Action(delegate { Spell.GetCachedAuras(); return RunStatus.Failure; }),
                        G.InitializeOnKeyActions(),
                        Movement.MovingFacingBehavior(),
                        Movement.CreateMoveToLosBehavior(),
                        LowLevel());

            }
        }
        #endregion

        #region Rotations

        static Composite LowLevel()
        {
            return new PrioritySelector(
                Spell.Cast("Eviscerate", ret => WaLua.PlayerComboPts > 4),
                Spell.Cast("Sinister Strike"));

      
        }
        #endregion

        #region Tricks of the trade
        private static WoWUnit TricksTarget
        {
            get
            {
                return G.BestTricksTarget;
            }
        }
        #endregion
    }
}
