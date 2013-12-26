
#region Bugs & To-Do
/*
 * Current Bugs & To-do's:
 * 
 * DONE - Rename entire routine including all namings / namespaces.
 * DONE - Create setting to enable AoE usage on Garalon & Twin Consorts.
 * DONE - Improve composites to dump close to all rage when enemyunit is almost dead.
 * DONE - Create de-sync option for trinkets/hands (Separate them from cooldowns).
 * DONE - Implement tierset detection.
 * DONE - Prot: Enable usage for VC/IV on T15 2P
 * DONE - Prot: When you have the T15 4P bonus. only cast demoshout when shield slam and maybe revenge arent on cd.
 * DONE - Support facing, targetting and movement via the I Want Movement plugin.
 * DONE - Re-do Unholy AoE for optimal DPS/Performance (More code but less checks, should perform better).
 * DONE - Prot: Create VC/ImpVic threshold - Also fixed T15-2P setbonus with settings.
 * DONE - Fix spell-queueing with On-Keypress hotkeys.
 * DONE - Finish Frost Spec with GUI.
 * DONE - Create option to disable pre-buffing shouts.
 * DONE - Fix AoE execute from PM --> tombot
 * Tweak the new GUI.
 * Create PvP check.
 * Investigate HP scan for all party members (Rallying Cry).
 * Do something with VC when aura is fading.
 * Fix tierdetection.
 * 
 */
#endregion

using Bubbleman.Helpers;
using Bubbleman.Interfaces.GUI;
using Bubbleman.Managers;
using JetBrains.Annotations;
using Styx;
using Styx.CommonBot;
using System.Linq;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using WW = Bubbleman.Routines.BMRetribution;
using BM = Bubbleman.Routines.BMProtection;
using SG = Bubbleman.Interfaces.Settings.BMSettings;
using G = Bubbleman.Routines.BMGlobal;
using Spell = Bubbleman.Core.BMSpell;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Bubbleman.Core;

namespace Bubbleman
{
    public class BMMain : CombatRoutine
    {
        [UsedImplicitly]
        public static BMMain Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static readonly Version Revision = new Version(1, 2);
        internal static double _initap = 0;
        internal static double _NewAP = 0;
        internal static double _NoModifierAP = 0;

        internal static readonly string BMName = "Bubbleman - IR " + Revision;

        public override string Name { get { return BMName; } }
        public override WoWClass Class { get { return WoWClass.Paladin; } }

        #region BotEvents
        public BMMain()
        {
            Instance = this;
            BotEvents.OnBotStopped += BMHotKeyManager.BotEvents_OnBotStopped;
            BotEvents.OnBotStarted += BMHotKeyManager.BotEvents_OnBotStarted;
        }

        private static void StopBot(string reason)
        {
            BMLogger.InitLogW(reason);
            TreeRoot.Stop();
        }
        #endregion

        #region Graphical User Interface
        public override bool WantButton { get { return true; } }
        public override void OnButtonPress()
        {
            new BMInterface().ShowDialog();
        }
        #endregion

        #region Behaviors
        private Composite _combat, _combatBehavior, _preCombatBuffBehavior;

        public override Composite CombatBehavior { get { return _combatBehavior ?? (_combatBehavior = CreateCombat()); } }
        public override Composite PreCombatBuffBehavior { get { return _preCombatBuffBehavior; } }
        #endregion

        #region Pulse
        public override void Pulse()
        {
            try
            {
              
                // Double cast shit
                Spell.PulseDoubleCastEntries();
                if (!Me.Combat && !Me.HasAura(120267))
               _initap = StyxWoW.Me.AttackPower;
                _NewAP = BMLua.Vengeance(120267);
                _NoModifierAP = StyxWoW.Me.AttackPower;



            }

            catch (Exception e)
            {
                BMLogger.DebugLog("Exception thrown in Pulse(): {0}", e);
            }
        }


#endregion

        #region Initialize
        public override void Initialize()
        {
            BMLogger.InitLogO("Team Random presents");
            BMLogger.InitLogF(Name + " by nomnomnom & alxaw.");
            BMLogger.InitLogF("Internal revision: " + Revision);
            BMLogger.InitLogO("Our special thanks go out to:");
            BMLogger.InitLogF("Entire PureRotation / CLU Team.");
            BMLogger.InitLogF("Apoc, Highvoltz & Singular Team.");
            BMLogger.InitLogF("All testers & contributors.");
            BMLogger.InitLogF("A BMll list is available on the forums.");
            
            /* Update TalentManager */
            try { BMTalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }

            BMLogger.InitLogO("Your specialization is " + BMTalentManager.CurrentSpec + " and your race is " + Me.Race + ".");

            /* Gather required information */
            BMLogger.StatCounter();
            BMLogger.LogTimer(500);
            G.GetBinding();
            

            //Cooldownwatcher
            CooldownWatcher.Initialize();
            CombatLogHandler.Initialize();
            _initap = StyxWoW.Me.AttackPower;
            _NewAP = StyxWoW.Me.AttackPower;

            /* Start Combat */
            PreBuffSelector();
            CreateCombat();
        }
        #endregion

        #region Buff & Rotation Selector
        internal Composite PreBuffSelector()
        {
            if (_preCombatBuffBehavior == null)
            {
                BMLogger.InitLogF("Initializing pre-combat buff behaviours.");
                _preCombatBuffBehavior = new PrioritySelector(G.InitializePreBuff);
            }
            return null;
        }

        internal static Composite CreateCombat()
        {
            return new Switch<WoWSpec>(ret => Me.Specialization,
                new SwitchArgument<WoWSpec>(WoWSpec.PaladinRetribution, WW.InitializeRetribution),
                new SwitchArgument<WoWSpec>(WoWSpec.PaladinProtection, BM.InitializeProtection));
        }
        #endregion

        
    }
}
