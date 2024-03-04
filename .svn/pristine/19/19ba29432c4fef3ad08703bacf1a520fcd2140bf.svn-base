using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Controls;
using System.Windows.Data;

namespace VideoGui
{
    class VideoDetailsConverter : IMultiValueConverter
    {
        [SupportedOSPlatform("windows")]
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string returnvalue = "";
            bool ischecked = false;
            int tag = (values[2] is string ident) ? ident.ToInt() : -1; // tago
            if (values[1] is string VideoCardName) // text
            {
                if (values[0] is ItemCollection SourceFile) // items of listbox
                {
                    List<VideoCardDetails> SourceFiles = (List<VideoCardDetails>)SourceFile.SourceCollection;
                    foreach (var SourceFileDetails in SourceFiles.Where(SourceFileDetails => SourceFileDetails.VideoCardName == VideoCardName))
                    {
                        switch (tag)
                        {
                            case 2:
                                {
                                    returnvalue = SourceFileDetails.VideoCardMemory != "" ? "" : SourceFileDetails.VideoCardMemory.ToString();
                                    break;
                                }
                            case 3:
                                {
                                    ischecked = false;
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
