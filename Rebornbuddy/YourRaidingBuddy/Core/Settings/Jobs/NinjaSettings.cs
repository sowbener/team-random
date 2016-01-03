using ff14bot.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class NinjaSetting : JsonSettings
    {
        public NinjaSetting()
            : base(InternalSettings.RoutineSettingsPath + "_Ninja.json")
        {
        }

        [Setting]
        [DefaultValue(false)]
        [Category("Ninja")]
        [DisplayName("Use Shuriken instead of Raiton")]
        public bool ShurikenAlways { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Ninja")]
        [DisplayName("Use Kassatsu")]
        public bool Kassatsu { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Ninja")]
        [DisplayName("When to recast Huton")]
        public int HutonClip { get; set; }

        [Setting]
        [DefaultValue(10000)]
        [Category("Ninja")]
        [DisplayName("Suiton HP Value")]
        public int SuitonHP { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rogue")]
        [DisplayName("Use Dancing Edge")]
        public bool DancingEdge { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Rogue")]
        public int DancingEdgeClip { get; set; }

        [Setting]
        [DefaultValue(5000)]
        [Category("Rogue")]
        public int ShadowFangClip { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Rogue")]
        public int MutilationClip { get; set; }

        [Setting]
        [DefaultValue(80)]
        [Category("Rogue")]
        [DisplayName("DO NOT TOUCH")]
        public int TrickIsBehindAdjustment { get; set; }

        [Setting]
        [DefaultValue(15000)]
        [Category("Rogue")]
        [DisplayName("When to prioritize Armor Crush")]
        public int EmergencyHutonClip { get; set; }

        [Setting]
        [DefaultValue(10000)]
        [Category("Rogue")]
        [DisplayName("Dot HP Limit")]
        public int MobHP { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rogue")]
        [DisplayName("Always Wasp Posion")]
        public bool Wasp { get; set; }

        [Setting]
        [DefaultValue(10000)]
        [Category("Rogue")]
        [DisplayName("Buff HP Limit")]
        public int BuffHP { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("AoE")]
        [DisplayName("AoE Toggle")]
        public bool AoEToggle { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("AoE")]
        [DisplayName("Death Blossom Toggle")]
        public bool DeathBlossomToggle { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("AoE")]
        [DisplayName("Kassatsu AoE Toggle")]
        public bool KassatsuAoE { get; set; }



    }
}