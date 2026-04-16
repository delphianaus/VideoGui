using Accessibility;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using ToggleClass;
using Xceed.Wpf.Toolkit;
using static CustomComponents.delegates;
using static CustomComponents.Extensions;


namespace CustomComponents.ListBoxExtensions
{


    public class MultiListboxTemplateColumn
    {
        public string HeaderText { get; set; } = string.Empty;
        public double Width { get; set; }
        public bool DynamicWidth { get; set; } = false;
        public string WidthBindingPath { get; set; } = string.Empty;
        public string DataField { get; set; } = string.Empty;

        public string FontWeight { get; set; } = string.Empty;
        public string Foreground { get; set; } = string.Empty;

        public string ComponentType { get; set; } = "TextBlock";
        public VerticalAlignment ContentVerticalAlignment { get; set; } = VerticalAlignment.Center;
        public Style? Style { get; set; }
        public string ClickHandler { get; set; } = string.Empty;

    }

    public enum MultiListboxResizeDirection
    {
        None,
        Top,
        Bottom,
        All
    }


    public partial class MultiListbox : UserControl, INotifyPropertyChanged
    {
        [ContentProperty(nameof(Columns))]
        public class MultiListboxTemplate
        {
            public string Key { get; set; } = string.Empty;

            public ObservableCollection<MultiListboxTemplateColumn> Columns { get; set; }
                = new ObservableCollection<MultiListboxTemplateColumn>();
        }
        public ObservableCollection<MultiListboxTemplate> Templates
        {
            get => (ObservableCollection<MultiListboxTemplate>)GetValue(TemplatesProperty);
            set => SetValue(TemplatesProperty, value);
        }
        public VerticalAlignment ItemsVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(ItemsVerticalAlignmentProperty);
            set => SetValue(ItemsVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty ItemsVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(ItemsVerticalAlignment),
                typeof(VerticalAlignment), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(VerticalAlignment.Center));


        public HorizontalAlignment ItemsHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(ItemsHorizontalAlignmentProperty);
            set => SetValue(ItemsHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty ItemsHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(ItemsHorizontalAlignment),
                typeof(HorizontalAlignment), typeof(MultiListboxColumnDefinition),
                new PropertyMetadata(HorizontalAlignment.Left));



        public static readonly DependencyProperty TemplatesProperty =
            DependencyProperty.Register(nameof(Templates),
                typeof(ObservableCollection<MultiListboxTemplate>),
                typeof(MultiListbox), new PropertyMetadata(null, OnTemplatesChanged));

        private static void OnTemplatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not MultiListbox control)
                return;

