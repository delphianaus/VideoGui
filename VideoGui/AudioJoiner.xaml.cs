using CliWrap;
using FolderBrowserEx;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VideoGui.Models;
using VideoGui.Models.delegates;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Path = System.IO.Path;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for AudioJoiner.xaml
    /// </summary>
    public partial class AudioJoiner : Window
    {
        int MaxFile = 0;
        private object thisLockduration = new object();
        private object thisLockduration2 = new object();
        private object thisLock = new object();
        List<Joiner> AudioJoiners = new List<Joiner>();
        TimeSpan Duration = TimeSpan.Zero;
        public bool IsClosing = false, IsClosed = false;
        ObservableCollection<AudioJoinerInfo> MediaInfoTimes = new ObservableCollection<AudioJoinerInfo>();
        public CancellationTokenSource cancel = new CancellationTokenSource();
        bool Ready = false;
        DispatcherTimer LocationChanger = new(), LocationChangedTimer = new();
        public static readonly DependencyProperty SrcFileNameWidthProperty =
            DependencyProperty.Register(nameof(SrcFileNameWidth), typeof(double),
                typeof(AudioJoiner),
                new FrameworkPropertyMetadata(340.0));

        public double SrcFileNameWidth
        {
            get => (double)GetValue(SrcFileNameWidthProperty);
            set => SetValue(SrcFileNameWidthProperty, value);
        }
        public AudioJoiner(OnFinishIdObj _OnClose)
        {
            InitializeComponent();
            // marked for mcu listbox upgrade
            Closing += (s, e) => { IsClosing = true; };
            Closed += (s, e) => { IsClosed = true; _OnClose?.Invoke(this,-1); };
        }



        public async Task ReadFile(string filePath, bool UseVideoDuration = false)
        {
            try
            {
                MaxFile++;
                TimeSpan TBL = TimeSpan.Zero;
                var mw1 = new MediaInfo.MediaInfo();
                mw1.Open(filePath);
                var s = mw1.Get(MediaInfo.StreamKind.Other, 0, "TimeCode_FirstFrame");
                var e = mw1.Get(MediaInfo.StreamKind.Other, 0, "TimeCode_LastFrame");
                if ((s == "") || (e == ""))
                {
                    var videoduration = mw1.Get(MediaInfo.StreamKind.General, 0, "Duration");
                    if (videoduration != "")
                    {
                        float flt = 0;
                        float.TryParse(videoduration, out flt);
                        if (flt > 0)
                        {
                            flt = flt / 1000;
                            lock (thisLockduration)
                            {
                                Duration += TimeSpan.FromSeconds(flt);
                                TBL = TimeSpan.FromSeconds(flt);
                            }
                        }
                    }
                }
                else
                {
                    TimeSpan timeframe = s.FromFFmpegTime();
                    TimeSpan Timeframeend = e.FromFFmpegTime();
                    Timeframeend -= timeframe;
                    lock (thisLockduration)
                    {
                        Duration += Timeframeend;
                        TBL = Timeframeend;
                    }
                }

                mw1.Close();
                lock (thisLockduration2)
                {
                    bool found = false;
                    for (int i = 0; i < MediaInfoTimes.Count - 1; i++)
                    {
                        if (MediaInfoTimes[i].FileName == filePath)
                        {
                            found = true;
                            MediaInfoTimes[i].TimeData = TBL;
                            break;
                        }
                    }
                    if (!found)
                    {

                        var mm = new AudioJoinerInfo(filePath, "", TBL);
                        MediaInfoTimes.Add(mm);
                    }

                }
                MaxFile--;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                if (MaxFile > 0)
                {
                    MaxFile--;
                }
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded && Ready)
                {
                    ResizeWindows(e.NewSize.Width, e.NewSize.Height, e.WidthChanged, e.HeightChanged);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                msuAudioJoiner.ItemsSource = MediaInfoTimes;
                RegistryKey key1 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string InitFolder = key1.GetValueStr("AudioJoinerDestDir", "c:\\");
                key1?.Close();
                txtDestDir.Text = InitFolder;

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
                        key2?.SetValue("AJleft", Left);
                        key2?.SetValue("AJtop", Top);
                        key2?.Close();
                    };
                    LocationChangedTimer.Start();
                };
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void LocationChanger_Tick(object? sender, EventArgs e)
        {
            try
            {
                LocationChanger.Stop();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                var _width = key.GetValue("AJWidth", ActualWidth).ToDouble();
                var _height = key.GetValue("AJHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("AJleft", Left).ToDouble();
                var _top = key.GetValue("AJtop", Top).ToDouble();
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

        public void ResizeWindows(double _w, double _h, bool WidthChanged = false,
             bool HeightChanged = false)
        {
            try
            {
                if (WidthChanged && Ready)
                {
                    msuAudioJoiner.Width = _w - 23;
                    brdControls.Width = _w - 26;
                    brdFileInfo.Width = _w - 26;
                    Canvas.SetLeft(btnSelectSourceDir, _w - 63);
                    Canvas.SetLeft(btnSetDetDir, _w - 171);
                    Canvas.SetLeft(btnClose, _w - 133);
                    txtDestDir.Width = _w - 413;
                    txtsrcdir.Width = _w - 186;
                    var srw = _w - 300;
                    SrcFileNameWidth = srw < 145 ? 145 : srw;
                }
                if (HeightChanged && Ready)
                {
                    msuAudioJoiner.Height = _h - (347 - 207) - 26;
                }

                if (HeightChanged || WidthChanged && Ready)
                {
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key?.SetValue("AJWidth", ActualWidth);
                    key?.SetValue("AJHeight", ActualHeight);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ReiszeWindows {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public int numberoffiles = 0;

        public async Task GetFiles(string SourceDir)
        {
            try
            {
                MediaInfoTimes.Clear();
                var files = Directory.EnumerateFiles(SourceDir, "*.mp4", SearchOption.AllDirectories).ToList();
                foreach (string file in files)
                {
                    var fname = System.IO.Path.GetFileNameWithoutExtension(file);
                    var dir = System.IO.Path.GetDirectoryName(file);
                    string ddir = txtDestDir.Text;
                    string destfname = System.IO.Path.Combine(ddir, fname) + ".mp4";
                    var fnmp3 = System.IO.Path.Combine(dir, fname) + ".mp3";
                    var bridge = new ffmpegbridge();
                    await bridge.ReadFile(fname);
                    var SRC_TIME = bridge.GetDuration();
                    bool skip = false;
                    if (File.Exists(destfname))
                    {
                        var bridge2 = new ffmpegbridge();
                        await bridge2.ReadFile(destfname);
                        var SRC_TIME2 = bridge2.GetDuration();
                        TimeSpan S1 = SRC_TIME.Subtract(TimeSpan.FromSeconds(15));
                        TimeSpan S2 = SRC_TIME.Add(TimeSpan.FromSeconds(15));
                        if (SRC_TIME2.IfBetweenTimeSpans(S1, S2))
                        {
                            skip = true;
                        }
                    }

                    if (!skip && File.Exists(fnmp3))
                    {
                        while (MaxFile > 16)
                        {
                            Thread.Sleep(25);
                        }
                        numberoffiles++;
                        ReadFile(file).ConfigureAwait(false);
                    }
                }
                while (MaxFile > 0)
                {
                    Thread.Sleep(100);
                }
                btnRename.IsEnabled = true;
                //cancel.Cancel();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


        private void btnSelectSourceDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key1 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string InitFolder = key1.GetValueStr("AudioJoinerSourceDir", "c:\\");
                key1?.Close();
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Title = "Select Source folder";
                folderBrowserDialog.InitialFolder = InitFolder;
                folderBrowserDialog.DefaultFolder = InitFolder;
                folderBrowserDialog.AllowMultiSelect = false;

                var folder = "";
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key2.SetValue("AudioJoinerSourceDir", folderBrowserDialog.SelectedFolder);
                    folder = folderBrowserDialog.SelectedFolder;
                    string Dest = Path.Combine(folder, "dest");
                    if (!Directory.Exists(Dest))
                    { 
                        Directory.CreateDirectory(Dest);
                    }
                    key2.SetValue("AudioJoinerDestDir", Dest);
                    key2?.Close();
                    txtDestDir.Text = Dest;
                    txtsrcdir.Text = folder;
                    GetFiles(folder).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnSetDetDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key1 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string InitFolder = key1.GetValueStr("AudioJoinerDestDir", "c:\\");
                key1?.Close();
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Title = "Select a destination folder";
                folderBrowserDialog.InitialFolder = InitFolder;
                folderBrowserDialog.AllowMultiSelect = false;
                var folder = "";
                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key2.SetValue("AudioJoinerDestDir", folderBrowserDialog.SelectedFolder);
                    key2?.Close();
                    txtDestDir.Text = folderBrowserDialog.SelectedFolder;
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }



        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MaxFile = 0;
                foreach (var f in MediaInfoTimes)
                {
                    while (MaxFile >= 4)
                    {
                        Thread.Sleep(250);
                        System.Windows.Forms.Application.DoEvents();
                    }
                    MaxFile++;
                    var NewFileName = txtDestDir.Text + "\\" + Path.GetFileNameWithoutExtension(f.FileName) + ".mp4";
                    AudioJoiners.Add(new Joiner(f.FileName, NewFileName, DoOnStart, DoOnProgress, DoOnStop, DoOnAviDemuxStart, DoOnAviDemuxEnd));// (mwx.FileName, mwx.FileName);
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private async Task DoVideoCheck(int i, TimeSpan Date, string DestinationFile, string SourceFile)
        {
            var bridge = new ffmpegbridge();
            bridge.ReadFile(DestinationFile).ConfigureAwait(true);
            var desttime = bridge.GetDuration();
            if (desttime == Date)
            {
                MediaInfoTimes[i].Status = $"Converted OK";
            }
            else
            {
                TimeSpan P = desttime - MediaInfoTimes[i].TimeData;
                if (P.TotalSeconds < 1)
                {
                    MediaInfoTimes[i].Status = $"Converted OK";
                }
                else MediaInfoTimes[i].Status = $"Converted???";
            }
            var py = SourceFile.Replace(".mp4", ".py");
            if (File.Exists(py))
            {
                File.Delete(py);
            }
            bridge = null;

        }
        private void DoOnAviDemuxEnd(string SourceFileName, string DestinationFile, int exitcode)
        {
            lock (thisLock)
            {
                for (int i = 0; i < MediaInfoTimes.Count; i++)
                {
                    if (MediaInfoTimes[i].FileName == SourceFileName)
                    {
                        MediaInfoTimes[i].Status = $"Script Ended";
                        //Task.Run(async () =>
                        // {
                        //     DoVideoCheck(i, MediaInfoTimes[i].TimeData, DestinationFile, SourceFileName);
                        // });
                        break;
                    }
                }
                for (int i = 0; i < AudioJoiners.Count - 1; i++)
                {
                    if (AudioJoiners[i].SourceFile == SourceFileName)
                    {
                        Thread.Sleep(500);
                        AudioJoiners[i].Dispose();
                        AudioJoiners.RemoveAt(i);
                        MaxFile--;
                    }
                }
            }
        }

        private void DoOnAviDemuxStart(string SourceFileName)
        {
            MaxFile++;
            lock (thisLock)
            {

                for (int i = 0; i < MediaInfoTimes.Count; i++)
                {
                    if (MediaInfoTimes[i].FileName == SourceFileName)
                    {
                        MediaInfoTimes[i].Status = $"Script Starting";
                        
                        break;
                    }
                }
            }
        }

        private void DoOnStop(string SourceFileName)
        {
            lock (thisLock)
            {
                for (int i = 0; i < MediaInfoTimes.Count; i++)
                {
                    if (MediaInfoTimes[i].FileName == SourceFileName)
                    {
                        MediaInfoTimes[i].Status = $"Processed";
                       
                        break;
                    }
                }
            }
        }

        private void DoOnProgress(string Progress, string SourceFileName)
        {
            lock (thisLock)
            {
                for (int i = 0; i < MediaInfoTimes.Count; i++)
                {
                    if (MediaInfoTimes[i].FileName == SourceFileName)
                    {
                        int idx = Progress.IndexOf("done");
                        string s = "";
                        if (idx != -1)
                        {
                            s = Progress.Substring(0, idx - 1).Trim();
                            s = $"Converting {s}";
                        }

                        MediaInfoTimes[i].Status = s;
                        

                        break;
                    }
                }
            }
        }

        private void DoOnStart(string SourceFileName)
        {
            try
            {
                lock (thisLock)
                {

                    for (int i = 0; i < MediaInfoTimes.Count; i++)
                    {
                        if (MediaInfoTimes[i].FileName == SourceFileName)
                        {
                            MediaInfoTimes[i].Status = "Starting";
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            cancel.Cancel();
            Close();
        }
    }



}
