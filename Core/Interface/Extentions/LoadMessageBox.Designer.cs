namespace YourRaidingBuddy.Interfaces.GUI.Extentions
{
    partial class LoadMessageBox
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
            this.LoadSpecializationSettingsButton = new ChromeButton();
            this.LoadHotkeySettingsButton = new ChromeButton();
            this.LoadGeneralSettingsButton = new ChromeButton();
            this.FuryUnleashedMessageBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // FuryUnleashedMessageBox
            // 
            this.FuryUnleashedMessageBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.FuryUnleashedMessageBox.BorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.FuryUnleashedMessageBox.Controls.Add(this.LogoPictureBox);
            this.FuryUnleashedMessageBox.Controls.Add(this.LoadSpecializationSettingsButton);
            this.FuryUnleashedMessageBox.Controls.Add(this.LoadHotkeySettingsButton);
            this.FuryUnleashedMessageBox.Controls.Add(this.LoadGeneralSettingsButton);
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
            this.LogoPictureBox.TabIndex = 3;
            this.LogoPictureBox.TabStop = false;
            this.LogoPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.YRB_GuiDragDrop);
            // 
            // LoadSpecializationSettingsButton
            // 
            this.LoadSpecializationSettingsButton.Customization = "7e3t//Ly8v/r6+v/5ubm/+vr6//f39//p6en/zw8PP88PDz/PDw8/w==";
            this.LoadSpecializationSettingsButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.LoadSpecializationSettingsButton.Image = null;
            this.LoadSpecializationSettingsButton.Location = new System.Drawing.Point(12, 112);
            this.LoadSpecializationSettingsButton.Name = "LoadSpecializationSettingsButton";
            this.LoadSpecializationSettingsButton.NoRounding = false;
            this.LoadSpecializationSettingsButton.Size = new System.Drawing.Size(170, 23);
            this.LoadSpecializationSettingsButton.TabIndex = 2;
            this.LoadSpecializationSettingsButton.Text = "Load Specialization Settings";
            this.LoadSpecializationSettingsButton.Transparent = false;
            this.LoadSpecializationSettingsButton.Click += new System.EventHandler(this.LoadSpecializationSettingsButton_Click);
            // 
            // LoadHotkeySettingsButton
            // 
            this.LoadHotkeySettingsButton.Customization = "7e3t//Ly8v/r6+v/5ubm/+vr6//f39//p6en/zw8PP88PDz/PDw8/w==";
            this.LoadHotkeySettingsButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.LoadHotkeySettingsButton.Image = null;
            this.LoadHotkeySettingsButton.Location = new System.Drawing.Point(12, 83);
            this.LoadHotkeySettingsButton.Name = "LoadHotkeySettingsButton";
            this.LoadHotkeySettingsButton.NoRounding = false;
            this.LoadHotkeySettingsButton.Size = new System.Drawing.Size(170, 23);
            this.LoadHotkeySettingsButton.TabIndex = 1;
            this.LoadHotkeySettingsButton.Text = "Load Hotkey Settings";
            this.LoadHotkeySettingsButton.Transparent = false;
            this.LoadHotkeySettingsButton.Click += new System.EventHandler(this.LoadHotkeySettingsButton_Click);
            // 
            // LoadGeneralSettingsButton
            // 
            this.LoadGeneralSettingsButton.Customization = "7e3t//Ly8v/r6+v/5ubm/+vr6//f39//p6en/zw8PP88PDz/PDw8/w==";
            this.LoadGeneralSettingsButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.LoadGeneralSettingsButton.Image = null;
            this.LoadGeneralSettingsButton.Location = new System.Drawing.Point(12, 54);
            this.LoadGeneralSettingsButton.Name = "LoadGeneralSettingsButton";
            this.LoadGeneralSettingsButton.NoRounding = false;
            this.LoadGeneralSettingsButton.Size = new System.Drawing.Size(170, 23);
            this.LoadGeneralSettingsButton.TabIndex = 0;
            this.LoadGeneralSettingsButton.Text = "Load General Settings";
            this.LoadGeneralSettingsButton.Transparent = false;
            this.LoadGeneralSettingsButton.Click += new System.EventHandler(this.LoadGeneralSettingsButton_Click);
            // 
            // LoadMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(194, 145);
            this.Controls.Add(this.FuryUnleashedMessageBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoadMessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MessageBox";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.FuryUnleashedMessageBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ChromeForm FuryUnleashedMessageBox;
        private ChromeButton LoadGeneralSettingsButton;
        private ChromeButton LoadSpecializationSettingsButton;
        private ChromeButton LoadHotkeySettingsButton;
        private System.Windows.Forms.PictureBox LogoPictureBox;
    }
}