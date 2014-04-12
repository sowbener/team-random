using YourRaidingBuddy.Core;
using YourRaidingBuddy.Core.Helpers;
using YourRaidingBuddy.Core.Managers;
using YourRaidingBuddy.Core.Utilities;
using YourRaidingBuddy.Interfaces.GUI;
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
using G = YourRaidingBuddy.Rotations.Global;
using WWM = YourRaidingBuddy.Rotations.Monk.Windwalker;
using BMM = YourRaidingBuddy.Rotations.Monk.Brewmaster;
using SR = YourRaidingBuddy.Rotations.Rogue.Subtlety;
using AR = YourRaidingBuddy.Rotations.Rogue.Assassination;
using CR = YourRaidingBuddy.Rotations.Rogue.Combat;
using BD = YourRaidingBuddy.Rotations.Deathknight.Blood;
using FD = YourRaidingBuddy.Rotations.Deathknight.Frost;
using UD = YourRaidingBuddy.Rotations.Deathknight.Unholy;
using PP = YourRaidingBuddy.Rotations.Paladin.Protection;
using DF = YourRaidingBuddy.Rotations.Druid.Feral;
using DB = YourRaidingBuddy.Rotations.Druid.Boomkin;
using RP = YourRaidingBuddy.Rotations.Paladin.Retribution;
using SV = YourRaidingBuddy.Rotations.Hunter.Survival;
using BMH = YourRaidingBuddy.Rotations.Hunter.BeastMastery;
using MM = YourRaidingBuddy.Rotations.Hunter.Marksmanship;
using ES = YourRaidingBuddy.Rotations.Shaman.Enhancement;
using EES = YourRaidingBuddy.Rotations.Shaman.Elemental;

using System.Windows.Forms;
using BotEvents = Styx.CommonBot.BotEvents;
using Lua = YourRaidingBuddy.Core.Helpers.LuaClass;

// HB API Documentation: http://docs.honorbuddy.com/

namespace YourRaidingBuddy
{
    public class Root : CombatRoutine
    {
        [UsedImplicitly]
        public static Root Instance { get; private set; }
        public static LocalPlayer Me { get { return StyxWoW.Me; } }
        public static readonly Version Revision = new Version(1, 0, 1);
        public static readonly string YbName = "YourRaidingBuddy"  + Revision;
        public static readonly string WoWVersion = "5.4.2";

        internal static double _initap = 0;
        internal static double _NewAP = 0;
        internal static double _NoModifierAP = 0;
        public static double dps;

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

              //  if (GlobalSettings.Instance.UseFrameLock)
             //   {
             //       MessageBox.Show("Hard Framelock is Enabled - I suggest disable it for Soft Lock in Tyrael!");
            //    }

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

            if (TalentManager.Pulse())
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
            if (G.AllowedClassListDie.Contains(StyxWoW.Me.Class) && StyxWoW.Me.CurrentTarget != null)
                DpsMeter.Update();

            if (Me.Specialization == WoWSpec.MonkBrewmaster && !Me.Combat && !Me.HasAura(120267))
                _initap = StyxWoW.Me.AttackPower;
            if (Me.Specialization == WoWSpec.MonkBrewmaster)
            _NewAP = Lua.Vengeance(120267);

            if (StyxWoW.Me.Specialization == WoWSpec.DruidFeral) SnapShotStats();

            Spell.PulseDoubleCastEntries();


         //   if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightUnholy && !DoTTracker.Initialized) DoTTracker.Initialize();

            if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightBlood)
                DeathStrikeTracker.Pulse();
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
            Logger.CombatLogOr("Special thanks to: Stormchasing, Handnavi, Wulf, Mirabis, Nomnomnom, Chinajade, Weischbier & Millz!\r\n");
            Logger.CombatLogOr("Your specialization is " + Me.Specialization.ToString().CamelToSpaced() + " and your race is " + Me.Race + ".");
           // if (!GlobalSettings.Instance.UseFrameLock) { Logger.CombatLogFb("Framelock is disabled - I suggest enabling it for optimal DPS/TPS!"); }
           // else { Logger.CombatLogOr("Framelock is enabled at {0} ticks per second.\r\n", GlobalSettings.Instance.TicksPerSecond); }
            Logger.CombatLogWh("-------------------------------------------\r\n");


            /* Update TalentManager */
            try { TalentManager.Update(); }
            catch (Exception e) { StopBot(e.ToString()); }


            /* Set Characters GUID */
            MyGuid = Me.Guid;

            /* Gather required information */
            Logger.StatCounter();
            Logger.LogTimer(500);

            /* Attack Power For Brewmaster Monks */
            _initap = StyxWoW.Me.AttackPower;
            _NewAP = StyxWoW.Me.AttackPower;
        //    Styx.WoWInternals.Lua.Events.AttachEvent("UPDATE_MOUSEOVER_UNIT", G.HandleMouseOverTarget);
            DpsMeter.Initialize();

