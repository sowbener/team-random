using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Buddy.Overlay;
using Buddy.Overlay.Controls;
using YourRaidingBuddy.Books;
using YourRaidingBuddy.Helpers;
using YourRaidingBuddy.Interfaces.Settings;

namespace YourRaidingBuddy.Interface.Overlay
{
    // Stackpanel vs DockPanel http://stackoverflow.com/questions/569095/how-to-get-stackpanels-children-to-fill-maximum-space-downward
    //https://msdn.microsoft.com/en-us/library/ms754213%28v=vs.110%29.aspx
    internal class FuOverlayControl : OverlayUIComponent
    {
        internal static OverlayControl OvControl;

        private static StackPanel _stackpanel;
        private static Grid _maingrid;
        private static Grid _contentgrid;
        private static Border _titlebar;

        private static OutlinedTextBlock _cooldownslabel;
        private static OutlinedTextBlock _cooldownsenabled;
        private static OutlinedTextBlock _cooldownsdisabled;

        private static OutlinedTextBlock _pauselabel;
        private static OutlinedTextBlock _pausedisabled;
        private static OutlinedTextBlock _pauseenabled;

        private static OutlinedTextBlock _multitargetlabel;
        private static OutlinedTextBlock _multitargetenabled;
        private static OutlinedTextBlock _multitargetdisabled;

        public FuOverlayControl()
            : base(true) { }

        /// <summary>
        /// Updating and rendering the actual overlay.
        /// 
        /// Everything to add/remove/update the UI should be done in here. Pretty much all of RenderOverlay should be in here.
        /// </summary>
        protected override void Update()
        {
            try
            {
                #region RebuildRequired
                if (VariableBook.OverlayRebuildRequired)
                {
                    DisposeOverlay();

                    VariableBook.OverlayRebuildRequired = false;
                    VariableBook.OverlayActive = false;

                    Overlay.CreateOverlay();
                }
                #endregion

                #region OverlayActive
                if (!VariableBook.OverlayActive)
                {
                    var titlebarbg = new SolidColorBrush(Colors.DarkOrange) { Opacity = 0.25 };

                    #region Variables
                    /* Create the titlebar and add it as child to the maingrid */
                    _titlebar = new Border
                    {

                        Background = titlebarbg,
                        BorderThickness = new Thickness(0, 0, 0, 1),
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Opacity = InternalSettings.Instance.General.OverlayTextOpacity,

                        Child = new TextBlock
                        {
                            Foreground = Brushes.Black,
                            FontFamily = new FontFamily("Century Gothic"),
                            FontWeight = FontWeights.Bold,
                            FontSize = InternalSettings.Instance.General.OverlayTextSize,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Text = "YourRaidingBuddy",
                            VerticalAlignment = VerticalAlignment.Center,
                        }
                    };

                    /* Creating a TextBlock */
                    _pauselabel = new OutlinedTextBlock
                    {
                        FontFamily = new FontFamily("Century Gothic"),
                        Fill = new SolidColorBrush(Colors.White),
                        FontSize = InternalSettings.Instance.General.OverlayTextSize,
                        FontWeight = FontWeights.ExtraBold,
                        Opacity = InternalSettings.Instance.General.OverlayTextOpacity,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Black),
                        Text = " Pause State: ",

                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    /* Creating a TextBlock */
                    _pauseenabled = new OutlinedTextBlock
                    {
                        FontFamily = new FontFamily("Century Gothic"),
                        Fill = new SolidColorBrush(Colors.White),
                        FontSize = InternalSettings.Instance.General.OverlayTextSize,
                        FontWeight = FontWeights.ExtraBold,
                        Opacity = InternalSettings.Instance.General.OverlayTextOpacity,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Red),
                        Text = " Paused ",

                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    /* Creating a TextBlock */
                    _pausedisabled = new OutlinedTextBlock
                    {
                        FontFamily = new FontFamily("Century Gothic"),
                        Fill = new SolidColorBrush(Colors.White),
                        FontSize = InternalSettings.Instance.General.OverlayTextSize,
                        FontWeight = FontWeights.ExtraBold,
                        Opacity = InternalSettings.Instance.General.OverlayTextOpacity,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Green),
                        Text = " Resumed ",

                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    /* Creating a TextBlock */
                    _multitargetlabel = new OutlinedTextBlock
                    {
                        FontFamily = new FontFamily("Century Gothic"),
                        Fill = new SolidColorBrush(Colors.White),
                        FontSize = InternalSettings.Instance.General.OverlayTextSize,
                        FontWeight = FontWeights.ExtraBold,
                        Opacity = InternalSettings.Instance.General.OverlayTextOpacity,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Black),
                        Text = " AoE State: ",

                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    /* Creating a TextBlock */
                    _multitargetdisabled = new OutlinedTextBlock
                    {
                        FontFamily = new FontFamily("Century Gothic"),
                        Fill = new SolidColorBrush(Colors.White),
                        FontSize = InternalSettings.Instance.General.OverlayTextSize,
                        FontWeight = FontWeights.ExtraBold,
                        Opacity = InternalSettings.Instance.General.OverlayTextOpacity,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Red),
                        Text = " Off ",

                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    /* Creating a TextBlock */
                    _multitargetenabled = new OutlinedTextBlock
                    {
                        FontFamily = new FontFamily("Century Gothic"),
                        Fill = new SolidColorBrush(Colors.White),
                        FontSize = InternalSettings.Instance.General.OverlayTextSize,
                        FontWeight = FontWeights.ExtraBold,
                        Opacity = InternalSettings.Instance.General.OverlayTextOpacity,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Green),
                        Text = " On ",

                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    /* Creating a TextBlock */
                    _cooldownslabel = new OutlinedTextBlock
                    {
                        FontFamily = new FontFamily("Century Gothic"),
                        Fill = new SolidColorBrush(Colors.White),
                        FontSize = InternalSettings.Instance.General.OverlayTextSize,
                        FontWeight = FontWeights.ExtraBold,
                        Opacity = InternalSettings.Instance.General.OverlayTextOpacity,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Black),
                        Text = " Cooldown State: ",

                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    /* Creating a TextBlock */
                    _cooldownsdisabled = new OutlinedTextBlock
                    {
                        FontFamily = new FontFamily("Century Gothic"),
                        Fill = new SolidColorBrush(Colors.White),
                        FontSize = InternalSettings.Instance.General.OverlayTextSize,
                        FontWeight = FontWeights.ExtraBold,
                        Opacity = InternalSettings.Instance.General.OverlayTextOpacity,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Red),
                        Text = " Off ",

                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    /* Creating a TextBlock */
                    _cooldownsenabled = new OutlinedTextBlock
                    {
                        FontFamily = new FontFamily("Century Gothic"),
                        Fill = new SolidColorBrush(Colors.White),
                        FontSize = InternalSettings.Instance.General.OverlayTextSize,
                        FontWeight = FontWeights.ExtraBold,
                        Opacity = InternalSettings.Instance.General.OverlayTextOpacity,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Green),
                        Text = " On ",

                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    #endregion

                    #region Grids
                    /* Creating the Grids required for component placement */
                    _maingrid = new Grid
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    _contentgrid = new Grid
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    /* Adding a Row to maingrid (0) */
                    _maingrid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = GridLength.Auto // The size is determined by the size properties of the content object.
                    });

                    /* Adding a Row to maingrid (1) */
                    _maingrid.RowDefinitions.Add(new RowDefinition
                    {
                        //Height = GridLength.Auto, // The size is determined by the size properties of the content object.
                        Height = new GridLength(1, GridUnitType.Star) // The value is expressed as a weighted proportion of available space.
                    });

                    /* Adding a Row to contentgrid (0) */
                    _contentgrid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = GridLength.Auto
                    });

