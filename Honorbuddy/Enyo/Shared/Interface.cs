using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;

namespace Enyo.Shared
{
    partial class Interface : Form
    {
        #region Form Dragging API Support
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();

        private void GuiDragDrop(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    ReleaseCapture();
                    SendMessage(Handle, 0xa1, 0x2, 0);
                    break;
            }
        }
        #endregion

        #region Combobox Item (CboItem)
        public class CboItem
        {
            public readonly int E;
            private readonly string _s;

            public CboItem(int pe, string ps)
            {
                E = pe;
                _s = ps;
            }

            public override string ToString()
            {
                return _s;
            }
        }
        #endregion

        #region Combobox ENUM (Required for ENUM in comboboxes)
        private static void SetComboBoxEnum(ComboBox cb, int e)
        {
            CboItem item;
            for (var i = 0; i < cb.Items.Count; i++)
            {
                item = (CboItem)cb.Items[i];
                if (item.E != e) continue;
                cb.SelectedIndex = i;
                return;
            }
            item = (CboItem)cb.Items[0];
            Logger.DiagnosticLog("Dialog Error: Combobox {0} does not have enum({1}) in list, defaulting to enum({2})", cb.Name, e, item.E);
            cb.SelectedIndex = 0;
        }
        private static int GetComboBoxEnum(ComboBox cb)
        {
            var item = (CboItem)cb.Items[cb.SelectedIndex];
            return item.E;
        }
        #endregion

        public Interface()
        {
            if (TreeRoot.IsRunning)
            {
                Logger.PrintLog("We strongly advise to stop Honorbuddy before changing FrameLock settings (Hardlock and Softlock).");
            }

            InitializeComponent();
            GlobalSettings.Instance.Load();
            BotSettings.Instance.Load();

            FillComboBoxes(PauseKeyComboBox);
            FillComboBoxes(ModifierKeyComboBox, true);

            SetComboBoxEnum(ModifierKeyComboBox, (int)BotSettings.Instance.ModKeyChoice);
            SetComboBoxEnum(PauseKeyComboBox, (int)BotSettings.Instance.PauseKeyChoice);

            ChatOutput.Checked = BotSettings.Instance.UseOverlayOutput;
            EnyoNavigator.Checked = BotSettings.Instance.UseEnyoNavigator;
            HardLock.Checked = GlobalSettings.Instance.UseFrameLock;
            ContinuesHealingMode.Checked = BotSettings.Instance.UseContinuesHealingMode;
            ForcedCombatMode.Checked = BotSettings.Instance.UseForcedCombatMode;
            Plugins.Checked = BotSettings.Instance.UsePluginPulsing;
            ClicktoMove.Checked = BotSettings.Instance.ClickToMove;
            SoftLock.Checked = BotSettings.Instance.UseSoftLock;

            HonorbuddyTps = CharacterSettings.Instance.TicksPerSecond;
            TicksPerSecondTrackBar.Value = CharacterSettings.Instance.TicksPerSecond;
        }

