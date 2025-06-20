using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace CustomComponents.ListBoxExtensions
{
    public partial class MultiListbox : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region Events
        public event MouseButtonEventHandler ItemMouseDoubleClick;
        public event RoutedEventHandler ItemLostFocus;
        public event RoutedEventHandler ItemGotFocus;
        public event SelectionChangedEventHandler ItemSelectionChanged;
        public event RoutedEventHandler ToggleButtonClick;
        public event RoutedEventHandler DateTimePickerLostFocus;
        public event RoutedEventHandler DateTimePickerGotFocus;
        public event KeyEventHandler DateTimePickerKeyUp;
        public event EventHandler ComboBoxInitialized;
        public event SelectionChangedEventHandler ComboBoxSelectionChanged;
        public event RoutedEventHandler ComboBoxLostFocus;
        public event RoutedEventHandler ComboBoxGotFocus;

        public IList SelectedItems
        {
            get { return lstBoxUploadItems.SelectedItems; }
        }
        #endregion

        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.Register(nameof(ColumnDefinitions), typeof(ObservableCollection<MultiListboxColumnDefinition>),
                typeof(MultiListbox), new PropertyMetadata(null, OnColumnDefinitionsChanged));

        public static readonly DependencyProperty DebugOutputProperty =
            DependencyProperty.Register("DebugOutput", typeof(bool), typeof(MultiListbox),
                new PropertyMetadata(false));

        public bool DebugOutput
        {
            get { return (bool)GetValue(DebugOutputProperty); }
            set { SetValue(DebugOutputProperty, value); }
        }

        public static readonly DependencyProperty BorderMarginProperty =
            DependencyProperty.Register(nameof(BorderMargin), typeof(Thickness), typeof(MultiListbox),
                new PropertyMetadata(new Thickness(0), (d, e) => ((MultiListbox)d).OnBorderMarginChanged()));

        private void OnBorderMarginChanged()
        {
            UpdateAdjustedGroupBoxWidth();
        }

        public Thickness BorderMargin
        {
            get { return (Thickness)GetValue(BorderMarginProperty); }
            set { SetValue(BorderMarginProperty, value); }
        }

        public static readonly DependencyProperty AdjustedWidthProperty =
            DependencyProperty.Register("AdjustedWidth", typeof(double), typeof(MultiListbox),
                new PropertyMetadata(0.0));

        private double _AdjustedWidth;
        public double AdjustedWidth
        {
            get => _AdjustedWidth;
            private set
            {
                if (_AdjustedWidth != value)
                {
                    _AdjustedWidth = value;
                    OnPropertyChanged(nameof(AdjustedWidth));
                }
            }
        }

        public static readonly DependencyProperty AdjustedGroupBoxWidthProperty =
            DependencyProperty.Register("AdjustedGroupBoxWidth", typeof(double), typeof(MultiListbox),
                new PropertyMetadata(0.0));

        private double _AdjustedGroupBoxWidth;
        public double AdjustedGroupBoxWidth
        {
            get => _AdjustedGroupBoxWidth;
            private set
            {
                if (_AdjustedGroupBoxWidth != value)
                {
                    _AdjustedGroupBoxWidth = value;
                    OnPropertyChanged(nameof(AdjustedGroupBoxWidth));
                }
            }
        }

        private void UpdateAdjustedWidth()
        {
            // Subtract border thickness and a small buffer
            if (brdmain != null)
            {
                double borderWidth = brdmain.BorderThickness.Left + brdmain.BorderThickness.Right;
                double marginWidth = BorderMargin.Left + BorderMargin.Right;
                double buffer = 4; // Small buffer to prevent overflow

                AdjustedWidth = Math.Max(0, brdmain.ActualWidth - borderWidth - marginWidth - buffer);
                if (DebugOutput)
                {
                    Debug.WriteLine($"[MultiListbox] Border: {brdmain.ActualWidth}, BorderWidth: {borderWidth}, MarginWidth: {marginWidth}, Buffer: {buffer}, Adjusted: {AdjustedWidth}");
                }
            }
        }

        private void UpdateAdjustedGroupBoxWidth()
        {
            if (ActualWidth > 0)
            {
                var margin = BorderMargin;
                AdjustedGroupBoxWidth = Math.Max(0, ActualWidth - margin.Left - margin.Right - 5);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateAdjustedGroupBoxWidth();
            UpdateItemsListBoxHeight();
        }

        private void UpdateItemsListBoxHeight()
        {
            if (lstBoxUploadItems != null)
            {
                double headerHeight = 30;
                double availableHeight = ActualHeight - headerHeight;
                if (BorderMargin != null)
                {
                    availableHeight -= (BorderMargin.Top + BorderMargin.Bottom);
                }
                lstBoxUploadItems.Height = Math.Max(0, availableHeight);
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(MultiListbox),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty HeaderContextMenuProperty =
            DependencyProperty.Register(nameof(HeaderContextMenu), typeof(ContextMenu), typeof(MultiListbox),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ItemsContextMenuProperty =
            DependencyProperty.Register(nameof(ItemsContextMenu), typeof(ContextMenu), typeof(MultiListbox),
                new PropertyMetadata(null));

        public ContextMenu HeaderContextMenu
        {
            get => (ContextMenu)GetValue(HeaderContextMenuProperty);
            set => SetValue(HeaderContextMenuProperty, value);
        }

        public ContextMenu ItemsContextMenu
        {
            get => (ContextMenu)GetValue(ItemsContextMenuProperty);
            set => SetValue(ItemsContextMenuProperty, value);
        }

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(MultiListbox),
                new PropertyMetadata(Brushes.Black));

        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(nameof(BorderThickness), typeof(double), typeof(MultiListbox),
                new PropertyMetadata(2.0));

        public double BorderThickness
        {
            get => (double)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(MultiListbox),
                new PropertyMetadata(null));

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register(nameof(ColumnWidth), typeof(GridLength), typeof(MultiListbox),
                new PropertyMetadata(new GridLength(0, GridUnitType.Pixel)));

        private Grid _headerGrid;
        private Grid _itemGrid;
        private DataTemplate _cachedItemTemplate;
        private Dictionary<string, FrameworkElementFactory> _columnFactories;
        private ObservableCollection<MultiListboxColumnDefinition> _pendingColumnDefinitions;

        public GridLength ColumnWidth
        {
            get => (GridLength)GetValue(ColumnWidthProperty);
            set => SetValue(ColumnWidthProperty, value);
        }

        private ScrollViewer _headerScroller;
        private ScrollViewer _itemsScroller;
        private bool _isScrolling;

        public MultiListbox()
        {
            InitializeComponent();
            ColumnDefinitions = new ObservableCollection<MultiListboxColumnDefinition>();
            Loaded += MultiListbox_Loaded;
            SizeChanged += MultiListbox_SizeChanged;
            this.DataContext = this;
        }

        private void InitializeScrollViewers()
        {
            if (_headerScroller == null)
            {
                _headerScroller = GetScrollViewer(lstTitles);
                if (_headerScroller != null)
                {
                    _headerScroller.ScrollChanged += ScrollViewer_ScrollChanged;
                }
            }

            if (_itemsScroller == null)
            {
                _itemsScroller = GetScrollViewer(lstBoxUploadItems);
                if (_itemsScroller != null)
                {
                    _itemsScroller.ScrollChanged += ScrollViewer_ScrollChanged;
                }
            }
        }

        private ScrollViewer GetScrollViewer(DependencyObject element)
        {
            if (element == null) return null;
            var child = VisualTreeHelper.GetChild(element, 0);
            if (child == null) return null;
            return child as ScrollViewer ?? GetScrollViewer(child);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!_isScrolling && e.HorizontalChange != 0)
            {
                _isScrolling = true;
                var sourceScroller = sender as ScrollViewer;
                var targetScroller = sourceScroller == _headerScroller ? _itemsScroller : _headerScroller;

                if (targetScroller != null)
                {
                    targetScroller.ScrollToHorizontalOffset(e.HorizontalOffset);
                }
                _isScrolling = false;
            }
        }

        private void MultiListbox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                UpdateAdjustedWidth();
            }
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MultiListbox;
            if (control != null)
            {
                control.lstBoxUploadItems.ItemsSource = e.NewValue as IEnumerable;
            }
        }

        public ObservableCollection<MultiListboxColumnDefinition> ColumnDefinitions
        {
            get => (ObservableCollection<MultiListboxColumnDefinition>)GetValue(ColumnDefinitionsProperty);
            set => SetValue(ColumnDefinitionsProperty, value);
        }

        private void MultiListbox_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeScrollViewers();
                _headerGrid = ((ControlTemplate)lstTitles.Template).FindName("griddpl", lstTitles) as Grid;
                if (_headerGrid == null)
                {
                    Debug.WriteLine("Failed to find header grid");
                    return;
                }

                // Create a new item template with a Grid
                var gridFactory = new FrameworkElementFactory(typeof(Grid));
                gridFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 2, 0, 2));
                gridFactory.SetValue(FrameworkElement.HeightProperty, 30.0);
                gridFactory.SetValue(Panel.BackgroundProperty, Brushes.White);

                // Store reference to the grid factory for later use
                _itemGrid = new Grid(); // Temporary grid just for column definitions

                // Add each column definition and control
                int columnIndex = 0;
                foreach (var colDef in ColumnDefinitions)
                {
                    // Add column definition
                    var colDefFactory = new FrameworkElementFactory(typeof(ColumnDefinition));
                    if (!string.IsNullOrEmpty(colDef.WidthBinding))
                    {
                        var binding = new Binding(colDef.WidthBinding)
                        {
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(MultiListbox), 1)
                        };
                        colDefFactory.SetBinding(ColumnDefinition.WidthProperty, binding);
                    }
                    else
                    {
                        colDefFactory.SetValue(ColumnDefinition.WidthProperty, new GridLength(colDef.Width));
                    }
                    gridFactory.AppendChild(colDefFactory);

                    // Add control for this column if it has a data field
                    if (!string.IsNullOrEmpty(colDef.DataField))
                    {
                        var controlType = GetControlType(colDef.ComponentType.ToString());
                        var controlFactory = new FrameworkElementFactory(controlType);
                        controlFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(5));
                        controlFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
                        controlFactory.SetValue(Grid.ColumnProperty, columnIndex);

                        // Set up the binding
                        var binding = new Binding(colDef.DataField)
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Mode = BindingMode.TwoWay
                        };
                        var mainProperty = GetMainBindingProperty(colDef.ComponentType.ToString(), colDef.BoundTo);
                        if (mainProperty != null)
                        {
                            controlFactory.SetBinding(mainProperty, binding);
                        }

                        gridFactory.AppendChild(controlFactory);
                    }

                    columnIndex++;
                }

                // Create and set the item template
                var itemTemplate = new DataTemplate { VisualTree = gridFactory };
                lstBoxUploadItems.ItemTemplate = itemTemplate;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MultiListbox_Loaded: {ex}");
            }

            // Apply any pending column definitions
            if (_pendingColumnDefinitions != null)
            {
                ColumnDefinitions = _pendingColumnDefinitions;
                _pendingColumnDefinitions = null;
            }

            ApplyColumnDefinitions();
        }

        private static void OnColumnDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiListbox multiListbox)
            {
                multiListbox._columnFactories = null; // Force rebuild of factories
                multiListbox._cachedItemTemplate = null; // Force rebuild of template
                multiListbox.ApplyColumnDefinitions();
            }
        }

        private Type GetControlType(string componentType)
        {
            switch (componentType?.ToLower())
            {
                case "textbox":
                    return typeof(TextBox);
                case "combobox":
                    return typeof(ComboBox);
                case "checkbox":
                    return typeof(CheckBox);
                case "textblock":
                    return typeof(TextBlock);
                case "label":
                    return typeof(Label);
                case "button":
                    return typeof(Button);
                case "image":
                    return typeof(System.Windows.Controls.Image);
                case "progressbar":
                    return typeof(ProgressBar);
                case "passwordbox":
                    return typeof(PasswordBox);
                case "timepicker":
                    return typeof(TimePicker);
                case "datepicker":
                    return typeof(DatePicker);
                case "datetimepicker":
                    return typeof(DateTimePicker);
                case "togglebutton":
                    return typeof(ToggleButton);
                case "slider":
                    return typeof(Slider);
                default:
                    return typeof(TextBlock);
            }
        }

        private DependencyProperty GetDependencyPropertyByName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;

            // Map of control types to check for the property
            Type[] typesToCheck = new Type[]
            {
                typeof(TextBox),
                typeof(TextBlock),
                typeof(ComboBox),
                typeof(CheckBox),
                typeof(Label),
                typeof(Button),
                typeof(System.Windows.Controls.Image),
                typeof(ProgressBar),
                typeof(PasswordBox),
                typeof(TimePicker),
                typeof(DatePicker),
                typeof(DateTimePicker),
                typeof(ToggleButton),
                typeof(Slider),
                typeof(FrameworkElement) // Common base properties
            };

            foreach (var type in typesToCheck)
            {
                var fieldName = $"{propertyName}Property";
                var field = type.GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy);
                if (field != null)
                {
                    return field.GetValue(null) as DependencyProperty;
                }
            }

            return null;
        }

        private DependencyProperty GetMainBindingProperty(string componentType, string boundTo = null)
        {
            if (!string.IsNullOrEmpty(boundTo))
            {
                // Get the control type
                Type type = componentType?.ToLower() switch
                {
                    "textbox" => typeof(TextBox),
                    "combobox" => typeof(ComboBox),
                    "checkbox" => typeof(CheckBox),
                    "label" => typeof(Label),
                    "textblock" => typeof(TextBlock),
                    "button" => typeof(Button),
                    "progressbar" => typeof(ProgressBar),
                    "passwordbox" => typeof(PasswordBox),
                    "image" => typeof(System.Windows.Controls.Image),
                    "timepicker" => typeof(TimePicker),
                    "datepicker" => typeof(DatePicker),
                    "datetimepicker" => typeof(DateTimePicker),
                    "togglebutton" => typeof(ToggleButton),
                    "slider" => typeof(Slider),
                    _ => null
                };

                if (type != null)
                {
                    // Get the DependencyProperty field by name
                    var fieldName = $"{boundTo}Property";
                    var field = type.GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    return field?.GetValue(null) as DependencyProperty;
                }
            }

            // Fall back to defaults if BoundTo is not specified or invalid
            switch (componentType?.ToLower())
            {
                case "textbox":
                    return TextBox.TextProperty;
                case "combobox":
                    return ComboBox.SelectedItemProperty;
                case "checkbox":
                    return CheckBox.IsCheckedProperty;
                case "label":
                    return Label.ContentProperty;
                case "button":
                    return Button.ContentProperty;
                case "progressbar":
                    return ProgressBar.ValueProperty;
                case "image":
                    return System.Windows.Controls.Image.SourceProperty;
                case "textblock":
                    return TextBlock.TextProperty;
                case "passwordbox":
                    return PasswordBox.PasswordCharProperty;
                case "timepicker":
                    return TimePicker.ValueProperty;
                case "datepicker":
                    return DatePicker.SelectedDateProperty;
                case "datetimepicker":
                    return DateTimePicker.ValueProperty;
                case "togglebutton":
                    return ToggleButton.IsCheckedProperty;
                case "slider":
                    return Slider.ValueProperty;
                default:
                    return GetDependencyPropertyByName(boundTo);
            }
        }

        private void SaveDebugXaml()
        {
            if (!DebugOutput) return;

            try
            {
                var debugPath = System.IO.Path.Combine(
                    Debugger.IsAttached ? @"C:\YouTubeHelper" : AppDomain.CurrentDomain.BaseDirectory,
                    $"MultiListbox_Debug_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                );

                using (var writer = System.IO.File.CreateText(debugPath))
                {
                    writer.WriteLine("<Grid xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'");
                    writer.WriteLine("      xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'");
                    writer.WriteLine("      xmlns:local='clr-namespace:YouTubeHelper.components'>");

                    // Header Grid
                    writer.WriteLine("  <!-- Header Grid -->");
                    writer.WriteLine("  <Grid x:Name='HeaderGrid'>");
                    writer.WriteLine("    <Grid.ColumnDefinitions>");
                    foreach (var col in _headerGrid.ColumnDefinitions)
                    {
                        var binding = BindingOperations.GetBinding(col, ColumnDefinition.WidthProperty);
                        if (binding != null)
                        {
                            var source = (binding.RelativeSource?.AncestorType?.Name ?? "none");
                            writer.WriteLine($"      <ColumnDefinition Width='{{Binding {binding.Path.Path}, RelativeSource={{RelativeSource AncestorType={source}}}}}' />");
                        }
                        else
                        {
                            writer.WriteLine($"      <ColumnDefinition Width='{col.Width.Value}' />");
                        }
                    }
                    writer.WriteLine("    </Grid.ColumnDefinitions>");
                    foreach (FrameworkElement child in _headerGrid.Children)
                    {
                        writer.WriteLine($"    <TextBlock Text='{(child as TextBlock)?.Text}' Grid.Column='{Grid.GetColumn(child)}' />");
                    }
                    writer.WriteLine("  </Grid>");

                    // Item Grid Template
                    writer.WriteLine("\n  <!-- Item Grid Template -->");
                    writer.WriteLine("  <Grid x:Name='ItemGrid'>");
                    writer.WriteLine("    <Grid.ColumnDefinitions>");
                    foreach (var col in _itemGrid.ColumnDefinitions)
                    {
                        var binding = BindingOperations.GetBinding(col, ColumnDefinition.WidthProperty);
                        if (binding != null)
                        {
                            var source = (binding.RelativeSource?.AncestorType?.Name ?? "none");
                            writer.WriteLine($"      <ColumnDefinition Width='{{Binding {binding.Path.Path}, RelativeSource={{RelativeSource AncestorType={source}}}}}' />");
                        }
                        else
                        {
                            writer.WriteLine($"      <ColumnDefinition Width='{col.Width.Value}' />");
                        }
                    }
                    writer.WriteLine("    </Grid.ColumnDefinitions>");

                    writer.WriteLine("\n    <!-- Item Template Controls -->");
                    var itemTemplate = lstBoxUploadItems.ItemTemplate;
                    var gridFactory = itemTemplate?.VisualTree as FrameworkElementFactory;
                    if (gridFactory != null)
                    {
                        for (int i = 0; i < ColumnDefinitions.Count; i++)
                        {
                            var colDef = ColumnDefinitions[i];
                            if (!string.IsNullOrEmpty(colDef.DataField))
                            {
                                var controlType = GetControlType(colDef.ComponentType.ToString());
                                writer.WriteLine($"    <!-- Column {i} Control -->");
                                writer.WriteLine($"    <{controlType.Name} Grid.Column='{i}' Text='{{Binding {colDef.DataField}}}' />");
                            }
                        }
                    }
                    writer.WriteLine("  </Grid>");
                    writer.WriteLine("</Grid>");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Debug XAML save failed: {ex.Message}");
            }
        }

        private void BuildColumnFactories()
        {
            _columnFactories = new Dictionary<string, FrameworkElementFactory>();
            if (ColumnDefinitions == null) return;

            foreach (var colDef in ColumnDefinitions)
            {
                var factory = CreateColumnFactory(colDef);
                if (factory != null)
                {
                    _columnFactories[colDef.DataField] = factory;
                }
            }
        }

        private FrameworkElementFactory CreateColumnFactory(MultiListboxColumnDefinition colDef)
        {
            var controlType = GetControlType(colDef.ComponentType.ToString());
            var factory = new FrameworkElementFactory(controlType);
            factory.SetValue(FrameworkElement.MarginProperty, new Thickness(5));
            factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);

            // Apply ToggleButton properties if this is a ToggleButton
            if (controlType == typeof(ToggleButton))
            {
                if (colDef.ToggleButtonStyle != null)
                {
                    factory.SetValue(ToggleButton.StyleProperty, colDef.ToggleButtonStyle);
                }
                if (!double.IsNaN(colDef.ToggleButtonWidth))
                {
                    factory.SetValue(FrameworkElement.WidthProperty, colDef.ToggleButtonWidth);
                }
                if (!double.IsNaN(colDef.ToggleButtonHeight))
                {
                    factory.SetValue(FrameworkElement.HeightProperty, colDef.ToggleButtonHeight);
                }
                factory.AddHandler(ToggleButton.ClickEvent, new RoutedEventHandler((s, e) => ToggleButtonClick?.Invoke(s, e)));
            }

            // Set up the main binding
            var binding = new Binding(colDef.DataField)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            var mainProperty = GetMainBindingProperty(colDef.ComponentType.ToString(), colDef.BoundTo);
            if (mainProperty != null)
            {
                factory.SetBinding(mainProperty, binding);
            }

            // Set up additional bindings from BoundToProperties
            if (colDef.BoundToProperties != null)
            {
                foreach (var boundTo in colDef.BoundToProperties)
                {
                    var additionalBinding = new Binding(boundTo.DataField)
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    };
                    var property = GetDependencyPropertyByName(boundTo.BoundTo);
                    if (property != null)
                    {
                        factory.SetBinding(property, additionalBinding);
                    }
                }
            }

            return factory;
        }

        private void BuildItemTemplate()
        {
            if (ColumnDefinitions == null) return;

            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            gridFactory.Name = "griddpl";
            gridFactory.SetValue(Grid.BackgroundProperty, Brushes.White);

            // Add column definitions
            int columnIndex = 0;
            foreach (var colDef in ColumnDefinitions)
            {
                // Add column definition
                var colDefFactory = new FrameworkElementFactory(typeof(ColumnDefinition));
                colDefFactory.SetValue(ColumnDefinition.WidthProperty, new GridLength(colDef.Width));
                gridFactory.AppendChild(colDefFactory);

                // Create control for this column
                var controlType = GetControlType(colDef.ComponentType.ToString());
                var factory = new FrameworkElementFactory(controlType);
                factory.SetValue(FrameworkElement.MarginProperty, new Thickness(5));
                factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
                factory.SetValue(Grid.ColumnProperty, columnIndex);

                // Apply ToggleButton properties if this is a ToggleButton
                if (controlType == typeof(ToggleButton))
                {
                    if (colDef.ToggleButtonStyle != null)
                    {
                        factory.SetValue(ToggleButton.StyleProperty, colDef.ToggleButtonStyle);
                    }
                    if (!double.IsNaN(colDef.ToggleButtonWidth))
                    {
                        factory.SetValue(FrameworkElement.WidthProperty, colDef.ToggleButtonWidth);
                    }
                    if (!double.IsNaN(colDef.ToggleButtonHeight))
                    {
                        factory.SetValue(FrameworkElement.HeightProperty, colDef.ToggleButtonHeight);
                    }
                    factory.AddHandler(ToggleButton.ClickEvent, new RoutedEventHandler((s, e) => ToggleButtonClick?.Invoke(s, e)));
                }

                // Set up the main binding
                var binding = new Binding(colDef.DataField)
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                };
                var mainProperty = GetMainBindingProperty(colDef.ComponentType.ToString(), colDef.BoundTo);
                if (mainProperty != null)
                {
                    factory.SetBinding(mainProperty, binding);
                }

                // Set up additional bindings from BoundToProperties
                if (colDef.BoundToProperties != null)
                {
                    foreach (var boundTo in colDef.BoundToProperties)
                    {
                        var additionalBinding = new Binding(boundTo.DataField)
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Mode = BindingMode.TwoWay
                        };
                        var property = GetDependencyPropertyByName(boundTo.BoundTo);
                        if (property != null)
                        {
                            factory.SetBinding(property, additionalBinding);
                        }
                    }
                }

                gridFactory.AppendChild(factory);
                columnIndex++;
            }

            _cachedItemTemplate = new DataTemplate { VisualTree = gridFactory };
        }

        private void ApplyColumnDefinitions()
        {
            try
            {
                if (_headerGrid == null || _itemGrid == null) return;

                _headerGrid.ColumnDefinitions.Clear();
                _headerGrid.Children.Clear();
                _itemGrid.ColumnDefinitions.Clear();
                foreach (var colDef in ColumnDefinitions)
                {
                    // Only add to header grid if there's a header text
                    if (!string.IsNullOrEmpty(colDef.HeaderText))
                    {
                        var headerColumn = new System.Windows.Controls.ColumnDefinition();
                        if (!string.IsNullOrEmpty(colDef.WidthBinding))
                        {
                            var binding = new Binding(colDef.WidthBinding)
                            {
                                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(MultiListbox), 1)
                            };
                            BindingOperations.SetBinding(headerColumn, System.Windows.Controls.ColumnDefinition.WidthProperty, binding);
                        }
                        else
                        {
                            headerColumn.Width = new GridLength(colDef.Width);
                        }
                        _headerGrid.ColumnDefinitions.Add(headerColumn);

                        var headerTextBlock = new TextBlock
                        {
                            Text = colDef.HeaderText,
                            Margin = colDef.HeaderMargin,
                            HorizontalAlignment = colDef.HeaderHorizontalAlignment,
                            VerticalAlignment = colDef.HeaderVerticalAlignment
                        };
                        Grid.SetColumn(headerTextBlock, _headerGrid.ColumnDefinitions.Count - 1);
                        _headerGrid.Children.Add(headerTextBlock);
                    }

                    // Add column to item template grid
                    var itemColumn = new System.Windows.Controls.ColumnDefinition();
                    if (!string.IsNullOrEmpty(colDef.WidthBinding))
                    {
                        var binding = new Binding(colDef.WidthBinding)
                        {
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(MultiListbox), 1)
                        };
                        BindingOperations.SetBinding(itemColumn, System.Windows.Controls.ColumnDefinition.WidthProperty, binding);
                    }
                    else
                    {
                        itemColumn.Width = new GridLength(colDef.Width);
                    }
                    _itemGrid.ColumnDefinitions.Add(itemColumn);

                    if (!string.IsNullOrEmpty(colDef.DataField))
                    {
                        var controlType = GetControlType(colDef.ComponentType.ToString());
                        var factory = new FrameworkElementFactory(controlType);
                        factory.SetValue(FrameworkElement.MarginProperty, colDef.ContentMargin);
                        factory.SetValue(FrameworkElement.HorizontalAlignmentProperty, colDef.ContentHorizontalAlignment);
                        factory.SetValue(FrameworkElement.VerticalAlignmentProperty, colDef.ContentVerticalAlignment);
                        factory.SetValue(Grid.ColumnProperty, _itemGrid.ColumnDefinitions.Count - 1);

                        // Apply ToggleButton properties if this is a ToggleButton
                        if (controlType == typeof(ToggleButton))
                        {
                            if (colDef.ToggleButtonStyle != null)
                            {
                                factory.SetValue(ToggleButton.StyleProperty, colDef.ToggleButtonStyle);
                            }
                            if (!double.IsNaN(colDef.ToggleButtonWidth))
                            {
                                factory.SetValue(FrameworkElement.WidthProperty, colDef.ToggleButtonWidth);
                            }
                            if (!double.IsNaN(colDef.ToggleButtonHeight))
                            {
                                factory.SetValue(FrameworkElement.HeightProperty, colDef.ToggleButtonHeight);
                            }
                            factory.AddHandler(ToggleButton.ClickEvent, new RoutedEventHandler((s, e) => ToggleButtonClick?.Invoke(s, e)));
                        }
                        else if (controlType == typeof(CheckBox))
                        {
                            factory.SetValue(CheckBox.VerticalContentAlignmentProperty, colDef.CheckBoxVerticalContentAlignment);
                            factory.SetValue(CheckBox.HorizontalContentAlignmentProperty, colDef.CheckBoxHorizontalContentAlignment);
                            factory.SetValue(FrameworkElement.MarginProperty, colDef.CheckBoxMargin);
                            factory.SetValue(UIElement.IsManipulationEnabledProperty, colDef.CheckBoxIsManipulationEnabled);
                            factory.SetValue(UIElement.FocusableProperty, colDef.CheckBoxFocusable);
                        }
                        else if (controlType == typeof(Button))
                        {
                            if (colDef.ButtonImageSource != null)
                            {
                                var image = new FrameworkElementFactory(typeof(Image));
                                image.SetValue(Image.SourceProperty, colDef.ButtonImageSource);
                                image.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                                image.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
                                factory.SetValue(Button.ContentProperty, image);
                            }
                        }
                        else if (controlType == typeof(ComboBox))
                        {
                            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) => ComboBoxInitialized?.Invoke(s, e)));
                            factory.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler((s, e) => ComboBoxSelectionChanged?.Invoke(s, e)));
                            factory.AddHandler(ComboBox.LostFocusEvent, new RoutedEventHandler((s, e) => ComboBoxLostFocus?.Invoke(s, e)));
                            factory.AddHandler(ComboBox.GotFocusEvent, new RoutedEventHandler((s, e) => ComboBoxGotFocus?.Invoke(s, e)));
                        }
                        else if (controlType == typeof(DateTimePicker))
                        {
                            if (!string.IsNullOrEmpty(colDef.DateTimeFormat))
                            {
                                factory.SetValue(DateTimePicker.FormatProperty, colDef.DateTimeFormat);
                            }
                            if (!string.IsNullOrEmpty(colDef.DateTimeFormatString))
                            {
                                factory.SetValue(DateTimePicker.FormatStringProperty, colDef.DateTimeFormatString);
                            }
                            if (!string.IsNullOrEmpty(colDef.DateTimeTimeFormat))
                            {
                                factory.SetValue(DateTimePicker.TimeFormatProperty, colDef.DateTimeTimeFormat);
                            }
                            if (!string.IsNullOrEmpty(colDef.DateTimeTimeFormatString))
                            {
                                factory.SetValue(DateTimePicker.TimeFormatStringProperty, colDef.DateTimeTimeFormatString);
                            }
                            if (!double.IsNaN(colDef.DateTimeMinWidth))
                            {
                                factory.SetValue(FrameworkElement.MinWidthProperty, colDef.DateTimeMinWidth);
                            }
                            factory.SetValue(UIElement.FocusableProperty, colDef.DateTimeFocusable);
                            
                            factory.AddHandler(DateTimePicker.LostFocusEvent, new RoutedEventHandler((s, e) => DateTimePickerLostFocus?.Invoke(s, e)));
                            factory.AddHandler(DateTimePicker.GotFocusEvent, new RoutedEventHandler((s, e) => DateTimePickerGotFocus?.Invoke(s, e)));
                            factory.AddHandler(DateTimePicker.KeyUpEvent, new KeyEventHandler((s, e) => DateTimePickerKeyUp?.Invoke(s, e)));
                        }

                        // Set up the main binding
                        var binding = new Binding(colDef.DataField)
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Mode = BindingMode.TwoWay
                        };
                        var mainProperty = GetMainBindingProperty(colDef.ComponentType.ToString(), colDef.BoundTo);
                        if (mainProperty != null)
                        {
                            factory.SetBinding(mainProperty, binding);
                        }

                        // Set up additional bindings from BoundToProperties
                        foreach (var boundTo in colDef.BoundToProperties)
                        {
                            var additionalBinding = new Binding(boundTo.DataField)
                            {
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                Mode = BindingMode.TwoWay
                            };
                            var additionalProperty = GetMainBindingProperty(colDef.ComponentType.ToString(), boundTo.BoundTo);
                            if (additionalProperty != null)
                            {
                                factory.SetBinding(additionalProperty, additionalBinding);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ApplyColumnDefinitions: {ex.Message}");
            }
        }



        #region Event Handlers
        private void lstBoxUploadItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemMouseDoubleClick?.Invoke(sender, e);
        }

        private void lstBoxUploadItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemSelectionChanged?.Invoke(sender, e);
        }

        private void lstBoxUploadItems_LostFocus(object sender, RoutedEventArgs e)
        {
            ItemLostFocus?.Invoke(sender, e);
        }

        private void lstBoxUploadItems_GotFocus(object sender, RoutedEventArgs e)
        {
            ItemGotFocus?.Invoke(sender, e);
        }
        #endregion
    }
}
