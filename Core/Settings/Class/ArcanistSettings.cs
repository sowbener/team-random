using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class ArcanistSetting : JsonSettings
    {
        public ArcanistSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Arcanist.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Arcanist")]
        public bool Test{ get; set; }


    }

}