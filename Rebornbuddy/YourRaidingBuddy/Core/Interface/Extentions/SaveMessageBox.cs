using Clio.Utilities;
using System;
using System.IO;
using System.Windows.Forms;
using YourRaidingBuddy.Interfaces.Settings;
using YourRaidingBuddy.Settings;

namespace YourRaidingBuddy.Interfaces.GUI.Extentions
{
    public partial class SaveMessageBox : Form
    {
        public SaveMessageBox()
        {
            InitializeComponent();

            if (File.Exists(Utilities.AssemblyDirectory + @"\Routines\YourRaidingBuddy\Interfaces\Images\RoutineLogoSmallXX.png"))
                LogoPictureBox.ImageLocation =
                    string.Format(@"{0}\Routines\YourRaidingBuddy\Interfaces\Images\RoutineLogoSmallXX.png", Utilities.AssemblyDirectory);
        }

        /// <summary>
        /// Allows the form to be dragged on locations which have this function set.
        /// </summary>
        private void YRB_GuiDragDrop(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Extention2.ReleaseCapture();
                    Extention2.SendMessage(Handle, 0xa1, 0x2, 0);
                    break;
            }
        }

        private void SaveGeneralSettingsButton_Click(object sender, EventArgs e)
        {
            var saveGeneralDialog = new SaveFileDialog
            {
                Filter = @"Setting File|*.xml",
                Title = @"Save Settings from a File",
                InitialDirectory = string.Format("{0}\\Routines\\YourRaidingBuddy\\Interfaces\\Settings\\CustomSettings\\", Utilities.AssemblyDirectory),
                DefaultExt = "xml",
                FileName = "YRB_General_"  + "_" + DateTime.Now.ToString("dd-MM-yyyy")
            };

            var showDialog = saveGeneralDialog.ShowDialog();

            if (showDialog == DialogResult.OK)
            {
                InternalSettings.Instance.General.SaveAs(saveGeneralDialog.FileName);
              //  Logger.StartupLogR("Saved general settings to file ({0}).", saveGeneralDialog.FileName);
            }
            else
            {
               // Logger.StartupLogR("Cancelled Save to File function.");
            }
        }

        private void SaveHotkeySettingsButton_Click(object sender, EventArgs e)
        {
            var saveHotkeyDialog = new SaveFileDialog
            {
                Filter = @"Setting File|*.xml",
                Title = @"Save Settings from a File",
                InitialDirectory = string.Format("{0}\\Routines\\YourRaidingBuddy\\Interfaces\\Settings\\CustomSettings\\", Utilities.AssemblyDirectory),
                DefaultExt = "xml",
                FileName = "YRB_Hotkeys_" + "_" + DateTime.Now.ToString("dd-MM-yyyy")
            };

            var showDialog = saveHotkeyDialog.ShowDialog();

            if (showDialog == DialogResult.OK)
            {
               // InternalSettings.Instance.Hotkeys.SaveToFile(saveHotkeyDialog.FileName);
                //Logger.StartupLogR("Saved hotkey settings to file ({0}).", saveHotkeyDialog.FileName);
            }
            else
            {
                //Logger.StartupLogR("Cancelled Save to File function.");
            }
        }

        private void SaveSpecializationSettingsButton_Click(object sender, EventArgs e)
        {
            var saveSpecDialog = new SaveFileDialog
            {
                Filter = @"Setting File|*.xml",
                Title = @"Save Settings from a File",
                InitialDirectory = string.Format("{0}\\Routines\\YourRaidingBuddy\\Interfaces\\Settings\\CustomSettings\\", Utilities.AssemblyDirectory),
                DefaultExt = "xml",
                FileName = "YRB_Specialization_" + "_" + DateTime.Now.ToString("dd-MM-yyyy")
            };

            var showDialog = saveSpecDialog.ShowDialog();

        /*    if (showDialog == DialogResult.OK)
            {
                switch (Root.MyToonSpec)
                {
                    //Shaman
                    case WoWSpec.ShamanEnhancement:
                        InternalSettings.Instance.Enhancement.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Enhancement)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.ShamanElemental:
                        InternalSettings.Instance.Elemental.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Elemental)({0}).", saveSpecDialog.FileName);
                        break;
                    //Rogue
                    case WoWSpec.RogueAssassination:
                        InternalSettings.Instance.Assassination.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Assassination)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.RogueCombat:
                        InternalSettings.Instance.Combat.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Combat)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.RogueSubtlety:
                        InternalSettings.Instance.Subtlety.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Subtlety)({0}).", saveSpecDialog.FileName);
                        break;
                    //Hunter
                    case WoWSpec.HunterBeastMastery:
                        InternalSettings.Instance.Beastmastery.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Beastmastery)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.HunterMarksmanship:
                        InternalSettings.Instance.Marksmanship.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Marksmanship)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.HunterSurvival:
                        InternalSettings.Instance.Survival.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Survival)({0}).", saveSpecDialog.FileName);
                        break;
                    //Monk
                    case WoWSpec.MonkWindwalker:
                        InternalSettings.Instance.Windwalker.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Windwalker)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.MonkBrewmaster:
                        InternalSettings.Instance.Brewmaster.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Brewmaster)({0}).", saveSpecDialog.FileName);
                        break;
                    //Paladin
                    case WoWSpec.PaladinProtection:
                        InternalSettings.Instance.Protection.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Protection)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.PaladinRetribution:
                        InternalSettings.Instance.Retribution.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Retribution)({0}).", saveSpecDialog.FileName);
                        break;
                    //Druid
                    case WoWSpec.DruidBalance:
                        InternalSettings.Instance.Boomkin.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Boomkin)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.DruidGuardian:
                        InternalSettings.Instance.Guardian.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Guardian)({0}).", saveSpecDialog.FileName);
                        break;
                    //Deathknight
                    case WoWSpec.DeathKnightFrost:
                        InternalSettings.Instance.Frost.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Frost)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.DeathKnightBlood:
                        InternalSettings.Instance.Blood.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Blood)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.DeathKnightUnholy:
                        InternalSettings.Instance.Unholy.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Unholy)({0}).", saveSpecDialog.FileName);
                        break;
                    //Mage
                    case WoWSpec.MageArcane:
                        InternalSettings.Instance.Arcane.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Arcane)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.MageFire:
                        InternalSettings.Instance.Fire.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Fire)({0}).", saveSpecDialog.FileName);
                        break;
                    case WoWSpec.MageFrost:
                        InternalSettings.Instance.Frost.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Frost)({0}).", saveSpecDialog.FileName);
                        break;
                    //ShadowPriest
                    case WoWSpec.PriestShadow:
                        InternalSettings.Instance.Shadow.SaveToFile(saveSpecDialog.FileName);
                        Logger.StartupLogR("Saved specialization specifics to file (Shadow)({0}).", saveSpecDialog.FileName);
                        break;

                }
         */
            }
        }
    }
