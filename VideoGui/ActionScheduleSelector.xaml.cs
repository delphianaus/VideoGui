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

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ActionScheduleSelector.xaml
    /// </summary>
    public partial class ActionScheduleSelector : Window
    {
        databasehook<Object> Invoker = null;
        public bool IsClosed = false, IsClosing = false;
        public string Title = "";
        public int TitleId = -1;
        public ActionScheduleSelector(OnFinishIdObj DoOnFinish, databasehook<Object> _Invoker)
        {
            try
            {
                InitializeComponent();
                Invoker = _Invoker;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; DoOnFinish?.Invoke(this, -1); };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ActionScheduleSelector - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Invoker?.Invoke(this, new CustomParams_Initialize());
            Width++;
            Height++;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mnuSelectItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstItems.SelectedItems.Count > 0)
                {
                    if (lstItems.SelectedItems[0] is ScheduledActions item)
                    {
                        TitleId = item.Id;
                        Close();
                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuSelectItem_Click Constuctor {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuNewItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewActioner();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuNewItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void NewActioner(int id = -1, bool IsCopy = false)
        {
            try
            {

                var _frmScheduleActioner = new ScheduleActioner(scheduleActioner_onfinish, Invoker);
                Hide();
                _frmScheduleActioner.ShowActivated = true;
                _frmScheduleActioner.IsCopy = IsCopy;
                if (id != -1) _frmScheduleActioner.actionScheduleID = id;
                _frmScheduleActioner.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"NewActioner {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void scheduleActioner_onfinish(object sender, int id)
        {
            try
            {
                Show();

                if (sender is ScheduleActioner frm)
                {
                    frm = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuNewItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuEditItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in lstItems.SelectedItems)
                {
                    if (item is ScheduledActions sitem)
                    {
                        NewActioner(sitem.Id);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuEditItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in lstItems.SelectedItems)
                {
                    if (item is ScheduledActions sitem)
                    {
                        Invoker?.Invoke(this, new CustomParams_Delete(sitem.Id, ""));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuDeleteItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewActioner();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuDeleteItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }



        private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                mnuEditItem_Click(sender, e);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"lstItems_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuAddItemAsCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in lstItems.SelectedItems)
                {
                    if (item is ScheduledActions sitem)
                    {
                        NewActioner(sitem.Id, true);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuAddItemAsCopy_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    if (e.WidthChanged)
                    {
                        lstHeader.Width = e.NewSize.Width - 33;// 800-783 = 17
                        lstItems.Width = e.NewSize.Width - 33;
                    }
                    if (e.HeightChanged)
                    {
                        lstItems.Height = e.NewSize.Height - 144;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged - {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

    }
}
