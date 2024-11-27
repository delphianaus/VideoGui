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
    /// Interaction logic for ScheduleEventCreator.xaml
    /// </summary>
    public partial class ScheduleEventCreator : Window
    {
        OnFinish DoOnFinish;
        public bool IsClosed = false, IsClosing = false;
        databasehook<Object> ModuleCallBack = null;
        public ScheduleEventCreator(OnFinish _DoOnFinish, databasehook<Object> _DoDbHook)
        {
            try
            {
                InitializeComponent();
                DoOnFinish = _DoOnFinish;
                ModuleCallBack = _DoDbHook;
                EnableDisableDataEntry(false);

                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; _DoOnFinish?.Invoke(); };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduleEventCreator {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        /*
          string sql = "SELECCT APS.STARTHOUR,APS.ENDHOUR, APS.GAP, APS.DAYS , "+
                       "ES.SCHEDULEDATE FROM EVENTSCHEDULES ES JOIN SCHEDULES SP ON ES.EVENTID = SP.ID " +
                    $" JOIN APPLIEDSCHEDULE APS ON SP.ID = APS.SCHEDULEID WHERE EVENTID = {eventdef.Id}"+
                     " AND SP.ISSCHEDULE = 1;";
          
        /// AppliedSchedule (ID)  ** select schedules show names listbox & selector
        /// Schedule date - select start & end (dtp)
        /// 


        string sql = "SELECT SP.NAME,ESD.START,ESD.END,ESD.STARTTIME,ESD.ENDTIME,SD.START,SD.END,SD.STARTTIME,SD.ENDTIME FROM "+
              "EVENTSCHEDULES ES  "+
              "INNER JOIN EVENTSCHEDULEDATE ESD ON ESD.EVENTID = ES.EVENTID "+
              "INNER JOIN SCHEDULEDATE SD ON SD.EVENTID = ES.EVENTID "
              "INNER JOIN SCHEDULES SP ON SP.ID = ES.SCHEDULEID "
              "INNER JOIN SCHEDULEDPOOL SP ON SP.ID = ES.SCHEDULEID " +
              $"WHERE ES.EVENTID = {EventId} AND SP.ISSCHEDULE = 1;"

             // source , max daily , maxevent

         */

        public void EnableDisableDataEntry(bool Enable)
        {
            try
            {
                grp1.IsEnabled = Enable;
                grp2.IsEnabled = Enable;
                btnAddEvent.IsEnabled = Enable;
                btnAddSchedule.IsEnabled = Enable;
                BtnRemoveEvent.IsEnabled = Enable;
                BtnRemoveSchedule.IsEnabled = Enable;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"EnableDisableDataEntry {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
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

        
        
        private void lstSchedules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void lstSchedules_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void txtMax_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnClose_Click {MethodBase.GetCurrentMethod()?.Name} {this} {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string rootfolder = key.GetValueStr("DestinationDir", "c:\\");
                key?.Close();
                //txtdestdir.Text = rootfolder;
                //DoSetLists?.Invoke(0);// Set to true for Current
                ModuleCallBack?.Invoke(this, new CustomParams_Initialize());
                // callback Module Init - Get Lists for both listbox sources
                // Get Current Evenet Start and EndDate and populate and enable 
                // other query data 
                /*
                 EVENTSCHEDULES
                 * 
                 * */
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

    }
}
