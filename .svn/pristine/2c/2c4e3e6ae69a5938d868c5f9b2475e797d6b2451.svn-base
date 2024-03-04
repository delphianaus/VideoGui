using Microsoft.Win32;
using Nancy.Routing.Trie.Nodes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
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
using VideoGui.ffmpeg;
using VideoGui.ffmpeg.Streams.Audio;
using VideoGui.ffmpeg.Streams.Video;
using Path = System.IO.Path;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for SourceDestComo.xaml
    /// </summary>
    public partial class SourceDestComp : Window
    {
        public string Completed_Directory = "";
        private List<FileCompares> sourcefiles = new();
        public ProgressWindow.CancelScan OnCancel;// = new(OnCancelation);
        bool canclose = true;
        string CompletedDir = "";
        public bool formclosed = false;
        public Models.delegates.CompairFinished OnFinish;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private List<string> ErrorList = new();
        private List<FileCache> sourcefilesCache = new();
        private object LockObjectSourceList = new();
        public SourceDestComp(Models.delegates.CompairFinished _OnFinish, string _SourceDirectory = "")
        {
            InitializeComponent();
            OnCancel = new(OnCancelation);
            btnRemoveBadDest.IsEnabled = false;
            BtnPurge.IsEnabled = false;
            Btncompfiles.IsEnabled = false;
            OnFinish = _OnFinish;
            RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
            string ErrorDir = key.GetValueStr("ErrorDirectory", string.Empty);
            if (_SourceDirectory == string.Empty)
            {
                Completed_Directory = key.GetValueStr("CompDirectory", string.Empty);
            }
            else Completed_Directory = _SourceDirectory;
            key?.Close();
            string FileNameOnly;
            List<string> Error_List = Directory.EnumerateFiles(ErrorDir, "*.*", SearchOption.AllDirectories).ToList<string>();
            foreach (string Filename in Error_List)
            {
                FileNameOnly = System.IO.Path.GetFileNameWithoutExtension(Filename);
                if (ErrorList.IndexOf(FileNameOnly) == -1)
                {
                    ErrorList.Add(FileNameOnly);
                }
            }
        }


        public string GetAppPath()
        {
            try
            {
                string res = "";
                res = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }

        private void OnCancelation()
        {
            try
            {
                cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public List<FileCache> LoadCache(string cachename)
        {
            try
            {
                List<FileCache> sourcefilesCache = new();
                string source = "";
                if (System.IO.File.Exists(cachename))
                {
                    using (var sr = new StreamReader(cachename))
                    {
                        source = sr.ReadToEnd();
                        sr.Close();
                    }
                    if (source == "") return sourcefilesCache;
                    List<FileCache> fdd = Helper.AsObjectList<FileCache>(source);
                    foreach (FileCache fcc in fdd)
                    {
                        if (System.IO.File.Exists(fcc.SourceFilePath + "\\" + fcc.SourceFile))
                        {
                            string filename = fcc.SourceFilePath + "\\" + fcc.SourceFile;
                            double fsq = new System.IO.FileInfo(filename).Length;
                            DateTime SouceFileDateTime1 = new System.IO.FileInfo(filename).LastWriteTime;
                            if ((fcc.SourceFileSize == fsq) && (fcc.SouceFileDateTime.Date == SouceFileDateTime1.Date))
                            {
                                sourcefilesCache.Add(new FileCache(filename, fcc.SourceFileLength, SouceFileDateTime1, fsq));
                            }
                        }
                    }
                    return sourcefilesCache;
                }
                else return sourcefilesCache;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return sourcefilesCache;
            }

        }
        public FileCompares GetCachedRecord(string filename)
        {
            string SubPath = System.IO.Path.GetFileName(filename);
            if (sourcefilesCache == null)
                foreach (FileCache fcc in sourcefilesCache.Where(j => j.SourceFile == SubPath))
                {
                    if (System.IO.File.Exists(filename))
                    {
                        string Extension = System.IO.Path.GetExtension(filename);
                        double fsq = new System.IO.FileInfo(filename).Length;
                        DateTime SouceFileDateTime1 = new System.IO.FileInfo(filename).LastWriteTime;
                        return new FileCompares(filename, Extension, fcc.SourceFileLength, fsq, SouceFileDateTime1);
                    }
                    else return null;
                }
            return null;
        }



        private void AddToSourceList(string filename, string Extension, int totalseconds, double fsq, DateTime SouceFileDateTime1)
        {
            lock (LockObjectSourceList)
            {
                sourcefiles.Add(new FileCompares(filename, Extension, totalseconds, fsq, SouceFileDateTime1));
            }
        }
        private async Task DoProcessingDestAsync(FileCompares fc, string DestDirectory)
        {
            string source = System.IO.Path.GetFileNameWithoutExtension(fc.SourceFile);
            List<string> DestList = Directory.EnumerateFiles(DestDirectory, source + "*", SearchOption.AllDirectories).
                Where(s => s.EndsWith(".mkv")).ToList<string>();
            foreach (string filename1 in DestList)
            {
                try
                {
                    var Converter = new ffmpegbridge();
                    Converter.ReadFile(filename1);

                    int totalseconds = (int) Converter.GetDuration().TotalSeconds;
                    Converter = null;
                    fc.DestFileLength = totalseconds;
                }
                catch (Exception ex)
                {
                    ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                }
            }
        }

        private async Task DoFileProcessingAsync(string filename)
        {
            try
            {
                string AppPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                var FileConverter = new ffmpegbridge();

                (IVideoStream videoStream, IAudioStream AudioStream, TimeSpan Dur) = FileConverter.ReadMediaFile(filename);
                FileConverter = null;
                if (videoStream != null)
                {
                    if (videoStream.Width > 720)
                    {
                        FileCompares fcc = new FileCompares(filename, "MKV", videoStream.Width, 0.0, DateTime.Now);
                        fcc.DestFileLength = fcc.SourceFileLength;
                        if (CompletedDir != "")
                        {
                            string filenamex = System.IO.Path.GetFileName(filename);
                            filenamex = CompletedDir + "\\" + filenamex;
                            if (File.Exists(filenamex))
                            {
                                fcc.TimesMatch = true;
                                DeleteIfExists(filename);
                                string filenamey = System.IO.Path.GetFileName(filename);
                                filenamey = "e:\\todo\\" + filenamex;
                                MoveIfExists(filenamex, filenamey);

                            }
                            else 
                            {
                                fcc.TimesMatch = true;
                                string filenamey = System.IO.Path.GetFileName(filename);
                                filenamey = "e:\\todo\\" + System.IO.Path.GetFileName(filenamex); 
                                DeleteIfExists(filenamey);
                                DeleteIfExists("e:\\todo\\processing\\" + System.IO.Path.GetFileName(filenamex));
                                if (filenamey.Contains(" ")) filenamey = "\"" + filenamey + "\"";
                                MoveIfExists(filename, filenamey);
                            }

                        }

                        sourcefiles.Add(fcc);
                    }
                }


            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private async Task DoProcessingAsync(string filename)
        {
            try
            {
                var Converter = new ffmpegbridge();
                Converter.ReadFile(filename);
                int totalseconds = (int)Converter.GetDuration().TotalSeconds;
                Converter = null;
                double fsq = new System.IO.FileInfo(filename).Length;
                string Extension = System.IO.Path.GetExtension(filename);
                DateTime SouceFileDateTime1 = new System.IO.FileInfo(filename).LastWriteTime;
                AddToSourceList(filename, Extension, totalseconds, fsq, SouceFileDateTime1);
            }
            catch (Exception ex)
            {
                string Extension = System.IO.Path.GetExtension(filename);
                double fsq = new System.IO.FileInfo(filename).Length;
                DateTime SouceFileDateTime1 = new System.IO.FileInfo(filename).LastWriteTime;
                AddToSourceList(filename, Extension, -1, fsq, SouceFileDateTime1);
            }
        }
        private void DoMultiThreadDestScan(List<FileCompares> sourcefiles, string DestDirectory)
        {
            try
            {
                List<Task> MultiThreads = new();
                int Index = 0, InCompletedTasks = 0;
                ProgressWindow pgs;
                pgs = new ProgressWindow(OnCancel, "Scanning Dest Files", DestDirectory, sourcefiles.Count);
                pgs.Show();
                canclose = false;
                bool Hidden = true;
                this.Hide();
                foreach (FileCompares fc in sourcefiles)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }
                    MultiThreads.Add(Task.Run(() => DoProcessingDestAsync(fc, DestDirectory)));
                    pgs.UpdateProgress(Index, fc.SourceFile);
                    InCompletedTasks = MultiThreads.Where(downloader => !downloader.IsCompleted).Count();
                    while (InCompletedTasks >= 8)
                    {
                        InCompletedTasks = MultiThreads.Where(downloader => !downloader.IsCompleted).Count();
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(50);
                    }
                    Index++;

                }
                pgs.Close();
                if (Hidden) Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


      
        private void DoMultiThreadScan(List<string> SourceList, string SourceDirectory)
        {
            try
            {
                List<Task> MultiThreads = new();
                string FileNameOnly;
                int InCompletedTasks = 0;
                ProgressWindow pgs;
                pgs = new ProgressWindow(OnCancel, "Scanning Files", SourceDirectory, SourceList.Count);
                pgs.Show();
                canclose = false;
                bool Hidden = true;
                this.Hide();
                int index = 0;
                foreach (string filename in SourceList)
                {
                    if (cancellationTokenSource.IsCancellationRequested) break;
                    bool found = false;
                    FileNameOnly = System.IO.Path.GetFileNameWithoutExtension(filename);
                    if (ErrorList.IndexOf(FileNameOnly) != -1)
                    {
                        DeleteIfExists(filename);
                    }
                    else
                    {
                        if (GetCachedRecord(filename) is FileCompares fss)
                        {
                            sourcefiles.Add(fss);
                            found = true;
                        }
                    }
                    if ((found == false) && (System.IO.File.Exists(filename)))
                    {
                        MultiThreads.Add(Task.Run(() => DoProcessingAsync(filename)));
                    }
                    pgs.UpdateProgress(index, filename);
                    InCompletedTasks = MultiThreads.Where(downloader => !downloader.IsCompleted).Count();
                    while (InCompletedTasks >= 8)
                    {
                        InCompletedTasks = MultiThreads.Where(downloader => !downloader.IsCompleted).Count();
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(50);
                    }
                    index++;
                }
                pgs.Close();
                canclose = true;
                if (Hidden) this.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

      
        public async Task Scan()
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                btnscan.IsEnabled = false;
                sourcefiles.Clear();
                Btncompfiles.IsEnabled = false;
                LstBoxFiles.ItemsSource = sourcefiles;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string SourceDirectory = Completed_Directory;// (string)key.GetValue("CompDirectory", string.Empty);// : string.Empty;
                key?.Close();
                string AppName = Completed_Directory;
                AppName += "\\sourceFileCache.index";
                if (SourceDirectory != string.Empty)
                {
                    ProgressWindow pgs;
                    List<string> SourceList = new List<string>();
                    SourceList = Directory.EnumerateFiles(SourceDirectory, "*.*", SearchOption.AllDirectories).
                         Where(s => s.EndsWith(".avi") || s.EndsWith(".mkv") || s.EndsWith(".mp4") || s.EndsWith(".m2ts")).ToList<string>();
                    sourcefilesCache = LoadCache(AppName);
                    sourcefiles.Clear();


                    if (SourceList.Count > 1)
                    {
                        DoMultiThreadScan(SourceList, SourceDirectory);


                        string JsonString = Helper.AsJsonList(sourcefiles);
                        if (JsonString != "")
                        {
                            DeleteIfExists(AppName);
                            using (StreamWriter sr = new(new FileStream(AppName, FileMode.CreateNew)))
                            {
                                sr.Write(JsonString);
                                sr.Close();
                            }
                        }

                        canclose = true;

                    }
                }
                LstBoxFiles.Items.Refresh();
                btnRemoveBadDest.IsEnabled = false;
                BtnPurge.IsEnabled = false;
                Btncompfiles.IsEnabled = (sourcefiles.Count > 0);
                btnscan.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Btnscan_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
            string checkdir = key.GetValueStr("DestDirectory", string.Empty);// : string.Empty;
            key.Close();
            if (checkdir != Completed_Directory)
            {
                Scan().ConfigureAwait(false);
            }
            else btnscan.IsEnabled = false;
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


        private void BtnPurge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                Btncompfiles.IsEnabled = false;
                LstBoxFiles.ItemsSource = sourcefiles;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string SourceDirectory = Completed_Directory;// "\\\\10.10.1.90\\tv.shows\\SciFi\\disk\\\\";
                //LoadedKey ? (string)key.GetValue("CompDirectory", string.Empty) : string.Empty;
                key?.Close();
                //ProgressBar1.Value = 0;
                //lblCurrent.Content = "0";
                //lblMax.Content = (sourcefiles.Count - 1).ToString();
                List<FileCompares> newList = new List<FileCompares>();
                ProgressWindow pgs;
                bool Hidden = false;
                canclose = false;
                pgs = new ProgressWindow(OnCancel, "Purging Files", SourceDirectory, sourcefiles.Count);
                pgs.Show();
                Hidden = true;
                this.Hide();
                int index = 0;
                bool saverecord = false;
                foreach (FileCompares fc in sourcefiles)
                {
                    saverecord = true;
                    if ((fc.DestFileLength != -1) && (fc.SourceFileLength != -1))
                    {
                        pgs.UpdateProgress(index, fc.SourceFilePath + "\\" + fc.SourceFile);
                        int dif = Math.Abs(fc.DestFileLength - fc.SourceFileLength);
                        bool ischecked = (dif < 400);
                        if (ischecked)
                        {
                            string FileToDelete = fc.SourceFilePath + "\\" + fc.SourceFile;
                            Task.Run(() => DeleteIfExists(FileToDelete));
                            saverecord = false;
                        }
                    }

                    if (saverecord) newList.Add(fc);
                    //pgs.UpdateProgress(index, fc.SourceFilePath + "\\" + fc.SourceFile);
                    index++;
                }
                pgs.Close();
                if (Hidden) this.Show();
                canclose = true;
                LstBoxFiles.ItemsSource = null;
                sourcefiles.Clear();
                sourcefiles.AddRange(newList);
                LstBoxFiles.ItemsSource = sourcefiles;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
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
        private void btnRemoveBadDest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                List<string> DestList = new List<string>();
                string DestDirectory = key.GetValueStr("DestDirectory", string.Empty);
                string ErrorDirectory = key.GetValueStr("ErrorDirectory", string.Empty);
                string SourceDirectory = Completed_Directory;//; ; //LoadedKey ? (string)key.GetValue("SourceDirectory", string.Empty) : string.Empty;
                string CompDirectory = key.GetValueStr("CompDirectory", string.Empty);
                key?.Close();
                //ProgressBar1.Value = 0;    
                //lblCurrent.Content = "0";
                //lblMax.Content = (sourcefiles.Count - 1).ToString();
                bool update = false;
                ProgressWindow pgs;
                bool Hidden = false;
                pgs = new ProgressWindow(OnCancel, "Moving Files From " + CompDirectory, SourceDirectory, sourcefiles.Count);
                pgs.Show();
                canclose = false;
                Hidden = true;
                this.Hide();
                int index = 0;
                List<FileCompares> newList = new List<FileCompares>();
                bool saverecord = false;
                foreach (FileCompares fc in sourcefiles)
                {
                    saverecord = true;
                    if ((fc.DestFileLength != -1) && (fc.SourceFileLength != -1))
                    {
                        int dif = Math.Abs(fc.DestFileLength - fc.SourceFileLength);
                        bool ischecked = (dif < 100);
                        if ((fc.DestFileLength == 0) || (!ischecked))
                        {
                            DestList = Directory.EnumerateFiles(DestDirectory, fc.SourceFile, SearchOption.AllDirectories).ToList<string>();
                            string FileToDelete = DestList.FirstOrDefault<string>();
                            DeleteIfExists(FileToDelete);
                            string FileToMove = fc.SourceFile;
                            string Extension = fc.Extension;
                            if (fc.DestFileLength <= 0)
                            {
                                string oldfile = CompDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(fc.SourceFile) + Extension;
                                string moveTo = SourceDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(fc.SourceFile) + Extension;
                                MoveIfExists(oldfile, moveTo);
                            }
                            else
                            {
                                string oldfile = CompDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(fc.SourceFile) + Extension;
                                string moveTo = ErrorDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(fc.SourceFile) + Extension;
                                MoveIfExists(oldfile, moveTo);
                            }
                            fc.DestFileLength = -1;
                            saverecord = false;
                            update = true;
                        }
                    }
                    if (saverecord) newList.Add(fc);
                    //lblCurrent.Content = ProgressBar1.Value.ToString();
                    pgs.UpdateProgress(index, CompDirectory + "\\" + fc.SourceFile);
                    index++;
                }
                pgs.Close();
                if (Hidden) this.Show();
                canclose = true;
                if (update)
                {
                    LstBoxFiles.ItemsSource = null;
                    sourcefiles.Clear();
                    sourcefiles.AddRange(newList);
                    LstBoxFiles.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void BtnRemoveSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                List<string> DestList = new List<string>();
                string DestDirectory = key.GetValueStr("DestDirectory", string.Empty);
                string ErrorDirectory = key.GetValueStr("ErrorDirectory", string.Empty);
                string SourceDirectory = key.GetValueStr("SourceDirectory", string.Empty);
                string CompDirectory = key.GetValueStr("CompDirectory", string.Empty);
                key?.Close();
                //ProgressBar1.Value = 0;
                //lblCurrent.Content = "0";
                //lblMax.Content = (sourcefiles.Count - 1).ToString();
                bool update = false;
                List<FileCompares> newList = new List<FileCompares>();
                bool saverecord = false;
                ProgressWindow pgs;
                bool Hidden = false;
                canclose = false;
                pgs = new ProgressWindow(OnCancel, "Moving/Deleting Files From " + CompDirectory, SourceDirectory, sourcefiles.Count);
                pgs.Show();
                Hidden = true;
                this.Hide();
                int index = 0;
                foreach (FileCompares fc in sourcefiles)
                {
                    saverecord = true;
                    if ((fc.DestFileLength == -1) && (fc.SourceFileLength != -1))
                    {
                        string oldfile = CompDirectory + "\\" + System.IO.Path.GetFileName(fc.SourceFile);
                        string moveTo = ErrorDirectory + "\\" + System.IO.Path.GetFileName(fc.SourceFile);
                        MoveIfExists(oldfile, moveTo);
                    }
                    if ((fc.DestFileLength != -1) && (fc.SourceFileLength != -1))
                    {
                        int dif = Math.Abs(fc.DestFileLength - fc.SourceFileLength);
                        bool ischecked = (dif > 300);
                        if (ischecked)
                        {
                            DestList = Directory.EnumerateFiles(DestDirectory, fc.SourceFile, SearchOption.AllDirectories).ToList<string>();
                            string FileToDelete = DestList.FirstOrDefault<string>();
                            DeleteIfExists(FileToDelete);
                            string FileToMove = fc.SourceFile;
                            string Extension = fc.Extension;
                            if (fc.DestFileLength <= 0)
                            {
                                if (fc.DestFileLength < fc.SourceFileLength)
                                {
                                    string oldfile = CompDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(fc.SourceFile) + Extension;
                                    string moveTo = ErrorDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(fc.SourceFile) + Extension;
                                    MoveIfExists(oldfile, moveTo);
                                    DestList = Directory.EnumerateFiles(SourceDirectory, fc.SourceFile, SearchOption.AllDirectories).ToList<string>();
                                    FileToDelete = DestList.FirstOrDefault<string>();
                                    DeleteIfExists(FileToDelete);
                                }
                            }
                            else
                            {
                                string oldfile = CompDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(fc.SourceFile) + Extension;
                                string moveTo = ErrorDirectory + "\\" + System.IO.Path.GetFileNameWithoutExtension(fc.SourceFile) + Extension;
                                MoveIfExists(oldfile, moveTo);
                                DestList = Directory.EnumerateFiles(SourceDirectory, fc.SourceFile, SearchOption.AllDirectories).ToList<string>();
                                FileToDelete = DestList.FirstOrDefault<string>();
                                DeleteIfExists(FileToDelete);
                            }
                            fc.DestFileLength = -1;
                            saverecord = false;
                            update = true;
                        }
                    }
                    if (saverecord) newList.Add(fc);
                    pgs.UpdateProgress(index, CompDirectory + "\\" + fc.SourceFile);
                    index++;
                }
                pgs.Close();
                if (Hidden) this.Show();
                canclose = true;
                if (update)
                {
                    LstBoxFiles.ItemsSource = null;
                    sourcefiles.Clear();
                    sourcefiles.AddRange(newList);
                    LstBoxFiles.Items.Refresh();
                }
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

                OnFinish?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


    }
}