            try
            {
                if (e.OldValue is ObservableCollection<MultiListboxTemplate> oldTemplates)
                    oldTemplates.CollectionChanged -= control.Templates_CollectionChanged;

                if (e.NewValue is ObservableCollection<MultiListboxTemplate> newTemplates)
                    newTemplates.CollectionChanged += control.Templates_CollectionChanged;

                control.HookTemplateColumnCollections(e.OldValue as ObservableCollection<MultiListboxTemplate>, false);
                control.HookTemplateColumnCollections(e.NewValue as ObservableCollection<MultiListboxTemplate>, true);

                control.ApplySelectedTemplate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnTemplatesChanged: {ex}");
            }
        }

        private void HookTemplateColumnCollections(ObservableCollection<MultiListboxTemplate>? templates, bool hook)
        {
            if (templates == null)
                return;

            foreach (var template in templates)
            {
                if (hook)
                    template.Columns.CollectionChanged += TemplateColumns_CollectionChanged;
                else
                    template.Columns.CollectionChanged -= TemplateColumns_CollectionChanged;
            }
        }

        private void Templates_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems.OfType<MultiListboxTemplate>())
                    {
                        item.Columns.CollectionChanged -= TemplateColumns_CollectionChanged;
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems.OfType<MultiListboxTemplate>())
                    {
                        item.Columns.CollectionChanged += TemplateColumns_CollectionChanged;
                    }
                }

                ApplySelectedTemplate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Templates_CollectionChanged: {ex}");
            }
        }

        private void TemplateColumns_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                ApplySelectedTemplate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in TemplateColumns_CollectionChanged: {ex}");
            }
        }

        private static void OnActiveTemplateKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not MultiListbox control)
                return;

            try
            {
                control.ApplySelectedTemplate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnActiveTemplateKeyChanged: {ex}");
            }
        }
        public string ActiveTemplateKey
        {
            get => (string)GetValue(ActiveTemplateKeyProperty);
            set => SetValue(ActiveTemplateKeyProperty, value);
        }

        public static readonly DependencyProperty ActiveTemplateKeyProperty =
            DependencyProperty.Register(
                nameof(ActiveTemplateKey),
                typeof(string),
                typeof(MultiListbox),
                new PropertyMetadata(null, OnActiveTemplateKeyChanged));



        private void ApplySelectedTemplate()
        {
            if (IsBuilding)
                return;

            try
            {


                if (Templates == null || Templates.Count == 0)
                    return;

                if (string.IsNullOrWhiteSpace(ActiveTemplateKey))
                    return;

                var template = Templates.FirstOrDefault(t =>
                    string.Equals(t.Key, ActiveTemplateKey, StringComparison.OrdinalIgnoreCase));

                if (template == null)
                    return;
                IsBuilding = true;
                var newColumns = new ObservableCollection<MultiListboxColumnDefinition>();

                foreach (var item in template.Columns)
                {
                    var newColumn = new MultiListboxColumnDefinition
                    {
                        HeaderText = item.HeaderText,
                        DataField = item.DataField,
                        FontWeight = item.FontWeight,

                        Foreground = item.Foreground
                        //ContentVerticalAlignment = item.ContentVerticalAlignment
                    };
                    if (item.Style is not null)
                    {
                        newColumn.Style = item.Style;
                    }
                    // adjust this if your real property is enum/type/etc

                    if (item.DynamicWidth)
                    {
                        newColumn.WidthBinding = item.WidthBindingPath;
                    }
                    else
                    {
                        newColumn.Width = item.Width;
                    }
                    newColumn.ComponentType = item.ComponentType;
                    var clickHandler = ResolveClickHandler(item.ClickHandler);
                    if (clickHandler != null)
                    {
                        newColumn.Click += clickHandler;
                    }
                    newColumns.Add(newColumn);
                }

                ColumnDefinitions = newColumns;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ApplySelectedTemplate: {ex}");
            }
            finally
            {
                IsBuilding = false;
            }
        }

        public bool IsBuilding = false;
        private RoutedEventHandler? ResolveClickHandler(string handlerName)
        {
            if (string.IsNullOrWhiteSpace(handlerName))
                return null;

            var host = Window.GetWindow(this);
            if (host == null)
                return null;

            var method = host.GetType().GetMethod(
                handlerName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method == null)
                return null;

            return (sender, e) => method.Invoke(host, new object[] { sender, e });
        }
        private void BuildColumnDefinitionsFromTemplate()
        {
            try
            {
                IsBuilding = true;
                ColumnDefinitions.Clear();
                foreach (var column in Templates)
                {
                    // build column definition based on template column
                }
            }
            finally
            {
                IsBuilding = false;
                _columnFactories = null; // Force rebuild of factories
                _cachedItemTemplate = null; // Force rebuild of template
                ApplyColumnDefinitions();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private ErrorHandler _ErrorHandler;
        public event ErrorHandler ErrorHandler
        {
            add { _ErrorHandler += value; }
            remove { _ErrorHandler -= value; }
        }
        protected virtual void OnError(Exception _exception, string _callingMethod, string message)
        {
            _ErrorHandler?.Invoke(_exception, _callingMethod, message);
        }
        internal void RaiseError(Exception _exception, string _callingMethod, string message)
        {
            OnError(_exception, _callingMethod, message);
        }

        private double _originalHeight;
        private double _startPoint;

        private void TopThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ResizeDirection != MultiListboxResizeDirection.Top && ResizeDirection != MultiListboxResizeDirection.All)
                return;

            if (double.IsNaN(Height)) return;

            var newHeight = Math.Max(MinHeight, Height - e.VerticalChange);
            if (MaxHeight > 0)
                newHeight = Math.Min(MaxHeight, newHeight);

            Height = newHeight;
            BorderMargin = new Thickness(BorderMargin.Left, BorderMargin.Top + e.VerticalChange, BorderMargin.Right, BorderMargin.Bottom);
        }

        private void BottomThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ResizeDirection != MultiListboxResizeDirection.Bottom && ResizeDirection != MultiListboxResizeDirection.All)
                return;

            if (double.IsNaN(Height)) return;

            var newHeight = Math.Max(MinHeight, Height + e.VerticalChange);
            if (MaxHeight > 0)
                newHeight = Math.Min(MaxHeight, newHeight);

            Height = newHeight;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region Events

     


        /// <summary>
        /// Gets the ItemContainerGenerator for the internal ListBox.
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator
        {
            get { return lstBoxUploadItems.ItemContainerGenerator; }
        }
  



        public IList Items
        {
            get { return lstBoxUploadItems.Items; }
        }

        public int SelectedIndex
        {
            get { return lstBoxUploadItems.SelectedIndex; }
            set
            {
                lstBoxUploadItems.SelectedIndex = value;
            }
        }

        public IList SelectedItems
        {
            get { return lstBoxUploadItems.SelectedItems; }
        }
        #endregion

        public object SelectedItem
        {
            get { return lstBoxUploadItems.SelectedItem; }
        }


        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.Register(nameof(ColumnDefinitions),
                typeof(ObservableCollection<MultiListboxColumnDefinition>),
                typeof(MultiListbox),
                new PropertyMetadata(null, OnColumnDefinitionsChanged));

        public static readonly DependencyProperty DebugOutputProperty =
            DependencyProperty.Register("DebugOutput", typeof(bool), typeof(MultiListbox),
                new PropertyMetadata(false));


        public bool DebugOutput
        {
            get { return (bool)GetValue(DebugOutputProperty); }
            set { SetValue(DebugOutputProperty, value); }
        }
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public static readonly DependencyProperty FontSizeProperty =
                   DependencyProperty.Register(nameof(FontSize),
                       typeof(double), typeof(MultiListbox),
                       new PropertyMetadata(9.0, OnFontSizeChanged));


        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiListbox multiListbox && !multiListbox.IsBuilding)
            {
                multiListbox._columnFactories = null; // Force rebuild of factories
                multiListbox._cachedItemTemplate = null; // Force rebuild of template
                multiListbox.ApplyColumnDefinitions();
            }
        }
        public MultiListboxResizeDirection ResizeDirection
        {
            get { return (MultiListboxResizeDirection)GetValue(ResizeDirectionProperty); }
            set { SetValue(ResizeDirectionProperty, value); }
        }
        public static readonly DependencyProperty ResizeDirectionProperty =
            DependencyProperty.Register(nameof(ResizeDirection),
                typeof(MultiListboxResizeDirection),
                typeof(MultiListbox),
                new PropertyMetadata(MultiListboxResizeDirection.None,
                    OnResizeDirectionChanged));

        private static void OnResizeDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MultiListbox)d;
            control.UpdateResizeThumbsVisibility();
        }



        private void UpdateResizeThumbsVisibility()
        {
            if (topThumb != null)
                topThumb.Visibility = (ResizeDirection == MultiListboxResizeDirection.Top || ResizeDirection == MultiListboxResizeDirection.All)
                    ? Visibility.Visible : Visibility.Collapsed;

            if (bottomThumb != null)
                bottomThumb.Visibility = (ResizeDirection == MultiListboxResizeDirection.Bottom || ResizeDirection == MultiListboxResizeDirection.All)
                    ? Visibility.Visible : Visibility.Collapsed;
        }

        public Thickness BorderItemMargin
        {
            get { return (Thickness)GetValue(BorderItemMarginProperty); }
            set { SetValue(BorderItemMarginProperty, value); }
        }

        public static readonly DependencyProperty BorderItemMarginProperty =
             DependencyProperty.Register(nameof(BorderItemMargin),
                 typeof(Thickness), typeof(MultiListbox),
                 new PropertyMetadata(new Thickness(0), (d, e) =>
                 ((MultiListbox)d).OnBorderItemMarginChanged()));

        private void OnBorderItemMarginChanged()
        {
            UpdateAdjustedGroupBoxItemWidth();
        }

        public Thickness BorderMargin
        {
            get { return (Thickness)GetValue(BorderMarginProperty); }
            set { SetValue(BorderMarginProperty, value); }
        }

        public static readonly DependencyProperty BorderMarginProperty =
            DependencyProperty.Register(nameof(BorderMargin),
                typeof(Thickness), typeof(MultiListbox),
                new PropertyMetadata(new Thickness(0),
                    (d, e) => ((MultiListbox)d).OnBorderMarginChanged()));

        private void OnBorderMarginChanged()
        {
            UpdateAdjustedGroupBoxWidth();
        }

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode),
                typeof(SelectionMode), typeof(MultiListbox),
                new PropertyMetadata(SelectionMode.Single));



        public static readonly DependencyProperty AdjustedWidthProperty =
            DependencyProperty.Register("AdjustedWidth",
                typeof(double), typeof(MultiListbox),
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
            DependencyProperty.Register("AdjustedGroupBoxWidth",
                typeof(double), typeof(MultiListbox),
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

        private double _AdjustedGroupBoxItemWidth;
        public double AdjustedGroupBoxItemWidth
        {
            get => _AdjustedGroupBoxItemWidth;
            private set
            {
                if (_AdjustedGroupBoxItemWidth != value)
                {
                    _AdjustedGroupBoxItemWidth = value;
                    OnPropertyChanged(nameof(AdjustedGroupBoxItemWidth));
                }
            }
        }
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource),
                typeof(IEnumerable), typeof(MultiListbox),
                new PropertyMetadata(null, OnItemsSourceChanged));

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
                        control.lstBoxUploadItems.UpdateLayout();
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
                    // control.setTimer();

                }



            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnItemsSourceChanged: {ex}");
            }
        }




        public ContextMenu HeaderContextMenu
        {
            get => (ContextMenu)GetValue(HeaderContextMenuProperty);
            set => SetValue(HeaderContextMenuProperty, value);
        }

        public static readonly DependencyProperty HeaderContextMenuProperty =
            DependencyProperty.Register(nameof(HeaderContextMenu),
                typeof(ContextMenu), typeof(MultiListbox),
                new PropertyMetadata(null));

        public double ItemMaxHeight
        {
            get { return (double)GetValue(ItemMaxHeightProperty); }
            set { SetValue(ItemMaxHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemMaxHeightProperty =
           DependencyProperty.Register(nameof(ItemMaxHeight), typeof(double),
               typeof(MultiListbox),
               new PropertyMetadata(double.PositiveInfinity));

        public double ItemMinHeight
        {
            get { return (double)GetValue(ItemMinHeightProperty); }
            set { SetValue(ItemMinHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemMinHeightProperty =
          DependencyProperty.Register(nameof(ItemMinHeight),
              typeof(double), typeof(MultiListbox),
              new PropertyMetadata(5.0, OnItemMinHeightChanged));
        private static void OnItemMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d is MultiListbox listbox && listbox.ColumnDefinitions != null)
                {
                    foreach (var colDef in listbox.ColumnDefinitions)
                    {
                        if (colDef.ComponentType.ToString().ToLower() != "textblock")
                        {
                            colDef.MinHeight = (double)e.NewValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnItemMinHeightChanged: {ex}");
            }
        }
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(double),
                typeof(MultiListbox),
                new PropertyMetadata(13.0, OnHeightChanged));

        private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d is MultiListbox listbox && listbox.ColumnDefinitions != null)
                {
                    foreach (var colDef in listbox.ColumnDefinitions)
                    {
                        if (colDef.ComponentType.ToString().ToLower() != "textblock")
                        {
                            colDef.Height = (double)e.NewValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnHeightChanged: {ex}");
            }
        }


        public ContextMenu ItemsContextMenu
        {
            get => (ContextMenu)GetValue(ItemsContextMenuProperty);
            set => SetValue(ItemsContextMenuProperty, value);
        }

        public static readonly DependencyProperty ItemsContextMenuProperty =
        DependencyProperty.Register(nameof(ItemsContextMenu),
            typeof(ContextMenu), typeof(MultiListbox),
            new PropertyMetadata(null));
        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(nameof(BorderBrush),
                typeof(Brush), typeof(MultiListbox),
                new PropertyMetadata(Brushes.Black));

        public double BorderThickness
        {
            get => (double)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(nameof(BorderThickness),
                typeof(double), typeof(MultiListbox),
                new PropertyMetadata(2.0));

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header),
                typeof(object), typeof(MultiListbox),
                new PropertyMetadata(null));


        public GridLength ColumnWidth
        {
            get => (GridLength)GetValue(ColumnWidthProperty);
            set => SetValue(ColumnWidthProperty, value);
        }

        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register(nameof(ColumnWidth),
                typeof(GridLength), typeof(MultiListbox),
                new PropertyMetadata(new GridLength(0, GridUnitType.Pixel)));

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

        private void UpdateAdjustedGroupBoxItemWidth()
        {
            try
            {
                if (ActualWidth > 0)
                {
                    var margin = BorderItemMargin;
                    AdjustedGroupBoxItemWidth = Math.Max(0, ActualWidth - margin.Left - margin.Right - 5);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateAdjustedGroupBoxWidth: {ex}");
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
                    double headerHeight = 25; // Increased from 20 to 30
                    double scrollbarHeight = SystemParameters.HorizontalScrollBarHeight;
                    double extraPadding = 0; // Additional padding for better spacing
                    double availableHeight = ActualHeight - headerHeight - scrollbarHeight - extraPadding;
                    if (BorderMargin != null)
                    {
                        availableHeight -= (+5 +
                            BorderItemMargin.Top + BorderItemMargin.Bottom +
                            BorderMargin.Top + BorderMargin.Bottom); // Added 5 pixels of extra margin
                    }
                    lstBoxUploadItems.Height = Math.Max(0, availableHeight);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateItemsListBoxHeight: {ex}");
            }
        }







        private Grid _headerGrid;
        private Grid _itemGrid;
        private DataTemplate _cachedItemTemplate;
        private Dictionary<string, FrameworkElementFactory> _columnFactories;
        private ObservableCollection<MultiListboxColumnDefinition> _pendingColumnDefinitions;
        private Dictionary<int, EventHandler> _statusHandlers = new();



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
                UpdateResizeThumbsVisibility();
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
                    UpdateAdjustedWidth();
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
                Debug.WriteLine($"Error in MultiListbox_SizeChanged: {ex}");
            }
        }



        private INotifyCollectionChanged _currentCollection;



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
                         //  RebuildItemTemplate();
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


                gridFactory.SetValue(Panel.BackgroundProperty, Brushes.White);

                // Store reference to the grid factory for later use
                _itemGrid = new Grid(); // Temporary grid just for column definitions
                ;

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
                            Text = colDef.HeaderText+"*",
                            Margin = colDef.HeaderMargin,
                            Padding = colDef.HeaderPadding,
                            VerticalAlignment = colDef.HeaderVerticalAlignment,
                            HorizontalAlignment = colDef.HeaderHorizontalAlignment
                        };
                        Grid.SetColumn(headerTextBlock, columnIndex);
                        _headerGrid.Children.Add(headerTextBlock);
                    }

                    // Add control for this column if it has a data field

                    AddControlToTemplate(gridFactory, colDef, columnIndex);

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
                //controlFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                // controlFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
                controlFactory.SetValue(UIElement.VisibilityProperty, Visibility.Visible);
                controlFactory.SetValue(UIElement.IsHitTestVisibleProperty, true);
                controlFactory.SetValue(UIElement.OpacityProperty, 1.0);
                controlFactory.SetValue(Control.IsEnabledProperty, true);
                controlFactory.SetValue(Control.BackgroundProperty, Brushes.White);
                controlFactory.SetValue(Control.BorderBrushProperty, Brushes.Black);
                controlFactory.SetValue(Control.BorderThicknessProperty, new Thickness(1));
                // For TextBlocks, use ItemHeight if available, otherwise fall back to column def height
                if (controlType != typeof(TextBlock))
                {
                   // controlFactory.SetValue(FrameworkElement.HeightProperty, colDef.Height);
                }
                // Apply common properties
                //controlFactory.SetValue(FrameworkElement.MarginProperty, colDef.ItemMargin);
                controlFactory.SetValue(Control.ForegroundProperty, Brushes.Black);
                if (typeof(Control).IsAssignableFrom(controlType))
                {
                    controlFactory.SetValue(Control.PaddingProperty, colDef.ItemPadding);
                    controlFactory.SetValue(FrameworkElement.MinHeightProperty, colDef.MinHeight);
                    controlFactory.SetValue(FrameworkElement.MinWidthProperty, colDef.MinWidth);
                    // controlFactory.SetValue(FrameworkElement.WidthProperty, colDef.Width);
                }
                //   controlFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, colDef.ItemVerticalAlignment);
                //   controlFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, colDef.ItemHorizontalAlignment);

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
        bool ready = true;
        private void ItemContainerGenerator_StatusChanged(object? sender, EventArgs e)
        {
            try
            {
                if (lstBoxUploadItems.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    foreach (var item in lstBoxUploadItems.Items)
                    {
                        var container = lstBoxUploadItems.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                        if (container != null)
                        {

                            if (VisualTreeHelper.GetChildrenCount(container) > 0)
                            {
                                var border = VisualTreeHelper.GetChild(container, 0) as Border;
                                if (border != null && VisualTreeHelper.GetChildrenCount(border) > 0)
                                {
                                    var contentPresenter = VisualTreeHelper.GetChild(border, 0) as ContentPresenter;
                                    if (contentPresenter != null && VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                                    {
                                        var grid = VisualTreeHelper.GetChild(contentPresenter, 0) as Grid;
                                        if (grid != null)
                                        {
                                            InitializeGrid(grid);
                                            ready = true;
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error accessing visual tree for item : {ex}");
            }
        }


        private void MultiListbox_Loaded(object sender, RoutedEventArgs e)
        {

            int Line = 0;
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

                gridFactory.SetValue(Panel.BackgroundProperty, Brushes.White);
                int x = 0;
                // Store reference to the grid factory for later use
                _itemGrid = new Grid(); // Temporary grid just for column definitions
                double ValueHeight = 0.0, ValueWidth = 0.0;
                // Add each column definition and control
                int columnIndex = 0;
                bool IsToggleInColumn = ColumnDefinitions.
                    Where(s => GetControlType(s.ComponentType) == typeof(ToggleButtonEx)).Any();
                bool IsCheckBoxInColumn= ColumnDefinitions.
                Where(s => GetControlType(s.ComponentType) == typeof(CheckBox)).Any();
                bool HasValidStyle = false;
                var _ItemHeight = ItemHeight;
                StyleHandler styleHandler = new();
                if (IsToggleInColumn)
                {
                    int row = -1;
                    foreach (var colDef in ColumnDefinitions)
                    {

                        row++;
                        var _controlType = GetControlType(colDef.ComponentType);
                        if (_controlType == null) continue;
                        if (_controlType == typeof(ToggleButtonEx))
                        {
                            int _stylescnt = -1;
                            if (colDef.Style != null && colDef.Style.TargetType == _controlType)
                            {
                                foreach (SetterBase style in (SetterBaseCollection)colDef.Style.Setters)
                                {
                                    if (style is Setter _style && !_style.Property.Name.ToLower().Contains("template"))
                                    {
                                        _stylescnt++;
                                    }
                                }
                                if (_stylescnt > -1)
                                {
                                    List<Setter> styles = new List<Setter>();
                                    foreach (SetterBase style in (SetterBaseCollection)colDef.Style.Setters)
                                    {
                                        if (style is Setter _style && !_style.Property.Name.ToLower().Contains("template"))
                                        {
                                            styles.Add(_style);
                                        }
                                    }
                                    styleHandler = new(RaiseError, styles);
                                    HasValidStyle = true;
                                }
                            }
                            break;
                        }
                    }
                    if (HasValidStyle)
                    {
                        _ItemHeight = styleHandler.Height;
                    }
                }
                Line = 0;
                int r = 0;
                foreach (var colDef in ColumnDefinitions)
                {
                    r++;
                    var controlType = GetControlType(colDef.ComponentType);
                    if (controlType == null)
                    {
                        continue;
                    }
                    var colDefFactory = new FrameworkElementFactory(typeof(ColumnDefinition));
                    if (controlType != typeof(ToggleButtonEx) || colDef.Style is null)
                    {
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
                            var headerColDef = new ColumnDefinition();
                            /*if (colDef.HeaderWidth > colDef.Width)
                            {
                                var col_width = colDef.HeaderWidth + colDef.ItemMargin.Left + colDef.ItemMargin.Right;
                                colDefFactory.SetValue(ColumnDefinition.WidthProperty, new GridLength(col_width));
                                _headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col_width) });
                            }
                            else
                            {*/
                            var col_width = colDef.Width + colDef.ItemMargin.Left + colDef.ItemMargin.Right;
                            colDefFactory.SetValue(ColumnDefinition.WidthProperty, new GridLength(col_width));
                            _headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col_width) });
                            // }
                        }
                    }
                    Line = 1;
                    gridFactory.AppendChild(colDefFactory);
                    // Add header text
                    bool HasBinding = !string.IsNullOrEmpty(colDef.WidthBinding);
                    bool IsToggle = controlType == typeof(ToggleButtonEx);
                    bool IsCheckBox = controlType == typeof(CheckBox);
                    var va = (IsToggle) ? VerticalAlignment.Center :
                        colDef.HeaderVerticalAlignment;
                    var ha = (IsToggle) ? HorizontalAlignment.Center :
                        colDef.HeaderHorizontalAlignment;
                    var headerTextBlock = new TextBlock
                    {
                        Text = colDef.HeaderText,
                        Margin = colDef.HeaderMargin,
                        Padding = colDef.HeaderPadding,
                        VerticalAlignment = va,
                        HorizontalAlignment = ha
                    };
                    Line = 2;
                    if (IsToggle)
                    {
                        var headerColDef = new ColumnDefinition();
                        headerTextBlock.SetValue(FrameworkElement.WidthProperty, colDef.HeaderWidth);
                        _headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(colDef.HeaderWidth) });
                    }
                    else if (HasBinding)
                    {
                        Line = 3;
                        var binding = new Binding(colDef.WidthBinding)
                        {
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1)
                        };
                        headerTextBlock.SetBinding(FrameworkElement.WidthProperty, binding);

                        // Add matching column definition to header grid
                        var headerColDef = new ColumnDefinition();
                        headerColDef.SetBinding(ColumnDefinition.WidthProperty, binding);
                        _headerGrid.ColumnDefinitions.Add(headerColDef);
                    }
                    else
                    {
                        Line = 4;
                        var headerColDef = new ColumnDefinition();
                        double hdrwidth = (colDef.HeaderWidth > colDef.Width) ? colDef.HeaderWidth : colDef.Width;
                        headerTextBlock.SetValue(FrameworkElement.WidthProperty, hdrwidth);
                        _headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(hdrwidth) });
                    }

                    Grid.SetColumn(headerTextBlock, columnIndex);
                    _headerGrid?.Children.Add(headerTextBlock);
                    Line = 5;
                    var controlFactory = new FrameworkElementFactory(controlType);
                    controlFactory.SetValue(Grid.ColumnProperty, columnIndex);

                    Line = 100;
                    if (controlType == typeof(DatePicker))
                    {
                        Line = 6;
                        SetDataBinding(controlType, controlFactory, colDef);
                        SetFontBinding(controlType, controlFactory, colDef);
                        SetItemHeight(controlType, controlFactory, colDef);
                        controlFactory.SetValue(DatePicker.PaddingProperty, colDef.ItemPadding);
                        controlFactory.SetValue(FrameworkElement.MarginProperty, colDef.ItemMargin);
                        controlFactory.SetValue(FrameworkElement.FocusableProperty, colDef.ItemFocusable);
                        controlFactory.SetValue(Control.VerticalAlignmentProperty, VerticalAlignment.Stretch);

                        
                    }
                    else if (controlType == typeof(ToggleButtonEx))
                    {
                        Line = 7;
                        if (!string.IsNullOrEmpty(colDef.DataField))
                        {
                            var toggleBinding = new Binding(colDef.DataField)
                            {
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                Mode = BindingMode.OneWay
                            };
                            controlFactory.SetBinding(ToggleButtonEx.IsCheckedProperty, new Binding(colDef.DataField));
                        }
                        Line = 8;
                        controlFactory.SetValue(FrameworkElement.WidthProperty,
                            HasValidStyle ? styleHandler.Width : colDef.Width);
                        Line = 9;
                        controlFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty,
                            HorizontalAlignment.Left);  //colDef.ItemHorizontalAlignment
                        Line = 10;
                        gridFactory.SetValue(FrameworkElement.HeightProperty, styleHandler.Height);
                    }

                    else if (controlType == typeof(DateTimePicker))
                    {
                        controlFactory.SetValue(DateTimePicker.FormatProperty, colDef.Format);
                        controlFactory.SetValue(DateTimePicker.KindProperty, colDef.Kind);
                        controlFactory.SetValue(DateTimePicker.TimePickerVisibilityProperty, colDef.TimePickerVisibility);
                        controlFactory.SetValue(DateTimePicker.AllowSpinProperty, colDef.AllowSpin);
                        controlFactory.SetValue(DateTimePicker.AllowTextInputProperty, colDef.AllowTextInput);
                        controlFactory.SetValue(DateTimePicker.AllowSpinProperty, colDef.AllowSpin);
                        controlFactory.SetValue(DateTimePicker.FocusableProperty, colDef.DateTimeFocusable);
                        controlFactory.SetValue(DateTimePicker.TimePickerShowButtonSpinnerProperty, colDef.TimePickerShowButtonSpinner);
                        controlFactory.SetValue(DateTimePicker.CurrentDateTimePartProperty, colDef.CurrentDateTimePart);
                        controlFactory.SetValue(DateTimePicker.MouseWheelActiveTriggerProperty, colDef.MouseWheelActiveTrigger);
                        controlFactory.SetValue(DateTimePicker.IsManipulationEnabledProperty, colDef.ItemIsManipulationEnabled);
                        if (!string.IsNullOrEmpty(colDef.FormatString))
                        {
                            controlFactory.SetValue(DateTimePicker.FormatStringProperty, colDef.FormatString);
                        }
                        if (!string.IsNullOrEmpty(colDef.TimeFormatString))
                        {
                            controlFactory.SetValue(DateTimePicker.TimeFormatStringProperty, colDef.TimeFormatString);
                        }

                        if (!double.IsNaN(colDef.DateTimeMinWidth))
                        {
                            controlFactory.SetValue(DateTimePicker.MinWidthProperty, colDef.DateTimeMinWidth);
                        }

                    }
                    else if (controlType == typeof(CheckBox))
                    {
                        SetDataBinding(controlType, controlFactory, colDef);
                        controlFactory.SetValue(CheckBox.PaddingProperty, colDef.ItemPadding);
                        SetFontBinding(controlType, controlFactory, colDef);
                        SetVerticalAlignmentBinding(controlType, controlFactory, colDef);
                        SetHorizontalAlignmentBinding(controlType, controlFactory, colDef);
                        SetWidthBinding(controlType, controlFactory, colDef);
                       SetItemHeight(controlType, controlFactory, colDef);
                        controlFactory.SetValue(FrameworkElement.MarginProperty, colDef.ItemMargin);
                        controlFactory.SetValue(FrameworkElement.HeightProperty, colDef.ToggleButtonHeight);
                        controlFactory.SetValue(FrameworkElement.FocusableProperty, colDef.ItemFocusable);
                        controlFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding(colDef.DataField));
                        controlFactory.SetValue(FrameworkElement.IsManipulationEnabledProperty, 
                            colDef.ItemIsManipulationEnabled);

                        if (!IsToggle && IsCheckBoxInColumn)
                        {
                            gridFactory.SetValue(FrameworkElement.HeightProperty, double.NaN);
                        }
                    }   
                    else if (controlType == typeof(Button))
                    {
                        controlFactory.SetValue(Button.PaddingProperty, colDef.ItemPadding);
                        SetVerticalAlignmentBinding(controlType, controlFactory, colDef);
                        SetHorizontalAlignmentBinding(controlType, controlFactory, colDef);
                        controlFactory.SetValue(FrameworkElement.MarginProperty, colDef.ItemMargin);
                        controlFactory.SetValue(FrameworkElement.FocusableProperty, colDef.ItemFocusable);
                        SetItemHeight(controlType, controlFactory, colDef);
                        controlFactory.SetValue(FrameworkElement.IsManipulationEnabledProperty, colDef.ItemIsManipulationEnabled);
                        if (colDef.ItemImageSource is not null)
                        {
                            var imageFactory = new FrameworkElementFactory(typeof(System.Windows.Controls.Image));
                            imageFactory.SetValue(Image.SourceProperty, colDef.ItemImageSource);
                            imageFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                            imageFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
                            controlFactory.AppendChild(imageFactory);
                        }
                        SetFontBinding(controlType, controlFactory, colDef);
                    }
                    else if (controlType == typeof(ComboBox))
                    {
                        controlFactory.SetValue(ComboBox.PaddingProperty, colDef.ItemPadding);
                        controlFactory.SetValue(CheckBox.MarginProperty, colDef.ItemMargin);
                        controlFactory.SetValue(ComboBox.PaddingProperty, colDef.ItemPadding);
                        SetFontBinding(controlType, controlFactory, colDef);
                        SetMinWidthBinding(controlType, controlFactory, colDef);
                        controlFactory.SetValue(FrameworkElement.MarginProperty, colDef.ItemMargin);
                        controlFactory.SetValue(FrameworkElement.FocusableProperty, colDef.ItemFocusable);
                        SetItemHeight(controlType, controlFactory, colDef);
                        SetDataBinding(controlType, controlFactory, colDef);
                    }
                    else if (controlType == typeof(TextBlock))
                    {
                        SetDataBinding(controlType, controlFactory, colDef);
                        SetFontBinding(controlType, controlFactory, colDef);
                        SetVerticalAlignmentBinding(controlType, controlFactory, colDef);
                        SetHorizontalAlignmentBinding(controlType, controlFactory, colDef);
                        SetWidthBinding(controlType, controlFactory, colDef);
                        controlFactory.SetValue(TextBlock.PaddingProperty, colDef.ItemPadding);
                        controlFactory.SetValue(FrameworkElement.MarginProperty, colDef.ItemMargin);
                        controlFactory.SetValue(FrameworkElement.FocusableProperty, colDef.ItemFocusable);
                        controlFactory.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
                        controlFactory.SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.ClearType);
                        SetItemHeight(controlType,controlFactory,colDef);

                        // Set horizontal text alignment
                        //controlFactory.SetValue(TextBlock.TextAlignmentProperty, textAlignment);
                        // Set vertical text alignment
                        controlFactory.SetValue(TextBlock.LineStackingStrategyProperty, LineStackingStrategy.BlockLineHeight);
                        //controlFactory.SetValue(TextBlock.TextAlignmentProperty, textAlignment);
                    }
                    else if (controlType == typeof(TextBox))
                    {
                        SetDataBinding(controlType, controlFactory, colDef);
                        SetFontBinding(controlType, controlFactory, colDef);
                        SetVerticalAlignmentBinding(controlType, controlFactory, colDef);
                        SetHorizontalAlignmentBinding(controlType, controlFactory, colDef);
                        SetWidthBinding(controlType, controlFactory, colDef);
                        SetItemHeight(controlType, controlFactory, colDef);

                    }

                    // Handle specific control types

                    if (ItemHeightProperty is not null && !IsCheckBoxInColumn)
                    {
                        gridFactory.SetValue(FrameworkElement.HeightProperty, (IsToggle) ? _ItemHeight : ItemHeight);
                    }

                    if (IsCheckBoxInColumn)
                    {
                        gridFactory.SetValue(FrameworkElement.HeightProperty, double.NaN);
                    }

                    gridFactory.AppendChild(controlFactory);
                    columnIndex++;
                }

                var itemTemplate = new DataTemplate { VisualTree = gridFactory };
                lstBoxUploadItems.ItemTemplate = itemTemplate;
                // Apply any pending column definitions
                if (_pendingColumnDefinitions != null)
                {
                    ColumnDefinitions = _pendingColumnDefinitions;
                    _pendingColumnDefinitions = null;
                }
                ApplyColumnDefinitions();
                ApplyVisualProperties();



                
                // Give the template a chance to apply
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    lstBoxUploadItems.UpdateLayout();
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
                Debug.WriteLine($"Error in MultiListbox_Loaded: {ex} {Line}");
            }
        }

        public DispatcherTimer _initializationTimer;

        private void InitializeGrid(Grid grid)
        {
            try
            {
               
                int i = -1;
                foreach (var child in grid.Children)
                {
                    i++;
                    var colDef = ColumnDefinitions[i];
                    if (colDef == null) continue;
                    if (child is ToggleButtonEx ToggleButtonEx)
                    {
                        ToggleButtonEx.HorizontalAlignment = HorizontalAlignment.Left;
                        ToggleButtonEx.VerticalAlignment = VerticalAlignment.Center;
                        ToggleButtonEx.Visibility = Visibility.Visible;
                        ToggleButtonEx.Style = (colDef?.Style is not null && colDef.Style.TargetType == typeof(ToggleButtonEx)) ? colDef.Style : null;
                        HandleClickEvents<ToggleButtonEx>(ToggleButtonEx, colDef);
                    }
                    else if (child is TextBox textBox)
                    {
                        HandleInitialized<TextBox>(textBox, colDef);
                        HandleFocusEvents<TextBox>(textBox, colDef);
                        SetCustomBindings<TextBox>(textBox, colDef);
                    }
                    else if (child is ComboBox comboBox)
                    {
                        HandleInitialized<ComboBox>(comboBox, colDef);
                        HandleSelectionChangedEvent<ComboBox>(comboBox, colDef);
                        HandleFocusEvents<ComboBox>(comboBox, colDef);
                        SetCustomBindings<ComboBox>(comboBox, colDef);
                        HandleSelectionEvents<ComboBox>(comboBox, colDef);


                    }
                    else if (child is CheckBox checkBox)
                    {
                        HandleInitialized<CheckBox>(checkBox, colDef);
                        HandleClickEvents<CheckBox>(checkBox, colDef);
                        SetCustomBindings<CheckBox>(checkBox, colDef);
                        //checkBox.Height = 30.0;
                        checkBox.VerticalContentAlignment = colDef.ItemVerticalAlignment;
                        checkBox.Margin = colDef.ItemMargin;
                        
                    }
                    else if (child is TextBlock textBlock)
                    {
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
                        SetCustomBindings<Label>(label, colDef);
                    }
                    else if (child is Button button)
                    {
                        HandleClickEvents<Button>(button, colDef);
                        SetCustomBindings<Button>(button, colDef);
                    }
                    else if (child is System.Windows.Controls.Image image)
                    {
                        HandleInitialized<Image>(image, colDef);
                        SetCustomBindings<Image>(image, colDef);
                    }
                    else if (child is ProgressBar progressBar)
                    {
                        SetCustomBindings<ProgressBar>(progressBar, colDef);
                    }
                    else if (child is Slider slider)
                    {
                        SetCustomBindings<Slider>(slider, colDef);
                    }
                    else if (child is PasswordBox passwordBox)
                    {
                        HandleFocusEvents<PasswordBox>(passwordBox, colDef);
                        SetCustomBindings<PasswordBox>(passwordBox, colDef);
                    }
                    else if (child is TimePicker timePicker)
                    {
                        SetCustomBindings<TimePicker>(timePicker, colDef);
                    }
                    else if (child is DatePicker datePicker)
                    {
                        SetCustomBindings<DatePicker>(datePicker, colDef);
                    }
                    else if (child is Xceed.Wpf.Toolkit.DateTimePicker dateTimePicker)
                    {
                        HandleInitialized<DateTimePicker>(dateTimePicker, colDef);
                        HandleFocusEvents<DateTimePicker>(dateTimePicker, colDef);
                        SetCustomBindings<DateTimePicker>(dateTimePicker, colDef);

                        // set
                        dateTimePicker.Format = colDef.Format;
                        dateTimePicker.TimePickerVisibility = colDef.TimePickerVisibility;
                        dateTimePicker.Kind = colDef.Kind;
                        dateTimePicker.TimeFormat = colDef.TimeFormat;
                        dateTimePicker.AllowSpin = colDef.AllowSpin;
                        dateTimePicker.AllowTextInput = colDef.AllowTextInput;
                        dateTimePicker.TimePickerAllowSpin = colDef.AllowSpin;
                        dateTimePicker.TimePickerShowButtonSpinner = colDef.TimePickerShowButtonSpinner;
                        dateTimePicker.TimePickerVisibility = colDef.TimePickerVisibility;
                        dateTimePicker.CurrentDateTimePart = colDef.CurrentDateTimePart;
                        dateTimePicker.MouseWheelActiveTrigger = colDef.MouseWheelActiveTrigger;
                        dateTimePicker.IsManipulationEnabled = colDef.ItemIsManipulationEnabled;
                        dateTimePicker.Focusable = colDef.DateTimeFocusable;
                        if (!string.IsNullOrEmpty(colDef.FormatString))
                        {
                            dateTimePicker.FormatString = colDef.FormatString;
                        }
                        if (!string.IsNullOrEmpty(colDef.TimeFormatString))
                        {
                            dateTimePicker.TimeFormatString = colDef.TimeFormatString;
                        }

                        if (!double.IsNaN(colDef.DateTimeMinWidth))
                        {
                            dateTimePicker.MinWidth = colDef.DateTimeMinWidth;
                        }
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
                    b.SelectionChanged += (s, e) =>
                    {
                        colDef.RaiseSelectionChanged(s, e);
                       // SelectionChanged?.Invoke(s, e);
                    };
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Error in HandleSelectionChangedEvent", RaiseError);
            }
        }



        private void HandleClickEvents<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            try
            {
                if (control is Button b)
                {
                    b.Click += (s, e) =>
                    {
                        colDef.RaiseClick(s, e);
                        
                    };
                }
                else if (control is CheckBox c)
                {
                    c.Click += (s, e) =>
                    {
                        colDef.RaiseClick(s, e);
                        
                    };
                }
                else if (control is ToggleButtonEx r)
                {
                    r.Click += (s, e) =>
                    {
                        colDef.RaiseClick(s, e);
                        
                    };


                }
                else if (control is Xceed.Wpf.Toolkit.DateTimePicker dtp)
                {
                    dtp.KeyUp += (s, e) =>
                    {
                        colDef.RaiseKeyUpEvent(s, e);
                    };


                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HandleClickEvents: {ex}");
            }
        }

        private void HandleSelectionEvents<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            try
            {
                if (control is Button b)
                {
                    
                }
                else if (control is ComboBox cmb)
                {
                    cmb.SelectionChanged += (s, e) =>
                    {
                        colDef.RaiseSelectionChanged(s, e);
                    };
                    cmb.DropDownClosed += (s, ef) =>
                    {
                        colDef.RaiseDropDownClosed(s, ef);
                    };

                    
                }
                else if (control is CheckBox c)
                {
                   
                }
                else if (control is ToggleButtonEx r)
                {
                   


                }
                else if (control is Xceed.Wpf.Toolkit.DateTimePicker dtp)
                {
                    


                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HandleSelectionEvents: {ex}");
            }
        }

      

        public void unlockToggleBox(int id, int toggletype, bool state, int row)
        {
            try
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
                                if (grid != null && grid.Children.Count > 0)
                                {
                                    if (grid.Children[0] is TextBlock txt && txt.Text != id.ToString())
                                    {
                                        continue;
                                    }
                                    var toggle = grid.Children.OfType<ToggleButtonEx>().ElementAtOrDefault(row);

                                    if (toggle != null)
                                    {
                                        toggle.LockToggle = false;
                                        toggle.IsChecked = state;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in unlockToggleBox: {ex}");
            }
        }

        private void HandleInitialized<T>(T control, MultiListboxColumnDefinition colDef) where T : FrameworkElement
        {
            try
            {


                control.Loaded += (s, e) =>
                {
                    colDef.InitializedEvent(s, e);
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
                control.LostFocus += (s, e) =>
                {
                    colDef.LostFocusEvent(s, e);
                };

                control.GotFocus += (s, e) =>
                {
                    colDef.GotFocusEvent(s, e);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HandleFocusEvents: {ex}");
            }
        }
        private void SetDataBinding(Type? control, FrameworkElementFactory controlFactory,
          MultiListboxColumnDefinition? colDef)
        {
            try
            {
                if (colDef is null || controlFactory is null || control is null) return;
                var dp = (control == typeof(TextBox)) ? TextBox.TextProperty :
                         (control == typeof(Label)) ? Label.ContentProperty :
                         (control == typeof(DatePicker)) ? DatePicker.SelectedDateProperty :
                         (control == typeof(TimePicker)) ? TimePicker.TextProperty :
                          (control == typeof(ComboBox)) ? ComboBox.TagProperty :
                         (control == typeof(TextBlock)) ? TextBlock.TextProperty : null;
                if (dp is null || string.IsNullOrEmpty(colDef.DataField)) return;
                controlFactory.SetBinding(dp, new Binding(colDef.DataField));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetDataBinding {MethodBase.GetCurrentMethod()?.Name} {this}");
            }
        }

        private void SetItemHeight(Type? control, FrameworkElementFactory controlFactory,
          MultiListboxColumnDefinition? colDef)
        {
            try
            {
                if (colDef is null || controlFactory is null || control is null) return;
                var dp = (control == typeof(TextBox)) ? TextBox.HeightProperty :
                         (control == typeof(Label)) ? Label.HeightProperty :
                         (control == typeof(DatePicker)) ? DatePicker.HeightProperty :
                         (control == typeof(TimePicker)) ? TimePicker.HeightProperty :
                         (control == typeof(CheckBox)) ? CheckBox.HeightProperty :
                         (control == typeof(TextBlock)) ? TextBlock.HeightProperty : null;
                if (dp is null) return;
                if (ItemHeightProperty is not null)
                {
                    controlFactory.SetValue(dp, (double)GetValue(ItemHeightProperty));
                }
                else controlFactory.SetValue(dp, colDef.Height);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetDataBinding {MethodBase.GetCurrentMethod()?.Name} {this}");
            }
        }

        private void SetMinWidthBinding(Type? control, FrameworkElementFactory controlFactory,
          MultiListboxColumnDefinition? colDef)
        {
            try
            {
                if (colDef is null || controlFactory is null || control is null) return;
                var dp = (control == typeof(TextBox)) ? TextBox.MinHeightProperty :
                         (control == typeof(Label)) ? Label.MinHeightProperty :
                         (control == typeof(DatePicker)) ? DatePicker.MinHeightProperty :
                         (control == typeof(TimePicker)) ? TimePicker.MinHeightProperty :
                         (control == typeof(CheckBox)) ? CheckBox.MinHeightProperty :
                          (control == typeof(ComboBox)) ? CheckBox.MinHeightProperty :
                         (control == typeof(TextBlock)) ? TextBlock.MinHeightProperty : null;
                if (dp is null) return;
                if (!string.IsNullOrEmpty(colDef.MinHeightBinding))
                {
                    controlFactory.SetValue(ComboBox.MinHeightProperty,
                      new Binding(colDef.MinHeightBinding));
                }
                else controlFactory.SetValue(dp, colDef.MinHeight);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetMinWidthBinding {MethodBase.GetCurrentMethod()?.Name} {this}");
            }
        }
        private void SetHorizontalAlignmentBinding(Type? control, FrameworkElementFactory controlFactory,
          MultiListboxColumnDefinition? colDef)
        {
            try
            {
                if (colDef is null || controlFactory is null || control is null) return;
                var dp = (control == typeof(TextBox)) ? TextBox.HorizontalAlignmentProperty :
                         (control == typeof(Label)) ? Label.HorizontalAlignmentProperty :
                         (control == typeof(DatePicker)) ? DatePicker.HorizontalAlignmentProperty :
                         (control == typeof(TimePicker)) ? TimePicker.HorizontalAlignmentProperty :
                         (control == typeof(CheckBox)) ? CheckBox.HorizontalAlignmentProperty :
                          (control == typeof(ComboBox)) ? CheckBox.HorizontalAlignmentProperty :
                         (control == typeof(TextBlock)) ? TextBlock.HorizontalAlignmentProperty : null;
                if (dp is null) return;
                if (!string.IsNullOrEmpty(colDef.ContentHorizontalAlignmentBinding))
                {
                    controlFactory.SetBinding(dp, new Binding(colDef.ContentHorizontalAlignmentBinding));
                }
                else if (ItemsHorizontalAlignmentProperty is not null)
                {
                    controlFactory.SetValue(dp, (HorizontalAlignment)GetValue(ItemsHorizontalAlignmentProperty));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetHorizontalAlignmentBinding {MethodBase.GetCurrentMethod()?.Name} {this}");
            }
        }

        private void SetVerticalAlignmentBinding(Type? control, FrameworkElementFactory controlFactory,
          MultiListboxColumnDefinition? colDef)
        {
            try
            {
                if (colDef is null || controlFactory is null || control is null) return;
                var dp = (control == typeof(TextBox)) ? TextBox.VerticalAlignmentProperty :
                         (control == typeof(Label)) ? Label.VerticalAlignmentProperty :
                         (control == typeof(DatePicker)) ? DatePicker.VerticalAlignmentProperty :
                         (control == typeof(TimePicker)) ? TimePicker.VerticalAlignmentProperty :
                         (control == typeof(CheckBox)) ? CheckBox.VerticalAlignmentProperty :
                          (control == typeof(ComboBox)) ? CheckBox.VerticalAlignmentProperty :
                         (control == typeof(TextBlock)) ? TextBlock.VerticalAlignmentProperty : null;
                if (dp is null) return;
                if (!string.IsNullOrEmpty(colDef.ContentVerticalAlignmentBinding))
                {
                    controlFactory.SetBinding(dp, new Binding(colDef.ContentVerticalAlignmentBinding));
                }
                else if (ItemsVerticalAlignmentProperty is not null)
                {
                    controlFactory.SetValue(dp, (VerticalAlignment)GetValue(ItemsVerticalAlignmentProperty));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetVerticalAlignmentBinding {MethodBase.GetCurrentMethod()?.Name} {this}");
            }
        }

        private void SetWidthBinding(Type? control, FrameworkElementFactory controlFactory,
         MultiListboxColumnDefinition? colDef)
        {
            try
            {
                if (colDef is null || controlFactory is null || control is null) return;
                var dp = (control == typeof(TextBox)) ? TextBox.WidthProperty :
                         (control == typeof(Label)) ? Label.WidthProperty :
                         (control == typeof(DatePicker)) ? DatePicker.WidthProperty :
                         (control == typeof(TimePicker)) ? TimePicker.WidthProperty :
                         (control == typeof(CheckBox)) ? CheckBox.WidthProperty :
                          (control == typeof(ComboBox)) ? CheckBox.WidthProperty :
                         (control == typeof(TextBlock)) ? TextBlock.WidthProperty : null;
                if (dp is null) return;
                if (!string.IsNullOrEmpty(colDef.WidthBinding))
                {
                    var widthBinding = new Binding(colDef.WidthBinding)
                    {
                        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1)
                    };
                    controlFactory.SetBinding(dp, widthBinding);
                }
                else
                {
                    controlFactory.SetValue(dp, colDef.Width);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetWidthBinding {MethodBase.GetCurrentMethod()?.Name} {this}");
            }
        }
        private void SetFontBinding(Type? control, FrameworkElementFactory controlFactory,
            MultiListboxColumnDefinition? colDef)
        {
            try
            {
                if (colDef is null || controlFactory is null || control is null) return;
                var dp = (control == typeof(ComboBox)) ? ComboBox.FontSizeProperty :
                         (control == typeof(TextBox)) ? TextBox.FontSizeProperty :
                         (control == typeof(DateTimePicker)) ? DateTimePicker.FontSizeProperty :
                         (control == typeof(Label)) ? Label.FontSizeProperty :
                         (control == typeof(ProgressBar)) ? ProgressBar.FontSizeProperty :
                         (control == typeof(DatePicker)) ? DatePicker.FontSizeProperty :
                         (control == typeof(TimePicker)) ? TimePicker.FontSizeProperty :
                         (control == typeof(PasswordBox)) ? PasswordBox.FontSizeProperty :
                         (control == typeof(CheckBox)) ? CheckBox.FontSizeProperty :
                         (control == typeof(TextBlock)) ? TextBlock.FontSizeProperty : null;
                if (dp is null) return;
                if (!string.IsNullOrEmpty(colDef.TextualFontSizeBinding))
                {
                    controlFactory.SetBinding(dp, new Binding(colDef.TextualFontSizeBinding));
                }
                else if (FontSizeProperty is not null)
                {
                    controlFactory.SetValue(dp, (double)GetValue(FontSizeProperty));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetFontBinding {MethodBase.GetCurrentMethod()?.Name} {this}");
            }
        }

        private void SetCustomBindings<T>(T control, MultiListboxColumnDefinition? colDef) where T : FrameworkElement
        {
            try
            {
                if (colDef == null) return;
                if (control is Button btn)
                {
                    control.KeyUp += (s, e) =>
                    {
                        colDef.RaiseKeyUpEvent(s, e);
                    };
                }
                else if (control is Xceed.Wpf.Toolkit.DateTimePicker dtp)
                {
                    control.KeyUp += (s, e) =>
                    {
                        colDef.RaiseKeyUpEvent(s, e);
                    };
                    if (!string.IsNullOrEmpty(colDef.ContentVerticalAlignmentBinding))
                    {
                        control.SetBinding(Control.VerticalAlignmentProperty,
                            new Binding(colDef.ContentVerticalAlignmentBinding));
                    }
                    else if (ItemsVerticalAlignmentProperty is not null)
                    {
                        control.VerticalAlignment = (VerticalAlignment)GetValue(ItemsVerticalAlignmentProperty);
                    }
                }
                else if (control is ToggleButtonEx ToggleButtonEx1)
                {
                    control.Width = colDef.ToggleButtonWidth;
                    control.Height = colDef.ToggleButtonHeight;
                    var binding = new Binding(colDef.DataField)
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.OneWay
                    };
                    BindingOperations.SetBinding(control, ToggleButtonEx.IsCheckedProperty, binding);
                }
                else if (control is CheckBox chkb)
                {
                    control.SetValue(CheckBox.HeightProperty, double.NaN);
                }
                else if (control is ComboBox cmb)
                {
                    control.Initialized += (s, e) =>
                    {
                        colDef.InitializedEvent(s, e);
                    };

                   
                    cmb.FontSize = 14.0;
                    cmb.Width = colDef.Width;
                }
                else if (control is TextBlock textBlock)
                {
                    if (!string.IsNullOrEmpty(colDef.ContentVerticalAlignmentBinding))
                    {
                        control.SetBinding(Control.VerticalAlignmentProperty,
                            new Binding(colDef.ContentVerticalAlignmentBinding));
                    }
                    else if (ItemsVerticalAlignmentProperty is not null)
                    {
                        control.VerticalAlignment = (VerticalAlignment)GetValue(ItemsVerticalAlignmentProperty);
                    }
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
                    else control.Width = colDef.Width;
                }
                else
                {
                    control.Width = colDef.Width;
                    control.Height = colDef.Height;
                    control.MinWidth = colDef.MinWidth;
                    control.MaxWidth = colDef.MaxWidth;
                    control.MaxHeight = colDef.MaxHeight;
                }

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
                if (d is MultiListbox multiListbox && !multiListbox.IsBuilding)
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
                        return typeof(Xceed.Wpf.Toolkit.DateTimePicker);
                    case "togglebuttonex":
                        return typeof(ToggleButtonEx);
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
                typeof(Xceed.Wpf.Toolkit.DateTimePicker),
                typeof(ToggleButtonEx),
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
                        "datetimepicker" => typeof(Xceed.Wpf.Toolkit.DateTimePicker),
                        "ToggleButtonEx" => typeof(ToggleButtonEx),
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
                        return Xceed.Wpf.Toolkit.DateTimePicker.ValueProperty;
                    case "ToggleButtonEx":
                        return ToggleButtonEx.IsCheckedProperty;
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
                int i = 1;
                // Apply visual properties to header text blocks
                foreach (var child in _headerGrid.Children)
                {
                    var colDef = ColumnDefinitions[i];
                    if (child is TextBlock ttxb)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ttxb.Margin = colDef.HeaderMargin;
                            ttxb.Padding = colDef.HeaderPadding;
                            ttxb.HorizontalAlignment = colDef.HeaderHorizontalAlignment;
                            ttxb.VerticalAlignment = colDef.HeaderVerticalAlignment;
                        }));

                    }
                    else if (child is Xceed.Wpf.Toolkit.DateTimePicker dtp)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            dtp.Margin = colDef.HeaderMargin;
                            dtp.Padding = colDef.HeaderPadding;
                            dtp.HorizontalAlignment = colDef.HeaderHorizontalAlignment;
                            dtp.VerticalAlignment = colDef.HeaderVerticalAlignment;
                        }));
                    }
                    else if (child is ComboBox cmb)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            cmb.Margin = colDef.HeaderMargin;
                            cmb.Padding = colDef.HeaderPadding;
                            cmb.HorizontalAlignment = colDef.HeaderHorizontalAlignment;
                            cmb.VerticalAlignment = colDef.HeaderVerticalAlignment;
                        }));
                    }
                    else if (child is Button btn)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            btn.Margin = colDef.HeaderMargin;
                            btn.Padding = colDef.HeaderPadding;
                            btn.Width = colDef.Width;
                            btn.MinWidth = colDef.MinWidth;
                            btn.HorizontalAlignment = colDef.HeaderHorizontalAlignment;
                            btn.VerticalAlignment = colDef.HeaderVerticalAlignment;
                        }));
                    }
                    i++;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ApplyVisualProperties: {ex}");
            }
        }

        private void ApplyVisualProperties2()
        {
            try
            {
                if (_itemGrid == null) return;

                // Apply visual properties to header text blocks

                foreach (var child in _itemGrid.Children.OfType<Xceed.Wpf.Toolkit.DateTimePicker>())
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
                            child.MinHeight = 20.0;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ApplyVisualProperties: {ex}");
            }
        }

        public void ResetColumnDefinitions()
        {
            try
            {
                IsBuilding = true;
                try
                {
                    _columnFactories = null; // Force rebuild of factories
                    _cachedItemTemplate = null; // 
                    ApplyColumnDefinitions();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in ResetColumnDefinitions: {ex}");
                }
            }
            finally
            {
                IsBuilding = false;
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
                StyleHandler styleHandler = new StyleHandler();
                bool HasValidStyle = false;
                foreach (var colDef in ColumnDefinitions)
                {
                    var __controlType = GetControlType(colDef.ComponentType.ToString());
                    // Only add to header grid if there's a header text
                    if (!string.IsNullOrEmpty(colDef.HeaderText))
                    {
                        int stylescnt = -1;
                        bool IsToggle = false;
                        double headerWidth = (colDef.HeaderWidth > colDef.Width) ? colDef.HeaderWidth : colDef.Width;
                        if (__controlType == typeof(ToggleButtonEx)
                            && colDef.Style is not null)
                        {

                            IsToggle = true;
                            //   var ha = HorizontalAlignment.Center :
                            //      colDef.HeaderHorizontalAlignment;
                            if (colDef.Style != null && colDef.Style.TargetType == typeof(ToggleButtonEx))
                            {
                                foreach (SetterBase style in (SetterBaseCollection)colDef.Style.Setters)
                                {
                                    if (style is Setter _style && !_style.Property.Name.ToLower().Contains("template"))
                                    {
                                        stylescnt++;
                                    }
                                }

                                if (stylescnt > -1)
                                {
                                    List<Setter> styles = new List<Setter>();

                                    foreach (SetterBase style in (SetterBaseCollection)colDef.Style.Setters)
                                    {
                                        if (style is Setter _style && !_style.Property.Name.ToLower().Contains("template"))
                                        {
                                            styles.Add(_style);
                                        }
                                    }
                                    styleHandler = new(RaiseError, styles);
                                    headerWidth = colDef.HeaderWidth;

                                    HasValidStyle = styleHandler.Width > 0;
                                }
                            }
                        }


                        ////
                        var headerColumn = new System.Windows.Controls.ColumnDefinition();
                        if (!string.IsNullOrEmpty(colDef.WidthBinding) && !HasValidStyle)
                        {
                            var binding = new Binding(colDef.WidthBinding)
                            {
                                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1)
                            };
                            BindingOperations.SetBinding(headerColumn, System.Windows.Controls.ColumnDefinition.WidthProperty, binding);
                        }
                        else
                        {
                            headerColumn.Width = new GridLength(headerWidth);
                        }




                        _headerGrid.ColumnDefinitions.Add(headerColumn);


                        var headerTextBlock = new TextBlock
                        {
                            Text = colDef.HeaderText

                            //Padding = new Thickness(120,0,0,0)
                        };
                        headerTextBlock.HorizontalAlignment = colDef.HeaderHorizontalAlignment;



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
                       
                            // For ToggleButtonEx, ensure column is at least as wide as the button
                        if (__controlType == typeof(ToggleButtonEx))
                        {
                            if (!double.IsNaN(colDef.ToggleButtonWidth))
                            {
                                itemColumn.Width = new GridLength(Math.Max(colDef.Width, colDef.ToggleButtonWidth));
                                itemColumn.MinWidth = colDef.ToggleButtonWidth;
                            }
                            else
                            {
                                itemColumn.Width = new GridLength(HasValidStyle ? styleHandler.Width : colDef.Width);

                            }
                        }
                        else
                        {
                            double hdrwidth = colDef.Width;
                            itemColumn.Width = new GridLength(hdrwidth);

                        }
                    }

                    _itemGrid.ColumnDefinitions.Add(itemColumn);


                    var controlType = GetControlType(colDef.ComponentType.ToString());
                    var factory = new FrameworkElementFactory(controlType);
                    factory.SetValue(Grid.ColumnProperty, _itemGrid.ColumnDefinitions.Count - 1);
                    if (controlType == typeof(ToggleButtonEx))
                    {
                        factory = new FrameworkElementFactory(typeof(ToggleButtonEx));
                        factory.SetValue(FrameworkElement.NameProperty, "PART_ToggleButtonEx");
                        factory.SetValue(Grid.ColumnProperty, _itemGrid.ColumnDefinitions.Count - 1);

                        // Add Loaded handler to ensure style and visibility
                        factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                        {
                            var ToggleButtonEx = s as ToggleButtonEx;
                            if (ToggleButtonEx != null)
                            {
                                // Apply all visual properties and style after template is applied
                                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                                {
                                    ToggleButtonEx.Width = (HasValidStyle) ? styleHandler.Width : colDef.ToggleButtonWidth;
                                    ToggleButtonEx.Height = (HasValidStyle) ? styleHandler.Height : colDef.ToggleButtonHeight;
                                    ToggleButtonEx.MinWidth = colDef.ToggleButtonWidth;
                                    ToggleButtonEx.MinHeight = colDef.ToggleButtonHeight;
                                    ToggleButtonEx.MaxWidth = colDef.ToggleButtonWidth;
                                    ToggleButtonEx.MaxHeight = colDef.ToggleButtonHeight;
                                    ToggleButtonEx.HorizontalAlignment = HorizontalAlignment.Left;
                                    ToggleButtonEx.VerticalAlignment = VerticalAlignment.Center;
                                    ToggleButtonEx.Visibility = Visibility.Visible;
                                    ToggleButtonEx.Style = (colDef?.Style is not null && colDef.Style.TargetType == typeof(ToggleButtonEx)) ? colDef.Style : null;

                                    if (ToggleButtonEx.Template != null)
                                    {
                                        ToggleButtonEx.ApplyTemplate();
                                        var image = ToggleButtonEx.Template.FindName("PART_Image", ToggleButtonEx) as Image;
                                        ToggleButtonEx.Checked += (s2, e2) =>
                                        {
                                            var img = ToggleButtonEx.Template.FindName("PART_Image", ToggleButtonEx) as Image;
                                        };
                                        ToggleButtonEx.Unchecked += (s2, e2) =>
                                        {
                                            var img = ToggleButtonEx.Template.FindName("PART_Image", ToggleButtonEx) as Image;
                                            if (img != null)
                                            {
                                                img.Source = null;
                                            }
                                        };
                                    }
                                }));
                            }
                        }));

                        factory.AddHandler(ToggleButtonEx.ClickEvent, new RoutedEventHandler((s, e) =>
                        {
                            var toggle = s as ToggleButtonEx;
                            //colDef.RaiseClick(s, e);
                            /// Click?.Invoke(s, e);
                        }));
                    }
                    else if (controlType == typeof(TextBlock))
                    {
                        factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                        {
                            var textBlock = s as TextBlock;
                            if (textBlock != null)
                            {
                                if (!string.IsNullOrEmpty(colDef.TextualFontSizeBinding))
                                {
                                    textBlock.SetBinding(TextBlock.FontSizeProperty,
                                        new Binding(colDef.TextualFontSizeBinding));
                                }
                                else if (FontSizeProperty is not null)
                                {
                                    textBlock.SetValue(Control.FontSizeProperty,
                                        (double)GetValue(FontSizeProperty));
                                }
                                textBlock.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
                                textBlock.SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.ClearType);
                            }
                        }));
                    }
                    else if (controlType == typeof(CheckBox))
                    {
                        factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                        {
                            var checkBox = s as CheckBox;
                            if (checkBox != null)
                            {
                                if (!string.IsNullOrEmpty(colDef.DataField))
                                {
                                    checkBox.SetBinding(CheckBox.FontSizeProperty,
                                        new Binding(colDef.DataField));
                                }

                                // Visual properties will be set in MultiListbox_Loaded
                            }
                        }));
                    }
                    else if (controlType == typeof(Button))
                    {

                        if (colDef.ItemImageSource != null)
                        {
                            var ifactory = new FrameworkElementFactory(typeof(Image));
                            ifactory.SetValue(Image.SourceProperty, colDef.ItemImageSource);
                            ifactory.SetValue(Image.WidthProperty, 16.0);
                            ifactory.SetValue(Image.HeightProperty, 16.0);
                            ///factory.AppendChild(ifactory);

                        }
                        factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                        {
                            var button = s as Button;
                            if (button != null)
                            {
                                button.Width = 50.0;
                            }
                        }));
                    }
                    else if (controlType == typeof(ComboBox))
                    {



                        factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                        {
                            var comboBox = s as ComboBox;
                            colDef.InitializedEvent(s, e);
                        }));
                        factory.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler((s, e) =>
                        {
                            var comboBox = s as ComboBox;
                            colDef.RaiseSelectionChanged(s, e);
                        }));
                        factory.AddHandler(ComboBox.LostFocusEvent, new RoutedEventHandler((s, e) =>
                        {
                            var comboBox = s as ComboBox;
                            colDef.LostFocusEvent(s, e);
                        }));
                        factory.AddHandler(ComboBox.GotFocusEvent, new RoutedEventHandler((s, e) =>
                        {
                            var comboBox = s as ComboBox;
                            colDef.GotFocusEvent(s, e);
                        }));
                    }
                    else if (controlType == typeof(Xceed.Wpf.Toolkit.DateTimePicker))
                    {
                        factory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((s, e) =>
                        {
                            var datePicker = s as Xceed.Wpf.Toolkit.DateTimePicker;
                            if (datePicker != null)
                            {
                                datePicker.Format = colDef.Format;
                                datePicker.TimePickerVisibility = colDef.TimePickerVisibility;
                                datePicker.Kind = colDef.Kind;
                                datePicker.TimeFormat = colDef.TimeFormat;
                                datePicker.AllowSpin = colDef.AllowSpin;
                                datePicker.AllowTextInput = colDef.AllowTextInput;
                                datePicker.TimePickerAllowSpin = colDef.AllowSpin;
                                datePicker.TimePickerShowButtonSpinner = colDef.TimePickerShowButtonSpinner;
                                datePicker.TimePickerVisibility = colDef.TimePickerVisibility;
                                datePicker.CurrentDateTimePart = colDef.CurrentDateTimePart;
                                datePicker.MouseWheelActiveTrigger = colDef.MouseWheelActiveTrigger;
                                datePicker.IsManipulationEnabled = colDef.ItemIsManipulationEnabled;
                                datePicker.Focusable = colDef.DateTimeFocusable;
                                if (!string.IsNullOrEmpty(colDef.FormatString))
                                {
                                    datePicker.FormatString = colDef.FormatString;
                                }
                                if (!string.IsNullOrEmpty(colDef.TimeFormatString))
                                {
                                    datePicker.TimeFormatString = colDef.TimeFormatString;
                                }

                                if (!double.IsNaN(colDef.DateTimeMinWidth))
                                {
                                    datePicker.MinWidth = colDef.DateTimeMinWidth;
                                }
                               
                            }
                        }));
                        //factory.AddHandler(Xceed.Wpf.Toolkit.DateTimePicker.LostFocusEvent, new RoutedEventHandler((s, e) => colDef.LostFocus?.Invoke(s, e)));
                       // factory.AddHandler(Xceed.Wpf.Toolkit.DateTimePicker.GotFocusEvent, new RoutedEventHandler((s, e) => DateTimePickerGotFocus?.Invoke(s, e)));
                      //  factory.AddHandler(Xceed.Wpf.Toolkit.DateTimePicker.KeyUpEvent, new KeyEventHandler((s, e) => DateTimePickerKeyUp?.Invoke(s, e)));
                    }
                }



            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ApplyColumnDefinitions: {ex.Message}");
            }

            // Apply visual properties after column definitions are set up

        }


        #region Event Handlers

        public event MouseButtonEventHandler ItemMouseDoubleClick;
        public event RoutedEventHandler ItemLostFocus;
        public event RoutedEventHandler ItemGotFocus;
        public event SelectionChangedEventHandler ItemSelectionChanged;
        //public event SelectionChangedEventHandler SelectionChanged;


        private void lstBoxUploadItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemMouseDoubleClick?.Invoke(sender, e);
        }

        private void lstBoxUploadItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemSelectionChanged?.Invoke(sender, e);
        }

        private void lstBoxUploadItems_MouseWheel(object sender, MouseWheelEventArgs e)
        {

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
