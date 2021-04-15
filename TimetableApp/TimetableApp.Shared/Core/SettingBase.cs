using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableApp.Core
{
    public class SettingBase
    {
        [JsonProperty]
        public DateTime TimeStamp { get; private set; }
        
        [JsonIgnore]
        public string Key { get; set; }

        public SettingBase()
        {
            TimeStamp = DateTime.Now;
        }

        [JsonConstructor]
        protected SettingBase(DateTime time)
        {
            TimeStamp = time;
        }
    }
}
