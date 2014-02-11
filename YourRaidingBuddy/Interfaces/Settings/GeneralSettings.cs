using YourBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourBuddy.Interfaces.Settings
{
    internal class SettingsG : Styx.Helpers.Settings
    {

        public SettingsG()
            : base(InternalSettings.SettingsPath + "_General.xml")
        {
        }


        #region Global Interrupt Settings
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.Interrupts.RandomTimed)]
        [Category("Global Interrupt Settings")]
        [DisplayName("Interrupt Mode")]
        [Description("Select the interrupt mode - Constant or after a random casttime - RandomTimed IS RECOMMENDED!")]
        public Enum.Interrupts InterruptMode { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(1000)]
        [Category("Global Interrupt Settings")]
        [DisplayName("Constant Interrupt Value")]
        [Description("Select a value in milliseconds, when units casttime gets below this numer it will be interrupted.")]
        public int InterruptNum { get; set; }
        #endregion

        #region Updater
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Updater")]
        [DisplayName("Enable Automatic Updates")]
        [Description("Enables the automatic updater in this CC.")]
        public bool CheckAutoUpdate { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.SvnUrl.Release)]
        [Category("Updater")]
        [DisplayName("Auto Update Version")]
        [Description("Select the preferred SVN for the auto-updater.")]
        public Enum.SvnUrl SvnUrl { get; set; }
        #endregion

        #region Interrupt

        [Setting]
        [Styx.Helpers.DefaultValue(700)]
        [Category("General")]
        [DisplayName("Interrupt Start")]
        [Description("Select the interrupt start timer, DO NOT USE BELOW 700 MS.")]
        public int InterruptStart { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(500000)]
        [Category("General")]
        [DisplayName("Minimum Ability Start")]
        [Description("Select the Minimum Ability Start For Some Classes.")]
        public uint MinHPAbility { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(1800)]
        [Category("General")]
        [DisplayName("Interrupt End")]
        [Description("Select the interrupt end timer.")]
        public int InterruptEnd { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.InterruptList.MoP)]
        [Category("General")]
        [DisplayName("Interrupt List")]
        [Description("Select list over interrupts (Expension Pack).")]
        public Enum.InterruptList InterruptList { get; set; }
        #endregion

        #region General
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General Settings")]
        [DisplayName("Manual Cast Pause.")]
        [Description("When enabled, the routine allows you to properly manual-cast abilities.")]
        public bool AutoDetectManualCast { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General Settings")]
        [DisplayName("Enable Potion Usage on Bloodlust.")]
        [Description("When enabled, the routine uses Potion on Bloodlust..")]
        public bool CheckPotionUsage { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General Rogue Settings")]
        [DisplayName("Enable Tricks of the Trade Auto")]
        [Description("When enabled, the routine uses Tricks of the Trade on FocusTarget")]
        public bool UseTricksAutoRogue { get; set; }

        

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("General")]
        [DisplayName("Enables Raid Buffing")]
        [Description("This enables Raid Buffing")]
        public bool EnableRaidPartyBuffing { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("General Settings")]
        [DisplayName("Enable Hotkey ChatOutput")]
        [Description("Enables chat output for hotkeys.")]
        public bool CheckHotkeyChatOutput { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("General Settings")]
        [DisplayName("Enable PreCombat HKs")]
        [Description("Enabled allows you to use casting hotkeys outside of combat - COSTS PERFORMANCE!")]
        public bool CheckPreCombatHk { get; set; }
        #endregion

        #region Combat Settings
        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Combat Settings")]
        [DisplayName("Class Racials %")]
        [Description("Select the use-on HP or MP for Racials (All MP or HP dependant racials).")]
        public int RacialNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Combat Settings")]
        [DisplayName("Enable Pre-Combat Buff")]
        [Description("This enables shouts pre-combat, also keeps the buff up.")]
        public bool CheckPreCombatBuff { get; set; }

      //  [Styx.Helpers.DefaultValue(false)]
      //  [Category("Combat Settings")]
     //   [DisplayName("Enable Heroic Leap Usage")]
     //   [Description("This enables the routine to cast Heroic Leap when applicable on your target - EXPERIMENTAL!")]
    //    public bool CheckHeroicLeap { get; set; }

    //    [Setting]
     //   [Styx.Helpers.DefaultValue(Enum.VigilanceTrigger.OnTank)]
    //    [Category("Combat Settings")]
   //     [DisplayName("Vigilance Usage")]
   //     [Description("Select the usage of Vigilance.")]
   //     public Enum.VigilanceTrigger Vigilance { get; set; }

     //   [Setting]
    //    [Styx.Helpers.DefaultValue(15)]
    //    [Category("Combat Settings")]
   //     [DisplayName("Vigilance HP %")]
   //     [Description("Select the use-on HP for Vigilance usage.")]
  //      public int VigilanceNum { get; set; }
        #endregion

        #region Rotational Settings
    //    [Setting]
   //     [Styx.Helpers.DefaultValue(Enum.WindwalkerRotationVersion.Release)]
   //     [Category("Rotational Settings")]
   //     [DisplayName("Windwalker Rotation Version")]
  //      [Description("Select which rotations you prefer - Development, PvP or Release.")]
  //      public Enum.WindwalkerRotationVersion CrWindwalkerRotVersion { get; set; }

  //      [Setting]
  //      [Styx.Helpers.DefaultValue(Enum.BrewmasterRotationVersion.Release)]
 //       [Category("Rotational Settings")]
 //       [DisplayName("Brewmaster Rotation Version")]
 //       [Description("Select which rotations you prefer - Development or Release.")]
//        public Enum.BrewmasterRotationVersion CrBrewmasterRotVersion { get; set; }

        #endregion

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

        #region Movement
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Movement")]
        [DisplayName("Disable Click to Move")]
        [Description("This disables click to move.")]
        public bool CheckDisableClickToMove { get; set; }
        #endregion

        #region KeyPush
        [Setting]
        [Styx.Helpers.DefaultValue(0)]
        [Category("General")]
        [DisplayName("Pause Value MS")]
        [Description("Set the amount of MS you want the rotation to pause for.")]
        public int DetectKeyPress { get; set; }

#endregion

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
