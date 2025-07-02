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
using System.Windows.Shapes;
using System.Windows.Threading;
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
        public event RoutedEventHandler Click, LostFocus, GotFocus;
        public event TextChangedEventHandler TextChanged;
        public event SelectionChangedEventHandler SelectionChanged;
        public event EventHandler Initialized;


        /// <summary>
        /// Gets the ItemContainerGenerator for the internal ListBox.
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator
        {
            get { return lstBoxUploadItems.ItemContainerGenerator; }
        }
        public event RoutedEventHandler DateTimePickerLostFocus;
        public event RoutedEventHandler DateTimePickerGotFocus;
        public event KeyEventHandler DateTimePickerKeyUp;
        public event EventHandler ComboBoxInitialized;
        public event SelectionChangedEventHandler ComboBoxSelectionChanged;



        public IList Items
        {
            get { return lstBoxUploadItems.Items; }
        }

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

                // Clear existing header content
                _headerGrid.ColumnDefinitions.Clear();
                _headerGrid.Children.Clear();

                // Create a new item template with a Grid
                var gridFactory = new FrameworkElementFactory(typeof(Grid));
                gridFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));
                gridFactory.SetValue(FrameworkElement.HeightProperty, 35.0);
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

                        // Add matching column definition to header grid
                        var headerColDef = new ColumnDefinition();
                        headerColDef.SetBinding(ColumnDefinition.WidthProperty, binding);
                        _headerGrid.ColumnDefinitions.Add(headerColDef);
                    }
                    else
                    {
                        colDefFactory.SetValue(ColumnDefinition.WidthProperty, new GridLength(colDef.Width));

                        // Add matching column definition to header grid
                        _headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(colDef.Width) });
                    }
                    gridFactory.AppendChild(colDefFactory);

                    // Add header text
                    var headerTextBlock = new TextBlock
                    {
                        Text = colDef.HeaderText,
                        Margin = colDef.HeaderMargin,
                        Padding = colDef.HeaderPadding,
                        VerticalAlignment = colDef.HeaderVerticalAlignment,
                        HorizontalAlignment = colDef.HeaderHorizontalAlignment
                    };
                    Grid.SetColumn(headerTextBlock, columnIndex);
                    _headerGrid?.Children.Add(headerTextBlock);

                    // Add control for this column if it has a data field
                    if (!string.IsNullOrEmpty(colDef.DataField))
                    {
                        var controlType = GetControlType(colDef.ComponentType.ToString());
                        var controlFactory = new FrameworkElementFactory(controlType);
                        controlFactory.SetValue(Grid.ColumnProperty, columnIndex);
                        controlFactory.SetValue(FrameworkElement.MarginProperty, colDef.ContentMargin);
                        controlFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, colDef.ContentVerticalAlignment);
                        controlFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, colDef.ContentHorizontalAlignment);

                        // Handle specific control types
                        switch (colDef.ComponentType.ToString().ToLower())
                        {
                            case "togglebutton":
                                var toggleBinding = new Binding(colDef.DataField)
                                {
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                    Mode = BindingMode.TwoWay
                                };
                                controlFactory.SetBinding(ToggleButton.IsCheckedProperty, toggleBinding);
                                controlFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                                controlFactory.SetValue(FrameworkElement.WidthProperty, colDef.ToggleButtonWidth);
                                controlFactory.SetValue(FrameworkElement.HeightProperty, colDef.ToggleButtonHeight);
                                if (colDef.Style != null && colDef.Style.TargetType == typeof(ToggleButton))
                                {
                                    controlFactory.SetValue(FrameworkElement.StyleProperty, colDef.Style);
                                }
                                break;
                            case "checkbox":
                                controlFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding(colDef.DataField));
                                break;
                            default:
                                var mainProperty = GetMainBindingProperty(colDef.ComponentType.ToString(), colDef.BoundTo);
                                if (mainProperty != null)
                                {
                                    controlFactory.SetBinding(mainProperty, new Binding(colDef.DataField));

                                    // For TextBlocks, set TextAlignment based on ContentHorizontalAlignment
                                    if (controlType == typeof(TextBlock))
                                    {
                                        // Set horizontal text alignment
                                        var textAlignment = colDef.ContentHorizontalAlignment switch
                                        {
                                            HorizontalAlignment.Center => TextAlignment.Center,
                                            HorizontalAlignment.Right => TextAlignment.Right,
                                            _ => TextAlignment.Left
                                        };
                                        controlFactory.SetValue(TextBlock.TextAlignmentProperty, textAlignment);

                                        // Set vertical text alignment
                                        var verticalTextAlignment = colDef.ContentVerticalAlignment switch
                                        {
                                            VerticalAlignment.Top => VerticalAlignment.Top,
                                            VerticalAlignment.Bottom => VerticalAlignment.Bottom,
                                            _ => VerticalAlignment.Center
                                        };
                                        controlFactory.SetValue(TextBlock.LineStackingStrategyProperty, LineStackingStrategy.BlockLineHeight);
                                        controlFactory.SetValue(TextBlock.VerticalAlignmentProperty, verticalTextAlignment);
                                    }
                                }
                                break;
                        }

                        gridFactory.AppendChild(controlFactory);
                    }

                    columnIndex++;
                }

                // Create and set the item template
                var itemTemplate = new DataTemplate { VisualTree = gridFactory };
                lstBoxUploadItems.ItemTemplate = itemTemplate;

                // Apply any pending column definitions
                if (_pendingColumnDefinitions != null)
                {
                    ColumnDefinitions = _pendingColumnDefinitions;
                    _pendingColumnDefinitions = null;
                }

                ApplyColumnDefinitions();

                // Give the template a chance to apply
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (var item in lstBoxUploadItems.Items)
                    {
                        var container = lstBoxUploadItems.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                        if (container != null)
                        {
                            var border = VisualTreeHelper.GetChild(container, 0) as Border;
                            if (border != null)
                            {
                                var contentPresenter = VisualTreeHelper.GetChild(border, 0) as ContentPresenter;
                                if (contentPresenter != null)
                                {
                                    var grid = VisualTreeHelper.GetChild(contentPresenter, 0) as Grid;
                                    if (grid != null)
                                    {
                                        foreach (var child in grid.Children)
                                        {
                                            if (child is ToggleButton toggleButton)
                                            {
                                                var column = Grid.GetColumn(toggleButton);
                                                var colDef = ColumnDefinitions[column];
                                                if (colDef != null)
                                                {
                                                    // Set dimensions and style
                                                    toggleButton.Width = colDef.ToggleButtonWidth;
                                                    toggleButton.Height = colDef.ToggleButtonHeight;
                                                    toggleButton.MinWidth = colDef.ToggleButtonMinWidth;
                                                    toggleButton.MinHeight = colDef.ToggleButtonMinHeight;
                                                    toggleButton.MaxWidth = colDef.ToggleButtonMaxWidth;
                                                    toggleButton.MaxHeight = colDef.ToggleButtonMaxHeight;
                                                    toggleButton.HorizontalAlignment = HorizontalAlignment.Center;
                                                    toggleButton.VerticalAlignment = VerticalAlignment.Center;
                                                    toggleButton.Visibility = Visibility.Visible;
                                                    toggleButton.Style = (colDef?.Style is not null && colDef.Style.TargetType == typeof(ToggleButton)) ? colDef.Style : null;

                                                    HandleClickEvents<ToggleButton>(toggleButton, colDef);

                                                    // Set up binding
                                                    var binding = new Binding(colDef.DataField)
                                                    {
                                                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                                        Mode = BindingMode.TwoWay
                                                    };
                                                    BindingOperations.SetBinding(toggleButton, ToggleButton.IsCheckedProperty, binding);
                                                }
                                            }
                                            else if (child is TextBox textBox)
                                            {
                                                var column = Grid.GetColumn(textBox);
                                                var colDef = ColumnDefinitions[column];
                                                HandleInitialized<TextBox>(textBox, colDef);
                                                HandleFocusEvents<TextBox>(textBox, colDef);
                                                SetCustomBindings<TextBox>(textBox, colDef);
                                            }
                                            else if (child is ComboBox comboBox)
                                            {
                                                var column = Grid.GetColumn(comboBox);
                                                var colDef = ColumnDefinitions[column];

                                                HandleInitialized<ComboBox>(comboBox, colDef);
                                                HandleSelectionChangedEvent<ComboBox>(comboBox, colDef);
                                                HandleFocusEvents<ComboBox>(comboBox, colDef);
                                                SetCustomBindings<ComboBox>(comboBox, colDef);
                                            }
                                            else if (child is CheckBox checkBox)
                                            {
                                                var column = Grid.GetColumn(checkBox);
                                                var colDef = ColumnDefinitions[column];
                                                HandleClickEvents<CheckBox>(checkBox, colDef);  
                                                SetCustomBindings<CheckBox>(checkBox, colDef);
                                            }
                                            else if (child is TextBlock textBlock)
                                            {
                                                var column = Grid.GetColumn(textBlock);
                                                var colDef = ColumnDefinitions[column];
                                                HandleFocusEvents<TextBlock>(textBlock, colDef);
                                                SetCustomBindings<TextBlock>(textBlock, colDef);
                                            }
                                            else if (child is Label label)
                                            {
                                                var column = Grid.GetColumn(label);
                                                var colDef = ColumnDefinitions[column];

                                                SetCustomBindings<Label>(label, colDef);
                                            }
                                            else if (child is Button button)
                                            {
                                                var column = Grid.GetColumn(button);
                                                var colDef = ColumnDefinitions[column];
                                                button.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                                                button.VerticalAlignment = colDef.ContentVerticalAlignment;
                                                HandleClickEvents<Button>(button, colDef);
                                                SetCustomBindings<Button>(button, colDef);
                                            }
                                            else if (child is System.Windows.Controls.Image image)
                                            {
                                                var column = Grid.GetColumn(image);
                                                var colDef = ColumnDefinitions[column];

                                                if (colDef != null)
                                                {
                                                    image.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                                                    image.VerticalAlignment = colDef.ContentVerticalAlignment;
                                                    HandleInitialized<Image>(image, colDef);
                                                    SetCustomBindings<Image>(image, colDef);
                                                }
                                            }
                                            else if (child is ProgressBar progressBar)
                                            {
                                                var column = Grid.GetColumn(progressBar);
                                                var colDef = ColumnDefinitions[column];

                                                if (colDef != null)
                                                {
                                                    progressBar.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                                                    progressBar.VerticalAlignment = colDef.ContentVerticalAlignment;
                                                    SetCustomBindings<ProgressBar>(progressBar, colDef);
                                                }
                                            }
                                            else if (child is Slider slider)
                                            {
                                                var column = Grid.GetColumn(slider);
                                                var colDef = ColumnDefinitions[column];

                                                if (colDef != null)
                                                {
                                                    slider.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                                                    slider.VerticalAlignment = colDef.ContentVerticalAlignment;
                                                    SetCustomBindings<Slider>(slider, colDef);
                                                }
                                            }
                                            else if (child is PasswordBox passwordBox)
                                            {
                                                var column = Grid.GetColumn(passwordBox);
                                                var colDef = ColumnDefinitions[column];
                                                HandleFocusEvents<PasswordBox>(passwordBox, colDef);
                                                if (colDef != null)
                                                {
                                                    passwordBox.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                                                    passwordBox.VerticalAlignment = colDef.ContentVerticalAlignment;
                                                    SetCustomBindings<PasswordBox>(passwordBox, colDef);
                                                }
                                            }
                                            else if (child is TimePicker timePicker)
                                            {
                                                var column = Grid.GetColumn(timePicker);
                                                var colDef = ColumnDefinitions[column];

                                                if (colDef != null)
                                                {
                                                    timePicker.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                                                    timePicker.VerticalAlignment = colDef.ContentVerticalAlignment;
                                                    SetCustomBindings<TimePicker>(timePicker, colDef);
                                                }
                                            }
                                            else if (child is DatePicker datePicker)
                                            {
                                                var column = Grid.GetColumn(datePicker);
                                                var colDef = ColumnDefinitions[column];

                                                if (colDef != null)
                                                {
                                                    datePicker.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                                                    datePicker.VerticalAlignment = colDef.ContentVerticalAlignment;
                                                    SetCustomBindings<DatePicker>(datePicker, colDef);
                                                }
                                            }
                                            else if (child is DateTimePicker dateTimePicker)
                                            {
                                                var column = Grid.GetColumn(dateTimePicker);
                                                var colDef = ColumnDefinitions[column];
                                                HandleFocusEvents<DateTimePicker>(dateTimePicker, colDef);
                                                SetCustomBindings<DateTimePicker>(dateTimePicker, colDef);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }), DispatcherPriority.Loaded);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MultiListbox_Loaded: {ex}");
            }


        }

        private void HandleSelectionChangedEvent<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            if (control is ComboBox b)
            {
                // Remove any existing handlers
                b.SelectionChanged -= (s, e) => SelectionChanged?.Invoke(s, e);
                b.SelectionChanged -= (s, e) => colDef.RaiseSelectionChanged(s, e);

                // Add new handler
                b.SelectionChanged += (s, e) =>
                {
                    colDef.RaiseSelectionChanged(s, e);
                    SelectionChanged?.Invoke(s, e);
                };
            }
        }
        private void HandleClickEvents<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            if (control is Button b)
            {
                b.Click += (s, e) => colDef.RaiseClick(s, e);
                b.Click -= (s, e) => Click?.Invoke(s, e);
                b.Click += (s, e) =>
                {
                    colDef.RaiseClick(s, e);
                    Click?.Invoke(s, e);
                };
            }
            else if (control is CheckBox c)
            {
                c.Click += (s, e) => colDef.RaiseClick(s, e);
                c.Click -= (s, e) => Click?.Invoke(s, e);
                c.Click += (s, e) =>
                {
                    colDef.RaiseClick(s, e);
                    Click?.Invoke(s, e);
                };
            }
            else if (control is ToggleButton r)
            {
                r.Click += (s, e) => colDef.RaiseClick(s, e);
                r.Click -= (s, e) => Click?.Invoke(s, e);
                r.Click += (s, e) =>
                {
                    colDef.RaiseClick(s, e);
                    Click?.Invoke(s, e);
                };
            }
        }

        private void HandleInitialized<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            // Remove any existing handlers
            control.Initialized -= (s, e) => Initialized?.Invoke(s, e);
            control.Initialized -= (s, e) => colDef.RaiseInitializedEvent(s, e);

            // Add new handler
            control.Initialized += (s, e) => {
                colDef.RaiseInitializedEvent(s, e);
                Initialized?.Invoke(s, e);
            };
        }

        private void HandleFocusEvents<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            // Remove any existing handlers
            control.LostFocus -= (s, e) => LostFocus?.Invoke(s, e);
            control.LostFocus -= (s, e) => colDef.RaiseLostFocusEvent(s, e);
            control.GotFocus -= (s, e) => GotFocus?.Invoke(s, e);
            control.GotFocus -= (s, e) => colDef.RaiseGotFocusEvent(s, e);

            // Add new handlers
            control.LostFocus += (s, e) =>
            {
                colDef.RaiseLostFocusEvent(s, e);
                LostFocus?.Invoke(s, e);
            };
            control.GotFocus += (s, e) =>
            {
                colDef.RaiseGotFocusEvent(s, e);
                GotFocus?.Invoke(s, e);
            };
        }

        private void SetCustomBindings<T>(T control, MultiListboxColumnDefinition? colDef) where T : FrameworkElement
        {
            try
            {
                if (colDef == null) return;

                // Apply layout properties
                if (control is ToggleButton toggleButton1)
                {
                    // For ToggleButton, only set its specific dimensions
                    control.Width = colDef.ToggleButtonWidth;
                    control.Height = colDef.ToggleButtonHeight;
                    control.HorizontalAlignment = HorizontalAlignment.Left;

                    // Only apply style if it's meant for ToggleButton
                    if (colDef.Style != null && colDef.Style.TargetType == typeof(ToggleButton))
                    {
                        control.Style = colDef.Style;
                    }
                }
                else
                {
                    // For other controls, use the standard dimensions
                    control.Width = colDef.Width;
                    control.Height = colDef.Height;
                    control.MinWidth = colDef.MinWidth;
                    control.MaxWidth = colDef.MaxWidth;
                    control.MinHeight = colDef.MinHeight;
                    control.MaxHeight = colDef.MaxHeight;

                    // Apply style if it matches the control type
                    if (colDef.Style != null && colDef.Style.TargetType == control.GetType())
                    {
                        control.Style = colDef.Style;
                    }
                }

                // Apply alignment and margin
                control.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                control.VerticalAlignment = colDef.ContentVerticalAlignment;
                control.Margin = colDef.ContentMargin;

                // For ToggleButton, ensure binding is set up correctly
                if (control is ToggleButton toggleButton)
                {
                    var binding = new Binding(colDef.DataField)
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    };
                    BindingOperations.SetBinding(toggleButton, ToggleButton.IsCheckedProperty, binding);
                }

                control.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                control.VerticalAlignment = colDef.ContentVerticalAlignment;
                if (colDef.DataField.Contains(",") || colDef.DataField.Contains("|"))
                {
                    if (true)
                    {
                    }
                }
                if (!string.IsNullOrEmpty(colDef.DataField))
                {
                    if (colDef.DataField.Contains(",") || colDef.DataField.Contains("|"))
                    {
                        // Process DataFields - remove empty entries
                        var DataFields = colDef.GetFields(colDef.DataField).ToList();
                        var BoundTos = colDef.GetFields(colDef.BoundTo).ToList();
                        int MaxCount = DataFields.Count == BoundTos.Count ? DataFields.Count : 1;
                        for (int i = 0; i < MaxCount; i++)
                        {
                            if (!string.IsNullOrEmpty(DataFields[i]))
                            {
                                var binding = new Binding(DataFields[i].Trim())
                                {
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                    Mode = BindingMode.TwoWay
                                };

                                if (!string.IsNullOrEmpty(BoundTos[i]))
                                {
                                    // Get the property to bind to based on the BoundTo field
                                    var boundProperty = GetMainBindingProperty(colDef.ComponentType.ToString(), BoundTos[i].Trim());
                                    if (boundProperty != null)
                                    {
                                        BindingOperations.SetBinding(control, boundProperty, binding);
                                    }
                                }
                                else
                                {
                                    // If no specific BoundTo is provided, use the default main property for the component type
                                    var mainProperty = GetMainBindingProperty(colDef.ComponentType.ToString(), string.Empty);
                                    if (mainProperty != null)
                                    {
                                        BindingOperations.SetBinding(control, mainProperty, binding);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SetCustomBindings: {ex}");
            }
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

                if (type != null || !string.IsNullOrEmpty(boundTo))
                {
                    // Get the DependencyProperty field by name
                    var fieldName = $"{boundTo}Property";
                    var field = type.GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    var ret = field?.GetValue(null) as DependencyProperty;
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
                if (colDef.Style != null)
                {
                    factory.SetValue(ToggleButton.StyleProperty, colDef.Style);
                }
                if (!double.IsNaN(colDef.ToggleButtonWidth))
                {
                    factory.SetValue(FrameworkElement.WidthProperty, colDef.ToggleButtonWidth);
                }
                if (!double.IsNaN(colDef.ToggleButtonHeight))
                {
                    factory.SetValue(FrameworkElement.HeightProperty, colDef.ToggleButtonHeight);
                }
                factory.AddHandler(ToggleButton.ClickEvent, new RoutedEventHandler((s, e) => Click?.Invoke(s, e)));
            }
            else if (controlType == typeof(Button))
            {
                factory.AddHandler(Button.ClickEvent, new RoutedEventHandler((s, e) => Click?.Invoke(s, e)));
                factory.AddHandler(Button.LostFocusEvent, new RoutedEventHandler((s, e) => LostFocus?.Invoke(s, e)));
                factory.AddHandler(Button.GotFocusEvent, new RoutedEventHandler((s, e) => GotFocus?.Invoke(s, e)));
            }
            else if (controlType == typeof(CheckBox))
            {
                factory.AddHandler(CheckBox.ClickEvent, new RoutedEventHandler((s, e) => Click?.Invoke(s, e)));
                factory.AddHandler(CheckBox.LostFocusEvent, new RoutedEventHandler((s, e) => LostFocus?.Invoke(s, e)));
                factory.AddHandler(CheckBox.GotFocusEvent, new RoutedEventHandler((s, e) => GotFocus?.Invoke(s, e)));
            }
            else if (controlType == typeof(ComboBox))
            {
                factory.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler((s, e) => SelectionChanged?.Invoke(s, e)));
                factory.AddHandler(ComboBox.LostFocusEvent, new RoutedEventHandler((s, e) => LostFocus?.Invoke(s, e)));
                factory.AddHandler(ComboBox.GotFocusEvent, new RoutedEventHandler((s, e) => GotFocus?.Invoke(s, e)));
            }
            else if (controlType == typeof(TextBox))
            {
                factory.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler((s, e) => LostFocus?.Invoke(s, e)));
                factory.AddHandler(TextBox.GotFocusEvent, new RoutedEventHandler((s, e) => GotFocus?.Invoke(s, e)));
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

            // Set up additional binding
            if (!string.IsNullOrEmpty(colDef.BoundTo))
            {
                var additionalBinding = new Binding(colDef.DataField)
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                };
                var property = GetDependencyPropertyByName(colDef.BoundTo);
                if (property != null)
                {
                    factory.SetBinding(property, additionalBinding);
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
                    if (colDef.Style != null)
                    {
                        factory.SetValue(ToggleButton.StyleProperty, colDef.Style);
                    }
                    if (!double.IsNaN(colDef.ToggleButtonWidth))
                    {
                        factory.SetValue(FrameworkElement.WidthProperty, colDef.ToggleButtonWidth);
                    }
                    if (!double.IsNaN(colDef.ToggleButtonHeight))
                    {
                        factory.SetValue(FrameworkElement.HeightProperty, colDef.ToggleButtonHeight);
                    }
                    factory.AddHandler(ToggleButton.ClickEvent, new RoutedEventHandler((s, e) => Click?.Invoke(s, e)));
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

                // Set up additional binding
                if (!string.IsNullOrEmpty(colDef.BoundTo))
                {
                    var additionalBinding = new Binding(colDef.DataField)
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    };
                    var property = GetDependencyPropertyByName(colDef.BoundTo);
                    if (property != null)
                    {
                        factory.SetBinding(property, additionalBinding);
                    }
                }

                gridFactory.AppendChild(factory);
                columnIndex++;
            }

            _cachedItemTemplate = new DataTemplate { VisualTree = gridFactory };
        }

        private void ApplyVisualProperties()
        {
            if (_headerGrid == null) return;

            // Apply visual properties to header text blocks
            foreach (var child in _headerGrid.Children.OfType<TextBlock>())
            {
                var columnIndex = Grid.GetColumn(child);
                if (columnIndex >= 0 && columnIndex < ColumnDefinitions.Count)
                {
                    var colDef = ColumnDefinitions[columnIndex];
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        child.Margin = colDef.HeaderMargin;
                        child.Padding = colDef.HeaderPadding;
                        child.HorizontalAlignment = colDef.HeaderHorizontalAlignment;
                        child.VerticalAlignment = colDef.HeaderVerticalAlignment;
                        child.Style = (colDef?.Style is not null && colDef.Style.TargetType == typeof(TextBlock)) ? colDef.Style : null;
                    }));
                }
            }

            // Apply visual properties to all controls in the item template
            foreach (var colDef in ColumnDefinitions)
            {
                var controlType = GetControlType(colDef.ComponentType.ToString());

                if (controlType == typeof(ToggleButton))
                {
                    // ToggleButton properties are handled in the LoadedEvent handler
                }
                else if (controlType == typeof(CheckBox))
                {
                    // CheckBox properties are handled in the LoadedEvent handler
                }
                else if (controlType == typeof(Button))
                {
                    // Button properties are handled in the LoadedEvent handler
                }
                else if (controlType == typeof(ComboBox))
                {
                    // ComboBox properties are handled in the LoadedEvent handler
                }
                else if (controlType == typeof(DateTimePicker))
                {
                    // DateTimePicker properties are handled in the LoadedEvent handler
                }
            }
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

                        var headerTextBlock = new TextBlock { Text = colDef.HeaderText };
                        Grid.SetColumn(headerTextBlock, _headerGrid.ColumnDefinitions.Count - 1);
                        _headerGrid.Children.Add(headerTextBlock);

                        // Visual properties will be set in ApplyVisualProperties
                    }

                    // Add column to item template grid
                    var itemColumn = new System.Windows.Controls.ColumnDefinition();
                    // Width bindings will be set in MultiListbox_Loaded
                    if (!string.IsNullOrEmpty(colDef.WidthBinding))
                    {
                        // Width binding will be set in MultiListbox_Loaded
                    }
                    else
                    {
                        // For ToggleButton, ensure column is at least as wide as the button
                        if (colDef.ComponentType.ToString().ToLower() == "togglebutton" && !double.IsNaN(colDef.ToggleButtonWidth))
                        {
                            itemColumn.Width = new GridLength(Math.Max(colDef.Width, colDef.ToggleButtonWidth));
                            itemColumn.MinWidth = colDef.ToggleButtonWidth;
                        }
                        else
                        {
                            itemColumn.Width = new GridLength(colDef.Width);
                        }
                    }
                    _itemGrid.ColumnDefinitions.Add(itemColumn);

                    if (!string.IsNullOrEmpty(colDef.DataField))
                    {
                        var controlType = GetControlType(colDef.ComponentType.ToString());
                        var factory = new FrameworkElementFactory(controlType);
                        factory.SetValue(Grid.ColumnProperty, _itemGrid.ColumnDefinitions.Count - 1);
                        if (controlType == typeof(ToggleButton))
                        {
                            var toggleButton = new ToggleButton();
                            factory = new FrameworkElementFactory(typeof(ToggleButton));
                            factory.SetValue(FrameworkElement.NameProperty, "PART_ToggleButton");
                            factory.SetValue(Grid.ColumnProperty, _itemGrid.ColumnDefinitions.Count - 1);

                            // Add Loaded handler to ensure style and visibility
                            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                            {
                                var toggleButton = s as ToggleButton;
                                if (toggleButton != null)
                                {
                                    // Apply all visual properties and style after template is applied (step 2)
                                    Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                                    {
                                        toggleButton.Width = colDef.ToggleButtonWidth;
                                        toggleButton.Height = colDef.ToggleButtonHeight;
                                        toggleButton.MinWidth = colDef.ToggleButtonWidth;
                                        toggleButton.MinHeight = colDef.ToggleButtonHeight;
                                        toggleButton.MaxWidth = colDef.ToggleButtonWidth;
                                        toggleButton.MaxHeight = colDef.ToggleButtonHeight;
                                        toggleButton.HorizontalAlignment = HorizontalAlignment.Center;
                                        toggleButton.VerticalAlignment = VerticalAlignment.Center;
                                        toggleButton.Visibility = Visibility.Visible;
                                        toggleButton.IsChecked = false;
                                        toggleButton.Style = (colDef?.Style is null) ? null : colDef.Style;

                                        // Handle template-specific setup
                                        if (toggleButton.Template != null)
                                        {
                                            toggleButton.ApplyTemplate();
                                            var image = toggleButton.Template.FindName("PART_Image", toggleButton) as Image;
                                            toggleButton.Checked += (s2, e2) =>
                                            {
                                                var img = toggleButton.Template.FindName("PART_Image", toggleButton) as Image;
                                            };
                                            toggleButton.Unchecked += (s2, e2) =>
                                            {
                                                var img = toggleButton.Template.FindName("PART_Image", toggleButton) as Image;
                                                if (img != null)
                                                {
                                                    img.Source = null;
                                                }
                                            };
                                        }
                                    }));
                                }
                            }));

                            // Add size changed handler to check dimensions and visual tree
                            factory.AddHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler((s, e) =>
                            {
                                var toggle = s as ToggleButton;
                                if (toggle != null)
                                {
                                    toggle.Width = e.NewSize.Width;
                                    toggle.Height = e.NewSize.Height;
                                }
                            }));

                            // Add loaded handler to check initial state and template
                            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                            {
                                var toggle = s as ToggleButton;
                                if (toggle != null)
                                {
                                    var current = toggle as DependencyObject;
                                    if (toggle.Template != null)
                                    {
                                        toggle.ApplyTemplate();
                                        var image = toggle.Template.FindName("PART_Image", toggle) as Image;
                                        toggle.Checked += (s2, e2) =>
                                        {
                                            var img = toggle.Template.FindName("PART_Image", toggle) as Image;
                                        };
                                        toggle.Unchecked += (s2, e2) =>
                                        {
                                            var img = toggle.Template.FindName("PART_Image", toggle) as Image;
                                        };
                                    }
                                }
                            }));

                            // Add click handler that routes to the column definition's event
                            factory.AddHandler(ToggleButton.ClickEvent, new RoutedEventHandler((s, e) =>
                            {
                                var toggle = s as ToggleButton;
                                colDef.RaiseClick(s, e);
                                Click?.Invoke(s, e);
                            }));
                        }
                        if (controlType == typeof(CheckBox))
                        {
                            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                            {
                                var checkBox = s as CheckBox;
                                if (checkBox != null)
                                {
                                    // Visual properties will be set in MultiListbox_Loaded
                                }
                            }));
                        }
                        else if (controlType == typeof(Button))
                        {
                            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                            {
                                var button = s as Button;
                                if (button != null)
                                {
                                    // Visual properties will be set in MultiListbox_Loaded
                                    if (colDef.ButtonImageSource != null)
                                    {
                                        var image = new Image { Source = colDef.ButtonImageSource };
                                        button.Content = image;
                                    }
                                }
                            }));
                        }
                        else if (controlType == typeof(ComboBox))
                        {
                            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                            {
                                var comboBox = s as ComboBox;
                                if (comboBox != null)
                                {
                                    // Visual properties will be set in MultiListbox_Loaded
                                    ComboBoxInitialized?.Invoke(s, e);
                                }
                            }));
                            factory.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler((s, e) =>
                            {
                                var comboBox = s as ComboBox;
                                SelectionChanged?.Invoke(s, e);
                                colDef.RaiseSelectionChanged(s, e);
                            }));
                            factory.AddHandler(ComboBox.LostFocusEvent, new RoutedEventHandler((s, e) =>
                            {
                                var comboBox = s as ComboBox;
                                LostFocus?.Invoke(s, e);
                                colDef.RaiseLostFocusEvent(s, e);
                            }));
                            factory.AddHandler(ComboBox.GotFocusEvent, new RoutedEventHandler((s, e) =>
                            {
                                var comboBox = s as ComboBox;
                                GotFocus?.Invoke(s, e);
                                colDef.RaiseGotFocusEvent(s, e);
                            }));
                        }
                        else if (controlType == typeof(DateTimePicker))
                        {
                            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                            {
                                var datePicker = s as DateTimePicker;
                                if (datePicker != null)
                                {
                                    // Visual properties will be set in MultiListbox_Loaded
                                    if (!string.IsNullOrEmpty(colDef.DateTimeFormat))
                                    {
                                        if (Enum.TryParse<Xceed.Wpf.Toolkit.DateTimeFormat>(colDef.DateTimeFormat, true, out var format))
                                        {
                                            datePicker.Format = format;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(colDef.DateTimeFormatString))
                                    {
                                        datePicker.FormatString = colDef.DateTimeFormatString;
                                    }
                                    if (!string.IsNullOrEmpty(colDef.DateTimeTimeFormat))
                                    {
                                        if (Enum.TryParse<Xceed.Wpf.Toolkit.DateTimeFormat>(colDef.DateTimeTimeFormat, true, out var timeFormat))
                                        {
                                            datePicker.TimeFormat = timeFormat;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(colDef.DateTimeTimeFormatString))
                                    {
                                        datePicker.TimeFormatString = colDef.DateTimeTimeFormatString;
                                    }
                                    if (!double.IsNaN(colDef.DateTimeMinWidth))
                                    {
                                        datePicker.MinWidth = colDef.DateTimeMinWidth;
                                    }
                                    datePicker.Focusable = colDef.DateTimeFocusable;
                                }
                            }));
                            factory.AddHandler(DateTimePicker.LostFocusEvent, new RoutedEventHandler((s, e) => DateTimePickerLostFocus?.Invoke(s, e)));
                            factory.AddHandler(DateTimePicker.GotFocusEvent, new RoutedEventHandler((s, e) => DateTimePickerGotFocus?.Invoke(s, e)));
                            factory.AddHandler(DateTimePicker.KeyUpEvent, new KeyEventHandler((s, e) => DateTimePickerKeyUp?.Invoke(s, e)));
                        }

                        // Bindings will be set in MultiListbox_Loaded
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ApplyColumnDefinitions: {ex.Message}");
            }

            // Apply visual properties after column definitions are set up
            ApplyVisualProperties();
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

        private void DumpVisualTree(DependencyObject element, int level)
        {
            if (element == null) return;

            var indent = new string(' ', level * 4);
            var fe = element as FrameworkElement;
            if (fe != null)
            {
                System.Diagnostics.Debug.WriteLine($"{indent}Type: {element.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"{indent}Name: {fe.Name}");
                System.Diagnostics.Debug.WriteLine($"{indent}Width: {fe.Width}, ActualWidth: {fe.ActualWidth}");
                System.Diagnostics.Debug.WriteLine($"{indent}Height: {fe.Height}, ActualHeight: {fe.ActualHeight}");
                System.Diagnostics.Debug.WriteLine($"{indent}HorizontalAlignment: {fe.HorizontalAlignment}");
                System.Diagnostics.Debug.WriteLine($"{indent}VerticalAlignment: {fe.VerticalAlignment}");
                System.Diagnostics.Debug.WriteLine($"{indent}Margin: {fe.Margin}");
                if (fe is Grid grid)
                {
                    System.Diagnostics.Debug.WriteLine($"{indent}Grid Columns: {grid.ColumnDefinitions.Count}");
                    foreach (var col in grid.ColumnDefinitions)
                    {
                        System.Diagnostics.Debug.WriteLine($"{indent}  Column Width: {col.Width}, MinWidth: {col.MinWidth}, ActualWidth: {col.ActualWidth}");
                    }
                }
                System.Diagnostics.Debug.WriteLine("");
            }

            // Get visual children
            int childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                DumpVisualTree(child, level + 1);
            }

            // Get logical children
            if (element is ContentControl cc && cc.Content is DependencyObject content)
            {
                DumpVisualTree(content, level + 1);
            }
        }
        #endregion
    }
}
