using Styx;
using Styx.Common;
using System.ComponentModel;

namespace Waldo.Interfaces.Settings
{
    internal class WaSettings : Styx.Helpers.Settings
    {
        private static WaSettings _instance;

        public WaSettings()
            : base(SettingsPath + ".config")
        {
        }

        public static string SettingsPath
        {
            get
            {
                return string.Format("{0}\\Settings\\Waldo\\Wa_Settings_{1}-Rev{2}", Utilities.AssemblyDirectory, StyxWoW.Me.Name, WaMain.Revision);
            }
        }

        public static WaSettings Instance
        {
            get { return _instance ?? (_instance = new WaSettings()); }
        }

        #region Spec Loading Wrappers
        private WaSettingsC _WaSettingsC;
        private WaSettingsA _WaSettingsA;
        private WaSettingsG _WaSettingsG;
        private WaSettingsS _WaSettingsS;

        [Browsable(false)]
        public WaSettingsC Combat
        {
            get { return _WaSettingsC ?? (_WaSettingsC = new WaSettingsC()); }
        }

        [Browsable(false)]
        public WaSettingsA Assassination
        {
            get { return _WaSettingsA ?? (_WaSettingsA = new WaSettingsA()); }
        }

        [Browsable(false)]
        public WaSettingsS Subtlety
        {
            get { return _WaSettingsS ?? (_WaSettingsS = new WaSettingsS()); }
        }

        [Browsable(false)]
        public WaSettingsG General
        {
            get { return _WaSettingsG ?? (_WaSettingsG = new WaSettingsG()); }
        }
        #endregion
    }
}
