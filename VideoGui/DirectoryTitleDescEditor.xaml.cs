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

        public string GetShortsDirectorySql(int index = -1)
        {
            try
            {
                return "SELECT S.ID, S.DIRECTORYNAME, S.TITLEID, S.DESCID, " +
                       "(SELECT LIST(TAGID, ',') FROM TITLETAGS " +
                       " WHERE GROUPID = S.TITLEID) AS LINKEDTITLEIDS, " +
                       " (SELECT LIST(ID,',') FROM DESCRIPTIONS " +
                       "WHERE TITLETAGID = S.DESCID) AS LINKEDDESCIDS " +
                       "FROM SHORTSDIRECTORY S" +
                (index != -1 ? $" WHERE S.ID = {index} " : "");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} GetShortsDirectorySql {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "";
            }
        }
        public void DoTitleSelectCreate(int TitleId = -1)
        {
            try
            {
      

                if (DoTitleSelectFrm is not null)
                {
                    if (!DoTitleSelectFrm.IsClosing && !DoTitleSelectFrm.IsClosed)
                    {
                        DoTitleSelectFrm.Close();
                        DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, 
                            ModuleCallBack, true, TitleId);
                        Hide();
                        DoTitleSelectFrm.Show();
                    }
                }
                else
                {
                    DoTitleSelectFrm = new TitleSelectFrm(DoOnFinishTitleSelect, ModuleCallBack, true, TitleId);
                    Hide();
                    DoTitleSelectFrm.Show();
                }
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

        private void OnSelectFormClose()
        {
            try
            {
                
                ModuleCallBack?.Invoke(this, new CustomParams_Update(DoDescSelectFrm.TitleTagId, UpdateType.Description));
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
                ModuleCallBack?.Invoke(this, new CustomParams_Update(DoTitleSelectFrm.TitleId, UpdateType.Title));
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

        public DirectoryTitleDescEditor(databasehook<object> _dbInit, OnFinish _DoOnFinished)
        {
            try
            {
                InitializeComponent();
                ModuleCallBack = _dbInit;
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
