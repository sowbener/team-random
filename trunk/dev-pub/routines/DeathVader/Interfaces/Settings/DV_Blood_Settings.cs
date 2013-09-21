using DeathVader.Helpers;
using Styx.Helpers;
using System.ComponentModel;

namespace DeathVader.Interfaces.Settings
{
    internal class DvSettingsB : Styx.Helpers.Settings
    {

        public DvSettingsB()
            : base(DvSettings.SettingsPath + "_Blood.xml")
        {
        }


        #region Ability Options
        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Blood")]
        [DisplayName("Icebound Fortitude Percent")]
        [Description("Will use Icebound Fortitude at the set Health Percent. (Self Healing (General Tab) must be enabled as well.)")]
        public int BloodIceboundFortitudePercent
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood")]
        [DisplayName("Bone Shield Defensively")]
        [Description("Will use Bone Shield Defensively during combat.")]
        public bool UseBoneShieldDefensively
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood")]
        [DisplayName("Dancing Rune Weapon")]
        [Description("Will use Dancing Rune Weapon Defensively during combat.")]
        public bool UseDancingRuneWeapon
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(80)]
        [Category("Blood")]
        [DisplayName("Dancing Rune Weapon Percent")]
        [Description("Will use Dancing Rune Weapon at the set Health Percent. (Self Healing (General Tab) must be enabled as well.)")]
        public int DancingRuneWeaponPercent
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Common")]
        [DisplayName("Lichborne")]
        [Description("Will use Lichborne. (Self Healing (General Tab) must be enabled as well.)")]
        public bool UseLichborne
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Blood")]
        [DisplayName("Lichborne Percent")]
        [Description("Will use Lichborne at this HealthPercent. (Self Healing (General Tab) must be enabled as well.)")]
        public int LichbornePercent
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Blood")]
        [DisplayName("DeathCoil with Lichborne Percent")]
        [Description("Will use DeathCoil at this HealthPercent when Lichborne is active for self heal. (Self Healing (General Tab) must be enabled as well.)")]
        public int DeathCoilHealPercent
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood")]
        [DisplayName("Vampiric Blood")]
        [Description("If set to true Death Vader will use Vampiric Blood. (Self Healing (General Tab) must be enabled as well.)")]
        public bool UseVampiricBlood
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood")]
        [DisplayName("Unholy Blight")]
        [Description("If set to true Death Vader will use Unholy Blight. (Self Healing (General Tab) must be enabled as well.)")]
        public bool EnableUnholyBlight
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood")]
        [DisplayName("Empower Rune Weapon")]
        [Description("If set to true Death Vader will use Empower Rune Weapon. (Self Healing (General Tab) must be enabled as well.)")]
        public bool UseEmpowerRuneWeapon
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood")]
        [DisplayName("Pet Sacrifice")]
        [Description("If set to true Death Vader will use Pet Sacrifice. (Self Healing (General Tab) must be enabled as well.)")]
        public bool UsePetSacrifice
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood")]
        [DisplayName("Icebound Fortitude")]
        [Description("If set to true Death Vader will use Icebound Fortitude. (Self Healing (General Tab) must be enabled as well.)")]
        public bool UseIceboundFortitude
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood")]
        [DisplayName("Anti Magic Shell")]
        [Description("If set to true Death Vader will use AntiMagicShell. (Self Healing (General Tab) must be enabled as well.)")]
        public bool UseAntiMagicShell
        {
            get;
            set;
        }


        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Blood")]
        [DisplayName("Vampiric Blood Percent")]
        [Description("Will use Vampiric Blood at the set Healthpercent. (Self Healing (General Tab) must be enabled as well.)")]
        public int VampiricBloodPercent
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood")]
        [DisplayName("RuneTap & WoTN")]
        [Description("If set to true CLU will use RuneTap when Will of the Necropolis procs. (Self Healing (General Tab) must be enabled as well.)")]
        public bool UseRuneTapWoTN
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(80)]
        [Category("Blood")]
        [DisplayName("RuneTap & WoTN Percent")]
        [Description("Will use RuneTap when Will of the Necropolis procs at the set Healthpercent. (Self Healing (General Tab) must be enabled as well.)")]
        public int RuneTapWoTNPercent
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(30)]
        [Category("Blood")]
        [DisplayName("Pet Sacrifice Percent")]
        [Description("Will use PetSacrifice when set Healthpercent. (Self Healing (General Tab) must be enabled as well.)")]
        public int PetSacrificePercent
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood")]
        [DisplayName("RuneTap")]
        [Description("If set to true PureRotation will use RuneTap.")]
        public bool UseRuneTap
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(90)]
        [Category("Blood")]
        [DisplayName("RuneTap Percent")]
        [Description("Will use RuneTap at the set Healthpercent.")]
        public int RuneTapPercent
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(90)]
        [Category("Blood")]
        [DisplayName("Death Strike for Blood Shield Percent")]
        [Description("Will use Death Strike at the set Healthpercent and at the time [Death Strike Blood Shield Time Remaining] setting specifies. eg: If set to BS health = 101 and BS remaining = 2, Bloodshield will be maintained constantly")]
        public int DeathStrikeBloodShieldPercent
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(2)]
        [Category("Blood")]
        [DisplayName("Death Strike Blood Shield Time Remaining")]
        [Description("Will use Death Strike if the players bloodshield has less than the set time left and at the Healthpercent [Death Strike for Blood Shield Percent] setting specifies eg: If set to BS health = 101 and BS remaining = 2, Bloodshield will be maintained constantly")]
        public int DeathStrikeBloodShieldTimeRemaining
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(40)]
        [Category("Blood")]
        [DisplayName("Death Strike Percent")]
        [Description("Will use Death Strike at the set Healthpercent regardless of blood shield")]
        public int DeathStrikePercent
        {
            get;
            set;
        }

        [Setting]
        [Styx.Helpers.DefaultValue(60)]
        [Category("Blood")]
        [DisplayName("RuneStrike RunicPower Percent")]
        [Description("Will use RuneStrike when RunicPower Percent is greater thant the set value")]
        public int RuneStrikePercent
        {
            get;
            set;
        }

        #endregion

        #region Item Options
        [Setting]
        [Styx.Helpers.DefaultValue(DvEnum.AbilityTrigger.OnBossDummy)]
        [Category("Blood - Item Options")]
        [DisplayName("Hands / Waist")]
        [Description("Select the usage of your Hands / Waist.")]
        public DvEnum.AbilityTrigger UseHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Blood - Item Options")]
        [DisplayName("Healthstone")]
        [Description("Checked enables Healthstone usage.")]
        public bool CheckHealthStone { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(20)]
        [Category("Blood - Item Options")]
        [DisplayName("Healthstone %")]
        [Description("Select the use-on HP for Healthstone usage.")]
        public int CheckHealthStoneNum { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(DvEnum.AbilityTrigger.OnBossDummy)]
        [Category("Blood - Item Options")]
        [DisplayName("Trinket #1")]
        [Description("Select the usage of Trinket #1.")]
        public DvEnum.AbilityTrigger Trinket1 { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(DvEnum.AbilityTrigger.OnBossDummy)]
        [Category("Blood - Item Options")]
        [DisplayName("Trinket #2")]
        [Description("Select the usage of Trinket #2.")]
        public DvEnum.AbilityTrigger Trinket2 { get; set; }
        #endregion

        #region Selectable Options
        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood - Selectable Options")]
        [DisplayName("Enable Multi-Target (AoE)")]
        [Description("Enables AoE abilities, multi-target combat will be engaged.")]
        public bool CheckAoE { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood - Selectable Options")]
        [DisplayName("Enable AMZ")]
        [Description("Enables AMZ usage")]
        public bool CheckAMZ { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood - Selectable Options")]
        [DisplayName("Enable Auto Attack")]
        [Description("Enables auto attack, persistent combat while tabbing units.")]
        public bool CheckAutoAttack { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(36)]
        [Category("Blood - Selectable Options")]
        [DisplayName("Soul Reaper %")]
        [Description("Select the use-on HP for Soul Reaper usage.")]
        public int SoulReaperHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Blood - Selectable Options")]
        [DisplayName("Enable Item De-Sync")]
        [Description("Enables de-sync of items - With this enabled it uses Hands / Waist outside Trinket Cooldowns.")]
        public bool CheckDesyncTrinkHands { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(80)]
        [Category("Blood - Selectable Options")]
        [DisplayName("Death Siphon %")]
        [Description("Uses Death Siphon if specced at % of HP")]
        public int DeathSiphonHP { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(true)]
        [Category("Blood - Selectable Options")]
        [DisplayName("Enable Interrupts")]
        [Description("Enables interrupts, uses Pummel and Intimidating Shout.")]
        public bool CheckInterrupts { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(false)]
        [Category("Blood - Selectable Options")]
        [DisplayName("Enable Lifeblood")]
        [Description("Checked enables Lifeblood.")]
        public bool CheckLifeblood { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(10)]
        [Category("Blood - Selectable Options")]
        [DisplayName("Lifeblood %")]
        [Description("Will use Lifeblood when health % is less than or equal to the set value.")]
        public int CheckLifebloodNum { get; set; }
        #endregion
    }
}
