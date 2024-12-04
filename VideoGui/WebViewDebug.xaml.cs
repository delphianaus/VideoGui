using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VideoGui.Models;
using VideoGui.Models.delegates;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for WebViewDebug.xaml
    /// </summary>
    public partial class WebViewDebug : Window
    {
        public bool Ready, Initizalized = false, IsClosing = false, IsClosed = false;
        string defaultUrl = "";
        OnFinish DoOnFinish = null;
        public WebViewDebug(OnFinish _OnFinish ,string defaultUrl)
        {
            InitializeComponent();
            DoOnFinish = _OnFinish;
            this.defaultUrl = defaultUrl;
            Closing += (s, e) =>
            {
                IsClosing = true;
            };
            Closed += (s, e) =>
            {
                IsClosed = true;
                DoOnFinish?.Invoke();
            };
        }
        private async void ProcessWebView(object sender)
        {
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    ProcessWebView(sender);
                    return;
                }
                if (sender is WebView2 webView2Instance)
                {
                    var task = webView2Instance.ExecuteScriptAsync("document.body.innerHTML");
                    task.ContinueWith(x => { ProcessWV2(x.Result, sender); }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessWebView {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private async void ProcessWV2(string html, object sender)
        {
            try
            {
                if (html is not null)
                {
                    var ehtml = Regex.Unescape(html);
                    if (html is not null && ehtml.Contains("Customize channel"))
                    {
                        Ready = true;  // Load Default URL.(DefaultUrl)
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ProcessWV2 {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private async void wv2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                if ((e is not null && e.IsSuccess) || e is null)
                {
                    ProcessWebView(sender);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"wv2_NavigationCompleted {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public WebView2 GetWebView() => wv2;

        public void BrowseUrl(string url)
        {
            try
            {
                if (Ready && Initizalized)
                {
                    wv2.Source = new Uri(url);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"BrowseUrl {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    brdmain.Width = Width - 20;
                    brdmain.Height = Height - (258 - 70);
                    var p = new Thickness(0, 0, 0, 0);
                    p.Left = Width - 692;
                    lstMain.Width = Width - 25;
                    StatusBar.Width = Width - 5;
                    RegistryKey key = "SOFTWARE\\Scraper".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("debugWebWidth", ActualWidth);
                    key.SetValue("debugWebHeight", ActualHeight);
                    key.SetValue("debugWebleft", Left);
                    key.SetValue("debugWebtop", Top);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        async void InitAsync()
        {
            try
            {
                var env = await CoreWebView2Environment.CreateAsync(null, @"c:\stuff\scraper");
                await wv2.EnsureCoreWebView2Async(env);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InitAsync {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\Scraper".OpenSubKey(Registry.CurrentUser);
                var _width = key.GetValue("debugWebWidth", ActualWidth).ToDouble();
                var _height = key.GetValue("debugWebHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("debugWebleft", Left).ToDouble();
                var _top = key.GetValue("debugWebtop", Top).ToDouble();
                Left = (Left != _left && _left != 0) ? _left : Left;
                Top = (Top != _top && _top != 0) ? _top : Top;
                Width = (ActualWidth != _width && _width != 0) ? _width : Width;
                Height = (ActualHeight != _height && _height != 0) ? _height : Height;
                lstMain.Width = Width - 5;
                var thick = new Thickness(0, 0, 0, 0);
                thick.Left = Width - 190;
                key?.Close();
                wv2.CoreWebView2InitializationCompleted += Wv2_CoreWebView2InitializationCompleted;
                Dispatcher.Invoke(() =>
                {
                    InitAsync();
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void Wv2_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            try
            {
                Initizalized = true;
                wv2.Source = new Uri(defaultUrl);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Wv2_CoreWebView2InitializationCompleted {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
    }
}
