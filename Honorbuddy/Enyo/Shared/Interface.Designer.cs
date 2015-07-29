namespace Enyo.Shared
{
    partial class Interface
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
            this.SettingsPanel = new System.Windows.Forms.Panel();
            this.TicksPerSecondTrackBar = new System.Windows.Forms.TrackBar();
            this.PauseKeyComboBox = new System.Windows.Forms.ComboBox();
            this.ModifierKeyComboBox = new System.Windows.Forms.ComboBox();
            this.PauseLabel = new System.Windows.Forms.Label();
            this.Plugins = new System.Windows.Forms.CheckBox();
            this.ContinuesHealingMode = new System.Windows.Forms.CheckBox();
            this.HardLock = new System.Windows.Forms.CheckBox();
            this.ChatOutput = new System.Windows.Forms.CheckBox();
            this.SaveandCloseButton = new System.Windows.Forms.Button();
            this.NamePanel = new System.Windows.Forms.Panel();
            this.EnyoNameLabel = new System.Windows.Forms.Label();
            this.TpsLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ClicktoMove = new System.Windows.Forms.CheckBox();
            this.EnyoNavigator = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ForcedCombatMode = new System.Windows.Forms.CheckBox();
            this.AdditionalFeaturesLabel = new System.Windows.Forms.Label();
            this.SoftLock = new System.Windows.Forms.CheckBox();
            this.FrameLockFeaturesLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.LoggingLabel = new System.Windows.Forms.Label();
            this.SettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TicksPerSecondTrackBar)).BeginInit();
            this.NamePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // SettingsPanel
            // 
            this.SettingsPanel.BackColor = System.Drawing.Color.White;
            this.SettingsPanel.Controls.Add(this.TicksPerSecondTrackBar);
            this.SettingsPanel.Location = new System.Drawing.Point(2, 45);
            this.SettingsPanel.Name = "SettingsPanel";
            this.SettingsPanel.Size = new System.Drawing.Size(507, 50);
            this.SettingsPanel.TabIndex = 0;
            // 
            // TicksPerSecondTrackBar
            // 
            this.TicksPerSecondTrackBar.BackColor = System.Drawing.Color.White;
            this.TicksPerSecondTrackBar.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TicksPerSecondTrackBar.Location = new System.Drawing.Point(10, 2);
            this.TicksPerSecondTrackBar.Maximum = 100;
            this.TicksPerSecondTrackBar.Minimum = 10;
            this.TicksPerSecondTrackBar.Name = "TicksPerSecondTrackBar";
            this.TicksPerSecondTrackBar.Size = new System.Drawing.Size(487, 45);
            this.TicksPerSecondTrackBar.SmallChange = 10;
            this.TicksPerSecondTrackBar.TabIndex = 0;
            this.TicksPerSecondTrackBar.TickFrequency = 5;
            this.TicksPerSecondTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.TicksPerSecondTrackBar.Value = 30;
            this.TicksPerSecondTrackBar.Scroll += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            // 
            // PauseKeyComboBox
            // 
            this.PauseKeyComboBox.BackColor = System.Drawing.Color.White;
            this.PauseKeyComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PauseKeyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PauseKeyComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PauseKeyComboBox.ForeColor = System.Drawing.Color.DodgerBlue;
            this.PauseKeyComboBox.FormattingEnabled = true;
            this.PauseKeyComboBox.Location = new System.Drawing.Point(13, 59);
            this.PauseKeyComboBox.Name = "PauseKeyComboBox";
            this.PauseKeyComboBox.Size = new System.Drawing.Size(229, 25);
            this.PauseKeyComboBox.TabIndex = 7;
            this.PauseKeyComboBox.SelectedIndexChanged += new System.EventHandler(this.PauseKeyComboBox_SelectedIndexChanged);
            this.PauseKeyComboBox.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            // 
            // ModifierKeyComboBox
            // 
            this.ModifierKeyComboBox.BackColor = System.Drawing.Color.White;
            this.ModifierKeyComboBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ModifierKeyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModifierKeyComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ModifierKeyComboBox.ForeColor = System.Drawing.Color.DodgerBlue;
            this.ModifierKeyComboBox.FormattingEnabled = true;
            this.ModifierKeyComboBox.Location = new System.Drawing.Point(13, 22);
            this.ModifierKeyComboBox.Name = "ModifierKeyComboBox";
            this.ModifierKeyComboBox.Size = new System.Drawing.Size(229, 25);
            this.ModifierKeyComboBox.TabIndex = 6;
            this.ModifierKeyComboBox.SelectedIndexChanged += new System.EventHandler(this.ModifierKeyComboBox_SelectedIndexChanged);
            this.ModifierKeyComboBox.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            // 
            // PauseLabel
            // 
            this.PauseLabel.AutoSize = true;
            this.PauseLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PauseLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.PauseLabel.Location = new System.Drawing.Point(10, 4);
            this.PauseLabel.Name = "PauseLabel";
            this.PauseLabel.Size = new System.Drawing.Size(226, 16);
            this.PauseLabel.TabIndex = 5;
            this.PauseLabel.Text = "Select Pause Hotkey Combination";
            this.PauseLabel.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            // 
            // Plugins
            // 
            this.Plugins.AutoSize = true;
            this.Plugins.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Plugins.ForeColor = System.Drawing.Color.DodgerBlue;
            this.Plugins.Location = new System.Drawing.Point(10, 125);
            this.Plugins.Name = "Plugins";
            this.Plugins.Size = new System.Drawing.Size(217, 21);
            this.Plugins.TabIndex = 4;
            this.Plugins.Text = "Enable Plugins and AutoEquip";
            this.Plugins.UseVisualStyleBackColor = false;
            this.Plugins.CheckedChanged += new System.EventHandler(this.Plugins_CheckedChanged);
            this.Plugins.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.Plugins.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Plugins_MouseMove);
            // 
            // ContinuesHealingMode
            // 
            this.ContinuesHealingMode.AutoSize = true;
            this.ContinuesHealingMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ContinuesHealingMode.ForeColor = System.Drawing.Color.DodgerBlue;
            this.ContinuesHealingMode.Location = new System.Drawing.Point(10, 85);
            this.ContinuesHealingMode.Name = "ContinuesHealingMode";
            this.ContinuesHealingMode.Size = new System.Drawing.Size(231, 21);
            this.ContinuesHealingMode.TabIndex = 3;
            this.ContinuesHealingMode.Text = "Enable Continues Healing Mode";
            this.ContinuesHealingMode.UseVisualStyleBackColor = false;
            this.ContinuesHealingMode.CheckedChanged += new System.EventHandler(this.ContinuesHealingMode_CheckedChanged);
            this.ContinuesHealingMode.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.ContinuesHealingMode.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ContinuesHealingMode_MouseMove);
            // 
            // HardLock
            // 
            this.HardLock.AutoSize = true;
            this.HardLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HardLock.ForeColor = System.Drawing.Color.DodgerBlue;
            this.HardLock.Location = new System.Drawing.Point(10, 22);
            this.HardLock.Name = "HardLock";
            this.HardLock.Size = new System.Drawing.Size(133, 21);
            this.HardLock.TabIndex = 2;
            this.HardLock.Text = "Enable HardLock";
            this.HardLock.UseVisualStyleBackColor = false;
            this.HardLock.CheckedChanged += new System.EventHandler(this.HardLock_CheckedChanged);
            this.HardLock.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.HardLock.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HardLock_MouseMove);
            // 
            // ChatOutput
            // 
            this.ChatOutput.AutoSize = true;
            this.ChatOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChatOutput.ForeColor = System.Drawing.Color.DodgerBlue;
            this.ChatOutput.Location = new System.Drawing.Point(13, 119);
            this.ChatOutput.Name = "ChatOutput";
            this.ChatOutput.Size = new System.Drawing.Size(178, 21);
            this.ChatOutput.TabIndex = 1;
            this.ChatOutput.Text = "Enable Overlay Logging";
            this.ChatOutput.UseVisualStyleBackColor = false;
            this.ChatOutput.CheckedChanged += new System.EventHandler(this.OverlayOutput_CheckedChanged);
            this.ChatOutput.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.ChatOutput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChatOutput_MouseMove);
            // 
            // SaveandCloseButton
            // 
            this.SaveandCloseButton.BackColor = System.Drawing.Color.DodgerBlue;
            this.SaveandCloseButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.SaveandCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveandCloseButton.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.SaveandCloseButton.Location = new System.Drawing.Point(366, 319);
            this.SaveandCloseButton.Name = "SaveandCloseButton";
            this.SaveandCloseButton.Size = new System.Drawing.Size(143, 23);
            this.SaveandCloseButton.TabIndex = 0;
            this.SaveandCloseButton.Text = "Save and Close";
            this.SaveandCloseButton.UseVisualStyleBackColor = false;
            this.SaveandCloseButton.Click += new System.EventHandler(this.SaveandCloseButton_Click);
            this.SaveandCloseButton.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.SaveandCloseButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SaveButton_MouseMove);
            // 
            // NamePanel
            // 
            this.NamePanel.BackColor = System.Drawing.Color.White;
            this.NamePanel.Controls.Add(this.EnyoNameLabel);
            this.NamePanel.Location = new System.Drawing.Point(2, 2);
            this.NamePanel.Name = "NamePanel";
            this.NamePanel.Size = new System.Drawing.Size(507, 41);
            this.NamePanel.TabIndex = 0;
            this.NamePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GuiDragDrop);
            // 
            // EnyoNameLabel
            // 
            this.EnyoNameLabel.AutoSize = true;
            this.EnyoNameLabel.Font = new System.Drawing.Font("Century Gothic", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnyoNameLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.EnyoNameLabel.Location = new System.Drawing.Point(64, 1);
            this.EnyoNameLabel.Name = "EnyoNameLabel";
            this.EnyoNameLabel.Size = new System.Drawing.Size(371, 39);
            this.EnyoNameLabel.TabIndex = 0;
            this.EnyoNameLabel.Text = "Enyo - Raiding BotBase";
            this.EnyoNameLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GuiDragDrop);
            // 
            // TpsLabel
            // 
            this.TpsLabel.AutoSize = true;
            this.TpsLabel.BackColor = System.Drawing.Color.Transparent;
            this.TpsLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TpsLabel.ForeColor = System.Drawing.Color.White;
            this.TpsLabel.Location = new System.Drawing.Point(9, 322);
            this.TpsLabel.Name = "TpsLabel";
            this.TpsLabel.Size = new System.Drawing.Size(38, 17);
            this.TpsLabel.TabIndex = 1;
            this.TpsLabel.Text = "Enyo";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.ClicktoMove);
            this.panel1.Controls.Add(this.EnyoNavigator);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.ForcedCombatMode);
            this.panel1.Controls.Add(this.AdditionalFeaturesLabel);
            this.panel1.Controls.Add(this.SoftLock);
            this.panel1.Controls.Add(this.FrameLockFeaturesLabel);
            this.panel1.Controls.Add(this.ContinuesHealingMode);
            this.panel1.Controls.Add(this.HardLock);
            this.panel1.Controls.Add(this.Plugins);
            this.panel1.Location = new System.Drawing.Point(2, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(253, 220);
            this.panel1.TabIndex = 10;
            // 
            // ClicktoMove
            // 
            this.ClicktoMove.AutoSize = true;
            this.ClicktoMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ClicktoMove.ForeColor = System.Drawing.Color.DodgerBlue;
            this.ClicktoMove.Location = new System.Drawing.Point(10, 168);
            this.ClicktoMove.Name = "ClicktoMove";
            this.ClicktoMove.Size = new System.Drawing.Size(162, 21);
            this.ClicktoMove.TabIndex = 15;
            this.ClicktoMove.Text = "Enable Click to Move";
            this.ClicktoMove.UseVisualStyleBackColor = false;
            this.ClicktoMove.CheckedChanged += new System.EventHandler(this.ClicktoMove_CheckedChanged);
            this.ClicktoMove.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ClickToMove_MouseMove);
            // 
            // EnyoNavigator
            // 
            this.EnyoNavigator.AutoSize = true;
            this.EnyoNavigator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EnyoNavigator.ForeColor = System.Drawing.Color.DodgerBlue;
            this.EnyoNavigator.Location = new System.Drawing.Point(10, 188);
            this.EnyoNavigator.Name = "EnyoNavigator";
            this.EnyoNavigator.Size = new System.Drawing.Size(241, 21);
            this.EnyoNavigator.TabIndex = 14;
            this.EnyoNavigator.Text = "Enforce Enyo\'s Navigation Blocker";
            this.EnyoNavigator.UseVisualStyleBackColor = false;
            this.EnyoNavigator.CheckedChanged += new System.EventHandler(this.EnyoNavigator_CheckedChanged);
            this.EnyoNavigator.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.EnyoNavigator.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EnyoNavigator_MouseMove);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label1.Location = new System.Drawing.Point(10, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 16);
            this.label1.TabIndex = 13;
            this.label1.Text = "Movement Features";
            // 
            // ForcedCombatMode
            // 
            this.ForcedCombatMode.AutoSize = true;
            this.ForcedCombatMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ForcedCombatMode.ForeColor = System.Drawing.Color.DodgerBlue;
            this.ForcedCombatMode.Location = new System.Drawing.Point(10, 105);
            this.ForcedCombatMode.Name = "ForcedCombatMode";
            this.ForcedCombatMode.Size = new System.Drawing.Size(217, 21);
            this.ForcedCombatMode.TabIndex = 12;
            this.ForcedCombatMode.Text = "Enable Forced Combat Mode";
            this.ForcedCombatMode.UseVisualStyleBackColor = false;
            this.ForcedCombatMode.CheckedChanged += new System.EventHandler(this.ForcedCombatMode_CheckedChanged);
            this.ForcedCombatMode.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.ForcedCombatMode.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ForceCombat_MouseMove);
            // 
            // AdditionalFeaturesLabel
            // 
            this.AdditionalFeaturesLabel.AutoSize = true;
            this.AdditionalFeaturesLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AdditionalFeaturesLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.AdditionalFeaturesLabel.Location = new System.Drawing.Point(10, 67);
            this.AdditionalFeaturesLabel.Name = "AdditionalFeaturesLabel";
            this.AdditionalFeaturesLabel.Size = new System.Drawing.Size(135, 16);
            this.AdditionalFeaturesLabel.TabIndex = 11;
            this.AdditionalFeaturesLabel.Text = "Additional Features";
            // 
            // SoftLock
            // 
            this.SoftLock.AutoSize = true;
            this.SoftLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SoftLock.ForeColor = System.Drawing.Color.DodgerBlue;
            this.SoftLock.Location = new System.Drawing.Point(10, 42);
            this.SoftLock.Name = "SoftLock";
            this.SoftLock.Size = new System.Drawing.Size(126, 21);
            this.SoftLock.TabIndex = 10;
            this.SoftLock.Text = "Enable SoftLock";
            this.SoftLock.UseVisualStyleBackColor = false;
            this.SoftLock.CheckedChanged += new System.EventHandler(this.SoftLock_CheckedChanged);
            this.SoftLock.MouseLeave += new System.EventHandler(this.TicksPerSecondTrackBar_Scroll);
            this.SoftLock.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SoftLock_MouseMove);
            // 
            // FrameLockFeaturesLabel
            // 
            this.FrameLockFeaturesLabel.AutoSize = true;
            this.FrameLockFeaturesLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FrameLockFeaturesLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.FrameLockFeaturesLabel.Location = new System.Drawing.Point(10, 4);
            this.FrameLockFeaturesLabel.Name = "FrameLockFeaturesLabel";
            this.FrameLockFeaturesLabel.Size = new System.Drawing.Size(134, 16);
            this.FrameLockFeaturesLabel.TabIndex = 8;
            this.FrameLockFeaturesLabel.Text = "Framelock Features";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.LoggingLabel);
            this.panel2.Controls.Add(this.PauseKeyComboBox);
            this.panel2.Controls.Add(this.PauseLabel);
            this.panel2.Controls.Add(this.ModifierKeyComboBox);
            this.panel2.Controls.Add(this.ChatOutput);
            this.panel2.Location = new System.Drawing.Point(257, 97);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(252, 220);
            this.panel2.TabIndex = 11;
            // 
            // LoggingLabel
            // 
            this.LoggingLabel.AutoSize = true;
            this.LoggingLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoggingLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.LoggingLabel.Location = new System.Drawing.Point(10, 98);
            this.LoggingLabel.Name = "LoggingLabel";
            this.LoggingLabel.Size = new System.Drawing.Size(119, 16);
            this.LoggingLabel.TabIndex = 8;
            this.LoggingLabel.Text = "Logging Features";
            // 
            // Interface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.ClientSize = new System.Drawing.Size(511, 344);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.SaveandCloseButton);
            this.Controls.Add(this.TpsLabel);
            this.Controls.Add(this.NamePanel);
            this.Controls.Add(this.SettingsPanel);
            this.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Interface";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enyo GUI";
            this.SettingsPanel.ResumeLayout(false);
            this.SettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TicksPerSecondTrackBar)).EndInit();
            this.NamePanel.ResumeLayout(false);
            this.NamePanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel SettingsPanel;
        private System.Windows.Forms.Panel NamePanel;
        private System.Windows.Forms.Button SaveandCloseButton;
        private System.Windows.Forms.Label TpsLabel;
        private System.Windows.Forms.Label EnyoNameLabel;
        private System.Windows.Forms.TrackBar TicksPerSecondTrackBar;
        private System.Windows.Forms.CheckBox ChatOutput;
        private System.Windows.Forms.CheckBox Plugins;
        private System.Windows.Forms.CheckBox ContinuesHealingMode;
        private System.Windows.Forms.CheckBox HardLock;
        private System.Windows.Forms.Label PauseLabel;
        private System.Windows.Forms.ComboBox ModifierKeyComboBox;
        private System.Windows.Forms.ComboBox PauseKeyComboBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox SoftLock;
        private System.Windows.Forms.Label FrameLockFeaturesLabel;
        private System.Windows.Forms.Label AdditionalFeaturesLabel;
        private System.Windows.Forms.Label LoggingLabel;
        private System.Windows.Forms.CheckBox ForcedCombatMode;
        private System.Windows.Forms.CheckBox EnyoNavigator;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ClicktoMove;

    }
}