using System;
using System.Collections.Generic;
using System.Text;

namespace VideoGui
{
    public interface IUpdateParserHTML
    {
        (string, string, string, string) ParseHTML(string HTMLSource);
    }
}
