using YourRaidingBuddy.Interface.Extensions;

namespace YourRaidingBuddy.Interface.GUI
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
            this.components = new System.ComponentModel.Container();
            this.tabcontrolimagelist = new System.Windows.Forms.ImageList(this.components);
            this.chromeForm1 = new YourRaidingBuddy.Interface.Extensions.ChromeForm();
            this.SaveandCloseButton = new YourRaidingBuddy.Interface.Extensions.ChromeButton();
            this.closepicturebox = new System.Windows.Forms.PictureBox();
            this.tabcontrol = new YourRaidingBuddy.Interface.Extensions.ChromeTabcontrolNormal();
            this.tabpage1 = new System.Windows.Forms.TabPage();
            this.generalpropertygrid = new System.Windows.Forms.PropertyGrid();
            this.tabpage2 = new System.Windows.Forms.TabPage();
            this.hotkeypropertygrid = new System.Windows.Forms.PropertyGrid();
            this.tabpage7 = new System.Windows.Forms.TabPage();
            this.SettingsComposerButton = new YourRaidingBuddy.Interface.Extensions.ChromeButton();
            this.tabpage3 = new System.Windows.Forms.TabPage();
            this.spec1propertygrid = new System.Windows.Forms.PropertyGrid();
            this.tabpage4 = new System.Windows.Forms.TabPage();
            this.spec2propertygrid = new System.Windows.Forms.PropertyGrid();
            this.tabpage5 = new System.Windows.Forms.TabPage();
            this.spec3propertygrid = new System.Windows.Forms.PropertyGrid();
            this.tabpage6 = new System.Windows.Forms.TabPage();
            this.spec4propertygrid = new System.Windows.Forms.PropertyGrid();
            this.logopicturebox = new System.Windows.Forms.PictureBox();
            this.chromeForm1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closepicturebox)).BeginInit();
            this.tabcontrol.SuspendLayout();
            this.tabpage1.SuspendLayout();
            this.tabpage2.SuspendLayout();
            this.tabpage7.SuspendLayout();
            this.tabpage3.SuspendLayout();
            this.tabpage4.SuspendLayout();
            this.tabpage5.SuspendLayout();
            this.tabpage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logopicturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // tabcontrolimagelist
            // 
            this.tabcontrolimagelist.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.tabcontrolimagelist.ImageSize = new System.Drawing.Size(16, 16);
            this.tabcontrolimagelist.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // chromeForm1
            // 
            this.chromeForm1.BackColor = System.Drawing.Color.White;
            this.chromeForm1.BorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.chromeForm1.Controls.Add(this.SaveandCloseButton);
            this.chromeForm1.Controls.Add(this.closepicturebox);
            this.chromeForm1.Controls.Add(this.tabcontrol);
            this.chromeForm1.Controls.Add(this.logopicturebox);
            this.chromeForm1.Customization = "AAAA/1paWv9ycnL/";
            this.chromeForm1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chromeForm1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chromeForm1.Image = null;
            this.chromeForm1.Location = new System.Drawing.Point(0, 0);
            this.chromeForm1.Movable = true;
            this.chromeForm1.Name = "chromeForm1";
            this.chromeForm1.NoRounding = false;
            this.chromeForm1.Sizable = true;
            this.chromeForm1.Size = new System.Drawing.Size(586, 593);
            this.chromeForm1.SmartBounds = true;
            this.chromeForm1.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation;
            this.chromeForm1.TabIndex = 0;
            this.chromeForm1.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.chromeForm1.Transparent = false;
            // 
            // SaveandCloseButton
            // 
            this.SaveandCloseButton.Customization = "7e3t//Ly8v/r6+v/5ubm/+vr6//f39//p6en/zw8PP8UFBT/gICA/w==";
            this.SaveandCloseButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.SaveandCloseButton.Image = null;
            this.SaveandCloseButton.Location = new System.Drawing.Point(463, 568);
            this.SaveandCloseButton.Name = "SaveandCloseButton";
            this.SaveandCloseButton.NoRounding = false;
            this.SaveandCloseButton.Size = new System.Drawing.Size(120, 23);
            this.SaveandCloseButton.TabIndex = 6;
            this.SaveandCloseButton.Text = "Save and Close";
            this.SaveandCloseButton.Transparent = false;
            this.SaveandCloseButton.Click += new System.EventHandler(this.SaveandCloseButton_Click);
            // 
            // closepicturebox
            // 
            this.closepicturebox.Location = new System.Drawing.Point(567, 3);
            this.closepicturebox.Name = "closepicturebox";
            this.closepicturebox.Size = new System.Drawing.Size(16, 16);
            this.closepicturebox.TabIndex = 3;
            this.closepicturebox.TabStop = false;
            this.closepicturebox.Click += new System.EventHandler(this.closepicturebox_Click);
            // 
            // tabcontrol
            // 
            this.tabcontrol.Controls.Add(this.tabpage1);
            this.tabcontrol.Controls.Add(this.tabpage2);
            this.tabcontrol.Controls.Add(this.tabpage7);
            this.tabcontrol.Controls.Add(this.tabpage3);
            this.tabcontrol.Controls.Add(this.tabpage4);
            this.tabcontrol.Controls.Add(this.tabpage5);
            this.tabcontrol.Controls.Add(this.tabpage6);
            this.tabcontrol.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tabcontrol.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(1)), true);
            this.tabcontrol.HotTrack = true;
            this.tabcontrol.ItemSize = new System.Drawing.Size(160, 24);
            this.tabcontrol.Location = new System.Drawing.Point(3, 113);
            this.tabcontrol.Multiline = true;
            this.tabcontrol.Name = "tabcontrol";
            this.tabcontrol.Padding = new System.Drawing.Point(2, 0);
            this.tabcontrol.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabcontrol.SelectedIndex = 0;
            this.tabcontrol.ShowOuterBorders = false;
            this.tabcontrol.Size = new System.Drawing.Size(580, 449);
            this.tabcontrol.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabcontrol.SquareColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(87)))), ((int)(((byte)(100)))));
            this.tabcontrol.TabIndex = 2;
            // 
            // tabpage1
            // 
            this.tabpage1.BackColor = System.Drawing.Color.White;
            this.tabpage1.Controls.Add(this.generalpropertygrid);
            this.tabpage1.Location = new System.Drawing.Point(4, 52);
            this.tabpage1.Name = "tabpage1";
            this.tabpage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabpage1.Size = new System.Drawing.Size(572, 393);
            this.tabpage1.TabIndex = 1;
            this.tabpage1.Text = "  General Settings";
            // 
            // generalpropertygrid
            // 
            this.generalpropertygrid.CategoryForeColor = System.Drawing.Color.DimGray;
            this.generalpropertygrid.CommandsActiveLinkColor = System.Drawing.Color.DimGray;
            this.generalpropertygrid.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.generalpropertygrid.CommandsLinkColor = System.Drawing.Color.DimGray;
            this.generalpropertygrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.generalpropertygrid.HelpBackColor = System.Drawing.Color.White;
            this.generalpropertygrid.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.generalpropertygrid.HelpForeColor = System.Drawing.Color.DimGray;
            this.generalpropertygrid.Location = new System.Drawing.Point(6, 4);
            this.generalpropertygrid.Name = "generalpropertygrid";
            this.generalpropertygrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.generalpropertygrid.SelectedItemWithFocusBackColor = System.Drawing.Color.DimGray;
            this.generalpropertygrid.Size = new System.Drawing.Size(561, 384);
            this.generalpropertygrid.TabIndex = 6;
            this.generalpropertygrid.ToolbarVisible = false;
            this.generalpropertygrid.ViewBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.generalpropertygrid.ViewForeColor = System.Drawing.Color.DimGray;
            // 
            // tabpage2
            // 
            this.tabpage2.BackColor = System.Drawing.Color.White;
            this.tabpage2.Controls.Add(this.hotkeypropertygrid);
            this.tabpage2.Location = new System.Drawing.Point(4, 52);
            this.tabpage2.Name = "tabpage2";
            this.tabpage2.Size = new System.Drawing.Size(572, 393);
            this.tabpage2.TabIndex = 2;
            this.tabpage2.Text = "  Hotkey Settings";
            // 
            // hotkeypropertygrid
            // 
            this.hotkeypropertygrid.CategoryForeColor = System.Drawing.Color.DimGray;
            this.hotkeypropertygrid.CommandsActiveLinkColor = System.Drawing.Color.DimGray;
            this.hotkeypropertygrid.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.hotkeypropertygrid.CommandsLinkColor = System.Drawing.Color.DimGray;
            this.hotkeypropertygrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.hotkeypropertygrid.HelpBackColor = System.Drawing.Color.White;
            this.hotkeypropertygrid.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.hotkeypropertygrid.HelpForeColor = System.Drawing.Color.DimGray;
            this.hotkeypropertygrid.Location = new System.Drawing.Point(6, 6);
            this.hotkeypropertygrid.Name = "hotkeypropertygrid";
            this.hotkeypropertygrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.hotkeypropertygrid.SelectedItemWithFocusBackColor = System.Drawing.Color.DimGray;
            this.hotkeypropertygrid.Size = new System.Drawing.Size(561, 384);
            this.hotkeypropertygrid.TabIndex = 5;
            this.hotkeypropertygrid.ToolbarVisible = false;
            this.hotkeypropertygrid.ViewBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.hotkeypropertygrid.ViewForeColor = System.Drawing.Color.DimGray;
            // 
            // tabpage7
            // 
            this.tabpage7.BackColor = System.Drawing.Color.White;
            this.tabpage7.Controls.Add(this.SettingsComposerButton);
            this.tabpage7.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabpage7.Location = new System.Drawing.Point(4, 52);
            this.tabpage7.Name = "tabpage7";
            this.tabpage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabpage7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabpage7.Size = new System.Drawing.Size(572, 393);
            this.tabpage7.TabIndex = 0;
            this.tabpage7.Text = "  Help and Support";
            // 
            // SettingsComposerButton
            // 
            this.SettingsComposerButton.Customization = "7e3t//Ly8v/r6+v/5ubm/+vr6//f39//p6en/zw8PP8UFBT/gICA/w==";
            this.SettingsComposerButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.SettingsComposerButton.Image = null;
            this.SettingsComposerButton.Location = new System.Drawing.Point(6, 6);
            this.SettingsComposerButton.Name = "SettingsComposerButton";
            this.SettingsComposerButton.NoRounding = false;
            this.SettingsComposerButton.Size = new System.Drawing.Size(150, 23);
            this.SettingsComposerButton.TabIndex = 6;
            this.SettingsComposerButton.Text = "Compose Settings";
            this.SettingsComposerButton.Transparent = false;
            this.SettingsComposerButton.Click += new System.EventHandler(this.SettingsComposerButton_Click);
            // 
            // tabpage3
            // 
            this.tabpage3.BackColor = System.Drawing.Color.White;
            this.tabpage3.Controls.Add(this.spec1propertygrid);
            this.tabpage3.Location = new System.Drawing.Point(4, 52);
            this.tabpage3.Name = "tabpage3";
            this.tabpage3.Size = new System.Drawing.Size(572, 393);
            this.tabpage3.TabIndex = 3;
            this.tabpage3.Text = "  Arms Settings";
            // 
            // spec1propertygrid
            // 
            this.spec1propertygrid.CategoryForeColor = System.Drawing.Color.DimGray;
            this.spec1propertygrid.CommandsActiveLinkColor = System.Drawing.Color.DimGray;
            this.spec1propertygrid.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec1propertygrid.CommandsLinkColor = System.Drawing.Color.DimGray;
            this.spec1propertygrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.spec1propertygrid.HelpBackColor = System.Drawing.Color.White;
            this.spec1propertygrid.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec1propertygrid.HelpForeColor = System.Drawing.Color.DimGray;
            this.spec1propertygrid.Location = new System.Drawing.Point(6, 6);
            this.spec1propertygrid.Name = "spec1propertygrid";
            this.spec1propertygrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.spec1propertygrid.SelectedItemWithFocusBackColor = System.Drawing.Color.DimGray;
            this.spec1propertygrid.Size = new System.Drawing.Size(563, 384);
            this.spec1propertygrid.TabIndex = 5;
            this.spec1propertygrid.ToolbarVisible = false;
            this.spec1propertygrid.ViewBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec1propertygrid.ViewForeColor = System.Drawing.Color.DimGray;
            // 
            // tabpage4
            // 
            this.tabpage4.BackColor = System.Drawing.Color.White;
            this.tabpage4.Controls.Add(this.spec2propertygrid);
            this.tabpage4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabpage4.Location = new System.Drawing.Point(4, 52);
            this.tabpage4.Name = "tabpage4";
            this.tabpage4.Size = new System.Drawing.Size(572, 393);
            this.tabpage4.TabIndex = 4;
            this.tabpage4.Text = "Fury Settings";
            // 
            // spec2propertygrid
            // 
            this.spec2propertygrid.CategoryForeColor = System.Drawing.Color.DimGray;
            this.spec2propertygrid.CommandsActiveLinkColor = System.Drawing.Color.DimGray;
            this.spec2propertygrid.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec2propertygrid.CommandsLinkColor = System.Drawing.Color.DimGray;
            this.spec2propertygrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.spec2propertygrid.HelpBackColor = System.Drawing.Color.White;
            this.spec2propertygrid.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec2propertygrid.HelpForeColor = System.Drawing.Color.DimGray;
            this.spec2propertygrid.Location = new System.Drawing.Point(6, 6);
            this.spec2propertygrid.Name = "spec2propertygrid";
            this.spec2propertygrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.spec2propertygrid.SelectedItemWithFocusBackColor = System.Drawing.Color.DimGray;
            this.spec2propertygrid.Size = new System.Drawing.Size(561, 384);
            this.spec2propertygrid.TabIndex = 5;
            this.spec2propertygrid.ToolbarVisible = false;
            this.spec2propertygrid.ViewBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec2propertygrid.ViewForeColor = System.Drawing.Color.DimGray;
            // 
            // tabpage5
            // 
            this.tabpage5.BackColor = System.Drawing.Color.White;
            this.tabpage5.Controls.Add(this.spec3propertygrid);
            this.tabpage5.Location = new System.Drawing.Point(4, 52);
            this.tabpage5.Name = "tabpage5";
            this.tabpage5.Size = new System.Drawing.Size(572, 393);
            this.tabpage5.TabIndex = 5;
            this.tabpage5.Text = "  Gladiator Settings";
            // 
            // spec3propertygrid
            // 
            this.spec3propertygrid.CategoryForeColor = System.Drawing.Color.DimGray;
            this.spec3propertygrid.CommandsActiveLinkColor = System.Drawing.Color.DimGray;
            this.spec3propertygrid.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec3propertygrid.CommandsLinkColor = System.Drawing.Color.DimGray;
            this.spec3propertygrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.spec3propertygrid.HelpBackColor = System.Drawing.Color.White;
            this.spec3propertygrid.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec3propertygrid.HelpForeColor = System.Drawing.Color.DimGray;
            this.spec3propertygrid.Location = new System.Drawing.Point(6, 6);
            this.spec3propertygrid.Name = "spec3propertygrid";
            this.spec3propertygrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.spec3propertygrid.SelectedItemWithFocusBackColor = System.Drawing.Color.DimGray;
            this.spec3propertygrid.Size = new System.Drawing.Size(561, 384);
            this.spec3propertygrid.TabIndex = 5;
            this.spec3propertygrid.ToolbarVisible = false;
            this.spec3propertygrid.ViewBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec3propertygrid.ViewForeColor = System.Drawing.Color.DimGray;
            // 
            // tabpage6
            // 
            this.tabpage6.BackColor = System.Drawing.Color.White;
            this.tabpage6.Controls.Add(this.spec4propertygrid);
            this.tabpage6.Location = new System.Drawing.Point(4, 52);
            this.tabpage6.Name = "tabpage6";
            this.tabpage6.Size = new System.Drawing.Size(572, 393);
            this.tabpage6.TabIndex = 6;
            this.tabpage6.Text = "  Protection Settings";
            // 
            // spec4propertygrid
            // 
            this.spec4propertygrid.CategoryForeColor = System.Drawing.Color.DimGray;
            this.spec4propertygrid.CommandsActiveLinkColor = System.Drawing.Color.DimGray;
            this.spec4propertygrid.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec4propertygrid.CommandsLinkColor = System.Drawing.Color.DimGray;
            this.spec4propertygrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.spec4propertygrid.HelpBackColor = System.Drawing.Color.White;
            this.spec4propertygrid.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec4propertygrid.HelpForeColor = System.Drawing.Color.DimGray;
            this.spec4propertygrid.Location = new System.Drawing.Point(6, 6);
            this.spec4propertygrid.Name = "spec4propertygrid";
            this.spec4propertygrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.spec4propertygrid.SelectedItemWithFocusBackColor = System.Drawing.Color.DimGray;
            this.spec4propertygrid.Size = new System.Drawing.Size(561, 384);
            this.spec4propertygrid.TabIndex = 5;
            this.spec4propertygrid.ToolbarVisible = false;
            this.spec4propertygrid.ViewBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(167)))), ((int)(((byte)(167)))));
            this.spec4propertygrid.ViewForeColor = System.Drawing.Color.DimGray;
            // 
            // logopicturebox
            // 
            this.logopicturebox.Location = new System.Drawing.Point(7, 3);
            this.logopicturebox.Name = "logopicturebox";
            this.logopicturebox.Size = new System.Drawing.Size(558, 104);
            this.logopicturebox.TabIndex = 1;
            this.logopicturebox.TabStop = false;
            this.logopicturebox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GuiDragDrop);
            // 
            // Interface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 593);
            this.Controls.Add(this.chromeForm1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Interface";
            this.Text = "RootInterface";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.Load += new System.EventHandler(this.Interface_Load);
            this.chromeForm1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.closepicturebox)).EndInit();
            this.tabcontrol.ResumeLayout(false);
            this.tabpage1.ResumeLayout(false);
            this.tabpage2.ResumeLayout(false);
            this.tabpage7.ResumeLayout(false);
            this.tabpage3.ResumeLayout(false);
            this.tabpage4.ResumeLayout(false);
            this.tabpage5.ResumeLayout(false);
            this.tabpage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logopicturebox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList tabcontrolimagelist;
        private System.Windows.Forms.PictureBox logopicturebox;
        private System.Windows.Forms.TabPage tabpage1;
        private System.Windows.Forms.TabPage tabpage2;
        private System.Windows.Forms.PropertyGrid hotkeypropertygrid;
        private System.Windows.Forms.TabPage tabpage3;
        private System.Windows.Forms.PropertyGrid spec1propertygrid;
        private System.Windows.Forms.TabPage tabpage4;
        private System.Windows.Forms.PropertyGrid spec2propertygrid;
        private System.Windows.Forms.TabPage tabpage5;
        private System.Windows.Forms.PropertyGrid spec3propertygrid;
        private System.Windows.Forms.TabPage tabpage6;
        private System.Windows.Forms.PropertyGrid spec4propertygrid;
        private System.Windows.Forms.TabPage tabpage7;
        private ChromeButton SettingsComposerButton;
        private System.Windows.Forms.PictureBox closepicturebox;
        private ChromeButton SaveandCloseButton;
        private ChromeForm chromeForm1;
        internal ChromeTabcontrolNormal tabcontrol;
        private System.Windows.Forms.PropertyGrid generalpropertygrid;
    }
}