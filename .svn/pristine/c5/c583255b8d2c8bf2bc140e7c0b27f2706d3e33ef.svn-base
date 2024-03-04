using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for FinalMatchWindow.xaml
    /// </summary>
    public partial class FinalMatchWindow : Window
    {

        ShowMatcher.MatchClosed MatcherClosed;
        public delegate void OnRecordMatch(FileNamesClass record1, FileNamesClass record2);
        public OnRecordMatch OnRecordUpdate;
        ProgressWindow pgs;
        public object LockObject = new();
        public bool Hidden = false;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public ProgressWindow.CancelScan OnCancel;
        List<Task> PageDownloadTasks = new();
        public DispatcherTimer DoStuff = new();
        public List<FileNamesClass> FilesToProcess; // List of Source Files
        public List<FileNamesClass> FilesToShow; // Files With Corrected Titles
        public FinalMatchWindow(ShowMatcher.MatchClosed _MatcherClosed, List<FileNamesClass> _FilesToProcess, List<FileNamesClass> _FilesToShow)
        {
            try
            {
                MatcherClosed = _MatcherClosed;
                FilesToProcess = _FilesToProcess;
                FilesToShow = _FilesToShow;
                OnRecordUpdate = new(OnRecordUpdated);
                OnCancel = new(OnCancelation);
                InitializeComponent();
                DoStuff.Interval = new TimeSpan(0, 0, 1);
                DoStuff.Tick += new EventHandler(DoStuff_Tick);
                DoStuff.Start();

                //SetupMatcher();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void DoStuff_Tick(object sender, EventArgs e)
        {
            try
            {
                DoStuff.Stop();
                DoStuff.IsEnabled = false;
                Hide();
                System.Windows.Forms.Application.DoEvents();
                SetupMatcher();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
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

        public void OnRecordUpdated(FileNamesClass record1, FileNamesClass record2)
        {
            try
            {
                lock (LockObject)
                {
                    int index = FilesToProcess.IndexOf(record1);
                    if (index != -1)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (pgs != null)
                            {
                                System.Windows.Forms.Application.DoEvents();
                                FileNamesClass _FilesToProcess = FilesToProcess[index];
                                _FilesToProcess.ComboItems.Clear();
                                _FilesToProcess.ComboItems.AddRange(record2.ComboItems);
                                _FilesToProcess.SimpleStringProperty = record2.SimpleStringProperty;
                                _FilesToProcess.IsCorrect = record2.IsCorrect;
                                _FilesToProcess.IsEnabled = record2.IsEnabled;
                                _FilesToProcess.IdentifiedAs = record2.IdentifiedAs;
                                System.Windows.Forms.Application.DoEvents();
                            }

                        });

                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
 
        private void SetupMatcher()
        {
            try
            {
                int TotalTask = 0;
                int CompletedTasks = 0;
                StartAndShowProgress("Reading Files To Show ", "", FilesToProcess.Count);
                for (int i = 0; i < FilesToProcess.Count; i++)
                {
                    FileNamesClass ms = FilesToProcess[i];
                    PageDownloadTasks.Add(Task.Factory.StartNew(() => Match(i, OnRecordUpdate, ms, FilesToShow)));
                    TotalTask = PageDownloadTasks.Count;
                    UpdateProgress(i, "");
                    while (TotalTask - CompletedTasks >= 20)
                    {
                        CompletedTasks = (PageDownloadTasks.Where(downloader => downloader.IsCompleted)).Count();
                    }

                }
                TotalTask = PageDownloadTasks.Count;
                while (TotalTask - CompletedTasks > 0)
                {
                    CompletedTasks = (PageDownloadTasks.Where(downloader => downloader.IsCompleted)).Count();
                    System.Windows.Forms.Application.DoEvents();
                }
                KillProgressWindow();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LstBoxFiles.ItemsSource = FilesToProcess;
                });
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Match(int recno, OnRecordMatch OnMatched, FileNamesClass _FilesToProcess, List<FileNamesClass> _FilesToShow)
        {
            try
            {
                string SearchID = _FilesToProcess.Title.GetShortName().ToLower().Trim();
                bool found = false;
                foreach (var MS1 in _FilesToShow.Where(s => s.Title.ToLower().Trim() == SearchID))
                {
                    OnMatched?.Invoke(_FilesToProcess, MS1);
                    found = true;
                    break;
                }
                if (!found)
                {
                    string ss = "";

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void KillProgressWindow()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (pgs != null)
                    {
                        pgs.Close();
                        if (Hidden) this.Show();
                        Hidden = false;
                    }
                });
            }

            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void UpdateCount(int count)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (pgs != null)
                    {
                        pgs.UpdateCount(count);
                    }
                });
            }

            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void UpdateProgress(int progress, string status)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (pgs != null)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        pgs.UpdateProgress(progress, status);
                        System.Windows.Forms.Application.DoEvents();
                    }

                });
            }

            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void UpdateStatus(string Status)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (pgs != null)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        pgs.UpdateStatus(Status);
                        System.Windows.Forms.Application.DoEvents();
                    }
                });
            }

            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void StartAndShowProgress(string Header1, string Header2, int count, int start = 0)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {

                    pgs = new ProgressWindow(OnCancel, Header1, Header2, count, start);
                    pgs.Show();
                    Hidden = true;
                    this.Hide();
                });
            }

            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void File_Matcher__Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                MatcherClosed?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void File_Matcher_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                LstBoxFiles.Width = Width - 30;
                border1.Width = Width - 40;
                ProgressBar1.Width = Width - 720;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);

            }
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgressBar1.Value = 0;
                    ProgressBar1.Maximum = LstBoxFiles.Items.Count;

                });
                foreach (FileNamesClass fls in LstBoxFiles.Items)
                {
                    if (fls.InitialPath != "")
                    {
                        string newdir = fls.SimpleStringProperty.Replace("*", "_").Replace(":", "").Replace("'", "").Replace("?", "").Replace("//", "-").Replace("\\", "-").Replace(".", "").Trim();
                        
                        string dirname = fls.InitialPath + "\\" + newdir;
                        string OldFile = fls.InitialPath + "\\" + fls.Title;
                        string newfile = fls.InitialPath.Replace("done","dddd") + "\\" + newdir + "\\" + fls.Title;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            System.Windows.Forms.Application.DoEvents();
                            if (!System.IO.Directory.Exists(dirname.Replace("done", "dddd")))
                            {
                                System.IO.Directory.CreateDirectory(dirname.Replace("done", "dddd"));
                            }
                            System.Windows.Forms.Application.DoEvents();
                            if (System.IO.File.Exists(OldFile)) System.IO.File.Move(OldFile, newfile);
                            ProgressBar1.Value++;
                            System.Windows.Forms.Application.DoEvents();
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);

            }
        }


    }
}
