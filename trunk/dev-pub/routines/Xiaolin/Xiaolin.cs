
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

using Xiaolin.Helpers;
using Xiaolin.Interfaces.GUI;
using Xiaolin.Managers;
using JetBrains.Annotations;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using WW = Xiaolin.Routines.XIWindwalker;
using BM = Xiaolin.Routines.XIBrewmaster;
using SG = Xiaolin.Interfaces.Settings.XISettings;
using G = Xiaolin.Routines.XIGlobal;
using Spell = Xiaolin.Core.XISpell;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Xiaolin.Core;

namespace Xiaolin
{
    public class XIMain : CombatRoutine
    {
        [UsedImplicitly]
        public static XIMain Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static readonly Version Revision = new Version(1, 0, 9);

        internal static readonly string XIName = "Xiaolin - IR " + Revision;

        public override string Name { get { return XIName; } }
        public override WoWClass Class { get { return WoWClass.Monk; } }

        #region BotEvents
        public XIMain()
        {
            Instance = this;
            BotEvents.OnBotStopped += XIHotKeyManager.BotEvents_OnBotStopped;
            BotEvents.OnBotStarted += XIHotKeyManager.BotEvents_OnBotStarted;
        }

        private static void StopBot(string reason)
        {
            XILogger.InitLogW(reason);
            TreeRoot.Stop();
        }
        #endregion

        #region Graphical User Interface
        public override bool WantButton { get { return true; } }
        public override void OnButtonPress()
        {
            new XIInterface().ShowDialog();
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

            }

            catch (Exception e)
            {
                XILogger.DebugLog("Exception thrown in Pulse(): {0}", e);
            }
        }


#endregion

        #region Initialize
        public override void Initialize()
        {
            XILogger.InitLogO("Team Random presents");
            XILogger.InitLogF(Name + " by nomnomnom & alxaw.");
            XILogger.InitLogF("Internal revision: " + Revision);
            XILogger.InitLogO("Our special thanks go out to:");
            XILogger.InitLogF("Entire PureRotation / CLU Team.");
            XILogger.InitLogF("Apoc, Highvoltz & Singular Team.");
            XILogger.InitLogF("All testers & contributors.");
            XILogger.InitLogF("A XIll list is available on the forums.");
            
            /* Update TalentManager */
            try { XITalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }

            XILogger.InitLogO("Your specialization is " + XITalentManager.CurrentSpec + " and your race is " + Me.Race + ".");

            /* Gather required information */
            XILogger.StatCounter();
            XILogger.LogTimer(500);
            //Cooldownwatcher
            CooldownWatcher.Initialize();

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
                XILogger.InitLogF("Initializing pre-combat buff behaviours.");
                _preCombatBuffBehavior = new PrioritySelector(G.InitializePreBuff);
            }
            return null;
        }

        internal static Composite CreateCombat()
        {
            return new Switch<WoWSpec>(ret => Me.Specialization,
                new SwitchArgument<WoWSpec>(WoWSpec.MonkWindwalker, WW.InitializeWindwalker),
                new SwitchArgument<WoWSpec>(WoWSpec.MonkBrewmaster, BM.InitializeBrewmaster));
        }
        #endregion

        
    }
}
