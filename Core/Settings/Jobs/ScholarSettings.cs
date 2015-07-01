using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class ScholarSetting : JsonSettings
    {
        public ScholarSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Scholar.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Scholar")]
        public bool Test{ get; set; }


    }

}