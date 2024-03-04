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
using VideoGui.Models.delegates;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for CustomStringEntry.xaml
    /// </summary>
    /// 

   
    public partial class CustomStringEntry : Window
    {
        OnKeyPressEvent DoOnKeyPressEvent;
        OnFinish DoOnFinish;

        public string GetData()
        {
            try
            {
                return txtData.Text;
            }
            catch(Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }
        public CustomStringEntry(string value, string Filter,OnKeyPressEvent _KeyPress, OnFinish _OnFinish)
        {
            try
            {
                InitializeComponent();
                Title = value;
                txtData.Text = Filter;
                DoOnKeyPressEvent = _KeyPress;
                DoOnFinish = _OnFinish;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void txtData_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (IsLoaded)
                {
                    DoOnKeyPressEvent?.Invoke(e.Key, txtData.Text);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void frmEnterString_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DoOnFinish?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
