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
    /// Interaction logic for DialogAges.xaml
    /// </summary>
    public partial class DialogAges : Window
    {
        public SetFilterAge DoSetFilterAge;
        string OldMinAge = "", OldMaxAge = "";
        OnFinish DoOnFinish = null;
        public bool IsClosed = false;
        public DialogAges(string TitleToShow,int a,int b,SetFilterAge _SetFilterAges, 
            OnFinishIdObj _OnFinish)
        {
            InitializeComponent();
            txtMaxAge.Text = "";
            txtMinAge.Text = "";
            if (a!= -1) txtMinAge.Text = a.ToString();
            if (b != -1) txtMaxAge.Text = b.ToString();
            Title = TitleToShow;
            DoSetFilterAge = _SetFilterAges;
            Closed += (s, e) => { IsClosed = true; _OnFinish?.Invoke(this, -1); };
        }

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch(Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void txtMaxAge_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OldMaxAge != "")
                {
                    if (OldMaxAge != txtMaxAge.Text)
                    {
                        DoSetFilterAge(txtMinAge.Text.ToInt(-1), txtMaxAge.Text.ToInt(-1));
                    }
                }
                else
                {
                    OldMaxAge = txtMaxAge.Text;
                    DoSetFilterAge(txtMinAge.Text.ToInt(-1), txtMaxAge.Text.ToInt(-1));
                }
            }
            catch(Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void txtMinAge_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (OldMinAge != txtMinAge.Text)
                    {
                        OldMinAge = txtMinAge.Text;
                        txtMaxAge.Focus();
                    }
                    else
                    {
                        txtMaxAge.Focus();
                    }
                }
            }
            catch(Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void txtMaxAge_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (OldMaxAge != txtMaxAge.Text)
                    {
                        OldMaxAge = txtMaxAge.Text;
                        txtMinAge.Focus();
                    }
                    else
                    {
                        txtMinAge.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

       

        private void txtMinAge_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OldMinAge != "")
                {
                    if (OldMinAge != txtMinAge.Text)
                    {
                        DoSetFilterAge(txtMinAge.Text.ToInt(-1), txtMaxAge.Text.ToInt(-1));
                    }
                }
                else
                {
                    OldMinAge = txtMinAge.Text;
                        DoSetFilterAge(txtMinAge.Text.ToInt(-1), txtMaxAge.Text.ToInt(-1));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
