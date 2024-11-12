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


        


         */

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

        private void btnCloe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstSchedules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void lstSchedules_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {

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
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

    }
}
