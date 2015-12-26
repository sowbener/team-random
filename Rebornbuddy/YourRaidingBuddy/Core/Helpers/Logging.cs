using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Buddy.Overlay.Notifications;
using ff14bot.Helpers;
using ff14bot.Settings;
using TreeSharp;
using YourRaidingBuddy.Interfaces.Settings;
using Action = System.Action;

namespace YourRaidingBuddy.Helpers
{
    public static class Logger
    {
        private static int lineNo = 0;

        /// <summary>
        /// write message to log window and file
        /// </summary>
        /// <param name="message">message text</param>
        public static void Write(string message)
        {
            Write(Colors.Green, message);
        }

        internal class ToastLog : ToastUIComponent
        {
            private TextBlock _textBlock;

            public Color Color
            {
                get;
                private set;
            }

            public FontFamily FontFamily
            {
                get;
                private set;
            }

            public Color ShadowColor
            {
                get;
                private set;
            }

            public uint FontSize
            {
                get;
                private set;
            }

            public override FrameworkElement GuiElement
            {
                get { return TextBlock; }
            }

            private TextBlock TextBlock
            {
                get
                {
                    var textBlock0 = _textBlock;
                    if (textBlock0 != null) return textBlock0;
                    var textBlock = Updater();
                    var textBlock1 = textBlock;
                    _textBlock = textBlock;
                    textBlock0 = textBlock1;
                    return textBlock0;
                }
            }

            public Func<string> TextProducer { get; private set; }

            public ToastLog(Func<string> textProducer, TimeSpan duration, Color color, Color shadowColor, FontFamily fontFamily, uint fontSize)
            {
                TextProducer = textProducer;
                DisplayDuration = duration;
                Color = color;
                ShadowColor = shadowColor;
                FontFamily = fontFamily;
                FontSize = fontSize;

            }

            private TextBlock Updater()
            {
                var textBlock = new TextBlock
                {
                    Text = TextProducer(),
                    FontFamily = FontFamily,
                    FontSize = FontSize,
                    FontWeight = FontWeights.UltraBold,
                    Foreground = new SolidColorBrush(Color),
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    IsHitTestVisible = false,
                    TextAlignment = TextAlignment.Center
                };

                var dropShadowEffect = new DropShadowEffect
                {
                    Color = ShadowColor,
                    BlurRadius = 25,
                    ShadowDepth = 0
                };

                textBlock.Effect = dropShadowEffect;
                textBlock.RenderTransform = new ScaleTransform(1, 1);
                var textBlock1 = textBlock;
                var scaleTransform = new ScaleTransform(1, 1);
                textBlock1.RenderTransform = scaleTransform;

                return textBlock1;
            }

            protected override void Update()
            {
                TextBlock.Text = TextProducer();
            }
        }

        /// <summary>
        /// write message to log window and file
        /// </summary>
        /// <param name="message">message text with embedded parameters</param>
        /// <param name="args">replacement parameter values</param>
        public static void Write(string message, params object[] args)
        {
            Write(Colors.Green, message, args);
        }

        /// <summary>
        /// write message to log window and file.  overrides log windows duplicate
        /// line suppression by ensuring adjoining lines differ
        /// </summary>
        /// <param name="clr">color of message in window</param>
        /// <param name="message">message text with embedded parameters</param>
        /// <param name="args">replacement parameter values</param>
        public static void Write(Color clr, string message, params object[] args)
        {
            string sUniqueChar = (lineNo++ & 1) == 0 ? "" : " ";
            System.Windows.Media.Color newColor = System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
            if (GlobalSettings.Instance.LogLevel >= LogLevel.Normal)
                Logging.Write(newColor, "[YourRaidingBuddy] " + message + sUniqueChar, args);
            else if (GlobalSettings.Instance.LogLevel == LogLevel.Quiet)
                Logging.WriteToFileSync(LogLevel.Normal, "[YourRaidingBuddy] " + message + sUniqueChar, args);
        }

        /// <summary>
        /// write message to log window if Singular Debug Enabled setting true
        /// </summary>
        /// <param name="message">message text</param>
        public static void WriteDebug(string message)
        {
            WriteDebug(Colors.Orange, message);
        }

        /// <summary>
        /// write message to log window if Singular Debug Enabled setting true
        /// </summary>
        /// <param name="message">message text with embedded parameters</param>
        /// <param name="args">replacement parameter values</param>
        public static void WriteDebug(string message, params object[] args)
        {
            WriteDebug(Colors.Orange, message, args);
        }

