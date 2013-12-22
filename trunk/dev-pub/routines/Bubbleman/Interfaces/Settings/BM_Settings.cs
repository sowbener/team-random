using Styx;
using Styx.Common;
using System.ComponentModel;

namespace Bubbleman.Interfaces.Settings
{
    internal class BMSettings : Styx.Helpers.Settings
    {
        private static BMSettings _instance;

        public BMSettings()
            : base(SettingsPath + ".config")
        {
        }

        public static string SettingsPath
        {
            get
            {
                return string.Format("{0}\\Settings\\Bubbleman\\BM_Settings_{1}-Rev{2}", Utilities.AssemblyDirectory, StyxWoW.Me.Name, BMMain.Revision);
            }
        }

        public static BMSettings Instance
        {
            get { return _instance ?? (_instance = new BMSettings()); }
        }

        #region Spec Loading Wrappers
        private BMSettingsRE _BMSettingsRE;
        private BMSettingsPR _BMSettingsPR;
        private BMSettingsG _BMSettingsG;

        [Browsable(false)]
        public BMSettingsRE Retribution
        {
            get { return _BMSettingsRE ?? (_BMSettingsRE = new BMSettingsRE()); }
        }

        [Browsable(false)]
        public BMSettingsPR Protection
        {
            get { return _BMSettingsPR ?? (_BMSettingsPR = new BMSettingsPR()); }
        }

        [Browsable(false)]
        public BMSettingsG General
        {
            get { return _BMSettingsG ?? (_BMSettingsG = new BMSettingsG()); }
        }
        #endregion
    }
}
