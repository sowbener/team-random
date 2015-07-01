using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class MonkSetting : JsonSettings
    {
        public MonkSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Monk.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Monk")]
        public bool Test{ get; set; }


    }

}