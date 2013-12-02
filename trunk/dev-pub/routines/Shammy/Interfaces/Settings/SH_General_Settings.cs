using Styx.Helpers;
using System.ComponentModel;
using Shammy.Helpers;

namespace Shammy.Interfaces.Settings
{
    internal class SmSettingsG : Styx.Helpers.Settings
    {

        public SmSettingsG()
            : base(SmSettings.SettingsPath + "_General.xml")
        {
        }


        #region interrupt

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
        [Styx.Helpers.DefaultValue(SmEnum.InterruptList.MoP)]
        [Category("General")]
        [DisplayName("Interrupt List")]
        [Description("Select list over interrupts (Expension Pack).")]
        public SmEnum.InterruptList InterruptList { get; set; }

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

        #region ASmanced
        [Styx.Helpers.DefaultValue(false)]
        [Category("ASmanced")]
        [DisplayName("Enable ASmanced Logging")]
        [Description("Enables aSmanced logging, loglevel in HB should be set to debug.")]
        public bool CheckASmancedLogging { get; set; }

        [Styx.Helpers.DefaultValue(false)]
        [Category("ASmanced")]
        [DisplayName("Enable TreeSharp Timer")]
        [Description("Enables the timer to measure the amount of MS for a composite traverse.")]
        public bool CheckTreePerformance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(500)]
        [Category("ASmanced")]
        [DisplayName("Logging Throttle Time")]
        [Description("Time between aSmanced logs - Throttle.")]
        public int LoggingThrottleNum { get; set; }

        #endregion
    }
}
