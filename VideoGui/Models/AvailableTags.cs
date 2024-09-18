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
    public class AvailableTags : INotifyPropertyChanged
    {
        private int _Id;
        public string _Tag;
        public string Tag { get => _Tag; set { _Tag = value; OnPropertyChanged(); } }
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        public AvailableTags(string _tag, int _id)
        {
            Tag = _tag;
            Id = _id;
        }

        public AvailableTags(FbDataReader reader)
        {
            try
            {
                int _Id = reader["Id"].ToInt();
                string _Tag = reader["Tag"].ToString();
                Tag = _Tag;
                Id = _Id;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AvailableTags {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
