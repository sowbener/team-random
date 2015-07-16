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
        public bool ShurikenAlways { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Ninja")]
        [DisplayName("Use Kassatsu")]
        public bool Kassatsu { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Ninja")]
        public int HutonClip { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rogue")]
        public bool DancingEdge { get; set; }

        [Setting]
        [DefaultValue(3000)]
        [Category("Rogue")]
        public int DancingEdgeClip { get; set; }

        [Setting]
        [DefaultValue(3000)]
        [Category("Rogue")]
        public int ShadowFangClip { get; set; }

        [Setting]
        [DefaultValue(3000)]
        [Category("Rogue")]
        public int MutilationClip { get; set; }

        [Setting]
        [DefaultValue(80)]
        [Category("Rogue")]
        public int TrickIsBehindAdjustment { get; set; }

        [Setting]
        [DefaultValue(15000)]
        [Category("Rogue")]
        public int EmergencyHutonClip { get; set; }



    }
}