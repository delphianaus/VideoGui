using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Controls;
using System.Windows.Data;

namespace VideoGui
{

    class VideoSizeConverter : IMultiValueConverter
    {
        [SupportedOSPlatform("windows")]
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string returnvalue = "";
            bool ischecked = false;
            int tag = (values[2] is string ident) ? ident.ToInt() : -1; // tago
            if (values[1] is string FullFileName) // text
            {
                if (values[0] is ItemCollection SourceFile) // items of listbox
                {
                    List<VideoSizeInfo> SourceFiles = (List<VideoSizeInfo>)SourceFile.SourceCollection;
                    foreach (var SourceFileDetails in SourceFiles.Where(SourceFileDetails => SourceFileDetails.SourceFile == FullFileName))
                    {
                        switch (tag)
                        {
                            case 2:
                                {
                                    returnvalue = SourceFileDetails._SourceFileRes != 0 ? "" : SourceFileDetails._SourceFileRes.ToString();
                                    break;
                                }
                            case 3:
                                {
                                    ischecked = System.IO.File.Exists(SourceFileDetails.CompletedDir + "\\" + SourceFile);
                                    break;
                                }
                            case 4:
                                {
                                    ischecked = SourceFileDetails.ExcludeFile;
                                    break;
                                }
                        }
                    }
                }
            }
            return tag switch
            {
                2 => returnvalue,
                3 => ischecked,
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
