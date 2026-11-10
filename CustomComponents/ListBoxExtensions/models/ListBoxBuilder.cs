using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CustomComponents.ListBoxExtensions.models
{
    public class ListBoxBuilder
    {
        private object dataSource;
        private double fontSize = double.NaN, itemHeight = double.NaN;
        private List<MultiListboxColumnDefinition> columns = new List<MultiListboxColumnDefinition>();
        private List<object> menuItems_Title = null, menuItems_ListItems = null;

        public object DataSource { get => dataSource; set => dataSource = value; }
        public double FontSize { get => fontSize; set => fontSize = value; }
        public double ItemHeight { get => itemHeight; set => itemHeight = value; }
        public List<MultiListboxColumnDefinition> Columns { get => columns; set => columns = value; }
        public List<object> MenuItems_Title { get => menuItems_Title; set => menuItems_Title = value; }
        public List<object> MenuItems_ListItems { get => menuItems_ListItems; set => menuItems_ListItems = value; }
        public ListBoxBuilder(object dataSource, List<MultiListboxColumnDefinition> columns, 
            double fontSize = double.NaN, double itemHeight = double.NaN,
            List<object> contextMenuHdrItems = null, List<object> contextMenuItems = null)
        {
            this.dataSource = dataSource;
            this.columns = columns;
            this.fontSize = fontSize;
            this.itemHeight = itemHeight;
            this.menuItems_Title = contextMenuHdrItems;
            this.menuItems_ListItems = contextMenuItems;
        }
    }
}
