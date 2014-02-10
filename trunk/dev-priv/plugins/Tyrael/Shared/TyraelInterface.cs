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

            comboPauseKey.Items.Add(new CboItem((int)Keys.None, "Modifier Only"));
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

            checkAutomaticUpdater.Checked = TyraelSettings.Instance.CheckAutoUpdate;
            checkChatOutput.Checked = TyraelSettings.Instance.CheckChatOutput;
            checkClicktoMove.Checked = TyraelSettings.Instance.CheckClickToMove;
            CheckHardLock.Checked = GlobalSettings.Instance.UseFrameLock;
            checkHealingMode.Checked = TyraelSettings.Instance.CheckHealingMode;
            checkPlugins.Checked = TyraelSettings.Instance.CheckPluginPulsing;
            checkSoftLock.Checked = TyraelSettings.Instance.UseSoftLock;

            HonorbuddyTps = GlobalSettings.Instance.TicksPerSecond;
            TPSTrackBar.Value = GlobalSettings.Instance.TicksPerSecond;
        }

        private void TyraelForumTopicLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.thebuddyforum.com/honorbuddy-forum/botbases/102004-bot-tyrael-raiding-botbase.html");
        }

        private void checkAutomaticUpdater_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables the automatic updater.");
        }

        private void checkChatOutput_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables chat output in WoW.");
        }

        private void checkClicktoMove_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables click to move in WoW.");
        }

        private void CheckHardLock_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables HonorBuddy's HardLock (Framelock)");
        }

        private void checkSoftLock_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables Tyrael's SoftLock (Framelock)");
        }

        private void checkHealingMode_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables healing mode - Casts out of combat!");
        }

        private void checkPlugins_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Enables plugins in Tyrael.");
        }

        private void slowbutton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("15 TPS - Hardlock disabled - Softlock disabled.");
        }

        private void normalbutton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("30 TPS - Hardlock disabled - Softlock enabled.");
        }

        private void fastbutton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("60 TPS - Hardlock disabled - Softlock enabled.");
        }

        private void extremebutton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("90 TPS - Hardlock enabled - Softlock disabled.");
        }

        private void SaveButton_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Save the current settings and close the interface.");
        }

        private void TyraelForumTopicLabel_MouseMove(object sender, MouseEventArgs e)
        {
            TpsLabel.Text = Text = string.Format("Opens the Tyrael Topic in your default browser.");
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

        private void checkAutomaticUpdater_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.CheckAutoUpdate = checkAutomaticUpdater.Checked;
        }

        private void checkHardLock_CheckedChanged(object sender, EventArgs e)
        {
            GlobalSettings.Instance.UseFrameLock = CheckHardLock.Checked;
        }

        private void checkSoftLock_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.UseSoftLock = checkSoftLock.Checked;
        }

        private void checkChatOutput_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.CheckChatOutput = checkChatOutput.Checked;
        }

        private void checkClicktoMove_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.CheckClickToMove = checkClicktoMove.Checked;
        }

        private void checkHealingMode_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.CheckHealingMode = checkHealingMode.Checked;
        }

        private void checkPlugins_CheckedChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.CheckPluginPulsing = checkPlugins.Checked;
        }

        private void comboModifierKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.ModKeyChoice = (ModifierKeys)GetComboBoxEnum(comboModifierKey);
        }

        private void comboPauseKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            TyraelSettings.Instance.PauseKeyChoice = (Keys)GetComboBoxEnum(comboPauseKey);
        }

        private void ButtonLogging()
        {
            Logging.Write(Colors.White, "------------------------------------------");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.CheckAutoUpdate
                    ? "[Tyrael] Automatic Updater is enabled!"
                    : "[Tyrael] Automatic Updater is disabled!");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.CheckChatOutput
                    ? "[Tyrael] ChatOutput enabled!"
                    : "[Tyrael] ChatOutput disabled!");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.CheckClickToMove
                    ? "[Tyrael] Click to Move enabled!"
                    : "[Tyrael] Click to Move disabled!");
            Logging.Write(Colors.DodgerBlue,
                GlobalSettings.Instance.UseFrameLock
                    ? "[Tyrael] HardLock (Framelock) enabled!"
                    : "[Tyrael] HardLock (Framelock) disabled!");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.UseSoftLock
                    ? "[Tyrael] SoftLock (Framelock) enabled!"
                    : "[Tyrael] SoftLock (Framelock) disabled!");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.CheckHealingMode
                    ? "[Tyrael] Continues Healing mode enabled!"
                    : "[Tyrael] Continues Healing mode disabled!");
            Logging.Write(Colors.DodgerBlue,
                TyraelSettings.Instance.CheckPluginPulsing
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
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            GlobalSettings.Instance.Save();
            TyraelSettings.Instance.Save();

            TyraelUtilities.ClickToMove();
            TyraelUtilities.ReRegisterHotkeys();
            Tyrael.InitializePlugins();

            TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;

            ButtonLogging();

            Close();
        }

        private void slowbutton_Click(object sender, EventArgs e)
        {
            TPSTrackBar.Value = 15;

            GlobalSettings.Instance.TicksPerSecond = (byte)TPSTrackBar.Value;
            GlobalSettings.Instance.UseFrameLock = false;
            TyraelSettings.Instance.UseSoftLock = false;

            GlobalSettings.Instance.Save();
            TyraelSettings.Instance.Save();

            TyraelUtilities.ClickToMove();
            TyraelUtilities.ReRegisterHotkeys();
            Tyrael.InitializePlugins();

            TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;

            ButtonLogging();

            Close();
        }

        private void normalbutton_Click(object sender, EventArgs e)
        {
            TPSTrackBar.Value = 30;

            GlobalSettings.Instance.TicksPerSecond = (byte)TPSTrackBar.Value;
            GlobalSettings.Instance.UseFrameLock = false;
            TyraelSettings.Instance.UseSoftLock = true;

            GlobalSettings.Instance.Save();
            TyraelSettings.Instance.Save();

            TyraelUtilities.ClickToMove();
            TyraelUtilities.ReRegisterHotkeys();
            Tyrael.InitializePlugins();

            TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;

            ButtonLogging();

            Close();
        }

        private void fastbutton_Click(object sender, EventArgs e)
        {
            TPSTrackBar.Value = 60;

            GlobalSettings.Instance.TicksPerSecond = (byte)TPSTrackBar.Value;
            GlobalSettings.Instance.UseFrameLock = false;
            TyraelSettings.Instance.UseSoftLock = true;

            GlobalSettings.Instance.Save();
            TyraelSettings.Instance.Save();

            TyraelUtilities.ClickToMove();
            TyraelUtilities.ReRegisterHotkeys();
            Tyrael.InitializePlugins();

            TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;

            ButtonLogging();

            Close();
        }

        private void extremebutton_Click(object sender, EventArgs e)
        {
            TPSTrackBar.Value = 90;

            GlobalSettings.Instance.TicksPerSecond = (byte)TPSTrackBar.Value;
            GlobalSettings.Instance.UseFrameLock = true;
            TyraelSettings.Instance.UseSoftLock = false;

            GlobalSettings.Instance.Save();
            TyraelSettings.Instance.Save();

            TyraelUtilities.ClickToMove();
            TyraelUtilities.ReRegisterHotkeys();
            Tyrael.InitializePlugins();

            TreeRoot.TicksPerSecond = GlobalSettings.Instance.TicksPerSecond;

            ButtonLogging();

            Close();
        }
    }
}
