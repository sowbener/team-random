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


        // General
        [Browsable(false)]
        public SettingsG General
        {
            get { return _settingsG ?? (_settingsG = new SettingsG()); }
        }

        // PvE
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


        #endregion
    }
}
