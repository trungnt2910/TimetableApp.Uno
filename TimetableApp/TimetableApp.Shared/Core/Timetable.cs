using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TimetableApp.Core
{
    [Serializable]
    public class Timetable
    {
        #region Private values
        private static readonly int DaysInAWeek = System.Globalization.DateTimeFormatInfo.CurrentInfo.DayNames.Length;
        private static string DataFilePath => Path.Combine(ApplicationData.Current.LocalFolder.Path, "TimetableAppData", "data.json");
        #endregion

        #region Fields
        //Name: class name, school, id,... for human use.
        public string Name;
        public string UpdateURL;

        //0 is Sunday.
        public List<Lesson>[] Lessons;

        public event EventHandler<EventArgs> OnSucessfulUpdate;
        #endregion

        #region Constructors
        //This class is designed to be used by JSON parsers.
        [JsonConstructor]
        public Timetable() { Lessons = Lessons ?? Enumerable.Range(0, DaysInAWeek).Select(x => new List<Lesson>()).ToArray(); }
        #endregion

        #region Queries
        public Lesson GetCurrentLesson()
        {
            DateTime currentTime = DateTime.Now;
            int day = (int)currentTime.DayOfWeek;
            TimeSpan time = currentTime.TimeOfDay;

            //A typical student cannot have more than 1e5 lessons a day, so yes,
            //brute-forcing is practically acceptable here.

            foreach (var l in Lessons[day])
            {
                if ((l.StartTime <= time) && (time <= l.EndTime))
                {
                    return l;
                }
            }

            //Hooray! No classes for you!
            return null;
        }

        public Lesson GetNextLesson(TimeSpan? MaxDelay)
        {
            DateTime currentTime = DateTime.Now;
            int day = (int)currentTime.DayOfWeek;
            TimeSpan time = currentTime.TimeOfDay;

            for (int i = day; i < day + 7; ++i)
            {
                int dayOfWeek = i % 7;
                foreach (var l in Lessons[dayOfWeek])
                {
                    var startTime = l.StartTime + TimeSpan.FromDays(i - day);
                    if (startTime < time) continue;
                    if (MaxDelay == null)
                    {
                        return l;
                    }
                    else
                    {
                        if (startTime <= time + MaxDelay) return l;
                    }
                }
            }
            

            return null;
        }

        public bool CheckNextLesson(TimeSpan? MaxDelay)
        {
            return GetNextLesson(MaxDelay) != null;
        }
        #endregion

        #region File operations
        public async Task<bool> UpdateAsync()
        {
            if (string.IsNullOrEmpty(UpdateURL)) return false;

            try
            {
                string oldMd5;
                string newMd5;

                Timetable newTimetable = null;

                using (var localFile = File.OpenRead(DataFilePath))
                {
                    var hasher = MD5.Create();
                    var hash = hasher.ComputeHash(localFile);
                    oldMd5 = string.Concat(hash.Select(x => x.ToString("X2")));
                }

                using (var client = new WebClient())
                {
#if !__WASM__
                    byte[] data = await client.DownloadDataTaskAsync(UpdateURL);
#else
                    byte[] data = await WasmWebClient.DownloadDataTaskAsync(UpdateURL);
#endif
                    using (var stream = new MemoryStream(data))
                    using (var sr = new StreamReader(stream))
                    {
                        var response = (UpdateResponse)JsonConvert.DeserializeObject(sr.ReadToEnd(), typeof(UpdateResponse));
                        newMd5 = response.MD5;

                        if (string.Compare(oldMd5, newMd5, true) != 0)
                        {
#if !__WASM__
                            data = await client.DownloadDataTaskAsync(response.Location);
#else
                            data = await WasmWebClient.DownloadDataTaskAsync(response.Location);
#endif
                            var hash = MD5.Create().ComputeHash(data);

                            var hashString = string.Concat(hash.Select(x => x.ToString("X2")));
                            if (string.Compare(newMd5, hashString, true) != 0)
                            {
                                return false;
                            }
                            using (var timetableStream = new MemoryStream(data))
                            using (var timetableReader = new StreamReader(timetableStream))
                            {
                                var timetableData = timetableReader.ReadToEnd().Replace("$ASSEMBLY_NAME", Assembly.GetExecutingAssembly().GetName().Name);
                                //throw new Exception(timetableData);
                                newTimetable =
                                    (Timetable)JsonConvert.DeserializeObject(timetableData, typeof(Timetable),
                                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                            }
                        }
                        else return true;
                    }
                }


                if (newTimetable != null)
                {
                    Name = newTimetable.Name;
                    UpdateURL = newTimetable.UpdateURL;
                    Lessons = newTimetable.Lessons;
                }

                try
                {
                    OnSucessfulUpdate?.Invoke(this, null);
                }
                catch { }

                SaveAsync();

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                //throw;
                return false;
            }
        }   

        private async Task ReloadAsync()
        {
            await ApplicationData.Current.LocalFolder.CreateFolderAsync("TimetableAppData", CreationCollisionOption.OpenIfExists);
            try
            {
                using (var stream = File.OpenText(DataFilePath))
                {
                    var newTimetable = (Timetable)JsonConvert.DeserializeObject(stream.ReadToEnd().Replace("$ASSEMBLY_NAME", Assembly.GetExecutingAssembly().GetName().Name), typeof(Timetable),
                        new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });
                    if (newTimetable != null)
                    {
                        Name = newTimetable.Name;
                        UpdateURL = newTimetable.UpdateURL;
                        Lessons = newTimetable.Lessons;
                        OnSucessfulUpdate?.Invoke(this, null);
                    }
                    else
                    {
                        await SaveAsync();
                    }
                }
            }
            catch
            {
                await SaveAsync();
            }

        }

        public static Timetable Load()
        {
            try
            {
                // File system has not been initialized yet.
                if (PlatformHelper.RuntimePlatform == Platform.WASM)
                {
                    var timetable = new Timetable();
                    timetable.ReloadAsync();
                    return timetable;
                }
                using (var stream = File.OpenText(DataFilePath))
                {
                    var timetable = (Timetable)JsonConvert.DeserializeObject(stream.ReadToEnd().Replace("$ASSEMBLY_NAME", Assembly.GetExecutingAssembly().GetName().Name), typeof(Timetable),
                        new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });
                    if (timetable == null)
                    {
                        timetable = new Timetable();
                        // Yes, no await doesn't do any harm. The UI can continue work normally.
                        timetable.SaveAsync();
                    }
                    return timetable;
                }
            }
            catch
            {
                var timetable = new Timetable();
                timetable.SaveAsync();
                return timetable;
            }
        }

        public async Task SaveAsync()
        {
            await ApplicationData.Current.LocalFolder.CreateFolderAsync("TimetableAppData", CreationCollisionOption.OpenIfExists);
            using (var stream = File.CreateText(DataFilePath))
            {
                stream.Write(JsonConvert.SerializeObject(this,
                    new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    }
                ).Replace(Assembly.GetExecutingAssembly().GetName().Name, "$ASSEMBLY_NAME"));
            }
        }
#endregion
    }
}
