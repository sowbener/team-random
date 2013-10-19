using FuryUnleashed.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace FuryUnleashed.Interfaces.Settings
{
    internal class SettingsG : Styx.Helpers.Settings
    {

        public SettingsG()
            : base(InternalSettings.SettingsPath + "_General.xml")
        {
        }

        // ========================================================================================

        #region General
        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("General")]
        [DisplayName("Class Racials %")]
        [Description("Select the use-on HP or MP for Racials (All MP or HP dependant racials).")]
        public int RacialNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("General")]
        [DisplayName("Enable Automatic Updates")]
        [Description("Enables the automatic updater in this CC - Updates from RELEASE.")]
        public bool CheckAutoUpdate { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.SvnUrl.Release)]
        [Category("General")]
        [DisplayName("Auto Update Version")]
        [Description("Select the preferred SVN for the auto-updater.")]
        public Enum.SvnUrl SvnUrl { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("General")]
        [DisplayName("Enable Hotkey ChatOutput")]
        [Description("Enables chat output for hotkeys.")]
        public bool CheckHotkeyChatOutput { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Enable PreCombat HKs")]
        [Description("Enabled allows you to use casting hotkeys outside of combat - COSTS PERFORMANCE!")]
        public bool CheckPreCombatHk { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("General")]
        [DisplayName("Enable Pre-Combat Buff")]
        [Description("This enables shouts pre-combat, also keeps the buff up.")]
        public bool CheckPreCombatBuff { get; set; }
        #endregion

        // ========================================================================================

        #region Rotational Settings
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.WoWVersion.Release)]
        [Category("Rotational Settings")]
        [DisplayName("Arms Rotation Version")]
        [Description("Select which rotations you prefer - Development or Release.")]
        public Enum.WoWVersion CrArmsRotVersion { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.WoWVersion.Release)]
        [Category("Rotational Settings")]
        [DisplayName("Fury Rotation Version")]
        [Description("Select which rotations you prefer - Development or Release.")]
        public Enum.WoWVersion CrFuryRotVersion { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.WoWVersion.Release)]
        [Category("Rotational Settings")]
        [DisplayName("Prot Rotation Version")]
        [Description("Select which rotations you prefer - Development or Release.")]
        public Enum.WoWVersion CrProtRotVersion { get; set; }
        #endregion

        // ========================================================================================

        #region Advanced
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Debug Logging")]
        [DisplayName("Enable Debug Logging")]
        [Description("Enable this to trigger the debug logging tree.")]
        public bool CheckDebugLogging { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Debug Logging")]
        [DisplayName("Enable Cache Logging")]
        [Description("Enables advanced logging for cached functions - Requires Debug Logging enabled.")]
        public bool CheckCacheLogging { get; set; }

		[Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Debug Logging")]
        [DisplayName("Enable Test Logging")]
        [Description("Enables advanced logging for test functions - Requires Debug Logging enabled.")]
        public bool CheckTestLogging { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Debug Logging")]
        [DisplayName("Enable Unit Logging")]
        [Description("Enables advanced logging for unit functions - Requires Debug Logging enabled.")]
        public bool CheckUnitLogging { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.LogCategory.None)]
        [Category("Debug Logging")]
        [DisplayName("Performance Logging")]
        [Description("Performance = on, None = off.")]
        public Enum.LogCategory PerformanceLogging { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Debug Timers")]
        [DisplayName("Enable TreeSharp Timer")]
        [Description("Enables the timer to measure the amount of MS for a composite traverse - Requires diagnostic loglevel.")]
        public bool CheckTreePerformance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(2500)]
        [Category("Debug Timers")]
        [DisplayName("Logging Throttle Time")]
        [Description("Time between advanced logs - Throttle  - Requires Debug Logging enabled.")]
        public int LoggingThrottleNum { get; set; }
        #endregion

        // ========================================================================================

        #region Movement
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Movement")]
        [DisplayName("Disable Click to Move")]
        [Description("This disables click to move.")]
        public bool CheckDisableClickToMove { get; set; }
        #endregion

        // ========================================================================================

        #region Non Public Settings
        [Setting]
        [Styx.Helpers.DefaultValue("")]
        [Browsable(false)]
        [DisplayName("StatCounter")]
        [Description("Last time we updated our statCounter")]
        public string LastStatCounted { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(0)]
        [Browsable(false)]
        [DisplayName("UpdaterRevision")]
        [Description("Current revision for Updater")]
        public int CurrentRevision { get; set; }
        #endregion

    }
}