                    /* Adding a Row to contentgrid (1) */
                    _contentgrid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = GridLength.Auto
                    });

                    /* Adding a Row to contentgrid (2) */
                    _contentgrid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = GridLength.Auto
                    });

                    /* Adding a Row to contentgrid (3) */
                    _contentgrid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = GridLength.Auto
                    });

                    /* Adding a Column to contentgrid (0) */
                    _contentgrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    });

                    /* Adding a Column to contentgrid (1) */
                    _contentgrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    });
                    #endregion

                    #region Rows and Columns
                    /* Setting positions of the upcoming childs on the grids */
                    Grid.SetRow(_maingrid, 0);
                    Grid.SetRow(_contentgrid, 1);

                    /* MultiTarget Grid functions */
                    Grid.SetRow(_multitargetlabel, 1);
                    Grid.SetColumn(_multitargetdisabled, 2);
                    Grid.SetRow(_multitargetdisabled, 1);
                    Grid.SetColumn(_multitargetenabled, 2);
                    Grid.SetRow(_multitargetenabled, 1);

                    /* Cooldowns Grid functions */
                    Grid.SetRow(_cooldownslabel, 2);
                    Grid.SetColumn(_cooldownsdisabled, 2);
                    Grid.SetRow(_cooldownsdisabled, 2);
                    Grid.SetColumn(_cooldownsenabled, 2);
                    Grid.SetRow(_cooldownsenabled, 2);

                    /* Pause Grid functions */
                    Grid.SetRow(_pauselabel, 3);
                    Grid.SetColumn(_pauseenabled, 2);
                    Grid.SetRow(_pauseenabled, 3);
                    Grid.SetColumn(_pausedisabled, 2);
                    Grid.SetRow(_pausedisabled, 3);
                    #endregion

                    #region Rendering
                    /* Refreshing or building the actual overlay components */
                    Logger.Write("Overlay: Rendering the overlay (Complete).");

                    /* Making sure that the Stackpanel is empty */
                    _stackpanel.Children.Clear();

                    /* Adding the maingrid to the StackPanel - Main UI component */
                    _stackpanel.Children.Add(_maingrid);

                    /* Adding the remaining components to the maingrid */
                    _maingrid.Children.Add(_titlebar);
                    _maingrid.Children.Add(_contentgrid);

                    /* Adding the pause functions to contentgrid */
                    _contentgrid.Children.Add(_pauselabel);
                    _contentgrid.Children.Add(VariableBook.HkmPaused ? _pauseenabled : _pausedisabled);

                    /* Adding the multitarget functions to contentgrid */
                    _contentgrid.Children.Add(_cooldownslabel);
                    _contentgrid.Children.Add(VariableBook.HkmCooldowns ? _cooldownsenabled : _cooldownsdisabled);

                    /* Adding the multitarget functions to contentgrid */
                    _contentgrid.Children.Add(_multitargetlabel);
                    _contentgrid.Children.Add(VariableBook.HkmMultiTarget ? _multitargetenabled : _multitargetdisabled);

                    Logger.Write("Overlay: Overlay is rendered (Complete).");
                    #endregion

                    /* Setting Overlay to Active */
                    VariableBook.OverlayActive = true;
                }
                #endregion

                #region Replacements

                #region Paused
                if (_contentgrid.Children.Contains(_pausedisabled) && VariableBook.OverlayPaused)
                {
                    /* Refreshing or building the actual overlay components */
                    _contentgrid.Children.Remove(_pausedisabled);
                    _contentgrid.Children.Add(VariableBook.HkmPaused ? _pauseenabled : _pausedisabled);

                    Logger.Write("Overlay: Rendering the overlay (YRBPause - Disabled).");
                }

                if (_contentgrid.Children.Contains(_pauseenabled) && !VariableBook.OverlayPaused)
                {
                    /* Refreshing or building the actual overlay components */
                    _contentgrid.Children.Remove(_pauseenabled);
                    _contentgrid.Children.Add(VariableBook.HkmPaused ? _pauseenabled : _pausedisabled);

                    Logger.Write("Overlay: Rendering the overlay (YRBPause - Enabled).");
                }
                #endregion

                #region Cooldowns
                if (VariableBook.OverlayRefreshCooldowns)
                {
                    if (_contentgrid.Children.Contains(_cooldownsdisabled)) _contentgrid.Children.Remove(_cooldownsdisabled);
                    if (_contentgrid.Children.Contains(_cooldownsenabled)) _contentgrid.Children.Remove(_cooldownsenabled);
                    _contentgrid.Children.Add(VariableBook.HkmCooldowns ? _cooldownsenabled : _cooldownsdisabled);

                    Logger.Write("Overlay: Rendering the overlay (YRBCooldowns).");

                    VariableBook.OverlayRefreshCooldowns = false;
                }
                #endregion

                #region MultiTarget
                if (VariableBook.OverlayRefreshMultiTarget)
                {
                    if (_contentgrid.Children.Contains(_multitargetdisabled)) _contentgrid.Children.Remove(_multitargetdisabled);
                    if (_contentgrid.Children.Contains(_multitargetenabled)) _contentgrid.Children.Remove(_multitargetenabled);
                    _contentgrid.Children.Add(VariableBook.HkmMultiTarget ? _multitargetenabled : _multitargetdisabled);

                    Logger.Write("Overlay: Rendering the overlay (YRBMultiTarget).");

                    VariableBook.OverlayRefreshMultiTarget = false;
                }
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                Logger.WriteDebug("FuOverlayControl.Update() Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Creating the overlay control and initializing the primary stackpanel.
        /// http://wiki.thebuddyforum.com/index.php?title=Honorbuddy:Developer_Notebook:Game-client_Overlays
        /// </summary>
        public override OverlayControl Control
        {
            get
            {
                if (OvControl != null)
                    return OvControl;

                var ovcontrolbg = new SolidColorBrush(Colors.Black) { Opacity = 0.25 };

                if (InternalSettings.Instance.General.OverlayOpacity == OverlayColor.Black)
                    ovcontrolbg = new SolidColorBrush(Colors.Black);

                if (InternalSettings.Instance.General.OverlayOpacity == OverlayColor.BlackTranslucent)
                    ovcontrolbg = new SolidColorBrush(Colors.Black) { Opacity = 0.25 };

                if (InternalSettings.Instance.General.OverlayOpacity == OverlayColor.Translucent)
                    ovcontrolbg = new SolidColorBrush(Colors.Transparent);

                _stackpanel = new StackPanel();

                OvControl = new OverlayControl
                {
                    Content = _stackpanel,

                    AllowMoving = true,
                    AllowResizing = true,
                    Background = ovcontrolbg,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    X = InternalSettings.Instance.General.OverlayX,
                    Y = InternalSettings.Instance.General.OverlayY,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                return OvControl;
            }
        }

        /// <summary>
        /// Removes the Overlay when called.
        /// </summary>
        internal static void DisposeOverlay()
        {
            InternalSettings.Instance.General.OverlayX = OvControl.X;
            InternalSettings.Instance.General.OverlayY = OvControl.Y;
            InternalSettings.Instance.General.Save();

            ff14bot.Core.OverlayManager.RemoveUIComponent(Overlay.YRBOverlayComponent);
        }
    }
}