using Styx;
using Styx.Common;
using System.ComponentModel;

namespace DeathVader.Interfaces.Settings
{
    internal class DvSettings : Styx.Helpers.Settings
    {
        private static DvSettings _instance;

        public DvSettings()
            : base(SettingsPath + ".config")
        {
        }

        public static string SettingsPath
        {
            get
            {
                return string.Format("{0}\\Settings\\Death Vader\\Dv_Settings_{1}-Rev{2}", Utilities.AssemblyDirectory, StyxWoW.Me.Name, DvMain.Revision);
            }
        }

        public static DvSettings Instance
        {
            get { return _instance ?? (_instance = new DvSettings()); }
        }

        #region Spec Loading Wrappers
        private DvSettingsU _DvSettingsU;
        private DvSettingsB _DvSettingsB;
        private DvSettingsF _DvSettingsF;
        private DvSettingsG _DvSettingsG;

        [Browsable(false)]
        public DvSettingsU Unholy
        {
            get { return _DvSettingsU ?? (_DvSettingsU = new DvSettingsU()); }
        }

        public DvSettingsB Blood
        {
            get { return _DvSettingsB ?? (_DvSettingsB = new DvSettingsB()); }
        }

        [Browsable(false)]
        public DvSettingsF Frost
        {
            get { return _DvSettingsF ?? (_DvSettingsF = new DvSettingsF()); }
        }

        [Browsable(false)]
        public DvSettingsG General
        {
            get { return _DvSettingsG ?? (_DvSettingsG = new DvSettingsG()); }
        }
        #endregion
    }
}
