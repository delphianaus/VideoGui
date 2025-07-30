using AsyncAwaitBestPractices;
using FolderBrowserEx;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VideoGui.Models;
using VideoGui.Models.delegates;
using FolderBrowserDialog = FolderBrowserEx.FolderBrowserDialog;
using Path = System.IO.Path;


namespace VideoGui
{
    /// <summary>
    /// Interaction logic for MediaImporter.xaml
    /// </summary>
    /// 

    public partial class MediaImporter : Window
    {
        List<(string, TimeSpan, string, string)> MediaInfoTimes = new List<(string, TimeSpan, string, string)>();
        List<(TimeSpan, string, string)> MediaInfoTimesSort = new List<(TimeSpan, string, string)>();
        //ObservableCollection<FileInfoGoPro> FileRenamer = new ObservableCollection<FileInfoGoPro>();
        List<string> files = new List<string>();
        FileImporterClear File_Importer_Clear;
        ImportRecordAdd Import_Record_Add;
        ReOrderFiles DoReOrderFiles;
        CheckImports Check_Imports;
        ClearTimes DoClearTimes;
        SetFromTime DoSetFromTime;
        SetToTime DoSetToTime;
        //SetLists Set_Lists;
        FileRenamerClear DoFileRenamerClear;
        int MaxFile = 0;
        databasehook<object> ModuleCallBack;
        bool IsFirstResize = true, Ready = false;
        DispatcherTimer LocationChangedTimer = new DispatcherTimer();
        DispatcherTimer LocationChanger = new DispatcherTimer();
        double DockPanelWidth = 348;
        public MediaImporter(databasehook<object> _ModuleCallback, FileImporterClear _FileImporterClear,
            ImportRecordAdd _ImportRec, CheckImports _CheckImports,
            ReOrderFiles _ReOrderFiles, ClearTimes doClearTimes,
            SetFromTime doSetFromTime, SetToTime doSetToTime)
        {
            InitializeComponent();
            File_Importer_Clear = _FileImporterClear;
            Import_Record_Add = _ImportRec;
            Check_Imports = _CheckImports;
            ModuleCallBack = _ModuleCallback;
            //Set_Lists = _SetLists;
            DoReOrderFiles = _ReOrderFiles;
            DoClearTimes = doClearTimes;
            DoSetFromTime = doSetFromTime;
            DoSetToTime = doSetToTime;
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
                    key2?.SetValue("MIleft", Left);
                    key2?.SetValue("MItop", Top);
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
                var _width = key.GetValue("MIWidth", ActualWidth).ToDouble();
                var _height = key.GetValue("MIHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("MIleft", Left).ToDouble();
                var _top = key.GetValue("MItop", Top).ToDouble();
                key?.Close();
                Left = (Left != _left && _left != 0) ? _left : Left;
                Top = (Top != _top && _top != 0) ? _top : Top;
                Width = (ActualWidth != _width && _width != 0) ? _width : Width;
                Height = (ActualHeight != _height && _height != 0) ? _height : Height;
                SetControls(Width);
                SetControls(Height, false);


                LoadingPanel.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Visible;


                if (IsFirstResize)
                {
                    IsFirstResize = false;
                    LoadingPanel.Height = 0;
                    MainScroller.ScrollToVerticalOffset(439); // Scroll to the main content
                }
                Ready = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"LocationChanger_Tick {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }
        private void SetControls(double _v, bool IsWidth = true)
        {
            try
            {
                if (IsWidth)
                {
                    MainGrid.Width = _v;
                    LoadingPanel.Width = _v;
                    MainContent.Width = _v;
                    MainScroller.Width = _v;
                    brdFileInfo.Width = _v - 25;
                    brdControls.Width = brdFileInfo.Width;
                    msuSchedules.Width = _v - 113;
                    Canvas.SetLeft(brdt2, _v - 109);
                    Canvas.SetLeft(btnSelectSourceDir, _v - 55);
                    Canvas.SetLeft(btnClose, _v - 124);
                    txtsrcdir.Width = _v - 180;
                }
                else
                {
                    MainGrid.Height = _v;
                    LoadingPanel.Height = _v;
                    MainScroller.Height = _v;
                    MainContent.Height = _v;
                    msuSchedules.Height = _v - 152;
                    Canvas.SetTop(brdControls, _v - 93);
                    brdTControls.Height = _v - 161;
                    cnvControls.Height = brdTControls.Height;
                    Canvas.SetTop(btnMove, _v - 196);
                    Canvas.SetTop(btnSelectAll, _v - 227);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetControls {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }

        }
       
        private void frmImport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (e.WidthChanged)
                { 
                    SetControls(e.NewSize.Width);
                }
                if (e.HeightChanged)
                {
                    SetControls(e.NewSize.Height, false);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public async Task ReadFile(string filePath)
        {
            try
            {
                MaxFile++;
                var mw1 = new MediaInfo.MediaInfo();
                mw1.Open(filePath);
                List<string> FileData = mw1.Inform().Split(Environment.NewLine).ToList();
                mw1.Close();
                var timeframe = TimeSpan.Zero;
                bool Found = false;
                foreach (var file in FileData.Where(file => file.Contains("Time code of first frame")))
                {
                    timeframe = file.Substring("Time code of first frame".Length + 1).Trim().Replace(": ", "").FromStrToTimeSpan();
                    Found = (timeframe != TimeSpan.Zero);
                    if (Found) break;
                }
                MediaInfoTimes.Add((filePath, timeframe, "", ""));
                MediaInfoTimesSort.Add((timeframe, filePath, ""));

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

        public async Task GetFiles(string SourceDir)
        {
            try
            {
                MediaInfoTimes.Clear();
                MediaInfoTimesSort.Clear();
                File_Importer_Clear?.Invoke();
                DoClearTimes?.Invoke();
                files = Directory.EnumerateFiles(SourceDir, "*.mp4", SearchOption.AllDirectories).ToList();
                foreach (string file in files)
                {
                    while (MaxFile > 16)
                    {
                        Thread.Sleep(25);
                    }
                    ReadFile(file).ConfigureAwait(false);
                }
                while (MaxFile > 0)
                {
                    Thread.Sleep(100);
                }
                MediaInfoTimesSort.Sort();
                string Pad = "";
                for (int i = 0; i < MediaInfoTimesSort.Count; i++)
                {
                    string FN = "";
                    string id = "";
                    TimeSpan TS = TimeSpan.Zero;
                    (TS, FN, id) = MediaInfoTimesSort[i];
                    id = i.ToString().PadLeft(3, '0');
                    MediaInfoTimesSort[i] = (TS, FN, id);
                }
                for (int i = 0; i < MediaInfoTimes.Count; i++)
                {
                    (string, TimeSpan, string, string) fileinfos = MediaInfoTimes[i];
                    string FN1 = "", FN2 = "", FN3 = "";
                    TimeSpan TSA = TimeSpan.Zero;
                    foreach (var f in MediaInfoTimesSort.Where(f => f.Item2 == fileinfos.Item1))
                    {
                        (FN1, TSA, FN2, FN3) = MediaInfoTimes[i];
                        string fname = fileinfos.Item1.Split('\\').ToList().LastOrDefault();
                        string nname = fname;
                        if (fname != "" && !fname.Contains("GX_"))
                        {
                            nname = fname.Replace("GX", "GX_" + f.Item3 + "_");
                        }
                        if (FN1.Contains("GX_"))
                        {
                            nname = fname;
                        }
                        MediaInfoTimes[i] = (FN1, TSA, fname, nname);
                    }
                }
                foreach (var f in MediaInfoTimes)
                {
                    Import_Record_Add?.Invoke(f.Item3, f.Item4, f.Item2);
                    Thread.Sleep(15);
                }
                bool EnableButton = false;
                if (Check_Imports is not null)
                {
                    EnableButton = !Check_Imports.Invoke();
                }
                btnRename.IsEnabled = EnableButton;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private async void btnSelectSourceDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ModuleCallBack?.Invoke(this, new CustomParams_DataSelect());
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string Root = key.GetValueStr("MediaImporterSource", "c:\\");
                key?.Close();
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Title = "Select a folder";
                folderBrowserDialog.InitialFolder = Root;
                folderBrowserDialog.AllowMultiSelect = false;
                var folder = "";
                var selectresult = folderBrowserDialog.ShowDialog();
                if (selectresult == System.Windows.Forms.DialogResult.OK)
                {
                    folder = folderBrowserDialog.SelectedFolder;
                    var Flist = folder.Split("\\").ToList();
                    int cnt = Flist.Count - 1;
                    if (cnt > 0)
                    {
                        string fldr = Flist[cnt];
                        fldr = folder.Replace($"\\{fldr}", "");
                        if (fldr != "")
                        {
                            RegistryKey keyx = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                            keyx.SetValue("MediaImporterSource", fldr);
                            keyx?.Close();
                        }
                    }
                    txtsrcdir.Text = folder;
                    GetFiles(folder).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
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
                DoReOrderFiles?.Invoke(txtsrcdir.Text);
                System.Windows.MessageBox.Show("Done");
                GetFiles(txtsrcdir.Text).ConfigureAwait(false);
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
                if (IsLoaded)
                {
                    ModuleCallBack?.Invoke(this, new CustomParams_Initialize());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }

        private void mnuFilterFrom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoStartSelect();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void DoEndSelect()
        {
            try
            {

                if (msuSchedules.SelectedItems.Count > 0)
                {
                    var p = msuSchedules.SelectedItems[msuSchedules.SelectedItems.Count - 1];
                    if (p is FileInfoGoPro fpgx)
                    {
                        DoSetToTime?.Invoke(fpgx.TimeData);
                        txtEnd.Text = fpgx.TimeData.ToFFmpeg().Replace(".000", "");
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }

        private void mnuFilterTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //e.OriginalSource is MenuItem mnu && mnu.DataContext is FileInfoGoPro fpgx;
                DoEndSelect();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void DoStartSelect()
        {
            try
            {
                if (msuSchedules.SelectedItems.Count > 0)
                {
                    var p = msuSchedules.SelectedItems[0];
                    if (p is FileInfoGoPro fpgx)
                    {
                        DoSetFromTime?.Invoke(fpgx.TimeData);
                        txtStart.Text = fpgx.TimeData.ToFFmpeg().Replace(".000", "");
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void mnuClearFilters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoClearTimes?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void mnuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                msuSchedules.SelectAll();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void mnuMoveSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SoSelectMoveEvent();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }



        private void SoSelectMoveEvent()
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Title = "Select a folder";
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string defaultprogramlocation = key.GetValueStr("defaultprogramlocation", "c:\\videogui");
                string Root = key.GetValueStr("MediaImporterSource", "c:\\");
                key?.Close();
                string AppDrive = Path.GetPathRoot(defaultprogramlocation);
                folderBrowserDialog.InitialFolder = Root;
                //File.Exists("E:\\GoPro9") ? "E:\\GoPro9" : "C:\\GoPro9";
                if (!Directory.Exists(folderBrowserDialog.InitialFolder)) folderBrowserDialog.InitialFolder = AppDrive;
                folderBrowserDialog.AllowMultiSelect = false;

                var folder = "";
                var selectresult = folderBrowserDialog.ShowDialog();
                if (selectresult == System.Windows.Forms.DialogResult.OK)
                {
                    folder = folderBrowserDialog.SelectedFolder;
                    if (folder != null && folder != "")
                    {
                        foreach (var Selected in msuSchedules.SelectedItems)
                        {
                            if (Selected is FileInfoGoPro FGX)
                            {
                                string OldFile = FGX.NewFile;
                                string Path = txtsrcdir.Text;
                                string NewPath = folder + "\\" + FGX.NewFile;
                                string OldPath = Path + "\\" + OldFile;
                                File.Move(OldPath, NewPath);
                            }
                        }
                        System.Windows.MessageBox.Show("Done");

                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }





        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                msuSchedules.SelectAll();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SoSelectMoveEvent();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void labelstartevents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DoStartSelect();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void labelendevents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DoEndSelect();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoClearTimes?.Invoke();
                txtStart.Text = string.Empty;
                txtEnd.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void txtStart_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    var data = txtStart.Text.FromStrToTimeSpan();
                    DoSetFromTime?.Invoke(data);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void txtEnd_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    var data = txtEnd.Text.FromStrToTimeSpan();
                    DoSetToTime?.Invoke(data);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public TimeSpan DoCalcs(string datas)
        {
            try
            {
                var data = datas.FromStrToTimeSpan();
                bool ctl = (Keyboard.Modifiers == ModifierKeys.Control);
                bool shift = (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
                if (ctl && shift)
                {
                    data -= "00:00:30".FromStrToTimeSpan();
                }
                else if (ctl && !shift)
                {
                    data += "00:00:30".FromStrToTimeSpan();
                }
                else if (!ctl && shift)
                {
                    data -= "00:02:30".FromStrToTimeSpan();
                }
                else if (!ctl && !shift)
                {
                    data += "00:02:30".FromStrToTimeSpan();
                }
                return data;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return TimeSpan.Zero;
            }
        }
        private void btntartSelector_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtStart.Text != "")
                {
                    var t = DoCalcs(txtStart.Text);
                    txtStart.Text = t.ToFFmpeg().Replace(".000", "");
                    DoSetFromTime?.Invoke(t);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnEndSelector_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtEnd.Text != "")
                {
                    var t = DoCalcs(txtEnd.Text);
                    txtEnd.Text = t.ToFFmpeg().Replace(".000", "");
                    DoSetToTime?.Invoke(t);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void mnuSelectDateFrom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource is MenuItem mnu && mnu.DataContext is FileInfoGoPro fpgx)
                {
                    DoSetFromTime?.Invoke(fpgx.TimeData);
                    txtStart.Text = fpgx.TimeData.ToFFmpeg().Replace(".000", "");
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuSelectDateFrom_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuSelectDateTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.OriginalSource is MenuItem mnu && mnu.DataContext is FileInfoGoPro fpgx)
                {
                    DoSetToTime?.Invoke(fpgx.TimeData);
                    txtEnd.Text = fpgx.TimeData.ToFFmpeg().Replace(".000", "");
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuSelectDateFrom_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
    }
}
