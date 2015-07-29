﻿using ff14bot;
using System;
using System.Drawing;
using System.Windows.Forms;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Overlay
{
    public partial class Overlay : Form
    {
        #region Full credits goes to Monkee
        #endregion

        private static Overlay overlay;
        private static bool shown;
        private bool mouseIsDown = false;
        private Point firstPoint;
        public static event EventHandler onLocationChanged;
        public static Point overlayLocation { get; set; }
        public Overlay()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Black;
            TransparencyKey = Color.Black;
            label1.Text = "YourRaidingBuddy";
        }


        public static void updateLabels()
        {
            if (overlay == null)
                return;
            if (VariableBook.HkmMultiTarget)
            {
                overlay.lblAoE.Text = "AoE: Enabled";
                overlay.lblAoE.ForeColor = Color.LightGreen;
            }
            else
            {
                overlay.lblAoE.Text = "AoE: Disabled";
                overlay.lblAoE.ForeColor = Color.Red;
            }
            if (VariableBook.HkmCooldowns)
            {
                overlay.lblCoold.Text = "Cooldowns: Enabled";
                overlay.lblCoold.ForeColor = Color.LightGreen;
            }
            else
            {
                overlay.lblCoold.Text = "Cooldowns: Disabled";
                overlay.lblCoold.ForeColor = Color.Red;
            }
            if (VariableBook.HkmPaused)
            {
                overlay.lblManual.Text = "Pause: Enabled";
                overlay.lblManual.ForeColor = Color.LightGreen;
            }
            else
            {
                overlay.lblManual.Text = "Pause: Disabled";
                overlay.lblManual.ForeColor = Color.Red;
            }
        }

        public static bool showOverlay(Point p)
        {
            overlay = new Overlay();
            overlay.Location = new Point(InternalSettings.Instance.General.X, InternalSettings.Instance.General.Y);
            overlay.Show();
            shown = true;
            return shown;
        }

        private static void onLocationChange(EventArgs args)
        {
            EventHandler handler = onLocationChanged;
            if (handler != null)
            {
                handler(null, args);
            }

        }

        public static void closeOverlay()
        {
            overlay.Close();
            shown = false;
        }

        public static void setLocation(Point p)
        {
            if (p != null)
                overlay.Location = p;
        }

        public static Point getLocation()
        {
            return overlay.Location;
        }

        public static bool isShown()
        {

            return (overlay != null && shown) ? true : false;
        }

        private void lblMain_MouseDown(object sender, MouseEventArgs e)
        {
            firstPoint = e.Location;
            mouseIsDown = true;
        }

        private void lblMain_MouseUp(object sender, MouseEventArgs e)
        {
            mouseIsDown = false;
            onLocationChange(e);
        }

        private void lblMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                // Get the difference between the two points
                int xDiff = firstPoint.X - e.Location.X;
                int yDiff = firstPoint.Y - e.Location.Y;

                // Set the new point
                int x = this.Location.X - xDiff;
                int y = this.Location.Y - yDiff;
                this.Location = new Point(x, y);
            }
        }

        private void lblCoold_Click(object sender, EventArgs e)
        {
            VariableBook.HkmCooldowns = !VariableBook.HkmCooldowns;
        }

        private void lblManual_Click(object sender, EventArgs e)
        {
            VariableBook.HkmPaused = !VariableBook.HkmPaused;
        }


        private void lblAoE_Click(object sender, EventArgs e)
        {
            VariableBook.HkmMultiTarget = !VariableBook.HkmMultiTarget;
        }


        internal static void hideOverlay()
        {
            overlay.Hide();
            shown = false;
        }

        private void Overlay_Load(object sender, EventArgs e)
        {

        }
    }
}
