using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class DragoonSetting : JsonSettings
    {
        public DragoonSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Dragoon.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Dragoon")]
        public bool Test{ get; set; }


    }

}