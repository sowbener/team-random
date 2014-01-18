using YourBuddy.Core;
using YourBuddy.Core.Helpers;
using YourBuddy.Core.Managers;
using YourBuddy.Core.Utilities;
using YourBuddy.Interfaces.GUI;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using G = YourBuddy.Rotations.Global;
using WWM = YourBuddy.Rotations.Monk.Windwalker;
using BMM = YourBuddy.Rotations.Monk.Brewmaster;
using SR = YourBuddy.Rotations.Rogue.Subtlety;
using AR = YourBuddy.Rotations.Rogue.Assassination;
using CR = YourBuddy.Rotations.Rogue.Combat;
using BD = YourBuddy.Rotations.Deathknight.Blood;
using FD = YourBuddy.Rotations.Deathknight.Frost;
using UD = YourBuddy.Rotations.Deathknight.Unholy;
using System.Windows.Forms;
using BotEvents = Styx.CommonBot.BotEvents;
using Lua = YourBuddy.Core.Helpers.LuaClass;

// HB API Documentation: http://docs.honorbuddy.com/

namespace YourBuddy
{
    public class Root : CombatRoutine
    {
        [UsedImplicitly]
        public static Root Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }
        public static readonly Version Revision = new Version(1, 0, 0);
        public static readonly string YbName = "YourRaidingBuddy" + Revision;
        public static readonly string WoWVersion = "5.4.2";

        internal static double _initap = 0;
        internal static double _NewAP = 0;
        internal static double _NoModifierAP = 0;

        public override string Name { get { return YbName; } }
        public override bool WantButton { get { return true; } }
        
        public override Composite CombatBehavior { get { return _combatBehavior ?? (_combatBehavior = CombatSelector()); } }
        public override Composite PreCombatBuffBehavior { get { return _preCombatBehavior ?? (_preCombatBehavior = PreCombatSelector()); } }

        private Composite _combatBehavior, _preCombatBehavior;


        internal static ulong MyGuid = 0;

        #region Publics
        public override WoWClass Class { get { return G.AllowedClassList.Contains(StyxWoW.Me.Class) ? StyxWoW.Me.Class : WoWClass.None; } }


        public Root()
        {
            Instance = this;
            BotEvents.OnBotStopped += Core.Helpers.BotEvents.OnBotStopped;
            BotEvents.OnBotStarted += Core.Helpers.BotEvents.OnBotStarted;
            BotEvents.OnBotChanged += Core.Helpers.BotEvents.OnBotChanged;
        }

        public override void Initialize()
        {
            try
            {
                TreeHooks.Instance.ClearAll();
                Updater.CheckForUpdate();

                if (!GlobalSettings.Instance.UseFrameLock)
                {
                    MessageBox.Show("Framelock is disabled - I suggest enabling it for optimal DPS/TPS!");
                }

                Unleash();
            }
            catch (Exception exception)
            {
                Logger.DiagLogPu("Yb: Exception thrown at Initialize - {0}", new object[] { exception.ToString() });
            }
        }


        public override void Pulse()
        {
			if (!StyxWoW.IsInWorld || Me == null || !Me.IsValid || Me.IsDead)
            {
                return;
            }

            // intense if does work, so return if true
            if (TalentManager.Pulse())
                return;

            if (Me.Specialization == WoWSpec.MonkBrewmaster && !Me.Combat && !Me.HasAura(120267))
                _initap = StyxWoW.Me.AttackPower;
            _NewAP = Lua.Vengeance(120267);
            _NoModifierAP = StyxWoW.Me.AttackPower;

            Spell.PulseDoubleCastEntries();

            if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightUnholy && !DoTTracker.Initialized) DoTTracker.Initialize();

