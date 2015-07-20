using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows.Ink;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class DarkKnightSetting : JsonSettings
    {
        public DarkKnightSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Dark-Knight.json")
        {
        }

        [Setting]
        [DefaultValue(70)]
        [Category("Dark Knight - GCD Attacks")]
        [DisplayName("Souleater HP%")]
        public int SouleaterHpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - GCD Attacks")]
        [DisplayName("Use Delirium")]
        public bool UseDelirium { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Off GCD  Buffs")]
        [DisplayName("Use Darkside")]
        public bool UseDarkside { get; set; }

        [Setting]
        [DefaultValue(90)]
        [Category("Dark Knight - Off GCD  Buffs")]
        [DisplayName("Darkside MP%")]
        public int DarksideMpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Off GCD Attack Buffs")]
        [DisplayName("Use Dark Arts")]
        public bool UseDarkArts { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Dark Knight - Off GCD Attack Buffs")]
        [DisplayName("Dark Arts > Power Slash MP%")]
        public int DarkArtsPowerSlashHpPercentage { get; set; }

        [Setting]
        [DefaultValue(70)]
        [Category("Dark Knight - Off GCD Attack Buffs")]
        [DisplayName("Dark Arts > Souleater MP%")]
        public int DarkArtsSouleaterHpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Attacks")]
        [DisplayName("Use Scourge")]
        public bool UseScourge { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Off GCD Attacks")]
        [DisplayName("Use Reprisal")]
        public bool UseReprisal { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Off GCD Attacks")]
        [DisplayName("Use Low Blow")]
        public bool UseLowBlow { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Off GCD Attacks")]
        [DisplayName("Use Salted Earth")]
        public bool UseSaltedEarth { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Off GCD Defense Buffs")]
        [DisplayName("Use Dark Dance")]
        public bool UseDarkDance { get; set; }

        [Setting]
        [DefaultValue(70)]
        [Category("Dark Knight - Off GCD Defense Buffs")]
        [DisplayName("Dark Dance HP%")]
        public int DarkDanceHpPercentage { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Dark Knight - Off GCD Defense Buffs")]
        [DisplayName("Use Dark Mind")]
        public bool UseDarkMind { get; set; }

        [Setting]
        [DefaultValue(70)]
        [Category("Dark Knight - Off GCD Defense Buffs")]
        [DisplayName("Dark Mind HP%")]
        public int DarkMindHpPercentage { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Dark Knight - Off GCD Defense Buffs")]
        [DisplayName("Use Shadowskin")]
        public bool UseShadowskin { get; set; }

        [Setting]
        [DefaultValue(60)]
        [Category("Dark Knight - Off GCD Defense Buffs")]
        [DisplayName("Shadowskin HP%")]
        public int ShadowskinHpPercentage { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Dark Knight - Off GCD Defense Buffs")]
        [DisplayName("Use Shadow Wall")]
        public bool UseShadowWall { get; set; }

        [Setting]
        [DefaultValue(30)]
        [Category("Dark Knight - Off GCD Defense Buffs")]
        [DisplayName("Shadow Wall HP%")]
        public int ShadowWallHpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Off GCD Mana Buff")]
        [DisplayName("Use Blood Weapon")]
        public bool UseBloodWeapon { get; set; }

        [Setting]
        [DefaultValue(90)]
        [Category("Dark Knight - Off GCD Mana Buff")]
        [DisplayName("Blood Weapon MP%")]
        public int BloodWeaponManaPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Off GCD Mana Buff")]
        [DisplayName("Use Blood Price")]
        public bool UseBloodPrice { get; set; }

        [Setting]
        [DefaultValue(70)]
        [Category("Dark Knight - Off GCD Mana Buff")]
        [DisplayName("Blood Price Mana%")]
        public int BloodPriceManaPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Off GCD Mana Buff")]
        [DisplayName("Use Carve and Spit")]
        public bool UseCarveAndSpit { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Dark Knight - Off GCD Mana Buff")]
        [DisplayName("Carve and Spit MP%")]
        public int CarveAndSpitManaPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dark Knight - Off GCD Mana Buff")]
        [DisplayName("Use Sole Survivor")]
        public bool UseSoleSurvivor { get; set; }

        [Setting]
        [DefaultValue(70)]
        [Category("Dark Knight - Off GCD Mana Buff")]
        [DisplayName("Sole Survivor HP%")]
        public int SoleSurvivorHpPercentage { get; set; }

        [Setting]
        [DefaultValue(70)]
        [Category("Dark Knight - Off GCD Mana Buff")]
        [DisplayName("Sole Survivor MP%")]
        public int SoleSurvivorManaPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Cross Class")]
        [DisplayName("Use Foresight")]
        public bool UseForesight { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Cross Class")]
        [DisplayName("Foresight HP%")]
        public int ForesightHpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Cross Class")]
        [DisplayName("Use Convalescence")]
        public bool UseConvalescence { get; set; }

        [Setting]
        [DefaultValue(40)]
        [Category("Cross Class")]
        [DisplayName("Convalescence HP%")]
        public int ConvalescenceHpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Cross Class")]
        [DisplayName("Use Bloodbath")]
        public bool UseBloodbath { get; set; }

        [Setting]
        [DefaultValue(70)]
        [Category("Cross Class")]
        [DisplayName("Bloodbath HP%")]
        public int BloodbathHpPercentage { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Cross Class")]
        [DisplayName("Use Mercy Strokeh")]
        public bool UseMercyStroke { get; set; }


    }

}