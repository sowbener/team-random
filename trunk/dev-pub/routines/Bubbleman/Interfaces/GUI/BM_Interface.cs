using Bubbleman.Helpers;
using Bubbleman.Interfaces.Settings;
using Bubbleman.Managers;
using Styx;
using Styx.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bubbleman.Interfaces.GUI
{
    public partial class BMInterface : Form
    {
        /* Form Dragging Support */
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();

        private void BM_GuiDragDrop(object sender, MouseEventArgs e)
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
            BMEnum.BMCboItem item;
            for (var i = 0; i < cb.Items.Count; i++)
            {
                item = (BMEnum.BMCboItem)cb.Items[i];
                if (item.E != e) continue;
                cb.SelectedIndex = i;
                return;
            }
            item = (BMEnum.BMCboItem)cb.Items[0];
            BMLogger.DiagLogW("Dialog Error: Combobox {0} does not have enum({1}) in list, defaulting to enum({2})",
                          cb.Name, e, item.E);
            cb.SelectedIndex = 0;
        }

        private static int GetComboBoxEnum(ComboBox cb)
        {
            var item = (BMEnum.BMCboItem)cb.Items[cb.SelectedIndex];
            return item.E;
        }

        public BMInterface()
        {
            InitializeComponent();
            BMSettings.Instance.Load();
            BMSettingsH.Instance.Load();


            // Hotkey - ElusiveBrew of the Dead
            ComboHkElusiveBrew.Items.Add(new BMEnum.BMCboItem((int)Keys.None, "No Hotkey"));
            ComboHkElusiveBrew.Items.Add(new BMEnum.BMCboItem((int)Keys.Q, "Q"));
            ComboHkElusiveBrew.Items.Add(new BMEnum.BMCboItem((int)Keys.F, "F"));
            ComboHkElusiveBrew.Items.Add(new BMEnum.BMCboItem((int)Keys.R, "R"));
            ComboHkElusiveBrew.Items.Add(new BMEnum.BMCboItem((int)Keys.E, "E"));
            ComboHkElusiveBrew.Items.Add(new BMEnum.BMCboItem((int)Keys.G, "G"));
            ComboHkElusiveBrew.Items.Add(new BMEnum.BMCboItem((int)Keys.LControlKey, "Left Control"));
            ComboHkElusiveBrew.Items.Add(new BMEnum.BMCboItem((int)Keys.RControlKey, "Right Control"));
            ComboHkElusiveBrew.Items.Add(new BMEnum.BMCboItem((int)Keys.LShiftKey, "Left Shift"));
            ComboHkElusiveBrew.Items.Add(new BMEnum.BMCboItem((int)Keys.RShiftKey, "Right Shift"));

            // Hotkey - Raise DizzyingHaze
            ComboHkDizzyingHaze.Items.Add(new BMEnum.BMCboItem((int)Keys.None, "No Hotkey"));
            ComboHkDizzyingHaze.Items.Add(new BMEnum.BMCboItem((int)Keys.Q, "Q"));
            ComboHkDizzyingHaze.Items.Add(new BMEnum.BMCboItem((int)Keys.F, "F"));
            ComboHkDizzyingHaze.Items.Add(new BMEnum.BMCboItem((int)Keys.R, "R"));
            ComboHkDizzyingHaze.Items.Add(new BMEnum.BMCboItem((int)Keys.E, "E"));
            ComboHkDizzyingHaze.Items.Add(new BMEnum.BMCboItem((int)Keys.G, "G"));
            ComboHkDizzyingHaze.Items.Add(new BMEnum.BMCboItem((int)Keys.LControlKey, "Left Control"));
            ComboHkDizzyingHaze.Items.Add(new BMEnum.BMCboItem((int)Keys.RControlKey, "Right Control"));
            ComboHkDizzyingHaze.Items.Add(new BMEnum.BMCboItem((int)Keys.LShiftKey, "Left Shift"));
            ComboHkDizzyingHaze.Items.Add(new BMEnum.BMCboItem((int)Keys.RShiftKey, "Right Shift"));

            // Hotkey Mode Choice
            ComboHkMode.Items.Add(new BMEnum.BMCboItem((int)BMEnum.Mode.Auto, "Automatic Mode"));
            ComboHkMode.Items.Add(new BMEnum.BMCboItem((int)BMEnum.Mode.Hotkey, "Hotkey Mode"));

            // Hotkey Modifier Key
            ComboHkModifier.Items.Add(new BMEnum.BMCboItem((int)Styx.Common.ModifierKeys.Alt, "Alt - Mod"));
            ComboHkModifier.Items.Add(new BMEnum.BMCboItem((int)Styx.Common.ModifierKeys.Control, "Ctrl - Mod"));
            ComboHkModifier.Items.Add(new BMEnum.BMCboItem((int)Styx.Common.ModifierKeys.Shift, "Shift - Mod"));
            ComboHkModifier.Items.Add(new BMEnum.BMCboItem((int)Styx.Common.ModifierKeys.Win, "Win - Mod"));

            // Hotkey - Cooldowns
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F1, "F1"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F2, "F2"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F3, "F3"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F4, "F4"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F5, "F5"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F6, "F6"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F7, "F7"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F8, "F8"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F9, "F9"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F10, "F10"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F11, "F11"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F12, "F12"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.Q, "Q"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.E, "E"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.P, "P"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.F, "F"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.R, "R"));
            ComboHkCooldown.Items.Add(new BMEnum.BMCboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - AoE
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F1, "F1"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F2, "F2"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F3, "F3"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F4, "F4"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F5, "F5"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F6, "F6"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F7, "F7"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F8, "F8"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F9, "F9"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F10, "F10"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F11, "F11"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F12, "F12"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.Q, "Q"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.E, "E"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.P, "P"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.F, "F"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.R, "R"));
            ComboHkMultiTgt.Items.Add(new BMEnum.BMCboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - Pause
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F1, "F1"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F2, "F2"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F3, "F3"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F4, "F4"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F5, "F5"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F6, "F6"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F7, "F7"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F8, "F8"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F9, "F9"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F10, "F10"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F11, "F11"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F12, "F12"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.Q, "Q"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.E, "E"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.P, "P"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.F, "F"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.R, "R"));
            ComboHkPause.Items.Add(new BMEnum.BMCboItem((int)Keys.None, "No Hotkey"));

            SetComboBoxEnum(ComboHkDizzyingHaze, (int)BMSettingsH.Instance.DizzyingHaze);
            SetComboBoxEnum(ComboHkElusiveBrew, (int)BMSettingsH.Instance.ElusiveBrew);

            SetComboBoxEnum(ComboHkMode, (int)BMSettingsH.Instance.ModeSelection);
            SetComboBoxEnum(ComboHkModifier, (int)BMSettingsH.Instance.ModKeyChoice);
            SetComboBoxEnum(ComboHkCooldown, (int)BMSettingsH.Instance.CooldownKeyChoice);
            SetComboBoxEnum(ComboHkMultiTgt, (int)BMSettingsH.Instance.MultiTgtKeyChoice);
            SetComboBoxEnum(ComboHkPause, (int)BMSettingsH.Instance.PauseKeyChoice);

            if (File.Exists(Utilities.AssemblyDirectory + @"\Routines\Bubbleman\Interfaces\Images\Bubbleman.png"))
                LogoPicture.ImageLocation =
                    string.Format(@"{0}\Routines\Bubbleman\Interfaces\Images\Bubbleman.png",
                                  Utilities.AssemblyDirectory);
        }

        private void BMInterface_Load(object sender, EventArgs e)
        {
            GeneralGrid.SelectedObject = Settings.BMSettings.Instance.General;
            BMSettings BMSettings = BMSettings  .Instance;
            Styx.Helpers.Settings selectSpec = null;
            switch (StyxWoW.Me.Specialization)
            {
                case WoWSpec.PaladinRetribution:
                    selectSpec = BMSettings.Retribution;
                    break;

                case WoWSpec.PaladinProtection:
                    selectSpec = BMSettings.Protection;
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
     //   private void ComboHkAMZ_SelectedIndexChanged(object sender, EventArgs e)
     //   {
     //       BMSettingsH.Instance.AMZ = (Keys)GetComboBoxEnum(ComboHkAMZ);
   //     }
        private void ComboHkDizzyingHaze_SelectedIndexChanged(object sender, EventArgs e)
        {
            BMSettingsH.Instance.DizzyingHaze = (Keys)GetComboBoxEnum(ComboHkDizzyingHaze);
        }
 //       private void ComboHkTier6_SelectedIndexChanged(object sender, EventArgs e)
  //      {
  //          BMSettingsH.Instance.Tier6 = (Keys)GetComboBoxEnum(ComboHkTier6);
 //       }

        private void ComboHkElusiveBrew_SelectedIndexChanged(object sender, EventArgs e)
        {
            BMSettingsH.Instance.ElusiveBrew = (Keys)GetComboBoxEnum(ComboHkElusiveBrew);
        }

        private void ComboHkMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            BMSettingsH.Instance.ModeSelection = (BMEnum.Mode)GetComboBoxEnum(ComboHkMode);
        }
        private void ComboHkModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            BMSettingsH.Instance.ModKeyChoice = (ModifierKeys)GetComboBoxEnum(ComboHkModifier);
        }
        private void ComboHkCooldown_SelectedIndexChanged(object sender, EventArgs e)
        {
            BMSettingsH.Instance.CooldownKeyChoice = (Keys)GetComboBoxEnum(ComboHkCooldown);
        }
        private void ComboHkMultiTgt_SelectedIndexChanged(object sender, EventArgs e)
        {
            BMSettingsH.Instance.MultiTgtKeyChoice = (Keys)GetComboBoxEnum(ComboHkMultiTgt);
        }
        private void ComboHkPause_SelectedIndexChanged(object sender, EventArgs e)
        {
            BMSettingsH.Instance.PauseKeyChoice = (Keys)GetComboBoxEnum(ComboHkPause);
        }
        #endregion

        #region Buttons
        private void SaveButton_Click(object sender, EventArgs e)
        {
            BMHotKeyManager.RemoveAllKeys();
            BMHotKeyManager.RegisterKeys();
            BMSettings.Instance.Save();
            BMSettingsH.Instance.Save();
            ((Styx.Helpers.Settings)GeneralGrid.SelectedObject).Save();
            if (SpecGrid.SelectedObject != null)
            {
                ((Styx.Helpers.Settings)SpecGrid.SelectedObject).Save();
            }
            BMLogger.InitLogO("Settings for Bubbleman saved!");
            BMLogger.WriteToLogFile();
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

        private void ComboHkElusiveBrew_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            BMSettingsH.Instance.ElusiveBrew = (Keys)GetComboBoxEnum(ComboHkElusiveBrew);
        }
    }
}
