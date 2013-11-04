using Styx.Helpers;
using System.ComponentModel;
using Waldo.Helpers;

namespace Waldo.Interfaces.Settings
{
    internal class WaSettingsG : Styx.Helpers.Settings
    {

        public WaSettingsG()
            : base(WaSettings.SettingsPath + "_General.xml")
        {
        }

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
        [DisplayName("Enable or Disable Potion Usage")]
        [Description("This Enables Potion During BloodLust or similar")]
        public bool CheckPotion { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(WaEnum.InterruptList.MoP)]
        [Category("General")]
        [DisplayName("Interrupt List")]
        [Description("Select list over interrupts (Expension Pack).")]
        public WaEnum.InterruptList InterruptList { get; set; }

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
        [Styx.Helpers.DefaultValue(65)]
        [Category("General")]
        [DisplayName("HealingRest %")]
        [Description("Select the use-on HP for Resting usage.")]
        public int HPMovementRest { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("General")]
        [DisplayName("Allow Usage Tracking")]
        [Description("This enabled allows private tracking on the amount of users of this routine - ALL INFO IS PRIVATE!!!")]
        public bool CheckAllowUsageTracking { get; set; }

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

        #region AWaanced
        [Styx.Helpers.DefaultValue(false)]
        [Category("AWaanced")]
        [DisplayName("Enable AWaanced Logging")]
        [Description("Enables aWaanced logging, loglevel in HB should be set to debug.")]
        public bool CheckAWaancedLogging { get; set; }

        [Styx.Helpers.DefaultValue(false)]
        [Category("AWaanced")]
        [DisplayName("Enable TreeSharp Timer")]
        [Description("Enables the timer to measure the amount of MS for a composite traverse.")]
        public bool CheckTreePerformance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(500)]
        [Category("AWaanced")]
        [DisplayName("Logging Throttle Time")]
        [Description("Time between aWaanced logs - Throttle.")]
        public int LoggingThrottleNum { get; set; }

        #endregion
    }
}
