using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class ArcherSetting : JsonSettings
    {
        public ArcherSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Archer.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Archer")]
        public bool Test{ get; set; }


    }

}