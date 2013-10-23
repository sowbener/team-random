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
            this.dgwMyAuras = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.spellIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.spellDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.applyAuraTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.creatorGuidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flagsDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.durationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeLeftDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stackCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.levelDataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ısHarmfulDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ısActiveDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ısPassiveDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cancellableDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.myaurasbutton = new System.Windows.Forms.Button();
            this.targetaurastab = new System.Windows.Forms.TabPage();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.targetaurasbutton = new System.Windows.Forms.Button();
            this.dgwTargetAuras = new System.Windows.Forms.DataGridView();
            this.debuggertabcontrol.SuspendLayout();
            this.myaurastab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwMyAuras)).BeginInit();
            this.targetaurastab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwTargetAuras)).BeginInit();
            this.SuspendLayout();
            // 
            // debuggertabcontrol
            // 
            this.debuggertabcontrol.Controls.Add(this.myaurastab);
            this.debuggertabcontrol.Controls.Add(this.targetaurastab);
            this.debuggertabcontrol.Location = new System.Drawing.Point(12, 12);
            this.debuggertabcontrol.Name = "debuggertabcontrol";
            this.debuggertabcontrol.SelectedIndex = 0;
            this.debuggertabcontrol.Size = new System.Drawing.Size(1179, 464);
            this.debuggertabcontrol.TabIndex = 0;
            // 
            // myaurastab
            // 
            this.myaurastab.Controls.Add(this.dgwMyAuras);
            this.myaurastab.Controls.Add(this.myaurasbutton);
            this.myaurastab.Location = new System.Drawing.Point(4, 22);
            this.myaurastab.Name = "myaurastab";
            this.myaurastab.Padding = new System.Windows.Forms.Padding(3);
            this.myaurastab.Size = new System.Drawing.Size(1171, 438);
            this.myaurastab.TabIndex = 0;
            this.myaurastab.Text = "My Aura\'s";
            this.myaurastab.UseVisualStyleBackColor = true;
            // 
            // dgwMyAuras
            // 
            this.dgwMyAuras.BackgroundColor = System.Drawing.Color.White;
            this.dgwMyAuras.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwMyAuras.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn2,
            this.spellIdDataGridViewTextBoxColumn,
            this.spellDataGridViewTextBoxColumn,
            this.applyAuraTypeDataGridViewTextBoxColumn,
            this.creatorGuidDataGridViewTextBoxColumn,
            this.flagsDataGridViewTextBoxColumn1,
            this.durationDataGridViewTextBoxColumn,
            this.endTimeDataGridViewTextBoxColumn,
            this.timeLeftDataGridViewTextBoxColumn,
            this.stackCountDataGridViewTextBoxColumn,
            this.levelDataGridViewTextBoxColumn2,
            this.ısHarmfulDataGridViewCheckBoxColumn,
            this.ısActiveDataGridViewCheckBoxColumn,
            this.ısPassiveDataGridViewCheckBoxColumn,
            this.cancellableDataGridViewCheckBoxColumn});
            this.dgwMyAuras.Location = new System.Drawing.Point(6, 35);
            this.dgwMyAuras.Name = "dgwMyAuras";
            this.dgwMyAuras.Size = new System.Drawing.Size(1159, 397);
            this.dgwMyAuras.TabIndex = 1;
            this.dgwMyAuras.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgwMyAuras_DataError);
            // 
            // nameDataGridViewTextBoxColumn2
            // 
            this.nameDataGridViewTextBoxColumn2.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn2.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn2.Name = "nameDataGridViewTextBoxColumn2";
            this.nameDataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // spellIdDataGridViewTextBoxColumn
            // 
            this.spellIdDataGridViewTextBoxColumn.DataPropertyName = "SpellId";
            this.spellIdDataGridViewTextBoxColumn.HeaderText = "Spell ID";
            this.spellIdDataGridViewTextBoxColumn.Name = "spellIdDataGridViewTextBoxColumn";
            this.spellIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // spellDataGridViewTextBoxColumn
            // 
            this.spellDataGridViewTextBoxColumn.DataPropertyName = "Spell";
            this.spellDataGridViewTextBoxColumn.HeaderText = "Spell";
            this.spellDataGridViewTextBoxColumn.Name = "spellDataGridViewTextBoxColumn";
            this.spellDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // applyAuraTypeDataGridViewTextBoxColumn
            // 
            this.applyAuraTypeDataGridViewTextBoxColumn.DataPropertyName = "ApplyAuraType";
            this.applyAuraTypeDataGridViewTextBoxColumn.HeaderText = "Apply Aura Type";
            this.applyAuraTypeDataGridViewTextBoxColumn.Name = "applyAuraTypeDataGridViewTextBoxColumn";
            this.applyAuraTypeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // creatorGuidDataGridViewTextBoxColumn
            // 
            this.creatorGuidDataGridViewTextBoxColumn.DataPropertyName = "CreatorGuid";
            this.creatorGuidDataGridViewTextBoxColumn.HeaderText = "Creator GUID";
            this.creatorGuidDataGridViewTextBoxColumn.Name = "creatorGuidDataGridViewTextBoxColumn";
            this.creatorGuidDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // flagsDataGridViewTextBoxColumn1
            // 
            this.flagsDataGridViewTextBoxColumn1.DataPropertyName = "Flags";
            this.flagsDataGridViewTextBoxColumn1.HeaderText = "Flags";
            this.flagsDataGridViewTextBoxColumn1.Name = "flagsDataGridViewTextBoxColumn1";
            this.flagsDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // durationDataGridViewTextBoxColumn
            // 
            this.durationDataGridViewTextBoxColumn.DataPropertyName = "Duration";
            this.durationDataGridViewTextBoxColumn.HeaderText = "Duration";
            this.durationDataGridViewTextBoxColumn.Name = "durationDataGridViewTextBoxColumn";
            this.durationDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // endTimeDataGridViewTextBoxColumn
            // 
            this.endTimeDataGridViewTextBoxColumn.DataPropertyName = "EndTime";
            this.endTimeDataGridViewTextBoxColumn.HeaderText = "End Time";
            this.endTimeDataGridViewTextBoxColumn.Name = "endTimeDataGridViewTextBoxColumn";
            this.endTimeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // timeLeftDataGridViewTextBoxColumn
            // 
            this.timeLeftDataGridViewTextBoxColumn.DataPropertyName = "TimeLeft";
            this.timeLeftDataGridViewTextBoxColumn.HeaderText = "Time Left";
            this.timeLeftDataGridViewTextBoxColumn.Name = "timeLeftDataGridViewTextBoxColumn";
            this.timeLeftDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // stackCountDataGridViewTextBoxColumn
            // 
            this.stackCountDataGridViewTextBoxColumn.DataPropertyName = "StackCount";
            this.stackCountDataGridViewTextBoxColumn.HeaderText = "Stack Count";
            this.stackCountDataGridViewTextBoxColumn.Name = "stackCountDataGridViewTextBoxColumn";
            this.stackCountDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // levelDataGridViewTextBoxColumn2
            // 
            this.levelDataGridViewTextBoxColumn2.DataPropertyName = "Level";
            this.levelDataGridViewTextBoxColumn2.HeaderText = "Level";
            this.levelDataGridViewTextBoxColumn2.Name = "levelDataGridViewTextBoxColumn2";
            this.levelDataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // ısHarmfulDataGridViewCheckBoxColumn
            // 
            this.ısHarmfulDataGridViewCheckBoxColumn.DataPropertyName = "IsHarmful";
            this.ısHarmfulDataGridViewCheckBoxColumn.HeaderText = "Is Harmful";
            this.ısHarmfulDataGridViewCheckBoxColumn.Name = "ısHarmfulDataGridViewCheckBoxColumn";
            this.ısHarmfulDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // ısActiveDataGridViewCheckBoxColumn
            // 
            this.ısActiveDataGridViewCheckBoxColumn.DataPropertyName = "IsActive";
            this.ısActiveDataGridViewCheckBoxColumn.HeaderText = "Is Active";
            this.ısActiveDataGridViewCheckBoxColumn.Name = "ısActiveDataGridViewCheckBoxColumn";
            this.ısActiveDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // ısPassiveDataGridViewCheckBoxColumn
            // 
            this.ısPassiveDataGridViewCheckBoxColumn.DataPropertyName = "IsPassive";
            this.ısPassiveDataGridViewCheckBoxColumn.HeaderText = "Is Passive";
            this.ısPassiveDataGridViewCheckBoxColumn.Name = "ısPassiveDataGridViewCheckBoxColumn";
            this.ısPassiveDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // cancellableDataGridViewCheckBoxColumn
            // 
            this.cancellableDataGridViewCheckBoxColumn.DataPropertyName = "Cancellable";
            this.cancellableDataGridViewCheckBoxColumn.HeaderText = "Cancellable";
            this.cancellableDataGridViewCheckBoxColumn.Name = "cancellableDataGridViewCheckBoxColumn";
            this.cancellableDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // myaurasbutton
            // 
            this.myaurasbutton.Location = new System.Drawing.Point(3, 6);
            this.myaurasbutton.Name = "myaurasbutton";
            this.myaurasbutton.Size = new System.Drawing.Size(1162, 23);
            this.myaurasbutton.TabIndex = 0;
            this.myaurasbutton.Text = "Update my Aura\'s";
            this.myaurasbutton.UseVisualStyleBackColor = true;
            this.myaurasbutton.Click += new System.EventHandler(this.myaurasbutton_Click);
            // 
            // targetaurastab
            // 
            this.targetaurastab.Controls.Add(this.dgwTargetAuras);
            this.targetaurastab.Controls.Add(this.targetaurasbutton);
            this.targetaurastab.Location = new System.Drawing.Point(4, 22);
            this.targetaurastab.Name = "targetaurastab";
            this.targetaurastab.Padding = new System.Windows.Forms.Padding(3);
            this.targetaurastab.Size = new System.Drawing.Size(1171, 438);
            this.targetaurastab.TabIndex = 1;
            this.targetaurastab.Text = "Target Aura\'s";
            this.targetaurastab.UseVisualStyleBackColor = true;
            // 
            // targetaurasbutton
            // 
            this.targetaurasbutton.Location = new System.Drawing.Point(3, 6);
            this.targetaurasbutton.Name = "targetaurasbutton";
            this.targetaurasbutton.Size = new System.Drawing.Size(1162, 23);
            this.targetaurasbutton.TabIndex = 1;
            this.targetaurasbutton.Text = "Update my Target\'s Aura\'s";
            this.targetaurasbutton.UseVisualStyleBackColor = true;
            this.targetaurasbutton.Click += new System.EventHandler(this.targetaurasbutton_Click);
            // 
            // dgwTargetAuras
            // 
            this.dgwTargetAuras.BackgroundColor = System.Drawing.Color.White;
            this.dgwTargetAuras.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwTargetAuras.Location = new System.Drawing.Point(6, 35);
            this.dgwTargetAuras.Name = "dgwTargetAuras";
            this.dgwTargetAuras.Size = new System.Drawing.Size(1159, 397);
            this.dgwTargetAuras.TabIndex = 2;
            // 
            // DebuggerGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1203, 488);
            this.Controls.Add(this.debuggertabcontrol);
            this.Name = "DebuggerGui";
            this.Text = "Fury Unleashed - Debugger";
            this.Load += new System.EventHandler(this.Debugger_Load);
            this.debuggertabcontrol.ResumeLayout(false);
            this.myaurastab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgwMyAuras)).EndInit();
            this.targetaurastab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgwTargetAuras)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl debuggertabcontrol;
        private System.Windows.Forms.TabPage targetaurastab;
        private System.Windows.Forms.TabPage myaurastab;
        private System.Windows.Forms.Button myaurasbutton;
        private System.Windows.Forms.DataGridView dgwMyAuras;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn spellIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn spellDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn applyAuraTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn creatorGuidDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn flagsDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn durationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn endTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeLeftDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn stackCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn levelDataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ısHarmfulDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ısActiveDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ısPassiveDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cancellableDataGridViewCheckBoxColumn;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button targetaurasbutton;
        private System.Windows.Forms.DataGridView dgwTargetAuras;
    }
}