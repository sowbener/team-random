using System;
using System.Windows.Forms;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Settings;
using YourRaidingBuddy.Interfaces.Settings;
using ff14bot;
using YourRaidingBuddy.Interface.Extensions;

namespace YourRaidingBuddy.Interface.GUI
{
    public partial class Interface : Form
    {
        public Interface()
        {
            InitializeComponent();

            //generalwebbrowser.DocumentText = Resources.Resources.Introduction;
            //spec3 Propertygrid for Warrior,Deathknight,Demon-hunter,Druid and Paladin Tanks! Use Spec4 for dps if any else!
            closepicturebox.Image = Resources.Resources.IconCloseRound;
            logopicturebox.Image = Resources.Resources.yrb_logo;


            ExtensionMethods.ImportImage(tabcontrolimagelist, tabcontrol, tabpage7, Resources.Resources.IconInfo, 0);
            ExtensionMethods.ImportImage(tabcontrolimagelist, tabcontrol, tabpage1, Resources.Resources.IconSettings, 1);
            ExtensionMethods.ImportImage(tabcontrolimagelist, tabcontrol, tabpage2, Resources.Resources.IconKeyboard, 2);
            ExtensionMethods.ImportImage(tabcontrolimagelist, tabcontrol, tabpage3, Resources.Resources.IconKnife, 3);

        }

        internal void Interface_Load(object sender, EventArgs e)
        {
            // flowButton1.Text = Overlay.Overlay.isShown() ? "Close Overlay" : "Open Overlay";
            generalpropertygrid.SelectedObject = InternalSettings.Instance.General;
            switch (Core.Player.CurrentJob)
            {
                #region Jobs Loading on PropertyGrid
                case ff14bot.Enums.ClassJobType.Ninja:
                    tabpage3.Text = "Ninja";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Ninja;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Bard:
                    tabpage3.Text = "Bard";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Bard;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.BlackMage:
                    tabpage3.Text = "Black-Mage";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.BlackMage;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.DarkKnight:
                    tabpage3.Text = "Dark-Knight";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.DarkKnight;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Machinist:
                    tabpage3.Text = "Machinist";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Machinist;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Astrologian:
                    tabpage3.Text = "Astrologian";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Astrologian;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Dragoon:
                    tabpage3.Text = "Dragoon";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Dragoon;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Monk:
                    tabpage3.Text = "Monk";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Monk;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Paladin:
                    tabpage3.Text = "Paladin";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Paladin;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Scholar:
                    tabpage3.Text = "Scholar";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Scholar;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Summoner:
                    tabpage3.Text = "Summoner";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Summoner;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Warrior:
                    tabpage3.Text = "Warrior";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Warrior;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.WhiteMage:
                    tabpage3.Text = "White-Mage";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.WhiteMage;
                    Refresh();
                    break;
                #endregion

                #region Class Loading on PropertyGrid
                case ff14bot.Enums.ClassJobType.Arcanist:
                    tabpage3.Text = "Arcanist";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Arcanist;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Archer:
                    tabpage3.Text = "Archer";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Archer;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Conjurer:
                    tabpage3.Text = "Conjurer";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Conjurer;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Gladiator:
                    tabpage3.Text = "Gladiator";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Gladiator;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Lancer:
                    tabpage3.Text = "Lancer";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Lancer;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Marauder:
                    tabpage3.Text = "Marauder";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Marauder;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Pugilist:
                    tabpage3.Text = "Pugilist";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Pugilist;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Rogue:
                    tabpage3.Text = "Rogue";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Rogue;
                    Refresh();
                    break;
                case ff14bot.Enums.ClassJobType.Thaumaturge:
                    tabpage3.Text = "Thaumaturge";
                    spec1propertygrid.SelectedObject = InternalSettings.Instance.Thaumaturge;
                    Refresh();
                    break;
                    #endregion


            }
        }

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

    }
}