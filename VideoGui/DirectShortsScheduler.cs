using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoGui.Models.delegates;
using VideoGui.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Microsoft.Web.WebView2.Wpf;
using FirebirdSql.Data.FirebirdClient;
using Windows.ApplicationModel.Activation;

namespace VideoGui
{
    public class DirectshortsScheduler
    {
        OnFinish DoOnFinish = null;
        OnFinishBool DoOnFinishSchedulesComplete = null;
        public int ListScheduleIndex = 0;
        int DateIndex = 0;
        DateTime LastValidVideoScheduledAt = DateTime.MinValue;
        TimeOnly CurrentTime = new TimeOnly();
        DateOnly CurrentDate = DateOnly.FromDateTime(DateTime.Now);
        TimeOnly LastValidTime = TimeOnly.FromTimeSpan(TimeSpan.Zero);
        DateTime ScheduleAt = DateTime.Now.AddYears(-500);
        public string connectionString = "";
        bool IsTest = true, IsFirstTime = true;
        public CancellationTokenSource canceltoken = new CancellationTokenSource();
        List<ListScheduleItems> ScheduleList = new List<ListScheduleItems>();
        public DateTime StartDate = DateTime.Now, EndDate = DateTime.Now, LastValidDate = DateTime.Now;
        List<DateTime> AvailableSchedules = new List<DateTime>();
        ReportVideoScheduled DoReportScheduled = null;
        public int MaxNumberSchedules = 100, ScheduleNumber = 0;
        bool setup = false, BeginMode = false, FinishMode = false, FirstTime = false;
        public bool CanSchedule = true;
        public DirectshortsScheduler(OnFinish doOnFinish, OnFinishBool doOnFinishSchedulesComplete,
            List<ListScheduleItems> listSchedules,
            DateTime startDate, DateTime endDate, ReportVideoScheduled doReportSchedule, int maxNumberSchedules,
            bool isTest)
        {
            try
            {

                DoOnFinishSchedulesComplete = doOnFinishSchedulesComplete;
                MaxNumberSchedules = maxNumberSchedules;
                DoReportScheduled = doReportSchedule;
                DoOnFinish = doOnFinish;
                ScheduleList = listSchedules;
                StartDate = startDate;
                EndDate = endDate;
                IsTest = isTest;
                DateTime ScheduleStart = DateOnly.FromDateTime(startDate).ToDateTime(listSchedules.FirstOrDefault().Start);
                DateTime ScheduleEnd = DateOnly.FromDateTime(EndDate).ToDateTime(listSchedules.LastOrDefault().End);
                if (startDate <= ScheduleStart && endDate <= ScheduleStart)
                {
                    BeginMode = true;
                }
                else if (startDate > endDate && endDate > ScheduleEnd)
                {
                    FinishMode = true;
                }
                else if (startDate < DateOnly.FromDateTime(startDate).ToDateTime(listSchedules.FirstOrDefault().Start))
                {
                    startDate = DateOnly.FromDateTime(startDate).ToDateTime(listSchedules.FirstOrDefault().Start);
                }
                CurrentTime = TimeOnly.FromDateTime(startDate);
                CurrentDate = DateOnly.FromDateTime(startDate.Date);
                if (listSchedules.LastOrDefault() is not null)
                {
                    LastValidTime = listSchedules.LastOrDefault().End;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DirectshortsScheduler {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public void ScheduleNewDay(DateTime startDate, DateTime endDate)
        {
            try
            {
                StartDate = startDate;
                EndDate = endDate;
                DateTime ScheduleStart = DateOnly.FromDateTime(startDate).ToDateTime(ScheduleList.FirstOrDefault().Start);
                DateTime ScheduleEnd = DateOnly.FromDateTime(EndDate).ToDateTime(ScheduleList.LastOrDefault().End);
                TimeOnly StartTime = TimeOnly.FromDateTime(startDate);
                TimeOnly EndTime = TimeOnly.FromDateTime(endDate);
                if (startDate <= ScheduleStart && endDate <= ScheduleStart)
                {
                    BeginMode = true;
                }
                else if (startDate > endDate && endDate > ScheduleEnd)
                {
                    FinishMode = true;
                }
                else if (startDate < DateOnly.FromDateTime(startDate).ToDateTime(ScheduleList.FirstOrDefault().Start))
                {
                    startDate = DateOnly.FromDateTime(startDate).ToDateTime(ScheduleList.FirstOrDefault().Start);
                }
                CurrentTime = TimeOnly.FromDateTime(startDate);
                CurrentDate = DateOnly.FromDateTime(startDate.Date);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduleNewDay {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public string GetExePath()
        {
            try
            {
                string res = "";
                res = (Debugger.IsAttached) ? @"c:\VideoGUI" : Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                return res;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"GetExePath {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "";
            }
        }

        public byte[] CryptData(byte[] _password)
        {
            int[] AccessKey = { 32, 16, 22, 157, 214, 12, 138, 249, 133, 244, 116, 28, 99, 00, 111, 131, 17, 174, 21,
                88, 99, 33, 44, 166, 88, 99, 100, 11, 232, 157, 74, 1, 28, 39, 33, 244, 166, 88, 99, 100,
                14, 132, 157, 74, 123, 28, 49, 233, 144, 166, 188, 99 };
            EncryptionModule EMP = new EncryptionModule(AccessKey, AccessKey.Length);
            byte[] EncKey = { 122, 244, 162, 232, 133, 222, 127, 141, 244, 136, 172, 223, 132, 233, 125, 126 };
            byte[] encvar = EMP.RC4(_password, EncKey);
            return encvar;
        }
        public Stream GetClientSecrets()
        {
            try
            {
                byte[] clientSecret = Array.Empty<byte>();
                CancellationTokenSource cts = new CancellationTokenSource();
                connectionString.ExecuteReader("SELECT * FROM SETTINGS WHERE SETTINGNAME = 'CLIENT_SECRET';", cts,(FbDataReader r) =>
                {
                    clientSecret = (r["SETTINGBLOB"] is System.Byte[] res) ? CryptData(res) : Array.Empty<byte>();
                    cts.Cancel();
                });
                return (clientSecret.Length > 0) ? new MemoryStream(clientSecret) : new MemoryStream();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name + " " + ex.Message);
                return new MemoryStream();
            }
        }
        public async Task<bool> ApplyVideoSchedule(string videoId, string Title_str, string Desc_Str, DateTime ScheduleAt) //ApplyVideoSchedule(string videoId, string title_str, string Desc_Str,DateTime ScheduleAt)
        {
            try
            {
                // no client secret
                UserCredential credential;
                string[] scopes = new string[] { "https://www.googleapis.com/auth/youtube.upload",
                                                 "https://www.googleapis.com/auth/youtube",
                                                 "https://www.googleapis.com/auth/youtube.channel-memberships.creator",
                                                 "https://www.googleapis.com/auth/youtube.force-ssl",
                                                 "https://www.googleapis.com/auth/youtube.readonly",
                                                 "https://www.googleapis.com/auth/youtubepartner",
                                                 "https://www.googleapis.com/auth/youtubepartner-channel-audit"};
                // string fLocation = GetExePath() + "\\client_secrets.json";
                using (var stream = GetClientSecrets())
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        clientSecrets: GoogleClientSecrets.Load(stream).Secrets,
                        scopes, "user", CancellationToken.None, new FileDataStore("YouTubeHelper.MainWindow")
                    );

                    var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = GetType().ToString()
                    });

                    // Get video details
                    var videoRequest = youtubeService.Videos.List("snippet,status");
                    videoRequest.Id = videoId;
                    var videoResponse = videoRequest.Execute();
                    var video = videoResponse.Items.FirstOrDefault();
                    if (video != null && video.Status is not null && video.Status.PrivacyStatus != "public")
                    {
                        DateTime publishDateTime = ScheduleAt;
                        publishDateTime = DateTime.SpecifyKind(publishDateTime, DateTimeKind.Local);
                        video.Status.PublishAtDateTimeOffset = publishDateTime;
                        video.Status.PublishAt = publishDateTime;
                        video.Status.PrivacyStatus = "private";
                        if (Desc_Str != "" && Title_str != "")
                        {
                            int iidx = Title_str.IndexOf("#");
                            if (iidx != -1)
                            {
                                string r = Title_str.Substring(0, iidx - 1);
                                Title_str= r.ToPascalCase()+ " " +Title_str.Substring(iidx);
                            }
                            if (Desc_Str.ToLower().Contains("https://www.patreon.com/c/JustinsTrainJourneys".ToLower()))
                            {
                                Desc_Str = Desc_Str.Replace("https://www.patreon.com/c/JustinsTrainJourneys",
                                    "https://www.patreon.com/join/JustinsTrainJourneys");
                            }
                            if (!Desc_Str.ToLower().Contains("https://www.patreon.com/join/JustinsTrainJourneys".ToLower()))
                            {
                               Desc_Str += "\n\nSupport Me On Patreon: https://www.patreon.com/join/JustinsTrainJourneys";
                            }

                            video.Snippet.Description = Desc_Str;
                            video.Snippet.Title = Title_str;

                            var updateRequest = youtubeService.Videos.Update(video, $"Id,snippet,status");
                            
                            updateRequest.Execute();
                            ScheduleNumber++;

                            LastValidDate = ScheduleAt;
                        }
                        //DoReportScheduled(ScheduleAt, videoId, Title_str);

                        return true;
                    }
                    else
                    {
                        AvailableSchedules.Add(ScheduleAt);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                string[] ms = new string[] { "youtube", "exception", "exceeded", "quota" };

                if (message.ContainsAll(ms))
                {
                    string ls = $"YouTube Quota Exceeded @ ";
                    CanSchedule = false;
                    DoReportScheduled?.Invoke(DateTime.Now, "", ls);
                }
                ex.LogWrite($"ApplyVideoSchedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                AvailableSchedules.Add(ScheduleAt);
                return false;
            }
        }


        public bool ScheduleVideo(string videoId, string TitleStr, string DescStr, bool UseNewStart)
        {
            try
            {
                if (CanSchedule)
                {
                    if (AvailableSchedules.Count == 0)
                    {
                        return DoSchedule(videoId, TitleStr, DescStr);
                    }
                    else
                    {
                        DateTime newSchedule = AvailableSchedules.FirstOrDefault();
                        AvailableSchedules.RemoveAt(0);
                        return ApplyVideoSchedule(videoId, TitleStr, DescStr, newSchedule).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                }
                else return false;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduleVideo {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }


        public bool DoSchedule(string videoId, string TitleStr, string DescStr)
        {
            try
            {
                TimeSpan GapTime = TimeSpan.Zero;
                if (FirstTime)
                {
                    FirstTime = false;
                    CurrentDate = DateOnly.FromDateTime(StartDate);
                    CurrentTime = TimeOnly.FromDateTime(StartDate);
                }
                var Start = ScheduleList.FirstOrDefault().Start;
                var End = ScheduleList.LastOrDefault().End;
                TimeOnly StartTime = TimeOnly.FromDateTime(StartDate);
                TimeOnly EndTime = TimeOnly.FromDateTime(EndDate);
                if (CurrentTime >= Start && CurrentTime <= End)
                {
                    foreach (var Schedule in ScheduleList)
                    {
                        if (CurrentTime >= Schedule.Start && CurrentTime <= Schedule.End)
                        {
                            GapTime = TimeSpan.FromMinutes(Schedule.Gap);
                            break;
                        }
                    }
                }
                else
                {
                    if (CurrentTime < Start)
                    {
                        GapTime = TimeSpan.FromMinutes(ScheduleList.FirstOrDefault().Gap);
                    }
                    else if (CurrentTime > End)
                    {
                        GapTime = TimeSpan.FromMinutes(ScheduleList.LastOrDefault().Gap);
                    }
                }
                if (GapTime != TimeSpan.Zero)
                {
                    var ScheduleDate = CurrentDate.ToDateTime(CurrentTime);
                    if (ScheduleDate >= StartDate && ScheduleDate <= EndDate && ScheduleNumber < MaxNumberSchedules)
                    {
                        if (!IsTest)
                        {
                            var r = ApplyVideoSchedule(videoId, TitleStr, DescStr, ScheduleDate).ConfigureAwait(false).GetAwaiter().GetResult();
                            if (r)
                            {
                                DoReportScheduled(ScheduleDate, videoId, TitleStr);
                                CurrentTime = CurrentTime.Add(GapTime);
                            }
                            return r;
                        }
                        else
                        {
                            DoReportScheduled(ScheduleDate, videoId, TitleStr);
                            ScheduleNumber++;
                            CurrentTime = CurrentTime.Add(GapTime);
                            return true;
                        }

                    }
                    else if (ScheduleDate > EndDate)
                    {
                        DoOnFinishSchedulesComplete.Invoke();
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoSchedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return false;
            }
        }

    }
}
