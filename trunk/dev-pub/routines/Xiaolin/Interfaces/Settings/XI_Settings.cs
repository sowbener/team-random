using Styx;
using Styx.Common;
using System.ComponentModel;

namespace Xiaolin.Interfaces.Settings
{
    internal class XISettings : Styx.Helpers.Settings
    {
        private static XISettings _instance;

        public XISettings()
            : base(SettingsPath + ".config")
        {
        }

        public static string SettingsPath
        {
            get
            {
                return string.Format("{0}\\Settings\\Xiaolin\\XI_Settings_{1}-Rev{2}", Utilities.AssemblyDirectory, StyxWoW.Me.Name, XIMain.Revision);
            }
        }

        public static XISettings Instance
        {
            get { return _instance ?? (_instance = new XISettings()); }
        }

        #region Spec Loading Wrappers
        private XISettingsWW _XISettingsWW;
        private XISettingsBM _XISettingsBM;
        private XISettingsG _XISettingsG;

        [Browsable(false)]
        public XISettingsWW Windwalker
        {
            get { return _XISettingsWW ?? (_XISettingsWW = new XISettingsWW()); }
        }

        [Browsable(false)]
        public XISettingsBM Brewmaster
        {
            get { return _XISettingsBM ?? (_XISettingsBM = new XISettingsBM()); }
        }

        [Browsable(false)]
        public XISettingsG General
        {
            get { return _XISettingsG ?? (_XISettingsG = new XISettingsG()); }
        }
        #endregion
    }
}
