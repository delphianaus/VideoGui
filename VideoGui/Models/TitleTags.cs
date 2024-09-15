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
    public class TitleTags : INotifyPropertyChanged
    {
        private int _Id, _TagId, _GroupId;

        private string _Description;


        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        public int TagId { get => _TagId; set { _TagId = value; OnPropertyChanged(); } }

        public int GroupId { get => _GroupId; set { _GroupId = value; OnPropertyChanged(); } }

        public string Description { get => _Description; set { _Description = value; OnPropertyChanged(); } }

        public TitleTags(int _id, int _TagId, int _GroupId, string _Description)
        {

            Id = _id;
            TagId = _TagId;
            GroupId = _GroupId;
            Description = _Description;
        }



        public TitleTags(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int id) ? id : -1;
                GroupId = (reader["GroupID"] is int gid) ? gid : -1;
                TagId = (reader["TagID"] is int tid) ? tid : -1;
                Description = (reader["TAG"] is string desc) ? desc : null;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"TitleTags {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
