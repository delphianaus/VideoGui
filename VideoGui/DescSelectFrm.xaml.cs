using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using VideoGui.Models.delegates;
using VideoGui.Models;


namespace VideoGui
{
    /// <summary>
    /// Interaction logic for DescSelectFrm.xaml
    /// </summary>
    public partial class DescSelectFrm : Window
    {
        OnFinish DoOnFinish;
        bool IsUploadsBuilder = false;
        databasehook<object> DoDbHook;
        public string Desc = "";
        public bool IsShortVideo = false, IsDescChanged = false, IsClosed = false, IsClosing = false;
        public int Id = -1, LinkedId = -1, TitleTagId = -1;
        public DescSelectFrm(OnFinish _DoOnFinish, databasehook<Object> _DoDbHook, bool _IsUploadsBuilder = false)
        {
            try
            {
                DoOnFinish = _DoOnFinish;
                DoDbHook = _DoDbHook;
                IsUploadsBuilder = _IsUploadsBuilder;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; IsDescChanged = false; _DoOnFinish?.Invoke(); };
                InitializeComponent();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DescSelectFrm {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoDbHook?.Invoke(this, new CustomParams_AddDescription(Id, txtDesc.Text, txtDescName.Text));
                //DoOnUpdate?.Invoke(dataUpdatType.Add,-1,txtDesc.Text+"|"+txtDescName.Text,this);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnSelect_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void ImgSelect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if ((e.ChangedButton == MouseButton.Left) && (e.ClickCount == 1))
                {
                    btnSelect_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ImgSelect_MouseDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Desc != txtDesc.Text)
                {
                    IsDescChanged = true;
                }
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnClose_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DoDbHook?.Invoke(this, IsUploadsBuilder ? new CustomParams_Initialize(IsUploadsBuilder) : null);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_Loaded {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
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



        private void mnuDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (lstAllDescriptions.SelectedItem is Descriptions DS)
                {
                    DoDbHook?.Invoke(this, new CustomParams_Remove(DS.Id));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuDeleteItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuEditItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstAllDescriptions.SelectedItem is Descriptions DS)
                {
                    txtDesc.Text = DS.Description;
                    txtDescName.Text = DS.TitleTag;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuEditItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void mnuNewItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDesc.Text = "";
                txtDescName.Text = "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"mnuNewItem_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
    }
}
