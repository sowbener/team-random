using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class SummonerSetting : JsonSettings
    {
        public SummonerSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Summoner.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Summoner")]
        public bool Test{ get; set; }


    }

}