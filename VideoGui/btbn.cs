using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;


namespace VideoGui
{
    class btbn : IUpdateParserHTML
    {
        [SupportedOSPlatform("windows")]
        (string, string, string, string) IUpdateParserHTML.ParseHTML(string HTMLSource)
        {
            HTMLSource = HTMLSource.Replace("\r\n\r\n", "").Replace("\r\n", "").Replace("\n", "");
            string filename = "", GrabBuild = "";
            List<string> Builds = new List<string>();
            string SearchStr;
            while (true)
            {
                if (HTMLSource == "") break;//<div class="+'\u0022'+"d-flex flex-justify-between
                string IndexStr = "<div class=" + '\u0022' + "d-flex flex-justify-between";
                if (HTMLSource.IndexOf(IndexStr) != -1)
                {
                    SearchStr = HTMLSource.Substring(HTMLSource.IndexOf(IndexStr));
                    if (SearchStr.IndexOf("</div>") != -1)
                    {
                        SearchStr = SearchStr.Substring(0, SearchStr.IndexOf("</div>") + "</div>".Length);
                        if ((SearchStr != "") && (SearchStr.Contains("a href=")))
                        {
                            IndexStr = "a href=" + '\u0022';
                            if (SearchStr.IndexOf(IndexStr) != -1)
                            {
                                string FileName = SearchStr.Substring(SearchStr.IndexOf(IndexStr) + IndexStr.Length);
                                FileName = FileName.Substring(0, FileName.IndexOf('\u0022' + " rel"));
                                if ((FileName != "") && (FileName.Contains("-gpl")) && (!FileName.Contains("vulkan")) && (FileName.Contains("ffmpeg-n")) && (FileName.Contains("win64")))
                                {
                                    if (FileName != "") Builds.Add(FileName);
                                }
                            }
                        }
                    }
                }
                else break;
                HTMLSource = HTMLSource.Substring(HTMLSource.IndexOf(SearchStr) + SearchStr.Length);
            }
            if (Builds.Count > 0)
            {
                GrabBuild = "https://github.com" + Builds.First<string>();
                string regdate = "";
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                if (key == null)
                {
                    Registry.CurrentUser.CreateSubKey(@"SOFTWARE\VideoProcessor");
                    key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                }
                bool LoadedKey = (key != null);
                string filever = LoadedKey ? (string)key.GetValue("ffmpegver", "") : string.Empty;
                key.Close();
                string idlink2 = "";
                if (filever != "")
                {
                    string buildver2 = filever.FindString("ffmpeg-", "-win64-static");
                    idlink2 = buildver2.Substring(buildver2.LastIndexOf("-") + 1);
                    regdate = buildver2.Substring(0, idlink2.Length + 1).Replace("-", "");
                }
                SearchStr = GrabBuild.FindString("autobuild-", "ffmpeg-n");
                SearchStr = (SearchStr != "") ? SearchStr.Substring(0, 10) : SearchStr;
                string idlink = GrabBuild.FindString("ffmpeg-n", "-win64");
                idlink = idlink.Substring(idlink.LastIndexOf("-") + 1);

                if (regdate != "") // check date
                {
                    DateTime MyDate1 = DateTime.ParseExact(SearchStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime MyDate2 = DateTime.ParseExact(regdate, "yyyyMMdd", CultureInfo.InvariantCulture);
                    if (((MyDate1 > MyDate2) || ((MyDate1 == MyDate2) && (idlink != idlink2))))
                    {
                        filename = "ffmpeg-" + SearchStr + "-" + idlink + "-win64 -static";
                    }
                    //return ("https://github.com" + Builds.First<string>(), filename);
                }
                //return (regdate != "" ? GrabBuild : "", "");
            }
            return (GrabBuild, filename, "", "");
        }

    }
}
