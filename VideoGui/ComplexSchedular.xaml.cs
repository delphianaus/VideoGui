using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VideoGui.Models;
using System.Windows.Markup;
using Microsoft.Win32;
using VideoGui.Models.delegates;
using FolderBrowserDialog = FolderBrowserEx.FolderBrowserDialog;
using static VideoGui.ffmpeg.Probe.FormatModel;
using System.IO;
using Path = System.IO.Path;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ScrollingTest.xaml
    /// </summary>
    /// 


    public partial class ComplexSchedular : Window
    {

        string tstart = "", tduration = "", destinationdir = "", adobedir = "";
        DispatcherTimer TrayIcon;
        double LastWidth = 0;
        GridLength ColumnWidth = new GridLength(200, GridUnitType.Pixel);
        public int NewTextBoxId = 0;
        public string NewText = "";
        public System.Windows.Forms.Timer TextBoxUpdater;
        public AddRecordDelegate DoAddRecord;
        public RemoveRecordDelegate DoRemoveRecord;
        public SetFilterAge DoSetFilterAge;
        public SetFilterString DoSetFilterString;
        public GetFilerString DoGetFilerString;
        databasehook<object> ModuleCallBack = null;
        public GetFilterAges DoGetFilterAges;
        public Visibility deletemenuvisible = Visibility.Visible;
        public Visibility AgeFilter = Visibility.Collapsed;
        public DialogAges DialogAgesEntry = null;
        public CustomStringEntry SourceDirectoryEntry = null;
        public CustomStringEntry DestinationDirectoryEntry = null;
        public CustomStringEntry DestinationFileEntry = null;
        public bool IsClosing = false, IsClosed = false;
        bool Ready = false;
        DispatcherTimer LocationChanger = new(), LocationChangedTimer= new();


        public ComplexSchedular(databasehook<object> _ModuleCallBack, AddRecordDelegate _AdddRecord,
            RemoveRecordDelegate _RemoveRecord,
            OnFinishIdObj _ComplexFinished, SetFilterAge _SetFilterAge,
            SetFilterString _SetFilterString, GetFilterAges _GetFilterAges,
            GetFilerString _GetFilterString)
        {
            try
            {
                InitializeComponent();
                LastWidth = ActualWidth;
                ModuleCallBack = _ModuleCallBack;
                //lstSchedules.ItemsSource = data;
                DoAddRecord = _AdddRecord;
                DoGetFilterAges = _GetFilterAges;
                DoRemoveRecord = _RemoveRecord;
                DoSetFilterAge = _SetFilterAge;
                DoSetFilterString = _SetFilterString;
                DoGetFilerString = _GetFilterString;
                Closed += (s, e) =>
                {
                    IsClosing = false;
                    IsClosed = true;
                    _ComplexFinished?.Invoke(this, -1);
                };
                Closing += (s, e) => { IsClosing = true; };
                SetupHandlers();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }

        public void SetupHandlers()
        {
            try
            {
                chkDeleteMonitored.MouseLeave += MouseLeaveEventHander;
                chkPersistantSource.MouseLeave += MouseLeaveEventHander;
                ComboShortType.MouseLeave += MouseLeaveEventHander;
                ChkShorts.MouseLeave += MouseLeaveEventHander;
                Chk720P.Click += OnMouseClick;
                ChkCut.Click += OnMouseClick;
                ChkEnableTrim.Click += OnMouseClick;
                ChkShorts.Click += OnMouseClick;
                ChkTwitch.Click += OnMouseClick;
                ChkMuxer.Click += OnMouseClick;
                ChkElapsed.Click += OnMouseClick;
                txtdestdir.KeyUp += keyupEventHandler;
                txtDuration.KeyUp += keyupEventHandler;
                txtsrcdir.KeyUp += keyupEventHandler;
                txtStart.KeyUp += keyupEventHandler;
                txtMuxExt.KeyUp += keyupEventHandler;
                DataObject.AddPastingHandler(txtStart, OntextPaste);
                DataObject.AddPastingHandler(txtDuration, OntextPaste);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void OntextPaste(object sender, DataObjectPastingEventArgs e)
        {
            try
            {
                if (!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true))
                    return;

                var pastedText = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
                if (string.IsNullOrEmpty(pastedText))
                    return;


                if (sender is TextBox txtBox)
                {

                    var text = Clipboard.GetData(DataFormats.Text);
                    string TextPasted = text.ToString();
                    if (TextPasted.Trim() != "" && TextPasted.Contains("-"))
                    {
                        ChkElapsed.IsChecked = false;
                        List<string> pasted = TextPasted.Split('-').ToList();
                        if (txtBox.Name == txtStart.Name)
                        {
                            txtStart.Clear();
                            txtStart.Text = pasted.FirstOrDefault();
                            txtDuration.Text = pasted.LastOrDefault();
                            txtBox.Text = pasted.FirstOrDefault();
                            StartTextBoxUpdater(0, pasted.FirstOrDefault());

                        }
                        if (txtBox.Name == txtDuration.Name)
                        {
                            txtDuration.Clear();
                            txtStart.Text = pasted.FirstOrDefault();
                            txtDuration.Text = pasted.LastOrDefault();
                            txtBox.Text = pasted.LastOrDefault();
                            StartTextBoxUpdater(1, pasted.LastOrDefault());

                        }
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void StartTextBoxUpdater(int _textbox, string _NewText)
        {
            try
            {
                NewTextBoxId = _textbox;
                NewText = _NewText;
                TextBoxUpdater = new System.Windows.Forms.Timer();
                TextBoxUpdater.Tick += new EventHandler(TextBoxUpdater_Tick);
                TextBoxUpdater.Interval = (int)new TimeSpan(0, 0, 1).TotalSeconds;
                TextBoxUpdater.Start();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void TextBoxUpdater_Tick(object? sender, EventArgs e)
        {
            try
            {
                TextBoxUpdater.Stop();
                if (NewTextBoxId is int Id && Id == 0)
                {
                    txtStart.Text = NewText;
                }
                if (NewTextBoxId is int Id2 && Id2 == 1)
                {
                    txtDuration.Text = NewText;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }




        private void mnuGroupPaste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int RecNum = 1;
                if (txtFilename.Text != "" && txtsrcdir.Text != "" && txtdestdir.Text != "")
                {
                    var text = Clipboard.GetData(DataFormats.Text);
                    string TextPasted = text.ToString();
                    if (TextPasted.Trim() != "" && TextPasted.Contains("-"))
                    {
                        List<string> times = TextPasted.Split(Environment.NewLine).ToList();
                        foreach (string time in times.Where(ti => ti.Trim() != ""))
                        {
                            List<string> timespans = time.Split("-").ToList();
                            TimeSpan Start = timespans.FirstOrDefault().FromStrToTimeSpan();
                            TimeSpan End = timespans.LastOrDefault().FromStrToTimeSpan() - Start;
                            DoAddRecord?.Invoke(true, false, false, 0, true, false, false, false,
                                 true, Start.ToFFmpeg(), End.ToFFmpeg(), txtsrcdir.Text,
                                 txtdestdir.Text + "\\" + txtFilename.Text + $" {RecNum++}");
                        }
                    }
                }
                else
                {
                    string Message = "Please Enter Source & Destination Data First";
                    MessageBox.Show(Message);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void mnuDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.Source is MenuItem mnu && mnu.DataContext is ComplexJobList item)
                {
                    int id = item.Id.ToInt();
                    DoRemoveRecord?.Invoke(id, false, true);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void tglflip_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void tglflip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!tglflip.IsChecked.Value)
                {
                    btnInject.Visibility = Visibility.Hidden;
                    deletemenuvisible = Visibility.Hidden;
                    AgeFilter = Visibility.Collapsed;
                    ModuleCallBack?.Invoke(this, new CustomParams_DataSelect(1));
                }
                else
                {
                    btnInject.Visibility = Visibility.Visible;
                    deletemenuvisible = Visibility.Visible;
                    AgeFilter = Visibility.Visible;
                    ModuleCallBack?.Invoke(this, new CustomParams_DataSelect(0));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        

        public void ResizeWindows(double _w, double _h, bool WidthChanged = false, 
            bool HeightChanged = false)
        {
            try
            {
                if (WidthChanged && Ready)
                {
                    msuComplexSchedules.Width = _w - 20;
                    brdtimes.Width = _w - (925-369);
                    brdFileInfo.Width = _w - 22;
                    CnvTimes.Width = brdtimes.Width - 2;
                }
                if (HeightChanged && Ready)
                {
                    msuComplexSchedules.Height = _h - (474-222)-15;
                }

                if (HeightChanged || WidthChanged && Ready)
                {
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key?.SetValue("CSWidth", ActualWidth);
                    key?.SetValue("CSHeight", ActualHeight);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ReiszeWindows {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        

        private void btnSelectDestDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("DestinationDir", "c:\\");
                destinationdir = rootfolder;
                key?.Close();
                FolderBrowserDialog ofg = new FolderBrowserDialog();
                ofg.AllowMultiSelect = false;
                ofg.InitialFolder = rootfolder;
                ofg.Title = "Select Destination Directory";

                if (ofg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtdestdir.Text = ofg.SelectedFolder;
                    RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key2.SetValue("DestinationDir", txtdestdir.Text);
                    key2?.Close();
                }
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
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("SourceDirFiles", @"c:\");
                string root = key.GetValueStr("SourceAdobe", @"c:\");
                key?.Close();
                //txtdestdir.Text = rootfolder;
                if (!ChkMuxer.IsChecked.Value)
                {
                    FolderBrowserDialog ofg = new FolderBrowserDialog();
                    ofg.AllowMultiSelect = false;
                    ofg.InitialFolder = rootfolder;
                    ofg.Title = "Select Source Directory";

                    if (ofg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        txtsrcdir.Text = ofg.SelectedFolder;
                        List<string> files = txtsrcdir.Text.Split('\\').ToList();
                        string p = files.LastOrDefault();
                        string r = ofg.SelectedFolder.Replace(p, "");
                        if (r.EndsWith("\\"))
                        {
                            r = r.Substring(0, r.Length - 1);
                        }
                        RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        key2.SetValue("SourceDirFiles", r);
                        key2?.Close();

                        string fname = "";
                        if (Chk720P.IsChecked == true)
                        {
                            fname = " (Edt720)";
                        }
                        else if (ChkShorts.IsChecked == true)
                        {
                            fname = " (shorts)";
                        }
                        txtFilename.Text = files.LastOrDefault() + fname;
                    }
                }
                else
                {
                    var fld = new OpenFileDialog();
                    fld.Filter = "mp4|*.mp4";
                    fld.DefaultExt = "*.mp4";
                    fld.DefaultDirectory = root;
                    fld.Multiselect = false;
                    var fd = fld.ShowDialog();
                    if ((fd != null) && (fd.Value == true))
                    {
                        string src = fld.FileName;
                        string SelectedDir = System.IO.Path.GetDirectoryName(src);
                        key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        key.SetValue("SourceAdobe", SelectedDir);
                        key?.Close();
                        txtsrcdir.Text = src;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
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

        private void btnCloe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /// this form is marked for mcu Listbox upgrade
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void MouseLeaveEventHander(object sender, MouseEventArgs e)
        {
            try
            {
                string CompName;
                if (sender is System.Windows.Controls.CheckBox FormCheckBox)
                {
                    bool CompChecked = false;
                    (CompName, CompChecked) = (FormCheckBox.Name, FormCheckBox.IsChecked.Value);
                    switch (CompName)
                    {
                        case "ComboShortType":
                            {
                                //no event needed
                                break;
                            }
                        case "Chk720P":
                            {
                                bool IsChecked = ChkShorts.IsChecked.Value || CompChecked;
                                SetEditboxes(!IsChecked);
                                if (CompChecked)
                                {
                                    ChkShorts.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    ChkCut.IsChecked = false;
                                    ComboShortType.Visibility = Visibility.Hidden;
                                }
                                break;
                            }
                        case "ChkShorts":
                            {
                                bool IsChecked = Chk720P.IsChecked.Value || CompChecked;
                                SetEditboxes(!IsChecked);
                                ComboShortType.SelectedIndex = (!IsChecked) ? -1 : ComboShortType.SelectedIndex;

                                if (CompChecked)
                                {
                                    Chk720P.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    ChkCut.IsChecked = false;
                                    ComboShortType.Visibility = Visibility.Visible;
                                }
                                break;
                            }
                        case "ChkEnableTrim":
                            {
                                bool IsChecked = !ChkShorts.IsChecked.Value || !Chk720P.IsChecked.Value || CompChecked;
                                SetEditboxes(!IsChecked);

                                if (CompChecked)
                                {
                                    Chk720P.IsChecked = false;
                                    ChkShorts.IsChecked = false;
                                    ChkCut.IsChecked = false;
                                    ComboShortType.Visibility = Visibility.Hidden;
                                }
                                break;
                            }

                        case "ChkElapsed":
                            {
                                RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                key2.SetValue("Elasped", CompChecked);
                                key2?.Close();
                                TimeSpan st = txtStart.Text.FromStrToTimeSpan();
                                TimeSpan Dur = txtDuration.Text.FromStrToTimeSpan();
                                if (st != TimeSpan.Zero && Dur != TimeSpan.Zero)
                                {
                                    if (ChkElapsed.IsChecked.Value)
                                    {
                                        Dur.Subtract(st);
                                    }
                                    else
                                    {
                                        Dur.Add(st);
                                    }
                                    txtDuration.Text = Dur.ToFFmpeg();
                                    tduration = txtDuration.Text;
                                }

                                break;
                            }
                        case "ChkCut":
                            {
                                bool IsChecked = !ChkShorts.IsChecked.Value || !Chk720P.IsChecked.Value || CompChecked;
                                SetEditboxes(!IsChecked);
                                if (CompChecked)
                                {
                                    Chk720P.IsChecked = false;
                                    ChkShorts.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    ComboShortType.Visibility = Visibility.Hidden;
                                }
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
        public void SetEditboxes(bool chk)
        {
            try
            {
                txtStart.IsEnabled = !chk;
                txtDuration.IsEnabled = !chk;
                if (!chk)
                {
                    txtStart.Text = tstart;
                    txtDuration.Text = tduration;
                    txtStart.Visibility = Visibility.Visible;
                }
                else
                {
                    tstart = txtStart.Text;
                    tduration = txtDuration.Text;
                    txtStart.Clear();
                    txtDuration.Clear();
                    txtStart.Visibility = Visibility.Hidden;
                }
                txtDuration.Visibility = txtStart.Visibility;
                ChkElapsed.Visibility = txtStart.Visibility;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


        private void lstSchedules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.Source is MenuItem mnu && mnu.DataContext is ComplexJobList item2)
                {
                    Chk720P.IsChecked = item2.Is720p;
                    ChkEnableTrim.IsChecked = item2.IsEncodeTrim;
                    ComboShortType.SelectedIndex = item2.IsCreateShorts;
                    ChkCut.IsChecked = item2.IsCutTrim;
                    chkDeleteMonitored.IsChecked = item2.IsDeleteMonitoredSource;
                    chkPersistantSource.IsChecked = item2.IsPersistentJob;
                    txtsrcdir.Text = item2.SourceDirectory;
                    txtdestdir.Text = item2.DestinationDirectory;
                    txtFilename.Text = item2.Filename.Replace(".mp4", "");
                    ReleaseDate.Value = item2.TwitchSchedule;
                    ChkTwitch.IsChecked = item2.IsTwitchStream;
                    ChkMuxer.IsChecked = item2.IsMuxed;
                    txtMuxExt.Text = item2.MuxData;
                    txtMuxExt.Visibility = (ChkMuxer.IsChecked.Value) ? Visibility.Visible : Visibility.Hidden;
                    if (ChkTwitch.IsChecked.Value || Chk720P.IsChecked.Value || ChkShorts.IsChecked.Value)
                    {
                        SetEditboxes(true);
                    }
                    else
                    {
                        SetEditboxes(false);
                    }
                    txtStart.Text = item2.Start.ToFFmpeg().Replace(".000", "");
                    bool Insert = false;
                    TimeSpan Final = TimeSpan.Zero;
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    bool IsChkElapsedIsChecked = key.GetValueBool("Elapsed", true);
                    key?.Close();

                    TimeSpan st = txtStart.Text.FromStrToTimeSpan();
                    TimeSpan Dur = item2.Duration;
                    if (st != TimeSpan.Zero && Dur != TimeSpan.Zero)
                    {
                        if (!ChkElapsed.IsChecked.Value)
                        {
                            Dur += st;
                        }
                        txtDuration.Text = Dur.ToFFmpeg().Replace(".000", "");
                        tduration = txtDuration.Text;
                    }
                    else
                    {
                        txtDuration.Text = item2.Duration.ToFFmpeg().Replace(".000", "");
                        tduration = txtDuration.Text;
                    }
                    tglflip.IsChecked = !tglflip.IsChecked.Value;
                    btnInject.Visibility = Visibility.Visible;
                    // run list change
                }
                if (e.Source is Button mnu2 && mnu2.DataContext is ComplexJobList item)
                {
                    Chk720P.IsChecked = item.Is720p;
                    ChkEnableTrim.IsChecked = item.IsEncodeTrim;
                    ChkShorts.IsChecked = item.IsShorts;
                    ChkCut.IsChecked = item.IsCutTrim;
                    chkDeleteMonitored.IsChecked = item.IsDeleteMonitoredSource;
                    chkPersistantSource.IsChecked = item.IsPersistentJob;
                    txtsrcdir.Text = item.SourceDirectory;
                    txtdestdir.Text = item.DestinationDirectory;
                    txtFilename.Text = item.Filename.Replace(".mp4", "");
                    ReleaseDate.Value = item.TwitchSchedule;
                    ChkTwitch.IsChecked = item.IsTwitchStream;
                    ChkMuxer.IsChecked = item.IsMuxed;
                    txtMuxExt.Text = item.MuxData;
                    txtMuxExt.Visibility = (ChkMuxer.IsChecked.Value) ? Visibility.Visible : Visibility.Hidden;
                    if (ChkTwitch.IsChecked.Value || Chk720P.IsChecked.Value || ChkShorts.IsChecked.Value)
                    {
                        SetEditboxes(true);
                    }
                    else
                    {
                        SetEditboxes(false);
                    }
                    txtStart.Text = item.Start.ToFFmpeg().Replace(".000", "");
                    bool Insert = false;
                    TimeSpan Final = TimeSpan.Zero;
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    bool IsChkElapsedIsChecked = key.GetValueBool("Elapsed", true);
                    key?.Close();
                    TimeSpan st = txtStart.Text.FromStrToTimeSpan();
                    TimeSpan Dur = item.Duration;
                    if (st != TimeSpan.Zero && Dur != TimeSpan.Zero)
                    {
                        if (!ChkElapsed.IsChecked.Value)
                        {
                            Dur += st;
                        }
                        txtDuration.Text = Dur.ToFFmpeg().Replace(".000", "");
                        tduration = txtDuration.Text;
                    }
                    else
                    {
                        txtDuration.Text = item.Duration.ToFFmpeg().Replace(".000", "");
                        tduration = txtDuration.Text;
                    }
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
                SetupHandlers();
                tglflip.IsChecked = true;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("DestinationDir", "c:\\");
                key?.Close();
                txtdestdir.Text = rootfolder;
                ModuleCallBack?.Invoke(this, new CustomParams_Initialize(0));
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
                        key2?.SetValue("CSleft", Left);
                        key2?.SetValue("CStop", Top);
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
                var _width = key.GetValue("CSWidth", ActualWidth).ToDouble();
                var _height = key.GetValue("CSHeight", ActualHeight).ToDouble();
                var _left = key.GetValue("CSleft", Left).ToDouble();
                var _top = key.GetValue("CStop", Top).ToDouble();
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

        private void btnInject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                key2.SetValue("Elasped", ChkElapsed.IsChecked.Value);
                string RTMP = key2.GetValueStr("RTMP", "rtmp://syd03.contribute.live-video.net/app/live_1061414984_Vu5NrETzHYqB1f4bZO12dxaCOfUkxf");
                key2?.Close();

                string duration = txtDuration.Text;
                if (!ChkElapsed.IsChecked.Value)
                {
                    TimeSpan st = txtStart.Text.FromStrToTimeSpan();
                    TimeSpan dur = txtDuration.Text.FromStrToTimeSpan();
                    if (st != TimeSpan.Zero && dur != TimeSpan.Zero)
                    {
                        dur.Subtract(st);
                        duration = dur.ToFFmpeg();
                    }
                    else duration = txtDuration.Text;
                }
                bool IsAdobe = txtdestdir.Text.ToLower() == adobedir.ToLower();
                Nullable<DateTime> twitchdata = null;
                twitchdata = (ReleaseDate.Value.HasValue) ? ReleaseDate.Value.Value : null;
                DoAddRecord?.Invoke(ChkElapsed.IsChecked.Value, Chk720P.IsChecked.Value,
                    ChkShorts.IsChecked.Value, ComboShortType.SelectedIndex,  // number 4
                    ChkEnableTrim.IsChecked.Value, ChkCut.IsChecked.Value,
                    chkDeleteMonitored.IsChecked.Value,
                    chkPersistantSource.IsChecked.Value,
                    IsAdobe,
                    txtStart.Text, duration, txtsrcdir.Text,
                    txtdestdir.Text + "\\" + txtFilename.Text,
                    twitchdata, (ChkTwitch.IsChecked.Value ? RTMP : ""), ChkTwitch.IsChecked.Value,
                    ChkMuxer.IsChecked.Value, txtMuxExt.Text);

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void DialogAgesOnFinish(object sender, int i)
        {
            try
            {
                if (sender is DialogAges d)
                {
                    d = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void mnuSetAgeFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DialogAgesEntry is null)
                {
                    int min = -1, max = -1;
                    (min, max) = (DoGetFilterAges != null) ? DoGetFilterAges.Invoke() : (min, max);
                    DialogAgesEntry = new DialogAges("Select Age Ranges", min, max,
                        DoSetFilterAge, DialogAgesOnFinish);
                    DialogAgesEntry.ShowActivated = true;
                    DialogAgesEntry.Show();
                }
                else
                {
                    DialogAgesEntry.Focus();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void DestinationDirectoryEntry_OnFinish(object sender, int i)
        {
            try
            {
                if (sender is CustomStringEntry frm) frm = null;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void SourceDirectoryEntry_OnFinish(object sender, int i)
        {
            try
            {
                if (sender is CustomStringEntry frm)
                {
                    frm = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void DestinatinFileEntry_OnFinish(object sender, int i)
        {
            try
            {
                if (sender is CustomStringEntry frm) frm = null;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void OnKeyPress_Source(Key key, string Data)
        {
            try
            {
                if (key != Key.Enter && key != Key.Next && key != Key.CapsLock
                    && key != Key.Down && key != Key.Up && key != Key.Left && key != Key.Right)
                {
                    FilterClass FilterType = (tglflip.IsChecked.Value) ? FilterClass.Current : FilterClass.Historic;
                    DoSetFilterString?.Invoke(Data, FilterTypes.SourceDirectory, FilterType);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void OnKeyPress_DestinationDirection(Key key, string Data)
        {
            try
            {
                if (key != Key.Enter && key != Key.Next && key != Key.CapsLock
                    && key != Key.Down && key != Key.Up && key != Key.Left && key != Key.Right)
                {
                    FilterClass FilterType = (tglflip.IsChecked.Value) ? FilterClass.Current : FilterClass.Historic;
                    DoSetFilterString?.Invoke(Data, FilterTypes.DestinationDirectory, FilterType);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void OnKeyPress_DestinationFile(Key key, string Data)
        {
            try
            {
                if (key != Key.Enter && key != Key.Next && key != Key.CapsLock
                    && key != Key.Down && key != Key.Up && key != Key.Left && key != Key.Right)
                {
                    FilterClass FilterType = (tglflip.IsChecked.Value) ? FilterClass.Current : FilterClass.Historic;
                    DoSetFilterString?.Invoke(Data, FilterTypes.DestinationFileName, FilterType);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void mnuSetSourceFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SourceDirectoryEntry is null)
                {
                    FilterClass FilterType = (tglflip.IsChecked.Value) ? FilterClass.Current : FilterClass.Historic;
                    string Filter = DoGetFilerString(FilterTypes.SourceDirectory, FilterType);
                    SourceDirectoryEntry = new CustomStringEntry("Filter Source Directory", Filter, OnKeyPress_Source, SourceDirectoryEntry_OnFinish);
                    SourceDirectoryEntry.ShowActivated = true;
                    SourceDirectoryEntry.Show();
                }
                else SourceDirectoryEntry.Focus();

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }



        private void mnuSetDestinationDirectoryFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DestinationDirectoryEntry is null)
                {
                    FilterClass FilterType = (tglflip.IsChecked.Value) ? FilterClass.Current : FilterClass.Historic;
                    string Filter = DoGetFilerString(FilterTypes.DestinationDirectory, FilterType);
                    DestinationDirectoryEntry = new CustomStringEntry("Filter Destination Directory", Filter,
                        OnKeyPress_DestinationDirection, DestinationDirectoryEntry_OnFinish);
                    DestinationDirectoryEntry.ShowActivated = true;
                    DestinationDirectoryEntry.Show();
                }
                else DestinationDirectoryEntry.Focus();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }

        private void mnuSetDestinationFileFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DestinationFileEntry is null)
                {
                    FilterClass FilterType = (tglflip.IsChecked.Value) ? FilterClass.Current : FilterClass.Historic;
                    string Filter = DoGetFilerString(FilterTypes.DestinationFileName, FilterType);
                    DestinationFileEntry = new CustomStringEntry("Filter Destination File", Filter,
                        OnKeyPress_DestinationFile, DestinatinFileEntry_OnFinish);
                    DestinationFileEntry.ShowActivated = true;
                    DestinationFileEntry.Show();
                }
                else DestinationFileEntry.Focus();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void mnuDeleteCurrentSelection_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem mnu && mnu.DataContext is ComplexJobList item)
            {
                int id = item.Id.ToInt();
                DoRemoveRecord?.Invoke(id);
            }
            if (e.OriginalSource is MenuItem mnu1 && mnu1.DataContext is ComplexJobHistory itemx)
            {
                int id = itemx.Id.ToInt();
                DoRemoveRecord?.Invoke(id);
            }
        }

        private void ChkTwitch_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ChkTwitch.IsChecked.Value)
                {
                    ReleaseDate.Value = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void mnuChangeDestHeaderHistoric_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mnuChangeFilenameHeaderHistoric_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddMux_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string root = key.GetValueStr("SourceAdobe", @"c:\");
                key?.Close();

                if (root != @"c:\")
                {
                    string last = root.Split('\\').ToList().LastOrDefault();
                    string newdir = root.Replace(last, "filtered");
                    List<string> tobeprocessed = new List<string>();
                    var processingfiles = Directory.EnumerateFiles(root, "*.mp4", SearchOption.AllDirectories).ToList();
                    var processedfiles = Directory.EnumerateFiles(newdir, "*.mp4", SearchOption.AllDirectories).ToList();
                    List<string> pf = processedfiles.Select(file => Path.GetFileNameWithoutExtension(file)).ToList();
                    foreach (var file in processingfiles)
                    {
                        string dir = Path.GetDirectoryName(file);
                        string fileb = Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + "b.mp3");
                        string filep = Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + "p.mp3");
                        string fileo = Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + "o.mp3");
                        if (File.Exists(fileb) && File.Exists(filep) && File.Exists(fileo))
                        {
                            tobeprocessed.Add(file);
                        }
                    }
                    foreach (var tbpfile in tobeprocessed)
                    {
                        DoAddRecord?.Invoke(ChkElapsed.IsChecked.Value, Chk720P.IsChecked.Value,
                            ChkShorts.IsChecked.Value, ComboShortType.SelectedIndex,
                            ChkEnableTrim.IsChecked.Value, ChkCut.IsChecked.Value,
                            chkDeleteMonitored.IsChecked.Value, chkPersistantSource.IsChecked.Value,
                            false, txtStart.Text, "", tbpfile,
                            txtdestdir.Text + "\\" + txtFilename.Text,
                            null, "", ChkTwitch.IsChecked.Value,
                            ChkMuxer.IsChecked.Value, txtMuxExt.Text);
                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnAddMux_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void OnMouseClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string CompName;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("DestinationDir", "c:\\");
                destinationdir = rootfolder;
                key?.Close();

                if (sender is System.Windows.Controls.CheckBox FormCheckBox)
                {
                    bool CompChecked = false;
                    (CompName, CompChecked) = (FormCheckBox.Name, FormCheckBox.IsChecked.Value);
                    switch (CompName)
                    {
                        case "ChkMuxer":
                            {
                                bool IsCheckedMux = ChkMuxer.IsChecked.Value || CompChecked;
                                txtMuxExt.Visibility = (IsCheckedMux) ? Visibility.Visible : Visibility.Hidden;
                                lblmux.Visibility = txtMuxExt.Visibility;
                                btnAddMux.Visibility = txtMuxExt.Visibility;
                                ComboShortType.SelectedIndex = -1;
                                Chk720P.IsChecked = false;
                                ChkEnableTrim.IsChecked = false;
                                ChkCut.IsChecked = false;
                                ChkShorts.IsChecked = false;
                                ChkTwitch.IsChecked = false;
                                ComboShortType.Visibility = Visibility.Hidden;
                                break;
                            }
                        case "Chk720P":
                            {
                                bool IsChecked = ChkShorts.IsChecked.Value || CompChecked;

                                SetEditboxes(IsChecked);
                                if (CompChecked)
                                {
                                    txtMuxExt.Visibility = (!CompChecked) ? Visibility.Visible : Visibility.Hidden;
                                    lblmux.Visibility = txtMuxExt.Visibility;

                                    ChkShorts.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    ChkCut.IsChecked = false;
                                    txtdestdir.Text = destinationdir;
                                    ComboShortType.Visibility = Visibility.Hidden;
                                }
                                break;
                            }
                        case "ChkShorts":
                            {
                                bool IsChecked = Chk720P.IsChecked.Value || CompChecked;
                                ComboShortType.SelectedIndex = (!IsChecked) ? -1 : ComboShortType.SelectedIndex;
                                SetEditboxes(IsChecked);
                                if (CompChecked)
                                {
                                    txtMuxExt.Visibility = (!CompChecked) ? Visibility.Visible : Visibility.Hidden;
                                    lblmux.Visibility = txtMuxExt.Visibility;

                                    Chk720P.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    ChkCut.IsChecked = false;
                                    ComboShortType.SelectedIndex = 1;
                                    txtdestdir.Text = destinationdir;
                                    ComboShortType.Visibility = Visibility.Visible;
                                }
                                break;
                            }
                        case "ChkCreateShorts":
                            {
                                bool IsChecked = ChkShorts.IsChecked.Value && CompChecked;
                                txtMuxExt.Visibility = (!CompChecked) ? Visibility.Visible : Visibility.Hidden;
                                lblmux.Visibility = txtMuxExt.Visibility;

                                SetEditboxes(IsChecked);
                                ComboShortType.SelectedIndex = (!IsChecked) ? -1 : ComboShortType.SelectedIndex;
                                break;
                            }

                        case "ChkTwitch":
                            {
                                if (CompChecked)
                                {
                                    txtMuxExt.Visibility = (!CompChecked) ? Visibility.Visible : Visibility.Hidden;
                                    lblmux.Visibility = txtMuxExt.Visibility;
                                    ComboShortType.SelectedIndex = -1;// IsChecked = false;
                                    Chk720P.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    ChkCut.IsChecked = false;
                                    ComboShortType.Visibility = Visibility.Hidden;
                                }

                                ReleaseDate.Visibility = CompChecked ? Visibility.Visible : Visibility.Hidden;
                                SetEditboxes(!CompChecked);
                                break;
                            }

                        case "ChkEnableTrim":
                            {
                                bool IsChecked = !ChkShorts.IsChecked.Value || !Chk720P.IsChecked.Value || CompChecked;
                                txtMuxExt.Visibility = (!CompChecked) ? Visibility.Visible : Visibility.Hidden;
                                lblmux.Visibility = txtMuxExt.Visibility;

                                SetEditboxes(!IsChecked);
                                if (CompChecked)
                                {
                                    Chk720P.IsChecked = false;
                                    ChkShorts.IsChecked = false;
                                    ChkCut.IsChecked = false;
                                    txtdestdir.Text = adobedir;
                                    ComboShortType.Visibility = Visibility.Hidden;
                                }
                                break;
                            }
                        case "ChkElapsed":
                            {
                                RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                key2.SetValue("Elasped", ChkElapsed.IsChecked.Value);
                                TimeSpan st = txtStart.Text.FromStrToTimeSpan();
                                TimeSpan Dur = txtDuration.Text.FromStrToTimeSpan();
                                if (st != TimeSpan.Zero && Dur != TimeSpan.Zero)
                                {
                                    if (CompChecked)
                                    {
                                        Dur -= st;
                                    }
                                    else
                                    {
                                        Dur += st;
                                    }
                                    txtDuration.Text = Dur.ToFFmpeg().Replace(".000", "");
                                    tduration = txtDuration.Text;
                                }
                                key2?.Close();
                                break;
                            }
                        case "ChkCut":
                            {
                                bool IsChecked = !ChkShorts.IsChecked.Value || !Chk720P.IsChecked.Value || CompChecked;
                                SetEditboxes(!IsChecked);
                                if (CompChecked)
                                {
                                    txtMuxExt.Visibility = (!CompChecked) ? Visibility.Visible : Visibility.Hidden;
                                    lblmux.Visibility = txtMuxExt.Visibility;

                                    Chk720P.IsChecked = false;
                                    ChkShorts.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    txtdestdir.Text = adobedir;

                                }
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

        private void keyupEventHandler(object sender, KeyEventArgs e)
        {
            try
            {
                string CompName;
                if (sender is System.Windows.Controls.TextBox FormTextBox)
                {
                    CompName = FormTextBox.Name;
                    if (e.Key == Key.Enter)
                    {
                        switch (CompName)
                        {
                            case "txtsrcdir":
                                {
                                    txtdestdir.Focus();
                                    break;
                                }
                            case "txtdestdir":
                                {
                                    txtFilename.Focus();
                                    break;
                                }
                            case "txtFilename":
                                {
                                    chkDeleteMonitored.Focus();
                                    break;
                                }
                            case "txtStart":
                                {
                                    txtDuration.Focus();
                                    break;
                                }
                            case "txtDuration":
                                {
                                    ChkElapsed.Focus();
                                    break;
                                }
                            case "txtMuxExt":
                                {
                                    txtMuxExt.Focus();
                                    break;
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
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Chk720P.IsChecked = false;
                ChkShorts.IsChecked = false;
                ChkCut.IsChecked = false;
                chkDeleteMonitored.IsChecked = false;
                chkPersistantSource.IsChecked = false;
                ChkEnableTrim.IsChecked = false;
                txtStart.Clear();
                txtStart.IsEnabled = true;
                txtDuration.Clear();
                txtDuration.IsEnabled = true;
                tstart = "";
                tduration = "";
                txtsrcdir.Clear();
                txtdestdir.Clear();
                txtFilename.Clear();
                ChkMuxer.IsChecked = false;
                txtMuxExt.Text = "";
                SetEditboxes(false);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
