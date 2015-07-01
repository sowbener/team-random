using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class GladiatorSetting : JsonSettings
    {
        public GladiatorSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Gladiator.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Gladiator")]
        public bool Test{ get; set; }


    }

}