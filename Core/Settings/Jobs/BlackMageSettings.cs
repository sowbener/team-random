using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class BlackMageSetting : JsonSettings
    {
        public BlackMageSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Black-Mage.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Black-Mage")]
        public bool Test{ get; set; }


    }

}