            if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightBlood)
                DeathStrikeTracker.Pulse();

            /* Update TalentManager */
            try { TalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }
        }

        public override void OnButtonPress()
        {
            new Interface().ShowDialog();
        }

        public override void ShutDown()
        {
            StopBot("Yb: Shutting down HonorBuddy and all attached addons.");
        }
        #endregion

        #region Internals
        internal void Unleash()
        {
            Logger.CombatLogWh("------------------------------------------");
            Logger.CombatLogFb(Name + " by nomnomnom & alxaw.");
            Logger.CombatLogOr("Internal Revision: " + Revision + ".");
            Logger.CombatLogOr("Supported World of Warcraft version: " + WoWVersion + ".");
            Logger.CombatLogOr("Support will be handled via the HB Forums.");
            Logger.CombatLogOr("Thanks list is available in the topic!");
            Logger.CombatLogOr("Special thanks to: Stormchasing, Wulf, Mirabis, Nomnomnom, ChinaJade, Weischbier & Millz!\r\n");
            Logger.CombatLogOr("Your specialization is " + Me.Specialization.ToString().CamelToSpaced() + " and your race is " + Me.Race + ".");
            if (!GlobalSettings.Instance.UseFrameLock) { Logger.CombatLogFb("Framelock is disabled - I suggest enabling it for optimal DPS/TPS!"); }
            else { Logger.CombatLogOr("Framelock is enabled at {0} ticks per second.\r\n", GlobalSettings.Instance.TicksPerSecond); }
            Logger.CombatLogWh("-------------------------------------------\r\n");


            /* Update TalentManager */
            TalentManager.Update();

            /* Set Characters GUID */
            MyGuid = Me.Guid;

            /* Initialize Various Functions */
            HotKeyManager.InitializeBindings();

            /* Gather required information */
            Logger.StatCounter();
            Logger.LogTimer(500);

            /* Attack Power For Brewmaster Monks */
            _initap = StyxWoW.Me.AttackPower;
            _NewAP = StyxWoW.Me.AttackPower;

            /* Blood DK DeathStrike Tracker */
            if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightBlood)
                DeathStrikeTracker.Initialize();

            /* Unholy DoTTracker */
            if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightUnholy && !DoTTracker.Initialized) DoTTracker.Initialize();


            /* Start Combat */
            PreCombatSelector();
            CombatSelector();

            Logger.CombatLogOr("Routine initialized with " + Me.Specialization.ToString().CamelToSpaced() + " as rotation. \r\n");
        }

        internal Composite PreCombatSelector()
        {
            return new Switch<WoWSpec>(ret => Me.Specialization,
                new SwitchArgument<WoWSpec>(WoWSpec.MonkWindwalker, G.InitializePreBuffMonk),
                new SwitchArgument<WoWSpec>(WoWSpec.MonkBrewmaster, G.InitializePreBuffMonk),
                new SwitchArgument<WoWSpec>(WoWSpec.RogueSubtlety, G.InitializePreBuffRogue),
                new SwitchArgument<WoWSpec>(WoWSpec.RogueAssassination, G.InitializePreBuffRogue),
                new SwitchArgument<WoWSpec>(WoWSpec.RogueCombat, G.InitializePreBuffRogue));
        }

        internal Composite CombatSelector()
        {
            return new Switch<WoWSpec>(ret => Me.Specialization,
                new SwitchArgument<WoWSpec>(WoWSpec.MonkWindwalker, WWM.InitializeWindwalkerCombat),
                new SwitchArgument<WoWSpec>(WoWSpec.MonkBrewmaster, BMM.InitializeBrewmasterCombat),
                new SwitchArgument<WoWSpec>(WoWSpec.RogueSubtlety, SR.InitializeSub),
                new SwitchArgument<WoWSpec>(WoWSpec.RogueAssassination, AR.InitializeAss),
                new SwitchArgument<WoWSpec>(WoWSpec.RogueCombat, CR.InitializeCom),
                new SwitchArgument<WoWSpec>(WoWSpec.DeathKnightBlood, BD.InitializeBlood),
                new SwitchArgument<WoWSpec>(WoWSpec.DeathKnightFrost, FD.InitializeFrost),
                new SwitchArgument<WoWSpec>(WoWSpec.DeathKnightUnholy, UD.InitializeUnholy)
                
                );
        }

        internal static void StopBot(string reason)
        {
            Logger.CombatLogWh(reason);
            CombatLogHandler.Shutdown();
            TreeRoot.Stop();
        }
        #endregion

        #region LockSelector - FrameLock - AcquireFrame
        [UsedImplicitly]
        private class LockSelector : PrioritySelector
        {
            public LockSelector(params Composite[] children)
                : base(children)
            {
            }
            public override RunStatus Tick(object context)
            {
                using (StyxWoW.Memory.AcquireFrame())
                {
                    return base.Tick(context);
                }
            }
        }
        #endregion
    }
}
