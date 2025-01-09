using FirebirdSql.Data.FirebirdClient;
using FolderBrowserEx;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
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
using Path = System.IO.Path;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for SelectShortUpload.xaml
    /// </summary>
    public partial class SelectShortUpload : Window
    {
        databasehook<object> dbInit = null;
        public bool IsClosing = false, IsClosed = false;
        public TitleSelectFrm DoTitleSelectFrm = null;
        public DescSelectFrm DoDescSelectFrm = null;
        public SetLists ConnnectLists = null;
        public WebViewDebug webviewDebug = null;
        public SelectShortUpload(databasehook<object> _dbInit, OnFinish _DoOnFinished, SetLists _SetLists)
        {
            InitializeComponent();
            dbInit = _dbInit;
            ConnnectLists = _SetLists;
            Closing += (s, e) => { IsClosing = true; };
            Closed += (s, e) => { IsClosed = true; _DoOnFinished?.Invoke(); };
        }

        int ShortsIndex = -1;
        ScraperModule scraperModule = null;
        private void btnSelectSourceDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("UploadPath", @"D:\shorts\");
                string MaxUploads = key.GetValueStr("MaxUploads", "100");
                string destinationdir = rootfolder;
                key?.Close();
                FolderBrowserDialog ofg = new FolderBrowserDialog();
                ofg.AllowMultiSelect = false;
                ofg.InitialFolder = rootfolder;
                ofg.Title = "Select Shorts Upload Directory";
                if (ofg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtsrcdir.Text = ofg.SelectedFolder;
                    var p = new CustomParams_GetConnectionString();
                    dbInit?.Invoke(this, p);
                    int res = -1;
                    string ThisDir = ofg.SelectedFolder.Split(@"\").ToList().LastOrDefault();
                    if (p.ConnectionString is not null && p.ConnectionString.Length > 0)
                    {
                        string connectionString = p.ConnectionString;
                        string sql = "select ID from SHORTSDIRECTORY WHERE DIRECTORYNAME = @P0";
                        res = connectionString.ExecuteScalar(sql.ToUpper(), [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                        if (res == -1)
                        {
                            sql = "INSERT INTO SHORTSDIRECTORY(DIRECTORYNAME) VALUES (@P0) RETURNING ID";
                            res = connectionString.ExecuteScalar(sql.ToUpper(), [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                            ShortsIndex = res;
                            btnEditTitle.IsChecked = false;
                            btnEditDesc.IsChecked = false;
                        }
                        else
                        {
                            ShortsIndex = res;
                            dbInit?.Invoke(this, new CustomParams_Select(res));
                            connectionString.ExecuteReader(GetShortsDirectorySql(res), (FbDataReader r) =>
                            {
                                int titleid = r["TITLEID"].ToInt(-1);
                                int descid = r["DESCID"].ToInt(-1);
                                string LinkedTitleId = r["LINKEDTITLEIDS"] is string TITD ? TITD : "";
                                string LinkedDescId = r["LINKEDDESCIDS"] is string DITD ? DITD : "";
                                btnEditTitle.IsChecked = titleid != -1 && LinkedTitleId != "";
                                btnEditDesc.IsChecked = descid != -1 && LinkedDescId != "";
                            });
                        }

                    }

                    RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key2.SetValue("UploadPath", txtsrcdir.Text);
                    if (res != -1)
                    {
                        key2.SetValue("CurrentDirId", res);
                    }
                    if (txtMaxUpload.Text != "")
                    {
                        key2.SetValue("UploadNumber", txtMaxUpload.Text);
                    }
                    key2?.Close();
                    List<string> files = Directory.EnumerateFiles(ofg.SelectedFolder, "*.mp4", SearchOption.AllDirectories).ToList();
                    foreach (var filename in files.Where(filename => !filename.Contains("_") && res != -1))
                    {
                        string fnx = filename.Split(@"\").ToList().LastOrDefault();
                        string drx = filename.Replace(fnx, "");

                        string frr = System.IO.Path.GetFileNameWithoutExtension(fnx);
                        int fr = System.IO.Path.GetFileNameWithoutExtension(fnx).ToInt(-1);
                        if (fr != -1)
                        {
                            frr = fr.ToString();// "X");
                        }
                        string newfile = drx + frr + $"_{res}{Path.GetExtension(filename)}";
                        if (filename != newfile)
                        {

                            if (File.Exists(newfile))
                            {
                                File.Delete(newfile);
                            }
                            File.Move(filename, newfile);
                        }
                    }
                    lblShortNo.Content = files.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSelectSourceDir_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public int GetFileCount(string Folder)
        {
            try
            {
                int res = 0;
                List<string> files = Directory.EnumerateFiles(Folder, "*.mp4", SearchOption.AllDirectories).ToList();
                res += files.Where(filename => filename.Contains("_")).Count();
                lblShortNo.Content = res;
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite();
                return -1;
            }
        }

        public void UpdateDescId(int Id, string linkedDescids)
        {
            try
            {
                btnEditDesc.IsChecked = Id != -1 && linkedDescids != "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSelectSourceDir_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        public void UpdateTitleId(int Id, string linkedtitleids)
        {
            try
            {
                btnEditTitle.IsChecked = Id != -1 & linkedtitleids != "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSelectSourceDir_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        public void DoTitleSelectCreate()
        {
            try
            {

                if (DoTitleSelectFrm is not null)
                {
                    if (!DoTitleSelectFrm.IsClosing && !DoTitleSelectFrm.IsClosed)
                    {
                        DoTitleSelectFrm.Close();
                        DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, dbInit, true);
                        Hide();
                        DoTitleSelectFrm.Show();
                    }
                }
                else
                {
                    DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, dbInit, true);
                    Hide();
                    DoTitleSelectFrm.Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoTitleSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void DoOnFinishTitleSelect()
        {
            try
            {
                if (DoTitleSelectFrm is not null)
                {
                    var p = new CustomParams_GetConnectionString();
                    dbInit?.Invoke(this, p);
                    bool found = false;
                    int titleid = -1;
                    string connectionStr = p.ConnectionString;
                    string sql = "Select TITLEID FROM SHORTSDIRECTORY WHERE ID = @ID";
                    titleid = connectionStr.ExecuteScalar(sql, [("@ID", ShortsIndex)]).ToInt(-1);

                    if ((titleid == -1 || titleid != DoTitleSelectFrm.TitleId))
                    {
                        sql = "UPDATE SHORTSDIRECTORY SET TITLEID = @TITLEID WHERE ID = @ID";
                        connectionStr.ExecuteNonQuery(sql, [("@ID", ShortsIndex), ("@TITLEID", DoTitleSelectFrm.TitleId)]);
                    }

                    string linkedtitleid = "";
                    sql = GetShortsDirectorySql(ShortsIndex);
                    sql.ExecuteReader(connectionStr, (FbDataReader r) =>
                    {
                        linkedtitleid = (r["LINKEDTITLEID"] is string tidt ? tidt : "");
                    });
                    btnEditTitle.IsChecked = (linkedtitleid != "" && DoTitleSelectFrm.TitleId != -1);
                    //GetShortsDirectorySql(DoTitleSelectFrm.PersistId);


                    if (DoTitleSelectFrm.IsTitleChanged)
                    {
                        dbInit?.Invoke(this, new CustomParams_Update(DoTitleSelectFrm.TitleId, UpdateType.Title));
                    }
                    if (DoTitleSelectFrm.IsClosed)
                    {
                        DoTitleSelectFrm = null;
                    }
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
                       "WHERE TITLETAGID = S.DESCID) AS LINKEDDESCIDS " +
                       "FROM SHORTSDIRECTORY S" +
                (index != -1 ? $" WHERE S.ID = {index} " : "");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} GetShortsDirectorySql {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "";
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
                int cnt = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                lblShortNo.Content = cnt.ToString();
                if (scraperModule != null && !scraperModule.KilledUploads)
                {
                    List<string> filesdone = new List<string>();
                    bool Exc = scraperModule.Exceeded;
                    filesdone.AddRange(scraperModule.ScheduledOk);
                    int Uploaded = scraperModule.TotalScheduled;
                    int shortsleft = GetFileCount(rootfolder);
                    if (!Exc && shortsleft > 0 && Uploaded < txtTotalUploads.Text.ToInt())
                    {
                        int Maxuploads = (txtTotalUploads.Text != "") ? txtTotalUploads.Text.ToInt(100) : 100;
                        int UploadsPerSlot = (txtMaxUpload.Text != "") ? txtMaxUpload.Text.ToInt(5) : 5;
                        scraperModule = new ScraperModule(dbInit, doOnFinish, gUrl, Maxuploads, UploadsPerSlot);
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
                }


                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"doOnFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRunUploaders_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (scraperModule is not null && !scraperModule.IsClosed)
                {
                    if (scraperModule.IsClosing) scraperModule.Close();
                    while (!scraperModule.IsClosed)
                    {
                        Thread.Sleep(100);
                    }
                    scraperModule.Close();
                    scraperModule = null;
                }
                if (scraperModule is null)
                {
                    WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                    string gUrl = webAddressBuilder.Dashboard().Address;
                    int Maxuploads = (txtTotalUploads.Text != "") ? txtTotalUploads.Text.ToInt(100) : 100;
                    int UploadsPerSlot = (txtMaxUpload.Text != "") ? txtMaxUpload.Text.ToInt(5) : 5;
                    if (Maxuploads > lblShortNo.Content.ToInt())
                    {
                        Maxuploads = lblShortNo.Content.ToInt();
                    }

                    scraperModule = new ScraperModule(dbInit, doOnFinish, gUrl, Maxuploads, UploadsPerSlot);

                    scraperModule.ShowActivated = true;
                    Hide();
                    // Process[] webView2Processes = Process.GetProcessesByName("MicrosoftEdgeWebview2");
                    // foreach (Process process in webView2Processes)
                    //  {
                    //       process.Kill();
                    //   }
                    scraperModule.Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnRunUploaders_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void WebViewDebugOnFinish()
        {
            try
            {
                Show();
                if (webviewDebug is not null && !webviewDebug.IsClosed)
                {
                    if (webviewDebug.IsClosing) webviewDebug.Close();
                    while (!webviewDebug.IsClosed)
                    {
                        Thread.Sleep(100);
                        System.Windows.Forms.Application.DoEvents();
                    }
                    webviewDebug.Close();
                    webviewDebug = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"WebViewDebugOnFinish {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
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

        private void btnEditTitle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoTitleSelectCreate();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnEditTitle_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void btnEditDesc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    if (DoDescSelectFrm is not null && DoDescSelectFrm.IsActive)
                    {
                        DoDescSelectFrm.Close();
                        DoDescSelectFrm = null;
                    }

                    DoDescSelectFrm = new DescSelectFrm(OnSelectFormClose, dbInit, true);
                    Hide();
                    DoDescSelectFrm.Show();
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"{this} DoDescSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnEditTitle_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void OnSelectFormClose()
        {
            try
            {
                if (DoDescSelectFrm is not null)
                {
                    var p = new CustomParams_GetConnectionString();
                    dbInit?.Invoke(this, p);
                    string connectionStr = p.ConnectionString;
                    int id = DoDescSelectFrm.Id;
                    int descid = -1;
                    string sql = "SELECT DESCID FROM SHORTSDIRECTORY WHERE ID = @ID";
                    descid = connectionStr.ExecuteScalar(sql, [("@ID", id)]).ToInt(-1);
                    if ((descid == -1 || descid != DoDescSelectFrm.TitleTagId))
                    {
                        sql = "UPDATE SHORTSDIRECTORY SET DESCID = @DESCID WHERE ID = @ID";
                        connectionStr.ExecuteNonQuery(sql, [("@ID", ShortsIndex), ("@DESCID", DoDescSelectFrm.TitleTagId)]);
                    }
                    string linkeddescid = "";
                    sql = GetShortsDirectorySql(DoDescSelectFrm.Id);
                    connectionStr.ExecuteReader(sql, (FbDataReader r) =>
                    {
                        linkeddescid = (r["LINKEDDESCIDS"] is string did ? did : "");
                    });

                    btnEditDesc.IsChecked = (descid != -1 && linkeddescid != "");

                    if (DoDescSelectFrm.IsDescChanged)
                    {
                        dbInit?.Invoke(this, new CustomParams_Update(DoDescSelectFrm.TitleTagId, UpdateType.Description));
                    }
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

        private void btnEditTitle_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("UploadPath", @"D:\shorts");
                string uploadsnumber = key.GetValueStr("UploadNumber", "5");
                string MaxUploads = key.GetValueStr("MaxUploads", "100");
                key?.Close();
                //ConnnectLists?.Invoke(3);
                var p = new CustomParams_GetConnectionString();
                dbInit?.Invoke(this, p);
                txtsrcdir.Text = (rootfolder != "" && Directory.Exists(rootfolder)) ? rootfolder : txtsrcdir.Text;
                txtMaxUpload.Text = (uploadsnumber != "") ? uploadsnumber : txtMaxUpload.Text;
                txtTotalUploads.Text = (MaxUploads != "") ? MaxUploads : txtTotalUploads.Text;
                if (rootfolder != @"D:\shorts\")
                {
                    int cnt = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                    lblShortNo.Content = cnt.ToString();
                }
                else lblShortNo.Content = "N/A";
                if (rootfolder != "")
                {
                    string ThisDir = rootfolder.Split(@"\").ToList().LastOrDefault();
                    if (p.ConnectionString is not null && p.ConnectionString.Length > 0)
                    {
                        string connectionString = p.ConnectionString;
                        string sql = "select ID from SHORTSDIRECTORY WHERE DIRECTORYNAME = @P0";
                        int res = connectionString.ExecuteScalar(sql.ToUpper(), [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                        if (res == -1)
                        {
                            sql = "INSERT INTO SHORTSDIRECTORY(DIRECTORYNAME) VALUES (@P0) RETURNING ID";
                            res = connectionString.ExecuteScalar(sql.ToUpper(), [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                            btnEditTitle.IsChecked = false;
                            btnEditDesc.IsChecked = false;
                        }
                        else
                        {
                            dbInit?.Invoke(this, new CustomParams_Select(res));
                            connectionString.ExecuteReader(GetShortsDirectorySql(res), (FbDataReader r) =>
                            {
                                int titleid = r["TITLEID"].ToInt(-1);
                                int descid = r["DESCID"].ToInt(-1);
                                string LinkedTitleId = r["LINKEDTITLEIDS"] is string TITD ? TITD : "";
                                string LinkedDescId = r["LINKEDDESCIDS"] is string DITD ? DITD : "";
                                btnEditTitle.IsChecked = titleid != -1 && LinkedTitleId != "";
                                btnEditDesc.IsChecked = descid != -1 && LinkedDescId != "";
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
    }
}
