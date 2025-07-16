using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class CoverterSettings : INotifyPropertyChanged
    {
        private int _id { get; set; } = -1;
        private bool _IsDefault { get; set; } = false;
        private string _MinBitRate { get; set; } = "";
        private string _MaxBitRate { get; set; } = "";
        private string _BitRateBuffer { get; set; } = "";
        private string _VideoWidth { get; set; } = "";
        private string _VideoHeight { get; set; } = "";
        private string _ArModulas { get; set; } = "";
        private bool _ResizeEnable { get; set; } = false;
        private bool _ArRoundEnable { get; set; } = false;
        private bool _ArScalingEnabled { get; set; } = false;
        private bool _VSyncEnable { get; set; } = false;
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public bool IsDefault { get => _IsDefault; set { _IsDefault = value; OnPropertyChanged(); } }
        public string MinBitRate { get => _MinBitRate; set { _MinBitRate = value; OnPropertyChanged(); } }
        public string MaxBitRate { get => _MaxBitRate; set { _MaxBitRate = value; OnPropertyChanged(); } }
        public string BitRateBuffer { get => _BitRateBuffer; set { _BitRateBuffer = value; OnPropertyChanged(); } }
        public string VideoWidth { get => _VideoWidth; set { _VideoWidth = value; OnPropertyChanged(); } }
        public string VideoHeight { get => _VideoHeight; set { _VideoHeight = value; OnPropertyChanged(); } }
        public string ArModulas { get => _ArModulas; set { _ArModulas = value; OnPropertyChanged(); } }
        public bool ResizeEnable { get => _ResizeEnable; set { _ResizeEnable = value; OnPropertyChanged(); } }
        public bool ArRoundEnable { get => _ArRoundEnable; set { _ArRoundEnable = value; OnPropertyChanged(); } }
        public bool ArScalingEnabled { get => _ArScalingEnabled; set { _ArScalingEnabled = value; OnPropertyChanged(); } }
        public bool VSyncEnable { get => _VSyncEnable; set { _VSyncEnable = value; OnPropertyChanged(); } }



        /* MinBitRate, MaxBitRate, BitRateBuffer, VideoWidth, VideoHeight. ArModulas, 
         * ResizeEnable, ArRoundEnable, ArScalingEnabled, VSyncEnable  */
        public CoverterSettings(int _id, bool _IsDefault, string _MinBitRate, string _MaxBitRate, 
            string _BitRateBuffer, string _VideoWidth, string _VideoHeight, string _ArModulas,
            bool _ResizeEnable, bool _ArRoundEnable, bool _ArScalingEnabled, bool _VSyncEnable)
        {
            Id = _id;
            IsDefault = _IsDefault;
            MinBitRate = _MinBitRate;
            MaxBitRate = _MaxBitRate;
            BitRateBuffer = _BitRateBuffer;
            VideoWidth = _VideoWidth;
            VideoHeight = _VideoHeight;
            ArModulas = _ArModulas;
            ResizeEnable = _ResizeEnable;
            ArRoundEnable = _ArRoundEnable;
            ArScalingEnabled = _ArScalingEnabled;
            VSyncEnable = _VSyncEnable;
        }
        public CoverterSettings(FbDataReader r)
        {
            Id = (r["ID"] is int _id) ? _id : -1;
            IsDefault = (r["ISDEFAULT"] is Int16 _IsDefault) ? (_IsDefault == 1) : false;
            MinBitRate = (r["MINBITRATE"] is string _MinBitRate) ? _MinBitRate : "";
            MaxBitRate = (r["MAXBITRATE"] is string _MaxBitRate) ? _MaxBitRate : "";
            BitRateBuffer = (r["BITRATEBUFFER"] is string _BitRateBuffer) ? _BitRateBuffer : "";
            VideoWidth = (r["VIDEOWIDTH"] is string _VideoWidth) ? _VideoWidth : "";
            VideoHeight = (r["VIDEOHEIGHT"] is string _VideoHeight) ? _VideoHeight : "";
            ArModulas = (r["ARMODULAS"] is string _ArModulas) ? _ArModulas : "";
            ResizeEnable = (r["RESIZEENABLE"] is Int16 _ResizeEnable) ? (_ResizeEnable == 1) : false;
            ArRoundEnable = (r["ARROUNDENABLE"] is Int16 _ArRoundEnable) ? (_ArRoundEnable == 1) : false;
            ArScalingEnabled = (r["ARSCALINGENABLED"] is Int16 _ArScalingEnabled) ? (_ArScalingEnabled == 1) : false;
            VSyncEnable = (r["VSYNCEABLE"] is Int16 _VSyncEnable) ? (_VSyncEnable == 1) : false;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
