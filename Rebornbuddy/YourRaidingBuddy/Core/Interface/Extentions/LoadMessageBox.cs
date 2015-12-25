using Clio.Utilities;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using YourRaidingBuddy.Interfaces.Settings;
using YourRaidingBuddy.Settings;

namespace YourRaidingBuddy.Interface.Extensions
{
    public partial class LoadMessageBox : Form
    {
        public LoadMessageBox()
        {
            InitializeComponent();

            if (File.Exists(Utilities.AssemblyDirectory + @"\Routines\YourRaidingBuddy\Interfaces\Images\RoutineLogoSmallXX.png"))
                LogoPictureBox.ImageLocation =
                    string.Format(@"{0}\Routines\YourRaidingBuddy\Interfaces\Images\RoutineLogoSmallXX.png", Utilities.AssemblyDirectory);
        }

        /// <summary>
        /// Allows the form to be dragged on locations which have this function set.
        /// </summary>
        internal void YRB_GuiDragDrop(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    ExtensionMethods.ReleaseCapture();
                    ExtensionMethods.SendMessage(Handle, 0xa1, 0x2, 0);
                    break;
            }
        }

        private void LoadGeneralSettingsButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"Setting File|*.xml",
                Title = @"Load Settings from a File",
                InitialDirectory = string.Format("{0}\\Routines\\YourRaidingBuddy\\Interfaces\\Settings\\CustomSettings\\", Utilities.AssemblyDirectory),
            };

            var showDialog = openFileDialog.ShowDialog();

            if (!openFileDialog.FileName.Contains(".xml") || showDialog != DialogResult.OK) return;
            try
            {
                if (showDialog == DialogResult.Yes)
                {
                  // YourRaidingBuddy.Settings.CoreSetting.Instance. InternalSettings.Instance.LoadFromXML(XElement.Load(openFileDialog.FileName));
                }
                else
                {
                  //  InternalSettings.Instance.General.LoadFromXML(XElement.Load(openFileDialog.FileName));
                }

                InternalSettings.Instance.General.Save(); //InternalSettings.Instance.General.Save();
             //   Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show(string.Format(
                    "You have selected the wrong file - The selected file was: {0}. You need to load the corresponding 'general' file.", openFileDialog.FileName),
                    @"YourRaidingBuddy - An Error Has Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void LoadHotkeySettingsButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"Setting File|*.xml",
                Title = @"Load Settings from a File",
                InitialDirectory = string.Format("{0}\\Routines\\YourRaidingBuddy\\Interfaces\\Settings\\CustomSettings\\", Utilities.AssemblyDirectory),
            };

            var showDialog = openFileDialog.ShowDialog();

            if (!openFileDialog.FileName.Contains(".xml") || showDialog != DialogResult.OK) return;
            try
            {
                if (showDialog == DialogResult.Yes)
                {
                   // InternalSettings.Instance.LoadFromXML(XElement.Load(openFileDialog.FileName));
                }
                else
                {
                   // InternalSettings.Instance.Hotkeys.LoadFromXML(XElement.Load(openFileDialog.FileName));
                }

              //  InternalSettings.Instance.Hotkeys.Save();
            //    Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show(string.Format(
                    "You have selected the wrong file - The selected file was: {0}. You need to load the corresponding 'hotkey' file.", openFileDialog.FileName),
                    @"YourRaidingBuddy - An Error Has Occured",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void LoadSpecializationSettingsButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"Setting File|*.xml",
                Title = @"Load Settings from a File",
                InitialDirectory = string.Format("{0}\\Routines\\YourRaidingBuddy\\Interfaces\\Settings\\CustomSettings\\", Utilities.AssemblyDirectory),
            };

            var showDialog = openFileDialog.ShowDialog();

         /*   if (!openFileDialog.FileName.Contains(".xml") || showDialog != DialogResult.OK) return;
            try
            {
                if (showDialog == DialogResult.Yes)
                {
                 //   InternalSettings.Instance.LoadFromXML(XElement.Load(openFileDialog.FileName));
                }
                else
                {
                    switch (Root.MyToonSpec)
                    {
                        //Shaman Settings Load\\
                        case WoWSpec.ShamanEnhancement:
                         //   InternalSettings.Instance.Enhancement.LoadFromXML(XElement.Load(openFileDialog.FileName));
                        //    InternalSettings.Instance.Enhancement.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        case WoWSpec.ShamanElemental:
                        //    InternalSettings.Instance.Elemental.LoadFromXML(XElement.Load(openFileDialog.FileName));
                        //    InternalSettings.Instance.Elemental.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        //Monk Settings Load\\
                        case WoWSpec.MonkBrewmaster:
                            InternalSettings.Instance.Brewmaster.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Brewmaster.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        case WoWSpec.MonkWindwalker:
                            InternalSettings.Instance.Windwalker.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Windwalker.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        //Paladin Settings Load\\
                        case WoWSpec.PaladinProtection:
                            InternalSettings.Instance.Protection.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Protection.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        case WoWSpec.PaladinRetribution:
                            InternalSettings.Instance.Retribution.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Retribution.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        //Druid Settings Load\\
                        case WoWSpec.DruidBalance:
                            InternalSettings.Instance.Boomkin.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Boomkin.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                      //  case WoWSpec.DruidGuardian:
                       //     InternalSettings.Instance.Guardian.LoadFromXML(XElement.Load(openFileDialog.FileName));
                      //      InternalSettings.Instance.Guardian.Save();
                      //      Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                     //       Close();
                    //        break;
                      //Rogue Settings Load\\
                        case WoWSpec.RogueAssassination:
                            InternalSettings.Instance.Assassination.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Assassination.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        case WoWSpec.RogueCombat:
                            InternalSettings.Instance.Combat.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Combat.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        case WoWSpec.RogueSubtlety:
                            InternalSettings.Instance.Subtlety.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Subtlety.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        //Hunter Settings Load\\
                        case WoWSpec.HunterBeastMastery:
                            InternalSettings.Instance.Beastmastery.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Beastmastery.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        case WoWSpec.HunterMarksmanship:
                            InternalSettings.Instance.Marksmanship.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Marksmanship.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        case WoWSpec.HunterSurvival:
                            InternalSettings.Instance.Survival.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Survival.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        //Deathknight Settings Load\\
                        case WoWSpec.DeathKnightBlood:
                            InternalSettings.Instance.Blood.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Blood.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        case WoWSpec.DeathKnightFrost:
                            InternalSettings.Instance.Frost.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Frost.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        case WoWSpec.DeathKnightUnholy:
                            InternalSettings.Instance.Unholy.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Unholy.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                        //Mage Settings Load\\
                    //    case WoWSpec.MageArcane:
                    //        InternalSettings.Instance.Arcane.LoadFromXML(XElement.Load(openFileDialog.FileName));
                   //         InternalSettings.Instance.Arcane.Save();
                   //         Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                   //         Close();
                  //          break;
                        case WoWSpec.MageFire:
                            InternalSettings.Instance.Fire.LoadFromXML(XElement.Load(openFileDialog.FileName));
                            InternalSettings.Instance.Fire.Save();
                            Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                            Close();
                            break;
                      //  case WoWSpec.MageFrost:
                     //       InternalSettings.Instance.FrostM.LoadFromXML(XElement.Load(openFileDialog.FileName));
                    //        InternalSettings.Instance.FrostM.Save();
                    //        Logger.StartupLogR("Loaded file: {0}", openFileDialog.FileName);
                    //        Close();
                   //         break;
                    }
                }
          */
            }
        }
    }
