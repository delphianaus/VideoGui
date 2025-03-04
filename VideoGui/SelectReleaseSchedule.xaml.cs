﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
        public bool IsApplied = false,  IsClosing = false, IsClosed = false;
        databasehook<object> dbInitialzer = null;
        public string SelectedItem = "";
        public int SelectedId = -1;
        public SchedulingSelectEditor schedulingSelectEditor = null;


        public SelectReleaseSchedule(OnFinish _OnFinish, databasehook<object> _dbInitialzer, bool Is_Applied = false)
        {
            try
            {
                InitializeComponent();
                IsApplied = Is_Applied;
                DoOnFinish = _OnFinish;
                dbInitialzer = _dbInitialzer;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; _OnFinish?.Invoke(); };
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

        

        
        private void mnuEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstMainSchedules.SelectedItem is ScheduleMapNames SMN)
                {
                    if (SMN != null && SMN.Id != -1)
                    {
                        if (schedulingSelectEditor is not null)
                        {
                            if (schedulingSelectEditor.IsClosing) schedulingSelectEditor.Close();
                            while (!schedulingSelectEditor.IsClosed)
                            {
                                Thread.Sleep(100);
                                System.Windows.Forms.Application.DoEvents();
                            }
                            schedulingSelectEditor.Close();
                            schedulingSelectEditor = null;
                        }

                        schedulingSelectEditor = new SchedulingSelectEditor(SchedulingEditorOnFinish, dbInitialzer);
                        schedulingSelectEditor.ShowActivated = true;
                        schedulingSelectEditor.TitleId = SMN.Id;
                        Hide();
                        schedulingSelectEditor.Show();

                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuEdit_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void SchedulingEditorOnFinish()
        {
            try
            {
                Show();
                Task.Run(() =>
                {
                    if (schedulingSelectEditor is not null && !schedulingSelectEditor.IsClosed)
                    {
                        while (schedulingSelectEditor.IsClosing)
                        {
                            Thread.Sleep(100);
                        }
                        schedulingSelectEditor = null;
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + $" {ex.Message}");

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
                dbInitialzer?.Invoke(this, new CustomParams_Finish(txtScheduleName.Text));
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
                dbInitialzer?.Invoke(this, new CustomParams_Initialize());
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

        private void txtScheduleName_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                dbInitialzer?.Invoke(this, new CustomParams_Finish(txtScheduleName.Text));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"txtScheduleName_LostFocus {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
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
                    SelectedItem = SMN.Name;
                    SelectedId = SMN.Id;
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
