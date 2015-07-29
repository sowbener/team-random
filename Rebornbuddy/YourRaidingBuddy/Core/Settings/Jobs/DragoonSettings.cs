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
        [Category("General")]
        [DisplayName("Smart Animation Lock")]
        [Description("Don't use jumps when you may need to get into positionals.")]
        public bool SmartAnimationLock { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Lancer")]
        [DisplayName("Keen Flurry At")]
        [Description("Use KeenFlurry when youur health is lower than this percent.")]
        public int LancerKeenFlurryHP { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Lancer")]
        [DisplayName("Leg Sweep Use")]
        public bool LancerUseLegSweep { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Lancer")]
        [DisplayName("Heavy Thrust Clip")]
        public int LancerClipHeavyThrust { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Lancer")]
        [DisplayName("Piercing Talon Use")]
        public bool LancerUsePiercingTalon { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Lancer")]
        [DisplayName("Life Surge Use")]
        public bool LancerUseLifeSurge { get; set; }

        [Setting]
        [DefaultValue(440)]
        [Category("Lancer")]
        [DisplayName("Invigorate At")]
        public int LancerInvigorateTP { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Lancer")]
        [DisplayName("Phlebotomize Clip")]
        public int LancerClipPhlebotomize { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Lancer")]
        [DisplayName("Blood For Blood Use")]
        [Description("Use Blood For Blood when enemy has more than this percent.")]
        public int LancerBloodForBloodHP { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Lancer")]
        [DisplayName("Disembowel Clip")]
        public int LancerClipDisembowel { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Lancer")]
        [DisplayName("Chaos Thrust Clip")]
        public int LancerClipChaosThrust { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Jump Use")]
        public bool DragoonUseJump { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Jump With Blood")]
        [Description("Jump only when you have Blood of the Dragon.")]
        public bool DragoonUseJumpBlood { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Power Surge Use")]
        public bool DragoonUsePowerSurge { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Spineshatter Dive Use")]
        public bool DragoonUseSpineshatterDive { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Spineshatter Dive With Blood")]
        [Description("Dive only when you have Blood of the Dragon.")]
        public bool DragoonUseSpineshatterDiveBlood { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Dragonfire Dive Use")]
        public bool DragoonUseDragonfireDive { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Dragoon")]
        [DisplayName("Battle Litany Use")]
        public bool DragoonUseBttleLitany { get; set; }

        [Setting]
        [DefaultValue(13000)]
        [Category("Dragoon")]
        [DisplayName("Geirskogul Combo Timing")]
        [Description("Use Geirskogul before Fang and Claw or Wheeling Thrust if Blood of the Dragon has more time than this.")]
        public int DragoonTimeGeirskogulCombo { get; set; }

        [Setting]
        [DefaultValue(31000)]
        [Category("Dragoon")]
        [DisplayName("Geirskogul Timing")]
        [Description("Use Geirskogul if Blood of the Dragon has more time than this.")]

        public int DragoonTimeGeirskogul { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Dragoon")]
        [DisplayName("Blood Of The Dragon")]
        public bool DragoonUseBloodOfTheDragon { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Puglist")]
        [DisplayName("Featherfoot At")]
        [Description("Use Featherfoot when your health is lower than this percent.")]
        public int PuglistFeatherfootHP { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Puglist")]
        [DisplayName("Second Wind At")]
        [Description("Use Second Wind when your health is lower than this percent.")]
        public int PuglistSecondWindHP { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Puglist")]
        [DisplayName("Haymaker Use")]
        public bool PuglistUseHaymaker { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Puglist")]
        [DisplayName("Internal Release Use")]
        [Description("Use Internal Release when enemy has more than this percent.")]
        public int PuglistInternalReleaseHP { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Marauder")]
        [DisplayName("Foresight At")]
        [Description("Use Foresight when your health is lower than this percent.")]
        public int MarauderForesightHP { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Marauder")]
        [DisplayName("Fracture Clip")]
        public int ClipFracture { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Marauder")]
        [DisplayName("Bloodbath At")]
        [Description("Use Bloodbath when your health is lower than this percent.")]
        public int MarauderBloodbathHP { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Marauder")]
        [DisplayName("Mercy Stroke Use")]
        public bool MarauderUseMercyStroke { get; set; }
    }
}