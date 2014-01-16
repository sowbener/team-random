using Styx;
using Styx.Common;
using System.ComponentModel;

namespace YourBuddy.Interfaces.Settings
{
    internal class InternalSettings : Styx.Helpers.Settings
    {
        private static InternalSettings _instance;

        public InternalSettings()
            : base(SettingsPath + ".config")
        {
        }

        public static string SettingsPath
        {
            get
            {
                return string.Format("{0}\\Settings\\YourBuddy\\Yb_Settings_{1}-Rev{2}", Utilities.AssemblyDirectory, StyxWoW.Me.Name, Root.Revision);
            }
        }

        public static InternalSettings Instance
        {
            get { return _instance ?? (_instance = new InternalSettings()); }
        }

        #region Spec Loading Wrappers
        private SettingsG _settingsG;

        //Monk Settings
        private YbSettingsBMM _settingsBMM;
        private YbSettingsWWM _settingsWWM;

        //Rogue Settings
        private YbSettingsSR _settingsSR;
        private YbSettingsCR _settingsCR;
        private YbSettingsAR _settingsAR;


        // General
        [Browsable(false)]
        public SettingsG General
        {
            get { return _settingsG ?? (_settingsG = new SettingsG()); }
        }

        // Monk
        [Browsable(false)]
        public YbSettingsBMM Brewmaster
        {
            get { return _settingsBMM ?? (_settingsBMM = new YbSettingsBMM()); }
        }

        [Browsable(false)]
        public YbSettingsWWM Windwalker
        {
            get { return _settingsWWM ?? (_settingsWWM = new YbSettingsWWM()); }
        }


        //Rogue

            public YbSettingsSR Subtlety
        {
            get { return _settingsSR ?? (_settingsSR = new YbSettingsSR()); }
        }
            public YbSettingsCR Combat
        {
            get { return _settingsCR ?? (_settingsCR = new YbSettingsCR()); }
        }
            public YbSettingsAR Assassination
        {
            get { return _settingsAR ?? (_settingsAR = new YbSettingsAR()); }
        }

        #endregion
    }
}
