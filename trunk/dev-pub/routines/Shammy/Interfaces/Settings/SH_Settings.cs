using Styx;
using Styx.Common;
using System.ComponentModel;

namespace Shammy.Interfaces.Settings
{
    internal class SmSettings : Styx.Helpers.Settings
    {
        private static SmSettings _instance;

        public SmSettings()
            : base(SettingsPath + ".config")
        {
        }

        public static string SettingsPath
        {
            get
            {
                return string.Format("{0}\\Settings\\Shammy\\Sm_Settings_{1}-Rev{2}", Utilities.AssemblyDirectory, StyxWoW.Me.Name, SmMain.Revision);
            }
        }

        public static SmSettings Instance
        {
            get { return _instance ?? (_instance = new SmSettings()); }
        }

        #region Spec Loading Wrappers
        private SmSettingsU _SmSettingsU;
        private SmSettingsF _SmSettingsF;
        private SmSettingsG _SmSettingsG;

        [Browsable(false)]
        public SmSettingsU Elemental
        {
            get { return _SmSettingsU ?? (_SmSettingsU = new SmSettingsU()); }
        }

        [Browsable(false)]
        public SmSettingsF Enhancement
        {
            get { return _SmSettingsF ?? (_SmSettingsF = new SmSettingsF()); }
        }

        [Browsable(false)]
        public SmSettingsG General
        {
            get { return _SmSettingsG ?? (_SmSettingsG = new SmSettingsG()); }
        }
        #endregion
    }
}
