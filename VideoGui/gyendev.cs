using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;


namespace VideoGui
{
    [SupportedOSPlatform("windows")]
    class Gyendev : IUpdateParserHTML
    {
        public string DecryptPassword(byte[] _password)
        {
            int[] AccessKey = { 30, 11, 32, 157, 14, 22, 138, 249, 133, 44, 16, 228, 199, 00, 111, 31, 17, 74, 1, 8, 9, 33,
                44, 66, 88, 99, 00, 11, 132, 157, 174, 21, 18, 93, 233, 244, 66, 88, 199, 00, 11, 232, 157, 174, 31, 8, 19, 33, 44, 66, 88, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 22, 44, 62, 132, 233, 122, 27, 41, 44, 136, 172, 223, 132, 33, 25, 16 };
            byte[] encvar = EMP.RC4(_password, EncKey);
            return Encoding.ASCII.GetString(encvar);
        }
        public string GetEncryptedString(byte[] encriptedString)
        {
            try
            {
                return DecryptPassword(encriptedString);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
            return "";
        }
        (string, string, string, string) IUpdateParserHTML.ParseHTML(string HTMLSource)
        {
            string downloadlink = "", Version = "", filename = "", link = "", GitVersion = "";
            string LNK = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-git-full.7z",
            FNAME = "ffmpeg-git-full.7z";

            HTMLSource = HTMLSource.Replace("\r\n\r\n", "").Replace("\r\n", "").Replace("\n", "");
            if (HTMLSource != "")
            {
                string processingstr = HTMLSource;
                if (processingstr.Contains("latest git master branch build"))
                {
                    int xy = processingstr.IndexOf("latest git master branch build");
                    string ss = processingstr.Substring(xy);
                    if (ss != "")
                    {
                        int gtv = ss.IndexOf("git-version");
                        int span = ss.ToLower().IndexOf("span");
                        if ((gtv != -1) && (span != -1))
                        {
                            string veri = ss.Substring(gtv + "git-version".Length + 2, gtv + "git-version".Length + 5 - span);
                            if (veri != "")
                            {
                                string ffmpegdate = "";
                                DateOnly RegFFMPEGDate;
                                bool ValidKey=false,DateStored = false;
                                string WebVerDate = veri.Substring(0, 10);
                                GitVersion = veri[WebVerDate.Length..].Replace("-git-","");
                                Version = WebVerDate;
                                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                                if (key == null)
                                {
                                    Registry.CurrentUser.CreateSubKey(@"SOFTWARE\VideoProcessor");
                                    key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\VideoProcessor", true);
                                }
                                bool LoadedKey = key != null;
                                if (LoadedKey)
                                {
                                    string GVer = key.RegistryValueExists("ffmpeg_gitver") ? key.GetString("ffmpeg_gitver") : "";
                                    if (key.RegistryValueExists("ffmpeg_date"))
                                    {
                                        ValidKey = true;
                                        ffmpegdate = key.GetString("ffmpeg_date");
                                        DateStored = ffmpegdate.ParseDate(out RegFFMPEGDate);
                                        string SourceAssembly = "";
                                        RegistryKey key2 = "SOFTWARE\\VideoProcessor".OpenSubKey(Registry.CurrentUser);
                                        string defaultprogramlocation = key2?.GetValueStr("defaultprogramlocation", "c:\\videogui");
                                        key2?.Close();
                                        SourceAssembly = (Debugger.IsAttached) ? defaultprogramlocation : System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                                        bool downloadff = false;
                                        bool ffmpeg = false, ffprobe = false;
                                        List<string> PathListFF = Directory.EnumerateFiles(SourceAssembly, "ff*.exe", Debugger.IsAttached ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories).
                                        Where(s => s.EndsWith(".exe")).ToList<string>();
                                        var ffm = GetEncryptedString(new int[] { 144, 57, 66, 70, 244, 192, 128, 86, 234, 120 }.Select(i => (byte)i).ToArray());
                                        var ffp = GetEncryptedString(new int[] { 144, 57, 95, 68, 254, 197, 203, 29, 247, 101, 8 }.Select(i => (byte)i).ToArray());
                                        foreach (string sPath in PathListFF)
                                        {
                                            ffmpeg = (sPath.Contains(ffm)) ? true : ffmpeg;
                                            ffprobe = (sPath.Contains(ffp)) ? true : ffprobe;
                                        }
                                        downloadff = !(ffmpeg && ffprobe);
                                        if (DateStored || downloadff )
                                        {
                                            DateOnly SavedWebDate;
                                            if ((!downloadff) && (WebVerDate.ParseDate(out SavedWebDate)))
                                            {
                                                if (RegFFMPEGDate <= SavedWebDate)
                                                {
                                                    if ((RegFFMPEGDate == SavedWebDate) && (GitVersion != GVer))
                                                    {
                                                        (link, filename) = (LNK, FNAME);
                                                    }
                                                    else if (RegFFMPEGDate != SavedWebDate)
                                                    {
                                                        (link, filename) = (LNK, FNAME);
                                                    }
                                                }
                                                
                                            }
                                            else
                                            {
                                                (link, filename) = (LNK, FNAME);
                                            }
                                        }
                                        else
                                        {
                                            (link, filename) = (LNK, FNAME);
                                        }
                                    }

                                    if (!ValidKey)
                                    {
                                        (link, filename) = (LNK, FNAME);
                                    }
                                    key.Close();
                                }
                            }
                        }
                    }
                }
                return (link, filename, Version, GitVersion);
            }
            else return ("", "" , "", "");
        }
    }
}
