using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class ThaumaturgeSetting : JsonSettings
    {
        public ThaumaturgeSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Thaumaturge.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Thaumaturge")]
        public bool Test{ get; set; }


    }

}