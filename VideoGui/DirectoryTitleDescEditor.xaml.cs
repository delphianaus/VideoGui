using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for DirectoryTitleDescEditor.xaml
    /// </summary>
    public partial class DirectoryTitleDescEditor : Window
    {
        databasehook<object> dbInit = null;
        public bool IsClosing = false, IsClosed = false;
        databasehook<object> ModuleCallBack = null;
        public TitleSelectFrm DoTitleSelectFrm = null;
        public DescSelectFrm DoDescSelectFrm = null;

        int ShortsIndex = 0;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                ModuleCallBack?.Invoke(this, new CustomParams_Initialize());
                brd1.Width = Width - 16;
                brd1.Height = Height - 123;
                lstItems.Width = brd1.Width;
                lstSchedules.Width = lstItems.Width - 8;
                lstSchedules.Height = Height - 160;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // This for is marked for mcu listbox upgrade
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnClose_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
        
        private void TitleToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((sender is ToggleButton cbf) && (cbf.DataContext is ShortsDirectory ReleaseInfo))
                {
                    cbf.IsChecked = (ReleaseInfo.IsTitleAvailable) ? ReleaseInfo.IsTitleAvailable : cbf.IsChecked;
                    ModuleCallBack?.Invoke(this, new CustomParams_Select(ReleaseInfo.Id));
                    ModuleCallBack?.Invoke(this, new CustomParams_TitleSelect(ReleaseInfo));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"TitleToggle_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public void DoTitleSelectCreate(int TitleId = -1, int LinkedId = -1)
        {
            try
            {
                var _DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, 
                    ModuleCallBack, true, TitleId, LinkedId);
                Hide();
                _DoTitleSelectFrm.ShowActivated = true;
                _DoTitleSelectFrm.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoTitleSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public void DoDescSelectCreate(int DescId = -1)
        {
            try
            {
                if (DoDescSelectFrm is not null && DoDescSelectFrm.IsActive)
                {
                    DoDescSelectFrm.Close();
                    DoDescSelectFrm = null;
                }

                DoDescSelectFrm = new DescSelectFrm(OnSelectFormClose, ModuleCallBack, true, DescId);
                Hide();
                DoDescSelectFrm.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoDescSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void OnSelectFormClose(object sender, int e)
        {
            try
            {
                if (sender is DescSelectFrm frm)
                {
                    ModuleCallBack?.Invoke(this, 
                        new CustomParams_Update(frm.TitleTagId, UpdateType.Description));
                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} OnSelectFormClose {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void DoOnFinishTitleSelect(object sender, int id)
        {
            try
            {
                if (sender is TitleSelectFrm frm)
                {
                    ModuleCallBack?.Invoke(this, new CustomParams_Update(frm.TitleId, UpdateType.Title));
                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoOnFinishTitleSelect {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void DescToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((sender is ToggleButton cbf) && (cbf.DataContext is ShortsDirectory ReleaseInfo))
                {
                    cbf.IsChecked = (ReleaseInfo.IsDescAvailable) ? ReleaseInfo.IsDescAvailable : cbf.IsChecked;
                    ModuleCallBack?.Invoke(this, new CustomParams_Select(ReleaseInfo.Id));
                    ModuleCallBack?.Invoke(this, new CustomParams_DescSelect(ReleaseInfo));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DescToggle_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    brd1.Width = e.NewSize.Width - 16;
                    brd1.Height = e.NewSize.Height - 123;
                    lstItems.Width = brd1.Width;
                    lstSchedules.Width = lstItems.Width - 8;
                    lstSchedules.Height = e.NewSize.Height - 160;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public DirectoryTitleDescEditor(databasehook<object> _dbInit, OnFinishIdObj _DoOnFinished)
        {
            try
            {
                InitializeComponent();
                ModuleCallBack = _dbInit;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    _DoOnFinished?.Invoke(this,-1);
                };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
    }
}
