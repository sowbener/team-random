using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class LancerSetting : JsonSettings
    {
        public LancerSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Lancer.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Lancer")]
        public bool Test{ get; set; }


    }

}