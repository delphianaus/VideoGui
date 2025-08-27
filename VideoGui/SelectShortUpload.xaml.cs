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
using System.Windows.Controls.Primitives;
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
        databasehook<object> Invoker = null;
        public bool IsClosing = false, IsClosed = false;

        public int uploadedcnt = 0;
        public WebViewDebug webviewDebug = null;
        string connectionStr = "";
        public SelectShortUpload(databasehook<object> _Invoker,
            OnFinishIdObj _DoOnFinished)
        {
            InitializeComponent();
            Invoker = _Invoker;
            connectionStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
            Closing += (s, e) => { IsClosing = true; };
            Closed += (s, e) => { IsClosed = true; _DoOnFinished?.Invoke(this, -1); };
        }

        int ShortsIndex = -1, LinkedId = -1;
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
                    //Get TitleId
                    //Get DescId
                    //InsertUpdate into SHORTSDIRECTORY(TITLEID,DESCID,DIRECTORYNAME) Returning ID as LinkedId

                    txtsrcdir.Text = ofg.SelectedFolder;
                    string connectionStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                    string ThisDir = ofg.SelectedFolder.Split(@"\").ToList().LastOrDefault();
                    var ress = Invoker?.Invoke(this, new CustomParams_InsertIntoShortsDirectory(ThisDir));// ToInt(-1)
                    if (ress is int res && res != -1) LinkedId = res;
                    /*string sql = "select ID from SHORTSDIRECTORY WHERE DIRECTORYNAME = @P0";
                    int res = connectionStr.ExecuteScalar(sql.ToUpper(), [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                    if (res == -1)
                    {
                        sql = "INSERT INTO SHORTSDIRECTORY(DIRECTORYNAME) VALUES (@P0) RETURNING ID";
                        res = connectionStr.ExecuteScalar(sql, [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                        ShortsIndex = res;
                        bool processed = false;
                        DateTime LastTimeUploaded = DateTime.Now.AddYears(-100);    
                        int LinkedId = res, NumberofShorts = Directory.EnumerateFiles(ofg.SelectedFolder, "*.mp4", SearchOption.AllDirectories).Count();
                        string uploaddir = ofg.SelectedFolder;
                        (LastTimeUploaded, processed) = CheckUploads(LinkedId);
                        Invoker?.Invoke(this, new CustomParams_Select(res));
                        sql = "SELECT ID FROM DESCRIPTIONS WHERE NAME = @P0";
                        int rres = connectionStr.ExecuteScalar(sql, [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                        if (rres == -1)
                        {
                            string desc = Environment.NewLine + Environment.NewLine
                             + "Follow me @ twitch.tv/justinstrainclips" +
                             Environment.NewLine + Environment.NewLine +
                             "Support Me On Patreon - https://www.patreon.com/join/JustinsTrainJourneys";
                            sql = "INSERT INTO DESCRIPTIONS(DESCRIPTION,NAME,ISTAG,GROUPID) VALUES (@P0,@P1,@P2,@NAME) RETURNING ID";
                            int descid = connectionStr.ExecuteScalar(sql.ToUpper(), [("@P0", ThisDir.ToUpper()+desc),
                                ("@P1", false), ("@P2", res), ("@NAME", ThisDir.ToUpper())]).ToInt(-1);
                            if (descid != -1)
                            {
                                sql = "UPDATE SHORTSDIRECTORY SET DESCID = @P0 WHERE ID = @P1";
                                connectionStr.ExecuteNonQuery(sql.ToUpper(), [("@P0", descid), ("@P1", res)]);
                                Invoker?.Invoke(this, new CustomParams_Update(res, UpdateType.Description));
                            }
                        }
                        sql = "SELECT ID FROM TITLES WHERE DESCRIPTION = @P0";
                        rres = connectionStr.ExecuteScalar(sql, [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                        if (rres == -1)
                        {
                            sql = "INSERT INTO TITLES(DESCRIPTION,ISTAG,GROUPID) VALUES (@P0,@P1,@P2) RETURNING ID";
                            int titleid = connectionStr.ExecuteScalar(sql.ToUpper(), [("@P0", ThisDir.ToUpper()),
                                ("@P1", false), ("@P2", res)]).ToInt(-1);
                            if (titleid != -1)
                            {
                                sql = "UPDATE SHORTSDIRECTORY SET TITLEID = @P0 WHERE ID = @P1";
                                connectionStr.ExecuteNonQuery(sql.ToUpper(), [("@P0", titleid), ("@P1", res)]);
                                Invoker?.Invoke(this, new CustomParams_UpdateTitleById(res, titleid));
                            }

                        }*/

                    /*string sql = "SELECT ID FROM MULTISHORTSINFO WHERE" +
                            " LINKEDSHORTSDIRECTORYID = @LINKEDID;";
                    int id = connectionStr.ExecuteScalar(sql, [("@LINKEDID", LinkedId)]).ToInt(-1);
                    if (id == -1)
                    {
                        // InsertIntoMultiShortsInfo(NumberofShorts, LinkedId, LastTimeUploaded, true);
                        // add customparam_insert into tbl multi shorts info
                        Invoker?.Invoke(this,
                            new CustomParams_InsertMultiShortsInfo(NumberofShorts, LinkedId, LastTimeUploaded, true));
                    }
                    else
                    {
                        Invoker?.Invoke(this, new CustomParams_UpdateMultiShortsInfo(LinkedId, NumberofShorts,
                            LastTimeUploaded, uploaddir));
                    }*/

                    key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("UploadPath", ofg.SelectedFolder);
                    key?.Close();

                    btnEditTitle.IsChecked = false;
                    btnEditDesc.IsChecked = false;
                    ShortsIndex = LinkedId;
                    Invoker?.Invoke(this, new CustomParams_Select(LinkedId));
                    CancellationTokenSource cts = new CancellationTokenSource();
                    string connectStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                    connectStr.ExecuteReader(GetShortsDirectorySql(LinkedId), cts, (FbDataReader r) =>
                    {
                        int titleid = r["TITLEID"].ToInt(-1);
                        int descid = r["DESCID"].ToInt(-1);
                        string LinkedTitleId = r["LINKEDTITLEIDS"] is string TITD ? TITD : "";
                        string LinkedDescId = r["LINKEDDESCIDS"] is string DITD ? DITD : "";
                        btnEditTitle.IsChecked = titleid != -1 && LinkedTitleId != "";
                        btnEditDesc.IsChecked = descid != -1 && LinkedDescId != "";
                        cts.Cancel();
                    });



                    RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key2.SetValue("UploadPath", txtsrcdir.Text);
                    if (ShortsIndex != -1)
                    {
                        key2.SetValue("CurrentDirId", ShortsIndex);
                    }
                    if (txtMaxUpload.Text != "")
                    {
                        key2.SetValue("UploadNumber", txtMaxUpload.Text);
                    }
                    key2?.Close();
                    List<string> files = Directory.EnumerateFiles(ofg.SelectedFolder, "*.mp4", SearchOption.AllDirectories).ToList();
                    foreach (var filename in files.Where(filename => !filename.Contains("_") && LinkedId != -1))
                    {
                        string fnx = filename.Split(@"\").ToList().LastOrDefault();
                        string drx = filename.Replace(fnx, "");

                        string frr = System.IO.Path.GetFileNameWithoutExtension(fnx);
                        int fr = System.IO.Path.GetFileNameWithoutExtension(fnx).ToInt(-1);
                        if (fr != -1)
                        {
                            frr = fr.ToString();// "X");
                        }
                        string newfile = drx + frr + $"_{ShortsIndex}{Path.GetExtension(filename)}";
                        if (filename != newfile)
                        {

                            if (File.Exists(newfile))
                            {
                                File.Delete(newfile);
                            }
                            File.Move(filename, newfile);
                        }
                    }
                }
                //lblShortNo.Content = files.Count.ToString();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSelectSourceDir_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        public (DateTime, bool) CheckUploads(int LinkedId)
        {
            try
            {
                bool processed = false;
                string filen = "";
                DateTime LastTimeUploaded = DateTime.Now.Date.AddYears(-100);
                string SQLB = "SELECT * FROM UploadsRecord ORDER BY RDB$RECORD_VERSION DESC ROWS 500;";
                connectionStr.ExecuteReader(SQLB, (FbDataReader r) =>
                {
                    filen = (r["UPLOADFILE"] is string f) ? f : "";
                    var dt = (r["UPLOAD_DATE"] is DateTime d) ? d : DateTime.Now.Date.AddYears(-100);
                    TimeSpan dtr = (r["UPLOAD_TIME"] is TimeSpan t1) ? t1 : new TimeSpan();
                    if (filen.Contains("_") && !processed)
                    {
                        string Idx = filen.Split('_').LastOrDefault();
                        if (Idx != "" && Idx == LinkedId.ToString())
                        {
                            LastTimeUploaded = dt.AtTime(dtr);
                            processed = true;
                        }
                    }
                });
                return (LastTimeUploaded, processed);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"rds {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
                return (DateTime.Now.Date.AddYears(-100), false);
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
                btnEditDesc.IsChecked = Id != -1;//&& linkedDescids != "";
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
        int SelectedTitleId = -1, SelectedDescId = -1;
        public void DoTitleSelectCreate(int TitleId = -1)
        {
            try
            {
                SelectedTitleId = TitleId;
                var _DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, Invoker,
                    true, TitleId, LinkedId);
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
                Show();
                if (sender is TitleSelectFrm frm)
                {
                    bool found = false;

                    ShortsIndex = LinkedId;

                    string sql = "";
                    if ((SelectedTitleId != frm.TitleId))
                    {
                        sql = "UPDATE SHORTSDIRECTORY SET TITLEID = @TITLEID WHERE ID = @ID";
                        connectionStr.ExecuteNonQuery(sql,
                            [("@ID", ShortsIndex), ("@TITLEID", frm.TitleId)]);
                    }
                    string linkedtitleid = "";
                    sql = GetShortsDirectorySql(ShortsIndex);
                    CancellationTokenSource cts = new CancellationTokenSource();
                    connectionStr.ExecuteReader(sql, cts, (FbDataReader r) =>
                    {
                        linkedtitleid = (r["LINKEDTITLEIDS"] is string tidt ? tidt : "");
                        cts.Cancel();
                    });
                    btnEditTitle.IsChecked = (linkedtitleid != "" && frm.TitleId != -1);
                }
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


        private void doOnFinish(object sender, int id)
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
                if (sender != null && sender is ScraperModule scraperModulex && !scraperModulex.KilledUploads)
                {
                    List<string> filesdone = new List<string>();
                    bool Exc = scraperModule.Exceeded;
                    filesdone.AddRange(scraperModule.ScheduledOk);
                    int Uploaded = scraperModule.TotalScheduled;
                    int shortsleft = GetFileCount(rootfolder);
                    uploadedcnt = scraperModule.uploadedcnt;
                    if (!Exc && shortsleft > 0 && Uploaded < txtTotalUploads.Text.ToInt())
                    {
                        int Maxuploads = (txtTotalUploads.Text != "") ? txtTotalUploads.Text.ToInt(100) : 100;
                        int UploadsPerSlot = (txtMaxUpload.Text != "") ? txtMaxUpload.Text.ToInt(5) : 5;
                        var tscraperModule = new ScraperModule(Invoker, doOnFinish, gUrl, Maxuploads, UploadsPerSlot, 0, false);
                        tscraperModule.ShowActivated = true;
                        tscraperModule.ScheduledOk.AddRange(filesdone);
                        Hide();
                        Process[] webView2Processes = Process.GetProcessesByName("MicrosoftEdgeWebview2");
                        foreach (Process process in webView2Processes)
                        {
                            process.Kill();
                        }
                        tscraperModule.Show();
                        return;
                    }
                    else
                    {
                        string DirectoryPath = rootfolder.Split(@"\").ToList().LastOrDefault();
                        if (DirectoryPath != "")
                        {
                            Invoker?.Invoke(this, new CustomParams_UpdateMultishortsByDir(DirectoryPath, ""));
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRunUploaders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool Valid = true;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("UploadPath", "");
                string selFolder = txtsrcdir.Text;
                if (selFolder != rootfolder && selFolder != "")
                {
                    key.SetValue("UploadPath", selFolder);
                }
                key?.Close();
                if (Directory.Exists(selFolder))
                {
                    string connectionStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                    List<string> files = Directory.EnumerateFiles(selFolder, "*.mp4", SearchOption.AllDirectories).ToList();
                    string firstfile = files.FirstOrDefault();
                    if (firstfile is not null && File.Exists(firstfile))
                    {
                        string fid = Path.GetFileNameWithoutExtension(firstfile).Split('_').LastOrDefault();
                        int fidres = fid.ToInt(-1);
                        if (fidres != -1 && fidres != ShortsIndex)
                        {
                            ShortsIndex = fidres;
                        }
                        string sql = "update MULTISHORTS set ISSHORTSACTIVE = @ACTIVE where LINKEDSHORTSDIRECTORYID = @ID;";
                        connectionStr.ExecuteNonQuery(sql, [("@ACTIVE", 1), ("@ID", ShortsIndex)]);
                        sql = "update MULTISHORTS set ISSHORTSACTIVE = @ACTIVE where LINKEDSHORTSDIRECTORYID != @ID;";
                        connectionStr.ExecuteNonQuery(sql, [("@ACTIVE", 0), ("@ID", ShortsIndex)]);
                        sql = "SELECT DIRECTORYNAME FROM SHORTSDIRECTORY WHERE ID = @P0";
                        var ddirname = connectionStr.ExecuteScalar(sql, [("@P0", ShortsIndex)]);
                        if (ddirname is string dirname)
                        {
                            string DirName = selFolder.Split(@"\").ToList().LastOrDefault();
                            if (DirName.ToLower() != dirname.ToLower())
                            {
                                Valid = false;
                            }
                        }
                        if (!Valid)
                        {
                            string dir = selFolder.Split(@"\").ToList().LastOrDefault().ToUpper();
                            var r = Invoker?.Invoke(this, new
                                CustomParams_RematchedUpdate(ShortsIndex, dir));
                            if (r is bool res) Valid = res;

                            //todo do in Invoker 

                        }
                    }

                }

                if (Valid)
                {



                    WebAddressBuilder webAddressBuilder = new WebAddressBuilder("UCdMH7lMpKJRGbbszk5AUc7w");
                    string gUrl = webAddressBuilder.Dashboard().Address;
                    int Maxuploads = (txtTotalUploads.Text != "") ? txtTotalUploads.Text.ToInt(100) : 100;
                    int UploadsPerSlot = (txtMaxUpload.Text != "") ? txtMaxUpload.Text.ToInt(5) : 5;
                    if (Maxuploads > lblShortNo.Content.ToInt())
                    {
                        Maxuploads = lblShortNo.Content.ToInt();
                    }

                    var xscraperModule = new ScraperModule(Invoker, doOnFinish, gUrl, Maxuploads, UploadsPerSlot, 0, true);

                    xscraperModule.ShowActivated = true;
                    Hide();
                    xscraperModule.Show();

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
                string Dir = txtsrcdir.Text.Split(@"\").ToList().LastOrDefault();
                var r = Invoker?.Invoke(this, new CustomParams_LookUpTitleId(Dir));
                if (r is not null)
                {
                    DoTitleSelectCreate(r.ToInt(-1));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnEditTitle_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        bool OldState = false;
        private void btnEditDesc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql = "SELECT DESCID FROM SHORTSDIRECTORY WHERE ID = @ID";
                SelectedDescId = connectionStr.ExecuteScalar(sql,
                    [("@ID", LinkedId)]).ToInt(-1);
                var _DoDescSelectFrm = new DescSelectFrm(OnSelectFormClose, Invoker, true, SelectedDescId);
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
                Show();
                if (sender is DescSelectFrm frm)
                {
                    int descid = frm.Id;
                    if ((SelectedDescId != frm.Id))
                    {
                        string sql = "UPDATE SHORTSDIRECTORY SET DESCID = @DESCID WHERE ID = @ID";
                        connectionStr.ExecuteNonQuery(sql,
                            [("@ID", LinkedId), ("@TITLEID", frm.Id)]);
                    }
                    btnEditDesc.IsChecked = (descid != -1);//&& linkeddescid != "");
                }
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
                string connectionStr = Invoker.InvokeWithReturn<string>(this, new CustomParams_GetConnectionString());
                bool found = false;
                txtsrcdir.Text = rootfolder;// : txtsrcdir.Text;
                txtMaxUpload.Text = (uploadsnumber != "") ? uploadsnumber : txtMaxUpload.Text;
                txtTotalUploads.Text = (MaxUploads != "") ? MaxUploads : txtTotalUploads.Text;
                if (rootfolder != @"D:\shorts\" && Directory.Exists(rootfolder))
                {
                    int cnt = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                    lblShortNo.Content = cnt.ToString();
                }
                else lblShortNo.Content = "N/A";
                if (rootfolder != "")
                {
                    string ThisDir = rootfolder.Split(@"\").ToList().LastOrDefault();
                    if (Invoker?.Invoke(this, new CustomParams_AddDirectory(ThisDir.ToUpper())) is TurlpeDualString tds)
                    {
                        string sqle = GetShortsDirectorySql(tds.Id);
                        CancellationTokenSource cts = new CancellationTokenSource();
                        connectionStr.ExecuteReader(sqle, cts, (FbDataReader r) =>
                        {
                            int titleid = r["TITLEID"].ToInt(-1);
                            int descid = r["DESCID"].ToInt(-1);
                            string LinkedTitleId = r["LINKEDTITLEIDS"] is string TITD ? TITD : "";
                            string LinkedDescId = r["LINKEDDESCIDS"] is string DITD ? DITD : "";
                            btnEditTitle.IsChecked = titleid != -1 && LinkedTitleId != "";
                            btnEditDesc.IsChecked = descid != -1 && LinkedDescId != "";
                            cts.Cancel();
                        });
                    }
                    /*
                    
                    string sql = "select ID from SHORTSDIRECTORY WHERE DIRECTORYNAME = @P0";
                    int res = connectionStr.ExecuteScalar(sql.ToUpper(), [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                    if (res == -1)
                    {
                        sql = "INSERT INTO SHORTSDIRECTORY(DIRECTORYNAME) VALUES (@P0) RETURNING ID";
                        res = connectionStr.ExecuteScalar(sql, [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                        btnEditTitle.IsChecked = false;
                        btnEditDesc.IsChecked = false;
                        sql = "SELECT ID FROM TITLES WHERE DESCRIPTION = @P0";
                        res = connectionStr.ExecuteScalar(sql.ToUpper(), [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                        if (res == -1)
                        {
                            sql = "INSERT INTO TITLES(DESCRIPTION,ISTAG,GROUPID) VALUES (@P0,@P1,@P2) RETURNING ID";
                            int titleid = connectionStr.ExecuteScalar(sql.ToUpper(), [("@P0", ThisDir.ToUpper()),
                                ("@P1", false), ("@P2", res)]).ToInt(-1);
                            if (titleid != -1)
                            {
                                Invoker?.Invoke(this, new CustomParams_Select(titleid));
                            }
                        }

                    }
                    else
                    {
                        Invoker?.Invoke(this, new CustomParams_Select(res));
                        string sq = "SELECT ID FROM TITLES WHERE GROUPID = @GID AND ISTAG = @ISTAG";
                        int idd = connectionStr.ExecuteScalar(sq, [("@GID", res), ("@ISTAG", false)]).ToInt(-1);
                        if (idd == -1)
                        {
                            sq = "SELECT ID FROM TITLES WHERE DESCRIPTION = @P0";
                            int idy = connectionStr.ExecuteScalar(sq.ToUpper(), [("@P0", ThisDir.ToUpper())]).ToInt(-1);
                            if (idy == -1)
                            {
                                sq = "INSERT INTO TITLES(DESCRIPTION,ISTAG,GROUPID) VALUES (@P0,@P1,@P2) RETURNING ID";
                                int titleid = connectionStr.ExecuteScalar(sq.ToUpper(), [("@P0", ThisDir.ToUpper()),
                                ("@P1", false), ("@P2", res)]).ToInt(-1);
                                if (titleid != -1)
                                {
                                    Invoker?.Invoke(this, new CustomParams_Select(titleid));
                                }
                            }
                            else
                            {
                                sq = "UPDATE TITLES SET GROUPID = @GID WHERE ID = @ID";
                                connectionStr.ExecuteScalar(sq, [("@ID", idy), ("@GID", res)]);
                                Invoker?.Invoke(this, new CustomParams_Select(idy));
                            }
                        }

                        connectionStr.ExecuteReader(GetShortsDirectorySql(res), (FbDataReader r) =>
                        {
                            int titleid = r["TITLEID"].ToInt(-1);
                            int descid = r["DESCID"].ToInt(-1);
                            string LinkedTitleId = r["LINKEDTITLEIDS"] is string TITD ? TITD : "";
                            string LinkedDescId = r["LINKEDDESCIDS"] is string DITD ? DITD : "";
                            btnEditTitle.IsChecked = titleid != -1 && LinkedTitleId != "";
                            btnEditDesc.IsChecked = descid != -1 && LinkedDescId != "";
                        });
                    }

                    */

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
    }


}
