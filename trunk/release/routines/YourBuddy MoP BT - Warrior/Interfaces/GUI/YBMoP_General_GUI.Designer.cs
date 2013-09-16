namespace YBMoP_BT_Warrior.Interfaces.GUI
{
    partial class YBGeneralGui
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
            this.LogoText1 = new System.Windows.Forms.Label();
            this.panellogo = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.YBStatusStrip = new System.Windows.Forms.ToolStripStatusLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelcontent1 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.numAdvLogThrottleTime = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.checkAdvancedLogging = new System.Windows.Forms.CheckBox();
            this.checkClickToMove = new System.Windows.Forms.CheckBox();
            this.checkHkChatOutput = new System.Windows.Forms.CheckBox();
            this.lencounterspecific = new System.Windows.Forms.Label();
            this.checkReshapeLifeInterrupts = new System.Windows.Forms.CheckBox();
            this.checkTreePerformance = new System.Windows.Forms.CheckBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.panellogo.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panelcontent1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAdvLogThrottleTime)).BeginInit();
            this.SuspendLayout();
            // 
            // LogoText1
            // 
            this.LogoText1.AutoSize = true;
            this.LogoText1.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogoText1.ForeColor = System.Drawing.Color.SteelBlue;
            this.LogoText1.Location = new System.Drawing.Point(4, 1);
            this.LogoText1.Name = "LogoText1";
            this.LogoText1.Size = new System.Drawing.Size(247, 36);
            this.LogoText1.TabIndex = 0;
            this.LogoText1.Text = "YourBuddy MoP BT";
            this.LogoText1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.YBGUI_Movement_MouseDown);
            // 
            // panellogo
            // 
            this.panellogo.BackColor = System.Drawing.Color.White;
            this.panellogo.Controls.Add(this.LogoText1);
            this.panellogo.Location = new System.Drawing.Point(2, 2);
            this.panellogo.Name = "panellogo";
            this.panellogo.Size = new System.Drawing.Size(260, 40);
            this.panellogo.TabIndex = 2;
            this.panellogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.YBGUI_Movement_MouseDown);
            this.panellogo.MouseLeave += new System.EventHandler(this.YBGUI_Movement_MouseLeave);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.BackColor = System.Drawing.Color.White;
            this.statusStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.YBStatusStrip});
            this.statusStrip1.Location = new System.Drawing.Point(2, 475);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(260, 23);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 26;
            this.statusStrip1.Text = "YBStatusStrip";
            // 
            // YBStatusStrip
            // 
            this.YBStatusStrip.Name = "YBStatusStrip";
            this.YBStatusStrip.Size = new System.Drawing.Size(45, 18);
            this.YBStatusStrip.Text = "YBMoP";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.SteelBlue;
            this.label1.Location = new System.Drawing.Point(5, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Advanced Options";
            // 
            // panelcontent1
            // 
            this.panelcontent1.BackColor = System.Drawing.Color.White;
            this.panelcontent1.Controls.Add(this.numAdvLogThrottleTime);
            this.panelcontent1.Controls.Add(this.label14);
            this.panelcontent1.Controls.Add(this.label2);
            this.panelcontent1.Controls.Add(this.checkAdvancedLogging);
            this.panelcontent1.Controls.Add(this.checkClickToMove);
            this.panelcontent1.Controls.Add(this.checkHkChatOutput);
            this.panelcontent1.Controls.Add(this.lencounterspecific);
            this.panelcontent1.Controls.Add(this.checkReshapeLifeInterrupts);
            this.panelcontent1.Controls.Add(this.checkTreePerformance);
            this.panelcontent1.Controls.Add(this.buttonSave);
            this.panelcontent1.Controls.Add(this.label1);
            this.panelcontent1.Location = new System.Drawing.Point(2, 44);
            this.panelcontent1.Name = "panelcontent1";
            this.panelcontent1.Size = new System.Drawing.Size(260, 429);
            this.panelcontent1.TabIndex = 0;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(144, 241);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 15);
            this.label14.TabIndex = 60;
            this.label14.Text = "Throttle:";
            // 
            // numAdvLogThrottleTime
            // 
            this.numAdvLogThrottleTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numAdvLogThrottleTime.ForeColor = System.Drawing.Color.Black;
            this.numAdvLogThrottleTime.Location = new System.Drawing.Point(194, 237);
            this.numAdvLogThrottleTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numAdvLogThrottleTime.Name = "numAdvLogThrottleTime";
            this.numAdvLogThrottleTime.Size = new System.Drawing.Size(59, 23);
            this.numAdvLogThrottleTime.TabIndex = 59;
            this.numAdvLogThrottleTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numAdvLogThrottleTime.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numAdvLogThrottleTime.ValueChanged += new System.EventHandler(this.numAdvLogThrottleTime_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.SteelBlue;
            this.label2.Location = new System.Drawing.Point(5, 215);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 19);
            this.label2.TabIndex = 58;
            this.label2.Text = "Nom\'s Debug Options";
            // 
            // checkAdvancedLogging
            // 
            this.checkAdvancedLogging.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkAdvancedLogging.BackColor = System.Drawing.Color.Transparent;
            this.checkAdvancedLogging.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.checkAdvancedLogging.FlatAppearance.CheckedBackColor = System.Drawing.Color.SteelBlue;
            this.checkAdvancedLogging.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkAdvancedLogging.Location = new System.Drawing.Point(6, 237);
            this.checkAdvancedLogging.Name = "checkAdvancedLogging";
            this.checkAdvancedLogging.Size = new System.Drawing.Size(133, 23);
            this.checkAdvancedLogging.TabIndex = 57;
            this.checkAdvancedLogging.Text = "Advanced Logging";
            this.checkAdvancedLogging.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkAdvancedLogging.UseVisualStyleBackColor = false;
            this.checkAdvancedLogging.CheckedChanged += new System.EventHandler(this.checkAdvancedLogging_CheckedChanged);
            this.checkAdvancedLogging.MouseLeave += new System.EventHandler(this.YBGUI_Movement_MouseLeave);
            this.checkAdvancedLogging.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkAdvancedLogging_MouseMove);
            // 
            // checkClickToMove
            // 
            this.checkClickToMove.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkClickToMove.BackColor = System.Drawing.Color.Transparent;
            this.checkClickToMove.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.checkClickToMove.FlatAppearance.CheckedBackColor = System.Drawing.Color.SteelBlue;
            this.checkClickToMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkClickToMove.Location = new System.Drawing.Point(6, 107);
            this.checkClickToMove.Name = "checkClickToMove";
            this.checkClickToMove.Size = new System.Drawing.Size(247, 23);
            this.checkClickToMove.TabIndex = 56;
            this.checkClickToMove.Text = "Disable Click-To-Move on Startup";
            this.checkClickToMove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkClickToMove.UseVisualStyleBackColor = false;
            this.checkClickToMove.CheckedChanged += new System.EventHandler(this.checkClickToMove_CheckedChanged);
            this.checkClickToMove.MouseLeave += new System.EventHandler(this.YBGUI_Movement_MouseLeave);
            this.checkClickToMove.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkClickToMove_MouseMove);
            // 
            // checkHkChatOutput
            // 
            this.checkHkChatOutput.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkHkChatOutput.BackColor = System.Drawing.Color.Transparent;
            this.checkHkChatOutput.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.checkHkChatOutput.FlatAppearance.CheckedBackColor = System.Drawing.Color.SteelBlue;
            this.checkHkChatOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkHkChatOutput.Location = new System.Drawing.Point(6, 136);
            this.checkHkChatOutput.Name = "checkHkChatOutput";
            this.checkHkChatOutput.Size = new System.Drawing.Size(247, 23);
            this.checkHkChatOutput.TabIndex = 55;
            this.checkHkChatOutput.Text = "Enable Hotkey WoW ChatOutput";
            this.checkHkChatOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkHkChatOutput.UseVisualStyleBackColor = false;
            this.checkHkChatOutput.CheckedChanged += new System.EventHandler(this.checkHkChatOutput_CheckedChanged);
            this.checkHkChatOutput.MouseLeave += new System.EventHandler(this.YBGUI_Movement_MouseLeave);
            this.checkHkChatOutput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkHkChatOutput_MouseMove);
            // 
            // lencounterspecific
            // 
            this.lencounterspecific.AutoSize = true;
            this.lencounterspecific.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lencounterspecific.ForeColor = System.Drawing.Color.SteelBlue;
            this.lencounterspecific.Location = new System.Drawing.Point(5, 8);
            this.lencounterspecific.Name = "lencounterspecific";
            this.lencounterspecific.Size = new System.Drawing.Size(136, 19);
            this.lencounterspecific.TabIndex = 54;
            this.lencounterspecific.Text = "Encounter Specific";
            // 
            // checkReshapeLifeInterrupts
            // 
            this.checkReshapeLifeInterrupts.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkReshapeLifeInterrupts.BackColor = System.Drawing.Color.Transparent;
            this.checkReshapeLifeInterrupts.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.checkReshapeLifeInterrupts.FlatAppearance.CheckedBackColor = System.Drawing.Color.SteelBlue;
            this.checkReshapeLifeInterrupts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkReshapeLifeInterrupts.Location = new System.Drawing.Point(6, 34);
            this.checkReshapeLifeInterrupts.Name = "checkReshapeLifeInterrupts";
            this.checkReshapeLifeInterrupts.Size = new System.Drawing.Size(247, 23);
            this.checkReshapeLifeInterrupts.TabIndex = 53;
            this.checkReshapeLifeInterrupts.Text = "Amber-Shaper Un\'sok Specifics";
            this.checkReshapeLifeInterrupts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkReshapeLifeInterrupts.UseVisualStyleBackColor = false;
            this.checkReshapeLifeInterrupts.CheckedChanged += new System.EventHandler(this.checkReshapeLifeInterrupts_CheckedChanged);
            this.checkReshapeLifeInterrupts.MouseLeave += new System.EventHandler(this.YBGUI_Movement_MouseLeave);
            this.checkReshapeLifeInterrupts.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkReshapeLifeInterrupts_MouseMove);
            // 
            // checkTreePerformance
            // 
            this.checkTreePerformance.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkTreePerformance.BackColor = System.Drawing.Color.Transparent;
            this.checkTreePerformance.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.checkTreePerformance.FlatAppearance.CheckedBackColor = System.Drawing.Color.SteelBlue;
            this.checkTreePerformance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkTreePerformance.Location = new System.Drawing.Point(6, 266);
            this.checkTreePerformance.Name = "checkTreePerformance";
            this.checkTreePerformance.Size = new System.Drawing.Size(247, 23);
            this.checkTreePerformance.TabIndex = 52;
            this.checkTreePerformance.Text = "TreePerformance Timer";
            this.checkTreePerformance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkTreePerformance.UseVisualStyleBackColor = false;
            this.checkTreePerformance.CheckedChanged += new System.EventHandler(this.checkTreePerformance_CheckedChanged);
            this.checkTreePerformance.MouseLeave += new System.EventHandler(this.YBGUI_Movement_MouseLeave);
            this.checkTreePerformance.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkTreePerformance_MouseMove);
            // 
            // buttonSave
            // 
            this.buttonSave.BackColor = System.Drawing.Color.Transparent;
            this.buttonSave.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSave.Location = new System.Drawing.Point(6, 389);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(247, 23);
            this.buttonSave.TabIndex = 36;
            this.buttonSave.Text = "Save and Close";
            this.buttonSave.UseVisualStyleBackColor = false;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // YBGeneralGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(264, 500);
            this.ControlBox = false;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panellogo);
            this.Controls.Add(this.panelcontent1);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "YBGeneralGui";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "YourBuddy MoP Graphical User Interface - By Team Random";
            this.Load += new System.EventHandler(this.YBMoP_GUI_Load);
            this.panellogo.ResumeLayout(false);
            this.panellogo.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelcontent1.ResumeLayout(false);
            this.panelcontent1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAdvLogThrottleTime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LogoText1;
        private System.Windows.Forms.Panel panellogo;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel YBStatusStrip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelcontent1;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.CheckBox checkTreePerformance;
        private System.Windows.Forms.Label lencounterspecific;
        private System.Windows.Forms.CheckBox checkReshapeLifeInterrupts;
        private System.Windows.Forms.CheckBox checkHkChatOutput;
        private System.Windows.Forms.CheckBox checkClickToMove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkAdvancedLogging;
        private System.Windows.Forms.NumericUpDown numAdvLogThrottleTime;
        private System.Windows.Forms.Label label14;
    }
}