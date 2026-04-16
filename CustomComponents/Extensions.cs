using CustomComponents.ListBoxExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static CustomComponents.delegates;

using Accessibility;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Automation;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit;

namespace CustomComponents
{
    public static class Extensions
    {
        public static void  LogWrite(this Exception Debugger, string message,
             ErrorHandler? OnError = null,string callingmethod = "")
        {
            try
            {
                OnError?.Invoke(Debugger, message, callingmethod);
            }
            catch (Exception ex)
            {
                Debug.Print(MethodBase.GetCurrentMethod().Name.ToString() + " LogWrite" + ex.Message);
            }
        }

        private static void SetFontBinding(this Type? control, FrameworkElementFactory controlFactory,
          DependencyProperty thisProperty, DependencyObject sourceObject, MultiListboxColumnDefinition? colDef)
        {
            try
            {
                if (colDef is null || controlFactory is null || control is null) return;
                var dp = (control == typeof(ComboBox)) ? ComboBox.FontSizeProperty :
                         (control == typeof(TextBlock)) ? TextBlock.FontSizeProperty : null;
                if (dp is null) return;
                if (!string.IsNullOrEmpty(colDef.TextualFontSizeBinding))
                {
                    controlFactory.SetBinding(dp, new Binding(colDef.TextualFontSizeBinding));
                }
                else if (thisProperty is not null)
                {
                    controlFactory.SetValue(dp, (double)sourceObject.GetValue(thisProperty));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetFontBinding {MethodBase.GetCurrentMethod()?.Name}");
            }
        }
    }
}
