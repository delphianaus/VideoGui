using FirebirdSql.Data.FirebirdClient;
using FolderBrowserEx;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        public SelectShortUpload(databasehook<object> _dbInit, OnFinish _DoOnFinished)
        {
            InitializeComponent();
            dbInit = _dbInit;
            Closing += (s, e) => { IsClosing = true; };
            Closed += (s, e) => { IsClosed = true; _DoOnFinished?.Invoke(); };
        }

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
                        using (var connection = new FbConnection(connectionString))
                        {
                            connection.Open();
                            string sql = "select * from SHORTSDIRECTORY WHERE DIRECTORYNAME = @P0";
                            using (var command = new FbCommand(sql.ToUpper(), connection))
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@P0", ThisDir.ToUpper());
                                object result = command.ExecuteScalar();
                                res = result.ToInt(-1);
                            }
                            if (res == -1)
                            {
                                sql = "INSERT INTO SHORTSDIRECTORY(DIRECTORYNAME) VALUES (@P0) RETURNING ID";
                                using (var command = new FbCommand(sql.ToUpper(), connection))
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@p0", ThisDir.ToUpper());
                                    object result = command.ExecuteScalar();
                                    if (result is int idxx)
                                    {
                                        res = idxx;
                                    }
                                }
                            }
                            connection.Close();
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
                        string newfile = drx +System.IO.Path.GetFileNameWithoutExtension(filename) + $"_{res}{Path.GetExtension(filename)}";

                        File.Move(filename, newfile);
                    }
                    lblShortNo.Content = files.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSelectSourceDir_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        
        private void doOnFinish()
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
                    if (!Exc && Uploaded < txtTotalUploads.Text.ToInt())
                    {
                        int Maxuploads = (txtTotalUploads.Text != "") ? txtTotalUploads.Text.ToInt(100) : 100;
                        int UploadsPerSlot = (txtMaxUpload.Text != "") ? txtMaxUpload.Text.ToInt(5) : 5;
                        scraperModule = new ScraperModule(dbInit, doOnFinish, gUrl, false, true, Maxuploads, UploadsPerSlot);
                        scraperModule.ShowActivated = true;
                        scraperModule.ScheduledOk.AddRange(filesdone);
                        Hide();
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
                    scraperModule = new ScraperModule(dbInit, doOnFinish, gUrl, false, true, Maxuploads, UploadsPerSlot);
                    scraperModule.ShowActivated = true;
                    Hide();
                    scraperModule.Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnRunUploaders_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("UploadPath", @"D:\shorts");
                string uploadsnumber = key.GetValueStr("UploadNumber", "5");
                string MaxUploads = key.GetValueStr("MaxUploads", "100");
                key?.Close();
                txtsrcdir.Text = (rootfolder != "" && Directory.Exists(rootfolder)) ? rootfolder : txtsrcdir.Text;
                txtMaxUpload.Text = (uploadsnumber != "") ? uploadsnumber : txtMaxUpload.Text;
                txtTotalUploads.Text = (MaxUploads != "") ? MaxUploads : txtTotalUploads.Text;
                if (rootfolder != @"D:\shorts\")
                {
                    int cnt = Directory.EnumerateFiles(rootfolder, "*.mp4", SearchOption.AllDirectories).ToList().Count();
                    lblShortNo.Content = cnt.ToString();
                }
                else lblShortNo.Content = "N/A";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
    }
}
