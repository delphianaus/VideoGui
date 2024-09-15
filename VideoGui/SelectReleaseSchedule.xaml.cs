using System;
using System.Collections.Generic;
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
using VideoGui.Models;
using VideoGui.Models.delegates;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for SelectReleaseSchedule.xaml
    /// </summary>
    public partial class SelectReleaseSchedule : Window
    {
        OnFinish DoOnFinish = null;
        databasehook<object> dbInitialzer = null;
        public SelectReleaseSchedule(OnFinish _OnFinish, databasehook<object> _dbInitialzer)
        {
            try
            {
                InitializeComponent();
                DoOnFinish = _OnFinish;
                dbInitialzer = _dbInitialzer;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SelectReleaseSchedule Constructor {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtScheduleName.Text = "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuNew_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public class CustomParams_Add
        {
            public string Name { get; set; } = "";
            public dataUpdatType dataUpdatType { get; set; } = dataUpdatType.Add;
            public string data_string { get; set; } = "";
            public CustomParams_Add(string _Name, string datastring)
            {
                Name = _Name;
                data_string = datastring;
            }
        }

        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dbInitialzer?.Invoke(this, new CustomParams_Add(txtScheduleName.Text, ""));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuNew_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstMainSchedules.SelectedItem is ScheduleMapNames SMN)
                {
                    txtScheduleName.Text = SMN.Name;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuEdit_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstMainSchedules.SelectedItem is ScheduleMapNames SMN)
                {
                    dbInitialzer?.Invoke(this, new CustomParams_Delete(SMN.Id, ""));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuDelete_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DoOnFinish?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Closing {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dbInitialzer?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int Id = -1;
                if (lstMainSchedules.SelectedItem is ScheduleMapNames SMN)
                {
                    Id = SMN.Id;
                }
                dbInitialzer?.Invoke(this, new CustomParams_Save(txtScheduleName.Text, Id));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSave_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void lstMainSchedules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lstMainSchedules.SelectedItem is ScheduleMapNames SMN)
                {
                    txtScheduleName.Text = SMN.Name;

                    dbInitialzer?.Invoke(this, new CustomParams_Select(SMN.Id));
                    Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"lstMainSchedules_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
    }
}
