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
    public class VideoSchedules : INotifyPropertyChanged
    {
        private int _id = -1, _Days = 0; 
        private string _Name = "";
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        public int Days { get => _Days; set { _Days = value; OnPropertyChanged(); } }
        public VideoSchedules(int __id, string __Name, int __Days)
        {
            try
            {
                Id = __id;
                Name = __Name;
                Days = __Days;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"VideoSchedules {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public bool IsSunday()
        {
            try
            {
                return Days == 1;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsSunday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsMonday()
        {
            try
            {
                return Days == 2;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsMonday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsTuesday()
        {
            try
            {
                return Days == 4;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsTuesday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsWednesday()
        {
            try
            {
                return Days == 8;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsWednesday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsThursday()
        {
            try
            {
                return Days == 16;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsThursday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsFriday()
        {
            try
            {
                return Days == 32;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsFriday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public bool IsSaturday()
        {
            try
            {
                return Days == 64;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"IsSaturday {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }
        public VideoSchedules(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int ID) ? ID : -1;
                Name = (reader["NAME"] is string NAME) ? NAME : "";
                Days = (reader["DAYS"] is int DAYS) ? DAYS : 0;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"VideoSchedules_Reader {this} {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
