using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class MarauderSetting : JsonSettings
    {
        public MarauderSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Marauder.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Marauder")]
        public bool Test{ get; set; }


    }

}