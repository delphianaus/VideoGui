using System;
using System.Collections.Generic;
using System.IO;
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

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for TraceDebuggerInfo.xaml
    /// </summary>
    public partial class TraceDebuggerInfo : Window
    {
        bool Ready = false, IsClosed = false;
        public TraceDebuggerInfo(Action<object> onfinish)
        {
            InitializeComponent();
            Closed += (s, e) => 
            { 
                IsClosed = true;
                onfinish?.Invoke(this);
            };
        }

        public void InsertNewTrace(object sender, string text)
        {
            try
            {
                string frmname = sender.GetType().Name;
                string msg = $"{DateTime.Now:HH:mm:ss} {frmname} {text}";
                txttrace.Items.Insert(0, msg);
                msg.WriteLog(@"c:\videogui\DebuggerInfo_logs.txt");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InsertNewTrace {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (IsLoaded && Ready)
                {
                    ResizeWindows(e.NewSize.Width, e.NewSize.Height,
                        e.WidthChanged, e.HeightChanged);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Window_SizeChanged {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void ResizeWindows(double width, double height, bool widthChanged, bool heightChanged)
        {
            try
            {
                if (heightChanged)
                {
                    brd1.Height = height - 50;
                    txttrace.Height = brd1.Height;
                }
                if (widthChanged)
                {
                    brd1.Width = width - 20;
                    txttrace.Width = brd1.Width;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ResizeWindows {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void btnsave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(@"c:\videogui\DebuggerInfo_log.txt"))
                {
                    File.AppendAllText(@"c:\videogui\DebuggerInfo_log.txt", txttrace.Items.ToString());    
                }
                else
                {
                    File.WriteAllText(@"c:\videogui\DebuggerInfo_log.txt", txttrace.Items.ToString());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnsave_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void btnclear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txttrace.Items.Clear();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnclear_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"c:\videogui\DebuggerInfo_log.txt");
            }
            catch (Exception ex)
            {
                ex.LogWrite($"btnOpen_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {this}");
            }
        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
