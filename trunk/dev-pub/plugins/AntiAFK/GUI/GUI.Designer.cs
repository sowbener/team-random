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
            this.label3 = new System.Windows.Forms.Label();
            this.ginfocheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.msnumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // keydropdown
            // 
            this.keydropdown.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keydropdown.FormattingEnabled = true;
            this.keydropdown.Location = new System.Drawing.Point(12, 25);
            this.keydropdown.Name = "keydropdown";
            this.keydropdown.Size = new System.Drawing.Size(195, 24);
            this.keydropdown.TabIndex = 0;
            this.keydropdown.SelectedIndexChanged += new System.EventHandler(this.keydropdown_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select key to press!";
            // 
            // msnumeric
            // 
            this.msnumeric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.msnumeric.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msnumeric.Location = new System.Drawing.Point(12, 126);
            this.msnumeric.Maximum = new decimal(new int[] {
            1800000,
            0,
            0,
            0});
            this.msnumeric.Name = "msnumeric";
            this.msnumeric.Size = new System.Drawing.Size(195, 21);
            this.msnumeric.TabIndex = 2;
            this.msnumeric.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.msnumeric.Value = new decimal(new int[] {
            180000,
            0,
            0,
            0});
            this.msnumeric.ValueChanged += new System.EventHandler(this.msnumeric_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(12, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(202, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select amount of MS between action";
            // 
            // savebutton
            // 
            this.savebutton.Location = new System.Drawing.Point(12, 153);
            this.savebutton.Name = "savebutton";
            this.savebutton.Size = new System.Drawing.Size(195, 23);
            this.savebutton.TabIndex = 4;
            this.savebutton.Text = "Save and Close";
            this.savebutton.UseVisualStyleBackColor = true;
            this.savebutton.Click += new System.EventHandler(this.savebutton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Or let the plugin type /ginfo";
            // 
            // ginfocheckbox
            // 
            this.ginfocheckbox.AutoSize = true;
            this.ginfocheckbox.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ginfocheckbox.Location = new System.Drawing.Point(15, 69);
            this.ginfocheckbox.Name = "ginfocheckbox";
            this.ginfocheckbox.Size = new System.Drawing.Size(167, 20);
            this.ginfocheckbox.TabIndex = 6;
            this.ginfocheckbox.Text = "Use /GINFO instead of key";
            this.ginfocheckbox.UseVisualStyleBackColor = true;
            this.ginfocheckbox.CheckedChanged += new System.EventHandler(this.ginfocheckbox_CheckedChanged);
            // 
            // AntiAfkGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 186);
            this.Controls.Add(this.ginfocheckbox);
            this.Controls.Add(this.label3);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ginfocheckbox;
    }
}