using Accessibility;
using CustomComponents.ListBoxExtensions;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using ToggleClass;
using Xceed.Wpf.Toolkit;
using static CustomComponents.delegates;

namespace CustomComponents
{
    public static class Extensions
    {
        public static MultiListboxColumnDefinition AsTextBox(this MultiListboxColumnDefinition def)
        {
            try
            {
                def.ComponentType = nameof(TextBox);
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AsTextBox {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition AsButton(this MultiListboxColumnDefinition def)
        {
            try
            {
                def.ComponentType = nameof(Button);
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AsButton {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition AsTextBlock(this MultiListboxColumnDefinition def)
        {
            try
            {
                def.ComponentType = nameof(TextBlock);
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AsTextBlock {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition AsComboBox(this MultiListboxColumnDefinition def)
        {
            try
            {
                def.ComponentType = nameof(ComboBox);
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AsComboBox {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition AsProgressBar(this MultiListboxColumnDefinition def)
        {
            try
            {
                def.ComponentType = nameof(ProgressBar);
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AsProgressBar {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition AsToggleButtonEx(this MultiListboxColumnDefinition def)
        {
            try
            {
                def.ComponentType = nameof(ToggleButtonEx);
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AsToggleButtonEx {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition AsCheckBox(this MultiListboxColumnDefinition def)
        {
            try
            {
                def.ComponentType = nameof(CheckBox);
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AsCheckBox {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
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
