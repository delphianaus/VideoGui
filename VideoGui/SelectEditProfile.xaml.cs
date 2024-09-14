using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for SelectEditProfile.xaml
    /// </summary>
    public partial class SelectEditProfile : Window
    {
        List<string> Settings = new();
        List<string> SettingsNames = new();
        string oldname = "";
        public SelectEditProfile()
        {
            InitializeComponent();
            LoadDefaultSettings();
            RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
            string SetttingID = "";
            if (key2.RegistryValueExists("SelectedProfile"))
            {
                //RegistryKey key = key2.OpenSubKey("SelectedProfile");
                SetttingID = key2?.GetValueStr("SelectedProfile");
                key2?.Close();
            }
            if (SetttingID != "")
            {
                LoadSettings(SetttingID);
                int idx = this.GetIndexOf("cmbProfile", SetttingID);
                this.SetSelectedIndex("cmbProfile", idx);
            }
            key2?.Close();
        }

        public string GetDefaultSettings()
        {
            try
            {
                string result = "";
                result = $"{result}default|";
                result = $"{result}675K|";
                result = $"{result}1150K|";
                result = $"{result}1200K|";
                result = $"{result}720|";
                result = $"{result}|";
                result = $"{result}16|";
                result = $"{result}true|";
                result = $"{result}true|";
                result = $"{result}true|";
                result = $"{result}true|";
                return result;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
                return "";
            }
        }
        public void LoadDefaultSettings()
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                bool LoadedKey2 = (key != null);
                string profile;
                if (!key.RegistryValueExists("BitRateSettings"))
                {
                    string settings = GetDefaultSettings();
                    Settings.Add(settings);
                    profile = settings.Split("|").ToList().First();
                    SettingsNames.Add(profile);
                    key.SetValue("BitRateSettings", Settings.ToArray(), RegistryValueKind.MultiString);
                    key.SetValue("SelectedProfile", profile);
                }
                else
                {
                    Settings.AddRange(key.GetValueStrs("BitRateSettings"));
                    profile = key.GetValueStr("SelectedProfile");
                    if (profile == "")
                    {
                        if (Settings.Count > 0)
                        {
                            string profs = Settings.First();
                            if (profs != "")
                            {
                                profile = profs.Split("|").ToList().First();
                            }
                        }
                    }
                    foreach (string settingcomp in Settings)
                    {
                        string setps = settingcomp.Split("|").ToList().First();
                        SettingsNames.Add(setps);
                    }
                }
                foreach (string ss in SettingsNames)
                {
                    this.AddItems("cmbProfile", ss);
                }
                if (profile != "")
                {
                    int index = this.GetIndexOf("cmbProfile", profile);
                    if (index != -1) this.SetSelectedIndex("cmbProfile", index);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }
        }

        private void cmbProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SaveSettings();
                // Load New Settings After Save Current.
                LoadSettings(cmbProfile.Text.Trim());
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }

        }

        private void SaveSettings(string name = "")
        {
            try
            {
                bool found = false;
                string nnma = this.GetText("cmbProfile");
                nnma = (name == "") ? this.GetText("cmbProfile") : name;
                foreach (var _ in from string settingcomp in Settings
                                  let setps = settingcomp.Split("|").ToList().First()
                                  where setps == nnma
                                  select new { })
                {
                    found = true;
                }
                if (!found)
                {
                    string Height = !this.IsChecked("ChkArScaling") ? this.GetText("txtHeight") : "0.0";
                    Settings.Add(GetResultStr(nnma, Height.ToDouble()));
                    SettingsNames.Add(this.GetText("cmbProfile"));
                }
                if (found)
                {
                    for (int i = 0; i < Settings.Count; i++)
                    {
                        string result = Settings[i];
                        if (result.Split("|").ToArray().First() == nnma)
                        {
                            Settings[i] = GetResultStr(nnma, Height);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }
        }

        public string GetResultStr(string _nnma, double _height)
        {
            string result = "";
            result = $"{result}{_nnma}|";
            result = $"{result}{this.GetText("txtMin")}|";
            result = $"{result}{this.GetText("txtMax")}|";
            result = $"{result}{this.GetText("TxtBufferSize")}|";
            result = $"{result}{this.GetText("txtWidth")}|";
            result = (_height > 0.0) ? $"{result}{_height}|" : $"{result}|";
            result = $"{result}{this.GetText("txtARRounding")}| ";
            result = $"{result}{this.IsChecked("ChkEnabled")}|";
            result = $"{result}{this.IsChecked("ChkArRounding")}|";
            result = $"{result}{this.IsChecked("ChkArScaling")}|";
            result = $"{result}{this.IsChecked("ChkVSYNC")}|";

            return result;
        }
        private void cmbProfile_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    SaveSettings();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }
        }

        private void ChkArRounding_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtARRounding.IsEnabled = (bool)ChkArRounding.IsChecked;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }
        }

        private void ChkArScaling_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtHeight.IsEnabled = (bool)!ChkArScaling.IsChecked;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }
        }

        private void ChkEnabled_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtWidth.IsEnabled = (bool)ChkEnabled.IsChecked;
                ChkArScaling.IsEnabled = (bool)ChkEnabled.IsChecked;
                ChkArRounding.IsEnabled = (bool)ChkEnabled.IsChecked;
                if (ChkEnabled.IsEnabled)
                {
                    txtHeight.IsEnabled = !ChkArScaling.IsEnabled;
                }
                else
                {
                    txtHeight.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }
        }

        private void LoadSettings(string SetttingID)
        {
            try
            {
                foreach (var sdd in Settings)
                {
                    string setps = sdd.Split("|").ToList().First();
                    if (setps == SetttingID)
                    {
                        List<string> settingslist = sdd.Split("|").ToList();
                        if (settingslist.Count > 11)
                        {
                            this.SetText("txtMin", settingslist[1]);
                            this.SetText("txtMax", settingslist[2]);
                            this.SetText("TxtBufferSize", settingslist[3]);
                            this.SetText("txtWidth", settingslist[4]);
                            this.SetText("txtHeight", settingslist[5]);
                            this.SetText("txtARRounding", settingslist[6]);
                            this.SetChecked("ChkEnabled", settingslist[7].ToBool());
                            this.SetChecked("ChkArRounding", settingslist[8].ToBool());
                            this.SetChecked("ChkArScaling", settingslist[9].ToBool());
                            this.SetChecked("ChkVSYNC", settingslist[10].ToBool());
                        }

                    }
                }
                RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                key2?.SetValue("SelectedProfile", SetttingID);
                key2?.Close();

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveSettings(oldname);
                LoadSettings(this.GetText("cmbProfile"));

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }
        }

        private void cmbProfile_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                oldname = this.GetText("cmbProfile");//.Text.Trim();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }
        }

        private void frmProfileSelectEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (Settings.Count > 0)
                {
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key?.SetValue("BitRateSettings", Settings.ToArray(), RegistryValueKind.MultiString);
                    string cmbtext = this.GetText("cmbProfile");
                    if (cmbtext != "")
                    {
                        key?.SetValue("SelectedProfile", cmbtext);
                    }
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString() + " " + ex.Message);
            }

        }
    }
}
