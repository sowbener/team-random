using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using System.Windows.Input;
using Clio.Utilities;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Settings;
using YourRaidingBuddy.Interfaces.Settings;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using YourRaidingBuddy.Interface.Extensions;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace YourRaidingBuddy.Interface.GUI
{
    public partial class Interface : Form
    {
        public Interface()
        {
            InitializeComponent();
            FillComboBoxes();

            //generalwebbrowser.DocumentText = Resources.Resources.Introduction;
            //spec3 Propertygrid for Warrior,Deathknight,Demon-hunter,Druid and Paladin Tanks! Use Spec4 for dps if any else!
           logopicturebox.BackgroundImageLayout = ImageLayout.Zoom;
            string IconToLoad = "";
            try
            {
                IconToLoad = string.Format("{0}\\Routines\\YourRaidingBuddy\\Resources\\yrb-logo.jpg",
                    Utilities.AssemblyDirectory);
                if (File.Exists(IconToLoad))
                    logopicturebox.ImageLocation = IconToLoad;
            }
            catch (Exception e)
            {
                Logging.WriteDiagnostic("Couldn't load one of the Files ({0})", IconToLoad);
                Logging.WriteDiagnostic("Thrown Exception: {0}", e.ToString());
            }
            closepicturebox.Image = Resources.Resources.IconCloseRound;


            ExtensionMethods.ImportImage(tabcontrolimagelist, tabcontrol, tabpage7, Resources.Resources.IconInfo, 0);
            ExtensionMethods.ImportImage(tabcontrolimagelist, tabcontrol, tabpage1, Resources.Resources.IconSettings, 1);
            ExtensionMethods.ImportImage(tabcontrolimagelist, tabcontrol, tabpage2, Resources.Resources.IconKeyboard, 2);
            if(Core.Player.CurrentJob == ClassJobType.Pugilist ||
                Core.Player.CurrentJob == ClassJobType.Lancer || Core.Player.CurrentJob == ClassJobType.Archer || Core.Player.CurrentJob == ClassJobType.Rogue ||
                Core.Player.CurrentJob == ClassJobType.Arcanist || Core.Player.CurrentJob == ClassJobType.BlackMage || Core.Player.CurrentJob == ClassJobType.Monk ||
                Core.Player.CurrentJob == ClassJobType.Dragoon || Core.Player.CurrentJob == ClassJobType.Bard || Core.Player.CurrentJob == ClassJobType.Ninja || Core.Player.CurrentJob == ClassJobType.Summoner)
               ExtensionMethods.ImportImage(tabcontrolimagelist, tabcontrol, tabpage3, Resources.Resources.IconKnife, 3);
            if(Core.Player.CurrentJob == ClassJobType.DarkKnight || Core.Player.CurrentJob == ClassJobType.Warrior || Core.Player.CurrentJob == ClassJobType.Gladiator ||
                Core.Player.CurrentJob == ClassJobType.Paladin || Core.Player.CurrentJob == ClassJobType.Marauder)
                ExtensionMethods.ImportImage(tabcontrolimagelist, tabcontrol, tabpage3, Resources.Resources.IconShield, 3);

        }

        internal void Interface_Load(object sender, EventArgs e)
        {
            // flowButton1.Text = Overlay.Overlay.isShown() ? "Close Overlay" : "Open Overlay";
            generalpropertygrid.SelectedObject = InternalSettings.Instance.General;
            switch (Core.Player.CurrentJob)
            {
                #region Jobs Loading on PropertyGrid
                case ff14bot.Enums.ClassJobType.Ninja:
                    tabpage3.Text = "Ninja Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Ninja;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Bard:
                    tabpage3.Text = "Bard Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Bard;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.BlackMage:
                    tabpage3.Text = "Black-Mage Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.BlackMage;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.DarkKnight:
                    tabpage3.Text = "Dark-Knight Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.DarkKnight;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Machinist:
                    tabpage3.Text = "Machinist Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Machinist;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Astrologian:
                    tabpage3.Text = "Astrologian Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Astrologian;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Dragoon:
                    tabpage3.Text = "Dragoon Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Dragoon;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Monk:
                    tabpage3.Text = "Monk Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Monk;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Paladin:
                    tabpage3.Text = "Paladin Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Paladin;
                    chromeLabel7.Text = "Really Change This #1";
                    chromeLabel8.Text = "Really Change This #2";
                    chromeLabel9.Text = "Really Change This #3";
                    chromeLabel10.Text = "Really Change This #4";
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Scholar:
                    tabpage3.Text = "Scholar Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Scholar;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Summoner:
                    tabpage3.Text = "Summoner Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Summoner;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Warrior:
                    tabpage3.Text = "Warrior Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Warrior;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.WhiteMage:
                    tabpage3.Text = "White-Mage Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.WhiteMage;
                    Refresh();
                    break;
                #endregion

                #region Class Loading on PropertyGrid
                case ff14bot.Enums.ClassJobType.Arcanist:
                    tabpage3.Text = "Arcanist Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Arcanist;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Archer:
                    tabpage3.Text = "Archer Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Archer;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Conjurer:
                    tabpage3.Text = "Conjurer Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Conjurer;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Gladiator:
                    tabpage3.Text = "Gladiator Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Gladiator;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Lancer:
                    tabpage3.Text = "Lancer Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Lancer;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Marauder:
                    tabpage3.Text = "Marauder Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Marauder;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Pugilist:
                    tabpage3.Text = "Pugilist Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Pugilist;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Rogue:
                    tabpage3.Text = "Rogue Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Rogue;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Thaumaturge:
                    tabpage3.Text = "Thaumaturge Settings";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Thaumaturge;
                    Refresh();
                    break;
                    #endregion


            }
        }

        #region Fill Hotkeys!
        private void FillComboBoxes()
        {
            // Key library: http://msdn.microsoft.com/en-us/library/aa245676(v=vs.60).aspx
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxDark6.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F12, "F12"));

            // Hotkey - Heroic Leap
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxDark7.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F12, "F12"));


            //
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxDark8.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F12, "F12"));

            //
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxDark9.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F12, "F12"));

            // Hotkey - Mocking Banner
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxDark3.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F12, "F12"));

            // Hotkey - Tier 4 Abilities
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxDark4.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F12, "F12"));

            // Hotkey - Tier 6 Abilities
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.None, "No Hotkey"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.A, "A"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.B, "B"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.C, "D"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F, "F"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.E, "E"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.G, "G"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.H, "H"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.I, "I"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.J, "J"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.K, "K"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.L, "L"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.M, "M"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.O, "O"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.P, "P"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Q, "Q"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.R, "R"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.S, "S"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.T, "T"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.U, "U"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.V, "V"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.W, "W"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.X, "X"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Y, "Y"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.Z, "Z"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton1, "(Mouse4 (Works on some PC's)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.XButton2, "(Mouse5 (Works on some PC's)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LControlKey, "Left Control"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RControlKey, "Right Control"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.LShiftKey, "Left Shift"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.RShiftKey, "Right Shift"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D1, "1 (no numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D2, "2 (no numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D3, "3 (no numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D4, "4 (no numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D5, "5 (no numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D6, "6 (no numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D7, "7 (no numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D8, "8 (no numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D9, "9 (no numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.D0, "0 (no numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad1, "1 (numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad2, "2 (numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad3, "3 (numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad4, "4 (numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad5, "5 (numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad6, "6 (numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad7, "7 (numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad8, "8 (numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad9, "9 (numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.NumPad0, "0 (numpad)"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F1, "F1"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F2, "F2"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F3, "F3"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F4, "F4"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F5, "F5"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F6, "F6"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F7, "F7"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F8, "F8"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F9, "F9"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F10, "F10"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F11, "F11"));
            chromeComboBoxDark5.Items.Add(new ExtensionMethods.YRBCboItem((int)Keys.F12, "F12"));

            // Hotkey Mode Choice
            chromeComboBoxDark1.Items.Add(new ExtensionMethods.YRBCboItem((int)HotkeyMode.Automatic, "Automatic Mode"));
            chromeComboBoxDark1.Items.Add(new ExtensionMethods.YRBCboItem((int)HotkeyMode.HotkeyMode, "Hotkey Mode"));
            chromeComboBoxDark1.Items.Add(new ExtensionMethods.YRBCboItem((int)HotkeyMode.SemiHotkeyMode, "Semi Hotkey Mode"));

            // Hotkey Modifier Key
            chromeComboBoxDark2.Items.Add(new ExtensionMethods.YRBCboItem((int)ModifierKeyss.Alt, "Alt - Mod"));
            chromeComboBoxDark2.Items.Add(new ExtensionMethods.YRBCboItem((int)ModifierKeyss.Control, "Ctrl - Mod"));
            chromeComboBoxDark2.Items.Add(new ExtensionMethods.YRBCboItem((int)ModifierKeyss.Shift, "Shift - Mod"));


            ExtensionMethods.SetComboBoxEnum(chromeComboBoxDark1, (int)InternalSettings.Instance.Hotkeys.HotkeyModeSelection);
            ExtensionMethods.SetComboBoxEnum(chromeComboBoxDark2, (int)InternalSettings.Instance.Hotkeys.ModifierKey);
            ExtensionMethods.SetComboBoxEnum(chromeComboBoxDark3, (int)InternalSettings.Instance.Hotkeys.PauseKey);
            ExtensionMethods.SetComboBoxEnum(chromeComboBoxDark4, (int)InternalSettings.Instance.Hotkeys.CooldownKey);
            ExtensionMethods.SetComboBoxEnum(chromeComboBoxDark5, (int)InternalSettings.Instance.Hotkeys.MultiTargetKey);
            ExtensionMethods.SetComboBoxEnum(chromeComboBoxDark6, (int)InternalSettings.Instance.Hotkeys.SpecialKey);
            ExtensionMethods.SetComboBoxEnum(chromeComboBoxDark7, (int)InternalSettings.Instance.Hotkeys.SpecialKey1);
            ExtensionMethods.SetComboBoxEnum(chromeComboBoxDark8, (int)InternalSettings.Instance.Hotkeys.SpecialKey2);
            ExtensionMethods.SetComboBoxEnum(chromeComboBoxDark9, (int)InternalSettings.Instance.Hotkeys.SpecialKey3);
        }



        #endregion

        private void closepicturebox_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Allows the form to be dragged on locations which have this function set.
        /// </summary>
        private void GuiDragDrop(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    ExtensionMethods.ReleaseCapture();
                    ExtensionMethods.SendMessage(Handle, 0xa1, 0x2, 0);
                    break;
            }
        }


        private void flowButton1_Click_1(object sender, EventArgs e)
        {
            ; //To be added.
        }

        private void CloseForm()
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

        private void SaveandCloseButton_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void SettingsComposerButton_Click(object sender, EventArgs e)
        {
            ;//Nothing here yet!
        }

        private void chromeComboBoxDark1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.HotkeyModeSelection =
                (HotkeyMode) ExtensionMethods.GetComboBoxEnum(chromeComboBoxDark1);
        }

        private void chromeComboBoxDark2_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.ModifierKey = (ModifierKeys)ExtensionMethods.GetComboBoxEnum(chromeComboBoxDark2);
        }

        private void chromeComboBoxDark3_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.PauseKey = (Keys)ExtensionMethods.GetComboBoxEnum(chromeComboBoxDark3);
        }

        private void chromeComboBoxDark4_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.CooldownKey = (Keys)ExtensionMethods.GetComboBoxEnum(chromeComboBoxDark4);
        }

        private void chromeComboBoxDark5_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.MultiTargetKey = (Keys)ExtensionMethods.GetComboBoxEnum(chromeComboBoxDark5);
        }

        private void chromeComboBoxDark6_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.SpecialKey = (Keys)ExtensionMethods.GetComboBoxEnum(chromeComboBoxDark6);
        }

        private void chromeComboBoxDark7_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.SpecialKey1 = (Keys)ExtensionMethods.GetComboBoxEnum(chromeComboBoxDark7);
        }

        private void chromeComboBoxDark8_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.SpecialKey2 = (Keys)ExtensionMethods.GetComboBoxEnum(chromeComboBoxDark8);
        }

        private void chromeComboBoxDark9_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalSettings.Instance.Hotkeys.SpecialKey3 = (Keys)ExtensionMethods.GetComboBoxEnum(chromeComboBoxDark9);
        }
    }
}