using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using Microsoft.Win32;
using FolderBrowserEx;
using static System.Net.WebRequestMethods;
using System.Threading;
using Nancy.Routing.Trie.Nodes;
using VideoGui.Models.delegates;
using Path = System.IO.Path;
using System.Configuration;
using VideoGui.Models;
using System.Runtime.InteropServices;
using CliWrap;
using System.Windows.Forms;
using FolderBrowserDialog = FolderBrowserEx.FolderBrowserDialog;
using FirebirdSql.Data.FirebirdClient;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for VideoCutsEditor.xaml
    /// </summary>
    public partial class VideoCutsEditor : Window
    {
        OnFinish DoOnFinish;
        string Filename = "";
        int _TotalSecs = 0;
        string ConnectionString = "";
        public AddRecordDelegate DoAddRecord;
        List<VideoCutInfo> ListOfCuts = new List<VideoCutInfo>();
        public VideoCutsEditor(AddRecordDelegate _DoAddRecord, OnFinish _DoOnFinish, string connectionString)
        {
            InitializeComponent();
            ConnectionString = connectionString;
            DoAddRecord = _DoAddRecord;
            DoOnFinish = _DoOnFinish;
        }

        TimeSpan LastTarget = TimeSpan.Zero; // 22 mins
        int LastSegment = -1; // 5 seconds
        int LastThresh = -1; // 70 seconds

        private void btnSelectSourceDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string Root = key.GetValueStr("CutSourceDirectory", "c:\\");
                key?.Close();


                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Title = "Select a folder";
                folderBrowserDialog.InitialFolder = Root;
                folderBrowserDialog.AllowMultiSelect = false;
                folderBrowserDialog.DefaultFolder = Root;
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtsrcdir.Text = folderBrowserDialog.SelectedFolder;
                    key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("CutSourceDirectory", txtsrcdir.Text);
                    key?.Close();
                    int threash = 70;
                    string id = "";
                    string sql = $"SELECT ID FROM AUTOEDITS WHERE SOURCE = @P0";
                    int idx = ConnectionString.ExecuteScalar(sql, [("@P0", txtsrcdir.Text)]).ToInt(-1);
                    bool Loaded = false;
                    if (idx != -1)
                    {
                        CancellationTokenSource cts = new ();
                        ConnectionString.ExecuteReader($"SELECT * FROM AUTOEDITS WHERE ID = @ID", [("@ID", idx)], cts, (FbDataReader reader) =>
                        {
                            string DestDir = (reader["DESTINATION"] is string des) ? des : "";
                            string Target = (reader["TARGET"] is string target) ? target : "";
                            string Segment = (reader["SEGMENT"] is string segment) ? segment : "";
                            string Threashhold = (reader["THRESHHOLD"] is string theashhold) ? theashhold : "";
                            txxtEditDirectory.Text = (DestDir != "") ? DestDir : txxtEditDirectory.Text;
                            txtSegment.Text = (Segment != "") ? Segment : txtSegment.Text;
                            txtThreash.Text = (Threashhold != "") ? Threashhold : txtThreash.Text;
                            txtTarget.Text = (Target != "") ? Target : txtTarget.Text;
                            Loaded = true;
                            txtTarget.IsEnabled = true;
                            txtSegment.IsEnabled = true;
                            txtThreash.IsEnabled = true;
                            BtnCalc.IsEnabled = true;
                            cts.Cancel();
                        });
                    }
                    else
                    {
                        string r, Last = txtsrcdir.Text.Split("\\").ToList().LastOrDefault();
                        if (Last.Length >= 6)
                        {
                            r = Last.Substring(Last.Length - 6, 6);
                            CancellationTokenSource cts = new CancellationTokenSource();
                            ConnectionString.ExecuteReader("select * from autoedits WHERE source CONTAINING @P0", [("@P0", r)], cts, (FbDataReader reader) =>
                            {
                                string DestDir = (reader["DESTINATION"] is string des) ? des : "";
                                string Target = (reader["TARGET"] is string target) ? target : "";
                                string Segment = (reader["SEGMENT"] is string segment) ? segment : "";
                                string Threashhold = (reader["THRESHHOLD"] is string theashhold) ? theashhold : "";
                                txxtEditDirectory.Text = (DestDir != "") ? DestDir : txxtEditDirectory.Text;
                                txtSegment.Text = (Segment != "") ? Segment : txtSegment.Text;
                                txtThreash.Text = (Threashhold != "") ? Threashhold : txtThreash.Text;
                                txtTarget.Text = (Target != "") ? Target : txtTarget.Text;
                                Loaded = true;
                                cts.Cancel();
                            });
                        }


                    }
                    Filename = txtsrcdir.Text.Split("\\").ToList().LastOrDefault();
                    var FileIndexer = new ffmpegbridge();
                    List<string> files = Directory.EnumerateFiles(txtsrcdir.Text, "*.mp4", SearchOption.TopDirectoryOnly).ToList();
                    FileIndexer.ReadDuration(files);
                    while (!FileIndexer.Finished)
                    {
                        Thread.Sleep(100);
                    }
                    var TotalSecs = FileIndexer.GetDuration();
                    lblTotalTime.Content = TotalSecs.ToFFmpeg();
                    FileIndexer = null;
                    _TotalSecs = Math.Truncate(TotalSecs.TotalSeconds).ToInt();
                    TimeSpan Target = TimeSpan.Zero;
                    txtTarget.IsEnabled = true;
                    if (!Loaded)
                    {
                        if (txtTarget.Text != "")
                        {
                            string time = "00:" + txtTarget;
                            Target = time.FromStrToTimeSpan();
                        }
                        else Target = TimeSpan.FromMinutes(22);
                    }
                    else
                    {
                        Target = txtTarget.Text.FromStrToTimeSpan();

                    }
                    int Threash = txtThreash.Text.ToInt(-1);
                    if (Threash == -1) Threash = 70;
                    int Segment = txtSegment.Text.ToInt(-1);
                    if (Segment == -1) Segment = 5;
                    if (txxtEditDirectory.Text != "" || txtsrcdir.Text != "")
                    {
                        DoCalcs(Filename, _TotalSecs, Target, Threash, Segment);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Please Select Input And Output Directory");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void txtEditFileSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string Root = key.GetValueStr("AdobeDestinationDir", "c:\\");
                key?.Close();
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Title = "Select a folder";
                folderBrowserDialog.InitialFolder = Root;
                folderBrowserDialog.AllowMultiSelect = false;
                folderBrowserDialog.DefaultFolder = Root;
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txxtEditDirectory.Text = folderBrowserDialog.SelectedFolder;
                    key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("AdobeDestinationDir", txxtEditDirectory.Text);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


        public void DoCalcs(string FileName, int TotalSecs, TimeSpan Cut, int threash, int SegSize)
        {
            try
            {
                TimeSpan ParseTarget = Cut;
                TimeSpan Totals = TimeSpan.FromSeconds(TotalSecs);
                TimeSpan prr = TimeSpan.Zero;
                lblStatus.Content = "Calculating Cut Points";
                while (true)
                {
                    double ti = Totals.TotalSeconds / Cut.TotalSeconds;
                    double p = ti - Math.Floor(ti);
                    prr = TimeSpan.FromSeconds(Cut.TotalSeconds) - TimeSpan.FromSeconds(Cut.TotalSeconds * p);
                    if (prr.TotalSeconds < threash) break;
                    Cut -= TimeSpan.FromSeconds(SegSize);
                }
                lblStatus.Content = "";
                if (prr.TotalSeconds > 0)
                {
                    List<(TimeSpan, TimeSpan)> TotalSpans = new List<(TimeSpan, TimeSpan)>();
                    TimeSpan Start = TimeSpan.Zero;
                    while (Start + Cut < Totals)
                    {
                        TotalSpans.Add((Start, Start + Cut));
                        Start = Start + Cut;
                    }
                    int xp = 1;
                    TimeSpan tix = TimeSpan.FromSeconds((Cut - prr).TotalSeconds);
                    TimeSpan tiy = TimeSpan.FromSeconds((Start + tix).TotalSeconds);
                    TotalSpans.Add((Start, TimeSpan.FromSeconds(Math.Floor(tiy.TotalSeconds))));
                    string filename = Filename;
                    ListOfCuts.Clear();
                    if (TotalSpans.Count > 0)
                    {
                        foreach (var span in TotalSpans)
                        {
                            ListOfCuts.Add(new VideoCutInfo($"{filename.Trim()} ", span.Item1, span.Item2, xp++));
                        }
                    }
                    lstSchedules.ItemsSource = ListOfCuts;
                    lstSchedules.Items.Refresh();
                    txtsize.Text = Cut.ToCustomTimeString();
                    txtMin.Text = prr.ToCustomTimeString();
                    txtFiles.Text = ListOfCuts.Count.ToString();
                    txtThreash.Text = TimeSpan.FromSeconds(threash).TotalSeconds.ToString();
                    txtTarget.Text = ParseTarget.ToCustomTimeString();
                    txtSegment.Text = TimeSpan.FromSeconds(SegSize).ToCustomTimeString();
                    LastSegment = SegSize;
                    LastTarget = ParseTarget;
                    LastThresh = threash;
                    BtnCalc.IsEnabled = false;
                    btnAccept.IsEnabled = true;
                    btnAcceptSelected.IsEnabled = true;
                    chkExportForTwitch.IsEnabled = btnAcceptSelected.IsEnabled;
                    btnSaveCut.IsEnabled = btnAcceptSelected.IsEnabled;
                }
            }

            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void frmVideoCutsEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Width += 15;
            Height += 58;
            RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
            string Root = key.GetValueStr("AdobeDestinationDir", "c:\\");
            key?.Close();
            txxtEditDirectory.Text = Root;
        }

        private void frmVideoCutsEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DoOnFinish?.Invoke();
        }

        private void frmVideoCutsEditor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    StackHeader0.Width = frmVideoCutsEditor.Width;
                    StackHeader1.Width = frmVideoCutsEditor.Width;
                    StackHeader2.Width = frmVideoCutsEditor.Width - 245;//0ack Panel for right controls
                    StackHeader4.Width = frmVideoCutsEditor.Width - 18;
                    StackHeader5.Width = stackheader4.Width;
                    Stackheader6.Height = StackHeader0.Height - 30;
                    brdControls.Width = frmVideoCutsEditor.Width - 26;
                    cnvcontrols.Width = brdControls.Width - 8;
                    lstItems.Width = StackHeader5.Width;
                    lstSchedules.Width = StackHeader2.Width;
                    lstSchedules.Height = frmVideoCutsEditor.Height - 261; // was 40
                    lstItems.Width = lstSchedules.Width;
                    Canvas.SetLeft(btnClose, frmVideoCutsEditor.Width - 134);
                    stkmain.Height = frmVideoCutsEditor.Height;//40
                    brd1.Height = StackHeader0.Height - 4;
                    txtsrcdir.Width = frmVideoCutsEditor.Width - 200;
                    Canvas.SetLeft(btnSelectSourceDir, frmVideoCutsEditor.Width - 73);
                    stkFilterControls.Height = frmVideoCutsEditor.Height - 224;//158
                    brdControls1.Height = stkFilterControls.Height - 3;
                    stsBar1.Width = frmVideoCutsEditor.Width - 9;
                    lblStatus.Width = stsBar1.Width - 2;
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void IsCalcEnabled_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    TimeSpan t1 = TimeSpan.FromSeconds(txtThreash.Text.ToInt());
                    TimeSpan t2 = txtTarget.Text.FromStrToTimeSpan();
                    TimeSpan t3 = txtSegment.Text.FromStrToTimeSpan();
                    if ((t1 != TimeSpan.Zero) && (t2 != TimeSpan.Zero) && (t3 != TimeSpan.Zero))
                    {
                        TimeSpan tp = TimeSpan.FromSeconds(LastThresh);
                        TimeSpan tx = LastTarget;
                        TimeSpan ty = TimeSpan.FromSeconds(LastSegment);
                        if ((tp != t1) || (tx != t2) || (ty != t3))
                        {
                            BtnCalc.IsEnabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void BtnCalc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TimeSpan t1 = TimeSpan.FromSeconds(txtThreash.Text.ToInt());
                TimeSpan t2 = txtTarget.Text.FromStrToTimeSpan();
                TimeSpan t3 = txtSegment.Text.FromStrToTimeSpan();
                if ((t1 != TimeSpan.Zero) && (t2 != TimeSpan.Zero) && (t3 != TimeSpan.Zero))
                {
                    int Thresh = t1.TotalSeconds.ToInt();
                    int Segment = t3.TotalSeconds.ToInt();
                    DoCalcs(Filename, _TotalSecs, t2, Thresh, Segment);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void AddItem(VideoCutInfo ctv)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string TwitchDir = key.GetValueStr("DestDirectoryTwitch");
                string destdir = (chkExportForTwitch.IsChecked.Value) ? TwitchDir : txxtEditDirectory.Text;
                key?.Close();
                DoAddRecord?.Invoke(true, false, false, -1, true, false, false, false,
                                        true, ctv.TimeFrom.ToFFmpeg(), (ctv.TimeTo - ctv.TimeFrom).ToFFmpeg()
                                        , txtsrcdir.Text,
                                        destdir + "\\" + ctv.FileName + $"part {ctv._CutNo}.mp4",
                                        null, null, chkExportForTwitch.IsChecked.Value);
                lblStatus.Content = $"Injecting {ctv.FileName} part {ctv._CutNo}";
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void SaveData()
        {
            try
            {
                int idx = 0;
                string sql = $"SELECT ID FROM AUTOEDITS WHERE SOURCE = @SRC";
                var res = ConnectionString.ExecuteScalar(sql, [("@SRC", txtsrcdir.Text)]).ToInt(-1);
                if (res != -1)
                {
                    sql = $"UPDATE AUTOEDITS SET DESTINATION = @P1, THRESHHOLD = @P2, TARGET = @P3, " +
                        "SEGMENT = @P4 WHERE ID = @P5";
                    ConnectionString.ExecuteScalar(sql, [("@P1", txxtEditDirectory.Text), ("@P2", txtThreash.Text),
                        ("@P3", txtTarget.Text), ("@P4", txtSegment.Text), ("@P5", res)]);
                }
                else
                {
                    sql = "INSERT INTO AUTOEDITS" +
                            "(SOURCE,DESTINATION,TARGET,SEGMENT,THRESHHOLD)  " +
                            "VALUES(@P1,@P2,@P3,@P4,@P5) RETURNING ID";
                    idx = ConnectionString.ExecuteScalar(sql, [("@P1", txtsrcdir.Text), ("@P2", txxtEditDirectory.Text),
                        ("@P3", txtTarget.Text), ("@P4", txtSegment.Text), ("@P5", txtThreash.Text)]).ToInt(-1);
                    if (idx != -1)
                    {

                    }

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SaveData {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                lblStatus.Content = "Injecting";
                foreach (var ctv in ListOfCuts)
                {
                    AddItem(ctv);
                }
                btnAccept.IsEnabled = false;
                lblStatus.Content = "";
                SaveData();
                txtSegment1.Text = txtSegment.Text;
                txtTarget1.Text = txtTarget.Text;
                txtThreash1.Text = txtThreash.Text;

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void mnuAddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (VideoCutInfo ctv in lstSchedules.SelectedItems)
                {
                    AddItem(ctv);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void BtnRestore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtSegment.Text = txtSegment1.Text;
                txtTarget.Text = txtTarget1.Text;
                txtThreash.Text = txtThreash1.Text;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnAcceptSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (VideoCutInfo ctv in lstSchedules.SelectedItems)
                {
                    AddItem(ctv);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnSaveCut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveData();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
