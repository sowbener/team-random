using Styx;
using Styx.Common;
using System.ComponentModel;

namespace Bullseye.Interfaces.Settings
{
    internal class BsSettings : Styx.Helpers.Settings
    {
        private static BsSettings _instance;

        public BsSettings()
            : base(SettingsPath + ".config")
        {
        }

        public static string SettingsPath
        {
            get
            {
                return string.Format("{0}\\Settings\\Bullseye\\Bs_Settings_{1}-Rev{2}", Utilities.AssemblyDirectory, StyxWoW.Me.Name, BsMain.Revision);
            }
        }

        public static BsSettings Instance
        {
            get { return _instance ?? (_instance = new BsSettings()); }
        }

        #region Spec Loading Wrappers
        private BsSettingsMM _BsSettingsMM;
        private BsSettingsBM _BsSettingsBM;
        private BsSettingsSV _BsSettingsSV;
        private BsSettingsG _BsSettingsG;

        [Browsable(false)]
        public BsSettingsMM Marksmanship
        {
            get { return _BsSettingsMM ?? (_BsSettingsMM = new BsSettingsMM()); }
        }

        [Browsable(false)]
        public BsSettingsBM Beastmastery
        {
            get { return _BsSettingsBM ?? (_BsSettingsBM = new BsSettingsBM()); }
        }

        [Browsable(false)]
        public BsSettingsSV Survival
        {
            get { return _BsSettingsSV ?? (_BsSettingsSV = new BsSettingsSV()); }
        }

        [Browsable(false)]
        public BsSettingsG General
        {
            get { return _BsSettingsG ?? (_BsSettingsG = new BsSettingsG()); }
        }
        #endregion
    }
}