        private void FillComboBoxes(ComboBox combobox, bool special = false)
        {
            /* Fill ModifierKeyComboBox with keys. */
            if (combobox == ModifierKeyComboBox)
            {
                combobox.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Alt, "Alt - Mod"));
                combobox.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Control, "Ctrl - Mod"));
                combobox.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Shift, "Shift - Mod"));
                combobox.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Win, "Win - Mod"));
                combobox.Items.Add(new CboItem((int)ModifierKeys, "Disable HK"));
            }

            /* Stop if it's a special - Otherwise it will fill the keys below as well. */
            if (special) return;

            /* Fill all other boxes with keys. */
            combobox.Items.Add(new CboItem((int)Keys.None, "Modifier Only"));
            combobox.Items.Add(new CboItem((int)Keys.A, "A"));
            combobox.Items.Add(new CboItem((int)Keys.B, "B"));
            combobox.Items.Add(new CboItem((int)Keys.C, "C"));
            combobox.Items.Add(new CboItem((int)Keys.D, "D"));
            combobox.Items.Add(new CboItem((int)Keys.E, "E"));
            combobox.Items.Add(new CboItem((int)Keys.F, "F"));
            combobox.Items.Add(new CboItem((int)Keys.G, "G"));
            combobox.Items.Add(new CboItem((int)Keys.H, "H"));
            combobox.Items.Add(new CboItem((int)Keys.I, "I"));
            combobox.Items.Add(new CboItem((int)Keys.J, "J"));
            combobox.Items.Add(new CboItem((int)Keys.K, "K"));
            combobox.Items.Add(new CboItem((int)Keys.L, "L"));
            combobox.Items.Add(new CboItem((int)Keys.M, "M"));
            combobox.Items.Add(new CboItem((int)Keys.N, "N"));
            combobox.Items.Add(new CboItem((int)Keys.O, "O"));
            combobox.Items.Add(new CboItem((int)Keys.P, "P"));
            combobox.Items.Add(new CboItem((int)Keys.Q, "Q"));
            combobox.Items.Add(new CboItem((int)Keys.R, "R"));
            combobox.Items.Add(new CboItem((int)Keys.S, "S"));
            combobox.Items.Add(new CboItem((int)Keys.T, "T"));
            combobox.Items.Add(new CboItem((int)Keys.U, "U"));
            combobox.Items.Add(new CboItem((int)Keys.V, "V"));
            combobox.Items.Add(new CboItem((int)Keys.W, "W"));
            combobox.Items.Add(new CboItem((int)Keys.X, "X"));
            combobox.Items.Add(new CboItem((int)Keys.Y, "Y"));
            combobox.Items.Add(new CboItem((int)Keys.Z, "Z"));
            combobox.Items.Add(new CboItem((int)Keys.XButton1, "Mouse 4 (Works on some PC's)"));
            combobox.Items.Add(new CboItem((int)Keys.XButton2, "Mouse 5 (Works on some PC's)"));
            combobox.Items.Add(new CboItem((int)Keys.LControlKey, "Left Control"));
            combobox.Items.Add(new CboItem((int)Keys.RControlKey, "Right Control"));
            combobox.Items.Add(new CboItem((int)Keys.LShiftKey, "Left Shift"));
            combobox.Items.Add(new CboItem((int)Keys.RShiftKey, "Right Shift"));
            combobox.Items.Add(new CboItem((int)Keys.D1, "1 (no numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.D2, "2 (no numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.D3, "3 (no numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.D4, "4 (no numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.D5, "5 (no numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.D6, "6 (no numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.D7, "7 (no numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.D8, "8 (no numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.D9, "9 (no numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.D0, "0 (no numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.NumPad1, "1 (numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.NumPad2, "2 (numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.NumPad3, "3 (numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.NumPad4, "4 (numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.NumPad5, "5 (numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.NumPad6, "6 (numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.NumPad7, "7 (numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.NumPad8, "8 (numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.NumPad9, "9 (numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.NumPad0, "0 (numpad)"));
            combobox.Items.Add(new CboItem((int)Keys.F1, "F1"));
            combobox.Items.Add(new CboItem((int)Keys.F2, "F2"));
            combobox.Items.Add(new CboItem((int)Keys.F3, "F3"));
            combobox.Items.Add(new CboItem((int)Keys.F4, "F4"));
            combobox.Items.Add(new CboItem((int)Keys.F5, "F5"));
            combobox.Items.Add(new CboItem((int)Keys.F6, "F6"));
            combobox.Items.Add(new CboItem((int)Keys.F7, "F7"));
            combobox.Items.Add(new CboItem((int)Keys.F8, "F8"));
            combobox.Items.Add(new CboItem((int)Keys.F9, "F9"));
            combobox.Items.Add(new CboItem((int)Keys.F10, "F10"));
            combobox.Items.Add(new CboItem((int)Keys.F11, "F11"));
            combobox.Items.Add(new CboItem((int)Keys.F12, "F12"));
        }

        private void ChatOutput_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables local Chat output in WoW.");
        }

        private void ClickToMove_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables click to move in WoW.");
        }

        private void EnyoNavigator_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enforces Enyo's own navigation blocker.");
        }

        private void HardLock_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables HonorBuddy's HardLock (Framelock)");
        }

        private void SoftLock_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables Enyo's SoftLock (Framelock)");
        }

        private void ForceCombat_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables combat mode - Casts out of combat!");
        }

        private void ContinuesHealingMode_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables healing mode - Casts out of combat!");
        }

        private void Plugins_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables plugins in Enyo.");
        }

        private void SaveButton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Save the current settings and close the interface.");
        }

        private readonly int _var = CharacterSettings.Instance.TicksPerSecond;

        private int HonorbuddyTps
        {
            get
            {
                return (byte)TicksPerSecondTrackBar.Value;
            }
            set
            {
                Text = string.Format("Enyo now ticks with {0} Ticks per Second.", (byte)value);
                TpsLabel.Text = Text = string.Format("{0} Ticks per Second.", (byte)value);
                TicksPerSecondTrackBar.Value = (byte)value;
            }
        }

        private bool TicksPerSecondChanges
        {
            get { return HonorbuddyTps != _var; }
        }

        private void TicksPerSecondTrackBar_Scroll(object sender, EventArgs e)
        {
            CharacterSettings.Instance.TicksPerSecond = (byte)TicksPerSecondTrackBar.Value;
            HonorbuddyTps = (byte)TicksPerSecondTrackBar.Value;
        }

        private void HardLock_CheckedChanged(object sender, EventArgs e)
        {
            GlobalSettings.Instance.UseFrameLock = HardLock.Checked;
        }

        private void SoftLock_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.Instance.UseSoftLock = SoftLock.Checked;   
        }

        private void OverlayOutput_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.Instance.UseOverlayOutput = ChatOutput.Checked;
        }

        private void EnyoNavigator_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.Instance.UseEnyoNavigator = EnyoNavigator.Checked;
        }

        private void ContinuesHealingMode_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.Instance.UseContinuesHealingMode = ContinuesHealingMode.Checked;
        }

        private void ForcedCombatMode_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.Instance.UseForcedCombatMode = ForcedCombatMode.Checked;
        }

        private void Plugins_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.Instance.UsePluginPulsing = Plugins.Checked;
        }

        private void ClicktoMove_CheckedChanged(object sender, EventArgs e)
        {
            BotSettings.Instance.ClickToMove = ClicktoMove.Checked;
        }

        private void ModifierKeyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BotSettings.Instance.ModKeyChoice = (ModifierKeys)GetComboBoxEnum(ModifierKeyComboBox);
        }

        private void PauseKeyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BotSettings.Instance.PauseKeyChoice = (Keys)GetComboBoxEnum(PauseKeyComboBox);
        }

        private void CloseFormLogging()
        {
            Logger.PrintLog("------------------------------------------");
            Logger.PrintLog(BotSettings.Instance.UseOverlayOutput
                ? "ChatOutput enabled!"
                : "ChatOutput disabled!");
            Logger.PrintLog(BotSettings.Instance.UseContinuesHealingMode
                ? "Continues Healing Mode enabled!"
                : "Continues Healing Mode disabled!");
            Logger.PrintLog(GlobalSettings.Instance.UseFrameLock
                ? "HardLock (Framelock) enabled!"
                : "HardLock (Framelock) disabled!");
            Logger.PrintLog(BotSettings.Instance.UseSoftLock
                ? "SoftLock (Framelock) enabled!"
                : "SoftLock (Framelock) disabled!");
            Logger.PrintLog(BotSettings.Instance.UseForcedCombatMode
                ? "Force Rotation enabled!"
                : "Force Rotation disabled!");
            Logger.PrintLog(BotSettings.Instance.UsePluginPulsing
                ? "Plugins are enabled!"
                : "Plugins are disabled!");
            Logger.PrintLog(BotSettings.Instance.ClickToMove
                ? "Click to Move is enabled!"
                : "Click to Move is disabled!");
            Logger.PrintLog(BotSettings.Instance.UseEnyoNavigator
                ? "Block all movement from Routines and Plugins!"
                : "Allowing movement from Routines or Plugins!");

            Logger.PrintLog("{0} is the pause key, with {1} as modifier key.", BotSettings.Instance.PauseKeyChoice, BotSettings.Instance.ModKeyChoice);

            if (TicksPerSecondChanges)
            {
                Logger.PrintLog("New TPS at {0} saved!", HonorbuddyTps);
            }

            Logger.PrintLog("Interface saved and closed!");
            Logger.PrintLog("------------------------------------------");
        }

        private void SaveandCloseButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            GlobalSettings.Instance.Save();
            BotSettings.Instance.Save();

            Hotkeys.ReloadRegisterKeys();
            Logger.PrintInformation();

            Enyo.InitializePlugins();
            Enyo.InitializeNavigator();

            TreeRoot.TicksPerSecond = CharacterSettings.Instance.TicksPerSecond;
            Shared.ClicktoMove.ClickToMove(3000);

            CloseFormLogging();

            Close();
        }
    }
}
