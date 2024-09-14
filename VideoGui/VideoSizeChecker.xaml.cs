using MediaInfo.Model;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VideoGui.ffmpeg;
using VideoGui.ffmpeg.Streams.Video;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for VideoSizeChecker.xaml
    /// </summary>
    public partial class VideoSizeChecker : Window
    {
        Models.delegates.CompairFinished OnFinish;
        public ProgressWindow.CancelScan OnCancel;// = new(OnCancelation);
        bool canclose = true;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private List<VideoSizeInfo> sourcefiles = new();
        string CompletedDir;
        public VideoSizeChecker(Models.delegates.CompairFinished _OnFinish)
        {
            InitializeComponent();
            OnFinish = _OnFinish;
            ScanDestDir().ConfigureAwait(false);
        }

        public void DeleteIfExists(string filename)
        {
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.SetAttributes(filename, System.IO.FileAttributes.Normal);
                    System.IO.File.Delete(filename);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnFinish?.Invoke();
        }

        public void MoveIfExists(string filename, string newfile)
        {
            try
            {
                filename = filename.Replace("\"", "");
                newfile = newfile.Replace("\"", "");
                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.SetAttributes(filename, System.IO.FileAttributes.Normal);
                    DeleteIfExists(newfile);
                    System.IO.File.Move(filename, newfile);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

       
        private async Task DoFileProcessingAsync(string filename)
        {
            try
            {
                string AppPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                string defaultdrive = System.IO.Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null);
                string finished_dir = LoadedKey ? (string)key.GetValue("CompDirectory", defaultdrive) : string.Empty;
                key.Close();
                var FileConverter = new ffmpegbridge();
                (ffmpeg.Streams.Video.IVideoStream videoStream, ffmpeg.Streams.Audio.IAudioStream AudioStream,TimeSpan Dur) = FileConverter.ReadMediaFile(filename);
                FileConverter = null;
                if (videoStream != null)
                {
                    if (videoStream.Width > 720)
                    {
                        int vswidth = videoStream.Width;
                        sourcefiles.Add(new(filename, finished_dir, vswidth));
                    }
                }


            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void DoMutliThreadedScaning(List<string> SourceList, string SourceDirectory)
        {
            try
            {
                List<Task> MultiThreads = new();
                int InCompletedTasks = 0, index = 0; 
                ProgressWindow pgs;
                pgs = new ProgressWindow(OnCancel, "Scanning Files", SourceDirectory, SourceList.Count);
                pgs.Show();
                canclose = false;
                bool Hidden = true;
                Hide();
                
                foreach (string filename in SourceList)
                {
                    if (cancellationTokenSource.IsCancellationRequested) break;
                    DateTime createtime = new System.IO.FileInfo(filename).CreationTime;
                    int days = Math.Abs((createtime - DateTime.Now).Days);
                    if (File.Exists(filename) && (days < 31))
                        MultiThreads.Add(Task.Run(() => DoFileProcessingAsync(filename.QuoteStr())));
                    pgs.UpdateProgress(index, filename);
                    InCompletedTasks = MultiThreads.Where(downloader => !downloader.IsCompleted).Count();
                    while (InCompletedTasks >= 4)
                    {
                        InCompletedTasks = MultiThreads.Where(downloader => !downloader.IsCompleted).Count();
                        Thread.Sleep(50);
                    }
                    index++;
                }
                pgs.Close();
                canclose = true;
                if (Hidden) Show();
                BtnRemoveFiles.IsEnabled = sourcefiles.Count != 0;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public async Task ScanDestDir()
        {
            try
            {
                if (CompletedDir != string.Empty)
                {
                    ProgressWindow pgs;
                    List<string> SourceList = new List<string>();
                    SourceList = Directory.EnumerateFiles(CompletedDir, "*.*", SearchOption.AllDirectories).
                         Where(s => s.EndsWith(".avi") || s.EndsWith(".mkv") || s.EndsWith(".mp4") || s.EndsWith(".m2ts")).ToList<string>();
                    DoMutliThreadedScaning(SourceList, CompletedDir);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void DoScan()
        {
            try
            {
                //btnscan.IsEnabled = false;
                LstBoxFiles.ItemsSource = sourcefiles;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null);
                List<string> DestList = new List<string>();
                CompletedDir = LoadedKey ? (string)key.GetValue("CompDirectory", string.Empty) : string.Empty;
                key.Close();
                ScanDestDir().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void BtnRemoveFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null);
                List<string> DestList = new List<string>();
                string sourcedir = LoadedKey ? (string)key.GetValue("SourceDirectory", string.Empty) : string.Empty;
                key.Close();
                string srcfile = "", dstfile = "";
                bool update = false, Hidden = false;
                ProgressWindow pgs;
                pgs = new ProgressWindow(OnCancel, "Moving Files From " + CompletedDir, sourcedir, sourcefiles.Count);
                pgs.Show();
                canclose = false;
                Hidden = true;
                Hide();
                int index = 0;
                foreach (VideoSizeInfo file in sourcefiles.Where(file => !file.ExcludeFile))
                {
                    if (file.FileExistsInFinishedDir)
                    {
                        dstfile = sourcedir + "\\" + file.SourceFile;
                        srcfile = file.SourceFilePath + "\\" + file.SourceFile; // the finished dir
                        DeleteIfExists(file.SourceFilePath + "\\" + file.SourceFile);// delete from done dir.
                        MoveIfExists(srcfile, dstfile);
                    }
                    else
                    {
                        dstfile = sourcedir + "\\" + file.SourceFile;
                        srcfile = CompletedDir + "\\" + file.SourceFile;// is the done dir 
                        MoveIfExists(srcfile, dstfile);
                    }
                    pgs.UpdateProgress(index, CompletedDir + "\\" + file.SourceFile);
                    index++;
                }
                pgs.Close();
                if (Hidden) this.Show();
                canclose = true;

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
