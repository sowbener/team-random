using Styx.Common;
using Styx.CommonBot;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;

namespace Tyrael.Shared
{
    public partial class TyraelInterface : Form
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

        #region CBOItem
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

        #region Combobox ENUM - Required for ENUM in comboboxes
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
            Logging.Write("Dialog Error: Combobox {0} does not have enum({1}) in list, defaulting to enum({2})",
                          cb.Name, e, item.E);
            cb.SelectedIndex = 0;
        }
        private static int GetComboBoxEnum(ComboBox cb)
        {
            var item = (CboItem)cb.Items[cb.SelectedIndex];
            return item.E;
        }
        #endregion

        public TyraelInterface()
        {
            InitializeComponent();
            TyraelSettings.Instance.Load();

            comboModifierKey.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Alt, "Alt - Mod"));
            comboModifierKey.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Control, "Ctrl - Mod"));
            comboModifierKey.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Shift, "Shift - Mod"));
            comboModifierKey.Items.Add(new CboItem((int)ModifierKeys, "Disable HK"));
            SetComboBoxEnum(comboModifierKey, (int)TyraelSettings.Instance.ModKeyChoice);

            comboPauseKey.Items.Add(new CboItem((int)Keys.XButton1, "Mouse button 4"));
            comboPauseKey.Items.Add(new CboItem((int)Keys.XButton2, "Mouse button 5"));
            comboPauseKey.Items.Add(new CboItem((int)Keys.D1, "1 (no numpad)"));
            comboPauseKey.Items.Add(new CboItem((int)Keys.D2, "2 (no numpad)"));
            comboPauseKey.Items.Add(new CboItem((int)Keys.D3, "3 (no numpad)"));
            comboPauseKey.Items.Add(new CboItem((int)Keys.Q, "Q"));
            comboPauseKey.Items.Add(new CboItem((int)Keys.E, "E"));
            comboPauseKey.Items.Add(new CboItem((int)Keys.R, "R"));
            comboPauseKey.Items.Add(new CboItem((int)Keys.Z, "Z"));
            comboPauseKey.Items.Add(new CboItem((int)Keys.X, "X"));
            comboPauseKey.Items.Add(new CboItem((int)Keys.C, "C"));
            SetComboBoxEnum(comboPauseKey, (int)TyraelSettings.Instance.PauseKeyChoice);

            TPSTrackBar.Value = TyraelSettings.Instance.HonorbuddyTps;
            HonorbuddyTps = TyraelSettings.Instance.HonorbuddyTps;

            checkFrameLock.Checked = TyraelSettings.Instance.FrameLock == TyraelHelper.LockState.True;

            checkChatOutput.Checked = TyraelSettings.Instance.ChatOutput;
            checkHealingMode.Checked = TyraelSettings.Instance.HealingMode;
            checkPlugins.Checked = TyraelSettings.Instance.PluginPulsing;
        }

        private void checkChatOutput_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables chat output in WoW.");
        }

        private void checkFrameLock_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables framelock - Read the Tyrael FAQ for more info!");
        }

        private void checkHealingMode_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("We recommend you to use the Pause Hotkeys when this is enabled.");
        }

        private void checkPlugins_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables plugins in Tyrael.");
        }

        private readonly int _var = TyraelSettings.Instance.HonorbuddyTps;

        private int HonorbuddyTps
        {
            get
            {
                return TPSTrackBar.Value;
            }
            set
            {
                Text = string.Format("Tyrael now ticks with {0} Ticks per Second.", value);
                TpsLabel.Text = Text = string.Format("{0} Ticks per Second.", value);
                TPSTrackBar.Value = value;
            }
        }

        private bool TpsChanges
        {
            get { return HonorbuddyTps != _var; }
        }

        private void TPSTrackBar_Scroll(object sender, EventArgs e)
        {
            TyraelSettings.Instance.HonorbuddyTps = TPSTrackBar.Value;
            HonorbuddyTps = TPSTrackBar.Value;
        }

        private void checkFrameLock_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.FrameLock = checkFrameLock.Checked ? TyraelHelper.LockState.True : TyraelHelper.LockState.False;
        }

        private void checkChatOutput_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.ChatOutput = checkChatOutput.Checked;
        }

        private void checkHealingMode_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.HealingMode = checkHealingMode.Checked;
        }

        private void checkPlugins_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.PluginPulsing = checkPlugins.Checked;
        }

        private void comboModifierKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.ModKeyChoice = (ModifierKeys)GetComboBoxEnum(comboModifierKey);
        }

        private void comboPauseKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.PauseKeyChoice = (Keys)GetComboBoxEnum(comboPauseKey);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            TyraelSettings.Instance.Save();
            TreeRoot.TicksPerSecond = (byte)TyraelSettings.Instance.HonorbuddyTps;
            TyraelHotkeyManager.RemoveAllKeys();
            TyraelHotkeyManager.RegisterHotKeys();
            Tyrael.PluginPulsing();
            if (TpsChanges)
                Logging.Write(Colors.DodgerBlue, "[Tyrael] New TPS at {0} saved!", HonorbuddyTps);
            if (TyraelSettings.Instance.ChatOutput)
                Logging.Write(Colors.DodgerBlue, "[Tyrael] ChatOutput enabled!");
            if (TyraelSettings.Instance.FrameLock == TyraelHelper.LockState.True)
                Logging.Write(Colors.DodgerBlue, "[Tyrael] FrameLock enabled!");
            if (TyraelSettings.Instance.HealingMode)
                Logging.Write(Colors.DodgerBlue, "[Tyrael] HealingMode enabled!");
            Logging.Write(Colors.DodgerBlue, "[Tyrael] Interface saved and closed!");
            Close();
        }
    }
}
