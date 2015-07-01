using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class MachinistSetting : JsonSettings
    {
        public MachinistSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Machinist.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Machinist")]
        public bool Test{ get; set; }


    }

}