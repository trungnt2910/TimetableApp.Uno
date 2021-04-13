using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableApp.Core
{
    public static class Data
    {
        private static Timetable timetable = null;
        public static Timetable Timetable
        {
            get => (timetable != null) ? timetable : timetable = Timetable.Load();
        }
    }
}
