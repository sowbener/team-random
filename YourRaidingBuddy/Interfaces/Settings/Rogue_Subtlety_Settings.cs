﻿using YourRaidingBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourRaidingBuddy.Interfaces.Settings
{
    internal class YbSettingsSR : Styx.Helpers.Settings
    {

        public YbSettingsSR()
            : base(InternalSettings.SettingsPath + "_Subtlety.xml")
        {
        }


        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Subtlety - Ability Options")]
        [DisplayName("Shadow Blades")]
        [Description("Select the usage of Shadow Blades")]
        public Enum.AbilityTrigger ShadowBlades { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Subtlety - Ability Options")]
        [DisplayName("Shadow Dance")]
        [Description("Select the usage of Shadow Dance.")]
        public Enum.AbilityTrigger ShadowDance{ get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Subtlety - Ability Options")]
        [DisplayName("Tier6 Ability")]
        [Description("Select the usage of your Tier6 Ability")]
        public Enum.AbilityTrigger Tier6Abilities { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.PreperationUsage.OnBossAndVanishCooldown)]
        [Category("Subtlety - Ability Options")]
        [DisplayName("Preparation")]
        [Description("Select the usage of Preparation")]
        public Enum.PreperationUsage PreperationUsage { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Subtlety - Ability Options")]
        [DisplayName("Vanish")]
        [Description("Select the usage of Vanish")]
        public Enum.AbilityTrigger Vanish { get; set; }



        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Subtlety - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Subtlety - Ability Options")]
        [DisplayName("Expose Armor")]
        [Description("Select to Enable or Disable Expose Armor")]
        public bool CheckExposeArmor { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Subtlety - Ability Options")]
        [DisplayName("Feint")]
        [Description("Select to Enable or Disable Feint")]
        public bool EnableFeintUsage { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Subtlety - Ability Options")]
        [DisplayName("Shadowstep")]
        [Description("Select to Enable or Disable Shadowstep")]
        public bool CheckShadowstep { get; set; }

        
        [Setting]
        [Styx.Helpers.DefaultValue(50)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Shadowstep %")]
        [Description("Select the use-on HP for Shadowstep usage.")]
        public int NumShadowstep { get; set; }

        
        [Setting]
        [Styx.Helpers.DefaultValue(70)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Recuperate %")]
        [Description("Select the use-on HP for Recuperate usage.")]
        public int NumRecuperate { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Subtlety - Ability Options")]
        [DisplayName("Recuperate")]
        [Description("Select to Enable or Disable Recuperate")]
        public bool CheckRecuperate { get; set; }
        
        [Setting]
        [Styx.Helpers.DefaultValue(3)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Recuperate Combo Points")]
        [Description("Select the Combo Points Usage for Recuperate usage.")]
        public int NumRecuperateCombo { get; set; }
        
       

        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Potion")]
        [Description("Enable or Disable Potion usage.")]
        public bool CheckPotion { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.PoisonM.Deadly)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Poison Mainhand")]
        [Description("Select which poison to put on Main-Hand")]
        public Enum.PoisonM PoisonSelectorM { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.PoisonO.Leeching)]
        [Category("Subtlety  - Item Options")]
        [DisplayName("Poison Offhand")]
        [Description("Select which poison to put on Off-Hand")]
        public Enum.PoisonO PoisonSelectorO { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Subtlety - Item Options")]
        [DisplayName("Enable Auto Poison")]
        [Description("Enable Poison on MainHand and OffHand")]
        public bool CheckPoison { get; set; }

        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Subtlety - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Subtlety - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Subtlety - Selectable Options")]
        [DisplayName("Enable Self-Healing")]
        [Description("Enables SelfHealing and Defensive Cooldowns")]
        public bool EnableSelfHealing { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Subtlety - Selectable Options")]
        [DisplayName("Enable PvP Rotation")]
        [Description("Enables the Assassination PvP Rotation")]
        public bool PvPRotationCheck { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Subtlety - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Subtlety - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Subtlety - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Subtlety - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
