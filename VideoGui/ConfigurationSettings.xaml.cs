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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace VideoGui
{
    [SupportedOSPlatform("windows")]
    /// <summary>
    /// Interaction logic for ConfigurationSettings.xaml
    /// </summary>
    public partial class ConfigurationSettings : Window
    {
        [SupportedOSPlatform("windows")]
        public ConfigurationSettings()
        {
            InitializeComponent();
            LoadSettings();
        }
        [SupportedOSPlatform("windows")]
        public void LoadSettings()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                ChkAutoUpdate.IsChecked = key.GetValueBool("AutoUpdate", true);
                ChkEncryptData.IsChecked = key.GetValueBool("EncryptData", true);
                ChkShowUserPassword.IsChecked = key.GetValueBool("ShowUserPassword", true);
                ChkUtorrentIntegration.IsChecked = false;// key.GetValue("UseUWebApi", true).ToString() == "True");
                txtWebApiUsername.Text = "";// (string)(LoadedKey ? key.GetValue("WebAPIUsername",string.Empty) : string.Empty);
                txtVtag.Text = key.GetValueStr("vtag");
                txtqScale.Text = key.GetValueStr("qscale");//, string.Empty) : string.Empty);
                txtMinQ.Text = key.GetValueStr("qmin");//, string.Empty) : string.Empty);
                txtMaxQ.Text = key.GetValueStr("qmax");// : string.Empty);

                //byte[] password = (byte[])  key.GetValue("WebAPIUserpassword");// : string.Empty);
                //txtWebApiIP.Text = (string)(LoadedKey ? key.GetValue("WebApiIP", "localhost") : "localhost"
                //if (password != null)   
                // {
                //    byte[] passwordx = ChkEncryptData.IsChecked.Value ? _EncryptPassword(password) : password;
                //    string pw = Encoding.ASCII.GetString(passwordx);// : password;
                //    if (pw.Length > 0) txtWebApiPassword.Password = pw;
                //    txtpasswordshow.Text = txtWebApiPassword.Password;
                //}
                //else 
                //    txtWebApiPassword.Password = string.Empty;
                //TxtWebApiPort.Text = (string)(LoadedKey ? key.GetValue("WebAPIUserport", "8080") : "8080");
                //txtWebApiPassword.Width = txtpasswordshow.Width;
                //if (ChkShowUserPassword.IsChecked.Value)
                //{
                //    txtpasswordshow.Text = txtWebApiPassword.Password;
                //    txtWebApiPassword.Visibility = Visibility.Collapsed;
                //    txtpasswordshow.Visibility = Visibility.Visible;
                // }
                //else
                // {
                //    txtWebApiPassword.Password = txtpasswordshow.Text;
                //    txtWebApiPassword.Visibility = Visibility.Visible;
                //   txtpasswordshow.Visibility = Visibility.Collapsed;
                // }
                key.Close();
            }
            catch (Exception ex)
            {
                LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " 17 " + ex.Message);
            }
        }

        private void LogWrite(string v)
        {
            try
            {
                string m_exePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                using var w = File.AppendText(m_exePath + "\\" + "log.txt");
                Log(v, w);
            }
            catch (Exception ex)
            {
                Debug.Print(MethodBase.GetCurrentMethod().Name.ToString() + " LogWrite" + ex.Message);
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\n","Log Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("  :");
                txtWriter.WriteLine("  :{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
                Debug.Print(MethodBase.GetCurrentMethod().Name.ToString() + " LogWrite" + ex.Message);
            }
        }

        [SupportedOSPlatform("windows")]
        public void SaveSettings()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                key?.SetValue("AutoUpdate", ChkAutoUpdate.IsChecked.Value);
                key?.SetValue("EncryptData", ChkEncryptData.IsChecked.Value);
                key?.SetValue("ShowUserPassword", ChkShowUserPassword.IsChecked.Value);
                key?.SetValue("UseUWebApi", ChkUtorrentIntegration.IsChecked.Value);
                key?.SetValue("WebAPIUsername", txtWebApiUsername.Text);
                key?.SetValue("WebAPIUserpassword", ChkEncryptData.IsChecked.Value ? _EncryptPassword(Encoding.ASCII.GetBytes(txtWebApiPassword.Password)) : Encoding.ASCII.GetBytes(txtWebApiPassword.Password));
                key?.SetValue("WebAPIUserport", TxtWebApiPort.Text);
                key?.SetValue("WebApiIP", txtWebApiIP.Text);
                key?.SetValue("qscale", txtqScale.Text);
                key?.SetValue("qmin", txtMinQ.Text);
                key?.SetValue("qmax", txtMaxQ.Text);
                key?.SetValue("vtag", txtVtag.Text);
                key?.Close();
            }
            catch (Exception ex)
            {
                Debug.Print(MethodBase.GetCurrentMethod().Name.ToString() + " SaveSettings" + ex.Message);
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


        private void ChkUtorrentIntegration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool uTorrent = ChkUtorrentIntegration.IsChecked.Value;
                txtWebApiUsername.IsEnabled = uTorrent;
                txtWebApiPassword.IsEnabled = uTorrent;
                TxtWebApiPort.IsEnabled = uTorrent;
                txtWebApiIP.IsEnabled = uTorrent;
            }
            catch (Exception ex)
            {
                LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " ChkUtorrentIntegration_Click" + ex.Message);
            }
        }

        private void ChkShowUserPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool ShowPassword = ChkShowUserPassword.IsChecked.Value;
                if (ChkShowUserPassword.IsChecked.Value)
                {
                    txtpasswordshow.Text = txtWebApiPassword.Password;
                    txtWebApiPassword.Visibility = Visibility.Collapsed;
                    txtpasswordshow.Visibility = Visibility.Visible;
                }
                else
                {
                    txtWebApiPassword.Password = txtpasswordshow.Text;
                    txtWebApiPassword.Visibility = Visibility.Visible;
                    txtpasswordshow.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " ChkShowUserPassword_Click " + ex.Message);
            }
        }

        [SupportedOSPlatform("windows")]
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveSettings();
                Close();
            }
            catch (Exception ex)
            {
                LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " SaveSettings " + ex.Message);
            }
        }

        private void ChkShowUserPassword_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void TxtWebApiPort_Copy2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