            if (StyxWoW.Me.Specialization == WoWSpec.DruidFeral) grabMainHandDPS();
            if (StyxWoW.Me.Specialization == WoWSpec.DruidFeral) YourRaidingBuddy.Rotations.Druid.CommonDruid.Initialize();

            CombatLogHandler.Initialize();
            Spell.GcdInitialize();
            

            /* Lua SecondaryStats */
      //     if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightUnholy) Lua.PopulateSecondryStats();
           if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightBlood) Lua.PopulateSecondryStats();

            /* Blood DK DeathStrike Tracker */
            if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightBlood)
                DeathStrikeTracker.Initialize();

            if (StyxWoW.Me.Specialization == WoWSpec.RogueCombat || StyxWoW.Me.Specialization == WoWSpec.MonkWindwalker)
                Styx.WoWInternals.Lua.Events.AttachEvent("MODIFIER_STATE_CHANGED", G.HandleModifierStateChanged);

            /* Unholy DoTTracker */
        //    if (StyxWoW.Me.Specialization == WoWSpec.DeathKnightUnholy && !DoTTracker.Initialized) DoTTracker.Initialize();


            /* Start Combat */
            PreCombatSelector();
            CombatSelector();
            G.GetBinding();

            Logger.CombatLogOr("Routine initialized with " + Me.Specialization.ToString().CamelToSpaced() + " as rotation. \r\n");
        }

        internal Composite PreCombatSelector()
        {
            return new Switch<WoWSpec>(ret => Me.Specialization,
                new SwitchArgument<WoWSpec>(WoWSpec.MonkWindwalker, G.InitializePreBuffMonk),
                new SwitchArgument<WoWSpec>(WoWSpec.DeathKnightUnholy, G.InitializePreBuffDK),
                new SwitchArgument<WoWSpec>(WoWSpec.MonkBrewmaster, G.InitializePreBuffMonk),
                new SwitchArgument<WoWSpec>(WoWSpec.RogueSubtlety, G.InitializePreBuffRogue),
                new SwitchArgument<WoWSpec>(WoWSpec.RogueAssassination, G.InitializePreBuffRogue),
                new SwitchArgument<WoWSpec>(WoWSpec.ShamanElemental, G.InitializePreBuffShaman),
                new SwitchArgument<WoWSpec>(WoWSpec.ShamanEnhancement, G.InitializePreBuffShaman),
                new SwitchArgument<WoWSpec>(WoWSpec.PaladinProtection, G.InitializePreBuffPaladin),
                new SwitchArgument<WoWSpec>(WoWSpec.DruidBalance, G.InitializePreBuffBoomkin),
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
                new SwitchArgument<WoWSpec>(WoWSpec.DruidFeral, DF.InitializeFeral),
                new SwitchArgument<WoWSpec>(WoWSpec.DruidBalance, DB.InitializeBoomkin),
                new SwitchArgument<WoWSpec>(WoWSpec.DeathKnightBlood, BD.InitializeBlood),
                new SwitchArgument<WoWSpec>(WoWSpec.DeathKnightFrost, FD.InitializeFrost),
                new SwitchArgument<WoWSpec>(WoWSpec.DeathKnightUnholy, UD.InitializeUnholy),
                new SwitchArgument<WoWSpec>(WoWSpec.ShamanElemental, EES.InitializeElemental),
                new SwitchArgument<WoWSpec>(WoWSpec.ShamanEnhancement, ES.InitializeEnhancement),
                new SwitchArgument<WoWSpec>(WoWSpec.HunterBeastMastery, BMH.InitializeBeastMastery),
                new SwitchArgument<WoWSpec>(WoWSpec.HunterMarksmanship, MM.InitializeMarksmanship),
                new SwitchArgument<WoWSpec>(WoWSpec.HunterSurvival, SV.InitializeSurvival),
                new SwitchArgument<WoWSpec>(WoWSpec.PaladinRetribution, RP.InitializeRetribution),
                new SwitchArgument<WoWSpec>(WoWSpec.PaladinProtection, PP.InitializeProtection)
                
                );
        }

        public static double AP { get; set; }
        public static double Mastery { get; set; }
        public static double Multiplier { get; set; }

        private void SnapShotStats()
        {
            var DoCSID = Me.HasAura(145152);
            AP = StyxWoW.Me.AttackPower;

            using (StyxWoW.Memory.AcquireFrame())
            {
                Multiplier = Styx.WoWInternals.Lua.GetReturnVal<double>("return UnitDamage(\"player\");", 6);
                Mastery = (1 + (Styx.WoWInternals.Lua.GetReturnVal<double>("return GetMasteryEffect()", 0) / 100));
            }

            if (DoCSID)
                Multiplier = Multiplier * 1.3;
        }

        private void grabMainHandDPS()
        {
            var swingMin = Styx.WoWInternals.Lua.GetReturnVal<float>("return UnitDamage(\"player\");", 0);
            var swingMax = Styx.WoWInternals.Lua.GetReturnVal<float>("return UnitDamage(\"player\");", 1);
            var swingAvg = (swingMin + swingMax) / 2;
            dps = swingAvg / 2;
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
