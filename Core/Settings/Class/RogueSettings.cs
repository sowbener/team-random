using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class RogueSetting : JsonSettings
    {
        public RogueSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Rogue.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Rogue")]
        public bool Test{ get; set; }


    }

}