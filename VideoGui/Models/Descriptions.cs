using FirebirdSql.Data.FirebirdClient;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using VideoGui.Models.delegates;

namespace VideoGui.Models
{
    public class Descriptions : INotifyPropertyChanged
    {
        private int _Id, _TitleTagId;
        private string _Description, _TitleTag, _Name;
        private bool _IsShortVideo, _IsTag;

        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public bool IsTag { get => _IsTag; set { _IsTag = value; OnPropertyChanged(); } }
        public int TitleTagId { get => _TitleTagId; set { _TitleTagId = value; OnPropertyChanged(); } }
        public string Description { get => _Description; set { _Description = value; OnPropertyChanged(); } }
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        public string TitleTag { get => _TitleTag; set { _TitleTag = value; OnPropertyChanged(); } }
        public bool IsShortVideo { get => _IsShortVideo; set { _IsShortVideo = value; OnPropertyChanged(); } }

        public Descriptions(FbDataReader reader, OnGetTagIds DoOnGetTag)
        {
            try
            {
                Id = reader["ID"].ToInt();
                Description = reader["DESCRIPTION"].ToString();
                TitleTagId = reader["TITLETAGID"].ToInt();
                Name = reader["NAME"].ToString();
                TitleTag = reader["NAME"].ToString();
                IsShortVideo = reader["ISSHORTVIDEO"].ToInt() == 1 ? true : false;
                IsTag = reader["ISTAG"].ToInt() == 1 ? true : false;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Descriptions {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public Descriptions(int _TitleTagId, string _description, bool _IsShortVideo, string _TitleTag = "", string Name = "", bool _IsTag = false)
        {
            try
            {
                TitleTagId = _TitleTagId;
                Description = _description;
                IsShortVideo = _IsShortVideo;
                TitleTag = _TitleTag;
                Name = _Name;
                IsTag = _IsTag;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Descriptions {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
