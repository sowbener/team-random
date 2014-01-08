using Styx.Helpers;
using System.ComponentModel;
using DeathVader.Helpers;



namespace DeathVader.Interfaces.Settings
{
    internal class DvSettingsG : Styx.Helpers.Settings
    {

        public DvSettingsG()
            : base(DvSettings.SettingsPath + "_General.xml")
        {
        }

        [Setting]
        [Styx.Helpers.DefaultValue(0)]
        [Category("General")]
        [DisplayName("Logs if you press key to pause")]
        [Description("This enables logging for pause key.")]
        public int DetectKeyPress { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Enable to Use Modifier Keys")]
        [Description("This Enables Modifier Keys.")]
        public bool UseModifierKey { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Enable Pause so you'll be able to press cooldowns.")]
        [Description("This enables the new pause (same as TuanHa's)")]
        public bool AutoDetectManualCast { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(LogCategory.None)]
        [Category("Logging")]
        [DisplayName("Performance Logging")]
        [Description("Performance = on, None = off.")]
        public LogCategory PerformanceLogging { get; set; }

        #region General
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Disable Click to Move")]
        [Description("This disables click to move.")]
        public bool CheckDisableClickToMove { get; set; }

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
        [Styx.Helpers.DefaultValue(DvEnum.InterruptList.MoP)]
        [Category("General")]
        [DisplayName("Interrupt List")]
        [Description("Select list over interrupts (Expension Pack).")]
        public DvEnum.InterruptList InterruptList { get; set; }

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

        #region Advanced
        [Styx.Helpers.DefaultValue(false)]
        [Category("Advanced")]
        [DisplayName("Enable Advanced Logging")]
        [Description("Enables advanced logging, loglevel in HB should be set to debug.")]
        public bool CheckAdvancedLogging { get; set; }

        [Styx.Helpers.DefaultValue(false)]
        [Category("Advanced")]
        [DisplayName("Enable TreeSharp Timer")]
        [Description("Enables the timer to measure the amount of MS for a composite traverse.")]
        public bool CheckTreePerformance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(500)]
        [Category("Advanced")]
        [DisplayName("Logging Throttle Time")]
        [Description("Time between advanced logs - Throttle.")]
        public int LoggingThrottleNum { get; set; }

        #endregion
    }
}
