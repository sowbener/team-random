//!CompilerOption:AddRef:PresentationFramework.dll
//!CompilerOption:AddRef:System.Xaml.dll

using Buddy.Overlay.Notifications;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

using Styx.Common;
using Styx.Helpers;


namespace Enyo.Shared
{
    class Logger
    {
        /// <summary>
        ///     The loggers which get called for logging.
        /// </summary>
        public static void PauseLog(string message, params object[] args)
        {
            Write(LogLevel.Normal, PauseColorException(message) ? Colors.Red : Colors.LimeGreen, message, args);
        }

        public static void DiagnosticLog(string message, params object[] args)
        {
            Write(LogLevel.Diagnostic, Colors.MediumPurple, message, args);
        }

        public static void PrintLog(string message, params object[] args)
        {
            Write(LogLevel.Normal, ColorException(message) ? Colors.White : Colors.DodgerBlue, message, args);
        }

        public static void WriteToFileLog(string message, params object[] args)
        {
            Logging.WriteToFileSync(LogLevel.Verbose, "[Enyo] " + message, args);
        }

        /// <summary>
        ///     Main void for logging purposes.
        /// </summary>
        /// <param name="level">Define the loglevel (Eg: LogLevel.Diagnostic).</param>
        /// <param name="specificcolor">Define the color (Eg: Colors.OrangeRed).</param>
        /// <param name="message">The actual message to print.</param>
        /// <param name="args">Arguments to fill parameters.</param>
        private static void Write(LogLevel level, Color specificcolor, string message, params object[] args)
        {
            Logging.Write(level, specificcolor, string.Format("[Enyo] {0}", message), args);
        }

        private static bool ColorException(string message)
        {
            return message.Contains("------------------------------------------");
        }

        private static bool PauseColorException(string message)
        {
            return message.Contains("Paused");
        }

        /// <summary>
        ///     Prints the Settings files to the logfile of HB.
        /// </summary>
        private static void LogSettings(string desc, Settings set)
        {
            if (set == null)
            {
                return;
            }

            WriteToFileLog("====== {0} ======", desc);
            foreach (var kvp in set.GetSettings())
            {
                WriteToFileLog("{0}: {1}", kvp.Key, kvp.Value.ToString());
            }
            WriteToFileLog("");
        }

        /// <summary>
        ///     Prints required diagnostical information to the Logfile.
        /// </summary>
        public static void PrintInformation(bool reinitialized = false)
        {
            WriteToFileLog("");
            WriteToFileLog("------------------------------------------");
            WriteToFileLog("Diagnostic Logging");
            WriteToFileLog("");
            WriteToFileLog("Enyo Revision: {0}", Enyo.Revision);
            WriteToFileLog("");
            // ReSharper disable once CSharpWarnings::CS0618
            WriteToFileLog("HardLock Enabled: {0}", GlobalSettings.Instance.UseFrameLock);
            WriteToFileLog("Ticks per Second: {0}", CharacterSettings.Instance.TicksPerSecond);
            WriteToFileLog("");
            LogSettings("Settings", BotSettings.Instance);
            WriteToFileLog("");
            WriteToFileLog("------------------------------------------");
        }
    }

    internal class ToastLog : ToastUIComponent
    {
        private TextBlock _textBlock;

        public Color Color
        {
            get; private set;
        }

        public FontFamily FontFamily
        {
            get; private set;
        }

        public Color ShadowColor
        {
            get; private set;
        }

        public uint FontSize
        {
            get; private set;
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
}
