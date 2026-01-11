
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic.Logging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.Security.Principal;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VideoGui.ffmpeg;
using VideoGui.Models.delegates;
using Application = System.Windows.Application;
using Brushes = System.Windows.Media.Brushes;
using FlowDirection = System.Windows.FlowDirection;
using Label = System.Windows.Controls.Label;

namespace VideoGui
{


    public static class Helper
    {
        public static string AsJsonList<T>(List<T> tt)
        {
            return new JavaScriptSerializer().Serialize(tt);
        }
        public static string AsJson<T>(T t)
        {
            return new JavaScriptSerializer().Serialize(t);
        }


        public static List<T> AsObjectList<T>(string tt)
        {
            return new JavaScriptSerializer().Deserialize<List<T>>(tt);
        }
        public static bool IsBetween(this DateTime thisDateTime, DateTime from, DateTime to)
        {
            try
            {
                return (thisDateTime >= from && thisDateTime <= to);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsBetween {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public static bool ContainsAll(this string data, string[] containsall)
        {
            bool res = false;

            

            res = containsall.All(s => data.ToLower().Contains(s.ToLower()));
            return res;
        }
        public static bool ContainsAny(this string data, List<string> containsall)
        {
            bool res = false;
            res = containsall.Any(s => data.Contains(s));
            return res;
        }

        public static bool EndsWithAny(this string data, string[] containsall)
        {
            bool res = false;
            res = containsall.Any(s => data.EndsWith(s));
            return res;
        }
        public static string ToFFmpegFormat(this double number, int decimalPlaces = 1)
        {
            return string.Format(CultureInfo.GetCultureInfo("en-US"), $"{{0:N{decimalPlaces}}}", number);
        }
        public static void SetLabelWidth(this Label labelname)
        {
            try
            {
                labelname.Width = ((FrameworkElement)labelname.Parent).MeasureString(labelname.Name.ToString());
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string ToFFmpeg(this TimeSpan ts)
        {
            int milliseconds = ts.Milliseconds;
            int seconds = ts.Seconds;
            int minutes = ts.Minutes;
            var hours = (int)ts.TotalHours;
            // test
            string hr = $"{hours:D}";
            string res = $"{hours:D}:{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
            if (hr.Length == 1)
            {
                res = $"0{res}";
            }
            return res;
        }

        public static string Escape(this string output)
        {
            if (output == null)
            {
                return output;
            }

            if ((output.Last() == '\"' && output.First() == '\"') || (output.Last() == '\'' && output.First() == '\''))
            {
                output = output.Substring(1, output.Length - 2);
            }

            output = $"\"{output}\"";
            return output;
        }

        public static string Unescape(this string output)
        {
            if (output == null || output.Length < 2)
            {
                return output;
            }
            if ((output.Last() == '\"' && output.First() == '\"') || (output.Last() == '\'' && output.First() == '\''))
            {
                return output.Substring(1, output.Length - 2);
            }

            return output;
        }
        public static void AutoLabel(this Label labelname, string content = "")
        {
            try
            {
                labelname.Parent.Dispatcher.Invoke(() =>
                {
                    labelname.Content = content;
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }


        public static void AutoSizeLabel(this Label labelname, string content = "")
        {
            try
            {
                labelname.Parent.Dispatcher.Invoke(() =>
                {
                    double sZWidth = ((FrameworkElement)labelname.Parent).MeasureString(labelname.Name.ToString(), content);
                    labelname.Width = (labelname.Width < sZWidth) ? sZWidth : labelname.Width;
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public static bool SourceIs4K(this string dir)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string shortsdir = key.GetValueStr("SourceDirectory4K", @"D:\shorts\");
                key?.Close();
                return dir == shortsdir;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

		public static bool SourceIs1440p(this string dir)
		{
			try
			{
				RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
				string shortsdir = key.GetValueStr("SourceDirectory1440p", @"D:\shorts\");
				key?.Close();
				return dir == shortsdir;
			}
			catch (Exception ex)
			{
				ex.LogWrite(MethodBase.GetCurrentMethod().Name);
				return false;
			}
		}
		public static bool SourceIs4KAdobe(this string dir)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string shortsdir = key.GetValueStr("SourceDirectory4KAdobe", @"D:\shorts\");
                key?.Close();
                return dir == shortsdir;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
        public static bool IfContains(this string obj, string data)
        {
            try
            {
                return (data == "") ? true : obj.Contains(data);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
        public static bool NotNullOrEmpty(this string obj)
        {
            return !string.IsNullOrEmpty(obj);
        }
        public static bool IfBetweenTimeSpans(this TimeSpan span1, TimeSpan spanA, TimeSpan spanB)
        {
            try
            {
                bool res = false;
                if (spanA != TimeSpan.Zero && spanB != TimeSpan.Zero)
                {
                    if (span1 >= spanA && span1 <= spanB) return true;
                }
                if (spanA == TimeSpan.Zero && spanB == TimeSpan.Zero) return true;
                if (spanA == TimeSpan.Zero)
                {
                    if (span1 == TimeSpan.Zero)
                        return true;
                    return (span1 <= spanB);
                }
                if (spanB == TimeSpan.Zero)
                {
                    if (span1 == TimeSpan.Zero)
                        return true;
                    return (span1 >= spanA);
                }
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return true;
            }
        }
        public static bool IfBetweenInts(this int obj, int a, int b)
        {
            try
            {
                if ((a == -1) && (b == -1)) return true;
                if (b < a && b != -1 && a != -1)
                {
                    return true;
                }
                else if (a != -1 && b != -1)
                {
                    return (obj >= a && obj <= b);
                }
                else if ((a < 0) && (b > -1))
                {
                    return (obj <= b);
                }
                else if (a > -1 && b < 0)
                {
                    return (obj >= a);
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
        public static Bitmap ConvertToBitmap(this BitmapSource bitmapSource)
        {
            try
            {
                Bitmap bitmap;
                using (var outStream = new MemoryStream())
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(bitmapSource));
                    enc.Save(outStream);
                    bitmap = new Bitmap(outStream);
                }
                return bitmap;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConvertToBitmap {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return null;
            }
        }
        public static string ToHex(this int number)
        {
            return number.ToString("X");
        }
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                    return parent;

                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
        public static void Save(this WriteableBitmap wbitmap, string filename)
        {
            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(wbitmap));
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ToInt(string) {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public static double MeasureString(this FrameworkElement frameworkelement, string labelname, string content = "")
        {
            try
            {
                if (frameworkelement.FindName(labelname) is Label lbl)
                {
                    if (content != "") lbl.Content = content;
                    var formattedText = new FormattedText((string)lbl.Content + "W", CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, new Typeface(lbl.FontFamily, lbl.FontStyle, lbl.FontWeight, lbl.FontStretch),
                    lbl.FontSize, Brushes.Black, new NumberSubstitution(), 1);
                    return formattedText.Width;
                }
                else
                {
                    var test = frameworkelement.FindName(labelname);
                    if (test != null)
                    {

                    }
                    return double.NaN;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return double.NaN;
            }

        }


        public static T AsObject<T>(string t)
        {
            return new JavaScriptSerializer().Deserialize<T>(t);
        }
    }
    public static class Extensions
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {

            if (bitmap == null ) return null;
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        public static Bitmap Resize(this Bitmap SourceImage, int Margin, System.Drawing.Color BackgroundColor, int newWidth = 0, int newHeight = 0)
        {
            try
            {
                int maximumWidth = (newWidth == 0) ? SourceImage.Width : newWidth;
                int maximumHeight = (newHeight == 0) ? SourceImage.Height + Margin : newHeight + Margin;
                int originalWidth = SourceImage.Width;
                int originalHeight = SourceImage.Height;
                int MM = Margin;
                if (MM > SourceImage.Height) MM = SourceImage.Height;
                SourceImage.SetResolution(originalWidth, originalHeight - MM);
                var imageEncoders = ImageCodecInfo.GetImageEncoders();
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                var canvasWidth = maximumWidth;
                var canvasHeight = maximumHeight;
                var newImageWidth = maximumWidth;
                var newImageHeight = maximumHeight;
                var xPosition = 0;
                var yPosition = 0;
                var ratioX = maximumWidth / (double)SourceImage.Width;
                var ratioY = maximumHeight / (double)SourceImage.Height;
                var ratio = ratioX < ratioY ? ratioX : ratioY;
                newImageHeight = (int)(SourceImage.Height * ratio);
                newImageWidth = (int)(SourceImage.Width * ratio);
                xPosition = (int)((maximumWidth - (SourceImage.Width * ratio)) / 2);
                yPosition = 0;
                var thumbnail = new Bitmap(canvasWidth, canvasHeight);
                var graphic = Graphics.FromImage(thumbnail);
                graphic.Clear(BackgroundColor);
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                int ImgHeight = (Margin != 0) ? SourceImage.Height : newImageHeight;
                graphic.DrawImage(SourceImage, xPosition, yPosition, newImageWidth, ImgHeight);
                return thumbnail;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IfContains {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return null;
            }
        }
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                    return parent;

                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
        public static void ApplyMargin(this StatusBar statusBar, int offset = 85)
        {
            try
            {
                double sumWidths = statusBar.Items.OfType<FrameworkElement>().Sum(fe => fe.ActualWidth);
                var closeButton = statusBar.Items.OfType<System.Windows.Controls.Button>().FirstOrDefault(btn => btn.Name.Equals("btnclose", StringComparison.OrdinalIgnoreCase));
                if (closeButton != null)
                {
                    closeButton.Margin = new Thickness(statusBar.ActualWidth - sumWidths - offset, 0, 0, 0);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ApplyMargin {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public static void AddIfNotExists(this List<(string, bool)> list, string Key, bool Input)
        {
            bool found = false;
            foreach (var _ in list.Where(ti => (ti.Item1 == Key) && (ti.Item2 == Input)).Select(ti => new { }))
            {
                found = true;
                break;
            }

            if (!found)
            {
                list.Add((Key, Input));
            }
        }

        public static bool ParseDate(this string Value, out DateOnly DateValue)
        {
            bool res = false;
            res = DateOnly.TryParseExact(Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateValue);
            return res;
        }

        public static bool ParseDate(this string Value, out DateOnly DateValue, string Format)
        {
            bool res = false;
            res = DateOnly.TryParseExact(Value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateValue);
            return res;
        }

        public static string ToBitrate(this float bitrate, bool IsMBit = true)
        {
            string res = "";
            Application.Current.Dispatcher.Invoke(() =>
            {
                res = IsMBit ? Math.Round(bitrate /= 1000, 1).ToString() : Math.Round(bitrate, 0).ToString();
            });
            return res;
        }
        public static string RemoveExt(this string FileName)
        {
            string res = "";
            Application.Current.Dispatcher.Invoke(() =>
            {
                if ((FileName.Substring(1, 1) != ":") && (FileName.Length >= 4) && (FileName[(FileName.Length - 4)..^3] == "."))
                {
                    res = FileName[0..^(FileName.Length - 5)];
                }
                else
                {
                    res = (FileName.Substring(1, 1) != ":") ? FileName : Path.GetFileNameWithoutExtension(Path.GetFileName(FileName));
                }
            });
            return res;
        }
        public static string GetString(this RegistryKey ValueName, string name)
        {
            string res = "";
            if (ValueName.GetValue(name, "") is string returner)
            {
                res = returner;
            }
            return res;
        }


        public static void SetURL(this WebView2 webview, string URL)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    webview.AllowDrop = false;
                    webview.Source = new Uri(URL);
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetURL {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public static void SetURL(this WebView2CompositionControl webview, string URL)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    webview.AllowDrop = false;
                    webview.Source = new Uri(URL);
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SetURL {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public static double ToDouble(this object obj, double defaultint = -1)
        {
            try
            {
                if (obj != null)
                {
                    if (double.TryParse(obj.ToString(), out double result))
                    {
                        return result;
                    }
                    else return defaultint;
                }
                else return 0;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ToDouble(obj) {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return 0;
            }
        }
        public static Double ToDouble(this string obj)
        {
            try
            {
                if (Double.TryParse(obj.ToString(), out Double result))
                {
                    return result;
                }
                else return 0;
            }
            catch (Exception ex)
            {
                ex.LogWrite(" ToDouble() " + MethodBase.GetCurrentMethod().Name); return 0;
            }
        }
        public static bool GetValueBool(this RegistryKey RegKey, string KeyName, bool DefValue = false)
        {
            if (RegKey == null) return DefValue;
            if (!RegKey.GetValueNames().ToList().Contains(KeyName)) return DefValue;
            else return ((string)RegKey.GetValue(KeyName, DefValue)).ToBool();
        }

        public static string[] GetValueStrs(this RegistryKey RegKey, string KeyName)
        {
            string[] res = null;

            if (RegKey == null) return null;
            if (!RegKey.GetValueNames().ToList().Contains(KeyName))
            {
                return null;
            }
            return (string[])RegKey.GetValue(KeyName, RegistryValueKind.MultiString);
        }
        public static string GetValueStr(this RegistryKey RegKey, string KeyName, string DefValue = "")
        {
            string res = "";

            if (RegKey == null) return DefValue;
            if (!RegKey.GetValueNames().ToList().Contains(KeyName))
            {
                return DefValue;
            }
            return (string)RegKey.GetValue(KeyName, DefValue);
        }
        public static float GetValueFloat(this RegistryKey RegKey, string KeyName, float DefValue = 0)
        {
            float res = DefValue;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (RegKey == null) res = DefValue;
                if (!RegKey.GetValueNames().ToList().Contains(KeyName))
                {
                    return DefValue;
                }
                var resx = RegKey.GetValue(KeyName, DefValue);
                if (resx != null)
                {
                    return ((string)resx).ToFloat();
                }
                else return DefValue;
            });
            return res;
        }

        public static int GetValueInt(this RegistryKey RegKey, string name, int defValue = 0)
        {
            int res = defValue;

            if (RegKey == null) res = defValue;
            else if (RegKey.RegistryValueExists(name))
            {
                if (RegKey.GetValue(name, defValue) is int it)
                {
                    res = it;
                }
                else if (RegKey.GetValue(name, defValue) is string SS)
                {
                    res = SS.ToInt();
                }

                else res = defValue;
            }
            else res = defValue;

            return res;
        }

        public static double GetValueDouble(this RegistryKey RegKey, string name, double defValue = 0)
        {
            double res = defValue;

            if (RegKey == null) res = defValue;
            else if (RegKey.RegistryValueExists(name))
            {
                if (RegKey.GetValue(name, defValue) is double it)
                {
                    res = it;
                }
                else if (RegKey.GetValue(name, defValue) is string SS)
                {
                    res = SS.ToDouble();
                }

                else res = defValue;
            }
            else res = defValue;

            return res;
        }
        public static bool RegistryValueExists(this RegistryKey ValueName, string name)
        {
            bool retval = false;

            retval = ValueName.GetValueNames().ToList<string>().Contains(name);

            return retval;
        }

        public static void SetLabelContent(this FrameworkElement frameworkelement, string compname, string textvalue)
        {
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.Label label)
                {
                    frameworkelement.Dispatcher.Invoke(() => { label.Content = textvalue; });
                }
            });
        }
        public static void SetValue(this FrameworkElement frameworkelement, string compname, double value)
        {
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.ProgressBar progressbar)
                {
                    frameworkelement.Dispatcher.Invoke(() => { progressbar.Value = value; });
                }
            });
        }
        public static void AddItems(this FrameworkElement frameworkelement, string compname, string ItemString)
        {
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.ComboBox cmbbox)
                {
                    frameworkelement.Dispatcher.Invoke(() => { cmbbox.Items.Add(ItemString); });
                }
                else if (frameworkelement.FindName(compname) is System.Windows.Controls.ListBox lstbox)
                {
                    frameworkelement.Dispatcher.Invoke(() => { lstbox.Items.Add(ItemString); });
                }
            });
        }
        public static ComboBoxItem GetComboBoxItem(this FrameworkElement frameworkelement, string compname, int index)
        {
            ComboBoxItem res = null;
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.ComboBox cmb)
                {
                    res = (ComboBoxItem)cmb.Items[index];
                }
                else res = null;
            });
            return res;
        }
        public static int GetIndexOf(this FrameworkElement frameworkelement, string compname, string indexstr)
        {
            int res = -1;
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.ComboBox cmb)
                {
                    res = cmb.Items.IndexOf(indexstr);
                }
            });
            return res;
        }
        public static void SetSelectedIndex(this FrameworkElement frameworkelement, string compname, int index)
        {
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.ComboBox cmb)
                {
                    cmb.SelectedIndex = index;
                }
            });
        }

        public static void AddFieldToTable(this string connectionStr, string Table, string Field, string FieldType, object defaultvalue = null)
        {
            try
            {
                string sql = "";
                string sqlb = $"SELECT RDB$FIELD_NAME AS FIELD_NAME FROM RDB$RELATION_FIELDS WHERE " +
                   $"RDB$RELATION_NAME=@P1 AND RDB$FIELD_NAME = @P0;";
                var resx = connectionStr.ExecuteScalar(sqlb, [("@P0", Field.ToUpper()), ("@P1", Table.ToUpper())]);
                if (resx is not null) return;
                if (FieldType == "BLOB")
                {
                    var sq = "ALTER TABLE @P0 ADD @P1 BLOB SUB_TYPE BINARY SEGMENT SIZE 2048 DEFAULT NULL;";
                    connectionStr.ExecuteScalar(sq, [("@P0", Table.ToUpper()), ("@P1", Field.ToUpper())]);
                }
                else if (defaultvalue is null)
                {
                    sql = $"ALTER TABLE @P1 ADD @P0 @P2 NOT NULL;";
                    connectionStr.ExecuteScalar(sql, [("@P0", Field.ToUpper()), ("@P1", Table.ToUpper()),
                        ("@P2", FieldType.ToUpper())]);
                }
                else
                {
                    sql = $"ALTER TABLE @P1 ADD @P0 @P2 NOT NULL DEFAULT @DF NOT NULL;";
                    connectionStr.ExecuteScalar(sql, [("@P0", Field.ToUpper()), ("@P1", Table.ToUpper()),
                        ("@P2", FieldType.ToUpper()), ("@DF", defaultvalue)]);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFieldToTable {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public static void CreateTableIfNotExists(this string connectionStr, string sql)
        {
            try
            {
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();
                    string tableName = sql.Substring(12, sql.IndexOf("(") - 12);
                    var sqlx = $"select * from {tableName};";
                    bool created = true;
                    using (var command = new FbCommand(sqlx, connection))
                    {
                        try
                        {
                            command.ExecuteScalar();
                        }
                        catch (Exception exx)
                        {
                            created = false;
                        }
                    }
                    if (!created)
                    {
                        using (var command = new FbCommand(sql, connection))
                        {
                            command.ExecuteScalar();
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"CreateTableIfNotExists {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public static void CreatePathIfNotExists(this string path)
        {
            if (!Directory.Exists(path) && path.NotNullOrEmpty())
            {
                Directory.CreateDirectory(path);
            }
        }
        public static void ExecuteReader(this string connectionStr, string sql, List<(string, object)>? parameters, OnFirebirdReader Reader)
        {
            try
            {
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();
                    using (var command = new FbCommand(sql, connection))
                    {
                        parameters?.ForEach(x => command.Parameters.AddWithValue(x.Item1, x.Item2));
                        using (var cmd = command.ExecuteReader())

                        {
                            while (cmd.Read())
                            {
                                Reader?.Invoke(cmd);
                            }
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"ExecuteReader {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public static void ExecuteReader(this string connectionStr, string sql, List<(string, object)>? parameters, CancellationTokenSource cts,
            OnFirebirdReader Reader)
        {
            try
            {
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();
                    using (var command = new FbCommand(sql, connection))
                    {
                        parameters?.ForEach(x => command.Parameters.AddWithValue(x.Item1, x.Item2));
                        using (var cmd = command.ExecuteReader())

                        {
                            while (cmd.Read() && !cts.IsCancellationRequested)
                            {
                                Reader?.Invoke(cmd);
                            }
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"ExecuteReader {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public static void ExecuteReader(this string connectionStr, string sql, OnFirebirdReader Reader)
        {
            try
            {
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();
                    using (var command = new FbCommand(sql, connection))
                    {
                        using (var cmd = command.ExecuteReader())
                        {
                            while (cmd.Read())
                            {
                                Reader?.Invoke(cmd);
                            }
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"ExecuteReader {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public static void ExecuteReader(this string connectionStr, string sql, CancellationTokenSource cts,
            OnFirebirdReader Reader)
        {
            try
            {
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();
                    using (var command = new FbCommand(sql, connection))
                    {
                        using (var cmd = command.ExecuteReader())
                        {
                            while (cmd.Read() && !cts.IsCancellationRequested)
                            {
                                Reader?.Invoke(cmd);
                            }
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"ExecuteReader {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public static TResult InvokeWithReturn<TResult>(this databasehook<object> Handler,
            object ThisForm, object tld)
        {
            try
            {
                var result = Handler.Invoke(ThisForm, tld);
                if (result is TResult resultT)
                    return resultT;
                else
                    return default(TResult);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"InvokeWithReturn {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return default(TResult);
            }
        }
        public static object ExecuteScalar(this string connectionStr, string sql, List<(string, object)>? parameters = null)
        {
            try
            {
                object res = null;
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();
                    using (var command = new FbCommand(sql, connection))
                    {
                        parameters?.ForEach(x => command.Parameters.AddWithValue(x.Item1, x.Item2));
                        res = command.ExecuteScalar();
                    }
                    connection.Close();
                }
                return res;
            }
            catch (Exception ex)
            {
                //nsuccessful metadata update\r\nCREATE TABLE YTACTIONS failed\r\nTable YTACTIONS already exists"}
                if (ex.Message.ContainsAll(new[] { "already exists", "failed", "metadata update", "CREATE TABLE" }))
                {
                    return null;
                }
                ex.LogWrite($"ExecuteScalar {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return null;
            }
        }



        public static void ExecuteNonQuery(this string connectionStr, string sql, List<(string, object)>? parameters = null)
        {
            try
            {
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();
                    using (var command = new FbCommand(sql, connection))
                    {
                        parameters?.ForEach(x => command.Parameters.AddWithValue(x.Item1, x.Item2));
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ExecuteNonQuery {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public static int ExecuteNonQuery(this string sql, string connectionStr)
        {
            try
            {
                int res = 0;
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();

                    using (var command = new FbCommand(sql, connection))
                    {
                        res = command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ExecuteNonQuery {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return 0;
            }
        }

        public static void DropTable(this string connectionStr, string tableName)
        {
            try
            {
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var sql = $"drop table {tableName};";
                        using (var command = new FbCommand(sql, connection, transaction))
                        {
                            command.ExecuteScalar();
                        }
                        transaction.Commit();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public static bool TableExists(this string connectionStr, string tableName)
        {
            try
            {
                bool res = false;
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var sql = $"select * from {tableName};";

                        using (var command = new FbCommand(sql, connection, transaction))
                        {
                            try
                            {
                                command.ExecuteScalar();
                                res = true;
                            }
                            catch (Exception exx)
                            {
                                res = false;
                            }

                        }
                    }
                    connection.Close();
                }
                return res;
            }
            catch (Exception ex)
            {
                if (!ex.Message.ContainsAll(new string[] { "unknown", tableName.ToUpper() }))
                {
                    ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                }
                return false;
            }
        }
        public static bool IsEnabled(this FrameworkElement frameworkelement, string compname)
        {
            bool res = false;
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.Button btn)
                {
                    res = btn.IsEnabled;
                }
            });

            return res;
        }
        public static void ListBoxRefresh(this FrameworkElement frameworkelement, string compname)
        {
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.ListBox lstbox)
                {
                    frameworkelement.Dispatcher.Invoke(() =>
                    {
                        lstbox.Items.Refresh();
                    });

                }
            });
        }
        public static int GetCount(this FrameworkElement frameworkelement, string compname)
        {
            int res = -1;
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.ComboBox cmb)
                {
                    if (cmb != null)
                    {
                        if (cmb.Items != null)
                        {
                            res = cmb.Items.Count;
                        }
                    }
                }
            });


            return res;
        }
        public static int SelectedIndex(this FrameworkElement frameworkelement, string compname)
        {
            int res = -1;
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.ComboBox cmb)
                {
                    res = cmb.SelectedIndex;
                }
            });
            return res;
        }

        public static string GetText(this FrameworkElement frameworkelement, string compname)
        {
            string res = "";
            frameworkelement.Dispatcher.Invoke(() =>
            {

                if (frameworkelement.FindName(compname) is System.Windows.Controls.ComboBox cmb)
                {
                    res = cmb.Text.Trim();
                }
            });
            return res;
        }

        public static void SetText(this FrameworkElement frameworkelement, string compname, string TextStr)
        {
            frameworkelement.Dispatcher.Invoke(() =>
            {

                if (frameworkelement.FindName(compname) is System.Windows.Controls.TextBox ttb && ttb.Visibility == Visibility.Visible)
                {
                    ttb.Text = TextStr;
                }
            });
        }
        public static DateTime AtTime(this DateTime thisdate, TimeSpan? AddTime = null)
        {
            try
            {
                if (!AddTime.HasValue) return thisdate;
                return new DateTime(thisdate.Year, thisdate.Month, thisdate.Day, 
                    AddTime.Value.Hours, AddTime.Value.Minutes, AddTime.Value.Seconds);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AtTime {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return thisdate;
            }
        }


        public static void IncreaseProgressValue(this FrameworkElement frameworkelement, string compname)
        {
            try
            {
                frameworkelement.Dispatcher.Invoke(() =>
                {
                    if (frameworkelement.FindName(compname) is System.Windows.Controls.ProgressBar pgs)
                    {
                        if (pgs.Value < pgs.Maximum) pgs.Value++;
                    }

                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public static void ClearContents(this FrameworkElement frameworkelement, string[] companme)
        {
            try
            {
                frameworkelement.Dispatcher.Invoke(() =>
                {
                    foreach (string compname in companme)
                    {
                        if (frameworkelement.FindName(compname) is System.Windows.Controls.Label lbl)
                        {
                            lbl.Content = "";
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string GetContent(this FrameworkElement frameworkelement, string compname)
        {
            try
            {
                string res = "";
                frameworkelement.Dispatcher.Invoke(() =>
                {
                    if (frameworkelement.FindName(compname) is System.Windows.Controls.Label lbl)
                    {
                        res = lbl.Content.ToString();
                    }
                });
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }


        public static async Task CopyToAsync(this Stream source, Stream destination, OnPercentUpdate _DoOnPercent, OnFinish _DoOnFisish, long Max, CancellationToken cancellationToken = default(CancellationToken), int bufferSize = 0x1000)
        {
            try
            {
                var buffer = new byte[bufferSize];
                int bytesRead;
                long totalRead = 0;
                int OldVS = -1;
                while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    totalRead += bytesRead;
                    if (Max > 0)
                    {
                        float pt = (float)totalRead / Max;
                        int vs = (int)(float)(pt * 100);
                        if ((OldVS == -1) || (OldVS != vs))
                        {
                            OldVS = vs;
                            if (vs > 0)
                            {
                                _DoOnPercent?.Invoke(vs);
                            }
                        }
                    }

                }
                _DoOnFisish?.Invoke();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                _DoOnPercent?.Invoke(1000);
            }
        }
        public static void SetContent(this FrameworkElement frameworkelement, string compname, string content)
        {
            try
            {
                frameworkelement.Dispatcher.Invoke(() =>
                {
                    if (frameworkelement.FindName(compname) is System.Windows.Controls.Label lbl)
                    {
                        lbl.Content = content;
                    }

                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }
        public static void SetChecked(this FrameworkElement frameworkelement, string compname, bool isChecked)
        {
            try
            {
                frameworkelement.Dispatcher.Invoke(() =>
                {

                    if (frameworkelement.FindName(compname) is System.Windows.Controls.Primitives.ToggleButton chkx)
                    {
                        chkx.IsChecked = isChecked;
                    }
                    else if (frameworkelement.FindName(compname) is System.Windows.Controls.CheckBox chk)
                    {
                        chk.IsChecked = isChecked;
                    }
                });
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static int GetCmbContentToInt(this FrameworkElement frameworkelement, string compname)
        {
            int res = -1;
            frameworkelement.Dispatcher.Invoke(() =>
            {
                if (frameworkelement.FindName(compname) is System.Windows.Controls.ComboBox chk)
                {
                    if (chk.SelectedIndex == -1) res = -1;
                    else if (chk.Text != "")
                    {
                        int.TryParse(chk.Text, out res);
                    }
                }
            });
            return res;
        }
        public static bool IsChecked(this FrameworkElement frameworkelement, string compname)
        {
            try
            {
                bool res = false;
                frameworkelement.Dispatcher.Invoke(() =>
                {
                    if (frameworkelement.FindName(compname) is System.Windows.Controls.Primitives.ToggleButton chkx)
                    {
                        res = chkx.IsChecked.Value;
                    }
                    else if (frameworkelement.FindName(compname) is System.Windows.Controls.CheckBox chk)
                    {
                        res = chk.IsChecked.Value;
                    }
                    else res = false;
                });
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
        public static RegistryKey OpenSubKey(this string KeyName, RegistryKey RegKey)
        {

            RegistryKey key = null;

            key = RegKey.OpenSubKey(@KeyName, true);
            if (key == null)
            {
                RegKey.CreateSubKey(@KeyName);
                key = Registry.CurrentUser.OpenSubKey(@KeyName, true);
            }

            return key;
        }
        public static TimeSpan FromStrToTimeSpan(this string data)
        {
            try
            {

                TimeSpan ts = TimeSpan.Zero;
                if (data == "" || data is null) return ts;
                List<string> values = data.Split(':').ToList();
                if (values.Count < 3)
                {
                    ts += TimeSpan.FromMinutes(values[0].ToInt());
                    string secs = values[1].ToString();
                    if (secs.Contains("."))
                    {
                        List<string> values2 = secs.Split('.').ToList();
                        ts += TimeSpan.FromSeconds(values2[0].ToInt());
                        ts += TimeSpan.FromMilliseconds(values2[1].ToInt());
                    }
                    else
                    {
                        ts += TimeSpan.FromSeconds(values[1].ToInt());
                    }
                }
                else if (values.Count > 2)
                {
                    ts += TimeSpan.FromHours(values[0].ToInt());
                    ts += TimeSpan.FromMinutes(values[1].ToInt());
                    string secs = values[2].ToString();
                    if (secs.Contains("."))
                    {
                        List<string> values2 = secs.Split('.').ToList();
                        ts += TimeSpan.FromSeconds(values2[0].ToInt());
                        ts += TimeSpan.FromMilliseconds(values2[1].ToInt());
                    }
                    else
                    {
                        ts += TimeSpan.FromSeconds(values[2].ToInt());
                    }
                }
                return ts;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
                return TimeSpan.Zero;
            }
        }
        public static TimeSpan FromFFmpegTime(this string data)
        {
            try
            {
                TimeSpan ts;
                TimeSpan.TryParseExact(data, @"\hh:mm\:ss\.ff", null, out ts);
                return ts;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name.ToString());
                return TimeSpan.Zero;
            }
        }
        public static string ToFFmpegFormat(this VideoSize videoSize)
        {
            switch (videoSize)
            {
                case VideoSize.Ntsc:
                    return "720x480";
                case VideoSize.Pal:
                    return "720x576";
                case VideoSize.Qntsc:
                    return "352x240";
                case VideoSize.Qpal:
                    return "352x288";
                case VideoSize.Sntsc:
                    return "640x480";
                case VideoSize.Spal:
                    return "768x576";
                case VideoSize.Film:
                    return "352x240";
                case VideoSize.NtscFilm:
                    return "352x240";
                case VideoSize.Sqcif:
                    return "128x96";
                case VideoSize.Qcif:
                    return "176x144";
                case VideoSize.Cif:
                    return "352x288";
                case VideoSize._4Cif:
                    return "704x576";
                case VideoSize._16cif:
                    return "1408x1152";
                case VideoSize.Qqvga:
                    return "160x120";
                case VideoSize.Qvga:
                    return "320x240";
                case VideoSize.Vga:
                    return "640x480";
                case VideoSize.Svga:
                    return "800x600";
                case VideoSize.Xga:
                    return "1024x768";
                case VideoSize.Uxga:
                    return "1600x1200";
                case VideoSize.Qxga:
                    return "2048x1536";
                case VideoSize.Sxga:
                    return "1280x1024";
                case VideoSize.Qsxga:
                    return "2560x2048";
                case VideoSize.Hsxga:
                    return "5120x4096";
                case VideoSize.Wvga:
                    return "852x480";
                case VideoSize.Wxga:
                    return "1366x768";
                case VideoSize.Wsxga:
                    return "1600x1024";
                case VideoSize.Wuxga:
                    return "1920x1200";
                case VideoSize.Woxga:
                    return "2560x1600";
                case VideoSize.Wqsxga:
                    return "3200x2048";
                case VideoSize.Wquxga:
                    return "3840x2400";
                case VideoSize.Whsxga:
                    return "6400x4096";
                case VideoSize.Whuxga:
                    return "7680x4800";
                case VideoSize.Cga:
                    return "320x200";
                case VideoSize.Ega:
                    return "640x350";
                case VideoSize.Hd480:
                    return "852x480";
                case VideoSize.Hd720:
                    return "1280x720";
                case VideoSize.Hd1080:
                    return "1920x1080";
                case VideoSize._2K:
                    return "2048x1080";
                case VideoSize._2Kflat:
                    return "1998x1080";
                case VideoSize._2Kscope:
                    return "2048x858";
                case VideoSize._4K:
                    return "4096x2160";
                case VideoSize._4Kflat:
                    return "3996x2160";
                case VideoSize._4Kscope:
                    return "4096x1716";
                case VideoSize.Nhd:
                    return "640x360";
                case VideoSize.Hqvga:
                    return "240x160";
                case VideoSize.Wqvga:
                    return "400x240";
                case VideoSize.Fwqvga:
                    return "432x240";
                case VideoSize.Hvga:
                    return "480x320";
                case VideoSize.Qhd:
                    return "960x540";
                case VideoSize._2Kdci:
                    return "2048x1080";
                case VideoSize._4Kdci:
                    return "4096x2160";
                case VideoSize.Uhd2160:
                    return "3840x2160";
                case VideoSize.Uhd4320:
                    return "7680x4320";
                default:
                    throw new InvalidOperationException();
            }
        }

        public static void LogWrite(this Exception Debugx, string callingmethod = "")
        {
            string m_exePath = Debugger.IsAttached ? GetAppPath() : System.IO.Path.GetDirectoryName( Process.GetCurrentProcess().MainModule.FileName);
            try
            {
                string InternalCallingMethod = MethodBase.GetCurrentMethod().Name.ToString();
                string date = DateTime.Now.ToString("dd_MM_yyyy");
                using var txtWriter = System.IO.File.AppendText(m_exePath + $"\\{date}-log.log");
                txtWriter.Write("\r\n", "Log Entry : {0}", callingmethod);
                txtWriter.WriteLine("{0}", DateTime.Now.ToLongTimeString());
                txtWriter.WriteLine("Error :{0}", Debugx.Message);
                txtWriter.WriteLine("-------------------------------");
                txtWriter.WriteLine("\r\n");
                txtWriter.WriteLine("Stack Trace :{0}", Debugx.StackTrace); 
            }
            catch (Exception ex)
            {
                Debug.Print(MethodBase.GetCurrentMethod().Name.ToString() + " LogWrite" + ex.Message);
            }
        }
        public static int IncompleteCount(this List<Task> TaskList)
        {
            int res = 0;
            Application.Current.Dispatcher.Invoke(() =>
            {
                res = TaskList.Where(downloader => !downloader.IsCompleted).Count();
            });
            return res;
        }
        public static string FindString(this string token, string first, string second)
        {
            if (!token.Contains(first)) return "";
            var afterFirst = token.Split(new[] { first }, StringSplitOptions.None)[1];
            if (!afterFirst.Contains(second)) return "";
            return afterFirst.Split(new[] { second }, StringSplitOptions.None)[0];
        }

        public static string FindBetween(this string source, string left, string right)
        {
            return Regex.Match(
                    source,
                    string.Format("{ 0}(.*){1}", left, right))
                .Groups[1].Value;
        }

        public static async Task<byte[]> DownloadDataTaskAsync(this WebClient webClient, string address, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (cancellationToken.Register(webClient.CancelAsync))
            {
                return await webClient.DownloadDataTaskAsync(address);
            }
        }


        public static string ToHex(this byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }
        public static byte[] Bytecopy(this byte[] bytearray)
        {
            byte[] result = new byte[bytearray.Length];
            bytearray.CopyTo(result, 0);
            return result;
        }

        public static byte[] BytecopyTo(this byte[] bytearray, int endpos)
        {
            byte[] tempbuf = (byte[])bytearray.Skip(0).Take(endpos).ToArray();
            byte[] result = new byte[tempbuf.Length];
            bytearray.CopyTo(result, 0);
            return result;
        }



        public static byte[] BytecopyFrom(this byte[] bytearray, int skippos)
        {
            byte[] tempbuf = (byte[])bytearray.Skip(skippos).ToArray();
            byte[] result = new byte[tempbuf.Length];
            bytearray.CopyTo(result, 0);
            return result;
        }
        //public static T Deserialize<T>(this byte[] byteArray) where T : class
        //{
        //    if (byteArray == null)
        //    {
        //        return null;
        //    }
        //    using (var memStream = new MemoryStream())
        //    {
        //        var binForm = new BinaryFormatter();
        //        memStream.Write(byteArray, 0, byteArray.Length);
        //        memStream.Seek(0, SeekOrigin.Begin);
        //        var obj = (T)binForm.Deserialize(memStream);
        //        return obj;
        //    }
        //}

        public static string QuoteStr(this string obj)
        {
            try
            {
                string res = "";
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    if (obj.StartsWith("\"") && (obj.EndsWith("\""))) res = obj;
                    else res = obj.Contains(" ") ? "\"" + obj + "\"" : obj;
                });
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite(" QuoteStr() " + MethodBase.GetCurrentMethod().Name);
                return obj;
            }
        }
        public static string ForceQuoteStr(this string obj)
        {
            try
            {
                if (obj.StartsWith("\"") && (obj.EndsWith("\""))) return obj;
                else return "\"" + obj + "\"";
            }
            catch (Exception ex)
            {
                ex.LogWrite(" ForceQuoteStr() " + MethodBase.GetCurrentMethod().Name);
                return obj;
            }
        }

        public static bool ToBool(this string obj)
        {
            try
            {
                if (bool.TryParse(obj.ToString(), out bool result))
                {
                    return result;
                }
                else return false;
            }
            catch (Exception ex)
            {
                ex.LogWrite(" ToBool() " + MethodBase.GetCurrentMethod().Name);
                return false;
            }

        }

        public static float ToFloat(this string obj)
        {
            try
            {
                if ((obj != null) && (float.TryParse(obj.ToString(), out float result)))
                {
                    return result;
                }
                else return 0;
            }
            catch (Exception ex)
            {
                ex.LogWrite(" ToFloat() " + MethodBase.GetCurrentMethod().Name);
                return 0;
            }

        }



        public static bool IsAdministrator(this bool obj)
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }
        public static int ToIntYesNo(this string obj)
        {
            try
            {
                if ((obj != null) && (obj.ToString().ToLower() == "yes"))
                {
                    return 1;
                }
                else return 0;
            }
            catch (Exception ex)
            {
                ex.LogWrite(" ToIntYesNo() " + MethodBase.GetCurrentMethod().Name);
                return 0;
            }

        }
        public static int ToInt(this string obj, int def = 0)
        {
            try
            {
                if ((obj != null) && int.TryParse(obj.ToString(), out int result))
                {
                    return result;
                }
                else return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite(" ToInt()  " + MethodBase.GetCurrentMethod().Name);
                return def;
            }

        }

        public static void ExecuteReader(this string connectionStr, string sql, OnFirebirdReader Reader, bool brk = false)
        {
            try
            {
                using (var connection = new FbConnection(connectionStr))
                {
                    connection.Open();
                    using (var command = new FbCommand(sql, connection))
                    {
                        using (var cmd = command.ExecuteReader())
                        {
                            while (cmd.Read())
                            {
                                Reader?.Invoke(cmd);
                                if (brk) break;
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ExecuteReader {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public static int ToInt(this object obj, int def = -1)
        {
            try
            {
                if ((obj != null) && int.TryParse(obj.ToString(), out int result))
                {
                    return result;
                }
                else return def;
            }
            catch (Exception ex)
            {
                ex.LogWrite(" ToInt()  " + MethodBase.GetCurrentMethod().Name);
                return def;
            }

        }
        public static int ToInt(this double obj)
        {
            try
            {
                if ((obj != null) && int.TryParse(obj.ToString(), out int result))
                {
                    return result;
                }
                else return 0;
            }
            catch (Exception ex)
            {
                ex.LogWrite(" ToInt() " + MethodBase.GetCurrentMethod().Name);
                return 0;
            }

        }

        public static string IsNullStr(this string str)
        {
            try
            {
                if (str is null) return "";
                return str;
            }
            catch (Exception ex)
            {
                ex.LogWrite(" IsNullStr() " + MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }

        public static string GetShortName(this string FileName)
        {
            try
            {
                string showname = System.IO.Path.GetFileNameWithoutExtension(FileName);
                string _ShowName = showname.ToLower();
                if (_ShowName.Contains("720p"))
                {
                    int Indx = _ShowName.IndexOf("720p");
                    if (Indx > 0) _ShowName = _ShowName.Substring(0, Indx);
                }
                if (_ShowName.Contains("504p"))
                {
                    int Indx = _ShowName.IndexOf("504p");
                    if (Indx > 0) _ShowName = _ShowName.Substring(0, Indx);
                }
                if (_ShowName.Contains("pdtv"))
                {
                    int Indx = _ShowName.IndexOf("pdtv");
                    if (Indx > 0) _ShowName = _ShowName.Substring(0, Indx);
                }
                if (_ShowName.Contains("xvid"))
                {
                    int Indx = _ShowName.IndexOf("xvid");
                    if (Indx > 0) _ShowName = _ShowName.Substring(0, Indx);
                }
                if (_ShowName.Contains("ch4"))
                {
                    int Indx = _ShowName.IndexOf("ch4");
                    if (Indx == 0) _ShowName = _ShowName.Substring(3 + Indx);
                }

                if (_ShowName.Contains("-yestv"))
                {
                    int Indx = _ShowName.IndexOf("-yestv");
                    _ShowName = _ShowName.Substring(0, Indx);
                }
                if (_ShowName.Contains("yestv"))
                {
                    int Indx = _ShowName.IndexOf("yestv");
                    _ShowName = _ShowName.Substring(0, Indx);
                }
                if (_ShowName.Contains("ch5"))
                {
                    int Indx = _ShowName.IndexOf("ch5");
                    if (Indx == 0) _ShowName = _ShowName.Substring(3 + Indx);
                }
                if (_ShowName.Contains("bbc"))
                {
                    int Indx = _ShowName.IndexOf("bbc");
                    if (Indx == 0) _ShowName = _ShowName.Substring(3 + Indx);
                }
                if (_ShowName.Contains("bbc4."))
                {
                    int Indx = _ShowName.IndexOf("bbc4.");
                    if (Indx == 0) _ShowName = _ShowName.Substring(5 + Indx);
                }


                if (_ShowName.Contains("1440p"))
                {
                    int Indx = _ShowName.IndexOf("1440p");
                    if (Indx > 0) _ShowName = _ShowName.Substring(0, Indx);
                }
                if (_ShowName.Contains("hdtv"))
                {
                    int Indx = _ShowName.IndexOf("hdtv");
                    if (Indx > 0) _ShowName = _ShowName.Substring(0, Indx);
                }

                if (_ShowName.Contains("ac3"))
                {
                    int Indx = _ShowName.IndexOf("ac3");
                    if (Indx > 0) _ShowName = _ShowName.Substring(0, Indx);
                }
                if (_ShowName.Contains("hdcam"))
                {
                    int Indx = _ShowName.IndexOf("hdcam");
                    if (Indx > 0) _ShowName = _ShowName.Substring(0, Indx);
                }

                if (_ShowName.Contains("x264"))
                {
                    int Indx = _ShowName.IndexOf("x264");
                    if (Indx > 0) _ShowName = _ShowName.Substring(0, Indx);
                }
                if (Regex.Match(_ShowName, "^.*s[0-9]{1}e[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "(s)([0-9]{1}e)").Index);
                }
                if (Regex.Match(_ShowName, "^.*[0-9]{1}x[0-9]{1}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "([0-9]{1}x)").Index);
                }
                if (Regex.Match(_ShowName, "^.*[0-9]{2}x[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "([0-9]{2}x)").Index);
                }
                if (Regex.Match(_ShowName, "^.*s[0-9]{2} e[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "s[0-9]{2} e[0-9]{2}.").Index);
                }
                if (Regex.Match(_ShowName, "^.*s[0-9]{2}e[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "(s)([0-9]{2}e)").Index);
                }
                if (Regex.Match(_ShowName, "^.*[0-9]{2}e[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "([0-9]{2}e)").Index);
                }

                if (Regex.Match(_ShowName, "^.*s[0-9]{2} - [0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "(s[0-9]{2} = )").Index);
                }

                //string AA = "Great Continental Railway Journeys - S04 - E03 - Pisa To Lake Garda";

                if (Regex.Match(_ShowName, "^.* - s[0-9]{2} - e[0-9]{2} -*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "( - s[0-9]{2}- e[0-9]{2} -)").Index);
                }

                if (Regex.Match(_ShowName, "^.*[0-9]{1}x[0-9]{1}.*$").Success)
                {
                    int Match = Regex.Match(_ShowName, "([0-9]{1}x)").Index;
                    if (Match > 0) _ShowName = _ShowName.Substring(0, Match);
                }

                if (Regex.Match(_ShowName, "^.*[0-9]{1}.of.[0-9]{1}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "([0-9]{1}.of.[0-9]{1})").Index);
                }

                if (Regex.Match(_ShowName, "^.*s[0-9]{2}x[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "(s)([0-9]{2}x)").Index);
                }

                if (Regex.Match(_ShowName, "^.*s[0-9]{1}e[0-9]{1}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "(s)([0-9]{1}e)").Index);
                }

                if (Regex.Match(_ShowName, "^.* - s[0-9]{2} - e[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "( - s[0-9]{2} - e[0-9]{2})").Index);
                }

                if (Regex.Match(_ShowName, "^.*[0-9]{2}x[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "([0-9]{2}x)").Index);
                }
                if (Regex.Match(_ShowName, "^.*s[0-9]{1}x[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "(s)([0-9]{1}x)").Index);
                }
                if (Regex.Match(_ShowName, "^.*.[0-9]{1}of[0-9]{1}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "(.[0-9]{1}of[0-9]{1})").Index);
                    if (Regex.Match(_ShowName, "^.*[0-9]{1}of[0-9]{1}.*$").Success)
                    {
                        int indx = 4 + Regex.Match(_ShowName, "^.*[0-9]{1}of[0-9]{1}.*$").Index;
                        _ShowName = _ShowName.Substring(indx);
                        if (_ShowName.StartsWith(".") || _ShowName.StartsWith("-"))
                        {
                            _ShowName = _ShowName.Substring(1);
                        }
                    }

                }
                if (Regex.Match(_ShowName, "^.*[0-9]{1} of [0-9]{1}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "([0-9]{1} of [0-9]{1})").Index);
                }
                if (Regex.Match(_ShowName, "^.*.[0-9]{2}of[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "(.[0-9]{2}of[0-9]{2})").Index);
                }
                if (Regex.Match(_ShowName, "^.*series.[0-9]{1}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "(series.[0-9]{1})").Index);
                }
                if (Regex.Match(_ShowName, "^.*series.[0-9]{2}.*$").Success)
                {
                    _ShowName = _ShowName.Substring(0, Regex.Match(_ShowName, "(series.[0-9]{2})").Index);
                }
                if (_ShowName.Length > 5)
                {
                    string lastshow = _ShowName.Substring(_ShowName.Length - 5);
                    if (lastshow.StartsWith(".") && lastshow.EndsWith("."))
                    {
                        if (lastshow.Length > 2)
                        {
                            string a1 = lastshow.Substring(1, lastshow.Length - 2);
                            int bx = -1;
                            if (int.TryParse(a1, out bx))
                            {
                                int inxd = _ShowName.IndexOf(lastshow);
                                if (inxd != -1)
                                {
                                    _ShowName = _ShowName.Substring(0, inxd);
                                }
                            }
                        }
                    }
                }
                _ShowName = _ShowName.Trim();
                if (_ShowName.EndsWith(".")) _ShowName = _ShowName.Substring(0, _ShowName.Length - 1);
                if (_ShowName.EndsWith("-")) _ShowName = _ShowName.Substring(0, _ShowName.Length - 1);
                if (_ShowName.StartsWith(".")) _ShowName = _ShowName.Substring(1);
                if (_ShowName.StartsWith("-")) _ShowName = _ShowName.Substring(1);
                _ShowName = _ShowName.Replace(".", " ").Replace("_", " ");
                if (_ShowName.Contains("series"))
                {
                    int xxx = _ShowName.IndexOf("series");
                    _ShowName = _ShowName.Substring(0, xxx);
                }
                if (Regex.Match(_ShowName, "^.*[0-9]{1}of[0-9]{1}-.*$").Success)
                {
                    int indx = 4 + Regex.Match(_ShowName, "^.*[0-9]{1}of[0-9]{1}-.*$").Index;
                    _ShowName = _ShowName.Substring(indx + 1);
                    if (_ShowName.StartsWith(".") || _ShowName.StartsWith("-"))
                    {
                        _ShowName = _ShowName.Substring(1);
                    }
                }
                int iii = _ShowName.Length;
                if (iii > 4)
                {
                    string news = _ShowName.Substring(iii - 5);
                    if (news.StartsWith(" "))
                    {
                        string news1 = _ShowName.Substring(iii - 4);
                        if (int.TryParse(news1, out int ixi))
                        {
                            if ((ixi < 1700) && (ixi > 200)) _ShowName = _ShowName.Substring(0, iii - 5);
                        }
                    }
                    int iix = _ShowName.Length;
                    if (iix > 4)
                    {
                        news = _ShowName.Substring(iix - 4);
                        string news1w = _ShowName.Substring(iix - 4);
                        if (int.TryParse(news1w, out int ixi1))
                        {
                            if ((ixi1 < 1700) && (ixi1 > 200)) _ShowName = _ShowName.Substring(0, iix - 4);
                        }
                    }
                }

                return _ShowName;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }
        public static object CreateImmutableList(Type elementType)
        {
            // TODO: guard clauses for parameters == null
            var resultType = typeof(ImmutableList<>).MakeGenericType(elementType);
            var result = resultType.GetField("Empty").GetValue(null);
            return result;
        }


        public static void WriteToJsonFile<T>(this T objectToWrite, string filePath, bool append = false)
        {

            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }


        }
        public static string ToDisplayString(this DateTime _LastUploadedDate)
        {
            try
            {
                if (_LastUploadedDate.Year < 2000) return "";
                string data = $"   {_LastUploadedDate.ToString("dd/MM/yyyy")}";
                var start = _LastUploadedDate.TimeOfDay;
                string fillA1 = (start.Minutes < 10) ? "0" : "";
                string fillA2 = (start.Seconds < 10) ? "0" : "";
                string dx = $":{fillA1}{start.Minutes}:{fillA2}{start.Seconds}";
                data += (start.Hours > 12) ? $" {start.Hours - 12}{dx} PM" : $" {start.Hours}{dx} AM";
                return data;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ToDisplayString {MethodBase.GetCurrentMethod().Name} {ex.Message}");
                return "";
            }
        }
        public static string ToPascalCase(this string original, string parameter = "VLINE|NSW|VIC")
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value));
            string newcasestring = "";
            List<string> parameters = new List<string>();
            if (parameter.Contains("|"))
            {
                parameters = parameter.Split('|').ToList();
            }
            else
            {
                parameters.Add(parameter);
            }
            foreach (string sss in pascalCase)
            {
                if (!string.IsNullOrEmpty(sss) && !parameters.Contains(sss.ToUpper()))
                {
                    newcasestring += sss.Substring(0, 1) + sss.Substring(1).ToLower() + " ";
                }
                else
                {
                    newcasestring += sss + " ";
                }
            }
            return newcasestring.Trim();

            //return string.Concat(pascalCase);
        }

        public static string ToCustomTimeString(this TimeSpan span)
        {
            try
            {
                if (Math.Abs(span.TotalDays) > 0.95)
                {
                    return span.Days.ToString("00") + ":" + span.Hours.ToString("00") + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
                }
                else
                {
                    if (Math.Abs(span.TotalHours) > 0.95)
                    {
                        return span.Hours.ToString("00") + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
                    }
                    else
                        return span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
                }

            }
            catch (Exception ex)
            {
                Debug.Print(MethodBase.GetCurrentMethod().Name.ToString() + " LogWrite" + ex.Message);
                return "";
            }
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);


        public static async Task CopyFiles(string server, string source, string dest, string username, string password, IProgress<long> progressCallback, int buffersize = 1048576 * 16)
        {
            try
            {

                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;
                SafeAccessTokenHandle safeAccessTokenHandle;
                bool returnValue = LogonUser(username, ":", password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeAccessTokenHandle);
                if (returnValue)
                {
                    WindowsIdentity.RunImpersonated(
                    safeAccessTokenHandle,
                    async () =>
                    {
                        var from = source;
                        var to = dest;
                        CancellationToken cancellationToken = default(CancellationToken);


                        var buffer = new byte[buffersize];
                        int bytesRead;
                        long totalRead = 0;
                        using (var outStream = new FileStream(to, FileMode.Create, FileAccess.Write, FileShare.Read, buffersize))
                        {
                            using (var inStream = new FileStream(from, FileMode.Open, FileAccess.Read, FileShare.Read, buffersize))
                            {
                                while ((bytesRead = await inStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                                {
                                    await outStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                                    cancellationToken.ThrowIfCancellationRequested();
                                    totalRead += bytesRead;
                                    Thread.Sleep(10);
                                    progressCallback.Report(totalRead);
                                }
                            }
                        }

                        progressCallback.Report(0);

                    });
                }
                else
                {
                    int ret = Marshal.GetLastWin32Error();
                    string error = $"LogonUser failed with error code : {ret}";
                    error.WriteLog(" CopyFiles() " + MethodBase.GetCurrentMethod().Name);
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite("CopyFiles Outter " + MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string GetAppPath()
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string defaultprogramlocation = key.GetValueStr("defaultprogramlocation", "c:\\videogui");
                key?.Close();
                return defaultprogramlocation;
            }
            catch (Exception ex)
            {
                ex.LogWrite(" GetPathApp() " + MethodBase.GetCurrentMethod().Name);
                return "c:\videogui";
            }
        }

        public static void WriteLog(this List<string> obj, string callingmethod = "")
        {
            try
            {

                string m_exePath = System.IO.Path.GetDirectoryName(Debugger.IsAttached ? GetAppPath() : Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
                string InternalCallingMethod = MethodBase.GetCurrentMethod().Name.ToString();
                if (callingmethod != "")
                {
                    if (InternalCallingMethod != callingmethod)
                    {
                        InternalCallingMethod = callingmethod;
                    }
                }
                string date = DateTime.Now.ToString("dd_MM_yyyy");
                using var txtWriter = System.IO.File.AppendText(m_exePath + $"\\{date}-log.log");
                txtWriter.Write("\r\n", "Log Entry : {0}", InternalCallingMethod);
                foreach (string line in obj)
                {
                    txtWriter.WriteLine("{0}", line);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(" GetPathApp() " + MethodBase.GetCurrentMethod().Name);

            }
        }

        public static void WriteLog(this string obj, string LogName = "")
        {

            string m_exePath = Debugger.IsAttached ? GetAppPath() : System.IO.Path.GetDirectoryName( Process.GetCurrentProcess().MainModule.FileName);
            try
            {
                if (m_exePath.Contains("Debug")) m_exePath = GetAppPath();
                string date = DateTime.Now.ToString("dd_MM_yyyy");
                using (StreamWriter txtWriter = File.AppendText(m_exePath + $"\\{date}-{LogName}-WRITELOG.log"))
                {
                    if (LogName == "")
                    {
                        txtWriter.Write("\r\n", "Log Entry : ");
                        txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                        txtWriter.WriteLine("  :");
                    }
                    txtWriter.WriteLine($"{DateTime.Now.ToLongTimeString()} :{obj}");
                    if (LogName == "") txtWriter.WriteLine("-------------------------------");
                }
            }
            catch (Exception ex)
            {
                Debug.Print(MethodBase.GetCurrentMethod().Name.ToString() + " LogWrite" + ex.Message);
            }
        }



    }
}
