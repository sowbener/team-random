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
        [DefaultValue(false)]
        [Category("Ninja")]
        public bool ShurikenCheckMobs { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Ninja")]
        public string ShurikenMobs { get; set; }

        [Browsable(false)]
        public List<string> ShurikenList { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rogue")]
        public bool Assassinate { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rogue")]
        public bool MinHpCustom { get; set; }

        [Setting]
        [DefaultValue(15f)]
        [Category("Rogue")]
        public float MinHpTrick { get; set; }

        [Setting]
        [DefaultValue(15f)]
        [Category("Rogue")]
        public float MinHpDance { get; set; }

        [Setting]
        [DefaultValue(15f)]
        [Category("Rogue")]
        public float MinHpMut { get; set; }

        [Setting]
        [DefaultValue(15f)]
        [Category("Rogue")]
        public float MinHpSf { get; set; }

        [Setting]
        [DefaultValue(80)]
        [Category("Rogue")]
        public int TrickIsBehindAdjustment { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rogue Opener")]
        public bool WaitForCooldowns { get; set; }

        [Setting]
        [DefaultValue(15f)]
        [Category("Rogue Opener")]
        public float MinHp { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rogue Opener")]
        public bool UseOpener { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rogue Opener")]
        public bool PotionWaitForIrAndB4B { get; set; }

        [Setting]
        [DefaultValue(RogueOpeners.DancingEdgeShadowFangMutilate)]
        [Category("Rogue Opener")]
        public RogueOpeners Order { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Rogue Opener")]
        public bool AeolianWhileDancingHalted { get; set; }

        [Setting] 
        [DefaultValue(false)]
        [Category("Rogue")]
        public bool SuitonWaitForIrAndB4B { get; set; }
        
        [Setting] 
        [DefaultValue(false)]
        [Category("Rogue Opener")]
        public bool B4BBeforePull { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rogue")]
        public bool SneakAttack { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rogue")]
        public bool WaitEnergyPull { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rogue")]
        public bool UseHide { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Ninja")]
        public int HutonClip { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Ninja")]
        public bool HutonNoTarget { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Ninja")]
        public bool MoveStopNinjutsu { get; set; }

        [Setting]
        [DefaultValue(15f)]
        [Category("Ninja")]
        public float MinNinjutsu { get; set; }

        [Setting]
        [DefaultValue(8000)]
        [Category("Rogue")]
        public int DancingEdgeClip { get; set; }

        [Setting]
        [DefaultValue(7000)]
        [Category("Rogue")]
        public int ShadowFangClip { get; set; }

        [Setting]
        [DefaultValue(6000)]
        [Category("Rogue")]
        public int MutilationClip { get; set; }

        [Setting]
        [DefaultValue("")]
        [Category("Rogue")]
        public string JugulateSpells { get; set; }

        [Browsable(false)]
        public List<string> JugulateList { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Rogue")]
        public bool JugulateCheckSpells { get; set; }
        
        [Setting]
        [DefaultValue(true)]
        [Category("Rogue")]
        public bool JugulateDps { get; set; }

        [Setting, DefaultValue(System.Windows.Input.Key.G)]
        [Category("Rogue Hotkey")]
        public System.Windows.Input.Key JugulateHotKeyChoice { get; set; }

        [Setting, DefaultValue(ModifierKey.Shift)]
        [Category("Rogue Hotkey")]
        public ModifierKey JugulateModKeyChoice { get; set; }

        [Browsable(false)]
        public System.Windows.Input.Key LastJugulateHotKeyChoice { get; set; }
        [Browsable(false)]
        public ModifierKey LastJugulateModKeyChoice { get; set; }

        [Setting, DefaultValue(System.Windows.Input.Key.B)]
        [Category("Rogue Hotkey")]
        public System.Windows.Input.Key DancingEdgeHotKeyChoice { get; set; }

        [Setting, DefaultValue(ModifierKey.Shift)]
        [Category("Rogue Hotkey Hotkey")]
        public ModifierKey DancingEdgeModKeyChoice { get; set; }

        [Browsable(false)]
        public System.Windows.Input.Key LastDancingEdgeHotKeyChoice { get; set; }
        [Browsable(false)]
        public ModifierKey LastDancingEdgeModKeyChoice { get; set; }

        [Setting, DefaultValue(System.Windows.Input.Key.V)]
        [Category("Rogue Hotkey")]
        public System.Windows.Input.Key DotsHotKeyChoice { get; set; }

        [Setting, DefaultValue(ModifierKey.Shift)]
        [Category("Rogue Hotkey")]
        public ModifierKey DotsModKeyChoice { get; set; }

        [Browsable(false)]
        public System.Windows.Input.Key LastDotsHotKeyChoice { get; set; }
        [Browsable(false)]
        public ModifierKey LastDotsModKeyChoice { get; set; }


    }

}