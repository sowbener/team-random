using System;
using System.Linq;
using System.Windows.Forms;
using FuryUnleashed.Core;
using Styx;

namespace FuryUnleashed.Interfaces.GUI
{
    public partial class DebuggerGui : Form
    {
        public DebuggerGui()
        {
            InitializeComponent();
        }

        private void Debugger_Load(object sender, EventArgs e)
        {

        }

        #region Update Information Void's
        private void UpdateMyAuras()
        {
            if (Unit.IsViable(StyxWoW.Me))
            {
                dgwMyAuras.DataSource = StyxWoW.Me.Auras.Values.OrderBy(a => a.Name).ToList();
            }
        }

        private void UpdateTargetAuras()
        {
            if (Unit.IsViable(StyxWoW.Me.CurrentTarget))
            {
                dgwTargetAuras.DataSource = StyxWoW.Me.CurrentTarget.Auras.Values.OrderBy(a => a.Name).ToList();
            }
        }
        #endregion

        #region Errors
        private void dgwMyAuras_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
        #endregion

        private void myaurasbutton_Click(object sender, EventArgs e)
        {
            UpdateMyAuras();
        }

        private void targetaurasbutton_Click(object sender, EventArgs e)
        {
            UpdateTargetAuras();
        }
    }
}
