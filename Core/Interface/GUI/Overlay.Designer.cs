namespace YourRaidingBuddy.Overlay
{
    partial class Overlay
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
            this.lblManual = new System.Windows.Forms.Label();
            this.lblCoold = new System.Windows.Forms.Label();
            this.lblAoE = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblManual
            // 
            this.lblManual.AutoSize = true;
            this.lblManual.BackColor = System.Drawing.Color.Transparent;
            this.lblManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblManual.ForeColor = System.Drawing.Color.Red;
            this.lblManual.Location = new System.Drawing.Point(137, 44);
            this.lblManual.Name = "lblManual";
            this.lblManual.Size = new System.Drawing.Size(107, 17);
            this.lblManual.TabIndex = 9;
            this.lblManual.Text = "Pause Disabled";
            this.lblManual.Click += new System.EventHandler(this.lblManual_Click);
            // 
            // lblCoold
            // 
            this.lblCoold.AutoSize = true;
            this.lblCoold.BackColor = System.Drawing.Color.Transparent;
            this.lblCoold.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCoold.ForeColor = System.Drawing.Color.Red;
            this.lblCoold.Location = new System.Drawing.Point(12, 61);
            this.lblCoold.Name = "lblCoold";
            this.lblCoold.Size = new System.Drawing.Size(135, 17);
            this.lblCoold.TabIndex = 7;
            this.lblCoold.Text = "Cooldowns Disabled";
            this.lblCoold.Click += new System.EventHandler(this.lblCoold_Click);
            // 
            // lblAoE
            // 
            this.lblAoE.AutoSize = true;
            this.lblAoE.BackColor = System.Drawing.Color.Transparent;
            this.lblAoE.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblAoE.ForeColor = System.Drawing.Color.Red;
            this.lblAoE.Location = new System.Drawing.Point(12, 44);
            this.lblAoE.Name = "lblAoE";
            this.lblAoE.Size = new System.Drawing.Size(93, 17);
            this.lblAoE.TabIndex = 5;
            this.lblAoE.Text = "AoE Disabled";
            this.lblAoE.Click += new System.EventHandler(this.lblAoE_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(228, 24);
            this.label1.TabIndex = 10;
            this.label1.Text = "YourRaidingBuddy FFXIV";
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblMain_MouseDown);
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblMain_MouseMove);
            this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblMain_MouseUp);
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(250, 134);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblManual);
            this.Controls.Add(this.lblCoold);
            this.Controls.Add(this.lblAoE);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Overlay";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmOv";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Black;
            this.Load += new System.EventHandler(this.Overlay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblManual;
        public System.Windows.Forms.Label lblCoold;
        public System.Windows.Forms.Label lblAoE;
        private System.Windows.Forms.Label label1;
    }
}