using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using Application = System.Windows.Application;
using Path = System.IO.Path;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ShowMatcher.xaml
    /// </summary>
    /// 
    public class PageStatus
    {
        public int PageID;
        public bool finished = false;
        public PageStatus(int _PageID)
        {
            PageID = _PageID;
        }

    }
    public partial class ShowMatcher : Window
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        List<TVShows> TVShows = new();
        ProgressWindow pgs;
        List<FileNamesClass> FilesToProcess;
        List<FileSearcher> fsllist;
        List<Task> PageDownloadTasks = new();
        List<string> JsonResults = new();
        List<string> JsonResults2 = new();
        public object LockJson = new();
        public object MatchLock = new();
        public Models.delegates.CompairFinished OnFinish;
        public ProgressWindow.CancelScan OnCancel;
        public delegate void BuildJsonList();
        public delegate void UpdateSearchRecord(FileNamesClass FileRecord, string RecordToAdd);
        public UpdateSearchRecord SearchRecordUpdate;
        public BuildJsonList OnBuildJsonToList;
        public string JsonStringDownloading = "";
        public List<string> SearchList = new();
        List<FileNamesClass> FilesToProcessLocal;
        public delegate void MatchClosed();
        public MatchClosed OnMatchClosed;
        public MatchClosed OnFileMatchClosed;
        MatchedShowsVerifier MVS;
        List<FileNamesClass> CorrectList;
        public DispatcherTimer DoStuff = new();


        bool Hidden = false;
        public ShowMatcher(Models.delegates.CompairFinished _OnFinish)
        {
            try
            {
                OnFinish = _OnFinish;
                OnCancel = new(OnCancelation);
                OnBuildJsonToList = new(OnBuildJson);
                SearchRecordUpdate = new(SearchUpdate);
                OnMatchClosed = new(OnMatchFinished);
                OnFileMatchClosed = new(OnFileMatcherClosed);
                InitializeComponent();
                DoStuff.Interval = new TimeSpan(0, 0, 1);
                DoStuff.Tick += new EventHandler(DoStuff_Tick);
                DoStuff.Start();
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
                LoadJson();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void SearchUpdate(FileNamesClass FileRecord, string RecordToAdd)
        {
            try
            {
                lock (MatchLock)
                {
                    int index = FilesToProcessLocal.IndexOf(FileRecord);
                    if (index != -1)
                    {
                        FileNamesClass FileRecord1 = FilesToProcessLocal[index];
                        FileRecord1.IsEnabled = true;
                        FileRecord1.ComboItems.Add(RecordToAdd);
                        FileRecord1.SimpleStringProperty = RecordToAdd;
                        FileRecord1.IsCorrect = (RecordToAdd.Trim() != "");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void OnBuildJson()
        {
            try
            {
                List<TVShows> TVShowsX;
                TVShowsX = JsonSerializer.Deserialize<List<TVShows>>(JsonStringDownloading);
                TVShows.AddRange(TVShowsX.Where(tv => tv.language != null).Where(tv => tv.language.ToLower() == "english"));
                System.Windows.Application.Current.Dispatcher.Invoke(() => { AllowScanShows(); });
                LoadFileList();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void DownloadPage(int pageid, int LastPage)
        {
            try
            {
                string URL = "http://api.tvmaze.com/shows?page=" + pageid.ToString();
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage result = client.GetAsync(URL).Result;

                    if (result != null)
                    {
                        string source = result.Content.ReadAsStringAsync().Result;

                        lock (LockJson)
                        {

                            string Sources = source[1..];
                            int Len = Sources.Length - 1;
                            Sources = Sources.Substring(0, Len);
                            if (JsonResults.IndexOf(pageid.ToString()) == -1)
                            {
                                string sep = (JsonStringDownloading == "") ? "[" : ",";
                                JsonResults.Add(pageid.ToString());
                                JsonStringDownloading += sep + Sources;
                                // JsonResults2.Add(pageid.ToString()+"|"+ Sources.Substring(0,10));
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void AllowScanShows()
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { ScanShows(); });
                BtnLoadFileList.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void ScanShows()
        {
            try
            {

                if (TVShows != null)
                {
                    LblFiles.Content = TVShows.Count.ToString() + " Shows Loaded";
                    if (TVShows.Count > 0)
                    {
                        fsllist = new();

                        foreach (TVShows TV in TVShows)
                        {
                            string name = TV.name.ToLower();
                            if (name.Contains("the") && name.Contains("adolf") && (name.Contains("hitler")))
                            {
                                string ssx = "";
                                if (ssx != "")
                                {

                                }
                            }
                            if (TV.language != null)
                            {
                                if (TV.language.ToLower() != "english")
                                {
                                    continue;
                                }
                            }
                            FileSearcher fsp = new(TV.name);
                            fsllist.Add(fsp);
                            name = name.Replace(":", " ").Replace("-", " ").Replace(".", " ").Replace("'", "");

                            if (name.Contains(" "))
                            {
                                string[] names = name.ToLower().Split(" ");
                                if (names.Length > 0)
                                {
                                    foreach (string ss in names)
                                    {
                                        if ((names.Length == 1) && (ss.Length < 3)) continue;
                                        if ((names.Length > 1) && (ss.Length < 2)) continue;
                                        fsp.AddSeach(ss);
                                    }
                                }


                            }
                            else
                            {
                                fsp.AddSeach(name);
                            }

                        }

                    }
                }
                KillProgressWindow();

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public bool IsPageFound(int pageid)
        {
            try
            {
                bool pageres = false;
                string URL = "http://api.tvmaze.com/shows?page=" + pageid.ToString();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("X-Token", "VisualStudioApp");
                    using (var result = client.GetAsync(URL, HttpCompletionOption.ResponseHeadersRead).Result)
                    {
                        if (result != null)
                        {
                            if (result.StatusCode != HttpStatusCode.NotFound)
                            {
                                pageres = true;
                            }
                            result.Dispose();
                        }
                        return pageres;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadJson();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }

        private void LoadJson()
        {
            try
            {
                StartAndShowProgress("Working Out How Many Pages", "", -1);
                UpdateStatus("Working Out How Many Pages");
                UpdateProgress(0, "");
                Hide();
                int LastPage = 100;
                UpdateCount(-1);
                int MaxPages = 300;
                for (int i = 200; i < 500; i += 10)
                {
                    if (!IsPageFound(i))
                    {
                        MaxPages = i;
                        break;
                    }
                }
                for (int i = MaxPages; i > 210; i--)
                {
                    if (IsPageFound(i))
                    {
                        LastPage = i;
                        break;
                    }
                }

                UpdateCount(LastPage);
                UpdateStatus("Grabing " + LastPage.ToString() + " JSON Headers");
                //KillProgressWindow();
                //StartAndShowProgress("Attempting to Get " + LastPage.ToString() + " JSON Headers", "", LastPage, 0);
                DownloadHeaders(LastPage);
                UpdateStatus("Checking For Missing JSON Headers");
                int TotalTasks = PageDownloadTasks.Count;
                while (TotalTasks != (PageDownloadTasks.Where(downloader => downloader.IsCompleted)).Count())
                {
                    string ss = "";
                }
                List<int> MissingPages = new();
                if (JsonResults.Count <= LastPage)
                {
                    for (int i = 0; i <= LastPage; i++)
                    {
                        if (JsonResults.IndexOf(i.ToString()) == -1)
                        {
                            MissingPages.Add(i);
                        }
                    }
                }
                int iix = 1;
                UpdateStatus("");
                UpdateCount(MissingPages.Count);
                foreach (var ii in MissingPages)
                {
                    UpdateProgress(iix, "");
                    UpdateStatus("Grabing Missing JSON Page No. " + ii.ToString());
                    DownloadPage(ii, LastPage);
                    iix++;
                }
                UpdateStatus("");
                if (JsonResults.Count <= LastPage)
                {
                    MissingPages.Clear();
                    for (int i = 0; i <= LastPage; i++)
                    {
                        if (JsonResults.IndexOf(i.ToString()) == -1)
                        {
                            MissingPages.Add(i);
                        }
                    }
                }
                JsonStringDownloading += "]";
                UpdateStatus("Building " + LastPage.ToString() + " JSON Headers");
                Show();
                OnBuildJsonToList?.Invoke();
                KillProgressWindow();

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


        public void DownloadHeaders(int LastPage)
        {
            try
            {
                int UnCompletedTasks;
                for (int i = 0; i <= LastPage; i++)
                {
                    UpdateProgress(i, "");
                    UpdateStatus("Grabing " + LastPage.ToString() + " JSON Headers");
                    PageDownloadTasks.Add(Task.Run(() => DownloadPage(i, LastPage)));
                    int TotalTask = PageDownloadTasks.Count;
                    UnCompletedTasks = (PageDownloadTasks.Where(downloader => !downloader.IsCompleted)).Count();
                    while (UnCompletedTasks >= 10)
                    {
                        UnCompletedTasks = (PageDownloadTasks.Where(downloader => !downloader.IsCompleted)).Count();
                    }
 
                    continue;
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
                        pgs.UpdateProgress(progress, status);
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
                        pgs.UpdateStatus(Status);
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadFileList();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void LoadFileList()
        { 
            try
            { 
                List<string> EnumFiles = new();
                string CompDirectory = "\\\\leviathan\\tv\\done";
                StartAndShowProgress("Reading Files From " + CompDirectory, "", -1);

                int index = 0;
                EnumFiles = Directory.EnumerateFiles(CompDirectory, "*.*", SearchOption.AllDirectories).
                        Where(s => s.EndsWith(".avi") || s.EndsWith(".mkv") || s.EndsWith(".mp4") || s.EndsWith(".m2ts")).ToList<string>();
                FilesToProcess = new();
                UpdateCount(EnumFiles.Count);
                string ShowNameFile = "";
                UpdateStatus("Building FileList From " + CompDirectory);
                UpdateProgress(0, "");
                int counter = 0;
                foreach (string filename in EnumFiles)
                {
                    string fnn = System.IO.Path.GetFileName(filename);
                    FilesToProcess.Add(new FileNamesClass(filename, "", CompDirectory));
                    ShowNameFile = fnn.GetShortName().Trim();
                    UpdateProgress(counter++, fnn);
                    if (ShowNameFile != "")
                    {
                        if (SearchList.IndexOf(ShowNameFile) == -1)
                        {
                            SearchList.Add(ShowNameFile);
                        }
                    }
                    else
                    {
                        ShowNameFile = fnn.GetShortName();
                    }

                    //UpdateProgress(index++, filename);
                }
                KillProgressWindow();
                //LstBoxFiles.ItemsSource = FilesToProcess;
                SetupSeatchForMatches(CompDirectory);
                BtnMatchNames.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        
        private void frmShowMatcher_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                KillProgressWindow();
                OnFinish?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void SetupSeatchForMatches(string source)
        {
            try
            {
                FilesToProcessLocal = new();
                foreach (var file in SearchList)
                {
                    FilesToProcessLocal.Add(new FileNamesClass(file, "", source));
                }
                PageDownloadTasks.Clear();
                int UnCompletedTasks = 0;
                int cnt = 0;
                StartAndShowProgress("Building Search List", "", FilesToProcessLocal.Count);
                string AppName = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                AppName += "\\InCorrect.JSON";
                List<FileNamesClass> _SearchedTVShowsFiltered = new();
                if (System.IO.File.Exists(AppName))
                {
                    string JsonData = System.IO.File.ReadAllText(AppName);
                    _SearchedTVShowsFiltered = JsonSerializer.Deserialize<List<FileNamesClass>>(JsonData);
                }

                foreach (FileNamesClass fns in FilesToProcessLocal.Where(fns => _SearchedTVShowsFiltered.IndexOf(fns) == -1))
                {
                    UpdateProgress(cnt++, "");
                    PageDownloadTasks.Add(Task.Run(() => SearchForMatch(fns, new(fsllist), SearchRecordUpdate)));
                    UnCompletedTasks = (PageDownloadTasks.Where(downloader => !downloader.IsCompleted)).Count();
                    while (UnCompletedTasks >= 5)
                    {
                        UnCompletedTasks = (PageDownloadTasks.Where(downloader => !downloader.IsCompleted)).Count();
                    }

                    continue;
                }
                KillProgressWindow();
                MVS = new(OnMatchClosed, FilesToProcessLocal, TVShows);
                Hide();
                MVS.ShowDialog();
                //MatchAll();

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void MatchAll()
        {
            try
            {
                int cnt = 0;
                StartAndShowProgress("Matching File List", "", FilesToProcess.Count);
                foreach (var srp in FilesToProcess)
                {
                    UpdateProgress(cnt++, "");
                    string titleshow = srp.Title.ToLower();
                    string shortname = titleshow.GetShortName();
                    foreach (FileNamesClass sdx in FilesToProcessLocal.Where(srpp => srpp.Title.ToLower() == shortname))
                    {
                        if (sdx.IsEnabled)
                        {
                            srp.IsEnabled = sdx.IsEnabled;
                            srp.ComboItems = sdx.ComboItems;

                            srp.SimpleStringProperty = sdx.SimpleStringProperty;
                        }

                    }
                    srp.CorrentName = shortname;
                }
                KillProgressWindow();
                //LstBoxFiles.Items.Refresh();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void SearchForMatch(FileNamesClass SearchList, List<FileSearcher> _fsllist, UpdateSearchRecord _SearchRecordUpdate)
        {
            try
            {
                List<string> TVMatched = new();
                List<int> TVMatchedWords = new();
                List<int> TVMatchedWordsCnt = new();
                TVMatched.Clear();
                TVMatchedWords.Clear();
                TVMatchedWordsCnt.Clear();
                int matchcnt = 0;
                int totalsize = 0;
                string SearchStr = SearchList.Title.ToLower();
                foreach (var _fsps in _fsllist)
                {
                    string SearchStr2 = _fsps.TVShowName.ToLower();


                    bool found = false;
                    matchcnt = 0;
                    totalsize = 0;
                    found = false;

                    if (_fsps.TVSearchNames.Count > 0)
                    {
                        if (_fsps.TVSearchNames.Count == 1)
                        {
                            if (_fsps.TVSearchNames[0].Length < 4) continue;
                        }
                        found = _fsps.TVSearchNames.All(s => SearchStr.Contains(s));
                        foreach (string ss in _fsps.TVSearchNames)
                        {
                            totalsize += ss.Length;
                        }
                        matchcnt = _fsps.TVSearchNames.Count;
                    }
                    else found = false;

                    if (found)
                    {
                        if (_fsps.TVShowName == "The O.C.")
                        {
                            string sss = "";
                        }
                        TVMatched.Add(_fsps.TVShowName);
                        TVMatchedWords.Add(matchcnt);
                        TVMatchedWordsCnt.Add(totalsize);
                    }
                }
                if (TVMatched.Count > 0)
                {
                    int MaxID = -1;
                    int index1 = 0;
                    int total = 0;
                    int totalwords = 0;
                    string IDNAME = "";
                    foreach (int max in TVMatchedWordsCnt)
                    {
                        if (max > MaxID)
                        {
                            MaxID = max;
                            IDNAME = TVMatched[index1];
                            totalwords = TVMatchedWords[index1];
                            total = TVMatchedWordsCnt[index1];
                        }
                        index1++;
                    }
                    if (IDNAME != "")
                    {
                        string SearchLen = SearchStr.Replace(".", "").Replace("-", "").Replace("'", "").Replace(":", "");
                        if (((totalwords <= 2) && (total < 16)) || (total < Math.Round(0.7 * SearchLen.Length)))
                        {
                            string name = SearchList.Title.ToLower();
                            string IDNAME2 = EnquireOnName(name);
                            if (IDNAME2 != "")
                            {
                                _SearchRecordUpdate?.Invoke(SearchList, IDNAME2);
                            }
                            else _SearchRecordUpdate?.Invoke(SearchList, IDNAME);
                        }
                        else _SearchRecordUpdate?.Invoke(SearchList, IDNAME);
                    }
                }
                else
                {
                    string name = SearchList.Title.ToLower();
                    string IDNAME = EnquireOnName(name);
                    if (IDNAME != "") _SearchRecordUpdate?.Invoke(SearchList, IDNAME);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public string EnquireOnName(string name)
        {
            try
            {
                string url = "http://api.tvmaze.com/singlesearch/shows?q=\"" + name + "\"";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage result = client.GetAsync(url).Result;
                    if (result.StatusCode != HttpStatusCode.NotFound)
                    {
                        if (result != null)
                        {
                            string source = result.Content.ReadAsStringAsync().Result;

                            lock (LockJson)
                            {
                                if (source != "")
                                {
                                    TVShows TVShowsXX;
                                    TVShowsXX = JsonSerializer.Deserialize<TVShows>(source);
                                    return TVShowsXX.name;
                                }
                                else return "";
                            }

                        }
                        else return "";
                    }
                    else return "";
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }

        public void OnFileMatcherClosed()
        {
            try
            {

                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void OnMatchFinished()
        {
            try
            {
                CorrectList = new(MVS.FilesToShow);
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void BtnMatchNames_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CorrectList != null)
                {
                    List<FileNamesClass> NewFileRangeList = new(FilesToProcess);
                   
                    FinalMatchWindow FFM = new(OnFileMatcherClosed, new(NewFileRangeList), new(CorrectList));
                    FFM.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                Show();
                pgs.Close();
            }
        }
    }
}
