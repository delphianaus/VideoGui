using CustomComponents.ListBoxExtensions;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
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
using Windows.Devices.Geolocation;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Media.Core;
using Windows.UI.Composition;
using Path = System.IO.Path;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ProcessDraftTargets.xaml
    /// </summary>
    public partial class ProcessDraftTargets : Window
    {
        databasehook<object> Invoker = null;
        public bool IsClosed = false, IsClosing = false;
        int LinkedId = -1, SelectedTitleId = -1;
        public Action<object> RestoreParent = null;
        string connectionStr = "", OldTarget, OldgUrl;
        public int ShortsIndex = -1;
        private bool _isFirstResize = true;
        DispatcherTimer LocationChangedTimer = new DispatcherTimer(), LocationChanger = new DispatcherTimer();
        bool Ready = false, IsFirstResize = true;
        public static readonly DependencyProperty TitleWidthProperty =
            DependencyProperty.Register(nameof(TitleWidth), typeof(double),
                typeof(ProcessDraftTargets),
                new FrameworkPropertyMetadata(375.0));


        //ShortsDirectoryNameWidth & MultiShortsDirectoryNameWidth
        public double TitleWidth
        {
            get => (double)GetValue(TitleWidthProperty);
            set => SetValue(TitleWidthProperty, value);
        }
        public void ShowParent() => RestoreParent?.Invoke(this);

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public ProcessDraftTargets(databasehook<object> _Invoker, OnFinishObj _OnFinish, Action<object> _RestoreParent)
        {
            InitializeComponent();
            RestoreParent = _RestoreParent;
            Invoker = _Invoker;
            Closing += (s, e) => { IsClosing = true; };
            Closed += (s, e) => { IsClosed = true; _OnFinish?.Invoke(this); };
        }

        private void DoDescSelectCreate(int DescId = -1, int LinkedId = -1)
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
                ex.LogWrite($"DoDescSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void OnSelectFormClose(object sender, int e)
        {
            try
            {
                Show();
                if (sender is DescSelectFrm frm)
                {
                    int LinkedId = frm.LinkedId;
                    foreach (var sch in msuTargets.Items.OfType<SelectedShortsDirectories>().Where(x => x.LinkedShortsDirectoryId == ShortsIndex))
                    {
                        sch.DescId = frm.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} OnSelectFormClose {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Desc_ToggleButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleButton t && t.DataContext is SelectedShortsDirectories info)
                {
                    e.Handled = true;
                    LinkedId = info.LinkedShortsDirectoryId;
                    t.IsChecked = info.IsDescAvailable;
                    DoDescSelectCreate(info.DescId);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Desc_ToggleButtonClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void Title_ToggleButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleButton t && t.DataContext is SelectedShortsDirectories info)
                {
                    e.Handled = true;
                    Invoker?.Invoke(this, new CustomParams_SetIndex(info.LinkedShortsDirectoryId));
                    DoTitleSelectCreate(info.TitleId, info.LinkedShortsDirectoryId);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Title_ToggleButtonClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded && Ready)
                {
                    if (_isFirstResize)
                    {
                        _isFirstResize = false;
                        LoadingPanel.Visibility = Visibility.Collapsed;
                        MainContent.Visibility = Visibility.Visible;
                    }
                    SetControls(e.NewSize.Width, e.NewSize.Height, e.WidthChanged, e.HeightChanged);
                    if (e.HeightChanged || e.WidthChanged)
                    {
                        RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        key?.SetValue("MSDWidth", ActualWidth);
                        key?.SetValue("MSDHeight", ActualHeight);
                        key?.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} Window_SizeChanged {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                connectionStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                Invoker?.Invoke(this, new CustomParams_Initialize());
                key?.Close();
                Ready = false;
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
                        key2?.SetValue("MSDleft", Left);
                        key2?.SetValue("MSDtop", Top);
                        key2?.Close();
                    };
                    LocationChangedTimer.Start();
                };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void LocationChanger_Tick(object? sender, EventArgs e)
        {
            try
            {
                LocationChanger.Stop();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                var _widthidth = key.GetValue("MSDWidth", ActualWidth).ToDouble();
                var _heighteight = key.GetValue("MSDHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("MSDleft", Left).ToDouble();
                var _top = key.GetValue("MSDtop", Top).ToDouble();
                key?.Close();
                Left = (Left != _left && _left != 0) ? _left : Left;
                Top = (Top != _top && _top != 0) ? _top : Top;
                Width = (ActualWidth != _widthidth && _widthidth != 0) ? _widthidth : Width;
                Height = (ActualHeight != _heighteight && _heighteight != 0) ? _heighteight : Height;
                SetControls(Width, Height, true, true);
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
                ex.LogWrite($"{this} LocationChanger_Tick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }

        }

        private void SetControls(double _width, double _height, bool SetWidth = true, bool SetHeight = true)
        {
            try
            {
                if (SetHeight)
                {
                    MainGrid.Height = _height;
                    LoadingPanel.Height = _height;
                    MainScroller.Height = _height;
                    MainContent.Height = _height;
                    ResizeMultilistBoxes(msuTargets.Height);
                    Canvas.SetTop(BtnClose, _height - 78);
                    double r = 4.2;
                }
                if (SetWidth)
                {
                    MainScroller.Width = _width;
                    MainGrid.Width = _width;
                    MainContent.Width = _width;
                    msuTargets.Width = _width - 15;
                    Canvas.SetLeft(BtnClose, _width - BtnClose.Width - 25);
                    var r = _width - 530;
                    TitleWidth = r > 0 ? r : 100;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetCanvasChildren {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void ResizeMultilistBoxes(double newHeight)
        {
            try
            {
                double totalHeight = MainContent.Height - 77;
                double OtherHeight = totalHeight - newHeight;
                if (OtherHeight > 0)
                {
                    Canvas.SetTop(msuTargets, newHeight - 4);
                    msuTargets.Height = OtherHeight;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ResizeMultilistBoxes {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public void DoTitleSelectCreate(int TitleId = -1, int LinkedId = -1)
        {
            try
            {
                SelectedTitleId = TitleId;
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
        private void DoOnFinishTitleSelect(object sender, int e)
        {
            try
            {
                if (sender is TitleSelectFrm _DoTitleSelectFrm && _DoTitleSelectFrm.IsClosed)
                {
                    bool found = false;
                    ShortsIndex = _DoTitleSelectFrm.LinkedId;
                    string sql = "";
                    if ((SelectedTitleId != _DoTitleSelectFrm.TitleId))
                    {
                        sql = "UPDATE SHORTSDIRECTORY SET TITLEID = @TITLEID WHERE ID = @ID";
                        connectionStr.ExecuteNonQuery(sql, [("@ID", ShortsIndex), 
                            ("@TITLEID", _DoTitleSelectFrm.TitleId)]);
                    }
                    string linkedtitleid = "";
                    sql = GetShortsDirectorySql(ShortsIndex);
                    CancellationTokenSource cts = new CancellationTokenSource();
                    connectionStr.ExecuteReader(sql, cts, (FbDataReader r) =>
                    {
                        linkedtitleid = (r["LINKEDTITLEIDS"] is string tidt ? tidt : "");
                        cts.Cancel();
                    });
                    foreach (var sch in msuTargets.Items.OfType<SelectedShortsDirectories>().Where(x => x.LinkedShortsDirectoryId == ShortsIndex))
                    {
                        sch.LinkedTitleId = linkedtitleid;
                    }
                    _DoTitleSelectFrm = null;
                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoOnFinishTitleSelect {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public string GetShortsDirectorySql(int index = -1)
        {
            try
            {
                return "SELECT S.ID, S.DIRECTORYNAME, S.TITLEID, S.DESCID, " +
                       "(SELECT LIST(TAGID, ',') FROM TITLETAGS " +
                       " WHERE GROUPID = S.TITLEID) AS LINKEDTITLEIDS, " +
                       " (SELECT LIST(ID,',') FROM DESCRIPTIONS " +
                       "WHERE ID = S.DESCID) AS LINKEDDESCIDS " +
                       "FROM SHORTSDIRECTORY S" +
                (index != -1 ? $" WHERE S.ID = {index} " : "");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} GetShortsDirectorySql {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "";
            }
        }
        private void CheckLinkedIds(SelectedShortsDirectories item, string newpath)
        {
            try
            {
                List<string> files = Directory.EnumerateFiles(newpath, "*.mp4",
                                                           SearchOption.AllDirectories).ToList();
                string LinkedShortsDirectoryId =
                    item.LinkedShortsDirectoryId.ToString();
                if (LinkedShortsDirectoryId is not null && LinkedShortsDirectoryId != "")
                {
                    LinkedShortsDirectoryId = "_" + LinkedShortsDirectoryId;
                }

                foreach (var filex in files.Where(filex => !filex.Contains(LinkedShortsDirectoryId)))
                {
                    string newPath = filex.Contains("_")
                                            ? filex.Substring(0, filex.IndexOf("_")) + LinkedShortsDirectoryId + Path.GetExtension(filex)
                                            : filex + LinkedShortsDirectoryId;
                    if (newpath.EndsWith(".mp4"))
                    {
                        File.Move(filex, newPath);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"CheckLinkedIds {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
    }
}
