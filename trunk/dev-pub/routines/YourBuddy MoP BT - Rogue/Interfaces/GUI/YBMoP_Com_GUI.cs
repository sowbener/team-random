using Styx.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YBMoP_BT_Rogue.Helpers;
using YBMoP_BT_Rogue.Interfaces.Settings;
using YBMoP_BT_Rogue.Managers;

namespace YBMoP_BT_Rogue.Interfaces.GUI
{
    public partial class YBComGui : Form
    {
        public YBComGui()
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
            YBSettingsCom.Instance.Load();
            YBSettingsG.Instance.Load();

            #region ENUMs
            // ENUM - Berserker Rage
            comboAdrenalineRush.Items.Add(new CboItem((int)AbilityTrigger.Never, "Never"));
            comboAdrenalineRush.Items.Add(new CboItem((int)AbilityTrigger.OnBlTw, "On BL & TW"));
            comboAdrenalineRush.Items.Add(new CboItem((int)AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboAdrenalineRush.Items.Add(new CboItem((int)AbilityTrigger.Always, "Always"));

            // ENUM - Class Racials
            comboClassRacials.Items.Add(new CboItem((int)AbilityTrigger.Never, "Never"));
            comboClassRacials.Items.Add(new CboItem((int)AbilityTrigger.OnBlTw, "On BL & TW"));
            comboClassRacials.Items.Add(new CboItem((int)AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboClassRacials.Items.Add(new CboItem((int)AbilityTrigger.Always, "Always"));

            // ENUM - KillingSpree
            KillingSpree.Items.Add(new CboItem((int)AbilityTrigger.Never, "Never"));
            KillingSpree.Items.Add(new CboItem((int)AbilityTrigger.OnBlTw, "On BL & TW"));
            KillingSpree.Items.Add(new CboItem((int)AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            KillingSpree.Items.Add(new CboItem((int)AbilityTrigger.Always, "Always"));

            // ENUM - Skull Banner
            comboVanish.Items.Add(new CboItem((int)AbilityTrigger.Never, "Never"));
            comboVanish.Items.Add(new CboItem((int)AbilityTrigger.OnBlTw, "On BL & TW"));
            comboVanish.Items.Add(new CboItem((int)AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboVanish.Items.Add(new CboItem((int)AbilityTrigger.Always, "Always"));

            // ENUM - Skull Banner
            ShadowBlades.Items.Add(new CboItem((int)AbilityTrigger.Never, "Never"));
            ShadowBlades.Items.Add(new CboItem((int)AbilityTrigger.OnBlTw, "On BL & TW"));
            ShadowBlades.Items.Add(new CboItem((int)AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            ShadowBlades.Items.Add(new CboItem((int)AbilityTrigger.Always, "Always"));

            // ENUM - Tier 4
            comboPreperation.Items.Add(new CboItem((int)AbilityTrigger.Never, "Never"));
            comboPreperation.Items.Add(new CboItem((int)AbilityTrigger.OnBlTw, "On BL & TW"));
            comboPreperation.Items.Add(new CboItem((int)AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboPreperation.Items.Add(new CboItem((int)AbilityTrigger.Always, "Always"));

            // ENUM - Tier 6
            comboTier6.Items.Add(new CboItem((int)AbilityTrigger.Never, "Never"));
            comboTier6.Items.Add(new CboItem((int)AbilityTrigger.OnBlTw, "On BL & TW"));
            comboTier6.Items.Add(new CboItem((int)AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboTier6.Items.Add(new CboItem((int)AbilityTrigger.Always, "Always"));

            // ENUM - PoisonMainHand
            comboPoisonsM.Items.Add(new CboItem((int)PoisonM.Deadly, "Deadly Poison"));
            comboPoisonsM.Items.Add(new CboItem((int)PoisonM.Crippling, "Crippling Poison"));
            comboPoisonsM.Items.Add(new CboItem((int)PoisonM.Leeching, "Leeching Poison"));
            comboPoisonsM.Items.Add(new CboItem((int)PoisonM.Mindnumbing, "Mindnumbing Poison"));
            comboPoisonsM.Items.Add(new CboItem((int)PoisonM.Wound, "Wound Poison"));

            // ENUM- PoisonOffHand
            comboPoisonsO.Items.Add(new CboItem((int)PoisonO.Deadly, "Deadly Poison"));
            comboPoisonsO.Items.Add(new CboItem((int)PoisonO.Crippling, "Crippling Poison"));
            comboPoisonsO.Items.Add(new CboItem((int)PoisonO.Leeching, "Leeching Poison"));
            comboPoisonsO.Items.Add(new CboItem((int)PoisonO.Mindnumbing, "Mindnumbing Poison"));
            comboPoisonsO.Items.Add(new CboItem((int)PoisonO.Wound, "Wound Poison"));


            // ENUM - Use Hands
            comboUseHands.Items.Add(new CboItem((int)AbilityTrigger.Never, "Never"));
            comboUseHands.Items.Add(new CboItem((int)AbilityTrigger.OnBlTw, "On BL & TW"));
            comboUseHands.Items.Add(new CboItem((int)AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboUseHands.Items.Add(new CboItem((int)AbilityTrigger.Always, "Always"));

            // ENUM - Trinket 1
            comboTrinket1.Items.Add(new CboItem((int)AbilityTrigger.Never, "Never"));
            comboTrinket1.Items.Add(new CboItem((int)AbilityTrigger.OnBlTw, "On BL & TW"));
            comboTrinket1.Items.Add(new CboItem((int)AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboTrinket1.Items.Add(new CboItem((int)AbilityTrigger.Always, "Always"));

            // ENUM - Trinket 2
            comboTrinket2.Items.Add(new CboItem((int)AbilityTrigger.Never, "Never"));
            comboTrinket2.Items.Add(new CboItem((int)AbilityTrigger.OnBlTw, "On BL & TW"));
            comboTrinket2.Items.Add(new CboItem((int)AbilityTrigger.OnBossDummy, "On Boss & Dummy"));
            comboTrinket2.Items.Add(new CboItem((int)AbilityTrigger.Always, "Always"));

            // Hotkey Mode Choice
            comboHKMode.Items.Add(new CboItem((int)Mode.Auto, "Automatic Mode"));
            comboHKMode.Items.Add(new CboItem((int)Mode.Hotkey, "Hotkey Mode"));

            // Hotkey Modifier Key
            comboHKModifier.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Alt, "Alt - Mod"));
            comboHKModifier.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Control, "Ctrl - Mod"));
            comboHKModifier.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Shift, "Shift - Mod"));
            comboHKModifier.Items.Add(new CboItem((int)Styx.Common.ModifierKeys.Win, "Win - Mod"));


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

            SetComboBoxEnum(comboHKPause, (int)YBSettingsG.Instance.PauseKeyChoice);
            SetComboBoxEnum(comboHKSpecialKey, (int)YBSettingsG.Instance.SpecialKeyChoice);
            SetComboBoxEnum(comboHKAoE, (int)YBSettingsG.Instance.SwitchKeyChoice);
            SetComboBoxEnum(comboHKCooldown, (int)YBSettingsG.Instance.CooldownKeyChoice);
            SetComboBoxEnum(comboHKModifier, (int)YBSettingsG.Instance.ModKeyChoice);
            SetComboBoxEnum(comboHKMode, (int)YBSettingsG.Instance.ModeChoice);
            SetComboBoxEnum(comboAdrenalineRush, (int)YBSettingsCom.Instance.AdrenalineRush);
            SetComboBoxEnum(KillingSpree, (int)YBSettingsCom.Instance.KillingSpree);
            SetComboBoxEnum(ShadowBlades, (int)YBSettingsCom.Instance.ShadowBlades);
            SetComboBoxEnum(comboClassRacials, (int)YBSettingsCom.Instance.ClassRacials);
            SetComboBoxEnum(comboVanish, (int)YBSettingsCom.Instance.Vanish);
            SetComboBoxEnum(comboPreperation, (int)YBSettingsCom.Instance.Tier4Abilities);
            SetComboBoxEnum(comboTier6, (int)YBSettingsCom.Instance.Tier6Abilities);
            SetComboBoxEnum(comboPoisonsM, (int)YBSettingsCom.Instance.PoisonSelectorM);
            SetComboBoxEnum(comboPoisonsO, (int)YBSettingsCom.Instance.PoisonSelectorO);
            SetComboBoxEnum(comboUseHands, (int)YBSettingsCom.Instance.UseHands);
            SetComboBoxEnum(comboTrinket1, (int)YBSettingsCom.Instance.Trinket1);
            SetComboBoxEnum(comboTrinket2, (int)YBSettingsCom.Instance.Trinket2);

            checkAutoAttack.Checked = YBSettingsCom.Instance.CheckAutoAttack;
            checkAoE.Checked = YBSettingsCom.Instance.CheckAoE;
            checkShadowstep.Checked = YBSettingsCom.Instance.CheckShadowstep;
            checkRecuperate.Checked = YBSettingsCom.Instance.CheckRecuperate;
            Rupture.Checked = YBSettingsCom.Instance.CheckRupture;
            checkRecuperateCombo.Checked = YBSettingsCom.Instance.CheckRecuperateCombo;
            checkHealthStone.Checked = YBSettingsCom.Instance.CheckHealthStone;
            checkPoison.Checked = YBSettingsCom.Instance.CheckPoison;
            checkInterrupts.Checked = YBSettingsCom.Instance.CheckInterrupts;
            checkLifeblood.Checked = YBSettingsCom.Instance.CheckLifeblood;
            checkPotion.Checked = YBSettingsCom.Instance.CheckPotion;
            checkExposeArmor.Checked = YBSettingsCom.Instance.CheckExposeArmor;

            numShadowstep.Value = new decimal(YBSettingsCom.Instance.NumShadowstep);
            numRecuperate.Value = new decimal(YBSettingsCom.Instance.NumRecuperate);
            numRecuperateCombo.Value = new decimal(YBSettingsCom.Instance.NumRecuperateCombo);
            numHealthStone.Value = new decimal(YBSettingsCom.Instance.NumHealthStone);
            numLifeblood.Value = new decimal(YBSettingsCom.Instance.NumLifeblood);

            if (File.Exists(Utilities.AssemblyDirectory + @"\Routines\YourBuddy MoP BT - Rogue\Interfaces\Images\YBMoP-Rogue-Logo.png"))
                RogueCrest.ImageLocation =
                    string.Format(@"{0}\Routines\YourBuddy MoP BT - Rogue\Interfaces\Images\YBMoP-Rogue-Logo.png",
                                  Utilities.AssemblyDirectory);
        }
        #endregion

        #region YBMoP BT - ENUM ComboBoxes
        private void comboHKMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsG.Instance.ModeChoice = (Mode)GetComboBoxEnum(comboHKMode);
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


        private void comboAdrenalineRush_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.AdrenalineRush = (AbilityTrigger)GetComboBoxEnum(comboAdrenalineRush);
        }

        private void comboClassRacials_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.ClassRacials = (AbilityTrigger)GetComboBoxEnum(comboClassRacials);
        }

        private void comboShadowBlades_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.ShadowBlades = (AbilityTrigger)GetComboBoxEnum(ShadowBlades);
        }

        private void KillingSpree_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.KillingSpree = (AbilityTrigger)GetComboBoxEnum(KillingSpree);
        }

        private void comboVanish_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.Vanish = (AbilityTrigger)GetComboBoxEnum(comboVanish);
        }

        private void comboPoisonsM_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.PoisonSelectorM = (PoisonM)GetComboBoxEnum(comboPoisonsM);
        }

        private void comboPoisonsO_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.PoisonSelectorO = (PoisonO)GetComboBoxEnum(comboPoisonsO);
        }

        private void comboPreperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.Tier4Abilities = (AbilityTrigger)GetComboBoxEnum(comboPreperation);
        }

        private void comboTier6_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.Tier6Abilities = (AbilityTrigger)GetComboBoxEnum(comboTier6);
        }


        private void comboUseHands_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.UseHands = (AbilityTrigger)GetComboBoxEnum(comboUseHands);
        }

        private void comboTrinket1_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.Trinket1 = (AbilityTrigger)GetComboBoxEnum(comboTrinket1);
        }

