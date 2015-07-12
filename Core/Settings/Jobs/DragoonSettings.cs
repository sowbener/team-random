using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class DragoonSetting : JsonSettings
    {
        public DragoonSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Dragoon.json")
        {
        }

        [Setting]
        [DefaultValue(true)]
        [Category("Lancer")]
        [DisplayName("Leg Sweep Use")]
        public bool UseLegSweep { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Lancer")]
        [DisplayName("Heavy Thrust Clip")]
        public int ClipHeavyThrust { get; set; }

        [Setting]
        [DefaultValue(440)]
        [Category("Lancer")]
        [DisplayName("Invigorate At")]
        public int InvigorateTP { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Lancer")]
        [DisplayName("Phlebotomize Clip")]
        public int ClipPhlebotomize { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Lancer")]
        [DisplayName("Blood For Blood Use")]
        public bool UseBloodForBlood { get; set; }

        [Setting]
        [DefaultValue(5000)]
        [Category("Lancer")]
        [DisplayName("Disembowel Clip")]
        public int ClipDisembowel { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Jump Use")]
        public bool UseJump { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Power Surge Use")]
        public bool UsePowerSurge { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Spineshatter Dive Use")]
        public bool UseSpineshatterDive { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Dragonfire Dive Use")]
        public bool UseDragonfireDive { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Dragoon")]
        [DisplayName("Battle Litany Use")]
        public bool UseBttleLitany { get; set; }

        [Setting]
        [DefaultValue(13000)]
        [Category("Dragoon")]
        [DisplayName("Geirskogul CLip")]
        public int ClipGeirskogul { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Marauder")]
        [DisplayName("Mercy Stroke Use")]
        public bool UseMercyStroke { get; set; }

    }

}