using Styx.Common;
using Styx.CommonBot;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;
using Styx.Helpers;

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
            GlobalSettings.Instance.Load();
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

            TPSTrackBar.Value = GlobalSettings.Instance.TicksPerSecond;
            HonorbuddyTps = GlobalSettings.Instance.TicksPerSecond;

            checkFrameLock.Checked = GlobalSettings.Instance.UseFrameLock;

            checkChatOutput.Checked = TyraelSettings.Instance.ChatOutput;
            checkClicktoMove.Checked = TyraelSettings.Instance.ClickToMove;
            checkHealingMode.Checked = TyraelSettings.Instance.HealingMode;
            checkMinify.Checked = TyraelSettings.Instance.Minify == TyraelUtilities.Minify.True;
            checkPlugins.Checked = TyraelSettings.Instance.PluginPulsing;
        }

        private void checkChatOutput_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables chat output in WoW.");
        }

        private void checkClicktoMove_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables click to move in WoW.");
        }

        private void checkFrameLock_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables framelock - Read the Tyrael FAQ for more info!");
        }

        private void checkHealingMode_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Use the Pause Hotkeys when this is enabled!");
        }

        private void checkPlugins_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables plugins in Tyrael.");
        }

        private void checkMinify_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Minify reduces the stress on the PC when executing a routine.");
        }

        private readonly int _var = GlobalSettings.Instance.TicksPerSecond;

        private int HonorbuddyTps
        {
            get
            {
                return (byte)TPSTrackBar.Value;
            }
            set
            {
                Text = string.Format("Tyrael now ticks with {0} Ticks per Second.", (byte)value);
                TpsLabel.Text = Text = string.Format("{0} Ticks per Second.", (byte)value);
                TPSTrackBar.Value = (byte)value;
            }
        }

        private bool TpsChanges
        {
            get { return HonorbuddyTps != _var; }
        }

        private void TPSTrackBar_Scroll(object sender, EventArgs e)
        {
            GlobalSettings.Instance.TicksPerSecond = (byte)TPSTrackBar.Value;
            HonorbuddyTps = (byte)TPSTrackBar.Value;
        }

        private void checkFrameLock_CheckedChanged(object sender, EventArgs e)
        {
            GlobalSettings.Instance.UseFrameLock = checkFrameLock.Checked;
        }

        private void checkChatOutput_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.ChatOutput = checkChatOutput.Checked;
        }

        private void checkClicktoMove_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.ClickToMove = checkClicktoMove.Checked;
        }

        private void checkHealingMode_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.HealingMode = checkHealingMode.Checked;
        }

        private void checkPlugins_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.PluginPulsing = checkPlugins.Checked;
        }

        private void checkMinify_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.Minify = checkMinify.Checked ? TyraelUtilities.Minify.True : TyraelUtilities.Minify.False;
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
            GlobalSettings.Instance.Save();
            TyraelSettings.Instance.Save();
            TyraelUtilities.ClickToMove();
            TyraelUtilities.ReRegisterHotkeys();
            Tyrael.PluginPulsing();

            TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;

            Logging.Write(Colors.White, "------------------------------------------");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.ChatOutput 
                    ? "[Tyrael] ChatOutput enabled!" 
                    : "[Tyrael] ChatOutput disabled!");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.ClickToMove
                    ? "[Tyrael] Click to Move enabled!"
                    : "[Tyrael] Click to Move disabled!");
            Logging.Write(Colors.DodgerBlue,
                GlobalSettings.Instance.UseFrameLock
                    ? "[Tyrael] FrameLock enabled!"
                    : "[Tyrael] FrameLock disabled!");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.HealingMode
                    ? "[Tyrael] Continues Healing mode enabled!"
                    : "[Tyrael] Continues Healing mode disabled!");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.Minify == TyraelUtilities.Minify.True
                    ? "[Tyrael] Minify Performance Enhancer is enabled!"
                    : "[Tyrael] Minify Performance Enhancer is disabled!");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.PluginPulsing
                    ? "[Tyrael] Plugins are enabled!"
                    : "[Tyrael] Plugins are disabled!");

            Logging.Write(Colors.DodgerBlue, 
                    "[Tyrael] {0} is the pause key, with {1} as modifier key.", TyraelSettings.Instance.PauseKeyChoice, TyraelSettings.Instance.ModKeyChoice);

            if (TpsChanges)
            Logging.Write(Colors.DodgerBlue,
                    "[Tyrael] New TPS at {0} saved!", HonorbuddyTps);

            Logging.Write(Colors.DodgerBlue, 
                    "[Tyrael] Interface saved and closed!");
            Logging.Write(Colors.White, "------------------------------------------");
            Close();
        }
    }
}
