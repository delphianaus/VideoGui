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
                string dir = key.GetValueStr("UploadPath", @"D:\shorts");
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                connectionStr =
                      dbInit?.Invoke(this, new CustomParams_GetConnectionString())
                      is string conn ? conn : "";
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
                string dir = key.GetValueStr("UploadPath", @"D:\shorts");
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


        private void Desc_ToggleButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleButton t && t.DataContext is SelectedShortsDirectories info)
                {
                    LinkedId = info.LinkedShortsDirectoryId;
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
                    string dir = key.GetValueStr("UploadPath", @"D:\shorts");
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
                bool Valid = true;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("UploadPath", "");
                key?.Close();
                if (Directory.Exists(rootfolder))
                {
                    List<string> files = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList();
                    string firstfile = files.FirstOrDefault();
                    if (firstfile is not null && File.Exists(firstfile))
                    {
                        string fid = Path.GetFileNameWithoutExtension(firstfile).Split('_').LastOrDefault();
                        string DirName = rootfolder.Split(@"\").ToList().LastOrDefault();
                        var r = dbInit?.Invoke(this, new CustomParams_LookUpId(DirName));
                        ShortsIndex = (r is not null) ? r.ToInt(-1) : ShortsIndex;
                        var r1r = dbInit?.Invoke(this, new CustomParams_RematchedUpdate(ShortsIndex, DirName));
                        if (r1r is bool res) Valid = res;
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
                string rootfolder = key.GetValueStr("UploadPath", @"D:\shorts");
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
                    foreach (var f in msuSchedules.ItemsSource)
                    {
                        if (f is SelectedShortsDirectories rp && rp.IsActive)
                        {
                            rp.NumberOfShorts = cnt;
                            break;
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
                        string DirectoryPath = rootfolder.Split(@"\").ToList().LastOrDefault();
                        if (DirectoryPath != "")
                        {
                            dbInit?.Invoke(this, new CustomParams_UpdateMultishortsByDir(DirectoryPath));
                        }
                    }

                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"doOnFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
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
