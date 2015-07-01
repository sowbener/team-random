using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class PugilistSetting : JsonSettings
    {
        public PugilistSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Pugilist.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Pugilist")]
        public bool Test{ get; set; }


    }

}