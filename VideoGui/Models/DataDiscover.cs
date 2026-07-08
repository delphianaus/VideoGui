using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class DataDiscover
    {
        public string TableName { get; set; } = "";
        public List<string> Fields { get; set; } = new List<string>();
        public DataDiscover(string tableName)
        {
            TableName = tableName;
        }

        public DataDiscover(FbDataReader reader)
        {
            TableName = reader.GetString(0).ToString().Trim();
            Fields = new List<string>();
        }
    }
}
