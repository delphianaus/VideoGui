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
    /// Interaction logic for MultiShortsUploader.xaml
    /// </summary>
    public partial class MultiShortsUploader : Window
    {
        databasehook<object> dbInit = null;
        public bool IsClosing = false, IsClosed = false, Ready = false;
        DispatcherTimer LocationChangedTimer = new DispatcherTimer();
        public static readonly DependencyProperty Column_WidthProperty =
            DependencyProperty.Register("Column_Width",
            typeof(GridLength), typeof(MultiShortsUploader),
            new PropertyMetadata(new GridLength(363, GridUnitType.Pixel)));
        public GridLength Column_Width
        {
            get { return (GridLength)GetValue(Column_WidthProperty); }
            set { SetValue(Column_WidthProperty, value); }
        }


        public MultiShortsUploader(databasehook<object> _dbInit, OnFinish _DoOnFinished)
        {
            try
            {
                InitializeComponent();
                dbInit = _dbInit;
                Closing += (s, e) =>
                {
                    IsClosing = true;
                    RegistryKey key = "SOFTWARE\\Scraper".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("MSUWidth", ActualWidth);
                    key.SetValue("MSUHeight", ActualHeight);
                    key.SetValue("MSUleft", Left);
                    key.SetValue("MSUtop", Top);
                    key?.Close();
                };
                Closed += (s, e) => { IsClosed = true; _DoOnFinished?.Invoke(); };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dbInit?.Invoke(this, new CustomParams_Initialize());
                Ready = true;
                RegistryKey key = "SOFTWARE\\Scraper".OpenSubKey(Registry.CurrentUser);
                var _width = key.GetValue("MSUWidth", ActualWidth).ToDouble();
                var _height = key.GetValue("MSUHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("MSUleft", Left).ToDouble();
                var _top = key.GetValue("MSUtop", Top).ToDouble();
                this.WindowStartupLocation = WindowStartupLocation.Manual;
                Width = (ActualWidth != _width && _width != 0) ? _width : Width;
                Height = (ActualHeight != _height && _height != 0) ? _height : Height;
                Left = (Left != _left && _left != 0) ? _left : Left;
                Top = (Top != _top && _top != 0) ? _top : Top;
                Column_Width = new GridLength(393, GridUnitType.Pixel);
                LocationChanged += (s, e) =>
                {
                    LocationChangedTimer.Stop();
                    LocationChangedTimer.Interval = TimeSpan.FromSeconds(3);
                    LocationChangedTimer.Tick += (s1, e1) =>
                    {
                        LocationChangedTimer.Stop();
                        RegistryKey key2 = "SOFTWARE\\Scraper".OpenSubKey(Registry.CurrentUser);
                        key2.SetValue("MSUleft", Left);
                        key2.SetValue("MSUtop", Top);
                        key2?.Close();
                    };
                    LocationChangedTimer.Start();
                };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded && Ready)
                {
                    if (e.HeightChanged)
                    {
                        msuSchedules.Height = e.NewSize.Height - 269;
                    }
                    if (e.WidthChanged)
                    {
                        msuSchedules.Width = e.NewSize.Width - 25;
                        msuShorts.Width = e.NewSize.Width - 25;
                        Column_Width = new GridLength(e.NewSize.Width - 135, GridUnitType.Pixel);
                    }
                    if (e.HeightChanged || e.WidthChanged)
                    {
                        RegistryKey key = "SOFTWARE\\Scraper".OpenSubKey(Registry.CurrentUser);
                        key.SetValue("MSUWidth", ActualWidth);
                        key.SetValue("MSUHeight", ActualHeight);
                        key.SetValue("MSUleft", Left);
                        key.SetValue("MSUtop", Top);
                        key?.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void mnuMoveDirectory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (MultiShortsInfo info in msuShorts.SelectedItems)
                {
                    dbInit?.Invoke(this, new CustomParams_MoveDirectory(info.DirectoryName));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuMoveDirectory_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void mnuAddToSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    foreach (MultiShortsInfo info in msuShorts.SelectedItems)
                    {
                        dbInit?.Invoke(this, new CustomParams_AddDirectory(info.DirectoryName));
                    }
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"mnuMoveDirectory_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuAddToSelected_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }

        }

        private void MultiListboxColumnDefinition_ToggleButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleButton t)
                {
                    if (t.Width > 0 && t.Height > 0)
                    {

                    }

                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"MultiListboxColumnDefinition_ToggleButtonClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private T FindVisualChild<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && ((T)child).Name == name)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child, name);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
        private void tx_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in msuSchedules.Items)
                {

                    var container = msuSchedules.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    if (container != null)
                    {
                        var border = VisualTreeHelper.GetChild(container, 0) as Border;
                        if (border != null)
                        {
                            // Get the ContentPresenter inside the Border
                            var contentPresenter = VisualTreeHelper.GetChild(border, 0) as ContentPresenter;
                            if (contentPresenter != null)
                            {
                                // Get the Grid that's created by our ItemTemplate
                                var grid = VisualTreeHelper.GetChild(contentPresenter, 0) as Grid;
                                if (grid != null)
                                {
                                    foreach (var toggleButton in grid.Children.OfType<ToggleButton>())
                                    {
                                        toggleButton.Style = tx.Style;
                                        ///toggleButton.Template = FindResource("ToggleSetEditStyle") as ControlTemplate;
                                    }
                                }
                            }
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"tx_Checked {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void mnuRemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (SelectedShortsDirectories info in msuShorts.SelectedItems)
                {
                    dbInit?.Invoke(this, new CustomParams_RemoveSelectedDirectory(info.Id, info.DirectoryName));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuRemoveSelected_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"BtnClose_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
    }
}
