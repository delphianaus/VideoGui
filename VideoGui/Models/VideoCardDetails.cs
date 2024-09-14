using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VideoGui
{
    public class VideoCardDetails : INotifyPropertyChanged
    {
        private string _VideoCardName;
        private string _VideoCardMemory;
        private bool _Excluded;
        public bool _Selected;

        public string VideoCardName { get => _VideoCardName; set { _VideoCardName = value; OnPropertyChanged(); } }
        public string VideoCardMemory { get => _VideoCardMemory; set { _VideoCardMemory = value; OnPropertyChanged(); } }
        public bool Excluded { get => _Excluded; set { _Excluded = value; OnPropertyChanged(); } }
        public bool Selected { get => _Selected; set { _Selected = value; OnPropertyChanged(); } }


        public VideoCardDetails(string _VideoCardName, string _VideoCardMemory, bool _Excluded)
        {
            VideoCardName = _VideoCardName;
            VideoCardMemory = _VideoCardMemory;
            Excluded = _Excluded;
            _Selected = false;
        }
        public VideoCardDetails()
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
