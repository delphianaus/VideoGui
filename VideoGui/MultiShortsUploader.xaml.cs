using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using Path = System.IO.Path;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for MultiShortsUploader.xaml
    /// </summary>
    public partial class MultiShortsUploader : Window
    {
        databasehook<object> dbInit = null;
        public TitleSelectFrm DoTitleSelectFrm = null;
        public DescSelectFrm DoDescSelectFrm = null;
        string connectionStr = "", OldTarget, OldgUrl;
        ScraperModule scraperModule = null;
        public int ShortsIndex = -1;
        int SchMaxUploads = 100, LinkedId = -1, SelectedTitleId = -1;
        public bool IsClosing = false, IsClosed = false, Ready = false, IsFirstResize = false;
        private bool _isFirstResize = true;
        DispatcherTimer LocationChangedTimer = new DispatcherTimer();
        DispatcherTimer LocationChanger = new DispatcherTimer();
        public static readonly DependencyProperty Column_WidthProperty =
            DependencyProperty.Register("Column_Width",
            typeof(GridLength), typeof(MultiShortsUploader),
            new PropertyMetadata(new GridLength(363, GridUnitType.Pixel)));
        public GridLength Column_Width
        {
            get { return (GridLength)GetValue(Column_WidthProperty); }
            set { SetValue(Column_WidthProperty, value); }
        }
        public MultiShortsUploader(databasehook<object> _dbInit, OnFinishIdObj _DoOnFinished)
        {
            try
            {
                InitializeComponent();
                dbInit = _dbInit;
                Closing += (s, e) =>
                {
                    IsClosing = true;
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("MSUWidth", ActualWidth);
                    key.SetValue("MSUHeight", ActualHeight);
                    key.SetValue("MSUleft", Left);
                    key.SetValue("MSUtop", Top);
                    key?.Close();
                };
                Closed += (s, e) => { IsClosed = true; _DoOnFinished?.Invoke(this, -1); };
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string dir = FindUploadPath();
                key?.Close();
                string DirName = dir.Split(@"\").ToList().LastOrDefault();
                string sqla = "SELECT ID FROM SHORTSDIRECTORY WHERE DIRECTORYNAME = @DIRECTORYNAME";
                ShortsIndex = connectionStr.ExecuteScalar(sqla,
                    [("@DIRECTORYNAME", DirName)]).ToInt(-1);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        public string FindUploadPath()
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string shortsdir = key.GetValueStr("shortsdirectory", @"D:\shorts\");
                string uploaddir = key.GetValueStr("UploadPath", "");
                key?.Close();
                if (!Path.Exists(uploaddir))
                {
                    string NewPath = uploaddir.Split('\\').LastOrDefault();
                    List<string> dirs = Directory.EnumerateDirectories(shortsdir).
                        ToList().Where(dir => !dir.ToLower().EndsWith(NewPath.ToLower())).ToList();
                    if (dirs.Count > 0)
                    {
                        return dirs.FirstOrDefault();
                    }
                    string uploadsNewPath = Path.Combine(shortsdir, NewPath);
                    if (Path.Exists(uploadsNewPath))
                    {
                        uploaddir = uploadsNewPath;
                    }
                }
                return uploaddir;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"FindUploadPath {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return "";
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                connectionStr =
                      dbInit?.Invoke(this, new CustomParams_GetConnectionString())
                      is string conn ? conn : "";
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = FindUploadPath();
                string uploadsnumber = key.GetValueStr("UploadNumber", "5");
                string MaxUploads = key.GetValueStr("MaxUploads", "100");
                txtMaxUpload.Text = uploadsnumber.NotNullOrEmpty() ? uploadsnumber : txtMaxUpload.Text;
                txtTotalUploads.Text = MaxUploads.NotNullOrEmpty() ? MaxUploads : txtTotalUploads.Text;
                dbInit?.Invoke(this, new CustomParams_Initialize());
                key?.Close();
                Ready = false;
                LocationChanger.Interval = TimeSpan.FromMilliseconds(10);
                LocationChanger.Tick += LocationChanger_Tick;
                LocationChanger.Start();
                Column_Width = new GridLength(393, GridUnitType.Pixel);
                LocationChanged += (s, e) =>
                {
                    LocationChangedTimer.Stop();
                    LocationChangedTimer.Interval = TimeSpan.FromSeconds(3);
                    LocationChangedTimer.Tick += (s1, e1) =>
                    {
                        LocationChangedTimer.Stop();
                        RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        key2?.SetValue("MSUleft", Left);
                        key2?.SetValue("MSUtop", Top);
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
        private void LocationChanger_Tick(object? sender, EventArgs e)
        {
            try
            {
                LocationChanger.Stop();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                var _width = key.GetValue("MSUWidth", ActualWidth).ToDouble();
                var _height = key.GetValue("MSUHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("MSUleft", Left).ToDouble();
                var _top = key.GetValue("MSUtop", Top).ToDouble();
                key?.Close();
                Left = (Left != _left && _left != 0) ? _left : Left;
                Top = (Top != _top && _top != 0) ? _top : Top;
                Width = (ActualWidth != _width && _width != 0) ? _width : Width;
                Height = (ActualHeight != _height && _height != 0) ? _height : Height;
                MainGrid.Width = Width;
                MainGrid.Height = Height;
                LoadingPanel.Height = Height;
                LoadingPanel.Width = Width;
                MainScroller.Width = Width;
                MainScroller.Height = Height;
                MainContent.Width = Width;
                MainContent.Height = Height;
                msuShorts.Width = Width - 15;
                msuSchedules.Width = msuShorts.Width;
                msuShorts.Height = 191;
                msuSchedules.Height = Height - (msuShorts.Height) - 77;
                var msuShortsBottom = Canvas.GetBottom(msuShorts);
                Canvas.SetTop(msuSchedules, msuShorts.Height);
                Canvas.SetTop(BtnClose, Height - 68);
                Canvas.SetTop(BtnRunUploaders, Height - 68);
                Canvas.SetLeft(BtnClose, Width - BtnClose.Width - 25);
                Canvas.SetTop(txtTotalUploads, Height - 66);
                Canvas.SetTop(txtMaxUpload, Height - 66);
                Canvas.SetTop(lblupload, Height - 68);
                Canvas.SetTop(lblmax, Height - 68);
                Canvas.SetTop(lblupload, Height - 68);
                Canvas.SetTop(lblUploaded, Height - 68);
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
                    if (e.HeightChanged)
                    {
                        MainGrid.Height = e.NewSize.Height;
                        LoadingPanel.Height = e.NewSize.Height;
                        MainScroller.Height = e.NewSize.Height;
                        MainContent.Height = e.NewSize.Height;
                        msuSchedules.Height = Height - (msuShorts.Height) - 77;
                        var msuShortsBottom = Canvas.GetBottom(msuShorts);
                        Canvas.SetTop(msuSchedules, msuShorts.Height);
                        Canvas.SetTop(BtnClose, e.NewSize.Height - 68);
                        Canvas.SetTop(BtnRunUploaders, e.NewSize.Height - 68);
                        Canvas.SetTop(txtTotalUploads, e.NewSize.Height - 66);
                        Canvas.SetTop(txtMaxUpload, e.NewSize.Height - 66);
                        Canvas.SetTop(lblupload, e.NewSize.Height - 68);
                        Canvas.SetTop(lblmax, e.NewSize.Height - 68);
                        Canvas.SetTop(lblUploaded, e.NewSize.Height - 68);

                    }
                    if (e.WidthChanged)
                    {
                        MainScroller.Width = e.NewSize.Width;
                        MainGrid.Width = e.NewSize.Width;
                        MainContent.Width = e.NewSize.Width;
                        msuShorts.Width = e.NewSize.Width - 15;
                        msuSchedules.Width = msuShorts.Width;
                        Canvas.SetLeft(BtnClose, e.NewSize.Width - BtnClose.Width - 25);
                        Column_Width = new GridLength(e.NewSize.Width - 135, GridUnitType.Pixel);
             
                    }
                    if (e.HeightChanged || e.WidthChanged)
                    {
                        RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        key?.SetValue("MSUWidth", ActualWidth);
                        key?.SetValue("MSUHeight", ActualHeight);
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
        private void DoDescSelectCreate(int DescId = -1)
        {
            try
            {
                if (DoDescSelectFrm is not null && DoDescSelectFrm.IsActive)
                {
                    DoDescSelectFrm.Close();
                    DoDescSelectFrm = null;
                }
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string dir = FindUploadPath();
                key?.Close();
                string DirName = dir.Split(@"\").ToList().LastOrDefault();
                var r = dbInit?.Invoke(this, new CustomParams_GetDirectory(DirName));
                if (r is not null)
                {
                    ShortsIndex = r.ToInt(-1);
                    var rt = dbInit?.Invoke(this, new CustomParams_GetCurrentDescId(ShortsIndex));
                    DescId = (rt is not null) ? rt.ToInt(-1) : DescId;
                }
                DoDescSelectFrm = new DescSelectFrm(OnSelectFormClose, dbInit, true, DescId);
                Hide();
                DoDescSelectFrm.Show();
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
                    bool Updated = false;
                    string Desc = frm.txtDesc.Text;
                    string Dir = frm.txtDescName.Text;
                    var r = dbInit?.Invoke(this, new CustomParams_DescUpdate(Dir, Desc));
                    Updated = (r is IdhasUpdated idhasUpdated) ? idhasUpdated.hasUpdated : Updated;
                    if (Updated)
                    {
                        string linkeddescid = "";
                        string sql = GetShortsDirectorySql(frm.Id);
                        CancellationTokenSource cts = new CancellationTokenSource(10000);
                        connectionStr.ExecuteReader(sql, cts, (FbDataReader r) =>
                        {
                            linkeddescid = (r["LINKEDDESCIDS"] is string did ? did : "");
                            cts.Cancel();
                        });
                        foreach (var sch in msuSchedules.Items.OfType<SelectedShortsDirectories>().Where(x => x.LinkedShortsDirectoryId == ShortsIndex))
                        {
                            sch.LinkedDescId = linkeddescid;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} OnSelectFormClose {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public string GetShortsDirectorySql(int index = -1)
        {
            try
            {
                return "SELECT S.ID, S.DIRECTORYNAME, S.TITLEID, S.DESCID, " +
                       "(SELECT LIST(TAGID, ',') FROM TITLETAGS " +
                       " WHERE GROUPID = S.ID) AS LINKEDTITLEIDS, " +
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
        private void Desc_ToggleButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleButton t && t.DataContext is SelectedShortsDirectories info)
                {
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
                    LinkedId = info.LinkedShortsDirectoryId;
                    dbInit?.Invoke(this, new CustomParams_SetIndex(LinkedId));
                    DoTitleSelectCreate(info.TitleId);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Title_ToggleButtonClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public void DoTitleSelectCreate(int TitleId = -1)
        {
            try
            {
                SelectedTitleId = TitleId;
                var _DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, dbInit, true, TitleId);
                Hide();
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
                if (DoTitleSelectFrm is not null && DoTitleSelectFrm.IsClosed)
                {
                    bool found = false;
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    string dir = FindUploadPath();
                    key?.Close();
                    string DirName = dir.Split(@"\").ToList().LastOrDefault();
                    var r = dbInit?.Invoke(this, new CustomParams_LookUpId(DirName));
                    ShortsIndex = (r is not null) ? r.ToInt(-1) : ShortsIndex;
                    string sql = "";
                    if ((SelectedTitleId != DoTitleSelectFrm.TitleId))
                    {
                        sql = "UPDATE SHORTSDIRECTORY SET TITLEID = @TITLEID WHERE ID = @ID";
                        connectionStr.ExecuteNonQuery(sql, [("@ID", ShortsIndex), ("@TITLEID", DoTitleSelectFrm.TitleId)]);
                    }
                    string linkedtitleid = "";
                    sql = GetShortsDirectorySql(ShortsIndex);
                    CancellationTokenSource cts = new CancellationTokenSource();
                    connectionStr.ExecuteReader(sql, cts, (FbDataReader r) =>
                    {
                        linkedtitleid = (r["LINKEDTITLEIDS"] is string tidt ? tidt : "");
                        cts.Cancel();
                    });
                    foreach (var sch in msuSchedules.Items.OfType<SelectedShortsDirectories>().Where(x => x.LinkedShortsDirectoryId == ShortsIndex))
                    {
                        sch.LinkedTitleId = linkedtitleid;
                    }
                    DoTitleSelectFrm = null;
                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoOnFinishTitleSelect {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void BtnRunUploaders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string newdir = "", UploadFile = "", PathToCheck = "";
                int LinkedId = -1;
                bool Valid = true, Processed = false, IsProcessing = true;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = FindUploadPath();
                string shortsdir = key.GetValueStr("shortsdirectory", "");
                CancellationTokenSource cts = new();
                string SQLB = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 100;";
               
                while (IsProcessing)
                {
                    foreach (var sch in msuSchedules.Items.OfType<SelectedShortsDirectories>().Where(x => x.IsActive && x.NumberOfShorts > 0))
                    {
                        IsProcessing = false;
                        newdir = sch.DirectoryName;
                        LinkedId = sch.LinkedShortsDirectoryId;
                        PathToCheck = Path.Combine(shortsdir, newdir);
                        if (Directory.Exists(PathToCheck))
                        {
                            int cnt_files = Directory.EnumerateFiles(PathToCheck, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                            if (cnt_files > 0)
                            {
                                rootfolder = PathToCheck;
                                key.SetValue("UploadPath", rootfolder);
                                LinkedId = sch.LinkedShortsDirectoryId;
                                break;
                            }
                            else
                            {
                                sch.IsActive = false;
                                sch.NumberOfShorts = 0;
                                LinkedId = -1;
                            }
                            //var Idx = dbInit?.Invoke(this, new CustomParams_GetDirectory(sch.DirectoryName));
                            //sch.Id = (Idx is int _id && _id != -1) ? _id : sch.Id;
                        }
                    }
                    if (PathToCheck == "")
                    {
                        LinkedId = -1;
                        var item = msuSchedules.Items.OfType<SelectedShortsDirectories>().FirstOrDefault();
                        item.IsActive = (item is not null) ? true : item.IsActive;
                    }
                }
                key?.Close();
                if (Directory.Exists(rootfolder))
                {
                    bool reload = false;
                    List<string> files = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList();
                    foreach (var file in files.Where(f => !f.Contains("_")))
                    {
                        string filename = Path.GetFileNameWithoutExtension(file);
                        string filePath = Path.GetDirectoryName(file);
                        string ext = Path.GetExtension(file);
                        if (filename != "" && filePath != "" && ext != "")
                        {
                            string newfile = Path.Combine(filePath, filename + $"_{LinkedId}" + ext);
                            File.Move(file, newfile);
                            reload = true;
                        }
                    }
                    if (reload)
                    {
                        files.Clear();
                        files = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList();
                    }
                    string firstfile = files.FirstOrDefault();
                    if (LinkedId != -1)
                    {
                        ShortsIndex = LinkedId;
                        Valid = true;
                    }
                    if (firstfile is not null && File.Exists(firstfile) && LinkedId != -1)
                    {
                        string fid = Path.GetFileNameWithoutExtension(firstfile).Split('_').LastOrDefault();
                        string DirName = rootfolder.Split(@"\").ToList().LastOrDefault();
                        var r = dbInit?.Invoke(this, new CustomParams_LookUpId(DirName));
                        ShortsIndex = (r is not null) ? r.ToInt(-1) : ShortsIndex;
                        if (ShortsIndex == -1)
                        {
                            var r1r = dbInit?.Invoke(this, new CustomParams_RematchedUpdate(ShortsIndex, DirName));
                            if (r1r is bool res) Valid = res;
                        }
                        else Valid = true;
                    }
                }
                if (Valid)
                {
                    WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                    string gUrl = webAddressBuilder.Dashboard().Address;
                    int Maxuploads = (txtTotalUploads.Text != "") ? txtTotalUploads.Text.ToInt(100) : 100;
                    int UploadsPerSlot = (txtMaxUpload.Text != "") ? txtMaxUpload.Text.ToInt(5) : 5;
                    var _scraperModule = new ScraperModule(dbInit, doOnFinish, gUrl, Maxuploads, UploadsPerSlot, 0, true);
                    _scraperModule.ShowActivated = true;
                    Hide();
                    _scraperModule.Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"BtnRunUploaders_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void doOnFinish(object sender, int id)
        {
            try
            {
                if (sender is ScraperModule sa)
                {
                    WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                    string gUrl = webAddressBuilder.Dashboard().Address;
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    string rootfolder = FindUploadPath();
                    string BaseDir = key.GetValueStr("shortsdirectory", @"D:\shorts");
                    key?.Close();
                    int cnt = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                    if (!sa.KilledUploads && sa.TotalScheduled > 0)
                    {
                        List<string> filesdone = new List<string>();
                        bool Exc = sa.Exceeded;
                        filesdone.AddRange(sa.ScheduledOk);
                        int Uploaded = sa.TotalScheduled;
                        int shortsleft = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                        foreach (var rp in msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().Where(x => x.IsActive))
                        {
                            break;
                            rp.NumberOfShorts = cnt;
                            if (cnt == 0) rp.IsActive = false;
                            string DirPath = rp.DirectoryName;
                            string newpath = Path.Combine(BaseDir, DirPath);
                            if (Path.Exists(newpath))
                            {
                                shortsleft = Directory.EnumerateFiles(newpath, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                                rp.NumberOfShorts = shortsleft;
                                string sql = "UPDATE MULTISHORTSINFO SET NUMBEROFSHORTS = @NUMBEROFSHORTS WHERE LINKEDSHORTSDIRECTORYID = @LINKEDSHORTSDIRECTORYID";
                                connectionStr.ExecuteNonQuery(sql,
                                    [("@NUMBEROFSHORTS", shortsleft),
                                ("@LINKEDSHORTSDIRECTORYID", rp.LinkedShortsDirectoryId)]);
                            }
                        }
                        bool remove = !msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().Any(x => x.NumberOfShorts == 0);
                        if (remove) dbInit?.Invoke(this, new CustomParams_RemoveMulitShortsInfoById(-1));
                        if (cnt == 0)
                        {
                            int acnt = 0;
                            foreach (var rp in msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().Where(x => x.IsActive && x.NumberOfShorts > 0))
                            {
                                string newpaths = Path.Combine(BaseDir, rp.DirectoryName);
                                shortsleft = Directory.EnumerateFiles(newpaths, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                                if (shortsleft != rp.NumberOfShorts)
                                {
                                    rp.NumberOfShorts = shortsleft;
                                    if (shortsleft == 0)
                                    {
                                        rp.IsActive = false;
                                        string sql = "UPDATE MULTISHORTSINFO SET " +
                                            "NUMBEROFSHORTS = @NUMBEROFSHORTS, ACTIVE = @ACTIVE " +
                                            "WHERE LINKEDSHORTSDIRECTORYID = @LINKEDSHORTSDIRECTORYID";
                                        connectionStr.ExecuteNonQuery(sql,
                                            [("@NUMBEROFSHORTS", shortsleft),
                                        ("@ACTIVE", false),
                                        ("@LINKEDSHORTSDIRECTORYID", rp.LinkedShortsDirectoryId)]);
                                        dbInit?.Invoke(this, new
                                            CustomParams_RemoveMulitShortsInfoById(rp.LinkedShortsDirectoryId));
                                    }
                                }
                                else if (rp.NumberOfShorts > 0) acnt++;
                            }
                            if (acnt == 0)
                            {
                                var item = msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().FirstOrDefault();
                                if (item is not null)
                                {
                                    item.IsActive = true;
                                    string newpath = Path.Combine(BaseDir, item.DirectoryName);
                                    if (Path.Exists(newpath))
                                    {
                                        shortsleft = Directory.EnumerateFiles(newpath, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                                        item.NumberOfShorts = shortsleft;
                                        key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                        key.SetValue("UploadPath", newpath);
                                        key?.Close();
                                        CheckLinkedIds(item, newpath);
                                        var Idx = dbInit?.Invoke(this, new CustomParams_GetDirectory(item.DirectoryName));
                                        item.Id = (Idx is int _id && item.Id != _id) ? _id : item.Id;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().Where(x => x.IsActive && x.NumberOfShorts > 0))
                                {
                                    string newpath = Path.Combine(BaseDir, item.DirectoryName);
                                    if (Path.Exists(newpath))
                                    {
                                        shortsleft = Directory.EnumerateFiles(newpath, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                                        if (shortsleft == 0)
                                        {
                                            item.IsActive = false;
                                            continue;
                                        }
                                        item.NumberOfShorts = shortsleft;
                                        key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                        key.SetValue("UploadPath", newpath);
                                        key?.Close();
                                        var Idx = dbInit?.Invoke(this, new CustomParams_GetDirectory(item.DirectoryName));
                                        item.Id = (Idx is int _id && item.Id != _id) ? _id : item.Id;
                                        CheckLinkedIds(item, newpath);
                                    }
                                }
                            }
                        }
                        if (!Exc && shortsleft > 0 && Uploaded < txtTotalUploads.Text.ToInt())
                        {
                            int Maxuploads = (txtTotalUploads.Text != "") ? txtTotalUploads.Text.ToInt(100) : 100;
                            int UploadsPerSlot = (txtMaxUpload.Text != "") ? txtMaxUpload.Text.ToInt(5) : 5;
                            var sscraperModule = new ScraperModule(dbInit, doOnFinish, gUrl, Maxuploads, UploadsPerSlot, 0, false);
                            sscraperModule.ShowActivated = true;
                            sscraperModule.ScheduledOk.AddRange(filesdone);
                            Hide();
                            Process[] webView2Processes = Process.GetProcessesByName("MicrosoftEdgeWebview2");
                            foreach (Process process in webView2Processes)
                            {
                                process.Kill();
                            }
                            sscraperModule.Show();
                            return;
                        }
                        else
                        {
                            if (Uploaded > 0)
                            {
                                foreach (var rp in msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().Where(x => x.IsActive))
                                {
                                    rp.NumberOfShorts = shortsleft;
                                    int linkedId = rp.LinkedShortsDirectoryId;
                                    CancellationTokenSource ctscc = new CancellationTokenSource();
                                    string SQLB = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 100;";
                                    connectionStr.ExecuteReader(SQLB, ctscc, (FbDataReader r) =>
                                    {
                                        string UploadFile = (r["UPLOADFILE"] is string f) ? f : "";
                                        int idx = UploadFile.Split('_').LastOrDefault().ToInt(-1);
                                        idx = (dbInit?.Invoke(this, new CustomParams_RematchedLookup(idx)) is int trs) ? trs : idx;
                                        if (idx == linkedId)
                                        {
                                            DateTime dtr = r["UPLOAD_DATE"] as DateTime? ?? DateTime.Now.AddYears(-200);
                                            TimeSpan ttr = r["UPLOAD_TIME"] as TimeSpan? ?? TimeSpan.Zero;
                                            if (dtr.Year > 2000)
                                            {
                                                rp.LastUploadedDateFile = dtr.AtTime(TimeOnly.FromTimeSpan(ttr));
                                            }
                                        }
                                        ctscc.Cancel();
                                    });
                                }
                                Nullable<DateTime> startdate = DateTime.Now, enddate = DateTime.Now.AddHours(10);
                                List<ListScheduleItems> listSchedules2 = new();
                                int _eventid = 0;
                                SchMaxUploads = 100;
                                ShowScraper(startdate, enddate, listSchedules2, SchMaxUploads, _eventid);
                            }
                            else Show();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"doOnFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
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
                    File.Move(filex, newPath);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"CheckLinkedIds {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public void ShowScraper(Nullable<DateTime> startdate = null, Nullable<DateTime> enddate = null,
            List<ListScheduleItems> _listSchedules = null, int SchMaxUploads = 100, int _eventid = 0)
        {
            try
            {
                if (scraperModule is not null)
                {
                    if (!scraperModule.IsClosed && !scraperModule.IsClosing)
                    {
                        scraperModule.Close();
                    }
                    while (true)
                    {
                        if (!scraperModule.IsClosed && scraperModule.IsClosing)
                        {
                            Thread.Sleep(250);
                        }
                        if (scraperModule.IsClosed) break;
                    }
                    scraperModule = null;
                }
                WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                string gUrl = webAddressBuilder.Dashboard().Address;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string uploadsnumber = key.GetValueStr("UploadNumber", "5");
                string MaxUploads = key.GetValueStr("MaxUploads", "100");
                key?.Close();
                string TargetUrl = webAddressBuilder.AddFilterByDraftShorts().GetHTML();
                OldgUrl = gUrl;
                OldTarget = TargetUrl;
                scraperModule = new ScraperModule(dbInit, FinishScraper, gUrl, TargetUrl, 0);
                Hide();
                scraperModule.ShowActivated = true;
                scraperModule.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ShowScraper {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void FinishScraper(object sender, int id)
        {
            try
            {
                Show();
                Task.Run(() =>
                {
                    if (sender is ScraperModule scraperModulej)
                    {
                        bool IsTimeOut = scraperModulej.TimedOutClose;
                        while (true)
                        {
                            if (!scraperModulej.IsClosed && scraperModulej.IsClosing)
                            {
                                Thread.Sleep(250);
                            }
                            if (scraperModulej.IsClosed) break;
                        }
                        if (IsTimeOut)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                var gscraperModule = new ScraperModule(dbInit, FinishScraper, OldgUrl, OldTarget, 0);
                                Hide();
                                gscraperModule.ShowActivated = true;
                                gscraperModule.Show();
                            });
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"FinishScraper {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void txtMaxUpload_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("UploadNumber", txtMaxUpload.Text);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"txtMaxUpload_KeyDown {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private void mnuRemoveSchedule(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource is MenuItem m && m.DataContext is SelectedShortsDirectories rp)
                {
                    dbInit?.Invoke(this, new CustomParams_RemoveSchedule(rp.Id));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuRemoveSchedule {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private void mnuMakeActive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource is MenuItem m &&
                    m.DataContext is SelectedShortsDirectories rp)// && !rp.IsActive)
                {
                    dbInit?.Invoke(this, new CustomParams_SetActive(rp.LinkedShortsDirectoryId));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuMakeActive_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private void txtTotalUploads_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("MaxUploads", txtTotalUploads.Text);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"txtMaxUpload_KeyDown {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private void txtMaxUpload_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string uploadnmb = key.GetValueStr("UploadNumber", "5");
                if (uploadnmb is string str && str != txtMaxUpload.Text)
                {
                    key.SetValue("UploadNumber", txtMaxUpload.Text);
                }
                key?.Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"txtMaxUpload_LostFocus {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        private void mnuRemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource is MenuItem mnu && mnu.DataContext is SelectedShortsDirectories info)
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
