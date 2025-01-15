using FolderBrowserEx;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static VideoGui.ffmpeg.Probe.FormatModel;
using FolderBrowserDialog = FolderBrowserEx.FolderBrowserDialog;
using Path = System.IO.Path;


namespace VideoGui
{
    /// </summary>
    public partial class ConfigurationSettings : Window
    {
        bool SettingsLoaded = false;
        public ConfigurationSettings()
        {
            InitializeComponent();
           
        }
        public void LoadSettings()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                txtVtag.Text = key.GetValueStr("vtag");
                txtqScale.Text = key.GetValueStr("qscale");//, string.Empty) : string.Empty);
                txtMinQ.Text = key.GetValueStr("qmin");//, string.Empty) : string.Empty);
                txtMaxQ.Text = key.GetValueStr("qmax");// : string.Empty);
                string defaultdrive = System.IO.Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
                this.SetChecked("ChkChangeOutputname", key.GetValueBool("ChangeFileName"));
                this.SetChecked("ChkDropFormat", key.GetValueBool("DropFormat", true));
                this.SetChecked("ChkReEncode", key.GetValueBool("reencodefile"));
                txtqScale.Text = key.GetValueInt("qscale", 15);
                txtVtag.Text = key.GetValueStr("vTag", "XVID");
                txtMinQ.Text = key?.GetValueStr("qmin", 3);
                txtMaxQ.Text = key?.GetValueStr("qmax", 13);
                chkmovecompleted.IsChecked = key.GetValueBool("movecompleted", true);
                ChkMonitorDownloads.IsChecked = key.GetValueBool("MonitorDownloads", true);
                txtSrc720p.Text = key.GetValueStr("SourceDirectory720p", defaultdrive);
                txtSrc1080p.Text = key.GetValueStr("SourceDirectory1080p", defaultdrive);
                txtSrc4K.Text = key.GetValueStr("SourceDirectory4K", defaultdrive);
                txtSrc4KAdobe.Text = key.GetValueStr("SourceDirectory4KAdobe", defaultdrive);
                txtComp720p.Text = key.GetValueStr("CompDirectory720p", defaultdrive);
                txtComp1080p.Text = key.GetValueStr("CompDirectory1080p", defaultdrive);
                txtComp4K.Text = key.GetValueStr("CompDirectory4K", defaultdrive);
                txtComp4KAdobe.Text = key.GetValueStr("CompDirectory4KAdobe", defaultdrive);
                txtDone720p.Text = key.GetValueStr("DestDirectory720p", defaultdrive);
                txtDone1080p.Text = key.GetValueStr("DestDirectory1080p", defaultdrive);
                txtDone4k.Text = key.GetValueStr("DestDirectory4k", defaultdrive);
                txtDone4KAdobe.Text = key.GetValueStr("DestDirectoryAdobe4k", defaultdrive);
                txtErrorPath.Text = key.GetValueStr("ErrorDirectory", defaultdrive);
                txtShortspath.Text = key.GetValueStr("shortsdirectory", defaultdrive);
                this.SetChecked("ChkAudioConversion", key.GetValueBool("AudioConversionAC3", true));
                this.SetChecked("ChkAutoAAC", key.GetValueBool("AutoAAC"));
                this.SetSelectedIndex("cmbaudiomode", key.GetValueInt("Audiomode", 0));
                txtMin.Text = key.GetValueStr("minbitrate", "675K");
                txtMax.Text = key.GetValueStr("maxbitrate", "1150K");
                txtBuffer.Text = key.GetValueStr("buffer", "1200K");
                chkResize.IsChecked = key.GetValueBool("resize", true);
                ChkARScaling.IsChecked = key.GetValueBool("ARScaling", true);
                ChkRounding.IsChecked = key.GetValueBool("Rounding", true);
                ChkVSYNC.IsChecked = key.GetValueBool("VSYNC", true);
                txtWidth.Text = key.GetValueStr("ResizeWidth", "720");
                txtRounding.Text = key.GetValueStr("Rounding", "16");
                int Max = key.GetValueInt("maxthreads", 2);
                int Max1080p = key.GetValueInt("max1080pthreads", 1);
                int Max4K = key.GetValueInt("max4Kthreads", 1);
                int dx = this.GetIndexOf("cmbMaxThreads", Max.ToString());
                cmbH64Target.SelectedIndex = key.GetValueInt("h264Target",-1);
                string[] cmbname = { "cmbMaxThreads", "cmbMax1080pThreads", "cmb4KThreads" };
                foreach (string CmbName in cmbname)
                {
                    int cnt = 0;
                    if (CmbName.Contains("1080p")) Max = Max1080p;
                    else if (CmbName.Contains("4K")) Max = Max4K;
                    for (int i = 0; i < this.GetCount(CmbName); i++)
                    {
                        ComboBoxItem ssed = this.GetComboBoxItem(CmbName, i);
                        if (ssed.Content.ToString().ToInt() == Max)
                        {
                            this.SetSelectedIndex(CmbName, cnt);
                            break;
                        }
                        else cnt++;
                    }
                }
                key?.Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " 17 " + ex.Message);
            }
            finally
            {
                SettingsLoaded = true;
            }
        }
       
        public byte[] _EncryptPassword(byte[] _password)
        {
            int[] AccessKey = { 30, 11, 32, 57, 14, 2, 38, 49, 33, 44, 16, 28, 99, 00, 11, 31, 17, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 22, 44, 62, 32, 33, 22, 27, 41, 44, 36, 72, 23, 32, 33, 25, 16 };
            byte[] encvar = EMP.RC4(_password, EncKey);

            //byte[] encvar2 = EMP.RC4(encvar, EncKey);

            return encvar;
        }
        public string EncryptPassword(string password)
        {
            byte[] _Password = Encoding.ASCII.GetBytes(password);
            int[] AccessKey = { 30, 11, 32, 57, 14, 2, 38, 49, 33, 44, 16, 28, 99, 00, 11, 31, 17, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99, 00, 11, 32, 57, 74, 1, 8, 9, 33, 44, 66, 88, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 22, 44, 62, 32, 33, 22, 27, 41, 44, 36, 72, 23, 32, 33, 25, 16 };
            byte[] encvar = EMP.RC4(_Password, EncKey);
            byte[] encvar2 = EMP.RC4(encvar, EncKey);
            string bb = Encoding.ASCII.GetString(encvar2);
            if (bb != password)
            {

            }
            return Encoding.ASCII.GetString(encvar);
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnDoneSelectDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDone720p.Text = SelectMasterDir("Select 720p Destination Directory", "CompDirectory");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " BtnCompPath_Click " + ex.Message);
            }
        }
        string Root;
        public string SelectMasterDir(string SelectionText, string SettingsDirName)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey = (key != null);
                Root = key.GetValueStr("RootSelect", string.Empty);
                if (Root == "") Root = Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
                var dlg = new FolderBrowserDialog()
                {
                    Title = SelectionText,
                    InitialFolder = Root,
                };
                string InitDir = key.GetValueStr(SettingsDirName, string.Empty);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if ((dlg.SelectedFolder != InitDir) && (LoadedKey)) key.SetValue(SettingsDirName, dlg.SelectedFolder);
                    Root = Path.GetPathRoot(dlg.SelectedFolder);
                    if (LoadedKey) key.SetValue("RootSelect", Root);
                    key.Close();
                    return dlg.SelectedFolder;
                }
                else return string.Empty;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return string.Empty;
            }
        }
        private void btnErrorSelectDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtErrorPath.Text = SelectMasterDir("Select Error Directory", "ErrorDirectory");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }

        
        private void BtnCompleted_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtComp720p.Text = SelectMasterDir("Select 720p Completed Directory", "DestDirectory");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " BtnDestPath_Click " + ex.Message);
            }
        }

        private void grpDirectorySettings_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsLoaded && SettingsLoaded)
                {
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    string defaultdrive = Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
                    key.SetValue("MonitorDownloads", ChkMonitorDownloads.IsChecked);
                    key.SetValue("SourceDirectory720p", txtSrc720p.Text);
                    key.SetValue("SourceDirectory1080p", txtSrc1080p.Text );
                    key.SetValue("SourceDirectory4K", txtSrc4K.Text);
                    key.SetValue("SourceDirectory4KAdobe", txtSrc4KAdobe.Text);
                    key.SetValue("CompDirectory720p", txtComp720p.Text);
                    key.SetValue("CompDirectory1080p", txtComp1080p.Text);
                    key.SetValue("CompDirectory4K", txtComp4K.Text);
                    key.SetValue("CompDirectory4KAdobe", txtComp4KAdobe.Text);
                    key.SetValue("DestDirectory720p", txtDone720p.Text);
                    key.SetValue("DestDirectory1080p", txtDone1080p.Text);
                    key.SetValue("DestDirectoryAdobe4k", txtDone4k.Text);
                    key.SetValue("DestDirectory4KAdobe", txtDone4KAdobe.Text);
                    key?.SetValue("ErrorDirectory", txtErrorPath.Text);
                  
                    key?.SetValue("movecompleted", chkmovecompleted.IsChecked);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void grpMpg4_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsLoaded && SettingsLoaded)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                key?.SetValue("qscale", txtqScale.Text);
                key?.SetValue("qmin", txtMinQ.Text);
                key?.SetValue("qmax", txtMaxQ.Text);
                key?.SetValue("vtag", txtVtag.Text);
                key?.Close();
            }
        }

        private void ConfigSettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnShortsSelectDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtShortspath.Text = SelectMasterDir("Select Shorts Output Directory", "shortsdirectory");

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void grpEncoderSettings_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsLoaded && SettingsLoaded)
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                    key.SetValue("minbitrate", txtMin.Text);
                    key.SetValue("maxbitrate", txtMax.Text);
                    key.SetValue("buffer", txtBuffer.Text);
                    key.SetValue("ResizeWidth", txtWidth.Text);
                    key.SetValue("Rounding", txtRounding.Text);
                    key?.SetValue("resize", chkResize.IsChecked);
                    key?.SetValue("ARScaling", ChkARScaling.IsChecked);
                    key?.SetValue("Rounding", ChkRounding.IsChecked);
                    key?.SetValue("VSYNC", ChkVSYNC.IsChecked);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnCompleted720p_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtComp720p.Text = SelectMasterDir("Select Completed 720p Directory", "CompDirectory720p");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void BtnCompleted4k_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtComp4K.Text = SelectMasterDir("Select Completed 4K Directory", "CompDirectory4K");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void BtnCompleted4KAdobe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtComp4KAdobe.Text = SelectMasterDir("Select Completed 4K Adobe Directory", "CompDirectory4KAdobe");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnDoneSelectDir4K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDone4k.Text = SelectMasterDir("Select Output 4K Directory", "DestDirectory4k");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnDoneSelectDir4KAdobe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDone4k.Text = SelectMasterDir("Select Output 4K Adobe Directory", "DestDirectory4kAdobe");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnDoneSelectDir1080p_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDone4k.Text = SelectMasterDir("Select Output 1080p Directory", "DestDirectory10080p");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnSrc720p_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtSrc720p.Text = SelectMasterDir("Select Source 720p Directory", "SourceDirectory720p");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnSrc1080p_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtSrc1080p.Text = SelectMasterDir("Select Source 1080p Directory", "SourceDirectory1080p");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnSrc4K_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtSrc4K.Text = SelectMasterDir("Select Source 4K Directory", "SourceDirectory4K");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void btnSrc4KAdobe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtSrc4KAdobe.Text = SelectMasterDir("Select Source 4K Adobe Directory", "SourceDirectory4KAdobe");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void grpAppSettings_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsLoaded && SettingsLoaded)
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                    key.SetValue("shortsdirectory", txtShortspath.Text);
                    key?.SetValue("maxthreads", this.GetCmbContentToInt("cmbMaxThreads"));
                    key?.SetValue("max1080pthreads", this.GetCmbContentToInt("cmbMax1080pThreads"));
                    key?.SetValue("max4Kthreads", this.GetCmbContentToInt("cmb4KThreads"));
                    key?.SetValue("Audiomode", this.GetCmbContentToInt("cmbAudioMode"));
                    key.SetValue("reencodefile", ChkReEncode.IsChecked);
                    key.SetValue("DropFormat", ChkDropFormat.IsChecked);
                    key.SetValue("AudioConversionAC3", ChkAudioConversion.IsChecked);
                    key.SetValue("AutoAAC", ChkAutoAAC.IsChecked);
                    key.SetValue("h264Target", cmbH64Target.SelectedIndex);
                    key.SetValue("ChangeFileName", ChkChangeOutputname.IsChecked);
                    key.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
