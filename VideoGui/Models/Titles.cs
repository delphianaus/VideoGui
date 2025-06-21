using FirebirdSql.Data.FirebirdClient;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using VideoGui.Models.delegates;


namespace VideoGui.Models
{
    public class Titles : INotifyPropertyChanged
    {
        private int _Id, _GroupId;
        private bool _IsTag = false;
        private string _Description, _VisualDesciption;
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        public bool IsTag { get => _IsTag; set { _IsTag = value; OnPropertyChanged(); } }

        public int GroupId { get => _GroupId; set { _GroupId = value; OnPropertyChanged(); } }

        public string Description { get => _Description; set { _Description = value; OnPropertyChanged(); } }

        public string VisualDescription { get => _VisualDesciption; set { _VisualDesciption = value; OnPropertyChanged(); } }

        public Titles(int _ID, int _GroupId, string _Description, string _visualDescription, bool _IsTag)
        {

            Id = _ID;
            IsTag = _IsTag;
            GroupId = _GroupId;
            Description = _Description;
            VisualDescription = _visualDescription;
        }

        public Titles(int _ID)
        {
            Id = _ID;
        }
        public Titles(FbDataReader reader)
        {
            try
            {
                Id = reader["ID"].ToInt();
                Description = reader["Description"].ToString();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Titles {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


        public Titles(FbDataReader reader, OnGetTagIds OnGetTagId)
        {
            try
            {
                Id = reader["ID"].ToInt();
                Description = reader["Description"].ToString();
                GroupId = reader["GroupId"].ToInt();
                IsTag = reader["IsTag"].ToInt() == 1 ? true : false;
                VisualDescription = "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Titles {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}


