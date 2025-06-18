using System.Windows;

namespace VideoGui.components
{
    public class BoundToProperty : DependencyObject
    {
        public static readonly DependencyProperty DataFieldProperty =
            DependencyProperty.Register(nameof(DataField), typeof(string), typeof(BoundToProperty), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty BoundToValueProperty =
            DependencyProperty.Register(nameof(BoundTo), typeof(string), typeof(BoundToProperty), new PropertyMetadata(string.Empty));

        public string DataField
        {
            get => (string)GetValue(DataFieldProperty);
            set => SetValue(DataFieldProperty, value);
        }

        public string BoundTo
        {
            get => (string)GetValue(BoundToValueProperty);
            set => SetValue(BoundToValueProperty, value);
        }
    }
}
