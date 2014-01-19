﻿using YourBuddy.Core.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace YourBuddy.Interfaces.Settings
{
    internal class YbSettingsFD : Styx.Helpers.Settings
    {

        public YbSettingsFD()
            : base(InternalSettings.SettingsPath + "_Frost.xml")
        {
        }


        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Frost - Ability Options")]
        [DisplayName("Pillar of Frost")]
        [Description("Select the usage of Pillar of Frost.")]
        public Enum.AbilityTrigger PillarofFrost { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.Always)]
        [Category("Frost - Ability Options")]
        [DisplayName("Empower Rune Weapon")]
        [Description("Select the usage of Empower Rune Weapon.")]
        public Enum.AbilityTrigger EmpowerRuneWeapon { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Frost - Ability Options")]
        [DisplayName("Class Racials")]
        [Description("Select the usage of your class racials - Only DPS abilities (Buffs & Attacks).")]
        public Enum.AbilityTrigger ClassRacials { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Frost - Ability Options")]
        [DisplayName("Raise Dead")]
        [Description("Select the usage of Raise Dead.")]
        public Enum.AbilityTrigger RaiseDead { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Frost - Ability Options")]
        [DisplayName("Mastersimple Rotation")]
        [Description("Select the usage of Mastersimple Rotation.")]
        public bool MasterSimple { get; set; }
        
        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Frost - Ability Options")]
        [DisplayName("Enable Outbreak")]
        [Description("Select the usage of Outbreak. NOT RECOMMENDED TO ENABLE THIS UNLESS YOU DONT HAVE T16.")]
        public bool EnableOutbreak { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Frost - Ability Options")]
        [DisplayName("Enable Death Strike")]
        [Description("Select the usage of Death Strike.")]
        public bool EnableDeathStrike { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Frost - Ability Options")]
        [DisplayName("Enable Death Siphon")]
        [Description("Select the usage of Death Siphon")]
        public bool EnableDeathSiphon { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(80)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Death Siphon %")]
        [Description("Uses Death Siphon if specced at % of HP")]
        public int DeathSiphonHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(80)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Death Strike %")]
        [Description("Uses Death Strike if specced at % of HP")]
        public int DeathstrikeHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(36)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Soul Reaper %")]
        [Description("Select the use-on HP for Soul Reaper usage.")]
        public int SoulReaperHP { get; set; }

        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Frost - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public Enum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Frost - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Frost - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Frost - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public Enum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(Enum.AbilityTrigger.OnBossDummy)]
        [Category("Frost - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public Enum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable AMZ")]
        [Description("Enables AMZ usage")]
        public bool CheckAMZ { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Frost - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}