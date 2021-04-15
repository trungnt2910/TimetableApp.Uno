using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableApp.Core
{
    public class Setting<T> : SettingBase
    {
        private T _value;
        [JsonProperty]
        public T Value
        {
            get => _value;
            set
            {
                if ((value == null) || (typeof(T).IsAssignableFrom(value.GetType())))
                {
                    _value = value;
                    ValueChanged?.Invoke(this, null);
                }
                else
                {
                    throw new InvalidOperationException($"Invalid value of type {value.GetType()} assigned to Setting<{typeof(T)}>");
                }
            }
        }

        public event EventHandler ValueChanged;

        public Setting()
        {
        }

        [JsonConstructor]
        private Setting(T value, DateTime timeStamp) : base(timeStamp)
        {
            _value = value;
        }
    }
}
