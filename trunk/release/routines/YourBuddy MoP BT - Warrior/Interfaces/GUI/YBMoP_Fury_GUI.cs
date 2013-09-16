using Styx.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YBMoP_BT_Warrior.Helpers;
using YBMoP_BT_Warrior.Interfaces.Settings;
using YBMoP_BT_Warrior.Managers;

namespace YBMoP_BT_Warrior.Interfaces.GUI
{
    public partial class YBFuryGui : Form
    {
        public YBFuryGui()
        {
            InitializeComponent();
        }

        #region Form Dragging API Support
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();
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
            YBLogger.DiagLogW("Dialog Error: Combobox {0} does not have enum({1}) in list, defaulting to enum({2})",
                          cb.Name, e, item.E);
            cb.SelectedIndex = 0;
        }
        private static int GetComboBoxEnum(ComboBox cb)
        {
            var item = (CboItem)cb.Items[cb.SelectedIndex];
            return item.E;
        }
        #endregion

        #region GUI Load
        private void YBMoP_GUI_Load(object sender, EventArgs e)
        {
            YBSettingsF.Instance.Load();
            YBSettingsG.Instance.Load();

            #region ENUMs
            // ENUM - Berserker Rage
            comboBerserkerRage.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Never, "Never"));
            comboBerserkerRage.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBlTwHr, "On BL/Hero/TW"));
            comboBerserkerRage.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboBerserkerRage.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Always, "Always"));

            // ENUM - Class Racials
            comboClassRacials.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Never, "Never"));
            comboClassRacials.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBlTwHr, "On BL/Hero/TW"));
            comboClassRacials.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboClassRacials.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Always, "Always"));

            // ENUM - Recklessness
            comboRecklessness.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Never, "Never"));
            comboRecklessness.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBlTwHr, "On BL/Hero/TW"));
            comboRecklessness.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboRecklessness.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Always, "Always"));

            // ENUM - Skull Banner
            comboSkullBanner.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Never, "Never"));
            comboSkullBanner.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBlTwHr, "On BL/Hero/TW"));
            comboSkullBanner.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboSkullBanner.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Always, "Always"));

            // ENUM - Tier 4
            comboTier4.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Never, "Never"));
            comboTier4.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBlTwHr, "On BL/Hero/TW"));
            comboTier4.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboTier4.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Always, "Always"));

            // ENUM - Tier 6
            comboTier6.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Never, "Never"));
            comboTier6.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBlTwHr, "On BL/Hero/TW"));
            comboTier6.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboTier6.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Always, "Always"));

            // ENUM - Shouts
            comboShouts.Items.Add(new CboItem((int)YBEnum.Shouts.Battle, "Battle Shout"));
            comboShouts.Items.Add(new CboItem((int)YBEnum.Shouts.Commanding, "Commanding Shout"));

            // ENUM - Use Hands
            comboUseHands.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Never, "Never"));
            comboUseHands.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBlTwHr, "On BL/Hero/TW"));
            comboUseHands.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboUseHands.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Always, "Always"));

            // ENUM - Trinket 1
            comboTrinket1.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Never, "Never"));
            comboTrinket1.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBlTwHr, "On BL/Hero/TW"));
            comboTrinket1.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboTrinket1.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Always, "Always"));

            // ENUM - Trinket 2
            comboTrinket2.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Never, "Never"));
            comboTrinket2.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBlTwHr, "On BL/Hero/TW"));
            comboTrinket2.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboTrinket2.Items.Add(new CboItem((int)YBEnum.AbilityTrigger.Always, "Always"));

            // Hotkey Mode Choice
            comboHKMode.Items.Add(new CboItem((int)YBEnum.Mode.Auto, "Automatic Mode"));
            comboHKMode.Items.Add(new CboItem((int)YBEnum.Mode.Hotkey, "Hotkey Mode"));

            // Hotkey Modifier Key
            comboHKModifier.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Alt, "Alt - Mod"));
            comboHKModifier.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Control, "Ctrl - Mod"));
            comboHKModifier.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Shift, "Shift - Mod"));
            comboHKModifier.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Win, "Win - Mod"));

            // Hotkey - Demoralizing Banner
            comboHKDemoBanner.Items.Add(new CboItem((int)Keys.None, "No Hotkey"));
            comboHKDemoBanner.Items.Add(new CboItem((int)Keys.Q, "Q"));
            comboHKDemoBanner.Items.Add(new CboItem((int)Keys.E, "E"));
            comboHKDemoBanner.Items.Add(new CboItem((int)Keys.LControlKey, "Left Control"));
            comboHKDemoBanner.Items.Add(new CboItem((int)Keys.RControlKey, "Right Control"));
            comboHKDemoBanner.Items.Add(new CboItem((int)Keys.LShiftKey, "Left Shift"));
            comboHKDemoBanner.Items.Add(new CboItem((int)Keys.RShiftKey, "Right Shift"));
            comboHKDemoBanner.Items.Add(new CboItem((int)Keys.XButton1, "Mouse button 4"));
            comboHKDemoBanner.Items.Add(new CboItem((int)Keys.XButton2, "Mouse button 5"));

            // Hotkey - Heroic Leap
            comboHKLeap.Items.Add(new CboItem((int)Keys.None, "No Hotkey"));
            comboHKLeap.Items.Add(new CboItem((int)Keys.Q, "Q"));
            comboHKLeap.Items.Add(new CboItem((int)Keys.E, "E"));
            comboHKLeap.Items.Add(new CboItem((int)Keys.LControlKey, "Left Control"));
            comboHKLeap.Items.Add(new CboItem((int)Keys.RControlKey, "Right Control"));
            comboHKLeap.Items.Add(new CboItem((int)Keys.LShiftKey, "Left Shift"));
            comboHKLeap.Items.Add(new CboItem((int)Keys.RShiftKey, "Right Shift"));
            comboHKLeap.Items.Add(new CboItem((int)Keys.XButton1, "Mouse button 4"));
            comboHKLeap.Items.Add(new CboItem((int)Keys.XButton2, "Mouse button 5"));

            // Hotkey - Cooldowns
            comboHKCooldown.Items.Add(new CboItem((int)Keys.XButton1, "Mouse button 4"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.XButton2, "Mouse button 5"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.D1, "1 (no numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.D2, "2 (no numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.D3, "3 (no numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.D4, "4 (no numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.D5, "5 (no numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.D6, "6 (no numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.D7, "7 (no numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.D8, "8 (no numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.D9, "9 (no numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.D0, "0 (no numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.NumPad1, "1 (numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.NumPad2, "2 (numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.NumPad3, "3 (numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.NumPad4, "4 (numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.NumPad5, "5 (numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.NumPad6, "6 (numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.NumPad7, "7 (numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.NumPad8, "8 (numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.NumPad9, "9 (numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.NumPad0, "0 (numpad)"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F1, "F1"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F2, "F2"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F3, "F3"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F4, "F4"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F5, "F5"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F6, "F6"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F7, "F7"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F8, "F8"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F9, "F9"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F10, "F10"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F11, "F11"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F12, "F12"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.Q, "Q"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.E, "E"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.P, "P"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.F, "F"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.R, "R"));
            comboHKCooldown.Items.Add(new CboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - AoE
            comboHKAoE.Items.Add(new CboItem((int)Keys.XButton1, "Mouse button 4"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.XButton2, "Mouse button 5"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.D1, "1 (no numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.D2, "2 (no numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.D3, "3 (no numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.D4, "4 (no numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.D5, "5 (no numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.D6, "6 (no numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.D7, "7 (no numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.D8, "8 (no numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.D9, "9 (no numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.D0, "0 (no numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.NumPad1, "1 (numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.NumPad2, "2 (numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.NumPad3, "3 (numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.NumPad4, "4 (numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.NumPad5, "5 (numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.NumPad6, "6 (numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.NumPad7, "7 (numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.NumPad8, "8 (numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.NumPad9, "9 (numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.NumPad0, "0 (numpad)"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F1, "F1"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F2, "F2"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F3, "F3"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F4, "F4"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F5, "F5"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F6, "F6"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F7, "F7"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F8, "F8"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F9, "F9"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F10, "F10"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F11, "F11"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F12, "F12"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.Q, "Q"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.E, "E"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.P, "P"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.F, "F"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.R, "R"));
            comboHKAoE.Items.Add(new CboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - Special Key
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.XButton1, "Mouse button 4"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.XButton2, "Mouse button 5"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.D1, "1 (no numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.D2, "2 (no numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.D3, "3 (no numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.D4, "4 (no numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.D5, "5 (no numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.D6, "6 (no numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.D7, "7 (no numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.D8, "8 (no numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.D9, "9 (no numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.D0, "0 (no numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.NumPad1, "1 (numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.NumPad2, "2 (numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.NumPad3, "3 (numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.NumPad4, "4 (numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.NumPad5, "5 (numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.NumPad6, "6 (numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.NumPad7, "7 (numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.NumPad8, "8 (numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.NumPad9, "9 (numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.NumPad0, "0 (numpad)"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F1, "F1"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F2, "F2"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F3, "F3"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F4, "F4"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F5, "F5"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F6, "F6"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F7, "F7"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F8, "F8"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F9, "F9"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F10, "F10"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F11, "F11"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F12, "F12"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.Q, "Q"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.E, "E"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.P, "P"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.F, "F"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.R, "R"));
            comboHKSpecialKey.Items.Add(new CboItem((int)Keys.None, "No Hotkey"));

            // Hotkey - Pause
            comboHKPause.Items.Add(new CboItem((int)Keys.XButton1, "Mouse button 4"));
            comboHKPause.Items.Add(new CboItem((int)Keys.XButton2, "Mouse button 5"));
            comboHKPause.Items.Add(new CboItem((int)Keys.D1, "1 (no numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.D2, "2 (no numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.D3, "3 (no numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.D4, "4 (no numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.D5, "5 (no numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.D6, "6 (no numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.D7, "7 (no numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.D8, "8 (no numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.D9, "9 (no numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.D0, "0 (no numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.NumPad1, "1 (numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.NumPad2, "2 (numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.NumPad3, "3 (numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.NumPad4, "4 (numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.NumPad5, "5 (numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.NumPad6, "6 (numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.NumPad7, "7 (numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.NumPad8, "8 (numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.NumPad9, "9 (numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.NumPad0, "0 (numpad)"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F1, "F1"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F2, "F2"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F3, "F3"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F4, "F4"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F5, "F5"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F6, "F6"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F7, "F7"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F8, "F8"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F9, "F9"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F10, "F10"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F11, "F11"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F12, "F12"));
            comboHKPause.Items.Add(new CboItem((int)Keys.Q, "Q"));
            comboHKPause.Items.Add(new CboItem((int)Keys.E, "E"));
            comboHKPause.Items.Add(new CboItem((int)Keys.P, "P"));
            comboHKPause.Items.Add(new CboItem((int)Keys.F, "F"));
            comboHKPause.Items.Add(new CboItem((int)Keys.R, "R"));
            comboHKPause.Items.Add(new CboItem((int)Keys.None, "No Hotkey"));
            #endregion

            SetComboBoxEnum(comboHKDemoBanner, (int)YBSettingsG.Instance.DemoBannerChoice);
            SetComboBoxEnum(comboHKLeap, (int)YBSettingsG.Instance.HeroicLeapChoice);
            SetComboBoxEnum(comboHKPause, (int)YBSettingsG.Instance.PauseKeyChoice);
            SetComboBoxEnum(comboHKSpecialKey, (int)YBSettingsG.Instance.SpecialKeyChoice);
            SetComboBoxEnum(comboHKAoE, (int)YBSettingsG.Instance.SwitchKeyChoice);
            SetComboBoxEnum(comboHKCooldown, (int)YBSettingsG.Instance.CooldownKeyChoice);
            SetComboBoxEnum(comboHKModifier, (int)YBSettingsG.Instance.ModKeyChoice);
            SetComboBoxEnum(comboHKMode, (int)YBSettingsG.Instance.ModeChoice);
            SetComboBoxEnum(comboBerserkerRage, (int)YBSettingsF.Instance.BerserkerRage);
            SetComboBoxEnum(comboClassRacials, (int)YBSettingsF.Instance.ClassRacials);
            SetComboBoxEnum(comboRecklessness, (int)YBSettingsF.Instance.Recklessness);
            SetComboBoxEnum(comboSkullBanner, (int)YBSettingsF.Instance.SkullBanner);
            SetComboBoxEnum(comboTier4, (int)YBSettingsF.Instance.Tier4Abilities);
            SetComboBoxEnum(comboTier6, (int)YBSettingsF.Instance.Tier6Abilities);
            SetComboBoxEnum(comboShouts, (int)YBSettingsF.Instance.ShoutSelector);
            SetComboBoxEnum(comboUseHands, (int)YBSettingsF.Instance.UseHands);
            SetComboBoxEnum(comboTrinket1, (int)YBSettingsF.Instance.Trinket1);
            SetComboBoxEnum(comboTrinket2, (int)YBSettingsF.Instance.Trinket2);

            checkAutoAttack.Checked = YBSettingsF.Instance.CheckAutoAttack;
            checkAoE.Checked = YBSettingsF.Instance.CheckAoE;
            checkDemoBanner.Checked = YBSettingsF.Instance.CheckDemoBanner;
            checkDiebytheSword.Checked = YBSettingsF.Instance.CheckDiebytheSword;
            checkEnragedRegen.Checked = YBSettingsF.Instance.CheckEnragedRegen;
            checkHealthStone.Checked = YBSettingsF.Instance.CheckHealthStone;
            checkHeroicThrow.Checked = YBSettingsF.Instance.CheckHeroicThrow;
            checkImpVic.Checked = YBSettingsF.Instance.CheckImpVic;
            checkInterrupts.Checked = YBSettingsF.Instance.CheckInterrupts;
            checkLifeblood.Checked = YBSettingsF.Instance.CheckLifeblood;
            checkRallyingCry.Checked = YBSettingsF.Instance.CheckRallyingCry;
            checkRotImpVic.Checked = YBSettingsF.Instance.CheckRotImpVic;
            checkShatteringThrow.Checked = YBSettingsF.Instance.CheckShatteringThrow;

            numDemoBanner.Value = new decimal(YBSettingsF.Instance.NumDemoBanner);
            numDiebytheSword.Value = new decimal(YBSettingsF.Instance.NumDiebytheSword);
            numEnragedRegen.Value = new decimal(YBSettingsF.Instance.NumEnragedRegen);
            numHealthStone.Value = new decimal(YBSettingsF.Instance.NumHealthStone);
            numImpVic.Value = new decimal(YBSettingsF.Instance.NumImpVic);
            numLifeblood.Value = new decimal(YBSettingsF.Instance.NumLifeblood);
            numRallyingCry.Value = new decimal(YBSettingsF.Instance.NumRallyingCry);

            if (File.Exists(Utilities.AssemblyDirectory + @"\Routines\YourBuddy MoP BT - Warrior\Interfaces\Images\YBMoP-Warrior-Logo.png"))
                WarriorCrest.ImageLocation =
                    string.Format(@"{0}\Routines\YourBuddy MoP BT - Warrior\Interfaces\Images\YBMoP-Warrior-Logo.png",
                                  Utilities.AssemblyDirectory);
        }
        #endregion

        #region YBMoP BT - ENUM ComboBoxes
        private void comboHKMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.ModeChoice = (YBEnum.Mode)GetComboBoxEnum(comboHKMode);
            HotKeyManager.RemoveAllKeys();
            HotKeyManager.RegisterKeys();
        }

        private void comboHKModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.ModKeyChoice = (ModifierKeys)GetComboBoxEnum(comboHKModifier);
            HotKeyManager.RemoveAllKeys();
            HotKeyManager.RegisterKeys();
        }

        private void comboHKCooldown_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.CooldownKeyChoice = (Keys)GetComboBoxEnum(comboHKCooldown);
            HotKeyManager.RemoveAllKeys();
            HotKeyManager.RegisterKeys();
        }

        private void comboHKAoE_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.SwitchKeyChoice = (Keys)GetComboBoxEnum(comboHKAoE);
            HotKeyManager.RemoveAllKeys();
            HotKeyManager.RegisterKeys();
        }

        private void comboHKSpecialKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.SpecialKeyChoice = (Keys)GetComboBoxEnum(comboHKSpecialKey);
            HotKeyManager.RemoveAllKeys();
            HotKeyManager.RegisterKeys();
        }

        private void comboHKPause_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.PauseKeyChoice = (Keys)GetComboBoxEnum(comboHKPause);
            HotKeyManager.RemoveAllKeys();
            HotKeyManager.RegisterKeys();
        }

        private void comboHKDemoBanner_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.DemoBannerChoice = (Keys)GetComboBoxEnum(comboHKDemoBanner);
        }

        private void comboHKLeap_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.HeroicLeapChoice = (Keys)GetComboBoxEnum(comboHKLeap);
        }

        private void comboBerserkerRage_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.BerserkerRage = (YBEnum.AbilityTrigger)GetComboBoxEnum(comboBerserkerRage);
        }

        private void comboClassRacials_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.ClassRacials = (YBEnum.AbilityTrigger)GetComboBoxEnum(comboClassRacials);
        }

        private void comboRecklessness_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.Recklessness = (YBEnum.AbilityTrigger)GetComboBoxEnum(comboRecklessness);
        }

        private void comboSkullBanner_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.SkullBanner = (YBEnum.AbilityTrigger)GetComboBoxEnum(comboSkullBanner);
        }

        private void comboTier4_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.Tier4Abilities = (YBEnum.AbilityTrigger)GetComboBoxEnum(comboTier4);
        }

        private void comboTier6_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.Tier6Abilities = (YBEnum.AbilityTrigger)GetComboBoxEnum(comboTier6);
        }

        private void comboShouts_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.ShoutSelector = (YBEnum.Shouts)GetComboBoxEnum(comboShouts);
        }

        private void comboUseHands_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.UseHands = (YBEnum.AbilityTrigger)GetComboBoxEnum(comboUseHands);
        }

        private void comboTrinket1_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.Trinket1 = (YBEnum.AbilityTrigger)GetComboBoxEnum(comboTrinket1);
        }

        private void comboTrinket2_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.Trinket2 = (YBEnum.AbilityTrigger)GetComboBoxEnum(comboTrinket2);
        }
        #endregion

        #region LinkLabels
        private void YBMoPForumTopic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.thebuddyforum.com/honorbuddy-forum/combat-routines/warrior/80375-dps-fury-yourbuddy-mop.html");
            YBMoPForumTopic.LinkVisited = true;
        }
        #endregion

        #region CheckBoxes
        private void checkAutoAttack_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckAutoAttack = checkAutoAttack.Checked;
        }
        private void checkAoE_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckAoE = checkAoE.Checked;
        }
        private void checkDemoBanner_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckDemoBanner = checkDemoBanner.Checked;
        }
        private void checkDiebytheSword_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckDiebytheSword = checkDiebytheSword.Checked;
        }
        private void checkEnragedRegen_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckEnragedRegen = checkEnragedRegen.Checked;
        }
        private void checkHealthStone_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckHealthStone = checkHealthStone.Checked;
        }
        private void checkHeroicThrow_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckHeroicThrow = checkHeroicThrow.Checked;
        }
        private void checkImpVic_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckImpVic = checkImpVic.Checked;
        }
        private void checkInterrupts_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckInterrupts = checkInterrupts.Checked;
        }
        private void checkLifeblood_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckLifeblood = checkLifeblood.Checked;
        }
        private void checkRallyingCry_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckRallyingCry = checkRallyingCry.Checked;
        }
        private void checkRotImpVic_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckRotImpVic = checkRotImpVic.Checked;
        }
        private void checkShatteringThrow_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.CheckShatteringThrow = checkShatteringThrow.Checked;
        }
        #endregion

        #region NummericBoxes
        private void numDemoBanner_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.NumDemoBanner = (int)numDemoBanner.Value;
        }
        private void numDiebytheSword_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.NumDiebytheSword = (int)numDiebytheSword.Value;
        }
        private void numEnragedRegen_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.NumEnragedRegen = (int)numEnragedRegen.Value;
        }
        private void numHealthStone_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.NumHealthStone = (int)numHealthStone.Value;
        }
        private void numImpVic_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.NumImpVic = (int)numImpVic.Value;
        }
        private void numLifeblood_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.NumLifeblood = (int)numLifeblood.Value;
        }
        private void numRallyingCry_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsF.Instance.NumRallyingCry = (int)numRallyingCry.Value;
        }
        #endregion

        #region MouseOvers
        private void YBGUI_Movement_MouseLeave(object sender, EventArgs e)
        {
            YBStatusStrip.Text = @"YourBuddy MoP BT - A warrior raiding custom routine.";
        }

        private void YBGUI_Movement_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Click and hold to drag the YourBuddy MoP User Interface.";
        }

        private void YBMoPForumTopic_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Opens the YourBuddy MoP forum topic.";
        }

        private void comboBerserkerRage_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Berserker Rage - Trigger Enrage effect.";
        }

        private void comboClassRacials_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Class Racials.";
        }

        private void comboRecklessness_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Recklessness.";
        }

        private void comboSkullBanner_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Skull Banner.";
        }

        private void comboTier4_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Tier 4 - Usage of Bladestorm, Dragon Roar or Shockwave.";
        }

        private void comboTier6_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Tier 6 - Usage of Avatar, Bloodbath or Storm Bolt.";
        }

        private void comboShouts_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the preferred shout (Battle or Commanding).";
        }

        private void comboUseHands_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Hand Enchants.";
        }

        private void comboTrinket1_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of trinket in slot 1.";
        }

        private void comboTrinket2_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of trinket in slot 2.";
        }

        private void checkDiebytheSword_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables Die by the Sword usage.";
        }

        private void numDiebytheSword_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the use-on HP for Die by the Sword usage.";
        }

        private void checkDemoBanner_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Demoralizing Banner.";
        }

        private void numDemoBanner_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the use-on HP for Demoralizing Banner usage.";
        }

        private void checkEnragedRegen_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables Enraged Regeneration usage.";
        }

        private void numEnragedRegen_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the use-on HP for Enraged Regentation usage.";
        }

        private void checkHealthStone_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables Healthstone usage.";
        }

        private void numHealthStone_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the use-on HP for Healthstone usage.";
        }

        private void checkImpVic_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables Impending Victory.";
        }

        private void numImpVic_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the use-on HP for Impending Victory usage.";
        }

        private void checkRallyingCry_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables Rallying Cry.";
        }

        private void numRallyingCry_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the use-on HP for Rallying Cry usage.";
        }

        private void checkRotImpVic_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enabled Impending Victory as a rotational filler - Ignoring HP settings!";
        }

        private void checkLifeblood_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables Lifeblood.";
        }

        private void numLifeblood_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the use-on HP for Lifeblood usage.";
        }

        private void checkHeroicThrow_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables Heroic Throw.";
        }

        private void checkInterrupts_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables interrupts.";
        }

        private void checkAutoAttack_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Enables autoattack, easy for TABBING. Careful; Very persistent in targetting!";
        }

        private void checkAoE_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables Multi-Target combat.";
        }

        private void checkShatteringThrow_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables Shattering Throw.";
        }

        private void comboHKMode_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the hotkey mode - Hotkey = Use Hotkeys / Auto = No Hotkeys.";
        }

        private void comboHKCooldown_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the preferred key as cooldown hotkey.";
        }

        private void comboHKAoE_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the preferred key as Multi-Target hotkey (AoE).";
        }

        private void comboHKSpecialKey_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the preferred key as special hotkey.";
        }

        private void comboHKModifier_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the preferred modifier for the hotkeys.";
        }

        private void comboHKPause_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the preferred key to pause the combat routine.";
        }

        private void comboHKDemoBanner_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the preferred key for Demoralizing Banner.";
        }

        private void comboHKLeap_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the preferred key for Heroic Leap.";
        }

        private void buttonSave_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Save the settings.";
        }

        private void buttonAdvanced_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Opens the advanced options GUI.";
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

        #region Buttons
        private void buttonAdvanced_Click(object sender, EventArgs e)
        {
            new YBGeneralGui().ShowDialog();
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            YBSettingsF.Instance.Save();
            YBSettingsG.Instance.Save();
            YBLogger.InitLogO("Fury settings for YourBuddy MoP BT saved!");
            YBLogger.InitLogO("Hotkey settings for YourBuddy MoP BT saved!");
            YBLogger.WriteToLogFile();
            Close();
        }
        #endregion
    }
}