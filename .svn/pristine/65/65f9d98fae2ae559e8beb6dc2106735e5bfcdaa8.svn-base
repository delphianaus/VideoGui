using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using VideoGui.Models;

namespace VideoGui
{

    public class ComplexJobConverter : IMultiValueConverter
    {
        [SupportedOSPlatform("windows")]
        
        public (string,bool) FillList<T>(ObservableCollection<T> SourceFiles, int tag=0, string Idx = "") 
        {
            try
            {
                string returnvalue = "";
                bool ischecked = false;


                if (SourceFiles is ObservableCollection<ComplexJobList> cpis)
                {
                    foreach (ComplexJobList cpi1 in cpis.Where(cpd => cpd.Id.ToInt() == Idx.ToInt()))
                    {
                        switch (tag)
                        {
                            case 1:
                                {
                                    List<string> Sources = cpi1.SourceDirectory.Split("\\").ToList();
                                    returnvalue = Sources.LastOrDefault();
                                    break;
                                }
                            case 2:
                                {
                                    returnvalue = Path.GetFileNameWithoutExtension(cpi1.Filename);
                                    break;
                                }
                            case 3:
                                {
                                    if ((cpi1.Start == TimeSpan.Zero) || (cpi1.Is720p) || (cpi1.IsShorts))
                                    {
                                        returnvalue = "";
                                    }
                                    else returnvalue = cpi1.Start.ToFFmpeg().Replace(".000", "");
                                    break;
                                }
                            case 4:
                                {
                                    if ((cpi1.Duration == TimeSpan.Zero) || (cpi1.Is720p) || (cpi1.IsShorts))
                                    {
                                        returnvalue = "";
                                    }
                                    else returnvalue = cpi1.Duration.ToFFmpeg().Replace(".000", "");
                                    break;
                                }

                            case 5:
                                {
                                    returnvalue = "";
                                   
                                    break;
                                }

                            case 6:
                                {
                                    ischecked = cpi1.Is720p;
                                    break;
                                }
                            case 7:
                                {
                                    ischecked = cpi1.IsShorts;
                                    break;
                                }
                            case 8:
                                {
                                    ischecked = cpi1.IsCutTrim;
                                    break;
                                }
                            case 9:
                                {
                                    ischecked = cpi1.IsEncodeTrim;
                                    break;
                                }
                            case 10:
                                {
                                    ischecked = cpi1.IsDeleteMonitoredSource;
                                    break;
                                }
                            case 11:
                                {
                                    ischecked = cpi1.IsPersistentJob;
                                    break;
                                }

                        }
                        break;
                    }
                }

                if (SourceFiles is ObservableCollection<ComplexJobHistory> cpi)
                {
                    foreach (ComplexJobHistory cpi2 in cpi.Where(cpd => cpd.Id.ToInt() == Idx.ToInt()))
                    {
                        switch (tag)
                        {
                            case 1:
                                {
                                    List<string> Sources = cpi2.SourceDirectory.Split("\\").ToList();
                                    returnvalue = Sources.LastOrDefault();
                                    break;
                                }
                            case 2:
                                {
                                    returnvalue = Path.GetFileNameWithoutExtension(cpi2.Filename);
                                    break;
                                }
                            case 3:
                                {
                                    if ((cpi2.Start == TimeSpan.Zero) || (cpi2.Is720p) || (cpi2.IsShorts))
                                    {
                                        returnvalue = "";
                                    }
                                    else returnvalue = cpi2.Start.ToFFmpeg().Replace(".000", "");
                                    break;
                                }
                            case 4:
                                {
                                    if ((cpi2.Duration == TimeSpan.Zero) || (cpi2.Is720p) || (cpi2.IsShorts))
                                    {
                                        returnvalue = "";
                                    }
                                    else returnvalue = cpi2.Duration.ToFFmpeg().Replace(".000", "");
                                    break;
                                }

                            case 5:
                                {
                                    returnvalue = "";
                                    if (cpi2 is ComplexJobHistory CJH)
                                    {
                                        returnvalue = (DateOnly.FromDateTime(DateTime.Now).DayNumber - CJH.DateOfRecord.DayNumber).ToString();
                                    }
                                    break;
                                }

                            case 6:
                                {
                                    ischecked = cpi2.Is720p;
                                    break;
                                }
                            case 7:
                                {
                                    ischecked = cpi2.IsShorts;
                                    break;
                                }
                            case 8:
                                {
                                    ischecked = cpi2.IsCutTrim;
                                    break;
                                }
                            case 9:
                                {
                                    ischecked = cpi2.IsEncodeTrim;
                                    break;
                                }
                            case 10:
                                {
                                    ischecked = cpi2.IsDeleteMonitoredSource;
                                    break;
                                }
                            case 11:
                                {
                                    ischecked = cpi2.IsPersistentJob;
                                    break;
                                }

                        }
                        break;
                    }
                }
                return (returnvalue, ischecked);

            }
            
            catch (Exception ex)
            {
                ex.LogWrite(System.Reflection.MethodBase.GetCurrentMethod().Name);
                return ("", false);
            }
        }
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // 0 Record Number - 1 DestFile, 2 Source Directory - 3 Dest Directory -  4 Start Time -  5 Duration
            // 6 Is720p - 7 IsShorts - 8 IsVideoEncode -  9 IsVideoCut
            string returnvalue = "";
            bool ischecked = false;
            int tag = (values[2] is string ident) ? ident.ToInt() : -1; // tago
            if (values[1] is string Idx) // text
            {
                if (values[0] is ItemCollection SourceFile) // items of listbox
                {
                    bool IsValid = false;
                    if (SourceFile.SourceCollection is ComplexJobList)
                    {
                        ObservableCollection<ComplexJobList> SourceFiles = (ObservableCollection<ComplexJobList>)SourceFile.SourceCollection;
                        (returnvalue, ischecked) = FillList<ComplexJobList>(SourceFiles, tag, Idx);
                    }
                    if (SourceFile.SourceCollection is ComplexJobHistory)
                    {
                        ObservableCollection<ComplexJobHistory> SourceFiles = (ObservableCollection<ComplexJobHistory>)SourceFile.SourceCollection;
                        (returnvalue, ischecked) = FillList<ComplexJobHistory>(SourceFiles, tag, Idx);
                    }
                }
            }
            return tag switch
            {
                1 => returnvalue,
                2 => returnvalue,
                3 => returnvalue,
                4 => returnvalue,
                5 => returnvalue,

                6 => ischecked,
                7 => ischecked,
                8 => ischecked,
                9 => ischecked,
                10 => ischecked,
                11 => ischecked,
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