        private void comboTrinket2_SelectedIndexChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.Trinket2 = (AbilityTrigger)GetComboBoxEnum(comboTrinket2);
        }
        #endregion

        #region LinkLabels
        private void YBMoPForumTopic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.thebuddyforum.com/honorbuddy-forum/combat-routines/Rogue/80375-dps-fury-yourbuddy-mop.html");
            YBMoPForumTopic.LinkVisited = true;
        }
        #endregion

        #region CheckBoxes
        private void checkAutoAttack_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckAutoAttack = checkAutoAttack.Checked;
        }
        private void checkAoE_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckAoE = checkAoE.Checked;
        }
        private void checkShadowstep_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckShadowstep = checkShadowstep.Checked;
        }
        private void checkRecuperate_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckRecuperate = checkRecuperate.Checked;
        }

        private void checkRecuperateCombo_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckRecuperateCombo = checkRecuperateCombo.Checked;
        }

        private void checkHealthStone_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckHealthStone = checkHealthStone.Checked;
        }
        private void checkPoison_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckPoison = checkPoison.Checked;
        }
        private void checkInterrupts_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckInterrupts = checkInterrupts.Checked;
        }
        private void checkLifeblood_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckLifeblood = checkLifeblood.Checked;
        }
        private void checkPotion_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckPotion = checkPotion.Checked;
        }
        private void Rupture_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckRupture = Rupture.Checked;
        }
        private void checkExposeArmor_CheckedChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.CheckExposeArmor = checkExposeArmor.Checked;
        }
        #endregion

        #region NummericBoxes
        private void numShadowstep_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.NumShadowstep = (int)numShadowstep.Value;
        }
        private void numRecuperate_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.NumRecuperate = (int)numRecuperate.Value;
        }

        private void numRecuperateCombo_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.NumRecuperateCombo = (int)numRecuperateCombo.Value;
        }

        private void numHealthStone_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.NumHealthStone = (int)numHealthStone.Value;
        }
        private void numLifeblood_ValueChanged(object sender, EventArgs e)
        {
            YBSettingsCom.Instance.NumLifeblood = (int)numLifeblood.Value;
        }
        #endregion

        #region MouseOvers
        private void YBGUI_Movement_MouseLeave(object sender, EventArgs e)
        {
            YBStatusStrip.Text = @"YourBuddy MoP BT - A Rogue raiding custom routine.";
        }

        private void YBGUI_Movement_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Click and hold to drag the YourBuddy MoP User Interface.";
        }

        private void YBMoPForumTopic_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Opens the YourBuddy MoP forum topic.";
        }

        private void comboAdrenalineRush_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of AdrenalineRush.";
        }

        private void comboClassRacials_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Class Racials.";
        }

        private void KillingSpree_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Killing Spree";
        }

        private void ShadowBlades_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Shadow Blades";
        }

        private void comboVanish_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Vanish.";
        }

        private void comboPreperation_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Preperation";
        }

        private void comboTier6_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Marked For Death,";
        }

        private void comboPoisons_MouseMove(object sender, MouseEventArgs e)
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

        private void checkRecuperate_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Checked enables Die by the Sword usage.";
        }

        private void numRecuperate_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the use-on HP for Die by the Sword usage.";
        }

        private void checkShadowstep_MouseMove(object sender, MouseEventArgs e)
        {
            YBStatusStrip.Text = @"Select the usage of Demoralizing Banner.";
        }

        private void numShadowstep_MouseMove(object sender, MouseEventArgs e)
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

        private void checkPotion_MouseMove(object sender, MouseEventArgs e)
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

        private void checkPoison_MouseMove(object sender, MouseEventArgs e)
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

        private void checkExposeArmor_MouseMove(object sender, MouseEventArgs e)
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

        private void comboHKShadowstep_MouseMove(object sender, MouseEventArgs e)
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
            YBSettingsCom.Instance.Save();
            YBSettingsG.Instance.Save();
            YBLogger.InitLogF("Fury settings for YourBuddy MoP BT saved!");
            YBLogger.InitLogF("Hotkey settings for YourBuddy MoP BT saved!");
            Close();
        }
        #endregion

        private void LogoText2_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lShouts_Click(object sender, EventArgs e)
        {

        }

        private void lKillingSpree_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

    }
}