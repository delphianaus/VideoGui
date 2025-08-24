using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using VideoGui.Models.delegates;
using VideoGui.Models;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for MasterTagSelectForm.xaml
    /// </summary>
    public partial class MasterTagSelectForm : Window
    {
        OnFinish DoOnFinish;
        databasehook<Object> Invoker;
        dataupdate DoUpdate;
        public bool IsTitleTag = false, IsShort = false, IsChanged = false,
            TagSetChanged = false, IsClosing = false, IsClosed = false;
        public int SelectedTagId = -1, SelectedId = -1, ParentId = -1;
        public MasterTagSelectForm(bool _IsShort,
            OnFinish _DoOnFinish, databasehook<Object> _Invoker,
            bool _IsTitleTag = true, int TagFilter = -1)
        {
            try
            {
                InitializeComponent();
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; _DoOnFinish?.Invoke(); };
                IsTitleTag = _IsTitleTag;
                ParentId = TagFilter;
                IsShort = _IsShort;
                DoOnFinish = _DoOnFinish;
                Invoker = _Invoker;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetSelectedTag {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Invoker?.Invoke(this, new CustomParams_Initialize());
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void lstDescriptions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lstDescriptions.SelectedItem is not null)
                {
                    int selectid = (lstDescriptions.SelectedItem is Titles tgt) ? tgt.Id : -1;
                    if (selectid != -1) IsChanged = true;
                    Invoker?.Invoke(this, new CustomParams_Get(selectid, ""));
                    Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"lstDescriptions_MouseDoubleClick {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void lstDescriptions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    int selectid = (lstDescriptions.SelectedItem is Titles tgt) ? tgt.Id : -1;

                    Invoker?.Invoke(this, new CustomParams_Select(selectid));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"lstDescriptions_SelectionChanged {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
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
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
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
                ex.LogWrite($"btnClose_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }












    }
}
