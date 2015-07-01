﻿using System.ComponentModel;
using System.Configuration;
using System.IO;
using ff14bot.Helpers;
using Newtonsoft.Json;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Settings
{
    public class SettingsG : JsonSettings
    {
        public SettingsG()
            : base(InternalSettings.RoutineSettingsPath + "_General.json")
        {
        }

        [Setting]
        [DefaultValue(0)]
        [Category("Overlay")]
        [DisplayName("Overlay - X")]
        public int X { get; set; }

        [Setting]
        [DefaultValue(0)]
        [Category("Overlay")]
        [DisplayName("Overlay - Y")]
        public int Y { get; set; }

        [Setting]
        [DefaultValue(100)]
        [Category("Latency MS")]
        [DisplayName("Latency Ping")]
        public int PingMs { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Updater")]
        [DisplayName("Enable Auto-Updater")]
        public bool AutoUpdate { get; set; }

        [Setting]
        [DefaultValue("Potion of Dexterity")]
        [Category("Items")]
        public string PotionName { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Items")]
        public bool PotionForce { get; set; }

        [Setting, DefaultValue(System.Windows.Input.Key.Z)]
        [Category("Secondary Rotation")]
        public System.Windows.Input.Key HotKeyChoice { get; set; }

        [Setting, DefaultValue(ModifierKey.Shift)]
        [Category("Secondary Rotation")]
        public ModifierKey ModKeyChoice { get; set; }

        [Setting]
        [DefaultValue(null)]
        [Category("Healer")]
        public string TankName { get; set; }

        [Setting]
        [DefaultValue(80f)]
        [Category("Healer")]
        public float TankPrioritizeHealthPercent { get; set; }

        [Browsable(false)]
        public uint TankId { get; set; }

        [Setting]
        [DefaultValue(85f)]
        [Category("Healer")]
        public float HealthPercent { get; set; }

        [Setting]
        [DefaultValue(50)]
        [Category("Healer")]
        public int CriticalHealthPercent { get; set; }

        [Setting]
        [DefaultValue(2)]
        [Category("Healer")]
        public int CriticalHealthCount { get; set; }

        [Setting]
        [DefaultValue(8)]
        [Category("Healer")]
        public int WorldHealScanning { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Healer")]
        public bool WorldHealPartyOnly { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Information")]
        public bool ShowSpellIds { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Information")]
        public bool WriteSpellQueue { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Information")]
        public bool Debug { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Control")]
        public bool Combat { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Control")]
        public bool Targeting { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Control")]
        public bool Movement { get; set; }
        
        [Setting]
        [DefaultValue(false)]
        [Category("Control")]
        public bool Facing { get; set; }

        [Setting]
        [Category("Control")]
        [DefaultValue(CooldownUse.Always)]
        public CooldownUse Cooldowns { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Movement")]
        public bool DontMoveIfMovinToYou { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Movement")]
        public bool Dodge { get; set; }
            
        [Setting]
        [DefaultValue(true)]
        [Category("Movement")]
        public bool StopInRange { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Movement")]
        public bool AttemptCastWhileMoving { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Movement")]
        public bool MeshFreeMovement { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Movement")]
        public bool MeshOnStillTargets { get; set; }
        
        [Setting]
        [Category("Control")]
        [DefaultValue(false)]
        public bool Aoe { get; set; }

        [Setting]
        [DefaultValue(3f)]
        [Category("General")]
        public float AoeCount { get; set; }

        [Setting]
        [DefaultValue(40f)]
        [Category("General")]
        public float RestHealth { get; set; }

        [Setting]
        [DefaultValue(40f)]
        [Category("General")]
        public float RestEnergy { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        public bool CombatChocobo { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        public bool CombatChocoboExtraChecks { get; set; }

        [Setting]
        [DefaultValue(400)]
        [Category("Core")]
        public int SpellQueue { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Core")]
        public bool NoneGcdIgnoreShiftingConditions { get; set; }

        [Setting]
        [DefaultValue(1100)]
        [Category("Core")]
        public double NoneGcdWindowEnd { get; set; }

        [Setting]
        [DefaultValue(ForceGameContext.None)]
        [Category("Core")]
        public ForceGameContext ForcedContext { get; set; }

        [Setting]
        [DefaultValue(15f)]
        [Category("General")]
        public float MinDotHp { get; set; }

        [Browsable(false)]
        public System.Windows.Input.Key LastHotKeyChoice { get; set; }

        [Browsable(false)]
        public ModifierKey LastModKeyChoice { get; set; }

        [Browsable(false)]
        public virtual float RestEnergyDone
        {
            get
            {
                if (RestEnergy * 2 < 100)
                    return RestEnergy * 2;
                return 100f;
            }
        }

        [Browsable(false)]
        public virtual float RestHealthDone
        {
            get
            {
                if (RestHealth * 2 < 100)
                    return RestHealth * 2;
                return 100f;
            }
        }
    }
}
