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
        AudioJoinerOnClose DoOnClose;
        int MaxFile = 0;
        private object thisLockduration = new object();
        private object thisLockduration2 = new object();
        private object thisLock = new object();
        List<Joiner> AudioJoiners = new List<Joiner>();
        TimeSpan Duration = TimeSpan.Zero;
        ObservableCollection<AudioJoinerInfo> MediaInfoTimes = new ObservableCollection<AudioJoinerInfo>();


        public AudioJoiner(AudioJoinerOnClose _OnClose)
        {
            InitializeComponent();
            DoOnClose = _OnClose;
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
               
                brdControls.Width = frmAudioJoiner.Width - 8;
                brd1.Width = brdControls.Width;
                brdFileInfo.Width = brdControls.Width;
                cnvcontrols.Width = brdControls.Width - 2;

                lstItems.Width = brd1.Width;
                lstSchedules.Width = lstItems.Width;

                lstSchedules.Height = frmAudioJoiner.Height - (367 - 177);
                Canvas.SetLeft(btnClose, frmAudioJoiner.Width - 124);
                brd1.Height = frmAudioJoiner.Height - 150;
                txtsrcdir.Width = frmAudioJoiner.Width - (724 - 542);
                Canvas.SetLeft(btnSelectSourceDir, frmAudioJoiner.Width - 53);
                
               
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
                lstSchedules.ItemsSource = MediaInfoTimes;
                RegistryKey key1 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string InitFolder = key1.GetValueStr("AudioJoinerDestDir", "c:\\");
                key1?.Close();
                txtDestDir.Text = InitFolder;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            try
            {
                DoOnClose?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

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
                    var bridge2 = new ffmpegbridge();
                    bool skip = false;
                    if (File.Exists(destfname))
                    {
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
                        ReadFile(file).ConfigureAwait(false);
                    }
                }
                while (MaxFile > 0)
                {
                    Thread.Sleep(100);
                }
                btnRename.IsEnabled = true;
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
                    key2?.Close();
                    folder = folderBrowserDialog.SelectedFolder;
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
                        this.Dispatcher.Invoke(() =>
                        {
                            lstSchedules.Items.Refresh();
                        });
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
                        this.Dispatcher.Invoke(() =>
                        {
                            lstSchedules.Items.Refresh();
                        });
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
                        this.Dispatcher.Invoke(() =>
                        {
                            lstSchedules.Items.Refresh();
                        });

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
                            this.Dispatcher.Invoke(() =>
                            {
                                lstSchedules.Items.Refresh();
                            });
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
            Close();
        }
    }



}
