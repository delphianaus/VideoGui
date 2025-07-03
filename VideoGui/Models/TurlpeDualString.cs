using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class TurlpeDualString
    {
        public string turlpe1 { get; set; }
        public string turlpe2 { get; set; }

        public int Id { get; set; }
        public TurlpeDualString(string turlpe1, string turlpe2, int id=-1)
        {
            this.turlpe1 = turlpe1;
            this.turlpe2 = turlpe2;
            this.Id = id;
        }
    }
}
