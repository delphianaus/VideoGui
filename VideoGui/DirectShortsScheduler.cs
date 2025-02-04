﻿using System;
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
        List<ListScheduleItems> ScheduleList = new List<ListScheduleItems>();
        public DateTime StartDate = DateTime.Now, EndDate = DateTime.Now, LastValidDate = DateTime.Now;
        List<DateTime> AvailableSchedules = new List<DateTime>();
        ReportVideoScheduled DoReportScheduled = null;
        public int MaxNumberSchedules = 100, ScheduleNumber = 0;
        bool setup = false, BeginMode = false, FinishMode = false, FirstTime = false;
        public bool CanSchedule = true;
        public DirectshortsScheduler(OnFinish doOnFinish, OnFinishBool doOnFinishSchedulesComplete, List<ListScheduleItems> listSchedules,
            DateTime startDate, DateTime endDate, ReportVideoScheduled doReportSchedule, int maxNumberSchedules)
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
        public async Task<bool> ApplyVideoSchedule(string videoId, string title, DateTime ScheduleAt, string desc = "")
        {
            try
            {

                UserCredential credential;
                string[] scopes = new string[] { "https://www.googleapis.com/auth/youtube.upload",
                                                 "https://www.googleapis.com/auth/youtube",
                                                 "https://www.googleapis.com/auth/youtube.channel-memberships.creator",
                                                 "https://www.googleapis.com/auth/youtube.force-ssl",
                                                 "https://www.googleapis.com/auth/youtube.readonly",
                                                 "https://www.googleapis.com/auth/youtubepartner",
                                                 "https://www.googleapis.com/auth/youtubepartner-channel-audit"};
                string fLocation = GetExePath() + "\\client_secrets.json";
                using (var stream = new FileStream(fLocation, FileMode.Open, FileAccess.Read))
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
                        if (desc != "")
                        {
                            video.Snippet.Description = desc;
                            video.Snippet.Title = title;
                        }
                        var updateRequest = youtubeService.Videos.Update(video, $"Id,snippet,status");
                        if (ScheduleNumber > 118)
                        {
                            if (true)
                            {

                            }
                        }
                        updateRequest.Execute();
                        ScheduleNumber++;

                        LastValidDate = ScheduleAt;
                        DoReportScheduled(ScheduleAt, videoId, title);

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


        public void ScheduleVideo(string videoId, string title, string desc, bool UseNewStart)
        {
            try
            {
                if (CanSchedule)
                {
                    if (AvailableSchedules.Count > -1)
                    {
                        DoSchedule(videoId, title, UseNewStart, desc);
                    }
                    else
                    {
                        DateTime newSchedule = AvailableSchedules.FirstOrDefault();
                        AvailableSchedules.RemoveAt(0);
                        ApplyVideoSchedule(videoId, title, newSchedule, desc).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduleVideo {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        

        public void DoSchedule(string videoId, string title, bool UseNewStart, string desc = "")
        {
            try
            {
                LastValidVideoScheduledAt = CurrentDate.ToDateTime(CurrentTime);
                if (!CurrentDate.ToDateTime(CurrentTime).IsBetween(StartDate, EndDate))
                {

                }
                bool IsStartMode = BeginMode && CurrentDate.ToDateTime(CurrentTime) >= StartDate;
                bool IsEndMode = FinishMode && (CurrentDate.ToDateTime(CurrentTime) >= StartDate &&
                                                CurrentDate.ToDateTime(CurrentTime) <= EndDate);

                IsStartMode = IsStartMode;//|| IsEndMode
                if (CurrentDate.ToDateTime(CurrentTime).IsBetween(StartDate, EndDate) || IsEndMode || IsStartMode)
                {
                    bool IsValid = false;
                    while (!IsValid)
                    {
                        if (ListScheduleIndex < ScheduleList.Count)
                        {
                            var _end = ScheduleList.LastOrDefault().End;
                            if (IsEndMode || IsStartMode)
                            {
                                var ctp = CurrentDate.ToDateTime(CurrentTime);
                                var dtp = CurrentDate.ToDateTime(_end);
                                if (IsStartMode && ctp.IsBetween(StartDate, EndDate))
                                {
                                    if (StartDate.Day == EndDate.Day)
                                    {
                                        IsValid = true;
                                        break;
                                    }
                                    else if (ctp > dtp)
                                    {
                                        CurrentDate = CurrentDate.AddDays(1);
                                        if (UseNewStart)
                                        {
                                            CurrentTime = TimeOnly.FromDateTime(StartDate);
                                        }
                                        else
                                        {
                                            CurrentTime = ScheduleList[ListScheduleIndex].Start;
                                        }
                                        IsValid = true;
                                        break;
                                    }
                                    IsValid = true;
                                    break;
                                }
                                else if (IsEndMode && ctp.IsBetween(StartDate, EndDate))
                                {
                                    if (StartDate.Day == EndDate.Day)
                                    {
                                        IsValid = true;
                                        break;
                                    }
                                    else if (ctp > dtp)
                                    {
                                        CurrentDate = CurrentDate.AddDays(1);
                                        if (UseNewStart)
                                        {
                                            CurrentTime = TimeOnly.FromDateTime(StartDate);
                                        }
                                        else
                                        {
                                            CurrentTime = ScheduleList[ListScheduleIndex].Start;
                                        }
                                        IsValid = true;
                                        break;
                                    }
                                }
                            }
                            else if (CurrentTime >= _end)
                            {
                                CurrentDate = CurrentDate.AddDays(1);
                                if (UseNewStart)
                                {
                                    CurrentTime = TimeOnly.FromDateTime(StartDate);
                                }
                                else
                                {
                                    CurrentTime = ScheduleList[ListScheduleIndex].Start;
                                }
                                if (CurrentDate.ToDateTime(CurrentTime).IsBetween(StartDate, EndDate))
                                {
                                    IsValid = true;
                                    break;
                                }
                                else
                                {
                                    if (!FirstTime && (CurrentDate.ToDateTime(CurrentTime) > EndDate))
                                    {
                                        FirstTime = true;
                                        IsValid = !(CurrentDate > DateOnly.FromDateTime(EndDate.Date));
                                        if (!IsValid)
                                        {
                                            DoOnFinishSchedulesComplete?.Invoke();
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        DoOnFinishSchedulesComplete?.Invoke();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (CurrentTime.IsBetween(ScheduleList[ListScheduleIndex].Start, ScheduleList.LastOrDefault().End))
                                {
                                    if (CurrentDate.ToDateTime(CurrentTime).IsBetween(StartDate, EndDate))
                                    {
                                        IsValid = true;
                                        break;
                                    }
                                    else
                                    {
                                        if (true)
                                        {

                                        }
                                    }
                                }
                                else ListScheduleIndex++;
                                if (ListScheduleIndex >= ScheduleList.Count)
                                {
                                    ListScheduleIndex = 0;
                                }
                            }
                        }
                    }
                    if (IsValid)
                    {
                        ApplyVideoSchedule(videoId, title, CurrentDate.ToDateTime(CurrentTime)).ConfigureAwait(false);
                        int WrappedDays = 0;
                        CurrentTime = CurrentTime.AddMinutes(ScheduleList[ListScheduleIndex].Gap, out WrappedDays);
                        if (WrappedDays > 0)
                        {
                            CurrentDate = CurrentDate.AddDays(1);
                            CurrentTime = ScheduleList.FirstOrDefault().Start;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"DoSchedule {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
    }
}
