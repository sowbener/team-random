using ff14bot;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Interfaces.GUI.Extentions;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Interface.GUI
{
    public partial class YrbGui : Form
    {
        public Point overlayLocation { get; set; }
        public YrbGui()
        {
            InitializeComponent();
            FillComboBoxes();
        }

        internal void Interface_Load(object sender, EventArgs e)
        {
            flowButton1.Text = Overlay.Overlay.isShown() ? "Close Overlay" : "Open Overlay";
            propertyGrid2.SelectedObject = InternalSettings.Instance.General;
            switch (Core.Player.CurrentJob)
            {
                #region Jobs Loading on PropertyGrid
                case ff14bot.Enums.ClassJobType.Ninja:
                    tabPage3.Text = "Ninja";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Ninja;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Bard:
                    tabPage3.Text = "Bard";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Bard;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.BlackMage:
                    tabPage3.Text = "Black-Mage";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.BlackMage;
                    Refresh();
                    break;
                      case ff14bot.Enums.ClassJobType.DarkKnight:
                         tabPage3.Text = "Dark-Knight";
                       propertyGrid1.SelectedObject = InternalSettings.Instance.DarkKnight;
                       Refresh();
                     break;
                       case ff14bot.Enums.ClassJobType.Machinist:
                       tabPage3.Text = "Machinist";
                       propertyGrid1.SelectedObject = InternalSettings.Instance.Machinist;
                       Refresh();
                       break;
                       case ff14bot.Enums.ClassJobType.Astrologian:
                       tabPage3.Text = "Astrologian";
                       propertyGrid1.SelectedObject = InternalSettings.Instance.Astrologian;
                       Refresh();
                       break;
                case ff14bot.Enums.ClassJobType.Dragoon:
                    tabPage3.Text = "Dragoon";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Dragoon;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Monk:
                    tabPage3.Text = "Monk";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Monk;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Paladin:
                    tabPage3.Text = "Paladin";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Paladin;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Scholar:
                    tabPage3.Text = "Scholar";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Scholar;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Summoner:
                    tabPage3.Text = "Summoner";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Summoner;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Warrior:
                    tabPage3.Text = "Warrior";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Warrior;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.WhiteMage:
                    tabPage3.Text = "White-Mage";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.WhiteMage;
                    Refresh();
                    break;
                #endregion

                #region Class Loading on PropertyGrid
                case ff14bot.Enums.ClassJobType.Arcanist:
                    tabPage3.Text = "Arcanist";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Arcanist;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Archer:
                    tabPage3.Text = "Archer";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Archer;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Conjurer:
                    tabPage3.Text = "Conjurer";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Conjurer;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Gladiator:
                    tabPage3.Text = "Gladiator";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Gladiator;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Lancer:
                    tabPage3.Text = "Lancer";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Lancer;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Marauder:
                    tabPage3.Text = "Marauder";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Marauder;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Pugilist:
                    tabPage3.Text = "Pugilist";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Pugilist;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Rogue:
                    tabPage3.Text = "Rogue";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Rogue;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Thaumaturge:
                    tabPage3.Text = "Thaumaturge";
                    propertyGrid1.SelectedObject = InternalSettings.Instance.Thaumaturge;
                    Refresh();
                    break;
                    #endregion
            }
        }

        private void flowButton4_Click(object sender, EventArgs e)
        {
            //General Settings Save\\
            InternalSettings.Instance.General.Save();
            InternalSettings.Instance.Hotkeys.Save();
            //Jobs Settings Save\\
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Ninja) InternalSettings.Instance.Ninja.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Bard) InternalSettings.Instance.Bard.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.BlackMage) InternalSettings.Instance.BlackMage.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Dragoon) InternalSettings.Instance.Dragoon.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Monk) InternalSettings.Instance.Monk.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Paladin) InternalSettings.Instance.Paladin.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Scholar) InternalSettings.Instance.Scholar.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Summoner) InternalSettings.Instance.Summoner.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Warrior) InternalSettings.Instance.Warrior.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.WhiteMage) InternalSettings.Instance.WhiteMage.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.DarkKnight) InternalSettings.Instance.DarkKnight.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Machinist) InternalSettings.Instance.Machinist.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Astrologian) InternalSettings.Instance.Astrologian.Save();
            //Class Settings Save\\
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Arcanist) InternalSettings.Instance.Arcanist.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Archer) InternalSettings.Instance.Archer.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Conjurer) InternalSettings.Instance.Conjurer.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Gladiator) InternalSettings.Instance.Gladiator.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Lancer) InternalSettings.Instance.Lancer.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Marauder) InternalSettings.Instance.Marauder.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Pugilist) InternalSettings.Instance.Pugilist.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Rogue) InternalSettings.Instance.Rogue.Save();
            if (Core.Player.CurrentJob == ff14bot.Enums.ClassJobType.Thaumaturge) InternalSettings.Instance.Thaumaturge.Save();

            Logger.Write("Settings for YourRaidingBuddy saved!");
            Root.RebuildBehaviors("Save and Close Button!");

            Close();
        }

        #region Fill Hotkeys!
        private void FillComboBoxes()
        {
            // Key library: http://msdn.microsoft.com/en-us/library/aa245676(v=vs.60).aspx
            // Hotkey - Demoralizing Banner
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxLight6.Items.Add(new Extention2.YRBCboItem((int)Keys.F12, "F12"));

            // Hotkey - Heroic Leap
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxLight7.Items.Add(new Extention2.YRBCboItem((int)Keys.F12, "F12"));

            // Hotkey - Mocking Banner
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxLight3.Items.Add(new Extention2.YRBCboItem((int)Keys.F12, "F12"));

            // Hotkey - Tier 4 Abilities
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxLight4.Items.Add(new Extention2.YRBCboItem((int)Keys.F12, "F12"));

            // Hotkey - Tier 6 Abilities
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxLight5.Items.Add(new Extention2.YRBCboItem((int)Keys.F12, "F12"));

            // Hotkey Mode Choice
            chromeComboBoxLight1.Items.Add(new Extention2.YRBCboItem((int)HotkeyMode.Automatic, "Automatic Mode"));
            chromeComboBoxLight1.Items.Add(new Extention2.YRBCboItem((int)HotkeyMode.HotkeyMode, "Hotkey Mode"));
            chromeComboBoxLight1.Items.Add(new Extention2.YRBCboItem((int)HotkeyMode.SemiHotkeyMode, "Semi Hotkey Mode"));

            // Hotkey Modifier Key
            chromeComboBoxLight2.Items.Add(new Extention2.YRBCboItem((int)ModifierKeyss.Alt, "Alt - Mod"));
            chromeComboBoxLight2.Items.Add(new Extention2.YRBCboItem((int)ModifierKeyss.Control, "Ctrl - Mod"));
            chromeComboBoxLight2.Items.Add(new Extention2.YRBCboItem((int)ModifierKeyss.Shift, "Shift - Mod"));


            Extention2.SetComboBoxEnums(chromeComboBoxLight1, (int)InternalSettings.Instance.Hotkeys.HotkeyModeSelection);
            Extention2.SetComboBoxEnums(chromeComboBoxLight2, (int)InternalSettings.Instance.Hotkeys.ModifierKey);
            Extention2.SetComboBoxEnums(chromeComboBoxLight3, (int)InternalSettings.Instance.Hotkeys.CooldownKey);
            Extention2.SetComboBoxEnums(chromeComboBoxLight6, (int)InternalSettings.Instance.Hotkeys.SpecialKey);
            Extention2.SetComboBoxEnums(chromeComboBoxLight7, (int)InternalSettings.Instance.Hotkeys.SpecialKey1);
            Extention2.SetComboBoxEnums(chromeComboBoxLight5, (int)InternalSettings.Instance.Hotkeys.PauseKey);
            Extention2.SetComboBoxEnums(chromeComboBoxLight4, (int)InternalSettings.Instance.Hotkeys.MultiTargetKey);

        }



        #endregion

        private void flowButton1_Click_1(object sender, EventArgs e)
        {
            if (flowButton1.Text == "Open Overlay")
            {
                Overlay.Overlay.showOverlay(overlayLocation);
                flowButton1.Text = "Close Overlay";
            }
            else
            {
                Overlay.Overlay.hideOverlay();
                flowButton1.Text = "Open Overlay";
            }
        }

        private void chromeComboBoxLight1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.HotkeyModeSelection = (HotkeyMode)Extention2.GetComboBoxEnums(chromeComboBoxLight1);
        }

        private void chromeComboBoxLight2_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.ModifierKey = (ModifierKeys)Extention2.GetComboBoxEnums(chromeComboBoxLight2);
        }

        private void chromeComboBoxLight3_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.CooldownKey = (Keys)Extention2.GetComboBoxEnums(chromeComboBoxLight3);
        }

        private void chromeComboBoxLight4_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.MultiTargetKey= (Keys)Extention2.GetComboBoxEnums(chromeComboBoxLight4);
        }

        private void chromeComboBoxLight5_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.PauseKey = (Keys)Extention2.GetComboBoxEnums(chromeComboBoxLight5);
        }

        private void chromeComboBoxLight6_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.SpecialKey = (Keys)Extention2.GetComboBoxEnums(chromeComboBoxLight6);
        }

        private void chromeComboBoxLight7_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.SpecialKey1 = (Keys)Extention2.GetComboBoxEnums(chromeComboBoxLight7);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
