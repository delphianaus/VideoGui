using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace VideoGui
{
    public class CompareConverter : IMultiValueConverter
    {
        [SupportedOSPlatform("windows")]

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string returnvalue = "";
            bool ischecked = false;
            int tag = (values[2] is string ident) ? ident.ToInt() : -1; // tag

            if (values[1] is string Jobname) // text
            {
                if (values[0] is ItemCollection SourceFile) // items of listbox
                {
                    List<FileCompares> SourceFiles = (List<FileCompares>)SourceFile.SourceCollection;


                    foreach (var Job in SourceFiles.Where(Job => Job.SourceFile == Jobname))
                    {
                        switch (tag)
                        {
                            case 2:
                                {
                                    returnvalue = (Job.SourceFileLength == -1) ? "" : Job.SourceFileLength.ToString();
                                    break;
                                }
                            case 3:
                                {
                                    returnvalue = (Job.DestFileLength == -1) ? "" : Job.DestFileLength.ToString();
                                    break;
                                }
                            case 4:
                                {
                                    if ((Job.DestFileLength != -1) && (Job.SourceFileLength != -1))
                                    {
                                        ischecked = (Job.DestFileLength == Job.SourceFileLength);
                                        Job.TimesMatch = ischecked;
                                    }
                                    break;
                                }

                        }
                    }
                }
            }

            return tag switch
            {
                2 => returnvalue,
                3 => returnvalue,
                4 => ischecked,
                _ => false,
            };
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            bool parsed = bool.TryParse(value.ToString(), out bool num);
            if (parsed)
            {
                return new object[] { num };
            }
            else throw new NotImplementedException();
        }

    }
}
