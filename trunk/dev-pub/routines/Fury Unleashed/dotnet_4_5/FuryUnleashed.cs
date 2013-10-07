// TODO: Bleeds!
using FuryUnleashed.Core;
using FuryUnleashed.Interfaces.GUI;
using FuryUnleashed.Shared.Helpers;
using FuryUnleashed.Shared.Managers;
using JetBrains.Annotations;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using A = FuryUnleashed.Routines.ArmsCombat;
using F = FuryUnleashed.Routines.FuryCombat;
using P = FuryUnleashed.Routines.ProtCombat;

namespace FuryUnleashed
{
    public class Root : CombatRoutine
    {
        [UsedImplicitly]
        public static Root Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }
        public static readonly Version Revision = new Version(1, 5, 1);
        public static readonly string FuName = "Fury Unleashed Premium - IR " + Revision;
        public static readonly double WoWVersion = 5.4;

        public override string Name { get { return FuName; } }
        public override bool WantButton { get { return true; } }
        public override WoWClass Class { get { return WoWClass.Warrior; } }

        public override Composite CombatBehavior { get { return _combatBehavior ?? (_combatBehavior = CombatSelector()); } }
        public override Composite PreCombatBuffBehavior { get { return _preCombatBehavior ?? (_preCombatBehavior = PreBuffSelector()); } }

        private Composite _combatBehavior, _preCombatBehavior;

        #region Publics
        public Root()
        {
            Instance = this;
            Styx.CommonBot.BotEvents.OnBotStopped += Shared.Helpers.BotEvents.OnBotStopped;
            Styx.CommonBot.BotEvents.OnBotStarted += Shared.Helpers.BotEvents.OnBotStarted;
            Styx.CommonBot.BotEvents.OnBotChanged += Shared.Helpers.BotEvents.OnBotChanged;
        }

        public override void Initialize()
        {
            try
            {
                TreeHooks.Instance.ClearAll();
                Updater.CheckForUpdate();

                Unleash();
            }
            catch (Exception exception)
            {
                Logger.AdvancedLogP("FU: Exception thrown at Initialize - {0}", new object[] { exception.ToString() });
            }
        }

        public override void Pulse()
        {
			if (!StyxWoW.IsInWorld || Me == null || !Me.IsValid || Me.IsDead)
            {
                return;
            }

            if (TalentManager.Pulse())
			{
                // ReSharper disable once RedundantJumpStatement
				return;
			}
        }

        public override void OnButtonPress()
        {
            new Interface().ShowDialog();
        }

        public override void ShutDown()
        {
            StopBot("FU: Shutting down HonorBuddy and all attached addons.");
        }
        #endregion

        #region Internals
        internal void Unleash()
        {
            Logger.InitLogW("------------------------------------------");
            Logger.InitLogF(Name + " by nomnomnom & alxaw.");
            Logger.InitLogO("Internal Revision: " + Revision + ".");
            Logger.InitLogO("Supported World of Warcraft version: " + WoWVersion + ".");
            Logger.InitLogO("Support will be handled via the HB Forums.");
            Logger.InitLogO("Thanks list is available in the topic!");
            Logger.InitLogO("\r\n");
            Logger.InitLogO("Your specialization is " + Me.Specialization.ToString().CamelToSpaced() + " and your race is " + Me.Race + ".");
            Logger.InitLogW("-------------------------------------------\r\n");

            /* Update TalentManager */
            try { TalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }

            /* Gather required information */
            Logger.StatCounter();
            Logger.LogTimer(500);

            /* Start Combat */
            Spell.InitGcdSpell();
            PreBuffSelector();
            CombatSelector();

            Logger.InitLogO("Routine initialized with " + Me.Specialization.ToString().CamelToSpaced() + " as rotation. \r\n");
        }

        internal Composite PreBuffSelector()
        {
            return new Switch<WoWSpec>(ret => Me.Specialization,
                new SwitchArgument<WoWSpec>(WoWSpec.WarriorArms, A.InitializeArmsPreCombat),
                new SwitchArgument<WoWSpec>(WoWSpec.WarriorFury, F.InitializeFuryPreCombat),
                new SwitchArgument<WoWSpec>(WoWSpec.WarriorProtection, P.InitializeProtPreCombat));
        }

        internal Composite CombatSelector()
        {
            return new Switch<WoWSpec>(ret => Me.Specialization,
                new SwitchArgument<WoWSpec>(WoWSpec.WarriorArms, A.InitializeArmsCombat),
                new SwitchArgument<WoWSpec>(WoWSpec.WarriorFury, F.InitializeFuryCombat),
                new SwitchArgument<WoWSpec>(WoWSpec.WarriorProtection, P.InitializeProtCombat));
        }

        internal static void StopBot(string reason)
        {
            Logger.InitLogW(reason);
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
