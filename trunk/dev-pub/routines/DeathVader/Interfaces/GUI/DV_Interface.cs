using DeathVader.Helpers;
using DeathVader.Interfaces.Settings;
using DeathVader.Managers;
using Styx;
using Styx.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DeathVader.Interfaces.GUI
{
    public partial class DvInterface : Form
    {
        /* Form Dragging Support */
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();

        private void Dv_GuiDragDrop(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    ReleaseCapture();
                    SendMessage(Handle, 0xa1, 0x2, 0);
                    break;
            }
        }

        /* Requirements for Combobox Enumerations */
        private static void SetComboBoxEnum(ComboBox cb, int e)
        {
            DvEnum.DvCboItem item;
            for (var i = 0; i < cb.Items.Count; i++)
            {
                item = (DvEnum.DvCboItem)cb.Items[i];
                if (item.E != e) continue;
                cb.SelectedIndex = i;
                return;
            }
            item = (DvEnum.DvCboItem)cb.Items[0];
            DvLogger.DiagLogW("Dialog Error: Combobox {0} does not have enum({1}) in list, defaulting to enum({2})",
                          cb.Name, e, item.E);
            cb.SelectedIndex = 0;
        }

        private static int GetComboBoxEnum(ComboBox cb)
        {
            var item = (DvEnum.DvCboItem)cb.Items[cb.SelectedIndex];
            return item.E;
        }

        public DvInterface()
        {
            InitializeComponent();
            DvSettings.Instance.Load();
            DvSettingsH.Instance.Load();

            // Hotkey - AMZ
            ComboHkAMZ.Items.Add(new DvEnum.DvCboItem((int)Keys.None, "No Hotkey"));
            ComboHkAMZ.Items.Add(new DvEnum.DvCboItem((int)Keys.Q, "Q"));
            ComboHkAMZ.Items.Add(new DvEnum.DvCboItem((int)Keys.E, "E"));
            ComboHkAMZ.Items.Add(new DvEnum.DvCboItem((int)Keys.R, "R"));
            ComboHkAMZ.Items.Add(new DvEnum.DvCboItem((int)Keys.G, "G"));
            ComboHkAMZ.Items.Add(new DvEnum.DvCboItem((int)Keys.LControlKey, "Left Control"));
            ComboHkAMZ.Items.Add(new DvEnum.DvCboItem((int)Keys.RControlKey, "Right Control"));
            ComboHkAMZ.Items.Add(new DvEnum.DvCboItem((int)Keys.LShiftKey, "Left Shift"));
            ComboHkAMZ.Items.Add(new DvEnum.DvCboItem((int)Keys.RShiftKey, "Right Shift"));

            // Hotkey - Remorseless Winter etc
            ComboHkTier6.Items.Add(new DvEnum.DvCboItem((int)Keys.None, "No Hotkey"));
            ComboHkTier6.Items.Add(new DvEnum.DvCboItem((int)Keys.Q, "Q"));
            ComboHkTier6.Items.Add(new DvEnum.DvCboItem((int)Keys.E, "E"));
            ComboHkTier6.Items.Add(new DvEnum.DvCboItem((int)Keys.G, "G"));
            ComboHkTier6.Items.Add(new DvEnum.DvCboItem((int)Keys.LControlKey, "Left Control"));
            ComboHkTier6.Items.Add(new DvEnum.DvCboItem((int)Keys.RControlKey, "Right Control"));
            ComboHkTier6.Items.Add(new DvEnum.DvCboItem((int)Keys.LShiftKey, "Left Shift"));
            ComboHkTier6.Items.Add(new DvEnum.DvCboItem((int)Keys.RShiftKey, "Right Shift"));

            // Hotkey - Army of the Dead
            ComboHkArmy.Items.Add(new DvEnum.DvCboItem((int)Keys.None, "No Hotkey"));
            ComboHkArmy.Items.Add(new DvEnum.DvCboItem((int)Keys.Q, "Q"));
            ComboHkArmy.Items.Add(new DvEnum.DvCboItem((int)Keys.F, "F"));
            ComboHkArmy.Items.Add(new DvEnum.DvCboItem((int)Keys.R, "R"));
            ComboHkArmy.Items.Add(new DvEnum.DvCboItem((int)Keys.E, "E"));
            ComboHkArmy.Items.Add(new DvEnum.DvCboItem((int)Keys.G, "G"));
            ComboHkArmy.Items.Add(new DvEnum.DvCboItem((int)Keys.LControlKey, "Left Control"));
            ComboHkArmy.Items.Add(new DvEnum.DvCboItem((int)Keys.RControlKey, "Right Control"));
            ComboHkArmy.Items.Add(new DvEnum.DvCboItem((int)Keys.LShiftKey, "Left Shift"));
            ComboHkArmy.Items.Add(new DvEnum.DvCboItem((int)Keys.RShiftKey, "Right Shift"));

            // Hotkey - Raise Ally
            ComboHkAlly.Items.Add(new DvEnum.DvCboItem((int)Keys.None, "No Hotkey"));
            ComboHkAlly.Items.Add(new DvEnum.DvCboItem((int)Keys.Q, "Q"));
            ComboHkAlly.Items.Add(new DvEnum.DvCboItem((int)Keys.F, "F"));
            ComboHkAlly.Items.Add(new DvEnum.DvCboItem((int)Keys.R, "R"));
            ComboHkAlly.Items.Add(new DvEnum.DvCboItem((int)Keys.E, "E"));
            ComboHkAlly.Items.Add(new DvEnum.DvCboItem((int)Keys.G, "G"));
            ComboHkAlly.Items.Add(new DvEnum.DvCboItem((int)Keys.LControlKey, "Left Control"));
            ComboHkAlly.Items.Add(new DvEnum.DvCboItem((int)Keys.RControlKey, "Right Control"));
            ComboHkAlly.Items.Add(new DvEnum.DvCboItem((int)Keys.LShiftKey, "Left Shift"));
            ComboHkAlly.Items.Add(new DvEnum.DvCboItem((int)Keys.RShiftKey, "Right Shift"));

            // Hotkey Mode Choice
            ComboHkMode.Items.Add(new DvEnum.DvCboItem((int)DvEnum.Mode.Auto, "Automatic Mode"));
            ComboHkMode.Items.Add(new DvEnum.DvCboItem((int)DvEnum.Mode.Hotkey, "Hotkey Mode"));

            // Hotkey Modifier Key
            ComboHkModifier.Items.Add(new DvEnum.DvCboItem((int)Styx.Common.ModifierKeys.Alt, "Alt - Mod"));
            ComboHkModifier.Items.Add(new DvEnum.DvCboItem((int)Styx.Common.ModifierKeys.Control, "Ctrl - Mod"));
            ComboHkModifier.Items.Add(new DvEnum.DvCboItem((int)Styx.Common.ModifierKeys.Shift, "Shift - Mod"));
            ComboHkModifier.Items.Add(new DvEnum.DvCboItem((int)Styx.Common.ModifierKeys.Win, "Win - Mod"));

            // Hotkey - Cooldowns
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.XButton1, "Mouse button 4"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.XButton2, "Mouse button 5"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F1, "F1"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F2, "F2"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F3, "F3"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F4, "F4"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F5, "F5"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F6, "F6"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F7, "F7"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F8, "F8"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F9, "F9"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F10, "F10"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F11, "F11"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F12, "F12"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.Q, "Q"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.E, "E"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.P, "P"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.F, "F"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.R, "R"));
            ComboHkCooldown.Items.Add(new DvEnum.DvCboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - AoE
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.XButton1, "Mouse button 4"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.XButton2, "Mouse button 5"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F1, "F1"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F2, "F2"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F3, "F3"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F4, "F4"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F5, "F5"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F6, "F6"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F7, "F7"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F8, "F8"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F9, "F9"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F10, "F10"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F11, "F11"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F12, "F12"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.Q, "Q"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.E, "E"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.P, "P"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.F, "F"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.R, "R"));
            ComboHkMultiTgt.Items.Add(new DvEnum.DvCboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - Pause
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.XButton1, "Mouse button 4"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.XButton2, "Mouse button 5"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F1, "F1"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F2, "F2"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F3, "F3"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F4, "F4"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F5, "F5"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F6, "F6"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F7, "F7"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F8, "F8"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F9, "F9"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F10, "F10"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F11, "F11"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F12, "F12"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.Q, "Q"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.E, "E"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.P, "P"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.F, "F"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.R, "R"));
            ComboHkPause.Items.Add(new DvEnum.DvCboItem((int)Keys.None, "No Hotkey"));

            SetComboBoxEnum(ComboHkAMZ, (int)DvSettingsH.Instance.AMZ);
            SetComboBoxEnum(ComboHkAlly, (int)DvSettingsH.Instance.RaiseAlly);
            SetComboBoxEnum(ComboHkTier6, (int)DvSettingsH.Instance.Tier6);
            SetComboBoxEnum(ComboHkArmy, (int)DvSettingsH.Instance.ArmyofTheDeadKey);

            SetComboBoxEnum(ComboHkMode, (int)DvSettingsH.Instance.ModeSelection);
            SetComboBoxEnum(ComboHkModifier, (int)DvSettingsH.Instance.ModKeyChoice);
            SetComboBoxEnum(ComboHkCooldown, (int)DvSettingsH.Instance.CooldownKeyChoice);
            SetComboBoxEnum(ComboHkMultiTgt, (int)DvSettingsH.Instance.MultiTgtKeyChoice);
            SetComboBoxEnum(ComboHkPause, (int)DvSettingsH.Instance.PauseKeyChoice);

            if (File.Exists(Utilities.AssemblyDirectory + @"\Routines\Death Vader\Interfaces\Images\Darth.png"))
                LogoPicture.ImageLocation =
                    string.Format(@"{0}\Routines\Death Vader\Interfaces\Images\Darth.png",
                                  Utilities.AssemblyDirectory);
        }

        private void DvInterface_Load(object sender, EventArgs e)
        {
            GeneralGrid.SelectedObject = Settings.DvSettings.Instance.General;
            DvSettings DvSettings = DvSettings  .Instance;
            Styx.Helpers.Settings selectSpec = null;
            switch (StyxWoW.Me.Specialization)
            {
                case WoWSpec.DeathKnightFrost:
                    selectSpec = DvSettings.Frost;
                    break;

                case WoWSpec.DeathKnightUnholy:
                    selectSpec = DvSettings.Unholy;
                    break;
            }

            if (selectSpec != null)
            {
                SpecGrid.SelectedObject = selectSpec;
            }

            int splitterPositionG = GeneralGrid.GetInternalLabelWidth();
            GeneralGrid.MoveSplitterTo(splitterPositionG + 35);
            int splitterPositionS = SpecGrid.GetInternalLabelWidth();
            SpecGrid.MoveSplitterTo(splitterPositionS + 35);
        }

        #region Hotkeys
        private void ComboHkAMZ_SelectedIndexChanged(object sender, EventArgs e)
        {
            DvSettingsH.Instance.AMZ = (Keys)GetComboBoxEnum(ComboHkAMZ);
        }
        private void ComboHkAlly_SelectedIndexChanged(object sender, EventArgs e)
        {
            DvSettingsH.Instance.RaiseAlly = (Keys)GetComboBoxEnum(ComboHkAlly);
        }
        private void ComboHkTier6_SelectedIndexChanged(object sender, EventArgs e)
        {
            DvSettingsH.Instance.Tier6 = (Keys)GetComboBoxEnum(ComboHkTier6);
        }

        private void ComboHkArmy_SelectedIndexChanged(object sender, EventArgs e)
        {
            DvSettingsH.Instance.ArmyofTheDeadKey = (Keys)GetComboBoxEnum(ComboHkArmy);
        }

        private void ComboHkMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            DvSettingsH.Instance.ModeSelection = (DvEnum.Mode)GetComboBoxEnum(ComboHkMode);
        }
        private void ComboHkModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            DvSettingsH.Instance.ModKeyChoice = (ModifierKeys)GetComboBoxEnum(ComboHkModifier);
        }
        private void ComboHkCooldown_SelectedIndexChanged(object sender, EventArgs e)
        {
            DvSettingsH.Instance.CooldownKeyChoice = (Keys)GetComboBoxEnum(ComboHkCooldown);
        }
        private void ComboHkMultiTgt_SelectedIndexChanged(object sender, EventArgs e)
        {
            DvSettingsH.Instance.MultiTgtKeyChoice = (Keys)GetComboBoxEnum(ComboHkMultiTgt);
        }
        private void ComboHkPause_SelectedIndexChanged(object sender, EventArgs e)
        {
            DvSettingsH.Instance.PauseKeyChoice = (Keys)GetComboBoxEnum(ComboHkPause);
        }
        #endregion

        #region Buttons
        private void SaveButton_Click(object sender, EventArgs e)
        {
            DvHotKeyManager.RemoveAllKeys();
            DvHotKeyManager.RegisterKeys();
            DvSettings.Instance.Save();
            DvSettingsH.Instance.Save();
            ((Styx.Helpers.Settings)GeneralGrid.SelectedObject).Save();
            if (SpecGrid.SelectedObject != null)
            {
                ((Styx.Helpers.Settings)SpecGrid.SelectedObject).Save();
            }
            DvLogger.InitLogO("Settings for Death Vader saved!");
            DvLogger.WriteToLogFile();
            Close();
        }
        #endregion

        private void LogoPicture_Click(object sender, EventArgs e)
        {

        }

        private void SpecGrid_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void ComboHkArmy_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            DvSettingsH.Instance.ArmyofTheDeadKey = (Keys)GetComboBoxEnum(ComboHkArmy);
        }
    }
}
