using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VideoGui
{
    public class StatsHandler
    {
        List<Stats> RunningStats = new List<Stats>();
        public delegate void UpdateETA(string UpdateMessage);
        public delegate void UpdateTwoStrings(string A, string B);
        public delegate void UpdateSIngleString(string A);
        public delegate void UpdatePercents(string A, double B);
        public delegate void UpdateSpeeds(string bitrate, string bitratespeed, string fps, string frames, string Frames1440p);
        public UpdateETA _UpdateETA;
        public UpdateTwoStrings TotalDuration;
        public UpdateSIngleString TotalTime;
        public UpdatePercents ProgressValues;
        public UpdateSpeeds UpdateTotalSpeeds;
        public int count720p = 0;
        public int count1440p = 0;
        public int count4k = 0;
        object __lockObj;
        bool __lockWasTaken;
        public StatsHandler()
        {
            try
            {
                __lockWasTaken = false;
                __lockObj = new object();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void DoUnlock()
        {
            try
            {
                if (Monitor.IsEntered(__lockObj)) Monitor.Exit(__lockObj);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                __lockWasTaken = false;
            }
        }
        public string GetAllStats()
        {
            try
            {
                string result = string.Empty;
                foreach (Stats ss in RunningStats)
                {
                    result += (result == "") ? ss.filename : "|" + ss.filename;
                }

                return result;

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return string.Empty;
            }
        }
        private void DoLocking()
        {
            try
            {
                // Monitor.Wait(__lockObj, 100);
                if (!__lockWasTaken)
                {
                    if (!Monitor.IsEntered(__lockObj))
                    {
                        Monitor.TryEnter(__lockObj, ref __lockWasTaken);
                    }
                }
                else
                {
                    try
                    {
                        Monitor.Wait(__lockObj, 100);
                    }
                    catch (Exception ex)
                    {
                        // dont do anything
                        if (ex.Message == "")
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Argument must be initialized to false"))
                {
                    ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                }
            }
        }
        public bool FindFilename(string filename)
        {
            try
            {
                if (filename is null) return false;
                bool res = false;
                foreach (var _ in RunningStats.Where(ss => ss.filename.Trim() == filename.Trim()).Select(ss => new { }))
                {
                    res = true;
                    break;
                }
                return res;
            }

            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return false;
            }
            finally
            {

            }
        }
        public void AddNewStats(string filename)
        {
            try
            {
                if (!RunningStats.Any(job => job.filename == filename)) RunningStats.Add(new Stats(filename));
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {

            }
        }
        public Stats GetStats(string filename)
        {
            try
            {
                Stats statsA = null;
                foreach (var stats in RunningStats.Where(STATS => STATS.filename == filename))
                {
                    statsA = stats;
                    break;
                }
                return statsA;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {

            }
        }

        public void RemoveStats(string filename)
        {
            try
            {
                Stats stats = GetStats(filename);
                if (stats != null)
                {
                    RunningStats.Remove(stats);
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {

            }
        }

        public DateTime GetStartTime(string filename)
        {
            try
            {
                DateTime result = DateTime.UtcNow.AddYears(-100);
                foreach (var stats in RunningStats.Where(STATS => STATS.filename == filename))
                {
                    result = stats.StartTime;
                    break;
                }

                return result;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return DateTime.UtcNow.AddYears(-100);
            }
            finally
            {
                //DoUnlock();
            }

        }
        public void UpdateStartTime(string filename, DateTime StartTime)
        {
            try
            {
                foreach (var stats in RunningStats.Where(STATS => STATS.filename == filename))
                {
                    stats.StartTime = StartTime;
                    break;
                }
                // GetAverageTime();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                //DoUnlock();
            }
        }
        public void UpdateTime(string filename, TimeSpan eta)
        {
            try
            {
                //DoLocking();
                foreach (var stats in RunningStats.Where(STATS => STATS.filename == filename))
                {
                    stats.EstTime = eta;
                    break;
                }
                GetAverageEta();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                //DoUnlock();
            }
        }

        public void UpdateProcessingTime(string filename, TimeSpan eta)
        {
            try
            {
                //DoLocking();
                foreach (var stats in RunningStats.Where(STATS => STATS.filename == filename))
                {
                    stats.ProcessTime = eta;
                    break;
                }
                GetAverageprocTimes();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                //DoUnlock();
            }
        }
        public void UpdateSpeed(string filename, float fps, float bitrate, int frames, string frames1440p)
        {
            try
            {
                //DoLocking();

                foreach (var stats in RunningStats.Where(STATS => STATS.filename == filename))
                {
                    stats.fps = fps;
                    stats.bitrate = bitrate;
                    stats.frames = frames;
                    stats.CurrentFrame = frames1440p.ToInt();


                    break;
                }
                GetAverageSpeeds();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                //DoUnlock();
            }
        }

        public void GetAverageEta()
        {
            try
            {
                TimeSpan eta = TimeSpan.FromSeconds(0);
                foreach (Stats stats in RunningStats)
                {
                    eta += stats.EstTime;
                }
                if ((RunningStats.Count > 0) && (eta > TimeSpan.FromSeconds(0)))
                {
                    eta /= RunningStats.Count;
                    _UpdateETA?.Invoke(eta.ToCustomTimeString());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void GetAverageTime()
        {
            try
            {
                //DoLocking();
                TimeSpan Duration = TimeSpan.FromSeconds(0);
                TimeSpan TotalTime = TimeSpan.FromSeconds(0);
                foreach (Stats stats in RunningStats)
                {
                    Duration += stats.Duration;
                    TotalTime += (DateTime.Now - stats.StartTime);
                }
                if ((RunningStats.Count > 0) && (Duration > TimeSpan.FromSeconds(0) && (TotalTime > TimeSpan.FromSeconds(0))))
                {
                    TotalTime /= RunningStats.Count;
                    Duration /= RunningStats.Count;
                    TotalDuration?.Invoke(TotalTime.ToCustomTimeString(), Duration.ToCustomTimeString());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                //DoUnlock();
            }
        }

        public void GetAverageTimes()
        {
            try
            {
                //DoLocking();
                TimeSpan eta = TimeSpan.FromSeconds(0);
                foreach (Stats stats in RunningStats)
                {
                    eta += stats.EstTime;
                }
                if ((RunningStats.Count > 0) && (eta > TimeSpan.FromSeconds(0)))
                {
                    eta /= RunningStats.Count;
                    string fmt = @"mm\:ss";
                    //if (eta.TotalMinutes > 60) fmt = @"hh\:mm\:ss";
                    //if (eta.TotalDays > 0) fmt = @"dd\:hh\:mm\:ss";
                    _UpdateETA?.Invoke(eta.ToCustomTimeString());     // lblEta.Content = eta.ToString(@"mm\:ss");
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                //DoUnlock();
            }
        }

        public void GetAverageSpeeds()
        {
            try
            {
                //DoLocking();
                float fps = 0, bitrate = 0, frames = 0;
                int totalframes = 0;
                foreach (Stats stats in RunningStats)
                {
                    fps += stats.fps;
                    bitrate += stats.bitrate;
                    frames += stats.frames;
                    if (stats.CurrentFrame != -1) totalframes += stats.CurrentFrame;
                }
                if (RunningStats.Count > 0)
                {
                    string bitraatespeed = "";
                    string Bitrate = bitrate.ToBitrate(bitrate > 1024);
                    bitraatespeed = (bitrate > 1024) ? "MBits/s" : "KBits/s";
                    double fspstring = Math.Round(fps, 1);
                    UpdateTotalSpeeds?.Invoke(Bitrate, bitraatespeed, fspstring.ToString(), frames.ToString(), totalframes.ToString());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                //DoUnlock();
            }
        }
        public void GetAverageprocTimes()
        {
            try
            {
                TimeSpan eta = TimeSpan.FromSeconds(0);
                foreach (Stats stats in RunningStats)
                {
                    eta += stats.ProcessTime;
                }
                if ((RunningStats.Count > 0) && (eta > TimeSpan.FromSeconds(0)))
                {
                    eta /= RunningStats.Count;
                    TotalTime?.Invoke(eta.ToCustomTimeString());
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void GetAveragePercent()
        {
            try
            {
                double percents = RunningStats.Sum(percents => percents.Percent);
                if ((RunningStats.Count > 0) && (percents > 0))
                {
                    percents /= RunningStats.Count;
                    ProgressValues?.Invoke(Math.Round(percents,1).ToString() +" %", percents);
                }
            }

            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void UpdateProgress(string filename, double Progress, TimeSpan Duration, TimeSpan Total)
        {
            try
            {
                foreach (var stats in RunningStats.Where(STATS => STATS.filename == filename))
                {
                    stats.Percent = Progress;
                    stats.TotalTime = Total;
                    stats.Duration = Duration;
                    break;
                }
                GetAveragePercent();
                GetAverageTime();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
