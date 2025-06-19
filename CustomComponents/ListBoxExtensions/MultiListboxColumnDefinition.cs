using System.Collections.ObjectModel;
using System.Windows;

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
