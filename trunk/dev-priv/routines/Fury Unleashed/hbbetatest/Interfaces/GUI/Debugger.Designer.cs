namespace FuryUnleashed.Interfaces.GUI
{
    partial class DebuggerGui
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.debuggertabcontrol = new System.Windows.Forms.TabControl();
            this.myaurastab = new System.Windows.Forms.TabPage();
            this.myaurasdatagrid = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.spellIdDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.spellDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.applyAuraTypeDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.creatorGuidDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flagsDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.durationDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endTimeDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeLeftDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stackCountDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.levelDataGridViewTextBoxColumnMyAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ısHarmfulDataGridViewCheckBoxColumnMyAuras = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ısActiveDataGridViewCheckBoxColumnMyAuras = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ısPassiveDataGridViewCheckBoxColumnMyAuras = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cancellableDataGridViewCheckBoxColumnMyAuras = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.myaurasbutton = new System.Windows.Forms.Button();
            this.targetaurastab = new System.Windows.Forms.TabPage();
            this.mytargetaurasdatagrid = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.spellIdDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.spellDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.applyAuraTypeDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.creatorGuidDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flagsDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.durationDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endTimeDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeLeftDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stackCountDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.levelDataGridViewTextBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ısActiveDataGridViewCheckBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ısPassiveDataGridViewCheckBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cancellableDataGridViewCheckBoxColumnMyTargetAuras = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.targetaurasbutton = new System.Windows.Forms.Button();
            this.mycachedaurastab = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.mycachedaurasbutton = new System.Windows.Forms.Button();
            this.targetcachedaurastab = new System.Windows.Forms.TabPage();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.targetcachedaurasbutton = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.myauraslogfilebutton = new System.Windows.Forms.Button();
            this.mytargetauraslogfilebutton = new System.Windows.Forms.Button();
            this.debuggertabcontrol.SuspendLayout();
            this.myaurastab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.myaurasdatagrid)).BeginInit();
            this.targetaurastab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mytargetaurasdatagrid)).BeginInit();
            this.mycachedaurastab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.targetcachedaurastab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // debuggertabcontrol
            // 
            this.debuggertabcontrol.Controls.Add(this.myaurastab);
            this.debuggertabcontrol.Controls.Add(this.targetaurastab);
            this.debuggertabcontrol.Controls.Add(this.mycachedaurastab);
            this.debuggertabcontrol.Controls.Add(this.targetcachedaurastab);
            this.debuggertabcontrol.Location = new System.Drawing.Point(12, 12);
            this.debuggertabcontrol.Name = "debuggertabcontrol";
            this.debuggertabcontrol.SelectedIndex = 0;
            this.debuggertabcontrol.Size = new System.Drawing.Size(1179, 464);
            this.debuggertabcontrol.TabIndex = 0;
            // 
            // myaurastab
            // 
            this.myaurastab.Controls.Add(this.myauraslogfilebutton);
            this.myaurastab.Controls.Add(this.myaurasdatagrid);
            this.myaurastab.Controls.Add(this.myaurasbutton);
            this.myaurastab.Location = new System.Drawing.Point(4, 22);
            this.myaurastab.Name = "myaurastab";
            this.myaurastab.Padding = new System.Windows.Forms.Padding(3);
            this.myaurastab.Size = new System.Drawing.Size(1171, 438);
            this.myaurastab.TabIndex = 0;
            this.myaurastab.Text = "My Aura\'s";
            this.myaurastab.UseVisualStyleBackColor = true;
            // 
            // myaurasdatagrid
            // 
            this.myaurasdatagrid.BackgroundColor = System.Drawing.Color.White;
            this.myaurasdatagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.myaurasdatagrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumnMyAuras,
            this.spellIdDataGridViewTextBoxColumnMyAuras,
            this.spellDataGridViewTextBoxColumnMyAuras,
            this.applyAuraTypeDataGridViewTextBoxColumnMyAuras,
            this.creatorGuidDataGridViewTextBoxColumnMyAuras,
            this.flagsDataGridViewTextBoxColumnMyAuras,
            this.durationDataGridViewTextBoxColumnMyAuras,
            this.endTimeDataGridViewTextBoxColumnMyAuras,
            this.timeLeftDataGridViewTextBoxColumnMyAuras,
            this.stackCountDataGridViewTextBoxColumnMyAuras,
            this.levelDataGridViewTextBoxColumnMyAuras,
            this.ısHarmfulDataGridViewCheckBoxColumnMyAuras,
            this.ısActiveDataGridViewCheckBoxColumnMyAuras,
            this.ısPassiveDataGridViewCheckBoxColumnMyAuras,
            this.cancellableDataGridViewCheckBoxColumnMyAuras});
            this.myaurasdatagrid.Location = new System.Drawing.Point(6, 35);
            this.myaurasdatagrid.Name = "myaurasdatagrid";
            this.myaurasdatagrid.Size = new System.Drawing.Size(1159, 397);
            this.myaurasdatagrid.TabIndex = 1;
            this.myaurasdatagrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.myaurasdatagrid_DataError);
            // 
            // nameDataGridViewTextBoxColumnMyAuras
            // 
            this.nameDataGridViewTextBoxColumnMyAuras.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumnMyAuras.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumnMyAuras.Name = "nameDataGridViewTextBoxColumnMyAuras";
            this.nameDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // spellIdDataGridViewTextBoxColumnMyAuras
            // 
            this.spellIdDataGridViewTextBoxColumnMyAuras.DataPropertyName = "SpellId";
            this.spellIdDataGridViewTextBoxColumnMyAuras.HeaderText = "Spell ID";
            this.spellIdDataGridViewTextBoxColumnMyAuras.Name = "spellIdDataGridViewTextBoxColumnMyAuras";
            this.spellIdDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // spellDataGridViewTextBoxColumnMyAuras
            // 
            this.spellDataGridViewTextBoxColumnMyAuras.DataPropertyName = "Spell";
            this.spellDataGridViewTextBoxColumnMyAuras.HeaderText = "Spell";
            this.spellDataGridViewTextBoxColumnMyAuras.Name = "spellDataGridViewTextBoxColumnMyAuras";
            this.spellDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // applyAuraTypeDataGridViewTextBoxColumnMyAuras
            // 
            this.applyAuraTypeDataGridViewTextBoxColumnMyAuras.DataPropertyName = "ApplyAuraType";
            this.applyAuraTypeDataGridViewTextBoxColumnMyAuras.HeaderText = "Apply Aura Type";
            this.applyAuraTypeDataGridViewTextBoxColumnMyAuras.Name = "applyAuraTypeDataGridViewTextBoxColumnMyAuras";
            this.applyAuraTypeDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // creatorGuidDataGridViewTextBoxColumnMyAuras
            // 
            this.creatorGuidDataGridViewTextBoxColumnMyAuras.DataPropertyName = "CreatorGuid";
            this.creatorGuidDataGridViewTextBoxColumnMyAuras.HeaderText = "Creator GUID";
            this.creatorGuidDataGridViewTextBoxColumnMyAuras.Name = "creatorGuidDataGridViewTextBoxColumnMyAuras";
            this.creatorGuidDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // flagsDataGridViewTextBoxColumnMyAuras
            // 
            this.flagsDataGridViewTextBoxColumnMyAuras.DataPropertyName = "Flags";
            this.flagsDataGridViewTextBoxColumnMyAuras.HeaderText = "Flags";
            this.flagsDataGridViewTextBoxColumnMyAuras.Name = "flagsDataGridViewTextBoxColumnMyAuras";
            this.flagsDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // durationDataGridViewTextBoxColumnMyAuras
            // 
            this.durationDataGridViewTextBoxColumnMyAuras.DataPropertyName = "Duration";
            this.durationDataGridViewTextBoxColumnMyAuras.HeaderText = "Duration";
            this.durationDataGridViewTextBoxColumnMyAuras.Name = "durationDataGridViewTextBoxColumnMyAuras";
            this.durationDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // endTimeDataGridViewTextBoxColumnMyAuras
            // 
            this.endTimeDataGridViewTextBoxColumnMyAuras.DataPropertyName = "EndTime";
            this.endTimeDataGridViewTextBoxColumnMyAuras.HeaderText = "End Time";
            this.endTimeDataGridViewTextBoxColumnMyAuras.Name = "endTimeDataGridViewTextBoxColumnMyAuras";
            this.endTimeDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // timeLeftDataGridViewTextBoxColumnMyAuras
            // 
            this.timeLeftDataGridViewTextBoxColumnMyAuras.DataPropertyName = "TimeLeft";
            this.timeLeftDataGridViewTextBoxColumnMyAuras.HeaderText = "Time Left";
            this.timeLeftDataGridViewTextBoxColumnMyAuras.Name = "timeLeftDataGridViewTextBoxColumnMyAuras";
            this.timeLeftDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // stackCountDataGridViewTextBoxColumnMyAuras
            // 
            this.stackCountDataGridViewTextBoxColumnMyAuras.DataPropertyName = "StackCount";
            this.stackCountDataGridViewTextBoxColumnMyAuras.HeaderText = "Stack Count";
            this.stackCountDataGridViewTextBoxColumnMyAuras.Name = "stackCountDataGridViewTextBoxColumnMyAuras";
            this.stackCountDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // levelDataGridViewTextBoxColumnMyAuras
            // 
            this.levelDataGridViewTextBoxColumnMyAuras.DataPropertyName = "Level";
            this.levelDataGridViewTextBoxColumnMyAuras.HeaderText = "Level";
            this.levelDataGridViewTextBoxColumnMyAuras.Name = "levelDataGridViewTextBoxColumnMyAuras";
            this.levelDataGridViewTextBoxColumnMyAuras.ReadOnly = true;
            // 
            // ısHarmfulDataGridViewCheckBoxColumnMyAuras
            // 
            this.ısHarmfulDataGridViewCheckBoxColumnMyAuras.DataPropertyName = "IsHarmful";
            this.ısHarmfulDataGridViewCheckBoxColumnMyAuras.HeaderText = "Is Harmful";
            this.ısHarmfulDataGridViewCheckBoxColumnMyAuras.Name = "ısHarmfulDataGridViewCheckBoxColumnMyAuras";
            this.ısHarmfulDataGridViewCheckBoxColumnMyAuras.ReadOnly = true;
            this.ısHarmfulDataGridViewCheckBoxColumnMyAuras.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ısHarmfulDataGridViewCheckBoxColumnMyAuras.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ısActiveDataGridViewCheckBoxColumnMyAuras
            // 
            this.ısActiveDataGridViewCheckBoxColumnMyAuras.DataPropertyName = "IsActive";
            this.ısActiveDataGridViewCheckBoxColumnMyAuras.HeaderText = "Is Active";
            this.ısActiveDataGridViewCheckBoxColumnMyAuras.Name = "ısActiveDataGridViewCheckBoxColumnMyAuras";
            this.ısActiveDataGridViewCheckBoxColumnMyAuras.ReadOnly = true;
            this.ısActiveDataGridViewCheckBoxColumnMyAuras.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ısActiveDataGridViewCheckBoxColumnMyAuras.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ısPassiveDataGridViewCheckBoxColumnMyAuras
            // 
            this.ısPassiveDataGridViewCheckBoxColumnMyAuras.DataPropertyName = "IsPassive";
            this.ısPassiveDataGridViewCheckBoxColumnMyAuras.HeaderText = "Is Passive";
            this.ısPassiveDataGridViewCheckBoxColumnMyAuras.Name = "ısPassiveDataGridViewCheckBoxColumnMyAuras";
            this.ısPassiveDataGridViewCheckBoxColumnMyAuras.ReadOnly = true;
            this.ısPassiveDataGridViewCheckBoxColumnMyAuras.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ısPassiveDataGridViewCheckBoxColumnMyAuras.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // cancellableDataGridViewCheckBoxColumnMyAuras
            // 
            this.cancellableDataGridViewCheckBoxColumnMyAuras.DataPropertyName = "Cancellable";
            this.cancellableDataGridViewCheckBoxColumnMyAuras.HeaderText = "Cancellable";
            this.cancellableDataGridViewCheckBoxColumnMyAuras.Name = "cancellableDataGridViewCheckBoxColumnMyAuras";
            this.cancellableDataGridViewCheckBoxColumnMyAuras.ReadOnly = true;
            this.cancellableDataGridViewCheckBoxColumnMyAuras.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cancellableDataGridViewCheckBoxColumnMyAuras.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // myaurasbutton
            // 
            this.myaurasbutton.Location = new System.Drawing.Point(3, 6);
            this.myaurasbutton.Name = "myaurasbutton";
            this.myaurasbutton.Size = new System.Drawing.Size(300, 23);
            this.myaurasbutton.TabIndex = 0;
            this.myaurasbutton.Text = "Update my Aura\'s";
            this.myaurasbutton.UseVisualStyleBackColor = true;
            this.myaurasbutton.Click += new System.EventHandler(this.myaurasbutton_Click);
            // 
            // targetaurastab
            // 
            this.targetaurastab.Controls.Add(this.mytargetauraslogfilebutton);
            this.targetaurastab.Controls.Add(this.mytargetaurasdatagrid);
            this.targetaurastab.Controls.Add(this.targetaurasbutton);
            this.targetaurastab.Location = new System.Drawing.Point(4, 22);
            this.targetaurastab.Name = "targetaurastab";
            this.targetaurastab.Padding = new System.Windows.Forms.Padding(3);
            this.targetaurastab.Size = new System.Drawing.Size(1171, 438);
            this.targetaurastab.TabIndex = 1;
            this.targetaurastab.Text = "Target Aura\'s";
            this.targetaurastab.UseVisualStyleBackColor = true;
            // 
            // mytargetaurasdatagrid
            // 
            this.mytargetaurasdatagrid.BackgroundColor = System.Drawing.Color.White;
            this.mytargetaurasdatagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mytargetaurasdatagrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumnMyTargetAuras,
            this.spellIdDataGridViewTextBoxColumnMyTargetAuras,
            this.spellDataGridViewTextBoxColumnMyTargetAuras,
            this.applyAuraTypeDataGridViewTextBoxColumnMyTargetAuras,
            this.creatorGuidDataGridViewTextBoxColumnMyTargetAuras,
            this.flagsDataGridViewTextBoxColumnMyTargetAuras,
            this.durationDataGridViewTextBoxColumnMyTargetAuras,
            this.endTimeDataGridViewTextBoxColumnMyTargetAuras,
            this.timeLeftDataGridViewTextBoxColumnMyTargetAuras,
            this.stackCountDataGridViewTextBoxColumnMyTargetAuras,
            this.levelDataGridViewTextBoxColumnMyTargetAuras,
            this.ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras,
            this.ısActiveDataGridViewCheckBoxColumnMyTargetAuras,
            this.ısPassiveDataGridViewCheckBoxColumnMyTargetAuras,
            this.cancellableDataGridViewCheckBoxColumnMyTargetAuras});
            this.mytargetaurasdatagrid.Location = new System.Drawing.Point(6, 35);
            this.mytargetaurasdatagrid.Name = "mytargetaurasdatagrid";
            this.mytargetaurasdatagrid.Size = new System.Drawing.Size(1159, 397);
            this.mytargetaurasdatagrid.TabIndex = 2;
            this.mytargetaurasdatagrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.mytargetaurasdatagrid_DataError);
            // 
            // nameDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.nameDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumnMyTargetAuras.Name = "nameDataGridViewTextBoxColumnMyTargetAuras";
            this.nameDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // spellIdDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.spellIdDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "SpellId";
            this.spellIdDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "Spell ID";
            this.spellIdDataGridViewTextBoxColumnMyTargetAuras.Name = "spellIdDataGridViewTextBoxColumnMyTargetAuras";
            this.spellIdDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // spellDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.spellDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "Spell";
            this.spellDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "Spell";
            this.spellDataGridViewTextBoxColumnMyTargetAuras.Name = "spellDataGridViewTextBoxColumnMyTargetAuras";
            this.spellDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // applyAuraTypeDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.applyAuraTypeDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "ApplyAuraType";
            this.applyAuraTypeDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "Apply Aura Type";
            this.applyAuraTypeDataGridViewTextBoxColumnMyTargetAuras.Name = "applyAuraTypeDataGridViewTextBoxColumnMyTargetAuras";
            this.applyAuraTypeDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // creatorGuidDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.creatorGuidDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "CreatorGuid";
            this.creatorGuidDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "Creator GUID";
            this.creatorGuidDataGridViewTextBoxColumnMyTargetAuras.Name = "creatorGuidDataGridViewTextBoxColumnMyTargetAuras";
            this.creatorGuidDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // flagsDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.flagsDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "Flags";
            this.flagsDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "Flags";
            this.flagsDataGridViewTextBoxColumnMyTargetAuras.Name = "flagsDataGridViewTextBoxColumnMyTargetAuras";
            this.flagsDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // durationDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.durationDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "Duration";
            this.durationDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "Duration";
            this.durationDataGridViewTextBoxColumnMyTargetAuras.Name = "durationDataGridViewTextBoxColumnMyTargetAuras";
            this.durationDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // endTimeDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.endTimeDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "EndTime";
            this.endTimeDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "End Time";
            this.endTimeDataGridViewTextBoxColumnMyTargetAuras.Name = "endTimeDataGridViewTextBoxColumnMyTargetAuras";
            this.endTimeDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // timeLeftDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.timeLeftDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "TimeLeft";
            this.timeLeftDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "Time Left";
            this.timeLeftDataGridViewTextBoxColumnMyTargetAuras.Name = "timeLeftDataGridViewTextBoxColumnMyTargetAuras";
            this.timeLeftDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // stackCountDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.stackCountDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "StackCount";
            this.stackCountDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "Stack Count";
            this.stackCountDataGridViewTextBoxColumnMyTargetAuras.Name = "stackCountDataGridViewTextBoxColumnMyTargetAuras";
            this.stackCountDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // levelDataGridViewTextBoxColumnMyTargetAuras
            // 
            this.levelDataGridViewTextBoxColumnMyTargetAuras.DataPropertyName = "Level";
            this.levelDataGridViewTextBoxColumnMyTargetAuras.HeaderText = "Level";
            this.levelDataGridViewTextBoxColumnMyTargetAuras.Name = "levelDataGridViewTextBoxColumnMyTargetAuras";
            this.levelDataGridViewTextBoxColumnMyTargetAuras.ReadOnly = true;
            // 
            // ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras
            // 
            this.ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras.DataPropertyName = "IsHarmful";
            this.ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras.HeaderText = "Is Harmful";
            this.ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras.Name = "ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras";
            this.ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras.ReadOnly = true;
            this.ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ısActiveDataGridViewCheckBoxColumnMyTargetAuras
            // 
            this.ısActiveDataGridViewCheckBoxColumnMyTargetAuras.DataPropertyName = "IsActive";
            this.ısActiveDataGridViewCheckBoxColumnMyTargetAuras.HeaderText = "Is Active";
            this.ısActiveDataGridViewCheckBoxColumnMyTargetAuras.Name = "ısActiveDataGridViewCheckBoxColumnMyTargetAuras";
            this.ısActiveDataGridViewCheckBoxColumnMyTargetAuras.ReadOnly = true;
            this.ısActiveDataGridViewCheckBoxColumnMyTargetAuras.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ısActiveDataGridViewCheckBoxColumnMyTargetAuras.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ısPassiveDataGridViewCheckBoxColumnMyTargetAuras
            // 
            this.ısPassiveDataGridViewCheckBoxColumnMyTargetAuras.DataPropertyName = "IsPassive";
            this.ısPassiveDataGridViewCheckBoxColumnMyTargetAuras.HeaderText = "Is Passive";
            this.ısPassiveDataGridViewCheckBoxColumnMyTargetAuras.Name = "ısPassiveDataGridViewCheckBoxColumnMyTargetAuras";
            this.ısPassiveDataGridViewCheckBoxColumnMyTargetAuras.ReadOnly = true;
            this.ısPassiveDataGridViewCheckBoxColumnMyTargetAuras.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ısPassiveDataGridViewCheckBoxColumnMyTargetAuras.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // cancellableDataGridViewCheckBoxColumnMyTargetAuras
            // 
            this.cancellableDataGridViewCheckBoxColumnMyTargetAuras.DataPropertyName = "Cancellable";
            this.cancellableDataGridViewCheckBoxColumnMyTargetAuras.HeaderText = "Cancellable";
            this.cancellableDataGridViewCheckBoxColumnMyTargetAuras.Name = "cancellableDataGridViewCheckBoxColumnMyTargetAuras";
            this.cancellableDataGridViewCheckBoxColumnMyTargetAuras.ReadOnly = true;
            this.cancellableDataGridViewCheckBoxColumnMyTargetAuras.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cancellableDataGridViewCheckBoxColumnMyTargetAuras.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // targetaurasbutton
            // 
            this.targetaurasbutton.Location = new System.Drawing.Point(3, 6);
            this.targetaurasbutton.Name = "targetaurasbutton";
            this.targetaurasbutton.Size = new System.Drawing.Size(300, 23);
            this.targetaurasbutton.TabIndex = 1;
            this.targetaurasbutton.Text = "Update my Target\'s Aura\'s";
            this.targetaurasbutton.UseVisualStyleBackColor = true;
            this.targetaurasbutton.Click += new System.EventHandler(this.targetaurasbutton_Click);
            // 
            // mycachedaurastab
            // 
            this.mycachedaurastab.Controls.Add(this.dataGridView1);
            this.mycachedaurastab.Controls.Add(this.mycachedaurasbutton);
            this.mycachedaurastab.Location = new System.Drawing.Point(4, 22);
            this.mycachedaurastab.Name = "mycachedaurastab";
            this.mycachedaurastab.Padding = new System.Windows.Forms.Padding(3);
            this.mycachedaurastab.Size = new System.Drawing.Size(1171, 438);
            this.mycachedaurastab.TabIndex = 2;
            this.mycachedaurastab.Text = "My Cached Aura\'s";
            this.mycachedaurastab.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 35);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1159, 397);
            this.dataGridView1.TabIndex = 2;
            // 
            // mycachedaurasbutton
            // 
            this.mycachedaurasbutton.Location = new System.Drawing.Point(3, 6);
            this.mycachedaurasbutton.Name = "mycachedaurasbutton";
            this.mycachedaurasbutton.Size = new System.Drawing.Size(1162, 23);
            this.mycachedaurasbutton.TabIndex = 1;
            this.mycachedaurasbutton.Text = "Update my Cached Aura\'s";
            this.mycachedaurasbutton.UseVisualStyleBackColor = true;
            this.mycachedaurasbutton.Click += new System.EventHandler(this.mycachedaurasbutton_Click);
            // 
            // targetcachedaurastab
            // 
            this.targetcachedaurastab.Controls.Add(this.dataGridView2);
            this.targetcachedaurastab.Controls.Add(this.targetcachedaurasbutton);
            this.targetcachedaurastab.Location = new System.Drawing.Point(4, 22);
            this.targetcachedaurastab.Name = "targetcachedaurastab";
            this.targetcachedaurastab.Padding = new System.Windows.Forms.Padding(3);
            this.targetcachedaurastab.Size = new System.Drawing.Size(1171, 438);
            this.targetcachedaurastab.TabIndex = 3;
            this.targetcachedaurastab.Text = "Target Cached Aura\'s";
            this.targetcachedaurastab.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(6, 35);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(1159, 397);
            this.dataGridView2.TabIndex = 2;
            // 
            // targetcachedaurasbutton
            // 
            this.targetcachedaurasbutton.Location = new System.Drawing.Point(6, 6);
            this.targetcachedaurasbutton.Name = "targetcachedaurasbutton";
            this.targetcachedaurasbutton.Size = new System.Drawing.Size(1162, 23);
            this.targetcachedaurasbutton.TabIndex = 1;
            this.targetcachedaurasbutton.Text = "Update my Target\'s Cached Aura\'s";
            this.targetcachedaurasbutton.UseVisualStyleBackColor = true;
            this.targetcachedaurasbutton.Click += new System.EventHandler(this.targetcachedaurasbutton_Click);
            // 
            // myauraslogfilebutton
            // 
            this.myauraslogfilebutton.Location = new System.Drawing.Point(865, 6);
            this.myauraslogfilebutton.Name = "myauraslogfilebutton";
            this.myauraslogfilebutton.Size = new System.Drawing.Size(300, 23);
            this.myauraslogfilebutton.TabIndex = 2;
            this.myauraslogfilebutton.Text = "Write to Logfile";
            this.myauraslogfilebutton.UseVisualStyleBackColor = true;
            this.myauraslogfilebutton.Click += new System.EventHandler(this.myauraslogfilebutton_Click);
            // 
            // mytargetauraslogfilebutton
            // 
            this.mytargetauraslogfilebutton.Location = new System.Drawing.Point(865, 6);
            this.mytargetauraslogfilebutton.Name = "mytargetauraslogfilebutton";
            this.mytargetauraslogfilebutton.Size = new System.Drawing.Size(300, 23);
            this.mytargetauraslogfilebutton.TabIndex = 3;
            this.mytargetauraslogfilebutton.Text = "Write to Logfile";
            this.mytargetauraslogfilebutton.UseVisualStyleBackColor = true;
            this.mytargetauraslogfilebutton.Click += new System.EventHandler(this.mytargetauraslogfilebutton_Click);
            // 
            // DebuggerGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1203, 488);
            this.Controls.Add(this.debuggertabcontrol);
            this.Name = "DebuggerGui";
            this.Text = "Fury Unleashed - Debugger - Made by nomnomnom";
            this.Load += new System.EventHandler(this.Debugger_Load);
            this.debuggertabcontrol.ResumeLayout(false);
            this.myaurastab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.myaurasdatagrid)).EndInit();
            this.targetaurastab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mytargetaurasdatagrid)).EndInit();
            this.mycachedaurastab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.targetcachedaurastab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl debuggertabcontrol;
        private System.Windows.Forms.TabPage targetaurastab;
        private System.Windows.Forms.TabPage myaurastab;
        private System.Windows.Forms.Button myaurasbutton;
        private System.Windows.Forms.DataGridView myaurasdatagrid;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button targetaurasbutton;
        private System.Windows.Forms.DataGridView mytargetaurasdatagrid;
        private System.Windows.Forms.TabPage mycachedaurastab;
        private System.Windows.Forms.TabPage targetcachedaurastab;
        private System.Windows.Forms.Button mycachedaurasbutton;
        private System.Windows.Forms.Button targetcachedaurasbutton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn spellIdDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn spellDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn applyAuraTypeDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn creatorGuidDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn flagsDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn durationDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn endTimeDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeLeftDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn stackCountDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn levelDataGridViewTextBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ısHarmfulDataGridViewCheckBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ısActiveDataGridViewCheckBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ısPassiveDataGridViewCheckBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cancellableDataGridViewCheckBoxColumnMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn spellIdDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn spellDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn applyAuraTypeDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn creatorGuidDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn flagsDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn durationDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn endTimeDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeLeftDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn stackCountDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn levelDataGridViewTextBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ısHarmfulDataGridViewCheckBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ısActiveDataGridViewCheckBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ısPassiveDataGridViewCheckBoxColumnMyTargetAuras;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cancellableDataGridViewCheckBoxColumnMyTargetAuras;
        private System.Windows.Forms.Button myauraslogfilebutton;
        private System.Windows.Forms.Button mytargetauraslogfilebutton;
    }
}