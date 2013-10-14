using Styx;
using Styx.Common;
using Styx.Helpers;
using System.Windows.Forms;

namespace AntiAfk.GUI
{
    internal class AntiAfkSettings : Settings
    {
        private static AntiAfkSettings _instance;

        public AntiAfkSettings()
            : base(SettingsPath + ".xml")
        {
        }

        public static string SettingsPath
        {
            get { return string.Format("{0}\\Settings\\AntiAFK\\AntiAfk_{1}", Utilities.AssemblyDirectory, StyxWoW.Me.Name); }
        }

        public static AntiAfkSettings Instance
        {
            get { return _instance ?? (_instance = new AntiAfkSettings()); }
        }

        [Setting, DefaultValue(Keys.Space)]
        public Keys AntiAfkKey { get; set; }

        [Setting, DefaultValue(180000)]
        public int AntiAfkTime { get; set; }
    }
}
