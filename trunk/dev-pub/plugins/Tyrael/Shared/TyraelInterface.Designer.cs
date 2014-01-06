namespace Tyrael.Shared
{
    partial class TyraelInterface
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
            this.TPSTrackBar = new System.Windows.Forms.TrackBar();
            this.checkClicktoMove = new System.Windows.Forms.CheckBox();
            this.comboPauseKey = new System.Windows.Forms.ComboBox();
            this.comboModifierKey = new System.Windows.Forms.ComboBox();
            this.PauseLabel = new System.Windows.Forms.Label();
            this.checkPlugins = new System.Windows.Forms.CheckBox();
            this.checkHealingMode = new System.Windows.Forms.CheckBox();
            this.checkFrameLock = new System.Windows.Forms.CheckBox();
            this.checkChatOutput = new System.Windows.Forms.CheckBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.NamePanel = new System.Windows.Forms.Panel();
            this.LabelName = new System.Windows.Forms.Label();
            this.TpsLabel = new System.Windows.Forms.Label();
            this.checkMinify = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.slowbutton = new System.Windows.Forms.Button();
            this.normalbutton = new System.Windows.Forms.Button();
            this.extremebutton = new System.Windows.Forms.Button();
            this.SettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TPSTrackBar)).BeginInit();
            this.NamePanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // SettingsPanel
            // 
            this.SettingsPanel.BackColor = System.Drawing.Color.White;
            this.SettingsPanel.Controls.Add(this.TPSTrackBar);
            this.SettingsPanel.Location = new System.Drawing.Point(2, 45);
            this.SettingsPanel.Name = "SettingsPanel";
            this.SettingsPanel.Size = new System.Drawing.Size(507, 50);
            this.SettingsPanel.TabIndex = 0;
            // 
            // TPSTrackBar
            // 
            this.TPSTrackBar.BackColor = System.Drawing.Color.White;
            this.TPSTrackBar.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TPSTrackBar.Location = new System.Drawing.Point(24, 2);
            this.TPSTrackBar.Maximum = 100;
            this.TPSTrackBar.Minimum = 5;
            this.TPSTrackBar.Name = "TPSTrackBar";
            this.TPSTrackBar.Size = new System.Drawing.Size(449, 45);
            this.TPSTrackBar.SmallChange = 10;
            this.TPSTrackBar.TabIndex = 0;
            this.TPSTrackBar.TickFrequency = 5;
            this.TPSTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.TPSTrackBar.Value = 30;
            this.TPSTrackBar.Scroll += new System.EventHandler(this.TPSTrackBar_Scroll);
            // 
            // checkClicktoMove
            // 
            this.checkClicktoMove.AutoSize = true;
            this.checkClicktoMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkClicktoMove.ForeColor = System.Drawing.Color.DodgerBlue;
            this.checkClicktoMove.Location = new System.Drawing.Point(10, 23);
            this.checkClicktoMove.Name = "checkClicktoMove";
            this.checkClicktoMove.Size = new System.Drawing.Size(162, 21);
            this.checkClicktoMove.TabIndex = 8;
            this.checkClicktoMove.Text = "Enable Click to Move";
            this.checkClicktoMove.UseVisualStyleBackColor = false;
            this.checkClicktoMove.CheckedChanged += new System.EventHandler(this.checkClicktoMove_CheckedChanged);
            this.checkClicktoMove.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            this.checkClicktoMove.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkClicktoMove_MouseMove);
            // 
            // comboPauseKey
            // 
            this.comboPauseKey.BackColor = System.Drawing.Color.White;
            this.comboPauseKey.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboPauseKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPauseKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboPauseKey.ForeColor = System.Drawing.Color.DodgerBlue;
            this.comboPauseKey.FormattingEnabled = true;
            this.comboPauseKey.Location = new System.Drawing.Point(13, 59);
            this.comboPauseKey.Name = "comboPauseKey";
            this.comboPauseKey.Size = new System.Drawing.Size(229, 25);
            this.comboPauseKey.TabIndex = 7;
            this.comboPauseKey.SelectedIndexChanged += new System.EventHandler(this.comboPauseKey_SelectedIndexChanged);
            this.comboPauseKey.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            // 
            // comboModifierKey
            // 
            this.comboModifierKey.BackColor = System.Drawing.Color.White;
            this.comboModifierKey.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboModifierKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboModifierKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboModifierKey.ForeColor = System.Drawing.Color.DodgerBlue;
            this.comboModifierKey.FormattingEnabled = true;
            this.comboModifierKey.Location = new System.Drawing.Point(13, 22);
            this.comboModifierKey.Name = "comboModifierKey";
            this.comboModifierKey.Size = new System.Drawing.Size(229, 25);
            this.comboModifierKey.TabIndex = 6;
            this.comboModifierKey.SelectedIndexChanged += new System.EventHandler(this.comboModifierKey_SelectedIndexChanged);
            this.comboModifierKey.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            // 
            // PauseLabel
            // 
            this.PauseLabel.AutoSize = true;
            this.PauseLabel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.PauseLabel.Location = new System.Drawing.Point(10, 3);
            this.PauseLabel.Name = "PauseLabel";
            this.PauseLabel.Size = new System.Drawing.Size(227, 17);
            this.PauseLabel.TabIndex = 5;
            this.PauseLabel.Text = "Select Pause Hotkey Combination";
            this.PauseLabel.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            // 
            // checkPlugins
            // 
            this.checkPlugins.AutoSize = true;
            this.checkPlugins.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkPlugins.ForeColor = System.Drawing.Color.DodgerBlue;
            this.checkPlugins.Location = new System.Drawing.Point(10, 103);
            this.checkPlugins.Name = "checkPlugins";
            this.checkPlugins.Size = new System.Drawing.Size(116, 21);
            this.checkPlugins.TabIndex = 4;
            this.checkPlugins.Text = "Enable Plugins";
            this.checkPlugins.UseVisualStyleBackColor = false;
            this.checkPlugins.CheckedChanged += new System.EventHandler(this.checkPlugins_CheckedChanged);
            this.checkPlugins.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            this.checkPlugins.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkPlugins_MouseMove);
            // 
            // checkHealingMode
            // 
            this.checkHealingMode.AutoSize = true;
            this.checkHealingMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkHealingMode.ForeColor = System.Drawing.Color.DodgerBlue;
            this.checkHealingMode.Location = new System.Drawing.Point(10, 43);
            this.checkHealingMode.Name = "checkHealingMode";
            this.checkHealingMode.Size = new System.Drawing.Size(240, 21);
            this.checkHealingMode.TabIndex = 3;
            this.checkHealingMode.Text = "Enable Continuous Healing Mode";
            this.checkHealingMode.UseVisualStyleBackColor = false;
            this.checkHealingMode.CheckedChanged += new System.EventHandler(this.checkHealingMode_CheckedChanged);
            this.checkHealingMode.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            this.checkHealingMode.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkHealingMode_MouseMove);
            // 
            // checkFrameLock
            // 
            this.checkFrameLock.AutoSize = true;
            this.checkFrameLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkFrameLock.ForeColor = System.Drawing.Color.DodgerBlue;
            this.checkFrameLock.Location = new System.Drawing.Point(10, 63);
            this.checkFrameLock.Name = "checkFrameLock";
            this.checkFrameLock.Size = new System.Drawing.Size(139, 21);
            this.checkFrameLock.TabIndex = 2;
            this.checkFrameLock.Text = "Enable Framelock";
            this.checkFrameLock.UseVisualStyleBackColor = false;
            this.checkFrameLock.CheckedChanged += new System.EventHandler(this.checkFrameLock_CheckedChanged);
            this.checkFrameLock.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            this.checkFrameLock.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkFrameLock_MouseMove);
            // 
            // checkChatOutput
            // 
            this.checkChatOutput.AutoSize = true;
            this.checkChatOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkChatOutput.ForeColor = System.Drawing.Color.DodgerBlue;
            this.checkChatOutput.Location = new System.Drawing.Point(10, 3);
            this.checkChatOutput.Name = "checkChatOutput";
            this.checkChatOutput.Size = new System.Drawing.Size(149, 21);
            this.checkChatOutput.TabIndex = 1;
            this.checkChatOutput.Text = "Enable Chatoutput";
            this.checkChatOutput.UseVisualStyleBackColor = false;
            this.checkChatOutput.CheckedChanged += new System.EventHandler(this.checkChatOutput_CheckedChanged);
            this.checkChatOutput.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            this.checkChatOutput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkChatOutput_MouseMove);
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.Color.DodgerBlue;
            this.SaveButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.Location = new System.Drawing.Point(374, 263);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(135, 22);
            this.SaveButton.TabIndex = 0;
            this.SaveButton.Text = "Save and Close";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            this.SaveButton.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            this.SaveButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SaveButton_MouseMove);
            // 
            // NamePanel
            // 
            this.NamePanel.BackColor = System.Drawing.Color.White;
            this.NamePanel.Controls.Add(this.LabelName);
            this.NamePanel.Location = new System.Drawing.Point(2, 2);
            this.NamePanel.Name = "NamePanel";
            this.NamePanel.Size = new System.Drawing.Size(507, 41);
            this.NamePanel.TabIndex = 0;
            this.NamePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GuiDragDrop);
            // 
            // LabelName
            // 
            this.LabelName.AutoSize = true;
            this.LabelName.Font = new System.Drawing.Font("Century Gothic", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelName.ForeColor = System.Drawing.Color.DodgerBlue;
            this.LabelName.Location = new System.Drawing.Point(85, 2);
            this.LabelName.Name = "LabelName";
            this.LabelName.Size = new System.Drawing.Size(354, 36);
            this.LabelName.TabIndex = 0;
            this.LabelName.Text = "Tyrael - Raiding BotBase";
            this.LabelName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GuiDragDrop);
            // 
            // TpsLabel
            // 
            this.TpsLabel.AutoSize = true;
            this.TpsLabel.BackColor = System.Drawing.Color.Transparent;
            this.TpsLabel.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TpsLabel.ForeColor = System.Drawing.Color.White;
            this.TpsLabel.Location = new System.Drawing.Point(9, 265);
            this.TpsLabel.Name = "TpsLabel";
            this.TpsLabel.Size = new System.Drawing.Size(43, 17);
            this.TpsLabel.TabIndex = 1;
            this.TpsLabel.Text = "Tyrael";
            // 
            // checkMinify
            // 
            this.checkMinify.AutoSize = true;
            this.checkMinify.Enabled = false;
            this.checkMinify.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkMinify.ForeColor = System.Drawing.Color.DodgerBlue;
            this.checkMinify.Location = new System.Drawing.Point(10, 83);
            this.checkMinify.Name = "checkMinify";
            this.checkMinify.Size = new System.Drawing.Size(107, 21);
            this.checkMinify.TabIndex = 9;
            this.checkMinify.Text = "Enable Minify";
            this.checkMinify.UseVisualStyleBackColor = false;
            this.checkMinify.CheckedChanged += new System.EventHandler(this.checkMinify_CheckedChanged);
            this.checkMinify.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            this.checkMinify.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkMinify_MouseMove);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.checkMinify);
            this.panel1.Controls.Add(this.checkChatOutput);
            this.panel1.Controls.Add(this.checkClicktoMove);
            this.panel1.Controls.Add(this.checkHealingMode);
            this.panel1.Controls.Add(this.checkFrameLock);
            this.panel1.Controls.Add(this.checkPlugins);
            this.panel1.Location = new System.Drawing.Point(2, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(253, 130);
            this.panel1.TabIndex = 10;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.comboPauseKey);
            this.panel2.Controls.Add(this.PauseLabel);
            this.panel2.Controls.Add(this.comboModifierKey);
            this.panel2.Location = new System.Drawing.Point(257, 97);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(252, 130);
            this.panel2.TabIndex = 11;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.slowbutton);
            this.panel3.Controls.Add(this.normalbutton);
            this.panel3.Controls.Add(this.extremebutton);
            this.panel3.Location = new System.Drawing.Point(2, 229);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(507, 32);
            this.panel3.TabIndex = 12;
            // 
            // slowbutton
            // 
            this.slowbutton.BackColor = System.Drawing.Color.White;
            this.slowbutton.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.slowbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.slowbutton.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.slowbutton.ForeColor = System.Drawing.Color.DodgerBlue;
            this.slowbutton.Location = new System.Drawing.Point(3, 3);
            this.slowbutton.Name = "slowbutton";
            this.slowbutton.Size = new System.Drawing.Size(162, 26);
            this.slowbutton.TabIndex = 14;
            this.slowbutton.Text = "Slow Performance";
            this.slowbutton.UseVisualStyleBackColor = false;
            this.slowbutton.Click += new System.EventHandler(this.slowbutton_Click);
            this.slowbutton.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            this.slowbutton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.slowbutton_MouseMove);
            // 
            // normalbutton
            // 
            this.normalbutton.BackColor = System.Drawing.Color.White;
            this.normalbutton.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.normalbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.normalbutton.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.normalbutton.ForeColor = System.Drawing.Color.DodgerBlue;
            this.normalbutton.Location = new System.Drawing.Point(173, 3);
            this.normalbutton.Name = "normalbutton";
            this.normalbutton.Size = new System.Drawing.Size(162, 26);
            this.normalbutton.TabIndex = 14;
            this.normalbutton.Text = "Normal Performance";
            this.normalbutton.UseVisualStyleBackColor = false;
            this.normalbutton.Click += new System.EventHandler(this.normalbutton_Click);
            this.normalbutton.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            this.normalbutton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.normalbutton_MouseMove);
            // 
            // extremebutton
            // 
            this.extremebutton.BackColor = System.Drawing.Color.White;
            this.extremebutton.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.extremebutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.extremebutton.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.extremebutton.ForeColor = System.Drawing.Color.DodgerBlue;
            this.extremebutton.Location = new System.Drawing.Point(342, 3);
            this.extremebutton.Name = "extremebutton";
            this.extremebutton.Size = new System.Drawing.Size(162, 26);
            this.extremebutton.TabIndex = 13;
            this.extremebutton.Text = "Extreme Performance";
            this.extremebutton.UseVisualStyleBackColor = false;
            this.extremebutton.Click += new System.EventHandler(this.extremebutton_Click);
            this.extremebutton.MouseLeave += new System.EventHandler(this.TPSTrackBar_Scroll);
            this.extremebutton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.extremebutton_MouseMove);
            // 
            // TyraelInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.ClientSize = new System.Drawing.Size(511, 287);
            this.ControlBox = false;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.TpsLabel);
            this.Controls.Add(this.NamePanel);
            this.Controls.Add(this.SettingsPanel);
            this.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TyraelInterface";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tyrael GUI";
            this.SettingsPanel.ResumeLayout(false);
            this.SettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TPSTrackBar)).EndInit();
            this.NamePanel.ResumeLayout(false);
            this.NamePanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel SettingsPanel;
        private System.Windows.Forms.Panel NamePanel;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label TpsLabel;
        private System.Windows.Forms.Label LabelName;
        private System.Windows.Forms.TrackBar TPSTrackBar;
        private System.Windows.Forms.CheckBox checkChatOutput;
        private System.Windows.Forms.CheckBox checkPlugins;
        private System.Windows.Forms.CheckBox checkHealingMode;
        private System.Windows.Forms.CheckBox checkFrameLock;
        private System.Windows.Forms.Label PauseLabel;
        private System.Windows.Forms.ComboBox comboModifierKey;
        private System.Windows.Forms.ComboBox comboPauseKey;
        private System.Windows.Forms.CheckBox checkClicktoMove;
        private System.Windows.Forms.CheckBox checkMinify;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button extremebutton;
        private System.Windows.Forms.Button slowbutton;
        private System.Windows.Forms.Button normalbutton;

    }
}