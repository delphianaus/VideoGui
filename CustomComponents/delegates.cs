using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents
{
    public class delegates
    {
        public delegate void ErrorHandler(Exception _exception, string message,string _callingMethod="");
        public delegate void SortChangedHandler(object sender, bool direction);

    }
}
