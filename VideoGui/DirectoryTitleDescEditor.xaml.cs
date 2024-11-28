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
        SetLists SetLists = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ModuleCallBack?.Invoke(this, new CustomParams_Initialize());
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
                    var p = new CustomParams_TitleSelect(ReleaseInfo);
                    ModuleCallBack?.Invoke(this, p);
                    cbf.IsChecked = (p.UploadsReleaseInfo.IsTitleAvailable);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"TitleToggle_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public void DoTitleSelectCreate()
        {
            try
            {

                if (DoTitleSelectFrm is not null)
                {
                    if (!DoTitleSelectFrm.IsClosing && !DoTitleSelectFrm.IsClosed)
                    {
                        DoTitleSelectFrm.Close();
                        DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, ModuleCallBack, SetLists,true);
                        Hide();
                        DoTitleSelectFrm.Show();
                    }
                }
                else
                {
                    DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, ModuleCallBack, SetLists, true);
                    Hide();
                    DoTitleSelectFrm.Show();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoTitleSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public void DoDescSelectCreate()
        {
            try
            {
                if (DoDescSelectFrm is not null && DoDescSelectFrm.IsActive)
                {
                    DoDescSelectFrm.Close();
                    DoDescSelectFrm = null;
                }

                DoDescSelectFrm = new DescSelectFrm(OnSelectFormClose, ModuleCallBack, true);
                Hide();
                DoDescSelectFrm.Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} DoDescSelectCreate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void OnSelectFormClose()
        {
            try
            {
                if (DoDescSelectFrm is not null)
                {
                    if (DoDescSelectFrm.IsDescChanged)
                    {
                        ModuleCallBack?.Invoke(this, new CustomParams_Update(DoDescSelectFrm.TitleTagId, UpdateType.Description));
                    }
                    if (DoDescSelectFrm.IsClosed)
                    {
                        DoDescSelectFrm = null;
                    }
                }
                Show();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} OnSelectFormClose {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void DoOnFinishTitleSelect()
        {
            try
            {
                if (DoTitleSelectFrm is not null)
                {
                    if (DoTitleSelectFrm.IsTitleChanged)
                    {
                        ModuleCallBack?.Invoke(this, new CustomParams_Update(DoTitleSelectFrm.TitleId, UpdateType.Title));
                    }
                    if (DoTitleSelectFrm.IsClosed)
                    {
                        DoTitleSelectFrm = null;
                    }
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
                    var p = new CustomParams_DescSelect(ReleaseInfo);
                    ModuleCallBack?.Invoke(this, p);
                    cbf.IsChecked = (p.UploadsReleaseInfo.IsDescAvailable);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DescToggle_Click {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public DirectoryTitleDescEditor(databasehook<object> _dbInit, OnFinish _DoOnFinished, SetLists _SetLists)
        {
            try
            {
                InitializeComponent();
                ModuleCallBack = _dbInit;
                SetLists = _SetLists;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) =>
                {
                    IsClosed = true;
                    _DoOnFinished?.Invoke();
                };
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Constructor {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }
    }
}
