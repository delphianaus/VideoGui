
using System.Windows.Controls.Primitives;
using System.Windows;

namespace ToggleClass
{
    public class Toggle : ToggleButton
    {
        static Toggle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Toggle), new FrameworkPropertyMetadata(typeof(Toggle)));
        }
    }
}
