using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class ShortsDirectory : INotifyPropertyChanged
    {
        private int _Id = -1, _TitleId = -1, _DescId = -1;
        private string _Directory = "", _LinkedTitleIds = "" , _LinkedDescIds = "";
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public string Directory { get => _Directory; set { _Directory = value; OnPropertyChanged(); } }
        
        public string LinkedTitleIds { get => _LinkedTitleIds; set { _LinkedTitleIds = value; OnPropertyChanged(); } }
        public string LinkedDescIds { get => _LinkedDescIds; set { _LinkedDescIds = value; OnPropertyChanged(); } }
        public bool IsTitleAvailable { get => _TitleId != -1 && _LinkedTitleIds != ""; set { IsTitleAvailable = value; OnPropertyChanged(); } }
        public bool IsDescAvailable { get => _DescId != -1; set { IsDescAvailable = value; OnPropertyChanged(); } }

        public int TitleId { get => _TitleId; set { _TitleId = value; OnPropertyChanged(); } }
        public int DescId { get => _DescId; set { _DescId = value; OnPropertyChanged(); } }


        public ShortsDirectory(int id, string directory="")
        {
            try
            {
                Id = id;
                Directory = directory;
            }
            catch(Exception ex)
            {
                ex.LogWrite($"ShortsDirectory {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {ex.StackTrace}");
            }
        }
        public ShortsDirectory(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int ID) ? ID : -1;
                Directory = (reader["DIRECTORYNAME"] is string DIR) ? DIR.ToUpper() : "";
                TitleId = (reader["TITLEID"] is int TID) ? TID : -1;
                DescId = (reader["DESCID"] is int DID) ? DID : -1;
                //LinkedTitleIds = (reader["LINKEDTITLEIDS"] is string LTID) ? LTID : "";
                //LinkedDescIds = (reader["LINKEDDESCIDS"] is string LDID) ? LDID : "";

            }
            catch (Exception ex)
            {
                ex.LogWrite($"ShortsDirectory {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {ex.StackTrace}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
