using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode), typeof(SelectionMode), typeof(MultiListbox),
                new PropertyMetadata(SelectionMode.Single));

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
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
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateAdjustedWidth: {ex}");
            }
        }

        private void UpdateAdjustedGroupBoxWidth()
        {
            try
            {
                if (ActualWidth > 0)
                {
                    var margin = BorderMargin;
                    AdjustedGroupBoxWidth = Math.Max(0, ActualWidth - margin.Left - margin.Right - 5);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateAdjustedGroupBoxWidth: {ex}");
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            try
            {
                base.OnRenderSizeChanged(sizeInfo);
                UpdateAdjustedGroupBoxWidth();
                UpdateItemsListBoxHeight();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnRenderSizeChanged: {ex}");
            }
        }

        public void SelectAll()
        {
            lstBoxUploadItems.SelectAll();
        }
        private void UpdateItemsListBoxHeight()
        {
            try
            {
                if (lstBoxUploadItems != null)
                {
                    double headerHeight = 30; // Increased from 20 to 30
                    double scrollbarHeight = SystemParameters.HorizontalScrollBarHeight;
                    double extraPadding = 10; // Additional padding for better spacing
                    double availableHeight = ActualHeight - headerHeight - scrollbarHeight - extraPadding;
                    if (BorderMargin != null)
                    {
                        availableHeight -= (BorderMargin.Top + BorderMargin.Bottom + 5); // Added 5 pixels of extra margin
                    }
                    lstBoxUploadItems.Height = Math.Max(0, availableHeight);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateItemsListBoxHeight: {ex}");
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(MultiListbox),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty HeaderContextMenuProperty =
            DependencyProperty.Register(nameof(HeaderContextMenu), typeof(ContextMenu), typeof(MultiListbox),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ItemMinHeightProperty =
            DependencyProperty.Register(nameof(ItemMinHeight), typeof(double), typeof(MultiListbox),
                new PropertyMetadata(25.0, OnItemMinHeightChanged));

        private static void OnItemMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d is MultiListbox listbox && listbox.ColumnDefinitions != null)
                {
                    foreach (var colDef in listbox.ColumnDefinitions)
                    {
                        colDef.MinHeight = (double)e.NewValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnItemMinHeightChanged: {ex}");
            }
        }

        public static readonly DependencyProperty ItemMaxHeightProperty =
            DependencyProperty.Register(nameof(ItemMaxHeight), typeof(double),
                typeof(MultiListbox),
                new PropertyMetadata(35.0));



        public double ItemMinHeight
        {
            get { return (double)GetValue(ItemMinHeightProperty); }
            set { SetValue(ItemMinHeightProperty, value); }
        }



        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(double),
                typeof(MultiListbox),
                new PropertyMetadata(30.0, OnHeightChanged));

        private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d is MultiListbox listbox && listbox.ColumnDefinitions != null)
                {
                    foreach (var colDef in listbox.ColumnDefinitions)
                    {
                        colDef.Height = (double)e.NewValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnHeightChanged: {ex}");
            }
        }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }
        public double ItemMaxHeight
        {
            get { return (double)GetValue(ItemMaxHeightProperty); }
            set { SetValue(ItemMaxHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemsContextMenuProperty =
            DependencyProperty.Register(nameof(ItemsContextMenu),
                typeof(ContextMenu), typeof(MultiListbox),
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
        private Dictionary<int, EventHandler> _statusHandlers = new();

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
            try
            {
                InitializeComponent();
                ColumnDefinitions = new ObservableCollection<MultiListboxColumnDefinition>();
                Loaded += MultiListbox_Loaded;
                SizeChanged += MultiListbox_SizeChanged;
                this.DataContext = this;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MultiListbox: {ex}");
            }
        }

        private void InitializeScrollViewers()
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in InitializeScrollViewers: {ex}");
            }
        }

        private ScrollViewer GetScrollViewer(DependencyObject element)
        {
            try
            {
                if (element == null) return null;
                var child = VisualTreeHelper.GetChild(element, 0);
                if (child == null) return null;
                return child as ScrollViewer ?? GetScrollViewer(child);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetScrollViewer: {ex}");
                return null;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ScrollViewer_ScrollChanged: {ex}");
            }
        }

        private void MultiListbox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (e.WidthChanged)
                {
                    UpdateAdjustedWidth();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MultiListbox_SizeChanged: {ex}");
            }
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private INotifyCollectionChanged _currentCollection;

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var control = d as MultiListbox;
                if (control != null)
                {
                    bool Refresh = false;
                    if (control._currentCollection != null)
                    {
                        control._currentCollection.CollectionChanged -= control.OnCollectionChanged;
                    }
                    control.lstBoxUploadItems.ItemsSource = e.NewValue as IEnumerable;
                    if (e.NewValue != control._currentCollection)
                    {
                        Refresh = true;
                    }
                    control._currentCollection = e.NewValue as INotifyCollectionChanged;
                    if (control._currentCollection != null)
                    {
                        control._currentCollection.CollectionChanged += control.OnCollectionChanged;
                    }



                    control.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (var item in control.lstBoxUploadItems.Items)
                        {
                            var container = control.lstBoxUploadItems.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                            if (container != null)
                            {
                                container.ApplyTemplate();
                                var contentPresenter = control.FindVisualChild<ContentPresenter>(container);
                                if (contentPresenter != null)
                                {
                                    contentPresenter.ApplyTemplate();
                                    var grid = control.FindVisualChild<Grid>(contentPresenter);
                                    if (grid != null)
                                    {
                                        control.InitializeGrid(grid);
                                    }
                                }
                            }
                        }
                    }), DispatcherPriority.Loaded);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnItemsSourceChanged: {ex}");
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            if (e.NewItems != null)
                            {
                                for (int i = 0; i < e.NewItems.Count; i++)
                                {
                                    var currentIndex = e.NewStartingIndex + i;
                                    var currentItem = e.NewItems[i];
                                    EventHandler statusHandler = new EventHandler((s, args) =>
                                    {
                                        if (lstBoxUploadItems.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                                        {
                                            return;
                                        }
                                        if (_statusHandlers.ContainsKey(currentIndex))
                                        {
                                            lstBoxUploadItems.ItemContainerGenerator.StatusChanged -= _statusHandlers[currentIndex];
                                            _statusHandlers.Remove(currentIndex);
                                        }
                                        Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            var container = lstBoxUploadItems.ItemContainerGenerator.ContainerFromIndex(currentIndex) as ListBoxItem;
                                            if (container == null)
                                            {
                                                lstBoxUploadItems.UpdateLayout();
                                                lstBoxUploadItems.Visibility = Visibility.Collapsed;
                                                lstBoxUploadItems.ScrollIntoView(lstBoxUploadItems.Items[currentIndex]);
                                                container = lstBoxUploadItems.ItemContainerGenerator.ContainerFromIndex(currentIndex) as ListBoxItem;
                                            }
                                            if (container != null)
                                            {
                                                container.ApplyTemplate();
                                                var contentPresenter = FindVisualChild<ContentPresenter>(container);
                                                if (contentPresenter != null)
                                                {
                                                    contentPresenter.ApplyTemplate();
                                                    var grid = FindVisualChild<Grid>(contentPresenter);
                                                    if (grid != null)
                                                    {
                                                        InitializeGrid(grid);
                                                        if (currentIndex == e.NewStartingIndex + e.NewItems.Count - 1)
                                                        {
                                                            Dispatcher.BeginInvoke(new Action(() =>
                                                            {
                                                                lstBoxUploadItems.UpdateLayout();
                                                                if (lstBoxUploadItems.Items.Count > 0)
                                                                {
                                                                    lstBoxUploadItems.Visibility = Visibility.Visible;
                                                                    lstBoxUploadItems.ScrollIntoView(lstBoxUploadItems.Items[0]);
                                                                }
                                                            }), DispatcherPriority.Loaded);
                                                        }
                                                    }
                                                }
                                            }
                                        }), DispatcherPriority.Loaded);
                                    });
                                    if (_statusHandlers.ContainsKey(currentIndex))
                                    {
                                        lstBoxUploadItems.ItemContainerGenerator.StatusChanged -= _statusHandlers[currentIndex];
                                        _statusHandlers.Remove(currentIndex);
                                    }
                                    _statusHandlers[currentIndex] = statusHandler;
                                    lstBoxUploadItems.ItemContainerGenerator.StatusChanged += statusHandler;
                                }
                            }
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            // Handle removal logic if needed
                            break;

                        case NotifyCollectionChangedAction.Replace:
                            // Handle replace logic if needed
                            break;

                        case NotifyCollectionChangedAction.Move:
                            // Handle move logic if needed
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            // Full refresh needed only for Reset
                            RebuildItemTemplate();
                            UpdateVisualTree();
                            break;
                    }
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnCollectionChanged: {ex}");
            }
        }

        public ObservableCollection<MultiListboxColumnDefinition> ColumnDefinitions
        {
            get => (ObservableCollection<MultiListboxColumnDefinition>)GetValue(ColumnDefinitionsProperty);
            set => SetValue(ColumnDefinitionsProperty, value);
        }

        private void RebuildItemTemplate()
        {

            if (!IsLoaded) return;

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

                BuildAndApplyTemplate();

                // Apply any pending column definitions
                if (_pendingColumnDefinitions != null)
                {
                    ColumnDefinitions = _pendingColumnDefinitions;
                    _pendingColumnDefinitions = null;
                }

                ApplyColumnDefinitions();

                // Update visual tree
                UpdateVisualTree();

                if (true)
                {

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error rebuilding template: {ex}");
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            try
            {
                if (parent is null) return null;

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);

                    if (child is T found)
                        return found;

                    var result = FindVisualChild<T>(child);
                    if (result != null)
                        return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error finding visual child: {ex}");
                return null;
            }
        }

        private void UpdateVisualTree()
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var itemTemplate = lstBoxUploadItems.ItemTemplate as DataTemplate;
                    if (itemTemplate == null)
                    {
                        Debug.WriteLine("No item template found");
                        return;
                    }

                    foreach (var item in lstBoxUploadItems.Items)
                    {
                        var container = lstBoxUploadItems.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                        if (container != null)
                        {
                            container.ApplyTemplate();
                            container.UpdateLayout();

                            var contentPresenter = FindVisualChild<ContentPresenter>(container);
                            if (contentPresenter != null)
                            {
                                contentPresenter.ApplyTemplate();
                                contentPresenter.UpdateLayout();

                                var grid = FindVisualChild<Grid>(contentPresenter);
                                if (grid != null)
                                {
                                    foreach (var child in grid.Children)
                                    {
                                        if (child is FrameworkElement element)
                                        {
                                            element.UpdateLayout();
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
                Debug.WriteLine($"Error updating visual tree: {ex}");
            }
        }


        private void BuildAndApplyTemplate()
        {
            try
            {
                // Create a new item template with a Grid
                var gridFactory = new FrameworkElementFactory(typeof(Grid));
                gridFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));
                // do height thingey here
                var itemHeight = ColumnDefinitions.Any(cd => cd.Height > 0)
                ? ColumnDefinitions.Max(cd => cd.Height)
                : 30;
                if (ItemHeightProperty is not null)
                {
                    itemHeight = (double)GetValue(ItemHeightProperty);
                }
                gridFactory.SetValue(FrameworkElement.HeightProperty, itemHeight);

                gridFactory.SetValue(Panel.BackgroundProperty, Brushes.White);

                // Store reference to the grid factory for later use
                _itemGrid = new Grid(); // Temporary grid just for column definitions

                // Add each column definition and control
                int columnIndex = 0;
                foreach (var colDef in ColumnDefinitions)
                {
                    // Add column definition to the grid factory
                    var colDefFactory = new FrameworkElementFactory(typeof(ColumnDefinition));
                    if (!string.IsNullOrEmpty(colDef.WidthBinding))
                    {
                        var binding = new Binding(colDef.WidthBinding)
                        {
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1)
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
                        _headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(colDef.Width) });
                    }
                    gridFactory.AppendChild(colDefFactory);

                    // Add header text to the header grid
                    if (_headerGrid != null)
                    {
                        var headerTextBlock = new TextBlock
                        {
                            Text = colDef.HeaderText,
                            Margin = colDef.HeaderMargin,
                            Padding = colDef.HeaderPadding,
                            VerticalAlignment = colDef.HeaderVerticalAlignment,
                            HorizontalAlignment = colDef.HeaderHorizontalAlignment
                        };
                        Grid.SetColumn(headerTextBlock, columnIndex);
                        _headerGrid.Children.Add(headerTextBlock);
                    }

                    // Add control for this column if it has a data field
                    if (!string.IsNullOrEmpty(colDef.DataField))
                    {
                        AddControlToTemplate(gridFactory, colDef, columnIndex);
                    }
                    columnIndex++;
                }

                // Create and set the item template
                var itemTemplate = new DataTemplate { VisualTree = gridFactory };
                lstBoxUploadItems.ItemTemplate = itemTemplate;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error building and applying template: {ex}");
            }
        }

        private Type GetControlType(Type requestedType)
        {
            try
            {
                // If no specific type is requested, default to TextBlock
                if (requestedType == null)
                    return typeof(TextBlock);

                // Ensure the type is a FrameworkElement
                if (!typeof(FrameworkElement).IsAssignableFrom(requestedType))
                    return typeof(TextBlock);

                return requestedType;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting control type: {ex}");
                return typeof(TextBlock);
            }
        }

        private void AddControlToTemplate(FrameworkElementFactory gridFactory, MultiListboxColumnDefinition colDef, int columnIndex)
        {
            try
            {
                var controlType = GetControlType(colDef.ControlType);
                var controlFactory = new FrameworkElementFactory(controlType);

                // Set the column for the control
                controlFactory.SetValue(Grid.ColumnProperty, columnIndex);
                controlFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                controlFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
                controlFactory.SetValue(UIElement.VisibilityProperty, Visibility.Visible);
                controlFactory.SetValue(UIElement.IsHitTestVisibleProperty, true);
                controlFactory.SetValue(UIElement.OpacityProperty, 1.0);
                controlFactory.SetValue(Control.IsEnabledProperty, true);
                controlFactory.SetValue(Control.BackgroundProperty, Brushes.White);
                controlFactory.SetValue(Control.BorderBrushProperty, Brushes.Black);
                controlFactory.SetValue(Control.BorderThicknessProperty, new Thickness(1));
                // For TextBlocks, use ItemHeight if available, otherwise fall back to column def height
                if (controlType == typeof(TextBlock))
                {
                    double height = ItemHeightProperty != null ? (double)GetValue(ItemHeightProperty) : 0;
                    if (height <= 0)
                    {
                        height = colDef.Height;
                    }
                    controlFactory.SetValue(FrameworkElement.HeightProperty, height);
                }
                else
                {
                    controlFactory.SetValue(FrameworkElement.HeightProperty, colDef.Height);
                }
                // Apply common properties
                controlFactory.SetValue(FrameworkElement.MarginProperty, colDef.ItemMargin);
                controlFactory.SetValue(Control.ForegroundProperty, Brushes.Black);
                if (typeof(Control).IsAssignableFrom(controlType))
                {
                    controlFactory.SetValue(Control.PaddingProperty, colDef.ItemPadding);
                    controlFactory.SetValue(FrameworkElement.MinHeightProperty, colDef.MinHeight);
                    controlFactory.SetValue(FrameworkElement.MinWidthProperty, colDef.MinWidth);
                    controlFactory.SetValue(FrameworkElement.WidthProperty, colDef.Width);
                }
                controlFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, colDef.ItemVerticalAlignment);
                controlFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, colDef.ItemHorizontalAlignment);

                // Set up the bindings
                var bindingProperty = GetMainBindingProperty(controlType);
                if (bindingProperty != null)
                {
                    var binding = new Binding(colDef.DataField);
                    controlFactory.SetBinding(bindingProperty, binding);
                }

               
                // Set z-index to ensure control is visible
                controlFactory.SetValue(Panel.ZIndexProperty, columnIndex);

                // Add the control to the grid
                gridFactory.AppendChild(controlFactory);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding control to template: {ex}");
            }
        }

        private DependencyProperty GetMainBindingProperty(Type controlType)
        {
            try
            {
                if (controlType == typeof(TextBox))
                    return TextBox.TextProperty;
                if (controlType == typeof(TextBlock))
                    return TextBlock.TextProperty;
                if (controlType == typeof(CheckBox))
                    return CheckBox.IsCheckedProperty;
                if (controlType == typeof(ComboBox))
                    return ComboBox.SelectedItemProperty;
                if (controlType == typeof(DatePicker))
                    return DatePicker.SelectedDateProperty;

                // If it's a derived type, check base types
                if (typeof(TextBox).IsAssignableFrom(controlType))
                    return TextBox.TextProperty;
                if (typeof(ComboBox).IsAssignableFrom(controlType))
                    return ComboBox.SelectedItemProperty;

                return TextBlock.TextProperty; // Default to TextBlock.Text
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting main binding property: {ex}");
                return TextBlock.TextProperty;
            }
        }

        private void ItemContainerGenerator_StatusChanged(object? sender, EventArgs e)
        {
            if (lstBoxUploadItems.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
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
                                    InitializeGrid(grid);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MultiListbox_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                lstBoxUploadItems.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
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
                gridFactory.SetValue(FrameworkElement.HeightProperty, 25.0);
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
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1)
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
                        if (controlType != typeof(TextBlock))
                        {
                            controlFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, colDef.ContentVerticalAlignment);
                            controlFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, colDef.ContentHorizontalAlignment);
                        }


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
                                controlFactory.SetValue(FrameworkElement.WidthProperty, colDef.ToggleButtonWidth);
                                controlFactory.SetValue(FrameworkElement.HeightProperty, colDef.ToggleButtonHeight);
                                controlFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, colDef.ContentHorizontalAlignment);
                                controlFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, colDef.ContentVerticalAlignment);
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
                                    var binding = new Binding(colDef.DataField)
                                    {
                                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                        Mode = BindingMode.TwoWay
                                    };
                                    controlFactory.SetBinding(mainProperty, binding);

                                    // For TextBlocks, set TextAlignment based on ContentHorizontalAlignment
                                    if (controlType == typeof(TextBlock))
                                    {
                                        // Handle width binding if specified
                                        if (!string.IsNullOrEmpty(colDef.WidthBinding))
                                        {
                                            var widthBinding = new Binding(colDef.WidthBinding)
                                            {
                                                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1)
                                            };
                                            controlFactory.SetBinding(FrameworkElement.WidthProperty, widthBinding);
                                        }
                                        if (ItemHeightProperty is not null)
                                        {
                                            var itemHeight = (double)GetValue(ItemHeightProperty);
                                            controlFactory.SetValue(FrameworkElement.HeightProperty, itemHeight);
                                        }
                                        else controlFactory.SetValue(FrameworkElement.HeightProperty, colDef.Height);
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
                                        InitializeGrid(grid);
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
        private void InitializeGrid(Grid grid)
        {
            try
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
                        if (!string.IsNullOrEmpty(colDef.FontWeight))
                        {
                            // Binding must be like with as model might change
                            var binding = new Binding(colDef.FontWeight)
                            {
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                Mode = BindingMode.TwoWay
                            };
                            BindingOperations.SetBinding(textBlock, TextBlock.FontWeightProperty, binding);
                        }

                        if (!string.IsNullOrEmpty(colDef.Foreground))
                        {
                            // Binding must be like with as model might change
                            var binding = new Binding(colDef.Foreground)
                            {
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                Mode = BindingMode.TwoWay
                            };
                            BindingOperations.SetBinding(textBlock, TextBlock.ForegroundProperty, binding);
                        }

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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HandleChildEvents: {ex}");
            }
        }
        private void HandleSelectionChangedEvent<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            try
            {
                if (control is ComboBox b)
                {
                    b.SelectionChanged -= (s, e) => SelectionChanged?.Invoke(s, e);
                    b.SelectionChanged -= (s, e) => colDef.RaiseSelectionChanged(s, e);
                    b.SelectionChanged += (s, e) =>
                    {
                        colDef.RaiseSelectionChanged(s, e);
                        SelectionChanged?.Invoke(s, e);
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HandleSelectionChangedEvent: {ex}");
            }
        }
        private void HandleClickEvents<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HandleClickEvents: {ex}");
            }
        }

        private void HandleInitialized<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            try
            {
                // Remove any existing handlers
                control.Initialized -= (s, e) => Initialized?.Invoke(s, e);
                control.Initialized -= (s, e) => colDef.RaiseInitializedEvent(s, e);
                control.Initialized += (s, e) =>
                {
                    colDef.RaiseInitializedEvent(s, e);
                    Initialized?.Invoke(s, e);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HandleInitialized: {ex}");
            }
        }

        private void HandleFocusEvents<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            try
            {
                // Remove any existing handlers
                control.LostFocus -= (s, e) => LostFocus?.Invoke(s, e);
                control.LostFocus -= (s, e) => colDef.RaiseLostFocusEvent(s, e);
                control.GotFocus -= (s, e) => GotFocus?.Invoke(s, e);
                control.GotFocus -= (s, e) => colDef.RaiseGotFocusEvent(s, e);
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HandleFocusEvents: {ex}");
            }
        }

        private void SetCustomBindings<T>(T control, MultiListboxColumnDefinition? colDef) where T : FrameworkElement
        {
            try
            {
                if (colDef == null) return;

                // Handle font weight and color bindings if specified

                if (control is ToggleButton toggleButton1)
                {
                    control.Width = colDef.ToggleButtonWidth;
                    control.Height = colDef.ToggleButtonHeight;
                    control.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                    control.VerticalAlignment = colDef.ContentVerticalAlignment;
                    if (colDef.Style != null && colDef.Style.TargetType == typeof(ToggleButton))
                    {
                        control.Style = colDef.Style;
                    }
                }
                else
                {
                    if (control is TextBlock textBlock)
                    {
                        if (!string.IsNullOrEmpty(colDef.WidthBinding))
                        {

                            var binding = new Binding(colDef.WidthBinding)
                            {
                                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1),
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                Mode = BindingMode.TwoWay
                            };
                            BindingOperations.SetBinding(textBlock, FrameworkElement.WidthProperty, binding);
                        }

                        if (!string.IsNullOrEmpty(colDef.Foreground) || !string.IsNullOrEmpty(colDef.FontWeight))
                        {
                            if (textBlock.FontWeight == FontWeights.Normal)
                            {

                            }
                        }
                        FormattedText ft = new FormattedText("WOOW",
                    CultureInfo.CurrentCulture, CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
                    new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight,
                    textBlock.FontStretch),
                    textBlock.FontSize > 0 ? textBlock.FontSize : 9,
                    textBlock.Foreground ?? new SolidColorBrush(Colors.Black),
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);
                        var maxheightoff = (control.Height - ft.Height) / 2;
                        textBlock.Padding = new Thickness(0, maxheightoff, 0, 0);
                        if (ItemHeightProperty is not null)
                        {
                            var itemHeight = (double)GetValue(ItemHeightProperty);
                            control.Height = itemHeight;
                        }
                        else control.Height = colDef.Height;
                        control.Width = colDef.Width;
                        control.MinWidth = colDef.MinWidth;
                        control.MinWidth = colDef.MinWidth;
                        control.MaxWidth = colDef.MaxWidth;
                        control.MinHeight = colDef.MinHeight;
                        control.MaxHeight = colDef.MaxHeight;
                    }
                    else
                    {
                        control.Width = colDef.Width;
                        control.Height = colDef.Height;
                        control.MinWidth = colDef.MinWidth;
                        control.MinWidth = colDef.MinWidth;
                        control.MaxWidth = colDef.MaxWidth;
                        control.MinHeight = colDef.MinHeight;
                        control.MaxHeight = colDef.MaxHeight;
                    }
                    // Apply alignment and margin
                    control.HorizontalAlignment = colDef.ContentHorizontalAlignment;
                    control.VerticalAlignment = colDef.ContentVerticalAlignment;
                    control.Margin = colDef.ContentMargin;

                    if (colDef.Style != null && colDef.Style.TargetType == control.GetType())
                    {
                        control.Style = colDef.Style;
                    }
                    if (!string.IsNullOrEmpty(colDef.DataField))
                    {
                        var binding = new Binding(colDef.DataField)
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Mode = BindingMode.TwoWay
                        };
                        BindingOperations.SetBinding(control, GetMainBindingProperty(colDef.ComponentType.ToString(), colDef.BoundTo), binding);
                    }
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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SetCustomBindings: {ex}");
            }
        }

        private static void OnColumnDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d is MultiListbox multiListbox)
                {
                    multiListbox._columnFactories = null; // Force rebuild of factories
                    multiListbox._cachedItemTemplate = null; // Force rebuild of template
                    multiListbox.ApplyColumnDefinitions();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnColumnDefinitionsChanged: {ex}");
            }
        }

        private Type GetControlType(string componentType)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetControlType: {ex}");
                return typeof(TextBlock);
            }
        }

        private DependencyProperty GetDependencyPropertyByName(string propertyName)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetDependencyPropertyByName: {ex}");
                return null;
            }
        }

        private DependencyProperty GetMainBindingProperty(string componentType, string boundTo = null)
        {
            try
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
                        return TextBlock.TextProperty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetMainBindingProperty: {ex}");
                return null;
            }
        }
        private void ApplyVisualProperties()
        {
            try
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ApplyVisualProperties: {ex}");
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
                                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1)
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
                    }

                    // Add column to item template grid
                    var itemColumn = new System.Windows.Controls.ColumnDefinition();
                    
                    if (!string.IsNullOrEmpty(colDef.WidthBinding))
                    {
                        var binding = new Binding(colDef.WidthBinding)
                        {
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1)
                        };
                        BindingOperations.SetBinding(itemColumn, System.Windows.Controls.ColumnDefinition.WidthProperty, binding);
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
                            factory = new FrameworkElementFactory(typeof(ToggleButton));
                            factory.SetValue(FrameworkElement.NameProperty, "PART_ToggleButton");
                            factory.SetValue(Grid.ColumnProperty, _itemGrid.ColumnDefinitions.Count - 1);

                            // Add Loaded handler to ensure style and visibility
                            factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                            {
                                var toggleButton = s as ToggleButton;
                                if (toggleButton != null)
                                {
                                    // Apply all visual properties and style after template is applied
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
                                        toggleButton.Style = (colDef?.Style is not null && colDef.Style.TargetType == typeof(ToggleButton)) ? colDef.Style : null;

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

                            factory.AddHandler(ToggleButton.ClickEvent, new RoutedEventHandler((s, e) =>
                            {
                                var toggle = s as ToggleButton;
                                colDef.RaiseClick(s, e);
                                Click?.Invoke(s, e);
                            }));
                        }
                        else if (controlType == typeof(CheckBox))
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

        #endregion
    }
}
