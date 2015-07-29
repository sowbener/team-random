using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace TestGUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.ComponentModel;
    using System.Windows.Forms;


    class DotNetBarTabcontrol : TabControl
    {
        public DotNetBarTabcontrol()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(44, 136);
            Alignment = TabAlignment.Left;

            for (int i = 0; i <= TabPages.Count; i++)
                SelectedIndex = i;
            SelectedIndex = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            try
            {
                SelectedTab.BackColor = Color.FromArgb(35, 35, 35);
            }
            catch
            {
            }
            G.Clear(Color.FromArgb(40, 40, 40));
            G.FillRectangle(new SolidBrush(Color.FromArgb(35, 35, 35)), new Rectangle(0, 0, ItemSize.Height + 4, Height));
            G.DrawLine(new Pen(Color.FromArgb(40, 40, 40)), new Point(ItemSize.Height + 3, 0), new Point(ItemSize.Height + 3, 999));
            for (int i = 0; i <= TabCount - 1; i++)
            {
                if (i == SelectedIndex)
                {
                    Rectangle x2 = new Rectangle(new Point(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2), new Size(GetTabRect(i).Width + 3, GetTabRect(i).Height - 1));
                    ColorBlend myBlend = new ColorBlend();
                    myBlend.Colors = new Color[] { Color.FromArgb(35, 35, 35), Color.FromArgb(35, 35, 35), Color.FromArgb(35, 35, 35) };
                    myBlend.Positions = new float[] { 0f, 0.5f, 1f };
                    LinearGradientBrush lgBrush = new LinearGradientBrush(x2, Color.FromArgb(0, 132, 255), Color.FromArgb(0, 132, 255), 90f);
                    lgBrush.InterpolationColors = myBlend;
                    G.FillRectangle(lgBrush, x2);
                    G.DrawRectangle(new Pen(Color.FromArgb(35, 35, 35)), x2);

                    G.SmoothingMode = SmoothingMode.HighQuality;
                    Point[] p = {
        new Point(ItemSize.Height - 3, GetTabRect(i).Location.Y + 20),
        new Point(ItemSize.Height + 4, GetTabRect(i).Location.Y + 14),
        new Point(ItemSize.Height + 4, GetTabRect(i).Location.Y + 27)
                };
                    G.FillPolygon(Brushes.Black, p);
                    G.DrawPolygon(new Pen(Color.FromArgb(50, 50, 50)), p);

                    if (ImageList != null)
                    {
                        try
                        {

                            if (ImageList.Images[TabPages[i].ImageIndex] != null)
                            {
                                G.DrawImage(ImageList.Images[TabPages[i].ImageIndex], new Point(x2.Location.X + 8, x2.Location.Y + 6));
                                G.DrawString("  " + TabPages[i].Text, Font, Brushes.DeepSkyBlue, x2, new StringFormat
                                {
                                    LineAlignment = StringAlignment.Center,
                                    Alignment = StringAlignment.Center
                                });
                            }
                            else
                            {
                                G.DrawString(TabPages[i].Text, new Font(Font.FontFamily, Font.Size, FontStyle.Bold), Brushes.DeepSkyBlue, x2, new StringFormat
                                {
                                    LineAlignment = StringAlignment.Center,
                                    Alignment = StringAlignment.Center
                                });
                            }
                        }
                        catch (Exception)
                        {
                            G.DrawString(TabPages[i].Text, new Font(Font.FontFamily, Font.Size, FontStyle.Bold), Brushes.DeepSkyBlue, x2, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Center
                            });
                        }
                    }
                    else
                    {
                        G.DrawString(TabPages[i].Text, new Font(Font.FontFamily, Font.Size, FontStyle.Bold), Brushes.DeepSkyBlue, x2, new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        });
                    }

                    G.DrawLine(new Pen(Color.FromArgb(35, 35, 35)), new Point(x2.Location.X - 1, x2.Location.Y - 1), new Point(x2.Location.X, x2.Location.Y));
                    G.DrawLine(new Pen(Color.FromArgb(35, 35, 35)), new Point(x2.Location.X - 1, x2.Bottom - 1), new Point(x2.Location.X, x2.Bottom));
                }
                else
                {
                    Rectangle x2 = new Rectangle(new Point(GetTabRect(i).Location.X - 2, GetTabRect(i).Location.Y - 2), new Size(GetTabRect(i).Width + 3, GetTabRect(i).Height - 1));
                    G.FillRectangle(new SolidBrush(Color.FromArgb(50, 50, 50)), x2);
                    G.DrawLine(new Pen(Color.FromArgb(40, 40, 40)), new Point(x2.Right, x2.Top), new Point(x2.Right, x2.Bottom));
                    if (ImageList != null)
                    {
                        try
                        {
                            if (ImageList.Images[TabPages[i].ImageIndex] != null)
                            {
                                G.DrawImage(ImageList.Images[TabPages[i].ImageIndex], new Point(x2.Location.X + 8, x2.Location.Y + 6));
                                G.DrawString("  " + TabPages[i].Text, Font, Brushes.LightBlue, x2, new StringFormat
                                {
                                    LineAlignment = StringAlignment.Center,
                                    Alignment = StringAlignment.Center
                                });
                            }
                            else
                            {
                                G.DrawString(TabPages[i].Text, Font, Brushes.LightBlue, x2, new StringFormat
                                {
                                    LineAlignment = StringAlignment.Center,
                                    Alignment = StringAlignment.Center
                                });
                            }
                        }
                        catch (Exception)
                        {
                            G.DrawString(TabPages[i].Text, Font, Brushes.LightBlue, x2, new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Center
                            });
                        }
                    }
                    else
                    {
                        G.DrawString(TabPages[i].Text, Font, Brushes.LightBlue, x2, new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        });
                    }
                }
            }

            e.Graphics.DrawImage(B, new Point(0, 0));
            G.Dispose();
            B.Dispose();
        }

        int OldIndex;
        private int _Speed = 20;
        public int Speed
        {
            get { return _Speed; }
            set
            {
                if (value > 20 | value < -20)
                {
                    MessageBox.Show("Speed needs to be in between -20 and 20.");
                }
                else
                {
                    _Speed = value;
                }
            }
        }

        public void DoAnimationScrollLeft(TabPage FirstTabpage, TabPage SecondTabpage)
        {
            Graphics G = FirstTabpage.CreateGraphics();
            Bitmap P1 = new Bitmap(FirstTabpage.Width, FirstTabpage.Height);
            Bitmap P2 = new Bitmap(SecondTabpage.Width, SecondTabpage.Height);
            FirstTabpage.DrawToBitmap(P1, new Rectangle(0, 0, FirstTabpage.Width, FirstTabpage.Height));
            SecondTabpage.DrawToBitmap(P2, new Rectangle(0, 0, SecondTabpage.Width, SecondTabpage.Height));

            foreach (Control c in FirstTabpage.Controls)
                c.Hide();

            int Slide = FirstTabpage.Width - (FirstTabpage.Width % _Speed);

            int a = 0;
            for (a = 0; a <= Slide; a += _Speed)
            {
                G.DrawImage(P1, new Rectangle(a, 0, FirstTabpage.Width, FirstTabpage.Height));
                G.DrawImage(P2, new Rectangle(a - SecondTabpage.Width, 0, SecondTabpage.Width, SecondTabpage.Height));
            }
            a = FirstTabpage.Width;
            G.DrawImage(P1, new Rectangle(a, 0, FirstTabpage.Width, FirstTabpage.Height));
            G.DrawImage(P2, new Rectangle(a - SecondTabpage.Width, 0, SecondTabpage.Width, SecondTabpage.Height));

            SelectedTab = SecondTabpage;

            foreach (Control c in SecondTabpage.Controls)
                c.Show();

            foreach (Control c in FirstTabpage.Controls)
                c.Show();
        }

        protected override void OnSelecting(System.Windows.Forms.TabControlCancelEventArgs e)
        {
            if (OldIndex < e.TabPageIndex)
                DoAnimationScrollRight(TabPages[OldIndex], TabPages[e.TabPageIndex]);
            else
                DoAnimationScrollLeft(TabPages[OldIndex], TabPages[e.TabPageIndex]);
        }

        protected override void OnDeselecting(System.Windows.Forms.TabControlCancelEventArgs e)
        {
            OldIndex = e.TabPageIndex;
        }

        public void DoAnimationScrollRight(TabPage FirstTabpage, TabPage SecondTabpage)
        {
            Graphics G = FirstTabpage.CreateGraphics();
            Bitmap P1 = new Bitmap(FirstTabpage.Width, FirstTabpage.Height);
            Bitmap P2 = new Bitmap(SecondTabpage.Width, SecondTabpage.Height);
            FirstTabpage.DrawToBitmap(P1, new Rectangle(0, 0, FirstTabpage.Width, FirstTabpage.Height));
            SecondTabpage.DrawToBitmap(P2, new Rectangle(0, 0, SecondTabpage.Width, SecondTabpage.Height));

            foreach (Control c in FirstTabpage.Controls)
                c.Hide();

            int Slide = FirstTabpage.Width - (FirstTabpage.Width % _Speed);

            int a = 0;
            for (a = 0; a >= -Slide; a += -_Speed)
            {
                G.DrawImage(P1, new Rectangle(a, 0, FirstTabpage.Width, FirstTabpage.Height));
                G.DrawImage(P2, new Rectangle(a + SecondTabpage.Width, 0, SecondTabpage.Width, SecondTabpage.Height));
            }
            a = FirstTabpage.Width;
            G.DrawImage(P1, new Rectangle(a, 0, FirstTabpage.Width, FirstTabpage.Height));
            G.DrawImage(P2, new Rectangle(a + SecondTabpage.Width, 0, SecondTabpage.Width, SecondTabpage.Height));

            SelectedTab = SecondTabpage;

            foreach (Control c in SecondTabpage.Controls)
                c.Show();

            foreach (Control c in FirstTabpage.Controls)
                c.Show();
        }
    }

    public class ToggleButton2 : System.Windows.Forms.CheckBox
    {

        public ToggleButton2()
            : base()
        {
            // imageList is static to save space.
            if (imageList == null)
            {
                System.Windows.Forms.CheckBox chkTemp = new System.Windows.Forms.CheckBox(); // Temporary checkbox with no text.

                chkTemp.AutoSize = true;
                chkTemp.BackColor = System.Drawing.Color.Transparent;
                chkTemp.Size = new System.Drawing.Size(16, 16);
                chkTemp.UseVisualStyleBackColor = false;

                imageList = new System.Windows.Forms.ImageList();
                imageList.TransparentColor = System.Drawing.Color.Transparent;

                System.Drawing.Bitmap[] bm = 
         {
            new System.Drawing.Bitmap(chkTemp.Width, chkTemp.Height),
            new System.Drawing.Bitmap(chkTemp.Width, chkTemp.Height)
         };

                // Set checkbox false and capture bitmap.
                chkTemp.Checked = false;
                chkTemp.DrawToBitmap(bm[0], new System.Drawing.Rectangle(0, 0, chkTemp.Width, chkTemp.Height));
                imageList.Images.Add(bm[0]);

                // Set checkbox true and capture bitmap.
                chkTemp.Checked = true;
                chkTemp.DrawToBitmap(bm[1], new System.Drawing.Rectangle(0, 0, chkTemp.Width, chkTemp.Height));
                imageList.Images.Add(bm[1]);
            }

            this.ImageList = imageList;
            this.CheckedChanged += new System.EventHandler(this.btnToggleButton_CheckedChanged);

            btnToggleButton_CheckedChanged(null, null);

        }

        // Prevents Image property from deleting the ImageList.
        new public Image Image { get { return null; } set { ; } }

        // Change this.Image
        private void btnToggleButton_CheckedChanged(object sender, System.EventArgs e)
        {
            this.ImageIndex = this.Checked ? 1 : 0;
        }

        static private ImageList imageList;
    }

    class FlowTheme : ThemeContainer152
    {

        public FlowTheme()
        {
            MoveHeight = 24;
            BackColor = Color.FromArgb(35, 35, 35);
            TransparencyKey = Color.Fuchsia;

            SetColor("Sides", 40, 40, 40);
            SetColor("Gradient", 18, 18, 18);
            SetColor("Text", Color.DeepSkyBlue);
            SetColor("Border1", 40, 40, 40);
            SetColor("Border2", 22, 22, 22);
            SetColor("Border3", 65, 65, 65);
            SetColor("Border4", Color.Black);
            SetColor("Hatch1", 39, 39, 39);
            SetColor("Hatch2", 35, 35, 35);
            SetColor("Hatch3", 29, 29, 29);
            SetColor("Hatch4", 26, 26, 26);
            SetColor("Shade1", 50, 7, 7, 7);
            SetColor("Shade2", Color.Transparent);
        }

        private Color C1;
        private Color C2;
        private SolidBrush B1;
        private Pen P1;
        private Pen P2;
        private Pen P3;

        private Pen P4;
        protected override void ColorHook()
        {
            C1 = GetColor("Sides");
            C2 = GetColor("Gradient");

            B1 = new SolidBrush(GetColor("Text"));

            P1 = new Pen(GetColor("Border1"));
            P2 = new Pen(GetColor("Border2"));
            P3 = new Pen(GetColor("Border3"));
            P4 = new Pen(GetColor("Border4"));

            CreateTile();
            CreateShade();

            BackColor = GetColor("Hatch2");
        }


        private Rectangle RT1;
        protected override void PaintHook()
        {
            G.Clear(C1);

            DrawGradient(C2, C1, 0, 0, Width, 24);
            DrawText(B1, HorizontalAlignment.Left, 8, 0);

            RT1 = new Rectangle(8, 24, Width - 16, Height - 32);
            G.FillRectangle(Tile, RT1);

            for (int I = 0; I <= Shade.Length - 1; I++)
            {
                DrawBorders(Shade[I], RT1, I);
            }

            RT1 = new Rectangle(8, 24, Width - 16, Height - 32);
            DrawBorders(P1, RT1, 1);
            DrawBorders(P2, RT1);
            DrawCorners(C1, RT1);

            DrawBorders(P3, 1);
            DrawBorders(P4);

            DrawCorners(TransparencyKey);
        }


        private TextureBrush Tile;
        private byte[] TileData = Convert.FromBase64String("AgIBAQEBAwMBAQEBAAABAQEBAQEBAgIBAQEBAwMBAQEBAAAB");
        private void CreateTile()
        {
            Bitmap TileImage = new Bitmap(6, 6);
            Color[] TileColors = new Color[] {
			GetColor("Hatch1"),
			GetColor("Hatch2"),
			GetColor("Hatch3"),
			GetColor("Hatch4")
		};

            for (int I = 0; I <= 35; I++)
            {
                TileImage.SetPixel(I % 6, I / 6, TileColors[TileData[I]]);
            }

            Tile = new TextureBrush(TileImage);
            TileImage.Dispose();
        }

        private Pen[] Shade;
        private void CreateShade()
        {
            if (Shade != null)
            {
                for (int I = 0; I <= Shade.Length - 1; I++)
                {
                    Shade[I].Dispose();
                }
            }

            Bitmap ShadeImage = new Bitmap(1, 20);
            Graphics ShadeGraphics = Graphics.FromImage(ShadeImage);

            Rectangle ShadeBounds = new Rectangle(0, 0, 1, 20);
            LinearGradientBrush Gradient = new LinearGradientBrush(ShadeBounds, GetColor("Shade1"), GetColor("Shade2"), 90f);

            Shade = new Pen[20];
            ShadeGraphics.FillRectangle(Gradient, ShadeBounds);

            for (int I = 0; I <= Shade.Length - 1; I++)
            {
                Shade[I] = new Pen(ShadeImage.GetPixel(0, I));
            }

            Gradient.Dispose();
            ShadeGraphics.Dispose();
            ShadeImage.Dispose();
        }

    }

    class FlowButton : ThemeControl152
    {

        public FlowButton()
        {
            SetColor("DownGradient1", 24, 24, 24);
            SetColor("DownGradient2", 38, 38, 38);
            SetColor("NoneGradient1", 38, 38, 38);
            SetColor("NoneGradient2", 24, 24, 24);
            SetColor("Text", Color.DeepSkyBlue);
            SetColor("Border1", 22, 22, 22);
            SetColor("Border2A", 60, 60, 60);
            SetColor("Border2B", 36, 36, 36);
        }

        private Color C1;
        private Color C2;
        private Color C3;
        private Color C4;
        private Color C5;
        private Color C6;
        private SolidBrush B1;
        private LinearGradientBrush B2;
        private Pen P1;

        private Pen P2;
        protected override void ColorHook()
        {
            C1 = GetColor("DownGradient1");
            C2 = GetColor("DownGradient2");
            C3 = GetColor("NoneGradient1");
            C4 = GetColor("NoneGradient2");
            C5 = GetColor("Border2A");
            C6 = GetColor("Border2B");

            B1 = new SolidBrush(GetColor("Text"));

            P1 = new Pen(GetColor("Border1"));
        }

        protected override void PaintHook()
        {
            if (State == MouseState.Down)
            {
                DrawGradient(C1, C2, ClientRectangle, 90f);
            }
            else
            {
                DrawGradient(C3, C4, ClientRectangle, 90f);
            }

            DrawText(B1, HorizontalAlignment.Center, 0, 0);

            B2 = new LinearGradientBrush(ClientRectangle, C5, C6, 90f);
            P2 = new Pen(B2);

            DrawBorders(P1);
            DrawBorders(P2, 1);
        }
    }
    class FusionTheme : ThemeContainer152
    {

        private GraphicsPath Path;

        private ColorBlend Blend;
        public FusionTheme()
        {
            MoveHeight = 34;
            BackColor = Color.White;
            TransparencyKey = Color.Fuchsia;

            SetColor("Sides", 47, 47, 50);
            SetColor("Gradient1", 52, 52, 55);
            SetColor("Gradient2", 47, 47, 50);
            SetColor("Text", Color.White);
            SetColor("Back", Color.White);
            SetColor("Border1", Color.Black);
            SetColor("Border2", 60, 60, 63);
            SetColor("Border3", 60, 60, 63);
            SetColor("Border4", Color.Black);
            SetColor("Blend1", Color.Transparent);
            SetColor("Blend2", 60, 60, 63);

            Path = new GraphicsPath();

            Blend = new ColorBlend();
            Blend.Positions = new float[] {
			0f,
			0.5f,
			1f
		};
        }

        private Pen P1;
        private Pen P2;
        private Pen P3;
        private Pen P4;
        private Pen P5;
        private Color C1;
        private Color C2;
        private Color C3;
        private SolidBrush B1;

        private SolidBrush B2;
        protected override void ColorHook()
        {
            P1 = new Pen(TransparencyKey, 3);
            P2 = new Pen(GetColor("Border1"));
            P3 = new Pen(GetColor("Border2"));
            P4 = new Pen(GetColor("Border3"));
            P5 = new Pen(GetColor("Border4"));

            C1 = GetColor("Sides");
            C2 = GetColor("Gradient1");
            C3 = GetColor("Gradient2");

            B1 = new SolidBrush(GetColor("Text"));
            B2 = new SolidBrush(GetColor("Back"));

            Blend.Colors = new Color[] {
			GetColor("Blend1"),
			GetColor("Blend2"),
			GetColor("Blend1")
		};

            BackColor = B2.Color;
        }


        private Rectangle RT1;
        protected override void PaintHook()
        {
            G.DrawRectangle(P1, ClientRectangle);
            G.SetClip(Path);

            G.Clear(C1);

            DrawGradient(C2, C3, 0, 0, 16, Height);
            DrawGradient(C2, C3, Width - 16, 0, 16, Height);

            DrawText(B1, HorizontalAlignment.Left, 12, 0);

            RT1 = new Rectangle(12, 34, Width - 24, Height - 34 - 12);

            G.FillRectangle(B2, RT1);
            DrawBorders(P2, RT1, 1);
            DrawBorders(P3, RT1);

            DrawBorders(P4, 1);
            DrawGradient(Blend, 1, 1, Width - 2, 2, 0f);

            G.ResetClip();
            G.DrawPath(P5, Path);
        }

        protected override void OnResize(EventArgs e)
        {
            Path.Reset();
            Path.AddLines(new Point[] {
			new Point(2, 0),
			new Point(Width - 3, 0),
			new Point(Width - 1, 2),
			new Point(Width - 1, Height - 3),
			new Point(Width - 3, Height - 1),
			new Point(2, Height - 1),
			new Point(0, Height - 3),
			new Point(0, 2),
			new Point(2, 0)
		});

            base.OnResize(e);
        }

    }

    class FusionButton : ThemeControl152
    {

        public FusionButton()
        {
            SetColor("DownGradient1", 255, 127, 1);
            SetColor("DownGradient2", 255, 175, 12);
            SetColor("NoneGradient1", 255, 175, 12);
            SetColor("NoneGradient2", 255, 127, 1);
            SetColor("TextShade", 30, Color.Black);
            SetColor("Text", Color.White);
            SetColor("Border1", 255, 197, 19);
            SetColor("Border2", 254, 133, 0);
        }

        private Color C1;
        private Color C2;
        private Color C3;
        private Color C4;
        private SolidBrush B1;
        private SolidBrush B2;
        private Pen P1;

        private Pen P2;
        protected override void ColorHook()
        {
            C1 = GetColor("DownGradient1");
            C2 = GetColor("DownGradient2");
            C3 = GetColor("NoneGradient1");
            C4 = GetColor("NoneGradient2");

            B1 = new SolidBrush(GetColor("TextShade"));
            B2 = new SolidBrush(GetColor("Text"));

            P1 = new Pen(GetColor("Border1"));
            P2 = new Pen(GetColor("Border2"));
        }

        protected override void PaintHook()
        {
            if (State == MouseState.Down)
            {
                DrawGradient(C1, C2, ClientRectangle, 90f);
            }
            else
            {
                DrawGradient(C3, C4, ClientRectangle, 90f);
            }

            DrawText(B1, HorizontalAlignment.Center, 1, 1);
            DrawText(B2, HorizontalAlignment.Center, 0, 0);

            DrawBorders(P1, 1);
            DrawBorders(P2);

            DrawCorners(BackColor);
        }

    }
    abstract class ThemeContainer152 : ContainerControl
    {


        protected Graphics G;
        public ThemeContainer152()
        {
            SetStyle((ControlStyles)139270, true);
            _ImageSize = Size.Empty;

            MeasureBitmap = new Bitmap(1, 1);
            MeasureGraphics = Graphics.FromImage(MeasureBitmap);

            Font = new Font("Verdana", 8);

            InvalidateCustimization();
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (!(_LockWidth == 0))
                width = _LockWidth;
            if (!(_LockHeight == 0))
                height = _LockHeight;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        private Rectangle Header;
        protected override sealed void OnSizeChanged(EventArgs e)
        {
            if (_Movable && !_ControlMode)
                Header = new Rectangle(7, 7, Width - 14, _MoveHeight - 7);
            Invalidate();
            base.OnSizeChanged(e);
        }

        protected override sealed void OnPaint(PaintEventArgs e)
        {
            if (Width == 0 || Height == 0)
                return;
            G = e.Graphics;
            PaintHook();
        }

        protected override sealed void OnHandleCreated(EventArgs e)
        {
            InvalidateCustimization();
            ColorHook();

            if (!(_LockWidth == 0))
                Width = _LockWidth;
            if (!(_LockHeight == 0))
                Height = _LockHeight;
            if (!_ControlMode)
                base.Dock = DockStyle.Fill;

            base.OnHandleCreated(e);
        }

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
                    ParentForm.FormBorderStyle = _BorderStyle;
                    ParentForm.TransparencyKey = _TransparencyKey;
                }

                Parent.BackColor = BackColor;
            }

            OnCreation();
        }

        protected virtual void OnCreation()
        {
        }

        #region " Sizing and Movement "

        protected MouseState State;
        private void SetState(MouseState current)
        {
            State = current;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized))
            {
                if (_Sizable && !_ControlMode)
                    InvalidateMouse();
            }

            base.OnMouseMove(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled)
                SetState(MouseState.None);
            else
                SetState(MouseState.Block);
            base.OnEnabledChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            SetState(MouseState.Over);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            SetState(MouseState.Over);
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            SetState(MouseState.None);

            if (GetChildAtPoint(PointToClient(MousePosition)) != null)
            {
                if (_Sizable && !_ControlMode)
                {
                    Cursor = Cursors.Default;
                    Previous = 0;
                }
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                SetState(MouseState.Down);

            if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized || _ControlMode))
            {
                if (_Movable && Header.Contains(e.Location))
                {
                    Capture = false;
                    WM_LMBUTTONDOWN = true;
                    DefWndProc(ref Messages[0]);
                }
                else if (_Sizable && !(Previous == 0))
                {
                    Capture = false;
                    WM_LMBUTTONDOWN = true;
                    DefWndProc(ref Messages[Previous]);
                }
            }

            base.OnMouseDown(e);
        }

        private bool WM_LMBUTTONDOWN;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (WM_LMBUTTONDOWN && m.Msg == 513)
            {
                WM_LMBUTTONDOWN = false;

                SetState(MouseState.Over);
                if (!_SmartBounds)
                    return;

                if (IsParentMdi)
                {
                    CorrectBounds(new Rectangle(Point.Empty, Parent.Parent.Size));
                }
                else
                {
                    CorrectBounds(Screen.FromControl(Parent).WorkingArea);
                }
            }
        }

        private Point GetIndexPoint;
        private bool B1;
        private bool B2;
        private bool B3;
        private bool B4;
        private int GetIndex()
        {
            GetIndexPoint = PointToClient(MousePosition);
            B1 = GetIndexPoint.X < 7;
            B2 = GetIndexPoint.X > Width - 7;
            B3 = GetIndexPoint.Y < 7;
            B4 = GetIndexPoint.Y > Height - 7;

            if (B1 && B3)
                return 4;
            if (B1 && B4)
                return 7;
            if (B2 && B3)
                return 5;
            if (B2 && B4)
                return 8;
            if (B1)
                return 1;
            if (B2)
                return 2;
            if (B3)
                return 3;
            if (B4)
                return 6;
            return 0;
        }

        private int Current;
        private int Previous;
        private void InvalidateMouse()
        {
            Current = GetIndex();
            if (Current == Previous)
                return;

            Previous = Current;
            switch (Previous)
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

        private Message[] Messages = new Message[9];
        private void InitializeMessages()
        {
            Messages[0] = Message.Create(Parent.Handle, 161, new IntPtr(2), IntPtr.Zero);
            for (int I = 1; I <= 8; I++)
            {
                Messages[I] = Message.Create(Parent.Handle, 161, new IntPtr(I + 9), IntPtr.Zero);
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


        #region " Property Overrides "

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

        [Category("Misc")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                if (value == BackColor)
                    return;
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

        public override Font Font
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

        #region " Properties "

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

        private int _MoveHeight = 24;
        protected int MoveHeight
        {
            get { return _MoveHeight; }
            set
            {
                if (value < 8)
                    return;
                Header = new Rectangle(7, 7, Width - 14, value - 7);
                _MoveHeight = value;
                Invalidate();
            }
        }

        private bool _ControlMode;
        protected bool ControlMode
        {
            get { return _ControlMode; }
            set { _ControlMode = value; }
        }

        private Dictionary<string, Color> Items = new Dictionary<string, Color>();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

        #region " Property Helpers "

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

        #endregion


        #region " User Hooks "

        protected abstract void ColorHook();
        protected abstract void PaintHook();

        #endregion


        #region " Center Overloads "


        private Point CenterReturn;
        protected Point Center(Rectangle r1, Size s1)
        {
            CenterReturn = new Point((r1.Width / 2 - s1.Width / 2) + r1.X, (r1.Height / 2 - s1.Height / 2) + r1.Y);
            return CenterReturn;
        }
        protected Point Center(Rectangle r1, Rectangle r2)
        {
            return Center(r1, r2.Size);
        }

        protected Point Center(int w1, int h1, int w2, int h2)
        {
            CenterReturn = new Point(w1 / 2 - w2 / 2, h1 / 2 - h2 / 2);
            return CenterReturn;
        }

        protected Point Center(Size s1, Size s2)
        {
            return Center(s1.Width, s1.Height, s2.Width, s2.Height);
        }

        protected Point Center(Rectangle r1)
        {
            return Center(ClientRectangle.Width, ClientRectangle.Height, r1.Width, r1.Height);
        }
        protected Point Center(Size s1)
        {
            return Center(Width, Height, s1.Width, s1.Height);
        }
        protected Point Center(int w1, int h1)
        {
            return Center(Width, Height, w1, h1);
        }

        #endregion

        #region " Measure Overloads "

        private Bitmap MeasureBitmap;

        private Graphics MeasureGraphics;
        protected Size Measure(string text)
        {
            return MeasureGraphics.MeasureString(text, Font, Width).ToSize();
        }
        protected Size Measure()
        {
            return MeasureGraphics.MeasureString(Text, Font).ToSize();
        }

        #endregion

        #region " DrawCorners Overloads "


        private SolidBrush DrawCornersBrush;
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
            DrawCornersBrush = new SolidBrush(c1);
            G.FillRectangle(DrawCornersBrush, x, y, 1, 1);
            G.FillRectangle(DrawCornersBrush, x + (width - 1), y, 1, 1);
            G.FillRectangle(DrawCornersBrush, x, y + (height - 1), 1, 1);
            G.FillRectangle(DrawCornersBrush, x + (width - 1), y + (height - 1), 1, 1);
        }

        #endregion

        #region " DrawBorders Overloads "

        //TODO: Remove triple overload?

        protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset)
        {
            DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
        }
        protected void DrawBorders(Pen p1, int offset)
        {
            DrawBorders(p1, 0, 0, Width, Height, offset);
        }
        protected void DrawBorders(Pen p1, Rectangle r, int offset)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset);
        }

        protected void DrawBorders(Pen p1, int x, int y, int width, int height)
        {
            G.DrawRectangle(p1, x, y, width - 1, height - 1);
        }
        protected void DrawBorders(Pen p1)
        {
            DrawBorders(p1, 0, 0, Width, Height);
        }
        protected void DrawBorders(Pen p1, Rectangle r)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height);
        }

        #endregion

        #region " DrawText Overloads "

        //TODO: Remove triple overloads?

        private Point DrawTextPoint;

        private Size DrawTextSize;
        protected void DrawText(Brush b1, HorizontalAlignment a, int x, int y)
        {
            DrawText(b1, Text, a, x, y);
        }
        protected void DrawText(Brush b1, Point p1)
        {
            DrawText(b1, Text, p1.X, p1.Y);
        }
        protected void DrawText(Brush b1, int x, int y)
        {
            DrawText(b1, Text, x, y);
        }

        protected void DrawText(Brush b1, string text, HorizontalAlignment a, int x, int y)
        {
            if (text.Length == 0)
                return;
            DrawTextSize = Measure(text);

            if (_ControlMode)
            {
                DrawTextPoint = Center(DrawTextSize);
            }
            else
            {
                DrawTextPoint = new Point(Width / 2 - DrawTextSize.Width / 2, MoveHeight / 2 - DrawTextSize.Height / 2);
            }

            switch (a)
            {
                case HorizontalAlignment.Left:
                    DrawText(b1, text, x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Center:
                    DrawText(b1, text, DrawTextPoint.X + x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Right:
                    DrawText(b1, text, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y);
                    break;
            }
        }
        protected void DrawText(Brush b1, string text, Point p1)
        {
            DrawText(b1, text, p1.X, p1.Y);
        }
        protected void DrawText(Brush b1, string text, int x, int y)
        {
            if (text.Length == 0)
                return;
            G.DrawString(text, Font, b1, x, y);
        }

        #endregion

        #region " DrawImage Overloads "

        //TODO: Remove triple overloads?


        private Point DrawImagePoint;
        protected void DrawImage(HorizontalAlignment a, int x, int y)
        {
            DrawImage(_Image, a, x, y);
        }
        protected void DrawImage(Point p1)
        {
            DrawImage(_Image, p1.X, p1.Y);
        }
        protected void DrawImage(int x, int y)
        {
            DrawImage(_Image, x, y);
        }

        protected void DrawImage(Image image, HorizontalAlignment a, int x, int y)
        {
            if (image == null)
                return;

            if (_ControlMode)
            {
                DrawImagePoint = Center(image.Size);
            }
            else
            {
                DrawImagePoint = new Point(Width / 2 - image.Width / 2, MoveHeight / 2 - image.Height / 2);
            }

            switch (a)
            {
                case HorizontalAlignment.Left:
                    DrawImage(image, x, DrawImagePoint.Y + y);
                    break;
                case HorizontalAlignment.Center:
                    DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y);
                    break;
                case HorizontalAlignment.Right:
                    DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y);
                    break;
            }
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

        #region " DrawGradient Overloads "

        //TODO: Remove triple overload?

        private LinearGradientBrush DrawGradientBrush;

        private Rectangle DrawGradientRectangle;
        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height)
        {
            DrawGradient(blend, x, y, width, height, 90);
        }
        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height)
        {
            DrawGradient(c1, c2, x, y, width, height, 90);
        }

        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(blend, DrawGradientRectangle, angle);
        }
        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(c1, c2, DrawGradientRectangle, angle);
        }

        protected void DrawGradient(ColorBlend blend, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, angle);
            DrawGradientBrush.InterpolationColors = blend;
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawGradient(Color c1, Color c2, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, c1, c2, angle);
            G.FillRectangle(DrawGradientBrush, r);
        }

        #endregion

    }

    abstract class ThemeControl152 : Control
    {

        protected Graphics G;

        protected Bitmap B;
        public ThemeControl152()
        {
            SetStyle((ControlStyles)139270, true);

            _ImageSize = Size.Empty;

            MeasureBitmap = new Bitmap(1, 1);
            MeasureGraphics = Graphics.FromImage(MeasureBitmap);

            Font = new Font("Verdana", 8);

            InvalidateCustimization();
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (!(_LockWidth == 0))
                width = _LockWidth;
            if (!(_LockHeight == 0))
                height = _LockHeight;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override sealed void OnSizeChanged(EventArgs e)
        {
            if (_Transparent && !(Width == 0 || Height == 0))
            {
                B = new Bitmap(Width, Height);
                G = Graphics.FromImage(B);
            }

            Invalidate();
            base.OnSizeChanged(e);
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

        protected override sealed void OnHandleCreated(EventArgs e)
        {
            InvalidateCustimization();
            ColorHook();

            if (!(_LockWidth == 0))
                Width = _LockWidth;
            if (!(_LockHeight == 0))
                Height = _LockHeight;

            Transparent = _Transparent;
            if (_BackColorU && _Transparent)
                BackColor = Color.Transparent;

            base.OnHandleCreated(e);
        }

        protected override sealed void OnParentChanged(EventArgs e)
        {
            if (Parent != null)
                OnCreation();
            base.OnParentChanged(e);
        }

        protected virtual void OnCreation()
        {
        }

        #region " State Handling "

        private bool InPosition;
        protected override void OnMouseEnter(EventArgs e)
        {
            InPosition = true;
            SetState(MouseState.Over);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (InPosition)
                SetState(MouseState.Over);
            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                SetState(MouseState.Down);
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            InPosition = false;
            SetState(MouseState.None);
            base.OnMouseLeave(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled)
                SetState(MouseState.None);
            else
                SetState(MouseState.Block);
            base.OnEnabledChanged(e);
        }

        protected MouseState State;
        private void SetState(MouseState current)
        {
            State = current;
            Invalidate();
        }

        #endregion


        #region " Property Overrides "

        private bool _BackColorU;
        [Category("Misc")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                if (!IsHandleCreated && value == Color.Transparent)
                {
                    _BackColorU = true;
                    return;
                }

                base.BackColor = value;
                if (Parent != null)
                    ColorHook();
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

        #endregion

        #region " Properties "

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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

        #region " Property Helpers "

        private void InvalidateBitmap()
        {
            if (Width == 0 || Height == 0)
                return;
            B = new Bitmap(Width, Height);
            G = Graphics.FromImage(B);
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

        #endregion


        #region " User Hooks "

        protected abstract void ColorHook();
        protected abstract void PaintHook();

        #endregion


        #region " Center Overloads "


        private Point CenterReturn;
        protected Point Center(Rectangle r1, Size s1)
        {
            CenterReturn = new Point((r1.Width / 2 - s1.Width / 2) + r1.X, (r1.Height / 2 - s1.Height / 2) + r1.Y);
            return CenterReturn;
        }
        protected Point Center(Rectangle r1, Rectangle r2)
        {
            return Center(r1, r2.Size);
        }

        protected Point Center(int w1, int h1, int w2, int h2)
        {
            CenterReturn = new Point(w1 / 2 - w2 / 2, h1 / 2 - h2 / 2);
            return CenterReturn;
        }

        protected Point Center(Size s1, Size s2)
        {
            return Center(s1.Width, s1.Height, s2.Width, s2.Height);
        }

        protected Point Center(Rectangle r1)
        {
            return Center(ClientRectangle.Width, ClientRectangle.Height, r1.Width, r1.Height);
        }
        protected Point Center(Size s1)
        {
            return Center(Width, Height, s1.Width, s1.Height);
        }
        protected Point Center(int w1, int h1)
        {
            return Center(Width, Height, w1, h1);
        }

        #endregion

        #region " Measure Overloads "

        private Bitmap MeasureBitmap;

        private Graphics MeasureGraphics;
        protected Size Measure(string text)
        {
            return MeasureGraphics.MeasureString(text, Font, Width).ToSize();
        }
        protected Size Measure()
        {
            return MeasureGraphics.MeasureString(Text, Font, Width).ToSize();
        }

        #endregion

        #region " DrawCorners Overloads "


        private SolidBrush DrawCornersBrush;
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

        #region " DrawBorders Overloads "

        //TODO: Remove triple overload?

        protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset)
        {
            DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
        }
        protected void DrawBorders(Pen p1, int offset)
        {
            DrawBorders(p1, 0, 0, Width, Height, offset);
        }
        protected void DrawBorders(Pen p1, Rectangle r, int offset)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset);
        }

        protected void DrawBorders(Pen p1, int x, int y, int width, int height)
        {
            G.DrawRectangle(p1, x, y, width - 1, height - 1);
        }
        protected void DrawBorders(Pen p1)
        {
            DrawBorders(p1, 0, 0, Width, Height);
        }
        protected void DrawBorders(Pen p1, Rectangle r)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height);
        }

        #endregion

        #region " DrawText Overloads "

        //TODO: Remove triple overloads?

        private Point DrawTextPoint;

        private Size DrawTextSize;
        protected void DrawText(Brush b1, HorizontalAlignment a, int x, int y)
        {
            DrawText(b1, Text, a, x, y);
        }
        protected void DrawText(Brush b1, Point p1)
        {
            DrawText(b1, Text, p1.X, p1.Y);
        }
        protected void DrawText(Brush b1, int x, int y)
        {
            DrawText(b1, Text, x, y);
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
                    DrawText(b1, text, x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Center:
                    DrawText(b1, text, DrawTextPoint.X + x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Right:
                    DrawText(b1, text, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y);
                    break;
            }
        }
        protected void DrawText(Brush b1, string text, Point p1)
        {
            DrawText(b1, text, p1.X, p1.Y);
        }
        protected void DrawText(Brush b1, string text, int x, int y)
        {
            if (text.Length == 0)
                return;
            G.DrawString(text, Font, b1, x, y);
        }

        #endregion

        #region " DrawImage Overloads "

        //TODO: Remove triple overloads?


        private Point DrawImagePoint;
        protected void DrawImage(HorizontalAlignment a, int x, int y)
        {
            DrawImage(_Image, a, x, y);
        }
        protected void DrawImage(Point p1)
        {
            DrawImage(_Image, p1.X, p1.Y);
        }
        protected void DrawImage(int x, int y)
        {
            DrawImage(_Image, x, y);
        }

        protected void DrawImage(Image image, HorizontalAlignment a, int x, int y)
        {
            if (image == null)
                return;
            DrawImagePoint = Center(image.Size);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    DrawImage(image, x, DrawImagePoint.Y + y);
                    break;
                case HorizontalAlignment.Center:
                    DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y);
                    break;
                case HorizontalAlignment.Right:
                    DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y);
                    break;
            }
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

        #region " DrawGradient Overloads "

        //TODO: Remove triple overload?

        private LinearGradientBrush DrawGradientBrush;

        private Rectangle DrawGradientRectangle;
        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height)
        {
            DrawGradient(blend, x, y, width, height, 90);
        }
        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height)
        {
            DrawGradient(c1, c2, x, y, width, height, 90);
        }

        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(blend, DrawGradientRectangle, angle);
        }
        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(c1, c2, DrawGradientRectangle, angle);
        }

        protected void DrawGradient(ColorBlend blend, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, angle);
            DrawGradientBrush.InterpolationColors = blend;
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawGradient(Color c1, Color c2, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, c1, c2, angle);
            G.FillRectangle(DrawGradientBrush, r);
        }

        #endregion

    }

    enum MouseState : byte
    {
        None = 0,
        Over = 1,
        Down = 2,
        Block = 3
    }

    class Bloom
    {

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private Color _Value;
        public Color Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public Bloom()
        {
        }

        public Bloom(string name, Color value)
        {
            _Name = name;
            _Value = value;
        }

    }
}