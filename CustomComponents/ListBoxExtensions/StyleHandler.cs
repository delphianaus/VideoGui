using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static CustomComponents.delegates;


namespace CustomComponents.ListBoxExtensions
{
    public class StyleHandler
    {
        List<Setter> styles;
        private ErrorHandler ErrorHandler;
        bool HasHeight = false, HasWidth = false;
        public double Height = double.NaN, Width = double.NaN;
        public StyleHandler(ErrorHandler _errorHandler, List<Setter> _styles)
        {
            try
            {
                ErrorHandler = _errorHandler;
                styles = _styles;
                processStyle();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"StyleHandler Constructor Error", ErrorHandler);
            }
        }

        public StyleHandler()
        {

        }
        public void processStyle()
        {
            try
            {
                foreach (var style in styles)
                {
                    if (style.Property != null && style.Value != null && style.Property.Name.ToLower() == "height")
                    {
                        Height = (style.Value is double d) ? d : Height;
                        HasHeight = !double.IsNaN(Height);
                    }
                    if (style.Property != null && style.Value != null && style.Property.Name.ToLower() == "width")
                    {
                        Width = (style.Value is double d) ? d : Width;
                        HasWidth = !double.IsNaN(Width);
                    }
                }
            }
            catch (Exception e)
            {
                e.LogWrite($"StyleHandler Error", ErrorHandler);
            }
        }

    }
}
