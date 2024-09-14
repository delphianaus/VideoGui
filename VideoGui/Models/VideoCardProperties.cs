using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VideoGui
{
    public class VideoCardProperties : INotifyPropertyChanged
    {
        private string _VideoCardPropName;
        private string _VideoCardPropValue;

        public VideoCardProperties()
        {

        }

        public VideoCardProperties(string _VideoCardPropName, string _VideoCardPropValue)
        {
            VideoCardPropName = _VideoCardPropName;
            VideoCardPropValue = _VideoCardPropValue;

        }
        public string VideoCardPropName { get => _VideoCardPropName; set { _VideoCardPropName = value; OnPropertyChanged(); } }
        public string VideoCardPropValue { get => _VideoCardPropValue; set { _VideoCardPropValue = value; OnPropertyChanged(); } }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
