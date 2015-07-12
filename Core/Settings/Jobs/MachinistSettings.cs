using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class MachinistSetting : JsonSettings
    {
        public MachinistSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Machinist.json")
        {
        }

      //  private const int MinHpBuff = 1000;
       // private const int MinHpDeBuff = 1000;
      //  private const int UseInvigorateTP = 550;

        [Setting]
        [DefaultValue(true)]
        [Category("Machinist")]
        [DisplayName("Enable Auto-Buffs")]
        public bool EnableAutoBuffs{ get; set; }

        [Setting]
        [DefaultValue(1000)]
        [Category("Machinist")]
        [DisplayName("Minimum Hp Buff")]
        public int MinHpBuff { get; set; }

        [Setting]
        [DefaultValue(1000)]
        [Category("Machinist")]
        [DisplayName("Minimum Hp Debuff")]
        public int MinHpDeBuff { get; set; }

        [Setting]
        [DefaultValue(550)]
        [Category("Machinist")]
        [DisplayName("Use Invigorate TP At")]
        public int UseInvigorateTP { get; set; }


    }

}