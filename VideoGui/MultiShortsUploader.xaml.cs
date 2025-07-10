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
        string connectionStr = "";
        ScraperModule scraperModule = null;
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
                key?.Close();
                //bool found = false;
                txtMaxUpload.Text = (uploadsnumber != "") ? uploadsnumber : txtMaxUpload.Text;
                txtTotalUploads.Text = (MaxUploads != "") ? MaxUploads : txtTotalUploads.Text;

                dbInit?.Invoke(this, new CustomParams_Initialize());
                Ready = true;
                key = "SOFTWARE\\Scraper".OpenSubKey(Registry.CurrentUser);
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

        /*private T FindVisualChild<T>(DependencyObject obj, string name) where T : FrameworkElement
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
        }*/

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
                string dir =  FindUploadPath();
                key?.Close();
                string DirName = dir.Split(@"\").ToList().LastOrDefault();
                var r = dbInit?.Invoke(this, new CustomParams_GetDirectory(DirName));
                if (r is not null)
                {
                    ShortsIndex = r.ToInt(-1);
                    var rt = dbInit?.Invoke(this, new CustomParams_GetCurrentDescId(ShortsIndex));
                    if (rt is not null)
                    {
                        DescId = rt.ToInt(-1);
                    }
                }
                DoDescSelectFrm = new DescSelectFrm(OnSelectFormClose, dbInit, true, DescId);
                Hide();
                //DoDescSelectFrm.Id = DescId;
                DoDescSelectFrm.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoDescSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void OnSelectFormClose()
        {
            try
            {
                Show();
                if (DoDescSelectFrm is not null)
                {
                    bool Updated = false;
                    int descid = -1;
                    string Desc = DoDescSelectFrm.txtDesc.Text;
                    string Dir = DoDescSelectFrm.txtDescName.Text;
                    var r = dbInit?.Invoke(this, new CustomParams_DescUpdate(Dir, Desc));
                    if (r is IdhasUpdated idhasUpdated)
                    {
                        Updated = idhasUpdated.hasUpdated;
                        descid = idhasUpdated.id;
                    }
                    if (Updated)
                    {
                        string linkeddescid = "";
                        string sql = GetShortsDirectorySql(DoDescSelectFrm.Id);
                        connectionStr.ExecuteReader(sql, (FbDataReader r) =>
                        {
                            linkeddescid = (r["LINKEDDESCIDS"] is string did ? did : "");
                        });
                        foreach (var sch in msuSchedules.Items.OfType<SelectedShortsDirectories>().Where(x => x.LinkedShortsDirectoryId == ShortsIndex))
                        {
                            sch.LinkedDescId = linkeddescid;
                        }
                    }
                    //btnEditDesc.IsChecked = (descid != -1 && linkeddescid != "");

                    if (DoDescSelectFrm.IsClosed)
                    {
                        DoDescSelectFrm = null;
                    }
                }
                Show();
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

        int LinkedId = -1;
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
        int SelectedTitleId = -1;
        public void DoTitleSelectCreate(int TitleId = -1)
        {
            try
            {

                SelectedTitleId = TitleId;
                if (DoTitleSelectFrm is not null)
                {
                    if (!DoTitleSelectFrm.IsClosing && !DoTitleSelectFrm.IsClosed)
                    {
                        DoTitleSelectFrm.Close();
                        DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, dbInit, true, TitleId);
                        Hide();
                        DoTitleSelectFrm.Show();
                    }
                }
                else
                {
                    DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, dbInit, true, TitleId);
                    Hide();
                    DoTitleSelectFrm.Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoTitleSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        int ShortsIndex = -1;

        private void DoOnFinishTitleSelect()
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
                    if (r is not null)
                    {
                        ShortsIndex = r.ToInt(-1);
                    }
                    string sql = "";
                    if ((SelectedTitleId != DoTitleSelectFrm.TitleId))
                    {
                        sql = "UPDATE SHORTSDIRECTORY SET TITLEID = @TITLEID WHERE ID = @ID";
                        connectionStr.ExecuteNonQuery(sql, [("@ID", ShortsIndex), ("@TITLEID", DoTitleSelectFrm.TitleId)]);
                    }
                    string linkedtitleid = "";
                    sql = GetShortsDirectorySql(ShortsIndex);
                    connectionStr.ExecuteReader(sql, (FbDataReader r) =>
                    {
                        linkedtitleid = (r["LINKEDTITLEIDS"] is string tidt ? tidt : "");
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

        int SchMaxUploads = 100;
        private void BtnRunUploaders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string newdir = "", UploadFile = "";
                int LinkedId = -1;

                bool Valid = true, Processed = false;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = FindUploadPath();
                string shortsdir = key.GetValueStr("shortsdirectory", "");
                string SQLB = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 100;";
                connectionStr.ExecuteReader(SQLB, (FbDataReader r) =>
                {
                    if (Processed) return;
                    UploadFile = (r["UPLOADFILE"] is string f) ? f : "";
                    LinkedId = (UploadFile.Contains("_")) ? UploadFile.Split('_').LastOrDefault().ToInt(-1) : 93;
                    if (LinkedId != -1) Processed = true;
                });

                bool IsProcessing = true;
                string PathToCheck = "";
                while (IsProcessing)
                {
                    foreach (var sch in msuSchedules.Items.OfType<SelectedShortsDirectories>().Where(x => x.IsActive &&
                    (x.LinkedShortsDirectoryId == LinkedId || LinkedId == -1)))
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
                                if (LinkedId == -1)
                                {
                                    LinkedId = sch.LinkedShortsDirectoryId;
                                }
                                break;
                            }
                            else
                            {
                                sch.IsActive = false;
                                sch.NumberOfShorts = 0;
                                LinkedId = -1;
                            }

                            var Idx = dbInit?.Invoke(this, new CustomParams_GetDirectory(sch.DirectoryName));
                            if (Idx is int _id)
                            {
                                if (sch.Id != _id)
                                {
                                    sch.Id = _id;
                                }
                            }
                        }


                    }
                    

                    if (PathToCheck == "")
                    {   
                        LinkedId = -1;
                        var item = msuSchedules.Items.OfType<SelectedShortsDirectories>().FirstOrDefault();
                        if (item is not null)
                        {
                            item.IsActive = true;
                        }
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
                    if (scraperModule is not null && !scraperModule.IsClosed)
                    {
                        if (scraperModule.IsClosing) scraperModule.Close();
                        while (!scraperModule.IsClosing)
                        {
                            Thread.Sleep(100);
                        }
                        scraperModule.Close();
                        scraperModule = null;
                    }
                    if (scraperModule is not null && scraperModule.IsClosed)
                    {
                        scraperModule = null;
                    }

                    if (scraperModule is null)
                    {
                        WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                        string gUrl = webAddressBuilder.Dashboard().Address;
                        int Maxuploads = (txtTotalUploads.Text != "") ? txtTotalUploads.Text.ToInt(100) : 100;
                        int UploadsPerSlot = (txtMaxUpload.Text != "") ? txtMaxUpload.Text.ToInt(5) : 5;

                        scraperModule = new ScraperModule(dbInit, doOnFinish, gUrl, Maxuploads, UploadsPerSlot, 0, true);

                        scraperModule.ShowActivated = true;
                        Hide();
                        scraperModule.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"BtnRunUploaders_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }

        }
        private void doOnFinish(int id)
        {
            try
            {
                WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                string gUrl = webAddressBuilder.Dashboard().Address;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = FindUploadPath();
                string BaseDIr = key.GetValueStr("shortsdirectory", @"D:\shorts");
                key?.Close();
                // update shorts left.

                int cnt = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                if (scraperModule != null && !scraperModule.KilledUploads)
                {
                    List<string> filesdone = new List<string>();
                    bool Exc = scraperModule.Exceeded;
                    filesdone.AddRange(scraperModule.ScheduledOk);
                    int Uploaded = scraperModule.TotalScheduled;
                    int shortsleft = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                    foreach (var rp in msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().Where(x => x.IsActive))
                    {
                        rp.NumberOfShorts = cnt;
                        if (cnt == 0) rp.IsActive = false;
                        break;
                    }

                    for (int i = msuSchedules.Items.Count - 1; i >= 0; i--)
                    {
                        if (msuSchedules.Items[i] is SelectedShortsDirectories rp && rp.NumberOfShorts == 0)
                        {
                            msuSchedules.Items.RemoveAt(i);
                        }
                    }
                    if (cnt == 0)
                    {
                        int acnt = 0;
                        foreach (var rp in msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().Where(x => x.IsActive))
                        {
                            acnt += 1;
                        }
                        if (acnt == 0)
                        {
                            var item = msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().FirstOrDefault();
                            if (item is not null)
                            {
                                item.IsActive = true;
                                string newpath = Path.Combine(BaseDIr, item.DirectoryName);
                                if (Path.Exists(newpath))
                                {
                                    shortsleft = Directory.EnumerateFiles(newpath, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                                    item.NumberOfShorts = shortsleft;
                                    key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                    key.SetValue("UploadPath", newpath);
                                    key?.Close();
                                    var Idx = dbInit?.Invoke(this,new CustomParams_GetDirectory(item.DirectoryName));
                                    if (Idx is int _id)
                                    {
                                        if (item.Id != _id)
                                        {
                                            item.Id = _id;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var item = msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().Where(x => x.IsActive).FirstOrDefault();
                            if (item is not null)
                            {
                                string newpath = Path.Combine(BaseDIr, item.DirectoryName);
                                if (Path.Exists(newpath))
                                {
                                    shortsleft = Directory.EnumerateFiles(newpath, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                                    item.NumberOfShorts = shortsleft;
                                    key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                    key.SetValue("UploadPath", newpath);
                                    key?.Close();
                                    var Idx = dbInit?.Invoke(this, new CustomParams_GetDirectory(item.DirectoryName));
                                    if (Idx is int _id)
                                    {
                                        if (item.Id != _id)
                                        {
                                            item.Id = _id;
                                        }
                                    }

                                }
                            }
                        }
                    }
                    if (!Exc && shortsleft > 0 && Uploaded < txtTotalUploads.Text.ToInt())
                    {
                        int Maxuploads = (txtTotalUploads.Text != "") ? txtTotalUploads.Text.ToInt(100) : 100;
                        int UploadsPerSlot = (txtMaxUpload.Text != "") ? txtMaxUpload.Text.ToInt(5) : 5;
                        scraperModule = new ScraperModule(dbInit, doOnFinish, gUrl, Maxuploads, UploadsPerSlot, 0, false);
                        scraperModule.ShowActivated = true;
                        scraperModule.ScheduledOk.AddRange(filesdone);
                        Hide();
                        Process[] webView2Processes = Process.GetProcessesByName("MicrosoftEdgeWebview2");
                        foreach (Process process in webView2Processes)
                        {
                            process.Kill();
                        }
                        scraperModule.Show();
                        return;
                    }
                    else
                    {
                        Show ();
                        bool Processed = false;
                        int ucnt = Uploaded;
                        foreach (var rp in msuSchedules.ItemsSource.OfType<SelectedShortsDirectories>().Where(x => x.IsActive))
                        {
                            rp.NumberOfShorts = shortsleft;
                            int linkedId = rp.LinkedShortsDirectoryId;
                            string SQLB = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 100;";
                            connectionStr.ExecuteReader(SQLB, (FbDataReader r) =>
                            {
                                if (Processed) return;
                                string UploadFile = (r["UPLOADFILE"] is string f) ? f : "";
                                int idx = UploadFile.Split('_').LastOrDefault().ToInt(-1);
                                if (dbInit?.Invoke(this, new CustomParams_RematchedLookup(idx)) is int trs)
                                {
                                    idx = trs;
                                }

                                if (idx == linkedId)
                                {
                                    DateTime dtr = (r["UPLOAD_DATE"] is DateTime dt) ? dt : DateTime.Now.AddYears(-200);
                                    TimeSpan ttr = (r["UPLOAD_TIME"] is TimeSpan ts) ? ts : TimeSpan.Zero;
                                    if (dtr.Year > 2000)
                                    {
                                        rp.LastUploadedDateFile = dtr.AtTime(TimeOnly.FromTimeSpan(ttr));
                                    }
                                }
                            });
                        }
                        Task.Run(() =>
                        {
                            var cts = new CancellationTokenSource();


                            if (ucnt > 0)
                            {
                                Hide();
                                Dispatcher.Invoke(() =>
                                {
                                    Nullable<DateTime> startdate = DateTime.Now, enddate = DateTime.Now.AddHours(10);
                                    List<ListScheduleItems> listSchedules2 = new();
                                    int _eventid = 0;
                                    SchMaxUploads = 100;
                                    ShowScraper(startdate, enddate, listSchedules2, SchMaxUploads, _eventid);
                                });
                            }
                        });
                    }

                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"doOnFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
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
                int MaxShorts = MaxUploads.ToInt(80);
                int MaxPerSlot = uploadsnumber.ToInt(100);

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
        private void FinishScraper(int id)
        {
            try
            {
                Show();

                Task.Run(() =>
                {
                    bool IsTimeOut = (scraperModule is ScraperModule sls) ? sls.TimedOutClose : false;
                    while (true)
                    {
                        if (!scraperModule.IsClosed && scraperModule.IsClosing)
                        {
                            Thread.Sleep(250);
                        }
                        if (scraperModule.IsClosed) break;
                    }
                    scraperModule = null;
                    if (IsTimeOut)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            scraperModule = new ScraperModule(dbInit, FinishScraper, OldgUrl, OldTarget, 0);
                            Hide();
                            scraperModule.ShowActivated = true;
                            scraperModule.Show();
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"FinishScraper {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        string OldTarget, OldgUrl;
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
