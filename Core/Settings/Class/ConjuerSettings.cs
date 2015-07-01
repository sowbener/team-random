using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class ConjurerSetting : JsonSettings
    {
        public ConjurerSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Conjurer.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Conjurer")]
        public bool Test{ get; set; }


    }

}