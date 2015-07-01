using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class BardSetting : JsonSettings
    {
        public BardSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Bard.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Bard")]
        public bool Test{ get; set; }


    }

}