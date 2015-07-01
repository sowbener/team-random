using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class WarriorSetting : JsonSettings
    {
        public WarriorSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Warrior.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Warrior")]
        public bool Test{ get; set; }


    }

}