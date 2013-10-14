using System;
using System.Windows.Forms;

namespace AntiAfk.GUI
{
    public partial class AntiAfkGui : Form
    {
        /* Requirements for Combobox Enumerations */
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

        public AntiAfkGui()
        {
            InitializeComponent();
            AntiAfkSettings.Instance.Load();

            keydropdown.Items.Add(new CboItem((int)Keys.Space, "Space"));
            keydropdown.Items.Add(new CboItem((int)Keys.Enter, "Enter/Return"));

            SetComboBoxEnum(keydropdown, (int)AntiAfkSettings.Instance.AntiAfkKey);
            ginfocheckbox.Checked = AntiAfkSettings.Instance.AntiAfkGinfo;
            msnumeric.Value = new decimal(AntiAfkSettings.Instance.AntiAfkTime);
        }

        private void keydropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            AntiAfkSettings.Instance.AntiAfkKey = (Keys)GetComboBoxEnum(keydropdown);
        }

        private void ginfocheckbox_CheckedChanged(object sender, EventArgs e)
        {
            AntiAfkSettings.Instance.AntiAfkGinfo = ginfocheckbox.Checked;
        }

        private void msnumeric_ValueChanged(object sender, EventArgs e)
        {
            AntiAfkSettings.Instance.AntiAfkTime = (int)msnumeric.Value;
        }

        private void savebutton_Click(object sender, EventArgs e)
        {
            AntiAfkSettings.Instance.Save();
            AntiAfk.AfkLogging("[AntiAFK] Settings are saved!");
            Close();
        }
    }
}
