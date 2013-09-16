﻿namespace Shammy.Interfaces.GUI
{
    partial class SmInterface
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
            this.GeneralGrid = new System.Windows.Forms.PropertyGrid();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.LogoPicture = new System.Windows.Forms.PictureBox();
            this.HotkeyPanel = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ComboHkPause = new System.Windows.Forms.ComboBox();
            this.ComboHkMultiTgt = new System.Windows.Forms.ComboBox();
            this.ComboHkCooldown = new System.Windows.Forms.ComboBox();
            this.ComboHkModifier = new System.Windows.Forms.ComboBox();
            this.ComboHkMode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ComboHkTier6 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ComboHkAMZ = new System.Windows.Forms.ComboBox();
            this.ComboHkSpecialKey = new System.Windows.Forms.ComboBox();
            this.SpecGrid = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPicture)).BeginInit();
            this.HotkeyPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // GeneralGrid
            // 
            this.GeneralGrid.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.GeneralGrid.CommandsLinkColor = System.Drawing.Color.Black;
            this.GeneralGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.GeneralGrid.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.GeneralGrid.Location = new System.Drawing.Point(4, 254);
            this.GeneralGrid.Name = "GeneralGrid";
            this.GeneralGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.GeneralGrid.SelectedItemWithFocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.GeneralGrid.SelectedItemWithFocusForeColor = System.Drawing.Color.Black;
            this.GeneralGrid.Size = new System.Drawing.Size(308, 372);
            this.GeneralGrid.TabIndex = 1;
            this.GeneralGrid.ToolbarVisible = false;
            this.GeneralGrid.ViewBackColor = System.Drawing.Color.White;
            this.GeneralGrid.ViewBorderColor = System.Drawing.Color.White;
            // 
            // StatusStrip
            // 
            this.StatusStrip.AutoSize = false;
            this.StatusStrip.BackColor = System.Drawing.Color.White;
            this.StatusStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.StatusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.StatusStrip.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.StatusStrip.Location = new System.Drawing.Point(3, 634);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            this.StatusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.StatusStrip.Size = new System.Drawing.Size(928, 24);
            this.StatusStrip.SizingGrip = false;
            this.StatusStrip.TabIndex = 2;
            this.StatusStrip.Text = "Shammy";
            this.StatusStrip.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Sm_GuiDragDrop);
            // 
            // LogoPicture
            // 
            this.LogoPicture.BackColor = System.Drawing.Color.White;
            this.LogoPicture.Location = new System.Drawing.Point(4, 2);
            this.LogoPicture.Name = "LogoPicture";
            this.LogoPicture.Size = new System.Drawing.Size(927, 253);
            this.LogoPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.LogoPicture.TabIndex = 0;
            this.LogoPicture.TabStop = false;
            this.LogoPicture.Click += new System.EventHandler(this.LogoPicture_Click);
            this.LogoPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Sm_GuiDragDrop);
            // 
            // HotkeyPanel
            // 
            this.HotkeyPanel.BackColor = System.Drawing.Color.White;
            this.HotkeyPanel.Controls.Add(this.comboBox1);
            this.HotkeyPanel.Controls.Add(this.label10);
            this.HotkeyPanel.Controls.Add(this.SaveButton);
            this.HotkeyPanel.Controls.Add(this.label9);
            this.HotkeyPanel.Controls.Add(this.label8);
            this.HotkeyPanel.Controls.Add(this.label7);
            this.HotkeyPanel.Controls.Add(this.label6);
            this.HotkeyPanel.Controls.Add(this.ComboHkPause);
            this.HotkeyPanel.Controls.Add(this.ComboHkMultiTgt);
            this.HotkeyPanel.Controls.Add(this.ComboHkCooldown);
            this.HotkeyPanel.Controls.Add(this.ComboHkModifier);
            this.HotkeyPanel.Controls.Add(this.ComboHkMode);
            this.HotkeyPanel.Controls.Add(this.label5);
            this.HotkeyPanel.Controls.Add(this.label4);
            this.HotkeyPanel.Controls.Add(this.label3);
            this.HotkeyPanel.Controls.Add(this.ComboHkTier6);
            this.HotkeyPanel.Controls.Add(this.label2);
            this.HotkeyPanel.Controls.Add(this.label1);
            this.HotkeyPanel.Controls.Add(this.ComboHkAMZ);
            this.HotkeyPanel.Location = new System.Drawing.Point(645, 254);
            this.HotkeyPanel.Name = "HotkeyPanel";
            this.HotkeyPanel.Size = new System.Drawing.Size(286, 372);
            this.HotkeyPanel.TabIndex = 3;
            // 
            // comboBox1
            // 
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.comboBox1.DropDownHeight = 110;
            this.comboBox1.DropDownWidth = 140;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.IntegralHeight = false;
            this.comboBox1.Location = new System.Drawing.Point(155, 316);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 20;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 317);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(121, 16);
            this.label10.TabIndex = 19;
            this.label10.Text = "Select Special Key";
            // 
            // SaveButton
            // 
            this.SaveButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.SaveButton.Location = new System.Drawing.Point(9, 343);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(267, 23);
            this.SaveButton.TabIndex = 5;
            this.SaveButton.Text = "Save and Close";
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(5, 293);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(114, 16);
            this.label9.TabIndex = 18;
            this.label9.Text = "Select Pause Key";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(5, 266);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(134, 16);
            this.label8.TabIndex = 17;
            this.label8.Text = "Select Multi-TGT Key";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(5, 233);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 16);
            this.label7.TabIndex = 16;
            this.label7.Text = "Select Cooldown Key";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(5, 203);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "Select Modifier Key";
            // 
            // ComboHkPause
            // 
            this.ComboHkPause.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ComboHkPause.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboHkPause.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ComboHkPause.DropDownHeight = 110;
            this.ComboHkPause.DropDownWidth = 140;
            this.ComboHkPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboHkPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ComboHkPause.FormattingEnabled = true;
            this.ComboHkPause.IntegralHeight = false;
            this.ComboHkPause.Location = new System.Drawing.Point(155, 291);
            this.ComboHkPause.Name = "ComboHkPause";
            this.ComboHkPause.Size = new System.Drawing.Size(121, 21);
            this.ComboHkPause.TabIndex = 14;
            this.ComboHkPause.SelectedIndexChanged += new System.EventHandler(this.ComboHkPause_SelectedIndexChanged);
            // 
            // ComboHkMultiTgt
            // 
            this.ComboHkMultiTgt.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ComboHkMultiTgt.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboHkMultiTgt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ComboHkMultiTgt.DropDownHeight = 110;
            this.ComboHkMultiTgt.DropDownWidth = 140;
            this.ComboHkMultiTgt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboHkMultiTgt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ComboHkMultiTgt.FormattingEnabled = true;
            this.ComboHkMultiTgt.IntegralHeight = false;
            this.ComboHkMultiTgt.Location = new System.Drawing.Point(155, 261);
            this.ComboHkMultiTgt.Name = "ComboHkMultiTgt";
            this.ComboHkMultiTgt.Size = new System.Drawing.Size(121, 21);
            this.ComboHkMultiTgt.TabIndex = 13;
            this.ComboHkMultiTgt.SelectedIndexChanged += new System.EventHandler(this.ComboHkMultiTgt_SelectedIndexChanged);
            // 
            // ComboHkCooldown
            // 
            this.ComboHkCooldown.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ComboHkCooldown.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboHkCooldown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ComboHkCooldown.DropDownHeight = 110;
            this.ComboHkCooldown.DropDownWidth = 140;
            this.ComboHkCooldown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboHkCooldown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ComboHkCooldown.FormattingEnabled = true;
            this.ComboHkCooldown.IntegralHeight = false;
            this.ComboHkCooldown.Location = new System.Drawing.Point(155, 231);
            this.ComboHkCooldown.Name = "ComboHkCooldown";
            this.ComboHkCooldown.Size = new System.Drawing.Size(121, 21);
            this.ComboHkCooldown.TabIndex = 12;
            this.ComboHkCooldown.SelectedIndexChanged += new System.EventHandler(this.ComboHkCooldown_SelectedIndexChanged);
            // 
            // ComboHkModifier
            // 
            this.ComboHkModifier.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ComboHkModifier.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboHkModifier.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ComboHkModifier.DropDownHeight = 110;
            this.ComboHkModifier.DropDownWidth = 140;
            this.ComboHkModifier.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboHkModifier.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ComboHkModifier.FormattingEnabled = true;
            this.ComboHkModifier.IntegralHeight = false;
            this.ComboHkModifier.Location = new System.Drawing.Point(155, 201);
            this.ComboHkModifier.Name = "ComboHkModifier";
            this.ComboHkModifier.Size = new System.Drawing.Size(121, 21);
            this.ComboHkModifier.TabIndex = 11;
            this.ComboHkModifier.SelectedIndexChanged += new System.EventHandler(this.ComboHkModifier_SelectedIndexChanged);
            // 
            // ComboHkMode
            // 
            this.ComboHkMode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ComboHkMode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboHkMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ComboHkMode.DropDownHeight = 110;
            this.ComboHkMode.DropDownWidth = 140;
            this.ComboHkMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboHkMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ComboHkMode.FormattingEnabled = true;
            this.ComboHkMode.IntegralHeight = false;
            this.ComboHkMode.Location = new System.Drawing.Point(155, 171);
            this.ComboHkMode.Name = "ComboHkMode";
            this.ComboHkMode.Size = new System.Drawing.Size(121, 21);
            this.ComboHkMode.TabIndex = 10;
            this.ComboHkMode.SelectedIndexChanged += new System.EventHandler(this.ComboHkMode_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(5, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "Select Hotkey Mode";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(4, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(245, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Hotkey Options - Toggle Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Tier 5 Healing Abilities";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // ComboHkTier6
            // 
            this.ComboHkTier6.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ComboHkTier6.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboHkTier6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ComboHkTier6.DropDownHeight = 110;
            this.ComboHkTier6.DropDownWidth = 140;
            this.ComboHkTier6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboHkTier6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ComboHkTier6.FormattingEnabled = true;
            this.ComboHkTier6.IntegralHeight = false;
            this.ComboHkTier6.Location = new System.Drawing.Point(155, 59);
            this.ComboHkTier6.Name = "ComboHkTier6";
            this.ComboHkTier6.Size = new System.Drawing.Size(121, 21);
            this.ComboHkTier6.TabIndex = 6;
            this.ComboHkTier6.SelectedIndexChanged += new System.EventHandler(this.ComboHkTier6_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(252, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Hotkey Options - Casting Type";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Healing Rain";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // ComboHkAMZ
            // 
            this.ComboHkAMZ.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ComboHkAMZ.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboHkAMZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ComboHkAMZ.DropDownHeight = 110;
            this.ComboHkAMZ.DropDownWidth = 140;
            this.ComboHkAMZ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboHkAMZ.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboHkAMZ.FormattingEnabled = true;
            this.ComboHkAMZ.IntegralHeight = false;
            this.ComboHkAMZ.Location = new System.Drawing.Point(155, 29);
            this.ComboHkAMZ.Name = "ComboHkAMZ";
            this.ComboHkAMZ.Size = new System.Drawing.Size(121, 21);
            this.ComboHkAMZ.TabIndex = 0;
            this.ComboHkAMZ.SelectedIndexChanged += new System.EventHandler(this.ComboHkAMZ_SelectedIndexChanged);
            // 
            // ComboHkSpecialKey
            // 
            this.ComboHkSpecialKey.Location = new System.Drawing.Point(0, 0);
            this.ComboHkSpecialKey.Name = "ComboHkSpecialKey";
            this.ComboHkSpecialKey.Size = new System.Drawing.Size(121, 21);
            this.ComboHkSpecialKey.TabIndex = 0;
            // 
            // SpecGrid
            // 
            this.SpecGrid.CommandsBorderColor = System.Drawing.Color.White;
            this.SpecGrid.CommandsLinkColor = System.Drawing.Color.Black;
            this.SpecGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.SpecGrid.HelpBorderColor = System.Drawing.Color.White;
            this.SpecGrid.Location = new System.Drawing.Point(315, 254);
            this.SpecGrid.Name = "SpecGrid";
            this.SpecGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.SpecGrid.SelectedItemWithFocusBackColor = System.Drawing.Color.DarkOrange;
            this.SpecGrid.SelectedItemWithFocusForeColor = System.Drawing.Color.Black;
            this.SpecGrid.Size = new System.Drawing.Size(329, 372);
            this.SpecGrid.TabIndex = 4;
            this.SpecGrid.ToolbarVisible = false;
            this.SpecGrid.ViewBackColor = System.Drawing.Color.White;
            this.SpecGrid.ViewBorderColor = System.Drawing.Color.White;
            this.SpecGrid.Click += new System.EventHandler(this.SpecGrid_Click);
            // 
            // SmInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(933, 667);
            this.Controls.Add(this.SpecGrid);
            this.Controls.Add(this.HotkeyPanel);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.GeneralGrid);
            this.Controls.Add(this.LogoPicture);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SmInterface";
            this.ShowIcon = false;
            this.Text = "Shammy";
            this.Load += new System.EventHandler(this.SmInterface_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Sm_GuiDragDrop);
            ((System.ComponentModel.ISupportInitialize)(this.LogoPicture)).EndInit();
            this.HotkeyPanel.ResumeLayout(false);
            this.HotkeyPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid GeneralGrid;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.PictureBox LogoPicture;
        private System.Windows.Forms.Panel HotkeyPanel;
        private System.Windows.Forms.ComboBox ComboHkAMZ;
        private System.Windows.Forms.ComboBox ComboHkTier6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ComboHkMode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox ComboHkPause;
        private System.Windows.Forms.ComboBox ComboHkSpecialKey;
        private System.Windows.Forms.ComboBox ComboHkMultiTgt;
        private System.Windows.Forms.ComboBox ComboHkCooldown;
        private System.Windows.Forms.ComboBox ComboHkModifier;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.PropertyGrid SpecGrid;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label10;
    }
}