        /// <summary>
        /// write message to log window if Singular Debug Enabled setting true
        /// </summary>
        /// <param name="clr">color of message in window</param>
        /// <param name="message">message text with embedded parameters</param>
        /// <param name="args">replacement parameter values</param>
        public static void WriteDebug(Color clr, string message, params object[] args)
        {
            System.Windows.Media.Color newColor = System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

            if (InternalSettings.Instance.General.Debug)
            {
                Logging.Write(newColor, "(YourRaidingBuddy) " + message, args);
            }
        }

        /// <summary>
        /// write message to log file
        /// </summary>
        /// <param name="message">message text</param>
        public static void WriteFile(string message)
        {
            WriteFile(LogLevel.Verbose, message);
        }

        /// <summary>
        /// write message to log file
        /// </summary>
        /// <param name="message">message text with replaceable parameters</param>
        /// <param name="args">replacement parameter values</param>
        public static void WriteFile(string message, params object[] args)
        {
            WriteFile(LogLevel.Diagnostic, message, args);
        }

        /// <summary>
        /// write message to log file
        /// </summary>
        /// <param name="ll">level to code entry with (doesn't control if written)</param>
        /// <param name="message">message text with replaceable parameters</param>
        /// <param name="args">replacement parameter values</param>
        public static void WriteFile(LogLevel ll, string message, params object[] args)
        {
            if (GlobalSettings.Instance.LogLevel >= LogLevel.Quiet)
                Logging.WriteToFileSync(ll, "(YourRaidingBuddy) " + message, args);
        }

        /// <summary>
        /// write message to log window if Singular Debug Enabled setting true
        /// </summary>
        /// <param name="message">message text</param>
        public static void WriteDiagnostic(string message)
        {
            WriteDiagnostic(Colors.Orange, message);
        }

        /// <summary>
        /// write message to log window if Singular Debug Enabled setting true
        /// </summary>
        /// <param name="message">message text with embedded parameters</param>
        /// <param name="args">replacement parameter values</param>
        public static void WriteDiagnostic(string message, params object[] args)
        {
            WriteDiagnostic(Colors.Orange, message, args);
        }

        /// <summary>
        /// write message to log window if Singular Debug Enabled setting true
        /// </summary>
        /// <param name="clr">color of message in window</param>
        /// <param name="message">message text with embedded parameters</param>
        /// <param name="args">replacement parameter values</param>
        public static void WriteDiagnostic(Color clr, string message, params object[] args)
        {
            System.Windows.Media.Color newColor = System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

            if (InternalSettings.Instance.General.Debug)
            {
                Logging.Write(newColor, "(YourRaidingBuddy) " + message, args);
            }
            else
            {
                WriteFile(LogLevel.Diagnostic, "(YourRaidingBuddy) " + message, args);
            }
        }

        public static void PrintStackTrace(string reason = "Debug")
        {
            WriteDebug("Stack trace for " + reason);
            var stackTrace = new StackTrace(true);
            StackFrame[] stackFrames = stackTrace.GetFrames();
            // Start at frame 1 (just before this method entrance)
            for (int i = 1; i < Math.Min(stackFrames.Length, 10); i++)
            {
                StackFrame frame = stackFrames[i];
                WriteDebug(string.Format("\tCaller {0}: {1} in {2} line {3}", i, frame.GetMethod().Name, Path.GetFileName(frame.GetFileName()), frame.GetFileLineNumber()));
            }
        }

        /// <summary>
        /// write behavior creation message to log window and file
        /// </summary>
        /// <param name="message">message text with embedded parameters</param>
        /// <param name="args">replacement parameter values</param>
        public static void WriteInBehaviorCreate(string message, params object[] args)
        {
                Write(message, args);
        }

        public static void WriteInBehaviorCreate(Color clr, string message, params object[] args)
        {
                Write(clr, message, args);
        }

        /// <summary>
        /// write behavior creation message to log window and file
        /// </summary>
        /// <param name="message">message text with embedded parameters</param>
        /// <param name="args">replacement parameter values</param>
        public static void WriteDebugInBehaviorCreate(string message, params object[] args)
        {
                WriteDebug(message, args);
        }

        public static void WriteDebugInBehaviorCreate(Color clr, string message, params object[] args)
        {
                WriteDebug(clr, message, args);
        }
    }
   
}