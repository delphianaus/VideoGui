using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VideoGui.Models
{
    class MultiShortsInfo : INotifyPropertyChanged
    {
        private string _DirectoryName = "";
        private int _NumberOfShorts = 0;
        //private Brush Brush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        private bool _IsActive = false;
        private FontWeight _FontWeight = FontWeights.Normal;
        public bool IsActive
        {
            get => _IsActive;
            set
            {
                _IsActive = value;
                SetActive(value);
                OnPropertyChanged();
            }
        }

        public FontWeight AutoFontWeight
        {
            get => _FontWeight;
            set
            {
                _FontWeight = (IsActive) ? FontWeights.Bold : FontWeights.Normal;
                OnPropertyChanged();
            }
        }
        
        public string DirectoryName { get => _DirectoryName; set { _DirectoryName = value; OnPropertyChanged(); } }
        public int NumberOfShorts { get => _NumberOfShorts; set { _NumberOfShorts = value; OnPropertyChanged(); } }

        public MultiShortsInfo(string _ShortsDir, int _NumberOfShorts, bool _IsActive)
        {
            try
            {
                DirectoryName = _ShortsDir.Split("\\").LastOrDefault().Trim();
                NumberOfShorts = _NumberOfShorts;
                IsActive = _IsActive;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"MultiShortsInfo {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {ex.StackTrace}");
            }

        }
        public void SetActive(bool value)
        {
            AutoFontWeight = value ? FontWeights.Bold : FontWeights.Normal;  // Reversed this line
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
