using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ToggleClass
{
    public class ToggleButtonEx : ToggleButton
    {
        static ToggleButtonEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButtonEx),
             new FrameworkPropertyMetadata(typeof(ToggleButtonEx)));
        }

        protected override void OnToggle()
        {
            if (!LockToggle)
            {
                base.OnToggle();
                if (AutoLock)
                {
                    LockToggle = !LockToggle;
                }
            }
        }

        public bool LockToggle
        {
            get { return (bool)GetValue(LockToggleProperty); }
            set { SetValue(LockToggleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LockToggle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LockToggleProperty =
            DependencyProperty.Register("LockToggle", typeof(bool),
                typeof(ToggleButtonEx),
                new UIPropertyMetadata(true));
        public bool AutoLock
        {
            get { return (bool)GetValue(AutoLockProperty); }
            set { SetValue(AutoLockProperty, value); }
        }
        public static readonly DependencyProperty AutoLockProperty =
            DependencyProperty.Register("AutoLock", typeof(bool),
                typeof(ToggleButtonEx),
                new UIPropertyMetadata(true));
    }
}
