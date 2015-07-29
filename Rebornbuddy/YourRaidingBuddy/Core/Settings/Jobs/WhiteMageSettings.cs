using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class WhiteMageSetting : JsonSettings
    {
        public WhiteMageSetting()
            : base(InternalSettings.RoutineSettingsPath + "_White-Mage.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("White-Mage")]
        public bool Test{ get; set; }


    }

}