using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace CustomComponents.ListBoxExtensions
{
    public class MultiListboxColumnDefinition : DependencyObject
    {
        public MultiListboxColumnDefinition()
        {
            BoundToProperties = new ObservableCollection<BoundToProperty>();
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

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double), typeof(MultiListboxColumnDefinition), new PropertyMetadata(100.0));

        public static readonly DependencyProperty WidthBindingProperty =
            DependencyProperty.Register(nameof(WidthBinding), typeof(string), typeof(MultiListboxColumnDefinition), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ComponentTypeProperty =
            DependencyProperty.Register(nameof(ComponentType), typeof(string), typeof(MultiListboxColumnDefinition), new PropertyMetadata("TextBlock"));

        public static readonly DependencyProperty BoundToProperty =
            DependencyProperty.Register(nameof(BoundTo), 
                typeof(string), typeof(MultiListboxColumnDefinition), 
                new PropertyMetadata("Text"));

        public static readonly DependencyProperty BoundToPropertiesProperty =
            DependencyProperty.Register(nameof(BoundToProperties), typeof(ObservableCollection<BoundToProperty>), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new ObservableCollection<BoundToProperty>()));

        public ObservableCollection<BoundToProperty> BoundToProperties
        {
            get => (ObservableCollection<BoundToProperty>)GetValue(BoundToPropertiesProperty);
            set => SetValue(BoundToPropertiesProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonStyleProperty =
            DependencyProperty.Register(nameof(ToggleButtonStyle), typeof(Style), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(null));

        public Style ToggleButtonStyle
        {
            get => (Style)GetValue(ToggleButtonStyleProperty);
            set => SetValue(ToggleButtonStyleProperty, value);
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

        private RoutedEventHandler _toggleButtonClick;
        public event RoutedEventHandler ToggleButtonClick
        {
            add { _toggleButtonClick += value; }
            remove { _toggleButtonClick -= value; }
        }

        protected virtual void OnToggleButtonClick(object sender, RoutedEventArgs e)
        {
            _toggleButtonClick?.Invoke(sender, e);
        }

        internal void RaiseToggleButtonClick(object sender, RoutedEventArgs e)
        {
            OnToggleButtonClick(sender, e);
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
                new PropertyMetadata(double.NaN));

        public double ToggleButtonMinWidth
        {
            get => (double)GetValue(ToggleButtonMinWidthProperty);
            set => SetValue(ToggleButtonMinWidthProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMaxWidthProperty =
            DependencyProperty.Register(nameof(ToggleButtonMaxWidth), typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));

        public double ToggleButtonMaxWidth
        {
            get => (double)GetValue(ToggleButtonMaxWidthProperty);
            set => SetValue(ToggleButtonMaxWidthProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMinHeightProperty =
            DependencyProperty.Register(nameof(ToggleButtonMinHeight), typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));

        public double ToggleButtonMinHeight
        {
            get => (double)GetValue(ToggleButtonMinHeightProperty);
            set => SetValue(ToggleButtonMinHeightProperty, value);
        }

        public static readonly DependencyProperty ToggleButtonMaxHeightProperty =
            DependencyProperty.Register(nameof(ToggleButtonMaxHeight), typeof(double), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(double.NaN));

        public double ToggleButtonMaxHeight
        {
            get => (double)GetValue(ToggleButtonMaxHeightProperty);
            set => SetValue(ToggleButtonMaxHeightProperty, value);
        }

        public static readonly DependencyProperty HeaderMarginProperty =
            DependencyProperty.Register(nameof(HeaderMargin), typeof(Thickness), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new Thickness(5)));

        public Thickness HeaderMargin
        {
            get => (Thickness)GetValue(HeaderMarginProperty);
            set => SetValue(HeaderMarginProperty, value);
        }

        public static readonly DependencyProperty ContentMarginProperty =
            DependencyProperty.Register(nameof(ContentMargin), typeof(Thickness), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(new Thickness(5)));

        public Thickness ContentMargin
        {
            get => (Thickness)GetValue(ContentMarginProperty);
            set => SetValue(ContentMarginProperty, value);
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

        public string DataField
        {
            get => (string)GetValue(DataFieldProperty);
            set => SetValue(DataFieldProperty, value);
        }

        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
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

        public string BoundTo
        {
            get => (string)GetValue(BoundToProperty);
            set => SetValue(BoundToProperty, value);
        }
    }
}
