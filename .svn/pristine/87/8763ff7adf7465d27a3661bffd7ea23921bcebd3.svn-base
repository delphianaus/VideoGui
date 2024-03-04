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
        public AddRecordDelegate DoAddRecord;
        List<VideoCutInfo> ListOfCuts = new List<VideoCutInfo>();
        public VideoCutsEditor(AddRecordDelegate _DoAddRecord, OnFinish _DoOnFinish)
        {
            InitializeComponent();
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
                    Filename = txtsrcdir.Text.Split("\\").ToList().LastOrDefault();
                    var FileIndexer = new ffmpegbridge();
                    List<string> files = Directory.EnumerateFiles(txtsrcdir.Text,"*.mp4", SearchOption.TopDirectoryOnly).ToList();
                    FileIndexer.ReadDuration(files);
                    while (!FileIndexer.Finished)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(100);
                    }
                    var TotalSecs = FileIndexer.GetDuration().TotalSeconds;

                    FileIndexer = null;
                    _TotalSecs = Math.Truncate(TotalSecs).ToInt();
                    TimeSpan Target = TimeSpan.FromMinutes(22);
                    if (txxtEditDirectory.Text != "" || txtsrcdir.Text != "")
                    {
                        DoCalcs(Filename, _TotalSecs, Target, 70, 5);
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
                    key.SetValue("AdobeDestinationDir", txtsrcdir.Text);
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
                string destdir = key.GetValueStr("AdobeDestinationDir", "c:\\Adobe");
                key?.Close();
                DoAddRecord?.Invoke(true, false, false, false, true, false, false, false,
                                        true, ctv.TimeFrom.ToFFmpeg(), (ctv.TimeTo - ctv.TimeFrom).ToFFmpeg()
                                        , txtsrcdir.Text,
                                        destdir + "\\" + ctv.FileName + $"part {ctv._CutNo}.mp4");
                System.Windows.Forms.Application.DoEvents();
                lblStatus.Content = $"Injecting {ctv.FileName} part {ctv._CutNo}";
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
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
                foreach(VideoCutInfo ctv in lstSchedules.SelectedItems)
                {
                    AddItem(ctv);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
