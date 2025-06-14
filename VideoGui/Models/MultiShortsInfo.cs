using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    class MultiShortsInfo : INotifyPropertyChanged
    {
        private string _ShortsDir = "";
        private int _NumberOfShorts = 0;

        public string ShortsDir { get => _ShortsDir; set { _ShortsDir = value; OnPropertyChanged(); } }
        public int NumberOfShorts { get => _NumberOfShorts; set { _NumberOfShorts = value; OnPropertyChanged(); } }

        public MultiShortsInfo(string _ShortsDir, int _NumberOfShorts)
        {
            try
            {
                ShortsDir = _ShortsDir.Split("\\").LastOrDefault().Trim();
                NumberOfShorts = _NumberOfShorts;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"MultiShortsInfo {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {ex.StackTrace}");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
