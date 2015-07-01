using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class DarkKnightSetting : JsonSettings
    {
        public DarkKnightSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Dark-Knight.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Dark-Knight")]
        public bool Test{ get; set; }


    }

}