using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VideoGui.Models;
using VideoGui.Models.delegates;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for DirectoryTitleDescEditor.xaml
    /// </summary>
    public partial class DirectoryTitleDescEditor : Window
    {
        databasehook<object> Invoker = null;
        public bool IsClosing = false, IsClosed = false;
        databasehook<object> Invoker = null;
        public static readonly DependencyProperty SourceDirectoryProperty =
            DependencyProperty.Register(nameof(SourceDirectory), typeof(double),
                typeof(DirectoryTitleDescEditor), new FrameworkPropertyMetadata(100.0));

        public double SourceDirectory
        {
            get => (double)GetValue(SourceDirectoryProperty);
            set => SetValue(SourceDirectoryProperty, value);
        }
        int ShortsIndex = 0;
        bool IsFirstResize = true, Ready = false;
        DispatcherTimer LocationChangedTimer = new DispatcherTimer();
        DispatcherTimer LocationChanger = new DispatcherTimer();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    Invoker?.Invoke(this, new CustomParams_Initialize());
                    LocationChanger.Interval = TimeSpan.FromMilliseconds(10);
                    LocationChanger.Tick += LocationChanger_Tick;
                    LocationChanger.Start();
                    LocationChanged += (s, e) =>
                    {
                        LocationChangedTimer.Stop();
                        LocationChangedTimer.Interval = TimeSpan.FromSeconds(3);
                        LocationChangedTimer.Tick += (s1, e1) =>
                        {
                            LocationChangedTimer.Stop();
                            RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                            key2?.SetValue("DEleft", Left);
                            key2?.SetValue("DEtop", Top);
                            key2?.Close();
                        };
                        LocationChangedTimer.Start();
                    };
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // This for is marked for mcu listbox upgrade
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnClose_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void TitleToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((sender is ToggleButton cbf) && (cbf.DataContext is ShortsDirectory ReleaseInfo))
                {
                    cbf.IsChecked = (ReleaseInfo.IsTitleAvailable) ? ReleaseInfo.IsTitleAvailable : cbf.IsChecked;
                    Invoker?.Invoke(this, new CustomParams_Select(ReleaseInfo.Id));
                    Invoker?.Invoke(this, new CustomParams_TitleSelect(ReleaseInfo));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"TitleToggle_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public void DoTitleSelectCreate(int TitleId = -1, int LinkedId = -1)
        {
            try
            {
                var _DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect,
                    Invoker, true, TitleId, LinkedId);
                Hide();
                _DoTitleSelectFrm.ShowActivated = true;
                _DoTitleSelectFrm.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoTitleSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public void DoDescSelectCreate(int DescId = -1, int LinkedId = -1)
        {
            try
            {
                var _DoDescSelectFrm = new DescSelectFrm(OnSelectFormClose, Invoker,
                    true, DescId, LinkedId);
                Hide();
                _DoDescSelectFrm.ShowActivated = true;
                _DoDescSelectFrm.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoDescSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void OnSelectFormClose(object sender, int e)
        {
            try
            {
                if (sender is DescSelectFrm frm)
                {
                    Invoker?.Invoke(this,
                        new CustomParams_Update(frm.TitleTagId, UpdateType.Description));
                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} OnSelectFormClose {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void DoOnFinishTitleSelect(object sender, int id)
        {
            try
            {
                if (sender is TitleSelectFrm frm)
                {
                    Invoker?.Invoke(this, new CustomParams_Update(frm.TitleId, UpdateType.Title));
                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoOnFinishTitleSelect {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void DescToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((sender is ToggleButton cbf) && (cbf.DataContext is ShortsDirectory ReleaseInfo))
                {
                    cbf.IsChecked = (ReleaseInfo.IsDescAvailable) ? ReleaseInfo.IsDescAvailable : cbf.IsChecked;
                    Invoker?.Invoke(this, new CustomParams_Select(ReleaseInfo.Id));
                    Invoker?.Invoke(this, new CustomParams_DescSelect(ReleaseInfo));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DescToggle_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded && Ready)
                {
                    ResizeControls(e.NewSize.Width, e.NewSize.Height, e.WidthChanged, e.HeightChanged);
                    if (e.HeightChanged || e.WidthChanged)
                    {
                        RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        key?.SetValue("DEWidth", ActualWidth);
                        key?.SetValue("DEHeight", ActualHeight);
                        key?.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public DirectoryTitleDescEditor(databasehook<object> _Invoker, OnFinishIdObj _DoOnFinished)
        {
            try
            {
                InitializeComponent();
                Invoker = _Invoker;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    _DoOnFinished?.Invoke(this, -1);
                };
                LocationChanger.Interval = TimeSpan.FromMilliseconds(10);
                LocationChanger.Tick += LocationChanger_Tick;
                LocationChanger.Start();
                LocationChanged += (s, e) =>
                {
                    LocationChangedTimer.Stop();
                    LocationChangedTimer.Interval = TimeSpan.FromSeconds(3);
                    LocationChangedTimer.Tick += (s1, e1) =>
                    {
                        LocationChangedTimer.Stop();
                        RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        key2?.SetValue("DEleft", Left);
                        key2?.SetValue("DEtop", Top);
                        key2?.Close();
                    };
                    LocationChangedTimer.Start();
                };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void LocationChanger_Tick(object? sender, EventArgs e)
        {
            try
            {
                LocationChanger.Stop();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                var _width = key.GetValue("DEWidth", ActualWidth).ToDouble();
                var _height = key.GetValue("DEHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("DEleft", Left).ToDouble();
                var _top = key.GetValue("DEtop", Top).ToDouble();
                key?.Close();
                Left = (Left != _left && _left != 0) ? _left : Left;
                Top = (Top != _top && _top != 0) ? _top : Top;
                Width = (ActualWidth != _width && _width != 0) ? _width : Width;
                Height = (ActualHeight != _height && _height != 0) ? _height : Height;
                ResizeControls(Width, Height, true, true);
                LoadingPanel.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Visible;
                if (IsFirstResize)
                {
                    IsFirstResize = false;
                    LoadingPanel.Height = 0;
                    MainScroller.ScrollToVerticalOffset(439); // Scroll to the main content
                }
                Ready = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LocationChanger_Tick {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void ResizeControls(double width, double height, bool v1, bool v2)
        {
            try
            {
                if (v1)
                {
                    msuDirectoryList.Width = width - 40;
                    MainGrid.Width = width;
                    LoadingPanel.Width = width;
                    MainContent.Width = width;
                    MainScroller.Width = width;
                    Canvas.SetLeft(btnClose, width - 118);
                    SourceDirectory = (width - 184);
                }
                if (v2)
                {
                    msuDirectoryList.Height = height - 90;
                    MainGrid.Height = height;
                    LoadingPanel.Height = height;
                    MainContent.Height = height;
                    MainScroller.Height = height;
                    Canvas.SetTop(btnClose, height - 75);
                }
                
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ResizeControls {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
    }
}
