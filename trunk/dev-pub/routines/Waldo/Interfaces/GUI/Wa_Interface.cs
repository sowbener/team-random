using Waldo.Helpers;
using Waldo.Interfaces.Settings;
using Waldo.Managers;
using Styx;
using Styx.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Waldo.Interfaces.GUI
{
    public partial class WaInterface : Form
    {
        /* Form Dragging Support */
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();

        private void Wa_GuiDragDrop(object sender, MouseEventArgs e)
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
            WaEnum.WaCboItem item;
            for (var i = 0; i < cb.Items.Count; i++)
            {
                item = (WaEnum.WaCboItem)cb.Items[i];
                if (item.E != e) continue;
                cb.SelectedIndex = i;
                return;
            }
            item = (WaEnum.WaCboItem)cb.Items[0];
            WaLogger.DiagLogW("Dialog Error: Combobox {0} does not have enum({1}) in list, defaulting to enum({2})",
                          cb.Name, e, item.E);
            cb.SelectedIndex = 0;
        }

        private static int GetComboBoxEnum(ComboBox cb)
        {
            var item = (WaEnum.WaCboItem)cb.Items[cb.SelectedIndex];
            return item.E;
        }

        public WaInterface()
        {
            InitializeComponent();
            WaSettings.Instance.Load();
            WaSettingsH.Instance.Load();

            // Hotkey Mode Choice
            ComboHkMode.Items.Add(new WaEnum.WaCboItem((int)WaEnum.Mode.Auto, "Automatic Mode"));
            ComboHkMode.Items.Add(new WaEnum.WaCboItem((int)WaEnum.Mode.Hotkey, "Hotkey Mode"));

            // Hotkey Modifier Key
            ComboHkModifier.Items.Add(new WaEnum.WaCboItem((int)Styx.Common.ModifierKeys.Alt, "Alt - Mod"));
            ComboHkModifier.Items.Add(new WaEnum.WaCboItem((int)Styx.Common.ModifierKeys.Control, "Ctrl - Mod"));
            ComboHkModifier.Items.Add(new WaEnum.WaCboItem((int)Styx.Common.ModifierKeys.Shift, "Shift - Mod"));
            ComboHkModifier.Items.Add(new WaEnum.WaCboItem((int)Styx.Common.ModifierKeys.Win, "Win - Mod"));

            // Hotkey - Cooldowns
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.XButton1, "Mouse button 4"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.XButton2, "Mouse button 5"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F1, "F1"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F2, "F2"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F3, "F3"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F4, "F4"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F5, "F5"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F6, "F6"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F7, "F7"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F8, "F8"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F9, "F9"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F10, "F10"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F11, "F11"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F12, "F12"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.Q, "Q"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.E, "E"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.P, "P"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.F, "F"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.R, "R"));
            ComboHkCooldown.Items.Add(new WaEnum.WaCboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - AoE
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.XButton1, "Mouse button 4"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.XButton2, "Mouse button 5"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F1, "F1"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F2, "F2"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F3, "F3"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F4, "F4"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F5, "F5"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F6, "F6"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F7, "F7"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F8, "F8"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F9, "F9"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F10, "F10"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F11, "F11"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F12, "F12"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.Q, "Q"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.E, "E"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.P, "P"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.F, "F"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.R, "R"));
            ComboHkMultiTgt.Items.Add(new WaEnum.WaCboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - SpecialKey
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.XButton1, "Mouse button 4"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.XButton2, "Mouse button 5"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F1, "F1"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F2, "F2"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F3, "F3"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F4, "F4"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F5, "F5"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F6, "F6"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F7, "F7"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F8, "F8"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F9, "F9"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F10, "F10"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F11, "F11"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F12, "F12"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.Q, "Q"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.E, "E"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.P, "P"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.F, "F"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.R, "R"));
            ComboHkSpecialKey.Items.Add(new WaEnum.WaCboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - TricksKey
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.XButton1, "Mouse button 4"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.XButton2, "Mouse button 5"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F1, "F1"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F2, "F2"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F3, "F3"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F4, "F4"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F5, "F5"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F6, "F6"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F7, "F7"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F8, "F8"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F9, "F9"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F10, "F10"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F11, "F11"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F12, "F12"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.Q, "Q"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.E, "E"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.P, "P"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.F, "F"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.R, "R"));
            ComboHkTricks.Items.Add(new WaEnum.WaCboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - Pause
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.XButton1, "Mouse button 4"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.XButton2, "Mouse button 5"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.D1, "1 (no numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.D2, "2 (no numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.D3, "3 (no numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.D4, "4 (no numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.D5, "5 (no numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.D6, "6 (no numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.D7, "7 (no numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.D8, "8 (no numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.D9, "9 (no numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.D0, "0 (no numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad1, "1 (numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad2, "2 (numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad3, "3 (numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad4, "4 (numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad5, "5 (numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad6, "6 (numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad7, "7 (numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad8, "8 (numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad9, "9 (numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.NumPad0, "0 (numpad)"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F1, "F1"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F2, "F2"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F3, "F3"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F4, "F4"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F5, "F5"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F6, "F6"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F7, "F7"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F8, "F8"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F9, "F9"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F10, "F10"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F11, "F11"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F12, "F12"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.Q, "Q"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.E, "E"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.P, "P"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.F, "F"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.R, "R"));
            ComboHkPause.Items.Add(new WaEnum.WaCboItem((int)Keys.None, "No Hotkey"));

            SetComboBoxEnum(ComboHkMode, (int)WaSettingsH.Instance.ModeSelection);
            SetComboBoxEnum(ComboHkTricks, (int)WaSettingsH.Instance.Tricks);
            SetComboBoxEnum(ComboHkModifier, (int)WaSettingsH.Instance.ModKeyChoice);
            SetComboBoxEnum(ComboHkCooldown, (int)WaSettingsH.Instance.CooldownKeyChoice);
            SetComboBoxEnum(ComboHkMultiTgt, (int)WaSettingsH.Instance.MultiTgtKeyChoice);
            SetComboBoxEnum(ComboHkSpecialKey, (int)WaSettingsH.Instance.SpecialKeyChoice);
            SetComboBoxEnum(ComboHkPause, (int)WaSettingsH.Instance.PauseKeyChoice);

            if (File.Exists(Utilities.AssemblyDirectory + @"\Routines\Waldo\Interfaces\Images\Waldo.jpg"))
                LogoPicture.ImageLocation =
                    string.Format(@"{0}\Routines\Waldo\Interfaces\Images\Waldo.jpg",
                                  Utilities.AssemblyDirectory);
        }

        private void WaInterface_Load(object sender, EventArgs e)
        {
            GeneralGrid.SelectedObject = Settings.WaSettings.Instance.General;
            WaSettings WaSettings = WaSettings  .Instance;
            Styx.Helpers.Settings selectSpec = null;
            switch (StyxWoW.Me.Specialization)
            {
                case WoWSpec.RogueAssassination:
                    selectSpec = WaSettings.Assassination;
                    break;

                case WoWSpec.RogueCombat:
                    selectSpec = WaSettings.Combat;
                    break;

                case WoWSpec.RogueSubtlety:
                    selectSpec = WaSettings.Subtlety;
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
        private void ComboHkMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            WaSettingsH.Instance.ModeSelection = (WaEnum.Mode)GetComboBoxEnum(ComboHkMode);
        }
        private void ComboHkModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            WaSettingsH.Instance.ModKeyChoice = (ModifierKeys)GetComboBoxEnum(ComboHkModifier);
        }
        private void ComboHkCooldown_SelectedIndexChanged(object sender, EventArgs e)
        {
            WaSettingsH.Instance.CooldownKeyChoice = (Keys)GetComboBoxEnum(ComboHkCooldown);
        }
        private void ComboHkMultiTgt_SelectedIndexChanged(object sender, EventArgs e)
        {
            WaSettingsH.Instance.MultiTgtKeyChoice = (Keys)GetComboBoxEnum(ComboHkMultiTgt);
        }
        private void ComboHkTricks_SelectedIndexChanged(object sender, EventArgs e)
        {
            WaSettingsH.Instance.Tricks = (Keys)GetComboBoxEnum(ComboHkTricks);
        }
        private void ComboHkPause_SelectedIndexChanged(object sender, EventArgs e)
        {
            WaSettingsH.Instance.PauseKeyChoice = (Keys)GetComboBoxEnum(ComboHkPause);
        }
        private void ComboHkSpecialKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            WaSettingsH.Instance.SpecialKeyChoice = (Keys)GetComboBoxEnum(ComboHkSpecialKey);
        }
        #endregion

        #region Buttons
        private void SaveButton_Click(object sender, EventArgs e)
        {
            WaHotKeyManager.RemoveAllKeys();
            WaHotKeyManager.RegisterKeys();
            WaSettings.Instance.Save();
            WaSettingsH.Instance.Save();
            ((Styx.Helpers.Settings)GeneralGrid.SelectedObject).Save();
            if (SpecGrid.SelectedObject != null)
            {
                ((Styx.Helpers.Settings)SpecGrid.SelectedObject).Save();
            }
            WaLogger.InitLogO("Settings for Waldo saved!");
            WaLogger.WriteToLogFile();
            Close();
        }
        #endregion

        private void LogoPicture_Click(object sender, EventArgs e)
        {

        }

        private void SpecGrid_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
