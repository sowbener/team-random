namespace YourRaidingBuddy.Interfaces.GUI.Extentions
{
    partial class SaveMessageBox
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
            this.FuryUnleashedMessageBox = new ChromeForm();
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            this.SaveSpecializationSettingsButton = new ChromeButton();
            this.SaveHotkeySettingsButton = new ChromeButton();
            this.SaveGeneralSettingsButton = new ChromeButton();
            this.FuryUnleashedMessageBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // FuryUnleashedMessageBox
            // 
            this.FuryUnleashedMessageBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.FuryUnleashedMessageBox.BorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.FuryUnleashedMessageBox.Controls.Add(this.LogoPictureBox);
            this.FuryUnleashedMessageBox.Controls.Add(this.SaveSpecializationSettingsButton);
            this.FuryUnleashedMessageBox.Controls.Add(this.SaveHotkeySettingsButton);
            this.FuryUnleashedMessageBox.Controls.Add(this.SaveGeneralSettingsButton);
            this.FuryUnleashedMessageBox.Customization = "AAAA/1paWv9ycnL/";
            this.FuryUnleashedMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FuryUnleashedMessageBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FuryUnleashedMessageBox.Image = null;
            this.FuryUnleashedMessageBox.Location = new System.Drawing.Point(0, 0);
            this.FuryUnleashedMessageBox.Movable = true;
            this.FuryUnleashedMessageBox.Name = "FuryUnleashedMessageBox";
            this.FuryUnleashedMessageBox.NoRounding = false;
            this.FuryUnleashedMessageBox.Sizable = true;
            this.FuryUnleashedMessageBox.Size = new System.Drawing.Size(194, 145);
            this.FuryUnleashedMessageBox.SmartBounds = true;
            this.FuryUnleashedMessageBox.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FuryUnleashedMessageBox.TabIndex = 0;
            this.FuryUnleashedMessageBox.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.FuryUnleashedMessageBox.Transparent = false;
            // 
            // LogoPictureBox
            // 
            this.LogoPictureBox.Location = new System.Drawing.Point(0, 24);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new System.Drawing.Size(194, 24);
            this.LogoPictureBox.TabIndex = 4;
            this.LogoPictureBox.TabStop = false;
            this.LogoPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.YRB_GuiDragDrop);
            // 
            // SaveSpecializationSettingsButton
            // 
            this.SaveSpecializationSettingsButton.Customization = "7e3t//Ly8v/r6+v/5ubm/+vr6//f39//p6en/zw8PP88PDz/PDw8/w==";
            this.SaveSpecializationSettingsButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.SaveSpecializationSettingsButton.Image = null;
            this.SaveSpecializationSettingsButton.Location = new System.Drawing.Point(12, 112);
            this.SaveSpecializationSettingsButton.Name = "SaveSpecializationSettingsButton";
            this.SaveSpecializationSettingsButton.NoRounding = false;
            this.SaveSpecializationSettingsButton.Size = new System.Drawing.Size(170, 23);
            this.SaveSpecializationSettingsButton.TabIndex = 2;
            this.SaveSpecializationSettingsButton.Text = "Save Specialization Settings";
            this.SaveSpecializationSettingsButton.Transparent = false;
            this.SaveSpecializationSettingsButton.Click += new System.EventHandler(this.SaveSpecializationSettingsButton_Click);
            // 
            // SaveHotkeySettingsButton
            // 
            this.SaveHotkeySettingsButton.Customization = "7e3t//Ly8v/r6+v/5ubm/+vr6//f39//p6en/zw8PP88PDz/PDw8/w==";
            this.SaveHotkeySettingsButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.SaveHotkeySettingsButton.Image = null;
            this.SaveHotkeySettingsButton.Location = new System.Drawing.Point(12, 83);
            this.SaveHotkeySettingsButton.Name = "SaveHotkeySettingsButton";
            this.SaveHotkeySettingsButton.NoRounding = false;
            this.SaveHotkeySettingsButton.Size = new System.Drawing.Size(170, 23);
            this.SaveHotkeySettingsButton.TabIndex = 1;
            this.SaveHotkeySettingsButton.Text = "Save Hotkey Settings";
            this.SaveHotkeySettingsButton.Transparent = false;
            this.SaveHotkeySettingsButton.Click += new System.EventHandler(this.SaveHotkeySettingsButton_Click);
            // 
            // SaveGeneralSettingsButton
            // 
            this.SaveGeneralSettingsButton.Customization = "7e3t//Ly8v/r6+v/5ubm/+vr6//f39//p6en/zw8PP88PDz/PDw8/w==";
            this.SaveGeneralSettingsButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.SaveGeneralSettingsButton.Image = null;
            this.SaveGeneralSettingsButton.Location = new System.Drawing.Point(12, 54);
            this.SaveGeneralSettingsButton.Name = "SaveGeneralSettingsButton";
            this.SaveGeneralSettingsButton.NoRounding = false;
            this.SaveGeneralSettingsButton.Size = new System.Drawing.Size(170, 23);
            this.SaveGeneralSettingsButton.TabIndex = 0;
            this.SaveGeneralSettingsButton.Text = "Save General Settings";
            this.SaveGeneralSettingsButton.Transparent = false;
            this.SaveGeneralSettingsButton.Click += new System.EventHandler(this.SaveGeneralSettingsButton_Click);
            // 
            // SaveMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.ClientSize = new System.Drawing.Size(194, 145);
            this.Controls.Add(this.FuryUnleashedMessageBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SaveMessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MessageBox";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.FuryUnleashedMessageBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ChromeForm FuryUnleashedMessageBox;
        private ChromeButton SaveGeneralSettingsButton;
        private ChromeButton SaveSpecializationSettingsButton;
        private ChromeButton SaveHotkeySettingsButton;
        private System.Windows.Forms.PictureBox LogoPictureBox;
    }
}