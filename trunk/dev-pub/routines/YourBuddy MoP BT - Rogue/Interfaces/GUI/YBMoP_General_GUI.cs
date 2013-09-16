using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YBMoP_BT_Rogue.Helpers;
using YBMoP_BT_Rogue.Interfaces.Settings;

namespace YBMoP_BT_Rogue.Interfaces.GUI
{
    public partial class YBGeneralGui : Form
    {
        public YBGeneralGui()
        {
            InitializeComponent();
        }

        #region Form Dragging API Support
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();
        #endregion

        #region GUI Load
        private void YBMoP_GUI_Load(object sender, EventArgs e)
        {
            YBSettingsG.Instance.Load();
            checkAdvancedLogging.Checked = YBSettingsG.Instance.CheckAdvancedLogging;
            checkTreePerformance.Checked = YBSettingsG.Instance.CheckTreePerformance;
            checkReshapeLifeInterrupts.Checked = YBSettingsG.Instance.CheckReshapeLifeInterrupts;
            checkHkChatOutput.Checked = YBSettingsG.Instance.EnableWoWChatOutput;
            checkClickToMove.Checked = YBSettingsG.Instance.CheckDisableClickToMove;
            numAdvLogThrottleTime.Value = new decimal(YBSettingsG.Instance.NumAdvLogThrottleTime);
        }
        #endregion

        #region Nummeric
        private void numAdvLogThrottleTime_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.NumAdvLogThrottleTime = (int)numAdvLogThrottleTime.Value;
        }
        #endregion

        #region MouseOvers
        private void YBGUI_Movement_MouseLeave(object sender, EventArgs e)
        {
            YBStatusStrip.Text = @"YourBuddy MoP BT - A Rogue raiding custom routine.";
        }
        private void checkTreePerformance_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Enables the TreePerformance Stopwatch. Only use for debugging!";
        }
        private void checkReshapeLifeInterrupts_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Enables the usage of ActionBar actions for Amber-Shaper Un'sohk.";
        }
        private void checkHkChatOutput_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Enables the chatoutput for the Hotkey System.";
        }
        private void checkClickToMove_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked lets routine disable click to move.";
        }
        private void checkAdvancedLogging_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables advanced logging.";
        }
        #endregion

        #region Checkboxes
        private void checkAdvancedLogging_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.CheckAdvancedLogging = checkAdvancedLogging.Checked;
        }
        private void checkTreePerformance_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.CheckTreePerformance = checkTreePerformance.Checked;
        }
        private void checkReshapeLifeInterrupts_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.CheckReshapeLifeInterrupts = checkReshapeLifeInterrupts.Checked;
        }
        private void checkHkChatOutput_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.EnableWoWChatOutput = checkHkChatOutput.Checked;
        }
        private void checkClickToMove_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.CheckDisableClickToMove = checkClickToMove.Checked;
        }
        #endregion

        #region MouseDowns
        private void YBGUI_Movement_MouseDown(object sender, MouseEventArgs e)
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

        #region SaveButton
        private void buttonSave_Click(object sender, EventArgs e)
        {
            YBSettingsG.Instance.Save();
            YBLogger.InitLogF("Advanced settings for YourBuddy MoP BT saved!");
            Close();
        }
        #endregion
    }
}