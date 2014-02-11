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

        //Deathknight Settings
        private YbSettingsBD _settingsBD;
        private YbSettingsFD _settingsFD;
        private YbSettingsUD _settingsUD;

        
        // Hunter Settings
        private YbSettingsSV  _settingsSV;
        private YbSettingsMM  _settingsMM;
        private YbSettingsBM  _settingsBM;

        //Shaman Settings
        private YbSettingsES  _settingsES;
        private YbSettingsENS _settingsENS;

        //Paladin Settings
        private YbSettingsPR _settingsPR;
        private YbSettingsRE _settingsRE;

        //Druid Settings
        private YbSettingsDF _settingsDF;



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
        [Browsable(false)]
            public YbSettingsSR Subtlety
        {
            get { return _settingsSR ?? (_settingsSR = new YbSettingsSR()); }
        }
        [Browsable(false)]
            public YbSettingsCR Combat
        {
            get { return _settingsCR ?? (_settingsCR = new YbSettingsCR()); }
        }
        [Browsable(false)]
            public YbSettingsAR Assassination
        {
            get { return _settingsAR ?? (_settingsAR = new YbSettingsAR()); }
        }

        //DeathKnight
        [Browsable(false)]
        public YbSettingsBD Blood
        {
            get { return _settingsBD ?? (_settingsBD = new YbSettingsBD()); }
        }

        [Browsable(false)]
        public YbSettingsFD Frost
        {
            get { return _settingsFD ?? (_settingsFD = new YbSettingsFD()); }
        }

        [Browsable(false)]
        public YbSettingsUD Unholy
        {
            get { return _settingsUD ?? (_settingsUD = new YbSettingsUD()); }
        }

        
// Hunter Settings
    [Browsable(false)]
        public YbSettingsSV Survival
        {
            get { return _settingsSV ?? (_settingsSV = new YbSettingsSV()); }
        }
    [Browsable(false)]
        public YbSettingsMM Marksmanship
        {
            get { return _settingsMM ?? (_settingsMM = new YbSettingsMM()); }
        }
    [Browsable(false)]
        public YbSettingsBM Beastmastery
        {
            get { return _settingsBM ?? (_settingsBM = new YbSettingsBM()); }
        }

//Shaman Settings
        [Browsable(false)]
        public YbSettingsES Elemental
        {
            get { return _settingsES ?? (_settingsES = new YbSettingsES()); }
        }

        [Browsable(false)]
        public YbSettingsENS Enhancement
        {
            get { return _settingsENS ?? (_settingsENS = new YbSettingsENS()); }
        }

//Paladin Settings
          [Browsable(false)]
        public YbSettingsPR Protection
        {
            get { return _settingsPR ?? (_settingsPR = new YbSettingsPR()); }
        }
          [Browsable(false)]
          public YbSettingsRE Retribution
        {
            get { return _settingsRE ?? (_settingsRE = new YbSettingsRE()); }
        }

//Druid Settings
        [Browsable(false)]
        public YbSettingsDF Feral
        {
            get { return _settingsDF ?? (_settingsDF = new YbSettingsDF()); }
        }



        #endregion
    }
}
