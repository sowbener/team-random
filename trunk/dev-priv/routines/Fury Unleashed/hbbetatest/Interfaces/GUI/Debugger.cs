using System;
using System.Linq;
using System.Windows.Forms;
using FuryUnleashed.Core;
using FuryUnleashed.Core.Utilities;
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
                myaurasdatagrid.DataSource = StyxWoW.Me.Auras.Values.OrderBy(a => a.Name).ToList();
            }
        }

        private void UpdateTargetAuras()
        {
            if (Unit.IsViable(StyxWoW.Me.CurrentTarget))
            {
                mytargetaurasdatagrid.DataSource = StyxWoW.Me.CurrentTarget.Auras.Values.OrderBy(a => a.Name).ToList();
            }
        }

        private void UpdateMyCachedAuras()
        {
            if (Unit.IsViable(StyxWoW.Me))
            {
                myaurasdatagrid.DataSource = StyxWoW.Me.Auras.Values.OrderBy(a => a.Name).ToList();
            }
        }

        private void UpdateTargetCachedAuras()
        {
            if (Unit.IsViable(StyxWoW.Me.CurrentTarget))
            {
                mytargetaurasdatagrid.DataSource = StyxWoW.Me.CurrentTarget.Auras.Values.OrderBy(a => a.Name).ToList();
            }
        }
        #endregion

        #region Errors
        private void myaurasdatagrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void mytargetaurasdatagrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
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

        private void mycachedaurasbutton_Click(object sender, EventArgs e)
        {
            UpdateMyCachedAuras();
        }

        private void targetcachedaurasbutton_Click(object sender, EventArgs e)
        {
            UpdateTargetCachedAuras();
        }

        private void myauraslogfilebutton_Click(object sender, EventArgs e)
        {
            Logger.WriteFile(myaurasdatagrid.DataSource.ToString());
        }

        private void mytargetauraslogfilebutton_Click(object sender, EventArgs e)
        {
            Logger.WriteFile(mytargetaurasdatagrid.DataSource.ToString());
        }
    }
}
