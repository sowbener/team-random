using Styx.Helpers;
using System.ComponentModel;
using Bubbleman.Helpers;

namespace Bubbleman.Interfaces.Settings
{
    internal class BMSettingsG : Styx.Helpers.Settings
    {

        public BMSettingsG()
            : base(BMSettings.SettingsPath + "_General.xml")
        {
        }

        #region Interrupt

        [Setting]
        [Styx.Helpers.DefaultValue(700)]
        [Category("General")]
        [DisplayName("Interrupt Start")]
        [Description("Select the interrupt start timer, DO NOT USE BELOW 700 MS.")]
        public int InterruptStart { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(1800)]
        [Category("General")]
        [DisplayName("Interrupt End")]
        [Description("Select the interrupt end timer.")]
        public int InterruptEnd { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(BMEnum.InterruptList.MoP)]
        [Category("General")]
        [DisplayName("Interrupt List")]
        [Description("Select list over interrupts (Expension Pack).")]
        public BMEnum.InterruptList InterruptList { get; set; }

        #endregion

        #region General
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Disable Click to Move")]
        [Description("This disables click to move.")]
        public bool CheckDisableClickToMove { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Enables Addon usage")]
        [Description("This Enables BubblemanRoutineToggler")]
        public bool EnableMacroOutput { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("General")]
        [DisplayName("Enables Raid Buffing")]
        [Description("This enables Raid Buffing")]
        public bool EnableRaidPartyBuffing { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(LogCategory.None)]
        [Category("Logging")]
        [DisplayName("Performance Logging")]
        [Description("Performance = on, None = off.")]
        public LogCategory PerformanceLogging { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("General")]
        [DisplayName("Allow Usage Tracking")]
        [Description("This enabled allows private tracking on the amount of users of this routine - ALL INFO IS PRIVATE!!!")]
        public bool CheckAllowUsageTracking { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Enable or Disable Potion Usage")]
        [Description("This Enables Potion During BloodLust or similar")]
        public bool CheckPotion { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Enable Wow ChatOutput")]
        [Description("Enables chat output for hotkeys.")]
        public bool EnableWoWChatOutput { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("General")]
        [DisplayName("Enable Pre-Combat Buff")]
        [Description("This enables shouts pre-combat, also keeps the buff up.")]
        public bool CheckPreCombatBuff { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Enable Specific AoE")]
        [Description("This enables AoE on Garalon and Twin Consorts.")]
        public bool CheckSpecialAoE { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Enable PreCombat HKs")]
        [Description("Enabled allows you to use casting hotkeys outside of combat - COSTS PERFORMANCE!")]
        public bool CheckPreCombatHk { get; set; }

        #endregion

        #region ABManced
        [Styx.Helpers.DefaultValue(false)]
        [Category("ABManced")]
        [DisplayName("Enable ABManced Logging")]
        [Description("Enables aBManced logging, loglevel in HB should be set to debug.")]
        public bool CheckABMancedLogging { get; set; }

        [Styx.Helpers.DefaultValue(false)]
        [Category("ABManced")]
        [DisplayName("Enable TreeSharp Timer")]
        [Description("Enables the timer to measure the amount of MS for a composite traverse.")]
        public bool CheckTreePerformance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(500)]
        [Category("ABManced")]
        [DisplayName("Logging Throttle Time")]
        [Description("Time between aBManced logs - Throttle.")]
        public int LoggingThrottleNum { get; set; }

        #endregion
    }
}
