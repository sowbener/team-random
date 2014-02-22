using Styx;
using Styx.Common;
using Styx.Helpers;
using System.Windows.Forms;

namespace AntiAFK.GUI
{
    public class AntiAfkSettings : Settings
    {
        private static AntiAfkSettings _instance;

        public static string SettingsPath
        {
            get { return string.Format("{0}\\Settings\\AntiAFK\\AntiAfk_{1}", Utilities.AssemblyDirectory, StyxWoW.Me.Name); }
        }

        public static AntiAfkSettings Instance
        {
            get { return _instance ?? (_instance = new AntiAfkSettings()); }
        }

        public AntiAfkSettings()
            : base(SettingsPath + ".xml")
        {
        }

        [Setting, DefaultValue(Keys.Space)]
        public Keys AntiAfkKey { get; set; }

        [Setting, DefaultValue(5)]
        public int AntiAfkTimeValue { get; set; }

        [Setting, DefaultValue(20)]
        public int AntiAfkRandomValue { get; set; }

        [Setting, DefaultValue(true)]
        public bool AntiAfkPlugins { get; set; }

        [Setting, DefaultValue("")]
        public string LastStatCounted { get; set; }
    }
}
