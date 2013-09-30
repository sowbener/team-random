﻿using Styx.Helpers;
using System.ComponentModel;

namespace Bullseye.Interfaces.Settings
{
    internal class BsSettingsG : Styx.Helpers.Settings
    {

        public BsSettingsG()
            : base(BsSettings.SettingsPath + "_General.xml")
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
        [Styx.Helpers.DefaultValue(true)]
        [Category("General")]
        [DisplayName("Enable or Disable Trap Usage")]
        [Description("This enables Trap Usage")]
        public bool EnableTraps { get; set; }


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

        #region ABsanced
        [Styx.Helpers.DefaultValue(false)]
        [Category("ABsanced")]
        [DisplayName("Enable ABsanced Logging")]
        [Description("Enables aBsanced logging, loglevel in HB should be set to debug.")]
        public bool CheckABsancedLogging { get; set; }

        [Styx.Helpers.DefaultValue(false)]
        [Category("ABsanced")]
        [DisplayName("Enable TreeSharp Timer")]
        [Description("Enables the timer to measure the amount of MS for a composite traverse.")]
        public bool CheckTreePerformance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(500)]
        [Category("ABsanced")]
        [DisplayName("Logging Throttle Time")]
        [Description("Time between aBsanced logs - Throttle.")]
        public int LoggingThrottleNum { get; set; }

        #endregion
    }
}
