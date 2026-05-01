using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using static CustomComponents.delegates;

namespace CustomComponents.ListBoxExtensions
{
    public class MultiListboxColumnDefinition : DependencyObject
    {
        public class InheritedDouble
        {
            public string Value { get; set; } = "9.0";
            public string Inherited { get; set; } = "True";
            public InheritedDouble(string _Inherited, string _Value)
            {
                Value = _Value;
                Inherited = _Inherited;
            }
        }
        public MultiListboxColumnDefinition()
        {
        }

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(nameof(HeaderText),
                typeof(string), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(string.Empty));


        public double HeaderWidth
        {
            get => (double)GetValue(HeaderWidthProperty);
            set => SetValue(HeaderWidthProperty, value);
        }

        public static readonly DependencyProperty HeaderWidthProperty =
            DependencyProperty.Register(nameof(HeaderWidth),
                typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));
        public string DataField
        {
            get => (string)GetValue(DataFieldProperty);
            set => SetValue(DataFieldProperty, value);
        }

        public static readonly DependencyProperty DataFieldProperty =
            DependencyProperty.Register(nameof(DataField),
                typeof(string), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(string.Empty));
        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));

        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiListboxColumnDefinition colDef)
            {
                var parent = VisualTreeHelper.GetParent(colDef) as MultiListbox;
                if (parent != null)
                {
                    var itemWidth = parent.Width < parent.MinWidth ? parent.MinWidth : parent.Width;

                    colDef.Width = itemWidth;

                }
            }
        }

        public double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height),
                typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(10.0));
        private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiListboxColumnDefinition colDef)
            {
                var parent = VisualTreeHelper.GetParent(colDef) as MultiListbox;
                if (parent != null)
                {
                    var itemHeight = parent.Height < parent.MinHeight ? parent.MinHeight : parent.Height;
                    if (itemHeight > 0 && colDef.ComponentType.ToString().ToLower() != "textblock")
                    {
                        colDef.Height = itemHeight;
                    }
                }
            }
        }
        public Type ControlType
        {
            get => (Type)GetValue(ControlTypeProperty);
            set => SetValue(ControlTypeProperty, value);
        }
        public static readonly DependencyProperty ControlTypeProperty =
            DependencyProperty.Register(nameof(ControlType), typeof(Type),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(typeof(TextBlock)));
        public Thickness ItemMargin
        {
            get => (Thickness)GetValue(ItemMarginProperty);
            set => SetValue(ItemMarginProperty, value);
        }
        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register(nameof(ItemMargin), typeof(Thickness),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new Thickness(0)));
        public Thickness ItemPadding
        {
            get => (Thickness)GetValue(ItemPaddingProperty);
            set => SetValue(ItemPaddingProperty, value);
        }
        public static readonly DependencyProperty ItemPaddingProperty =
            DependencyProperty.Register(nameof(ItemPadding),
                typeof(Thickness), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new Thickness(0)));
        public VerticalAlignment ItemVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(ItemVerticalAlignmentProperty);
            set => SetValue(ItemVerticalAlignmentProperty, value);
        }
        public static readonly DependencyProperty ItemVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(ItemVerticalAlignment), typeof(VerticalAlignment),
                typeof(MultiListboxColumnDefinition), new PropertyMetadata(VerticalAlignment.Center));
        public string FontWeight
        {
            get => (string)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register(nameof(FontWeight), typeof(string),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(""));
        public string Foreground
        {
            get => (string)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(string),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(""));
        public HorizontalAlignment ItemHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(ItemHorizontalAlignmentProperty);
            set => SetValue(ItemHorizontalAlignmentProperty, value);
        }
        public static readonly DependencyProperty ItemHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(ItemHorizontalAlignment), typeof(HorizontalAlignment),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(HorizontalAlignment.Left));
        public double MinWidth
        {
            get => (double)GetValue(MinWidthProperty);
            set => SetValue(MinWidthProperty, value);
        }
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(nameof(MinWidth),
                typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));
        public double MinHeight
        {
            get => (double)GetValue(MinHeightProperty);
            set => SetValue(MinHeightProperty, value);
        }
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(nameof(MinHeight),
                typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(12.0));
        private static void OnMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiListboxColumnDefinition colDef)
            {
                if (colDef.ComponentType.ToString().ToLower() == "datetimepicker")
                {
                    return; // Don't override MinHeight for TextBlocks
                }
                var parent = VisualTreeHelper.GetParent(colDef) as MultiListbox;
                if (parent != null)
                {
                    var itemMinHeight = parent.ItemMinHeight;
                    if (itemMinHeight > 0 && colDef.ComponentType.ToString().ToLower() != "textblock")
                    {
                        colDef.MinHeight = itemMinHeight;
                    }
                }
            }
        }
        public double MaxWidth
        {
            get => (double)GetValue(MaxWidthProperty);
            set => SetValue(MaxWidthProperty, value);
        }
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(nameof(MaxWidth), typeof(double),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity));
        public double MaxHeight
        {
            get => (double)GetValue(MaxHeightProperty);
            set => SetValue(MaxHeightProperty, value);
        }
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(nameof(MaxHeight), typeof(double),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity));
        //ContentVerticalAlignmentBinding

        //TextualFontSizeBinding
        public string TextualFontSizeBinding
        {
            get => (string)GetValue(TextualFontSizeBindingProperty);
            set => SetValue(TextualFontSizeBindingProperty, value);
        }
        public static readonly DependencyProperty TextualFontSizeBindingProperty =
            DependencyProperty.Register(nameof(TextualFontSizeBindingProperty), typeof(string),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(string.Empty));
        

        public TextAlignment ItemTextAlignment
        {
            get => (TextAlignment)GetValue(ItemTextAlignmentProperty);
            set => SetValue(ItemTextAlignmentProperty, value);
        }
        public static readonly DependencyProperty ItemTextAlignmentProperty =
            DependencyProperty.Register(nameof(ItemTextAlignmentProperty), typeof(TextAlignment),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(TextAlignment.Left));




        public string ContentHorizontalAlignmentBinding
        {
            get => (string)GetValue(ContentHorizontalAlignmentBindingProperty);
            set => SetValue(ContentHorizontalAlignmentBindingProperty, value);
        }
        public static readonly DependencyProperty ContentHorizontalAlignmentBindingProperty =
            DependencyProperty.Register(nameof(ContentHorizontalAlignmentBindingProperty), typeof(string),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(string.Empty));

        public string ContentVerticalAlignmentBinding
        {
            get => (string)GetValue(ContentVerticalAlignmentBindingProperty);
            set => SetValue(ContentVerticalAlignmentBindingProperty, value);
        }
        public static readonly DependencyProperty ContentVerticalAlignmentBindingProperty =
            DependencyProperty.Register(nameof(ContentVerticalAlignmentBinding), typeof(string),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(string.Empty));


        public string MinHeightBinding
        {
            get => (string)GetValue(MinHeightBindingProperty);
            set => SetValue(MinHeightBindingProperty, value);
        }
        public static readonly DependencyProperty MinHeightBindingProperty =
            DependencyProperty.Register(nameof(MinHeightBinding), typeof(string),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(string.Empty));
        public string WidthBinding
        {
            get => (string)GetValue(WidthBindingProperty);
            set => SetValue(WidthBindingProperty, value);
        }
        public static readonly DependencyProperty WidthBindingProperty =
            DependencyProperty.Register(nameof(WidthBinding), typeof(string),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(string.Empty));




        public string BoundTo
        {
            get => (string)GetValue(BoundToProperty);
            set => SetValue(BoundToProperty, value);
        }
        public static readonly DependencyProperty BoundToProperty =
            DependencyProperty.Register(nameof(BoundTo), typeof(string), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(string.Empty));

        public string ComponentType
        {
            get => (string)GetValue(ComponentTypeProperty);
            set => SetValue(ComponentTypeProperty, value);
        }
        public static readonly DependencyProperty ComponentTypeProperty =
            DependencyProperty.Register(nameof(ComponentType), typeof(string),
                typeof(MultiListboxColumnDefinition), new PropertyMetadata("TextBlock"));
        public Style Style
        {
            get => (Style)GetValue(StyleProperty);
            set => SetValue(StyleProperty, value);
        }
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register(nameof(Style), typeof(Style),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(null));

        public double ToggleButtonWidth
        {
            get => (double)GetValue(ToggleButtonWidthProperty);
            set => SetValue(ToggleButtonWidthProperty, value);
        }
        public static readonly DependencyProperty ToggleButtonWidthProperty =
            DependencyProperty.Register(nameof(ToggleButtonWidth),
                typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));
        public double ToggleButtonHeight
        {
            get => (double)GetValue(ToggleButtonHeightProperty);
            set => SetValue(ToggleButtonHeightProperty, value);
        }
        public static readonly DependencyProperty ToggleButtonHeightProperty =
            DependencyProperty.Register(nameof(ToggleButtonHeight),
                typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));
        public Thickness HeaderMargin
        {
            get => (Thickness)GetValue(HeaderMarginProperty);
            set => SetValue(HeaderMarginProperty, value);
        }
        public static readonly DependencyProperty HeaderMarginProperty =
            DependencyProperty.Register(nameof(HeaderMargin),
                typeof(Thickness), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty HeaderHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(HeaderHorizontalAlignment), typeof(HorizontalAlignment), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment HeaderHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(HeaderHorizontalAlignmentProperty);
            set => SetValue(HeaderHorizontalAlignmentProperty, value);
        }
        public VerticalAlignment HeaderVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(HeaderVerticalAlignmentProperty);
            set => SetValue(HeaderVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty HeaderVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(HeaderVerticalAlignment),
                typeof(VerticalAlignment), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(VerticalAlignment.Center));


        public static readonly DependencyProperty HeaderPaddingProperty =
            DependencyProperty.Register(nameof(HeaderPadding),
                typeof(Thickness), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new Thickness(0)));

        public Thickness HeaderPadding
        {
            get => (Thickness)GetValue(HeaderPaddingProperty);
            set => SetValue(HeaderPaddingProperty, value);
        }

        public static readonly DependencyProperty UseStyleDimensionsProperty =
                    DependencyProperty.Register(nameof(UseStyleDimensions),
                        typeof(bool), typeof(MultiListboxColumnDefinition),
                        new PropertyMetadata(true));

        public bool Sortable
        {
            get => (bool)GetValue(SortableProperty);
            set => SetValue(SortableProperty, value);
        }

        public static readonly DependencyProperty SortableProperty =
            DependencyProperty.Register(nameof(Sortable),
                typeof(bool), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(false));

        public bool SortDirection
        {
            get => (bool)GetValue(SortDirectionProperty);
            set => SetValue(SortDirectionProperty, value);
        }

        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register(nameof(SortDirection),
                typeof(bool), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(true));

        public bool IsSortActive
        {
            get => (bool)GetValue(IsSortActiveProperty);
            set => SetValue(IsSortActiveProperty, value);
        }

        public static readonly DependencyProperty IsSortActiveProperty =
            DependencyProperty.Register(nameof(IsSortActive),
                typeof(bool), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(true));
        public bool UseStyleDimensions
        {
            get => (bool)GetValue(UseStyleDimensionsProperty);
            set => SetValue(UseStyleDimensionsProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMinWidthProperty =
            DependencyProperty.Register(nameof(ToggleButtonMinWidth),
                typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(0.0));

        public double ToggleButtonMinWidth
        {
            get => (double)GetValue(ToggleButtonMinWidthProperty);
            set => SetValue(ToggleButtonMinWidthProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMaxWidthProperty =
            DependencyProperty.Register(nameof(ToggleButtonMaxWidth),
                typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity));

        public double ToggleButtonMaxWidth
        {
            get => (double)GetValue(ToggleButtonMaxWidthProperty);
            set => SetValue(ToggleButtonMaxWidthProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMinHeightProperty =
            DependencyProperty.Register(nameof(ToggleButtonMinHeight),
                typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(0.0));

        public double ToggleButtonMinHeight
        {
            get => (double)GetValue(ToggleButtonMinHeightProperty);
            set => SetValue(ToggleButtonMinHeightProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMaxHeightProperty =
            DependencyProperty.Register(nameof(ToggleButtonMaxHeight),
                typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity));

        public double ToggleButtonMaxHeight
        {
            get => (double)GetValue(ToggleButtonMaxHeightProperty);
            set => SetValue(ToggleButtonMaxHeightProperty, value);
        }

        //MouseWheelActiveTrigger MouseWheelActiveTrigger
        public DateTimePart CurrentDateTimePart
        {
            get => (DateTimePart)GetValue(CurrentDateTimePartProperty);
            set => SetValue(MouseWheelActiveTriggerProperty, value);
        }

        public static readonly DependencyProperty CurrentDateTimePartProperty =
           DependencyProperty.Register(nameof(CurrentDateTimePart),
               typeof(DateTimePart), typeof(MultiListboxColumnDefinition),
               new PropertyMetadata(DateTimePart.MonthName));

        public MouseWheelActiveTrigger MouseWheelActiveTrigger
        {
            get => (MouseWheelActiveTrigger)GetValue(MouseWheelActiveTriggerProperty);
            set => SetValue(MouseWheelActiveTriggerProperty, value);
        }

        public static readonly DependencyProperty MouseWheelActiveTriggerProperty =
           DependencyProperty.Register(nameof(MouseWheelActiveTrigger),
               typeof(MouseWheelActiveTrigger), typeof(MultiListboxColumnDefinition),
               new PropertyMetadata(MouseWheelActiveTrigger.Disabled));


        public bool TimePickerAllowSpin
        {
            get => (bool)GetValue(TimePickerAllowSpinProperty);
            set => SetValue(TimePickerAllowSpinProperty, value);
        }
        public static readonly DependencyProperty TimePickerAllowSpinProperty =
           DependencyProperty.Register(nameof(TimePickerAllowSpin),
               typeof(bool), typeof(MultiListboxColumnDefinition),
               new PropertyMetadata(true));

        public bool TimePickerShowButtonSpinner
        {
            get => (bool)GetValue(TimePickerShowButtonSpinnerProperty);
            set => SetValue(TimePickerShowButtonSpinnerProperty, value);
        }
        public static readonly DependencyProperty TimePickerShowButtonSpinnerProperty =
           DependencyProperty.Register(nameof(TimePickerShowButtonSpinner),
               typeof(bool), typeof(MultiListboxColumnDefinition),
               new PropertyMetadata(true));


        public bool AllowTextInput
        {
            get => (bool)GetValue(AllowTextInputProperty);
            set => SetValue(AllowTextInputProperty, value);
        }
        public static readonly DependencyProperty AllowTextInputProperty =
           DependencyProperty.Register(nameof(AllowTextInput),
               typeof(bool), typeof(MultiListboxColumnDefinition),
               new PropertyMetadata(true));

        public bool AllowSpin
        {
            get => (bool)GetValue(AllowSpinProperty);
            set => SetValue(AllowSpinProperty, value);
        }

        public static readonly DependencyProperty AllowSpinProperty =
           DependencyProperty.Register(nameof(AllowSpin),
               typeof(bool), typeof(MultiListboxColumnDefinition),
               new PropertyMetadata(true));


        public static readonly DependencyProperty KindProperty =
           DependencyProperty.Register(nameof(Kind),
               typeof(DateTimeKind), typeof(MultiListboxColumnDefinition),
               new PropertyMetadata(DateTimeKind.Local));

        public DateTimeKind Kind
        {
            get => (DateTimeKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }


        public static readonly DependencyProperty TimePickerVisibilityProperty =
            DependencyProperty.Register(nameof(TimePickerVisibility),
                typeof(Visibility), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(Visibility.Visible));

        public Visibility TimePickerVisibility
        {
            get => (Visibility)GetValue(TimePickerVisibilityProperty);
            set => SetValue(TimePickerVisibilityProperty, value);
        }

        public static readonly DependencyProperty FormatStringProperty =
            DependencyProperty.Register(nameof(FormatString),
                typeof(string), typeof(MultiListboxColumnDefinition));

        public DateTimeFormat Format
        {
            get => (DateTimeFormat)GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format),
                typeof(DateTimeFormat), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(DateTimeFormat.FullDateTime));

        public string FormatString
        {
            get => (string)GetValue(FormatStringProperty);
            set => SetValue(FormatStringProperty, value);
        }
        public static readonly DependencyProperty TimeFormatProperty =
            DependencyProperty.Register(nameof(TimeFormat),
                typeof(DateTimeFormat), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(DateTimeFormat.LongTime));

        public DateTimeFormat TimeFormat
        {
            get => (DateTimeFormat)GetValue(TimeFormatProperty);
            set => SetValue(TimeFormatProperty, value);
        }
        public string TimeFormatString
        {
            get => (string)GetValue(TimeFormatStringProperty);
            set => SetValue(TimeFormatStringProperty, value);
        }

        public static readonly DependencyProperty TimeFormatStringProperty =
            DependencyProperty.Register(nameof(TimeFormatString),
                typeof(string), typeof(MultiListboxColumnDefinition));
        public double DateTimeMinWidth
        {
            get => (double)GetValue(DateTimeMinWidthProperty);
            set => SetValue(DateTimeMinWidthProperty, value);
        }
        public static readonly DependencyProperty DateTimeMinWidthProperty =
            DependencyProperty.Register(nameof(DateTimeMinWidth),
               typeof(double), typeof(MultiListboxColumnDefinition),
                 new PropertyMetadata(double.NaN));
        public bool DateTimeFocusable
        {
            get => (bool)GetValue(DateTimeFocusableProperty);
            set => SetValue(DateTimeFocusableProperty, value);
        }
        public static readonly DependencyProperty DateTimeFocusableProperty =
            DependencyProperty.Register(nameof(DateTimeFocusable),
                typeof(bool), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(true));
        public ImageSource ItemImageSource
        {
            get => (ImageSource)GetValue(ItemImageSourceProperty);
            set => SetValue(ItemImageSourceProperty, value);
        }
        public static readonly DependencyProperty ItemImageSourceProperty =
            DependencyProperty.Register(nameof(ItemImageSource),
                typeof(ImageSource), typeof(MultiListboxColumnDefinition));




        public bool ItemIsManipulationEnabled
        {
            get => (bool)GetValue(ItemIsManipulationEnabledProperty);
            set => SetValue(ItemIsManipulationEnabledProperty, value);
        }
        public static readonly DependencyProperty ItemIsManipulationEnabledProperty =
            DependencyProperty.Register(nameof(ItemIsManipulationEnabled),
                typeof(bool), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(false));


        public bool ItemFocusable
        {
            get => (bool)GetValue(ItemFocusableProperty);
            set => SetValue(ItemFocusableProperty, value);
        }
        public static readonly DependencyProperty ItemFocusableProperty =
            DependencyProperty.Register(nameof(ItemFocusable),
                typeof(bool), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(true));




        // EVENTS
      

        private RoutedEventHandler _Click;
        public event RoutedEventHandler Click
        {
            add { _Click += value; }
            remove { _Click -= value; }
        }
        protected virtual void OnClick(object sender, RoutedEventArgs e)
        {
            _Click?.Invoke(sender, e);
            e.Handled = true;
        }
        internal void RaiseClick(object sender, RoutedEventArgs e)
        {

            OnClick(sender, e);
        }

        private SortChangedHandler _SortChange;
        public event SortChangedHandler SortChange
        {
            add { _SortChange += value; }
            remove { _SortChange-= value; }
        }
        protected virtual void OnSortChange(object sender, bool e)
        {
            _SortChange?.Invoke(sender, e);
        }   
        internal void RaiseSortChange(object sender, bool e)
        {
            OnSortChange(sender, e);
        }


        // DropDownClosed
        private EventHandler _DropDownClosed;
        public event EventHandler DropDownClosed
        {
            add { _DropDownClosed += value; }
            remove { _DropDownClosed-= value; }
        }
        protected virtual void OnDropDownClosed(object sender, EventArgs e)
        {

            _DropDownClosed?.Invoke(sender, e);
           
        }
        internal void RaiseDropDownClosed(object sender, EventArgs e)
        {
            OnDropDownClosed(sender, e);
           
        }


        // textChanged
        private RoutedEventHandler _TextChanged;
        public event RoutedEventHandler TextChanged
        {
            add { _TextChanged += value; }
            remove { _TextChanged -= value; }
        }
        protected virtual void OnTextChanged(object sender, RoutedEventArgs e)
        {
            _TextChanged?.Invoke(sender, e);
        }
        internal void RaiseTextChanged(object sender, TextChangedEventArgs e)
        {
            OnTextChanged(sender, e);
        }
        // selection changed event
        private SelectionChangedEventHandler _SelectionChanged;
        public event SelectionChangedEventHandler SelectionChanged
        {
            add { _SelectionChanged += value; }
            remove { _SelectionChanged -= value; }
        }

        protected virtual void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _SelectionChanged?.Invoke(sender, e);
        }
        internal void RaiseSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectionChanged(sender, e);
        }

        //KeyEventHandler
        private KeyEventHandler _KeyUp;
        public event KeyEventHandler KeyUp
        {
            add { _KeyUp += value; }
            remove { _KeyUp -= value; }
        }
        protected virtual void OnKeyUp(object sender, KeyEventArgs e)
        {
            _KeyUp?.Invoke(sender, e);
        }
        internal void RaiseKeyUpEvent(object sender, KeyEventArgs e)
        {
            OnKeyUp(sender, e);
        }
        //RaiseLostFocus
        private RoutedEventHandler _LostFocus;
        public event RoutedEventHandler LostFocus
        {
            add { _LostFocus += value; }
            remove { _LostFocus -= value; }
        }
        protected virtual void OnLostFocus(object sender, RoutedEventArgs e)
        {
            _LostFocus?.Invoke(sender, e);
        }
        internal void LostFocusEvent(object sender, RoutedEventArgs e)
        {
            OnLostFocus(sender, e);
        }
        //RaiseGotFocus
        private RoutedEventHandler _GotFocus;
        public event RoutedEventHandler GotFocus
        {
            add { _GotFocus += value; }
            remove { _GotFocus -= value; }
        }
        protected virtual void OnGotFocus(object sender, RoutedEventArgs e)
        {
            _GotFocus?.Invoke(sender, e);
        }
        internal void GotFocusEvent(object sender, RoutedEventArgs e)
        {
            OnGotFocus(sender, e);
        }

        //RaiseInitialized


        private EventHandler _Initialized;
        public event EventHandler Initialized
        {
            add { _Initialized += value; }
            remove { _Initialized -= value; }
        }
        protected virtual void OnInitialized(object sender, EventArgs e)
        {
            _Initialized?.Invoke(sender, e);
        }
        internal void       InitializedEvent(object sender, EventArgs e)
        {
            OnInitialized(sender, e);
        }













    }
}
