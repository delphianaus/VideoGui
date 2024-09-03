using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xaml.Schema;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for MatchedShowsVerifier.xaml
    /// </summary>
    public partial class MatchedShowsVerifier : Window
    {
        public bool AllowChange = false;
        public ShowMatcher.MatchClosed OnClose;
        List<TVShows> TotalList = new();
        List<Task> PageDownloadTasks = new();
        List<string> SearchedTVShows = new();
        string SourceDir = "";
        public List<FileNamesClass> SearchedTVShowsFiltered = new();
        public List<FileNamesClass> FilesToShow;
        List<string> ShowList = new();
        List<string> ShowListFiltered = new();
        public Object LockSearches = new();
        public delegate void UpdateSearchRecord(string RecordToAdd);
        public UpdateSearchRecord UpdateSearchRecords;
        public bool InCorrentOnlyShowing = false;
        public bool IsLowerCaseOnlyShowing = false;
        public MatchedShowsVerifier(ShowMatcher.MatchClosed _OnClose, List<FileNamesClass> _FilesToShow, List<TVShows> _TotalShows)
        {
            try
            {
                OnClose = _OnClose;
                TotalList = _TotalShows;
                InitializeComponent();
                FilesToShow = new(_FilesToShow);
                LoadInCorrectItems();
                UpdateCnt();
                LstBoxFiles.ItemsSource = FilesToShow;
                LstAllShows.ItemsSource = SearchedTVShows;
                UpdateSearchRecords = new(OnAddSearchRecord);

                LstBoxFiles.Items.Refresh();
                ShowList.AddRange(FilesToShow.Where(fsh => fsh.SimpleStringProperty != "").Where(fsh => ShowList.IndexOf(fsh.SimpleStringProperty) == -1).Select(fsh => fsh.SimpleStringProperty));
                CmbShows.ItemsSource = ShowList;

                LoadGridSizes();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        public void OnAddSearchRecord(string RecordToAdd)
        {
            try
            {
                lock (LockSearches)
                {
                    if (SearchedTVShows.IndexOf(RecordToAdd) == -1)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SearchedTVShows.Add(RecordToAdd);
                            LstAllShows.Items.Refresh();

                        });

                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        public void LoadGridSizes()
        {
            try
            {

                //GridView GVCS = LstBoxFiles.vi

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }
        private void CmbGridSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AllowChange)
                {

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }

        }

        public void CopyNonCorrectBack()
        {
            try
            {

                foreach (FileNamesClass tv in SearchedTVShowsFiltered)
                {
                    foreach (FileNamesClass tv2 in FilesToShow.Where(s => s.Title == tv.Title))
                    {
                        tv2.IsEnabled = tv.IsEnabled;
                        tv2.IdentifiedAs = tv.IdentifiedAs;
                        tv2.IsCorrect = tv.IsCorrect;
                        tv2.ComboItems = tv.ComboItems;
                        tv2.SimpleStringProperty = tv.SimpleStringProperty;
                    }

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }
        private void MatchVerifier_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (InCorrentOnlyShowing) CopyNonCorrectBack();

                SaveInCorrentItems();

                OnClose?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                LstBoxFiles.Width = Width - 40;
                LstAllShows.Width = Width - 290;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }


        private void CmbSearchStrings_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    string search = CmbSearchStrings.Text.Trim().ToLower();
                    if (CmbSearchStrings.Items.IndexOf(search) == -1)
                    {
                        CmbSearchStrings.Items.Add(search);
                        CmbSearchStrings.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void BtnDeleteSearchString_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbSearchStrings.SelectedIndex != -1)
                {
                    string search = CmbSearchStrings.Text.Trim().ToLower();
                    if (CmbSearchStrings.Items.IndexOf(search) == -1)
                    {
                        CmbSearchStrings.Items.Remove(search);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void BtnClearSearchStrings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LstAllShows.Items.Clear();
                CmbSearchStrings.Items.Clear();
                CmbSearchStrings.Text = "";
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }
        private void SetupSearch()
        {
            try
            {
                SearchedTVShows.Clear();
                List<string> ListOfItems = new(CmbSearchStrings.Items.OfType<string>().ToList());
                int UnCompletedTasks = 0, TotalTask = 0;
                (ProgressBar1.Maximum, ProgressBar1.Value) = (TotalList.Count - 1, 0);
                for (int i = 0; i < TotalList.Count - 1; i++)
                {
                    ProgressBar1.Value = i;
                    PageDownloadTasks.Add(Task.Run(() => SearchList(new(ListOfItems), TotalList[i], UpdateSearchRecords)));
                    UnCompletedTasks = (PageDownloadTasks.Where(downloader => !downloader.IsCompleted)).Count();
                    while (UnCompletedTasks >= 5)
                    {
                        UnCompletedTasks = (PageDownloadTasks.Where(downloader => !downloader.IsCompleted)).Count();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }
        private void SearchList(List<string> searchstrings, TVShows _tv, UpdateSearchRecord UpdateSearchRecords)
        {
            try
            {
                bool found = false;
                found = searchstrings.All(s => _tv.name.ToLower().Contains(s));
                if (found)
                {
                    UpdateSearchRecords?.Invoke(_tv.name);
                }


            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void BtnSearchList_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (CmbSearchStrings.Items.Count > -1)
                {
                    SetupSearch();
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        public void SaveInCorrentItems()
        {
            try
            {
                List<FileNamesClass> _SearchedTVShowsFiltered = new(FilesToShow.Where(s => (!s.IsCorrect) || (s.IdentifiedAs)));
                string AppName = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                AppName += "\\InCorrect.JSON";
                string JsonData = JsonSerializer.Serialize<List<FileNamesClass>>(_SearchedTVShowsFiltered);// (JsonStringDownloading);
                DeleteIfExists(AppName);
                System.IO.File.WriteAllText(AppName, JsonData);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }
        public void DeleteIfExists(string filename)
        {
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ShowNonCorrect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchedTVShowsFiltered = new(FilesToShow.Where(s => !s.IsCorrect || s.IdentifiedAs));
                LstBoxFiles.ItemsSource = SearchedTVShowsFiltered;
                LstBoxFiles.Items.Refresh();
                InCorrentOnlyShowing = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void ShowAllItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CopyNonCorrectBack();
                InCorrentOnlyShowing = false;
                LstBoxFiles.ItemsSource = FilesToShow;
                LstBoxFiles.Items.Refresh();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        public void UpdateCnt()
        {
            try
            {
                int total = FilesToShow.Count;
                int Cnt = FilesToShow.Where(s => s.IsCorrect == false).Count();
                int titlesize = "Matched Shows Verifier".Length;
                string cnt = "[Total " + total.ToString() + " InCorrect " + Cnt.ToString() + "]";
                Title = Title.Substring(0, titlesize) + " " + cnt;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        public void LoadInCorrectItems()
        {
            try
            {

                string AppName = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                AppName += "\\InCorrect.JSON";
                if (System.IO.File.Exists(AppName))
                {
                    List<FileNamesClass> _SearchedTVShowsFiltered = new();
                    string JsonData = System.IO.File.ReadAllText(AppName);
                    _SearchedTVShowsFiltered = JsonSerializer.Deserialize<List<FileNamesClass>>(JsonData);
                    foreach (FileNamesClass tv in _SearchedTVShowsFiltered)
                    {
                        foreach (FileNamesClass tv2 in FilesToShow.Where(s => s.Title == tv.Title))
                        {
                            tv2.IsEnabled = tv.IsEnabled;
                            tv2.IdentifiedAs = tv.IdentifiedAs;
                            tv2.IsCorrect = tv.IsCorrect;
                            tv2.ComboItems = tv.ComboItems;
                            tv2.SimpleStringProperty = tv.SimpleStringProperty;
                        }
                    }
                }

                //string JsonData = JsonSerializer.Serialize<List<FileNamesClass>>(_SearchedTVShowsFiltered);// (JsonStringDownloading);

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }
        private void SaveInCorrectItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((InCorrentOnlyShowing) || (IsLowerCaseOnlyShowing)) CopyNonCorrectBack();
                SaveInCorrentItems();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void EnterDetailsForInCorrect_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void IsCorrect_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                UpdateCnt();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void LstBoxFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UpdateCnt();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void CopyItemToClip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LstBoxFiles.SelectedIndex != -1)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string Selection = ((FileNamesClass)LstBoxFiles.SelectedItem).Title;
                        System.Windows.Clipboard.SetText(Selection);
                    });

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void CopySelectedCMB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    if ((LstBoxFiles.SelectedIndex != -1) && (CmbShows.SelectedIndex != -1))
                    {
                        ((FileNamesClass)LstBoxFiles.SelectedItem).ComboItems.Clear();
                        ((FileNamesClass)LstBoxFiles.SelectedItem).ComboItems.Add(CmbShows.Text);
                        ((FileNamesClass)LstBoxFiles.SelectedItem).SimpleStringProperty = CmbShows.Text;
                        ((FileNamesClass)LstBoxFiles.SelectedItem).IdentifiedAs = true;
                        ((FileNamesClass)LstBoxFiles.SelectedItem).IsCorrect = true;
                    }
                }
                catch (Exception ex)
                {
                    ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        //CopyTitle_Click
        private void CopyTitle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((LstBoxFiles.SelectedIndex != -1))
                {
                    string Title = ((FileNamesClass)LstBoxFiles.SelectedItem).Title.ToPascalCase();
                    ((FileNamesClass)LstBoxFiles.SelectedItem).ComboItems.Clear();
                    ((FileNamesClass)LstBoxFiles.SelectedItem).ComboItems.Add(Title);
                    ((FileNamesClass)LstBoxFiles.SelectedItem).SimpleStringProperty = Title;
                    ((FileNamesClass)LstBoxFiles.SelectedItem).IdentifiedAs = true;
                    ((FileNamesClass)LstBoxFiles.SelectedItem).IsCorrect = true;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }
        private void RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((LstBoxFiles.SelectedIndex != -1))
                {
                    string Title = ((FileNamesClass)LstBoxFiles.SelectedItem).Title;
                    string Path = ((FileNamesClass)LstBoxFiles.SelectedItem).InitialPath;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        DeleteIfExists(Path + "\\" + Title);
                        if (!System.IO.File.Exists(Path + "\\" + Title))
                        {
                            FileNamesClass RemFile = (FileNamesClass)LstBoxFiles.SelectedItem;
                            if (InCorrentOnlyShowing) SearchedTVShowsFiltered.Remove(RemFile);
                            FilesToShow.Remove(RemFile);
                        }
                        LstBoxFiles.Items.Refresh();
                    });


                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }
        private void CopyEnteredShow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    if ((LstBoxFiles.SelectedIndex != -1) && (UseEnteredShow.Text != ""))
                    {
                        ((FileNamesClass)LstBoxFiles.SelectedItem).ComboItems.Clear();
                        ((FileNamesClass)LstBoxFiles.SelectedItem).ComboItems.Add(UseEnteredShow.Text.ToPascalCase());
                        ((FileNamesClass)LstBoxFiles.SelectedItem).SimpleStringProperty = UseEnteredShow.Text.ToPascalCase();
                        ((FileNamesClass)LstBoxFiles.SelectedItem).IdentifiedAs = true;
                        ((FileNamesClass)LstBoxFiles.SelectedItem).IsCorrect = true;
                    }
                }
                catch (Exception ex)
                {
                    ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void LstAllShows_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (LstAllShows.SelectedIndex != -1)
                {
                    UseEnteredShow.Text = LstAllShows.SelectedItems[0].ToString();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void CmbShows_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (CmbShows.Text.Trim() != "")
                {
                    ShowListFiltered.AddRange(ShowList.Where(s => s.Contains(CmbShows.Text)));
                    CmbShows.ItemsSource = ShowListFiltered;
                }
                else
                {
                    CmbShows.ItemsSource = ShowList;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }

        private void ShowLowerCaseNames_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsLowerCaseOnlyShowing = true;
                SearchedTVShowsFiltered = new(FilesToShow.Where(s => (s.SimpleStringProperty.ToLower() == s.SimpleStringProperty)));
                LstBoxFiles.ItemsSource = SearchedTVShowsFiltered;
                LstBoxFiles.Items.Refresh();
                InCorrentOnlyShowing = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }
        }


        private void FixLowerCaseNames_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsLowerCaseOnlyShowing)
                {
                    foreach (var show in SearchedTVShowsFiltered)
                    {
                        if (show.SimpleStringProperty.ToLower() == show.SimpleStringProperty)
                        {
                            for (int i = 0; i < show.ComboItems.Count; i++)
                            {
                                show.ComboItems[i] = show.ComboItems[i].ToPascalCase();
                            }
                            show.SimpleStringProperty = show.SimpleStringProperty.ToPascalCase();
                        }
                    }
                }
                else
                {
                    foreach (var show in FilesToShow)
                    {
                        if (show.SimpleStringProperty.ToLower() == show.SimpleStringProperty)
                        {
                            for (int i = 0; i < show.ComboItems.Count; i++)
                            {
                                show.ComboItems[i] = show.ComboItems[i].ToPascalCase();
                            }
                            show.SimpleStringProperty = show.SimpleStringProperty.ToPascalCase();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
            }

        }

        private void BtnUseThisShow_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
