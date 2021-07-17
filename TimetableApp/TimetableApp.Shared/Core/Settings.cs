using System;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Text;

using Windows.Storage;
using Windows.Foundation.Collections;

namespace TimetableApp.Core
{
    public class Settings
    {
        static ApplicationDataContainer Local = ApplicationData.Current.LocalSettings;
        static ApplicationDataContainer Roaming = ApplicationData.Current.RoamingSettings;

        private static JsonSerializerSettings JsonOptions = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        static Settings()
        {
            try
            {
                CleanInvalidSettings(Local.Values);
                CleanInvalidSettings(Roaming.Values);
                // Some platforms don't support enumerating Settings.
                SyncSettings();
            }
            catch
            { }
        }

        private static void UpdateSetting(object sender, EventArgs args)
        {
            var setting = sender as SettingBase;
            var key = setting.Key;
            var json = JsonConvert.SerializeObject(setting, JsonOptions);
            Local.Values[key] = json;
            Roaming.Values[key] = json;
            System.Diagnostics.Debug.WriteLine($"Updated setting with key: {key} and value: {json}");
        }

        private static Setting<T> GetSetting<T>(string key, T defaultValue)
        {
            if (Local.Values.ContainsKey(key))
            {
                try
                {
                    var setting = JsonConvert.DeserializeObject<Setting<T>>(Local.Values[key] as string, JsonOptions);
                    System.Diagnostics.Debug.WriteLine(key);
                    System.Diagnostics.Debug.WriteLine(Local.Values[key]);
                    setting.Key = key;
                    setting.ValueChanged += UpdateSetting;
                    return setting;
                }
                catch (JsonSerializationException)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to deserialize setting: {key}. Using default value.");
                    Local.Values.Remove(key);
                }
            }

            var newSetting = new Setting<T> { Value = defaultValue };
            newSetting.Key = key;
            newSetting.ValueChanged += UpdateSetting;
            var json = JsonConvert.SerializeObject(newSetting as SettingBase, JsonOptions);
            Local.Values.Add(key, json);
            if (!Roaming.Values.ContainsKey(key))
                Roaming.Values.Add(key, json);
            return newSetting;
        }

        private static void SyncSettings()
        {
            foreach (var kvp in Roaming.Values)
            {
                if (Local.Values.ContainsKey(kvp.Key))
                {
                    if (JsonConvert.DeserializeObject<SettingBase>(Local.Values[kvp.Key] as string, JsonOptions).TimeStamp < JsonConvert.DeserializeObject<SettingBase>(kvp.Value as string, JsonOptions).TimeStamp)
                    {
                        Local.Values[kvp.Key] = kvp.Value;
                    }
                }
                else
                {
                    Local.Values.Add(kvp);
                }
            }
            Roaming.Values.Clear();
            foreach (var kvp in Local.Values)
            {
                Roaming.Values.Add(kvp);
            }
        }

        private static void CleanInvalidSettings(IPropertySet properties)
        {
            var toRemove = new List<string>();
            foreach (var kvp in properties)
            {
                try
                {
                    _ = JsonConvert.DeserializeObject<SettingBase>(kvp.Value as string, JsonOptions);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to deserialize setting: {kvp.Key}: {e.GetType()}: {e.Message}");
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var r in toRemove)
            {
                properties.Remove(r);
            }
        }

        private static readonly Setting<bool> AutoJoinSetting = GetSetting("AutoJoin", false);
        private static readonly Setting<TimeSpan> ReportBeforeTimeSetting = GetSetting("ReportBeforeTime", TimeSpan.Parse("00:15:00"));
        private static readonly Setting<bool> AlwaysReportSetting = GetSetting("AlwaysReport", false);
        private static readonly Setting<TimeSpan> AllowJoinBeforeTimeSetting = GetSetting("AllowJoinBeforeTime", TimeSpan.Parse("00:05:00"));
        private static readonly Setting<bool> AlwaysAllowJoinSetting = GetSetting("AlwaysAllowJoin", false);
        private static readonly Setting<string> UserNameSetting = GetSetting("UserName", string.Empty);

        public static bool AutoJoin
        {
            get => AutoJoinSetting.Value;
            set => AutoJoinSetting.Value = value;
        }

        public static TimeSpan ReportBeforeTime
        {
            get => ReportBeforeTimeSetting.Value;
            set => ReportBeforeTimeSetting.Value = value;
        }

        public static bool AlwaysReport
        {
            get => AlwaysReportSetting.Value;
            set => AlwaysReportSetting.Value = value;
        }

        public static TimeSpan AllowJoinBeforeTime
        {
            get => AllowJoinBeforeTimeSetting.Value;
            set => AllowJoinBeforeTimeSetting.Value = value;
        }

        public static bool AlwaysAllowJoin
        {
            get => AlwaysAllowJoinSetting.Value;
            set => AlwaysAllowJoinSetting.Value = value;
        }

        public static string UserName
        {
            get => UserNameSetting.Value;
            set => UserNameSetting.Value = value;
        }
    }
}
