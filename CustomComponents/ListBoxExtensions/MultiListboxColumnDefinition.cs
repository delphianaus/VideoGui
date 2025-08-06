using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace CustomComponents.ListBoxExtensions
{
    public class MultiListboxColumnDefinition : DependencyObject
    {


        public MultiListboxColumnDefinition()
        {
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(MultiListboxColumnDefinition), new PropertyMetadata(string.Empty));

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty DataFieldProperty =
            DependencyProperty.Register(nameof(DataField), typeof(string), typeof(MultiListboxColumnDefinition), new PropertyMetadata(string.Empty));

        public string DataField
        {
            get => (string)GetValue(DataFieldProperty);
            set => SetValue(DataFieldProperty, value);
        }



        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double), typeof(MultiListboxColumnDefinition), new PropertyMetadata(100.0));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height), typeof(double), typeof(MultiListboxColumnDefinition), 
                new PropertyMetadata(30.0, OnHeightChanged));

        public double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public static readonly DependencyProperty ControlTypeProperty =
            DependencyProperty.Register(nameof(ControlType), typeof(Type), 
                typeof(MultiListboxColumnDefinition), new PropertyMetadata(typeof(TextBlock)));

        public Type ControlType
        {
            get => (Type)GetValue(ControlTypeProperty);
            set => SetValue(ControlTypeProperty, value);
        }

        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register(nameof(ItemMargin), typeof(Thickness), typeof(MultiListboxColumnDefinition), new PropertyMetadata(new Thickness(2)));

        public Thickness ItemMargin
        {
            get => (Thickness)GetValue(ItemMarginProperty);
            set => SetValue(ItemMarginProperty, value);
        }

        public static readonly DependencyProperty ItemPaddingProperty =
            DependencyProperty.Register(nameof(ItemPadding), typeof(Thickness), typeof(MultiListboxColumnDefinition), new PropertyMetadata(new Thickness(2)));

        public Thickness ItemPadding
        {
            get => (Thickness)GetValue(ItemPaddingProperty);
            set => SetValue(ItemPaddingProperty, value);
        }

        public static readonly DependencyProperty ItemVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(ItemVerticalAlignment), typeof(VerticalAlignment), typeof(MultiListboxColumnDefinition), new PropertyMetadata(VerticalAlignment.Center));

        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register(nameof(FontWeight), typeof(string),
                typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(""));

        public string FontWeight
        {
            get => (string)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(string), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(""));

        public string Foreground
        {
            get => (string)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public VerticalAlignment ItemVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(ItemVerticalAlignmentProperty);
            set => SetValue(ItemVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty ItemHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(ItemHorizontalAlignment), typeof(HorizontalAlignment), typeof(MultiListboxColumnDefinition), new PropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment ItemHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(ItemHorizontalAlignmentProperty);
            set => SetValue(ItemHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(nameof(MinWidth), typeof(double), typeof(MultiListboxColumnDefinition), new PropertyMetadata(100.0));

        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(nameof(MinHeight), typeof(double), typeof(MultiListboxColumnDefinition), 
                new PropertyMetadata(25.0, OnMinHeightChanged));


        private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiListboxColumnDefinition colDef)
            {
                var parent = VisualTreeHelper.GetParent(colDef) as MultiListbox;
                if (parent != null)
                {
                    var itemHeight = parent.Height < parent.MinHeight ? parent.MinHeight : parent.Height;
                    if (itemHeight > 0)
                    {
                        colDef.Height = itemHeight;
                    }
                }
            }
        }
        private static void OnMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiListboxColumnDefinition colDef)
            {
                var parent = VisualTreeHelper.GetParent(colDef) as MultiListbox;
                if (parent != null)
                {
                    var itemMinHeight = parent.ItemMinHeight;
                    if (itemMinHeight > 0)
                    {
                        colDef.MinHeight = itemMinHeight;
                    }
                }
            }
        }

        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(nameof(MaxWidth), typeof(double), typeof(MultiListboxColumnDefinition), new PropertyMetadata(double.PositiveInfinity));

        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(nameof(MaxHeight), typeof(double), typeof(MultiListboxColumnDefinition), new PropertyMetadata(double.PositiveInfinity));

        public static readonly DependencyProperty WidthBindingProperty =
            DependencyProperty.Register(nameof(WidthBinding), typeof(string), typeof(MultiListboxColumnDefinition), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ComponentTypeProperty =
            DependencyProperty.Register(nameof(ComponentType), typeof(string), typeof(MultiListboxColumnDefinition), new PropertyMetadata("TextBlock"));

        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register(nameof(Style), typeof(Style), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(null));

        public Style Style
        {
            get => (Style)GetValue(StyleProperty);
            set => SetValue(StyleProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonWidthProperty =
            DependencyProperty.Register(nameof(ToggleButtonWidth), typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));

        public double ToggleButtonWidth
        {
            get => (double)GetValue(ToggleButtonWidthProperty);
            set => SetValue(ToggleButtonWidthProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonHeightProperty =
            DependencyProperty.Register(nameof(ToggleButtonHeight), typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));

        public double ToggleButtonHeight
        {
            get => (double)GetValue(ToggleButtonHeightProperty);
            set => SetValue(ToggleButtonHeightProperty, value);
        }

        public static readonly DependencyProperty HeaderMarginProperty =
            DependencyProperty.Register(nameof(HeaderMargin), typeof(Thickness), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new Thickness(5)));

        public Thickness HeaderMargin
        {
            get => (Thickness)GetValue(HeaderMarginProperty);
            set => SetValue(HeaderMarginProperty, value);
        }

        public static readonly DependencyProperty HeaderHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(HeaderHorizontalAlignment), typeof(HorizontalAlignment), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment HeaderHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(HeaderHorizontalAlignmentProperty);
            set => SetValue(HeaderHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty HeaderVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(HeaderVerticalAlignment), typeof(VerticalAlignment), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(VerticalAlignment.Center));

        public VerticalAlignment HeaderVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(HeaderVerticalAlignmentProperty);
            set => SetValue(HeaderVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty HeaderPaddingProperty =
            DependencyProperty.Register(nameof(HeaderPadding), typeof(Thickness), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new Thickness(0)));

        public Thickness HeaderPadding
        {
            get => (Thickness)GetValue(HeaderPaddingProperty);
            set => SetValue(HeaderPaddingProperty, value);
        }

        // click
        private RoutedEventHandler _Click;
        public event RoutedEventHandler Click
        {
            add { _Click += value; }
            remove { _Click -= value; }
        }
        protected virtual void OnClick(object sender, RoutedEventArgs e)
        {
            _Click?.Invoke(sender, e);
        }
        internal void RaiseClick(object sender, RoutedEventArgs e)
        {
            OnClick(sender, e);
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
        //RaiseLostFocus
        private RoutedEventHandler _RaiseLostFocus;
        public event RoutedEventHandler RaiseLostFocus
        {
            add { _RaiseLostFocus += value; }
            remove { _RaiseLostFocus -= value; }
        }
        protected virtual void OnRaiseLostFocus(object sender, RoutedEventArgs e)
        {
            _RaiseLostFocus?.Invoke(sender, e);
        }
        internal void RaiseLostFocusEvent(object sender, RoutedEventArgs e)
        {
            OnRaiseLostFocus(sender, e);
        }
        //RaiseGotFocus
        private RoutedEventHandler _RaiseGotFocus;
        public event RoutedEventHandler RaiseGotFocus
        {
            add { _RaiseGotFocus += value; }
            remove { _RaiseGotFocus -= value; }
        }
        protected virtual void OnRaiseGotFocus(object sender, RoutedEventArgs e)
        {
            _RaiseGotFocus?.Invoke(sender, e);
        }
        internal void RaiseGotFocusEvent(object sender, RoutedEventArgs e)
        {
            OnRaiseGotFocus(sender, e);
        }

        //RaiseInitialized
        private EventHandler _RaiseInitialized;
        public event EventHandler RaiseInitialized
        {
            add { _RaiseInitialized += value; }
            remove { _RaiseInitialized -= value; }
        }
        protected virtual void OnRaiseInitialized(object sender, EventArgs e)
        {
            _RaiseInitialized?.Invoke(sender, e);
        }
        internal void RaiseInitializedEvent(object sender, EventArgs e)
        {
            OnRaiseInitialized(sender, e);
        }

        public static readonly DependencyProperty UseStyleDimensionsProperty =
            DependencyProperty.Register(nameof(UseStyleDimensions), typeof(bool), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(true));

        public bool UseStyleDimensions
        {
            get => (bool)GetValue(UseStyleDimensionsProperty);
            set => SetValue(UseStyleDimensionsProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMinWidthProperty =
            DependencyProperty.Register(nameof(ToggleButtonMinWidth), typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(0.0));

        public double ToggleButtonMinWidth
        {
            get => (double)GetValue(ToggleButtonMinWidthProperty);
            set => SetValue(ToggleButtonMinWidthProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMaxWidthProperty =
            DependencyProperty.Register(nameof(ToggleButtonMaxWidth), typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity));

        public double ToggleButtonMaxWidth
        {
            get => (double)GetValue(ToggleButtonMaxWidthProperty);
            set => SetValue(ToggleButtonMaxWidthProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMinHeightProperty =
            DependencyProperty.Register(nameof(ToggleButtonMinHeight), typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(0.0));

        public double ToggleButtonMinHeight
        {
            get => (double)GetValue(ToggleButtonMinHeightProperty);
            set => SetValue(ToggleButtonMinHeightProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMaxHeightProperty =
            DependencyProperty.Register(nameof(ToggleButtonMaxHeight), typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity));

        public double ToggleButtonMaxHeight
        {
            get => (double)GetValue(ToggleButtonMaxHeightProperty);
            set => SetValue(ToggleButtonMaxHeightProperty, value);
        }

        public static readonly DependencyProperty ContentMarginProperty =
            DependencyProperty.Register(nameof(ContentMargin), typeof(Thickness), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new Thickness(0)));

        public Thickness ContentMargin
        {
            get => (Thickness)GetValue(ContentMarginProperty);
            set => SetValue(ContentMarginProperty, value);
        }

        public static readonly DependencyProperty ContentHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(ContentHorizontalAlignment), typeof(HorizontalAlignment), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment ContentHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(ContentHorizontalAlignmentProperty);
            set => SetValue(ContentHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty ContentVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(ContentVerticalAlignment), typeof(VerticalAlignment), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(VerticalAlignment.Center));

        public VerticalAlignment ContentVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(ContentVerticalAlignmentProperty);
            set => SetValue(ContentVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register(nameof(DateTimeFormat), typeof(string), typeof(MultiListboxColumnDefinition));

        public string DateTimeFormat
        {
            get => (string)GetValue(DateTimeFormatProperty);
            set => SetValue(DateTimeFormatProperty, value);
        }

        public static readonly DependencyProperty DateTimeFormatStringProperty =
            DependencyProperty.Register(nameof(DateTimeFormatString), typeof(string), typeof(MultiListboxColumnDefinition));

        public string DateTimeFormatString
        {
            get => (string)GetValue(DateTimeFormatStringProperty);
            set => SetValue(DateTimeFormatStringProperty, value);
        }

        public static readonly DependencyProperty DateTimeTimeFormatProperty =
            DependencyProperty.Register(nameof(DateTimeTimeFormat), typeof(string), typeof(MultiListboxColumnDefinition));

        public string DateTimeTimeFormat
        {
            get => (string)GetValue(DateTimeTimeFormatProperty);
            set => SetValue(DateTimeTimeFormatProperty, value);
        }

        public static readonly DependencyProperty DateTimeTimeFormatStringProperty =
            DependencyProperty.Register(nameof(DateTimeTimeFormatString), typeof(string), typeof(MultiListboxColumnDefinition));

        public string DateTimeTimeFormatString
        {
            get => (string)GetValue(DateTimeTimeFormatStringProperty);
            set => SetValue(DateTimeTimeFormatStringProperty, value);
        }

        public static readonly DependencyProperty DateTimeMinWidthProperty =
            DependencyProperty.Register(nameof(DateTimeMinWidth), typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));

        public double DateTimeMinWidth
        {
            get => (double)GetValue(DateTimeMinWidthProperty);
            set => SetValue(DateTimeMinWidthProperty, value);
        }

        public static readonly DependencyProperty DateTimeFocusableProperty =
            DependencyProperty.Register(nameof(DateTimeFocusable), typeof(bool), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(true));

        public bool DateTimeFocusable
        {
            get => (bool)GetValue(DateTimeFocusableProperty);
            set => SetValue(DateTimeFocusableProperty, value);
        }

        public static readonly DependencyProperty ButtonImageSourceProperty =
            DependencyProperty.Register(nameof(ButtonImageSource), typeof(ImageSource), typeof(MultiListboxColumnDefinition));

        public ImageSource ButtonImageSource
        {
            get => (ImageSource)GetValue(ButtonImageSourceProperty);
            set => SetValue(ButtonImageSourceProperty, value);
        }

        public static readonly DependencyProperty CheckBoxVerticalContentAlignmentProperty =
            DependencyProperty.Register(nameof(CheckBoxVerticalContentAlignment), typeof(VerticalAlignment), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(VerticalAlignment.Center));

        public VerticalAlignment CheckBoxVerticalContentAlignment
        {
            get => (VerticalAlignment)GetValue(CheckBoxVerticalContentAlignmentProperty);
            set => SetValue(CheckBoxVerticalContentAlignmentProperty, value);
        }

        public static readonly DependencyProperty CheckBoxHorizontalContentAlignmentProperty =
            DependencyProperty.Register(nameof(CheckBoxHorizontalContentAlignment), typeof(HorizontalAlignment), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment CheckBoxHorizontalContentAlignment
        {
            get => (HorizontalAlignment)GetValue(CheckBoxHorizontalContentAlignmentProperty);
            set => SetValue(CheckBoxHorizontalContentAlignmentProperty, value);
        }

        public static readonly DependencyProperty CheckBoxMarginProperty =
            DependencyProperty.Register(nameof(CheckBoxMargin), typeof(Thickness), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new Thickness(5)));

        public Thickness CheckBoxMargin
        {
            get => (Thickness)GetValue(CheckBoxMarginProperty);
            set => SetValue(CheckBoxMarginProperty, value);
        }

        public static readonly DependencyProperty CheckBoxIsManipulationEnabledProperty =
            DependencyProperty.Register(nameof(CheckBoxIsManipulationEnabled), typeof(bool), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(false));

        public bool CheckBoxIsManipulationEnabled
        {
            get => (bool)GetValue(CheckBoxIsManipulationEnabledProperty);
            set => SetValue(CheckBoxIsManipulationEnabledProperty, value);
        }

        public static readonly DependencyProperty CheckBoxFocusableProperty =
            DependencyProperty.Register(nameof(CheckBoxFocusable), typeof(bool), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(true));

        public bool CheckBoxFocusable
        {
            get => (bool)GetValue(CheckBoxFocusableProperty);
            set => SetValue(CheckBoxFocusableProperty, value);
        }

        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public double MinWidth
        {
            get => (double)GetValue(MinWidthProperty);
            set => SetValue(MinWidthProperty, value);
        }

        public double MinHeight
        {
            get => (double)GetValue(MinHeightProperty);
            set => SetValue(MinHeightProperty, value);
        }

        public double MaxWidth
        {
            get => (double)GetValue(MaxWidthProperty);
            set => SetValue(MaxWidthProperty, value);
        }

        public double MaxHeight
        {
            get => (double)GetValue(MaxHeightProperty);
            set => SetValue(MaxHeightProperty, value);
        }
        public string WidthBinding
        {
            get => (string)GetValue(WidthBindingProperty);
            set => SetValue(WidthBindingProperty, value);
        }

        public string ComponentType
        {
            get => (string)GetValue(ComponentTypeProperty);
            set => SetValue(ComponentTypeProperty, value);
        }

        public static readonly DependencyProperty BoundToProperty =
            DependencyProperty.Register(nameof(BoundTo), typeof(string), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(string.Empty));

        public string BoundTo
        {
            get => (string)GetValue(BoundToProperty);
            set => SetValue(BoundToProperty, value);
        }
    }
}
