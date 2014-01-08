
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

using DeathVader.Helpers;
using DeathVader.Interfaces.GUI;
using DeathVader.Managers;
using JetBrains.Annotations;
using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using DeathVader.Core;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using F = DeathVader.Routines.DvFrost;
using U = DeathVader.Routines.DvUnholy;
using B = DeathVader.Routines.DvBlood;
using SG = DeathVader.Interfaces.Settings.DvSettings;
using G = DeathVader.Routines.DvGlobal;
using Spell = DeathVader.Core.DvSpell;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace DeathVader
{
    public class DvMain : CombatRoutine
    {
        [UsedImplicitly]
        public static DvMain Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }

        internal static readonly Version Revision = new Version(1, 0, 8);
        internal static readonly string DvName = "Death Vader - IR " + Revision;

        public override string Name { get { return DvName; } }
        public override WoWClass Class { get { return WoWClass.DeathKnight; } }

        #region BotEvents
        public DvMain()
        {
            Instance = this;
            BotEvents.OnBotStopped += DvHotKeyManager.BotEvents_OnBotStopped;
            BotEvents.OnBotStarted += DvHotKeyManager.BotEvents_OnBotStarted;
            Styx.CommonBot.BotEvents.OnBotChanged += DvHotKeyManager.OnBotChanged;
        }

        private static void StopBot(string reason)
        {
            DvLogger.InitLogW(reason);
            TreeRoot.Stop();
        }
        #endregion

        #region Graphical User Interface
        public override bool WantButton { get { return true; } }
        public override void OnButtonPress()
        {
            new DvInterface().ShowDialog();
        }
        #endregion

        #region Behaviors
        private Composite _preCombatBuffBehavior;

        public override Composite CombatBehavior { get { return CombatSelector(); ; } }
        public override Composite PreCombatBuffBehavior { get { return _preCombatBuffBehavior; } }
        #endregion

        #region Pulse
        public override void Pulse()
        {
            try
            {
              
                // Double cast shit
                Spell.PulseDoubleCastEntries();

                if (!StyxWoW.IsInWorld || Me == null || !Me.IsValid || Me.IsDead)
                {
                    return;
                }

                /*Update Units*/
                DvUnit.GetNearbyAttackableUnitsCount();
                Spell.GetCachedAuras();

                //DotTTracker
                if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightUnholy && !DoTTracker.Initialized) DoTTracker.Initialize();

                if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightBlood)
                    DeathStrikeTracker.Initialize();
            }

            catch (Exception e)
            {
                DvLogger.DebugLog("Exception thrown in Pulse(): {0}", e);
            }
        }


#endregion

        #region Initialize
        public override void Initialize()
        {
            DvLogger.InitLogO("Team Random presents");
            DvLogger.InitLogF(Name + " by nomnomnom & alxaw.");
            DvLogger.InitLogF("Internal revision: " + Revision);
            DvLogger.InitLogO("Our special thanks go out to:");
            DvLogger.InitLogF("Entire PureRotation / CLU Team.");
            DvLogger.InitLogF("Apoc, Highvoltz & Singular Team.");
            DvLogger.InitLogF("All testers & contributors.");
            DvLogger.InitLogF("A Dvll list is available on the forums.");
            
            /* Update TalentManager */
            try { DvTalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }

            DvLogger.InitLogO("Your specialization is " + DvTalentManager.CurrentSpec + " and your race is " + Me.Race + ".");

            /*Update LUA Stats*/
            DvLua.PopulateSecondryStats();
            CooldownWatcher.Initialize();
            CombatLogHandler.Initialize();

            /* Gather required information */
            DvLogger.StatCounter();
            DvLogger.LogTimer(500);
            G.GetBinding();

            DvUnit.GetNearbyAttackableUnitsCount();

            /* Start Combat */
            PreBuffSelector();
            CombatSelector();
        }
        #endregion

        #region Buff & Rotation Selector
        internal Composite PreBuffSelector()
        {
            if (_preCombatBuffBehavior == null)
            {
                DvLogger.InitLogF("Initializing pre-combat buff behaviours.");
                _preCombatBuffBehavior = new PrioritySelector(G.InitializePreBuff);
            }
            return null;
        }


        internal Composite CombatSelector()
        {
            return new Switch<WoWSpec>(ret => Me.Specialization,
                new SwitchArgument<WoWSpec>(WoWSpec.DeathKnightBlood, B.InitializeBlood),
                new SwitchArgument<WoWSpec>(WoWSpec.DeathKnightFrost, F.InitializeFrost),
                new SwitchArgument<WoWSpec>(WoWSpec.DeathKnightUnholy, U.InitializeUnholy));
        }

        #endregion
    }
}
