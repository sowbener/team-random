using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YourRaidingBuddy.Interface.Extensions
{
    // Google Chrome Theme - Ported to C# by Ecco
    // Original Theme http://www.hackforums.net/showthread.php?tid=2926688
    // Credit to Mavamaarten~ for Google Chrome Theme & Aeonhack for Themebase
    // Edits and extra controls by weischbier & nomnomnom

    class ChromeForm : ThemeContainer154
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeForm"/> class.
        /// </summary>
        public ChromeForm()
        {
            TransparencyKey = Color.Fuchsia;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9);
            SetColor("Title color", Color.Black);
            SetColor("X-color", 90, 90, 90);
            SetColor("X-ellipse", 114, 114, 114);
        }

        /// <summary>
        /// Gets or sets the color of the back.
        /// </summary>
        /// <value>
        /// The color of the back.
        /// </value>
        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary>
        /// The _title color
        /// </summary>
        Color _titleColor;

        /// <summary>
        /// The _xcolor
        /// </summary>
        Color _xcolor;

        /// <summary>
        /// The _xellipse
        /// </summary>
        Color _xellipse;

        /// <summary>
        /// Colors the hook.
        /// </summary>
        protected override void ColorHook()
        {
            _titleColor = GetColor("Title color");
            _xcolor = GetColor("X-color");
            _xellipse = GetColor("X-ellipse");
        }

        /// <summary>
        /// The _x
        /// </summary>
        int _x;

        /// <summary>
        /// The _y
        /// </summary>
        int _y;
        /// <summary>
        /// Raises the <see cref="E:MouseMove" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _x = e.Location.X;
            _y = e.Location.Y;
            base.OnMouseMove(e);
            Invalidate();
        }
        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseClick" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.MouseEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnClick(e);
        }

        /// <summary>
        /// Paints the hook.
        /// </summary>
        protected override void PaintHook()
        {
            G.Clear(BackColor);
            DrawCorners(Color.Fuchsia);
            DrawCorners(Color.Fuchsia, 1, 0, Width - 2, Height);
            DrawCorners(Color.Fuchsia, 0, 1, Width, Height - 2);
            DrawCorners(Color.Fuchsia, 2, 0, Width - 4, Height);
            DrawCorners(Color.Fuchsia, 0, 2, Width, Height - 4);

            G.SmoothingMode = SmoothingMode.HighQuality;

            DrawText(new SolidBrush(_titleColor), new Point(8, 7));
        }
    }

    class ChromeButton : ThemeControl154
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeButton"/> class.
        /// </summary>
        public ChromeButton()
        {
            Font = new Font("Segoe UI", 9);
            SetColor("Gradient top normal", 237, 237, 237);
            SetColor("Gradient top over", 242, 242, 242);
            SetColor("Gradient top down", 235, 235, 235);
            SetColor("Gradient bottom normal", 230, 230, 230);
            SetColor("Gradient bottom over", 235, 235, 235);
            SetColor("Gradient bottom down", 223, 223, 223);
            SetColor("Border", 167, 167, 167);
            SetColor("Text normal", 60, 60, 60);
            SetColor("Text down/over", 20, 20, 20);
            SetColor("Text disabled", Color.Gray);
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public override sealed Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        /// <summary>
        /// The _GTN
        /// </summary>
        Color _gtn;
        /// <summary>
        /// The _gto
        /// </summary>
        Color _gto;
        /// <summary>
        /// The _GTD
        /// </summary>
        Color _gtd;
        /// <summary>
        /// The _GBN
        /// </summary>
        Color _gbn;
        /// <summary>
        /// The _gbo
        /// </summary>
        Color _gbo;
        /// <summary>
        /// The _GBD
        /// </summary>
        Color _gbd;
        /// <summary>
        /// The _bo
        /// </summary>
        Color _bo;
        /// <summary>
        /// The _TN
        /// </summary>
        Color _tn;
        /// <summary>
        /// The _TD
        /// </summary>
        Color _td;
        /// <summary>
        /// The _tdo
        /// </summary>
        Color _tdo;
        /// <summary>
        /// Colors the hook.
        /// </summary>
        protected override void ColorHook()
        {
            _gtn = GetColor("Gradient top normal");
            _gto = GetColor("Gradient top over");
            _gtd = GetColor("Gradient top down");
            _gbn = GetColor("Gradient bottom normal");
            _gbo = GetColor("Gradient bottom over");
            _gbd = GetColor("Gradient bottom down");
            _bo = GetColor("Border");
            _tn = GetColor("Text normal");
            _tdo = GetColor("Text down/over");
            _td = GetColor("Text disabled");
        }

        /// <summary>
        /// Paints the hook.
        /// </summary>
        protected override void PaintHook()
        {
            G.Clear(BackColor);
            LinearGradientBrush lgb;
            G.SmoothingMode = SmoothingMode.HighQuality;


            switch (State)
            {
                case MouseStateControl.None:
                    lgb = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), _gtn, _gbn, 90f);
                    break;
                case MouseStateControl.Over:
                    lgb = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), _gto, _gbo, 90f);
                    break;
                default:
                    lgb = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), _gtd, _gbd, 90f);
                    break;
            }

            if (!Enabled)
            {
                lgb = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), _gtn, _gbn, 90f);
            }

            var buttonpath = CreateRound(Rectangle.Round(lgb.Rectangle), 3);
            G.FillPath(lgb, CreateRound(Rectangle.Round(lgb.Rectangle), 3));
            if (!Enabled)
                G.FillPath(new SolidBrush(Color.FromArgb(50, Color.White)), CreateRound(Rectangle.Round(lgb.Rectangle), 3));
            G.SetClip(buttonpath);
            lgb = new LinearGradientBrush(new Rectangle(0, 0, Width, Height / 6), Color.FromArgb(80, Color.White), Color.Transparent, 90f);
            G.FillRectangle(lgb, Rectangle.Round(lgb.Rectangle));



            G.ResetClip();
            G.DrawPath(new Pen(_bo), buttonpath);

            if (Enabled)
            {
                switch (State)
                {
                    case MouseStateControl.None:
                        DrawText(new SolidBrush(_tn), HorizontalAlignment.Center, 1, 0);
                        break;
                    default:
                        DrawText(new SolidBrush(_tdo), HorizontalAlignment.Center, 1, 0);
                        break;
                }
            }
            else
            {
                DrawText(new SolidBrush(_td), HorizontalAlignment.Center, 1, 0);
            }
        }
    }

    class ChromeSeparator : ThemeControl154
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeSeparator"/> class.
        /// </summary>
        public ChromeSeparator()
        {
            LockHeight = 2;
            BackColor = Color.FromArgb(78, 87, 100);
        }

        /// <summary>
        /// Gets or sets the color of the back.
        /// </summary>
        /// <value>
        /// The color of the back.
        /// </value>
        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }


        /// <summary>
        /// Colors the hook.
        /// </summary>
        protected override void ColorHook()
        {
        }

        /// <summary>
        /// Paints the hook.
        /// </summary>
        protected override void PaintHook()
        {
            G.Clear(BackColor);
        }
    }

    class ChromeTabcontrol : TabControl
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeTabcontrol"/> class.
        /// </summary>
        public ChromeTabcontrol()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(30, 115);
        }

        /// <summary>
        /// Dieser Member hat für das genannte Steuerelement keine Bedeutung.
        /// </summary>
        protected override sealed bool DoubleBuffered
        {
            get { return base.DoubleBuffered; }
            set { base.DoubleBuffered = value; }
        }

        /// <summary>
        /// Dieser Member überschreibt <see cref="M:System.Windows.Forms.Control.CreateHandle" />.
        /// </summary>
        protected override void CreateHandle()
        {
            base.CreateHandle();
            Alignment = TabAlignment.Left;
        }

        /// <summary>
        /// The _C1
        /// </summary>
        Color _c1 = Color.FromArgb(78, 87, 100);
        /// <summary>
        /// Gets or sets the color of the square.
        /// </summary>
        /// <value>
        /// The color of the square.
        /// </value>
        public Color SquareColor
        {
            get { return _c1; }
            set
            {
                _c1 = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The _ob
        /// </summary>
        bool _ob = false;
        /// <summary>
        /// Gets or sets a value indicating whether [show outer borders].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show outer borders]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowOuterBorders
        {
            get { return _ob; }
            set
            {
                _ob = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.Paint" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.PaintEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            var B = new Bitmap(Width, Height);
            var G = Graphics.FromImage(B);
            try
            {
                SelectedTab.BackColor = Color.White;
            }
            catch (Exception)
            {
            }
            G.Clear(Color.White);
            for (int i = 0; i <= TabCount - 1; i++)
            {
                var x2 = new Rectangle(new Point(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2), new Size(GetTabRect(i).Width + 3, GetTabRect(i).Height - 1));
                var textrectangle = new Rectangle(x2.Location.X + 15, x2.Location.Y, x2.Width - 20, x2.Height); // 10 to 15 for icons
                if (i == SelectedIndex)
                {
                    G.FillRectangle(new SolidBrush(_c1), new Rectangle(x2.Location, new Size(9, x2.Height)));


                    if (ImageList != null)
                    {
                        try
                        {
                            G.DrawImage(ImageList.Images[TabPages[i].ImageIndex], new Point(textrectangle.Location.X + 8, textrectangle.Location.Y + 6));
                            G.DrawString("      " + TabPages[i].Text, Font, Brushes.Black, textrectangle, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            });
                        }
                        catch
                        {
                            G.DrawString(TabPages[i].Text, Font, Brushes.Black, textrectangle, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            });
                        }
                    }
                    else
                    {
                        G.DrawString(TabPages[i].Text, Font, Brushes.Black, textrectangle, new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Near
                        });
                    }

                }
                else
                {
                    if (ImageList != null)
                    {
                        try
                        {
                            G.DrawImage(ImageList.Images[TabPages[i].ImageIndex], new Point(textrectangle.Location.X + 8, textrectangle.Location.Y + 6));
                            G.DrawString("      " + TabPages[i].Text, Font, Brushes.DimGray, textrectangle, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            });
                        }
                        catch
                        {
                            G.DrawString(TabPages[i].Text, Font, Brushes.DimGray, textrectangle, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            });
                        }
                    }
                    else
                    {
                        G.DrawString(TabPages[i].Text, Font, Brushes.DimGray, textrectangle, new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Near
                        });
                    }
                }
            }

            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    class ChromeTabcontrolNormal : TabControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeTabcontrolNormal"/> class.
        /// </summary>
        public ChromeTabcontrolNormal()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(165, 28);
            //ItemSize = new Size(290, 28);
        }

        /// <summary>
        /// Dieser Member hat für das genannte Steuerelement keine Bedeutung.
        /// </summary>
        protected override sealed bool DoubleBuffered
        {
            get { return base.DoubleBuffered; }
            set { base.DoubleBuffered = value; }
        }

        /// <summary>
        /// Dieser Member überschreibt <see cref="M:System.Windows.Forms.Control.CreateHandle" />.
        /// </summary>
        protected override void CreateHandle()
        {
            base.CreateHandle();
            Alignment = TabAlignment.Top;
        }

        /// <summary>
        /// The _C1
        /// </summary>
        Color _c1 = Color.FromArgb(78, 87, 100);
        /// <summary>
        /// Gets or sets the color of the square.
        /// </summary>
        /// <value>
        /// The color of the square.
        /// </value>
        public Color SquareColor
        {
            get { return _c1; }
            set
            {
                _c1 = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The _ob
        /// </summary>
        bool _ob = false;
        /// <summary>
        /// Gets or sets a value indicating whether [show outer borders].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show outer borders]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowOuterBorders
        {
            get { return _ob; }
            set
            {
                _ob = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.Paint" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.PaintEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            var B = new Bitmap(Width, Height);
            var G = Graphics.FromImage(B);
            try
            {
                SelectedTab.BackColor = Color.White;
            }
            catch (Exception)
            {
            }
            G.Clear(Color.White);
            for (int i = 0; i <= TabCount - 1; i++)
            {
                var x2 = new Rectangle(new Point(GetTabRect(i).Location.X + 2, GetTabRect(i).Location.Y + 17), new Size(GetTabRect(i).Width - 1, GetTabRect(i).Height - 1));
                var textrectangle = new Rectangle(x2.Location.X + 12, x2.Location.Y - 21, x2.Width - 20, x2.Height);
                if (i == SelectedIndex)
                {
                    G.FillRectangle(new SolidBrush(_c1), new Rectangle(x2.Location, new Size(x2.Width, 9)));


                    if (ImageList != null)
                    {
                        try
                        {
                            G.DrawImage(ImageList.Images[TabPages[i].ImageIndex], new Point(textrectangle.Location.X + 6, textrectangle.Location.Y + 3));
                            G.DrawString("      " + TabPages[i].Text, Font, Brushes.Black, textrectangle, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            });
                        }
                        catch
                        {
                            G.DrawString(TabPages[i].Text, Font, Brushes.Black, textrectangle, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            });
                        }
                    }
                    else
                    {
                        G.DrawString(TabPages[i].Text, Font, Brushes.Black, textrectangle, new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Near
                        });
                    }

                }
                else
                {
                    if (ImageList != null)
                    {
                        try
                        {
                            G.DrawImage(ImageList.Images[TabPages[i].ImageIndex], new Point(textrectangle.Location.X + 6, textrectangle.Location.Y + 3));
                            G.DrawString("      " + TabPages[i].Text, Font, Brushes.DimGray, textrectangle, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            });
                        }
                        catch
                        {
                            G.DrawString(TabPages[i].Text, Font, Brushes.DimGray, textrectangle, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            });
                        }
                    }
                    else
                    {
                        G.DrawString(TabPages[i].Text, Font, Brushes.DimGray, textrectangle, new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Near
                        });
                    }
                }
            }

            e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
            G.Dispose();
            B.Dispose();
        }
    }

    class ChromeComboBoxLight : ComboBox
    {

        /// <summary>
        /// The _x
        /// </summary>
        private int _x;
        /// <summary>
        /// The _start index
        /// </summary>
        private int _startIndex = 0;
        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        public int StartIndex
        {
            get { return _startIndex; }
            set
            {
                _startIndex = value;
                try { base.SelectedIndex = value; }
                // ReSharper disable once EmptyGeneralCatchClause
                catch { }

                Invalidate();
            }
        }

        /// <summary>
        /// The _white
        /// </summary>
        private readonly Color _white = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// The _uppergradientgrey
        /// </summary>
        private readonly Color _uppergradientgrey = Color.FromArgb(237, 237, 237);
        /// <summary>
        /// The _bottomgradientgrey
        /// </summary>
        private readonly Color _bottomgradientgrey = Color.FromArgb(230, 230, 230);
        /// <summary>
        /// The _lightgrey
        /// </summary>
        private readonly Color _lightgrey = Color.FromArgb(167, 167, 167);
        /// <summary>
        /// The _mediumgrey
        /// </summary>
        private readonly Color _mediumgrey = Color.FromArgb(105, 105, 105);
        /// <summary>
        /// The _darkgrey
        /// </summary>
        private readonly Color _darkgrey = Color.FromArgb(78, 87, 100);
        /// <summary>
        /// The _textnormal
        /// </summary>
        private readonly Color _textnormal = Color.FromArgb(60, 60, 60);

        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="G">The g.</param>
        protected void DrawBorders(Pen p1, Graphics G)
        {
            DrawBorders(p1, 0, 0, Width, Height, G);
        }

        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="G">The g.</param>
        protected void DrawBorders(Pen p1, int x, int y, int width, int height, Graphics G)
        {
            /* Initial Border around Combobox */
            G.DrawRectangle(p1, x, y, width - 1, height - 1);
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseMove" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.MouseEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _x = e.X;
            base.OnMouseMove(e);
        }
        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseLeave" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Eine <see cref="T:System.EventArgs" />-Klasse, die die Ereignisdaten enthält.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            _x = 0;
            base.OnMouseLeave(e);
        }
        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseClick" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.MouseEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _x = 0;
            }
            base.OnMouseClick(e);
        }

        //private readonly SolidBrush _b1;
        /// <summary>
        /// The _B2
        /// </summary>
        private readonly SolidBrush _b2;
        /// <summary>
        /// The _B3
        /// </summary>
        private readonly SolidBrush _b3;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeComboBoxLight"/> class.
        /// </summary>
        public ChromeComboBoxLight()
        {
            SetStyle((ControlStyles)139286, true);
            SetStyle(ControlStyles.Selectable, false);
            DrawMode = DrawMode.OwnerDrawFixed;

            /* BackColor - No idea what it does */
            //BackColor = Color.FromArgb(255, 0, 0);
            DropDownStyle = ComboBoxStyle.DropDownList;

            /* Segoe UI; 9pt - Font */
            Font = new Font("Segoe UI", 9);

            /* B1 - No idea what it does */
            //B1 = new SolidBrush(Color.FromArgb(255, 0, 0));

            /* Dropdown background color */
            _b2 = new SolidBrush(_white);

            /* Arrow Color */
            _b3 = new SolidBrush(_mediumgrey);
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.Paint" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.PaintEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            Point[] points = { new Point(Width - 15, 9), new Point(Width - 6, 9), new Point(Width - 11, 14) };
            G.Clear(BackColor);
            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            /* Combobox gradient - Upper and bottom - 2 Colors in this line */
            LinearGradientBrush lgb1 = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), _uppergradientgrey, _bottomgradientgrey, 90F);

            G.FillRectangle(lgb1, new Rectangle(0, 0, Width, Height));

            /* Horizontal Splitter between text and button */
            //G.DrawLine(new Pen(new SolidBrush(Color.FromArgb(167, 167, 167))), new Point(Width - 21, 1), new Point(Width - 21, Height));

            /* Outer Border */
            DrawBorders(new Pen(new SolidBrush(_lightgrey)), G);

            /* Inner Border */
            //DrawBorders(new Pen(new SolidBrush(Color.FromArgb(255, 0, 0))), 1, G);

            try
            {
                /* Text in combobox - When not dropdowned */
                G.DrawString(Items[SelectedIndex].ToString(), Font, new SolidBrush(_textnormal), new Point(3, 4));
            }
            catch
            {
                /* Text in combobox - When not dropdowned */
                G.DrawString("Select Default", Font, new SolidBrush(_textnormal), new Point(3, 4));
            }

            if (_x >= 1)
            {
                /* Arrow mouse-over gradient - 2 Colors in this line */
                LinearGradientBrush lgb3 = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), _uppergradientgrey, _bottomgradientgrey, 90F);
                G.FillRectangle(lgb3, new Rectangle(Width - 20, 2, 18, 17));
                G.FillPolygon(_b3, points);
            }
            else
            {
                G.FillPolygon(_b3, points);
            }
        }
        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.ComboBox.DrawItem" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.DrawItemEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            /* Gradient for actual dropdown - DROPDOWN SELECTION */
            LinearGradientBrush lgb1 = new LinearGradientBrush(e.Bounds, _darkgrey, _darkgrey, 90);

            /* Diagonal brushes to draw diagonal lines in the gradient - DROPDOWN SELECTION */
            HatchBrush hb1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, _darkgrey, _darkgrey);

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(lgb1, new Rectangle(1, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                e.Graphics.FillRectangle(hb1, e.Bounds);
                /* Dropdown text color when hovering through - selected */
                e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(_white), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(_b2, e.Bounds);
                /* Regular dropdown text color */
                try { e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(_mediumgrey), e.Bounds); }
                // ReSharper disable once EmptyGeneralCatchClause
                catch { }
            }

        }
    }

    class ChromeComboBoxDark : ComboBox
    {
        /// <summary>
        /// The _x
        /// </summary>
        private int _x;
        /// <summary>
        /// The _start index
        /// </summary>
        private int _startIndex = 0;
        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        public int StartIndex
        {
            get { return _startIndex; }
            set
            {
                _startIndex = value;
                try { base.SelectedIndex = value; }
                // ReSharper disable once EmptyGeneralCatchClause
                catch { }

                Invalidate();
            }
        }
        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="G">The g.</param>
        protected void DrawBorders(Pen p1, Graphics G)
        {
            DrawBorders(p1, 0, 0, Width, Height, G);
        }
        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="G">The g.</param>
        protected void DrawBorders(Pen p1, int offset, Graphics G)
        {
            DrawBorders(p1, 0, 0, Width, Height, offset, G);
        }
        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="G">The g.</param>
        protected void DrawBorders(Pen p1, int x, int y, int width, int height, Graphics G)
        {
            /* Initial Border around Combobox */
            G.DrawRectangle(p1, x, y, width - 1, height - 1);
        }
        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="G">The g.</param>
        protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset, Graphics G)
        {
            /* Bordersize of right and down. */
            DrawBorders(p1, x + offset, y + offset, width - (offset * 1), height - (offset * 1), G);
        }
        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseMove" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.MouseEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _x = e.X;
            base.OnMouseMove(e);
        }
        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseLeave" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Eine <see cref="T:System.EventArgs" />-Klasse, die die Ereignisdaten enthält.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            _x = 0;
            base.OnMouseLeave(e);
        }
        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseClick" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.MouseEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _x = 0;
            }
            base.OnMouseClick(e);
        }

        //private readonly SolidBrush _b1;
        /// <summary>
        /// The _B2
        /// </summary>
        private readonly SolidBrush _b2;
        /// <summary>
        /// The _B3
        /// </summary>
        private readonly SolidBrush _b3;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeComboBoxDark"/> class.
        /// </summary>
        public ChromeComboBoxDark()
        {
            SetStyle((ControlStyles)139286, true);
            SetStyle(ControlStyles.Selectable, false);
            DrawMode = DrawMode.OwnerDrawFixed;

            /* BackColor - No idea what it does */
            //BackColor = Color.FromArgb(255, 0, 0);
            DropDownStyle = ComboBoxStyle.DropDownList;

            /* Segoe UI; 9pt - Font */
            Font = new Font("Segoe UI", 9);

            /* B1 - No idea what it does */
            //B1 = new SolidBrush(Color.FromArgb(255, 0, 0));

            /* Dropdown background color */
            _b2 = new SolidBrush(Color.FromArgb(255, 255, 255));

            /* Arrow Color */
            _b3 = new SolidBrush(Color.FromArgb(105, 105, 105));
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.Paint" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.PaintEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            Point[] points = { new Point(Width - 15, 9), new Point(Width - 6, 9), new Point(Width - 11, 14) };
            G.Clear(BackColor);
            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            /* Combobox gradient - Upper and bottom - 2 Colors in this line */
            LinearGradientBrush lgb1 = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), 90F);

            G.FillRectangle(lgb1, new Rectangle(0, 0, Width, Height));

            /* Horizontal Splitter between text and button */
            G.DrawLine(new Pen(new SolidBrush(Color.FromArgb(78, 87, 100))), new Point(Width - 21, 1), new Point(Width - 21, Height));

            /* Outer Border */
            DrawBorders(new Pen(new SolidBrush(Color.FromArgb(105, 105, 105))), G);

            /* Inner Border */
            //DrawBorders(new Pen(new SolidBrush(Color.FromArgb(255, 0, 0))), 1, G);

            try
            {
                /* Text in combobox - When not dropdowned */
                G.DrawString(Items[SelectedIndex].ToString(), Font, new SolidBrush(Color.FromArgb(78, 87, 100)), new Point(3, 4));
            }
            catch
            {
                /* Text in combobox - When not dropdowned */
                G.DrawString("Select Default", Font, new SolidBrush(Color.FromArgb(78, 87, 100)), new Point(3, 4));
            }

            if (_x >= 1)
            {
                /* Arrow mouse-over gradient - 2 Colors in this line */
                LinearGradientBrush lgb3 = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), 90F);
                G.FillRectangle(lgb3, new Rectangle(Width - 20, 2, 18, 17));
                G.FillPolygon(_b3, points);
            }
            else
            {
                G.FillPolygon(_b3, points);
            }
        }
        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.ComboBox.DrawItem" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.DrawItemEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            /* Gradient for actual dropdown - DROPDOWN SELECTION */
            LinearGradientBrush lgb1 = new LinearGradientBrush(e.Bounds, Color.FromArgb(78, 87, 100), Color.FromArgb(78, 87, 100), 90);

            /* Diagonal brushes to draw diagonal lines in the gradient - DROPDOWN SELECTION */
            HatchBrush hb1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, Color.FromArgb(78, 87, 100), Color.FromArgb(78, 87, 100));

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(lgb1, new Rectangle(1, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                e.Graphics.FillRectangle(hb1, e.Bounds);
                /* Dropdown text color when hovering through - selected */
                e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(Color.FromArgb(255, 255, 255)), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(_b2, e.Bounds);
                /* Regular dropdown text color */
                try { e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(Color.FromArgb(105, 105, 105)), e.Bounds); }
                // ReSharper disable once EmptyGeneralCatchClause
                catch { }
            }

        }
    }

    class ChromeComboBoxNormal : ComboBox
    {
        /// <summary>
        /// The x
        /// </summary>
        private int X;
        /// <summary>
        /// The _ start index
        /// </summary>
        private int _startIndex;
        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        public int StartIndex
        {
            get { return _startIndex; }
            set
            {
                _startIndex = value;
                try
                {
                    base.SelectedIndex = value;
                }
                catch
                {
                }
                Invalidate();
            }
        }
        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="G">The g.</param>
        private void DrawBorders(Pen p1, Graphics G)
        {
            DrawBorders(p1, 0, 0, Width, Height, G);
        }

        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="G">The g.</param>
        private void DrawBorders(Pen p1, int x, int y, int width, int height, Graphics G)
        {
            G.DrawRectangle(p1, x, y, width - 1, height - 1); // Initial Border around Combobox
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseMove" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.MouseEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            X = e.X;
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseLeave" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Eine <see cref="T:System.EventArgs" />-Klasse, die die Ereignisdaten enthält.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            X = 0;
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseClick" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.MouseEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                X = 0;
            }
            base.OnMouseClick(e);
        }

        /// <summary>
        /// The b2
        /// </summary>
        private readonly SolidBrush _b2;

        /// <summary>
        /// The b3
        /// </summary>
        private readonly SolidBrush _b3;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeComboBoxNormal"/> class.
        /// </summary>
        public ChromeComboBoxNormal()
        {
            SetStyle((ControlStyles)139286, true);
            SetStyle(ControlStyles.Selectable, false);
            DrawMode = DrawMode.OwnerDrawFixed;

            BackColor = Color.FromArgb(255, 0, 0); // ???
            DropDownStyle = ComboBoxStyle.DropDownList;

            Font = new Font("Segoe UI", 9); //Segoe UI; 9pt - Font

            new SolidBrush(Color.FromArgb(255, 0, 0)); // ???
            _b2 = new SolidBrush(Color.FromArgb(255, 255, 255)); // dropdown background color
            _b3 = new SolidBrush(Color.FromArgb(105, 105, 105)); // Arrow Color
        }

        public override sealed Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        // Dimgray = 105, 105, 105
        // Dark Color = 78, 87, 100

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.Paint" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.PaintEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            var points = new Point[] { new Point(Width - 15, 9), new Point(Width - 6, 9), new Point(Width - 11, 14) };
            G.Clear(BackColor);
            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var lgb1 = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), 90F); // Combobox gradient - Upper and bottom.

            G.FillRectangle(lgb1, new Rectangle(0, 0, Width, Height));

            //G.DrawLine(new Pen(new SolidBrush(Color.FromArgb(78, 87, 100))), new Point(Width - 21, 1), new Point(Width - 21, Height)); // Horizontal Splitter between text and button

            DrawBorders(new Pen(new SolidBrush(Color.FromArgb(105, 105, 105))), G); // Outter Border
            //DrawBorders(new Pen(new SolidBrush(Color.FromArgb(255, 0, 0))), 1, G); // Inner Border - Disabled

            try { G.DrawString((string)Items[SelectedIndex].ToString(), Font, new SolidBrush(Color.FromArgb(78, 87, 100)), new Point(3, 4)); } // Text in combobox - When not dropdowned
            catch { G.DrawString("Select Default", Font, new SolidBrush(Color.FromArgb(78, 87, 100)), new Point(3, 4)); } // Text in combobox - When not dropdowned

            if (X >= 1)
            {
                var lgb3 = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 255), 90F); // Arrow mouse-over GRADIENT
                G.FillRectangle(lgb3, new Rectangle(Width - 20, 2, 18, 17));
                G.FillPolygon(_b3, points);
            }
            else
            {
                G.FillPolygon(_b3, points);
            }
        }
        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.ComboBox.DrawItem" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.DrawItemEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            var LGB1 = new LinearGradientBrush(e.Bounds, Color.FromArgb(78, 87, 100), Color.FromArgb(78, 87, 100), 90); // Gradient for actual dropdown - DROPDOWN SELECTION
            var HB1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, Color.FromArgb(78, 87, 100), Color.FromArgb(78, 87, 100)); // Diagonal brushes to draw diagonal lines in the gradient - DROPDOWN SELECTION

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(LGB1, new Rectangle(1, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                e.Graphics.FillRectangle(HB1, e.Bounds);
                e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(Color.FromArgb(255, 255, 255)), e.Bounds); // Text color when hovering through - selected
            }
            else
            {
                e.Graphics.FillRectangle(_b2, e.Bounds);
                try { e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(Color.FromArgb(105, 105, 105)), e.Bounds); } // Regular text color
                catch { }
            }

        }
    }

    class ChromeLabel : Label
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeLabel"/> class.
        /// </summary>
        public ChromeLabel()
        {
            ForeColor = Color.FromArgb(78, 87, 100);
            BackColor = Color.Transparent;
            Font = new Font("Segoe", 9);
        }
    }

    [DefaultEvent("CheckedChanged")]
    class ChromeOnOff : ThemeControl154
    {
        /// <summary>
        /// The tb
        /// </summary>
        Brush TB;
        /// <summary>
        /// The b
        /// </summary>
        Pen b;

        /// <summary>
        /// The _ checked
        /// </summary>
        private bool _Checked = false;
        /// <summary>
        /// Occurs when [checked changed].
        /// </summary>
        public event CheckedChangedEventHandler CheckedChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">The sender.</param>
        public delegate void CheckedChangedEventHandler(object sender);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChromeOnOff"/> is checked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if checked; otherwise, <c>false</c>.
        /// </value>
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                Invalidate();
                if (CheckedChanged != null)
                {
                    CheckedChanged(this);
                }
            }
        }
        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="G">The g.</param>
        protected void DrawBorders(Pen p1, Graphics G)
        {
            DrawBorders(p1, 0, 0, Width, Height, G);
        }

        /// <summary>
        /// Draws the borders.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="G">The g.</param>
        protected void DrawBorders(Pen p1, int x, int y, int width, int height, Graphics G)
        {
            G.DrawRectangle(p1, x, y, width - 1, height - 1);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDown" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _Checked = !_Checked;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeOnOff"/> class.
        /// </summary>
        public ChromeOnOff()
        {
            LockHeight = 24;
            LockWidth = 62;
            SetColor("Texts", Color.FromArgb(100, 100, 100));
            SetColor("border", Color.FromArgb(78, 87, 100));
        }

        /// <summary>
        /// Colors the hook.
        /// </summary>
        protected override void ColorHook()
        {
            TB = GetBrush("Texts");
            b = GetPen("border");
        }

        /// <summary>
        /// Paints the hook.
        /// </summary>
        protected override void PaintHook()
        {
            new Rectangle();
            G.Clear(BackColor);
            var lgb1 = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(78, 87, 100), Color.FromArgb(78, 87, 100), 90);
            //HatchBrush HB1 = new HatchBrush(HatchStyle.DarkUpwardDiagonal, Color.FromArgb(10, Color.White), Color.Transparent);

            if (_Checked)
            {
                G.FillRectangle(lgb1, new Rectangle(0, 0, (Width / 2) - 0, Height - 0));
                //G.FillRectangle(HB1, new Rectangle(2, 2, (Width / 2) - 2, Height - 4));
                G.DrawString("On", Font, TB, new Point(36, 6));
            }
            else if (!_Checked)
            {
                G.FillRectangle(lgb1, new Rectangle((Width / 2) - 0, 0, (Width / 2) - 0, Height - 0));
                //G.FillRectangle(HB1, new Rectangle((Width / 2) - 1, 2, (Width / 2) - 1, Height - 4));
                G.DrawString("Off", Font, TB, new Point(5, 6));
            }
            DrawBorders(new Pen(new SolidBrush(Color.FromArgb(78, 87, 100))), G);
            //DrawBorders(new Pen(new SolidBrush(Color.FromArgb(78, 87, 100))), 1, G); -- Inner Border - Disabled 

        }
    }

    [DefaultEvent("CheckedChanged")]
    class ChromeCheckbox : ThemeControl154
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeCheckbox"/> class.
        /// </summary>
        public ChromeCheckbox()
        {
            LockHeight = 17;
            Font = new Font("Segoe UI", 9);
            SetColor("Gradient top normal", 237, 237, 237);
            SetColor("Gradient top over", 242, 242, 242);
            SetColor("Gradient top down", 235, 235, 235);
            SetColor("Gradient bottom normal", 230, 230, 230);
            SetColor("Gradient bottom over", 235, 235, 235);
            SetColor("Gradient bottom down", 223, 223, 223);
            SetColor("Border", 167, 167, 167);
            SetColor("Text", 60, 60, 60);
            Width = 160;
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public override sealed Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        /// <summary>
        /// The _x
        /// </summary>
        private int _x;
        /// <summary>
        /// The _GTN
        /// </summary>
        Color _gtn;
        /// <summary>
        /// The _gto
        /// </summary>
        Color _gto;
        /// <summary>
        /// The _GTD
        /// </summary>
        Color _gtd;
        /// <summary>
        /// The _GBN
        /// </summary>
        Color _gbn;
        /// <summary>
        /// The _gbo
        /// </summary>
        Color _gbo;
        /// <summary>
        /// The _GBD
        /// </summary>
        Color _gbd;
        /// <summary>
        /// The _bo
        /// </summary>
        Color _bo;
        /// <summary>
        /// The _
        /// </summary>
        Color _;
        /// <summary>
        /// Colors the hook.
        /// </summary>
        protected override void ColorHook()
        {
            _gtn = GetColor("Gradient top normal");
            _gto = GetColor("Gradient top over");
            _gtd = GetColor("Gradient top down");
            _gbn = GetColor("Gradient bottom normal");
            _gbo = GetColor("Gradient bottom over");
            _gbd = GetColor("Gradient bottom down");
            _bo = GetColor("Border");
            _ = GetColor("Text");
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseMove" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.MouseEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _x = e.Location.X;
            Invalidate();
        }

        /// <summary>
        /// Paints the hook.
        /// </summary>
        protected override void PaintHook()
        {
            G.Clear(BackColor);
            LinearGradientBrush lgb;
            G.SmoothingMode = SmoothingMode.HighQuality;
            switch (State)
            {
                case MouseStateControl.None:
                    lgb = new LinearGradientBrush(new Rectangle(0, 0, 14, 14), _gtn, _gbn, 90f);
                    break;
                case MouseStateControl.Over:
                    lgb = new LinearGradientBrush(new Rectangle(0, 0, 14, 14), _gto, _gbo, 90f);
                    break;
                default:
                    lgb = new LinearGradientBrush(new Rectangle(0, 0, 14, 14), _gtd, _gbd, 90f);
                    break;
            }
            var buttonpath = CreateRound(Rectangle.Round(lgb.Rectangle), 5);
            G.FillPath(lgb, CreateRound(Rectangle.Round(lgb.Rectangle), 3));
            G.SetClip(buttonpath);
            lgb = new LinearGradientBrush(new Rectangle(0, 0, 14, 5), Color.FromArgb(150, Color.White), Color.Transparent, 90f);
            G.FillRectangle(lgb, Rectangle.Round(lgb.Rectangle));
            G.ResetClip();
            G.DrawPath(new Pen(_bo), buttonpath);

            DrawText(new SolidBrush(_), 17, -2);


            if (!Checked) return;
            var check = Image.FromStream(new MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAsAAAAJCAYAAADkZNYtAAAABGdBTUEAALGOfPtRkwAAACBjSFJNAACHDwAAjA8AAP1SAACBQAAAfXkAAOmLAAA85QAAGcxzPIV3AAAKOWlDQ1BQaG90b3Nob3AgSUNDIHByb2ZpbGUAAEjHnZZ3VFTXFofPvXd6oc0wAlKG3rvAANJ7k15FYZgZYCgDDjM0sSGiAhFFRJoiSFDEgNFQJFZEsRAUVLAHJAgoMRhFVCxvRtaLrqy89/Ly++Osb+2z97n77L3PWhcAkqcvl5cGSwGQyhPwgzyc6RGRUXTsAIABHmCAKQBMVka6X7B7CBDJy82FniFyAl8EAfB6WLwCcNPQM4BOB/+fpFnpfIHomAARm7M5GSwRF4g4JUuQLrbPipgalyxmGCVmvihBEcuJOWGRDT77LLKjmNmpPLaIxTmns1PZYu4V8bZMIUfEiK+ICzO5nCwR3xKxRoowlSviN+LYVA4zAwAUSWwXcFiJIjYRMYkfEuQi4uUA4EgJX3HcVyzgZAvEl3JJS8/hcxMSBXQdli7d1NqaQffkZKVwBALDACYrmcln013SUtOZvBwAFu/8WTLi2tJFRbY0tba0NDQzMv2qUP91829K3NtFehn4uWcQrf+L7a/80hoAYMyJarPziy2uCoDOLQDI3fti0zgAgKSobx3Xv7oPTTwviQJBuo2xcVZWlhGXwzISF/QP/U+Hv6GvvmckPu6P8tBdOfFMYYqALq4bKy0lTcinZ6QzWRy64Z+H+B8H/nUeBkGceA6fwxNFhImmjMtLELWbx+YKuGk8Opf3n5r4D8P+pMW5FonS+BFQY4yA1HUqQH7tBygKESDR+8Vd/6NvvvgwIH554SqTi3P/7zf9Z8Gl4iWDm/A5ziUohM4S8jMX98TPEqABAUgCKpAHykAd6ABDYAasgC1wBG7AG/iDEBAJVgMWSASpgA+yQB7YBApBMdgJ9oBqUAcaQTNoBcdBJzgFzoNL4Bq4AW6D+2AUTIBnYBa8BgsQBGEhMkSB5CEVSBPSh8wgBmQPuUG+UBAUCcVCCRAPEkJ50GaoGCqDqqF6qBn6HjoJnYeuQIPQXWgMmoZ+h97BCEyCqbASrAUbwwzYCfaBQ+BVcAK8Bs6FC+AdcCXcAB+FO+Dz8DX4NjwKP4PnEIAQERqiihgiDMQF8UeikHiEj6xHipAKpAFpRbqRPuQmMorMIG9RGBQFRUcZomxRnqhQFAu1BrUeVYKqRh1GdaB6UTdRY6hZ1Ec0Ga2I1kfboL3QEegEdBa6EF2BbkK3oy+ib6Mn0K8xGAwNo42xwnhiIjFJmLWYEsw+TBvmHGYQM46Zw2Kx8lh9rB3WH8vECrCF2CrsUexZ7BB2AvsGR8Sp4Mxw7rgoHA+Xj6vAHcGdwQ3hJnELeCm8Jt4G749n43PwpfhGfDf+On4Cv0CQJmgT7AghhCTCJkIloZVwkfCA8JJIJKoRrYmBRC5xI7GSeIx4mThGfEuSIemRXEjRJCFpB+kQ6RzpLuklmUzWIjuSo8gC8g5yM/kC+RH5jQRFwkjCS4ItsUGiRqJDYkjiuSReUlPSSXK1ZK5kheQJyeuSM1J4KS0pFymm1HqpGqmTUiNSc9IUaVNpf+lU6RLpI9JXpKdksDJaMm4ybJkCmYMyF2TGKQhFneJCYVE2UxopFykTVAxVm+pFTaIWU7+jDlBnZWVkl8mGyWbL1sielh2lITQtmhcthVZKO04bpr1borTEaQlnyfYlrUuGlszLLZVzlOPIFcm1yd2WeydPl3eTT5bfJd8p/1ABpaCnEKiQpbBf4aLCzFLqUtulrKVFS48vvacIK+opBimuVTyo2K84p6Ss5KGUrlSldEFpRpmm7KicpFyufEZ5WoWiYq/CVSlXOavylC5Ld6Kn0CvpvfRZVUVVT1Whar3qgOqCmrZaqFq+WpvaQ3WCOkM9Xr1cvUd9VkNFw08jT6NF454mXpOhmai5V7NPc15LWytca6tWp9aUtpy2l3audov2Ax2yjoPOGp0GnVu6GF2GbrLuPt0berCehV6iXo3edX1Y31Kfq79Pf9AAbWBtwDNoMBgxJBk6GWYathiOGdGMfI3yjTqNnhtrGEcZ7zLuM/5oYmGSYtJoct9UxtTbNN+02/R3Mz0zllmN2S1zsrm7+QbzLvMXy/SXcZbtX3bHgmLhZ7HVosfig6WVJd+y1XLaSsMq1qrWaoRBZQQwShiXrdHWztYbrE9Zv7WxtBHYHLf5zdbQNtn2iO3Ucu3lnOWNy8ft1OyYdvV2o/Z0+1j7A/ajDqoOTIcGh8eO6o5sxybHSSddpySno07PnU2c+c7tzvMuNi7rXM65Iq4erkWuA24ybqFu1W6P3NXcE9xb3Gc9LDzWepzzRHv6eO7yHPFS8mJ5NXvNelt5r/Pu9SH5BPtU+zz21fPl+3b7wX7efrv9HqzQXMFb0ekP/L38d/s/DNAOWBPwYyAmMCCwJvBJkGlQXlBfMCU4JvhI8OsQ55DSkPuhOqHC0J4wybDosOaw+XDX8LLw0QjjiHUR1yIVIrmRXVHYqLCopqi5lW4r96yciLaILoweXqW9KnvVldUKq1NWn46RjGHGnIhFx4bHHol9z/RnNjDn4rziauNmWS6svaxnbEd2OXuaY8cp40zG28WXxU8l2CXsTphOdEisSJzhunCruS+SPJPqkuaT/ZMPJX9KCU9pS8Wlxqae5Mnwknm9acpp2WmD6frphemja2zW7Fkzy/fhN2VAGasyugRU0c9Uv1BHuEU4lmmfWZP5Jiss60S2dDYvuz9HL2d7zmSue+63a1FrWWt78lTzNuWNrXNaV78eWh+3vmeD+oaCDRMbPTYe3kTYlLzpp3yT/LL8V5vDN3cXKBVsLBjf4rGlpVCikF84stV2a9021DbutoHt5turtn8sYhddLTYprih+X8IqufqN6TeV33zaEb9joNSydP9OzE7ezuFdDrsOl0mX5ZaN7/bb3VFOLy8qf7UnZs+VimUVdXsJe4V7Ryt9K7uqNKp2Vr2vTqy+XeNc01arWLu9dn4fe9/Qfsf9rXVKdcV17w5wD9yp96jvaNBqqDiIOZh58EljWGPft4xvm5sUmoqbPhziHRo9HHS4t9mqufmI4pHSFrhF2DJ9NProje9cv+tqNWytb6O1FR8Dx4THnn4f+/3wcZ/jPScYJ1p/0Pyhtp3SXtQBdeR0zHYmdo52RXYNnvQ+2dNt293+o9GPh06pnqo5LXu69AzhTMGZT2dzz86dSz83cz7h/HhPTM/9CxEXbvUG9g5c9Ll4+ZL7pQt9Tn1nL9tdPnXF5srJq4yrndcsr3X0W/S3/2TxU/uA5UDHdavrXTesb3QPLh88M+QwdP6m681Lt7xuXbu94vbgcOjwnZHokdE77DtTd1PuvriXeW/h/sYH6AdFD6UeVjxSfNTws+7PbaOWo6fHXMf6Hwc/vj/OGn/2S8Yv7ycKnpCfVEyqTDZPmU2dmnafvvF05dOJZ+nPFmYKf5X+tfa5zvMffnP8rX82YnbiBf/Fp99LXsq/PPRq2aueuYC5R69TXy/MF72Rf3P4LeNt37vwd5MLWe+x7ys/6H7o/ujz8cGn1E+f/gUDmPP8usTo0wAAAAlwSFlzAAALEQAACxEBf2RfkQAAAK1JREFUKFN10D0OhSAQBGAOp2D8u4CNHY0kegFPYaSyM+EQFhY2NsTGcJ3xQbEvxlBMQsg3SxYGgMWitUbbtjiO40fAotBaizzPIYQI8YUo7rqO4DAM78nneYYLH2MMOOchdV3DOffH4zgiyzJM04T7vlFVFeF1XWkI27YNaZpSiqKgs1KKIC0opXwVfLksS1zX9cW+Nc9zeDpJkpBlWV7w83X7vqNpGvR9/4EePztSBhXQfRi8AAAAAElFTkSuQmCC")));
            G.DrawImage(check, new Rectangle(2, 3, check.Width, check.Height));
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChromeCheckbox"/> is checked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if checked; otherwise, <c>false</c>.
        /// </value>
        private bool Checked { get; set; }

        /// <summary>
        /// Raises the <see cref="E:MouseDown" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Checked = !Checked;
            if (CheckedChanged != null)
                CheckedChanged(this);
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Occurs when [checked changed].
        /// </summary>
        public event CheckedChangedEventHandler CheckedChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">The sender.</param>
        public delegate void CheckedChangedEventHandler(object sender);

    }

    [DefaultEvent("CheckedChanged")]
    class ChromeRadioButton : ThemeControl154
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeRadioButton"/> class.
        /// </summary>
        public ChromeRadioButton()
        {
            Font = new Font("Segoe UI", 9);
            LockHeight = 17;
            SetColor("Text", 60, 60, 60);
            SetColor("Gradient top", 237, 237, 237);
            SetColor("Gradient bottom", 230, 230, 230);
            SetColor("Borders", 167, 167, 167);
            SetColor("Bullet", 100, 100, 100);
            Width = 180;
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public override sealed Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        /// <summary>
        /// The _x
        /// </summary>
        private int _x;
        /// <summary>
        /// The _text color
        /// </summary>
        private Color _textColor;
        /// <summary>
        /// The _G1
        /// </summary>
        private Color _g1;
        /// <summary>
        /// The _G2
        /// </summary>
        private Color _g2;
        /// <summary>
        /// The _bo
        /// </summary>
        private Color _bo;
        /// <summary>
        /// The _BB
        /// </summary>
        private Color _bb;
        /// <summary>
        /// Colors the hook.
        /// </summary>
        protected override void ColorHook()
        {
            _textColor = GetColor("Text");
            _g1 = GetColor("Gradient top");
            _g2 = GetColor("Gradient bottom");
            _bb = GetColor("Bullet");
            _bo = GetColor("Borders");
        }

        /// <summary>
        /// Löst das <see cref="E:System.Windows.Forms.Control.MouseMove" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.Windows.Forms.MouseEventArgs" />, das die Ereignisdaten enthält.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _x = e.Location.X;
            Invalidate();
        }

        /// <summary>
        /// Paints the hook.
        /// </summary>
        protected override void PaintHook()
        {
            G.Clear(BackColor);
            G.SmoothingMode = SmoothingMode.HighQuality;
            if (_checked)
            {
                var lgb = new LinearGradientBrush(new Rectangle(new Point(0, 0), new Size(14, 14)), _g1, _g2, 90f);
                G.FillEllipse(lgb, new Rectangle(new Point(0, 0), new Size(14, 14)));
            }
            else
            {
                var lgb = new LinearGradientBrush(new Rectangle(new Point(0, 0), new Size(14, 16)), _g1, _g2, 90f);
                G.FillEllipse(lgb, new Rectangle(new Point(0, 0), new Size(14, 14)));
            }

            if (State == MouseStateControl.Over & _x < 15)
            {
                var sb = new SolidBrush(Color.FromArgb(10, Color.Black));
                G.FillEllipse(sb, new Rectangle(new Point(0, 0), new Size(14, 14)));
            }
            else if (State == MouseStateControl.Down & _x < 15)
            {
                var sb = new SolidBrush(Color.FromArgb(20, Color.Black));
                G.FillEllipse(sb, new Rectangle(new Point(0, 0), new Size(14, 14)));
            }

            var p = new GraphicsPath();
            p.AddEllipse(new Rectangle(0, 0, 14, 14));
            G.SetClip(p);

            var llggbb = new LinearGradientBrush(new Rectangle(0, 0, 14, 5), Color.FromArgb(150, Color.White), Color.Transparent, 90f);
            G.FillRectangle(llggbb, llggbb.Rectangle);

            G.ResetClip();

            G.DrawEllipse(new Pen(_bo), new Rectangle(new Point(0, 0), new Size(14, 14)));

            if (_checked)
            {
                var lgb = new SolidBrush(_bb);
                G.FillEllipse(lgb, new Rectangle(new Point(4, 4), new Size(6, 6)));
            }

            DrawText(new SolidBrush(_textColor), HorizontalAlignment.Left, 17, -2);
        }

        /// <summary>
        /// The _field
        /// </summary>
        private int _field = 16;
        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public int Field
        {
            get { return _field; }
            set
            {
                if (value < 4)
                    return;
                _field = value;
                LockHeight = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The _checked
        /// </summary>
        private bool _checked;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChromeRadioButton"/> is checked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if checked; otherwise, <c>false</c>.
        /// </value>
        private bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                InvalidateControls();
                if (CheckedChanged != null)
                {
                    CheckedChanged(this);
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDown" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!_checked)
                Checked = true;
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Occurs when [checked changed].
        /// </summary>
        public event CheckedChangedEventHandler CheckedChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">The sender.</param>
        public delegate void CheckedChangedEventHandler(object sender);

        /// <summary>
        /// Called when [creation].
        /// </summary>
        protected override void OnCreation()
        {
            InvalidateControls();
        }

        /// <summary>
        /// Invalidates the controls.
        /// </summary>
        private void InvalidateControls()
        {
            if (!IsHandleCreated || !_checked)
                return;

            foreach (var c in from Control c in Parent.Controls where !ReferenceEquals(c, this) && c is ChromeRadioButton select c)
            {
                ((ChromeRadioButton)c).Checked = false;
            }
        }
    }

    [DefaultEvent("Scroll")]
    class ChromeTrackBar : ThemeControl154
    {

        public event ScrollEventHandler Scroll;
        public delegate void ScrollEventHandler(object sender);

        private int _Minimum;
        public int Minimum
        {
            get { return _Minimum; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Property value is not valid.");
                }

                _Minimum = value;
                if (value > _Value)
                    _Value = value;
                if (value > _Maximum)
                    _Maximum = value;
                Invalidate();
            }
        }

        private int _Maximum = 10;
        public int Maximum
        {
            get { return _Maximum; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Property value is not valid.");
                }

                _Maximum = value;
                if (value < _Value)
                    _Value = value;
                if (value < _Minimum)
                    _Minimum = value;
                Invalidate();
            }
        }

        private int _Value;
        public int Value
        {
            get { return _Value; }
            set
            {
                if (value == _Value)
                    return;

                if (value > _Maximum || value < _Minimum)
                {
                    throw new Exception("Property value is not valid.");
                }

                _Value = value;
                Invalidate();

                if (Scroll != null)
                {
                    Scroll(this);
                }
            }
        }

        public ChromeTrackBar()
        {
            LockHeight = 17;

            SetColor("Track", 100, 100, 100);
            SetColor("Border", 70, 70, 70);
            SetColor("Line", 135, 135, 135);
            SetColor("BarBack", 118, 118, 118);
            SetColor("BarGloss1", 150, 150, 150);
            SetColor("BarGloss2", 128, 128, 128);
            SetColor("BarShine", 165, 165, 165);
            SetColor("BarBorder", 80, 80, 80);

            //SetColor("Track", 100, 100, 100);
            //SetColor("Border", 70, 70, 70);
            //SetColor("Line", 135, 135, 135);
            //SetColor("BarBack", 118, 118, 118);
            //SetColor("BarGloss1", 150, 150, 150);
            //SetColor("BarGloss2", 128, 128, 128);
            //SetColor("BarShine", 165, 165, 165);
            //SetColor("BarBorder", 80, 80, 80);
        }

        private SolidBrush B1;
        private SolidBrush B2;
        private Pen P1;
        private Pen P2;
        private Pen P3;
        private Pen P4;
        private Color C1;

        private Color C2;
        protected override void ColorHook()
        {
            B1 = GetBrush("Track");
            B2 = GetBrush("BarBack");

            P1 = GetPen("Border");
            P2 = GetPen("Line");
            P3 = GetPen("BarShine");
            P4 = GetPen("BarBorder");

            C1 = GetColor("BarGloss1");
            C2 = GetColor("BarGloss2");
        }

        private Rectangle R1;

        private int I1;
        protected override void PaintHook()
        {
            G.Clear(BackColor);

            G.FillRectangle(B1, 0, 8, Width, 2);

            DrawBorders(P1, 0, 7, Width, 4);
            G.DrawLine(P2, 0, 11, Width, 11);

            I1 = Convert.ToInt32((double)(_Value - _Minimum) / (_Maximum - _Minimum) * (Width - 9));
            R1 = new Rectangle(I1, 0, 9, 17);

            G.FillRectangle(B2, R1);
            DrawGradient(C1, C2, R1.X, 0, 9, 7);

            G.DrawLine(P3, R1.X, 1, R1.X + 8, 1);
            G.DrawRectangle(P4, R1.X, 0, 8, 16);
        }

        private bool TrackDown;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                I1 = Convert.ToInt32((double)(_Value - _Minimum) / (_Maximum - _Minimum) * (Width - 9));
                R1 = new Rectangle(I1, 0, 9, 17);

                TrackDown = R1.Contains(e.Location);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (TrackDown && e.X > -1 && e.X < (Width + 1))
            {
                Value = _Minimum + Convert.ToInt32((_Maximum - _Minimum) * ((double)e.X / Width));
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            TrackDown = false;
            base.OnMouseUp(e);
        }

    }

    //------------------
    //Creator: aeonhack
    //Site: elitevs.net
    //Created: 08/02/2011
    //Changed: 12/06/2011
    //Version: 1.5.4
    //------------------
    abstract class ThemeContainer154 : ContainerControl
    {

        #region " Initialization "

        protected Graphics G;

        private Bitmap B;

        protected ThemeContainer154()
        {
            SetStyle((ControlStyles)139270, true);

            _ImageSize = Size.Empty;
            Font = new Font("Verdana", 8);

            MeasureBitmap = new Bitmap(1, 1);
            MeasureGraphics = Graphics.FromImage(MeasureBitmap);

            DrawRadialPath = new GraphicsPath();

            InvalidateCustimization();
        }

        protected override sealed void OnHandleCreated(EventArgs e)
        {
            if (_doneCreation)
                InitializeMessages();

            InvalidateCustimization();
            ColorHook();

            if (_LockWidth != 0)
                Width = _LockWidth;
            if (_LockHeight != 0)
                Height = _LockHeight;
            if (!_ControlMode)
                base.Dock = DockStyle.Fill;

            Transparent = _Transparent;
            if (_Transparent && _BackColor)
                BackColor = Color.Transparent;

            base.OnHandleCreated(e);
        }

        private bool _doneCreation;
        protected override sealed void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent == null)
                return;
            _IsParentForm = Parent is Form;

            if (!_ControlMode)
            {
                InitializeMessages();

                if (_IsParentForm)
                {
                    if (ParentForm != null)
                    {
                        ParentForm.FormBorderStyle = _BorderStyle;
                        ParentForm.TransparencyKey = _TransparencyKey;

                        if (!DesignMode)
                        {
                            ParentForm.Shown += FormShown;
                        }
                    }
                }

                Parent.BackColor = BackColor;
            }

            OnCreation();
            _doneCreation = true;
            InvalidateTimer();
        }

        #endregion

        private void DoAnimation(bool i)
        {
            OnAnimation();
            if (i)
                Invalidate();
        }

        protected override sealed void OnPaint(PaintEventArgs e)
        {
            if (Width == 0 || Height == 0)
                return;

            if (_Transparent && _ControlMode)
            {
                PaintHook();
                e.Graphics.DrawImage(B, 0, 0);
            }
            else
            {
                G = e.Graphics;
                PaintHook();
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            ThemeShare.RemoveAnimationCallback(DoAnimation);
            base.OnHandleDestroyed(e);
        }

        private bool _hasShown;
        private void FormShown(object sender, EventArgs e)
        {
            if (_ControlMode || _hasShown)
                return;

            if (_StartPosition == FormStartPosition.CenterParent || _StartPosition == FormStartPosition.CenterScreen)
            {
                Rectangle SB = Screen.PrimaryScreen.Bounds;
                Rectangle CB = ParentForm.Bounds;
                ParentForm.Location = new Point(SB.Width / 2 - CB.Width / 2, SB.Height / 2 - CB.Width / 2);
            }

            _hasShown = true;
        }


        #region " Size Handling "

        private Rectangle _frame;
        protected override sealed void OnSizeChanged(EventArgs e)
        {
            if (_Movable && !_ControlMode)
            {
                _frame = new Rectangle(7, 7, Width - 14, _Header - 7);
            }

            InvalidateBitmap();
            Invalidate();

            base.OnSizeChanged(e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (_LockWidth != 0)
                width = _LockWidth;
            if (_LockHeight != 0)
                height = _LockHeight;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        #endregion

        #region " State Handling "

        private MouseStateControl State;
        private void SetState(MouseStateControl current)
        {
            State = current;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (ParentForm != null && !(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized))
            {
                if (_Sizable && !_ControlMode)
                    InvalidateMouse();
            }

            base.OnMouseMove(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            SetState(Enabled ? MouseStateControl.None : MouseStateControl.Block);
            base.OnEnabledChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            SetState(MouseStateControl.Over);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            SetState(MouseStateControl.Over);
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            SetState(MouseStateControl.None);

            if (GetChildAtPoint(PointToClient(MousePosition)) != null)
            {
                if (_Sizable && !_ControlMode)
                {
                    Cursor = Cursors.Default;
                    _previous = 0;
                }
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SetState(MouseStateControl.Down);

            if (ParentForm != null && !(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized || _ControlMode))
            {
                if (_Movable && _frame.Contains(e.Location))
                {
                    if (!new Rectangle(Width - 22, 5, 15, 15).Contains(e.Location))
                    {
                        Capture = false;
                    }
                    _wmLmbuttondown = true;
                    DefWndProc(ref _messages[0]);
                }
                else if (_Sizable && _previous != 0)
                {
                    Capture = false;
                    _wmLmbuttondown = true;
                    DefWndProc(ref _messages[_previous]);
                }
            }

            base.OnMouseDown(e);
        }

        private bool _wmLmbuttondown;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (_wmLmbuttondown && m.Msg == 513)
            {
                _wmLmbuttondown = false;

                SetState(MouseStateControl.Over);
                if (!_SmartBounds)
                    return;

                CorrectBounds(IsParentMdi ? new Rectangle(Point.Empty, Parent.Parent.Size) : Screen.FromControl(Parent).WorkingArea);
            }
        }

        private Point _getIndexPoint;
        private bool _b1;
        private bool _b2;
        private bool _b3;
        private bool _b4;
        private int GetIndex()
        {
            _getIndexPoint = PointToClient(MousePosition);
            _b1 = _getIndexPoint.X < 7;
            _b2 = _getIndexPoint.X > Width - 7;
            _b3 = _getIndexPoint.Y < 7;
            _b4 = _getIndexPoint.Y > Height - 7;

            if (_b1 && _b3)
                return 4;
            if (_b1 && _b4)
                return 7;
            if (_b2 && _b3)
                return 5;
            if (_b2 && _b4)
                return 8;
            if (_b1)
                return 1;
            if (_b2)
                return 2;
            if (_b3)
                return 3;
            return _b4 ? 6 : 0;
        }

        private int _current;
        private int _previous;
        private void InvalidateMouse()
        {
            _current = GetIndex();
            if (_current == _previous)
                return;

            _previous = _current;
            switch (_previous)
            {
                case 0:
                    Cursor = Cursors.Default;
                    break;
                case 1:
                case 2:
                    Cursor = Cursors.SizeWE;
                    break;
                case 3:
                case 6:
                    Cursor = Cursors.SizeNS;
                    break;
                case 4:
                case 8:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case 5:
                case 7:
                    Cursor = Cursors.SizeNESW;
                    break;
            }
        }

        private readonly Message[] _messages = new Message[9];
        private void InitializeMessages()
        {
            _messages[0] = Message.Create(Parent.Handle, 161, new IntPtr(2), IntPtr.Zero);
            for (var I = 1; I <= 8; I++)
            {
                _messages[I] = Message.Create(Parent.Handle, 161, new IntPtr(I + 9), IntPtr.Zero);
            }
        }

        private void CorrectBounds(Rectangle bounds)
        {
            if (Parent.Width > bounds.Width)
                Parent.Width = bounds.Width;
            if (Parent.Height > bounds.Height)
                Parent.Height = bounds.Height;

            int X = Parent.Location.X;
            int Y = Parent.Location.Y;

            if (X < bounds.X)
                X = bounds.X;
            if (Y < bounds.Y)
                Y = bounds.Y;

            int Width = bounds.X + bounds.Width;
            int Height = bounds.Y + bounds.Height;

            if (X + Parent.Width > Width)
                X = Width - Parent.Width;
            if (Y + Parent.Height > Height)
                Y = Height - Parent.Height;

            Parent.Location = new Point(X, Y);
        }

        #endregion


        #region " Base Properties "

        public override DockStyle Dock
        {
            get { return base.Dock; }
            set
            {
                if (!_ControlMode)
                    return;
                base.Dock = value;
            }
        }

        private bool _BackColor;
        [Category("Misc")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                if (value == base.BackColor)
                    return;

                if (!IsHandleCreated && _ControlMode && value == Color.Transparent)
                {
                    _BackColor = true;
                    return;
                }

                base.BackColor = value;
                if (Parent != null)
                {
                    if (!_ControlMode)
                        Parent.BackColor = value;
                    ColorHook();
                }
            }
        }

        public override Size MinimumSize
        {
            get { return base.MinimumSize; }
            set
            {
                base.MinimumSize = value;
                if (Parent != null)
                    Parent.MinimumSize = value;
            }
        }

        public override Size MaximumSize
        {
            get { return base.MaximumSize; }
            set
            {
                base.MaximumSize = value;
                if (Parent != null)
                    Parent.MaximumSize = value;
            }
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }

        public override sealed Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                Invalidate();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor
        {
            get { return Color.Empty; }
            set { }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get { return null; }
            set { }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return ImageLayout.None; }
            set { }
        }

        #endregion

        #region " Public Properties "

        private bool _SmartBounds = true;
        public bool SmartBounds
        {
            get { return _SmartBounds; }
            set { _SmartBounds = value; }
        }

        private bool _Movable = true;
        public bool Movable
        {
            get { return _Movable; }
            set { _Movable = value; }
        }

        private bool _Sizable = true;
        public bool Sizable
        {
            get { return _Sizable; }
            set { _Sizable = value; }
        }

        private Color _TransparencyKey;
        public Color TransparencyKey
        {
            get
            {
                if (_IsParentForm && !_ControlMode)
                    return ParentForm.TransparencyKey;
                else
                    return _TransparencyKey;
            }
            set
            {
                if (value == _TransparencyKey)
                    return;
                _TransparencyKey = value;

                if (_IsParentForm && !_ControlMode)
                {
                    ParentForm.TransparencyKey = value;
                    ColorHook();
                }
            }
        }

        private FormBorderStyle _BorderStyle;
        public FormBorderStyle BorderStyle
        {
            get
            {
                if (_IsParentForm && !_ControlMode)
                    return ParentForm.FormBorderStyle;
                else
                    return _BorderStyle;
            }
            set
            {
                _BorderStyle = value;

                if (_IsParentForm && !_ControlMode)
                {
                    ParentForm.FormBorderStyle = value;

                    if (!(value == FormBorderStyle.None))
                    {
                        Movable = false;
                        Sizable = false;
                    }
                }
            }
        }

        private FormStartPosition _StartPosition;
        public FormStartPosition StartPosition
        {
            get
            {
                if (_IsParentForm && !_ControlMode)
                    return ParentForm.StartPosition;
                else
                    return _StartPosition;
            }
            set
            {
                _StartPosition = value;

                if (_IsParentForm && !_ControlMode)
                {
                    ParentForm.StartPosition = value;
                }
            }
        }

        private bool _NoRounding;
        public bool NoRounding
        {
            get { return _NoRounding; }
            set
            {
                _NoRounding = value;
                Invalidate();
            }
        }

        private Image _Image;
        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value == null)
                    _ImageSize = Size.Empty;
                else
                    _ImageSize = value.Size;

                _Image = value;
                Invalidate();
            }
        }

        private Dictionary<string, Color> Items = new Dictionary<string, Color>();
        public Bloom[] Colors
        {
            get
            {
                List<Bloom> T = new List<Bloom>();
                Dictionary<string, Color>.Enumerator E = Items.GetEnumerator();

                while (E.MoveNext())
                {
                    T.Add(new Bloom(E.Current.Key, E.Current.Value));
                }

                return T.ToArray();
            }
            set
            {
                foreach (Bloom B in value)
                {
                    if (Items.ContainsKey(B.Name))
                        Items[B.Name] = B.Value;
                }

                InvalidateCustimization();
                ColorHook();
                Invalidate();
            }
        }

        private string _Customization;
        public string Customization
        {
            get { return _Customization; }
            set
            {
                if (value == _Customization)
                    return;

                byte[] Data = null;
                Bloom[] Items = Colors;

                try
                {
                    Data = Convert.FromBase64String(value);
                    for (int I = 0; I <= Items.Length - 1; I++)
                    {
                        Items[I].Value = Color.FromArgb(BitConverter.ToInt32(Data, I * 4));
                    }
                }
                catch
                {
                    return;
                }

                _Customization = value;

                Colors = Items;
                ColorHook();
                Invalidate();
            }
        }

        private bool _Transparent;
        public bool Transparent
        {
            get { return _Transparent; }
            set
            {
                _Transparent = value;
                if (!(IsHandleCreated || _ControlMode))
                    return;

                if (!value && !(BackColor.A == 255))
                {
                    throw new Exception("Unable to change value to false while a transparent BackColor is in use.");
                }

                SetStyle(ControlStyles.Opaque, !value);
                SetStyle(ControlStyles.SupportsTransparentBackColor, value);

                InvalidateBitmap();
                Invalidate();
            }
        }

        #endregion

        #region " Private Properties "

        private Size _ImageSize;
        protected Size ImageSize
        {
            get { return _ImageSize; }
        }

        private bool _IsParentForm;
        protected bool IsParentForm
        {
            get { return _IsParentForm; }
        }

        protected bool IsParentMdi
        {
            get
            {
                if (Parent == null)
                    return false;
                return Parent.Parent != null;
            }
        }

        private int _LockWidth;
        protected int LockWidth
        {
            get { return _LockWidth; }
            set
            {
                _LockWidth = value;
                if (!(LockWidth == 0) && IsHandleCreated)
                    Width = LockWidth;
            }
        }

        private int _LockHeight;
        protected int LockHeight
        {
            get { return _LockHeight; }
            set
            {
                _LockHeight = value;
                if (!(LockHeight == 0) && IsHandleCreated)
                    Height = LockHeight;
            }
        }

        private int _Header = 24;
        protected int Header
        {
            get { return _Header; }
            set
            {
                _Header = value;

                if (!_ControlMode)
                {
                    _frame = new Rectangle(7, 7, Width - 14, value - 7);
                    Invalidate();
                }
            }
        }

        private bool _ControlMode;
        protected bool ControlMode
        {
            get { return _ControlMode; }
            set
            {
                _ControlMode = value;

                Transparent = _Transparent;
                if (_Transparent && _BackColor)
                    BackColor = Color.Transparent;

                InvalidateBitmap();
                Invalidate();
            }
        }

        private bool _IsAnimated;
        protected bool IsAnimated
        {
            get { return _IsAnimated; }
            set
            {
                _IsAnimated = value;
                InvalidateTimer();
            }
        }

        #endregion


        #region " Property Helpers "

        protected Pen GetPen(string name)
        {
            return new Pen(Items[name]);
        }
        protected Pen GetPen(string name, float width)
        {
            return new Pen(Items[name], width);
        }

        protected SolidBrush GetBrush(string name)
        {
            return new SolidBrush(Items[name]);
        }

        protected Color GetColor(string name)
        {
            return Items[name];
        }

        protected void SetColor(string name, Color value)
        {
            if (Items.ContainsKey(name))
                Items[name] = value;
            else
                Items.Add(name, value);
        }
        protected void SetColor(string name, byte r, byte g, byte b)
        {
            SetColor(name, Color.FromArgb(r, g, b));
        }
        protected void SetColor(string name, byte a, byte r, byte g, byte b)
        {
            SetColor(name, Color.FromArgb(a, r, g, b));
        }
        protected void SetColor(string name, byte a, Color value)
        {
            SetColor(name, Color.FromArgb(a, value));
        }

        private void InvalidateBitmap()
        {
            if (_Transparent && _ControlMode)
            {
                if (Width == 0 || Height == 0)
                    return;
                B = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
                G = Graphics.FromImage(B);
            }
            else
            {
                G = null;
                B = null;
            }
        }

        private void InvalidateCustimization()
        {
            MemoryStream M = new MemoryStream(Items.Count * 4);

            foreach (Bloom B in Colors)
            {
                M.Write(BitConverter.GetBytes(B.Value.ToArgb()), 0, 4);
            }

            M.Close();
            _Customization = Convert.ToBase64String(M.ToArray());
        }

        private void InvalidateTimer()
        {
            if (DesignMode || !_doneCreation)
                return;

            if (_IsAnimated)
            {
                ThemeShare.AddAnimationCallback(DoAnimation);
            }
            else
            {
                ThemeShare.RemoveAnimationCallback(DoAnimation);
            }
        }

        #endregion


        #region " User Hooks "

        protected abstract void ColorHook();
        protected abstract void PaintHook();

        protected virtual void OnCreation()
        {
        }

        protected virtual void OnAnimation()
        {
        }

        #endregion


        #region " Offset "

        private Rectangle OffsetReturnRectangle;
        protected Rectangle Offset(Rectangle r, int amount)
        {
            OffsetReturnRectangle = new Rectangle(r.X + amount, r.Y + amount, r.Width - (amount * 2), r.Height - (amount * 2));
            return OffsetReturnRectangle;
        }

        private Size OffsetReturnSize;
        protected Size Offset(Size s, int amount)
        {
            OffsetReturnSize = new Size(s.Width + amount, s.Height + amount);
            return OffsetReturnSize;
        }

        private Point OffsetReturnPoint;
        protected Point Offset(Point p, int amount)
        {
            OffsetReturnPoint = new Point(p.X + amount, p.Y + amount);
            return OffsetReturnPoint;
        }

        #endregion

        #region " Center "


        private Point CenterReturn;
        protected Point Center(Rectangle p, Rectangle c)
        {
            CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X + c.X, (p.Height / 2 - c.Height / 2) + p.Y + c.Y);
            return CenterReturn;
        }
        protected Point Center(Rectangle p, Size c)
        {
            CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X, (p.Height / 2 - c.Height / 2) + p.Y);
            return CenterReturn;
        }

        protected Point Center(Rectangle child)
        {
            return Center(Width, Height, child.Width, child.Height);
        }
        protected Point Center(Size child)
        {
            return Center(Width, Height, child.Width, child.Height);
        }
        protected Point Center(int childWidth, int childHeight)
        {
            return Center(Width, Height, childWidth, childHeight);
        }

        protected Point Center(Size p, Size c)
        {
            return Center(p.Width, p.Height, c.Width, c.Height);
        }

        protected Point Center(int pWidth, int pHeight, int cWidth, int cHeight)
        {
            CenterReturn = new Point(pWidth / 2 - cWidth / 2, pHeight / 2 - cHeight / 2);
            return CenterReturn;
        }

        #endregion

        #region " Measure "

        private Bitmap MeasureBitmap;

        private Graphics MeasureGraphics;
        protected Size Measure()
        {
            lock (MeasureGraphics)
            {
                return MeasureGraphics.MeasureString(Text, Font, Width).ToSize();
            }
        }
        protected Size Measure(string text)
        {
            lock (MeasureGraphics)
            {
                return MeasureGraphics.MeasureString(text, Font, Width).ToSize();
            }
        }

        #endregion


        #region " DrawPixel "


        private SolidBrush DrawPixelBrush;
        protected void DrawPixel(Color c1, int x, int y)
        {
            if (_Transparent)
            {
                B.SetPixel(x, y, c1);
            }
            else
            {
                DrawPixelBrush = new SolidBrush(c1);
                G.FillRectangle(DrawPixelBrush, x, y, 1, 1);
            }
        }

        #endregion

        #region " DrawCorners "


        private SolidBrush DrawCornersBrush;
        protected void DrawCorners(Color c1, int offset)
        {
            DrawCorners(c1, 0, 0, Width, Height, offset);
        }
        protected void DrawCorners(Color c1, Rectangle r1, int offset)
        {
            DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height, offset);
        }
        protected void DrawCorners(Color c1, int x, int y, int width, int height, int offset)
        {
            DrawCorners(c1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
        }

        protected void DrawCorners(Color c1)
        {
            DrawCorners(c1, 0, 0, Width, Height);
        }
        protected void DrawCorners(Color c1, Rectangle r1)
        {
            DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height);
        }
        protected void DrawCorners(Color c1, int x, int y, int width, int height)
        {
            if (_NoRounding)
                return;

            if (_Transparent)
            {
                B.SetPixel(x, y, c1);
                B.SetPixel(x + (width - 1), y, c1);
                B.SetPixel(x, y + (height - 1), c1);
                B.SetPixel(x + (width - 1), y + (height - 1), c1);
            }
            else
            {
                DrawCornersBrush = new SolidBrush(c1);
                G.FillRectangle(DrawCornersBrush, x, y, 1, 1);
                G.FillRectangle(DrawCornersBrush, x + (width - 1), y, 1, 1);
                G.FillRectangle(DrawCornersBrush, x, y + (height - 1), 1, 1);
                G.FillRectangle(DrawCornersBrush, x + (width - 1), y + (height - 1), 1, 1);
            }
        }

        #endregion

        #region " DrawBorders "

        protected void DrawBorders(Pen p1, int offset)
        {
            DrawBorders(p1, 0, 0, Width, Height, offset);
        }
        protected void DrawBorders(Pen p1, Rectangle r, int offset)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset);
        }
        protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset)
        {
            DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
        }

        protected void DrawBorders(Pen p1)
        {
            DrawBorders(p1, 0, 0, Width, Height);
        }
        protected void DrawBorders(Pen p1, Rectangle r)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height);
        }
        protected void DrawBorders(Pen p1, int x, int y, int width, int height)
        {
            G.DrawRectangle(p1, x, y, width - 1, height - 1);
        }

        #endregion

        #region " DrawText "

        private Point DrawTextPoint;

        private Size DrawTextSize;
        protected void DrawText(Brush b1, HorizontalAlignment a, int x, int y)
        {
            DrawText(b1, Text, a, x, y);
        }
        protected void DrawText(Brush b1, string text, HorizontalAlignment a, int x, int y)
        {
            if (text.Length == 0)
                return;

            DrawTextSize = Measure(text);
            DrawTextPoint = new Point(Width / 2 - DrawTextSize.Width / 2, Header / 2 - DrawTextSize.Height / 2);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawString(text, Font, b1, x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawString(text, Font, b1, DrawTextPoint.X + x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawString(text, Font, b1, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y);
                    break;
            }
        }

        protected void DrawText(Brush b1, Point p1)
        {
            if (Text.Length == 0)
                return;
            G.DrawString(Text, Font, b1, p1);
        }
        protected void DrawText(Brush b1, int x, int y)
        {
            if (Text.Length == 0)
                return;
            G.DrawString(Text, Font, b1, x, y);
        }

        #endregion

        #region " DrawImage "


        private Point DrawImagePoint;
        protected void DrawImage(HorizontalAlignment a, int x, int y)
        {
            DrawImage(_Image, a, x, y);
        }
        protected void DrawImage(Image image, HorizontalAlignment a, int x, int y)
        {
            if (image == null)
                return;
            DrawImagePoint = new Point(Width / 2 - image.Width / 2, Header / 2 - image.Height / 2);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawImage(image, x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
            }
        }

        protected void DrawImage(Point p1)
        {
            DrawImage(_Image, p1.X, p1.Y);
        }
        protected void DrawImage(int x, int y)
        {
            DrawImage(_Image, x, y);
        }

        protected void DrawImage(Image image, Point p1)
        {
            DrawImage(image, p1.X, p1.Y);
        }
        protected void DrawImage(Image image, int x, int y)
        {
            if (image == null)
                return;
            G.DrawImage(image, x, y, image.Width, image.Height);
        }

        #endregion

        #region " DrawGradient "

        private LinearGradientBrush DrawGradientBrush;

        private Rectangle DrawGradientRectangle;
        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(blend, DrawGradientRectangle);
        }
        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(blend, DrawGradientRectangle, angle);
        }

        protected void DrawGradient(ColorBlend blend, Rectangle r)
        {
            DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, 90f);
            DrawGradientBrush.InterpolationColors = blend;
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawGradient(ColorBlend blend, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, angle);
            DrawGradientBrush.InterpolationColors = blend;
            G.FillRectangle(DrawGradientBrush, r);
        }


        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(c1, c2, DrawGradientRectangle);
        }
        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(c1, c2, DrawGradientRectangle, angle);
        }

        protected void DrawGradient(Color c1, Color c2, Rectangle r)
        {
            DrawGradientBrush = new LinearGradientBrush(r, c1, c2, 90f);
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawGradient(Color c1, Color c2, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, c1, c2, angle);
            G.FillRectangle(DrawGradientBrush, r);
        }

        #endregion

        #region " DrawRadial "

        private GraphicsPath DrawRadialPath;
        private PathGradientBrush DrawRadialBrush1;
        private LinearGradientBrush DrawRadialBrush2;

        private Rectangle DrawRadialRectangle;
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, width / 2, height / 2);
        }
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, Point center)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, center.X, center.Y);
        }
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, int cx, int cy)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, cx, cy);
        }

        public void DrawRadial(ColorBlend blend, Rectangle r)
        {
            DrawRadial(blend, r, r.Width / 2, r.Height / 2);
        }
        public void DrawRadial(ColorBlend blend, Rectangle r, Point center)
        {
            DrawRadial(blend, r, center.X, center.Y);
        }
        public void DrawRadial(ColorBlend blend, Rectangle r, int cx, int cy)
        {
            DrawRadialPath.Reset();
            DrawRadialPath.AddEllipse(r.X, r.Y, r.Width - 1, r.Height - 1);

            DrawRadialBrush1 = new PathGradientBrush(DrawRadialPath);
            DrawRadialBrush1.CenterPoint = new Point(r.X + cx, r.Y + cy);
            DrawRadialBrush1.InterpolationColors = blend;

            if (G.SmoothingMode == SmoothingMode.AntiAlias)
            {
                G.FillEllipse(DrawRadialBrush1, r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3);
            }
            else
            {
                G.FillEllipse(DrawRadialBrush1, r);
            }
        }


        protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(c1, c2, DrawGradientRectangle);
        }
        protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(c1, c2, DrawGradientRectangle, angle);
        }

        protected void DrawRadial(Color c1, Color c2, Rectangle r)
        {
            DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, 90f);
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawRadial(Color c1, Color c2, Rectangle r, float angle)
        {
            DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, angle);
            G.FillEllipse(DrawGradientBrush, r);
        }

        #endregion

        #region " CreateRound "

        private GraphicsPath CreateRoundPath;

        private Rectangle CreateRoundRectangle;
        public GraphicsPath CreateRound(int x, int y, int width, int height, int slope)
        {
            CreateRoundRectangle = new Rectangle(x, y, width, height);
            return CreateRound(CreateRoundRectangle, slope);
        }

        public GraphicsPath CreateRound(Rectangle r, int slope)
        {
            CreateRoundPath = new GraphicsPath(FillMode.Winding);
            CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180f, 90f);
            CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270f, 90f);
            CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0f, 90f);
            CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90f, 90f);
            CreateRoundPath.CloseFigure();
            return CreateRoundPath;
        }

        #endregion

    }
    abstract class ThemeControl154 : Control
    {


        #region " Initialization "

        protected Graphics G;

        protected Bitmap B;
        public ThemeControl154()
        {
            SetStyle((ControlStyles)139270, true);

            _ImageSize = Size.Empty;
            Font = new Font("Verdana", 8);

            MeasureBitmap = new Bitmap(1, 1);
            MeasureGraphics = Graphics.FromImage(MeasureBitmap);

            DrawRadialPath = new GraphicsPath();

            InvalidateCustimization();
            //Remove?
        }

        protected override sealed void OnHandleCreated(EventArgs e)
        {
            InvalidateCustimization();
            ColorHook();

            if (!(_LockWidth == 0))
                Width = _LockWidth;
            if (!(_LockHeight == 0))
                Height = _LockHeight;

            Transparent = _Transparent;
            if (_Transparent && _BackColor)
                BackColor = Color.Transparent;

            base.OnHandleCreated(e);
        }

        private bool DoneCreation;
        protected override sealed void OnParentChanged(EventArgs e)
        {
            if (Parent != null)
            {
                OnCreation();
                DoneCreation = true;
                InvalidateTimer();
            }

            base.OnParentChanged(e);
        }

        #endregion

        private void DoAnimation(bool i)
        {
            OnAnimation();
            if (i)
                Invalidate();
        }

        protected override sealed void OnPaint(PaintEventArgs e)
        {
            if (Width == 0 || Height == 0)
                return;

            if (_Transparent)
            {
                PaintHook();
                e.Graphics.DrawImage(B, 0, 0);
            }
            else
            {
                G = e.Graphics;
                PaintHook();
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            ThemeShare.RemoveAnimationCallback(DoAnimation);
            base.OnHandleDestroyed(e);
        }

        #region " Size Handling "

        protected override sealed void OnSizeChanged(EventArgs e)
        {
            if (_Transparent)
            {
                InvalidateBitmap();
            }

            Invalidate();
            base.OnSizeChanged(e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (!(_LockWidth == 0))
                width = _LockWidth;
            if (!(_LockHeight == 0))
                height = _LockHeight;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        #endregion

        #region " State Handling "

        private bool InPosition;
        protected override void OnMouseEnter(EventArgs e)
        {
            InPosition = true;
            SetState(MouseStateControl.Over);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (InPosition)
                SetState(MouseStateControl.Over);
            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                SetState(MouseStateControl.Down);
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            InPosition = false;
            SetState(MouseStateControl.None);
            base.OnMouseLeave(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled)
                SetState(MouseStateControl.None);
            else
                SetState(MouseStateControl.Block);
            base.OnEnabledChanged(e);
        }

        protected MouseStateControl State;
        private void SetState(MouseStateControl current)
        {
            State = current;
            Invalidate();
        }

        #endregion


        #region " Base Properties "

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor
        {
            get { return Color.Empty; }
            set { }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get { return null; }
            set { }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return ImageLayout.None; }
            set { }
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                Invalidate();
            }
        }

        private bool _BackColor;
        [Category("Misc")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                if (!IsHandleCreated && value == Color.Transparent)
                {
                    _BackColor = true;
                    return;
                }

                base.BackColor = value;
                if (Parent != null)
                    ColorHook();
            }
        }

        #endregion

        #region " Public Properties "

        private bool _NoRounding;
        public bool NoRounding
        {
            get { return _NoRounding; }
            set
            {
                _NoRounding = value;
                Invalidate();
            }
        }

        private Image _Image;
        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else
                {
                    _ImageSize = value.Size;
                }

                _Image = value;
                Invalidate();
            }
        }

        private bool _Transparent;
        public bool Transparent
        {
            get { return _Transparent; }
            set
            {
                _Transparent = value;
                if (!IsHandleCreated)
                    return;

                if (!value && !(BackColor.A == 255))
                {
                    throw new Exception("Unable to change value to false while a transparent BackColor is in use.");
                }

                SetStyle(ControlStyles.Opaque, !value);
                SetStyle(ControlStyles.SupportsTransparentBackColor, value);

                if (value)
                    InvalidateBitmap();
                else
                    B = null;
                Invalidate();
            }
        }

        private Dictionary<string, Color> Items = new Dictionary<string, Color>();
        public Bloom[] Colors
        {
            get
            {
                List<Bloom> T = new List<Bloom>();
                Dictionary<string, Color>.Enumerator E = Items.GetEnumerator();

                while (E.MoveNext())
                {
                    T.Add(new Bloom(E.Current.Key, E.Current.Value));
                }

                return T.ToArray();
            }
            set
            {
                foreach (Bloom B in value)
                {
                    if (Items.ContainsKey(B.Name))
                        Items[B.Name] = B.Value;
                }

                InvalidateCustimization();
                ColorHook();
                Invalidate();
            }
        }

        private string _Customization;
        public string Customization
        {
            get { return _Customization; }
            set
            {
                if (value == _Customization)
                    return;

                byte[] Data = null;
                Bloom[] Items = Colors;

                try
                {
                    Data = Convert.FromBase64String(value);
                    for (int I = 0; I <= Items.Length - 1; I++)
                    {
                        Items[I].Value = Color.FromArgb(BitConverter.ToInt32(Data, I * 4));
                    }
                }
                catch
                {
                    return;
                }

                _Customization = value;

                Colors = Items;
                ColorHook();
                Invalidate();
            }
        }

        #endregion

        #region " Private Properties "

        private Size _ImageSize;
        protected Size ImageSize
        {
            get { return _ImageSize; }
        }

        private int _LockWidth;
        protected int LockWidth
        {
            get { return _LockWidth; }
            set
            {
                _LockWidth = value;
                if (!(LockWidth == 0) && IsHandleCreated)
                    Width = LockWidth;
            }
        }

        private int _LockHeight;
        protected int LockHeight
        {
            get { return _LockHeight; }
            set
            {
                _LockHeight = value;
                if (!(LockHeight == 0) && IsHandleCreated)
                    Height = LockHeight;
            }
        }

        private bool _IsAnimated;
        protected bool IsAnimated
        {
            get { return _IsAnimated; }
            set
            {
                _IsAnimated = value;
                InvalidateTimer();
            }
        }

        #endregion


        #region " Property Helpers "

        protected Pen GetPen(string name)
        {
            return new Pen(Items[name]);
        }
        protected Pen GetPen(string name, float width)
        {
            return new Pen(Items[name], width);
        }

        protected SolidBrush GetBrush(string name)
        {
            return new SolidBrush(Items[name]);
        }

        protected Color GetColor(string name)
        {
            return Items[name];
        }

        protected void SetColor(string name, Color value)
        {
            if (Items.ContainsKey(name))
                Items[name] = value;
            else
                Items.Add(name, value);
        }
        protected void SetColor(string name, byte r, byte g, byte b)
        {
            SetColor(name, Color.FromArgb(r, g, b));
        }
        protected void SetColor(string name, byte a, byte r, byte g, byte b)
        {
            SetColor(name, Color.FromArgb(a, r, g, b));
        }
        protected void SetColor(string name, byte a, Color value)
        {
            SetColor(name, Color.FromArgb(a, value));
        }

        private void InvalidateBitmap()
        {
            if (Width == 0 || Height == 0)
                return;
            B = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            G = Graphics.FromImage(B);
        }

        private void InvalidateCustimization()
        {
            MemoryStream M = new MemoryStream(Items.Count * 4);

            foreach (Bloom B in Colors)
            {
                M.Write(BitConverter.GetBytes(B.Value.ToArgb()), 0, 4);
            }

            M.Close();
            _Customization = Convert.ToBase64String(M.ToArray());
        }

        private void InvalidateTimer()
        {
            if (DesignMode || !DoneCreation)
                return;

            if (_IsAnimated)
            {
                ThemeShare.AddAnimationCallback(DoAnimation);
            }
            else
            {
                ThemeShare.RemoveAnimationCallback(DoAnimation);
            }
        }
        #endregion


        #region " User Hooks "

        protected abstract void ColorHook();
        protected abstract void PaintHook();

        protected virtual void OnCreation()
        {
        }

        protected virtual void OnAnimation()
        {
        }

        #endregion


        #region " Offset "

        private Rectangle OffsetReturnRectangle;
        protected Rectangle Offset(Rectangle r, int amount)
        {
            OffsetReturnRectangle = new Rectangle(r.X + amount, r.Y + amount, r.Width - (amount * 2), r.Height - (amount * 2));
            return OffsetReturnRectangle;
        }

        private Size OffsetReturnSize;
        protected Size Offset(Size s, int amount)
        {
            OffsetReturnSize = new Size(s.Width + amount, s.Height + amount);
            return OffsetReturnSize;
        }

        private Point OffsetReturnPoint;
        protected Point Offset(Point p, int amount)
        {
            OffsetReturnPoint = new Point(p.X + amount, p.Y + amount);
            return OffsetReturnPoint;
        }

        #endregion

        #region " Center "


        private Point CenterReturn;
        protected Point Center(Rectangle p, Rectangle c)
        {
            CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X + c.X, (p.Height / 2 - c.Height / 2) + p.Y + c.Y);
            return CenterReturn;
        }
        protected Point Center(Rectangle p, Size c)
        {
            CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X, (p.Height / 2 - c.Height / 2) + p.Y);
            return CenterReturn;
        }

        protected Point Center(Rectangle child)
        {
            return Center(Width, Height, child.Width, child.Height);
        }
        protected Point Center(Size child)
        {
            return Center(Width, Height, child.Width, child.Height);
        }
        protected Point Center(int childWidth, int childHeight)
        {
            return Center(Width, Height, childWidth, childHeight);
        }

        protected Point Center(Size p, Size c)
        {
            return Center(p.Width, p.Height, c.Width, c.Height);
        }

        protected Point Center(int pWidth, int pHeight, int cWidth, int cHeight)
        {
            CenterReturn = new Point(pWidth / 2 - cWidth / 2, pHeight / 2 - cHeight / 2);
            return CenterReturn;
        }

        #endregion

        #region " Measure "

        private Bitmap MeasureBitmap;
        //TODO: Potential issues during multi-threading.
        private Graphics MeasureGraphics;

        protected Size Measure()
        {
            return MeasureGraphics.MeasureString(Text, Font, Width).ToSize();
        }
        protected Size Measure(string text)
        {
            return MeasureGraphics.MeasureString(text, Font, Width).ToSize();
        }

        #endregion


        #region " DrawPixel "


        private SolidBrush DrawPixelBrush;
        protected void DrawPixel(Color c1, int x, int y)
        {
            if (_Transparent)
            {
                B.SetPixel(x, y, c1);
            }
            else
            {
                DrawPixelBrush = new SolidBrush(c1);
                G.FillRectangle(DrawPixelBrush, x, y, 1, 1);
            }
        }

        #endregion

        #region " DrawCorners "


        private SolidBrush DrawCornersBrush;
        protected void DrawCorners(Color c1, int offset)
        {
            DrawCorners(c1, 0, 0, Width, Height, offset);
        }
        protected void DrawCorners(Color c1, Rectangle r1, int offset)
        {
            DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height, offset);
        }
        protected void DrawCorners(Color c1, int x, int y, int width, int height, int offset)
        {
            DrawCorners(c1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
        }

        protected void DrawCorners(Color c1)
        {
            DrawCorners(c1, 0, 0, Width, Height);
        }
        protected void DrawCorners(Color c1, Rectangle r1)
        {
            DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height);
        }
        protected void DrawCorners(Color c1, int x, int y, int width, int height)
        {
            if (_NoRounding)
                return;

            if (_Transparent)
            {
                B.SetPixel(x, y, c1);
                B.SetPixel(x + (width - 1), y, c1);
                B.SetPixel(x, y + (height - 1), c1);
                B.SetPixel(x + (width - 1), y + (height - 1), c1);
            }
            else
            {
                DrawCornersBrush = new SolidBrush(c1);
                G.FillRectangle(DrawCornersBrush, x, y, 1, 1);
                G.FillRectangle(DrawCornersBrush, x + (width - 1), y, 1, 1);
                G.FillRectangle(DrawCornersBrush, x, y + (height - 1), 1, 1);
                G.FillRectangle(DrawCornersBrush, x + (width - 1), y + (height - 1), 1, 1);
            }
        }

        #endregion

        #region " DrawBorders "

        protected void DrawBorders(Pen p1, int offset)
        {
            DrawBorders(p1, 0, 0, Width, Height, offset);
        }
        protected void DrawBorders(Pen p1, Rectangle r, int offset)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset);
        }
        protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset)
        {
            DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
        }

        protected void DrawBorders(Pen p1)
        {
            DrawBorders(p1, 0, 0, Width, Height);
        }
        protected void DrawBorders(Pen p1, Rectangle r)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height);
        }
        protected void DrawBorders(Pen p1, int x, int y, int width, int height)
        {
            G.DrawRectangle(p1, x, y, width - 1, height - 1);
        }

        #endregion

        #region " DrawText "

        private Point DrawTextPoint;

        private Size DrawTextSize;
        protected void DrawText(Brush b1, HorizontalAlignment a, int x, int y)
        {
            DrawText(b1, Text, a, x, y);
        }
        protected void DrawText(Brush b1, string text, HorizontalAlignment a, int x, int y)
        {
            if (text.Length == 0)
                return;

            DrawTextSize = Measure(text);
            DrawTextPoint = Center(DrawTextSize);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawString(text, Font, b1, x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawString(text, Font, b1, DrawTextPoint.X + x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawString(text, Font, b1, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y);
                    break;
            }
        }

        protected void DrawText(Brush b1, Point p1)
        {
            if (Text.Length == 0)
                return;
            G.DrawString(Text, Font, b1, p1);
        }
        protected void DrawText(Brush b1, int x, int y)
        {
            if (Text.Length == 0)
                return;
            G.DrawString(Text, Font, b1, x, y);
        }

        #endregion

        #region " DrawImage "


        private Point DrawImagePoint;
        protected void DrawImage(HorizontalAlignment a, int x, int y)
        {
            DrawImage(_Image, a, x, y);
        }
        protected void DrawImage(Image image, HorizontalAlignment a, int x, int y)
        {
            if (image == null)
                return;
            DrawImagePoint = Center(image.Size);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawImage(image, x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
            }
        }

        protected void DrawImage(Point p1)
        {
            DrawImage(_Image, p1.X, p1.Y);
        }
        protected void DrawImage(int x, int y)
        {
            DrawImage(_Image, x, y);
        }

        protected void DrawImage(Image image, Point p1)
        {
            DrawImage(image, p1.X, p1.Y);
        }
        protected void DrawImage(Image image, int x, int y)
        {
            if (image == null)
                return;
            G.DrawImage(image, x, y, image.Width, image.Height);
        }

        #endregion

        #region " DrawGradient "

        private LinearGradientBrush DrawGradientBrush;

        private Rectangle DrawGradientRectangle;
        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(blend, DrawGradientRectangle);
        }
        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(blend, DrawGradientRectangle, angle);
        }

        protected void DrawGradient(ColorBlend blend, Rectangle r)
        {
            DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, 90f);
            DrawGradientBrush.InterpolationColors = blend;
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawGradient(ColorBlend blend, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, angle);
            DrawGradientBrush.InterpolationColors = blend;
            G.FillRectangle(DrawGradientBrush, r);
        }


        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(c1, c2, DrawGradientRectangle);
        }
        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(c1, c2, DrawGradientRectangle, angle);
        }

        protected void DrawGradient(Color c1, Color c2, Rectangle r)
        {
            DrawGradientBrush = new LinearGradientBrush(r, c1, c2, 90f);
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawGradient(Color c1, Color c2, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, c1, c2, angle);
            G.FillRectangle(DrawGradientBrush, r);
        }

        #endregion

        #region " DrawRadial "

        private GraphicsPath DrawRadialPath;
        private PathGradientBrush DrawRadialBrush1;
        private LinearGradientBrush DrawRadialBrush2;

        private Rectangle DrawRadialRectangle;
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, width / 2, height / 2);
        }
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, Point center)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, center.X, center.Y);
        }
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, int cx, int cy)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, cx, cy);
        }

        public void DrawRadial(ColorBlend blend, Rectangle r)
        {
            DrawRadial(blend, r, r.Width / 2, r.Height / 2);
        }
        public void DrawRadial(ColorBlend blend, Rectangle r, Point center)
        {
            DrawRadial(blend, r, center.X, center.Y);
        }
        public void DrawRadial(ColorBlend blend, Rectangle r, int cx, int cy)
        {
            DrawRadialPath.Reset();
            DrawRadialPath.AddEllipse(r.X, r.Y, r.Width - 1, r.Height - 1);

            DrawRadialBrush1 = new PathGradientBrush(DrawRadialPath);
            DrawRadialBrush1.CenterPoint = new Point(r.X + cx, r.Y + cy);
            DrawRadialBrush1.InterpolationColors = blend;

            if (G.SmoothingMode == SmoothingMode.AntiAlias)
            {
                G.FillEllipse(DrawRadialBrush1, r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3);
            }
            else
            {
                G.FillEllipse(DrawRadialBrush1, r);
            }
        }


        protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(c1, c2, DrawRadialRectangle);
        }
        protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(c1, c2, DrawRadialRectangle, angle);
        }

        protected void DrawRadial(Color c1, Color c2, Rectangle r)
        {
            DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, 90f);
            G.FillEllipse(DrawRadialBrush2, r);
        }
        protected void DrawRadial(Color c1, Color c2, Rectangle r, float angle)
        {
            DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, angle);
            G.FillEllipse(DrawRadialBrush2, r);
        }

        #endregion

        #region " CreateRound "

        private GraphicsPath CreateRoundPath;

        private Rectangle CreateRoundRectangle;
        public GraphicsPath CreateRound(int x, int y, int width, int height, int slope)
        {
            CreateRoundRectangle = new Rectangle(x, y, width, height);
            return CreateRound(CreateRoundRectangle, slope);
        }

        public GraphicsPath CreateRound(Rectangle r, int slope)
        {
            CreateRoundPath = new GraphicsPath(FillMode.Winding);
            CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180f, 90f);
            CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270f, 90f);
            CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0f, 90f);
            CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90f, 90f);
            CreateRoundPath.CloseFigure();
            return CreateRoundPath;
        }

        #endregion

    }
    static class ThemeShare
    {

        #region " Animation "

        private static int Frames;
        private static bool Invalidate;

        public static PrecisionTimer ThemeTimer = new PrecisionTimer();
        //1000 / 50 = 20 FPS
        private const int FPS = 50;

        private const int Rate = 10;
        public delegate void AnimationDelegate(bool invalidate);


        private static List<AnimationDelegate> Callbacks = new List<AnimationDelegate>();
        private static void HandleCallbacks(IntPtr state, bool reserve)
        {
            Invalidate = (Frames >= FPS);
            if (Invalidate)
                Frames = 0;

            lock (Callbacks)
            {
                for (int I = 0; I <= Callbacks.Count - 1; I++)
                {
                    Callbacks[I].Invoke(Invalidate);
                }
            }

            Frames += Rate;
        }

        private static void InvalidateThemeTimer()
        {
            if (Callbacks.Count == 0)
            {
                ThemeTimer.Delete();
            }
            else
            {
                ThemeTimer.Create(0, Rate, HandleCallbacks);
            }
        }

        public static void AddAnimationCallback(AnimationDelegate callback)
        {
            lock (Callbacks)
            {
                if (Callbacks.Contains(callback))
                    return;

                Callbacks.Add(callback);
                InvalidateThemeTimer();
            }
        }

        public static void RemoveAnimationCallback(AnimationDelegate callback)
        {
            lock (Callbacks)
            {
                if (!Callbacks.Contains(callback))
                    return;

                Callbacks.Remove(callback);
                InvalidateThemeTimer();
            }
        }

        #endregion

    }
    enum MouseStateControl : byte
    {
        None = 0,
        Over = 1,
        Down = 2,
        Block = 3
    }
    struct Bloom
    {

        public string _Name;
        public string Name
        {
            get { return _Name; }
        }

        private Color _Value;
        public Color Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public string ValueHex
        {
            get { return string.Concat("#", _Value.R.ToString("X2", null), _Value.G.ToString("X2", null), _Value.B.ToString("X2", null)); }
            set
            {
                try
                {
                    _Value = ColorTranslator.FromHtml(value);
                }
                catch
                {
                }
            }
        }


        public Bloom(string name, Color value)
        {
            _Name = name;
            _Value = value;
        }
    }

    //------------------
    //Creator: aeonhack
    //Site: elitevs.net
    //Created: 11/30/2011
    //Changed: 11/30/2011
    //Version: 1.0.0
    //------------------
    class PrecisionTimer : IDisposable
    {

        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
        }

        private IntPtr _handle;

        private TimerDelegate _timerCallback;
        [DllImport("kernel32.dll", EntryPoint = "CreateTimerQueueTimer")]
        private static extern bool CreateTimerQueueTimer(ref IntPtr handle, IntPtr queue, TimerDelegate callback, IntPtr state, uint dueTime, uint period, uint flags);

        [DllImport("kernel32.dll", EntryPoint = "DeleteTimerQueueTimer")]
        private static extern bool DeleteTimerQueueTimer(IntPtr queue, IntPtr handle, IntPtr callback);

        public delegate void TimerDelegate(IntPtr r1, bool r2);

        public void Create(uint dueTime, uint period, TimerDelegate callback)
        {
            if (_enabled)
                return;

            _timerCallback = callback;
            bool success = CreateTimerQueueTimer(ref _handle, IntPtr.Zero, _timerCallback, IntPtr.Zero, dueTime, period, 0);

            if (!success)
                ThrowNewException("CreateTimerQueueTimer");
            _enabled = success;
        }

        public void Delete()
        {
            if (!_enabled)
                return;
            bool success = DeleteTimerQueueTimer(IntPtr.Zero, _handle, IntPtr.Zero);

            if (!success && Marshal.GetLastWin32Error() != 997)
            {
                ThrowNewException("DeleteTimerQueueTimer");
            }

            _enabled = !success;
        }

        private void ThrowNewException(string name)
        {
            throw new Exception(string.Format("{0} failed. Win32Error: {1}", name, Marshal.GetLastWin32Error()));
        }

        public void Dispose()
        {
            Delete();
        }
    }
}