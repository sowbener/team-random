using YourBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourBuddy.Interfaces.Settings
{
    internal class YbSettingsCR : Styx.Helpers.Settings
    {

        public YbSettingsCR()
            : base(InternalSettings.SettingsPath + "_Combat.xml")
        {
        }

        #region Ability Options


        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Combat - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Combat - Ability Options")]
        [DisplayName("Killing Spree")]
        [Description("Select the usage of Killing Spree.")]
        public Enum.AbilityTrigger KillingSpree { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Combat - Ability Options")]
        [DisplayName("Adrenaline Rush")]
        [Description("Select the usage of Adrenaline Rush")]
        public Enum.AbilityTrigger AdrenalineRush { get; set; }


        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Combat - Ability Options")]
        [DisplayName("Tier6 Ability")]
        [Description("Select the usage of your Tier6 Ability")]
        public Enum.AbilityTrigger Tier6Abilities { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Combat - Ability Options")]
        [DisplayName("Shadowblades")]
        [Description("Select the usage of Shadow Blades.")]
        public Enum.AbilityTrigger ShadowBlades { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Ability Options")]
        [DisplayName("Expose Armor")]
        [Description("Select to Enable or Disable Expose Armor")]
        public bool CheckExposeArmor { get; set; }
        
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Combat - Ability Options")]
        [DisplayName("Rupture Hotkey")]
        [Description("Select to Enable or Disable Rupture Hotkey")]
        public bool EnableRuptureHotkey { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Ability Options")]
        [DisplayName("Blade Flurry")]
        [Description("Select to Enable or Disable Automated Blade Flurry.")]
        public bool AutoTurnOffBladeFlurry { get; set; } 

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Ability Options")]
        [DisplayName("Rupture")]
        [Description("Select to Enable or Disable Rupture")]
        public bool CheckRupture { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Combat - Ability Options")]
        [DisplayName("Rupture")]
        [Description("Select the usage of Rupture")]
        public Enum.AbilityTrigger Rupture { get; set; }

        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Combat - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Combat - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Combat - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Combat - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Combat - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.PoisonM.Deadly)]
        [Category("Combat - Item Options")]
        [DisplayName("Poison Mainhand")]
        [Description("Select which poison to put on Main-Hand")]
        public Enum.PoisonM PoisonSelectorM { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.PoisonO.Leeching)]
        [Category("Combat - Item Options")]
        [DisplayName("Poison Offhand")]
        [Description("Select which poison to put on Off-Hand")]
        public Enum.PoisonO PoisonSelectorO { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Combat - Item Options")]
        [DisplayName("Enable Auto Poison")]
        [Description("Enable Poison on MainHand and OffHand")]
        public bool CheckPoison { get; set; }

        #endregion

        #region SelfHealingStuff


        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Recuperate")]
        [Description("Select to Enable or Disable Recuperate")]
        public bool CheckRecuperate { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Combat Readiness")]
        [Description("Select to Enable or Disable Combat Readiness")]
        public bool CheckCombatReadiness { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Shadowstep")]
        [Description("Select to Enable or Disable Shadowstep")]
        public bool CheckShadowstep { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Evasion")]
        [Description("Select to Enable or Disable Evasion")]
        public bool CheckEvasion { get; set; }
        

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Cloak of Shadows")]
        [Description("Select to Enable or Disable Cloak of Shadows")]
        public bool CheckCloakofShadows { get; set; }
        
        [Setting]
        [Styx.Helpers.DefaultValue(4)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Recuperate Combo Points")]
        [Description("Will use Recuperate at given combo point.")]
        public int NumRecuperateComboPoints { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Recuperate %")]
        [Description("Will use Recuperate at given hp %")]
        public int NumRecuperateHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Cloak of Shadows %")]
        [Description("Will use Cloak of Shadows at given hp %")]
        public int NumCloakofShadowsHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Combat Readiness %")]
        [Description("Will use Combat Readiness at given hp %")]
        public int NumCombatReadiness { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Shadowstep %")]
        [Description("Will use Shadowstep at given hp %")]
        public int NumShadowstep { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Combat - Selfhealing")]
        [DisplayName("Evasion %")]
        [Description("Will use Evasion at given hp %")]
        public int NumEvasion { get; set; }




        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Combat - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Combat - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Selectable Options")]
        [DisplayName("Enable PvP Rotation")]
        [Description("Enables the Combat PvP Rotation")]
        public bool PvPRotationCheck { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Combat - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Combat - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Combat - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
