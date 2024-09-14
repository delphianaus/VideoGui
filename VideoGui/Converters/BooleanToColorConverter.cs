using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Data;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace VideoGui
{

    public class BooleanToColorConverter : IMultiValueConverter
    {
        [SupportedOSPlatform("windows")]
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool colordef = false;

            int tag = (values[2] is string ident) ? ident.ToInt() : -1; // tag
            if (values[1] is string Jobname)
            {
                if (values[0] is IEnumerable<JobListDetails> JobList)
                {
                    foreach (var Job in JobList.Where(Job => Job.Title == Jobname))
                    {
                        colordef = Job.X264Override;
                    }
                }
            }
            return tag switch
            {
                0 => colordef ? FontStyle.Regular : FontStyle.Bold,
                1 => colordef ? Brushes.Red : new SolidColorBrush(Color.FromRgb(6, 176, 28)),
                _ => false,
            };
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
