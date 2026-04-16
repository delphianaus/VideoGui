using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for UploadLimits.xaml
    /// </summary>
    public partial class UploadLimits : Window
    {
        databasehook<Object> Invoker = null;
        public bool IsClosing = true;
        string FormName = "AUL";
        int Id = -1;
        bool Ready = false;
        DateOnly CurrentDate = new DateOnly();
        DispatcherTimer LocationChangedTimer = new DispatcherTimer(),
            LocationChanger = new DispatcherTimer();

        //LimitDateWidth
        public double LimitDateWidth
        {
            get => (double)GetValue(LimitDateWidthProperty);
            set => SetValue(LimitDateWidthProperty, value);
        }


        public static readonly DependencyProperty LimitDateWidthProperty = DependencyProperty.Register(nameof(LimitDateWidthProperty),
          typeof(double), typeof(UploadLimits), new FrameworkPropertyMetadata(375.0));
        //ActiveLimitWidth
        public double LimitWidth
        {
            get => (double)GetValue(LimitWidthProperty);
            set => SetValue(LimitWidthProperty, value);
        }

        public static readonly DependencyProperty LimitWidthProperty = DependencyProperty.Register(nameof(LimitWidthProperty),
          typeof(double), typeof(UploadLimits), new FrameworkPropertyMetadata(200.0));

        //ActiveLimitWidth
        public double ActiveLimitWidth
        {
            get => (double)GetValue(ActiveLimitWidthProperty);
            set => SetValue(ActiveLimitWidthProperty, value);
        }

        public static readonly DependencyProperty ActiveLimitWidthProperty = DependencyProperty.Register(nameof(ActiveLimitWidthProperty),
          typeof(double), typeof(UploadLimits), new FrameworkPropertyMetadata(75.0));

        public UploadLimits(databasehook<object> _Invoker, OnFinishIdObj _DoOnFinished)
        {
            InitializeComponent();
            Invoker = _Invoker;
            Closing += (s, e) =>
            {
                IsClosing = true;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                key.SetValue($"{FormName}Width", ActualWidth);
                key.SetValue($"{FormName}Height", ActualHeight);
                key.SetValue($"{FormName}left", Left);
                key.SetValue($"{FormName}top", Top);
                key?.Close();
            };
        }
        private void LocationChanger_Tick(object? sender, EventArgs e)
        {
            try
            {
                LocationChanger.Stop();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                var _width = key.GetValue($"{FormName}Width", ActualWidth).ToDouble();
                var _height = key.GetValue($"{FormName}Height", ActualHeight).ToDouble();
                var _left = key.GetValue($"{FormName}left", Left).ToDouble();
                var _top = key.GetValue($"{FormName}top", Top).ToDouble();
                //var _msuShortsHeight = key.GetValue("MSUSHortsHeight", msuShorts.Height).ToDouble();
                key?.Close();
                Left = (Left != _left && _left != 0) ? _left : Left;
                Top = (Top != _top && _top != 0) ? _top : Top;
                Width = (ActualWidth != _width && _width != 0) ? _width : Width;
                Height = (ActualHeight != _height && _height != 0) ? _height : Height;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LocationChanger_Tick {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void msuLimits_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.OriginalSource is TextBlock m &&
                                   m.DataContext is AutoUploadLimits rp)// && !rp.IsActive)
                {
                    Id = rp.Id;
                    LimitDate.Value = rp.LimitDate.ToDateTime(new TimeOnly(0, 0, 0));
                    txtLimit.Text = rp.Limit.ToString();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"msuLimits_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LimitDate.Value.HasValue && txtLimit.ToInt(-1) != -1 && Id != -1)
                {
                    Invoker?.Invoke(this, new CustomParams_EditLimit(
                     Id, ChkActive.IsChecked.Value, txtLimit.Text.ToInt(-1)));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSave_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LimitDate.Value.HasValue && txtLimit.ToInt(-1) != -1)
                {
                    Invoker?.Invoke(this, new CustomParams_AddNewLimit(
                     DateOnly.FromDateTime(LimitDate.Value.Value.Date),
                    txtLimit.Text.ToInt(-1), ChkActive.IsChecked.Value));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSave_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void ReleaseDate_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LimitDate is not null)
                {
                    CurrentDate = DateOnly.FromDateTime(LimitDate.Value.Value.Date);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"UploadLimits.ReleaseDate_LostFocus {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-100));
                LimitDate.Text = "";
                txtLimit.Text = "";
                Id = -1;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"UploadLimits.btnCancel_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void LimitDate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (Ready && LimitDate.Value.HasValue)
                {
                    CurrentDate = DateOnly.FromDateTime(LimitDate.Value.Value.Date);
                    Id = -1;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"UploadLimits.btnCancel_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Invoker?.Invoke(this, new CustomParams_Initialize());
                LocationChanger.Interval = TimeSpan.FromMilliseconds(20);
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
                        key2?.SetValue($"{FormName}left", Left);
                        key2?.SetValue($"{FormName}top", Top);
                        key2?.Close();
                    };
                    LocationChangedTimer.Start();
                };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"UploadLimits.Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
    }

}
