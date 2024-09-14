using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VideoGui
{
    /// <summary>
    /// Interaction logic for VideoCardSelector.xaml
    /// </summary>
    public partial class VideoCardSelector : Window
    {
        private readonly List<VideoCardDetails> VidCardDetails = new();
        private readonly List<VideoCardProperties> vidCardProperies = new();
        readonly Models.delegates.CompairFinished OnFinish;
        public ProgressWindow.CancelScan OnCancel;// = new(OnCancelation);
        bool canclose = true;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public VideoCardSelector(Models.delegates.CompairFinished _OnFinish)
        {
            try
            {
                InitializeComponent();
                OnFinish = _OnFinish ?? throw new ArgumentNullException(nameof(_OnFinish));

                BuildListOfCards();
                LstBoxVideoCards.ItemsSource = VidCardDetails;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void BuildPropertyTable(string Description)
        {
            try
            {
                ManagementObjectSearcher searcher = new($"SELECT * FROM Win32_VideoController where Description = {Description.ForceQuoteStr()}");
                vidCardProperies.Clear();
                foreach (ManagementObject mo in searcher.Get())
                {
                    PropertyDataCollection collection = mo.Properties;
                    foreach (PropertyData data in collection)
                    {
                        string Name = data.Name != null ? data.Name.ToString() : "";
                        string Value = data.Value != null ? data.Value.ToString() : "";
                        if (Name.Trim() != "")
                        {
                            vidCardProperies.Add(new VideoCardProperties(Name, Value));
                        }
                    }
                }
                LstBoxProps.ItemsSource = vidCardProperies;

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }

        }

        public void BuildListOfCards()
        {
            try
            {
                String Card = string.Empty;
                VidCardDetails.Clear();
                ManagementObjectSearcher searcher = new("SELECT * FROM Win32_VideoController");

                foreach (ManagementObject mo in searcher.Get())
                {
                    PropertyData description = mo.Properties["Description"];
                    PropertyData VideoProcessor = mo.Properties["VideoProcessor"];
                    if ((description != null) && (VideoProcessor.Value != null))
                    {
                        Card = description.Value.ToString();
                        VidCardDetails.Add(new VideoCardDetails(Card, "", false));
                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void VideoCardSelect_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnFinish?.Invoke();
        }

        private void LstBoxVideoCards_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                VideoCardDetails lst = (VideoCardDetails)LstBoxVideoCards.SelectedItem;
                if (lst != null)
                {
                    string videocardname = lst.VideoCardName;
                    BuildPropertyTable(videocardname);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void LstBoxVideoCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VideoCardDetails lst = (VideoCardDetails)LstBoxVideoCards.SelectedItem;
                if (lst != null)
                {
                    string videocardname = lst.VideoCardName;
                    txtName.Text = "";
                    txtValue.Text = "";
                    BuildPropertyTable(videocardname);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void LstBoxProps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VideoCardProperties lst = (VideoCardProperties)LstBoxProps.SelectedItem;
                if (lst != null)
                {
                    string videocardname = lst.VideoCardPropName;
                    string videocardProp = lst.VideoCardPropValue;
                    txtName.Text = videocardname;
                    txtValue.Text = videocardProp;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LstBoxVideoCards.SelectedItem != null)
                {
                    VideoCardDetails lst = (VideoCardDetails)LstBoxVideoCards.SelectedItem;
                    string cardname = lst.VideoCardName;
                    RegistryKey key = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                    key?.SetValue("selectedcard", cardname);
                    key?.Close();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
