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

        [Setting]
        [DefaultValue(true)]
        [Category("Machinist")]
        [DisplayName("Enable Auto-Buffs")]
        public bool EnableAutoBuffs { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Machinist")]
        [DisplayName("Enable Potions")]
        public bool EnablePotions { get; set; }

        [Setting]
        [DefaultValue("X-Potion of Dexterity")]
        [Category("Machinist")]
        [DisplayName("Potion to be used")]
        public string PotName { get; set; }

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

        [Setting]
        [DefaultValue(true)]
        [Category("Machinist")]
        [DisplayName("Enable AutoTurret Summon")]
        public bool EnableAutoSummon { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Machinist")]
        [DisplayName("Enable Use of Head Graze")]
        public bool EnableStuns { get; set; }

        [Setting]
        [DefaultValue(5)]
        [Category("Machinist")]
        [DisplayName("Auto-AoE Number of Mobs")]
        public int AoEMonster { get; set; }

    }

}