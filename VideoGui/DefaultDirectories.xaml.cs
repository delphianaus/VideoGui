using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;
using VideoGui.Models.delegates;
using FolderBrowserEx;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for DefaultDirectories.xaml
    /// </summary>
    public partial class DefaultDirectories : Window
    {
        string Root;
        public Models.delegates.CompairFinished OnFinish;
        public DefaultDirectories(Models.delegates.CompairFinished _OnFinish)
        {
            OnFinish = _OnFinish;
            InitializeComponent();
            LoadSettings();
        }

        public void LoadSettings()
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string defaultdrive = Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);

                chkmovecompleted.IsChecked = key.GetValueBool("movecompleted", true);
                txtDonepath.Text = key.GetValueStr("DestDirectory", defaultdrive);
                txtDestPath.Text = key.GetValueStr("CompDirectory", defaultdrive);
                txtErrorPath.Text = key.GetValueStr("ErrorDirectory", defaultdrive);
                int Max = key.GetValueInt("maxthreads", 2);
                int Max1080p = key.GetValueInt("max1080pthreads", 1);
                int dx = this.GetIndexOf("cmbMaxThreads",Max.ToString());
                string[] cmbname = { "cmbMaxThreads", "cmbMax1080pThreads" };
                foreach (string CmbName in cmbname)
                {
                    int cnt = 0;
                    if (CmbName.Contains("1080p")) Max = Max1080p;
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
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
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
        private void frmDefDirectories_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string defaultdrive = Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
                key?.SetValue("DestDirectory", txtDonepath.Text);
                key?.SetValue("CompDirectory", txtDestPath.Text);
                key?.SetValue("ErrorDirectory", txtErrorPath.Text);
                key?.SetValue("maxthreads", this.GetCmbContentToInt("cmbMaxThreads"));
                key?.SetValue("max1080pthreads", this.GetCmbContentToInt("cmbMax1080pThreads"));
                key?.SetValue("movecompleted", chkmovecompleted.IsChecked);
                key?.Close();
                OnFinish?.Invoke();

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void BtnErrorSelectDir_Click(object sender, RoutedEventArgs e)
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

        private void btnDoneSelectDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDonepath.Text = SelectMasterDir("Select Completed Directory", "CompDirectory");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " BtnCompPath_Click " + ex.Message);
            }
        }

        private void BtnCompletedSelectDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDestPath.Text = SelectMasterDir("Select Destination Directory", "DestDirectory");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " BtnDestPath_Click " + ex.Message);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " CloseBtn_click " + ex.Message);
            }

        }
    }


}

