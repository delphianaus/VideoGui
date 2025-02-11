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
    /// Interaction logic for SchedulingSelectEditor.xaml
    /// </summary>
    public partial class SchedulingSelectEditor : Window
    {
        OnFinish DoOnFinish = null;
        SelectReleaseSchedule SRS = null;
        databasehook<object> dbInitialzer = null;
        public bool IsClosing = false, IsClosed = false;
        public int TitleId = -1;
        public SchedulingSelectEditor(OnFinish _OnFinish, databasehook<object> _dbInitialzer)
        {
            try
            {
                InitializeComponent();
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; _OnFinish?.Invoke(); };
                DoOnFinish = _OnFinish;
                dbInitialzer = _dbInitialzer;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SchedulingSelectEditor Constuctor {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    if (e.WidthChanged)
                    {
                        brdShortsVideoCa1t.Width = e.NewSize.Width - 76;
                        brdShortsVideoCat.Width = e.NewSize.Width - 54;
                        btnSelect.Margin = new Thickness(e.NewSize.Width - 50, 0, 0, 0);
                        brdActions.Width = e.NewSize.Width - 163;
                    }
                    if (e.HeightChanged)
                    {
                        brdShortsVideoCat.Height = e.NewSize.Height - 142;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
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

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnclose_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuNewItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtTitle.Text = "";
                ReleaseDate.Value = null;
                ReleaseEndDate.Value = null;
                txtGap.Text = "";
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
                if (lstTitles.SelectedItem is ScheduleMapItem smi && smi.Id != -1)
                {
                    TitleId = smi.Id;
                    DateTime n = DateTime.Now;
                    ReleaseDate.Value = n.Date + smi.Start;
                    ReleaseEndDate.Value = n.Date + smi.End;
                    txtGap.Text = smi.Gap.ToString();
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
                if (lstTitles.SelectedItem is ScheduleMapItem smi && smi.Id != -1)
                {
                    dbInitialzer?.Invoke(this, new CustomParams_RemoveTimeSpans(smi.Id));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuDeleteItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void DoShow()
        {
            try
            {
                Show();
                dbInitialzer(this, new CustomParams_Refresh());
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoShow {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SRS is not null)
                {
                    SRS.Close();
                    SRS = null;
                }

                SRS = new SelectReleaseSchedule(() => { DoShow(); }, dbInitialzer);
                SRS.Show();
                if (SRS is not null)
                {
                    Hide();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSelect_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TitleId != -1 && ReleaseDate.Value.HasValue &&
                    ReleaseEndDate.Value.HasValue && txtGap.Text.ToInt(-1) != -1)
                {
                    dbInitialzer?.Invoke(this,
                        new CustomParams_AddTimeSpanEntries(ReleaseDate.Value.Value.TimeOfDay,
                        ReleaseEndDate.Value.Value.TimeOfDay, txtGap.Text.ToInt()));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSave_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstTitles.SelectedItem is ScheduleMapItem smi)
                {
                    if (smi.Id != -1 && TitleId != -1 && ReleaseDate.Value.HasValue &&
                     ReleaseEndDate.Value.HasValue && txtGap.Text.ToInt(-1) != -1)
                    {
                        dbInitialzer?.Invoke(this,
                         new CustomParams_EditTimeSpans(smi.Id, ReleaseDate.Value.Value.TimeOfDay,
                         ReleaseEndDate.Value.Value.TimeOfDay,
                         txtGap.Text.ToInt()));
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnUpdate_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void lstTitles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lstTitles.SelectedItem is ScheduleMapItem smi && smi.Id != -1)
                {
                    TitleId = smi.Id;
                    DateTime n = DateTime.Now;
                    ReleaseDate.Value = n.Date + smi.Start;
                    ReleaseEndDate.Value = n.Date + smi.End;
                    txtGap.Text = smi.Gap.ToString();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"lstTitles_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuAddEndItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstTitles.SelectedItem is ScheduleMapItem smi && smi.Id != -1)
                {
                    DateTime n = DateTime.Now;
                    ReleaseDate.Value = n.Date + smi.End;
                    ReleaseEndDate.Value = n.Date + smi.End.Add(TimeSpan.FromHours(1));
                    txtGap.Text = smi.Gap.ToString();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuAddEndItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuDeleteAllItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dbInitialzer?.Invoke(this,
                         new CustomParams_RemoveTimeSpans(TitleId, true));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuDeleteAllItems_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
    }
}
