using Google.Apis.YouTube.v3.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VideoGui.Models;
using VideoGui.Models.delegates;

using static System.Net.Mime.MediaTypeNames;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using FolderBrowserDialog = FolderBrowserEx.FolderBrowserDialog;



namespace VideoGui
{
    /// <summary>
    /// Interaction logic for ThumbnailCreator.xaml
    /// </summary>
    public partial class ThumbnailCreator : Window
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        WriteableBitmap writeableBitmap = null;
        WriteableBitmap writeableBitmap2 = null;
        BitmapImage SourceImage = null;
        BitmapImage CroppedSourceImage = null;
        public bool IsClosing = false, IsClosed = false;
        int SystemImageMargin = 76, SystemImageLeft = 0, SystemImageTop = 0;
        bool IsMouseActive = false, ThumbnailReady = false;
        System.Windows.Point RectEndPoint;
        System.Windows.Point RectStartPoint;
        Rect Selector = new Rect();
        public string SourceDirectory = "";
        public LoadTemplate DoLoadTemplate = null;
        public SaveTemplate DoSaveTemplate = null;
        public OnFinish DoOnFinish = null;
        public string Display = "";
        public string ThumbnailFileName = "";
        public List<int> Steps = new List<int>() { 1, 5, 10, 20, 50, 100 };
        public int StepIndex = 0;
        public bool IsRectChanged(Rect r)
        {
            try
            {
                return (!Selector.Equals(r));
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} RectChanged {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public ThumbnailCreator(string _display, LoadTemplate OnLoadTemplate,
            SaveTemplate OnSaveTemplate, OnFinishIdObj _DoOnFinish = null, string Line1 = "", string Line2 = "",
            string FromNum1 = "", string FromNum2 = "")
        {
            try
            {
                InitializeComponent();
                Title += " " + _display;
                Closing += (s, e) => { IsClosing = true; };
                Closed += (s, e) => { IsClosed = true; _DoOnFinish?.Invoke(this, -1); };
                Display = _display;
                DoLoadTemplate = OnLoadTemplate;
                DoSaveTemplate = OnSaveTemplate;
                txtLine1.Text = Line1;
                txtLine2.Text = Line2;
                txtNumThumbs.Text = FromNum1;
                txtNumThumbs2.Text = FromNum2;
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                int PMargin = key.GetValueInt("ImageMargin", 70);
                int PLeft = key.GetValueInt("ImageLeft", 0);
                int PTop = key.GetValueInt("ImageTop", 0);
                int SIndex = key.GetValueInt("ImageStepIndex", 0);
                int FontSize1 = key.GetValueInt("ImageFontSize1", 34);
                int FontSize2 = key.GetValueInt("ImageFontSize2", 31);
                int TestOffSet1 = key.GetValueInt("TextOffset1", 90);
                int TestOffSet2 = key.GetValueInt("TextOffset2", 65);
                string Root = key.GetValueStr("Thumbnails", "c:\\");
                txtOutputDir.Text = Root;
                txtfFontSize1.Text = FontSize1.ToString();
                txtfFontSize2.Text = FontSize2.ToString();
                txtfOffset1.Text = TestOffSet1.ToString();
                txtfOffset2.Text = TestOffSet2.ToString();
                txtMargin.Text = PMargin.ToString();
                SystemImageMargin = PMargin;
                SystemImageLeft = PLeft;
                SystemImageTop = PTop;
                if (SIndex < 0) SIndex = 0;
                if (SIndex >= Steps.Count) SIndex = 0;
                txtStep.Text = Steps[SIndex].ToString();
                key?.Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} ThumbnailCreator {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        double minwidth = 0;
        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fld = new Microsoft.Win32.OpenFileDialog();
                fld.Filter = "Image Files|*.jpg;*.png;*.jpeg";
                fld.DefaultExt = "*.jpg";
                fld.Multiselect = false;
                var fd = fld.ShowDialog();
                if ((fd != null) && (fd.Value == true))
                {
                    SourceImage = new BitmapImage(new Uri(fld.FileName));
                    txtImage.Text = fld.FileName;
                    srcImage.Source = SourceImage;
                    writeableBitmap = new WriteableBitmap((BitmapSource)srcImage.Source);
                    double sourceWidth = brdImg.Width;
                    double sourceHeight = brdImg.Height;
                    double destWidth = srcImage.DesiredSize.Width; ;
                    double destHeight = srcImage.DesiredSize.Height;
                    double scaleX = destWidth / SourceImage.Width;
                    double scaleY = destHeight / SourceImage.Height;
                    double leftoverWidth = sourceWidth * scaleY;
                    minwidth = destWidth - (sourceWidth * scaleY * 1.1);
                    double leftoverHeight = destHeight - sourceHeight;
                    //Select16by9();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} btnSelectImage_Click {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                key.SetValue("ImageMargin", SystemImageMargin);
                key?.Close();
                Close();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} CropImage {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                Close();
            }
        }
        public WriteableBitmap QuickWriteText(string Text1, string Text2)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                key.SetValue("ImageMargin", SystemImageMargin);
                int aFontSize1 = txtfFontSize1.Text.ToInt();
                int aFontSize2 = txtfFontSize2.Text.ToInt();
                int aTextOffset = txtfOffset1.Text.ToInt();
                int aTextOffset2 = txtfOffset2.Text.ToInt();
                key.SetValue("ImageFontSize1", aFontSize1);
                key.SetValue("ImageFontSize2", aFontSize2);
                key.SetValue("TextOffset1", aTextOffset);
                key.SetValue("TextOffset2", aTextOffset2);
                key?.Close();
                System.Drawing.Bitmap bmpx = CroppedSourceImage.ConvertToBitmap();
                var dpiX = CroppedSourceImage.DpiX;
                var dpiY = CroppedSourceImage.DpiY;
                BitmapData data = bmpx.LockBits(new System.Drawing.Rectangle(0, 0, bmpx.Width, bmpx.Height),
                    ImageLockMode.ReadOnly, bmpx.PixelFormat);
                writeableBitmap2.Lock();
                CopyMemory(writeableBitmap2.BackBuffer, data.Scan0,
                           (uint)(writeableBitmap2.BackBufferStride * bmpx.Height));
                writeableBitmap2.AddDirtyRect(new Int32Rect(0, 0, bmpx.Width, bmpx.Height));
                writeableBitmap2.Unlock();
                bmpx.UnlockBits(data);
                int MarginOffset = txtMargin.Text.ToInt() - 5;
                BitmapSource bImage = CroppedSourceImage;
                DrawingVisual dVisual = new DrawingVisual();
                using (DrawingContext dc = dVisual.RenderOpen())
                {
                    dc.DrawImage(bImage, new Rect(0, 0, bImage.PixelWidth, bImage.PixelHeight));
                    if (Text1 != "")
                    {
                        var fmtText = new FormattedText(Text1, CultureInfo.InvariantCulture, FlowDirection,
                              new Typeface("Dubai Medium"), aFontSize1, Brushes.Black);
                        var dt = fmtText.Width;
                        var dp = bImage.PixelWidth;
                        var di = (dp - dt) / 2;
                        dc.DrawText(fmtText, new System.Windows.Point(di, bImage.PixelHeight - aTextOffset));
                    }
                    if (Text2 != "")
                    {
                        var fmtText2 = new FormattedText(Text2, CultureInfo.InvariantCulture, FlowDirection,
                              new Typeface("Dubai Medium"), aFontSize2, Brushes.Black);
                        var dt = fmtText2.Width;
                        var dp = bImage.PixelWidth;
                        var di = (dp - dt) / 2;
                        dc.DrawText(fmtText2, new System.Windows.Point(di, bImage.PixelHeight - aTextOffset2));
                    }
                }
                RenderTargetBitmap targetBitmap = new RenderTargetBitmap(bmpx.Width, bmpx.Height, 96, 96, PixelFormats.Default);
                targetBitmap.Render(dVisual);
                return new WriteableBitmap(targetBitmap);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} CropImage {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return null;
            }
        }
        public WriteableBitmap QuickCopyBitmap(Rect Selector)
        {
            try
            {
                Bitmap bmp = SourceImage.ConvertToBitmap();
                var dpiX = SourceImage.DpiX;
                var dpiY = SourceImage.DpiY;
                BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly, bmp.PixelFormat);
                writeableBitmap.Lock();
                CopyMemory(writeableBitmap.BackBuffer, data.Scan0,
                           (uint)(writeableBitmap.BackBufferStride * bmp.Height));
                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, bmp.Width, bmp.Height));
                writeableBitmap.Unlock();
                bmp.UnlockBits(data);
                BitmapSource bImage = SourceImage;
                DrawingVisual dVisual = new DrawingVisual();
                using (DrawingContext dc = dVisual.RenderOpen())
                {
                    dc.DrawImage(bImage, new Rect(0, 0, bImage.PixelWidth, bImage.PixelHeight));
                    SolidColorBrush rectBrush = new SolidColorBrush(Colors.Red);
                    rectBrush.Opacity = 0.5;
                    dc.DrawRectangle(rectBrush, null, Selector);
                }
                RenderTargetBitmap targetBitmap = new RenderTargetBitmap(bmp.Width, bmp.Height, 96, 96, PixelFormats.Default);
                targetBitmap.Render(dVisual);
                return new WriteableBitmap(targetBitmap);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} CropImage {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return null;
            }
        }


        private void srcImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                if (IsMouseActive)
                {
                    System.Windows.Point p = e.GetPosition(srcImage);
                    double pixelWidth = srcImage.Source.Width;
                    double pixelHeight = srcImage.Source.Height;
                    double x = pixelWidth * p.X / srcImage.ActualWidth;
                    double y = pixelHeight * p.Y / srcImage.ActualHeight;
                    RectEndPoint = new System.Windows.Point(x, y);
                    var SizeWidth = Math.Abs(RectEndPoint.X - RectStartPoint.X);
                    var SizeHeight = Math.Abs(RectEndPoint.Y - RectStartPoint.Y);
                    var LocationStartX = (RectEndPoint.X < RectStartPoint.X) ? RectEndPoint.X : RectStartPoint.X;
                    var LocationStartY = (RectEndPoint.Y < RectStartPoint.Y) ? RectEndPoint.Y : RectStartPoint.Y;
                    Selector.Size = new System.Windows.Size(SizeWidth, SizeHeight);
                    if (LocationStartX < minwidth)
                    {
                        // LocationStartX = minwidth;

                    }

                    Selector.X = LocationStartX;
                    Selector.Y = LocationStartY;

                    srcImage.Source = QuickCopyBitmap(Selector);
                    Selector.Size = new System.Windows.Size(SizeWidth, SizeHeight);

                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} MouseDraw {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void srcImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!IsMouseActive)
                {
                    IsMouseActive = true;
                    System.Windows.Point p = e.GetPosition(srcImage);
                    double pixelWidth = srcImage.Source.Width;
                    double pixelHeight = srcImage.Source.Height;
                    double x = pixelWidth * p.X / srcImage.ActualWidth;
                    double y = pixelHeight * p.Y / srcImage.ActualHeight;
                    RectStartPoint = new System.Windows.Point(x, y);
                    srcImage.Source = QuickCopyBitmap(new Rect(0, 0, 0, 0));
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} MouseDraw {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void srcImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseActive = false;
        }

        private void btnCrop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Selector != new Rect(0, 0, 0, 0))
                {

                    var source = (BitmapSource)SourceImage;

                    int x = (int)Selector.X;
                    int y = (int)Selector.Y;
                    int w = (int)Selector.Width;
                    int h = (int)Selector.Height;

                    // clamp
                    x = Math.Max(0, x);
                    y = Math.Max(0, y);
                    w = Math.Min(w, source.PixelWidth - x);
                    h = Math.Min(h, source.PixelHeight - y);

                    var int32Rect = new Int32Rect(x, y, w, h);

                    {
                        // Selector.Width = SourceImage.PixelWidth - (minwidth * 1.1);
                    }
                    var sourceImg = $"W:{SourceImage.Width} H:{SourceImage.Height}";

                    var image = new CroppedBitmap(SourceImage, int32Rect);
                    CroppedSourceImage = image.ConvertToBitmap().Resize(SystemImageMargin, Color.Thistle).ToBitmapImage();
                    srcImageClip.Source = null;
                    srcImageClip.Source = CroppedSourceImage;
                    writeableBitmap2 = new WriteableBitmap(CroppedSourceImage);
                    ThumbnailReady = true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("value does not fall within the expected range"))
                {
                    btnCrop_Click(sender, e);
                }
                else ex.LogWrite($"{this} MouseDraw {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void imgDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                try
                {
                    if ((e.ChangedButton == MouseButton.Left) && (e.ClickCount == 1))
                    {
                        var step = txtStep.Text.ToInt(1);
                        if (step < 0) step = 1;
                        var btn = Extensions.FindParent<System.Windows.Controls.Button>
                        (sender as System.Windows.Controls.Image);

                        if (btn is not null && btn.Name != "")
                        {
                            bool Proceed = ProcessSteps(btn.Name, false);
                            if (chkAutoCrop.IsChecked == true && Proceed)
                            {
                                btnCrop_Click(sender, e);
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    ex.LogWrite($"{this} imgDown_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgDown_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void imgUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if ((e.ChangedButton == MouseButton.Left) && (e.ClickCount == 1))
                {
                    var btn = Extensions.FindParent<System.Windows.Controls.Button>
                        (sender as System.Windows.Controls.Image);

                    if (btn is not null && btn.Name != "")
                    {
                        bool Proceed = ProcessSteps(btn.Name, true);
                        if (chkAutoCrop.IsChecked == true && Proceed)
                        {
                            btnCrop_Click(sender, e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void AdjustByMargin(int step)
        {
            try
            {
                Selector.Height += step;
                srcImage.Source = QuickCopyBitmap(Selector);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} AdjustByMargin {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private bool ProcessSteps(string btnName, bool Direction = false)
        {
            try
            {
                var step = txtStep.Text.ToInt(1);
                if (step < 0) step = 1;
                step = (!Direction) ? step : -step; 
                bool Proceed = false;
                if (btnName.EndsWith("Margin"))
                {
                    SystemImageMargin -= step;
                    if (SystemImageMargin < 0) SystemImageMargin = 0;
                    txtMargin.Text = SystemImageMargin.ToString();
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("ImageMargin", SystemImageMargin);
                    key?.Close();
                    Selector.Height += (Direction) ? step : -step;
                    srcImage.Source = QuickCopyBitmap(Selector);
                    Proceed = true;
                }
                else if (btnName.EndsWith("Left"))
                {
                    if (Selector.X + step > 0 && (Selector.X + Selector.Width + step) <= SourceImage.Width)
                    {
                        SystemImageLeft += step;
                        if (SystemImageLeft < 0) SystemImageLeft = 0;
                        txtLeft.Text = SystemImageLeft.ToString();
                        RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        key.SetValue("ImageLeft", SystemImageLeft);
                        key?.Close();
                        Selector.X += step;
                        if (Selector.X < 0) Selector.X = 0;
                        srcImage.Source = QuickCopyBitmap(Selector);
                        Proceed = true;
                    }
                }
                else if (btnName.EndsWith("Top"))
                {
                    if (Selector.Y + step > 0 && (Selector.Y + Selector.Height + step) <= SourceImage.Height)
                    {
                        SystemImageTop += step;
                        if (SystemImageTop < 0) SystemImageTop = 0;
                        txtTop.Text = SystemImageTop.ToString();
                        RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                        key.SetValue("ImageTop", SystemImageTop);
                        key?.Close();
                        Selector.Y += step;
                        if (Selector.Y < 0) Selector.Y = 0;
                        srcImage.Source = QuickCopyBitmap(Selector);
                        Proceed = true;
                    }
                    srcImage.Source = QuickCopyBitmap(Selector);
                    Proceed = true;
                }
                else if (btnName.EndsWith("Step"))
                {
                    StepIndex = (Direction) ? StepIndex - 1 : StepIndex + 1;
                    if (StepIndex < 0) StepIndex = Steps.Count - 1;
                    if (StepIndex >= Steps.Count) StepIndex = 0;
                    txtStep.Text = Steps[StepIndex].ToString();
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key.SetValue("ImageStepIndex", StepIndex);
                    key?.Close();
                    return false;
                }
                return Proceed;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} ProcessSteps {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }

        private void btnPlaceText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Selector != new Rect(0, 0, 0, 0))
                {
                    if ((txtLine1.Text.Length > 0) && (srcImageClip.Source != null))
                    {
                        srcImageClip.Source = QuickWriteText(txtLine1.Text, txtLine2.Text);
                        BitmapSource image = (BitmapSource)srcImageClip.Source;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnFontSize1Up_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var f = txtfFontSize1.Text.ToInt(-1);
                var step = txtStep.Text.ToInt(1);
                txtfFontSize1.Text = (f != -1) ? $"{f + step}" : txtfFontSize1.Text;
                btnPlaceText_Click(sender, e);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnFontSize1Down_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var f = txtfFontSize1.Text.ToInt(-1);
                if (f != 0 && f != -1)
                {
                    var step = txtStep.Text.ToInt(1);
                    if (f - step < 0) step = f;
                    txtfFontSize1.Text = (f != -1) ? $"{f - step}" : txtfFontSize1.Text;
                    btnPlaceText_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void btnFontSize2Up_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var f = txtfFontSize2.Text.ToInt(-1);
                var step = txtStep.Text.ToInt(1);
                txtfFontSize2.Text = (f != -1) ? $"{f + step}" : txtfFontSize2.Text;
                btnPlaceText_Click(sender, e);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void btnFontSize2Down_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var f = txtfFontSize2.Text.ToInt(-1);
                if (f != 0 && f != -1)
                {
                    var step = txtStep.Text.ToInt(1);
                    if (f - step < 0) step = f;
                    txtfFontSize2.Text = (f != -1) ? $"{f - step}" : txtfFontSize2.Text;
                    btnPlaceText_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnTextOffset1UP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var f = txtfOffset1.Text.ToInt(-1);
                var step = txtStep.Text.ToInt(1);
                txtfOffset1.Text = (f != -1) ? $"{f + step}" : txtfOffset1.Text;
                btnPlaceText_Click(sender, e);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void btnTextOffset1DOWN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var f = txtfOffset1.Text.ToInt(-1);
                if (f != 0 && f != -1)
                {
                    var step = txtStep.Text.ToInt(1);
                    if (f - step < 0) step = f;
                    txtfOffset1.Text = (f != -1) ? $"{f - step}" : txtfOffset1.Text;
                    btnPlaceText_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnTextOffset2UP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var f = txtfOffset2.Text.ToInt(-1);
                var step = txtStep.Text.ToInt(1);
                txtfOffset2.Text = (f != -1) ? $"{f + step}" : txtfOffset2.Text;
                btnPlaceText_Click(sender, e);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnTextOffset2DOWN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var f = txtfOffset2.Text.ToInt(-1);
                if (f != 0 && f != -1)
                {
                    var step = txtStep.Text.ToInt(1);
                    if (f - step < 0) step = f;
                    txtfOffset2.Text = (f != -1) ? $"{f - step}" : txtfOffset2.Text;
                    btnPlaceText_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnSetOutput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string Root = key.GetValueStr("Thumbnails", "c:\\");
                key?.Close();
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Title = "Select a folder To Export";
                folderBrowserDialog.InitialFolder = Root;
                folderBrowserDialog.AllowMultiSelect = false;
                var folder = "";
                var selectresult = folderBrowserDialog.ShowDialog();
                if (selectresult == System.Windows.Forms.DialogResult.OK)
                {
                    txtOutputDir.Text = folderBrowserDialog.SelectedFolder;
                    RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key2.SetValue("Thumbnails", txtOutputDir.Text);
                    key2?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void Down_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Primitives.RepeatButton btn)
                {
                    bool Proceed = ProcessSteps(btn.Name, true);
                    if (chkAutoCrop.IsChecked == true && Proceed)
                    {
                        btnCrop_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        private void Up_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Primitives.RepeatButton btn)
                {
                    bool Proceed = ProcessSteps(btn.Name, false);
                    if (chkAutoCrop.IsChecked == true && Proceed)
                    {
                        btnCrop_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        private void btnClose_Copy3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Date = "";
                string rfDate = Date;
                var DatePtr = txtLine1.Text.Split(' ').ToList();//.LastOrDefault();
                var DatePtr2 = txtLine2.Text.Split(' ').ToList();
                DatePtr.AddRange(DatePtr2);
                if (txtOutputDir.Text.ToUpper() == @"C:\")
                {
                    if (!Directory.Exists(@"C:\GoPro9\Thumbnails"))
                    {
                        Directory.CreateDirectory(@"C:\GoPro9\Thumbnails");
                    }
                    txtOutputDir.Text = @"C:\GoPro9\Thumbnails";
                }

                bool IsNonDateSingle = false;
                foreach (var item in from item in DatePtr.Where(ite => ite.Length == 6)
                                     where item.ToInt(-1) != -1
                                     select item)
                {
                    Date = item;
                    rfDate = Date;
                    if (txtLine2.Text.Contains(Date)) rfDate = "";
                }

                if (txtLine1.Text != "" && txtLine2.Text != "" && ThumbnailReady)
                {
                    if (txtNumThumbs.Text == "")
                    {
                        //btnCrop_Click(this, e);
                        var thumbnail = QuickWriteText(txtLine1.Text, txtLine2.Text);
                        if (thumbnail != null)
                        {
                            if (rfDate != "")
                            {
                                rfDate = $"[{rfDate}]";
                            }

                            string fname = "";

                            if (rfDate == "")
                            {
                                fname = System.IO.Path.Combine(txtOutputDir.Text,
                                $"thumbnail_{txtLine2.Text}.png");
                            }
                            else fname = System.IO.Path.Combine(txtOutputDir.Text,
                                $"thumbnail_{txtLine2.Text} {rfDate}.png");

                            if (Date != "")
                            {
                                if (rfDate == "")
                                {
                                    fname = fname.Replace($"{Date}", $"[{Date}]");
                                }

                                if (!fname.Contains(Date))
                                {

                                    fname = fname.Replace("Part", $"{Date} Part");
                                }
                            }
                            thumbnail.Save(fname);
                            ThumbnailFileName = fname;
                        }
                    }
                    else
                    {
                        string pr = txtLine2.Text;
                        var cntr = txtNumThumbs.Text.ToInt();
                        var cntr2 = txtNumThumbs2.Text.ToInt();
                        if (pr.ToLower().Contains("part"))
                        {
                            int id = pr.ToLower().LastIndexOf("part");
                            if (id > -1)
                            {
                                string Part = pr.Substring(id, 4).Trim();

                                string cnt = pr.Substring(0, id).Trim() + $" {Part}";

                                if (cnt != "")
                                {
                                    for (int i = cntr; i < cntr2 + 1; i++)
                                    {
                                        string fnn = cnt + $" {i}";
                                        //btnCrop_Click(this, e);
                                        var thumbnail2 = QuickWriteText(txtLine1.Text, fnn);
                                        string fname = System.IO.Path.Combine(txtOutputDir.Text, $"thumbnail_{fnn}.png");
                                        if (rfDate == "")
                                        {
                                            fname = fname.Replace($"{Date}", $"[{Date}]");
                                        }
                                        if (!fname.Contains(Date) && fname.Contains("Part"))
                                        {
                                            fname = fname.Replace("Part", $"{Date} Part");
                                        }
                                        if (!fname.Contains(Date) && fname.Contains("PART"))
                                        {
                                            fname = fname.Replace("PART", $"{Date} Part");
                                        }
                                        thumbnail2.Save(fname);
                                        ThumbnailFileName = fname;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var thumbnail2 = QuickWriteText(txtLine1.Text, txtLine2.Text);
                            string fname = System.IO.Path.Combine(txtOutputDir.Text, $"thumbnail_Custom_{txtLine1.Text}.png");
                            thumbnail2.Save(fname);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} imgUp_MouseLeftButtonDown {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public void setTemplate(ThumbnailsInfo thumbnailsInfo)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} settemplate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


        private void btnLoadTemplate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                string Root = key.GetValueStr("ThumbnailTemplateOpen", "c:\\");
                key?.Close();
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Title = "Select folder for Source Matching";
                folderBrowserDialog.InitialFolder = Root;
                folderBrowserDialog.AllowMultiSelect = false;
                var selectresult = folderBrowserDialog.ShowDialog();
                if (selectresult == System.Windows.Forms.DialogResult.OK)
                {
                    RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    string SourceDirectory = folderBrowserDialog.SelectedFolder;
                    string keydir = SourceDirectory.Split('\\').ToList().LastOrDefault();
                    SourceDirectory = SourceDirectory.Replace(keydir, "");
                    key2.SetValue("ThumbnailTemplateOpen", SourceDirectory);
                    key2?.Close();
                    if (keydir != null && keydir != "")
                    {
                        SourceDirectory = keydir;
                        DoLoadTemplate?.Invoke(this, keydir);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"{this} LoadTemplate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


    }
}
