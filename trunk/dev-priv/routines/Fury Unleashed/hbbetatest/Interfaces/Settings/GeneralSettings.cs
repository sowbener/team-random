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

        #region Global Interrupt Settings
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.Interrupts.RandomTimed)]
        [Category("Global Interrupt Settings")]
        [DisplayName("Interrupt Mode")]
        [Description("Select the interrupt mode - Constant or after a random casttime - RANDOM IS RECOMMENDED!")]
        public Enum.Interrupts InterruptMode { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(1000)]
        [Category("Global Interrupt Settings")]
        [DisplayName("Constant Interrupt Value")]
        [Description("Select a value in milliseconds, when units casttime gets below this numer it will be interrupted.")]
        public int InterruptNum { get; set; }
        #endregion

        // ========================================================================================

        #region General
        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("General")]
        [DisplayName("Class Racials %")]
        [Description("Select the use-on HP or MP for Racials (All MP or HP dependant racials).")]
        public int RacialNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General")]
        [DisplayName("Enable Automatic Updates")]
        [Description("Enables the automatic updater in this CC - Updates from RELEASE.")]
        public bool CheckAutoUpdate { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.SvnUrl.Development)]
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
        [Styx.Helpers.DefaultValue(Enum.RotationVersion.Release)]
        [Category("Rotational Settings")]
        [DisplayName("Arms Rotation Version")]
        [Description("Select which rotations you prefer - Development or Release.")]
        public Enum.RotationVersion CrArmsRotVersion { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.RotationVersion.Release)]
        [Category("Rotational Settings")]
        [DisplayName("Fury Rotation Version")]
        [Description("Select which rotations you prefer - Development or Release.")]
        public Enum.RotationVersion CrFuryRotVersion { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.RotationVersion.Release)]
        [Category("Rotational Settings")]
        [DisplayName("Prot Rotation Version")]
        [Description("Select which rotations you prefer - Development or Release.")]
        public Enum.RotationVersion CrProtRotVersion { get; set; }
        #endregion

        // ========================================================================================

        #region Advanced
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Debug Logging")]
        [DisplayName("CooldownTracker Logging")]
        [Description("Enables the logging for the Cooldown Tracker - Requires diagnostic loglevel.")]
        public bool CheckCooldownTrackerLogging { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Debug Logging")]
        [DisplayName("Enable TreeSharp Timer")]
        [Description("Enables the timer to measure the amount of MS for a composite traverse - Requires diagnostic loglevel.")]
        public bool CheckTreePerformance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.LogCategory.None)]
        [Category("Debug Logging")]
        [DisplayName("Performance Logging")]
        [Description("Performance = on, None = off.")]
        public Enum.LogCategory PerformanceLogging { get; set; }
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
