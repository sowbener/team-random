using Styx;
using Styx.Common;
using System.ComponentModel;

namespace FuryUnleashed.Interfaces.Settings
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
                return string.Format("{0}\\Settings\\Fury Unleashed\\FU_Settings_{1}-Rev{2}", Utilities.AssemblyDirectory, StyxWoW.Me.Name, Root.Revision);
            }
        }

        public static InternalSettings Instance
        {
            get { return _instance ?? (_instance = new InternalSettings()); }
        }

        #region Spec Loading Wrappers
        private SettingsA _settingsA;
        private SettingsF _settingsF;
        private SettingsG _settingsG;
        private SettingsP _settingsP;

        [Browsable(false)]
        public SettingsA Arms
        {
            get { return _settingsA ?? (_settingsA = new SettingsA()); }
        }

        [Browsable(false)]
        public SettingsF Fury
        {
            get { return _settingsF ?? (_settingsF = new SettingsF()); }
        }

        [Browsable(false)]
        public SettingsG General
        {
            get { return _settingsG ?? (_settingsG = new SettingsG()); }
        }

        [Browsable(false)]
        public SettingsP Protection
        {
            get { return _settingsP ?? (_settingsP = new SettingsP()); }
        }
        #endregion
    }
}
