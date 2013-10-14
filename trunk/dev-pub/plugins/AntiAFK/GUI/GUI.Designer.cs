namespace AntiAfk.GUI
{
    partial class AntiAfkGui
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
            this.keydropdown = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.msnumeric = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.savebutton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.msnumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // keydropdown
            // 
            this.keydropdown.FormattingEnabled = true;
            this.keydropdown.Location = new System.Drawing.Point(12, 25);
            this.keydropdown.Name = "keydropdown";
            this.keydropdown.Size = new System.Drawing.Size(195, 21);
            this.keydropdown.TabIndex = 0;
            this.keydropdown.SelectedIndexChanged += new System.EventHandler(this.keydropdown_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select key to press!";
            // 
            // msnumeric
            // 
            this.msnumeric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.msnumeric.Location = new System.Drawing.Point(12, 75);
            this.msnumeric.Maximum = new decimal(new int[] {
            1800000,
            0,
            0,
            0});
            this.msnumeric.Name = "msnumeric";
            this.msnumeric.Size = new System.Drawing.Size(195, 20);
            this.msnumeric.TabIndex = 2;
            this.msnumeric.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.msnumeric.Value = new decimal(new int[] {
            180000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(195, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select amount of MS between keypress";
            // 
            // savebutton
            // 
            this.savebutton.Location = new System.Drawing.Point(12, 101);
            this.savebutton.Name = "savebutton";
            this.savebutton.Size = new System.Drawing.Size(195, 23);
            this.savebutton.TabIndex = 4;
            this.savebutton.Text = "Save and Close";
            this.savebutton.UseVisualStyleBackColor = true;
            this.savebutton.Click += new System.EventHandler(this.savebutton_Click);
            // 
            // AntiAfkGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 137);
            this.Controls.Add(this.savebutton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.msnumeric);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.keydropdown);
            this.Name = "AntiAfkGui";
            this.Text = "GUI";
            ((System.ComponentModel.ISupportInitialize)(this.msnumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox keydropdown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown msnumeric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button savebutton;
    }
}