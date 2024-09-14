using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;


namespace VideoGui
{
    [SupportedOSPlatform("windows")]
    class Ottverse : IUpdateParserHTML
    {
        public (string, string, string, string) ParseHTML(string HTMLSource)
        {
            string filename = "", link = "";
            List<string> Links = new();
            if (HTMLSource != "")
            {
                string A1 = HTMLSource.FindBetween("<tbody>", "</tbody>");
                if (A1 != "")
                {
                    filename = A1.FindBetween("<tr><td>", "</td><td>");
                    do
                    {
                        link = A1.FindBetween("<a href=", ">static</a>");
                        if (link != "") Links.Add(link);
                        A1 = A1[(A1.IndexOf(link) + link.Length + "ref=static</a>".Length)..];
                    }
                    while (link != "");
                    foreach (var linky in Links.Where(linky => linky.Contains(filename.Substring(filename.LastIndexOf("-") + 1))))
                    {
                        link = linky;
                        break;
                    }
                    filename += "-win64-static";
                }
            }
            return (link, filename, "", "");
        }
    }
}
