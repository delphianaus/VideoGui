using CliWrap;
using FirebirdSql.Data.FirebirdClient;
using FolderBrowserEx;
using Microsoft.Win32;
using Nancy.Routing.Trie.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using VideoGui.Models;
using VideoGui.Models.delegates;
using static System.Net.WebRequestMethods;
using static VideoGui.ffmpeg.Probe.FormatModel;
using FolderBrowserDialog = FolderBrowserEx.FolderBrowserDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using Wpf.Ui.Controls;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for VideoCutsEditor.xaml
    /// </summary>
    public partial class VideoCutsEditor : FluentWindow
    {

        string Filename = "", XML_Filename = "", XML_Dest = "";
        int _TotalSecs = 0;
        string ConnectionString = "";
        public AddRecordDelegate DoAddRecord;
        public ObservableCollection<VideoCutInfo> ListOfCuts = new();

        public static readonly DependencyProperty SrcFileNameWidthProperty =
            DependencyProperty.Register(nameof(SrcFileNameWidth), typeof(double),
                typeof(VideoCutsEditor),
                new FrameworkPropertyMetadata(200.0));

        public double SrcFileNameWidth
        {
            get => (double)GetValue(SrcFileNameWidthProperty);
            set => SetValue(SrcFileNameWidthProperty, value);
        }
        public VideoCutsEditor(AddRecordDelegate _DoAddRecord, OnFinishIdObj _DoOnFinish, string connectionString)
        {
            InitializeComponent();
            // this form is marked for mcu listbox upgrade
            ConnectionString = connectionString;
            DoAddRecord = _DoAddRecord;
            Closed += (s, e) => { _DoOnFinish?.Invoke(this, -1); };
        }

        TimeSpan LastTarget = TimeSpan.Zero; // 22 mins
        int LastSegment = -1; // 5 seconds
        int LastThresh = -1; // 70 seconds
        bool Ready = false;
        DispatcherTimer LocationChanger = new(), LocationChangedTimer = new();

        private void btnSelectSourceDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string Root = key.GetValueStr("CutSourceDirectory", "c:\\");
                key?.Close();



                if (tbSource.IsChecked.Value)
                {
                    cnvControls.Visibility = Visibility.Visible;
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
                            CancellationTokenSource cts = new();
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
                else
                {

                    XML_Dest = "";
                    cnvControls.Visibility = Visibility.Hidden;
                    var fld = new OpenFileDialog();
                    fld.Filter = "xml|*.xml";
                    // fld.DefaultExt = "*.xml";
                    // fld.DefaultDirectory = Root;
                    //fld.Multiselect = false;
                    fld.Title = "Select Final Pro XML Source";
                    var fd = fld.ShowDialog();
                    if ((fd != null) && (fd.Value == true))
                    {
                        txtsrcdir.Text = fld.FileName;
                        XDocument doc = XDocument.Load(fld.FileName);
                        var _fps = doc
                            .Descendants("clipitem")
                            .Select(c => new
                            {
                                PathUrl = (string)c.Element("file")?.Element("timebase")
                                  ?? (string)c.Descendants("timebase").FirstOrDefault(),
                            }).FirstOrDefault().ToString();
                        var urlLocation = doc
                            .Descendants("clipitem")
                            .Select(c => new
                            {
                                PathUrl = (string)c.Element("file")?.Element("pathurl")
                                  ?? (string)c.Descendants("pathurl").FirstOrDefault(),
                            }).FirstOrDefault().ToString();

                        var clipItems = doc
                            .Descendants("clipitem")
                            .Select(c => new
                            {
                                Id = (string)c.Attribute("id"),
                                Name = (string)c.Element("name"),
                                Start = (int?)c.Element("start"),
                                End = (int?)c.Element("end"),
                                In = (int?)c.Element("in"),
                                Out = (int?)c.Element("out"),
                            })
                            .ToList();
                        if (clipItems != null)
                        {
                            int clipno = 0;
                            bool newclip = false;
                            int prevend = 0;
                            List<AdobeClipData> adobeclips = new List<AdobeClipData>();
                            var DestPat = urlLocation.Split('/').ToList();
                            if (DestPat.Count > 0)
                            {
                                DestPat.RemoveAt(DestPat.Count - 1);
                                XML_Dest = Uri.UnescapeDataString(DestPat.LastOrDefault().ToString());
                            }
                            

                            /*List<string> times = new List<string>();
                            foreach (var clip in clipItems)
                            {
                                int start = clip.Start.Value;
                                int end = clip.End.Value;
                                int In = clip.In.Value;
                                int Out = clip.Out.Value;

                                times.Add($"{start},{end},{In},{Out}");
                            }*/
                            // System.IO.File.AppendAllLines(@"c:\videogui\times.csv",times.ToArray());


                            List<string> names = new();
                            foreach (var clip in clipItems)
                            {

                                string nameid = clip.Name.ToString();
                                if (names.IndexOf(nameid) != -1)
                                {
                                    continue;
                                }
                                names.Add(nameid);
                                int start = clip.Start.Value;
                                int end = clip.End.Value;
                                int In = clip.In.Value;
                                int Out = clip.Out.Value;
                                int Startframe = start + In;
                                int Endframe = start + Out;

                                if (adobeclips.Count == 0)
                                {
                                    adobeclips.Add(new AdobeClipData(clipno++, Startframe, Endframe));
                                }
                                else if (newclip || In > 0)
                                {
                                    adobeclips.Add(new AdobeClipData(clipno++, start, end));
                                }
                                else adobeclips.LastOrDefault().end = Endframe;
                                newclip = Startframe != prevend;
                                prevend = Endframe;
                            }

                            int totalframes = 0;
                            List<TimeSpan> frames = new List<TimeSpan>();
                            double tts = 0;
                            foreach (var aclips in adobeclips)
                            {
                                int fm = aclips.end - aclips.start;
                                double totalsecs = fm / 50;
                                TimeSpan tfs = TimeSpan.FromSeconds(totalsecs);
                                TimeSpan tfs1 = TimeSpan.FromSeconds(aclips.start / 50);
                                TimeSpan tfs2 = TimeSpan.FromSeconds(aclips.end / 50);
                                frames.Add(tfs);
                                tts += totalsecs;
                            }




                            var filenamelist = urlLocation.Replace("%20", " ").Replace("%3a", ":").
                              Replace("file://xctkhost/", "").Replace(@"/", @"\").ToString();
                            XML_Filename = filenamelist.ToString().Replace("{ PathUrl = ", "").Trim();


                            int idx = XML_Filename.IndexOf(@"\GX_");
                            if (idx != -1)
                            {
                                XML_Filename = XML_Filename.Substring(0, idx);
                            }

                            string fn = XML_Filename.Split(@"\").ToList().LastOrDefault() as string;
                            double fps = _fps.ToDouble(50);
                            clipno = 1;
                            foreach (var _aclip in adobeclips)
                            {
                                // filename is the source dir, last part of //
                                TimeSpan _Start = TimeSpan.FromSeconds(_aclip.start / fps);
                                TimeSpan _End = TimeSpan.FromSeconds(_aclip.end / fps);
                                var vcut = new VideoCutInfo(fn.Trim(), _Start, _End, clipno++);
                                ListOfCuts.Add(vcut);
                            }

                            TimeSpan TotalD = TimeSpan.FromSeconds(tts);
                            lblTotalTime.Content = TotalD.ToFFmpeg().Substring(0, 8);
                            msuVideoCuts.ItemsSource = ListOfCuts;
                            btnAccept.IsEnabled = true;
                            btnAcceptSelected.IsEnabled = true;
                        }

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
                    msuVideoCuts.ItemsSource = ListOfCuts;
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
            LocationChanger.Interval = TimeSpan.FromMilliseconds(10);
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
                    key2?.SetValue("VCEleft", Left);
                    key2?.SetValue("VCEtop", Top);
                    key2?.Close();
                };
                LocationChangedTimer.Start();
            };

        }

        private void LocationChanger_Tick(object? sender, EventArgs e)
        {
            try
            {
                LocationChanger.Stop();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                var _width = key.GetValue("VCEWidth", ActualWidth).ToDouble();
                var _height = key.GetValue("VCEHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("VCEleft", Left).ToDouble();
                var _top = key.GetValue("VCEtop", Top).ToDouble();
                key?.Close();
                Left = (Left != _left && _left != 0) ? _left : Left;
                Top = (Top != _top && _top != 0) ? _top : Top;
                Width = (ActualWidth != _width && _width != 0) ? _width : Width;
                Height = (ActualHeight != _height && _height != 0) ? _height : Height;
                Ready = true;
                ResizeWindows(Width, Height, true, true);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LocationChanger_Tick {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        public void ResizeWindows(double _w, double _h, bool WidthChanged = false, bool HeightChanged = false)
        {
            try
            {
                if (WidthChanged && Ready && IsLoaded)
                {
                    msuVideoCuts.Width = (tbSource.IsChecked.Value) ? _w - 245 : _w - 16;
                    brdControls.Width = _w - 20;
                    stsBar1.Width = _w - 19;
                    lblStatus.Width = _w - 19;
                    Canvas.SetLeft(btnClose, _w - 133);
                    Canvas.SetLeft(tbSource, _w - 235);
                    Canvas.SetLeft(btnSelectSourceDir, _w - 63);
                    txtsrcdir.Width = _w - 186;
                    Canvas.SetLeft(lblTotalTime, _w - 95);
                    Canvas.SetLeft(lblDuration, _w - 150);
                    Canvas.SetLeft(btnEditFileSelect, _w - 180);
                    txxtEditDirectory.Width = _w - 305;
                    var _ww = _w - 465;
                    if (_ww < 255) _ww = 255;
                    SrcFileNameWidth = _ww;
                }
                if (HeightChanged && Ready && IsLoaded)
                {
                    msuVideoCuts.Height = _h - (609 - 409) - 24;
                    brdControls1.Height = _h - (609 - 409) - 35;
                }
                if (HeightChanged || WidthChanged && Ready && IsLoaded)
                {
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key?.SetValue("VCEWidth", ActualWidth);
                    key?.SetValue("VCEHeight", ActualHeight);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ReiszeWindows {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }


        private void frmVideoCutsEditor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded && Ready)
                {
                    ResizeWindows(e.NewSize.Width, e.NewSize.Height,
                        e.WidthChanged, e.HeightChanged);
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
                string destdir = (chkExportForTwitch.IsChecked.Value) ? TwitchDir : 
                   Path.Combine(txxtEditDirectory.Text,XML_Dest);
                if (!Directory.Exists(destdir))
                {
                   Directory.CreateDirectory(destdir);
                }
                key?.Close();


                DoAddRecord?.Invoke(chkExportForTwitch.IsChecked.Value, true, false, false, -1, true, false, false, false,
                                        true, ctv.TimeFrom.ToFFmpeg(), ctv.TimeTo.ToFFmpeg()
                                        , (tbSource.IsChecked.Value) ? txtsrcdir.Text : XML_Filename,
                                        destdir + "\\" + ctv.FileName.Trim() + $" Part {ctv._CutNo}.mp4",
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
                foreach (VideoCutInfo ctv in msuVideoCuts.SelectedItems)
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
                foreach (VideoCutInfo ctv in msuVideoCuts.SelectedItems)
                {
                    AddItem(ctv);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void tbSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lblSourceInfo.Content = (tbSource.IsChecked.Value) ? "Source Directory" : "Source File";
                cnvControls.Visibility = (tbSource.IsChecked.Value) ? Visibility.Visible : Visibility.Hidden;
                if (tbSource.IsChecked.Value)
                {
                    brdControls1.Visibility = Visibility.Visible;
                }
                else
                {
                    brdControls1.Visibility = Visibility.Hidden;
                }

                Width++;
                Width--;

            }
            catch (Exception ex)
            {
                ex.LogWrite($"tbSource_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
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
