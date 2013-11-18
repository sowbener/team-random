namespace AntiAFK.GUI
{
    partial class AntiAFKGui
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
            this.buttonsaveandclose = new System.Windows.Forms.Button();
            this.msnumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.keydropdown = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pluginscheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.msnumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonsaveandclose
            // 
            this.buttonsaveandclose.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonsaveandclose.Location = new System.Drawing.Point(12, 223);
            this.buttonsaveandclose.Name = "buttonsaveandclose";
            this.buttonsaveandclose.Size = new System.Drawing.Size(184, 23);
            this.buttonsaveandclose.TabIndex = 0;
            this.buttonsaveandclose.Text = "Save and Close";
            this.buttonsaveandclose.UseVisualStyleBackColor = true;
            this.buttonsaveandclose.Click += new System.EventHandler(this.buttonsaveandclose_Click);
            // 
            // msnumeric
            // 
            this.msnumeric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.msnumeric.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msnumeric.Location = new System.Drawing.Point(12, 28);
            this.msnumeric.Maximum = new decimal(new int[] {
            1800000,
            0,
            0,
            0});
            this.msnumeric.Name = "msnumeric";
            this.msnumeric.Size = new System.Drawing.Size(184, 23);
            this.msnumeric.TabIndex = 1;
            this.msnumeric.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.msnumeric.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.msnumeric.ValueChanged += new System.EventHandler(this.msnumeric_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Stopwatch Timer (In seconds):";
            // 
            // keydropdown
            // 
            this.keydropdown.FormattingEnabled = true;
            this.keydropdown.Location = new System.Drawing.Point(12, 77);
            this.keydropdown.Name = "keydropdown";
            this.keydropdown.Size = new System.Drawing.Size(184, 21);
            this.keydropdown.TabIndex = 3;
            this.keydropdown.SelectedIndexChanged += new System.EventHandler(this.keydropdown_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select key to press:";
            // 
            // pluginscheckbox
            // 
            this.pluginscheckbox.AutoSize = true;
            this.pluginscheckbox.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pluginscheckbox.Location = new System.Drawing.Point(12, 197);
            this.pluginscheckbox.Name = "pluginscheckbox";
            this.pluginscheckbox.Size = new System.Drawing.Size(104, 20);
            this.pluginscheckbox.TabIndex = 5;
            this.pluginscheckbox.Text = "Enable Plugins";
            this.pluginscheckbox.UseVisualStyleBackColor = true;
            this.pluginscheckbox.CheckedChanged += new System.EventHandler(this.pluginscheckbox_CheckedChanged);
            // 
            // AntiAFKGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 257);
            this.Controls.Add(this.pluginscheckbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.keydropdown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.msnumeric);
            this.Controls.Add(this.buttonsaveandclose);
            this.Name = "AntiAFKGui";
            this.Text = "AntiAFK GUI";
            this.Load += new System.EventHandler(this.AntiAFKGui_Load);
            ((System.ComponentModel.ISupportInitialize)(this.msnumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonsaveandclose;
        private System.Windows.Forms.NumericUpDown msnumeric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox keydropdown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox pluginscheckbox;
    }
}