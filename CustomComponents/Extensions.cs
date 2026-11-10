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
using ToggleClass;
using System.CodeDom;
using Microsoft.Windows.Themes;

namespace CustomComponents
{
    public static class Extensions
    {
        public static void LogWrite(this Exception Debugger, string message,
             ErrorHandler? OnError = null, string callingmethod = "")
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
        public static MultiListboxColumnDefinition SetStyle(this MultiListboxColumnDefinition def, Style styleKey)
        {
            try
            {
                def.Style = styleKey ?? def.Style;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetStyle {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
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
        public static MultiListboxColumnDefinition SetAllowTextInput(this MultiListboxColumnDefinition def, bool _fw)
        {
            try
            {
                def.AllowTextInput = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetAllowTextInput {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition SetAllowSpin(this MultiListboxColumnDefinition def, bool _fw)
        {
            try
            {
                def.AllowSpin = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetAllowSpin {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        private static ItemCollection CreateItemCollection()
        {
            return new ContextMenu().Items;
        }
        public static MultiListboxColumnDefinition SetContentHorizontalAlignmentBinding(this MultiListboxColumnDefinition def, string _fw)
        {
            try
            {
                def.ContentHorizontalAlignmentBinding = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetContentHorizontalAlignmentBinding {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition SetContentVerticalAlignmentBinding(this MultiListboxColumnDefinition def, string _fw)
        {
            try
            {
                def.ContentVerticalAlignmentBinding = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetContentVerticalAlignmentBinding {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition SetFormat(this MultiListboxColumnDefinition def, DateTimeFormat _fw)
        {
            try
            {
                def.Format = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetFormat {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetFormatString(this MultiListboxColumnDefinition def, string _fw)
        {
            try
            {
                def.FormatString = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetFormatString {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition SetKind(this MultiListboxColumnDefinition def, DateTimeKind _fw)
        {
            try
            {
                def.Kind = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetKind {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetHeight(this MultiListboxColumnDefinition def, double _fw)
        {
            try
            {
                def.Height = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetHeight {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetUseStyleDimensions(this MultiListboxColumnDefinition def, bool _fw)
        {
            try
            {
                def.UseStyleDimensions = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetUseStyleDimensions {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition SetCurrentDateTimePart(this MultiListboxColumnDefinition def, DateTimePart _fw)
        {
            try
            {
                def.CurrentDateTimePart = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetCurrentDateTimePart {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetDropDownClosed(this MultiListboxColumnDefinition def, EventHandler _fw)
        {
            try
            {
                def.DropDownClosed += _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetDropDownClosed {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition SetDateTimeMinWidth(this MultiListboxColumnDefinition def, double _fw)
        {
            try
            {
                def.DateTimeMinWidth = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetDateTimeMinWidth {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetDateTimeFocusable(this MultiListboxColumnDefinition def, bool _fw)
        {
            try
            {
                def.DateTimeFocusable = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetDateTimeFocusable {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetItemFocusable(this MultiListboxColumnDefinition def, bool _fw)
        {
            try
            {
                def.ItemFocusable = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetItemFocusable {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetItemImageSource(this MultiListboxColumnDefinition def, ImageSource _fw)
        {
            try
            {
                def.ItemImageSource = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetItemImageSource {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetItemIsManipulationEnabled(this MultiListboxColumnDefinition def, bool _fw)
        {
            try
            {
                def.ItemIsManipulationEnabled = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetItemIsManipulationEnabled {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetItemMargin(this MultiListboxColumnDefinition def, Thickness _fw)
        {
            try
            {
                def.ItemMargin = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetItemMargin {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetItemPadding(this MultiListboxColumnDefinition def, Thickness _fw)
        {
            try
            {
                def.ItemPadding = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetItemPadding {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetItemVerticalAlignment(this MultiListboxColumnDefinition def, VerticalAlignment _fw)
        {
            try
            {
                def.ItemVerticalAlignment = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetItemVerticalAlignment {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetMinHeight(this MultiListboxColumnDefinition def, double _fw)
        {
            try
            {
                def.MinHeight = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetMinHeight {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetMinHeightBinding(this MultiListboxColumnDefinition def, string _fw)
        {
            try
            {
                def.MinHeightBinding = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetMinHeightBinding {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetMinWidth(this MultiListboxColumnDefinition def, double _fw)
        {
            try
            {
                def.MinWidth = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetMinWidth {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetSelectionChanged(this MultiListboxColumnDefinition def, SelectionChangedEventHandler _fw)
        {
            try
            {
                def.SelectionChanged += _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetSelectionChanged {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetTextChanged(this MultiListboxColumnDefinition def, RoutedEventHandler _fw)
        {
            try
            {
                def.TextChanged += _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetTextChanged {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetTextualFontSizeBinding(this MultiListboxColumnDefinition def, string _fw)
        {
            try
            {
                def.TextualFontSizeBinding = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetTextualFontSizeBinding {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetTimeFormatString(this MultiListboxColumnDefinition def, string _fw)
        {
            try
            {
                def.TimeFormatString = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetTimeFormatString {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetTimePickerAllowSpin(this MultiListboxColumnDefinition def, bool _fw)
        {
            try
            {
                def.TimePickerAllowSpin = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetTimePickerAllowSpin {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetTimePickerShowButtonSpinner(this MultiListboxColumnDefinition def, bool _fw)
        {
            try
            {
                def.TimePickerShowButtonSpinner = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetTimePickerShowButtonSpinner {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetTimePickerVisibility(this MultiListboxColumnDefinition def, Visibility _fw)
        {
            try
            {
                def.TimePickerVisibility = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetTimePickerVisibility {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetToggleButtonHeight(this MultiListboxColumnDefinition def, double _fw)
        {
            try
            {
                def.ToggleButtonHeight = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetToggleButtonHeight {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetToggleButtonMaxWidth(this MultiListboxColumnDefinition def, double _fw)
        {
            try
            {
                def.ToggleButtonMaxWidth = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetToggleButtonMaxWidth {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetToggleButtonMinWidth(this MultiListboxColumnDefinition def, double _fw)
        {
            try
            {
                def.ToggleButtonMinWidth = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetToggleButtonMinWidth {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetToggleButtonWidth(this MultiListboxColumnDefinition def, double _fw)
        {
            try
            {
                def.ToggleButtonWidth = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetToggleButtonWidth {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetWidthBinding(this MultiListboxColumnDefinition def, RoutedEventHandler _fw)
        {
            try
            {
                def.WidthBinding += _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetWidthBinding {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetClick(this MultiListboxColumnDefinition def, RoutedEventHandler _fw)
        {
            try
            {
                def.Click += _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetClick {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetKeyUp(this MultiListboxColumnDefinition def, KeyEventHandler _fw)
        {
            try
            {
                def.KeyUp += _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetKeyUp {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetInitialized(this MultiListboxColumnDefinition def, EventHandler _fw)
        {
            try
            {
                def.Initialized += _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetInitialized {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetLostFocus(this MultiListboxColumnDefinition def, RoutedEventHandler _fw)
        {
            try
            {
                def.LostFocus += _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetLostFocus {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition SetGotFocus(this MultiListboxColumnDefinition def, RoutedEventHandler _fw)
        {
            try
            {
                def.GotFocus += _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetGotFocus {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetHeaderVerticalAlignment(this MultiListboxColumnDefinition def, VerticalAlignment _fw)
        {
            try
            {
                def.HeaderVerticalAlignment = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetHeaderVerticalAlignment {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition HeaderHorizontalAlignment(this MultiListboxColumnDefinition def, HorizontalAlignment _fw)
        {
            try
            {
                def.HeaderHorizontalAlignment = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetHeaderHorizontalAlignment {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetHeaderHorizontalAlignment(this MultiListboxColumnDefinition def, HorizontalAlignment _fw)
        {
            try
            {
                def.HeaderHorizontalAlignment = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetHeaderHorizontalAlignment {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetHeaderPadding(this MultiListboxColumnDefinition def, Thickness _fw)
        {
            try
            {
                def.HeaderPadding = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetHeaderPadding {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition SetHeaderMargin(this MultiListboxColumnDefinition def, Thickness _fw)
        {
            try
            {
                def.HeaderMargin = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetHeaderMargin {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetForeground(this MultiListboxColumnDefinition def, string _fw)
        {
            try
            {
                def.Foreground = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetForeground {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition SetFontWeight(this MultiListboxColumnDefinition def, string _fw)
        {
            try
            {
                def.FontWeight = _fw;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetFontWeight {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetName(this MultiListboxColumnDefinition def, string _name)
        {
            try
            {
                def.Name = _name;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetName {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetWidth(this MultiListboxColumnDefinition def, double _width)
        {
            try
            {
                def.Width = _width;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetWidth {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetProgressBarMinimum(this MultiListboxColumnDefinition def, double _Minimum)
        {
            try
            {
                def.ProgressBarMinimum = _Minimum;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetProgressBarMinimum {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }

        public static MultiListboxColumnDefinition SetProgressBarMaximum(this MultiListboxColumnDefinition def, double _Maximum)
        {
            try
            {
                def.ProgressBarMaximum = _Maximum;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetProgressBarMaximum {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetProgressBarForeground(
            this MultiListboxColumnDefinition def, Color NewColor)
        {
            try
            {
                def.ProgressBarForeground = new SolidColorBrush(NewColor);
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetProgressBarForeground {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition Hidden(this MultiListboxColumnDefinition def)
        {
            try
            {
                def.Width = 0;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Hidden {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetDataField(this MultiListboxColumnDefinition def, string _DataField)
        {
            try
            {
                def.DataField = _DataField;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetDataField {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetHeaderText(this MultiListboxColumnDefinition def, string headerText)
        {
            try
            {
                def.HeaderText = headerText;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetHeaderText {MethodBase.GetCurrentMethod()?.Name}");
                return def;
            }
        }
        public static MultiListboxColumnDefinition SetHeaderFontSize(this MultiListboxColumnDefinition def, double fontSize)
        {
            try
            {
                def.HeaderFontSize = fontSize;
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetHeaderFontSize {MethodBase.GetCurrentMethod()?.Name}");
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

        public static MultiListboxColumnDefinition AsLabel(this MultiListboxColumnDefinition def)
        {
            try
            {
                def.ComponentType = nameof(Label);
                return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AsLabel {MethodBase.GetCurrentMethod()?.Name}");
                return def;
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
