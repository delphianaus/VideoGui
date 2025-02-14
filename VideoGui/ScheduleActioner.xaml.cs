using System;
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
using static VideoGui.TitleSelectFrm;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ScheduleActioner.xaml
    /// </summary>
    public partial class ScheduleActioner : Window
    {
        databasehook<Object> ModuleCallBack = null;
        public bool IsClosed = false, IsClosing = false;
        ActionScheduleSelector frmActionScheduleSelector = null;
        SchedulingSelectEditor frmSchedulingSelectEditor = null;
        Nullable<DateTime> actionDate = null, scheduleDate = null, completeDate = null;
        public int actionScheduleID =-1;
        public ScheduleActioner(OnFinish DoOnFinish, databasehook<Object> _ModuleCallBack)
        {
            try
            {
                InitializeComponent();
                ModuleCallBack = _ModuleCallBack;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; DoOnFinish?.Invoke(); };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduleActioner - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (actionScheduleID == -1)
                {
                    ModuleCallBack?.Invoke(this, new CustomParams_Initialize());
                }
                else
                {
                    ModuleCallBack?.Invoke(this, new CustomParams_Initialize(actionScheduleID));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (frmSchedulingSelectEditor is not null)
                {
                    if (!frmSchedulingSelectEditor.IsClosed)
                    {
                        frmSchedulingSelectEditor.Close();
                        while (!frmSchedulingSelectEditor.IsClosed)
                        {
                            Thread.Sleep(100);
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                    frmSchedulingSelectEditor = null;
                }
                frmSchedulingSelectEditor = new SchedulingSelectEditor(SchedulingSelectEditor_OnFinish, ModuleCallBack);
                Hide();
                frmSchedulingSelectEditor.ShowActivated = true;
                frmSchedulingSelectEditor.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSelect_Click - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void SchedulingSelectEditor_OnFinish()
        {
            try
            {
                Show();
                int LastId = -1;
                if (frmSchedulingSelectEditor is not null)
                {

                    LastId = frmSchedulingSelectEditor.TitleId;
                    txtSchName.Text = (LastId != -1) ? frmSchedulingSelectEditor.Title : txtSchName.Text;
                    if (!frmSchedulingSelectEditor.IsClosing)
                    {
                        while (!frmSchedulingSelectEditor.IsClosed)
                        {
                            Thread.Sleep(100);
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                    if (frmSchedulingSelectEditor.IsClosed)
                    {
                        frmSchedulingSelectEditor = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SchedulingSelectEditor_OnFinish - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void BtnSelectAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (frmActionScheduleSelector is not null)
                {
                    if (!frmActionScheduleSelector.IsClosed)
                    {
                        frmActionScheduleSelector.Close();
                        while (!frmActionScheduleSelector.IsClosed)
                        {
                            Thread.Sleep(100);
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                    frmActionScheduleSelector = null;
                }
                frmActionScheduleSelector = new ActionScheduleSelector(ActionScheduleSelector_OnFinish,ModuleCallBack);
                Hide();
                frmActionScheduleSelector.ShowActivated = true;
                frmActionScheduleSelector.Show();

            }
            catch (Exception ex)
            {
                ex.LogWrite($"BtnSelectAction_Click - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void ActionScheduleSelector_OnFinish()
        {
            try
            {
                Show();
                if (frmActionScheduleSelector is not null)
                {
                    txtActionName.Text = (frmActionScheduleSelector.PersistId != -1) ? frmActionScheduleSelector.Title : txtActionName.Text;
                    if (!frmActionScheduleSelector.IsClosing)
                    {
                        while (!frmActionScheduleSelector.IsClosed)
                        {
                            Thread.Sleep(100);
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                    if (frmActionScheduleSelector.IsClosed)
                    {
                        frmActionScheduleSelector = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ActionScheduleSelector_OnFinish - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void BtnSaveAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ModuleCallBack?.Invoke(this, new 
                    CustomParams_UpdateAction(actionScheduleID,actionDate,scheduleDate,completeDate, 
                    txtSchName.Text, txtActionName.Text, txtMaxSchedules.Text.ToInt(0)));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"BtnSaveAction_Click - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
