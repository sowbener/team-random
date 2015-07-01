using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class PaladinSetting : JsonSettings
    {
        public PaladinSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Paladin.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Paladin")]
        public bool Test{ get; set; }


    }

}