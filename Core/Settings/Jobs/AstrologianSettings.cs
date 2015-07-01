using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class AstrologianSetting : JsonSettings
    {
        public AstrologianSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Astrologian.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Astrologian")]
        public bool Test{ get; set; }


    }

}