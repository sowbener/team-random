using System;
using System.Windows.Forms;

namespace AntiAFK.GUI
{
    public partial class AntiAfkGui : Form
    {
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
            AntiAfk.AfkLogging("[AntiAFK] Dialog Error: Combobox {0} does not have enum({1}) in list, defaulting to enum({2})",
                          cb.Name, e, item.E);
            cb.SelectedIndex = 0;
        }

        private static int GetComboBoxEnum(ComboBox cb)
        {
            var item = (CboItem)cb.Items[cb.SelectedIndex];
            return item.E;
        }
        #endregion

        public AntiAfkGui()
        {
            InitializeComponent();
            AntiAfkSettings.Instance.Load();

            keydropdown.Items.Add(new CboItem((int)Keys.Space, "Space (Jump)"));
            keydropdown.Items.Add(new CboItem((int)Keys.Up, "Forward (Up Arrow)"));
            keydropdown.Items.Add(new CboItem((int)Keys.Down, "Backward (Down Arrow)"));
            keydropdown.Items.Add(new CboItem((int)Keys.Left, "Left (Left Arrow)"));
            keydropdown.Items.Add(new CboItem((int)Keys.Right, "Right (Right Arrow)"));
            keydropdown.Items.Add(new CboItem((int)Keys.W, "Forward (W)"));
            keydropdown.Items.Add(new CboItem((int)Keys.S, "Backward (S)"));
            keydropdown.Items.Add(new CboItem((int)Keys.A, "Left (A)"));
            keydropdown.Items.Add(new CboItem((int)Keys.D, "Right (D)"));

            SetComboBoxEnum(keydropdown, (int)AntiAfkSettings.Instance.AntiAfkKey);
            msnumeric.Value = new decimal(AntiAfkSettings.Instance.AntiAfkTimeValue);
            varnumeric.Value = new decimal(AntiAfkSettings.Instance.AntiAfkRandomValue);
            pluginscheckbox.Checked = AntiAfkSettings.Instance.AntiAfkPlugins;
        }

        private void AntiAFKGui_Load(object sender, EventArgs e) { }

        private void keydropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            AntiAfkSettings.Instance.AntiAfkKey = (Keys)GetComboBoxEnum(keydropdown);
        }

        private void msnumeric_ValueChanged(object sender, EventArgs e)
        {
            AntiAfkSettings.Instance.AntiAfkTimeValue = (int)msnumeric.Value;
        }

        private void varnumeric_ValueChanged(object sender, EventArgs e)
        {
            AntiAfkSettings.Instance.AntiAfkRandomValue = (int)varnumeric.Value;
        }

        private void pluginscheckbox_CheckedChanged(object sender, EventArgs e)
        {
            AntiAfkSettings.Instance.AntiAfkPlugins = pluginscheckbox.Checked;
        }

        private void buttonsaveandclose_Click(object sender, EventArgs e)
        {
            AntiAfkSettings.Instance.Save();
            AntiAfk.PluginPulsing();
            AntiAfk.AfkLogging("[AntiAFK] Settings are saved!");
            Close();
        }
    }
}
