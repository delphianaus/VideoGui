﻿using System;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VideoGui.Models;
using System.Windows.Markup;
using Microsoft.Win32;
using VideoGui.Models.delegates;
using FolderBrowserDialog = FolderBrowserEx.FolderBrowserDialog;

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
        public ComplexFinished ComplexOnClose;
        public SetLists DoSetLists;
        public GetFilerString DoGetFilerString;


        public GetFilterAges DoGetFilterAges;
        public Visibility deletemenuvisible = Visibility.Visible;
        public Visibility AgeFilter = Visibility.Collapsed;
        public DialogAges DialogAgesEntry = null;
        public CustomStringEntry SourceDirectoryEntry = null;
        public CustomStringEntry DestinationDirectoryEntry = null;
        public CustomStringEntry DestinationFileEntry = null;

        public ComplexSchedular(AddRecordDelegate _AdddRecord, RemoveRecordDelegate _RemoveRecord,
            ComplexFinished _ComplexFinished, SetLists _SetLists, SetFilterAge _SetFilterAge,
            SetFilterString _SetFilterString, GetFilterAges _GetFilterAges, GetFilerString _GetFilterString)
        {
            try
            {
                InitializeComponent();
                LastWidth = ActualWidth;
                //lstSchedules.ItemsSource = data;
                DoAddRecord = _AdddRecord;
                DoGetFilterAges = _GetFilterAges;
                DoRemoveRecord = _RemoveRecord;
                ComplexOnClose = _ComplexFinished;
                DoSetLists = _SetLists;

                DoSetFilterAge = _SetFilterAge;
                DoSetFilterString = _SetFilterString;
                DoGetFilerString = _GetFilterString;
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
                Chk720P.Click += OnMouseClick;
                ChkCut.Click += OnMouseClick;
                ChkEnableTrim.Click += OnMouseClick;
                ChkShorts.Click += OnMouseClick;
                chkCreateShorts.Click += OnMouseClick;
                ChkElapsed.Click += OnMouseClick;
                txtdestdir.KeyUp += keyupEventHandler;
                txtDuration.KeyUp += keyupEventHandler;
                txtsrcdir.KeyUp += keyupEventHandler;
                txtStart.KeyUp += keyupEventHandler;

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

        public void CreateTemplate(int srcwidth = 200, int destwidth = 240, int timeswidth = 127, int RecordAge = 30, int processingwidth = 30,
            int ProcessingsActionsWidth = 100)
        {
            try
            {
                var xamlString =
               "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                  "<Grid Margin=\"0,2\">" +
                    "<Grid.ColumnDefinitions>" +
                        "<ColumnDefinition Width=\"0\"/>" +
                       $"<ColumnDefinition Width=\"{srcwidth}\"/>" +
                       $"<ColumnDefinition Width=\"{destwidth}\"/>" +
                       $"<ColumnDefinition Width=\"{timeswidth}\"/>" +
                       $"<ColumnDefinition Width=\"{RecordAge}\"/>" +
                       $"<ColumnDefinition Width=\"{processingwidth}\"/>" +
                       $"<ColumnDefinition Width=\"{ProcessingsActionsWidth}\"/>" +
                    "</Grid.ColumnDefinitions>" +
                    "<TextBlock Name=\"Idx\" Grid.Column=\"0\" Text=\"{Binding Id}\" Width=\"0\"/>" +
                    "<TextBlock Name=\"SRC\" Grid.Column=\"1\" Text=\"{Binding SRC}\" Width=\"" + srcwidth.ToString() + "}\"/>" +
                    "<TextBlock Name=\"DEST\" Grid.Column=\"2\" Text=\"{Binding DEST}\" Width=\"" + destwidth.ToString() + "\"/>" +
                    "<TextBlock Name=\"Times\" Grid.Column=\"3\" Text=\"{Binding Times}\" Width=\"" + timeswidth.ToString() + "\"/>" +
                    "<TextBlock Name=\"RecordAge\" Grid.Column=\"4\" Text=\"{Binding RecordAge}\" Width=\"" + RecordAge.ToString() + "30\"/>" +
                    "<TextBlock Name=\"ProcessingType\" Grid.Column=\"5\" Text=\"{Binding ProceessingType}\" Width=\"" + processingwidth.ToString() + "\"/>" +
                    "<TextBlock Name=\"ProcessingActions\" Grid.Column=\"6\" Text=\"{Binding ProcessingActions}\" Width=\"" + ProcessingsActionsWidth.ToString() + "\"/>" +
                  "</Grid>" +
               "</DataTemplate>";
                var dataTemplate = (DataTemplate)XamlReader.Parse(xamlString);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void mnuChangeFilenameHeader_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //IsFileNameCompacted = !IsFileNameCompacted;

                //if (IsFileNameCompacted)
                //{
                //    destcolwidth += 150;
                //    filenamecolwidth -= 150;
                // }
                // else
                //{
                //   destcolwidth -= 150;
                //   filenamecolwidth += 150;
                // }
                //lstItems.Template = null;
                //lstItems.Template = (ControlTemplate)FindName("ControlMainGrid");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void mnuChangeDestHeader_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //IsDestCompacted = !IsDestCompacted;
                // if (IsDestCompacted)
                //{
                //   destcolwidth -= 150;
                //  filenamecolwidth += 150;
                // }
                //else
                //{
                //   destcolwidth += 150;
                //   filenamecolwidth -= 150;
                //}
                //lstItems.Template = null;
                //lstItems.Template = (ControlTemplate)FindName("ControlMainGrid");
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
                        foreach (string time in times)
                        {
                            if (time != "")
                            {
                                System.Windows.Forms.Application.DoEvents();
                                List<string> timespans = time.Split("-").ToList();
                                TimeSpan Start = timespans.FirstOrDefault().FromStrToTimeSpan();
                                TimeSpan End = timespans.LastOrDefault().FromStrToTimeSpan() - Start;
                                DoAddRecord?.Invoke(true, false, false, false,true, false, false, false,
                                     true, Start.ToFFmpeg(), End.ToFFmpeg(), txtsrcdir.Text,
                                     txtdestdir.Text + "\\" + txtFilename.Text + $" {RecNum++}");
                                System.Windows.Forms.Application.DoEvents();

                            }
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
                if (lstSchedules.SelectedItem is ComplexJobList item)
                {
                    if (item is not null)
                    {
                        int id = item.Id.ToInt();
                        DoRemoveRecord?.Invoke(id, false, true);
                    }
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
                    mnuGroupPaste1.Visibility = Visibility.Hidden;
                    deletemenuvisible = Visibility.Hidden;
                    AgeFilter = Visibility.Collapsed;
                    DoSetLists?.Invoke(1);
                }
                else
                {
                    btnInject.Visibility = Visibility.Visible;
                    mnuGroupPaste1.Visibility = Visibility.Visible;
                    deletemenuvisible = Visibility.Visible;
                    AgeFilter = Visibility.Visible;
                    DoSetLists?.Invoke(0);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ResizeAuto(object? sender, EventArgs e)
        {
            try
            {
                TrayIcon.Stop();

                ResizeAutos(sender, e);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ResizeAutos(object? sender, EventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    var rh = ActualHeight;
                    double rp = ActualWidth - 787;
                    if (rh > 0)
                    {
                        brd1.Height = rh - 240;
                        lstSchedules.Height = brd1.Height - 41;
                        lstSchedules.Width = brd1.Width;
                        LastWidth = ActualWidth;
                        CnvMedialElements.Width = brdFileInfo.Width - 5;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


        private void btnSelectDestDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("DestinationDir", "c:\\");
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
                string rootfolder = key.GetValueStr("SourceDirFiles", "c:\\");
                key?.Close();
                //txtdestdir.Text = rootfolder;
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
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    ResizeAutos(sender, e);
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
                        case "Chk720P":
                            {
                                bool IsChecked = ChkShorts.IsChecked.Value || CompChecked;
                                SetEditboxes(!IsChecked);
                                if (CompChecked)
                                {
                                    ChkShorts.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    ChkCut.IsChecked = false;
                                }
                                break;
                            }
                        case "ChkShorts":
                            {
                                bool IsChecked = Chk720P.IsChecked.Value || CompChecked;
                                SetEditboxes(!IsChecked);
                                chkCreateShorts.IsChecked = (!IsChecked) ? false : chkCreateShorts.IsChecked;
                                
                                if (CompChecked)
                                {
                                    Chk720P.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    ChkCut.IsChecked = false;
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
                if (lstSchedules.SelectedItem is ComplexJobHistory item2)
                {
                    Chk720P.IsChecked = item2.Is720p;
                    ChkEnableTrim.IsChecked = item2.IsEncodeTrim;
                    ChkShorts.IsChecked = item2.IsCreateShorts;
                    ChkCut.IsChecked = item2.IsCutTrim;
                    chkDeleteMonitored.IsChecked = item2.IsDeleteMonitoredSource;
                    chkPersistantSource.IsChecked = item2.IsPersistentJob;
                    txtsrcdir.Text = item2.SourceDirectory;
                    txtdestdir.Text = item2.DestinationDirectory;
                    txtFilename.Text = item2.Filename.Replace(".mp4", "");
                    if (Chk720P.IsChecked.Value || ChkShorts.IsChecked.Value)
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
                    mnuGroupPaste1.Visibility = Visibility.Visible;
                    // run list change
                }
                if (lstSchedules.SelectedItem is ComplexJobList item)
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
                    if (Chk720P.IsChecked.Value || ChkShorts.IsChecked.Value)
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
                TrayIcon = new DispatcherTimer();
                TrayIcon.Tick += new EventHandler(ResizeAuto);
                TrayIcon.Interval = new TimeSpan(0, 0, 0, 150);
                TrayIcon.Start();
                tglflip.IsChecked = true;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("DestinationDir", "c:\\");
                key?.Close();
                txtdestdir.Text = rootfolder;
                DoSetLists?.Invoke(0);// Set to true for Current
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnInject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                key2.SetValue("Elasped", ChkElapsed.IsChecked.Value);
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

                DoAddRecord?.Invoke(ChkElapsed.IsChecked.Value, Chk720P.IsChecked.Value, 
                    ChkShorts.IsChecked.Value, chkCreateShorts.IsChecked.Value,
                    ChkEnableTrim.IsChecked.Value, ChkCut.IsChecked.Value,
                    chkDeleteMonitored.IsChecked.Value,
                    chkPersistantSource.IsChecked.Value,
                    IsAdobe,
                    txtStart.Text, duration, txtsrcdir.Text,
                    txtdestdir.Text + "\\" + txtFilename.Text);

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void DialogAgesOnFinish()
        {
            try
            {
                if (!DialogAgesEntry.IsActive)
                {
                    DialogAgesEntry = null;
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
                    DialogAgesEntry = new DialogAges("Select Age Ranges", min, max, DoSetFilterAge, DialogAgesOnFinish);
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

        public void DestinationDirectoryEntry_OnFinish()
        {
            try
            {
                DestinationDirectoryEntry = null;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void SourceDirectoryEntry_OnFinish()
        {
            try
            {
                SourceDirectoryEntry = null;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void DestinatinFileEntry_OnFinish()
        {
            try
            {
                DestinationFileEntry = null;
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
                if (SourceDirectoryEntry == null)
                {
                    FilterClass FilterType = (tglflip.IsChecked.Value) ? FilterClass.Current : FilterClass.Historic;
                    string Filter = DoGetFilerString(FilterTypes.SourceDirectory, FilterType);
                    SourceDirectoryEntry = new CustomStringEntry("Filter Source Directory", Filter, OnKeyPress_Source, SourceDirectoryEntry_OnFinish);
                    SourceDirectoryEntry.Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                DialogAgesEntry?.Close();
                SourceDirectoryEntry?.Close();
                DestinationDirectoryEntry?.Close();
                DestinationFileEntry?.Close();
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
                if (DestinationDirectoryEntry == null)
                {
                    FilterClass FilterType = (tglflip.IsChecked.Value) ? FilterClass.Current : FilterClass.Historic;
                    string Filter = DoGetFilerString(FilterTypes.DestinationDirectory, FilterType);
                    DestinationDirectoryEntry = new CustomStringEntry("Filter Destination Directory", Filter, OnKeyPress_DestinationDirection, DestinationDirectoryEntry_OnFinish);
                    DestinationDirectoryEntry.Show();
                }
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
                if (DestinationFileEntry == null)
                {
                    FilterClass FilterType = (tglflip.IsChecked.Value) ? FilterClass.Current : FilterClass.Historic;
                    string Filter = DoGetFilerString(FilterTypes.DestinationFileName, FilterType);
                    DestinationFileEntry = new CustomStringEntry("Filter Destination File", Filter, OnKeyPress_DestinationFile, DestinatinFileEntry_OnFinish);
                    DestinationFileEntry.Show();
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void mnuDeleteCurrentSelection_Click(object sender, RoutedEventArgs e)
        {
            if (lstSchedules.SelectedItem is ComplexJobList item)
            {
                if (item is not null)
                {
                    int id = item.Id.ToInt();
                    DoRemoveRecord?.Invoke(id);
                }
            }
            if (lstSchedules.SelectedItem is ComplexJobHistory itemx)
            {
                if (itemx is not null)
                {
                    int id = itemx.Id.ToInt();
                    DoRemoveRecord?.Invoke(id);
                }
            }
        }

        private void OnMouseClick(object sender, RoutedEventArgs e)
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
                        case "Chk720P":
                            {
                                bool IsChecked = ChkShorts.IsChecked.Value || CompChecked;
                                SetEditboxes(IsChecked);
                                if (CompChecked)
                                {
                                    ChkShorts.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    ChkCut.IsChecked = false;
                                    txtdestdir.Text = destinationdir;
                                }
                                break;
                            }
                        case "ChkShorts":
                            {
                                bool IsChecked = Chk720P.IsChecked.Value || CompChecked;
                                chkCreateShorts.IsChecked = (!IsChecked) ? false : chkCreateShorts.IsChecked;
                                SetEditboxes(IsChecked);
                                if (CompChecked)
                                {
                                    Chk720P.IsChecked = false;
                                    ChkEnableTrim.IsChecked = false;
                                    ChkCut.IsChecked = false;
                                    txtdestdir.Text = destinationdir;
                                }
                                break;
                            }
                        case "ChkCreateShorts":
                            {
                                bool IsChecked = ChkShorts.IsChecked.Value && CompChecked;
                                SetEditboxes(IsChecked);
                                chkCreateShorts.IsChecked = (!IsChecked) ? false : chkCreateShorts.IsChecked;
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
                                    txtdestdir.Text = adobedir;
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
                SetEditboxes(false);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
